/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaklingLaser : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int damage = 4;
    [SerializeField] private float interval = 1f;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Monster"))
        {
            return;
        }

        if(!targets.Contains(other))
        {
            targets.Add(other);
        }

        if(shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(shoot(other));
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Monster"))
        {
            return;
        }

        targets.Remove(other);

        if(targets.Count == 0 && shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator shoot(Collider2D other)
    {
        while (targets.Count > 0)
        {
            for(int i = targets.Count - 1; i >= 0; i--)
            {
                Collider2D colider = targets[i];

                if(Collider == null || colider.gameObject == null || colider.gameObject.activeSelf)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                MonsterTest monster = colider.GetComponent<MonsterTest>();
                if(monster != null)
                {
                    monster.TakeDamage(damage);
                }

                GameObject instance = Instantiate(prefab, shootPoint.position, shootPoint.rotation);
            }

            if(targets.Count == 0)
            {
                break;
            }

            yield return new WaitForSeconds(interval);
        }

        shootCoroutine = null;
    }

    private void OnDisable()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
        targets.Clear();
    }
}
*/