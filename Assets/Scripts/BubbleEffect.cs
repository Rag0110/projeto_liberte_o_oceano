using UnityEngine;

public class BubbleEffect : MonoBehaviour
{
    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * 2f) * 0.02f;
        transform.localScale = new Vector3(scale, scale, 1);
    }
}