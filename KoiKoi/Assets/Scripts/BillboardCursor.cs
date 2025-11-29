using UnityEngine;

public class BillboardCursor : MonoBehaviour
{
    public Camera targetCamera;
    public float size = 0.1f; // регулируй размер курсора здесь

    void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Поворачиваем лицом к камере
        transform.LookAt(
            transform.position + targetCamera.transform.rotation * Vector3.forward,
            targetCamera.transform.rotation * Vector3.up
        );

        // Масштаб фиксирован на экране
        float distance = Vector3.Distance(transform.position, targetCamera.transform.position);
        float scale = size * distance;
        transform.localScale = Vector3.one * scale;
    }
}
