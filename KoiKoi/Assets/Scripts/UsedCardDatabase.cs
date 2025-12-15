using System;
using System.Collections.Generic;
using UnityEngine;

namespace KoiKoiProject
{
    public class UsedCardDatabase : MonoBehaviour
    {
        public static UsedCardDatabase Instance { get; private set; }

        private HashSet<Card> usedCards = new HashSet<Card>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool IsUsed(Card card)
        {
            return usedCards.Contains(card);
        }

        public bool TryAdd(Card card)
        {
            return usedCards.Add(card); // false если уже была
        }

        public void ClearAll()
        {
            usedCards.Clear();
        }

    }
}
