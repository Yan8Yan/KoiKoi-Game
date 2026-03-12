using UnityEngine;

namespace KoiKoiProject
{
    public class CardCaptureManager : MonoBehaviour
    {
        [SerializeField] private Transform plantsRoot;
        [SerializeField] private Transform ribbonsRoot;
        [SerializeField] private Transform animalsRoot;
        [SerializeField] private Transform brightsRoot;

        public void CaptureCard(Transform cardTransform, Card cardData, PlayerController player)
        {
            Transform root = GetPaperRoot(cardData);

            cardTransform.SetParent(root, false);
            cardTransform.localPosition = Vector3.up * 0.01f * (root.childCount - 1);
            cardTransform.localRotation = Quaternion.Euler(0, 180, 0);

            player.AddCapturedCard(cardData);
        }

        private Transform GetPaperRoot(Card card)
        {
            switch (card.cardType)
            {
                case Card.CardType.kasu: return plantsRoot;
                case Card.CardType.tanzaku: return ribbonsRoot;
                case Card.CardType.tane: return animalsRoot;
                case Card.CardType.hikari: return brightsRoot;
                default: return plantsRoot;
            }
        }
    }
}