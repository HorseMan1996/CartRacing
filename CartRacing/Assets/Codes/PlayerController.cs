using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive ds;
    float lastTimeMoving = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;

    void ResetLayer()
    {
        ds.carRb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }
    void Start()
    {
        ds = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;
    }

    void Update()
    {

        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        if (ds.carRb.velocity.magnitude > 1 || !RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
        }

        RaycastHit hit;
        if (Physics.Raycast(ds.carRb.gameObject.transform.position, -Vector3.up, out hit, 10))
        {
            if (hit.collider.gameObject.tag == "road")
            {
                lastPosition = ds.carRb.gameObject.transform.position;
                lastRotation = ds.carRb.gameObject.transform.rotation;
            }
        }

        if (Time.time > lastTimeMoving + 4)
        {
            ds.carRb.gameObject.transform.position = lastPosition;
            ds.carRb.gameObject.transform.rotation = lastRotation;
            ds.carRb.gameObject.layer = 6;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (!RaceMonitor.racing)
        {
            a = 0;
        }
        ds.Go(a, s, b);

        ds.CheckForSkid();

        ds.CalculateEngineSound();
    }
}
