using NaughtyAttributes;
using UC;
using UnityEngine;

[CreateAssetMenu(fileName = "Globals", menuName = "Loop/Globals")]
public class Globals : GlobalsBase
{
    [HorizontalLine(color: EColor.Red)]
    [SerializeField]
    private LayerMask   _damageLayers;
    [SerializeField, Layer]
    private int         _headBounceRotationLayer;

    public static int headBounceRotationLayer => instance?._headBounceRotationLayer ?? 0;
    public static LayerMask damageLayers => instance?._damageLayers ?? 0;

    public static Globals instance
    {
        get
        {
            return (instanceBase as Globals);
        }
    }
}
