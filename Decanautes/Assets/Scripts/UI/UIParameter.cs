using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        Nothing,
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
        GlobalVolume,
    }

    public ParameterType UiParameterType;
    public PlayerPreferencesType PlayerPreferencetype;

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
        SetVisual();
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

    public void SetVisual()
    {
        switch (PlayerPreferencetype)
        {
            case PlayerPreferencesType.Nothing:
                break;
            case PlayerPreferencesType.CameraSensibility:
                SliderValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.CameraSensibility;
                Slider.value = SliderValue;
                ChangeSliderParameter(SliderValue);
                break;
            case PlayerPreferencesType.Forward:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Forward;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.Backward:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Forward;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.Left:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Left;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.Right:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Right;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.Interact:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Interact;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.Wright:
                KeyValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.Wright;
                ChangeKeyParameter(KeyValue);
                break;
            case PlayerPreferencesType.OutlineColor:
                ColorValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.OutlineColor;
                ChangeColorParameter(ColorValue);
                break;
            case PlayerPreferencesType.ContrastMode:
                BoolValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.ContrastMode;
                ChangeBoolParameter(BoolValue);
                break;
            case PlayerPreferencesType.QuadrichromicMode:
                BoolValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.QuadrichromicMode;
                ChangeBoolParameter(BoolValue);
                break;
            case PlayerPreferencesType.MusicVolume:
                SliderValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.MusicVolume;
                Slider.value = SliderValue;
                ChangeSliderParameter(SliderValue);
                break;
            case PlayerPreferencesType.AmbianceVolume:
                SliderValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.AmbianceVolume;
                Slider.value = SliderValue;
                ChangeSliderParameter(SliderValue);
                break;
            case PlayerPreferencesType.VFXVolume:
                SliderValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.VFXVolume;
                Slider.value = SliderValue;
                ChangeSliderParameter(SliderValue);
                break;
            case PlayerPreferencesType.GlobalVolume:
                SliderValue = PlayerPreferencesManager.Instance.PlayerPreferencesData.GlobalVolume;
                Slider.value = SliderValue;
                ChangeSliderParameter(SliderValue);
                break;
        }
    }

    #region bool
    private void ChangeBoolParameter(bool value)
    {
        BoolValue = value;
        BoolFalseButton.GetComponent<Image>().enabled = !BoolValue;
        BoolTrueButton.GetComponent<Image>().enabled = BoolValue;
        OnBoolValueCange?.Invoke(BoolValue);
        if (PlayerPreferencetype == PlayerPreferencesType.ContrastMode)
        {
            PlayerPreferencesManager.Instance.ChangeContrastMode(BoolValue);
        }
        if (PlayerPreferencetype == PlayerPreferencesType.QuadrichromicMode)
        {
            PlayerPreferencesManager.Instance.ChangeQuadrichromicMode(BoolValue);
        }
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
        if (PlayerPreferencetype == PlayerPreferencesType.OutlineColor)
        {
            PlayerPreferencesManager.Instance.ChangeOutlineColor(ColorValue);
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
        if (PlayerPreferencetype == PlayerPreferencesType.CameraSensibility)
            PlayerPreferencesManager.Instance.ChangeSensibility(SliderValue);
        if (PlayerPreferencetype == PlayerPreferencesType.VFXVolume)
            PlayerPreferencesManager.Instance.ChangeVfxVolume(SliderValue);
        if (PlayerPreferencetype == PlayerPreferencesType.AmbianceVolume)
            PlayerPreferencesManager.Instance.ChangeAmbianceVolume(SliderValue);
        if (PlayerPreferencetype == PlayerPreferencesType.MusicVolume)
            PlayerPreferencesManager.Instance.ChangeMusicVolume(SliderValue);
        if (PlayerPreferencetype == PlayerPreferencesType.GlobalVolume)
            PlayerPreferencesManager.Instance.ChangeGlobalVolume(SliderValue);

    }

    #endregion

}
