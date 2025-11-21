using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // 카메라 확대 / 축소
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minSize = 3f;

    private float maxSize;

    // 카메라 이동 속도
    [SerializeField] private float cameraSpeed = 1f;
   
    private Camera cam;
    private Vector3 dragOrigin; // 드래그 시작 위치

    // 카메라 이동 가능 범위
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;   

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        // 시작 시 카메라 크기를 최대 크기로 지정
        maxSize = cam.orthographicSize;
        // 카메라 크기를 카메라 이동 경계 계산
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;
        // 현재 카메라 위치 기준 경계 계산
        mapMinX = cam.transform.position.x - horzExtent;
        mapMaxX = cam.transform.position.x + horzExtent;
        mapMinY = cam.transform.position.y - vertExtent;
        mapMaxY = cam.transform.position.y + vertExtent;
    }

    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        HandleCameraMove();
        ClampCameraPosition();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float targetSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(targetSize, minSize, maxSize);
        }
    }

    void HandleCameraMove()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPos;

            cam.transform.position += difference * cameraSpeed;
        }
    }

    void ClampCameraPosition()
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minX = mapMinX + horzExtent;
        float maxX = mapMaxX - horzExtent;
        float minY = mapMinY + vertExtent;
        float maxY = mapMaxY - vertExtent;

        Vector3 pos = cam.transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = -10f;

        cam.transform.position = pos;
    }
}
