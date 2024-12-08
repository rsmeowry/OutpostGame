using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace External.Util
{
    [RequireComponent(typeof(Slider))]
    public class UISliderStep : MonoBehaviour
    {
        [FormerlySerializedAs("StepSize")]
        [Tooltip("The desired difference between neighbouring values of the Slider component.")]
        [Min(0.0001f)]
        public float stepSize = 0.0001f;

        private Slider _slider;

        void Start()
        {
            _slider = GetComponent<Slider>();
            if (_slider != null)
            {
                _slider.onValueChanged.AddListener(ClampSliderValue);
            }
        }

        /// <summary>
        /// Calculates the nearest stepped value and updates the Slider component.
        /// </summary>
        /// <param name="value">Current slider value</param>
        public void ClampSliderValue(float value)
        {
            if (_slider != null && stepSize > 0)
            {
                float steppedValue = Mathf.Round(value / stepSize) * stepSize;
                if (!Mathf.Approximately(steppedValue, value))
                {
                    _slider.value = steppedValue;
                }
            }
        }
    }
}