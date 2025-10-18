using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChocochipTower : MonoBehaviour
{
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private List<Transform> enemiesInRange = new List<Transform>();
    private float fireCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if(enemiesInRange.Count > 0 && fireCooldown <= 0f)
        {
            Transform target = enemiesInRange[0];
            Shoot(target);
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot(Transform target)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Monster"))
        {
            enemiesInRange.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Monster"))
        {
            enemiesInRange.Remove(other.transform);
        }
    }
}
