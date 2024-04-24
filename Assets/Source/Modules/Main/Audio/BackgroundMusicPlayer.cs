using IJunior.CompositeRoot;
using UnityEngine.UI;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicPlayer : Script, IActivatable
    {
        [SerializeField] private static BackgroundMusicPlayer Instance = null;

        private static AudioSource AudioSource;

        private Slider _volumeSlider;

        public static BackgroundMusicPlayer CurrentInstance => Instance;

        public BackgroundMusicPlayer Initialize(Slider volumeSlider)
        {
            _volumeSlider = volumeSlider;

            if (Instance == null)
            {
                Instance = this;
                AudioSource = GetComponent<AudioSource>();

                ChangeVolume(_volumeSlider.value);
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Instance._volumeSlider = volumeSlider;
                _volumeSlider.value = AudioSource.volume;

                Destroy(gameObject);
            }

            return Instance;
        }

        public void OnActivate()
        {
            _volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        public void OnDeactivate()
        {
            _volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }

        public void Pause() => AudioSource.Pause();
        
        public void UnPause() => AudioSource.UnPause();
        
        private void ChangeVolume(float value) => AudioSource.volume = value;
    }
}