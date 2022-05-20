using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class HPController : MonoBehaviour
{
    public bool player;
    [Header("AI Required")]
    public Component AI;
    public float forceMultiply;
    public GameObject objectWithDissolveShader;
    public Material dissolveMaterial;
    [Header("Player Required")]
    public TextMeshProUGUI HP;
    [Header("Required")]
    public float _hp;
  
    private bool die = false;
    private Rigidbody[] rigRigidbodies;
    private Vector3 beforePosition;
    private Vector3 currentPosition;
    private float timeCheckingRealDead = 2;
    private float counter = -1;
    private bool atDisolving = false;
    public void Awake()
    {
        beforePosition = gameObject.transform.position;
        rigRigidbodies = GetComponentsInChildren<Rigidbody>();
        if (player)
            HP.text = _hp.ToString();
        foreach (Rigidbody _rb in rigRigidbodies)
        {
            _rb.isKinematic = true;
        }
    }
    public void TakeHit(float damage,Vector3 hitposition)
    {
        _hp -= damage;
        if (_hp <= 0 && !player)
        {
            Destroy(AI);
            foreach (Rigidbody _rb in rigRigidbodies)
            {
                _rb.isKinematic = false;
                if (!die) 
                {
                    _rb.AddForce(hitposition*forceMultiply,ForceMode.Impulse);
                    beforePosition = gameObject.transform.position;
                }
            }
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            die = true;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        if (player)
            HP.text = _hp.ToString();
    }
    public void Update()
    {
        if (!player)
        {
            if (atDisolving)
            {
                objectWithDissolveShader.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_DisolveTime", counter);
                if (counter >= 1)
                    Destroy(gameObject);
                counter += Time.deltaTime;
            }
            else 
            {
                if (timeCheckingRealDead > 0 && die)
                {
                    timeCheckingRealDead -= Time.deltaTime;
                }
                else if(timeCheckingRealDead <=0 && die)
                {
                    if (beforePosition == currentPosition)
                    {
                        objectWithDissolveShader.GetComponent<SkinnedMeshRenderer>().material = dissolveMaterial;
                        atDisolving = true;
                    }
                    else
                    {
                        beforePosition = currentPosition;
                        currentPosition = gameObject.transform.position;
                        timeCheckingRealDead = 2;
                    }
                }
            }
        }
    }
}
