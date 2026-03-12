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

        [SerializeField] private DeckTurnResolver deckTurnResolver;
        [SerializeField] private CardCaptureManager captureManager;

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

                        offset = draggedCard.position - hit.point;
                    }
                }
            }
        }

        private void HandleDrag()
        {
            if (draggedCard == null)
                return;

            Vector3 targetPos = inputManager.GetSelectedMapPosition();

            Vector3 newPos = targetPos + offset + Vector3.up;

            float minY = 11.27f;
            newPos.y = Mathf.Max(newPos.y, minY);

            draggedCard.position = newPos;
            draggedCard.rotation = HorizontalRotation;
        }

        private void HandleDrop()
        {
            if (draggedCard == null || !Input.GetMouseButtonUp(0))
                return;

            Transform slot = slotManager.GetClosestSlot(draggedCard.position);

            if (slot == null)
            {
                Debug.Log("Слот не найден рядом с позицией карты");
                ReturnCard();
                draggedCard = null;
                return;
            }

            if (slot.childCount == 0)
            {
                PlaceCardInSlot(draggedCard, slot);

                deckTurnResolver.ResolveDeckDraw(ownerPlayer);
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

                PlaceCardInSlot(draggedCard, slot);

                captureManager.CaptureCard(draggedCard, droppedData, ownerPlayer);
                captureManager.CaptureCard(tableCard, tableData, ownerPlayer);

                ownerPlayer.CheckForYaku();

                deckTurnResolver.ResolveDeckDraw(ownerPlayer);
            }

            draggedCard = null;
        }

        private void ReturnCard()
        {
            draggedCard.SetParent(originalParent, false);

            draggedCard.localPosition = originalLocalPos;
            draggedCard.localRotation = originalLocalRot;
            draggedCard.localScale = originalLocalScale;

            draggedCard.SetSiblingIndex(originalSiblingIndex);

            handController.RefreshHand();
        }

        private void PlaceCardInSlot(Transform card, Transform slot)
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

            if (draggedCard != null)
                handController.RemoveCardFromHand(card.gameObject);
        }
    }
}