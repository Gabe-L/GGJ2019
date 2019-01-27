using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour {

    public bool isControlled = false;
    private Material activeMaterial;
    private Material inactiveMaterial;

    private void Awake()
    {
        activeMaterial = Resources.Load<Material>("Materials/Active Material");
        inactiveMaterial = Resources.Load<Material>("Materials/Inactive Material");
    }

    // Use this for initialization
    private void Start()
    {
        GetComponent<Renderer>().material = inactiveMaterial;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isControlled)
        {
            GetComponent<Renderer>().material = activeMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = inactiveMaterial;
        }
    }
}
