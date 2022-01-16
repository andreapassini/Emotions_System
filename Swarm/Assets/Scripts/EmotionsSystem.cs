using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionsSystem : MonoBehaviour
{
    #region Variables
    private MarkovSM markovSM;

    private float[] stateVector;

    private float[][] defaultMatrix;

    private float[][] inRageMatrix;
    private float[][] scaredMatrix;
    private float[][] braveMatrix;
    private float[][] shyMatrix;

    private float startTime;
    public float timer = 5f;

    public bool scared = false;
    public bool inRage = false;
    #endregion

    #region Unity Methods

    void Start()
    {
        stateVector = new float[]{
            0.0f,
            0.0f,
            1.0f,
            0.0f,
            0.0f
        };

        TransitionMatrixRageInit();

        MarkovSMState markovSMState = new MarkovSMState(stateVector);

        // Actions
        MarkovSMAction m_a_Default = new MarkovSMAction(ResetTimer);
        MarkovSMAction m_a_ResetInRage = new MarkovSMAction(ResetInRage);
        MarkovSMAction m_a_ResetScared = new MarkovSMAction(ResetScared);

        // Default transition
        MarkovSMTransition m_t_default = new MarkovSMTransition(TimerOff, defaultMatrix);
        m_t_default.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t_default);

        // Feeling inRage
        MarkovSMTransition m_t_FinRage = new MarkovSMTransition(FeelingInRage, inRageMatrix);
        m_t_FinRage.myActions.Add(m_a_Default);
        m_t_FinRage.myActions.Add(m_a_ResetInRage);
        markovSMState.AddTransition(m_t_FinRage);

        // Feeling scared
        MarkovSMTransition m_t_Fscared = new MarkovSMTransition(FeelingScared, scaredMatrix);
        m_t_Fscared.myActions.Add(m_a_Default);
        m_t_Fscared.myActions.Add(m_a_ResetScared);
        markovSMState.AddTransition(m_t_Fscared);

        MarkovSMTransition m_t1 = new MarkovSMTransition(AlliesAround, braveMatrix);
        m_t1.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t1);

        MarkovSMTransition m_t2 = new MarkovSMTransition(EnemiesAround, shyMatrix);
        m_t2.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t2);

        MarkovSMTransition m_t3 = new MarkovSMTransition(GetComponent<Health>().IsHealthLow, shyMatrix);
        m_t3.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t3);

        MarkovSMTransition m_t4 = new MarkovSMTransition(GetComponent<Health>().IsHealthHig, braveMatrix);
        m_t4.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t4);

        MarkovSM markovSM = new MarkovSM(markovSMState);
    }

    void Update()
    {

    }

	#region Matrix
	public void TransitionMatrixDefaultInit()
    {
        // Declare a jagged array.
        defaultMatrix = new float[5][];

        defaultMatrix[0] = new float[5] { 1.0f, 0.1f, 0.1f, 0.1f, 0.1f };
        defaultMatrix[1] = new float[5] { 0.1f, 1.0f, 0.1f, 0.1f, 0.1f };
        defaultMatrix[2] = new float[5] { 0.1f, 0.1f, 1.0f, 0.1f, 0.1f };
        defaultMatrix[3] = new float[5] { 0.1f, 0.1f, 0.1f, 1.0f, 0.1f };
        defaultMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 1.0f };
    }

    public void TransitionMatrixRageInit()
	{
        inRageMatrix = new float[5][];

        inRageMatrix[0] = new float[5] { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f };
        inRageMatrix[1] = new float[5] { 0.1f, 0.7f, 0.7f, 0.0f, 0.0f };
        inRageMatrix[2] = new float[5] { 0.1f, 0.1f, 0.0f, 0.1f, 0.1f };
        inRageMatrix[3] = new float[5] { 0.1f, 0.1f, 0.1f, 0.0f, 0.1f };
        inRageMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.0f };
    }

    public void TransitionMatrixScaredInit()
    {
        scaredMatrix = new float[5][];

        scaredMatrix[0] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
        scaredMatrix[1] = new float[5] { 0.1f, 0.7f, 0.7f, 0.0f, 0.0f };
        scaredMatrix[2] = new float[5] { 0.1f, 0.1f, 0.0f, 0.1f, 0.1f };
        scaredMatrix[3] = new float[5] { 0.1f, 0.1f, 0.1f, 0.0f, 0.1f };
        scaredMatrix[4] = new float[5] { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f };
    }
    #endregion


    #region Conditions
    public bool TimerOff()
	{
        if (Time.time >= startTime + timer) {
            return true;
		}
        return false;
	}

    public bool EnemiesAround()
	{
        int n = 0;
        string enemyTag = GetComponent<DecisionMaker>().enemyTag;
        float sightRange = GetComponent<DecisionMaker>().sightRange;

        // Otherwise i can calculate Angle with every target /in range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies) {
            //Check distance
            if (Vector3.Distance(transform.position, enemy.transform.position) < sightRange) {
                n++;
            }
        }

        if (n >= 3)
            return true;
        return false;
	}

    public bool AlliesAround()
    {
        int n = 0;
        string enemyTag = GetComponent<DecisionMaker>().enemyTag;
        float sightRange = GetComponent<DecisionMaker>().sightRange;

        // Otherwise i can calculate Angle with every target /in range
        GameObject[] allies = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject ally in allies) {
            //Check distance
            if (Vector3.Distance(transform.position, ally.transform.position) < sightRange) {
                n++;
            }
        }

        if (n >= 3)
            return true;
        return false;
    }

    public bool FeelingInRage()
	{
        if (inRage)
            return true;
        return false;
	}

    public bool FeelingScared()
	{
        if (scared)
            return true;
        return false;
	}
	#endregion


	#region Actions
	public void ResetTimer()
	{
        startTime = Time.time;
	}

    public void ResetInRage()
	{
        inRage = false;
	}

    public void ResetScared()
	{
        scared = false;
	}
	#endregion

	#endregion
}
