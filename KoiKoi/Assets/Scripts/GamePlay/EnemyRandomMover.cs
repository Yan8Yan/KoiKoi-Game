using UnityEngine;
using KoiKoiProject;
using System.Collections.Generic;

public class EnemyRandomMover : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private HandController3D enemyHand;

    [Header("Table")]
    [SerializeField] private Transform[] tableSlots;

    [Header("Settings")]
    [SerializeField] private int maxAttempts = 200;

    [SerializeField] private PlayerController enemyPlayer;

    [SerializeField] private CardCaptureManager captureManager;

    public bool TryMakeRandomMove()
    {
        if (enemyHand == null)
        {
            Debug.LogError("EnemyRandomMover: enemyHand не назначен");
            return false;
        }

        if (enemyPlayer == null)
        {
            Debug.LogError("EnemyRandomMover: enemyPlayer не назначен");
            return false;
        }

        if (captureManager == null)
        {
            Debug.LogError("EnemyRandomMover: captureManager не назначен");
            return false;
        }

        if (tableSlots == null || tableSlots.Length == 0)
        {
            Debug.LogError("EnemyRandomMover: tableSlots не назначены");
            return false;
        }

        if (enemyHand.transform.childCount == 0)
        {
            Debug.Log("У противника нет карт в руке");
            return false;
        }

        List<Transform> enemyCards = GetEnemyCardsShuffled();

        foreach (Transform enemyCard in enemyCards)
        {
            CardDisplay3D enemyDisplay = enemyCard.GetComponent<CardDisplay3D>();

            if (enemyDisplay == null)
                continue;

            Card enemyCardData = enemyDisplay.CardData();

            foreach (Transform slot in tableSlots)
            {
                if (slot.childCount == 0)
                    continue;

                Transform tableCard = slot.GetChild(0);

                CardDisplay3D tableDisplay = tableCard.GetComponent<CardDisplay3D>();

                if (tableDisplay == null)
                    continue;

                Card tableCardData = tableDisplay.CardData();

                if (enemyCardData.month != tableCardData.month)
                    continue;

                PlaceCardInSlot(enemyCard, slot);

                captureManager.CaptureCard(enemyCard, enemyCardData, enemyPlayer);
                captureManager.CaptureCard(tableCard, tableCardData, enemyPlayer);

                enemyPlayer.CheckForYaku();

                Debug.Log("Противник положил карту правильно и захватил пару: " + enemyCardData.month);

                return true;
            }
        }

        Debug.Log("Противник не нашёл подходящую карту для захвата");
        return false;
    }

    private List<Transform> GetEnemyCardsShuffled()
    {
        List<Transform> cards = new List<Transform>();

        for (int i = 0; i < enemyHand.transform.childCount; i++)
        {
            cards.Add(enemyHand.transform.GetChild(i));
        }

        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);

            Transform temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        return cards;
    }

    private bool CanPlaceCard(Transform card, Transform slot)
    {
        if (slot.childCount == 0)
            return true;

        CardDisplay3D enemyCardDisplay = card.GetComponent<CardDisplay3D>();
        CardDisplay3D tableCardDisplay = slot.GetChild(0).GetComponent<CardDisplay3D>();

        if (enemyCardDisplay == null || tableCardDisplay == null)
            return false;

        Card enemyCardData = enemyCardDisplay.CardData();
        Card tableCardData = tableCardDisplay.CardData();

        return enemyCardData.month == tableCardData.month;
    }

    private void PlaceCardInSlot(Transform card, Transform slot)
    {
        card.SetParent(slot, true);

        card.position = slot.position + Vector3.up * 0.05f;
        card.rotation = Quaternion.Euler(0, 180, 0);

        Vector3 desiredScale = Vector3.one * 2.2f;
        Vector3 parentScale = slot.lossyScale;

        card.localScale = new Vector3(
            desiredScale.x / parentScale.x,
            desiredScale.y / parentScale.y,
            desiredScale.z / parentScale.z
        );

        enemyHand.RemoveCardFromHand(card.gameObject);
    }
}