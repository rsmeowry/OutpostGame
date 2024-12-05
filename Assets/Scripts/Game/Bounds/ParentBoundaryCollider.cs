using System.Collections;
using Game.Controllers;
using Game.Controllers.States;
using UnityEngine;

namespace Game.Bounds
{
    public class ParentBoundaryCollider: MonoBehaviour
    {
        private bool _isInside = true;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Camera"))
                return;
            _isInside = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isInside)
                return;
            if(other.CompareTag("Camera"))
                _isInside = true;
        }

        private IEnumerator OnTriggerExit(Collider other)
        {
            Debug.Log("TRIGGER EXIT");
            // we might be entering another boundary
            if (!other.CompareTag("Camera"))
                yield break;
            Debug.Log("CAMERA EXIT");
            // wait a bit of time
            _isInside = false;
            yield return new WaitForSecondsRealtime(0.2f);
            var direction = (transform.position - other.transform.position).normalized * 30;
            var point = GetComponent<Collider>().ClosestPointOnBounds(other.transform.position) + direction; 
             while (!_isInside)
            {
                TownCameraController.Instance.ShouldClampTo(point);
                yield return null;
            }
        }
    }

}