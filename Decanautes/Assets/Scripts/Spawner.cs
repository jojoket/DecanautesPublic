using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.ComponentModel;

public class Spawner : MonoBehaviour
{
    [TitleGroup("Components")]
    public GameObject ToSpawnPrefab;

    [TitleGroup("Parameters")]
    public bool IsDisplay;
    private bool _IsDisplaying = false;
    public float SpawnSpan;

    private GameObject _spawned = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleEnabled = true;
        Debug.Log("1");
        if (IsDisplay)
        {
            Debug.Log("2");
            SpawnPrefab();
            _IsDisplaying = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SpawnNext()
    {
        yield return new WaitForSeconds(SpawnSpan);
        Debug.Log("5");
        SpawnPrefab();
    }

    public void SpawnPrefab()
    {
        _spawned = Instantiate(ToSpawnPrefab,transform);
        Debug.Log("3");
        _spawned.name = ToSpawnPrefab.name + _spawned.GetInstanceID();
        _spawned.transform.position = transform.position;
        _spawned.transform.rotation = transform.rotation;
    }

    public void StartSpawnCoroutine()
    {
        _spawned = null;
        _IsDisplaying = false;
        Debug.Log("4");
        StartCoroutine(SpawnNext());
    }
}
