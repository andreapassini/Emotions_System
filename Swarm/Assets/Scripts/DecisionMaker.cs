using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using CRBT;
using System;

[RequireComponent(typeof(NavMeshAgent))]

public class DecisionMaker : MonoBehaviour
{
    #region Variables
    // AI FRAME
    // i could even use different reaction time for each 
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

    private BehaviorTree bt_Search;
    private BehaviorTree bt_Heal;
    private BehaviorTree bt_Regroup;
    private BehaviorTree bt_Attack;
    private BehaviorTree bt_Chase;
    private BehaviorTree bt_Runaway;


    // Internal knowledge
    [Header("Internal knowledge")]
    [Space]
    public float sightRange = 50f;
    public float sightAngle = 45f;

    private int emotionsValue = 150;
    public int emotionsValue_increment = 15;

    // External knowledge
    [Header("External knowledge")]
    [Space]
    private Transform target;
    public float targetNear_range = 3.5f;
    public int targetHealthLow;
    public string enemyTag = "B";
    public Transform enemyBase;

    private bool isDashing;
    private float speed;
    private float acceleration;
    public float dashTime = 100f;
    public float dashSpeed = 1.1f;

    #endregion

    #region Unity Methods

    void Start()
    {
        // Get the Child
        firePoint = transform.GetChild(0);

        #region FSM
        //Init FSM 
        FSMState normal = new FSMState();
        normal.stayActions.Add(WalkDTNormal);

        FSMState brave = new FSMState();
        brave.stayActions.Add(WalkDTBrave);

        FSMState inRage = new FSMState();
        inRage.stayActions.Add(WalkDTInRage);
        inRage.stayActions.Add(SpreadInRage);

        FSMState shy = new FSMState();
        shy.stayActions.Add(WalkDTShy);

        FSMState scared = new FSMState();
        scared.stayActions.Add(WalkDTScared);
        scared.stayActions.Add(SpreadScared);

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

        #region BT Runaway
        BTAction bt_runaway_a1 = new BTAction(TurnAraound);
        BTAction bt_runaway_a2 = new BTAction(Run);

        BTSequence bt_runaway_s1 = new BTSequence(new IBTTask[] 
        {
            bt_runaway_a1,
            bt_runaway_a2
        });
        #endregion

        #region BT Attack
        BTAction bt_attack_a1 = new BTAction(Shoot);

        BTSequence bt_attack_s1 = new BTSequence(new IBTTask[]
        {
            bt_attack_a1, 
            bt_attack_a1, 
            bt_attack_a1
        });

        BTAction bt_attack_a2 = new BTAction(CreateWeapon);
        BTAction bt_attack_a3 = new BTAction(Dash);
        BTAction bt_attack_a4 = new BTAction(DestroyWeapon);

        BTSequence bt_attack_s2 = new BTSequence(new IBTTask[]
        {
            bt_attack_a2,
            bt_attack_a3,
            bt_attack_a4
        });

        BTSelector bt_attack_s3 = new BTSelector(new IBTTask[]
        {
            bt_attack_s1,
            bt_attack_s2
        });
        #endregion

        #endregion

        // Start monitoring FSM
        StartCoroutine(Patrol());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Run();
        }
    }

    #region EmotionValue Controller
    public void IncreaseEmotionsValue(int increment)
    {
        emotionsValue += increment;
        Mathf.Clamp(emotionsValue, 0, 300);
    }
    #endregion

    #region FSM Activity
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

    public void SpreadInRage()
    {
        foreach (GameObject ally in GameObject.FindGameObjectsWithTag(tag)) {
            if (Vector3.Distance(transform.position, ally.transform.position) < sightRange) {
                ally.GetComponent<DecisionMaker>().IncreaseEmotionsValue(emotionsValue_increment);
            }
        }
    }

    public void SpreadScared()
    {
        foreach (GameObject ally in GameObject.FindGameObjectsWithTag(tag)) {
            if (Vector3.Distance(transform.position, ally.transform.position) < sightRange) {
                ally.GetComponent<DecisionMaker>().IncreaseEmotionsValue(-emotionsValue_increment);
            }
        }
    }

    #endregion

    #region FSM Conditions
    public bool IsBrave()
    {
        return GetComponent<EmotionsSystem>().Brave();
    }

    public bool IsNormal()
    {
        return GetComponent<EmotionsSystem>().Normal();
    }

    public bool IsShy()
    {
        return GetComponent<EmotionsSystem>().Shy();
    }

    public bool IsInRage()
    {
        return GetComponent<EmotionsSystem>().InRage();
    }

    public bool IsScared()
    {
        return GetComponent<EmotionsSystem>().Scared();
    }

    #endregion

    #region Patrol
    public IEnumerator Patrol()
    {
        while (true) {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // DT
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

    // BT
    public IEnumerator PatrolBTSearch()
    {
        while (bt_Search.Step()) {
            yield return new WaitForSeconds(reactionTime);
        }
    }

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

    public IEnumerator PatrolBTChase()
    {
        while (bt_Chase.Step()) {
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
        // Start patroling
        StartCoroutine(PatrolBTChase());
        return null;
    }

    // BT
    public object Search(object o)
    {
        // Start patroling
        StartCoroutine(PatrolBTSearch());
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
        StartCoroutine(PatrolBTHeal());
        return null;
    }

    // BT
    public object Regroup(object o)
    {
        // Start patroling
        StartCoroutine(PatrolBTRegroup());
        return null;
    }

    //BT
    public object Attack(object o)
    {
        // Start patroling
        StartCoroutine(PatrolBTAttack());
        return null;
    }

    public object GoToEnemyBase(object o)
    {
        return null;
    }

    #endregion

    #region DT Conditions
    public object IsHealthLow(object o)
    {
        if (transform.GetComponent<Health>().IsHealthLow()) {
            return true;
        }
        return false;
    }

    public object IsTargetAquired(object o)
    {
        if (target != enemyBase && target != null && Vector3.Distance(transform.position, target.position) <= sightRange * 2) {
            return true;
        } else if (AquireTarget()) {
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
        if (transform.GetComponent<Health>().IsHealthHig())
            return true;
        return false;
    }

    public object IsTargetHealthLow(object o)
    {
        if (target != enemyBase && target != null) {
            if (target.GetComponent<Health>().GetHealth() <= targetHealthLow) {
                return true;
            }
        }
        return null;
    }

    public bool AquireTarget()
    {
        // I can calculate Angle with every target /in range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies) {
            //Check distance
            if (Vector3.Distance(transform.position, enemy.transform.position) < sightRange) {

                //Check the angle
                if (Vector3.Angle(transform.position, enemy.transform.position) < sightAngle) {

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

    #region BT Conditions


    #endregion

    #region BT Actions
    public bool TurnAraound()
    {
        // Turn Away from the target
        transform.RotateAroundLocal(transform.up, Vector3.Angle(transform.position, target.position) + 180f);

        return true;
    }

    public bool Run()
    {
        // Opposite direction of the vector from position to target position
        GetComponent<NavMeshAgent>().destination = -(transform.position - target.position).normalized;

        return true;
    }

    public bool Shoot()
	{
        GameObject bullet = Instantiate(bulletPrfab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.AddForce(firePoint.forward * 100f, ForceMode.Impulse);

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

    public bool DestroyWeapon()
    {
        return true;
    }

    public bool Dash()
    {
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

    #endregion

    private void OnDrawGizmos()
    {
        if (firePoint == null)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(firePoint.position, attackMeleeRange);
    }

    #endregion
}
