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
        if (IsDisplay)
        {
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
        SpawnPrefab();
    }

    public void SpawnPrefab()
    {
        _spawned = Instantiate(ToSpawnPrefab,transform);
        _spawned.name = ToSpawnPrefab.name + _spawned.GetInstanceID();
        _spawned.transform.position = transform.position;
        _spawned.transform.rotation = transform.rotation;
    }

    public void StartSpawnCoroutine()
    {
        _spawned = null;
        _IsDisplaying = false;
        StartCoroutine(SpawnNext());
    }
}
