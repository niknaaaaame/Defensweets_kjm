using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Basic : MonoBehaviour
{
    public TowerSO towerData;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform energyBar;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;
    private Vector3 originalScale;
    private float energy;

    // Start is called before the first frame update
    void Start()
    {
        energy = towerData.levels[0].energy;
        originalScale = energyBar.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = energy / towerData.levels[0].energy;
        energyBar.localScale = new Vector3(originalScale.x * ratio, originalScale.y, originalScale.z);
        
        float widthDifference = originalScale.x - energyBar.localScale.x;
        energyBar.localPosition = new Vector3(-widthDifference / 2f, energyBar.localPosition.y, energyBar.localPosition.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster"))
        {
            return;
        }

        if (!targets.Contains(other))
        {
            targets.Add(other);
        }

        if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(shoot());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Monster"))
        {
            return;
        }

        targets.Remove(other);

        if (targets.Count == 0 && shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator shoot()
    {
        while (targets.Count > 0)
        {
            GameObject instance = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            
            Bullet bullet = instance.GetComponent<Bullet>();
            bullet.Setting(targets[0].transform, towerData.levels[0].damage);
            
            energy -= 10;
            if(energy <= 0)
            {
                energy = 0;
                yield break;
            }

            yield return new WaitForSeconds(towerData.levels[0].attackSpeed);
        }

        shootCoroutine = null;
    }
}
