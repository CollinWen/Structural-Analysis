using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElementData))]
public class ElementMeshGenerator : MonoBehaviour
{
    public ElementData data;

    public Mesh mesh;    

    public ElementMeshGenerator(ElementData data) {
        this.data = data;
        this.mesh = new Mesh();
        this.mesh.name = data.label;
    }

    public void generateMesh() {
        // get vertices from ElementData object
        Vector3[] vertices = this.data.getVertices();
        this.mesh.vertices = vertices;

        // defined subMeshes
        // this.mesh.subMeshCount = 1;
	    List<int> extrudedTriangles = new List<int>();
	    // List<int> endTriangles = new List<int>();

        for(int i = 0; i < vertices.Length; i+=2) {
            // bottom left triangle
            extrudedTriangles.Add(i);
            extrudedTriangles.Add((i+1) % vertices.Length);
            extrudedTriangles.Add((i+2) % vertices.Length);

            // top right triangle
            extrudedTriangles.Add((i+2) % vertices.Length);
            extrudedTriangles.Add((i+1) % vertices.Length);
            extrudedTriangles.Add((i+3) % vertices.Length);
        }

        for(int i = 0; i < vertices.Length; i++) {
            Debug.Log(vertices[i]);
        }

        // this.mesh.SetTriangles(endTriangles.ToArray(), 0);
        // this.mesh.SetTriangles(extrudedTriangles.ToArray(), 1);
        this.mesh.triangles = extrudedTriangles.ToArray();

        this.mesh.RecalculateNormals();    
    }
}
