using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Runtime.LevelEditor
{
    public class UIValueController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_InputField inputField;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            slider?.onValueChanged.AddListener(OnSliderValueChanged);
            inputField?.onEndEdit.AddListener(OnInputFieldValueChanged);
        }

        private void Start()
        {
            SetValues();
        }

        private void SetValues()
        {
            slider.value = slider.minValue;
            inputField.text = slider.value.ToString(CultureInfo.CurrentCulture);
        }
        
        private void OnSliderValueChanged(float value)
        {
            inputField.text = value.ToString(CultureInfo.CurrentCulture);
        }
        
        private void OnInputFieldValueChanged(string value)
        {
            if (!float.TryParse(value, out var result)) return;
            
            if (result < slider.minValue || result > slider.maxValue)
            {
                var text = Instantiate(inputField.GetComponentInChildren<TextMeshProUGUI>(), inputField.transform);
                text.transform.position = 
                    new Vector3(inputField.transform.position.x, inputField.transform.position.y, inputField.transform.position.z  + 1);
                text.enabled = true;
                text.text = $"Value must be between {slider.minValue} and {slider.maxValue}, for now";
                text.DOFade(0, 6f).OnComplete(() => Destroy(text.gameObject));
            }
            
            result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
            
            inputField.text = result.ToString(CultureInfo.CurrentCulture);
            slider.value = result;
        }
    }
}