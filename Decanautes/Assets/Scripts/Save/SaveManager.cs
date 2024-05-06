using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SaveManager : MonoBehaviour
{
    [TitleGroup("ToSave")]
    public List<ScriptableObject> toSave = new List<ScriptableObject>();

    public string EditorPath;
    public string BuildPath;

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


    [Button("Save datas")]
    public void SaveDatas()
    {
        foreach (ScriptableObject data in toSave)
        {
            string path = Application.isEditor ? Application.persistentDataPath + "/" + EditorPath : BuildPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string savedJson = JsonUtility.ToJson(data);
            File.WriteAllText(path + "/" + data.name + ".json", savedJson);
            //Debug.Log("SAVED : " +  data.name + " to : " + Path + "/" + data.name + ".json");
        }
    }

    public void ReadData()
    {
        for (int i = 0; i < toSave.Count; i++)
        {
            string path = Application.isEditor ? Application.persistentDataPath + "/" + EditorPath : BuildPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(path + "/" + toSave[i].name + ".json"))
            {
                continue;
            }
            string readJson = File.ReadAllText(path + "/" + toSave[i].name + ".json");
            JsonUtility.FromJsonOverwrite(readJson, toSave[i]);
            //Debug.Log("READ : " + toSave[i].name + " from : " + Path + "/" + toSave[i].name + ".json");
        }
    }

}
