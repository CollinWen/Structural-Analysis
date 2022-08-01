using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Globalization;

public class TrussRenderer : MonoBehaviour
{
    // position of origin for truss
    public Vector3 position;

    // vertices of truss
    [SerializeField] public Vector3[] vertices;

    // members of truss, stored as array of arrays in the form [startPoint, endPoint]
    [SerializeField] public int[,] members;

    // file containing vertex points (line separated points in the form '{x, y, z}')
    public TextAsset vertexDataFile;

    // file containing truss members (line separated integers, where every two lines indicates start and end vertices)
    public TextAsset memberDataFile;

    // material for lines
    [SerializeField] private Material lineMat;
    
    // material for spheres
    [SerializeField] private Material sphereMat;

    // Given the vertexDataFile and memberDataFile, parses vertices and members and stores them in gameObject
    void parseData() {
        string[] verticesText = vertexDataFile.text.Split('\n');
        string[] membersText = memberDataFile.text.Split('\n');

        this.vertices = new Vector3[verticesText.Length];
        this.members = new int[membersText.Length/2, 2]; 

        for (int i = 0; i < verticesText.Length; i++) {
            string s = verticesText[i].Replace("}", string.Empty).Replace("{", string.Empty);
            string[] nums = s.Split(',');

            var fmt = new NumberFormatInfo();
            fmt.NegativeSign = "-";
            vertices[i] = new Vector3(float.Parse(nums[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(nums[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(nums[1], CultureInfo.InvariantCulture.NumberFormat));
        }

        for (int i = 0; i < membersText.Length; i+=2) {
            members[i/2, 0] = int.Parse(membersText[i]);
            members[i/2, 1] = int.Parse(membersText[i+1]);
        }
    }

    // renders a line in the specified location with given parameters
    void DrawLine(Vector3 start, Vector3 end, string memberId, float width, Color color) {
        GameObject myLine = new GameObject();
        myLine.name = memberId; 
        myLine.transform.parent = this.gameObject.transform;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lr.colorGradient = SingleColorGradient(color);

        lr.SetWidth(width, width);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    // renders a sphere in the specified location with given parameters
    void DrawSphere(Vector3 location, string vertexId, float radius=0.25f) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = vertexId;
        sphere.transform.parent = this.gameObject.transform;
        sphere.transform.position = location;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
        sphere.GetComponent<Renderer>().material = sphereMat;
    }

    // creates gradient with single color
    Gradient SingleColorGradient(Color newColor) {
        Gradient temp = new Gradient();
        GradientColorKey[] tempColorKeys = new GradientColorKey[2];
        tempColorKeys[0] = new GradientColorKey(newColor, 0);
        tempColorKeys[1] = new GradientColorKey(newColor, 1);

        temp.colorKeys = tempColorKeys;
        return temp;
    }

    // gets lengths of every member and stores in an array
    float[] GetLengths() {
        float[] lengths = new float[this.members.GetLength(0)];
        
        for (int i = 0; i < this.members.GetLength(0); i++) {
            lengths[i] = Vector3.Distance(this.vertices[this.members[i, 0]], this.vertices[this.members[i, 1]]);
        }

        Array.Sort(lengths);
        return lengths;
    }

    Color LengthToColor(float dist) {
        float[] lengths = GetLengths();
        float percentile = 0.0f;

        for (int i = 0; i < lengths.Length; i++) {
            if (dist > lengths[i]) {
                percentile = ((float)(i+1))/((float)lengths.Length);
            } else {
                break;
            }
        }

        Color c = new Color(1.0f, percentile, 0.0f, 1.0f);
        return c;
    }

    // Start is called before the first frame update
    void Start()
    {
        parseData();

        for(int i = 0; i < this.vertices.Length; i++) {
            DrawSphere(this.vertices[i], "Node " + i, 1.0f);
        }

        for(int i = 0; i < this.members.GetLength(0); i++) {
            float dist = Vector3.Distance(this.vertices[this.members[i, 0]], this.vertices[this.members[i, 1]]);
            DrawLine(this.vertices[this.members[i, 0]], this.vertices[this.members[i, 1]], "Line " + i, 0.25f, LengthToColor(dist));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
