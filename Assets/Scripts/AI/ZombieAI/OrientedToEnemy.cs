using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class OrientedToEnemy : Node
{
    private Transform _transform;
    public OrientedToEnemy(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        Transform target = (Transform)t;
        Vector3 direction = new Vector3(target.position.x - _transform.position.x, _transform.position.y, target.position.z - _transform.position.z);
        Quaternion toRotation = Quaternion.LookRotation(direction);
        Debug.Log(toRotation);
        if (Quaternion.Angle(_transform.rotation, toRotation) <= 0.01f)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
