using UnityEngine;

public class GridOnTable : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform gridParent;  // Твоя сетка
    public GameObject table;      // Стол с BoxCollider

    void Start()
    {
        if (gridParent == null || table == null)
        {
            Debug.LogError("GridParent или Table не назначены!");
            return;
        }

        // Получаем BoxCollider стола
        BoxCollider tableCollider = table.GetComponent<BoxCollider>();
        if (tableCollider == null)
        {
            Debug.LogError("У стола нет BoxCollider!");
            return;
        }

        // Вычисляем верхнюю поверхность стола
        float topY = tableCollider.bounds.max.y;

        // Позиция центра стола
        Vector3 tableCenter = tableCollider.bounds.center;

        // Ставим сетку на стол и центрируем по x и z
        Vector3 newGridPos = new Vector3(tableCenter.x, topY, tableCenter.z);
        gridParent.position = newGridPos;

        Debug.Log("Сетка установлена на поверхность стола!");
    }
}
