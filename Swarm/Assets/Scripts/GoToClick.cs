using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoToClick : MonoBehaviour
{
	public Transform target;

	private Transform firePoint;

	private void Start()
    {
		// Get the Child
		firePoint = transform.GetChild(0);
	}

    void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().destination = hit.point;
			}

			//vec = Multi(defaultMat, vec);
			//Debug.Log(vec[0] + " - " + vec[1] + " - " + vec[2] + " - " + vec[3] + " - " + vec[4]);
		}

		if (Input.GetMouseButtonDown(1)) {
			if (Input.GetMouseButtonDown(1)) {
				GameObject[] allies = GameObject.FindGameObjectsWithTag(transform.tag);

				if(allies == null) {
					Debug.Log("null");
				} else {
					Debug.Log(allies.Length);
				}
			}
		}
    }
}