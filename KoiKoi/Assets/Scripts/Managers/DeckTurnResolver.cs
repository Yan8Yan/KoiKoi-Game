using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace KoiKoiProject
{
    public class DeckTurnResolver : MonoBehaviour
    {
        [SerializeField] private TableSlotManager slotManager;
        [SerializeField] private HandController3D handController;
        [SerializeField] private Transform deckSpawnPoint;
        [SerializeField] private float deckDrawDelay = 1f;

        [Header("Players")]
        [SerializeField] private PlayerController mainPlayer;

        [SerializeField] private CardCaptureManager playerCaptureManager;
        [SerializeField] private CardCaptureManager enemyCaptureManager;

        public void ResolveDeckDraw(PlayerController player)
        {
            StartCoroutine(ResolveDeckDrawWithDelay(player));
        }

        private IEnumerator ResolveDeckDrawWithDelay(PlayerController player)
        {
            yield return new WaitForSeconds(deckDrawDelay);

            Card drawnCard = DeckManager.Instance.DrawCard();

            if (drawnCard == null)
                yield break;

            Transform slot = slotManager.GetEmptySlot();

            if (slot == null)
                yield break;

            GameObject cardObj = Instantiate(
                handController.cardPrefab,
                deckSpawnPoint.position,
                deckSpawnPoint.rotation
            );

            CardDisplay3D display = cardObj.GetComponent<CardDisplay3D>();
            display.SetCard(drawnCard);

            AnimateMove(cardObj.transform, slot, () =>
            {
                PlaceCard(cardObj.transform, slot);

                CheckDeckMatch(cardObj.transform, drawnCard, player);
            });
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
            CardCaptureManager currentCaptureManager =
            player == mainPlayer ? playerCaptureManager : enemyCaptureManager;

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
                    currentCaptureManager.CaptureCard(drawnCardTransform, drawnCardData, player);
                    currentCaptureManager.CaptureCard(tableCard, tableData, player);

                    player.CheckForYaku();
                    return;
                }
            }
        }
    }
}