using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    private float minIntensity;
    private float maxIntensity;

    private void Awake()
    {
        minIntensity = GetComponent<Light>().intensity - 1;
        maxIntensity = GetComponent<Light>().intensity + 1;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Light>().intensity = Random.Range(minIntensity, maxIntensity);

    }
}
