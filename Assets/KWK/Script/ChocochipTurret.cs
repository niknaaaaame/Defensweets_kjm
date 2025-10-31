using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChocochipTurret : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D other)
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
            shootCoroutine = StartCoroutine(shoot());
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

    private IEnumerator shoot()
    {
        while (targets.Count > 0)
        {
            GameObject instance = Instantiate(prefab, shootPoint.position, shootPoint.rotation);
            Bullet bullet = instance.GetComponent<Bullet>();
            
            if (bullet != null)
            {
                bullet.SetTarget(targets[0].transform);
                bullet.SetDamage(damage);
            }

            yield return new WaitForSeconds(interval);
        }

        shootCoroutine = null;
    }
}
