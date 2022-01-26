using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider))]

public class AgentBase : MonoBehaviour
{
    #region Variables
    public string allyTag;
    public int heal = 20;
    #endregion

    #region Unity Methods

    void Start()
    {
        allyTag.ToUpper();
    }

    void Update()
    {
        
    }

	private void OnCollisionStay(Collision collision)
	{
        if (collision.gameObject.CompareTag(allyTag)) {
            if(collision.gameObject.TryGetComponent(out Health h)) {
                int hMax = h.maxHealth;
                h.HealOnTouch(hMax / 100);
            }
        }
    }

	#endregion
}
