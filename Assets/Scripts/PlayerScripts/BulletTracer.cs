using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float speed = 400f;
    private Vector3 target;
    private bool active = false;

    public void Init(Vector3 from, Vector3 to)
    {
        transform.position = from;
        target = to;
        active = true;
        Destroy(gameObject, 0.05f);
    }

    void Update()
    {
        if (!active) return;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }
}