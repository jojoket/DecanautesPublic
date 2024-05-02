using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class MapManager : MonoBehaviour
{
    public MapData MapData;
    public GameObject savedFile;
    

    // Start is called before the first frame update
    void Start()
    {
        LoadMap();
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

        foreach (SavedObject obj in toSaveObjects)
        {
            if (obj.GetComponentInParent<Spawner>() != null)
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
            if (obj.TryGetComponent<Event>(out Event eventFound))
            {
                objectSave.EventInteractionsTofix = eventFound.InteractionsToFix.ToList();
                objectSave.EventInteractionsToBreak = eventFound.InteractionsToBreak.ToList();
            }
            int index = MapData.SavedObjects.FindIndex(x => x.Name == objectSave.Name);
            if (index != -1)
            {
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
                    found.GetComponent<PostIt>().LockPostIt();
                }
                if (found.TryGetComponent<Interactable>(out Interactable interactable))
                    interactable.isActivated = obj.IsActivated;
                if (found.TryGetComponent<Event>(out Event eventFound))
                {
                    eventFound.InteractionsToFix = obj.EventInteractionsTofix.ToList();
                    eventFound.InteractionsToBreak = obj.EventInteractionsToBreak.ToList();
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
