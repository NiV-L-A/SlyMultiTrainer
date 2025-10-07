using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.Layered;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SlyMultiTrainer
{
    [DebuggerDisplay("{Version} | Tasks count: {Tasks.Count}")]
    public class DAG_t
    {
        public List<Task_t> Tasks;
        public List<Cluster_t> Clusters;
        public GViewer Viewer;
        public Graph Graph;
        public DAG_VERSION Version;

        public string RootNodePointer = "";
        public string CurrentCheckpointNodePointer = "";
        public string TaskStringTablePointer = "";
        public string SavefileStartAddress = "";
        public string ClusterIdAddress = "";
        public string SavefileValuesOffsetsTablePointer = "";
        public string Sly3Time = "";
        public string Sly3Flag = "";
        public string OffsetId = "";
        public string OffsetNextNodePointer = "";
        public string OffsetState = "";
        public string OffsetFocusCount1 = "";
        public string OffsetFocusCount2 = "";
        public string OffsetGoalDescription = ""; // FOR CLUSTER: "Locate The Job Start Point" (for satellite sabotage)
        public string OffsetMissionName = ""; // FOR CLUSTER: "Satellite Sabotage"
        public string OffsetMissionDescription = ""; // FOR CLUSTER (we don't use it for now)
        //public string OffsetIsChalktalk = ""; // Not found yet
        public string OffsetClusterPointer = ""; // Pointer to the cluster node
        public string OffsetChildrenCount = ""; // Count of how many children the task has. The pointer to the array of node pointers is at +4
        public string OffsetSuckPointer = ""; // FOR CLUSTER: pointer to the suck value of the job
        public string OffsetCheckpointEntranceValue = ""; // Entrance value of the checkpoint
        public string OffsetAttributes = "";
        public string OffsetAttributesForCluster = "";
        public int AttributeCountForTask = 0;

        public Dictionary<short, string> TaskStringTable;
        public Dictionary<int, int> SavefileValuesTable; // Id + SubId combined, absolute address
        public Func<int, string> GetStringFromId;
        public Action<int, int, int> LoadMap;
        public Action<int, int> WriteActCharId;

        public int NodeDefaultLineWidth = 1;
        public int SelectedNodeDefaultLineWidth = 5;
        public int NodeCurrentCheckpointDefaultLineWidth = 3;
        public int SelectedClusterDefaultLineWidth = 10;

        private Memory.Mem _m;
        private Node? _selectedNode; // For highlighting
        private bool _showAddress = false;
        private bool _showId = false;
        private bool _showName = true;
        private bool _showIdAsDecimal = false;
        // Store the current zoom and pan before refreshing
        private bool _lockPanAndZoomOnRefresh = false;
        private Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation _oldTransform;
        public DAG_t(Memory.Mem m)
        {
            Viewer = new();
            Viewer.BackColor = System.Drawing.Color.White;
            Viewer.PanButtonPressed = true;
            Viewer.ToolBarIsVisible = false;
            Viewer.Dock = DockStyle.Fill;
            Viewer.OutsideAreaBrush = Brushes.White;
            Viewer.MouseClick += DAGViewer_MouseClick;
            Tasks = new();
            Clusters = new();
            _m = m;
        }

        public void Init()
        {
            Tasks.Clear();
            Clusters.Clear();
            TaskStringTable = GetTaskStringTable();
            SavefileValuesTable = GetSavefileValuesTable();
        }

        public void SetVersion(DAG_VERSION version)
        {
            Version = version;
            if (Version == DAG_VERSION.V0)
            {
                AttributeCountForTask = 3;
            }
            else
            {
                AttributeCountForTask = 5;
            }
        }

        public void GetDAG()
        {
            ReadTask(RootNodePointer);

            // We make sure that we parsed every node by checking the nextNode field instead of a node's children.
            // This way we fix sly 2 ep3, in which there's a floating node not attached to the dag
            for (int i = 0; i < Tasks.Count; i++)
            {
                string nextNodeAddress = _m.ReadInt($"{Tasks[i].Address}+{OffsetNextNodePointer}").ToString("X");
                
                // The nextNode field is 0 when it's the last task ("t1_complete")
                if (nextNodeAddress != "0")
                {
                    bool found = false;
                    for (int j = 0; j < Tasks.Count; j++)
                    {
                        if (Tasks[j].Address == nextNodeAddress)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        // not parsed
                        ReadTask($"{Tasks[i].Address}+{OffsetNextNodePointer}");
                    }
                }
            }

            // Remove from SavefileValuesTable everything except the keys used by the tasks and clusters
            if (Version != DAG_VERSION.V0)
            {
                List<int> keysToKeep = new();
                for (int i = 0; i < Tasks.Count; i++)
                {
                    for (int j = 0; j < Tasks[i].Attributes.Count; j++)
                    {
                        int combined = GetIdSubIdCombined(Tasks[i].Attributes[j]);
                        keysToKeep.Add(combined);
                    }
                }

                for (int i = 0; i < Clusters.Count; i++)
                {
                    for (int j = 0; j < Clusters[i].Attributes.Count; j++)
                    {
                        int combined = GetIdSubIdCombined(Clusters[i].Attributes[j]);
                        keysToKeep.Add(combined);
                    }
                }

                var keysToRemove = SavefileValuesTable.Keys.Where(k => !keysToKeep.Contains(k)).ToList();
                foreach (var key in keysToRemove)
                {
                    SavefileValuesTable.Remove(key);
                }
            }
        }

        public Task_t ReadTask(string taskPointer)
        {
            string taskAddress = _m.ReadInt(taskPointer).ToString("X");
            Task_t task = ReadTask(taskAddress, true);
            return task;
        }

        public Task_t ReadTask(string taskAddress, bool contributeToDag, Task_t parentTask = null)
        {
            Task_t task = new();
            if (taskAddress == "0")
            {
                // main menu
                return task;
            }

            task.Id = _m.ReadInt($"{taskAddress}+{OffsetId}");
            if (parentTask != null)
            {
                task.Parent.Add(parentTask);
            }

            // Check if we already parsed the task
            if (contributeToDag)
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    if (Tasks[i].Id == task.Id)
                    {
                        // It was already parsed
                        // This means that it has more than 1 parent
                        if (parentTask != null)
                        {
                            Tasks[i].Parent.Add(parentTask);
                        }
                        return Tasks[i];
                    }
                }
            }

            task.Address = $"{taskAddress:X}";
            task.State = (STATE)_m.ReadInt($"{taskAddress}+{OffsetState}");
            task.FocusCount1 = _m.ReadInt($"{taskAddress}+{OffsetFocusCount1}");
            task.FocusCount2 = _m.ReadInt($"{taskAddress}+{OffsetFocusCount2}");
            task.CheckpointEntranceValue = _m.ReadInt($"{taskAddress}+{OffsetCheckpointEntranceValue}");
            if (task.CheckpointEntranceValue != -1)
            {
                task.MapId = _m.ReadInt($"{taskAddress}+{OffsetCheckpointEntranceValue}+4");
                task.EntityId = _m.ReadInt($"{taskAddress}+{OffsetCheckpointEntranceValue}+8");
                if (Version >= DAG_VERSION.V2)
                {
                    task.EntityId2 = _m.ReadInt($"{taskAddress}+{OffsetCheckpointEntranceValue}+C");
                }
            }

            if (contributeToDag)
            {
                for (int i = 0; i < AttributeCountForTask; i++)
                {
                    Attribute_t attribute = new();
                    attribute.Id = _m.ReadShort($"{taskAddress}+{OffsetAttributes}+{8 * i + 4:X},0");
                    attribute.SubId = _m.ReadShort($"{taskAddress}+{OffsetAttributes}+{8 * i + 4:X},2");
                    task.Attributes.Add(attribute);
                }

                // Name
                task.Name = TaskStringTable.GetValueOrDefault(task.Attributes[0].Id, "");

                if (Regex.IsMatch(task.Name, @"^t\d_chalktalk\d$"))
                {
                    task.IsChalktalk = true;
                }

                // Description
                int descriptionStringId = _m.ReadInt($"{taskAddress}+{OffsetGoalDescription}");
                task.GoalDescription = GetStringFromId(descriptionStringId);

                // Set the text to be displayed
                task.SetText(_showAddress, _showId, _showName, _showIdAsDecimal);

                string clusterAddress = _m.ReadInt($"{taskAddress}+{OffsetClusterPointer}").ToString("X");
                if (clusterAddress == "0")
                {
                    // It's not part of a cluster, but we must create it anyway,
                    // so we create a dummy cluster so that the graph flows correctly
                    // (the node becomes "floating" if we don't have it inside a cluster)
                    Cluster_t cluster = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0), "0");
                    task.Cluster = cluster;
                    cluster.Tasks.Add(task);
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < Tasks.Count; i++)
                    {
                        if (Tasks[i].Cluster.Address == clusterAddress)
                        {
                            // Cluster already added, just add the task to it
                            task.Cluster = Tasks[i].Cluster;
                            task.Cluster.Tasks.Add(task);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        // This is a new cluster, so let's create it
                        Cluster_t cluster = ReadCluster(clusterAddress, true);
                        task.Cluster = cluster;
                        cluster.Tasks.Add(task);
                        Clusters.Add(cluster);
                    }
                }

                Tasks.Add(task);

                // Add children (if any)
                int childrenCount = _m.ReadInt($"{taskAddress}+{OffsetChildrenCount}");
                int childrenPointer = _m.ReadInt($"{taskAddress}+{OffsetChildrenCount}+4");
                if (childrenCount != 0)
                {
                    // We go backwards. It creates the same graph as shown in Bruce's presentation this way
                    for (int i = childrenCount - 1; i >= 0; i--)
                    {
                        string childAddress = _m.ReadInt($"{childrenPointer + i * 4:X}").ToString("X");
                        Task_t child = ReadTask(childAddress, true, task);
                        task.Children.Add(child);
                    }
                }
            }

            return task;
        }

        public Cluster_t ReadCluster(string clusterAddress, bool contributeToDag)
        {
            Cluster_t cluster = new(_m.ReadInt($"{clusterAddress}+{OffsetId}"), clusterAddress);

            if (contributeToDag)
            {
                for (int i = 0; i < AttributeCountForTask - 1; i++)
                {
                    Attribute_t attr = new();
                    attr.Id = _m.ReadShort($"{clusterAddress}+{OffsetAttributesForCluster}+{8 * i + 4:X},0");
                    attr.SubId = _m.ReadShort($"{clusterAddress}+{OffsetAttributesForCluster}+{8 * i + 4:X},2");
                    cluster.Attributes.Add(attr);
                }

                // Get the mission name (if any)
                cluster.Name = TaskStringTable.GetValueOrDefault(cluster.Attributes[0].Id, "");

                if (Version != DAG_VERSION.V2)
                {
                    short missionNameId = _m.ReadShort($"{clusterAddress}+{OffsetMissionName}");
                    cluster.Description = GetStringFromId(missionNameId);
                }

                cluster.SetText(_showAddress, _showId, _showName, _showIdAsDecimal);
            }

            float suckValue = _m.ReadFloat($"{clusterAddress}+{OffsetSuckPointer},0");
            cluster.Suck = suckValue;

            return cluster;
        }

        public bool IsTaskEqualToTask(Task_t task1, Task_t task2)
        {
            if (task1.State != task2.State
                || task1.FocusCount1 != task2.FocusCount1
                || task1.FocusCount2 != task2.FocusCount2)
            {
                return false;
            }

            return true;
        }

        public bool IsClusterEqualToCluster(Cluster_t cluster1, Cluster_t cluster2)
        {
            if (cluster1.Suck != cluster2.Suck)
            {
                return false;
            }

            return true;
        }

        public Dictionary<int, int> GetSavefileValuesTable()
        {
            Dictionary<int, int> table = new();
            if (Version == DAG_VERSION.V0)
            {
                return table;
            }

            int tableStart = _m.ReadInt($"{SavefileValuesOffsetsTablePointer}");
            short nextIdOffset = 1;
            do
            {
                int current = tableStart + nextIdOffset * 0xA;
                short id = _m.ReadShort($"{current:X}");
                short childOffset = _m.ReadShort($"{current + 6:X}");
                nextIdOffset = _m.ReadShort($"{current + 8:X}");
                while (childOffset != 0)
                {
                    current = tableStart + childOffset * 0xA;

                    // NOTE: Instead of adding the offset to the dictionary,
                    // we already convert it to an absolute address by adding the offset to SavefileStartAddress
                    // In sly 2 ntsc, this sum is done at 1D814C, where s3 is 3D4A60 and v0 is the offset (16A0 for t1_follow_intro)
                    short subId = _m.ReadShort($"{current:X}");
                    int combined = GetIdSubIdCombined(id, subId);
                    short savefileValueOffset = _m.ReadShort($"{current + 4:X}");
                    int address = Convert.ToInt32(SavefileStartAddress, 16) + savefileValueOffset;
                    table.TryAdd(combined, address);

                    childOffset = _m.ReadShort($"{current + 8:X}");
                }
            } while (nextIdOffset != 0);

            return table;
        }

        public Dictionary<short, string> GetTaskStringTable()
        {
            int i = 0;
            short stringId = 0;
            Dictionary<short, string> table = new();

            while (stringId != 0x100)
            {
                stringId = _m.ReadShort($"{TaskStringTablePointer},{i * 0x10 + 8:X}");
                int stringPointer = _m.ReadInt($"{TaskStringTablePointer},{i * 0x10 + 4:X}");
                string taskName = _m.ReadNullTerminatedString(stringPointer.ToString("X"));
                table.TryAdd(stringId, taskName);
                i++;
            }

            return table;
        }

        public string GetCurrentCheckpointAddress()
        {
            var tmp = _m.ReadInt(CurrentCheckpointNodePointer).ToString("X");
            return tmp;
        }

        public void WriteCurrentCheckpointAddress(Task_t? task)
        {
            string value = "0";
            if (task != null)
            {
                value = $"0x{task.Address}";
            }

            _m.WriteMemory($"{CurrentCheckpointNodePointer}", "int", value);
        }

        public void WriteClusterSuck(Cluster_t cluster)
        {
            _m.WriteMemory($"{cluster.Address}+{OffsetSuckPointer},0", "float", cluster.Suck.ToString());
        }

        public STATE ReadTaskState(Task_t task)
        {
            STATE state = (STATE)_m.ReadInt($"{task.Address}+{OffsetState}");
            return state;
        }

        public void WriteTaskState(Task_t task)
        {
            _m.WriteMemory($"{task.Address}+{OffsetState}", "int", ((int)task.State).ToString());
        }

        public void WriteTaskFocusCount(Task_t task)
        {
            int[] tmp =
            {
                task.FocusCount1,
                task.FocusCount2,
            };

            byte[] byteArray = Util.IntArrayToByteArray(tmp);
            _m.WriteBytes($"{task.Address}+{OffsetFocusCount1}", byteArray);
        }

        public void WriteTaskFocusCount1(Task_t task)
        {
            _m.WriteMemory($"{task.Address}+{OffsetFocusCount1}", "int", task.FocusCount1.ToString());
        }

        public void WriteTaskFocusCount2(Task_t task)
        {
            _m.WriteMemory($"{task.Address}+{OffsetFocusCount2}", "int", task.FocusCount2.ToString());
        }

        private void LoadCheckpoint(Task_t targetTask, bool withZeroFocus)
        {
            // https://www.youtube.com/watch?v=Yl20uIQ3fEw
            // Current implementation has some tiny differences from what Bruce described

            // Not all jobs have a checkpoint as the first task (sly 2 breaking and entering)
            // If we use the function "load job" we can't use the first task as it's not a checkpoint (it doesn't hold map id and entity id data)
            // We manually go look for the first available checkpoint in the job
            while (targetTask.CheckpointEntranceValue == -1)
            {
                targetTask = targetTask.Children[0];
            }

            // The "State" field (along with other fields like the focus count) for a task is acquired from the savefile region, which is always in the same place at all times
            // We write to the savefile region directly and let the game create new instances of the tasks with our parameters.

            // Each task has a certain number of "attributes"
            // For version 0, tasks have 3 attributes and clusters have 2
            // For all other versions, tasks have 5 attributes and clusters have 4
            // Each attribute represents a value stored in the savefile region.
            // Each attribute has an id (the same for all 5 attributes) and a subId.
            // The id is used to get the internal task name (t1_follow_intro) and the internal mission name (m1_follow_dimitri)
            // The subId is a code to indicate which field it represents
            // For tasks in sly 2 ntsc,
            //  0x6F is FocusCount1,
            //  0xC1 is FocusCount2,
            //  0xC2 is State,
            //  Rest is unknown
            // For clusters in sly 2 ntsc,
            //  0x6F is "attempts" (how many reloads there have been since the start of the job),
            //  0x70 is if it has been completed,
            //  0x71 is playtime (maybe?),
            //  0x72 is the suck value
            // The subId codes are different between builds, so that's why an enum was not implemented.

            // When we loaded the dag, we have populated SavefileValuesTable.
            // By combining each attribute's id and subId, we get an offset from the beginning of the savefile region (in SavefileValuesTable it has already been converted to an absolute address)
            // For example, the task t1_follow_intro has these 5 attributes (id+subId pairs):
            // 0xF8, 0x6F
            // 0xF8, 0xC1
            // 0xF8, 0x70
            // 0xF8, 0xC2
            // 0xF8, 0xC3
            // We combine each pair to form an int:
            // 0x006F00F8
            // 0x00C100F8
            // 0x007000F8
            // 0x00C200F8
            // 0x00C300F8
            // We get from the SavefileValuesTable dictionary the absolute addresses:
            // 0x003D6100
            // 0x003D6104
            // 0x003D6108
            // 0x003D610C
            // 0x003D6110
            // So, for t1_follow_intro:
            // 0xF8, 0x6F = 0x006F00F8 = 0x003D6100 = FocusCount1
            // 0xF8, 0xC1 = 0x00C100F8 = 0x003D6104 = FocusCount2
            // 0xF8, 0x70 = 0x007000F8 = 0x003D6108 = ?
            // 0xF8, 0xC2 = 0x00C200F8 = 0x003D610C = State
            // 0xF8, 0xC3 = 0x00C300F8 = 0x003D6110 = ?
            // NOTE: As mentioned in a comment above, in memory there's a table of id+subId pairs that have an offset associated with them.
            // The offset is added to "SavefileStartAddress" (in sly 2 ntsc, at 3D4A60).
            // To get to 0x003D6100 we added 16A0 to 3D4A60. 16A0 is at 004B817E, at -4 from that (4B817A) we have 0x6F, which is the subId of the first attribute listed in the t1_follow_intro task's struct, and at -A from that (4B8170) we have 0xF8, which is the id of the first attribute (and all the other attributes for this task) listed in the t1_follow_intro task's struct

            // The goal is to write to these absolute addresses the correct values.
            // Do note that the 5 addresses we ended up with are all +4 from each other. We can use this knowledge to combine the 5 memory writes into 1. We will optimize this even further and combine the multiple memory writes for each task into 1 since these 0x14 groups are all one after another (but not ordered, we will order them ourselves)
            // NOTE: "are all one after another" - this is not true for sly 3 ep3, 4 and 5. We will detect later when there is a break between the flags and do multiple memory writes

            // https://youtu.be/Yl20uIQ3fEw?t=1845 (30:45)
            // BRUCE:
            // - Start with a task and a desired state.
            // - Recursively walk backwards through predecessors
            //   - Stop when predecessor is already in desired state
            //   - Mark predecessor with desired state
            //   - Skip predecessors already marked

            // - We start from "targetTask" and the desired state is "Available". "targetTask" is either the first node of a cluster or a specific checkpoint inside the cluster.
            // - We mark targetTask to be "Available" and we start walking backwards up until the root (node with no parents).
            // - This goes against the "Stop when predecessor is already in desired state" point. Is Bruce assuming the dag is *always* in a valid state?
            // - While we walk backwards we mark each node to be final (if we want to set targetTask to be available, it means everything before that must be final)
            // - NOTE: If a specific checkpoint was loaded, the tasks that come before targetTask AND that are part of the same cluster need to be marked as complete instead of final. We check for this later. For now, let's just mark all parents to be final
            // IMPORTANT: We manually go look for the root node. This fixes sly2 ep3 having more than 1 node with no parents (Sly 2 march ep3 has even more, 6 nodes with no parents)
            MarkTaskSavefileChangeBackward(targetTask, STATE.Available, out Task_t rootTask);

            // https://youtu.be/Yl20uIQ3fEw?t=1935 (32:15)
            // BRUCE:
            // - Go through all tasks in "predecessors first" order
            //   - Set each task to its desired state
            //   - Recursively walk forward through successors
            //   - Skip tasks marked with a state

            // - We start from the rootTask (top of the dag) and we mark the childrens, but we "skip" the ones already marked (the word "skip" here means we don't mark them AGAIN, but we still need to consider their children, which means every node in the dag)
            // - Childrens of final have to be available
            // - Childrens of available have to be unavailable
            // - Childrens of unavailable have to be unavailable
            // - Childrens of complete have to be available
            MarkTaskSavefileChangeForward();

            // At this point all nodes in the dag have been marked with a desired state
            // Let's now go through each task and get the array of 0x14 bytes that is going to be written to the savefile region
            List<(int Address, byte[] Values)> SavefileValues = new(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                Task_t task = Tasks[i];
                if (targetTask.Cluster == task.Cluster
                    && task.StateSavefileChange == STATE.Final)
                {
                    // If we are loading a checkpoint which is not the first task of the cluster
                    // Then we need to set to complete the previous tasks, but only in this cluster
                    task.StateSavefileChange = STATE.Complete;
                }

                var addressValuesPair = TaskSetSavefileValue(task, withZeroFocus);
                SavefileValues.Add(addressValuesPair);
                task.MarkSavefileChange = false;

                if (Version >= DAG_VERSION.V2)
                {
                    // In sly 3 the focus in the task structs are copied to the ones in the savefile
                    // So we also need to write the focus count to each task struct
                    // Unfortunately these memory writes will cost us some time
                    if (task.StateSavefileChange == STATE.Final
                        || task.StateSavefileChange == STATE.Complete)
                    {
                        task.FocusCount1 = 1;
                        task.FocusCount2 = 1;
                    }
                    else if (task.StateSavefileChange == STATE.Available && !withZeroFocus)
                    {
                        task.FocusCount1 = 1;
                        task.FocusCount2 = 1;
                    }
                    else
                    {
                        task.FocusCount1 = 0;
                        task.FocusCount2 = 0;
                    }

                    // Optimize the write to the 2 focus counts. This saves around 50ms overall. The difference is noticeable.
                    WriteTaskFocusCount(task);
                }
            }

            // Commit the desired state to the savefile
            List<(int Address, byte[] Values)> SavefileValuesToWrite = SavefileValues.Order().ToList();
            int toSkip = 0;
            int toTake = SavefileValuesToWrite.Count;
            if (Version >= DAG_VERSION.V2)
            {
                // The optimization only works for sly 3 in ep1 and 2. For ep3, 4 and 5, the sequence breaks.
                // For example, in ep3 day2, the 0x14 values at 46BAB8 come from the offset 0x2D88, which comes from the Id 0x29C and SubId 0x56. But the task with that attribute is from ep5
                // So, we detect when the next array of 0x14 bytes is not at +0x14 of the current address and we break up the values to write into pieces
                toTake = 0;
                for (int i = 0; i < SavefileValuesToWrite.Count; i++)
                {
                    if (i == SavefileValuesToWrite.Count - 1)
                    {
                        toTake = i + 1 - toTake;
                        break;
                    }

                    if (SavefileValuesToWrite[i + 1].Address != SavefileValuesToWrite[i].Address + 0x14)
                    {
                        // Sequence break, write then pass to the next
                        int tmp = toTake;
                        toTake = i + 1 - toTake;
                        byte[] flags = SavefileValuesToWrite
                                .Skip(toSkip)
                                .Take(toTake)
                                .Select(t => t.Values)
                                .SelectMany(b => b)
                                .ToArray();
                        _m.WriteBytes($"{SavefileValuesToWrite[toSkip].Address:X}", flags);
                        toSkip = i + 1;
                        toTake = toTake + tmp;
                    }
                }
            }

            byte[] flags2 = SavefileValuesToWrite
                            .Skip(toSkip)
                            .Take(toTake)
                            .Select(t => t.Values)
                            .SelectMany(b => b)
                            .ToArray();
            _m.WriteBytes($"{SavefileValuesToWrite[toSkip].Address:X}", flags2);

            // targetTask.Id wrote at +4 and +8, both are needed for sly 3
            int[] NodeIds =
            {
                targetTask.Cluster.Id, targetTask.Id, targetTask.Id
            };
            _m.WriteBytes($"{ClusterIdAddress}", Util.IntArrayToByteArray(NodeIds));

            // Entity
            WriteActCharId(targetTask.EntityId, targetTask.EntityId2);

            // Mode. Used for sly 3 to indicate if there should be a full reload (in case the targetTask is from a different day than the current one)
            int mode = 0;
            if (Version == DAG_VERSION.V3)
            {
                // From sly 3 sept proto at 0019adb0
                int time = _m.ReadInt($"{Sly3Time}");
                int flag = _m.ReadInt($"{Sly3Time}+F0");
                int flag2 = _m.ReadInt($"{Sly3Flag}"); // reload address - 70
                int timeTask = _m.ReadInt($"{targetTask.Address}+C0"); // 0xC = day1, else day2

                if (flag2 > 1)
                {
                    if ((timeTask == time) && (flag != 0x0))
                    {
                        timeTask = 0xc;
                    }
                    int final = 0xc;
                    if (flag == 0x0)
                    {
                        final = time;
                    }
                    if (timeTask != final)
                    {
                        mode = 2;
                    }
                }
            }

            LoadMap(targetTask.MapId, targetTask.CheckpointEntranceValue, mode);
        }

        public (int, byte[]) TaskSetSavefileValue(Task_t task, bool withZeroFocus)
        {
            int addr1 = GetSavefileAddress(task.Attributes[0]);

            int[] flags =
            {
                0,
                0,
                0,
                (int)task.StateSavefileChange,
                0,
            };

            if (task.StateSavefileChange == STATE.Final
                || task.StateSavefileChange == STATE.Complete)
            {
                // We set the focus if the desired state is final or complete (the player has been through that task)
                flags[0] = 1;
                flags[1] = 1;
            }
            else if (task.StateSavefileChange == STATE.Available)
            {
                // And for the "targetTask" (the desired state is available), we check if the user wants to load it with or without focus
                if (!withZeroFocus)
                {
                    flags[0] = 1;
                    flags[1] = 1;
                }
            }
            // The rest is unavailable, so no focus because the player has not been through that task

            byte[] tmp = Util.IntArrayToByteArray(flags);
            return (addr1, tmp);
        }

        private void MarkTaskSavefileChange(Task_t task, STATE state)
        {
            task.MarkSavefileChange = true;
            task.StateSavefileChange = state;
        }

        private void MarkTaskSavefileChangeBackward(Task_t task, STATE state, out Task_t rootTask)
        {
            rootTask = null;

            MarkTaskSavefileChange(task, state);

            if (/*task.State == state ||*/ task.Parent.Count == 0)
            {
                rootTask = task;
                return;
            }

            for (int i = 0; i < task.Parent.Count; i++)
            {
                MarkTaskSavefileChangeBackward(task.Parent[i], STATE.Final, out rootTask);
            }
        }

        private void MarkTaskSavefileChangeForward()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                Task_t task = Tasks[i];
                if (task.MarkSavefileChange)
                {
                    // Skip tasks already marked
                    continue;
                }

                if (task.Parent.Any(x => x.MarkSavefileChange && x.StateSavefileChange == STATE.Final))
                {
                    MarkTaskSavefileChange(task, STATE.Available);
                }
                else if (task.Parent.Any(x => x.MarkSavefileChange && x.StateSavefileChange == STATE.Available))
                {
                    MarkTaskSavefileChange(task, STATE.Unavailable);
                }
                else if (task.Parent.Any(x => x.MarkSavefileChange && x.StateSavefileChange == STATE.Unavailable))
                {
                    MarkTaskSavefileChange(task, STATE.Unavailable);
                }
                else if (task.Parent.Any(x => x.MarkSavefileChange && x.StateSavefileChange == STATE.Complete))
                {
                    MarkTaskSavefileChange(task, STATE.Available);
                }
            }

            // OLD IMPLEMENTATION:
            // Maybe closer to what Bruce described, but why go through a task again if we have already parsed it?
            //System.Diagnostics.Debug.WriteLine($"{task.Id} {task.Name} {task.MarkSavefileChange}");
            /*
            if (!task.MarkSavefileChange)
            {
                MarkTaskSavefileChange(task, state);
            }

            for (int i = 0; i < task.Children.Count; i++)
            {
                Task_t child = task.Children[i];
                //System.Diagnostics.Debug.WriteLine($"\t{child.Id} {child.Name} {child.MarkSavefileChange}");

                if (child.Parent.Any(x => x.StateSavefileChange == STATE.Final))
                {
                    MarkTaskSavefileChangeForward(child, STATE.Available);
                }
                else if (child.Parent.Any(x => x.StateSavefileChange == STATE.Available))
                {
                    MarkTaskSavefileChangeForward(child, STATE.Unavailable);
                }
                else if (child.Parent.Any(x => x.StateSavefileChange == STATE.Unavailable))
                {
                    MarkTaskSavefileChangeForward(child, STATE.Unavailable);
                }
                else if (child.Parent.Any(x => x.StateSavefileChange == STATE.Complete))
                {
                    MarkTaskSavefileChangeForward(child, STATE.Available);
                }
            }
            */
        }

        private int GetSavefileAddress(Attribute_t attribute)
        {
            int combined = GetIdSubIdCombined(attribute);
            SavefileValuesTable.TryGetValue(combined, out int address);
            return address;
        }

        public int GetIdSubIdCombined(short id, short subId)
        {
            int combined = (id << 16) | ((ushort)subId);
            return combined;
        }

        public int GetIdSubIdCombined(Attribute_t attribute)
        {
            return GetIdSubIdCombined(attribute.Id, attribute.SubId);
        }

        private void SetSavefileValue(int address, byte[] value)
        {
            _m.WriteBytes(address.ToString("X"), value);
        }

        //public void ClusterSetSavefileValue(Cluster_t cluster)
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        ResetDagValue(cluster.Attributes[i].Id, (short)cluster.Attributes[i].SubId, 0);
        //    }

        //    // Tasks in the cluster
        //    for (int i = 0; i < cluster.Tasks.Count; i++)
        //    {
        //        for (int j = 0; j < 5; j++)
        //        {
        //            if (i == 0 && cluster.Tasks[i].Attributes[j].SubId == ATTRIBUTE_SUBID.State)
        //            {
        //                ResetDagValue(cluster.Tasks[i].Attributes[j].Id, (short)cluster.Tasks[i].Attributes[j].SubId, (int)STATE.Available);
        //            }
        //            else
        //            {
        //                ResetDagValue(cluster.Tasks[i].Attributes[j].Id, (short)cluster.Tasks[i].Attributes[j].SubId, 0);
        //            }
        //        }
        //    }
        //}

        /*
        Microsoft.Msagl.Core.Geometry.Curves.ICurve GetNodeBoundary(Microsoft.Msagl.Drawing.Node node)
        {
            using (Graphics g = Viewer.CreateGraphics())
            {
                var font = new Font("GenericMonospace", 12);
                var sizeF = g.MeasureString(node.LabelText, font);
                
                double scale = 1.0 / 1.33; // Scale pixels to points
                double width = node.Label.Width * scale;
                double height = node.Label.Height * scale;

                var center = new Microsoft.Msagl.Core.Geometry.Point(0, 0);
                //node.GeometryNode.BoundaryCurve = Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateRectangle(width, height, center);
                if (node.Attr.Shape == Shape.Ellipse)
                {
                    return Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateEllipse(width, height, center);
                }
                else if (node.Attr.Shape == Shape.Diamond)
                {
                    return Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateDiamond(width * 1.2, height * 1.5, center);
                }
                else
                {
                    return Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateOctagon(width * 1.5, height * 1.5, center);
                }
            }
        }

        bool DrawNode(Node node, object graphics)
        {
            var font = new System.Drawing.Font(node.Label.FontName, (float)node.Label.FontSize);
            StringMeasure.MeasureWithFont(node.LabelText, font, out var width, out var height);
            node.Label.Width = width * 1000;
            node.Label.Height = height;
            return true;

            node.Label.GeometryLabel = new Microsoft.Msagl.Core.Layout.Label();
            node.Label.GeometryLabel.Side = Microsoft.Msagl.Core.Layout.Label.PlacementSide.Top;

            node.Label.GeometryLabel.Width = width;
            node.Label.GeometryLabel.Height = height;

            node.Label.Width = width;
            node.Label.Height = height;

            var r = node.Attr.LabelMargin;

            node.GeometryNode = new(Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateRectangleWithRoundedCorners(height + r * 2, height + r * 2, r, r, new Microsoft.Msagl.Core.Geometry.Point(0, 0)));

            return true;//returning false would enable the default rendering
        }
        */

        public void SetGraph()
        {
            if (_lockPanAndZoomOnRefresh)
            {
                _oldTransform = Viewer.Transform;
            }
            
            Graph = new();
            var settings = new SugiyamaLayoutSettings
            {
                SnapToGridByY = SnapToGridByY.Top // Make nodes and clusters start at the same layer
            };
            Graph.LayoutAlgorithmSettings = settings;

            var visited = new HashSet<int>();
            void Trav(Task_t task)
            {
                if (!visited.Add(task.Id))
                {
                    return;
                }
                
                Node node = new(task.Id.ToString("X"));
                node.Attr.FillColor = GetNodeColorFromState(task.State);
                node.LabelText = task.Text;
                node.UserData = task;
                node.Attr.LineWidth = NodeDefaultLineWidth;
                //node.DrawNodeDelegate = new DelegateToOverrideNodeRendering(DrawNode);
                //node.NodeBoundaryDelegate = new DelegateToSetNodeBoundary(GetNodeBoundary);

                if (task.CheckpointEntranceValue == -1)
                {
                    if (task.IsChalktalk)
                    {
                        node.Attr.Shape = Shape.Octagon;
                    }
                    else
                    {
                        node.Attr.Shape = Shape.Ellipse;
                    }
                }
                else
                {
                    node.Attr.Shape = Shape.Diamond;
                }

                task.MsaglNode = node;
                if (task.Cluster.Address == "0")
                {
                    // Create a dummy cluster. This is to make everything flow from top to bottom
                    Subgraph cluster = new(task.Cluster.Id.ToString("X"));
                    cluster.UserData = task.Cluster;
                    cluster.IsVisible = false;
                    task.Cluster.Subgraph = cluster;
                    cluster.AddNode(node);
                    Graph.RootSubgraph.AddSubgraph(cluster);
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < Graph.RootSubgraph.Subgraphs.Count(); i++)
                    {
                        Cluster_t cluster = (Cluster_t)Graph.RootSubgraph.Subgraphs.ElementAt(i).UserData;
                        if (cluster == task.Cluster)
                        {
                            // Cluster already present, just add the node to the cluster
                            found = true;
                            Graph.RootSubgraph.Subgraphs.ElementAt(i).AddNode(node);
                            break;
                        }
                    }

                    // Cluster was not found, let's add the cluster and this node
                    if (!found)
                    {
                        Subgraph cluster = new(task.Cluster.Id.ToString("X"));
                        cluster.LabelText = task.Cluster.Text;
                        cluster.Attr.FillColor = GetClusterColorFromState(task.State);
                        cluster.UserData = task.Cluster;
                        task.Cluster.Subgraph = cluster;
                        cluster.AddNode(node);
                        Graph.RootSubgraph.AddSubgraph(cluster);
                    }
                }

                Graph.AddNode(node);
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                Trav(Tasks[i]);

                foreach (var child in Tasks[i].Children)
                {
                    Trav(child);
                    Graph.AddEdge(Tasks[i].Id.ToString("X"), child.Id.ToString("X"));
                }
            }

            // It is possible that msagl fails to set the graph
            // It's rare, but it seems to happen if the window is minimized, the game is loading a map and a save state is being loaded
            try
            {
                Viewer.Graph = Graph;
            }
            catch (Exception ex)
            {

            }

            if (_lockPanAndZoomOnRefresh)
            {
                Viewer.Transform = _oldTransform;
            }
        }

        public Microsoft.Msagl.Drawing.Color GetClusterColorFromState(STATE nodeState)
        {
            switch (nodeState)
            {
                case STATE.Unavailable:
                    return Microsoft.Msagl.Drawing.Color.LightPink;
                case STATE.Available:
                    return Microsoft.Msagl.Drawing.Color.LightGreen;
                case STATE.Complete:
                    return Microsoft.Msagl.Drawing.Color.LightGreen;
                case STATE.Final:
                    return Microsoft.Msagl.Drawing.Color.LightGray;
                default:
                    return Microsoft.Msagl.Drawing.Color.White;
            }
        }

        public Microsoft.Msagl.Drawing.Color GetNodeColorFromState(STATE nodeState)
        {
            switch (nodeState)
            {
                case STATE.Unavailable:
                    return Microsoft.Msagl.Drawing.Color.Red;
                case STATE.Available:
                    return Microsoft.Msagl.Drawing.Color.Lime;
                case STATE.Complete:
                    return Microsoft.Msagl.Drawing.Color.DodgerBlue;
                case STATE.Final:
                    return Microsoft.Msagl.Drawing.Color.LightGray;
                default:
                    return Microsoft.Msagl.Drawing.Color.White;
            }
        }

        public bool IsNodeSelected(Node node)
        {
            bool tmp = _selectedNode == node;
            return tmp;
        }

        private void RemoveLastSelectedNode()
        {
            if (_selectedNode != null)
            {
                if (_selectedNode.Attr.Styles.Contains(Style.Dashed))
                {
                    _selectedNode.Attr.LineWidth = NodeCurrentCheckpointDefaultLineWidth;
                }
                else
                {
                    _selectedNode.Attr.LineWidth = NodeDefaultLineWidth;
                }
                _selectedNode = null;
            }
        }

        public void SelectNode(Node node, double lineWidth)
        {
            RemoveLastSelectedNode();
            node.Attr.LineWidth = lineWidth;
            _selectedNode = node;
            Viewer.Invalidate();
        }

        private void ResizeNodesAndClustersToFitLabels()
        {
            // We can set the node.LabelText property and then call ResizeNodeToLabel to resize each node to fit the new label size
            // But there is no similar method for clusters to fit them to the new node size
            // Which would also mean extend or shrink the edges
            // Let's abandon the real-time change for now
            // for (int i = 0; i < Tasks.Count; i++)
            // {
            //     Task_t task = Tasks[i];
            //     task.SetText(_showAddress, _showId, _showName, _showIdAsDecimal);
            //     task.MsaglNode.LabelText = task.Text;
            //     Viewer.ResizeNodeToLabel(task.MsaglNode);
            // }
            // for (int i = 0; i < Clusters.Count; i++)
            // {
            //     Cluster_t cluster = Clusters[i];
            //     cluster.SetText(_showAddress, _showId, _showName, _showIdAsDecimal);
            //     cluster.Subgraph.LabelText = cluster.Text;
            //     // Some method here to resize the cluster based on the new nodes size?
            // }
        }

        private string ShowInputBox(string title, string textBoxLabel, string initialTextBoxValue)
        {
            Form inputForm = new();
            inputForm.Text = title;
            inputForm.Width = 320;
            inputForm.Height = 160;
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.StartPosition = FormStartPosition.CenterScreen;
            inputForm.MinimizeBox = false;
            inputForm.MaximizeBox = false;
            inputForm.ShowInTaskbar = false;
            inputForm.AcceptButton = null;
            inputForm.CancelButton = null;

            System.Windows.Forms.Label textLabel = new() { Left = 10, Top = 10, Text = textBoxLabel, AutoSize = true };
            TextBox textBox = new() { Left = 10, Top = 35, Width = 280, Text = initialTextBoxValue };

            Button okButton = new() { Text = "OK", Left = 140, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            Button cancelButton = new() { Text = "Cancel", Left = 220, Width = 70, Top = 70, DialogResult = DialogResult.Cancel };

            inputForm.Controls.Add(textLabel);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            DialogResult result = inputForm.ShowDialog();
            return result == DialogResult.OK ? textBox.Text : null;
        }

        private void SaveGraphAs(string extension)
        {
            SaveFileDialog sfd = new();
            sfd.Filter = $"Images|*.{extension}";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (extension == "png")
            {
                Bitmap image = GetImageFromGraph();
                image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                image.Dispose();
            }

            MessageBox.Show($"File saved to:\n\"{sfd.FileName}\"", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Bitmap GetImageFromGraph()
        {
            GraphRenderer renderer = new(Graph);
            renderer.CalculateLayout();
            Bitmap image = new((int)Graph.Width, (int)Graph.Height);
            renderer.Render(image);
            return image;
        }

        private void DAGViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (Viewer.Graph == null)
                {
                    // Prevent crash when right clicking before the graph is loaded (pcsx2 just launched and in splash screen)
                    return;
                }

                DNode? dNode = Viewer.GetObjectAt(e.Location) as DNode;
                if (dNode != null)
                {
                    // Node node = (Node)dNode.DrawingObject
                    Node node = dNode.Node;
                    if (node.UserData is Task_t)
                    {
                        SelectNode(node, SelectedNodeDefaultLineWidth);
                        ShowNodeContextMenu(node, e.Location);
                    }
                    else if (node.UserData is Cluster_t)
                    {
                        SelectNode(node, SelectedClusterDefaultLineWidth);
                        ShowClusterContextMenu(node, e.Location);
                    }
                }
                else
                {
                    RemoveLastSelectedNode();
                    Viewer.Invalidate();
                    ShowGraphContextMenu(e.Location);
                }
            }
        }

        private void ShowGraphContextMenu(Point location)
        {
            var items = new ToolStripItem[]
            {
                new ToolStripMenuItem($"Graph ({Tasks.Count} tasks)")
                {
                    Font = new Font(SystemFonts.MenuFont, System.Drawing.FontStyle.Bold)
                },

                new ToolStripSeparator(),

                new ToolStripMenuItem("Go to root", null, (s, e) =>
                {
                    if (Tasks.Count == 0)
                    {
                        return;
                    }

                    var rootNode = Tasks[0].MsaglNode;
                    // var nodeWidth = rootNode.GeometryNode.BoundingBox.Width;
                    // var viewerWidth = Viewer.ClientSize.Width;
                    // double zoomX = viewerWidth / nodeWidth;
                    //Viewer.CenterToPoint(rootNode.GeometryNode.Center);
                    //Viewer.Refresh();
                    //Viewer.ResumeLayout();
                    //Viewer.ZoomF = zoomX;
                    
                    // var q = rootNode.GeometryNode.BoundingBox;
                    // q = rootNode.GeometryObject.BoundingBox;
                    // q = rootNode.BoundingBox;
                    // var b = rootNode.BoundingBox;
                    // Viewer.ShowBBox(b);

                    if (rootNode != null)
                    {
                        var b = rootNode.BoundingBox;
                        Viewer.ShowBBox(b);
                    }
                }),
                new ToolStripMenuItem("Restore pan and zoom", null, (s, e) =>
                {
                    Viewer.Transform = null;
                    Viewer.Invalidate();
                }),

                new ToolStripMenuItem("Settings (requires refresh)", null, new ToolStripItem[]
                {
                    new ToolStripMenuItem("Lock pan and zoom on refresh", null, (s, e) =>
                    {
                        _lockPanAndZoomOnRefresh = !(s as ToolStripMenuItem)!.Checked;
                    })
                    {
                        Checked = _lockPanAndZoomOnRefresh
                    },
                    new ToolStripMenuItem("Show name", null, (s, e) =>
                    {
                        _showName = !(s as ToolStripMenuItem)!.Checked;
                        ResizeNodesAndClustersToFitLabels();
                    })
                    {
                        Checked = _showName
                    },
                    new ToolStripMenuItem("Show address", null, (s, e) =>
                    {
                        _showAddress = !(s as ToolStripMenuItem)!.Checked;
                        ResizeNodesAndClustersToFitLabels();
                    })
                    {
                        Checked = _showAddress
                    },
                    new ToolStripMenuItem("Show id", null, (s, e) =>
                    {
                        _showId = !(s as ToolStripMenuItem)!.Checked;
                        ResizeNodesAndClustersToFitLabels();
                    })
                    {
                        Checked = _showId
                    },
                    new ToolStripMenuItem("Show id as decimal", null, (s, e) =>
                    {
                        _showIdAsDecimal = !(s as ToolStripMenuItem)!.Checked;
                        ResizeNodesAndClustersToFitLabels();
                    })
                    {
                        Checked = _showIdAsDecimal
                    },
                }),
                new ToolStripMenuItem("Refresh", null, (s, e) =>
                {
                    Graph = null;
                    //Viewer.Invalidate();
                }),
                new ToolStripMenuItem("Save as png...", null, (s, e) =>
                {
                    SaveGraphAs("png");
                }),
            };

            var menu = new ContextMenuStrip();
            menu.Items.AddRange(items);
            menu.Show(Viewer, location);
        }

        private void ShowNodeContextMenu(Node graphNode, Point location)
        {
            Task_t? task = graphNode.UserData as Task_t;
            if (task == null)
            {
                return;
            }

            var items = new ToolStripItem[]
            {
                new ToolStripMenuItem($"Task: {task.Text}")
                {
                    Font = new Font(SystemFonts.MenuFont, System.Drawing.FontStyle.Bold)
                },

                new ToolStripSeparator(),

                new ToolStripMenuItem("Set state to", null, new ToolStripMenuItem[]
                {
                    new("Unavailable", Util.CreateSquare(GetNodeColorFromState(STATE.Unavailable)), (s, e) =>
                    {
                        task.State = STATE.Unavailable;
                        task.IsStateChangedByUser = true;
                    }),
                    new("Available", Util.CreateSquare(GetNodeColorFromState(STATE.Available)), (s, e) =>
                    {
                        task.State = STATE.Available;
                        task.IsStateChangedByUser = true;
                    }),
                    new("Complete", Util.CreateSquare(GetNodeColorFromState(STATE.Complete)), (s, e) =>
                    {
                        task.State = STATE.Complete;
                        task.IsStateChangedByUser = true;
                    }),
                    new("Final", Util.CreateSquare(GetNodeColorFromState(STATE.Final)), (s, e) =>
                    {
                        task.State = STATE.Final;
                        task.IsStateChangedByUser = true;
                    }),
                }),

                new ToolStripMenuItem("Set as current checkpoint", null, (s, e) =>
                {
                    WriteCurrentCheckpointAddress(task);
                })
                {
                    Visible = task.CheckpointEntranceValue != -1
                },
                new ToolStripMenuItem($"Load to checkpoint", null, (s, e) =>
                {
                    LoadCheckpoint(task, false);
                })
                {
                    Visible = task.CheckpointEntranceValue != -1 && Version != DAG_VERSION.V0
                },
                new ToolStripMenuItem($"Load to checkpoint with zero focus", null, (s, e) =>
                {
                    LoadCheckpoint(task, true);
                })
                {
                    Visible = task.CheckpointEntranceValue != -1 && Version != DAG_VERSION.V0
                },
                new ToolStripMenuItem("Load to this entrance location", null, (s, e) =>
                {
                    LoadMap(task.MapId, task.CheckpointEntranceValue, 0);
                })
                {
                    Visible = task.CheckpointEntranceValue != -1 && Version != DAG_VERSION.V0
                },

                new ToolStripMenuItem("Copy", null, new ToolStripItem[]
                {
                    new ToolStripMenuItem($"Address: {task.Address}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.Address);
                    }),
                    new ToolStripMenuItem($"Id: {(_showIdAsDecimal ? task.Id : task.Id.ToString("X"))}", null, (s, e) =>
                    {
                        Clipboard.SetText(_showIdAsDecimal ? task.Id.ToString() : task.Id.ToString("X"));
                    }),
                    new ToolStripMenuItem($"Name: {task.Name}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.Name);
                    }),
                    new ToolStripMenuItem($"Goal description: {task.GoalDescription}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.GoalDescription);
                    })
                    {
                        Visible = task.GoalDescription != ""
                    },
                    new ToolStripMenuItem($"Entrance value: {task.CheckpointEntranceValue:X}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.CheckpointEntranceValue.ToString("X"));
                    })
                    {
                        Visible = task.CheckpointEntranceValue != -1
                    },
                    new ToolStripMenuItem($"Focus count 1: {task.FocusCount1}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.FocusCount1.ToString());
                    }),
                    new ToolStripMenuItem($"Focus count 2: {task.FocusCount2}", null, (s, e) =>
                    {
                        Clipboard.SetText(task.FocusCount2.ToString());
                    }),
                    new ToolStripMenuItem($"Savefile flags address: {GetSavefileAddress(task.Attributes[0]):X}", null, (s, e) =>
                    {
                        Clipboard.SetText($"{GetSavefileAddress(task.Attributes[0]):X}");
                    })
                    {
                        Visible = Version != DAG_VERSION.V0
                    },
                }),
            };

            var menu = new ContextMenuStrip();
            menu.Items.AddRange(items.ToArray());
            menu.Show(Viewer, location);
        }

        private void ShowClusterContextMenu(Node graphNode, Point location)
        {
            Cluster_t? cluster = graphNode.UserData as Cluster_t;
            if (cluster == null)
            {
                return;
            }
            else if (cluster.Address == "0")
            {
                // If clicked on a dummy cluster, show the graph
                // (it gives the illusion the invisible cluster is really not there)
                ShowGraphContextMenu(location);
                return;
            }

            var items = new ToolStripItem[]
            {
                new ToolStripMenuItem($"Job: {cluster.Text}")
                {
                    Font = new Font(SystemFonts.MenuFont, System.Drawing.FontStyle.Bold)
                },

                new ToolStripSeparator(),

                new ToolStripMenuItem("Set state to", null, new ToolStripMenuItem[]
                {
                    new("Unavailable", Util.CreateSquare(GetNodeColorFromState(STATE.Unavailable)), (s, e) =>
                    {
                        for (int i = 0; i < cluster.Tasks.Count; i++)
                        {
                            //if (cluster.Tasks[i].State == STATE.Unavailable
                            //    && cluster.Tasks[i].FocusCount1 == 0
                            //    && cluster.Tasks[i].FocusCount2 == 0)
                            //{
                            //    continue;
                            //}
                            cluster.Tasks[i].State = STATE.Unavailable;
                            cluster.Tasks[i].FocusCount1 = 0;
                            cluster.Tasks[i].FocusCount2 = 0;
                            cluster.Tasks[i].IsStateChangedByUser = true;
                        }
                    }),
                    new("Available", Util.CreateSquare(GetNodeColorFromState(STATE.Available)), (s, e) =>
                    {
                        cluster.Tasks[0].State = STATE.Available;
                        cluster.Tasks[0].FocusCount1 = 0;
                        cluster.Tasks[0].FocusCount2 = 0;
                        cluster.Tasks[0].IsStateChangedByUser = true;
                        for (int i = 1; i < cluster.Tasks.Count; i++)
                        {
                            cluster.Tasks[i].State = STATE.Unavailable;
                            cluster.Tasks[i].FocusCount1 = 0;
                            cluster.Tasks[i].FocusCount2 = 0;
                            cluster.Tasks[i].IsStateChangedByUser = true;
                        }
                    }),
                    new("Complete", Util.CreateSquare(GetNodeColorFromState(STATE.Complete)), (s, e) =>
                    {
                        for (int i = 0; i < cluster.Tasks.Count; i++)
                        {
                            cluster.Tasks[i].State = STATE.Complete;
                            cluster.Tasks[i].FocusCount1 = 1;
                            cluster.Tasks[i].FocusCount2 = 1;
                            cluster.Tasks[i].IsStateChangedByUser = true;
                        }
                    }),
                    new("Final", Util.CreateSquare(GetNodeColorFromState(STATE.Final)), (s, e) =>
                    {
                        for (int i = 0; i < cluster.Tasks.Count; i++)
                        {
                            cluster.Tasks[i].State = STATE.Final;
                            cluster.Tasks[i].FocusCount1 = 1;
                            cluster.Tasks[i].FocusCount2 = 1;
                            cluster.Tasks[i].IsStateChangedByUser = true;
                        }
                    }),
                }),

                new ToolStripMenuItem("Load job", null, (s, e) =>
                {
                    LoadCheckpoint(cluster.Tasks[0], false);
                })
                {
                    Visible = Version != DAG_VERSION.V0
                },
                new ToolStripMenuItem("Load job with zero focus", null, (s, e) =>
                {
                    LoadCheckpoint(cluster.Tasks[0], true);
                })
                {
                    Visible = Version != DAG_VERSION.V0
                },
                new ToolStripMenuItem($"Suck value: {cluster.Suck:0.0} (click to edit)", null, (s, e) =>
                {
                    string input = ShowInputBox($"{cluster.Name}", "Edit suck value", $"{cluster.Suck:0.0}");
                    if (input == null)
                    {
                        // cancel
                        return;
                    }

                    float.TryParse(input, out float result);
                    cluster.Suck = result;
                    WriteClusterSuck(cluster);
                }),

                new ToolStripMenuItem("Copy", null, new ToolStripMenuItem[]
                {
                    new($"Address: {cluster.Address}", null, (s, e) =>
                    {
                        Clipboard.SetText(cluster.Address);
                    }),
                    new($"Id: {(_showIdAsDecimal ? cluster.Id : cluster.Id.ToString("X"))}", null, (s, e) =>
                    {
                        Clipboard.SetText(_showIdAsDecimal ? cluster.Id.ToString() : cluster.Id.ToString("X"));
                    }),
                    new($"Name: {cluster.Name}", null, (s, e) =>
                    {
                        Clipboard.SetText(cluster.Name);
                    }),
                    new($"Description: {cluster.Description}", null, (s, e) =>
                    {
                        Clipboard.SetText(cluster.Description);
                    }),
                    new ToolStripMenuItem($"Savefile flags address: {GetSavefileAddress(cluster.Attributes[0]):X}", null, (s, e) =>
                    {
                        Clipboard.SetText($"{GetSavefileAddress(cluster.Attributes[0]):X}");
                    })
                    {
                        Visible = Version != DAG_VERSION.V0
                    },
                }),
            };

            var menu = new ContextMenuStrip();
            menu.Items.AddRange(items.ToArray());
            menu.Show(Viewer, location);
        }
    }

    [DebuggerDisplay("{Id} | {Text}")]
    public class Cluster_t
    {
        /// <summary>
        /// Unique id for the cluster
        /// </summary>
        public int Id;

        /// <summary>
        /// Internal name for the cluster (m1_follow_dimitri)
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the cluster (the job's name, e.g. "Follow Dimitri").
        /// </summary>
        public string Description;

        /// <summary>
        /// Text to be displayed on the graph
        /// </summary>
        public string Text;

        /// <summary>
        /// Memory address of the cluster's struct
        /// </summary>
        public string Address;

        /// <summary>
        /// Flag to identify if the state of a cluster was changed by the user, instead of the game.
        /// If this flag is set to true, then a memory write is needed
        /// </summary>
        public bool IsStateChangedByUser;

        /// <summary>
        /// Tasks that are part of this cluster
        /// </summary>
        public List<Task_t> Tasks;

        /// <summary>
        /// Suck value
        /// </summary>
        public float Suck;

        /// <summary>
        /// Reference to the subgraph on the graph
        /// </summary>
        public Subgraph Subgraph;

        /// <summary>
        /// Attributes of the cluster (id + subid pairs)
        /// </summary>
        public List<Attribute_t> Attributes;

        public Cluster_t(int id, string address)
        {
            Tasks = new();
            Attributes = new();
            Id = id;
            Address = address;
        }

        public void SetText(bool withAddress, bool withId, bool withName, bool showIdAsDecimal = false)
        {
            Text = "";
            if (withAddress)
            {
                Text += $"{Address}";
            }

            if (withId)
            {
                if (Text != "")
                {
                    Text += $"_";
                }

                if (showIdAsDecimal)
                {
                    Text += $"{Id}";
                }
                else
                {
                    Text += $"{Id:X}";
                }
            }

            if (withName)
            {
                if (Text != "")
                {
                    Text += $"_";
                }

                Text += $"{Name}";
            }

            if (Description != null && Description != "")
            {
                Text += $"\n({Description})";
            }
        }
    }

    [DebuggerDisplay("{Id} | {Text}")]
    public class Task_t
    {
        /// <summary>
        /// Reference to the node on the graph
        /// </summary>
        public Microsoft.Msagl.Drawing.Node MsaglNode;

        /// <summary>
        /// Unique id for the task
        /// </summary>
        public int Id;

        /// <summary>
        /// Internal name for the task (e.g. "t1_follow_intro")
        /// </summary>
        public string Name;

        /// <summary>
        /// How many times the task has got the focus 1
        /// </summary>
        public int FocusCount1;

        /// <summary>
        /// How many times the task has got the focus 2
        /// </summary>
        public int FocusCount2;

        /// <summary>
        /// Description of the task (pause menu -> job help, the "Goals" in the job).
        /// If not present, it is set to -1.
        /// </summary>
        public string GoalDescription;

        /// <summary>
        /// The cluster associated with the task
        /// </summary>
        public Cluster_t Cluster;

        /// <summary>
        /// Text to be displayed on the graph
        /// </summary>
        public string Text;

        /// <summary>
        /// Memory address of the task's struct
        /// </summary>
        public string Address;

        /// <summary>
        /// Map id (ONLY FOR CHECKPOINTS)
        /// </summary>
        public int MapId;

        /// <summary>
        /// Entity id (ONLY FOR CHECKPOINTS)
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Entity id player 2 (ONLY FOR CHECKPOINTS in Sly 3)
        /// </summary>
        public int EntityId2;

        /// <summary>
        /// State of the task
        /// </summary>
        public STATE State;

        /// <summary>
        /// Flag to identify if a field of a task was changed by the user instead of the game.
        /// If this flag is set to true, then a memory write is needed
        /// </summary>
        public bool IsStateChangedByUser;

        /// <summary>
        /// Variable used to mark the task to a desired state when loading a checkpoint
        /// </summary>
        public bool MarkSavefileChange;

        /// <summary>
        /// State to set the task to when loading a checkpoint. Only considered when MarkSavefileChange is set to true
        /// </summary>
        public STATE StateSavefileChange;

        /// <summary>
        /// The entrance value. Used as a flag to determine if the task is a checkpoint and also needed when loading a checkpoint
        /// </summary>
        public int CheckpointEntranceValue;

        /// <summary>
        /// Flag to identify if the task is a chalktalk.
        /// </summary>
        public bool IsChalktalk;

        /// <summary>
        /// Parents of the task
        /// </summary>
        public List<Task_t> Parent;

        /// <summary>
        /// Childrens of the task
        /// </summary>
        public List<Task_t> Children;

        /// <summary>
        /// Attributes of the task (id + subid pairs)
        /// </summary>
        public List<Attribute_t> Attributes;

        public Task_t()
        {
            Parent = new();
            Children = new();
            Attributes = new();
            EntityId2 = -1;
        }

        public void SetText(bool withAddress, bool withId, bool withName, bool showIdAsDecimal = false)
        {
            Text = "";
            if (withAddress)
            {
                Text += $"{Address}";
            }

            if (withId)
            {
                if (Text != "")
                {
                    Text += $"_";
                }

                if (showIdAsDecimal)
                {
                    Text += $"{Id}";
                }
                else
                {
                    Text += $"{Id:X}";
                }
            }

            if (withName)
            {
                if (Text != "")
                {
                    Text += $"_";
                }

                Text += $"{Name}";
            }

            if (GoalDescription != "")
            {
                Text += $"\n({GoalDescription})";
            }
        }
    }

    public enum DAG_VERSION
    {
        V0 = 0, // Sly 2 e3 demo, march
        V1 = 1, // Sly 2 retail
        V2 = 2, // Sly 3 e3 demo
        V3 = 3 // Sly 3 retail
    }

    [DebuggerDisplay("Id: {Id,h} SubId: {SubId,h}")]
    public class Attribute_t
    {
        public short Id;
        public short SubId;
        public Attribute_t()
        {

        }
    }

    public enum STATE
    {
        Unavailable = 0,
        Available = 1,
        Complete = 2,
        Final = 3,
    }
}
