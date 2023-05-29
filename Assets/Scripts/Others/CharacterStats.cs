using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public int damage;
    public int maxHealth;

    [SerializeField] private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if(currentHealth <= 0 )
            Die();
    }

    private void Die()
    {
        throw new NotImplementedException();
    }
}
