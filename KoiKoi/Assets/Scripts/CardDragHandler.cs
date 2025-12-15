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

        private Transform draggedCard = null;
        private Vector3 offset;

        private static readonly Quaternion HorizontalRotation =
           Quaternion.Euler(0f, 180f, 0f);

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



        // Отпускаем карту
        private void HandleDrop()
        {
            if (draggedCard != null && Input.GetMouseButtonUp(0))
            {
                Transform slot = slotManager.GetClosestSlot(draggedCard.position);
                if (slot != null)
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
                }
                else
                {
                    Debug.Log("Слот не найден рядом с позицией карты");
                }

                draggedCard = null;
            }
        }



    }
}
