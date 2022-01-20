#AI4Vid_Project

Decision Making with Markov State Machines.


##Summary of the game’s rules:

In a 3D environment, 2 groups of agents, controlled by AI, have to fight each other. 
Each group has a base where agents can get healed up.
To win, a group needs to kill agents from the other group.


##Summary of the AI:

###The agents will be able to move to a certain destination and avoid objects.
This will be achieved using Navigation Meshes.

###The agents will be able to take decisions on which actions will be executed next.
This will be achieved using different decision making techniques:
####Finite State Machines
####Decision Tree
####Behavior Tree

###To enrich the behavior, agents will be able to:
####Collaborate and Coordinate with other agents of the same group
####Feel Emotions
This will be achieved with 5 emotions (InRage, Brave, Normal, Shy, Scared). Each emotions will be represented in 2 ways:
As values, grouped in an array called State Vector in the Markov State Machines. The values in the array will change when a transition of the Markov State Machine is fired.
As FSM states in order to have a different behavior for each different emotion.
