using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(AudioSource))]
    public class ArrowBlockEffector : Script
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private float _minTrailDistance;
        [SerializeField] private AudioClip _activationSound;

        private Transform _transform;
        private AudioSource _audioSource;

        public void Initialize(Transform transform)
        {
            _transform = transform;
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayActivationSound()
        {
            _audioSource.PlayOneShot(_activationSound);
        }

        public void TryActivateTrail(Vector3 targetPosition)
        {
            if (Vector3.Distance(_transform.position, targetPosition) >= _minTrailDistance)
                _trail.enabled = true;
        }

        public void DeactivateTrail() => _trail.enabled = false;

        public void ClearTrail() => _trail.Clear();
    }
}