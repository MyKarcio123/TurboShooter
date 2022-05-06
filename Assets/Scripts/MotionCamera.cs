using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCamera : MonoBehaviour
{
    public Transform mainCamera;
    public Transform player;
    public Animator animator;
    void Update()
    {
        gameObject.transform.rotation = mainCamera.rotation;
        gameObject.transform.position = player.position;
        if (Input.GetKeyDown(KeyCode.F))
            PlayAnimation();
    }
    public bool PlayAnimation()
    {
        animator.Play("LedgeAnimation",0,0f);
        return true;
    }
}
