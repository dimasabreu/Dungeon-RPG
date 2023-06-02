using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public Stats strenght;
    public Stats damage;
    public Stats maxHealth;

    

    [SerializeField] private int currentHealth;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
        
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        int totalDamage = damage.GetValue() + strenght.GetValue();
        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        Debug.Log(_damage);

        if(currentHealth <= 0 )
            Die();
    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }
}
