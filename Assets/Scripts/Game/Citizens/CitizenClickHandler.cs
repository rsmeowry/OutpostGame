using Game.Controllers;
using Game.POI;
using Game.Production.POI;
using UnityEngine;

namespace Game.Citizens
{
    public class CitizenClickHandler: MonoBehaviour, ICameraClickable
    {
        [SerializeField]
        private ResourceContainingPOI target;
        public void OnClick()
        {
        }
    }
}