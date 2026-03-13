using System;
using System.Collections.Generic;
using UnityEngine;

namespace KoiKoiProject
{
    public class TableCardSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject cardPrefab;       // префаб карты
        [SerializeField] private List<Transform> tableSlots; 

        [SerializeField] private Vector3 tableCardScale = new Vector3(0.22f, 0.22f, 0.22f); // масштаб карт на столе

        private void Start()
        {
            SpawnCardsInSlots();
        }

        public void SpawnCardsInSlots()
        {
            if (tableSlots.Count < 8)
            {
                Debug.LogError("Need at least 8 table slots!");
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                Card drawnCard = DeckManager.Instance.DrawCard();

                if (drawnCard == null)
                {
                    Debug.Log("Deck empty");
                    return;
                }

                Transform slot = tableSlots[i];
                GameObject cardObj = Instantiate(cardPrefab);

                cardObj.transform.SetParent(slot, false);
                cardObj.transform.localPosition = Vector3.zero;
                cardObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
                cardObj.transform.localScale = tableCardScale;

                var display = cardObj.GetComponent<CardDisplay3D>();
                display.SetCard(drawnCard);
            }

        }
        public void ResetTable()
        {
            foreach (var slot in tableSlots)
            {
                if (slot.childCount > 0) 
                {
                    for (int i = slot.childCount - 1; i >= 0; i--)
                    {
                        Destroy(slot.GetChild(i).gameObject);
                    }
                }
            }
        }


    }
    }

