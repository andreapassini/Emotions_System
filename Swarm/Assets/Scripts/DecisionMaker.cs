using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    #region Variables
    public float reactionTime = 3f; // AI FRAME
    private FSM fsm;

    private DecisionTree dt_Normal;
    private DecisionTree dt_Shy;
    private DecisionTree dt_Brave;
    private DecisionTree dt_InRage;
    private DecisionTree dt_Scared;

    #endregion

    #region Unity Methods

    void Start()
    {
		#region FSM
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

		#endregion


		#region DT

		// Actions
		DTAction a_chase = new DTAction(Chase);
        DTAction a_search = new DTAction(Search);
        DTAction a_runAway = new DTAction(RunAway);
        DTAction a_heal = new DTAction(Heal);
        DTAction a_regroup = new DTAction(Regroup);
        DTAction a_attack = new DTAction(Attack);

        #region DT Brave

        // DT Brave
        DTDecision dbrave_1 = new DTDecision(IsHealthLow);
        DTDecision dbrave_2 = new DTDecision(IsTargetAquired);
        DTDecision dbrave_3 = new DTDecision(IsTargetAquired);
        DTDecision dbrave_4 = new DTDecision(IsTargetNear);
        DTDecision dbrave_5 = new DTDecision(IsTargetNear);
        DTDecision dbrave_6 = new DTDecision(IsTargetInLineOfSight);

        dbrave_1.AddLink(false, dbrave_2);
        dbrave_1.AddLink(true, dbrave_3);

        dbrave_2.AddLink(false, a_search);
        dbrave_2.AddLink(true, dbrave_4);

        dbrave_3.AddLink(false, a_heal);
        dbrave_3.AddLink(true, dbrave_5);

        dbrave_4.AddLink(false, a_chase);
        dbrave_4.AddLink(true, dbrave_6);

        dbrave_5.AddLink(false, a_runAway);
        dbrave_5.AddLink(true, a_heal);

        dbrave_6.AddLink(false, a_chase);
        dbrave_6.AddLink(true, a_attack);

        dt_Brave = new DecisionTree(dbrave_1);
        #endregion

        #region DT Shy
        DTDecision dshy_1 = new DTDecision(IsHealthHigh);
        DTDecision dshy_2 = new DTDecision(IsTargetAquired);
        DTDecision dshy_3 = new DTDecision(IsTargetAquired);
        DTDecision dshy_4 = new DTDecision(IsTargetNear);
        DTDecision dshy_5 = new DTDecision(IsTargetNear);

        dshy_1.AddLink(false, dshy_2);
        dshy_1.AddLink(true, dshy_3);

        dshy_2.AddLink(false, a_heal);
        dshy_2.AddLink(true, dshy_4);

        dshy_3.AddLink(false, a_regroup);
        dshy_3.AddLink(true, dshy_5);

        dshy_4.AddLink(false, a_heal);
        dshy_4.AddLink(true, a_runAway);

        dshy_5.AddLink(false, a_regroup);
        dshy_5.AddLink(true, a_runAway);

        dt_Shy = new DecisionTree(dshy_1);
        #endregion

        #region DT Rage
        DTDecision drage_1 = new DTDecision(IsTargetAquired);
        DTDecision drage_2 = new DTDecision(IsTargetNear);
        DTDecision drage_3 = new DTDecision(IsTargetInLineOfSight);

        drage_1.AddLink(false, a_search);
        drage_1.AddLink(true, drage_2);

        drage_2.AddLink(false, a_chase);
        drage_2.AddLink(true, drage_3);

        drage_3.AddLink(false, a_chase);
        drage_3.AddLink(true, a_attack);

        dt_InRage = new DecisionTree(drage_1);
        #endregion

        #region DT Scared
        dt_Shy = new DecisionTree(a_runAway);

		#endregion

		#endregion


		#region BT



		#endregion

		// Start monitoring FSM
		StartCoroutine(Patrol());
    }

    void Update()
    {
        
    }

    // FSM Activity
    public void WalkDTNormal()
	{
        // Start patroling
        StartCoroutine(PatrolDTNormal());
    }

    public void WalkDTBrave()
	{
        // Start patroling
        StartCoroutine(PatrolDTBrave());
    }

    public void WalkDTInRage()
    {
        StartCoroutine(PatrolDTInRage());
    }

    public void WalkDTShy()
    {
        StartCoroutine(PatrolDTShy());
    }

    public void WalkDTScared()
    {
        StartCoroutine(PatrolDTScared());
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

    // Periodic update, run forever
    public IEnumerator Patrol()
    {
        while (true) {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTNormal()
    {
        while (true) {
            dt_Normal.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTBrave()
    {
        while (true) {
            dt_Brave.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTShy()
    {
        while (true) {
            dt_Shy.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTInRage()
    {
        while (true) {
            dt_InRage.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTScared()
    {
        while (true) {
            dt_Scared.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // DT Actions
    public object Chase(object o)
    {
        return null;
    }

    public object Search(object o)
    {
        return null;
    }

    public object RunAway(object o)
    {
        return null;
    }

    public object Heal(object o)
    {
        return null;
    }

    public object Regroup(object o)
    {
        return null;
    }

    public object Attack(object o)
	{
        return null;
	}

    public object IsHealthLow(object o)
    {
        return false;
    }

    public object IsTargetAquired(object o)
	{
        return false;
	}

    public object IsTargetNear(object o)
    {
        return false;
    }

    public object IsTargetInLineOfSight(object o)
    {
        return false;
    }

    public object IsHealthHigh(object o)
	{
        return false;
	}
    #endregion
}
