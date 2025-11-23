using UnityEditor;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera; //so we can raycast and detect our position
    //Функция `Raycast` в Unity необходима для определения того,
    //находится ли какой-либо объект в луче, выпущенном из определенной точки в пространстве

    private Vector2 lastPosition; //so we can catch the last position detected

    [SerializeField]
    private LayerMask placementLayerMask;
    //Когда вы выпускаете луч, LayerMask определяет, какие объекты этот луч может "увидеть".

    public Vector3 GetSelectedMapPosition() //метод вычисляет точку на карте, по которой пользователь щелкнул мышью
    {
        Vector3 mousePos = Input.mousePosition; //unity возвращает позицию курсора в пикселях
        mousePos.z = sceneCamera.nearClipPlane; //это вроде не нужно но я оставлю на всякий случай. штука задает глубину

        Ray ray = sceneCamera.ScreenPointToRay(mousePos); //Теперь камера "выстреливает" луч через точку на экране, где находится курсор.

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayerMask)) //проверяет — пересекает ли луч объект в 3D - сценe
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }

}
