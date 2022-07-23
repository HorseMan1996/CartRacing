using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameUIController : MonoBehaviour
{
    public TMP_Text playerName;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRend;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<TMP_Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    private void LateUpdate()
    {
        if (carRend == null)
        {
            return;
        }
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView ? 1 : 0;
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.2f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
