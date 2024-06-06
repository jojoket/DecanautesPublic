using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIParameter : MonoBehaviour
{
    public enum ParameterType
    {
        Bool,
        Color,
        Key,
        Slider,
    }

    public enum PlayerPreferencesType
    {
        CameraSensibility,
        Forward,
        Backward,
        Left,
        Right,
        Interact,
        Wright,
        OutlineColor,
        ContrastMode,
        QuadrichromicMode,
        MusicVolume,
        AmbianceVolume,
        VFXVolume,
        UIVolume,
    }

    public ParameterType UiParameterType;

    [ShowIf("UiParameterType", ParameterType.Bool)]
    public bool BoolValue;
    [ShowIf("UiParameterType", ParameterType.Bool)]
    public Button BoolTrueButton;
    [ShowIf("UiParameterType", ParameterType.Bool)]
    public Button BoolFalseButton;
    [ShowIf("UiParameterType", ParameterType.Bool)]
    public UnityEvent<bool> OnBoolValueCange;

    [ShowIf("UiParameterType", ParameterType.Color)]
    public Color ColorValue;
    [ShowIf("UiParameterType", ParameterType.Color)]
    public List<Button> ColorButtons;
    [ShowIf("UiParameterType", ParameterType.Color)]
    public List<UnityEvent<Color>> OnColorValueCanges;

    [ShowIf("UiParameterType", ParameterType.Key)]
    public string KeyValue;
    [ShowIf("UiParameterType", ParameterType.Key)]
    public Button KeyButton;
    [ShowIf("UiParameterType", ParameterType.Key)]
    public UnityEvent<string> OnKeyValueCange;

    [ShowIf("UiParameterType", ParameterType.Slider)]
    public float SliderValue;
    [ShowIf("UiParameterType", ParameterType.Slider)]
    public Slider Slider;
    [ShowIf("UiParameterType", ParameterType.Slider)]
    public TMP_Text SliderText;
    [ShowIf("UiParameterType", ParameterType.Slider)]
    public UnityEvent<float> OnSliderValueChange;

    // Start is called before the first frame update
    void Start()
    {
        SetupInteraction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupInteraction()
    {
        switch (UiParameterType)
        {
            case ParameterType.Bool:
                {
                    BoolFalseButton.onClick.AddListener(() =>
                    {
                        ChangeBoolParameter(false);
                    });
                    BoolTrueButton.onClick.AddListener(() =>
                    {
                        ChangeBoolParameter(true);
                    });
                    break;
                }
            case ParameterType.Color:
                {
                    foreach (Button button in ColorButtons)
                    {
                        button.onClick.AddListener(() =>
                        {
                            ChangeColorParameter(button.transform.GetChild(0).GetComponent<Image>().color);
                        });
                    }
                    break;
                }
            case ParameterType.Key:
                {
                    break;
                }
            case ParameterType.Slider:
                {
                    Slider.onValueChanged.AddListener(ChangeSliderParameter);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    #region bool
    private void ChangeBoolParameter(bool value)
    {
        BoolValue = value;
        BoolFalseButton.GetComponent<Image>().enabled = !BoolValue;
        BoolTrueButton.GetComponent<Image>().enabled = BoolValue;
        OnBoolValueCange?.Invoke(BoolValue);
    }
    #endregion

    #region Color
    private void ChangeColorParameter(Color color)
    {
        ColorValue = color;
        for (int i = 0; i < ColorButtons.Count; i++)
        {
            Button colorButton = ColorButtons[i];
            if (colorButton.transform.GetChild(0).GetComponent<Image>().color == color)
            {
                colorButton.GetComponent<Image>().enabled = true;
                if (OnColorValueCanges.Count != 0 && OnColorValueCanges.Count > i)
                {
                    OnColorValueCanges[i]?.Invoke(ColorValue);
                }
                continue;
            }
            colorButton.GetComponent<Image>().enabled = false;
        }
    }

    #endregion

    #region Key

    private void ChangeKeyParameter(string key)
    {
        KeyValue = key;
        KeyButton.GetComponentInChildren<TMP_Text>().text = KeyValue;
        OnKeyValueCange?.Invoke(KeyValue);
    }

    #endregion

    #region Slider

    private void ChangeSliderParameter(float value)
    {
        SliderValue = value;
        SliderText.text = Math.Round(value, 2).ToString();
        OnSliderValueChange?.Invoke(SliderValue);
    }

    #endregion

}
