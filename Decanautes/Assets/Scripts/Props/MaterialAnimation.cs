using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.Utilities;
using FMODUnity;
using FMOD;

public class MaterialAnimation : MonoBehaviour
{
    public List<MaterialChangement> materialChangements;




    // Start is called before the first frame update
    void Start()
    {
        foreach (var materialChangement in materialChangements)
        {
            materialChangement.SetMaterialInitialState();
            if(materialChangement.StartAnimOnStart){
                TriggerAnimation(materialChangement);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var materialChangement in materialChangements)
        {
            materialChangement.SetMaterialBackToInitialState();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    [Button]
    public void TriggerAllAnimations()
    {
        foreach (var materialChangement in materialChangements)
        {
            if (materialChangement.IsInRythm)
            {
                RythmManager.Instance.OnBeatTrigger.AddListener(() =>
                {
                    StartCoroutine(StartAnimation(materialChangement));
                });
                if (materialChangement.HasSound)
                {
                    foreach (AnimationSound sound in materialChangement.animationSounds)
                    {
                        if (!sound.DoLaunchSoundAfterAnimation)
                        {
                            RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
                        }
                    }
                }
                return;
            }
            StartCoroutine(StartAnimation(materialChangement));

        }
    }

    [Button]
    public void TriggerAnimationByIndex(int index)
    {
        if (materialChangements[index].IsInRythm)
        {
            RythmManager.Instance.OnBeatTrigger.AddListener(() =>
            {
                StartCoroutine(StartAnimation(materialChangements[index]));
            });
            if (materialChangements[index].HasSound)
            {
                foreach (AnimationSound sound in materialChangements[index].animationSounds)
                {
                    if (!sound.DoLaunchSoundAfterAnimation)
                    {
                        RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
                    }
                }
            }
            return;
        }
        StartCoroutine(StartAnimation(materialChangements[index]));

    }

    public void TriggerAnimation(MaterialChangement materialChangement)
    {
        if (materialChangement.IsInRythm)
        {
            RythmManager.Instance.OnBeatTrigger.AddListener(() =>
            {
                StartCoroutine(StartAnimation(materialChangement));
            });
            if (materialChangement.HasSound)
            {
                foreach (AnimationSound sound in materialChangement.animationSounds)
                {
                    if (!sound.DoLaunchSoundAfterAnimation)
                    {
                        RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
                    }
                }
            }
            return;
        }
        StartCoroutine(StartAnimation(materialChangement));

    }


    private IEnumerator StartAnimation(MaterialChangement materialChangement)
    {
        float startTime = Time.time;
        float endTime = Time.time + materialChangement.LerpDuration;

        if (materialChangement.IsMaterialInstance)
        {
            materialChangement.Material = materialChangement.Renderer.materials[materialChangement.MaterialIndex];
        }
        materialChangement.SetMaterialStartValues();
        float delta = 0;
        while (Time.time < endTime)
        {
            delta += Time.deltaTime / materialChangement.LerpDuration;
            materialChangement.ChangeMaterialFromDelta(delta);
            yield return 0;
        }
        if (materialChangement.HasSound)
        {
            foreach (AnimationSound sound in materialChangement.animationSounds)
            {
                if (sound.DoLaunchSoundAfterAnimation)
                {
                    RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
                }
            }
        }
    }


}
