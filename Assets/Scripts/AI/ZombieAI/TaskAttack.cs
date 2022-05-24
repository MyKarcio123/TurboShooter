using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskAttack : Node
{
    private float _attackTime = 1f;
    private float _attackCounter = 0f;
    private Transform _transform;
    public TaskAttack(Transform transform)
    {
        _transform = transform;
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        Debug.Log("Attacking");
        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            //deal damage
            _attackCounter = 0f;
        }
        Vector3 direction = new Vector3(target.position.x - _transform.position.x, _transform.position.y, target.position.z - _transform.position.z);
        Quaternion toRotation = Quaternion.LookRotation(direction);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, toRotation, 1f * Time.deltaTime);
        Debug.Log(Quaternion.Dot(_transform.rotation, toRotation));
        /*
        if (Quaternion.Dot(_transform.rotation, toRotation) > 0.9999f)
        {
            Debug.Log("YES");
        }
        /*
        counter += Time.deltaTime;
        if (counter >= 10f * Time.deltaTime)
        {
            Debug.Log("Yeeees");
            counter = 0;
        }*/
        state = NodeState.RUNNING;
        return state;
    }
}
