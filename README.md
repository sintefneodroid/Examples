![neodroid](.github/images/header.png)

# Examples

<p align="center" width="100%">
  <a href="https://unity3d.com/">
    <img alt="unity" src=".github/images/unity.svg" height="40" align="left">
  </a>
  <a href="https://docs.microsoft.com/en-us/dotnet/csharp/index">
    <img alt="csharp" src=".github/images/csharp.svg" height="40" align="center">
  </a>
<p>

## Usage

1. ```git clone https://github.com/sintefneodroid/droid```
2. ```git submodule init && git submodule update && cd Assets/droid && git-lfs pull```
3. Open the repo folder as an Unity project

## Demo
<!--![droid](.github/images/neodroid.png)
![lunarlander](.github/images/lunarlander.png)
-->
![manipulator](.github/images/animated.gif)

## Repository Structure
---
<!--    ├  └  ─  │   -->
    sintefneodroid/droid      # This repository
    │
    ├── docs                
    │   ├── source            # Documentation files
    │   │
    │   ├── make.bat          # Compile docs
    │   └── Makefile          # ^
    │       
    ├── Examples              # Prebuilt Neodroid environments
    │   ├── Assets            # Model checkpoints
    │   │   ├── Neodroid      # Symlinked folder to top-level Neodroid folder
    │   │   ├── SceneAssets   # All scene-specific assets for the prebuilt environments
    │   │   └── Scenes        # All prebuilt environment scenes
    │   │
    │   └── Examples.sln      # C# project file
    │
    ├── .github            # Images and such for this README
    │
    ├── Neodroid              # The Neodroid unity package
    │   ├── Prototyping       # All classes for quick prototyping of observations and actions
    │   │   ├── Actors        
    │   │   ├── Evaluation    
    │   │   ├── Observers     
    │   │   ├── Displayers    
    │   │   ├── Configurables
    │   │   └── Motors        
    │   │
    │   ├── Environments      # Classes for encapsulating all Neodroid environments
    │   ├── Managers          # Classes for managing the simulation of Neodroid environments
    │   └── Utilities         # Lots of helper functionalities
    │   
    ├── LICENSE               # License file (Important but boring)
    └── README.md             # The top-level README
---


# Components Of The Neodroid Platform
- [agent](https://github.com/sintefneodroid/agent)
- [neo](https://github.com/sintefneodroid/neo)
- [droid](https://github.com/sintefneodroid/droid)
