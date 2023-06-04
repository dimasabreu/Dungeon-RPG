using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stats strenght;      // 1 point increase damage by 1 and crit.power by 1%
    public Stats agility;       // 1 point increase evasion by 1% and crit.chance by 1%
    public Stats intelligence;  // 1 point increase magic damage by 1 and magica resistance by 3
    public Stats vitality;      // 1 point increase health by 3 or 5 points.

    [Header("Defensive stats")]
    public Stats maxHealth;
    public Stats armor;
    public Stats evasion; 

    
    public Stats damage;

    [SerializeField] private int currentHealth;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;


        int totalDamage = damage.GetValue() + strenght.GetValue();
        
        
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
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
    
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

}
