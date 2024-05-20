using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public RythmManagerData rythmManagerData;

    //(une noire en français)
    private float quarterNoteRythmInMs;

    // Start is called before the first frame update
    void Start()
    {
        quarterNoteRythmInMs = 60000 / rythmManagerData.Bpm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator 
}
