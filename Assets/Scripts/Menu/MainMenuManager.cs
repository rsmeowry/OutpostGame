using System;
using System.Collections;
using System.Numerics;
using DG.Tweening;
using External.Data;
using External.Util;
using Game.Player;
using Tutorial.Impl;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace Menu
{
    public class MainMenuManager: MonoBehaviour
    {
        [SerializeField]
        private Image black;

        [SerializeField]
        private GameObject settingsPrefab;
        [SerializeField]
        private GameObject faqPrefab;

        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button settingsButton;
        [SerializeField]
        private Button faqButton;
        [SerializeField]
        private Button quitButton;

        [SerializeField]
        private Transform dataContainer;

        [SerializeField]
        private GameObject[] bearPrefabs;

        [SerializeField]
        private RuntimeAnimatorController animator;

        private AudioSource _as;

        [SerializeField]
        private Transform bearContainer;

        [SerializeField]
        private AudioMixer soundMixer;

        private void Awake()
        {
            black.enabled = true;
        }

        private void Start()
        {
            black.DOFade(0f, 2f).Play();
            _as = GetComponent<AudioSource>();
            _as.volume = 0f;
            _as.DOFade(0.3f, 2f).Play();
            
            playButton.onClick.AddListener(() =>
            {
                StartCoroutine(ChangeScene());
            });
            quitButton.onClick.AddListener(Application.Quit);
            faqButton.onClick.AddListener(() => SwitchTab(faqPrefab));
            settingsButton.onClick.AddListener(() => SwitchTab(settingsPrefab));

            var bear = Instantiate(Rng.Choice(bearPrefabs), bearContainer);
            bear.AddComponent<Animator>().runtimeAnimatorController = animator;
            
            soundMixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Max(Preferences.Instance.Prefs.SoundVolume, 0.0001f)) * 20f);
            soundMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(Preferences.Instance.Prefs.MusicVolume, 0.0001f)) * 20f);
            Screen.SetResolution(Preferences.Instance.Prefs.ResWidth, Preferences.Instance.Prefs.ResHeight, Screen.fullScreenMode);

            PlayerDataManager.Instance.playerName = Preferences.Instance.Prefs.LastPlayerName;
        }
        
        private IEnumerator ChangeScene()
        {
            if (_busySwitching)
                yield break;
            _busySwitching = true;

            _as.DOFade(0f, 1f).Play();
            yield return black.DOFade(1f, 1f).Play().WaitForCompletion();
            SceneManager.LoadScene(1);
        }

        private GameObject _activePrefab;
        private GameObject _currentTab;
        private bool _busySwitching;
        private void SwitchTab(GameObject newTab)
        {
            if (_busySwitching)
                return;
            _busySwitching = true;
            StartCoroutine(DoSwitch(newTab));
        }

        private IEnumerator DoSwitch(GameObject newTab)
        {
            if (_currentTab != null)
                yield return _currentTab.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(_currentTab)).Play().WaitForCompletion();
            if (_activePrefab == newTab)
            {
                _currentTab = null;
                _activePrefab = null;
                _busySwitching = false;
                yield break;
            }

            _activePrefab = newTab;

            _currentTab = Instantiate(newTab, dataContainer);
            _currentTab.transform.localScale = Vector3.zero;
            yield return _currentTab.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).Play()
                .WaitForCompletion();
            _busySwitching = false;
        }
    }
}