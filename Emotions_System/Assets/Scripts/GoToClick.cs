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

	bool draw = false;

	Vector3 verticalAdj;

	private void Start()
    {
		// Get the Child
		//firePoint = transform.GetChild(0);
		velocity = GetComponent<NavMeshAgent>().velocity;
		acceleration = GetComponent<NavMeshAgent>().acceleration;
	}

    void Update()
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			GetComponent<NavMeshAgent>().velocity = velocity;
			GetComponent<NavMeshAgent>().acceleration = acceleration;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().destination = hit.point;
			}

			draw = false;
		}

		if (Input.GetMouseButtonDown(1)) 
		{
			GetComponent<NavMeshAgent>().velocity = Vector3.zero;
			GetComponent<NavMeshAgent>().acceleration = 0f;

			verticalAdj = new Vector3(target.position.x, transform.position.y, target.position.z);
			Vector3 angleDir = verticalAdj - transform.position;
			angleBetween = Vector3.SignedAngle(transform.forward, angleDir, Vector3.up);

			transform.Rotate(0f, 180f + angleBetween, 0f, Space.Self);

			//Vector3 dir = transform.forward.normalized * 5f;

			//GetComponent<NavMeshAgent>().destination = dir;
			//GetComponent<NavMeshAgent>().velocity = velocity;
			//GetComponent<NavMeshAgent>().acceleration = acceleration;

			draw = true;
=======
			//Forse è meglio fare un raycast a 5cm davantia me, traporlo sul piano, ed andare in quella direzione

			GetComponent<NavMeshAgent>().destination = transform.forward.normalized * 5f;
			GetComponent<NavMeshAgent>().velocity = velocity;
			GetComponent<NavMeshAgent>().acceleration = acceleration;

			Debug.Log(angleBetween);
>>>>>>> Stashed changes
		}

		Debug.Log(transform.forward);
	}

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(target.position, transform.position * 1.5f);

        if (draw)
        {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.forward.normalized * 5f);
		}
		
    }
=======
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.forward * 10f);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(verticalAdj, transform.position);
	}
>>>>>>> Stashed changes
}