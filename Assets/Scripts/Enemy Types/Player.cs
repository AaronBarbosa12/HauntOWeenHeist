using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IEnemyMoveable
{
    //Movement 
    [field: SerializeField] public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Rigidbody2D RB { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsFacingRight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;


        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    public void Damage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void MoveEnemy(Vector2 velocity)
    {
        throw new System.NotImplementedException();
    }

    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        throw new System.NotImplementedException();
    }
}