using UnityEngine;

public class Enemy2 : BaseEnemy
{
    protected override void Start()
    {
        base.Start();
        health = 200f;
        moveSpeed = 1f;
    }

    protected override void Move()
    {
        base.Move();
    }
}
