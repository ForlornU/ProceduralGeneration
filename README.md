# Procedural Generation in Unity3D: Voxel and Octree Implementation

This project demonstrates the use of procedural generation techniques to create a voxel-based environment using a random walk algorithm, with the generated voxels then organized and stored in an octree structure.

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## Introduction
Procedural generation is a method of creating data algorithmically rather than manually, often used in game development for generating content such as landscapes, dungeons, and entire worlds. This project focuses on:
- Generating voxels using a random walk algorithm.
- Organizing and optimizing voxel storage with an octree data structure.

## Features
- **Random Walk**: Generate caves or floating islands using a random walk algorithm.
- **Voxel Generation**: Instead of manually instantiating meshes or similar, voxels are generated to act as geometry
- **Octree Implementation**: Efficiently manage and query voxel data with an octree.

## Installation
1. **Clone the repository:**
2. **Open the project in Unity:**
   - Open Unity Hub.
   - Click on `Add` and navigate to the cloned project folder.
   - Select the folder and open the project.

## Usage
Once the project is set up in Unity, you can run the scene and see the procedural generation in action.
**Select Scene**:
   - Inside the scenes folder select either Inside or Outside.
   - Inside is for caverns, outside is for floating island generation
**Adjust Parameters**:
   - Depending on the scene you selected, locate the `Inside.asset` or the 'Outside.asset' of type Generation Settings
   - Adjust parameters such as `voxelsToCreate`, `noise`, `inflationPasses`, and `startBlockSize` in the Inspector to see different generation results.

### Key Scripts
- **VoxelGenerator.cs**: Manages voxel generation and random walking
- **Octree.cs**: Defines the octree data structure
- **OctreeNode.cs**: Each node in the octree and their logic

## Contributing
Contributions are welcome! If you have any ideas, suggestions, or improvements, feel free to fork the repository and submit a pull request.
