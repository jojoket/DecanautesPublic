using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.ComponentModel;
using Decanautes.Interactable;

public class Spawner : MonoBehaviour
{
    [TitleGroup("Components")]
    public GameObject ToSpawnPrefab;

    [TitleGroup("Parameters")]
    public bool IsDisplay;
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private bool _IsDisplaying = false;
    public float SpawnSpan;
    public bool IsPostItCarry = false;

    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private GameObject _spawned = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleEnabled = true;
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
        _spawned.GetComponent<Grabbable>().Spawner = this;
        if (_spawned.TryGetComponent<PostIt>(out PostIt postIt))
        {
            postIt.SetCycleText();
            _spawned.GetComponent<Grabbable>().InteractionStart();
            GameObject.FindFirstObjectByType<PlayerController>().grabbed = _spawned.GetComponent<Grabbable>();
        }
    }

    public void StartSpawnCoroutine()
    {
        _spawned = null;
        _IsDisplaying = false;
        StartCoroutine(SpawnNext());
    }
}
