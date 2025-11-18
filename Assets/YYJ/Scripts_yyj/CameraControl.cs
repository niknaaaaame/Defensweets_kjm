using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minSize = 3f;
    [SerializeField] private float maxSize = 10f;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            float targetSize = cam.orthographicSize - scroll * zoomSpeed;

            cam.orthographicSize = Mathf.Clamp(targetSize, minSize, maxSize);
        }
    }
}
