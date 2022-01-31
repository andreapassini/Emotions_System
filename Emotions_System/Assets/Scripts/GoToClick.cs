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

	Vector3 verticalAdj;

	private void Start()
    {
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

		}

		if (Input.GetMouseButtonDown(1)) 
		{
			GetComponent<NavMeshAgent>().velocity = Vector3.zero;
			GetComponent<NavMeshAgent>().acceleration = 0f;

			verticalAdj = new Vector3(target.position.x, transform.position.y, target.position.z);
			Vector3 angleDir = verticalAdj - transform.position;
			angleBetween = Vector3.SignedAngle(transform.forward, angleDir, Vector3.up);

			transform.Rotate(0f, 180f + angleBetween, 0f, Space.Self);

			GetComponent<NavMeshAgent>().destination = RandomNavmeshLocation(70f);

			//GetComponent<NavMeshAgent>().destination = transform.forward.normalized * 5f;
			GetComponent<NavMeshAgent>().velocity = velocity;
			GetComponent<NavMeshAgent>().acceleration = acceleration;

			Debug.Log(angleBetween);
		}

	}

	public Vector3 RandomNavmeshLocation(float radius)
	{
		while (true) {
			Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
			randomDirection += transform.position;
			NavMeshHit hit;
			Vector3 finalPosition = Vector3.zero;
			if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
				finalPosition = hit.position;
				return finalPosition;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.forward * 10f);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(verticalAdj, transform.position);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 70f);
	}

}