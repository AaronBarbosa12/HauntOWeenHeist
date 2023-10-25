using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private Transform targetTransform;

    private float timer;
    private float exitTimer; 
    private float timeBetweenShots = 0.5f;

    private float timeTillExit = 2f;
    private float distanceToCountExit = 3f;

    private float bulletSpeed = 2f;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (timer > timeBetweenShots)
        {
            timer = 0f;

            Vector2 dir = (targetTransform.position - enemy.transform.position).normalized;
            GameObject bullet = GameObject.Instantiate(enemy.BulletPrefab, enemy.transform.position, Quaternion.identity);
            Rigidbody2D RB = bullet.GetComponent<Rigidbody2D>();
            Vector2 bulletUpdate = dir * bulletSpeed;
            RB.velocity = new Vector3(bulletUpdate.x, bulletUpdate.y, 0);
        }

        if (Vector2.Distance(targetTransform.position, enemy.transform.position) > distanceToCountExit)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer > timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        }
        else
        {
            exitTimer = 0f;
        }

        timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
