using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using CRBT;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EmotionsSystem))]

public class DecisionMaker : MonoBehaviour
{
    #region Variables
    private EmotionsSystem _emotionSystem;

    // AI FRAME
    // i could use different reaction time for each 
    // structure involved
    public float reactionTime = 3f;

    public GameObject bulletPrfab;
    public float attackMeleeRange = 1f;
    public int meleeDmg = 25;
    public float dashForce = 20f;

    private Transform firePoint;

    private FSM fsm;

    private DecisionTree dt_Normal;
    private DecisionTree dt_Shy;
    private DecisionTree dt_Brave;
    private DecisionTree dt_InRage;
    private DecisionTree dt_Scared;

    private BehaviorTree bt_Heal;
    private BehaviorTree bt_Regroup;
    private BehaviorTree bt_Attack;
    private BehaviorTree bt_Runaway;


    // Internal knowledge
    [Space]
    public float sightRange = 50f;
    public float sightAngle = 45f;

    // External knowledge
    [Space]
    private Transform target;
    public float targetNear_range = 50f;
    public int targetHealthLow;
    public string enemyTag = "B";
    public Transform allyBase;
    public Transform enemyBase;
    public float searchRange = 100f;

    private bool isDashing;
    private float speed;
    private Vector3 velocity;
    private float acceleration;
    public float dashTime = 100f;
    public float dashSpeed = 1.1f;

    private Transform startingDashPoint;
    private Transform allyPosition;

    [Space]
    private MeshRenderer _meshRenderer;

    public Material MaterialScare;
    public Material MaterialShy;
    public Material MaterialNormal;
    public Material MaterialBrave;
    public Material MaterialInRage;

    private bool AiIsStopped = false;

    //Just for test TO DELETE
    private bool _colorChange = false;

    #endregion

    #region Unity Methods

    void Start()
    {
        // Get the Child
        firePoint = transform.GetChild(0);

        _emotionSystem = GetComponent<EmotionsSystem>();
        _meshRenderer = GetComponent<MeshRenderer>();

        GetComponent<NavMeshAgent>().destination = enemyBase.position;

        // Simply Change the color 
        _meshRenderer.material = MaterialNormal;

        #region FSM
        //Init FSM 
        FSMState normal = new FSMState();
        normal.stayActions.Add(WalkDTNormal);

        FSMState brave = new FSMState();
        brave.stayActions.Add(WalkDTBrave);

        FSMState inRage = new FSMState();
        inRage.stayActions.Add(SpreadInRage);
        inRage.stayActions.Add(WalkDTInRage);

        FSMState shy = new FSMState();
        shy.stayActions.Add(WalkDTShy);

        FSMState scared = new FSMState();
        scared.stayActions.Add(SpreadScared);
        scared.stayActions.Add(WalkDTScared);

        // FSM Transitions
        FSMTransition t1 = new FSMTransition(IsBrave); // Normal to Brave
        FSMTransition t2 = new FSMTransition(IsShy); // Normal to Shy

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
        DTDecision dbrave_2 = new DTDecision(IsTargetAcquired);
        DTDecision dbrave_3 = new DTDecision(IsTargetAcquired);
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
        DTDecision dshy_2 = new DTDecision(IsTargetAcquired);
        DTDecision dshy_3 = new DTDecision(IsTargetAcquired);
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
        DTDecision drage_1 = new DTDecision(IsTargetAcquired);
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
        DTDecision dtscared_1 = new DTDecision(True);
        dtscared_1.AddLink(true, a_runaway);

        dt_Scared = new DecisionTree(dtscared_1);

        #endregion

        #region DT Normal

        DTDecision dnormal_1 = new DTDecision(IsTargetAcquired);
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

        #region BT Runaway
        BTAction bt_runaway_a1 = new BTAction(StopMoving);
        BTAction bt_runaway_a2 = new BTAction(TurnAraound);
        BTAction bt_runaway_a3 = new BTAction(Run);

        BTSequence bt_runaway_s1 = new BTSequence(new IBTTask[] 
        {
            bt_runaway_a1,
            bt_runaway_a2
        });

        bt_Runaway = new BehaviorTree(bt_runaway_s1);
        #endregion

        #region BT Attack
        BTAction bt_attack_a1 = new BTAction(Shoot);

        BTSequence bt_attack_s1 = new BTSequence(new IBTTask[]
        {
            bt_attack_a1, 
            bt_attack_a1, 
            bt_attack_a1
        });

        BTAction bt_attack_a2 = new BTAction(Dash);
        BTAction bt_attack_a3 = new BTAction(CreateWeapon);
        BTAction bt_attack_a4 = new BTAction(DashBack);

        BTSequence bt_attack_s2 = new BTSequence(new IBTTask[]
        {
            bt_attack_a2,
            bt_attack_a3,
            bt_attack_a4
        });

        BTRandomSelector bt_attack_s3 = new BTRandomSelector(new IBTTask[]
        {
            bt_attack_s1,
            bt_attack_s2
        });

        bt_Attack = new BehaviorTree(bt_attack_s3);
        #endregion

        #region BT Regroup
        BTAction bt_regroup_a1 = new BTAction(FindNearAlly);
        BTAction bt_regroup_a2 = new BTAction(GoToAlly);

        BTSequence bt_regroup_s1 = new BTSequence(new IBTTask[]
        {
            bt_regroup_a1,
            bt_regroup_a2
        });

        bt_Regroup = new BehaviorTree(bt_regroup_s1);
        #endregion

        #region BT Heal
        BTAction bt_heal_a1 = new BTAction(GoBackToBase);
        BTCondition bt_heal_c1 = new BTCondition(IsHealthNotHigh);

        BTSequence bt_heal_s1 = new BTSequence(new IBTTask[]
        {
            bt_heal_a1,
            bt_heal_c1
        });

        BTDecoratorUntilFail bt_heal_dec1 = new BTDecoratorUntilFail(bt_heal_s1);

        bt_Heal = new BehaviorTree(bt_heal_dec1);
        #endregion

		#endregion

		// Start monitoring FSM
		StartCoroutine(Patrol());
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
	}

