using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

    private bool hooked = false;
    Rigidbody hookedBody;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if (hooked)
  //      {
  //          if (!GetComponent<FixedJoint>().connectedBody)
  //          {
  //              GetComponent<FixedJoint>().connectedBody = hookedBody;
  //          }
  //      }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody && !FindObjectOfType<RopeScript>().ropeJoints.Contains(collision.gameObject) && !gameObject.GetComponent<FixedJoint>()) {
            gameObject.AddComponent<FixedJoint>();
            hookedBody = collision.rigidbody;
            GetComponent<FixedJoint>().connectedBody = hookedBody;
            FindObjectOfType<RopeScript>().SlowJoints();
        }
    }

}
