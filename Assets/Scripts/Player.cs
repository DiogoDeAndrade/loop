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
    }
}