	#region FSM Activity
	public void WalkDTNormal()
    {
        // Start patroling
        //StartCoroutine(PatrolDTNormal());
        // No need for a second level coroutine since same framerate
        dt_Normal.walk();


        // Simply Change the color 
        _meshRenderer.material = MaterialNormal;
    }

    public void WalkDTBrave()
    {
        // Start patroling
        //StartCoroutine(PatrolDTBrave());
        // No need for a second level coroutine since same framerate
        dt_Brave.walk();


        // Simply Change the color 
        _meshRenderer.material = MaterialBrave;
    }

    public void WalkDTInRage()
    {
        //StartCoroutine(PatrolDTInRage());
        // No need for a second level coroutine since same framerate
        dt_InRage.walk();

        // Simply Change the color 
        _meshRenderer.material = MaterialInRage;
    }

    public void WalkDTShy()
    {
        //StartCoroutine(PatrolDTShy());
        // No need for a second level coroutine since same framerate
        dt_Shy.walk();

        // Simply Change the color 
        _meshRenderer.material = MaterialShy;
    }

    public void WalkDTScared()
    {
        //StartCoroutine(PatrolDTScared());
        // No need for a second level coroutine since same framerate
        dt_Scared.walk();

        // Simply Change the color 
        _meshRenderer.material = MaterialScare;
    }

    public void SpreadInRage()
    {
        foreach (GameObject ally in GameObject.FindGameObjectsWithTag(tag)) {
            if (Vector3.Distance(transform.position, ally.transform.position) < sightRange) {
                ally.GetComponent<EmotionsSystem>().inRage = true;
            }
        }
    }

    public void SpreadScared()
    {
        foreach (GameObject ally in GameObject.FindGameObjectsWithTag(tag)) {
            if (Vector3.Distance(transform.position, ally.transform.position) < sightRange) {
                ally.GetComponent<EmotionsSystem>().scared = true;
            }
        }
    }

    #endregion

    #region FSM Conditions
    public bool IsNormal()
    {
        if (_emotionSystem.Normal())
            return true;
        return false;
    }

    public bool IsBrave()
    {
        if (_emotionSystem.Brave())
            return true;
        return false;
    }


    public bool IsShy()
    {
        if (_emotionSystem.Shy())
            return true;
        return false;
    }

    public bool IsInRage()
    {
        if (_emotionSystem.InRage())
            return true;
        return false;
    }

