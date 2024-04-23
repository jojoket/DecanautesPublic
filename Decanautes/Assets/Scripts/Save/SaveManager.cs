using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SaveManager : MonoBehaviour
{
    [TitleGroup("ToSave")]
    public List<ScriptableObject> toSave = new List<ScriptableObject>();

    public string Path;

    // Start is called before the first frame update
    private void Awake()
    {
        ReadData();
    }

    private void OnDestroy()
    {
        SaveDatas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SaveDatas()
    {
        foreach (ScriptableObject data in toSave)
        {
            string savedJson = JsonUtility.ToJson(data);
            File.WriteAllText(Path + "/" + data.name + ".json", savedJson);
            //Debug.Log("SAVED : " +  data.name + " to : " + Path + "/" + data.name + ".json");
        }
    }

    public void ReadData()
    {
        for (int i = 0; i < toSave.Count; i++)
        {
            if (!File.Exists(Path + "/" + toSave[i].name + ".json"))
            {
                continue;
            }
            string readJson = File.ReadAllText(Path + "/" + toSave[i].name + ".json");
            JsonUtility.FromJsonOverwrite(readJson, toSave[i]);
            //Debug.Log("READ : " + toSave[i].name + " from : " + Path + "/" + toSave[i].name + ".json");
        }
    }

}
