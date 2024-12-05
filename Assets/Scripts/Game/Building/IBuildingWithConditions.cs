using UnityEngine;

namespace Game.Building
{
    public interface IBuildingWithConditions
    {
        public bool CanBePlacedAt(Vector3 position);
    }
}