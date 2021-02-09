using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -2);
    public float smoothSpeed = 0.125f;
    public GameObject target;
    void FixedUpdate()
    {
        try
        {
            Vector3 desiredPos = target.transform.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.fixedDeltaTime);
            transform.position = smoothedPos;
        }
        catch
        {
            Debug.LogError("CameraController : Target is null.");
            target = GameObject.FindWithTag("Player");
            // retrouver un game object 
        }
    }
}
