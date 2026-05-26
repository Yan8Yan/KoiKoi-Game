using KoiKoiProject;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private HandController3D mainHand;
    [SerializeField] private HandController3D enemyHand;
    [SerializeField] private TableCardSpawner tableSpawner;
    [SerializeField] private PlayerController mainPlayer;
    [SerializeField] private CardCaptureManager enemyCapturedCards;

    [ContextMenu("Restart Round")]
    public void RestartRound()
    {
        mainHand.ResetHand();
        enemyHand.ResetHand();

        mainPlayer.ResetCapturedCards();
        enemyCapturedCards.ResetCapturedCardTransder();

        deckManager.ResetDeck();
        tableSpawner.ResetTable();
        for (int i = 0; i < 8; i++)
        {
            mainHand.AddCard();
            enemyHand.AddCard();
        }
        tableSpawner.SpawnCardsInSlots();

        Debug.Log("Round restarted, scores preserved.");
    }
}