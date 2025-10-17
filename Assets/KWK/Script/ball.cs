using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 3f;

    private Transform target;
    private Rigidbody2D ballrigidbody;

    // Start is called before the first frame update
    void Start()
    {
        ballrigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
