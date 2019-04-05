using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaionController : MonoBehaviour
{
    [SerializeField]
    float speed = 100;

    GameObject modelParent;
    GameObject modelPivot;

    void Start()
    {
        modelParent = GameObject.FindGameObjectWithTag("modelParent");
        modelPivot = modelParent.transform.Find("Pivot").gameObject;
    }
    
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            modelParent.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
        }
        if (Input.GetMouseButtonUp(1))
        {
            modelPivot.transform.parent = null;
            modelParent.transform.rotation = Quaternion.identity;
            modelPivot.transform.parent = modelParent.transform;
        }
    }

    private void ResetRotation()
    {
        modelParent.transform.rotation = Quaternion.identity;
        modelPivot.transform.rotation = Quaternion.identity;
    }
}
