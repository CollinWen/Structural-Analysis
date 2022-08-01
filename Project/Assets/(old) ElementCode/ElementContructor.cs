using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(ElementMeshGenerator))]
public class ElementContructor : MonoBehaviour
{
    [SerializeField] private Material elementMat1;
    [SerializeField] private Material elementMat2;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    public ElementMeshGenerator emg;

    public MeshFilter mf;

    public MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(0,0,0);
        endPos = new Vector3(2,2,3);
        
        emg = new ElementMeshGenerator(new ElementData(startPos, endPos, "testElement"));
        emg.generateMesh();
        
        mf = GetComponent<MeshFilter>();
        mf.mesh = emg.mesh;

        mr = GetComponent<MeshRenderer>();
        mr.materials = new Material[1] {elementMat1};
    }

    // Update is called once per frame
    void Update()
    {
        emg = new ElementMeshGenerator(new ElementData(startPos, endPos, "testElement"));
        emg.generateMesh();
        
        mf = GetComponent<MeshFilter>();
        mf.mesh = emg.mesh;

        mr = GetComponent<MeshRenderer>();
        mr.materials = new Material[2] {elementMat1, elementMat2};
    }
}
