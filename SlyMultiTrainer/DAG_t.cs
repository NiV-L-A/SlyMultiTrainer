using Memory;
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
        public string ClusterIdAddress = "";
        public string Sly3Time = "";
        public string Sly3Flag = "";
        public string OffsetState = "";
        public string OffsetFocusCount = "";
        public string OffsetCompleteCount = "";
        public string OffsetGoalDescription = ""; // FOR CLUSTER: "Locate The Job Start Point" (for satellite sabotage)
        public string OffsetMissionName = ""; // FOR CLUSTER: "Satellite Sabotage"
        public string OffsetMissionDescription = ""; // FOR CLUSTER (we don't use it for now)
        public string OffsetClusterPointer = ""; // Pointer to the cluster node
        public string OffsetChildrenCount = ""; // Count of how many children the task has. The pointer to the array of node pointers is at +4
        public string OffsetCheckpointEntranceValue = ""; // Entrance value of the checkpoint
        public string OffsetAttributes = "";
        public string OffsetAttributesForCluster = "";
        public string OffsetVerticalLayer = "";
        public int AttributeCountForTask = 0;

        public Func<int, string> GetStringFromId;
        public Action<int, int, int> LoadMap;
        public Action<int, int> WriteActCharId;

        public int NodeDefaultLineWidth = 1;
        public int SelectedNodeDefaultLineWidth = 5;
        public int NodeCurrentCheckpointDefaultLineWidth = 3;
        public int SelectedClusterDefaultLineWidth = 10;

        private Memory.Mem _m;
        private Sly2_3_Savefile _savefile;
        private Node? _selectedNode; // For highlighting
        private bool _showAddress = false;
        private bool _showId = false;
        private bool _showName = true;
        private bool _showIdAsDecimal = false;
        // Store the current zoom and pan before refreshing
        private bool _lockPanAndZoomOnRefresh = false;
        private Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation _oldTransform;
        private Font _boldFont;

        public DAG_t(Memory.Mem m)
        {
            Viewer = new();
            Viewer.BackColor = System.Drawing.Color.White;
            Viewer.PanButtonPressed = true;
            Viewer.ToolBarIsVisible = false;
            Viewer.Dock = DockStyle.Fill;
            Viewer.OutsideAreaBrush = Brushes.White;
            Viewer.MouseClick += Viewer_MouseClick;
            Viewer.MouseDoubleClick += Viewer_MouseDoubleClick;
            Viewer.KeyUp += Viewer_KeyUp;
            Tasks = new();
            Clusters = new();
            _m = m;
            _boldFont = new Font(SystemFonts.MenuFont, System.Drawing.FontStyle.Bold);
        }

        public void Init(Sly2_3_Savefile savefile)
        {
            Tasks.Clear();
            Clusters.Clear();
            _savefile = savefile;
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

        public void TriggerRefresh()
        {
            Viewer.Enabled = false;
            Graph = null;
        }

        public void GetDAG()
        {
            ReadTask(RootNodePointer);

            // We make sure that we parsed every node by checking the nextNode field instead of a node's children.
            // This way we fix sly 2 ep3, in which there's a floating node not attached to the dag
            for (int i = 0; i < Tasks.Count; i++)
            {
                string nextNodeAddress = _m.ReadInt($"{Tasks[i].Address}+20").ToString("X");

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
                        ReadTask($"{Tasks[i].Address}+20");
                    }
                }
            }

            // We sort tasks in "predecessors first" order to be able to properly walk through the dag during checkpoint loading
            Tasks.Sort(SortTasks);

            // While we're at it, we sort the clusters and tasks inside each cluster too
            Clusters = Clusters.OrderBy(cluster => Tasks.FindIndex(task => task.Cluster == cluster)).ToList();
            foreach (Cluster_t cluster in Clusters)
            {
                cluster.Tasks = cluster.Tasks.OrderBy(Tasks.IndexOf).ToList();
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

            task.Id = _m.ReadInt($"{taskAddress}+18");
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
            task.FocusCount = _m.ReadInt($"{taskAddress}+{OffsetFocusCount}");
            task.CompleteCount = _m.ReadInt($"{taskAddress}+{OffsetCompleteCount}");
            task.VerticalLayer = _m.ReadInt($"{taskAddress}+{OffsetVerticalLayer}");
            task.VerticalLayerGroup = _m.ReadInt($"{taskAddress}+{OffsetVerticalLayer}+4");
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
                    SavefileAttribute_t attr = new();
                    attr.Id = _m.ReadShort($"{taskAddress}+{OffsetAttributes}+{8 * i + 4:X},0");
                    attr.SubId = _m.ReadShort($"{taskAddress}+{OffsetAttributes}+{8 * i + 4:X},2");
                    task.Attributes.Add(attr);
                }

                if (Version == DAG_VERSION.V0)
                {
                    // Sly 2 march has the first savefile value in the second attributes position
                    task.SavefileFlagsAddress = _savefile.GetSavefileAddress(task.Attributes[1].Id, task.Attributes[1].SubId);
                }
                else
                {
                    task.SavefileFlagsAddress = _savefile.GetSavefileAddress(task.Attributes[0].Id, task.Attributes[0].SubId);
                }

                // Name
                task.Name = _savefile.SavefileStringTable.GetValueOrDefault(task.Attributes[0].Id, "");

                // Chalktalk detection, based on the name for now.
                // Usually "tN_chalktalkM" or for some sly 3 tasks "tN_chalktalk_M"
                if (Regex.IsMatch(task.Name, @"^t\d_chalktalk_?\d$"))
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
            Cluster_t cluster = new(_m.ReadInt($"{clusterAddress}+18"), clusterAddress);

            if (contributeToDag)
            {
                for (int i = 0; i < AttributeCountForTask - 1; i++)
                {
                    SavefileAttribute_t attr = new();
                    attr.Id = _m.ReadShort($"{clusterAddress}+{OffsetAttributesForCluster}+{8 * i + 4:X},0");
                    attr.SubId = _m.ReadShort($"{clusterAddress}+{OffsetAttributesForCluster}+{8 * i + 4:X},2");
                    cluster.Attributes.Add(attr);
                }

                if (Version == DAG_VERSION.V0)
                {
                    // Sly 2 march has the first savefile value in the second attributes position
                    cluster.SavefileFlagsAddress = _savefile.GetSavefileAddress(cluster.Attributes[1].Id, cluster.Attributes[1].SubId);
                }
                else
                {
                    cluster.SavefileFlagsAddress = _savefile.GetSavefileAddress(cluster.Attributes[0].Id, cluster.Attributes[0].SubId);
                }

                // Get the mission name (if any)
                cluster.Name = _savefile.SavefileStringTable.GetValueOrDefault(cluster.Attributes[0].Id, "");

                if (Version != DAG_VERSION.V2)
                {
                    int missionNameId = _m.ReadInt($"{clusterAddress}+{OffsetMissionName}");
                    cluster.Description = GetStringFromId(missionNameId);
                }

                cluster.SetText(_showAddress, _showId, _showName, _showIdAsDecimal);
            }

            cluster.Suck = _savefile.ReadSavefileValue<float>(cluster.Name, "suck_value");
            return cluster;
        }

        public bool IsTaskEqualToTask(Task_t task1, Task_t task2)
        {
            if (task1.State != task2.State
             || task1.FocusCount != task2.FocusCount
             || task1.CompleteCount != task2.CompleteCount)
            {
                return false;
            }

            return true;
        }

        private int SortTasks(Task_t task1, Task_t task2)
        {
            // From Sly 3 PAL September 2 at 0019B658
            // Tasks are sorted in "predecessors first" order:
            // 1) By VerticalLayerGroup
            int result = task1.VerticalLayerGroup.CompareTo(task2.VerticalLayerGroup);
            if (result != 0)
            {
                return result;
            }

            // 2) By cluster's id
            // NOTE: Tasks that are not part of a cluster have more priority than tasks that are part of a cluster
            int clusterId1 = -1;
            if (task1.Cluster.Address != "0")
            {
                clusterId1 = task1.Cluster.Id;
            }

            int clusterId2 = -1;
            if (task2.Cluster.Address != "0")
            {
                clusterId2 = task2.Cluster.Id;
            }

            result = clusterId1.CompareTo(clusterId2);
            if (result != 0)
            {
                return result;
            }

            // 3) By VerticalLayer (for tasks in the same cluster or when both tasks are not part of any cluster)
            result = task1.VerticalLayer.CompareTo(task2.VerticalLayer);
            if (result != 0)
            {
                return result;
            }

            // 4) By task's id (for tasks in the same vertical layer)
            return task1.Id.CompareTo(task2.Id);
        }

        public bool IsClusterEqualToCluster(Cluster_t cluster1, Cluster_t cluster2)
        {
            if (cluster1.Suck != cluster2.Suck)
            {
                return false;
            }

            return true;
        }

        public string GetCurrentCheckpointAddress()
        {
            return _m.ReadInt(CurrentCheckpointNodePointer).ToString("X");
        }

        public void WriteCurrentCheckpointAddress(Task_t? task)
        {
            string value = "0";
            if (task != null)
            {
                value = $"0x{task.Address}";
            }

            _m.WriteMemory($"{CurrentCheckpointNodePointer}", "int", $"{value}");
        }

        public void WriteClusterSuck(Cluster_t cluster, float suck)
        {
            cluster.Suck = suck;
            _savefile.WriteSavefileValue("float", ((float)suck).ToString(), cluster.Name, "suck_value");
        }

        public void WriteClusterState(Cluster_t cluster, STATE state)
        {
            if (state == STATE.Unavailable)
            {
                for (int i = 0; i < cluster.Tasks.Count; i++)
                {
                    WriteTaskState(cluster.Tasks[i], STATE.Unavailable);
                    WriteTaskFocusAndCompleteCount(cluster.Tasks[i], 0, 0);
                }
            }
            else if (state == STATE.Available)
            {
                WriteTaskState(cluster.Tasks[0], STATE.Available);
                WriteTaskFocusAndCompleteCount(cluster.Tasks[0], 0, 0);

                for (int i = 1; i < cluster.Tasks.Count; i++)
                {
                    WriteTaskState(cluster.Tasks[i], STATE.Unavailable);
                    WriteTaskFocusAndCompleteCount(cluster.Tasks[i], 0, 0);
                }
            }
            else if (state == STATE.Complete)
            {
                for (int i = 0; i < cluster.Tasks.Count; i++)
                {
                    WriteTaskState(cluster.Tasks[i], STATE.Complete);
                    WriteTaskFocusAndCompleteCount(cluster.Tasks[i], 1, 1);
                }
            }
            else if (state == STATE.Final)
            {
                for (int i = 0; i < cluster.Tasks.Count; i++)
                {
                    WriteTaskState(cluster.Tasks[i], STATE.Final);
                    WriteTaskFocusAndCompleteCount(cluster.Tasks[i], 1, 1);
                }
            }
        }

        public void WriteTaskState(Task_t task, STATE state, bool alsoToSavefile = true)
        {
            _m.WriteMemory($"{task.Address}+{OffsetState}", "int", ((int)state).ToString());

            if (alsoToSavefile)
            {
                _savefile.WriteSavefileValue("int", ((int)state).ToString(), task.Name, "state");
            }
        }

        public void WriteTaskFocusAndCompleteCount(Task_t task, int focusCount, int completeCount, bool alsoToSavefile = true)
        {
            task.FocusCount = focusCount;
            task.CompleteCount = completeCount;

            int[] tmp;
            if (Version == DAG_VERSION.V0)
            {
                tmp =
                [
                    task.FocusCount,
                ];
            }
            else
            {
                tmp =
                [
                    task.FocusCount,
                    task.CompleteCount,
                ];
            }

            string bytes = EndianBitConverter.ByteArrayToString(EndianBitConverter.ArrayToByteArray(tmp));
            _m.WriteMemory($"{task.Address}+{OffsetFocusCount}", "bytes", bytes);
            if (alsoToSavefile)
            {
                _savefile.WriteSavefileValue("bytes", bytes, task.Name, "focus_count");
            }
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
            // The subId codes are different between builds, so that's why an enum was not implemented.
            
            // For tasks in sly 2 ntsc,
            //  0x6F is the focus count (internally called "focus_count"),
            //  0xC1 is internally called "complete_count",
            //  0x70 is internally called "is_complete",
            //  0xC2 is State (internally called "state"),
            //  0xC3 is internally called "from_memcard"

            // For clusters in sly 2 ntsc,
            //  0x6F is the number of attempts (internally called "focus_count")
            //  0x70 is internally called "is_complete",
            //  0x71 is internally called "time_played",
            //  0x72 is internally called "suck_value"

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
            // 0xF8, 0x6F = 0x006F00F8 = 0x003D6100 = FocusCount
            // 0xF8, 0xC1 = 0x00C100F8 = 0x003D6104 = CompleteCount
            // 0xF8, 0x70 = 0x007000F8 = 0x003D6108 = IsComplete
            // 0xF8, 0xC2 = 0x00C200F8 = 0x003D610C = State
            // 0xF8, 0xC3 = 0x00C300F8 = 0x003D6110 = FromMemcard
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
            // - We go against the "Stop when predecessor is already in desired state" point. Is Bruce assuming the dag is *always* in a valid state?
            // - While we walk backwards we mark each node to be final (if we want to set targetTask to be available, it means everything before that must be final)
            // - NOTE: If a specific checkpoint was loaded, the tasks that come before targetTask AND that are part of the same cluster need to be marked as complete instead of final. We check for this later. For now, let's just mark all parents to be final
            // IMPORTANT: We manually go look for the root node. This fixes sly2 ep3 having more than 1 node with no parents (Sly 2 march ep3 has even more)
            MarkTaskSavefileChangeBackward(targetTask, STATE.Available);

            // https://youtu.be/Yl20uIQ3fEw?t=1935 (32:15)
            // BRUCE:
            // - Go through all tasks in "predecessors first" order (we sorted the tasks already when we loaded the dag. Our tasks here are already in the correct order)
            //   - Set each task to its desired state
            //   - Recursively walk forward through successors
            //   - Skip tasks marked with a state

            // - We start from the rootTask (top of the dag) and we mark the childrens,
            // but we "skip" the ones already marked (the word "skip" here means we don't mark them AGAIN, but we still need to consider their children, which means every node in the dag)
            // - Childrens of final have to be available
            // - Childrens of available have to be unavailable
            // - Childrens of unavailable have to be unavailable
            // - Childrens of complete have to be available
            MarkTaskSavefileChangeForward();

            // At this point all nodes in the dag have been marked with a desired state
            // Let's now go through each task and get the array of bytes that is going to be written to the savefile region
            List<(int Address, byte[] Values)> SavefileValues = new(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                Task_t task = Tasks[i];
                if (targetTask.Cluster == task.Cluster
                 && task.StateSavefileChange == STATE.Final)
                {
                    // If we are loading a checkpoint which is not the first task of the cluster
                    // Then we need to set to complete the previous tasks, but only in this cluster
                    
                    // NOTE: This half goes against the debug menu:
                    // loading a checkpoint that has tasks of the same cluster in the final state will keep them final
                    // loading a checkpoint that has tasks of the same cluster NOT in the final state, will change them to complete
                    
                    // Our implementation here is to always change them to complete, no matter their current state
                    task.StateSavefileChange = STATE.Complete;
                }

                var addressValuesPair = TaskGetSavefileFlags(task, withZeroFocus, targetTask == task);
                SavefileValues.Add(addressValuesPair);
                task.IsMarkedForSavefileChange = false;

                if (Version >= DAG_VERSION.V2)
                {
                    // In sly 3 the focus in the task structs are copied to the ones in the savefile
                    // So we also need to write the focus count to each task struct
                    // Unfortunately these memory writes will cost us some time

                    // Assume the player has not been through that task (state is unavailable)
                    int focusCount = 0;
                    int completeCount = 0;

                    if (task.StateSavefileChange == STATE.Final
                     || task.StateSavefileChange == STATE.Complete)
                    {
                        // The player has been through that task
                        focusCount = 1;
                        completeCount = 1;
                    }
                    else if (task == targetTask)
                    {
                        // And for the "targetTask" (the state is available) we check if the user wants to load it with or without focus
                        if (!withZeroFocus)
                        {
                            focusCount = 1;
                            completeCount = 1;
                        }
                    }

                    WriteTaskFocusAndCompleteCount(task, focusCount, completeCount, false);
                }
            }

            // Commit the desired dag state to the savefile
            List<(int Address, byte[] Values)> SavefileValuesToWrite = SavefileValues.Order().ToList();
            int toSkip = 0;
            int toTake = SavefileValuesToWrite.Count;
            if (Version >= DAG_VERSION.V2)
            {
                // The optimization only works for sly 3 in ep1 and 2. For ep3, 4 and 5, the sequence breaks.
                // For example, in ep3 day2, the 0x14 values at 46BAB8 come from the offset 0x2D88, which comes from the Id 0x29C and SubId 0x56. But the task with that attribute (t3_quiet_kill_purchase_query) is from ep5
                // So, we detect when the next array of 0x14 bytes is not at +0x14 of the current address and we break up the values to write into pieces
                toTake = 0;
                for (int i = 0; i < SavefileValuesToWrite.Count; i++)
                {
                    if (i == SavefileValuesToWrite.Count - 1)
                    {
                        toTake = i + 1 - toTake;
                        break;
                    }

                    if (SavefileValuesToWrite[i + 1].Address != SavefileValuesToWrite[i].Address + SavefileValuesToWrite[i].Values.Length)
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
                        _m.WriteMemory($"{SavefileValuesToWrite[toSkip].Address:X}", "bytes", EndianBitConverter.ByteArrayToString(flags));
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
            _m.WriteMemory($"{SavefileValuesToWrite[toSkip].Address:X}", "bytes", EndianBitConverter.ByteArrayToString(flags2));

            List<int> NodeIds =
            [
                targetTask.Cluster.Id, // Job id
                targetTask.Id, // Last task that got the focus
                targetTask.Id // Checkpoint id
            ];

            // If it's sly 3, we write 0 at +C.
            // This disables 3D for the maps that support it (e.g. ep1 t1_gauntlet_interior_intro)
            if (Version >= DAG_VERSION.V2)
            {
                NodeIds.Add(0);
            }

            _m.WriteMemory($"{ClusterIdAddress}", "bytes", EndianBitConverter.ByteArrayToString(EndianBitConverter.ArrayToByteArray(NodeIds.ToArray())));

            // Entity
            WriteActCharId(targetTask.EntityId, targetTask.EntityId2);

            // Mode. Used for sly 3 to indicate if there should be a full reload (in case the targetTask is from a different day than the current one)
            int mode = 0;
            if (Version == DAG_VERSION.V3)
            {
                // From Sly 3 PAL September 2 at 0019ADB0
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
                    if (flag == 0)
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

        public (int, byte[]) TaskGetSavefileFlags(Task_t task, bool withZeroFocus, bool isTargetTask)
        {
            byte[] bytes;
            if (Version == DAG_VERSION.V0)
            {
                int[] flags =
                {
                    0,
                    (int)task.StateSavefileChange,
                };

                if (task.StateSavefileChange == STATE.Final
                 || task.StateSavefileChange == STATE.Complete)
                {
                    // We set the focus if the desired state is final or complete (the player has been through that task)
                    flags[0] = 1;
                }
                else if (isTargetTask)
                {
                    // And for the "targetTask" we check if the user wants to load it with or without focus
                    if (!withZeroFocus)
                    {
                        flags[0] = 1;
                    }
                }
                else
                {
                    // The rest is unavailable or available, so no focus because the player has not been through that task
                }

                bytes = new byte[9];
                byte[] intBytes = EndianBitConverter.ArrayToByteArray(flags);
                Array.Copy(intBytes, 0, bytes, 1, 8);
            }
            else
            {
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
                else if (isTargetTask)
                {
                    // And for the "targetTask" we check if the user wants to load it with or without focus
                    if (!withZeroFocus)
                    {
                        flags[0] = 1;
                        flags[1] = 1;
                    }
                }
                else
                {
                    // The rest is unavailable or available, so no focus because the player has not been through that task
                }

                bytes = EndianBitConverter.ArrayToByteArray(flags);
            }

            return (task.SavefileFlagsAddress, bytes);
        }

        private void MarkTaskSavefileChange(Task_t task, STATE state)
        {
            task.IsMarkedForSavefileChange = true;
            task.StateSavefileChange = state;
        }

        private void MarkTaskSavefileChangeBackward(Task_t task, STATE state)
        {
            if (task.IsMarkedForSavefileChange)
            {
                return;
            }

            MarkTaskSavefileChange(task, state);

            // We go against the "Stop when predecessor is already in desired state" point. Is Bruce assuming the dag is *always* in a valid state?
            // Just return when we reached the root
            if (/*task.State == state ||*/ task.Parent.Count == 0)
            {
                return;
            }

            for (int i = 0; i < task.Parent.Count; i++)
            {
                MarkTaskSavefileChangeBackward(task.Parent[i], STATE.Final);
            }
        }

        private void MarkTaskSavefileChangeForward()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                Task_t task = Tasks[i];
                if (task.IsMarkedForSavefileChange)
                {
                    // Skip tasks already marked
                    continue;
                }

                if (task.Parent.All(x => x.IsMarkedForSavefileChange && x.StateSavefileChange == STATE.Final))
                {
                    // Tasks on the same vertical layer as targetTask should be set to available
                    if (task.State == STATE.Final)
                    {
                        // but if they are final RIGHT NOW, keep them final
                        // This mimics the debug menu
                        MarkTaskSavefileChange(task, STATE.Final);
                    }
                    else
                    {
                        MarkTaskSavefileChange(task, STATE.Available);
                    }
                }
                else if (task.Parent.Any(x => x.IsMarkedForSavefileChange && x.StateSavefileChange == STATE.Available))
                {
                    MarkTaskSavefileChange(task, STATE.Unavailable);
                }
                else if (task.Parent.Any(x => x.IsMarkedForSavefileChange && x.StateSavefileChange == STATE.Unavailable))
                {
                    MarkTaskSavefileChange(task, STATE.Unavailable);
                }
                else if (task.Parent.Any(x => x.IsMarkedForSavefileChange && x.StateSavefileChange == STATE.Complete))
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

        /*
        Microsoft.Msagl.Core.Geometry.Curves.ICurve GetNodeBoundary(Microsoft.Msagl.Drawing.Node node)
        {
            using (Graphics g = Viewer.CreateGraphics())
            {
                var font = new Font("Courier New", 8F);
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

        public bool SetGraph()
        {
            if (_lockPanAndZoomOnRefresh)
            {
                _oldTransform = Viewer.Transform;
            }

            Graph = new();
            var settings = new SugiyamaLayoutSettings
            {
                // Make nodes and clusters start at the same layer
                SnapToGridByY = SnapToGridByY.Top,
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
                    if (child.Address == null)
                    {
                        TriggerRefresh();
                        return false;
                    }

                    Trav(child);
                    Graph.AddEdge(Tasks[i].Id.ToString("X"), child.Id.ToString("X"));
                }
            }

            // It is possible that msagl fails to set the graph
            // It seems to happen if the window is minimized and the game is loading a map
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

            return true;
        }

        public Microsoft.Msagl.Drawing.Color GetClusterColorFromState(STATE state)
        {
            switch (state)
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

        public Microsoft.Msagl.Drawing.Color GetNodeColorFromState(STATE state)
        {
            switch (state)
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

        private string? ShowSearchNodeInputBox(string title, string textBoxLabel, string initialTextBoxValue, out string cmbSelectedText)
        {
            int topMargin = 20;
            Form inputForm = new()
            {
                Text = title,
                Width = 320,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                AcceptButton = null,
                CancelButton = null
            };

            System.Windows.Forms.Label lblText = new()
            {
                Left = 10,
                Top = 10,
                Text = textBoxLabel,
                AutoSize = true
            };

            ComboBox cmbType = new()
            {
                Left = lblText.Width,
                Top = lblText.Top - 4,
                AutoSize = true,
                Items = { "Address", "Id" },
                SelectedIndex = 0,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            TextBox txtInput = new()
            {
                Left = 10,
                Top = cmbType.Top + topMargin + 10,
                Width = 280,
                Text = initialTextBoxValue
            };

            Button btnOk = new()
            {
                Text = "OK",
                Left = 140,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.OK
            };

            Button btnCancel = new()
            {
                Text = "Cancel",
                Left = 220,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.Cancel
            };

            inputForm.Controls.Add(lblText);
            inputForm.Controls.Add(txtInput);
            inputForm.Controls.Add(cmbType);
            inputForm.Controls.Add(btnOk);
            inputForm.Controls.Add(btnCancel);
            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            DialogResult result = inputForm.ShowDialog();

            cmbSelectedText = cmbType.Text;
            if (result != DialogResult.OK)
            {
                return null;
            }

            return (string?)txtInput.Text;
        }

        private string? ShowClusterSuckInputBox(string title, string textBoxLabel, string initialTextBoxValue)
        {
            int topMargin = 20;
            Form inputForm = new()
            {
                Text = title,
                Width = 320,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                AcceptButton = null,
                CancelButton = null
            };

            System.Windows.Forms.Label lblText = new()
            {
                Left = 10,
                Top = 10,
                Text = textBoxLabel,
                AutoSize = true
            };

            TextBox txtInput = new()
            {
                Left = 10,
                Top = lblText.Top + topMargin + 10,
                Width = 280,
                Text = initialTextBoxValue
            };

            Button btnOk = new()
            {
                Text = "OK",
                Left = 140,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.OK
            };

            Button btnCancel = new()
            {
                Text = "Cancel",
                Left = 220,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.Cancel
            };

            inputForm.Controls.Add(lblText);
            inputForm.Controls.Add(txtInput);
            inputForm.Controls.Add(btnOk);
            inputForm.Controls.Add(btnCancel);
            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            DialogResult result = inputForm.ShowDialog();

            if (result != DialogResult.OK)
            {
                return null;
            }

            return (string?)txtInput.Text;
        }

        private void SaveGraphAs(string extension)
        {
            if (extension == "png")
            {
                SaveFileDialog sfd = new();
                sfd.Filter = $"Images|*.{extension}";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Bitmap image = GetImageFromGraph();
                image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                image.Dispose();

                MessageBox.Show($"File saved to:\n\"{sfd.FileName}\"", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private Bitmap GetImageFromGraph()
        {
            GraphRenderer renderer = new(Graph);
            renderer.CalculateLayout();
            Bitmap image = new((int)Graph.Width, (int)Graph.Height);
            renderer.Render(image);
            return image;
        }

        private void Viewer_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (Viewer.Graph == null)
                {
                    // Prevent crash when clicking before the graph is loaded (pcsx2 just launched and in splash screen)
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
                        ShowTaskContextMenu(node, e.Location);
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

        private void Viewer_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Load checkpoint

                if (Viewer.Graph == null)
                {
                    // Prevent crash when clicking before the graph is loaded (pcsx2 just launched and in splash screen)
                    return;
                }

                DNode? dNode = Viewer.GetObjectAt(e.Location) as DNode;
                if (dNode == null)
                {
                    return;
                }

                Node node = dNode.Node;
                Task_t? task = null;
                if (node.UserData is Task_t)
                {
                    SelectNode(node, SelectedNodeDefaultLineWidth);
                    task = node.UserData as Task_t;

                    if (task.CheckpointEntranceValue == -1)
                    {
                        return;
                    }
                }
                else if (node.UserData is Cluster_t cluster)
                {
                    if (cluster.Address == "0")
                    {
                        // dummy cluster
                        return;
                    }

                    SelectNode(node, SelectedClusterDefaultLineWidth);
                    task = cluster.Tasks[0];
                }

                // load checkpoint with focus if shift is being held down
                bool hasShift = GViewer.ModifierKeys.HasFlag(Keys.Shift);
                LoadCheckpoint(task, hasShift);
            }
        }

        private void Viewer_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                HandleSearchNode();
                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                TriggerRefresh();
                return;
            }
        }

        private void ShowGraphContextMenu(Point location)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add(new ToolStripMenuItem($"Graph ({Tasks.Count} tasks)")
            {
                Font = _boldFont,
            });

            menu.Items.Add(new ToolStripSeparator());

            if (Tasks.Count != 0)
            {
                menu.Items.Add(new ToolStripMenuItem
                (
                    "Go to root",
                    null,
                    (s, e) =>
                    {
                        var rootNode = Tasks[0].MsaglNode;
                        if (rootNode != null)
                        {
                            Viewer.ShowBBox(rootNode.BoundingBox);
                        }
                    }
                ));
            }

            menu.Items.Add(new ToolStripMenuItem
            (
                "Restore pan and zoom",
                null,
                (s, e) =>
                {
                    Viewer.Transform = null;
                    Viewer.Invalidate();
                }
            ));

            var settingsMenu = new ToolStripMenuItem("Settings (requires refresh)");

            settingsMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                "Lock pan and zoom on refresh",
                null,
                (s, e) =>
                {
                    _lockPanAndZoomOnRefresh = !(s as ToolStripMenuItem)!.Checked;
                }
            )
            {
                Checked = _lockPanAndZoomOnRefresh
            });

            settingsMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                "Show name",
                null,
                (s, e) =>
                {
                    _showName = !(s as ToolStripMenuItem)!.Checked;
                    ResizeNodesAndClustersToFitLabels();
                }
            )
            {
                Checked = _showName
            });

            settingsMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                "Show address",
                null,
                (s, e) =>
                {
                    _showAddress = !(s as ToolStripMenuItem)!.Checked;
                    ResizeNodesAndClustersToFitLabels();
                }
            )
            {
                Checked = _showAddress
            });

            settingsMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                "Show id",
                null,
                (s, e) =>
                {
                    _showId = !(s as ToolStripMenuItem)!.Checked;
                    ResizeNodesAndClustersToFitLabels();
                }
            )
            {
                Checked = _showId
            });

            settingsMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                "Show id as decimal",
                null,
                (s, e) =>
                {
                    _showIdAsDecimal = !(s as ToolStripMenuItem)!.Checked;
                    ResizeNodesAndClustersToFitLabels();
                }
            )
            {
                Checked = _showIdAsDecimal
            });

            menu.Items.Add(settingsMenu);

            menu.Items.Add(new ToolStripMenuItem
            (
                "Refresh",
                Util.GetEmbeddedImage($"refresh.png"),
                (s, e) => TriggerRefresh(),
                Keys.F5
            ));

            menu.Items.Add(new ToolStripMenuItem
            (
                "Search node...",
                null,
                (s, e) => HandleSearchNode(),
                Keys.F3
            ));

            menu.Items.Add(new ToolStripMenuItem
            (
                "Save as png...",
                null,
                (s, e) => SaveGraphAs("png")
            ));

            menu.Show(Viewer, location);
        }

        private void HandleSearchNode()
        {
            string clipboard = Clipboard.GetText();

            // Cap to 8 characters to prevent freezing the textbox
            if (clipboard.Length > 8)
            {
                clipboard = "";
            }

            string? input = ShowSearchNodeInputBox($"Search node", "Search node by", $"{clipboard}", out string cmbSelectedText);
            if (input == null)
            {
                // cancel
                return;
            }

            Microsoft.Msagl.Drawing.Node? nodeToFocus = null;
            if (cmbSelectedText == "Id")
            {
                if (_showIdAsDecimal)
                {
                    // (0...)2578
                    if (!int.TryParse(input, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int id))
                    {
                        MessageBox.Show($"Invalid decimal number: {input}", "Search node by id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    nodeToFocus = GetNodeFromId(id);
                    if (nodeToFocus is null)
                    {
                        MessageBox.Show($"Could not find node with id '{input}' (parsed as decimal)", "Search node by id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // (0...)A12
                    // (0...)a12
                    // 0x(0...)A12
                    // 0x(0...)a12

                    if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        input = input.Substring(2);
                    }

                    input = input.ToUpper();

                    if (!int.TryParse(input, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int id))
                    {
                        MessageBox.Show($"Invalid hexadecimal number: {input}", "Search node by id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    nodeToFocus = GetNodeFromId(id);
                    if (nodeToFocus is null)
                    {
                        MessageBox.Show($"Could not find node with id '{input}' (parsed as hexadecimal)", "Search node by id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else if (cmbSelectedText == "Address")
            {
                // (0...)6F9CC0
                // (0...)6f9cc0
                // 0x(0...)6F9CC0
                // 0x(0...)6f9cc0

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                input = input.ToUpper();

                if (!int.TryParse(input, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int res))
                {
                    MessageBox.Show($"Invalid hexadecimal number: {input}", "Search node by address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                nodeToFocus = GetNodeFromAddress(res.ToString("X"));
                if (nodeToFocus is null)
                {
                    MessageBox.Show($"Could not find node with address '{input}'", "Search node by address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Viewer.ShowBBox(nodeToFocus.BoundingBox);
        }

        private Microsoft.Msagl.Drawing.Node? GetNodeFromId(int id)
        {
            var task = Tasks.FirstOrDefault(x => x.Id == id);
            if (task is not null)
            {
                return task.MsaglNode;
            }

            // Maybe it's a cluster
            var cluster = Clusters.FirstOrDefault(x => x.Id == id);
            if (cluster is not null)
            {
                return cluster.Subgraph;
            }

            return null;
        }

        private Microsoft.Msagl.Drawing.Node? GetNodeFromAddress(string address)
        {
            var task = Tasks.FirstOrDefault(x => x.Address == address);
            if (task is not null)
            {
                return task.MsaglNode;
            }

            // Maybe it's a cluster
            var cluster = Clusters.FirstOrDefault(x => x.Address == address);
            if (cluster is not null)
            {
                return cluster.Subgraph;
            }

            return null;
        }

        private void ShowTaskContextMenu(Node graphNode, Point location)
        {
            if (graphNode.UserData is not Task_t task)
            {
                return;
            }

            var menu = new ContextMenuStrip();

            menu.Items.Add(new ToolStripMenuItem($"Task: {task.Text}")
            {
                Font = _boldFont,
            });

            menu.Items.Add(new ToolStripSeparator());

            var stateMenu = new ToolStripMenuItem("Set state to");
            foreach (STATE state in Enum.GetValues<STATE>().Cast<STATE>())
            {
                var item = new ToolStripMenuItem
                (
                    state.ToString(),
                    Util.CreateSquare(GetNodeColorFromState(state)),
                    (s, e) => WriteTaskState(task, state)
                );

                stateMenu.DropDownItems.Add(item);
            }

            menu.Items.Add(stateMenu);

            if (task.CheckpointEntranceValue != -1)
            {
                string currentCheckpointAddress = GetCurrentCheckpointAddress();
                if (task.Address != currentCheckpointAddress)
                {
                    menu.Items.Add(new ToolStripMenuItem
                    (
                        "Set as current checkpoint",
                        null,
                        (s, e) => WriteCurrentCheckpointAddress(task)
                    ));
                }
                else
                {
                    menu.Items.Add(new ToolStripMenuItem
                    (
                        "Unset as current checkpoint",
                        null,
                        (s, e) => WriteCurrentCheckpointAddress(null)
                    ));
                }

                ToolStripMenuItem tmp = new("Load to checkpoint", null, (s, e) => LoadCheckpoint(task, false));
                tmp.ShortcutKeyDisplayString = "Double click";
                menu.Items.Add(tmp);

                tmp = new("Load to checkpoint with zero focus", null, (s, e) => LoadCheckpoint(task, true));
                tmp.ShortcutKeyDisplayString = "Shift+Double click";
                menu.Items.Add(tmp);

                menu.Items.Add(new ToolStripMenuItem
                (
                    "Load to this entrance location", null, (s, e) => LoadMap(task.MapId, task.CheckpointEntranceValue, 0)
                ));
            }

            var copyMenu = new ToolStripMenuItem("Copy");

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Address: {task.Address}",
                null,
                (s, e) => Clipboard.SetText(task.Address)
            ));

            string id = task.Id.ToString();
            if (!_showIdAsDecimal)
            {
                id = task.Id.ToString("X");
            }

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Id: {id}",
                null,
                (s, e) => Clipboard.SetText(id)
            ));

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Name: {task.Name}",
                null,
                (s, e) => Clipboard.SetText(task.Name)
            ));

            if (task.GoalDescription != "")
            {
                copyMenu.DropDownItems.Add(new ToolStripMenuItem
                (
                    $"Goal description: {task.GoalDescription}",
                    null,
                    (s, e) => Clipboard.SetText(task.GoalDescription)
                ));
            }

            if (task.CheckpointEntranceValue != -1)
            {
                copyMenu.DropDownItems.Add(new ToolStripMenuItem
                (
                    $"Entrance value: {task.CheckpointEntranceValue:X}",
                    null,
                    (s, e) => Clipboard.SetText(task.CheckpointEntranceValue.ToString("X"))
                ));
            }

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Focus count: {task.FocusCount}",
                null,
                (s, e) => Clipboard.SetText(task.FocusCount.ToString())
            ));

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Complete count: {task.CompleteCount}",
                null,
                (s, e) => Clipboard.SetText(task.CompleteCount.ToString())
            ));

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Savefile flags address: {task.SavefileFlagsAddress:X}",
                null,
                (s, e) => Clipboard.SetText($"{task.SavefileFlagsAddress:X}")
            ));

            menu.Items.Add(copyMenu);

            menu.Show(Viewer, location);
        }

        private void ShowClusterContextMenu(Node graphNode, Point location)
        {
            if (graphNode.UserData is not Cluster_t cluster)
            {
                return;
            }

            if (cluster.Address == "0")
            {
                // If clicked on a dummy cluster, show the graph
                // (it gives the illusion the invisible cluster is really not there)
                ShowGraphContextMenu(location);
                return;
            }

            var menu = new ContextMenuStrip();

            menu.Items.Add(new ToolStripMenuItem($"Job: {cluster.Text}")
            {
                Font = _boldFont,
            });

            menu.Items.Add(new ToolStripSeparator());

            var stateMenu = new ToolStripMenuItem("Set state to");
            foreach (var state in Enum.GetValues<STATE>().Cast<STATE>())
            {
                var item = new ToolStripMenuItem(
                    state.ToString(),
                    Util.CreateSquare(GetNodeColorFromState(state)),
                    (s, e) => WriteClusterState(cluster, state)
                );

                stateMenu.DropDownItems.Add(item);
            }

            menu.Items.Add(stateMenu);

            ToolStripMenuItem tmp = new("Load job", null, (s, e) => LoadCheckpoint(cluster.Tasks[0], false));
            tmp.ShortcutKeyDisplayString = "Double click";
            menu.Items.Add(tmp);

            tmp = new("Load job with zero focus", null, (s, e) => LoadCheckpoint(cluster.Tasks[0], true));
            tmp.ShortcutKeyDisplayString = "Shift+Double click";
            menu.Items.Add(tmp);

            menu.Items.Add(new ToolStripMenuItem
            (
                $"Suck value: {cluster.Suck:0.0} (click to edit)",
                null,
                (s, e) =>
                {
                    string? input = ShowClusterSuckInputBox($"{cluster.Name}", "Edit suck value", $"{cluster.Suck:0.0}");
                    if (input == null)
                    {
                        // cancel
                        return;
                    }

                    float.TryParse(input, out float suck);
                    WriteClusterSuck(cluster, suck);
                }
            ));

            var copyMenu = new ToolStripMenuItem("Copy");

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Address: {cluster.Address}",
                null,
                (s, e) => Clipboard.SetText(cluster.Address)
            ));

            string id = cluster.Id.ToString();
            if (!_showIdAsDecimal)
            {
                id = cluster.Id.ToString("X");
            }

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Id: {id}",
                null,
                (s, e) => Clipboard.SetText(id)
            ));

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Name: {cluster.Name}",
                null,
                (s, e) => Clipboard.SetText(cluster.Name)
            ));

            if (cluster.Description != "")
            {
                copyMenu.DropDownItems.Add(new ToolStripMenuItem
                (
                    $"Description: {cluster.Description}",
                    null,
                    (s, e) => Clipboard.SetText(cluster.Description)
                ));
            }

            copyMenu.DropDownItems.Add(new ToolStripMenuItem
            (
                $"Savefile flags address: {cluster.SavefileFlagsAddress:X}",
                null,
                (s, e) => Clipboard.SetText($"{cluster.SavefileFlagsAddress:X}")
            ));

            menu.Items.Add(copyMenu);

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
        public List<SavefileAttribute_t> Attributes;

        /// <summary>
        /// The first memory address of the array of flags in the savefile region of this cluster
        /// </summary>
        public int SavefileFlagsAddress;

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
        /// How many times the task has got the focus
        /// </summary>
        public int FocusCount;

        /// <summary>
        /// How many times the task has been set to the complete state
        /// </summary>
        public int CompleteCount;

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
        /// Variable used to mark the task to a desired state when loading a checkpoint
        /// </summary>
        public bool IsMarkedForSavefileChange;

        /// <summary>
        /// State to set the task to when loading a checkpoint. Only considered when IsMarkedForSavefileChange is set to true
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
        public List<SavefileAttribute_t> Attributes;

        /// <summary>
        /// The first memory address of the array of flags in the savefile region of the task
        /// </summary>
        public int SavefileFlagsAddress;

        /// <summary>
        /// Vertical layer of the task. Theoretically, tasks with the same vertical layer are on the same horizontal line.
        /// </summary>
        public int VerticalLayer;

        /// <summary>
        /// Vertical layer group of the task.
        /// </summary>
        public int VerticalLayerGroup;

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
        V0 = 0, // Sly 2 NTSC E3 Demo, Sly 2 NTSC March 17
        V1 = 1, // All other Sly 2 builds
        V2 = 2, // Sly 3 NTSC Demo April 18
        V3 = 3 // All other Sly 3 builds
    }

    [DebuggerDisplay("Id: {Id,h} SubId: {SubId,h}")]
    public class SavefileAttribute_t
    {
        public short Id;
        public short SubId;
        public SavefileAttribute_t()
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
