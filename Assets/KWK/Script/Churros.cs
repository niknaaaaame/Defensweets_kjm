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
            //MonsterTest monster = other.GetComponent<MonsterTest>(); -¿©¿µºÎ-
            IDamageable monster = other.GetComponent<IDamageable>();
            if (monster == null)
            {
                return;
            }
            monster.TakeDamage(damage);

            StopCoroutine(moveCoroutine);
            Destroy(gameObject);
        }
    }

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
}
