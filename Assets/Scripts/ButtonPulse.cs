using UnityEngine;

public class ButtonPulse : MonoBehaviour
{
    public float speed = 3f;
    public float amount = 0.1f;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * speed) * amount;

        transform.localScale = baseScale * pulse;
    }
}