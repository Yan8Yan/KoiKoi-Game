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
        [SerializeField] private List<Transform> tableSlots;  // слоты на столе (первые 4 слота)

        [SerializeField] private Vector3 tableCardScale = new Vector3(0.22f, 0.22f, 0.22f); // масштаб карт на столе

        private void Start()
        {
            SpawnCardsInSlots();
        }

        private void SpawnCardsInSlots()
        {
            if (tableSlots.Count < 8)
            {
                Debug.LogError("Необходимо минимум 8 слотов на столе!");
                return;
            }

            // Получаем список карт, которых нет в руке игрока
            List<Card> availableCards = new List<Card>();
            foreach (var card in cardDatabase.allCards)
            {
                if (!playerHand.Contains(card))
                    availableCards.Add(card);
            }

            if (availableCards.Count < 8)
            {
                Debug.LogError("Недостаточно карт для генерации на столе!");
                return;
            }

            // Случайно выбираем 8 карт без повторов
            List<Card> selectedCards = new List<Card>();
            for (int i = 0; i < 8; i++)
            {
                int index = Random.Range(0, availableCards.Count);
                selectedCards.Add(availableCards[index]);
                availableCards.RemoveAt(index);
            }

            // Размещаем карты в первых восьми
            for (int i = 0; i < 8; i++)
            {
                Transform slot = tableSlots[i];
                GameObject cardObj = Instantiate(cardPrefab);

                // Привязываем к слоту
                cardObj.transform.SetParent(slot, false); // false = сохраняем локальные координаты
                cardObj.transform.localPosition = Vector3.zero;
                cardObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
                cardObj.transform.localScale = tableCardScale;

                // Назначаем данные карты через CardDisplay3D
                var cardDisplay = cardObj.GetComponent<CardDisplay3D>();
                if (cardDisplay != null)
                    cardDisplay.SetCard(selectedCards[i]);
            }
        }
    }
}
