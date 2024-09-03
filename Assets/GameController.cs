using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("People")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject enemyAura;

    [Header("Health Bar")]
    [SerializeField] private Slider playerHealth;
    [SerializeField] private Slider enemyHealth;

    [Header("Character Selection")]
    [SerializeField] private GameObject player1Selection;
    [SerializeField] private GameObject player2Selection;
    [SerializeField] private GameObject enemySelection;

    [Header("End Screens")]
    [SerializeField] private GameObject enemyDefeated;
    [SerializeField] private GameObject enemyConverted;
    [SerializeField] private GameObject enemyWon;

    [Header("Player Actions")]
    [SerializeField] private float player1AttackValue = 0.25f;
    [SerializeField] private float player1EmpathyValue = 0.5f;
    [SerializeField] private float player2AttackValue = 0.25f;
    [SerializeField] private float player2EmpathyValue = 0.5f;

    [Header("Action Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button empathyButton;

    [Header("Enemy Stats")]
    [SerializeField] private float enemyAttackValue = 0.25f;
    [SerializeField] private float enemyHealthValue = 1.0f;

    private List<GameObject> turnOrder;
    private int currentTurnIndex;

    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        turnOrder = new List<GameObject> { player1, player2, enemy };
        currentTurnIndex = 0;
        enemyHealth.maxValue = enemyHealthValue;
        enemyHealth.value = enemyHealthValue;
        UpdateTurn();
    }

    private void UpdateTurn()
    {
        DeactivateSelections();
        DisableActionButtons();

        if (turnOrder[currentTurnIndex] == player1)
        {
            ActivatePlayer1Turn();
        }
        else if (turnOrder[currentTurnIndex] == player2)
        {
            ActivatePlayer2Turn();
        }
        else if (turnOrder[currentTurnIndex] == enemy)
        {
            ActivateEnemyTurn();
        }
    }

    private void DeactivateSelections()
    {
        player1Selection.SetActive(false);
        player2Selection.SetActive(false);
        enemySelection.SetActive(false);
    }

    private void DisableActionButtons()
    {
        attackButton.interactable = false;
        empathyButton.interactable = false;
    }

    private void ActivatePlayer1Turn()
    {
        player1Selection.SetActive(true);
        EnableActionButtons();
    }

    private void ActivatePlayer2Turn()
    {
        player2Selection.SetActive(true);
        EnableActionButtons();
    }

    private void ActivateEnemyTurn()
    {
        enemySelection.SetActive(true);
        StartCoroutine(EnemyAttackRoutine());
    }

    private void EnableActionButtons()
    {
        attackButton.interactable = true;
        empathyButton.interactable = true;
    }

    private IEnumerator EnemyAttackRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        playerHealth.value -= playerHealth.maxValue * enemyAttackValue;

        if (playerHealth.value <= 0)
        {
            enemyWon.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            NextTurn();
        }
    }

    public void EmpathyButton()
    {
        AdjustEnemyAura();

        if (enemyAura.GetComponent<Image>().color.a == 0)
        {
            enemyConverted.SetActive(true);
        }
        else
        {
            NextTurn();
        }
    }

    private void AdjustEnemyAura()
    {
        Color auraColor = enemyAura.GetComponent<Image>().color;
        float empathyValue = turnOrder[currentTurnIndex] == player1 ? player1EmpathyValue : player2EmpathyValue;
        auraColor.a = Mathf.Max(0, auraColor.a - empathyValue);
        enemyAura.GetComponent<Image>().color = auraColor;
    }

    public void AttackButton()
    {
        AdjustEnemyHealth();

        if (enemyHealth.value == 0)
        {
            enemyDefeated.SetActive(true);
        }
        else
        {
            NextTurn();
        }
    }

    private void AdjustEnemyHealth()
    {
        float attackValue = turnOrder[currentTurnIndex] == player1 ? player1AttackValue : player2AttackValue;
        enemyHealth.value -= enemyHealth.maxValue * attackValue;
    }

    private void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        UpdateTurn();
    }

    public void ResetSceneButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
