using UnityEngine;

public class HeadRotate : MonoBehaviour
{
    Sprite  downHead;
    Sprite  rightHead;
    bool    rightFlip;
    Sprite  leftHead;
    bool    leftFlip;
    Sprite  upHead;
    SpriteRenderer  spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }

    public void UpdateHead(Vector2 dir)
    {
        if (dir.x < -0.1f)
        {
            spriteRenderer.sprite = leftHead;
            spriteRenderer.flipX = leftFlip;
        }
        else if (dir.x > 0.1f)
        {
            spriteRenderer.sprite = rightHead;
            spriteRenderer.flipX = rightFlip;
        }
        else
        {
            if (dir.y < -0.2f)
            {
                spriteRenderer.sprite = downHead;
                spriteRenderer.flipX = false;
            }
            else if (dir.y > 0.2f)
            {
                spriteRenderer.sprite = upHead;
                spriteRenderer.flipX = false;
            }
        }
    }
}
