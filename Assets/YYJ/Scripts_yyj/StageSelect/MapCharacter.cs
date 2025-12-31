using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCharacter : MonoBehaviour
{
    [Header("움직임 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float arrivalDistance = 0.1f;

    [Header("위치 조정")]
    [SerializeField] private float yOffset = 0.5f;

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
        targetPosition = new Vector3(newPos.x, newPos.y + yOffset, transform.position.z);
        isMoving = true;
    }

    public void TeleportTo(Vector3 newPos)
    {
        Vector3 finalPos = new Vector3(newPos.x, newPos.y + yOffset, transform.position.z);
        transform.position = finalPos;
        targetPosition = finalPos;
        isMoving = false;
    }
}
