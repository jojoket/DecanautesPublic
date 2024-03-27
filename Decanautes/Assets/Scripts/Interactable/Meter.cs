using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Meter : MonoBehaviour
{
    [TabGroup("Components")]
    public GameObject fill;
    public MeshRenderer fillRenderer;
    [HideInInspector]
    public Material material;

    [TabGroup("Property")]
    [PropertyRange(0,1)]
    public float FillAmount;
    [TabGroup("Property")]
    public Vector3 FillSizeMax;
    [TabGroup("Property")]
    public Vector3 FillDirection;

    private Vector3 baseScale;
    private Vector3 basePosition;

    // Start is called before the first frame update
    void Start()
    {
        baseScale = fill.transform.localScale;
        basePosition = fill.transform.position;
        fillRenderer = fill.GetComponent<MeshRenderer>();
        material = Instantiate<Material>(fillRenderer.material);
        fillRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        FillVisual();
    }


    private void FillVisual()
    {
        Vector3 currentFill = FillAmount * FillSizeMax;
        fill.transform.localScale = baseScale + currentFill;
        fill.transform.position = basePosition + new Vector3(currentFill.x * FillDirection.x/2, currentFill.y * FillDirection.y/2, currentFill.z * FillDirection.z/2); 
    }

    private void OnDrawGizmosSelected()
    {
        Mesh mesh = fill.GetComponent<MeshFilter>().sharedMesh;
        Vector3 currentFill = FillAmount * FillSizeMax;
        Vector3 scale = fill.transform.localScale + currentFill;
        Vector3 pos = fill.transform.position + new Vector3(currentFill.x * FillDirection.x/2, currentFill.y * FillDirection.y/2, currentFill.z * FillDirection.z/2);

        Gizmos.DrawWireMesh(mesh, pos, fill.transform.rotation, scale);
    }

}
