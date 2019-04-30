using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[CreateAssetMenu]
public class InputProfile : ScriptableObject {

    // Use this for initialization
    [Header("XBox Controller Controls")]
    public InputControlType throwButton;
    public InputControlType jumpButton, catchButton, lockMovementButton, resetButton, swapButton, upButton,downButton,leftButton,rightButton,confirmButton,altUpButton,altDownButton,altLeftButton,altRightButton,lockThrowingButton;

    [Header("Keyboard Controls")]
    public Key throwKey;
    public Key jumpKey, catchKey, lockMovementKey, resetKey, swapKey, upKey, downKey, leftKey, rightKey, confirmKey, altUpKey, altDownKey, altLeftKey, altRightKey,lockThrowingKey;

    [Header("2 Player Keyboard Controls Player 1")]
    public Key p1ThrowKey;
    public Key p1JumpKey, p1CatchKey, p1LockMovementKey, p1ResetKey, p1SwapKey, p1UpKey, p1DownKey, p1LeftKey, p1RightKey, p1ConfirmKey, p1AltUpKey, p1AltDownKey, p1AltLeftKey, p1AltRightKey, p1LockThrowingKey;

    [Header("2 Player Keyboard Controls Player 2")]
    public Key p2ThrowKey;
    public Key p2JumpKey, p2CatchKey, p2LockMovementKey, p2ResetKey, p2SwapKey, p2UpKey, p2DownKey, p2LeftKey, p2RightKey, p2ConfirmKey, p2AltUpKey, p2AltDownKey, p2AltLeftKey, p2AltRightKey, p2LockThrowingKey;
}
