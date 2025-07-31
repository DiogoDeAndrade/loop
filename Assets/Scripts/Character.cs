using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] 
    protected float maxSpeed = 200.0f;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
