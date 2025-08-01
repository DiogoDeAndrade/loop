using System;
using System.Collections;
using UC;
using Unity.VisualScripting;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    Sprite              downHead;
    Sprite              rightHead;
    bool                rightFlip;
    Sprite              leftHead;
    bool                leftFlip;
    Sprite              upHead;
    SpriteRenderer      spriteRenderer;
    PolygonCollider2D   polygonCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    public void SetHeads(Sprite downHead,
                         Sprite rightHead, bool rightFlip,
                         Sprite leftHead, bool leftFlip,
                         Sprite upHead)
    {
        this.downHead = downHead;
        this.rightHead = rightHead;
        this.rightFlip = rightFlip;
        this.leftHead = leftHead;
        this.leftFlip = leftFlip;
        this.upHead = upHead;

        UpdateHead(Vector2.down);
    }

    public void UpdateHead(Vector2 dir)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (dir.x < -0.4f)
        {
            spriteRenderer.sprite = leftHead;
            spriteRenderer.flipX = leftFlip;
        }
        else if (dir.x > 0.4f)
        {
            spriteRenderer.sprite = rightHead;
            spriteRenderer.flipX = rightFlip;
        }
        else
        {
            if (dir.y < -0.5f)
            {
                spriteRenderer.sprite = downHead;
                spriteRenderer.flipX = false;
            }
            else if (dir.y > 0.5f)
            {
                spriteRenderer.sprite = upHead;
                spriteRenderer.flipX = false;
            }
        }
    }

    public void PopOff(Vector3 dir)
    {
        // Remember current Y position as "ground" height
        float groundY = transform.position.y;
        Character character = GetComponentInParent<Character>();
        if (character) groundY = character.transform.position.y;

        // Create ground plane 
        var groundPlaneObj = new GameObject();
        groundPlaneObj.layer = LayerMask.NameToLayer("SpecialTemporaryGround");
        groundPlaneObj.transform.position = character.transform.position;
        var groundPlaneCollider = groundPlaneObj.AddComponent<BoxCollider2D>();
        groundPlaneCollider.offset = new Vector2(0.0f, -2.5f);
        groundPlaneCollider.size = new Vector2(400.0f, 5.0f);

        // Detach from parent
        transform.SetParent(null);

        // Ensure the object has a Rigidbody2D
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 4.0f; // Enable gravity
        rb.angularDamping = 0.4f;
        rb.linearDamping = 0.3f;
        rb.includeLayers = LayerMask.GetMask("SpecialTemporaryGround");

        // Refresh collider
        if (polygonCollider) polygonCollider.SetSprite(spriteRenderer.sprite);

        // Apply a pop force (tweak values as needed)
        rb.AddForce(new Vector2(dir.x, Mathf.Abs(dir.y)) * 40f, ForceMode2D.Impulse);

        float spin = UnityEngine.Random.Range(-400f, 400f); // random spin direction and speed
        rb.AddTorque(spin, ForceMode2D.Impulse);

        // Start coroutine to stop the fall at ground level
        StartCoroutine(BounceAndFade(rb, groundPlaneObj));
    }

    private IEnumerator BounceAndFade(Rigidbody2D rb, GameObject groundPlaneObj)
    {
        const float minLinearVelocity = 0.5f;
        const float minAngularVelocity = 1.0f;
        const float stillTimeThreshold = 0.5f;
        const float fadeDelay = 1.0f;

        float stillTime = 0f;

        while (true)
        {
            float linear = rb.linearVelocity.magnitude;
            float angular = Mathf.Abs(rb.angularVelocity);

            if (linear < minLinearVelocity && angular < minAngularVelocity)
            {
                stillTime += Time.fixedDeltaTime;

                if (stillTime >= stillTimeThreshold)
                {
                    Debug.Log("Object is still. Triggering fade.");

                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.gravityScale = 0;
                    rb.simulated = false;

                    yield return new WaitForSeconds(1.0f);

                    spriteRenderer.FadeTo(spriteRenderer.color.ChangeAlpha(0.0f), fadeDelay);

                    yield return new WaitForSeconds(fadeDelay * 2.0f);
                    Destroy(gameObject);
                    Destroy(groundPlaneObj);
                    yield break;
                }
            }
            else
            {
                stillTime = 0f; // reset if movement resumes
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
