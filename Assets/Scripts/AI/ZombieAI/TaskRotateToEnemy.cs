using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskRotateToEnemy : Node
{
    private Transform _transform;
    public TaskRotateToEnemy(Transform transform)
    {
        _transform = transform;
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        /*
        Vector3 direction = new Vector3(target.position.x - _transform.position.x, _transform.position.y, target.position.z - _transform.position.z);
        Quaternion toRotation = Quaternion.LookRotation(direction);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, toRotation, 1f * Time.deltaTime);
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
