# DSC412-project: Mazerunner Speedrun (Reinforcement Learning in Unity)

## Introduction

For my machine-learning (ML) project, I am attempting to create an intelligent avatar that can beat a game I developed in the Unity software called Mazerunner.
This project explores reinforcement learning (RL) in a Bomberman-like environment created using Unity's ML-Agents toolkit.

The underlying motivation for this project is to explore the metaphysical concept of "the present."
In short, as an artificial agent generates as it learns to navigate a virtual task environment the spacetime patterns generated are a subset of the vast possible spatiotemporal arrangments that can arise from a single starting moment.
More information about this incorporation of possibility in a spacetime framework can be found in the part 2 project proposal section.

The remainder of the README.md file contains the ordered project parts.

## Part 2 - Project Proposal

To view the project proposal navigate to the proposal folder and download DSC412_001_FA24_PR_sbrantl.pdf.

## Part 3 - Project Check In

As of 10/24, a week before the check-in date, I have managed to integrate

### Data Generation Process

Unlike supervised learning projects where data is imported and organized from external files, RL generates data through interactions between the agent and the environment. 

The Unity ML-Agent toolkit was used to enable this integration. Setting up the environment involved coding state observations, action spaces, and reward functions to simulate meaningful interactions for the agent. The agent learned from these interactions, which creates the dataset that will be fed back into the learning model.

### Model Training

In reinforcement learning, the agent learns through trial and error by receiving rewards or penalties for its actions in the environment. This project uses the Unity ML-Agent toolkit to train an agent based on a custom reward structure.

#### Key Components:
- **Observation Space:** The agent perceives the environment using Ray Perception Sensors, detecting objects such as walls, bombs, and enemies.
- **Action Space:** The agent decides whether to move, place bombs, or wait based on its perception.
- **Reward Function:** The agent is rewarded for avoiding danger and eliminating obstacles, while penalties are given for unfavorable actions.

Training was done iteratively, with the model continuously improving as it explored more possible scenarios.

### Rubric Items Not Addressed

Due to the nature of reinforcement learning, certain elements of the milestone rubric were not applicable:

1. **Importing Data:** No external data was imported. The data will be generated in real-time by the agent interacting with the environment.
2. **Organizing Data:** The project focuses on continuous data generation during training, as opposed to organizing static datasets.
3. **Train-Test Split:** RL does not use a train-test split, as learning is performed iteratively by interacting with the environment.
4. **Analyzing Data:** Data analysis in RL happens dynamically as part of the agent's feedback loop.

