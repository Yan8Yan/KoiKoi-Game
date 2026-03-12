using UnityEngine;

namespace KoiKoiProject
{
    public class DeckTurnResolver : MonoBehaviour
    {
        [SerializeField] private TableSlotManager slotManager;
        [SerializeField] private HandController3D handController;
        [SerializeField] private CardCaptureManager captureManager;

        public void ResolveDeckDraw(PlayerController player)
        {
            Card drawnCard = DeckManager.Instance.DrawCard();

            if (drawnCard == null)
                return;

            GameObject cardObj = Instantiate(handController.cardPrefab);

            CardDisplay3D display = cardObj.GetComponent<CardDisplay3D>();
            display.SetCard(drawnCard);

            Transform slot = slotManager.GetEmptySlot();

            if (slot == null)
                return;

            PlaceCard(cardObj.transform, slot);

            CheckDeckMatch(cardObj.transform, drawnCard, player);
        }

        private void PlaceCard(Transform card, Transform slot)
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
        }

        private void CheckDeckMatch(Transform drawnCardTransform, Card drawnCardData, PlayerController player)
        {
            foreach (Transform slot in slotManager.GetAllSlots())
            {
                if (slot.childCount == 0)
                    continue;

                Transform tableCard = slot.GetChild(0);

                if (tableCard == drawnCardTransform)
                    continue;

                CardDisplay3D display = tableCard.GetComponent<CardDisplay3D>();
                Card tableData = display.CardData();

                if (tableData.month == drawnCardData.month)
                {
                    captureManager.CaptureCard(drawnCardTransform, drawnCardData, player);
                    captureManager.CaptureCard(tableCard, tableData, player);

                    player.CheckForYaku();
                    return;
                }
            }
        }
    }
}