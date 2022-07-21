using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;
using System;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class TestMultipleLevel : MonoBehaviour
{
	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private Vector3 stdVelocity;

	public Transform target;

	[SerializeField] private float AiFrameRate = .1f;
	private bool AiIsStopped = false;

	private FSM fsm;
	private DecisionTree dt;

	[SerializeField] private float coolDown = 2f;
	private bool isActive = false;

	public Material red;
	public Material green;
	private MeshRenderer meshRenderer;
	private bool isGreen = false;

	private int i = 0;

	private void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		stdVelocity = navMeshAgent.velocity;
		meshRenderer = GetComponent<MeshRenderer>();
	}

	void Start()
    {
		navMeshAgent.destination = target.position;
		meshRenderer.material = green;
		isGreen = true;

		// FSM
		FSMState idle = new FSMState();
		idle.stayActions.Add(RunBT);

		// DT
		DTAction a_changeColor = new DTAction(ChangeColor);
		DTAction a_stop = new DTAction(StopMoving);

		DTDecision d1 = new DTDecision(IsTimeToAct);

		d1.AddLink(true, a_changeColor);
		d1.AddLink(false, a_stop);

		dt = new DecisionTree(d1);

		fsm = new FSM(idle);

		StartCoroutine(Patrol());
    }

	#region Patrol
	private IEnumerator Patrol()
	{
		while (true) {

			if (!AiIsStopped)
				fsm.Update();

			yield return new WaitForSeconds(AiFrameRate);
		}
	}
	#endregion

	private void RunBT()
	{
		dt.walk();
	}

	private object StopMoving(object o)
	{
		AiIsStopped = true;

		navMeshAgent.velocity = Vector3.zero;
		navMeshAgent.isStopped = true;

		StartCoroutine(StopTimer(coolDown/4));

		return null;
	}

	private void RestartMoving()
	{
		navMeshAgent.isStopped = false ;
		navMeshAgent.velocity = stdVelocity;

		AiIsStopped = false;
	}

	private object ChangeColor(object o)
	{
		if (isGreen) {
			meshRenderer.material = red;
			isGreen = false;
		} else {
			meshRenderer.material = green;
			isGreen = true;
		}

		return null;
	}

	private object IsTimeToAct(object o)
	{
		if(i%2 == 0) {
			i++;
			return true;
		} else {
			i++;
			return false;
		}
	}

	private IEnumerator StopTimer(float timer)
	{

		yield return new WaitForSeconds(timer);

		RestartMoving();
	}
}
