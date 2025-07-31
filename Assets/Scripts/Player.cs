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
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    UC.InputControl inventoryControl;
    [SerializeField]
    float           rotateOnWalk = 0.0f;
    [SerializeField]
    Hypertag        cameraTag;

    Vector2             inputVector;
    Vector2             currentVelocity = Vector2.zero;
    Camera              mainCamera;
    InventoryDisplay    inventoryDisplay;

    protected override void Start()
    {
        base.Start();

        rb.linearDamping = characterType.dragCoeff;

        moveControl.playerInput = playerInput;
        inventoryControl.playerInput = playerInput;

        mainCamera = HypertaggedObject.GetFirstOrDefault<Camera>(cameraTag);

        // Setup UI
        var inventory = GetComponent<Inventory>();
        inventoryDisplay = FindAnyObjectByType<InventoryDisplay>();
        inventoryDisplay?.SetInventory(inventory);
    }


    private void Update()
    {
        inputVector = moveControl.GetAxis2().normalized;

        // Point to mouse
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir= (position.xy() - transform.position.xy()).normalized;
        headSpriteRenderer.UpdateHead(mouseDir);

        // Inventory
        if (inventoryControl.IsDown())
        {
            inventoryDisplay?.Toggle();
        }
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
