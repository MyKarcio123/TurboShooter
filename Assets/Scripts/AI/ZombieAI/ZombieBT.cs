using System.Collections;
using System.Collections.Generic;
using BehaviorTree;

public class ZombieBT : Tree
{
    public static float speed = 2f;
    public static float fovRange = 15f;
    public static float attackRange = 2f;
    public UnityEngine.AI.NavMeshAgent agent;
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>{ 
                new CheckIfEnemyInRange(transform,agent),
                //new TaskRotateToEnemy(transform),
                new TaskAttack(transform),
            }),
            new Sequence(new List<Node>{
                new PlayerinFOV(transform,agent),
                new ChaisePlayer(transform,agent),
            }),
            new Patrol(transform),
        });
        return root;
    }
}
