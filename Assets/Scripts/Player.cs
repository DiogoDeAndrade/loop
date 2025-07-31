using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [HorizontalLine(color: EColor.Red)]
    [SerializeField] 
    PlayerInput     playerInput;
    [SerializeField, InputPlayer(nameof(playerInput))] 
    UC.InputControl moveControl;
    [SerializeField]
    float           rotateOnWalk = 0.0f;

    Vector2         inputVector;
    Vector2         currentVelocity = Vector2.zero;

    protected override void Start()
    {
        base.Start();

        rb.linearDamping = characterType.dragCoeff;

        moveControl.playerInput = playerInput;
    }


    private void Update()
    {
        inputVector = moveControl.GetAxis2().normalized;
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void MoveCharacter()
    {
        inputVector = moveControl.GetAxis2().normalized;

        currentVelocity = rb.linearVelocity;

        currentVelocity += characterType.acceleration * inputVector * Time.fixedDeltaTime;
        
        float currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > characterType.maxSpeed)
            currentVelocity = currentVelocity.normalized * characterType.maxSpeed;

        rb.linearVelocity = currentVelocity;

        if (rotateOnWalk > 0.0f)
        {
            if (Mathf.Abs(inputVector.x) > 0.1f)
            {
                bodySpriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rotateOnWalk * inputVector.x);
            }
            else
            {
                bodySpriteRenderer.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
