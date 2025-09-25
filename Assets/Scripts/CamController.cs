using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private Camera cam;
    private Vector3 new_pos;
    private float new_zoom;
    private Vector3 startPosition;
    private bool firstTouch = true;

    private static float SCROLL_ZOOM_SPEED = 2f;
    private static float DRAG_SPEED = 5f;
    private static float CAM_ZOOM_OUT = 7.2f;
    private static float CAM_ZOOM_IN = 4.5f;
    private Color BACKGROUND = new Color(182.0f / 255.0f, 220.0f / 255.0f, 244.0f / 255.0f);

    private void Start()
    {
        cam = GetComponent<Camera>();
        new_pos = new Vector3(0f, 0f, -1f);
        new_zoom = CAM_ZOOM_OUT;
        startPosition = Vector3.zero;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.position != new_pos)
        {
            transform.position = Vector3.Lerp(transform.position, new_pos, 2f * Time.deltaTime);
        }

        if (cam.orthographicSize != new_zoom)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, new_zoom, 2f * Time.deltaTime);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        new_pos = pos;
    }
    public void ZoomOut()
    {
        new_zoom = CAM_ZOOM_OUT;
    }
    public void ZoomIn()
    {
        if (cam.orthographicSize > CAM_ZOOM_IN)
        {
            new_zoom = CAM_ZOOM_IN;
        }
    }

    public void SetBackground(Color c)
    {
        cam.backgroundColor = c;
    }

    public void MoveTo(Vector3 pos)
    {
        new_pos = pos;
    }

    public bool isReady()
    {
        return Mathf.Abs(transform.position.x - new_pos.x) < 0.1f && Mathf.Abs(transform.position.y - new_pos.y) < 0.1f;
    }

    public void ResetCam()
    {
        //Reset camera
        MoveTo(new Vector3(0f, 0f, -1f));
        ZoomOut();
        SetBackground(BACKGROUND);
    }
    public void ZoomWithMouse()
    {
        //Zoom with mouse wheel
        float deltaZoom = Input.GetAxis("Mouse ScrollWheel") * SCROLL_ZOOM_SPEED;
        if (deltaZoom != 0f)
        {
            cam.orthographicSize -= deltaZoom;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1.0f, 7.5f);
            new_zoom = cam.orthographicSize;
        }
    }

    public void DragCam()
    {
        if (Input.touchCount > 0 && firstTouch)
        {
            startPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            firstTouch = false;
        }
        else if(Input.touchCount == 1)
        {
            float speed = DRAG_SPEED * Time.deltaTime;
            Vector3 direction = startPosition - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Camera.main.transform.position += direction * speed;
            new_pos = cam.transform.position;
        }
        else
        {
            firstTouch = true;
        }
        
    }
}
