using IJunior.CompositeRoot;
using System.Collections;
using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Bomb : Script
    {
        [SerializeField] private float _explosionCheckRadius;
        [SerializeField] private float _maxDelayForExplosionEffect;
        [SerializeField] private ParticleSystem _explosionEffect;

        private Rigidbody _rigidbody;
        private Coroutine _autoDestroyer;
        private Transform _transform;
        private Coroutine _exploder;
        private MeshRenderer _meshRenderer;

        public event Action Destroyed;

        private void OnCollisionEnter(Collision collision)
        {
            _exploder = StartCoroutine(Explode());
        }

        public void Initialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();

            _transform = transform;
        }

        public void ResetValues()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _transform.rotation = Quaternion.identity;

            gameObject.SetActive(false);
            _meshRenderer.enabled = true;

            if (_autoDestroyer != null)
                StopCoroutine(_autoDestroyer);

            if (_exploder != null)
                StopCoroutine(_exploder);

            if (_explosionEffect.isPlaying)
            {
                _explosionEffect.Stop();
                _explosionEffect.Clear();
            }
        }

        public void Throw(Vector3 startPosition, Vector3 startVelocity,
            Vector3 angularVelocity, float autoDestructionDelay)
        {
            _transform.position = startPosition;
            _rigidbody.velocity = startVelocity;
            _rigidbody.angularVelocity = angularVelocity;

            _transform.rotation = Quaternion.LookRotation(startVelocity.normalized);

            _autoDestroyer = StartCoroutine(AutoDestroy(autoDestructionDelay));
        }

        private IEnumerator AutoDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy();
        }

        private IEnumerator Explode()
        {
            _meshRenderer.enabled = false;

            Collider[] colliders = Physics.OverlapSphere(_transform.position, _explosionCheckRadius);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ArrowBlock arrowBlock))
                    arrowBlock.FastRelease();
            }

            _explosionEffect.Play();
            yield return new WaitForSeconds(_maxDelayForExplosionEffect);

            Destroy();
        }

        private void Destroy()
        {
            ResetValues();
            Destroyed?.Invoke();
        }
    }
}