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

The emotion is represented by a single value: `emotionsValue`.
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

![DTInRage](https://user-images.githubusercontent.com/71270277/148593728-540068e6-7c67-4c60-864c-e0f27b5df66d.png)

### Brave

![DTBrave](https://user-images.githubusercontent.com/71270277/148593722-ec86960a-1e75-42ff-ace9-e7cedb908197.png)

### Normal

![DTNormal](https://user-images.githubusercontent.com/71270277/148593720-b5a03cfb-8ea5-410a-99af-c272ac2f2d39.png)

### Shy

![DTShy](https://user-images.githubusercontent.com/71270277/148593719-56b91b90-023b-40a4-a919-6bd377730303.png)

### Scared

![DTScared](https://user-images.githubusercontent.com/71270277/148541267-de24ac51-ab99-4772-8d29-bad781eb49ef.jpg)


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
