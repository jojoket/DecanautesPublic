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
    public GameObject PostItPrefab;
    

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

        foreach (ObjectSave objectSave in MapData.SavedObjects)
        {
            if (Array.Find(toSaveObjects, x => x.name == objectSave.Name) == null)
            {
                MapData.SavedObjects.Remove(objectSave);
            }
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
                    found.GetComponent<PostIt>().Text.text = obj.PostItText;
            }
            else
            {
                GameObject objSpn = Instantiate(PrefabUtility.LoadPrefabContents(obj.PrefabPath), savedFile.transform);
                if (obj.PostItText != null && obj.PostItText != "")
                {
                    objSpn.GetComponent<PostIt>().Text.text = obj.PostItText;
                }
                objSpn.name = obj.Name;
                objSpn.transform.position = obj.Position;
                objSpn.transform.rotation = obj.Rotation;
                objSpn.transform.localScale = obj.Scale;
            }
        }


    }

}
