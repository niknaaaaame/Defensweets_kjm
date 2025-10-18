using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int health = 10;
    public float speed = 2f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 move = new Vector2(speed * Time.fixedDeltaTime, 0);
        rb.MovePosition(rb.position + move);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Monster health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
