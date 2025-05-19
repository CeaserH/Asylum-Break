using UnityEngine;

public class BreathingSystem : MonoBehaviour
{
    public float breathSpeed;
    public float breathAmount;
    private Vector3 initialLocalPosition;
    private float timer;


    void Start()
    {
        initialLocalPosition = transform.localPosition;
    }

    public void HandleBreathing()
    {
        timer += Time.deltaTime * breathSpeed;
        float newY = initialLocalPosition.y + Mathf.Sin(timer) * breathAmount;
        Vector3 localPos = transform.localPosition;
        localPos.y = newY;
        transform.localPosition = localPos;
    }
}
