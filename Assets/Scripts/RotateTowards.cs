using NaughtyAttributes;
using System;
using UC;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    private enum TargetType { Mouse, Object };

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

    bool isTargetMouse => type == TargetType.Mouse;
    bool isTargetObject => type == TargetType.Object;

    void Start()
    {
        mainCamera = HypertaggedObject.GetFirstOrDefault<Camera>(cameraTag);
    }

    void Update()
    {
        if (type == TargetType.Mouse)
        {
            // Point to mouse
            Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RotateTo(position);
        }
        else
        {
            var targetObject = HypertaggedObject.GetFirstOrDefault<Transform>(objectTag);
            if (targetObject != null)
            {
                // Check radius and LOS (?)
                RotateTo(targetObject.position);
            }
        }
    }

    void RotateTo(Vector3 position)
    {
        Vector2 dir = (position.xy() - transform.position.xy()).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, dir.Perpendicular());

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);

        if (flipOnMinusX)
        {
            transform.localScale = new Vector3(1.0f, (dir.x < 0.0f) ? (-1.0f) : (1.0f), 1.0f);
        }
    }
}
