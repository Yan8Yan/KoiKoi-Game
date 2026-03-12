using UnityEngine;

namespace KoiKoiProject
{
    public class CardDragHandler : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private LayerMask placementLayerMask;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private TableSlotManager slotManager;
        [SerializeField] private HandController3D handController;
        [SerializeField] private PlayerController ownerPlayer;

        [SerializeField] private Transform plantsRoot;
        [SerializeField] private Transform ribbonsRoot;
        [SerializeField] private Transform animalsRoot;
        [SerializeField] private Transform brightsRoot;

        private Transform draggedCard = null;
        private Vector3 offset;

        private static readonly Quaternion HorizontalRotation =
           Quaternion.Euler(0f, 180f, 0f);

        private Transform originalParent;
        private Vector3 originalLocalPos;
        private Quaternion originalLocalRot;
        private Vector3 originalLocalScale;
        private int originalSiblingIndex;

        private void Update()
        {
            HandlePickUp();
            HandleDrag();
            HandleDrop();
        }

        // Берём карту, на которую навели курсор
        private void HandlePickUp()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    if (hit.transform.CompareTag("Drag"))
                    {
                        draggedCard = hit.transform;
                        originalParent = draggedCard.parent;
                        originalLocalPos = draggedCard.localPosition;
                        originalLocalRot = draggedCard.localRotation;
                        originalLocalScale = draggedCard.localScale;
                        originalSiblingIndex = draggedCard.GetSiblingIndex();

                        // смещение, чтобы карта не прыгала в центр луча
                        offset = draggedCard.position - hit.point;
                    }
                }
            }
        }

        // Если карту держим — двигаем
        private void HandleDrag()
        {
            if (draggedCard != null)
            {
                Vector3 targetPos = inputManager.GetSelectedMapPosition();

                // Смещение карты над поверхностью
                Vector3 newPos = targetPos + offset + Vector3.up;

                // Ограничиваем минимальную высоту
                float minY = 9.0f; // например, высота стола
                newPos.y = Mathf.Max(newPos.y, minY);

                draggedCard.position = newPos;
                draggedCard.rotation = HorizontalRotation;
            }
        }

        private void ReturnCard()
        {
            // Возвращаем в исходного родителя
            draggedCard.SetParent(originalParent, worldPositionStays: false);

            // Возвращаем локальные трансформы как было
            draggedCard.localPosition = originalLocalPos;
            draggedCard.localRotation = originalLocalRot;
            draggedCard.localScale = originalLocalScale;

            // Возвращаем порядок в иерархии (чтобы веер не “перемешался”)
            draggedCard.SetSiblingIndex(originalSiblingIndex);

            // На всякий — пересобрать руку (если у тебя рука управляет раскладкой)
            handController.RefreshHand();
        }


        // Отпускаем карту
        private void HandleDrop()
        {
            if (draggedCard != null && Input.GetMouseButtonUp(0))
            {
                Transform slot = slotManager.GetClosestSlot(draggedCard.position);
                if (slot != null)
                {
                    if (slot.childCount == 0)
                    {
                        PlaceCardInSlot(draggedCard, slot);
                        DrawCardFromDeck(); //кладём карту в слот, если он пустой
                    }
                    else
                    {
                        var droppedDisplay = draggedCard.GetComponent<CardDisplay3D>();
                        Card droppedData = droppedDisplay.CardData();

                        Transform tableCard = slot.GetChild(0);
                        var tableDisplay = tableCard.GetComponent<CardDisplay3D>();
                        Card tableData = tableDisplay.CardData();

                        if (tableData.month != droppedData.month)
                        {
                            ReturnCard();
                            draggedCard = null;
                            return;
                        }
                        else
                        {
                            PlaceCardInSlot(draggedCard, slot);

                            SendCardToPaper(draggedCard, droppedData);
                            SendCardToPaper(tableCard, tableData);

                            ownerPlayer.CheckForYaku();
                            DrawCardFromDeck();
                        }
                    }
                }
                else
                {
                    Debug.Log("Слот не найден рядом с позицией карты");
                    ReturnCard();
                }

                draggedCard = null;
            }
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
        private void SendCardToPaper(Transform cardTransform, Card cardData)
        {
            Transform root = GetPaperRoot(cardData);

            cardTransform.SetParent(root, false);
            cardTransform.localPosition = Vector3.up * 0.01f * (root.childCount - 1);
            cardTransform.localRotation = Quaternion.Euler(0, 180, 0);

            ownerPlayer.AddCapturedCard(cardData);
        }

        private void PlaceCardInSlot(Transform card, Transform slot)
        {
            Debug.Log("Карта положена в слот: " + slot.name);

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

            if (draggedCard != null)
                handController.RemoveCardFromHand(card.gameObject);
        }



        private void CheckDeckMatch(Transform drawnCardTransform, Card drawnCardData)
        {
            Transform matchedCard = null;
            Card matchedData = null;

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
                    matchedCard = tableCard;
                    matchedData = tableData;
                    break;
                }
            }

            if (matchedCard != null)
            {
                Debug.Log("Совпадение с картой на столе!");

                SendCardToPaper(drawnCardTransform, drawnCardData);
                SendCardToPaper(matchedCard, matchedData);

                ownerPlayer.CheckForYaku();
            }
        }

        private void DrawCardFromDeck()
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

            PlaceCardInSlot(cardObj.transform, slot);

            CheckDeckMatch(cardObj.transform, drawnCard);
        }



    }
}
