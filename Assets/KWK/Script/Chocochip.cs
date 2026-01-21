using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chocochip : MonoBehaviour
{
    [SerializeField] private float speed;

    private Transform target;
    private Rigidbody2D rb;
    private Coroutine moveCoroutine;
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setting(Transform newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;

        rb = GetComponent<Rigidbody2D>();

        moveCoroutine = StartCoroutine(move());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Monster"))
        {
            MonsterTest monster = other.GetComponent<MonsterTest>();
            monster.TakeDamage(damage);

            StopCoroutine(moveCoroutine);
            Destroy(gameObject);
        }
    }

    private IEnumerator move()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        yield return new WaitForSeconds(0.1f);
    }
}
