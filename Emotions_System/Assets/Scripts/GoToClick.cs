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

	//For the transitions
	private float[] stateVector;

	private float[][] defaultMatrix;

	private float[][] inRageMatrix;
	private float[][] scaredMatrix;
	private float[][] braveMatrix;
	private float[][] shyMatrix;

    private Add addClass;

    int i;

    public Material OtherMaterial;

    private void Start()
    {
		velocity = GetComponent<NavMeshAgent>().velocity;
		acceleration = GetComponent<NavMeshAgent>().acceleration;

        stateVector = new float[]{
            0.0f,
            0.0f,
            1.0f,
            0.0f,
            0.0f
        };

        //      TransitionMatrixRageInit();
        //      TransitionMatrixBraveInit();
        //      TransitionMatrixDefaultInit();
        //      TransitionMatrixShyInit();
        //      TransitionMatrixScaredInit();
    }

    void Update()
	{
		if (Input.GetMouseButtonDown(0)) 
		{
            //GetComponent<NavMeshAgent>().velocity = velocity;
            //GetComponent<NavMeshAgent>().acceleration = acceleration;

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit)) {
            //	GetComponent<NavMeshAgent>().destination = hit.point;
            //}

            // Simply Change the color 
            GetComponent<MeshRenderer>().material = OtherMaterial;
        }

		#region A

		//if (Input.GetMouseButtonUp(1)) 
		//{
		//	#region Runaway
		//	//GetComponent<NavMeshAgent>().velocity = Vector3.zero;
		//	//GetComponent<NavMeshAgent>().acceleration = 0f;

		//	//verticalAdj = new Vector3(target.position.x, transform.position.y, target.position.z);
		//	//Vector3 angleDir = verticalAdj - transform.position;
		//	//angleBetween = Vector3.SignedAngle(transform.forward, angleDir, Vector3.up);

		//	//transform.Rotate(0f, 180f + angleBetween, 0f, Space.Self);

		//	//Vector3 dir = new Vector3();
		//	//dir = (transform.position + Vector3.forward.normalized * 50f);

		//	//GetComponent<NavMeshAgent>().destination = dir;
		//	//GetComponent<NavMeshAgent>().velocity = velocity;
		//	//GetComponent<NavMeshAgent>().acceleration = acceleration;

		//	//Debug.Log(Vector3.forward.normalized * 5f);
		//	#endregion

		//	#region Transition
		//	////stateVector = Multiply(shyMatrix, stateVector);
		//	//Debug.Log(stateVector[0] + " " +
		//	//        stateVector[1] + " " +
		//	//        stateVector[2] + " " +
		//	//        stateVector[3] + " " +
		//	//        stateVector[4]);
		//	#endregion

		//	if (GetComponent<EmotionsSystem>().Normal()) {
  //              Debug.Log("InRage: " + GetComponent<EmotionsSystem>().InRage());
  //              Debug.Log("Brave " + GetComponent<EmotionsSystem>().Brave());
  //              Debug.Log("Normal " + GetComponent<EmotionsSystem>().Normal());
  //              Debug.Log("Shy " + GetComponent<EmotionsSystem>().Shy());
  //              Debug.Log("Scared " + GetComponent<EmotionsSystem>().Scared());
  //          }

  //          #region Output Vector
  //          ////stateVector = Multiply(shyMatrix, stateVector);
  //          //Debug.Log("SECOND");
  //          //Debug.Log(stateVector[0] + " " +
  //          //        stateVector[1] + " " +
  //          //        stateVector[2] + " " +
  //          //        stateVector[3] + " " +
  //          //        stateVector[4]);
  //          #endregion
  //      }

		#endregion
	}

	public Vector3 RandomNavmeshLocation(float radius)
	{
		while (true) 
		{
			Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
			randomDirection += transform.position;
			NavMeshHit hit;
			Vector3 finalPosition = Vector3.zero;

			if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) 
			{
				finalPosition = hit.position;
				return finalPosition;
			}
		}
	}

    //#region Matrix
    //public void TransitionMatrixDefaultInit()
    //{
    //    // Declare a jagged array.
    //    defaultMatrix = new float[5][];

    //    defaultMatrix[0] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    //    defaultMatrix[1] = new float[5] { 0.1f, 0.1f, 0.2f, 0.1f, 0.1f };
    //    defaultMatrix[2] = new float[5] { 0.1f, 0.5f, 1.0f, 0.5f, 0.1f };
    //    defaultMatrix[3] = new float[5] { 0.1f, 0.1f, 0.2f, 0.1f, 0.1f };
    //    defaultMatrix[4] = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    //}

    //public void TransitionMatrixRageInit()
    //{
    //    inRageMatrix = new float[5][];

    //    inRageMatrix[0] = new float[5] { 1.3f, 0.1f, 0.1f, 0.0f, 0.0f };
    //    inRageMatrix[1] = new float[5] { 0.0f, 1.1f, 0.1f, 0.0f, 0.0f };
    //    inRageMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
    //    inRageMatrix[3] = new float[5] { 0.0f, 0.0f, 0.0f, 0.7f, 0.0f };
    //    inRageMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.6f };
    //}

    //public void TransitionMatrixBraveInit()
    //{
    //    braveMatrix = new float[5][];

    //    braveMatrix[0] = new float[5] { 1.1f, 0.1f, 0.0f, 0.0f, 0.0f };
    //    braveMatrix[1] = new float[5] { 0.1f, 1.3f, 0.1f, 0.0f, 0.0f };
    //    braveMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
    //    braveMatrix[3] = new float[5] { 0.0f, 0.0f, 0.0f, 0.7f, 0.0f };
    //    braveMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.6f };
    //}

    //public void TransitionMatrixScaredInit()
    //{
    //    scaredMatrix = new float[5][];

    //    scaredMatrix[0] = new float[5] { 0.6f, 0.0f, 0.0f, 0.0f, 0.0f };
    //    scaredMatrix[1] = new float[5] { 0.0f, 0.7f, 0.0f, 0.0f, 0.0f };
    //    scaredMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
    //    scaredMatrix[3] = new float[5] { 0.0f, 0.0f, 0.1f, 1.3f, 0.1f };
    //    scaredMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.1f, 1.1f };
    //}

    //public void TransitionMatrixShyInit()
    //{
    //    shyMatrix = new float[5][];

    //    shyMatrix[0] = new float[5] { 0.6f, 0.0f, 0.0f, 0.0f, 0.0f };
    //    shyMatrix[1] = new float[5] { 0.0f, 0.7f, 0.0f, 0.0f, 0.0f };
    //    shyMatrix[2] = new float[5] { 0.0f, 0.0f, 0.8f, 0.0f, 0.0f };
    //    shyMatrix[3] = new float[5] { 0.0f, 0.0f, 0.1f, 1.3f, 0.1f };
    //    shyMatrix[4] = new float[5] { 0.0f, 0.0f, 0.0f, 0.1f, 1.1f };
    //}
    //#endregion

    public float[] Multiply(float[][] matrix, float[] _stateVector)
    {
        float[] vector = new float[5];

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                vector[i] += matrix[i][j] * _stateVector[j];
            }
        }

        return vector;
    }

    public float Total(float[] vec)
    {
        float total = 0f;

        //foreach (float item in vec)
        //{
        //    total += item;
        //}

        for(int i=0; i<5; i++)
        {
            total += vec[i];
            vec[i]++;
        }

        return total;
    }

    private void OnDrawGizmos()
	{
		
	}
}

public class Add{
    public float[] myVector;

    public Add(float[] current)
    {
        myVector = current;
    }

}