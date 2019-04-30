using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public void Update()
    {
        CheckHealth();
    }

    public void CheckHealth()
    {
        if (health == 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log("dead");
    }

    public virtual void OnHeartCollide(float heartVal, DetachedPiece thisHeart) //when collided with a heart
    {

    }

    public virtual void TakeDamage(float damageVal)
    {
        health -= damageVal;
    }
}
