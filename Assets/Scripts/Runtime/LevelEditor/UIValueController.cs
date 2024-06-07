﻿using System;
using System.Globalization;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Runtime.Extentions;

namespace Runtime.LevelEditor
{
    public enum ValueType
    {
        LevelID,
        GridWidth,
        GridLength,
        Time
    }
    
    public class UIValueController : MonoBehaviour
    {
        [SerializeField] private ValueType valueType;
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_InputField inputField;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (slider != null) slider?.onValueChanged.AddListener(OnSliderValueChanged);
            if (inputField != null) inputField?.onEndEdit.AddListener(OnInputFieldValueChanged);
        }

        private void OnEnable()
        {
            LevelEditorManager.OnValueChanged += SetValues;
        }

        private void OnDisable()
        {
            LevelEditorManager.OnValueChanged -= SetValues;
        }

        private void SetValues(int id)
        {
            float value = 0;
            switch (valueType)
            {
                case ValueType.LevelID:
                    value = LevelEditorManager.Instance.levelID;
                    break;
                case ValueType.GridWidth:
                    value = LevelEditorManager.Instance.gridWidth;
                    break;
                case ValueType.GridLength:
                    value = LevelEditorManager.Instance.gridLength;
                    break;
                case ValueType.Time:
                    value = LevelEditorManager.Instance.time;
                    break;
                default:
                    break;
            }
            
            if (slider != null) slider.value = value;
            if (inputField != null) inputField.text = value.ToString(CultureInfo.CurrentCulture);
        }
        
        private void OnSliderValueChanged(float value)
        {
            if (inputField != null) inputField.text = value.ToString(CultureInfo.CurrentCulture);
            
            SwitchFunction(value);
        }
        
        private void OnInputFieldValueChanged(string value)
        {
            if (!float.TryParse(value, out var result)) return;

            if (slider != null)
            {
                if (result < slider.minValue || result > slider.maxValue)
                {
                    inputField.GetComponentInChildren<TextMeshProUGUI>().GetComponent<TextMeshProUGUI>()
                        .CreateAndFadeOut(inputField.transform,$"Value must be between {slider.minValue} and {slider.maxValue}, for now", 5f);
                }
                result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
            }
            
            if (inputField != null) inputField.text = result.ToString(CultureInfo.CurrentCulture);
            if (slider != null) slider.value = result;
            
            SwitchFunction(result);
        }
        
        private void SwitchFunction(float value)
        {
            switch (valueType)
            {
                case ValueType.LevelID:
                    LevelEditorManager.Instance.levelID = (int)value;
                    break;
                case ValueType.GridWidth:
                    LevelEditorManager.Instance.gridWidth = (byte)value;
                    LevelEditorManager.Instance.CreateGrid((byte)value, LevelEditorManager.Instance.gridLength
                        ,LevelOrganizer.OnGetLevelEditorData.Invoke(LevelEditorManager.Instance.levelID));
                    break;
                case ValueType.GridLength:
                    LevelEditorManager.Instance.gridLength = (byte)value;
                    LevelEditorManager.Instance.CreateGrid(LevelEditorManager.Instance.gridWidth, (byte)value
                        ,LevelOrganizer.OnGetLevelEditorData.Invoke(LevelEditorManager.Instance.levelID));
                    break;
                case ValueType.Time:
                    LevelEditorManager.Instance.time = value;
                    break;
                default:
                    break;
            }
        }
    }
}