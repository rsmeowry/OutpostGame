using System;
using System.Collections.Generic;
using System.Linq;
using External.Data;
using Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class SettingsArea: MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField playerName;
        [SerializeField]
        private Slider musicVolume;
        [SerializeField]
        private Slider soundVolume;
        [SerializeField]
        private TMP_Dropdown resolutionChoice;

        [SerializeField]
        private AudioMixer soundMixer;

        private Resolution[] _resolutions;

        private void Start()
        {
            soundVolume.value = Mathf.RoundToInt(Preferences.Instance.Prefs.SoundVolume * 10f);
            musicVolume.value = Mathf.RoundToInt(Preferences.Instance.Prefs.MusicVolume * 10f);
            playerName.text = Preferences.Instance.Prefs.LastPlayerName;
            soundMixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Max(Preferences.Instance.Prefs.SoundVolume, 0.0001f)) * 20f);
            soundMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(Preferences.Instance.Prefs.MusicVolume, 0.0001f)) * 20f);
            
            Screen.SetResolution(Preferences.Instance.Prefs.ResWidth, Preferences.Instance.Prefs.ResHeight, Screen.fullScreenMode);

            _resolutions = Screen.resolutions;
            var filteredRes = new List<Resolution>();

            for (var i = 0; i < _resolutions.Length; i++)
            {
                filteredRes.Add(_resolutions[i]);
            }

            var options = new List<string>();
            var current = Screen.currentResolution;
            var currentIdx = 0;
            for(var i = 0; i < filteredRes.Count; i++)
            {
                var res = filteredRes[i];
                options.Add($"{res.width}x{res.height}");
                if (res.Equals(current))
                    currentIdx = i;
            }
            
            resolutionChoice.AddOptions(options);
            resolutionChoice.value = currentIdx;
            resolutionChoice.RefreshShownValue();
            
            resolutionChoice.onValueChanged.AddListener(v =>
            {
                Screen.SetResolution(filteredRes[v].width, filteredRes[v].height, Screen.fullScreenMode, filteredRes[v].refreshRateRatio);
                Preferences.Instance.Prefs.ResWidth = filteredRes[v].width;
                Preferences.Instance.Prefs.ResHeight = filteredRes[v].height;
                Preferences.Instance.Save();
            });
            
            soundVolume.onValueChanged.AddListener(v =>
            {
                Preferences.Instance.Prefs.SoundVolume = v / 10f;
                soundMixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Max(v / 10f, 0.0001f)) * 20f);
            });
            musicVolume.onValueChanged.AddListener(v =>
            {
                Preferences.Instance.Prefs.MusicVolume = v / 10f;
                soundMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(v / 10f, 0.0001f)) * 20f);
            });
            playerName.onValueChanged.AddListener(v =>
            {
                Preferences.Instance.Prefs.LastPlayerName = v;
                PlayerDataManager.Instance.playerName = v;
            });
        }

        private void OnDisable()
        {
            Preferences.Instance.Save();
        }
    }
}