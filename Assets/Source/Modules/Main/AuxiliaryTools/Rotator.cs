using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class Rotator : Script, IRootUpdateble
    {
        [SerializeField] private Vector3 _velocity;

        private Transform _transform;

        public void Initialize()
        {
            _transform = transform;
        }

        public void RootUpdate()
        {
            _transform.Rotate(_velocity * Time.deltaTime);
        }
    }
}