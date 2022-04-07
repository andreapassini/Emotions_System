using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


// Defer function to trigger activation condition
// Returns true when transition can fire
public delegate bool MarkovSMCondition();

// Defer function to perform action
public delegate void MarkovSMAction();

public class MarkovSMTransition
{
    // The method to evaluate if the transition is ready to fire
    public MarkovSMCondition myCondition;

	// Matrix
	public float[][] myMatrix;

    // A list of actions to perform when this transition fires
    public List<MarkovSMAction> myActions = new List<MarkovSMAction>();

    public MarkovSMTransition(MarkovSMCondition condition, float[][] matrix, MarkovSMAction[] actions = null)
    {
		myMatrix = matrix;
        myCondition = condition;
        if (actions != null) myActions.AddRange(actions);
    }

    // Call all  actions
    public void Fire()
    {
        foreach (MarkovSMAction action in myActions) action();
    }
}

public class MarkovSMState
{
    // A dictionary of transitions and the states they are leading to
    private List<MarkovSMTransition> links;

	public float[] myStateVector;

	public MarkovSMState(float[] stateVector)
	{
		links = new List<MarkovSMTransition>();
		myStateVector = new float[5];
		myStateVector = stateVector;
	}

	public void AddTransition(MarkovSMTransition transition)
	{
		links.Add(transition);
	}

	public MarkovSMTransition VerifyTransitions()
	{
		foreach (MarkovSMTransition t in links) {
			if (t.myCondition()) return t;
		}
		return null;
	}
}

public class MarkovSM
{
	public MarkovSMState current; // Current state

	public MarkovSM(MarkovSMState state)
	{
		current = state;
	}

	public void Update()
	{ // NOTE: this is NOT a MonoBehaviour
		MarkovSMTransition transition = current.VerifyTransitions();
		if (transition != null) {

			PrintMatrix(transition.myMatrix);

			// (1) Multiply the matrix of that transition with the state vector
			current.myStateVector = Multiply(transition.myMatrix, current.myStateVector);

			// (2) Execute the actions associated with the transition
			transition.Fire(); 
		}
	}

	public float[] Multiply(float[][] matrix, float[] _stateVector)
	{
		float[] vector = new float[5];
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				vector[i] += matrix[i][j] * _stateVector[j];
			}
		}
		return vector;
	}

	public void PrintMatrix(float[][] m)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("Transition's Matrix");
		sb.AppendLine();

		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				sb.Append(m[i][j]);
				sb.Append(' ');
			}
			sb.AppendLine();

		}

		Debug.Log("\n" + sb.ToString());
	}
}

