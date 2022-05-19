using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class CheckIfEnemyInRange : Node
{
    private Transform _transform;
    private Animator _animator;
    private NavMeshAgent _agent;
    public CheckIfEnemyInRange(Transform transform, NavMeshAgent agent)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _agent = agent;
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
        if(Vector3.Distance(_transform.position,target.position) <= ZombieBT.attackRange)
        {
            _agent.isStopped = true;
            _animator.SetBool("Attacking", true);
            _animator.SetBool("Walking", false);
            state = NodeState.SUCCESS;
            return state;
        }
        _animator.SetBool("Attacking", false);
        _animator.SetBool("Walking", true);
        state = NodeState.FAILURE;
        return state;
    }
}
