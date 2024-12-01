using System;
using Game.Controllers;
using UnityEngine;

namespace Game.Sound
{
    public class AmbientArea: MonoBehaviour
    {
        [SerializeField]
        private AudioClip clip;
        private BoxCollider _box;

        private void Start()
        {
            _box = GetComponent<BoxCollider>();
            var c = _box.center;
            c.y = 100;
            _box.center = c;
            var s = _box.size;
            s.y = 200;
            _box.size = s;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.tag);
            if (!other.CompareTag("Camera"))
                return;
            AmbientFadingChannel.Instance.QueueCrossfade(clip);
        }
    }
}