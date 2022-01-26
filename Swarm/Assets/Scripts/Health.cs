using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    public int maxHealth;
    public int health { get; private set; } = 0;

    public int healthHigh = 70;
    public int healthLow = 30;
    #endregion

    #region Unity Methods

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDmg(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Die();
        }
    }

    public void HealOnTouch(int hlt)
	{
        health += hlt;
        Mathf.Clamp(health, 0, maxHealth);
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
