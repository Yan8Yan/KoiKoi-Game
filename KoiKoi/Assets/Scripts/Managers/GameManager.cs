using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStates CurrentState { get; private set; }
    public TurnState CurrentTurn { get; private set; }

    private bool playerPlayedCardThisTurn;

    [SerializeField] private float enemyTurnDelay = 3.5f;

    [SerializeField] private EnemyCardAgent enemyCardAgent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;
        playerPlayedCardThisTurn = false;

        Debug.Log("Твой ход");
    }

    public void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;

        Debug.Log("Ход противника");

        if (enemyCardAgent != null)
        {
            enemyCardAgent.BeginEnemyTurn();
        }
        else
        {
            Debug.LogError("EnemyCardAgent не назначен в GameManager");
        }
    }

    public bool CanPlayerPlayCard()
    {
        return CurrentTurn == TurnState.PlayerTurn && !playerPlayedCardThisTurn;
    }

    public void NotifyPlayerPlayedCard()
    {
        if (CurrentTurn != TurnState.PlayerTurn)
            return;

        playerPlayedCardThisTurn = true;

        Debug.Log("Игрок сыграл карту");
        StartEnemyTurn();
    }

    //Заглушка для врага, так как он глюпи не умеет играть
    //private void MakeEnemyTurn()
    //{
    //    Debug.Log("Противник пытается сделать рандомный ход");

    //    if (enemyRandomMover != null)
    //    {
    //        enemyRandomMover.TryMakeRandomMove();
    //    }
    //    else
    //    {
    //        Debug.LogError("EnemyRandomMover не назначен в GameManager");
    //    }

    //    StartPlayerTurn();
    //}

    public void NotifyEnemyPlayedCard()
    {
        if (CurrentTurn != TurnState.EnemyTurn)
            return;

        Debug.Log("Противник успешно сыграл карту");

        StartPlayerTurn();
    }

    public void SkipEnemyTurn()
    {
        if (CurrentTurn != TurnState.EnemyTurn)
            return;

        Debug.Log("Противник пропускает ход");

        StartPlayerTurn();
    }
}

public enum GameStates
{
    None,
}

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

