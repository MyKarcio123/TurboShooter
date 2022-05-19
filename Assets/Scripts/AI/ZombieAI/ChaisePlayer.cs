using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class ChaisePlayer : Node
{
    private Transform _transform;
    private NavMeshAgent _agent;
    public ChaisePlayer(Transform transform,NavMeshAgent agent)
    {
        _transform = transform;
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        Debug.Log(Vector3.Distance(new Vector3(_transform.position.x, 0, _transform.position.z), new Vector3(target.position.x, 0, target.position.z)) > 1f);
        if (Vector3.Distance(new Vector3(_transform.position.x, 0, _transform.position.z), new Vector3(target.position.x, 0, target.position.z)) > 1f)
        {
            _agent.SetDestination(new Vector3(target.position.x, 1, target.position.z));
            _agent.isStopped = false;
            //_transform.LookAt(new Vector3(target.position.x, 1, target.position.z));
        }
        else
        {
            //_agent.isStopped = true;
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
