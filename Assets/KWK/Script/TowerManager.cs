using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    private GameObject selectedTower;
    private GameObject ghostTower;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ghostTower != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            ghostTower.transform.position = mousePos;

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(selectedTower, mousePos, Quaternion.identity);
                Destroy(ghostTower);
                ghostTower = null;
                selectedTower = null;
            }

            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghostTower);
                ghostTower = null;
                selectedTower = null;
            }
        }
    }

    public void SelectTower(GameObject towerPrefab)
    {
        if (ghostTower != null)
        {
            Destroy(ghostTower);
        }

        selectedTower = towerPrefab;

        ghostTower = Instantiate(towerPrefab);

        SetLayerAlpha(ghostTower, 0.5f);

    }

    void SetLayerAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;

        foreach (Transform child in obj.transform)
        {
            SetLayerAlpha(child.gameObject, alpha);
        }
    }
}
