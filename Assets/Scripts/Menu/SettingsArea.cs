using System;
using System.Collections.Generic;
using System.Linq;
using External.Data;
using Game.Building;
using Game.Player;
using Game.Production.POI;
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
            soundVolume.value = Preferences.Instance.Prefs.SoundVolume;
            musicVolume.value = Preferences.Instance.Prefs.MusicVolume;
            playerName.text = Preferences.Instance.Prefs.LastPlayerName;
            Debug.Log($"SETTING NEW VOLUME {Preferences.Instance.Prefs.SoundVolume}");
            soundMixer.SetFloat("SoundVolume", Mathf.Log10(Preferences.Instance.Prefs.SoundVolume) * 20f);
            soundMixer.SetFloat("MusicVolume", Mathf.Log10(Preferences.Instance.Prefs.MusicVolume) * 20f);
            
            Preferences.Instance.Save();
            
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
                Preferences.Instance.Prefs.SoundVolume = v;
                soundMixer.SetFloat("SoundVolume", Mathf.Log10(v) * 20f);
            });
            musicVolume.onValueChanged.AddListener(v =>
            {
                Preferences.Instance.Prefs.MusicVolume = v;
                soundMixer.SetFloat("MusicVolume", Mathf.Log10(v) * 20f);
            });
            playerName.onValueChanged.AddListener(v =>
            {
                var filtered = v.Replace(" ", "_").Replace("\"", "").Replace("'", "").Replace("\\", "").Replace("(", " ").Replace(")", " ");
                Preferences.Instance.Prefs.LastPlayerName = filtered;
                PlayerDataManager.Instance.playerName = filtered;
            });
        }

        private void OnDisable()
        {
            Preferences.Instance.Save();
        }
    }
}