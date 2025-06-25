using UnityEngine;
public class EnemyBot : Character
{
    public static EnemyBot Instance { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void TakeDamage(float damage)
    {
        float totalDamage = damage;
        totalDamage -= (totalDamage * (incomingDamageReduction / 100));
        currentHealth -= (int)totalDamage;
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log($"{characterName} получил {damage} урона. Осталось здоровья: {currentHealth}");
    }
    public override bool IsAlive()
    {
        return currentHealth > 0;
    }
    public override bool CanUseSuper()
    {
        return superGauge >= superThreshold;
    }
    public void AddToSuperGauge(float amount)
    {
        superGauge += amount;
        superGauge = Mathf.Clamp(superGauge, 0, superThreshold);
    }
    public string BotMakeMove(Character target)
    {
        float damage;
        float rand = Random.value;

        if (rand < 0.33f)
        {
            damage = AttackPower;
            target.TakeDamage(damage);
            return $"{characterName} нанес {damage} физического урона!";
        }
        else if (rand < 0.66f)
        {
            damage = MagicAttackPower;
            target.TakeDamage(damage);
            return $"{characterName} нанес {damage} магического урона!";
        }
        else
        {
            damage = RangedAttackPower;
            target.TakeDamage(damage);
            return $"{characterName} нанес {damage} урона дальней атакой!";
        }
    }  
}