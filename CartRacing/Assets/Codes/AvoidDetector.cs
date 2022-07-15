using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidDetector : MonoBehaviour
{
    public float avoidPath = 0;
    public float avoidTime = 0;
    public float wanderDistance = 4; 
    public float avoidLenght = 1; // 1 sec


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "car")
        {
            return;
        }
        avoidTime = 0;

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "car")
        {
            return;
        }

        Rigidbody otherCar = collision.rigidbody;
        avoidTime = Time.time + avoidLenght;

        Vector3 otherCarLocalTarget = transform.InverseTransformPoint(otherCar.gameObject.transform.position);
        float otherCarAngle = Mathf.Atan2(otherCarLocalTarget.x, otherCarLocalTarget.z);
        avoidPath = wanderDistance * -Mathf.Sign(otherCarAngle);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
