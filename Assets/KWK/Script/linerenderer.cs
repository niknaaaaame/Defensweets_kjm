using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linerenderer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public float range = 5f;

    // Start is called before the first frame update
    void Start()
    {
        range /= 2;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawrange();
        }
    }

    void drawrange()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 4; // 사각형을 닫기 위해 5개의 점 필요
        lineRenderer.useWorldSpace = false; // 타워 이동 시 함께 움직이도록 로컬 좌표 사용
        lineRenderer.loop = true; // 자동으로 시작점과 끝점을 연결
        lineRenderer.widthMultiplier = 0.1f; // 선의 두께 설정sha

        // 사각형의 네 모서리 좌표 설정 (Y축은 바닥에 붙도록 0으로 설정)
        Vector3[] points = new Vector3[4];
        points[0] = new Vector3(-range, range, 0.1f); // 왼쪽 위
        points[1] = new Vector3(range, range, 0.1f); // 오른쪽 위
        points[2] = new Vector3(range, -range, 0.1f); // 오른쪽 아래
        points[3] = new Vector3(-range, -range, 0.1f); // 왼쪽 아래
        //points[4] = new Vector3(-range, 0.1f, range); // 다시 왼쪽 위 (닫기)

        lineRenderer.SetPositions(points);
    }
}
