using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceTest : MonoBehaviour
{
    public static ResourceTest Instance { get; private set; }

    [SerializeField] private int sugar;
    [SerializeField] private int crystal;

    [SerializeField] TextMeshProUGUI sugarText;
    [SerializeField] TextMeshProUGUI crystalText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
