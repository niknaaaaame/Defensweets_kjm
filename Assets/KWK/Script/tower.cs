using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float interval = 1f;
    [SerializeField] private float fireTIme = 0.3f;

    private List<Transform> enemiesInRange = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        interval -= Time.deltaTime;

        if (enemiesInRange.Count > 0)
        {

        }
    }
    void Shoot()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }

}