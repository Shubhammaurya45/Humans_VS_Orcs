using UnityEngine;

public class CameraController
{
    private float cameraSpeed;
    private float mobileCameraSpeed;
    public bool lockCamera;

    public CameraController(float cameraSpeed, float mobileCameraSpeed)
    {
        this.cameraSpeed = cameraSpeed;
        this.mobileCameraSpeed = mobileCameraSpeed;
    }

    public void Update()
    {
        if (lockCamera)
            return;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector2 normalizedData = touchDeltaPosition / new Vector2(Screen.width, Screen.height);
            Camera.main.transform.Translate(
                -normalizedData.x * mobileCameraSpeed,
                -normalizedData.y * mobileCameraSpeed,
                0
            );
        }
        else if (Input.touchCount == 0 && Input.GetMouseButton(0))
        {
            Vector2 mouseDeltaPosition = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );
            Camera.main.transform.Translate(
                -mouseDeltaPosition.x * Time.deltaTime * cameraSpeed,
                -mouseDeltaPosition.y * Time.deltaTime * cameraSpeed,
                0
            );
        }
    }
}
