using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    #region Variables
    public string enemyTag;

    public float reactionTime = 3f; // AI FRAME
    private FSM fsm;

    private DecisionTree dt_Normal;
    private DecisionTree dt_Shy;
    private DecisionTree dt_Brave;
    private DecisionTree dt_InRage;
    private DecisionTree dt_Scared;

    
    // Internal knowledge
    public int healthHigh = 70;
    public int healthLow = 30;

    //External knowledge
    private Transform target;
    public float targetNear_range = 3.5f;
    public int targetHealthLow;

    
    public Transform enemyBase;
    public float sightRange = 50f;
    public float sightAngle = 45f;

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
        DTAction a_runaway = new DTAction(Runaway);
        DTAction a_heal = new DTAction(Heal);
        DTAction a_regroup = new DTAction(Regroup);
        DTAction a_attack = new DTAction(Attack);
        DTAction a_goToEnemyBase = new DTAction(GoToEnemyBase);

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

        dbrave_5.AddLink(false, a_runaway);
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
        dshy_4.AddLink(true, a_runaway);

        dshy_5.AddLink(false, a_regroup);
        dshy_5.AddLink(true, a_runaway);

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
        dt_Shy = new DecisionTree(a_runaway);

        #endregion

        #region DT Normal

        DTDecision dnormal_1 = new DTDecision(IsTargetAquired);
        DTDecision dnormal_2 = new DTDecision(IsHealthHigh);
        DTDecision dnormal_3 = new DTDecision(IsHealthLow);
        DTDecision dnormal_4 = new DTDecision(IsTargetNear);
        DTDecision dnormal_5 = new DTDecision(IsHealthLow);
        DTDecision dnormal_6 = new DTDecision(IsHealthHigh);
        DTDecision dnormal_7 = new DTDecision(IsHealthHigh);
        DTDecision dnormal_8 = new DTDecision(IsHealthLow);
        DTDecision dnormal_9 = new DTDecision(IsTargetInLineOfSight);
        DTDecision dnormal_10 = new DTDecision(IsTargetHealthLow);
        DTDecision dnormal_11 = new DTDecision(IsTargetInLineOfSight);

        dnormal_1.AddLink(false, dnormal_2);
        dnormal_2.AddLink(false, dnormal_3);
        dnormal_2.AddLink(true, a_goToEnemyBase);
        dnormal_3.AddLink(false, a_regroup);
        dnormal_3.AddLink(true, a_heal);

        dnormal_1.AddLink(true, dnormal_4);
        dnormal_4.AddLink(false, dnormal_5);
        dnormal_4.AddLink(true, dnormal_6);
        dnormal_5.AddLink(false, dnormal_7);
        dnormal_5.AddLink(true, a_heal);
        dnormal_6.AddLink(false, dnormal_8);
        dnormal_6.AddLink(true, dnormal_9);
        dnormal_7.AddLink(false, a_regroup);
        dnormal_7.AddLink(true, a_chase);
        dnormal_8.AddLink(false, dnormal_10);
        dnormal_8.AddLink(true, a_runaway);
        dnormal_9.AddLink(false, a_chase);
        dnormal_9.AddLink(true, a_attack);
        dnormal_10.AddLink(false, a_regroup);
        dnormal_10.AddLink(true, dnormal_11);
        dnormal_11.AddLink(false, a_chase);
        dnormal_11.AddLink(true, a_attack);

        dt_Normal = new DecisionTree(dnormal_1);
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

    public object Runaway(object o)
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

    public object GoToEnemyBase(object o)
	{
        return null;
	}

    public object IsHealthLow(object o)
    {
        if(transform.GetComponent<Health>().GetHealth() <= healthLow) {
            return true;
		}
        return false;
    }

    public object IsTargetAquired(object o)
	{
        if(target != enemyBase && target != null) {
            return true;
		} else {
			if (AquireTarget()) {
                return true;
			}
		}
        return false;
	}

    public object IsTargetNear(object o)
    {
        if(target != enemyBase && target != null) {
            if(Vector3.Distance(transform.position, target.position) < targetNear_range) {
                return true;
            }
		} 
        return false;
    }

    public object IsTargetInLineOfSight(object o)
    {
        Vector3 ray = target.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, ray, out hit)) {
            if (hit.transform == target) {
                return true;
            }
        }
        return false;
    }

    public object IsHealthHigh(object o)
	{
        if (transform.GetComponent<Health>().GetHealth() <= healthHigh) {
            return true;
		}
        return false;
	}

    public object IsTargetHealthLow(object o)
	{
        if(target != enemyBase && target != null) {
            if(target.GetComponent<Health>().GetHealth() <= targetHealthLow) {
                return true;
			}
		}
        return null;
	}

    public bool AquireTarget()
	{
		//RaycastHit hit;
		//bool leftHit = Physics.BoxCast(transform.position,
		//								GetComponent<Collider>().bounds.extents,
		//								Quaternion.Euler(0f, -sightAngle, 0f) * transform.forward,
		//								out hit,
		//								transform.rotation,
		//								sightRange);

		//bool centerHit = Physics.BoxCast(transform.position,
		//								  GetComponent<Collider>().bounds.extents,
		//								  transform.forward,
		//								  out hit,
		//								  transform.rotation,
		//								  sightRange);

		//bool rightHit = Physics.BoxCast(transform.position,
  //                                       GetComponent<Collider>().bounds.extents,
  //                                       Quaternion.Euler(0f, sightAngle, 0f) * transform.forward,
  //                                       out hit,
  //                                       transform.rotation,
  //                                       sightRange);



        // Otherwise i can calculate Angle with every target /in range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach(GameObject enemy in enemies) {
            //Check distance
            if(Vector3.Distance(transform.position, enemy.transform.position) < sightRange) {

                //Check the angle
                if(Vector3.Angle(transform.position, enemy.transform.position) < sightAngle) {

                    //Check if in line of sight
                    Vector3 ray = target.position - transform.position;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, ray, out hit)) {
                        if (hit.transform.GetComponent<GameObject>().tag == enemyTag) {
                            target = enemy.transform;
                            return true;
                        }
                    }
                }
            }
		}

        return false;
	}

	#endregion
}
