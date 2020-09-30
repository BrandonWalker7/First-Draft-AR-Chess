using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneVisual : MonoBehaviour
{

    public Material MatTransparent;
    // Start is called before the first frame update
    void Start()
    {
        GoogleARCore.Examples.HelloAR.ARChess chessScript = GameObject.FindGameObjectWithTag("ARCoreController").GetComponent<GoogleARCore.Examples.HelloAR.ARChess>();

        if(chessScript.spawn==true)
        {
            GetComponent<MeshRenderer>().material = MatTransparent;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
