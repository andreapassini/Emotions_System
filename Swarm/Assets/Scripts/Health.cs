using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    public int maxHealth;
    private int health;

    public int healthHigh = 70;
    public int healthLow = 30;
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

    public bool IsHealthLow()
	{
        if (this.health <= healthLow)
            return true;
        return false;
	}

    public bool IsHealthHig()
    {
        if (this.health >= healthHigh)
            return true;
        return false;
    }

    #endregion
}
