using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Churros : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Transform target;
    private Coroutine moveCoroutine;
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setting(Transform newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;

        moveCoroutine = StartCoroutine(move());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Destroy(gameObject);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            //MonsterTest monster = other.GetComponent<MonsterTest>(); -여영부-
            IDamageable monster = other.GetComponent<IDamageable>();
            if (monster == null)
            {
                return;
            }
            monster.TakeDamage(damage);

            StopCoroutine(moveCoroutine);
            Destroy(gameObject);
        }
    }*/

    private IEnumerator move()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;
        float distance = Vector3.Distance(startPos, endPos);
        float elapsedTime = 0f;
        float maxHeight = 1.5f;

        float t = 0f;
        while (t < 1f)
        {
            elapsedTime += Time.deltaTime;
            t = (elapsedTime * speed) / distance;

            Vector3 nextPos = Vector3.Lerp(startPos, endPos, t);

            float height = Mathf.Sin(t * Mathf.PI) * maxHeight;
            nextPos.y += height;

            Vector3 direction = nextPos - transform.position;
            if (direction != Vector3.zero)
            {
                // 2D 회전: Atan2 사용 (스프라이트가 위쪽을 향하도록 만들었다면 -90 등 보정)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // 만약 스프라이트가 위(0,1)를 향하도록 만들어졌다면 -90도 보정 필요
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

                //transform.rotation = Quaternion.LookRotation(direction);
            }

            transform.position = nextPos;

            yield return null;
        }

        transform.position = endPos;
    }

    /*
     private IEnumerator move()
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target.position);
        float elapsedTime = 0f;
        float maxHeight = 1.5f;

        float t = 0f;
        while (t < 1f)
        {
            elapsedTime += Time.deltaTime;

            t = (elapsedTime * speed) / distance;

            Vector3 currentPos = Vector3.Lerp(startPos, target.position, t);
            float height = Mathf.Sin(t * Mathf.PI) * maxHeight;
            currentPos.y += height;

            transform.position = currentPos;

            yield return null;
        }

        transform.position = target.position;
    }
     */
}
