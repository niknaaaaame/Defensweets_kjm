using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageSelectCamera : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapBackground;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float minZoomLimit = 0f;
    [SerializeField] private MapCharacter mapCharacter;
    [SerializeField] private Vector2 defaultStartPoint = Vector2.zero; // 저장 위치 없을 시 캐릭터 좌표
    
    private Camera cam;
    // 카메라 드래그
    private Vector3 dragOrigin;
    private bool isDragging = false;
    // 줌
    private bool isFocused = false;
    private StageNode currentFocusNode; 
    private Vector3 targetPosition;
    private float targetSize;
    private float maxPossibleSize;
    private Bounds mapBounds;
    private Vector3 moveVelocity;
    private float zoomVelocity;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        if (mapBackground != null)
        {
            mapBounds = mapBackground.bounds;
            // 초기 카메라 상태 저장
            CalculateMaxZoomSize();
        }
        else
        {
            Debug.LogError("배경이 연결되지 않았습니다.");
            maxPossibleSize = cam.orthographicSize;
        }
        // 초기값
        float startSize = Mathf.Clamp(cam.orthographicSize, minZoomLimit, maxPossibleSize);
        cam.orthographicSize = startSize;
        targetSize = startSize;
        // 마지막 방문 노드 위치 불러오기
        string sceneKey = SceneManager.GetActiveScene().name;
        Vector3 logicalPos;

        if (PlayerPrefs.HasKey(sceneKey + "_LastX"))
        {
            float lastX = PlayerPrefs.GetFloat(sceneKey + "_LastX");
            float lastY = PlayerPrefs.GetFloat(sceneKey + "_LastY");
            logicalPos = new Vector3(lastX, lastY, -10f);
        }
        else
        {
            logicalPos = new Vector3(defaultStartPoint.x, defaultStartPoint.y, -10f);
        }

        Vector3 clampedCamPos = ClampPosition(logicalPos, startSize);
        cam.transform.position = clampedCamPos;
        targetPosition = clampedCamPos;
        // 캐릭터 위치 노드 찾아 보상창 켜기
        StageNode nearestNode = null;
        float minDst = 10f;

        StageNode[] allNodes = FindObjectsOfType<StageNode>();
        foreach (var node in allNodes)
        {
            float dst = Vector3.Distance(node.transform.position, logicalPos);
            if (dst < minDst)
            {
                minDst = dst;
                nearestNode = node;
            }
        }

        if (nearestNode != null)
        {
            currentFocusNode = nearestNode;
            isFocused = true;
            nearestNode.ShowReward(true);
            // 캐릭터도 해당 노드로 텔레포트
            if (mapCharacter != null)  
                mapCharacter.TeleportTo(nearestNode.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        HandleInput();          // 클릭 / 드래그 입력 처리
        MoveCameraSmoothly();   // 카메라 목표 위치로 이동
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false;
        }

        if (Input.GetMouseButton(0) && !isFocused)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            float diffX = dragOrigin.x - currentPos.x;

            if (Mathf.Abs(diffX) > 0.01f)
            {
                isDragging = true;
                targetPosition.x += diffX * dragSpeed;
                // 맵 밖 제한
                targetPosition = ClampPosition(targetPosition, targetSize);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 드래그 상태가 아닐 경우
            if (!isDragging) HandleClick();
            isDragging = false;
        }
    }

    void HandleClick()
    {
        // 마우스 위치에 무엇이 있는지 확인
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // 스테이지 노드 클릭 시
            StageNode clickedNode = hit.collider.GetComponent<StageNode>();
            if(clickedNode != null)
            {
                if (clickedNode.CurrentState == StageState.Locked)
                {
                    Debug.Log("잠긴 스테이지입니다.");
                    return;
                }

                if (currentFocusNode == clickedNode && isFocused)
                {
                    ZoomToNode(clickedNode);
                    clickedNode.ShowInfo(true);
                }
                else
                {
                    FocusOnNode(clickedNode);
                }
                return;
            }
        }
        // 확대 상태 + 바깥 클릭 시
        if (isFocused) Unfocus();
    }
    // 이동
    void FocusOnNode(StageNode targetNode)
    {
        // 현재 위치 모른다면, 캐릭터와 가까운 노드 찾기
        if (currentFocusNode == null)
        {
            float minDistance = float.MaxValue;
            StageNode nearest = null;
            StageNode[] allNodes = FindObjectsOfType<StageNode>();
            
            foreach (var node in allNodes)
            {
                // 맵 캐릭터 있다면 캐릭터 기준, 없다면 카메라 기준
                Vector3 searchPos;
                if (mapCharacter != null)
                {
                    searchPos = mapCharacter.transform.position;
                }
                else
                {
                    searchPos = (Vector3)targetPosition;
                }
                float dist = Vector3.Distance(node.transform.position, searchPos);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = node;
                }
            }
            // 가장 가까운 노드를 현재위치로 간주
            currentFocusNode = nearest;
        }
        if (currentFocusNode != null)
        {
            currentFocusNode.ShowInfo(false);
            currentFocusNode.ShowReward(false);
        }

        // 경로 계산
        List<Vector3> pathPoints = new List<Vector3>();
        if (mapCharacter != null)
        {
            pathPoints = mapCharacter.CalculatePath(currentFocusNode, targetNode);
        }
        else
        {
            // 캐릭터가 없으면 그냥 목표만 추가
            pathPoints.Add(targetNode.transform.position);
        }

        isFocused = true;
        currentFocusNode = targetNode;

        Vector3 nodePos = targetNode.transform.position;
        targetSize = maxPossibleSize;

        targetPosition = new Vector3(nodePos.x, nodePos.y, -10f);
        targetPosition = ClampPosition(targetPosition, targetSize);

        // 캐릭터 이동 명령
        if (mapCharacter != null)
        {
            mapCharacter.MoveAlongPath(pathPoints, () =>
            {
                if (currentFocusNode == targetNode)
                {
                    targetNode.ShowReward(true);
                }
            });
        }

        // 위치 저장
        string sceneKey = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetFloat(sceneKey + "_LastX", nodePos.x);
        PlayerPrefs.SetFloat(sceneKey + "_LastY", nodePos.y);
        PlayerPrefs.Save();
    }

   /*
    // 두 노드 사이의 경로를 찾는 함수
    List<Vector3> GetPathPoints(StageNode startNode, StageNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        if (startNode == null || endNode == null)
        {
            if (endNode != null) path.Add(endNode.transform.position);
            return path;
        }
        // 시작점 부모들 찾기
        List<StageNode> startAncestors = new List<StageNode>();
        StageNode curr = startNode;
        // 무한 루프 방지용 카운트
        int safetyCount = 0;
        while (curr != null && safetyCount < 100)
        {
            startAncestors.Add(curr);
            curr = curr.parentStageNode;
            safetyCount++;
        }
        // 도착점 조상들을 찾으면서, 시작점 조상과 겹치는지 확인
        StageNode commonAncestor = null;
        List<StageNode> endAncestors = new List<StageNode>();
        curr = endNode;
        safetyCount = 0;

        while (curr != null && safetyCount < 100)
        {
            if (startAncestors.Contains(curr))
            {
                commonAncestor = curr;
                break;  // 공통 조상 발견
            }
            endAncestors.Add(curr);
            curr = curr.parentStageNode;
            safetyCount++;
        }
        // 공통 조상 없으면 직선 이동
        if (commonAncestor == null)
        {
            Debug.LogWarning($"공통 조상을 찾을 수 없어 직선으로 이동 합니다. {startNode.name}");
            path.Add(endNode.transform.position);
            return path;
        }
        // 경로 구성
        foreach (var node in startAncestors)
        {
            if (node == commonAncestor) break;
            if (node != startNode) path.Add(node.transform.position);
        }
        // 공통 조상 추가
        path.Add(commonAncestor.transform.position);
        // 공통 조상 -> 도착점
        for (int i = endAncestors.Count - 1; i >= 0; i--)
        {
            path.Add(endAncestors[i].transform.position);
        }

        return path;
    }
   */
    // 확대
    void ZoomToNode(StageNode node)
    {
        Bounds bounds = node.GetTotalBounds();
        Vector3 nodePos = node.transform.position;

        float distX = Mathf.Max(Mathf.Abs(bounds.max.x - nodePos.x), Mathf.Abs(bounds.min.x - nodePos.x));
        float distY = Mathf.Max(Mathf.Abs(bounds.max.y - nodePos.y), Mathf.Abs(bounds.min.y - nodePos.y));
        float screenRatio = (float)Screen.width / (float)Screen.height;

        float sizeFromWidth = distX / screenRatio;
        float calculatedSize = Mathf.Max(distY, sizeFromWidth);

        // if (node.overrideZoomSize > 0) calculatedSize = node.overrideZoomSize;
        // 줌 범위 제한
        calculatedSize = Mathf.Max(calculatedSize, minZoomLimit);
        calculatedSize = Mathf.Min(calculatedSize, maxPossibleSize);
        // 목표 사이즈 갱신
        targetSize = calculatedSize;
        // 위치 다시 조정
        targetPosition = new Vector3(nodePos.x, nodePos.y, -10f);
        targetPosition = ClampPosition(targetPosition, targetSize);
    }

    void Unfocus()
    {
        if (currentFocusNode != null) currentFocusNode.ShowInfo(false);
        isFocused = false;
        currentFocusNode = null;
        targetSize = maxPossibleSize;
        Vector3 returnPos = targetPosition;      
        targetPosition = ClampPosition(returnPos, targetSize);
    }

    void MoveCameraSmoothly()
    {
        if (isDragging) // 드래그 시 즉시 이동
        {
            cam.transform.position = targetPosition;
            moveVelocity = Vector3.zero;
        }
        else    // 아닐 시 부드러게 이동
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref moveVelocity, smoothTime);
        }
        // 확대, 축소는 항상 부드럽게 유지
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, smoothTime);
    }

    void CalculateMaxZoomSize()
    {
        float screenRatio = cam.aspect;
        float heightLimit = mapBounds.size.y / 2f;
        float widthLimit = (mapBounds.size.x / 2f) / screenRatio;
        maxPossibleSize = Mathf.Min(heightLimit, widthLimit);
    }

    Vector3 ClampPosition(Vector3 targetPos, float currentZoomSize)
    {
        float vertExtent = currentZoomSize;
        float horzExtent = vertExtent * cam.aspect;

        float minX = mapBounds.min.x + horzExtent;
        float maxX = mapBounds.max.x - horzExtent;
        float minY = mapBounds.min.y + vertExtent;
        float maxY = mapBounds.max.y - vertExtent;

        if (minX > maxX) minX = maxX = mapBounds.center.x;
        if (minY > maxY) minY = maxY = mapBounds.center.y;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        return targetPos;
    }
}
