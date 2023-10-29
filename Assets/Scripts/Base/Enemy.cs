using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public Rigidbody2D RB { get; set; }
    public bool IsFacingRight { get; set; } = false;

    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    #endregion

    public bool IsAggroed { get; set; } = false;
    public bool IsWithinStrikingDistance { get; set; } = false;

    #region Idle Variables
    public GameObject BulletPrefab;

    #endregion

    #region ScriptableObject Variables
    [SerializeField] private EnemyIdleSOBase enemyIdleBase;
    [SerializeField] private EnemyChaseSOBase enemyChaseBase;
    [SerializeField] private EnemyAttackSOBase enemyAttackBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance {get; set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }  
    #endregion


    protected void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(enemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(enemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(enemyAttackBase);

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        //ChaseState = new EnemyChaseState(this, StateMachine);
        //AttackState = new EnemyAttackState(this, StateMachine);

    }
    public void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        CurrentHealth = MaxHealth;

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        //EnemyChaseBaseInstance.Initialize(gameObject, this);
        //EnemyAttackBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    protected void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    protected void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }
    #region Movement
    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {

    }
    public void MoveEnemy(Vector2 velocity)
    {
        RB.velocity = velocity;
        CheckForLeftOrRightFacing(velocity);
    }
    #endregion

    #region Health Checks
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount; 

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        throw new System.NotImplementedException();
    }
    #endregion
    #region Distance Checks
    public void SetAggroStatus(bool isAggroed)
    {
        this.IsAggroed = isAggroed;
    }

    public void SetStriking(bool isWithinStrikingDistance)
    {
        this.IsWithinStrikingDistance = isWithinStrikingDistance;
    }
    #endregion

    #region Animation Triggers
    private void AnimationTriggerEven(AnimationTriggerType triggerType) {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }
    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayeFootStepSound
    }
    #endregion
}
