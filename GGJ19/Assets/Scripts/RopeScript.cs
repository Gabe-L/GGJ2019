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
    [SerializeField] private Light shipLight;
    private float timeTrack = 0.0f;
    private bool ropeFinished = false;
    [HideInInspector] public bool reel = false;
    private float reelSpeed = 0.0f;

    private Vector2 leftStick;
    private Vector2 rightStick;
    [HideInInspector] public bool ropeOut = false;
    private bool ropeSlowed = false;
    private float prevAngle = 0.0f;
    private GameObject turret;
    private GameObject point;
    private Vector3 originalParentPos;

    Vector3 fireDirection;

    bool L1orR1 = false;
    bool square = false;
    bool circle = false;

    Vector3 debugPoint = Vector3.zero;

    private void OnDrawGizmos()
    {
        var spaceshipCenter = FindObjectOfType<Spaceship>().transform.position;
        Gizmos.DrawLine(spaceshipCenter, spaceshipCenter + (debugPoint * 10));
    }

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
        L1orR1 = Input.GetButtonDown("L1") || Input.GetButtonDown("R1");
        square = Input.GetButtonDown("Square");

        leftStick.x = FindObjectOfType<Player>().GetRelativeStickDirection(Player.Stick.Left).x;
        leftStick.y = -FindObjectOfType<Player>().GetRelativeStickDirection(Player.Stick.Left).z;

        rightStick.x = Input.GetAxis("RightStickXAxis");
        rightStick.y = Input.GetAxis("RightStickYAxis");

        circle = Input.GetButton("Circle");

    }

    // Update is called once per frame
    void Update()
    {

        //if (circle && rightStick.magnitude > 0.2f)
        //{
        //    GameObject ship = GameObject.Find("Spaceship");

        //    float oldYPos = shipLight.transform.position.y;

        //    shipLight.transform.Rotate(Vector3.up, 70.0f * rightStick.x * Time.deltaTime);
        //    Vector3 shipLightPosOne = shipLight.transform.position;
        //    shipLightPosOne = ship.transform.position;
        //    shipLightPosOne += shipLight.transform.forward * 3;
        //    shipLightPosOne.y = oldYPos;
        //    shipLight.transform.position = shipLightPosOne;
        //}

        ropeOut = ropeJoints.Count > 0 ? true : false;
        GameObject hpp = GameObject.Find("HarpoonParent");

        float stickAngle = Mathf.Atan2(leftStick.y, leftStick.x) * Mathf.Rad2Deg;
        stickAngle += 45;
        hpp.transform.rotation = Quaternion.identity;
        hpp.transform.Rotate(Vector3.up, stickAngle);

        if (!ropeOut)
        {

            if (L1orR1 && leftStick.magnitude > 0.2f)
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
            if (!ropeJoints[0].GetComponent<FixedJoint>() && ropeJoints.Count > 1)
            {
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
                if (Vector3.Distance(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position) >= 1.0f)
                {
                    AddRopeSegment(ropeJoints[ropeJoints.Count - 1]);

                    if (!ropeFinished)
                    {
                        ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().AddForce(fireDirection * fireForce, ForceMode.Impulse);
                    }
                }
                else if (ropeJoints[0].GetComponent<FixedJoint>())
                {
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

                ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
                ropeJoints[ropeJoints.Count - 1].transform.position = Vector3.MoveTowards(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position, angProp);

                if (Vector3.Distance(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position) < 0.1f)
                {
                    RemoveJoint();
                }
            }

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
            joint.GetComponent<Rigidbody>().velocity /= 8 * (ropeJoints.Count / 10);
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