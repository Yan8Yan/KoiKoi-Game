using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using KoiKoiProject;
using DG.Tweening;

public class EnemyCardAgent : Agent
{
    [Header("Enemy")]
    [SerializeField] private HandController3D enemyHand;
    [SerializeField] private PlayerController enemyPlayer;

    [Header("Table")]
    [SerializeField] private Transform[] tableSlots;

    [Header("Game Services")]
    [SerializeField] private CardCaptureManager captureManager;
    [SerializeField] private DeckTurnResolver deckTurnResolver;

    [Header("ML Settings")]
    [Tooltip("Максимальное количество карт, которое может быть в руке противника.")]
    [SerializeField] private int maxHandSize = 8;

    private bool isEnemyTurnActive;

    /// <summary>
    /// GameManager вызывает этот метод, когда начинается ход противника.
    /// </summary>
    public void BeginEnemyTurn()
    {
        if (enemyHand == null || enemyPlayer == null || captureManager == null)
        {
            Debug.LogError("EnemyCardAgent: не назначены обязательные ссылки в Inspector.");
            return;
        }

        if (tableSlots == null || tableSlots.Length == 0)
        {
            Debug.LogError("EnemyCardAgent: не назначены tableSlots.");
            return;
        }

        if (enemyHand.transform.childCount == 0)
        {
            Debug.Log("EnemyCardAgent: у противника нет карт.");
            GameManager.Instance.SkipEnemyTurn();
            return;
        }

        if (!HasAnyLegalMove())
        {
            Debug.Log("EnemyCardAgent: нет возможных ходов.");
            GameManager.Instance.SkipEnemyTurn();
            return;
        }

        isEnemyTurnActive = true;
        RequestDecision();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Карты в руке противника.
        // Модель видит месяц каждой карты.
        for (int i = 0; i < maxHandSize; i++)
        {
            if (i < enemyHand.transform.childCount)
            {
                Transform cardTransform = enemyHand.transform.GetChild(i);
                CardDisplay3D display = cardTransform.GetComponent<CardDisplay3D>();

                if (display != null)
                {
                    Card cardData = display.CardData();
                    sensor.AddObservation((int)cardData.month);
                }
                else
                {
                    sensor.AddObservation(-1);
                }
            }
            else
            {
                sensor.AddObservation(-1);
            }
        }

        // Карты на столе.
        // Пустой слот передаётся как -1.
        for (int i = 0; i < tableSlots.Length; i++)
        {
            Transform slot = tableSlots[i];

            if (slot.childCount == 0)
            {
                sensor.AddObservation(-1);
                continue;
            }

            CardDisplay3D display = slot.GetChild(0).GetComponent<CardDisplay3D>();

            if (display != null)
            {
                Card cardData = display.CardData();
                sensor.AddObservation((int)cardData.month);
            }
            else
            {
                sensor.AddObservation(-1);
            }
        }

        // Сообщаем модели, что сейчас действительно ход противника.
        sensor.AddObservation(isEnemyTurnActive ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isEnemyTurnActive)
            return;

        int chosenMove = actions.DiscreteActions[0];

        int slotCount = tableSlots.Length;
        int chosenCardIndex = chosenMove / slotCount;
        int chosenSlotIndex = chosenMove % slotCount;

        if (chosenCardIndex < 0 || chosenCardIndex >= enemyHand.transform.childCount)
        {
            Debug.LogError("EnemyCardAgent: модель выбрала отсутствующую карту.");
            return;
        }

        if (chosenSlotIndex < 0 || chosenSlotIndex >= tableSlots.Length)
        {
            Debug.LogError("EnemyCardAgent: модель выбрала отсутствующий слот.");
            return;
        }

        Transform chosenCard = enemyHand.transform.GetChild(chosenCardIndex);
        Transform chosenSlot = tableSlots[chosenSlotIndex];

        CardDisplay3D cardDisplay = chosenCard.GetComponent<CardDisplay3D>();
        if (cardDisplay == null)
        {
            Debug.LogError("EnemyCardAgent: нет CardDisplay3D.");
            return;
        }

        Card enemyCardData = cardDisplay.CardData();

  
        if (chosenSlot.childCount == 0)
        {
            AnimateMove(chosenCard, chosenSlot, () =>
            {
                PlaceCardInSlot(chosenCard, chosenSlot);

                FinishSuccessfulMove(0.3f);
            });

            Debug.Log("Enemy placed card in empty slot: " + enemyCardData.month);
            return;
        }
        Transform tableCard = chosenSlot.GetChild(0);

        CardDisplay3D tableDisplay = tableCard.GetComponent<CardDisplay3D>();
        if (tableDisplay == null)
        {
            Debug.LogError("EnemyCardAgent: нет CardDisplay3D у карты на столе.");
            return;
        }

        Card tableCardData = tableDisplay.CardData();

        AnimateMove(chosenCard, chosenSlot, () =>
        {
            PlaceCardInSlot(chosenCard, chosenSlot);

            captureManager.CaptureCard(chosenCard, enemyCardData, enemyPlayer);
            captureManager.CaptureCard(tableCard, tableCardData, enemyPlayer);

            enemyPlayer.CheckForYaku();

            FinishSuccessfulMove(1f);
        });

        Debug.Log("Enemy captured pair: " + enemyCardData.month);
    }

