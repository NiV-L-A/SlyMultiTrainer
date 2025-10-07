# Sly Multi Trainer

[Video showcase](https://www.youtube.com/watch?v=nfF_uditjxA) <br>
[![](https://img.youtube.com/vi/nfF_uditjxA/0.jpg)](https://www.youtube.com/watch?v=nfF_uditjxA)

<img src="SlyMultiTrainer/Img/Sly1_Main.png">
<img src="SlyMultiTrainer/Img/Sly2_Main.png">
<img src="SlyMultiTrainer/Img/Sly3_Main.png">

## Description
Sly Multi Trainer is a trainer for the Sly Cooper games. It is meant to be used with the PCSX2 emulator. <br>
Please, refer to the [Changelog](/SlyMultiTrainer-Changelog.txt) file for information about changes for each version.

Supported builds:
| Game  |      Region      | Short-format build date |   Serial   |   CRC    |
| :---: | :--------------: | :---------------------: | :--------: | :------: |
| Sly 1 |       NTSC       |        0824.2206        | SCUS-97198 | C77AF2CA |
| Sly 1 |       PAL        |        1121.2105        | SCES-50917 | DA3DD765 |
| Sly 1 |      NTSC-J      |        0131.1715        | SCPS-15036 | 15C88C7B |
| Sly 1 |      NTSC-K      |        1231.1308        | SCKA-20004 | 71017DE1 |
| Sly 1 |    NTSC Demo     |        0408.2044        | SCUS-97210 | EF7F0CE6 |
| Sly 1 |     PAL Demo     |        1206.1234        | SCES-50917 | DA3DD765 |
| Sly 1 |   NTSC-J Demo    |        1219.2129        | PAPX-90231 | 9C29F787 |
| Sly 1 |   NTSC-K Demo    |        1219.2129        | SCKA-90004 | 9CB33FB5 |
| Sly 2 |       NTSC       |        0813.0032        | SCUS-97316 | 07652DD9 |
| Sly 2 |   PAL (v1.00)    |        0914.1846        | SCES-52529 | FDA1CBF6 |
| Sly 2 |   PAL (v2.01)    |        1006.2123        | SCES-52529 | 15DD1F6F |
| Sly 2 |      NTSC-J      |        0121.1144        | SCPS-15090 | 615EA2DB |
| Sly 2 |      NTSC-K      |        1221.1745        | SCKA-20044 | 518DD841 |
| Sly 2 |   NTSC E3 Demo   |        0411.1757        | SCUS-97415 | 5B93397F |
| Sly 2 | PAL September 11 |        0911.1830        | SCES-52529 | B89723F2 |
| Sly 2 |  NTSC March 17   |        0317.1405        | SCUS-97198 | DD0B5E6C |
| Sly 2 |   NTSC July 11   |        0711.1656        | SCUS-97316 | A480549C |
| Sly 3 |       NTSC       |        0828.0212        | SCUS-97464 | 8BC95883 |
| Sly 3 |       PAL        |        0921.1843        | SCES-53409 | 8164C614 |
| Sly 3 |      NTSC-K      |        1112.1525        | SCKA-20063 | A8CC1583 |
| Sly 3 |   NTSC E3 Demo   |        0418.1711        | SCUS-97484 | 3130A4D3 |
| Sly 3 |     PAL Demo     |        0906.1452        | SCED-53802 | BAE3B5E9 |
| Sly 3 |   NTSC July 16   |        0716.1854        | SCUS-97464 | 0190CF8B |
| Sly 3 |   PAL August 2   |        0802.0136        | SCES-52529 | 8C146034 |
| Sly 3 | PAL September 2  |        0902.1747        | SCES-53409 | 3670B6F9 |

## How to run / Troubleshooting
**IMPORTANT:** *.NET Desktop Runtime 9.0 x64* must be installed in order to run this program.<br>
https://dotnet.microsoft.com/en-us/download/dotnet/9.0

- ***The application may need to be ran as administrator***
- The application automatically detects the PCSX2 process and the game build
- Controls appear and disappear depending on the build that is being detected
  - For example, for all Sly 1 builds a "Lucky charms" combobox is present, but it is not present for all Sly 2 and Sly 3 builds
  - This also applies to builds of the same game: for example, a checkbox control is present for the build `Sly 2 NTSC` which toggles the guards AI but it is not present for `Sly 2 NTSC E3 Demo` <br/>

## Features
- Load maps
- Warp to pre-defined locations
- Edit coins
- Edit and freeze the camera's FOV
- Edit and freeze the camera's draw distance
- Edit and freeze the game's clock
- Edit and freeze health/lives
- Edit and freeze the current character's coordinates
- Fly mode
  - Press the "Fly" button to enable and press it again to disable
  - Hold cross to go up, hold circle to go down and hold L1 to accelerate
- A trackbar is present to edit the amount to add to the coordinates when pressing the "-" and "+" buttons. The trackbar is also used for the speed when the fly mode is enabled
- Toggle all gadgets/thief moves
- Infinite gadget power
- Disable guards AI
- Invulnerability
- Undetectability
- Infinite double jump
- Disable death barriers
- Skip current dialogue
- Sly 1: World states tab
  <img src="SlyMultiTrainer/Img/Sly1_World.png">
  - 5 sub-tabs for each world
  - Edit each world's "Started", "1 key collected", "3 keys collected" and "7 keys collected" flags
  - Edit each world's "Keys collected", "Safes opened" and "Sprints completed" values
  - Edit each level's "Unlocked", "Key", "Safe" and "Sprint" flags
  - World 1
    - Edit "Treasure in the Depths - Chests"
  - World 2
    - Edit "At the Dog Track - Nitros"
    - Edit "At the Dog Track - Laps"
  - World 3
    - Edit "Piranha Lake - Fish"
    - Edit "Piranha Lake - Torch"
    - Edit "Down Home Cooking - Chicken"
  - World 4
    - Edit "A Desperate Race - Nitros"
    - Edit "A Desperate Race - Laps"
  - World 5
    - Edit "Burning Rubber - Fire slugs computer"
    - Edit "Burning Rubber - Computer"
    - Edit "Bentley Comes Through - Chip"
- Sly 2 & 3: FK$X objects viewer tab
  <img src="SlyMultiTrainer/Img/Sly2_Entities.png">
  <img src="SlyMultiTrainer/Img/Sly3_Entities.png">
  - Populated on map change
  - "Refresh" button to force reading the game's memory again to parse the FK$X objects list
  - When a node is selected:
    - The address of the selected entity is shown and is able to be copied to the clipboard
    - Depending on the entity's properties, it might be possible to edit its local transformation (position, rotation and scale)
    - Show world (final) position
    - Warp the active character to the selected entity
- Sly 2 & 3: Interactive DAG viewer tab
  <img src="SlyMultiTrainer/Img/Sly2_DAG_1.png">
  <img src="SlyMultiTrainer/Img/Sly2_DAG_2.png">
  <img src="SlyMultiTrainer/Img/Sly3_DAG_1.png">
  <img src="SlyMultiTrainer/Img/Sly3_DAG_2.png">
  - Populated on map change
  - Might take some moments to show
  - Mouse wheel to zoom-in and zoom-out
  - Hold left click and drag to pan
  - Right click on the graph
    - Shows the number of tasks parsed
    - Go to root: goes to the root node of the DAG
    - Restore pan and zoom: restores the user's transformation to fit the current DAG's size
    - Settings (requires refresh)
      - Lock pan and zoom on refresh: if checked, on map change or if the user clicks "Refresh", the viewer will not assume the default graph's transformation
      - Show name: show the node's name
      - Show address: show the node's address
      - Show id: show the node's id
      - Show id as decimal: show the node's id as decimal instead of hexadecimal
    - Refresh: force reading the game's memory again to parse the DAG
    - Save as png...: save the graph as a png file
  - Right click on a task
    - Set state to
      - Unavailable, Available, Complete or Final
    - Copy (to clipboard)
      - Address
      - Id
      - Name
      - Focus count 1
      - Focus count 2
      - Savefile flags address
        - Array of 5 integers, in which
          - The first is "Focus count 1" (copied to the task's struct on map loading)
          - The second is "Focus count 2" (copied to the task's struct on map loading)
          - The third is unknown
          - The fourth is the state (copied to the task's struct on map loading)
          - The fifth is unknown
      - Goal description (only if the node has a goal description)
      - Entrance value (only if the node is a checkpoint)
    - If the node is a checkpoint there are 3 more options:
      - Set as current checkpoint
      - Load to checkpoint
      - Load to checkpoint with zero focus
      - Load to this entrance location
  - Right click on a job
    - Set state to
      - Unavailable, Available, Complete or Final
    - Load job
    - Load job with zero focus
    - Suck value (click to edit)
    - Copy
      - Address
      - Id
      - Name
      - Description
      - Savefile flags address
        - Array of 2 integers and 2 floats, in which
          - The first is "attempts" (how many reloads there have been since the start of the job)
          - The second indicates if the job has been completed
          - The third is the playtime
          - The fourth is the suck value
- "About" tab
  - Credits
  - A button to re-initialize the application. This has the same effect as closing and opening the application
  - Addresses list used for the current build

## TO-DO
- General
  - Custom warps
  - Custom hotkeys
  - Dark mode
  - Tab index
  - RPCS3 support
  - Empty slot in the maps combobox. If selected, it should load the current map
  - Hide HUD
- Sly 1
  - Disable guards AI
  - Invulnerability
  - Undetectability
  - Fly accelarate forward
- Sly 1 NTSC Demo
  - Fov
- Sly 2
  - Skip dialogue
- Sly 2 NTSC E3 Demo
  - Disable guards AI
  - Invulnerability
  - Undetectability
- Sly 2 NTSC March 17
  - Invulnerability
  - Undetectability
- Sly 2 NTSC July 11
  - Invulnerability
  - Undetectability
  - Gadget binds
- Sly 3
  - Skip dialogue
  - Other characters in the "characters" combobox
  - Reload without 3D Screen
- Sly 3 NTSC E3 Demo
  - Disable death barriers
- Entities
  - Rotation doesn't always work
  - Search bar
  - "Warp active character to entity" logic does not take in consideration if the entity did not spawn at 0,0,0
  - "Warp entity to active character" button
- DAG
  - Find a flag to indicate that a task is a chalktalk (to apply the octagon shape). Right now it uses a regex pattern
  - Progress bar when loading?
  - Some edges should be straight but aren't (sly 2 ntsc ep1, satellite sabotage into breaking and entering)
  - Cluster's label should be center aligned
  - Graph->Settings without refreshing (this works for nodes, but msagl doesn't have a way to resize the cluster to the new node sizes, which would also mean extend or shrink the edges)
  - Let the user change values of a node (map id, entity id, etc...)
  - Load jobs for Version 0 (`Sly 2 NTSC March 17` and `Sly 2 NTSC E3 Demo`)
  - "Load to this entrance location" for Version 0
  - Search node function (search by address, id, etc...)
  - For Sly 2, the option "Load to this entrance location" could actually be warping without reloading. The entrance value of a checkpoint is the id (the field at +0x18) of an entrance struct. For Sly 2 NTSC, the first entrance struct is pointed at by the address 0x3E06C0 (calculated from 3DD490 + 3038 + 2A * C). <br>
    Function stack: address 0x3E06C0 calculated by the function at 15E3A8 called from 15EB74. Instruction at 15EBB8 stores to the stack an array of entrance struct pointers, which was requested by the instruction at address 22803C. The rest of the function goes through the array of entrance structs and checks if the field at +0x18 is the same as the requested entrance value. When found, the transformation matrix is at +0x60

## Known issues
- DAG
  - NOTE: The below can be fixed by setting the address "ClusterIdAddress + 4" to -1 instead of "targetTask.Id". 
  - Sly 2
    - Loading the task in Episode 1 "t1_bug_intro" without focus will try to play the binocucom dialogue immediately (and also, failing to do so)
    - Loading the task in Episode 2 "t2_steal_tuxedo_intro" will not play the binocucom dialogue after walking inside the hotel
  - Sly 3
    - Loading the task in Episode 4 "t4_van_defend_pkturret_intro" with the Grapple-Cam gadget already acquired will make the gang's van not appear

## Build instructions
Sly Multi Trainer uses a [modified version of memory.dll](https://github.com/NiV-L-A/memory.dll) to facilitate the development of the program.<br>
The modifications add additional data types for read and write operations and a custom way to deal with addresses, offsets and pointers.

1. Clone the SlyMultiTrainer repository by clicking on the "Code" button and selecting "Open with Visual Studio"
2. Download the memory.dll source code from [this fork of memory.dll](https://github.com/NiV-L-A/memory.dll) by clicking on the "Code" button and selecting "Download ZIP"
3. Extract the memory.dll-master.zip archive
4. In Visual Studio, in the Solution Explorer window, right click on the SlyMultiTrainer solution, select "Add" and then "Existing project..."
5. Select the "Memory.csproj" file from the extracted memory.dll-master.zip archive
6. In the Solution Explorer window, expand the SlyMultiTrainer project entry, right click on "Dependencies" and select "Add Project Reference..."
7. On the left side select "Projects", add the "Memory" item and make sure its checkbox is checked. Click OK
8. Visual Studio should recognize the file and the project should accept the "Memory" namespace
9. Run the application by clicking on "Debug" and then "Start Debugging"
  - Do note that debug symbols are not emitted when in "Release" mode. To actually debug the application (e.g. set breakpoints) make sure to be in "Debug" mode

## Release workflow
1. In the Solution Explorer window, right click on the SlyMultiTrainer project entry, go to "Properties", "Package" and increase the "Package Version"
2. Run the application and make sure the window's title reflects the new package version
3. Remove unused using directives
4. Change changelog
5. In the Solution Explorer window, right click on the SlyMultiTrainer project entry and select "Publish". Use the following settings, then click "Publish":
<img src="SlyMultiTrainer/Img/publish_settings.png">

## Credits
- NiV-L-A
- TheOnlyZac
- Fr4nk0
- SlyCooperReloadCoded
- Sly Cooper Modding Discord Server: https://discord.gg/2GSXcEzPJA
- memory.dll: https://github.com/NiV-L-A/memory.dll
- Microsoft Automatic Graph Layout: https://github.com/microsoft/automatic-graph-layout