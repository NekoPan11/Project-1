using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 120;
    public int currentHealth;
    public float baseAttackPower = 7f;
    public float baseMagicAttackPower = 12f;
    public float baseRangedAttackPower = 10f;
    public float attackPowerModifier = 0f;
    public float magicAttackPowerModifier = 0f;
    public float rangedAttackPowerModifier = 0f;
    public float AttackPower { get { return baseAttackPower + attackPowerModifier; } }
    public float MagicAttackPower { get { return baseMagicAttackPower + magicAttackPowerModifier; } }
    public float RangedAttackPower { get { return baseRangedAttackPower + rangedAttackPowerModifier; } }
    public float incomingDamageReduction = 0;
    public float incomingDamageReductionModifier = 0; 
    public float superGauge = 0f;
    public float superThreshold = 100f;
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    public abstract void TakeDamage(float damage);
    public abstract bool IsAlive();
    public abstract bool CanUseSuper();
    public virtual void ResetModifiers()
    {
        attackPowerModifier = 0f;
        magicAttackPowerModifier = 0f;
        rangedAttackPowerModifier = 0f;
        incomingDamageReductionModifier = 0;
    }
}