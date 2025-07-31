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
    float           acceleration = 200.0f;
    [SerializeField] 
    float           dragCoeff = 5.0f;

    Vector2         inputVector;
    Vector2         currentVelocity = Vector2.zero;

    protected override void Start()
    {
        base.Start();

        rb.linearDamping = dragCoeff;

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

        currentVelocity += acceleration * inputVector * Time.fixedDeltaTime;
        
        float currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > maxSpeed)
            currentVelocity = currentVelocity.normalized * maxSpeed;

        rb.linearVelocity = currentVelocity;
    }
}
