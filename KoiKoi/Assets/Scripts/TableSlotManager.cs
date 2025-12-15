using System.Collections.Generic;
using UnityEngine;

namespace KoiKoiProject
{
    public class TableSlotManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> tableSlots;

        // Возвращает ближайший слот к позиции, если есть в радиусе maxDistance, иначе null
        public Transform GetClosestSlot(Vector3 position, float maxDistance = 2.0f)
        {
            Transform closestSlot = null;
            float minDistance = maxDistance;

            foreach (Transform slot in tableSlots)
            {
                float dist = Vector3.Distance(position, slot.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestSlot = slot;
                }
            }
            return closestSlot;
        }
    }
}
