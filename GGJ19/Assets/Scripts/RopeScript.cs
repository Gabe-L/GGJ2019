﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    public int ropeLength = 10;
    public float range = 20.0f;
    public float fireForce = 60.0f;

    private List<GameObject> ropeJoints;
    [SerializeField] private GameObject RopeSegmentPrefab;
    private float timeTrack = 0.0f;
    private bool ropeFinished = false;
    private bool reel = false;

    // Use this for initialization
    void Start()
    {
        ropeJoints = new List<GameObject>();
        CreateRope();
        ropeJoints[0].GetComponent<Rigidbody>().AddForce(Vector3.left * fireForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        int track = 0;

        foreach (var joint in ropeJoints)
        {
            track++;
            if (track == ropeJoints.Count) { break; }
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
                    ropeJoints[ropeJoints.Count - 1].GetComponent<Rigidbody>().AddForce(Vector3.left * fireForce, ForceMode.Impulse);
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
            ropeJoints[ropeJoints.Count - 1].transform.position = Vector3.MoveTowards(ropeJoints[ropeJoints.Count - 1].transform.position, transform.position, 1.0f);
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

    void CreateRope()
    {
        for (int i = 0; i < ropeLength; i++)
        {
            GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
            if (i > 0)
            {
                newRopeJoint.transform.parent = transform;
                newRopeJoint.transform.position = ropeJoints[i - 1].transform.position + Vector3.right;
                newRopeJoint.AddComponent<CharacterJoint>();
                newRopeJoint.GetComponent<CharacterJoint>().connectedBody = ropeJoints[i - 1].GetComponent<Rigidbody>();
                newRopeJoint.GetComponent<LineRenderer>().SetPosition(0, newRopeJoint.transform.position);
                newRopeJoint.GetComponent<LineRenderer>().SetPosition(1, ropeJoints[i - 1].transform.position);
            }
            else
            {
                newRopeJoint.transform.parent = transform;
                newRopeJoint.transform.position = transform.position;
                newRopeJoint.AddComponent<Hook>();
            }
            ropeJoints.Add(newRopeJoint);
        }
    }

    public void SlowJoints()
    {
        foreach (var joint in ropeJoints)
        {
            joint.GetComponent<Rigidbody>().velocity /= 4;
        }
    }

    void AddRopeSegment(GameObject previousSegment)
    {
        GameObject newRopeJoint = Instantiate(RopeSegmentPrefab);
        newRopeJoint.transform.parent = transform;
        newRopeJoint.transform.position = previousSegment.transform.position + Vector3.right;
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

    }

}