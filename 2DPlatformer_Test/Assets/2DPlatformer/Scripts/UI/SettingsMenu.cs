namespace GSGD2.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.UI;
    using TMPro;

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] AudioMixer _audioMixer;
        [SerializeField] TMP_Dropdown _resolutionDropdown;
        [SerializeField] TMP_Dropdown _qualityDropdown;
        [SerializeField] TMP_Dropdown _textureDropdown;
        [SerializeField] TMP_Dropdown _aaDropdown;
        [SerializeField] Slider _volumeSlider;
        private float _currentVolume = 0.5f;
        private Resolution[] _resolutions;

        private void Start()
        {
            _resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            _resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.RefreshShownValue();
            LoadSettings(currentResolutionIndex);
        }

        public void SetVolume(float volume)
        {
            _audioMixer.SetFloat("Volume", volume);
            _currentVolume = volume;
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width,
                      resolution.height, Screen.fullScreen);
        }

        public void SetTextureQuality(int textureIndex)
        {
            QualitySettings.masterTextureLimit = textureIndex;
            _qualityDropdown.value = 6;
        }

        public void SetAntiAliasing(int aaIndex)
        {
            QualitySettings.antiAliasing = aaIndex;
            _qualityDropdown.value = 6;
        }

        public void SetQuality(int qualityIndex)
        {
            if (qualityIndex != 6) // if the user is not using 
                                   //any of the presets
                QualitySettings.SetQualityLevel(qualityIndex);
            switch (qualityIndex)
            {
                case 0: // quality level - very low
                    _textureDropdown.value = 3;
                    _aaDropdown.value = 0;
                    break;
                case 1: // quality level - low
                    _textureDropdown.value = 2;
                    _aaDropdown.value = 0;
                    break;
                case 2: // quality level - medium
                    _textureDropdown.value = 1;
                    _aaDropdown.value = 0;
                    break;
                case 3: // quality level - high
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 0;
                    break;
                case 4: // quality level - very high
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 1;
                    break;
                case 5: // quality level - ultra
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 2;
                    break;
            }
            _qualityDropdown.value = qualityIndex;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QualitySettingPreference",
                       _qualityDropdown.value);
            PlayerPrefs.SetInt("ResolutionPreference",
                       _resolutionDropdown.value);
            PlayerPrefs.SetInt("TextureQualityPreference",
                       _textureDropdown.value);
            PlayerPrefs.SetInt("AntiAliasingPreference",
                       _aaDropdown.value);
            PlayerPrefs.SetInt("FullscreenPreference",
                       Convert.ToInt32(Screen.fullScreen));
            PlayerPrefs.SetFloat("VolumePreference",
                       _currentVolume);
        }

        public void LoadSettings(int currentResolutionIndex)
        {
            if (PlayerPrefs.HasKey("QualitySettingPreference"))
                _qualityDropdown.value =
                             PlayerPrefs.GetInt("QualitySettingPreference");
            else
                _qualityDropdown.value = 3;
            if (PlayerPrefs.HasKey("ResolutionPreference"))
                _resolutionDropdown.value =
                             PlayerPrefs.GetInt("ResolutionPreference");
            else
                _resolutionDropdown.value = currentResolutionIndex;
            if (PlayerPrefs.HasKey("TextureQualityPreference"))
                _textureDropdown.value =
                             PlayerPrefs.GetInt("TextureQualityPreference");
            else
                _textureDropdown.value = 0;
            if (PlayerPrefs.HasKey("AntiAliasingPreference"))
                _aaDropdown.value =
                             PlayerPrefs.GetInt("AntiAliasingPreference");
            else
                _aaDropdown.value = 1;
            if (PlayerPrefs.HasKey("FullscreenPreference"))
                Screen.fullScreen =
                Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            else
                Screen.fullScreen = true;
            if (PlayerPrefs.HasKey("VolumePreference"))
                _volumeSlider.value =
                            PlayerPrefs.GetFloat("VolumePreference");
            else
                _volumeSlider.value =
                            PlayerPrefs.GetFloat("VolumePreference");
        }

    }
}