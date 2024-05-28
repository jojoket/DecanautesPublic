using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Danger_Controller : MonoBehaviour
{
    public MeshRenderer Mesh1;
    public MeshRenderer Mesh2;

    public float dangerLevel;
    private Material[] Mat1;
    private Material[] Mat2;

    // Start is called before the first frame update
    void Start()
    {
        Mat1 = Mesh1.materials;

        Mat2 = Mesh2.materials;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material mat in Mat1)
        {
            if (mat.HasProperty("_Danger"))
            {
                mat.SetFloat("_Danger", dangerLevel);
            
            }
        }
        foreach (Material mat in Mat2)
        {
            if (mat.HasProperty("_Danger"))
            {
                mat.SetFloat("_Danger", dangerLevel);

            }
        }


    }
}
