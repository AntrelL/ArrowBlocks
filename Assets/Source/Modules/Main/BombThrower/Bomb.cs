using System;
using System.Collections;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class Bomb : Script
    {
        [SerializeField] private float _explosionCheckRadius;
        [SerializeField] private float _maxDelayForExplosionEffect;
        [SerializeField] private ParticleSystem _explosionEffect;
        [SerializeField] private AudioClip _explosionSound;

        private Rigidbody _rigidbody;
        private Coroutine _autoDestroyer;
        private Transform _transform;
        private Coroutine _exploder;
        private MeshRenderer _meshRenderer;
        private AudioSource _audioSource;

        private WaitForSeconds _maxDelayForExplosionEffectObject;
        private WaitForSeconds _autoDestructionDelayObject;

        private bool _isExploded = false;

        public event Action Destroyed;

        private void OnCollisionEnter(Collision collision)
        {
            if (_isExploded)
                return;

            _exploder = StartCoroutine(Explode());
        }

        public void Initialize(float autoDestructionDelay)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _audioSource = GetComponent<AudioSource>();

            _maxDelayForExplosionEffectObject = new WaitForSeconds(_maxDelayForExplosionEffect);
            _autoDestructionDelayObject = new WaitForSeconds(autoDestructionDelay);

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

            _isExploded = false;
        }

        public void Throw(
            Vector3 startPosition,
            Vector3 startVelocity,
            Vector3 angularVelocity)
        {
            _transform.position = startPosition;
            _rigidbody.velocity = startVelocity;
            _rigidbody.angularVelocity = angularVelocity;

            _transform.rotation = Quaternion.LookRotation(startVelocity.normalized);

            _autoDestroyer = StartCoroutine(AutoDestroy());
        }

        private IEnumerator AutoDestroy()
        {
            yield return _autoDestructionDelayObject;
            Destroy();
        }

        private IEnumerator Explode()
        {
            _isExploded = true;
            _meshRenderer.enabled = false;

            Collider[] colliders = Physics.OverlapSphere(_transform.position, _explosionCheckRadius);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ArrowBlock arrowBlock))
                    arrowBlock.FastRelease();
            }

            _explosionEffect.Play();
            _audioSource.PlayOneShot(_explosionSound);

            yield return _maxDelayForExplosionEffectObject;

            Destroy();
        }

        private void Destroy()
        {
            ResetValues();
            Destroyed?.Invoke();
        }
    }
}