    private void FailedAttempt(string message)
    {
        AddReward(-1f);

        Debug.Log(message + " Агент пробует снова.");

        // Ход не передаётся игроку.
        // Агент должен выбрать другую карту и/или другой слот.
        RequestDecision();
    }

    private void FinishSuccessfulMove(float reward)
    {
        AddReward(reward);

        isEnemyTurnActive = false;

        if (deckTurnResolver != null)
        {
            deckTurnResolver.ResolveDeckDraw(enemyPlayer);
        }
        EndEpisode();
        GameManager.Instance.NotifyEnemyPlayedCard();
    }

    private bool HasAnyLegalMove()
    {
        for (int cardIndex = 0; cardIndex < enemyHand.transform.childCount; cardIndex++)
        {
            for (int slotIndex = 0; slotIndex < tableSlots.Length; slotIndex++)
            {
                if (IsLegalMove(cardIndex, slotIndex))
                    return true;
            }
        }

        return false;
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

    /// <summary>
    /// Режим проверки без обученной модели.
    /// Если поставить Behavior Type = Heuristic Only,
    /// противник будет случайно выбирать карту и слот,
    /// но уже через новую агентную систему.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        for (int cardIndex = 0; cardIndex < enemyHand.transform.childCount; cardIndex++)
        {
            for (int slotIndex = 0; slotIndex < tableSlots.Length; slotIndex++)
            {
                if (IsLegalMove(cardIndex, slotIndex))
                {
                    discreteActions[0] = cardIndex * tableSlots.Length + slotIndex;
                    return;
                }
            }
        }

        discreteActions[0] = 0;
    }
    private bool IsLegalMove(int cardIndex, int slotIndex)
    {
        if (cardIndex < 0 || cardIndex >= enemyHand.transform.childCount)
            return false;

        if (slotIndex < 0 || slotIndex >= tableSlots.Length)
            return false;

        Transform card = enemyHand.transform.GetChild(cardIndex);
        Transform slot = tableSlots[slotIndex];

        CardDisplay3D enemyDisplay = card.GetComponent<CardDisplay3D>();

        if (enemyDisplay == null)
            return false;

        Card enemyCardData = enemyDisplay.CardData();
        bool hasMatchOnTable = HasMatchingCardOnTable(enemyCardData);

        // Если у выбранной карты есть совпадение, она обязана идти на совпадающую карту.
        if (hasMatchOnTable)
        {
            if (slot.childCount == 0)
                return false;

            CardDisplay3D tableDisplay = slot.GetChild(0).GetComponent<CardDisplay3D>();

            if (tableDisplay == null)
                return false;

            Card tableCardData = tableDisplay.CardData();

            return enemyCardData.month == tableCardData.month;
        }

        // Если совпадения для выбранной карты нет, её можно положить только в пустой слот.
        return slot.childCount == 0;
    }

    private bool HasMatchingCardOnTable(Card enemyCardData)
    {
        foreach (Transform slot in tableSlots)
        {
            if (slot == null || slot.childCount == 0)
                continue;

            CardDisplay3D tableDisplay = slot.GetChild(0).GetComponent<CardDisplay3D>();

            if (tableDisplay == null)
                continue;

            Card tableCardData = tableDisplay.CardData();

            if (enemyCardData.month == tableCardData.month)
                return true;
        }

        return false;
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!isEnemyTurnActive || enemyHand == null || tableSlots == null)
            return;

        int slotCount = tableSlots.Length;

        for (int cardIndex = 0; cardIndex < maxHandSize; cardIndex++)
        {
            for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
            {
                int moveIndex = cardIndex * slotCount + slotIndex;
                bool isLegal = IsLegalMove(cardIndex, slotIndex);


                actionMask.SetActionEnabled(0, moveIndex, isLegal);
            }
        }
    }

    private void AnimateMove(Transform card, Transform slot, System.Action onComplete)
    {
        Vector3 targetPos = slot.position + Vector3.up * 0.05f;
        Quaternion targetRot = Quaternion.Euler(0, 180, 0);

        card
            .DOMove(targetPos, 0.25f)
            .SetEase(Ease.OutQuad);

        card
            .DORotateQuaternion(targetRot, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
}
