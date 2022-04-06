using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public GameObject bulletHoleGraphics;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject newHole = Instantiate(bulletHoleGraphics, collision.contacts[0].point + collision.contacts[0].normal *0.001f, Quaternion.identity) as GameObject;
        newHole.transform.LookAt(collision.contacts[0].point + collision.contacts[0].normal);
        Destroy(newHole, 5f);
        Destroy(gameObject);
    }
}
