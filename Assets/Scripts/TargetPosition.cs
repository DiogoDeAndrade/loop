using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = Vector3.zero;

    public Vector3 worldPos => transform.position + offset;

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + offset;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pos + new Vector3(-10.0f, -10.0f, 0.0f), pos + new Vector3(10.0f, 10.0f, 0.0f));
        Gizmos.DrawLine(pos + new Vector3(-10.0f, 10.0f, 0.0f), pos + new Vector3(10.0f, -10.0f, 0.0f));
    }
}

public static class TargetPositionExtension
{
    public static Vector3 GetTargetPosition(this Component c)
    {
        var tp = c.GetComponent<TargetPosition>();
        if (tp) return tp.worldPos;

        return c.transform.position;
    }

    public static Vector3 GetTargetPosition(this GameObject o)
    {
        var tp = o.GetComponent<TargetPosition>();
        if (tp) return tp.worldPos;

        return o.transform.position;
    }
}
