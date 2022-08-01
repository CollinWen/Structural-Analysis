using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using UnityEngine;

public class AISCReader : MonoBehaviour
{
    public TextAsset textAssetData;

    // dictionary mapping AISC Manual Label to measurement values
    public static Dictionary<string, float[]> AISC_data;

    // dictionary mapping AISC Manual Label to shape type
    public static Dictionary<string, string> AISC_types;

    // top row labels for all fields in AISC database
    public static string[] AISC_labels;

    // Start is called before the first frame update
    void Start()
    {
        AISCReader.AISC_data = new Dictionary<string, float[]>();
        AISCReader.AISC_types = new Dictionary<string, string>();

        string[] rows = textAssetData.text.Split('\n');        
        string[] labels = rows[0].Split('\n');
        AISCReader.AISC_labels = labels.Skip(4).ToArray();

        for(int i = 0; i < AISCReader.AISC_labels.Length; i++) {
            Debug.Log(AISCReader.AISC_labels[i]);
        }

        for (int i = 1; i < rows.Length; i++) {
            string[] values = rows[i].Split(',');

            AISCReader.AISC_types.Add(values[2], values[0]);
            
            float[] numberValues = new float[values.Length - 4];
            for (int j = 4; j < values.Length; j++) {
                if (!values[j].Contains("-")) {
                    numberValues[j-4] = float.Parse(values[j], CultureInfo.InvariantCulture);
                }
            }

            AISCReader.AISC_data.Add(values[2], numberValues);
        }
    }

    // gets the AISC shape type given an ID
    public string getType(string id) {
        if (AISCReader.AISC_types.ContainsKey(id)) {
            return AISCReader.AISC_types[id];
        } else {
            throw new System.Exception("Invalid shape ID");
        }
    }

    // gets specific value of a parameter given an ID, returns null if parameter does not exist
    public float getValue(string id, string param) {
        int idx = Array.IndexOf(AISCReader.AISC_labels, param);
        
        if (idx == -1)  
            throw new System.Exception("Invalid parameter");

        return AISCReader.AISC_data[id][idx];
    }
}
