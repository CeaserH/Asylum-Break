using UnityEngine;

public class LeanSystem : MonoBehaviour
{
    public float leanAngle = 15f;
    public float leanDistance = 0.2f;
    public float leanSpeed = 8f;
    public KeyCode leanLeftKey = KeyCode.Q;
    public KeyCode leanRightKey = KeyCode.E;
    public LayerMask leanCollisionMask = ~0;
    public float collisionCheckRadius = 0.16f;
    public float collisionCheckOffset = 0.1f;
    public Transform viewmodelRoot;

    float currentLean = 0f;
    float targetLean = 0f;
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

        float desiredLean = targetLean;

        if (targetLean != 0f)
        {
            Vector3 checkDir = transform.right * (targetLean * (leanDistance + collisionCheckOffset));
            Vector3 checkOrigin = transform.position + transform.up * 0.5f;
            if (Physics.CheckSphere(checkOrigin + checkDir, collisionCheckRadius, leanCollisionMask))
            {
                desiredLean = 0f;
            }
        }

        currentLean = Mathf.Lerp(currentLean, desiredLean, Time.deltaTime * leanSpeed);

        Vector3 leanPos = initialLocalPosition + transform.right * (currentLean * leanDistance);
        Quaternion leanRot = initialLocalRotation * Quaternion.Euler(0, 0, -currentLean * leanAngle);

        transform.localPosition = leanPos;
        transform.localRotation = leanRot;

        if (viewmodelRoot != null)
        {
            Vector3 vmPos = Vector3.Lerp(viewmodelRoot.localPosition, leanPos, Time.deltaTime * leanSpeed * 1.5f);
            Quaternion vmRot = Quaternion.Slerp(viewmodelRoot.localRotation, leanRot, Time.deltaTime * leanSpeed * 1.5f);
            viewmodelRoot.localPosition = vmPos;
            viewmodelRoot.localRotation = vmRot;
        }
    }
}
