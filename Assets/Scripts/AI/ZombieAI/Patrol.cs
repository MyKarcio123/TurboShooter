using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Patrol : Node
{
    private Animator _animator;
    
    public Patrol(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        _animator.SetBool("Walking", false);
        state = NodeState.RUNNING;
        return state;
    }
}
