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
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Light shipLightOne;
    [SerializeField] private Light shipLightTwo;
    private float timeTrack = 0.0f;
    private bool ropeFinished = false;
    private bool reel = false;
    private float reelSpeed = 0.0f;

    private Vector2 leftStick;
    private Vector2 rightStick;
    private bool ropeOut = false;
    private bool ropeSlowed = false;
    private float prevAngle = 0.0f;
    private GameObject turret;
    private GameObject point;

    Vector3 fireDirection;

    bool cross = false;
    bool square = false;
    bool circle = false;

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
        turret.transform.parent = GameObject.Find("RopeManager").transform;

        point = Instantiate(pointPrefab);
        
        point.transform.position = Vector3.zero;
        point.name = "Point";
    }

    public void UpdateInput()
    {
        cross = Input.GetButtonDown("Cross");
        square = Input.GetButtonDown("Square");

        leftStick.x = Input.GetAxis("LeftStickXAxis");
        leftStick.y = Input.GetAxis("LeftStickYAxis");

        rightStick.x = Input.GetAxis("RightStickXAxis");
        rightStick.y = Input.GetAxis("RightStickYAxis");

        circle = Input.GetButton("Circle");

    }

    // Update is called once per frame
    void Update()
    {
        if (circle && rightStick.magnitude > 0.2f)
        {
            Debug.Log("Rotating");
            if ((shipLightOne.transform.rotation.y < 40.0f && rightStick.x > 0) || (shipLightOne.transform.rotation.y > -40.0f && rightStick.x < 0))
            {
                shipLightOne.transform.Rotate(Vector3.up, 50.0f * rightStick.x * Time.deltaTime);
                shipLightTwo.transform.Rotate(Vector3.up, 50.0f * rightStick.x * Time.deltaTime);
            }
        }

        ropeOut = ropeJoints.Count > 0 ? true : false;

        float stickAngle = Mathf.Atan2(leftStick.y, leftStick.x) * Mathf.Rad2Deg;
        GameObject hpp = GameObject.Find("HarpoonParent");
        hpp.transform.rotation = Quaternion.identity;
        hpp.transform.Rotate(Vector3.up, stickAngle + 90);

        //turret.transform.position = transform.position + new Vector3(leftStick.x, 0,-leftStick.y);

        if (!ropeOut)
        {

            if (cross && leftStick.magnitude > 0.2f)
            {
                FireRope(-fireDirection);
                ropeJoints[0].GetComponent<Rigidbody>().AddForce(fireDirection * fireForce, ForceMode.Impulse);
            }
            fireDirection = new Vector3(leftStick.x, 0, -leftStick.y);
        }
        else
        {
            int track = 0;

            point.transform.position = ropeJoints[0].transform.position;
            if (!ropeJoints[0].GetComponent<FixedJoint>() && ropeJoints.Count > 1) {
                point.transform.forward = ((ropeJoints[0].transform.position - ropeJoints[1].transform.position).normalized).normalized;
            }


            foreach (var joint in ropeJoints)
            {
                track++;
                if (track == ropeJoints.Count) { break; }

                joint.GetComponent<LineRenderer>().SetPosition(0, joint.transform.position);
                joint.GetComponent<LineRenderer>().SetPosition(1, ropeJoints[track].transform.position);
            }

            float distToFirstJoint = Vector3.Distance(ropeJoints[0].transform.position, transform.position);
            if (distToFirstJoint < range && !ropeFinished)
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

                float rightAngle = Mathf.Atan2(rightStick.y, rightStick.x) * Mathf.Rad2Deg;

                float angProp = Mathf.Abs(rightAngle - prevAngle) / 360.0f;

                prevAngle = rightAngle;
                angProp *= 20;
                angProp *= 0.1f;
                angProp = Mathf.Min(angProp, 0.5f);

                //angProp = Mathf.Max(0.5f, angProp * 100);


                ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
                ropeJoints[ropeJoints.Count - 1].transform.position = Vector3.MoveTowards(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position, angProp);
                Debug.Log(angProp * 0.1f);

                if (Vector3.Distance(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position) < 0.1f)
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

        if (ropeFinished && ropeJoints.Count <= 2 || square)
        {
            Debug.Log("Trying");
            RemoveJoint();
            RemoveJoint();
        }

    }

    void FireRope(Vector3 direction)
    {
        GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
        newRopeJoint.transform.parent = GameObject.Find("RopeManager").transform;
        newRopeJoint.transform.position = transform.position;
        newRopeJoint.AddComponent<Hook>();
        newRopeJoint.GetComponent<Renderer>().enabled = false;
        point.transform.GetChild(0).GetComponent<Renderer>().enabled = true;

        ropeJoints.Add(newRopeJoint);
    }

    public void SlowJoints()
    {
        if (ropeSlowed) { return; }
        foreach (var joint in ropeJoints)
        {
            joint.GetComponent<Rigidbody>().velocity /= 8 * (ropeJoints.Count / 5);
            //joint.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        ropeSlowed = true;

        //ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        //ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;

        //var headOfTheRope = ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>();
        //headOfTheRope.velocity = -headOfTheRope.velocity * 10;
        Debug.Log("Slowing");
    }

    void AddRopeSegment(GameObject previousSegment)
    {
        GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
        newRopeJoint.transform.parent = GameObject.Find("RopeManager").transform;
        newRopeJoint.transform.position = transform.position;// previousSegment.transform.position - fireDirection;
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
            point.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        }

    }

}