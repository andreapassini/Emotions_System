using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionsSystem : MonoBehaviour
{
    #region Variables
    float[] stateVector;

    float[][] defaultMatrix;

    float[][] rageMatrix;
    #endregion

    #region Unity Methods

    void Start()
    {
        stateVector = new float[]{
            0.1f,
            0.2f,
            1.0f,
            0.2f,
            0.1f
        };

        Debug.Log(stateVector[0]);

        // Declare a jagged array.
        defaultMatrix = new float[5][];

        defaultMatrix[0] = new float[5] { 1.0f, 0.1f, 0.1f, 0.1f, 0.1f };
        defaultMatrix[1] = new float[5] { 0.1f, 1.0f, 0.1f, 0.1f, 0.1f };
        defaultMatrix[2] = new float[5] { 0.1f, 0.1f, 1.0f, 0.1f, 0.1f };
        defaultMatrix[3] = new float[5] { 0.1f, 0.1f, 0.1f, 1.0f, 0.1f };
        defaultMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 1.0f };

        TransitionMatrixRageInit();

        Multiply(rageMatrix);

        Debug.Log(stateVector[0]);
    }

    void Update()
    {
    }

    public void Multiply(float[][] _matrix)
	{
        float[] vector = new float[5];

        for (int i=0; i < 5; i++) {
            for(int j=0; j<5; j++) {
                vector[i] += _matrix[i][j] * stateVector[j];
			}
            Debug.Log(i + " - " + vector[i]);
        }

        stateVector = vector;
	}

    public void TransitionMatrixRageInit()
	{
        rageMatrix = new float[5][];

        rageMatrix[0] = new float[5] { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f };
        rageMatrix[1] = new float[5] { 0.1f, 0.7f, 0.7f, 0.0f, 0.0f };
        rageMatrix[2] = new float[5] { 0.1f, 0.1f, 0.0f, 0.1f, 0.1f };
        rageMatrix[3] = new float[5] { 0.1f, 0.1f, 0.1f, 0.0f, 0.1f };
        rageMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.0f };
    }

    #endregion
}
