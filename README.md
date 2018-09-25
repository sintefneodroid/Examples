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

Requires a version of Unity >= 2018.2.0f2 (Due to a unity package manager functionality)

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
    sintefneodroid/Examples      # This repository
    │
    ├── Packages
    │   └── droid          # ^
    │
    ├── Assets              # Prebuilt Neodroid environments
    │   ├── SceneAssets            # All scene-specific assets for the prebuilt environments
    │   └── Scenes        # All prebuilt environment scenes
    │
    ├── .github            # Images and such for this README
    │
    ├── Presets              # The Neodroid unity package
    │   ├── Environments      # Classes for encapsulating all Neodroid environments
    │   ├── Managers          # Classes for managing the simulation of Neodroid environments
    │   └── Utilities         # Lots of helper functionalities
    │
    ├── LICENSE               # License file (Important but boring)
    └── README.md             # The top-level README
---

# Citation

For citation you may use the following bibtex entry:
````
@misc{neodroid-examples,
  author = {Heider, Christian},
  title = {Neodroid Platform Examples},
  year = {2018},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/sintefneodroid/Examples}},
}
````

# Components Of The Neodroid Platform
- [agent](https://github.com/sintefneodroid/agent)
- [neo](https://github.com/sintefneodroid/neo)
- [droid](https://github.com/sintefneodroid/droid)
