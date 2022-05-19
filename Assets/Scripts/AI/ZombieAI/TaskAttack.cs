using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskAttack : Node
{
    private float _attackTime = 1f;
    private float _attackCounter = 0f;
    private Transform _target;
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            //deal damage
            _attackCounter = 0f;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
