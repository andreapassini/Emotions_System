# AI4Vid_Project

Esploration of chapters 5.5 e 5.6 of Millingotn's book, on Fuzzy Logic e Markov System.


In a 3D environment, 2 groups of agents have to fioght each other.
Each with a base where they can get healed.


Agent's Beahviour on Multiple Levels:

1 - Emotions ( FSM )

2 - Behaviour while feelin that emotion ( DT )

3 - Action descriprion ( BT )

	
Pathfindong: NavMesh


## Livello 1 - Emotions:

The emotion is represented by a single value: emotionsValue.
Its different value will determined the emotion that the agent is feeling.

FSM

States:

  InRage
  Brave
  Normal
  Shy
  Scared
  
Increment EmotionValue:

  Markov System

Emotion Evaluation:

  Fuzzy Logic:

  emotionsValue is used as a membership degree and feed to a probabilistic evaluator.


## Lv2 2 - Behaviour while feelin that emotion :

DT

One for each FSM States - Emotions

### InRage

![DTRage](https://user-images.githubusercontent.com/71270277/147860208-3a22f81c-1b0a-4faf-9176-a7a048dd8c12.png)

### Brave
![DTBrave](https://user-images.githubusercontent.com/71270277/147860153-ec6322ac-6f4b-4bb3-b2cb-e3e71447bec1.png)


## Livello 3 - Action description:

BT


## Collaboration and Coordination:


### At Lvl 1 (emotions):

Whem the agent is feeling an extreme emotion (InRage or Scared),
it will affects also nearby agents, increasing o decresing their emotion value.


Potrei anche introdurre un cambiamento della emotionsValue a seconda del numero e dello stato di salute degli alleati circostanti.



### At Lvl 2:


Coordinamento su un target: 
  Nel momento in cui, un agente senza target, ne individuasse uno, dovrebbe comunicarlo anche agli alleati circostanti, i quali a loro volta ripeterebbero la trasmissione.

Coordinamento nell'uscita dalla base: 
  Nel momento in cui un agente sia in una delle basi a curarsi, prima di uscire dovrebbe attendere che un certo numero di alleati sia vicino a lui ed in salute.
