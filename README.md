# AI4Vid_Project

Esplorare il capitolo 5.5 e 5.6 del libro, su Fuzzy Logic e Markov System.


In un ambiente 3D, degli agenti hanno il compito di individuare e distruggere un target. 
Nella mappa sono situate delle basi in cui gli agenti possono curarsi.


Comportamento agenti su Livelli:

1 - Emozioni ( FSM )

2 - Comportamento in quell'emozione ( DT )

3 - Descrizione azione ( BT )

	
Pathfindong: NavMesh


## Livello 1 - Emozioni:

FSM

States:

  Rage
  Brave
  Normal
  Suspicious
  Fear
  
Transitions:

  Markov System
  
  Condizione:

  Le emozioni sono descritte da una singola variabile (emotionsValue) incrementata o decrementata a seconda degli eventi, con un DT.

  Fuzzy Logic:

  Membership degree, ricavata dal valore della variabile emotionsValue


## Livello 2 - Comportamento delle singole emozioni:

DT

Uno per ogni stato della FSM - Emotions

![DTBrave](https://user-images.githubusercontent.com/71270277/147860153-ec6322ac-6f4b-4bb3-b2cb-e3e71447bec1.png)


## Livello 3 - Descrizione delle Azioni:

BT


Collaborazione e Cordinamento:


A livello 1 (emozioni):

Nel momento in cui un agente provasse un'emozione estrema (fear e rage), 
influenzerebbe anche gli alleati attorno a lui incrementando o decrementando il valore della emotionsValue.

Potrei anche introdurre un cambiamento della emotionsValue a seconda del numero e dello stato di salute degli alleati circostanti.



A livello 2:


Coordinamento su un target: 
  Nel momento in cui, un agente senza target, ne individuasse uno, dovrebbe comunicarlo anche agli alleati circostanti, i quali a loro volta ripeterebbero la trasmissione.

Coordinamento nell'uscita dalla base: 
  Nel momento in cui un agente sia in una delle basi a curarsi, prima di uscire dovrebbe attendere che un certo numero di alleati sia vicino a lui ed in salute.
