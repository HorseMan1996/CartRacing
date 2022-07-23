using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Drive : MonoBehaviour
{
    public WheelCollider[] WC;
    public GameObject[] Wheel;

    public float torque = 200;

    public float maxSteeringAngle = 30;

    public float maxBrakeTorque = 500f;

    public AudioSource skidSound;
    public AudioSource engineSound;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public GameObject brakeLight;

    public Rigidbody carRb;
    public float gearLength = 3;
    public float currentSpeed { get { return carRb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPerc;
    public float maxSpeed = 200;

    public GameObject playerNamePrefabs;
    public Renderer jeepMesh;

    string[] aiNames = { "Enes", "Kaan", "Berkay", "Nuh", "Mehmet", "Samet"};

    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];
    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }

        skidTrails[i].parent = WC[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WC[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            return;
        }

        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }
    
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        brakeLight.SetActive(false);

        GameObject playerName = Instantiate(playerNamePrefabs);
        playerName.GetComponent<NameUIController>().target = carRb.gameObject.transform;

        if (this.GetComponent<aiController>().enabled)
        {
            playerName.GetComponent<Text>().text = aiNames[Random.Range(0, aiNames.Length)];
        }
        else
        {
            playerName.GetComponent<Text>().text = "Racer";
        }
        playerName.GetComponent<NameUIController>().carRend = jeepMesh;
    }

    public void CalculateEngineSound()
    {
        float gearPercentage = ( 1 / (float)numGears );
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));

        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears * currentGear);

        if (currentGear>0&&speedPercentage<downGearMax)
        {
            currentGear--;
        }
        if (speedPercentage>upperGearMax&&(currentGear < (numGears -1)))
        {
            currentGear++;
        }
        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        engineSound.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    public void Go(float acc, float steer, float brake)
    {
        acc = Mathf.Clamp(acc, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteeringAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        float thrustTorque = 0;
        if (currentSpeed < maxSpeed)
        {
            thrustTorque = acc * torque;
        }
        

        if (brake != 0)
        {
            brakeLight.SetActive(true);
        }
        else
        {
            brakeLight.SetActive(false);
        }


        for(int i = 0; i < 4; i++)
        {
            WC[i].motorTorque = thrustTorque;

            if (i < 2 )
            {
                WC[i].steerAngle = steer;
            }

            else
            {
                WC[i].brakeTorque = brake;
            }

            Quaternion quat;
            Vector3 position;
            WC[i].GetWorldPose(out position, out quat);
            Wheel[i].transform.position = position;
            Wheel[i].transform.rotation = quat;
        }

    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WC[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                StartSkidTrail(i);
                skidSmoke[i].transform.position = WC[i].transform.position - WC[i].transform.up * WC[i].radius;
                skidSmoke[i].Emit(1);
            }
            else
            {
                EndSkidTrail(i);
            }
        }

        if (numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }


}
