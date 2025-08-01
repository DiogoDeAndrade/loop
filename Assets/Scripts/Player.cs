using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [HorizontalLine(color: EColor.Red)]
    [SerializeField] 
    PlayerInput         playerInput;
    [SerializeField, InputPlayer(nameof(playerInput))] 
    UC.InputControl     moveControl;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    UC.InputControl     inventoryControl;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    UC.InputControl[]   abilityControl;
    [SerializeField]
    float           rotateOnWalk = 0.0f;
    [SerializeField]
    Hypertag        cameraTag;

    Vector2             inputVector;
    Vector2             currentVelocity = Vector2.zero;
    Camera              mainCamera;
    InventoryDisplay    inventoryDisplay;

    class AbilityElem
    {
        public float     abilityTriggerStartTime;
        public Ability   ability;
    }
    AbilityElem[]       abilities;

    protected override void Start()
    {
        base.Start();

        rb.linearDamping = characterType.dragCoeff;

        moveControl.playerInput = playerInput;
        inventoryControl.playerInput = playerInput;

        mainCamera = HypertaggedObject.GetFirstOrDefault<Camera>(cameraTag);

        // Setup Inventory
        var inventory = GetComponent<Inventory>();
        if ((inventory) && (characterType.initialItems != null))
        {
            foreach (var item in characterType.initialItems)
            {
                inventory.Add(item.item, item.count);
            }
        }

        // Setup UI
        inventoryDisplay = FindAnyObjectByType<InventoryDisplay>();
        inventoryDisplay?.SetInventory(inventory);

        abilities = new AbilityElem[abilityControl.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            abilityControl[i].playerInput = playerInput;
            abilities[i] = new AbilityElem();
            abilities[i].abilityTriggerStartTime = float.MaxValue;
            abilities[i].ability = GetAbilityByIndex(i);
        }
    }


    private void Update()
    {
        inputVector = moveControl.GetAxis2().normalized;

        // Point to mouse
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir= (position.xy() - transform.position.xy());
        if (mouseDir.magnitude > 15.0f)
        {
            mouseDir.Normalize();
            headSpriteRenderer.UpdateHead(mouseDir);
        }

        // Trigger abilities
        for (int i = 0; i < abilities.Length; i++)
        {
            var ability = abilities[i].ability;
            if (ability == null) continue;
            if (!ability.CanTrigger())
            {
                // Inform of lack of ammo
                if (!ability.HasAmmo())
                {
                    switch (ability.triggerMode)
                    {
                        case Ability.TriggerMode.Single:
                            if (abilityControl[i].IsDown()) CombatTextManager.SpawnText(gameObject, "No bullets!", combatTextColorError, combatTextColorError.ChangeAlpha(0.0f));
                            break;
                        case Ability.TriggerMode.Charge:
                            break;
                        case Ability.TriggerMode.Continuous:
                            break;
                        default:
                            break;
                    }
                }
                continue;
            }

            switch (ability.triggerMode)
            {
                case Ability.TriggerMode.Single:
                    if (abilityControl[i].IsDown()) ability.Trigger(0.0f);
                    break;
                case Ability.TriggerMode.Charge:
                    if (abilityControl[i].IsDown()) abilities[i].abilityTriggerStartTime = Time.time;
                    else if (abilityControl[i].IsUp())
                    {
                        ability.Trigger(Time.time - abilities[i].abilityTriggerStartTime);
                    }
                    break;
                case Ability.TriggerMode.Continuous:
                    if (abilityControl[i].IsPressed()) ability.Trigger(0.0f);
                    break;
                default:
                    break;
            }
        }

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

    Ability GetAbilityByIndex(int index)
    {
        var abilities = GetComponentsInChildren<Ability>();
        foreach (var a in abilities)
        {
            if (a.abilityIndex == index) return a;
        }

        return null;
    }
}
