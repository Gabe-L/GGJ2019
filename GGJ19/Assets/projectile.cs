using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {
    float lifeTimer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > 2.0f)
        {
            Destroy(gameObject);
        }
	}
}
