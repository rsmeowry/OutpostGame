using System;
using Game.Bounds;
using UnityEngine;

namespace External.Util
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GPUInstanced: MonoBehaviour
    {
        private void Awake()
        {
            var propertyBlock = new MaterialPropertyBlock();
            var mRenderer = GetComponent<MeshRenderer>();
            mRenderer.SetPropertyBlock(propertyBlock);
            foreach(var child in GetComponentsInChildren<MeshRenderer>())
                child.SetPropertyBlock(new MaterialPropertyBlock());
        }
    }
}