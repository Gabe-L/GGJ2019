using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    const float minIntensity = 19.0f;
    const float maxIntensity = 21.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Light>().intensity = Random.Range(minIntensity, maxIntensity);

    }
}
