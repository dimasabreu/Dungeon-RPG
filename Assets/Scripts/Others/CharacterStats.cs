using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stats strenght;      // 1 point increase damage by 1 and crit.power by 1%
    public Stats agility;       // 1 point increase evasion by 1% and crit.chance by 1%
    public Stats intelligence;  // 1 point increase magic damage by 1 and magica resistance by 3
    public Stats vitality;      // 1 point increase health by 3 or 5 points.

    [Header("Offensive stats")]
    public Stats damage;
    public Stats critChance;
    public Stats critPower;     // default 150%


    [Header("Defensive stats")]
    public Stats maxHealth;
    public Stats armor;
    public Stats evasion; 
    public Stats magicResistance;

    [Header("Magic stats")]
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightingDamage;
    

    public bool isIgnited; // fire damage over time
    public bool isChilled; // reduce armor by 20 %
    public bool isShocked; // reduce accuracy by 20 %

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;


    public int currentHealth;

    public System.Action onHealthChanged;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update() 
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if(ignitedTimer < 0)
            isIgnited = false;
        if(chilledTimer < 0)
            isChilled = false;
        if(shockedTimer < 0)
            isShocked = false;
        
        if(igniteDamageTimer < 0 && isIgnited)
        {
            Debug.Log("Take burn damage" + igniteDamage);

            DecreaseHealthBy(igniteDamage);

            if(currentHealth <= 0)
                Die();
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strenght.GetValue();

        if(CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            
        }
                    
        
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        //_targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if(Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage; 
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while(!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if(Random.value < .4f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApllyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("ta pegando fogo bixo");
                return;
            }

            if(Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApllyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("gelado");
                return;
            }

            if(Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApllyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("choque do trovao");
                return;
            }

            
        }

        if(canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApllyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public void ApllyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if(isIgnited || isChilled || isShocked)
            return;
        
        if(_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }
        if(_chill)
        {   
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        if(_shock)
        {
            isShocked = _shock;
            shockedTimer = ailmentsDuration;
            fx.ShockFxFor(ailmentsDuration);
        }
    }


    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        Debug.Log(_damage);

        if(currentHealth <= 0 )
            Die();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if(onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }
    
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if(isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strenght.GetValue()) * .01f;
        

        float critDamage = _damage * totalCritPower;
        

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
}
