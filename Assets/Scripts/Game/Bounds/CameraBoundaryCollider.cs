using System;
using System.Collections;
using DG.Tweening;
using Game.Controllers;
using UnityEngine;

namespace Game.Bounds
{
    public class CameraBoundaryCollider: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Camera"))
                return;
            BoundaryManager.Instance.activeBoundary = this;
        }

        private void OnTriggerStay(Collider other)
        {
            BoundaryManager.Instance.activeBoundary = this;
        }

        private IEnumerator OnTriggerExit(Collider other)
        {
            // we might be entering another boundary
            if (!other.CompareTag("Camera"))
                yield break;
            // wait a bit of time
            yield return new WaitForSecondsRealtime(0.2f);
            
            if (BoundaryManager.Instance.activeBoundary == this)
            {
                BoundaryManager.Instance.activeBoundary = null;
                var direction = (transform.position - other.transform.position).normalized * 30;
                var point = GetComponent<Collider>().ClosestPointOnBounds(other.transform.position) + direction; 
                while (BoundaryManager.Instance.activeBoundary != this)
                {
                    TownCameraController.Instance.ShouldClampTo(point);
                    yield return null;
                }
            }
        }
    }
}