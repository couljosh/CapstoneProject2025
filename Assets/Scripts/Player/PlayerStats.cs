using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    //-------------------------------------------------------------------------------------------------------
    [Header("----| Move Stats |--------------------")]
    [Range(1,10)]
    [Tooltip("The players movement speed")]
    public float initialMoveSpeed;
    [Range(1, 10)]
    [Tooltip("The players rotation speed")]
    public float rotateSpeed;
    [Range(0, 20)]
    [Tooltip("The strength of gravity appied to the player")]
    public float gravity;
    [Range(0, 1)]
    [Tooltip("The delay time a player has before falling in a pit")]
    public float coyoteTimeThreshold;







    [Space]
    [Space]
    //-------------------------------------------------------------------------------------------------------
    [Header("----| Kick Stats |--------------------")]
    
    [Range(0, 5)]
    [Tooltip("Distance a player can kick an object from")]
    public float kickDectDist;

    [Range(0, 100000)]
    [Tooltip("Multiplied to the kick force to account for the mass of minecarts")]
    public float initialKickStrength;

    [Space]

    [Range(1, 5)]
    [Tooltip("Multiplied to the kick force to account for the mass of minecarts")]
    public float cartForceMultiplier;
    [Range(1, 5)]
    [Tooltip("Multiplied to the kick force to account for the mass of moveable rocks")]
    public float rockForceMultiplier;
    [Range(0, 1)]
    [Tooltip("Multiplied to the kick force to account for the mass of the player / ideal kick distance")]
    public float playerForceMultiplier;
    [Range(1, 5)]

    [Space]

    [Tooltip("Multiplied to the kick force to set and limit the maximum kick force")]
    public float maximumKickMultiplier;
    [Range(0, 10)]
    [Tooltip("The time a kick needs to be charged to become fully charged")]
    public float timeToMaxStrength;
    [Range(0, 1)]
    [Tooltip("The time a kick needs to be charged for the player to slow down")]
    public float timeBeforePlayerSlowWhenCharge;
    [Range(0, 1)]
    [Tooltip("Sets the speed a played is slowed when their kick is fully charged")]
    public float maxPlayerChargeSlowdown;






    [Space]
    [Space]
    //-------------------------------------------------------------------------------------------------------
    [Header("----| Bomb Stats |--------------------")]
    [Range(1, 3)]
    [Tooltip("Maximum amount of bombs a place can hold at once")]
    public int maxBombsHeld;

    [Space]

    [Range(0, 5)]
    [Tooltip("How quickly bombs regenerate to the players inventory")]
    public float bombRegenTime;
    [Range(0, 1)]
    [Tooltip("The time between when a bomb is placed and when another can be placed")]
    public float bombUseCooldown;





    [Space]
    [Space]
    //-------------------------------------------------------------------------------------------------------
    [Header("----| Death Stats |--------------------")]
    [Range(0, 10)]
    [Tooltip("How long a player is invicible after death")]
    public float invincibleDuration; 
    [Range(0, 10)]
    [Tooltip("The time until a killed player respawns")]
    public float respawnDelay;

    [Space]

    [Range(0, 5000)]
    [Tooltip("The force applied to each drop gem to make them scatter")]
    public float scatterForce;
    [Range(0, 1)]
    [Tooltip("A delay to ensure the gems drop after the player dies **Without it gems dont drop on death**")]
    public float gemDropDelay;
}
