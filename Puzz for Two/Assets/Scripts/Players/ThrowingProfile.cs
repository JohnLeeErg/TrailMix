using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Throw
{
    public float yDistance, xDistance, timeToApex;
    [HideInInspector] public float gravity;
    [HideInInspector] public Vector2 startingVelocity;
    public Throw(float y, float x, float time)
    {
        yDistance = y;
        xDistance = x;
        timeToApex = time;
        gravity = ((2 * yDistance) / (timeToApex * timeToApex));
        startingVelocity.x = xDistance / (timeToApex * 2);
        startingVelocity.y = (yDistance + gravity * (timeToApex * timeToApex) / 2) / timeToApex;
    }
}

[CreateAssetMenu]
public class ThrowingProfile : ScriptableObject {

    public Throw diagonalUp;
    public Throw diagonalDown;
    public Throw upwards;
    public Throw downwards;
    public Throw horizontals;
}
