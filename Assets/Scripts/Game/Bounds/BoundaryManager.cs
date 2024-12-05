using UnityEngine;

namespace Game.Bounds
{
    public class BoundaryManager: MonoBehaviour
    {
        public static BoundaryManager Instance { get; set; }

        [SerializeField]
        private GameObject genericBoundaryExpansion;

        public CameraBoundaryCollider activeBoundary;

        public void Awake()
        {
            Instance = this;
        }

        public void ExpandBoundaries(Vector3 at)
        {
            var e = Instantiate(genericBoundaryExpansion, transform);
            e.transform.position = at;
        }
    }
}