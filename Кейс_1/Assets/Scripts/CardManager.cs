using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class Card
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string EffectType { get; set; }
    public Dictionary<string, float> EffectParams { get; set; }
    public Card(string name, string description, string effectType, Dictionary<string, float> effectParams)
    {
        Name = name;
        Description = description;
        EffectType = effectType;
        EffectParams = effectParams;
    }
}
public class CardManager : MonoBehaviour
{
    public BattleManager battleManager;
    public GameObject cardSelectionPanel;
    public List<Button> cardButtons;
    public List<Text> cardDescriptionTexts;
    public List<Card> damageCards = new List<Card>();
    public List<Card> defenseCards = new List<Card>();
    public List<Card> healthCards = new List<Card>();
    public List<Card> superChargeCards = new List<Card>();
    public List<Card> enemyControlCards = new List<Card>();
    [HideInInspector] public List<Card> playerGeneratedCards = new List<Card>();
    [HideInInspector] public List<Card> enemyGeneratedCards = new List<Card>();
    [HideInInspector] public Card playerSelectedCard = null;
    [HideInInspector] public Card enemySelectedCard = null;
    private bool isPlayerTurn = true;
    void Start()
    {
        InitializeCards();
        battleManager.OnCardsGenerated += OnCardsGenerated;
        cardSelectionPanel.SetActive(false);
    }
    void InitializeCards()
    {
        damageCards.Add(new Card("Гневное нападение", "Увеличивает физический урон на 5 единиц.", "Урон", new Dictionary<string, float> { { "physicalDamage", 5 } }));
        damageCards.Add(new Card("Поток энергии", "Увеличивает урон от заклинаний на 1 единицу.", "Урон", new Dictionary<string, float> { { "spellDamage", 1 } }));
        damageCards.Add(new Card("Точность стрелка", "Увеличивает урон от стрельбы на 7 единиц.", "Урон", new Dictionary<string, float> { { "rangedDamage", 7 } }));
        damageCards.Add(new Card("Сверхсила", "Увеличивает общий урон на 3 единицы.", "Урон", new Dictionary<string, float> { { "globalDamage", 3 } }));
        defenseCards.Add(new Card("Инвизибилити", "На следующем ходу противник не может нанести урон", "Защита", new Dictionary<string, float> { { "invisibility", 1 } }));
        healthCards.Add(new Card("Лечебный свет", "Восстанавливает 15 единиц здоровья.", "Здоровье", new Dictionary<string, float> { { "heal", 15 } }));
        healthCards.Add(new Card("Регенерация", "Восстанавливает 5 единиц здоровья.", "Здоровье", new Dictionary<string, float> { { "heal", 5 } }));
        healthCards.Add(new Card("Великая исцеление", "Восстанавливает 25 единиц здоровья, но снижает супер-шкалу на 25 единиц.", "Здоровье", new Dictionary<string, float> { { "heal", 25 }, { "superReduction", 25 } }));
        superChargeCards.Add(new Card("Энергетический заряд", "Добавляет 50 очков к супер-шкале.", "Супер", new Dictionary<string, float> { { "superCharge", 50 } }));
        superChargeCards.Add(new Card("Супер-заряд", "Накапливает 25 очков в супер-шкалу.", "Супер", new Dictionary<string, float> { { "superCharge", 25 } }));
        superChargeCards.Add(new Card("Экстренный запас", "Если супер-шкала пуста, добавляет 75 очков сразу.", "Супер", new Dictionary<string, float> { { "superChargeIfEmpty", 75 } }));
        enemyControlCards.Add(new Card("Обездвиживание", "Противник пропускает следующий ход.", "Контроль", new Dictionary<string, float> { { "immobilize", 1 } }));
    }
    public void GenerateCardsForTurn()
    {
        playerGeneratedCards.Clear();
        enemyGeneratedCards.Clear();
        playerGeneratedCards.AddRange(GetRandomCards(2));
        enemyGeneratedCards.AddRange(GetRandomCards(2));
    }
    private List<Card> GetRandomCards(int count)
    {
        List<Card> allCards = new List<Card>();
        allCards.AddRange(damageCards);
        allCards.AddRange(defenseCards);
        allCards.AddRange(healthCards);
        allCards.AddRange(superChargeCards);
        allCards.AddRange(enemyControlCards);
        List<Card> selectedCards = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            if (allCards.Count == 0)
            {
                Debug.LogWarning("Not enough cards to select from!");
                break;
            }
            int randomIndex = Random.Range(0, allCards.Count);
            selectedCards.Add(allCards[randomIndex]);
            allCards.RemoveAt(randomIndex);
        }
        return selectedCards;
    }
    public void ShowCardSelection(bool isPlayer)
    {
        if (cardSelectionPanel == null)
        {
            Debug.LogError("cardSelectionPanel is null! Please assign it in the Inspector.");
            return;
        }
        cardSelectionPanel.SetActive(true);
        if (cardButtons == null || cardButtons.Count == 0)
        {
            Debug.LogError("cardButtons is null or empty! Please assign buttons in the Inspector.");
            return;
        }
        if (cardDescriptionTexts == null || cardDescriptionTexts.Count == 0)
        {
            Debug.LogError($"cardDescriptionTexts is null or empty! Please assign descriptionTexts in the Inspector.");
            return;
        }
        List<Card> currentCards = isPlayer ? playerGeneratedCards : enemyGeneratedCards;
        for (int i = 0; i < cardButtons.Count; i++)
        {
            Button button = cardButtons[i];
            Text descriptionText = cardDescriptionTexts[i];
            if (button == null)
            {
                Debug.LogError($"Button {i} in cardButtons is null!");
                continue;
            }
            if (descriptionText == null)
            {
                Debug.LogError($"DescriptionText {i} in cardDescriptionTexts is null!");
                continue;
            }
            if (i < currentCards.Count)
            {
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<Text>().text = currentCards[i].Description;
                descriptionText.text = currentCards[i].Description;
                button.interactable = isPlayer;
                button.onClick.RemoveAllListeners();
                if (isPlayer)
                {
                    int index = i;
                    button.onClick.AddListener(() => OnPlayerCardSelected(index));
                }
            }
            else
            {
                button.gameObject.SetActive(false);
                descriptionText.text = "";
            }
        }
    }
    public void ShowEnemyCardSelection()
    {
        ShowCardSelection(false);
    }
    public void OnPlayerCardSelected(int cardIndex)
    {
        if (cardIndex >= 0 && cardIndex < playerGeneratedCards.Count)
        {
            playerSelectedCard = playerGeneratedCards[cardIndex];
            battleManager.Log($"Игрок выбрал карту: {playerSelectedCard.Description}");
            ApplyCardEffects(playerSelectedCard, battleManager.player, battleManager.enemy, true);
            cardSelectionPanel.SetActive(false);
            battleManager.PlayerActionPhase();
        }
        else
        {
            Debug.LogError("Неверный индекс карты игрока!");
        }
    }
    public void EnemyChooseCard()
    {
        if (battleManager.enemySkipNextTurn)
        {
            battleManager.Log($"Враг пропускает ход из-за эффекта.");
            cardSelectionPanel.SetActive(false);
            return;
        }

        if (enemyGeneratedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, enemyGeneratedCards.Count);
            enemySelectedCard = enemyGeneratedCards[randomIndex];
            ApplyCardEffects(enemySelectedCard, battleManager.enemy, battleManager.player, false);
            battleManager.Log($"Враг использовал карту: {enemySelectedCard.Description}");
        }
        else
        {
            Debug.LogError("Нет доступных карт для противника!");
        }
        cardSelectionPanel.SetActive(false);
    }
    void ApplyCardEffects(Card card, Character target, Character otherTarget, bool isPlayer)
    {
        if (card == null)
        {
            Debug.LogError("Card is null!");
            return;
        }

        switch (card.EffectType)
        {
            case "Урон":
                ApplyDamageEffects(card, target, card.EffectParams, isPlayer);
                break;
            case "Защита":
                ApplyDefenseEffects(card, target, otherTarget, card.EffectParams, isPlayer);
                break;
            case "Здоровье":
                ApplyHealthEffects(card, target, card.EffectParams, isPlayer);
                break;
            case "Супер":
                ApplySuperEffects(card, target, card.EffectParams, isPlayer);
                break;
            case "Контроль":
                ApplyEnemyControlEffects(card, target, otherTarget, card.EffectParams, isPlayer);
                break;
            default:
                Debug.LogError($"Unknown effect type: {card.EffectType}");
                break;
        }
    }
    void ApplyDamageEffects(Card card, Character target, Dictionary<string, float> effectParams, bool isPlayer)
    {
        if (effectParams.ContainsKey("physicalDamage"))
        {
            float damageAmount = effectParams["physicalDamage"];
            if (target is Player)
            {
                Player player = (Player)target;
                player.attackPowerModifier += damageAmount;
                battleManager.Log($"Игроку добавлено {damageAmount} к физическому урону (итого: {player.AttackPower})");
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.attackPowerModifier += damageAmount;
                battleManager.Log($"Врагу добавлено {damageAmount} к физическому урону (итого: {enemy.AttackPower})");
            }
            battleManager.UpdateUI();
        }
        if (effectParams.ContainsKey("spellDamage"))
        {
            float damageAmount = effectParams["spellDamage"];
            if (target is Player)
            {
                Player player = (Player)target;
                player.magicAttackPowerModifier += damageAmount;
                battleManager.Log($"Игроку добавлено {damageAmount} к магическому урону (итого: {player.MagicAttackPower})");
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.magicAttackPowerModifier += damageAmount;
                battleManager.Log($"Врагу добавлено {damageAmount} к магическому урону (итого: {enemy.MagicAttackPower})");
            }
            battleManager.UpdateUI();
        }
        if (effectParams.ContainsKey("rangedDamage"))
        {
            float damageAmount = effectParams["rangedDamage"];
            if (target is Player)
            {
                Player player = (Player)target;
                player.rangedAttackPowerModifier += damageAmount;
                battleManager.Log($"Игроку добавлено {damageAmount} к урону дальней атаки (итого: {player.RangedAttackPower})");
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.rangedAttackPowerModifier += damageAmount;
                battleManager.Log($"Врагу добавлено {damageAmount} к урону дальней атаки (итого: {enemy.RangedAttackPower})");
            }
            battleManager.UpdateUI();
        }
        if (effectParams.ContainsKey("globalDamage"))
        {
            float damageAmount = effectParams["globalDamage"];
            if (target is Player)
            {
                Player player = (Player)target;
                player.attackPowerModifier += damageAmount;
                player.magicAttackPowerModifier += damageAmount;
                player.rangedAttackPowerModifier += damageAmount;
                battleManager.Log($"Весь урон игрока увеличен на {damageAmount} (физ: {player.AttackPower}, маг: {player.MagicAttackPower}, дальн: {player.RangedAttackPower})");
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.attackPowerModifier += damageAmount;
                enemy.magicAttackPowerModifier += damageAmount;
                enemy.rangedAttackPowerModifier += damageAmount;
                battleManager.Log($"Весь урон врага увеличен на {damageAmount} (физ: {enemy.AttackPower}, маг: {enemy.MagicAttackPower}, дальн: {enemy.RangedAttackPower})");
            }
            battleManager.UpdateUI();
        }
    }
    void ApplyDefenseEffects(Card card, Character target, Character otherTarget, Dictionary<string, float> effectParams, bool isPlayer)
    {
        if (effectParams.ContainsKey("invisibility"))
        {
            battleManager.Log("Инвизибилити применена");
            if (isPlayer)
            {
                battleManager.enemySkipNextTurn = true;
                battleManager.Log($"Враг пропустит следующий ход!");
            }
            else
            {
                battleManager.playerSkipNextTurn = true;
                battleManager.Log($"Игрок пропустит следующий ход!");
            }

            battleManager.UpdateUI();
        }
    }
    void ApplyHealthEffects(Card card, Character target, Dictionary<string, float> effectParams, bool isPlayer)
    {
        if (effectParams.ContainsKey("heal"))
        {
            float healAmount = effectParams["heal"];
            battleManager.Log("Исцеление");
            target.currentHealth = Mathf.Clamp(target.currentHealth + (int)healAmount, 0, target.maxHealth);
            battleManager.Log($"Восстановлено {healAmount} здоровья (итого: {target.currentHealth})");
            battleManager.UpdateUI();
        }
        if (effectParams.ContainsKey("superReduction"))
        {
            float superReductionAmount = effectParams["superReduction"];
            battleManager.Log("Уменьшение супер силы");
            if (target is Player)
            {
                Player player = (Player)target;
                player.superGauge -= superReductionAmount;
                player.superGauge = Mathf.Clamp(player.superGauge, 0, player.superThreshold);
                battleManager.Log($"Показатель супер силы уменьшен на {superReductionAmount} (итого: {player.superGauge})");
                battleManager.UpdateUI();
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.superGauge -= superReductionAmount;
                enemy.superGauge = Mathf.Clamp(enemy.superGauge, 0, enemy.superGauge);
                battleManager.Log($"Показатель супер силы игрока уменьшен на {superReductionAmount} (итого: {enemy.superGauge})");
                battleManager.UpdateUI();
            }
        }
    }
    void ApplySuperEffects(Card card, Character target, Dictionary<string, float> effectParams, bool isPlayer)
    {
        if (effectParams.ContainsKey("superCharge"))
        {
            battleManager.Log("Увеличение супер силы");
            float chargeAmount = effectParams["superCharge"];
            if (target is Player)
            {
                Player player = (Player)target;
                player.AddToSuperGauge(chargeAmount);
                battleManager.Log($"Добавлено {chargeAmount} к супер-шкале игрока (итого: {player.superGauge})");
                battleManager.UpdateUI();
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                enemy.AddToSuperGauge(chargeAmount);
                battleManager.Log($"Добавлено {chargeAmount} к супер-шкале врага (итого: {enemy.superGauge})");
                battleManager.UpdateUI();
            }
        }
        if (effectParams.ContainsKey("superChargeIfEmpty"))
        {
            battleManager.Log("Увеличение супер силы, если она пуста");
            float chargeAmount = effectParams["superChargeIfEmpty"];
            if (target is Player)
            {
                Player player = (Player)target;
                if (player.superGauge == 0)
                {
                    player.AddToSuperGauge(chargeAmount);
                    battleManager.Log($"Добавлено {chargeAmount} к супер-шкале игрока (шкала была пуста, итого: {player.superGauge})");
                    battleManager.UpdateUI();
                }
            }
            else if (target is EnemyBot)
            {
                EnemyBot enemy = (EnemyBot)target;
                if (enemy.superGauge == 0)
                {
                    enemy.AddToSuperGauge(chargeAmount);
                    battleManager.Log($"Добавлено {chargeAmount} к супер-шкале врага (шкала была пуста, итого: {enemy.superGauge})");
                    battleManager.UpdateUI();
                }
            }
        }
    }
    void ApplyEnemyControlEffects(Card card, Character target, Character otherTarget, Dictionary<string, float> effectParams, bool isPlayer)
    {
        if (effectParams.ContainsKey("immobilize"))
        {
            battleManager.Log("Обездвиживание применена");
            if (isPlayer)
            {
                battleManager.enemySkipNextTurn = true;
                battleManager.Log($"Враг пропустит следующий ход!");
            }
            else
            {
                battleManager.playerSkipNextTurn = true;
                battleManager.Log($"Игрок пропустит следующий ход!");
            }
            battleManager.UpdateUI();
        }
    }
    void OnCardsGenerated()
    {
        GenerateCardsForTurn();

        if (isPlayerTurn)
        {
            ShowCardSelection(true);
        }
    }
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
    }
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
