using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SmoothFollow : MonoBehaviour
{
    int currentCamera = 1;
    public CinemachineVirtualCamera followCamera;
    public CinemachineCameraOffset personFollow;
    public Transform target;
    public float distance = 0f;
    public float height = 1.5f;
    public float heightOffset = 1f;
    public float heightDamping = 4f;
    public float rotationDamping = 2f;

    int FP = -1;

    // Start is called before the first frame update
    void Start()
    {

    }
   /* private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (FP == 1)
        {

        }
        else
        {
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            transform.position = new Vector3(transform.position.x,
                                     currentHeight + heightOffset,
                                     transform.position.z);
            transform.LookAt(target);
        }
    }*/

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentCamera == 1)
            {
                currentCamera = 2;
            }
            else if (currentCamera == 2)
            {
                currentCamera = 1;
            }
        }
        if (currentCamera == 1)
        {
            personFollow.m_Offset = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(-0.4f, -1.28f, 4.68f), 1f);

        }
        else if (currentCamera == 2)
        {
            personFollow.m_Offset = Vector3.Lerp(new Vector3(-0.4f, -1.28f, 4.68f), new Vector3(0, 0, 0), 1f);
        }
    }
}
