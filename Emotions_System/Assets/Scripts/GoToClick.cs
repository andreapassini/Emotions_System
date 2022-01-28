using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoToClick : MonoBehaviour
{
	public Transform target;

	private Transform firePoint;

	Vector3 velocity;
	float acceleration;

	private void Start()
    {
		// Get the Child
		firePoint = transform.GetChild(0);
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
			GetComponent<NavMeshAgent>().acceleration = 0;

			Vector3 rotation = new Vector3(0, 180, 0);
			transform.Rotate(rotation, Space.Self);
		}
    }
}