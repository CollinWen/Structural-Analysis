using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayLine : MonoBehaviour
{
    [SerializeField] private Material lineMat;


    void DrawLine(Vector3 start, Vector3 end) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = lineMat;
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void DrawSphere(Vector3 location, float radius) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
    }

    
    
    // Start is called before the first frame update
    void Start() {
        DrawLine(new Vector3(0,0,0), new Vector3(5,5,5));
        DrawSphere(new Vector3(1,2,3), 1.5f);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
