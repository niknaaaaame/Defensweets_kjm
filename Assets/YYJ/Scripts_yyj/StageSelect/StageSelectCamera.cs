using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectCamera : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapBackground;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float smoothTime = 0.2f;
    
    private Camera cam;
    // 카메라 드래그
    private Vector3 dragOrigin;
    private bool isDragging = false;
    // 줌
    private bool isFocused = false;
    private StageNode currentFocusNode;
    private Vector3 originalPosition;
    
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
        float startSize = Mathf.Clamp(cam.orthographicSize, 2f, maxPossibleSize);
        cam.orthographicSize = startSize;
        targetSize = startSize;
        // 초기 위치
        originalPosition = targetPosition = ClampPosition(cam.transform.position, startSize);
        cam.transform.position = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
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
            StageNode node = hit.collider.GetComponent<StageNode>();
            if(node != null)
            {
                FocusOnNode(node);
                return;
            }
        }
        // 확대 상태 + 바깥 클릭 시
        if (isFocused) Unfocus();
    }

    void FocusOnNode(StageNode node)
    {
        if (currentFocusNode != null) currentFocusNode.ShowInfo(false);

        isFocused = true;
        currentFocusNode = node;
        node.ShowInfo(true);

        Bounds bounds = node.GetTotalBounds();
        Vector3 nodePos = node.transform.position;
        // 노드에서 상하좌우 먼 경계까지의 거리 계산
        float distX = Mathf.Max(Mathf.Abs(bounds.max.x - nodePos.x), Mathf.Abs(bounds.min.x - nodePos.x));
        float distY = Mathf.Max(Mathf.Abs(bounds.max.y - nodePos.y), Mathf.Abs(bounds.min.y - nodePos.y));
        float screenRatio = (float)Screen.width / (float)Screen.height;
        // 가로 거리 세로 비율로 전환
        float sizeFromWidth = distX / screenRatio;
        targetSize = Mathf.Max(distY, sizeFromWidth);
        // 최소 크기 제한
        targetSize = Mathf.Max(targetSize, 0f);
        // 최대 크기 제한
        targetSize = Mathf.Min(targetSize, maxPossibleSize);
        // 목표 위치 설정
        targetPosition = new Vector3(nodePos.x, nodePos.y, -10f);
        // 목표 위치가 배경 밖일 시 제한
        targetPosition = ClampPosition(targetPosition, targetSize);
    }

    void Unfocus()
    {
        if (currentFocusNode != null)
        {
            currentFocusNode.ShowInfo(false);
            currentFocusNode = null;
        }
        isFocused = false;
        targetSize = maxPossibleSize;
        Vector3 returnPos = targetPosition;
        returnPos.y = originalPosition.y;
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
