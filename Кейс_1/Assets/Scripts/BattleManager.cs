using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Player player;
    public EnemyBot enemy;
    public Text logText;
    public Text turnCounterText;
    public Text healthPlayerText;
    public Text healthEnemyText;
    public Text superStatusText;
    public Button meleeAttackButton;
    public Button rangedAttackButton;
    public Button magicAttackButton;
    public Button skipTurnButton;
    private int turn = 1;
    private const int maxTurns = 10;
    private bool isPlayerTurn = true;
    private bool actionTaken = false;
    public CardManager cardManager;
    public event Action OnCardsGenerated;
    public bool enemySkipNextTurn = false;
    public bool playerSkipNextTurn = false;
    public static BattleManager Instance { get; private set; }
    public int logLength = 5;
    private Queue<string> logMessages = new Queue<string>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UpdateUI();
        Log("Бой начался!");
        StartTurn();
    }
    void StartTurn()
    {
        isPlayerTurn = true;
        actionTaken = false;
        EnablePlayerActions(false);

        if (isPlayerTurn && playerSkipNextTurn)
        {
            playerSkipNextTurn = false;
            Log($"{player.characterName} пропускает ход!");
            EndPlayerTurn();
            return;
        }
        else if (!isPlayerTurn && enemySkipNextTurn)
        {
            enemySkipNextTurn = false;
            Log($"{enemy.characterName} пропускает ход!");
            EndTurn();
            return;
        }

        GenerateCards();
    }
    public void PlayerActionPhase()
    {
        EnablePlayerActions(true);
    }
    public void OnMeleeAttack()
    {
        if (!isPlayerTurn || actionTaken) return;

        player.MeleeAttack(enemy);
        actionTaken = true;
        EndPlayerTurn();
    }
    public void OnRangedAttack()
    {
        if (!isPlayerTurn || actionTaken) return;

        player.RangedAttack(enemy);
        actionTaken = true;
        EndPlayerTurn();
    }
    public void OnMagicAttack()
    {
        if (!isPlayerTurn || actionTaken) return;

        player.MagicAttack(enemy);
        actionTaken = true;
        EndPlayerTurn();
    }
    public void OnSuperAttack()
    {
        if (!isPlayerTurn || actionTaken) return;

        if (player.CanUseSuper())
        {
            player.UseSuper(enemy);
            Log($"{player.characterName} использует СУПЕРУДАР: 25 урона!");
            actionTaken = true;
            EndPlayerTurn();
        }
        else
        {
            Log("Недостаточно энергии для суперудара!");
            UpdateUI();
        }
    }
    public void OnSkipTurn()
    {
        if (!isPlayerTurn || actionTaken) return;
        player.SkipTurn();
        Log($"{player.characterName} пропускает ход.");
        actionTaken = true;
        EndPlayerTurn();
    }
    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        EnablePlayerActions(false);
        UpdateUI();
        StartCoroutine(EnemyTurnCoroutine());
    }
    private IEnumerator EnemyTurnCoroutine()
    {
        cardManager.ShowCardSelection(false);
        yield return new WaitForSeconds(3f);
        cardManager.EnemyChooseCard();
        yield return new WaitForSeconds(2f);
        if (enemySkipNextTurn)
        {
            enemySkipNextTurn = false;
            Log($"{enemy.characterName} пропускает ход!");
            EndTurn();
            yield break;
        }
        if (!enemy.IsAlive())
        {
            Log($"{enemy.characterName} мёртв.");
            EndBattle();
            yield break;
        }
        string logMessage = enemy.BotMakeMove(player);
        Log(logMessage);
        UpdateUI();
        EndTurn();
    }
    public void EndTurn()
    {
        if (turn >= maxTurns)
        {
            EndBattle();
            return;
        }
        player.ResetModifiers();
        enemy.ResetModifiers();

        turn++;
        isPlayerTurn = true;
        actionTaken = false;

        StartTurn();
    }
    private void EndBattle()
    {
        string result = "";
        if (player.currentHealth > enemy.currentHealth)
            result = "Игрок побеждает!";
        else if (enemy.currentHealth > player.currentHealth)
            result = "Противник побеждает!";
        else
            result = "Ничья!";

        Log($"Бой окончен.\nРезультат: {result}");
    }
    public void UpdateUI()
    {
        turnCounterText.text = $"Ход: {turn}/{maxTurns}";
        healthPlayerText.text = $"Здоровье: {player.currentHealth}";
        healthEnemyText.text = $"Здоровье: {enemy.currentHealth}";
        superStatusText.text = player.CanUseSuper() ? "Суперудар готов!" : $"Супер-шкала: {player.superGauge:F0}/{player.superThreshold:F0}";
    }
    public void Log(string message)
    {
        logMessages.Enqueue(message);

        if (logMessages.Count > logLength)
        {
            logMessages.Dequeue();
        }

        UpdateLogText();
    }
    void UpdateLogText()
    {
        logText.text = string.Join("\n", logMessages.ToArray());
    }
    void EnablePlayerActions(bool enable)
    {
        meleeAttackButton.interactable = enable;
        rangedAttackButton.interactable = enable;
        magicAttackButton.interactable = enable;
        skipTurnButton.interactable = enable;
    }
    void GenerateCards()
    {
        OnCardsGenerated?.Invoke();
    }
}