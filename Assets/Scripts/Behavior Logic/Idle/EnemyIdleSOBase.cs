using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    public virtual void Initialize( GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;   
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void DoEnterLogic(){ }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() {
        if (this.enemy.IsAggroed)
        {
            this.enemy.StateMachine.ChangeState(this.enemy.ChaseState);
        }
    }
    public virtual void DoPhysicsUpdateLogic() { }
    public virtual void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
}
