using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Decanautes.Interactable;

public class MapManager : MonoBehaviour
{
    static public MapManager Instance { get; private set; }

    public MapData MapData;
    public GameObject savedFile;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("two or more map managers are present");
            return;
        }
        Instance = this;
    }

    void OnEnable() {
        LoadMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveMap();
    }


    public void SaveMap()
    {
        SavedObject[] toSaveObjects= GameObject.FindObjectsByType<SavedObject>(FindObjectsSortMode.None);
        List<ObjectSave> toDelete = new List<ObjectSave>();
        //Check for Deleted objects (to be removed)
        foreach (ObjectSave objectSave in MapData.SavedObjects)
        {
            if (Array.Find(toSaveObjects, x => x.name == objectSave.Name) == null)
            {
                toDelete.Add(objectSave);
            }
        }
        foreach (ObjectSave objectDeleted in toDelete)
        {
            MapData.SavedObjects.Remove(objectDeleted);
        }

        //Objects that are new from this cycle
        foreach (SavedObject obj in toSaveObjects)
        {
            if (obj.GetComponentInParent<Spawner>() != null || (obj.transform.parent && obj.transform.parent.name == "GrabPoint"))
            {
                continue;
            }
            ObjectSave objectSave = new ObjectSave();
            objectSave.Name = obj.name;
            objectSave.Position = obj.transform.position;
            objectSave.Rotation = obj.transform.rotation;
            objectSave.Scale = obj.transform.localScale;
            objectSave.PrefabPath = obj.originalPrefabPath;
            objectSave.PostItText = obj.TryGetComponent<PostIt>(out PostIt post) ? post.Text.text : "";
            objectSave.CycleNum = MapManager.Instance.MapData.CurrentCycle;
            if (obj.TryGetComponent<Event>(out Event eventFound))
            {
                foreach (Interactable item in eventFound.InteractionsToFix)
                {
                    objectSave.EventInteractionsTofix.Add(item.gameObject.name);
                }
                foreach (Interactable item in eventFound.InteractionsToBreak)
                {
                    objectSave.EventInteractionsToBreak.Add(item.gameObject.name);
                }
            }
            int index = MapData.SavedObjects.FindIndex(x => x.Name == objectSave.Name);
            if (index != -1)
            {
                objectSave.CycleNum = MapData.SavedObjects[index].CycleNum;
                MapData.SavedObjects[index] = objectSave;
                continue;
            }
            MapData.SavedObjects.Add(objectSave);
        }
    }

    public void LoadMap()
    {
        MapData.CurrentCycle++;
        SavedObject[] savedObjects = GameObject.FindObjectsByType<SavedObject>(FindObjectsSortMode.None);
        foreach (ObjectSave obj in MapData.SavedObjects)
        {
            SavedObject found = Array.Find(savedObjects, x => x.name == obj.Name);
            if (found != null)
            {
                found.transform.position = obj.Position;
                found.transform.rotation = obj.Rotation;
                found.transform.localScale = obj.Scale;
                if (obj.PostItText != null && obj.PostItText != "")
                {
                    found.GetComponent<PostIt>().Text.text = obj.PostItText;
                    found.GetComponent<PostIt>().CycleText.text = "Cycle : " + obj.CycleNum.ToString();
                    found.GetComponent<PostIt>().LockPostIt();
                }
                if (found.TryGetComponent<Interactable>(out Interactable interactable))
                    interactable.isActivated = obj.IsActivated;
                if (found.TryGetComponent<Event>(out Event eventFound))
                {
                    //Retrieve interactables for events
                    eventFound.InteractionsToFix.Clear();
                    foreach (string objName in obj.EventInteractionsTofix)
                    {
                        eventFound.InteractionsToFix.Add(GameObject.Find(objName).GetComponent<Interactable>());
                    }
                    eventFound.InteractionsToBreak.Clear();
                    foreach (string objName in obj.EventInteractionsToBreak)
                    {
                        eventFound.InteractionsToBreak.Add(GameObject.Find(objName).GetComponent<Interactable>());
                    }
                }
            }
            else
            {
                GameObject objSpn = Instantiate(transform.Find(obj.PrefabPath).gameObject, savedFile.transform);
                objSpn.SetActive(true);
                objSpn.name = obj.Name;
                objSpn.transform.position = obj.Position;
                objSpn.transform.rotation = obj.Rotation;
                objSpn.transform.localScale = obj.Scale;
                if (obj.PostItText != null && obj.PostItText != "")
                {
                    objSpn.GetComponent<PostIt>().Text.text = obj.PostItText;
                    objSpn.GetComponent<PostIt>().CycleText.text = "Cycle : " + obj.CycleNum.ToString();
                    objSpn.GetComponent<PostIt>().LockPostIt();
                }
                if (objSpn.TryGetComponent<Interactable>(out Interactable interactable))
                    interactable.isActivated = obj.IsActivated;
            }
        }


    }

    [Button("Reset Map Data")]
    public void ResetMapData()
    {
        MapData.CurrentCycle = 0;
        MapData.SavedObjects = new List<ObjectSave>();
    }

}
