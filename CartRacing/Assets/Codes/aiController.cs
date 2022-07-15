using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiController : MonoBehaviour
{
    public Circuit circuit;
    public float brakingSensitivity = 1f;
    Drive ds;
    public float steeringSensitivity = 0.01f;
    public float accSensitivity = 0.03f;
    Vector3 target;
    Vector3 nextTarget;
    int CurrentWP = 0;
    float totalDistanceTarget;

    GameObject tracker;
    int currentTrackerWP = 0;
    float lookAhead = 10;

    float lastTimeMoving = 0;

    // Start is called before the first frame update
    void Start()
    {
        ds = this.GetComponent<Drive>();
        target = circuit.wayPoints[CurrentWP].transform.position;
        nextTarget = circuit.wayPoints[CurrentWP + 1].transform.position;
        totalDistanceTarget = Vector3.Distance(target, ds.carRb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;

        tracker.transform.position = ds.carRb.gameObject.transform.position;
        tracker.transform.rotation = ds.carRb.gameObject.transform.rotation;

        this.GetComponent<Ghost>().enabled = false;
    }

    void ProgressTracker()
    {
        Debug.DrawLine(ds.carRb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(ds.carRb.gameObject.transform.position, tracker.transform.position) > lookAhead)
        {
            return;
        }

        tracker.transform.LookAt(circuit.wayPoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1f);

        if (Vector3.Distance(tracker.transform.position, circuit.wayPoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.wayPoints.Length)
            {
                currentTrackerWP = 0;
            }
        }
    }

    // Update is called once per frame
    bool isJump = false;

    void ResetLayer()
    {
        ds.carRb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }
    void Update()
    {
        if (!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }
        ProgressTracker();
        Vector3 localTarget;
        //Vector3 nextLocalTarget = ds.carRb.gameObject.transform.InverseTransformPoint(nextTarget);
        //float distanceToTarget = Vector3.Distance(target, ds.carRb.gameObject.transform.position);

        float targetAngle;
        //float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        if (ds.carRb.velocity.magnitude > 1)
        {
            lastTimeMoving = Time.time;
        }

        if (Time.time > lastTimeMoving + 4)
        {
            ds.carRb.gameObject.transform.position = circuit.wayPoints[currentTrackerWP].transform.position + Vector3.up * 2 + new Vector3(Random.Range(-1,1),0,Random.Range(-1,1));
            tracker.transform.position = ds.carRb.gameObject.transform.position;
            ds.carRb.gameObject.layer = 6;
            this.GetComponent<Ghost>().enabled = enabled;
            Invoke("ResetLayer", 3);
        }

        if (Time.time < ds.carRb.GetComponent<AvoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * ds.carRb.GetComponent<AvoidDetector>().avoidPath;
        }
        else
        {
            localTarget = ds.carRb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);

        float speedFactor = ds.currentSpeed / ds.maxSpeed;

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90f;


        //float distanceFactor = distanceToTarget / totalDistanceTarget;
        //float speedFactor = ds.currentSpeed / ds.maxSpeed;

        //float acc = Mathf.Lerp(accSensitivity, 1, distanceFactor);
        float acc = 1f;
        if (corner > 20 && speedFactor > 0.2f)
        {
            acc = Mathf.Lerp(0, 1 * accSensitivity, 1 - cornerFactor);
        }
        // float brake = Mathf.Lerp(-1 - Mathf.Abs(nextTargetAngle) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);
        float brake = 0;
        if (corner > 10 && speedFactor > 0.1f)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);
        }
        /*if (Mathf.Abs(nextTargetAngle) > 20)
        {
            brake += 0.8f;
            acc += 0.8f;
        }*/
        /* if (distanceToTarget < 5)
         {
             brake = 0.8f;
             acc = 0.1f;
         }*/
        /* if (isJump)
         {
             acc = 1;
             brake = 0;
         }*/
        ds.Go(acc, steer, brake);

       /* if (distanceToTarget < 4) //threshold, make larger if car starts to circle waypoint; 
        {
            CurrentWP++;
            if (CurrentWP >= circuit.wayPoints.Length)
            {
                CurrentWP = 0;
            }

            target = circuit.wayPoints[CurrentWP].transform.position;
            if (CurrentWP == circuit.wayPoints.Length - 1)
            {
                nextTarget = circuit.wayPoints[0].transform.position;
            }
            else
            {
                nextTarget = circuit.wayPoints[CurrentWP + 1].transform.position;
            }
            totalDistanceTarget = Vector3.Distance(target, ds.carRb.gameObject.transform.position);


            if (ds.carRb.gameObject.transform.InverseTransformPoint(target).y > 5)
            {
                isJump = true;
            }
            else
            {
                isJump = false;
            }
        }*/

        ds.CheckForSkid();
        ds.CalculateEngineSound();
    }
}
