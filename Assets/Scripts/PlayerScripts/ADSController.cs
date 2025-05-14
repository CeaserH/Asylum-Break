using UnityEngine;


[RequireComponent(typeof(Camera))]
public class AimDownSights : MonoBehaviour
{
    [Header("FOV (field‑of‑view) settings")]
    [SerializeField] float hipFov = 60f;
    [SerializeField] float adsFov = 35f;
    [SerializeField] float zoomSpeed = 8f;

    [Header("Controls")]
    [SerializeField] KeyCode aimButton = KeyCode.Mouse1;

    Camera cam;
    float targetFov;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = hipFov;
        targetFov = hipFov;
    }

    void Update()
    {
        // decide which FOV we want this frame
        targetFov = Input.GetKey(aimButton) ? adsFov : hipFov;

        // smooth transition
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,
                                     targetFov,
                                     Time.deltaTime * zoomSpeed);
    }
}
