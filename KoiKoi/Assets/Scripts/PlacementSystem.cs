using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace KoiKoiProject
{
    namespace Assets.Scripts
    {
        internal class PlacementSystem : MonoBehaviour
        {
            [SerializeField]
            private GameObject mouseIndicator, cellIndicator; //It will show shich position we are selecting

            [SerializeField]
            private InputManager inputManager;

            [SerializeField]
            private Grid grid;

            private void Update()
            {
                Vector3 mousePosition = inputManager.GetSelectedMapPosition();
                Vector3Int gridPosition = grid.WorldToCell(mousePosition);
                mouseIndicator.transform.position = mousePosition;
                cellIndicator.transform.position= grid.CellToWorld(gridPosition);
            }

        }
    }
}