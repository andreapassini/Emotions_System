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

		if (Input.GetMouseButton(1)) {
			GetComponent<DecisionMaker>().Shoot();
		}

		Debug.Log(transform.position);
	}

	public bool Shoot()
	{
		GameObject bullet = Instantiate(bulletPrfab, transform.GetChild(0).position, transform.GetChild(0).rotation);
		Rigidbody rb = bullet.GetComponent<Rigidbody>();

		rb.AddForce(transform.GetChild(0).forward.normalized * 20f, ForceMode.Impulse);

		return true;
	}
}