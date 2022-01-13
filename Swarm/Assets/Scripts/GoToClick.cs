using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoToClick : MonoBehaviour
{
	public Transform target;
	public GameObject bulletPrfab;

	void Update()
	{
		if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().destination = hit.point;
			}
		}

        if (Input.GetMouseButtonDown(1))
        {

			//GetComponent<NavMeshAgent>().destination = - (transform.position - target.position).normalized;
			GetComponent<NavMeshAgent>().velocity.Set(GetComponent<NavMeshAgent>().velocity.x, GetComponent<NavMeshAgent>().velocity.y, GetComponent<NavMeshAgent>().velocity.z * 10f);

		}
	}

}