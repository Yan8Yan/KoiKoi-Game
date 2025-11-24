using System.Collections.Generic;
using UnityEngine;

namespace KoiKoiProject
{
    public class TableCardSpawner : MonoBehaviour
    {
        [Header("Card Database")]
        [SerializeField] private CardDatabase cardDatabase;   // все карты
        [SerializeField] private List<Card> playerHand;       // карты в руке игрока

        [Header("Spawn Settings")]
        [SerializeField] private GameObject cardPrefab;       // префаб карты
        [SerializeField] private Transform tableParent;       // родитель для карт на столе
        [SerializeField] private int cardsOnTableCount = 5;  // сколько карт на столе
        [SerializeField] private float rowSpacing = 0.15f;     // расстояние между картами
        [SerializeField] private Vector3 tableCardScale = new Vector3(0.1f, 0.1f, 0.1f); // масштаб карт на столе

        private void Start()
        {
            SpawnCardsInRow();
        }

        private void SpawnCardsInRow()
        {
            // Получаем список карт, которых нет в руке игрока
            List<Card> availableCards = new List<Card>();
            foreach (var card in cardDatabase.allCards)
            {
                if (!playerHand.Contains(card))
                    availableCards.Add(card);
            }

            if (availableCards.Count < cardsOnTableCount)
            {
                Debug.LogError("Недостаточно карт для генерации на столе!");
                return;
            }

            // 2️⃣ Случайно выбираем карты без повторов
            List<Card> selectedCards = new List<Card>();
            for (int i = 0; i < cardsOnTableCount; i++)
            {
                int index = Random.Range(0, availableCards.Count);
                selectedCards.Add(availableCards[index]);
                availableCards.RemoveAt(index);
            }

            // 3Вычисляем стартовую позицию для ряда, чтобы ряд был по центру стола
            float totalWidth = (cardsOnTableCount - 1) * rowSpacing;
            Vector3 startPos = tableParent.position - new Vector3(totalWidth / 2f, 0, 0);

            //  Создаём карты и расставляем в ряд
            for (int i = 0; i < selectedCards.Count; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, tableParent);

                // Позиция относительно родителя
                cardObj.transform.localPosition = new Vector3(i * rowSpacing, 0, 0);
                cardObj.transform.localRotation = Quaternion.Euler(0, 180, 0); // повернуть лицом к игроку
                cardObj.transform.localScale = tableCardScale;                 // нормальный масштаб для стола

                // Назначаем данные карты через CardDisplay3D
                var cardDisplay = cardObj.GetComponent<CardDisplay3D>();
                if (cardDisplay != null)
                    cardDisplay.SetCard(selectedCards[i]);
            }
        }
    }
}
