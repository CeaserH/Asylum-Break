using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float sensitivity = 300f;
    [SerializeField] private float minVertical = -80f;
    [SerializeField] private float maxVertical = 80f;
    [SerializeField] private bool invertY = false;

    private float verticalRotation = 0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);

        // Handle vertical camera tilt
        verticalRotation += (invertY ? mouseY : -mouseY);
        verticalRotation = Mathf.Clamp(verticalRotation, minVertical, maxVertical);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
