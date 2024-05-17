using IJunior.CompositeRoot;
using UnityEngine;
using UnityEngine.UI;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicPlayer : Script, IActivatable
    {
        private AudioSource _audioSource;
        private Slider _volumeSlider;

        public void Initialize()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnActivate()
        {
            if (_volumeSlider == null)
                return;

            _volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        public void OnDeactivate()
        {
            if (_volumeSlider == null)
                return;

            _volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }

        public void UpdateVolumeSlider(Slider volumeSlider = null)
        {
            _volumeSlider = volumeSlider;
        }

        public void SetVolumeFromSlider()
        {
            ChangeVolume(_volumeSlider.value);
        }

        public void SetVolumeToSlider()
        {
            _volumeSlider.value = _audioSource.volume;
        }

        public void Pause() => _audioSource.Pause();

        public void UnPause() => _audioSource.UnPause();

        private void ChangeVolume(float value) => _audioSource.volume = value;
    }
}