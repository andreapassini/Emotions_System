using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    public int maxHealth;
    private int health;
    #endregion

    #region Unity Methods

    void Start()
    {
        health = maxHealth;
    }

    public int GetHealth()
	{
        return health;
	}

    public void TakeDmg(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion
}
