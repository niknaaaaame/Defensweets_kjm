using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public float attackRange = 4.0f;
    public float attackDuration = 0.5f;
    public float attackCooldown = 2.0f;
    public int damage = 10;

    public string targetTag = "Tower";

    private Monster monsterMovement;
    private float lastAttackTime;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
