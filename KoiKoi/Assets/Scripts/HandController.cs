using UnityEngine;
using System.Collections.Generic;
using KoiKoiProject;

public class HandController3D : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Transform cardTransform;
    public CardDatabase cardDatabase;

    [Header("Veer Settings")]
    public float fanAngle = 50f;      // Общий угол веера
    public float horizontalSpread = 2.0f;  // Расстояние между картами по X
    public float verticalLift = 0.05f;     // Подъём крайних карт по Y
    public float depthOffset = 0.1f;       // Смещение по Z для видимости нижних карт

    [Header("Hand")]
    public List<GameObject> cardsInHand = new List<GameObject>();

    void Start()
    {
        // Для примера добавим 5 карт
        for (int i = 0; i < 8; i++)
            AddCard();
    }

    public void AddCard()
    {
        Card drawnCard = DeckManager.Instance.DrawCard();

        if (drawnCard == null)
        {
            Debug.Log("Deck is empty!");
            return;
        }

        GameObject newCard = Instantiate(
            cardPrefab,
            cardTransform.position,
            Quaternion.Euler(0, 180, 0),
            cardTransform
        );

        newCard.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        CardDisplay3D display = newCard.GetComponent<CardDisplay3D>();
        display.SetCard(drawnCard);

        cardsInHand.Add(newCard);
        UpdateCardVisuals();
    }


    private void UpdateCardVisuals()
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        float midIndex = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];
            float offsetIndex = i - midIndex;

            // Проверка, чтобы midIndex не был 0
            float angle = 0f;
            if (midIndex != 0)
            {
                angle = (fanAngle / 2f) * (offsetIndex / midIndex);
            }
            card.transform.localRotation = Quaternion.Euler(0f, 180f + angle, 0f);

            // Позиция
            float x = offsetIndex * horizontalSpread;
            float y = Mathf.Abs(offsetIndex) * verticalLift;
            float z = Mathf.Abs(offsetIndex) * depthOffset;

            card.transform.localPosition = new Vector3(x, y, z);
        }
    }

    public void RefreshHand()
    {
        UpdateCardVisuals();
    }

    public void RemoveCardFromHand(GameObject card)
    {
        if (cardsInHand.Contains(card))
        {
            cardsInHand.Remove(card);
            UpdateCardVisuals(); // чтобы рука перестроилась
        }
    }

}
