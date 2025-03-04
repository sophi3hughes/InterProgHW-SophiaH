using UnityEngine;

// fast enemy
public class Enemy1 : BaseEnemy
{
    protected override void Start()
    {
        base.Start();
        health = 50f;
        moveSpeed = 5f;
    }

    protected override void Move()
    {
        base.Move();
    }
}
