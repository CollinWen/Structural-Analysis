using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementData : MonoBehaviour
{
    // Element starting point
    public Vector3 startPoint;
    
    // Element ending point
    public Vector3 endPoint;
    
    // Shape of element extrusion (2D cartesian coordinates centered at (0,0))
    public Vector3[] extrusion;

    // Element label
    public string label;

    public ElementData(Vector3 start, Vector3 end, string lab = "") {
        this.startPoint = start;
        this.endPoint = end;
        this.extrusion = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,0,0)};
        this.label = lab;
    }

    public ElementData(Vector3 start, Vector3 end, Vector3[] ext, string lab = "") {
        this.startPoint = start;
        this.endPoint = end;
        this.extrusion = ext;
        this.label = lab;
    }

    public Vector3[] getVertices() {
        // reference: https://math.stackexchange.com/questions/100761/how-do-i-find-the-projection-of-a-point-onto-a-plane
        Vector3 normal = new Vector3(this.endPoint.x - this.startPoint.x, this.endPoint.y - this.startPoint.y, this.endPoint.z - this.startPoint.z);
        Vector3[] vertices = new Vector3[this.extrusion.Length*2];

        for(int i = 0; i < this.extrusion.Length; i++) {
            // generate projection at starting point
            float t = (normal.x*this.startPoint.x - normal.x*this.extrusion[i].x + normal.y*this.startPoint.y - normal.y*this.extrusion[i].y + normal.z*this.startPoint.z - normal.z*this.extrusion[i].z)/(normal.x*normal.x + normal.y*normal.y + normal.z*normal.z);
            Vector3 projectedPoint = new Vector3(extrusion[i].x + t*normal.x, extrusion[i].y + t*normal.y, extrusion[i].z + t*normal.z);
            vertices[i*2] = projectedPoint;

            // generate projection at ending point
            float t2 = (normal.x*this.endPoint.x - normal.x*this.extrusion[i].x + normal.y*this.endPoint.y - normal.y*this.extrusion[i].y + normal.z*this.endPoint.z - normal.z*this.extrusion[i].z)/(normal.x*normal.x + normal.y*normal.y + normal.z*normal.z);
            Vector3 projectedPoint2 = new Vector3(extrusion[i].x + t2*normal.x, extrusion[i].y + t2*normal.y, extrusion[i].z + t2*normal.z);
            vertices[i*2 + 1] = projectedPoint2;
        }

        return vertices;
    }
}
