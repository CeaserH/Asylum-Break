using UnityEngine;

public class LeanSystem : MonoBehaviour
{
    public float leanAngle = 15f;
    public float leanDistance = 0.3f;
    public float leanSpeed = 8f;
    public KeyCode leanLeftKey = KeyCode.Q;
    public KeyCode leanRightKey = KeyCode.E;

    float targetLean = 0f;
    float currentLean = 0f;

    Vector3 initialLocalPosition;
    Quaternion initialLocalRotation;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetKey(leanLeftKey))
            targetLean = -1f;
        else if (Input.GetKey(leanRightKey))
            targetLean = 1f;
        else
            targetLean = 0f;

        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

        Vector3 leanOffset = initialLocalPosition + transform.right * (currentLean * leanDistance);
        Quaternion leanRot = initialLocalRotation * Quaternion.Euler(0, 0, -currentLean * leanAngle);

        transform.localPosition = leanOffset;
        transform.localRotation = leanRot;
    }
}
