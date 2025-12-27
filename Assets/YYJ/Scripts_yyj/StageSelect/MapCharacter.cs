using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float arrivalDistance = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // 타겟 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position,targetPosition, moveSpeed * Time.deltaTime);
            // 도착 판정
            if (Vector3.Distance(transform.position, targetPosition) < arrivalDistance)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void SetTarget(Vector3 newPos)
    {
        targetPosition = new Vector3(newPos.x, newPos.y, transform.position.z);
        isMoving = true;
    }

    public void TeleportTo(Vector3 newPos)
    {
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        targetPosition = transform.position;
        isMoving = false;
    }
}
