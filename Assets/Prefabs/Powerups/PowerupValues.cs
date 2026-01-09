using UnityEngine;

[CreateAssetMenu(fileName = "PowerupValues", menuName = "Scriptable Objects/PowerupValues")]
public class PowerupValues : ScriptableObject
{
    //-------------------------------------------------------------------------------------------------------
    [Header("----| Move Stats |--------------------")]
    //[Range(1, 10)]
    [Tooltip("The drill movement speed")]
    public float initialMoveSpeed;

    //[Range(1, 10)]
    [Tooltip("The drill rotation speed")]
    public float rotateSpeed;

    //[Range(1, 10)]
    [Tooltip("The drill rotation speed")]
    public float acceleration;

    //[Range(1, 10)]
    [Tooltip("The drill rotation speed")]
    public float startDelay;

    //-------------------------------------------------------------------------------------------------------

    [Header("----| Explosion Stats |--------------------")]
    //[Range(1, 10)]
    [Tooltip("The drill movement speed")]
    public float explodeRad;

    //-------------------------------------------------------------------------------------------------------
}

