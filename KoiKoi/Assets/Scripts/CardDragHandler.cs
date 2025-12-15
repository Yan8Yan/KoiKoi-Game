using UnityEngine;

namespace KoiKoiProject
{
    public class CardDragHandler : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private LayerMask placementLayerMask;
        [SerializeField] private InputManager inputManager;

        private Transform draggedCard = null;
        private Vector3 offset;

        private static readonly Quaternion HorizontalRotation =
           Quaternion.Euler(0f, 0f, 0f);

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

                draggedCard.position = targetPos + offset + Vector3.up * 0.1f;
                // 0.1f — чтобы карта слегка "поднималась" над поверхностью

                draggedCard.rotation = HorizontalRotation;
            }
        }

        // Отпускаем карту
        private void HandleDrop()
        {
            if (draggedCard != null && Input.GetMouseButtonUp(0))
            {
                draggedCard = null;
            }
        }
    }
}
