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
                        Debug.Log("Карта положена в слот: " + slot.name);

                        // Привязываем к слоту, сохраняя мировой масштаб
                        draggedCard.SetParent(slot, worldPositionStays: true);

                        // Центрируем карту и поворачиваем
                        draggedCard.position = slot.position + Vector3.up * 0.05f;

                        draggedCard.rotation = Quaternion.Euler(0, 180, 0);

                        // Устанавливаем фиксированный масштаб 0.22
                        Vector3 desiredScale = Vector3.one * 2.2f;
                        Vector3 parentScale = slot.lossyScale; // реальный мировой масштаб родителя
                        draggedCard.localScale = new Vector3(
                            desiredScale.x / parentScale.x,
                            desiredScale.y / parentScale.y,
                            desiredScale.z / parentScale.z
                        );

                        // Убираем карту из руки
                        handController.RemoveCardFromHand(draggedCard.gameObject);

                        SendCardToPaper(draggedCard, droppedData);
                        SendCardToPaper(tableCard, tableData);
                        slot = null;
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
            Debug.Log("Две подходящие карты собраны");
            Transform root = GetPaperRoot(cardData);
            cardTransform.SetParent(root, false);
            cardTransform.localPosition = Vector3.up * 0.01f * (root.childCount - 1);
            cardTransform.localRotation = Quaternion.Euler(0, 180, 0);
        }




    }
}
