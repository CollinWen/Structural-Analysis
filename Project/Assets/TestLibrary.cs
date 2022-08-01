using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dsg_wbm;

public class TestLibrary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Element e = new Element();
        Debug.Log(e.PosStart);
        Debug.Log(e.PosEnd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
