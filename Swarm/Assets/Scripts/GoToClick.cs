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

	public float[] vec;
	public float[][] mat;

	private float[][] defaultMat;

	private void Start()
    {
		// Get the Child
		firePoint = transform.GetChild(0);

		vec = new float[]{
			0.0f,
			0.2f,
			0.6f,
			0.2f,
			0.0f
		};

		TransitionMatrixDefaultInit();
		TransitionMatrixRageInit();
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
				vec = Multi(mat, vec);
				Debug.Log(vec[0] + " - " + vec[1] + " - " + vec[2] + " - " + vec[3] + " - " + vec[4]);
			}
		}
    }

	public void TransitionMatrixRageInit()
	{
		mat = new float[5][];

		mat[0] = new float[5] { 1.0f, 0.1f, 0.1f, 0.1f, 0.1f };
		mat[1] = new float[5] { 0.5f, 0.1f, 0.1f, 0.1f, 0.1f };
		mat[2] = new float[5] { 0.3f, 0.1f, 0.1f, 0.1f, 0.1f };
		mat[3] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
		mat[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
	}

	public void TransitionMatrixDefaultInit()
	{
		// Declare a jagged array.
		defaultMat = new float[5][];

		defaultMat[0] = new float[5] { 1.0f, 0.1f, 0.1f, 0.1f, 0.1f };
		defaultMat[1] = new float[5] { 0.1f, 1.0f, 0.1f, 0.1f, 0.1f };
		defaultMat[2] = new float[5] { 0.1f, 0.1f, 1.0f, 0.1f, 0.1f };
		defaultMat[3] = new float[5] { 0.1f, 0.1f, 0.1f, 1.0f, 0.1f };
		defaultMat[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 1.0f };
	}

	public float[] Multi(float[][] matrix, float[] stateVector)
	{
		float[] vector = new float[5];

		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				vector[i] += (matrix[i][j] * stateVector[j]);
			}
		}

		return vector;
	}
}