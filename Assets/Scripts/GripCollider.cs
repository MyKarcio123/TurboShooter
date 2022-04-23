using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripCollider : MonoBehaviour
{
    [HideInInspector]
    public bool collided = false;
    private void Update()
    {
        Debug.Log(collided);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("TAK");
        if (collision.gameObject.layer == 9)
        {
            collided = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        collided = false;
    }
}
