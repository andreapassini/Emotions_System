using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    #region Variables
    public float reactionTime = 3f; // AI FRAME
    private FSM fsm;
    #endregion

    #region Unity Methods

    void Start()
    {
        //Init FSM
        FSMState normal = new FSMState();
        normal.stayActions.Add(WalkDTNormal);

        FSMState brave = new FSMState();
        brave.stayActions.Add(WalkDTBrave);

        FSMState inRage = new FSMState();
        inRage.stayActions.Add(WalkDTInRage);

        FSMState shy = new FSMState();
        shy.stayActions.Add(WalkDTShy);

        FSMState scared = new FSMState();
        scared.stayActions.Add(WalkDTScared);

        // FSM Transitions
        FSMTransition t1 = new FSMTransition(IsBrave); // Normal to Brave
        FSMTransition t2 = new FSMTransition(IsShy); // NOrmal to Shy

        FSMTransition t3 = new FSMTransition(IsNormal); // Brave to Normal
        FSMTransition t4 = new FSMTransition(IsNormal); // Shy to Normal
        FSMTransition t5 = new FSMTransition(IsInRage); // Brave to Rage
        FSMTransition t6 = new FSMTransition(IsScared); // Shy to Scared

        FSMTransition t7 = new FSMTransition(IsShy); // Scared to Shy
        FSMTransition t8 = new FSMTransition(IsBrave); // Rage to Brave  

        normal.AddTransition(t1, brave);
        normal.AddTransition(t2, shy);

        brave.AddTransition(t3, normal);
        brave.AddTransition(t5, inRage);
        
        shy.AddTransition(t4, normal);
        shy.AddTransition(t6, scared);

        inRage.AddTransition(t8, brave);

        scared.AddTransition(t7, shy);

        // Setup a FSA at initial state
        fsm = new FSM(normal);

        //Init DT

        //Init BT


    }

    void Update()
    {
        
    }

    // FSM Activity
    public void WalkDTNormal()
	{

	}

    public void WalkDTBrave()
	{

	}

    public void WalkDTInRage()
    {

    }

    public void WalkDTShy()
    {

    }

    public void WalkDTScared()
    {

    }

    // FSM Transitions
    public bool IsBrave()
    {
        return false;
    }

    public bool IsNormal()
    {
        return false;
    }

    public bool IsShy()
    {
        return false;
    }

    public bool IsInRage()
    {
        return false;
    }

    public bool IsScared()
    {
        return false;
    }
    #endregion
}
