using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DecisionMaker))]
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

    public float reactionTime = 5f;
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

        MarkovSMState markovSMState = new MarkovSMState(stateVector);

        TransitionMatrixRageInit();
        TransitionMatrixBraveInit();
        TransitionMatrixDefaultInit();
        TransitionMatrixShyInit();
        TransitionMatrixScaredInit();

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

        MarkovSMTransition m_t1 = new MarkovSMTransition(AllyAround, braveMatrix);
        m_t1.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t1);

        MarkovSMTransition m_t2 = new MarkovSMTransition(EnemyAround, shyMatrix);
        m_t2.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t2);

        MarkovSMTransition m_t3 = new MarkovSMTransition(GetComponent<Health>().IsHealthLow, scaredMatrix);
        m_t3.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t3);

        MarkovSMTransition m_t4 = new MarkovSMTransition(GetComponent<Health>().IsHealthHigh, inRageMatrix);
        m_t4.myActions.Add(m_a_Default);
        markovSMState.AddTransition(m_t4);

        markovSM = new MarkovSM(markovSMState);

        StartCoroutine(Patrol());
    }

	void Update()
    {
        
    }

    public IEnumerator Patrol()
    {
        while (true) {
            markovSM.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    #region Matrix
    public void TransitionMatrixDefaultInit()
    {
        // Declare a jagged array.
        defaultMatrix = new float[5][];

        defaultMatrix[0] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
        defaultMatrix[1] = new float[5] { 0.1f, 0.1f, 0.2f, 0.1f, 0.1f };
        defaultMatrix[2] = new float[5] { 0.1f, 0.5f, 1.0f, 0.5f, 0.1f };
        defaultMatrix[3] = new float[5] { 0.1f, 0.1f, 0.2f, 0.1f, 0.1f };
        defaultMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    }

    public void TransitionMatrixRageInit()
    {
        inRageMatrix = new float[5][];

        inRageMatrix[0] = new float[5] { 1.3f, 0.1f, 0.1f, 0.0f, 0.0f };
        inRageMatrix[1] = new float[5] { 0.0f, 1.1f, 0.1f, 0.0f, 0.0f };
        inRageMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
        inRageMatrix[3] = new float[5] { 0.0f, 0.0f, 0.0f, 0.7f, 0.0f };
        inRageMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.6f };
    }

    public void TransitionMatrixBraveInit()
    {
        braveMatrix = new float[5][];

        braveMatrix[0] = new float[5] { 1.1f, 0.1f, 0.0f, 0.0f, 0.0f };
        braveMatrix[1] = new float[5] { 0.1f, 1.3f, 0.1f, 0.0f, 0.0f };
        braveMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
        braveMatrix[3] = new float[5] { 0.0f, 0.0f, 0.0f, 0.7f, 0.0f };
        braveMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.6f };
    }

    public void TransitionMatrixScaredInit()
    {
        scaredMatrix = new float[5][];

        scaredMatrix[0] = new float[5] { 0.6f, 0.0f, 0.0f, 0.0f, 0.0f };
        scaredMatrix[1] = new float[5] { 0.0f, 0.7f, 0.0f, 0.0f, 0.0f };
        scaredMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
        scaredMatrix[3] = new float[5] { 0.0f, 0.0f, 0.1f, 1.3f, 0.1f };
        scaredMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.1f, 1.1f };
    }

    public void TransitionMatrixShyInit()
    {
        shyMatrix = new float[5][];

        shyMatrix[0] = new float[5] { 0.6f, 0.0f, 0.0f, 0.0f, 0.0f };
        shyMatrix[1] = new float[5] { 0.0f, 0.7f, 0.0f, 0.0f, 0.0f };
        shyMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
        shyMatrix[3] = new float[5] { 0.0f, 0.0f, 0.1f, 1.3f, 0.1f };
        shyMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.1f, 1.1f };
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

    public bool EnemyAround()
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

    public bool AllyAround()
    {
        int n = 0;
        
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

	#region Emotions Evaluation
    public bool InRage()
	{
        if (ToPercentage(Total(stateVector), stateVector[0]) >= UnityEngine.Random.Range(0f, 100f))
            return true;
        return false;
	}


    public bool Brave()
	{
        if (ToPercentage(Total(stateVector), stateVector[1]) >= UnityEngine.Random.Range(0f, 100f))
            return true;
        return false;
    }

    public bool Normal()
    {
        if (ToPercentage(Total(stateVector), stateVector[2]) >= UnityEngine.Random.Range(0f, 100f))
            return true;
        return false;
    }

    public bool Shy()
    {
        if (ToPercentage(Total(stateVector), stateVector[3]) >= UnityEngine.Random.Range(0f, 100f))
            return true;
        return false;
    }

    public bool Scared()
    {
        if (ToPercentage(Total(stateVector), stateVector[4]) >= UnityEngine.Random.Range(0f, 100f))
            return true;
        return false;
    }
    #endregion

    public float ToPercentage(float tot, float a)
	{
        float x = (100 * a)/tot;
        return x;
	}

    public float Total(float[] vec)
	{
        float total = 0f;
        
        for(int i = 0; i < 5; i++)
        {
            float a = vec[i];
            total += a;
        }

        return total;
    }
    #endregion
}
