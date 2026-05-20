using System.Collections.Generic;
using UnityEngine;

namespace KoiKoiProject
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance;

        [SerializeField] private CardDatabase cardDatabase;

        private List<Card> deck = new List<Card>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            BuildDeck();
            ShuffleDeck();
        }

        void BuildDeck()
        {
            deck.Clear();

            foreach (Card card in cardDatabase.allCards)
            {
                deck.Add(card);
            }
        }

        void ShuffleDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                Card temp = deck[i];
                int randomIndex = Random.Range(i, deck.Count);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
        }

        public Card DrawCard()
        {
            if (deck.Count == 0)
                return null;

            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        public void ResetDeck()
        {
            BuildDeck();
            ShuffleDeck();
        }
    }
}