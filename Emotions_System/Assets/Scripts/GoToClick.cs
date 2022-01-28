using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoToClick : MonoBehaviour
{
	public Transform target;

	//private bool firePoint;

	Vector3 velocity;
	float acceleration;
	float angleBetween;

	private void Start()
    {
		// Get the Child
		//firePoint = transform.GetChild(0);
		velocity = GetComponent<NavMeshAgent>().velocity;
		acceleration = GetComponent<NavMeshAgent>().acceleration;
	}

    void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			GetComponent<NavMeshAgent>().velocity = velocity;
			GetComponent<NavMeshAgent>().acceleration = acceleration;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().destination = hit.point;
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			GetComponent<NavMeshAgent>().velocity = Vector3.zero;
			GetComponent<NavMeshAgent>().acceleration = 0f;

			Vector3 angleDir = target.position - transform.position;
			angleBetween = Vector3.Angle(transform.forward, angleDir);

			transform.Rotate(0f, - angleBetween, 0f);

			//Vector3 dir = transform.forward;

			//GetComponent<NavMeshAgent>().destination = - dir;
			//GetComponent<NavMeshAgent>().velocity = velocity;
			//GetComponent<NavMeshAgent>().acceleration = acceleration;
		}
	}
}