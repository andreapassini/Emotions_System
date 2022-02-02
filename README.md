# Emotions

5 emotions:

- InRage
- Brave
- Normal
- Shy
- Scared

each represented by a value, stored in an Array.

## Mutation in the emotions's value

In order to change the value of the array, will use a Markov State Machine characteriez by:

- One state
- Transitions

### One State

One single state defined by the emotions's array, called in stateVector.

### Transition

Transitions are defined by:

- Transition's Matrix
- Contions
- Actions

When a condition is trigger, a transition is fired, the stateVector will be multiplied with the Transition's Matrix and the Actions linked with the transition will be executed.

## Effect of the emotions

The emotion's value will affect the behaviour of the agent.
