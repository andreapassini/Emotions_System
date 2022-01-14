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

	private bool isDashing;

    private float speed;
    private float acceleration;

    private void Start()
    {
		this.isDashing = false;
        speed = GetComponent<NavMeshAgent>().speed;
        acceleration = GetComponent<NavMeshAgent>().acceleration;

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
    }
}