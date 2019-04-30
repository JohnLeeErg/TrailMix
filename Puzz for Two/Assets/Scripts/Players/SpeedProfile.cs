using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpeedProfile : ScriptableObject
{
    public float speedXGrounded = 2f,speedXAerial, jumpHeight, riseTime,hangTime,fallAccel,maxFallSpeed, shortHopDeAccel, maxHorizontalJump, xJumpSlowingPoint,bonkAssist, walkOffLedgeJumpWindow;
    public bool shortHops, castlevaniaJumps, pushing,confinedSpaceAssist,cantJumpWithRoof, jumpHorizontalLimit, jumpAfterWalkOffLedge;
}

