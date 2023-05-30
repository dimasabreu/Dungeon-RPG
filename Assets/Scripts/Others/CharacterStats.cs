using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public Stats damage;
    public Stats maxHealth;

    

    [SerializeField] private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth.GetValue();
        //example
        damage.AddModifier(4);
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
