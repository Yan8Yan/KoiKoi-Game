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
            private GameObject mouseIndicator; //It will show shich position we are selecting

            [SerializeField]
            private InputManager inputManager;

            private void Update()
            {
                Vector3 mousePosition = inputManager.GetSelectedMapPosition();
                mouseIndicator.transform.position = mousePosition;
            }

        }
    }
}