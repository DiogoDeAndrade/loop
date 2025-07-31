using UC;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField]
    Hypertag cameraTag;

    Camera mainCamera;

    void Start()
    {
        mainCamera = HypertaggedObject.GetFirstOrDefault<Camera>(cameraTag);

        Cursor.visible = false;
    }

    
    void Update()
    {
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition).ChangeZ(transform.position.z);
    }
}
