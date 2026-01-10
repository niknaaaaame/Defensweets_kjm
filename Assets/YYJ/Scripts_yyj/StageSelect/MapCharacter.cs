using System;
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

    private Queue<Vector3> pathQueue = new Queue<Vector3>();    // 이동 경로 큐
    private Vector3 currentTarget;  // 현재 이동 중 목표점
    private bool isMoving = false;
    private Action onArrivalCallback; // 도착 시 실행할 함수 저장 변수

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = transform.position; // 시작 시 현재 위치를 목표 설정, 제자리 유지
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // 타겟 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
            // 도착 판정
            if (Vector3.Distance(transform.position, currentTarget) < arrivalDistance)
            {
                transform.position = currentTarget; // 위치
                CheckNextWaypoint();    // 다음 지점 확인 
            }
        }
    }

    private void CheckNextWaypoint()    // 다음 경유지가 있으면 꺼내기, 없으면 이동 종료
    {
        if (pathQueue.Count > 0)
        {
            currentTarget = pathQueue.Dequeue();
        }
        else
        {
            isMoving = false;
            onArrivalCallback?.Invoke();    // 도착 시 콜백 실행
            onArrivalCallback = null;       // 콜백 초기화
        }
    }

    // 단일 목적지 아닌 경로를 받아 순서대로 이동
    public void MoveAlongPath(List<Vector3> pathPoints, Action onComplete = null)
    {
        pathQueue.Clear();
        onArrivalCallback = onComplete;

        if (pathPoints == null || pathPoints.Count == 0) return;

        // 전달받은 점들 큐에 추가
        foreach (Vector3 pos in pathPoints)
        {
            // y 오프셋 적용
            Vector3 adjustedPos = new Vector3(pos.x, pos.y + yOffset, transform.position.z);
            pathQueue.Enqueue(adjustedPos);
        }
        // 첫 목적지 설정 및 이동 시작
        if (pathQueue.Count > 0)
        {
            currentTarget = pathQueue.Dequeue();
            isMoving = true;
        }
        else
        {
            // 이동 필요 없을 시 바로 완료 처리
            onComplete?.Invoke();
        }
    }

    public void TeleportTo(Vector3 newPos)
    {
        transform.position = new Vector3(newPos.x, newPos.y + yOffset, transform.position.z);
        currentTarget = transform.position;
        pathQueue.Clear();
        isMoving = false;
        onArrivalCallback = null;
    }

    public List<Vector3> CalculatePath(StageNode startNode, StageNode targetNode)
    {
        List<Vector3> finalPath = new List<Vector3>();
        if (startNode == null || startNode == targetNode)
        {
            finalPath.Add(targetNode.transform.position);
            return finalPath;
        }
        // 시작 노드의 모든 조상을 찾아서 리스트에 저장
        List<StageNode> startAncestors = new List<StageNode>();
        StageNode current = startNode;
        while (current != null)
        {
            startAncestors.Add(current);
            current = current.parentStageNode;
        }
        // 목표 노드에서 거슬러 올라가며, 시작 노드의 조상과 겹치는 조상 찾기
        StageNode commonAncestor = null;
        List<StageNode> targetPathFromCommon = new List<StageNode>();

        current = targetNode;
        while (current != null)
        {
            if (startAncestors.Contains(current))
            {
                commonAncestor = current;
                break;
            }
            targetPathFromCommon.Add(current);  // 경로 추가
            current = current.parentStageNode;
        }

        if (commonAncestor == null)
        {
            Debug.LogWarning("두 노드가 연결되어 있지 않습니다.");
            finalPath.Add(targetNode.transform.position);
            return finalPath;
        }
        // 경로 생성
        foreach (var node in startAncestors)
        {
            if (node == commonAncestor) break;
            if (node != startNode) finalPath.Add(node.transform.position);
        }
        
        if (commonAncestor != startNode)
        {
            finalPath.Add(commonAncestor.transform.position);
        }

        targetPathFromCommon.Reverse();
        foreach (var node in targetPathFromCommon)
        {
            finalPath.Add(node.transform.position);
        }

        return finalPath;
    }
}