using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RoundManager roundManager;

    [Header("Match Settings")]
    [SerializeField] private int maxRounds = 3;

    private int currentRound = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartMatch();
    }

    public void StartMatch()
    {
        currentRound = 1;

        roundManager.ResetMatchScores();
        roundManager.RestartRound();

        Debug.Log("Матч начался");
        Debug.Log($"Раунд {currentRound}");
    }

    public void EndRound()
    {
        Debug.Log($"Раунд {currentRound} завершен");

        int playerRoundScore = roundManager.MainPlayer.roundScore;
        int enemyRoundScore = roundManager.EnemyPlayer.roundScore;

        roundManager.MainPlayer.matchScore += playerRoundScore;
        roundManager.EnemyPlayer.matchScore += enemyRoundScore;

        Debug.Log($"Очки игрока за раунд: {playerRoundScore}");
        Debug.Log($"Очки врага за раунд: {enemyRoundScore}");

        Debug.Log($"Всего очков игрока: {roundManager.MainPlayer.matchScore}");
        Debug.Log($"Всего очков врага: {roundManager.EnemyPlayer.matchScore}");

        if (currentRound >= maxRounds)
        {
            EndMatch();
            return;
        }

        currentRound++;

        Debug.Log($"Начинается раунд {currentRound}");

        roundManager.RestartRound();

        GameManager.Instance.StartPlayerTurn();
    }

    private void EndMatch()
    {
        int playerScore = roundManager.MainPlayer.matchScore;
        int enemyScore = roundManager.EnemyPlayer.matchScore;

        Debug.Log("Матч завершен");

        Debug.Log($"Игрок: {playerScore}");
        Debug.Log($"Противник: {enemyScore}");

        if (playerScore > enemyScore)
        {
            Debug.Log("Игрок победил матч!");
        }
        else if (enemyScore > playerScore)
        {
            Debug.Log("Противник победил матч!");
        }
        else
        {
            Debug.Log("Ничья!");
        }

        GameManager.Instance.SetMatchEnded();
    }
}
