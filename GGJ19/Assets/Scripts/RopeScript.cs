using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    public int ropeLength = 10;
    public float range = 20.0f;
    public float fireForce = 60.0f;

    public List<GameObject> ropeJoints;
    [SerializeField] private GameObject RopeSegmentPrefab;
    private float timeTrack = 0.0f;
    private bool ropeFinished = false;
    private bool reel = false;
    private float reelSpeed = 0.0f;

    private bool ropeOut = false;
    private bool ropeSlowed = false;
    private float prevAngle = 0.0f;
    private GameObject turret;

    Vector3 fireDirection;

    // Use this for initialization
    void Start()
    {
        ropeJoints = new List<GameObject>();

        GameObject tempContain = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(tempContain.GetComponent<SphereCollider>());
        tempContain.transform.position = transform.position;

        turret = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(turret.GetComponent<SphereCollider>());
        turret.transform.position = transform.position;
        turret.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        ropeOut = ropeJoints.Count > 0 ? true : false;

        Vector2 leftStick;
        leftStick.x = Input.GetAxis("LeftStickXAxis");
        leftStick.y = Input.GetAxis("LeftStickYAxis");
        
        turret.transform.position = transform.position + new Vector3(leftStick.x, -leftStick.y, 0);

        if (ropeFinished && ropeJoints.Count <= 5 || Input.GetButtonDown("Square"))
        {
            Debug.Log("Trying");
            RemoveJoint();
            RemoveJoint();
            RemoveJoint();
            RemoveJoint();
            RemoveJoint();
        }

        if (!ropeOut)
        {

            if (Input.GetButtonDown("Cross") && leftStick.magnitude > 0.2f)
            {
                FireRope(-fireDirection);
                ropeJoints[0].GetComponent<Rigidbody>().AddForce(fireDirection * fireForce, ForceMode.Impulse);
            }
            fireDirection = new Vector3(leftStick.x, -leftStick.y, 0);
        }
        else
        {
            int track = 0;

            foreach (var joint in ropeJoints)
            {
                track++;
                if (track == ropeJoints.Count) { break; }

                Debug.Log(Vector3.Distance(joint.transform.position, ropeJoints[track].transform.position));
                joint.GetComponent<LineRenderer>().SetPosition(0, joint.transform.position);
                joint.GetComponent<LineRenderer>().SetPosition(1, ropeJoints[track].transform.position);
            }

            float distToFirstJoint = Vector3.Distance(ropeJoints[0].transform.position, transform.position);
            if (distToFirstJoint < range)
            {
                if (Vector3.Distance(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position) >= 2.0f)
                {
                    AddRopeSegment(ropeJoints[ropeJoints.Count - 1]);

                    if (!ropeFinished)
                    {
                        ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().AddForce(fireDirection * fireForce, ForceMode.Impulse);
                    }
                }
                else if (ropeJoints[0].GetComponent<FixedJoint>())
                {
                    //ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
                    ropeFinished = true;
                }
            }
            else
            {
                ropeFinished = true;
                SlowJoints();
            }

            if (ropeFinished)
            {
                Vector2 rightStick;
                rightStick.x = Input.GetAxis("RightStickXAxis");
                rightStick.y = Input.GetAxis("RightStickYAxis");

                float rightAngle = Mathf.Atan2(rightStick.y, rightStick.x) * Mathf.Rad2Deg;

                float angProp = Mathf.Abs(rightAngle - prevAngle) / 360.0f;

                prevAngle = rightAngle;
                angProp *= 20;
                //angProp = Mathf.Max(0.5f, angProp * 100);


                ropeJoints[ropeJoints.Count - 1].transform.position = Vector3.MoveTowards(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position, angProp);
                if (Vector3.Distance(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position) < 0.2f)
                {
                    RemoveJoint();
                }
            }

            //if (timeTrack >= 1.0f)
            //{
            //    AddRopeSegment(ropeJoints[ropeJoints.Count - 1]);
            //    timeTrack = 0.0f;
            //}
            timeTrack += Time.deltaTime;
        }
    }

    void FireRope(Vector3 direction)
    {
        GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
        newRopeJoint.transform.parent = transform;
        newRopeJoint.transform.position = transform.position;
        newRopeJoint.AddComponent<Hook>();

        ropeJoints.Add(newRopeJoint);
    }

    public void SlowJoints()
    {
        if (ropeSlowed) { return; }
        foreach (var joint in ropeJoints)
        {
            joint.GetComponent<Rigidbody>().velocity /= 8 * (ropeJoints.Count / 10);
            ropeSlowed = true;
        }

        ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;

        //var headOfTheRope = ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>();
        //headOfTheRope.velocity = -headOfTheRope.velocity * 10;
        Debug.Log("Slowing");
    }

    void AddRopeSegment(GameObject previousSegment)
    {
        GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
        newRopeJoint.transform.parent = transform;
        newRopeJoint.transform.position = previousSegment.transform.position - fireDirection;
        newRopeJoint.AddComponent<CharacterJoint>();
        newRopeJoint.GetComponent<CharacterJoint>().connectedBody = previousSegment.GetComponent<Rigidbody>();
        newRopeJoint.GetComponent<LineRenderer>().SetPosition(0, newRopeJoint.transform.position);
        newRopeJoint.GetComponent<LineRenderer>().SetPosition(1, previousSegment.transform.position);

        ropeJoints.Add(newRopeJoint);
    }

    public void RemoveJoint()
    {
        GameObject deleteRope = ropeJoints[ropeJoints.Count - 1];
        ropeJoints.Remove(ropeJoints[ropeJoints.Count - 1]);

        Destroy(deleteRope);

        if (ropeJoints.Count <= 0)
        {
            reel = false;
            ropeFinished = false;
            ropeSlowed = false;
        }

    }

}