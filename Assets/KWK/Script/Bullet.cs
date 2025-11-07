using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private int damage = 0;
    private float lifetime = 3f;
    private Transform target;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChocochipTurret chocochipTurret = GetComponent<ChocochipTurret>();
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (Vector2)(target.position - transform.position).normalized;

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterTest monster = other.GetComponent<MonsterTest>();
            monster.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
