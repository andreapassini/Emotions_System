using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoToClick : MonoBehaviour
{
	public Transform target;
	public GameObject bulletPrfab;

	public float dashTime = 100f;
	public float dashSpeed = 1.1f;

	private Transform firePoint;

    private void Start()
    {
		// Get the Child
		firePoint = transform.GetChild(0);
	}

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
			GameObject bullet = Instantiate(bulletPrfab, firePoint.position, firePoint.rotation);
			Rigidbody rb = bullet.GetComponent<Rigidbody>();

			rb.AddForce(firePoint.forward * 100f, ForceMode.Impulse);
		}
    }
}