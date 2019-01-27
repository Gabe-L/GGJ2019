using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    public AudioClip GunSFX;
    public AudioSource SFXSource;

	// Use this for initialization
	void Start () {
        SFXSource.clip = GunSFX;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Cross"));
            SFXSource.Play();

	}
}
