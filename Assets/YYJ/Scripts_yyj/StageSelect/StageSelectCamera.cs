using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectCamera : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapBackground;
    [SerializeField] private float dragSpeed = 1f;

    private Camera cam;
    private Vector3 dragOrigin;

    private float minX, maxX;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        CalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        HandleHorizontalDrag();
    }

    void CalculateBounds()
    {
        if (mapBackground == null)
        {
            Debug.LogError("배경 오브젝트가 없습니다.");
            return;
        }
        // 카메라 가로 절반
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;
        // 배경 범위 가져오기
        Bounds bgBounds = mapBackground.bounds;
        // 카메라 최대, 최소 이동 범위
        minX = bgBounds.min.x + horzExtent;
        maxX = bgBounds.max.x - horzExtent;

        if (minX > maxX)
        {
            minX = maxX = bgBounds.center.x;
        }
    }

    void HandleHorizontalDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            float difference = dragOrigin.x - currentPos.x;

            Vector3 targetPos = cam.transform.position;
            targetPos.x += difference * dragSpeed;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

            cam.transform.position = targetPos;
        }
    }
}
