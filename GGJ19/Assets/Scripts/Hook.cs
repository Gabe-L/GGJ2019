using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody) {
            gameObject.AddComponent<FixedJoint>();
            GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
        }
    }

}
