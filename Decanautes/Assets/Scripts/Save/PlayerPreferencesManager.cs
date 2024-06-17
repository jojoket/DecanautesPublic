using Cinemachine;
using Decanautes.Interactable;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPreferencesManager : MonoBehaviour
{
    public static PlayerPreferencesManager Instance;
    public PlayerPreferences PlayerPreferencesData;

    public CinemachineVirtualCamera VirtualCamera;

    public Material Quadrichromic;

    private FMOD.Studio.VCA MusicVcaController;
    [ParamRef]
    public string MusicVcaName;
    private FMOD.Studio.VCA AmbianceVcaController;
    [ParamRef]
    public string AmbianceVcaName;
    private FMOD.Studio.VCA VFXVcaController;
    [ParamRef]
    public string VFXVcaName;
    private FMOD.Studio.VCA GlobalVcaController;
    [ParamRef]
    public string GlobalVcaName;



    void Awake()
    {
        Instance = this;
        MusicVcaController = FMODUnity.RuntimeManager.GetVCA("vca:/" + MusicVcaName);
        AmbianceVcaController = FMODUnity.RuntimeManager.GetVCA("vca:/" + AmbianceVcaName);
        VFXVcaController = FMODUnity.RuntimeManager.GetVCA("vca:/" + VFXVcaName);
        GlobalVcaController = FMODUnity.RuntimeManager.GetVCA("vca:/" + GlobalVcaName);
        ApplyPreferences();
    }

    private void Start()
    {
        
    }

    public void ApplyPreferences()
    {
        //Keys

        //Outline
        Interactable[] interactables = GameObject.FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        Color outlineColor = PlayerPreferencesData.OutlineColor;
        foreach (Interactable interactable in interactables)
        {
            interactable.HoverColor = outlineColor;
        }

        //Contrast
        //Quadrichromic
        Quadrichromic.SetFloat("_Opacity", PlayerPreferencesData.QuadrichromicMode ? 1 : 0);

        //Sounds
        MusicVcaController.setVolume(PlayerPreferencesData.MusicVolume/5);
        AmbianceVcaController.setVolume(PlayerPreferencesData.AmbianceVolume/5);
        VFXVcaController.setVolume(PlayerPreferencesData.VFXVolume/5);
        GlobalVcaController.setVolume(PlayerPreferencesData.GlobalVolume/5);
    }

    public void ChangeSensibility(float newSensibility)
    {
        PlayerPreferencesData.CameraSensibility = newSensibility;
        ApplyPreferences();
    }

    public void ChangeForward(string newForward)
    {
        PlayerPreferencesData.Forward = newForward;
        ApplyPreferences();
    }

    public void ChangeBackWard(string newBackward)
    {
        PlayerPreferencesData.Backward = newBackward;
        ApplyPreferences();
    }

    public void ChangeLeft(string newLeft)
    {
        PlayerPreferencesData.Left = newLeft;
        ApplyPreferences();
    }

    public void ChangeRight(string newRight)
    {
        PlayerPreferencesData.Right = newRight;
        ApplyPreferences();
    }

    public void ChangeInteract(string newInteract)
    {
        PlayerPreferencesData.Interact = newInteract;
        ApplyPreferences();
    }

    public void ChangeWright(string newWright)
    {
        PlayerPreferencesData.Wright = newWright;
        ApplyPreferences();
    }

    public void ChangeOutlineColor(Color color)
    {
        PlayerPreferencesData.OutlineColor = color;
        ApplyPreferences();
    }

    public void ChangeContrastMode(bool newContrastMode)
    {
        PlayerPreferencesData.ContrastMode = newContrastMode;
        ApplyPreferences();
    }

    public void ChangeQuadrichromicMode(bool newQuadrichromicMode)
    {
        PlayerPreferencesData.QuadrichromicMode = newQuadrichromicMode;
        ApplyPreferences();
    }

    public void ChangeMusicVolume(float newMusicVolume)
    {
        PlayerPreferencesData.MusicVolume = newMusicVolume;
        ApplyPreferences();
    }

    public void ChangeAmbianceVolume(float newAmbianceVolume)
    {
        PlayerPreferencesData.AmbianceVolume = newAmbianceVolume;
        ApplyPreferences();
    }

    public void ChangeVfxVolume(float newVFXVolume)
    {
        PlayerPreferencesData.VFXVolume = newVFXVolume;
        ApplyPreferences();
    }

    public void ChangeGlobalVolume(float newGlobalVolume)
    {
        PlayerPreferencesData.GlobalVolume = newGlobalVolume;
        ApplyPreferences();
    }


}
