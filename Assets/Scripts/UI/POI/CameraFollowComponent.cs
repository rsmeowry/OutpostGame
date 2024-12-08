using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using Game.Controllers;
using Game.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace UI.POI
{
    public class CameraFollowComponent : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField]
        private Button screenshotButton;
        
        private Transform _cameraTf;

        private void Start()
        {
            _cameraTf = TownCameraController.Instance.cameraTransform;

            _targetRotation = _cameraTf.rotation.eulerAngles;
            slider.onValueChanged.AddListener(ValueChanged);
            screenshotButton.onClick.AddListener(() =>
            {
                StartCoroutine(CaptureScreenshot());
            });
        }

        private Vector3 _targetRotation;
        private void ValueChanged(float newVal)
        {
            var newAngle = newVal * 180f;
            var euler = _cameraTf.rotation.eulerAngles;
            _targetRotation = new Vector3(-15f, newAngle, 0f);
        }

        private void LateUpdate()
        {
            _cameraTf.rotation = Quaternion.Lerp(_cameraTf.rotation, Quaternion.Euler(_targetRotation),
                UnityEngine.Time.unscaledDeltaTime * 5f);
        }

        private bool _takingScreenshot;
        private IEnumerator CaptureScreenshot()
        {
            if (_takingScreenshot)
                yield break;
            _takingScreenshot = true;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("PriorityCanvas").GetComponent<Canvas>().enabled = false;

            yield return new WaitForEndOfFrame();

            var dt = DateTime.Now.ToString("s").Replace(":", "-");
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Join(documents, "Urbanaria");
            var file = Path.Join(dir, $"Screenshot-{dt}.png");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            var screenImage = new Texture2D(Screen.width, Screen.height);
            // get Image from screen
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            // convert to png
            var imageBytes = screenImage.EncodeToPNG();
            using var f = File.Create(file);
            f.Write(imageBytes, 0, imageBytes.Length);
            f.Flush();
            
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("PriorityCanvas").GetComponent<Canvas>().enabled = true;

            _takingScreenshot = false;
        }
    }
}