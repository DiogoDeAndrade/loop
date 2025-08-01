using NaughtyAttributes;
using System;
using UC;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    private enum TargetType { Mouse, Object, Explicit };

    [SerializeField]
    private TargetType type;
    [SerializeField, ShowIf(nameof(isTargetMouse))]
    Hypertag cameraTag;
    [SerializeField, ShowIf(nameof(isTargetObject))]
    Hypertag objectTag;
    [SerializeField]
    private float maxRotationSpeed = 90.0f;
    [SerializeField]
    private bool flipOnMinusX = true;

    Camera          mainCamera;
    Transform       _target;
    float           _rotationDistance = float.MaxValue;

    bool isTargetMouse => type == TargetType.Mouse;
    bool isTargetObject => type == TargetType.Object;
    bool isTargetExplicit => type == TargetType.Explicit;

    public Transform target
    {
        get => _target;
        set { _target = value; }
    }

    void Start()
    {
        mainCamera = HypertaggedObject.GetFirstOrDefault<Camera>(cameraTag);
    }

    void Update()
    {
        _rotationDistance = float.MaxValue;

        if (type == TargetType.Mouse)
        {
            // Point to mouse
            Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RotateTo(position);
        }
        else if (type == TargetType.Object) 
        {
            var targetObject = HypertaggedObject.GetFirstOrDefault<Transform>(objectTag);
            if (targetObject != null)
            {
                // Check radius and LOS (?)
                RotateTo(targetObject.GetTargetPosition());
            }
        }
        else if (type == TargetType.Explicit)
        {
            if (target)
            {
                RotateTo(target.GetTargetPosition());
            }
        }
    }

    void RotateTo(Vector3 position)
    {
        Vector2 dir = (position.xy() - transform.position.xy()).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, dir.Perpendicular());

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
        _rotationDistance = Quaternion.Angle(targetRotation, transform.rotation);

        if (flipOnMinusX)
        {
            transform.localScale = new Vector3(1.0f, (dir.x < 0.0f) ? (-1.0f) : (1.0f), 1.0f);
        }
    }

    public float rotationDistance => _rotationDistance;
}
