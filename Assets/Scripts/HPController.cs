using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class HPController : MonoBehaviour
{
    public Component AI;
    public float _hp;
    public bool player;
    public float forceMultiply;
    public TextMeshProUGUI HP;
    private Rigidbody[] rigRigidbodies;
    private bool die = false;
    public void Awake()
    {
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
                }
            }
            die = true;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        if (player)
            HP.text = _hp.ToString();
    }
}
