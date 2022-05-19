using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class PlayerinFOV : Node
{
    private Transform _transform;
    private static int _enemyLayerMask = 1 << 13;
    private Animator _animator;
    private NavMeshAgent _agent;

    public PlayerinFOV(Transform transform, NavMeshAgent agent)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _agent = agent;
    }
    public override NodeState Evaluate()
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, ZombieBT.fovRange, _enemyLayerMask);
            
        if(colliders.Length > 0)
        {
            parent.parent.SetData("target", colliders[0].transform);
            _animator.SetBool("Walking", true);
            state = NodeState.SUCCESS;
            return state;
        }
        _agent.isStopped = true;
        _animator.SetBool("Walking", false);
        state = NodeState.FAILURE;
        return state;
    }
}
