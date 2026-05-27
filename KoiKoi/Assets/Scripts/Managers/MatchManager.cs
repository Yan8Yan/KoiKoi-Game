using System.Collections;
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
        StartCoroutine(StartMatchRoutine());
    }

    public void EndRound()
    {
        StartCoroutine(EndRoundRoutine());
    }

    private void EndMatch()
    {
        int playerScore = roundManager.MainPlayer.matchScore;
        int enemyScore = roundManager.EnemyPlayer.matchScore;

        Debug.Log("Матч завершен");
        Debug.Log($"Игрок: {playerScore}");
        Debug.Log($"Противник: {enemyScore}");

        string result;

        if (playerScore > enemyScore)
        {
            Debug.Log("Игрок победил матч!");
            result = "ПОБЕДА ИГРОКА";
        }
        else if (enemyScore > playerScore)
        {
            Debug.Log("Противник победил матч!");
            result = "ПОБЕДА ВРАГА";
        }
        else
        {
            Debug.Log("Ничья!");
            result = "НИЧЬЯ";
        }

        GameManager.Instance.SetMatchEnded();
        StartCoroutine(UIManager.Instance.ShowMatchEnd(result));
    }

    private IEnumerator ShowRoundStart()
    {
        yield return UIManager.Instance.ShowRoundBanner($"РАУНД {currentRound}", 2f);

        Debug.Log("Матч начался");
    }

    private IEnumerator EndRoundRoutine()
    {
        Debug.Log($"Раунд {currentRound} завершен");

        int playerRoundScore = roundManager.MainPlayer.roundScore;
        int enemyRoundScore = roundManager.EnemyPlayer.roundScore;

        roundManager.MainPlayer.matchScore += playerRoundScore;
        roundManager.EnemyPlayer.matchScore += enemyRoundScore;

        string roundResult =
            playerRoundScore > enemyRoundScore ? "Игрок победил раунд"
            : enemyRoundScore > playerRoundScore ? "Враг победил раунд"
            : "Ничья";

        yield return UIManager.Instance.ShowRoundBanner(roundResult, 2f);

        if (currentRound >= maxRounds)
        {
            EndMatch();
            yield break;
        }

        currentRound++;

        yield return UIManager.Instance.ShowRoundBanner($"РАУНД {currentRound}", 2f);

        roundManager.RestartRound();
        GameManager.Instance.StartPlayerTurn();
    }

    private IEnumerator StartMatchRoutine()
    {
        yield return null;

        currentRound = 1;

        roundManager.ResetMatchScores();
        roundManager.RestartRound();

        yield return UIManager.Instance.ShowRoundBanner($"РАУНД {currentRound}", 2f);

        Debug.Log("Матч начался");
    }
}