    public bool IsScared()
    {
        if (_emotionSystem.Scared())
            return true;
        return false;
    }

    #endregion

    #region Patrol
    public IEnumerator Patrol()
    {
        while (true) {
            if(!AiIsStopped)
                fsm.Update();

            yield return new WaitForSeconds(reactionTime);
        }
    }

    // DT
    public IEnumerator PatrolDTNormal()
    {
        while (true) {
            if(!AiIsStopped)
                dt_Normal.walk();

            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTBrave()
    {
        while (true) {
            if(!AiIsStopped)
                dt_Brave.walk();

            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTShy()
    {
        while (true) {
            if (!AiIsStopped)
                dt_Shy.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTInRage()
    {
        while (true) {
            if (!AiIsStopped)
                dt_InRage.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDTScared()
    {
        while (true) {
            if (!AiIsStopped)
                dt_Scared.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // BT
    public IEnumerator PatrolBTHeal()
    {
        while (bt_Heal.Step()) {
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolBTRegroup()
    {
        while (bt_Regroup.Step()) {
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolBTAttack()
    {
        while (bt_Attack.Step()) {
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PartolBTRunaway()
    {
        while (bt_Runaway.Step()) {
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public IEnumerator PatrolDash()
    {
        if (!isDashing)
            StopCoroutine(PatrolDash());

        yield return new WaitForSeconds(dashTime * 2.5f);

        GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        GetComponent<NavMeshAgent>().ResetPath();
        GetComponent<NavMeshAgent>().speed = speed;
        GetComponent<NavMeshAgent>().acceleration = acceleration;
        isDashing = false;
    }
    
    #endregion

    #region DT Actions
    public object Chase(object o)
    {
        transform.GetComponent<NavMeshAgent>().destination = target.position;
        return null;
    }

    // BT
    public object Search(object o)
    {
        // Start patroling
        Search();
        return null;
    }

    public object Runaway(object o)
    {
        // Start patroling
        StartCoroutine(PartolBTRunaway());
        return null;
    }

    // BT
    public object Heal(object o)
    {
        // Start patroling
        //StartCoroutine(PatrolBTHeal());
        bt_Heal.Step();

        return null;
    }

    // BT
    public object Regroup(object o)
    {
        // Start patroling
        //StartCoroutine(PatrolBTRegroup());
        bt_Regroup.Step();


        return null;
    }

    //BT
    public object Attack(object o)
    {
        // Start patroling
        Debug.Log("Attack");
        StartCoroutine(PatrolBTAttack());
        return null;
    }

    public object GoToEnemyBase(object o)
    {
        GetComponent<NavMeshAgent>().destination = enemyBase.position;
        return null;
    }

    #endregion

    #region DT Conditions

    public object True(object o)
	{
        return true;
	}

    public object IsHealthLow(object o)
    {
        if (transform.GetComponent<Health>().IsHealthLow()) {
            return true;
        }
        return false;
    }

    public object IsTargetAcquired(object o)
    {
        if (target != enemyBase && target != null && Vector3.Distance(transform.position, target.position) <= sightRange) {
            return true;
        } else if (AcquireTarget()) {
            return true;
        }
        target = enemyBase;
        return false;
    }

    public object IsTargetNear(object o)
    {
        if (target != enemyBase && target != null) {
            if (Vector3.Distance(transform.position, target.position) < targetNear_range) {
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
        return transform.GetComponent<Health>().IsHealthHigh();
            
    }

    public object IsTargetHealthLow(object o)
    {
        if (target != enemyBase && target != null) {
			if (target.TryGetComponent(out Health h)) {
                if (h.health <= targetHealthLow)
                    return true;
			}
        }
        return null;
    }

    public bool AcquireTarget()
    {
        // I can calculate Angle with every target /in range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies) {
            //Check distance
            if (Vector3.Distance(transform.position, enemy.transform.position) < sightRange) {
                //Check the angle
                if (Vector3.SignedAngle(transform.position, enemy.transform.position, Vector3.up) < sightAngle) {
                    //Check if in line of sight
                    Vector3 ray = target.position - transform.position;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, ray, out hit)) {
                        if (hit.transform.TryGetComponent(out GameObject tar)) {
                            if (tar.transform.tag == enemyTag) {
                                target = enemy.transform;
                                GetComponent<NavMeshAgent>().destination = target.position;
                                return true;
                            }
                        }                      
                    }
                }
            }
        }

        return false;
    }

    #endregion

    #region BT Conditions
    public bool IsHealthNotHigh()
	{
        return !transform.GetComponent<Health>().IsHealthHigh();
	}
    #endregion

    #region BT Actions
    public bool StopMoving()
	{
        AiIsStopped = true;

        velocity = GetComponent<NavMeshAgent>().velocity;
        acceleration = GetComponent<NavMeshAgent>().acceleration;

        GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        GetComponent<NavMeshAgent>().isStopped = true;
        return true;
	}

    public bool RestartMoving()
	{
        AiIsStopped = false;

        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().velocity = velocity;

        return true;
    }

    public bool TurnAraound()
    {
        // Turn Away from the target
        Vector3 verticalAdj = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 angleDir = verticalAdj - transform.position;
        float angleBetween = Vector3.SignedAngle(transform.forward, angleDir, Vector3.up);

        transform.Rotate(0f, 180f + angleBetween, 0f, Space.Self);

        return true;
    }

    public bool Run()
    {
        GetComponent<NavMeshAgent>().velocity = velocity;
        GetComponent<NavMeshAgent>().acceleration = acceleration;

        Vector3 dir = new Vector3();
        dir = (transform.position + Vector3.forward.normalized * 50f);

        GetComponent<NavMeshAgent>().destination = dir;

        return true;
    }

    public bool Shoot()
	{
        StopMoving();

        GameObject bullet = Instantiate(bulletPrfab, firePoint.position, firePoint.rotation);

        bullet.transform.tag = transform.tag;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * 100f, ForceMode.Impulse);

        RestartMoving();

        return true;
	}

    public bool CreateWeapon()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(firePoint.position, attackMeleeRange);

        foreach(Collider enemy in hitEnemies)
        {
            if(enemy.transform.tag != tag)
            {
                enemy.transform.GetComponent<Health>().TakeDmg(meleeDmg);
            }
        }

        return true;
    }

    public bool Dash()
    {
        startingDashPoint = transform;
        speed = GetComponent<NavMeshAgent>().speed;
        acceleration = GetComponent<NavMeshAgent>().acceleration;

        if (!isDashing)
        {
            isDashing = true;
            GetComponent<NavMeshAgent>().speed += 200f;
            GetComponent<NavMeshAgent>().acceleration += 200f;
        }
        StartCoroutine(PatrolDash());

        return true;
    }

    public bool DashBack()
	{
        GetComponent<NavMeshAgent>().destination = startingDashPoint.position;
        speed = GetComponent<NavMeshAgent>().speed;
        acceleration = GetComponent<NavMeshAgent>().acceleration;

        if (!isDashing) {
            isDashing = true;
            GetComponent<NavMeshAgent>().speed += 200f;
            GetComponent<NavMeshAgent>().acceleration += 200f;
        }
        StartCoroutine(PatrolDash());

        GetComponent<NavMeshAgent>().destination = target.position;

        return true;
    }

    public bool FindNearAlly()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag(transform.tag);
        if (allies == null)
            return false;

        float minDistance = -1f;
        foreach(GameObject ally in allies) {
            float distance = Vector3.Distance(transform.position, ally.transform.position);
            if (distance < minDistance
                || minDistance == -1f) {
                minDistance = distance;
                allyPosition = ally.transform;
			}
		}
        return true;
    }

    public bool GoToAlly()
	{
        GetComponent<NavMeshAgent>().destination = allyPosition.position;
        return true;
	}

    public bool GoBackToBase()
	{
        GetComponent<NavMeshAgent>().destination = allyBase.position;
        return true;
	}

    public bool Search()
    {
        target.position = RandomNavmeshLocation(searchRange);
        return true;
    }

    #endregion

    // Use instead the method seen in classe (a point around a circle in front of you)
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Transform a; 

		while (true) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
                return hit.position;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(firePoint.position, attackMeleeRange);
    }

    //Just for a test TO DELETE
    public void ChangeColor()
	{
		if (_colorChange) {
            // Simply Change the color 
            _meshRenderer.material = MaterialShy;
        } else {
            // Simply Change the color 
            _meshRenderer.material = MaterialBrave;
        }
    }

    #endregion


}