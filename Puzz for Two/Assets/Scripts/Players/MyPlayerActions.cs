using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
[System.Serializable]
public class MyCharacterActions : PlayerActionSet
{
    public enum actionSetType
    {
        controller,
        keyboard,
        P2KeyboardP1,
        P2KeyboardP2,
        both
    }
    public PlayerAction throwAction, catchAction, jumpAction, lockMovementAction, resetAction, switchAction, up, down, left, right, altUp, altLeft, altRight, altDown, confirmAction, lockThrowAction;
    public PlayerOneAxisAction horizontalAxis, verticalAxis, altHorizontalAxis, altVerticalAxis;
    public actionSetType currentControllerType;

    public MyCharacterActions()
    {
        throwAction = CreatePlayerAction("Throw");
        catchAction = CreatePlayerAction("Catch");
        jumpAction = CreatePlayerAction("Jump");
        lockMovementAction = CreatePlayerAction("LockMovement");
        resetAction = CreatePlayerAction("Reset");
        switchAction = CreatePlayerAction("Switch");
        left = CreatePlayerAction("Left");
        right = CreatePlayerAction("Right");
        up = CreatePlayerAction("Up");
        down = CreatePlayerAction("Down");
        altLeft = CreatePlayerAction("AltLeft");
        altRight = CreatePlayerAction("AltRight");
        altUp = CreatePlayerAction("AltUp");
        altDown = CreatePlayerAction("AltDown");
        horizontalAxis = CreateOneAxisPlayerAction(left, right);
        verticalAxis = CreateOneAxisPlayerAction(down, up);
        altHorizontalAxis = CreateOneAxisPlayerAction(altLeft, altRight);
        altVerticalAxis = CreateOneAxisPlayerAction(altDown, altUp);
        confirmAction = CreatePlayerAction("Confirm");
        lockThrowAction = CreatePlayerAction("LockThrow");
    }
    public void SetupDefaults(InputProfile sourceOfInputs, actionSetType type)
    {
        currentControllerType = type;
        Reset();
        if (type == actionSetType.controller || type == actionSetType.both)
        {
            throwAction.AddDefaultBinding(sourceOfInputs.throwButton);
            catchAction.AddDefaultBinding(sourceOfInputs.catchButton);
            jumpAction.AddDefaultBinding(sourceOfInputs.jumpButton);
            lockMovementAction.AddDefaultBinding(sourceOfInputs.lockMovementButton);
            resetAction.AddDefaultBinding(sourceOfInputs.resetButton);
            resetAction.AddDefaultBinding(InputControlType.Options);
            switchAction.AddDefaultBinding(sourceOfInputs.swapButton);
            up.AddDefaultBinding(sourceOfInputs.upButton);
            down.AddDefaultBinding(sourceOfInputs.downButton);
            left.AddDefaultBinding(sourceOfInputs.leftButton);
            right.AddDefaultBinding(sourceOfInputs.rightButton);
            altUp.AddDefaultBinding(sourceOfInputs.altUpButton);
            altDown.AddDefaultBinding(sourceOfInputs.altDownButton);
            altLeft.AddDefaultBinding(sourceOfInputs.altLeftButton);
            altRight.AddDefaultBinding(sourceOfInputs.altRightButton);
            confirmAction.AddDefaultBinding(sourceOfInputs.confirmButton);
            lockThrowAction.AddDefaultBinding(sourceOfInputs.lockThrowingButton);
        }
        if (type == actionSetType.keyboard || type == actionSetType.both)
        {
            throwAction.AddDefaultBinding(sourceOfInputs.throwKey);
            catchAction.AddDefaultBinding(sourceOfInputs.catchKey);
            jumpAction.AddDefaultBinding(sourceOfInputs.jumpKey);
            lockMovementAction.AddDefaultBinding(sourceOfInputs.lockMovementKey);
            resetAction.AddDefaultBinding(sourceOfInputs.resetKey);
            switchAction.AddDefaultBinding(sourceOfInputs.swapKey);
            up.AddDefaultBinding(sourceOfInputs.upKey);
            down.AddDefaultBinding(sourceOfInputs.downKey);
            left.AddDefaultBinding(sourceOfInputs.leftKey);
            right.AddDefaultBinding(sourceOfInputs.rightKey);
            altUp.AddDefaultBinding(sourceOfInputs.altUpKey);
            altDown.AddDefaultBinding(sourceOfInputs.altDownKey);
            altLeft.AddDefaultBinding(sourceOfInputs.altLeftKey);
            altRight.AddDefaultBinding(sourceOfInputs.altRightKey);
            confirmAction.AddDefaultBinding(sourceOfInputs.confirmKey);
            lockThrowAction.AddDefaultBinding(sourceOfInputs.lockThrowingKey);
        }
        if (type == actionSetType.P2KeyboardP1)
        {
            throwAction.AddDefaultBinding(sourceOfInputs.p1ThrowKey);
            catchAction.AddDefaultBinding(sourceOfInputs.p1CatchKey);
            jumpAction.AddDefaultBinding(sourceOfInputs.p1JumpKey);
            lockMovementAction.AddDefaultBinding(sourceOfInputs.p1LockMovementKey);
            resetAction.AddDefaultBinding(sourceOfInputs.p1ResetKey);
            switchAction.AddDefaultBinding(sourceOfInputs.p1SwapKey);
            up.AddDefaultBinding(sourceOfInputs.p1UpKey);
            down.AddDefaultBinding(sourceOfInputs.p1DownKey);
            left.AddDefaultBinding(sourceOfInputs.p1LeftKey);
            right.AddDefaultBinding(sourceOfInputs.p1RightKey);
            altUp.AddDefaultBinding(sourceOfInputs.p1AltUpKey);
            altDown.AddDefaultBinding(sourceOfInputs.p1AltDownKey);
            altLeft.AddDefaultBinding(sourceOfInputs.p1AltLeftKey);
            altRight.AddDefaultBinding(sourceOfInputs.p1AltRightKey);
            confirmAction.AddDefaultBinding(sourceOfInputs.p1ConfirmKey);
            lockThrowAction.AddDefaultBinding(sourceOfInputs.p1LockThrowingKey);
        }
        if (type == actionSetType.P2KeyboardP2)
        {
            throwAction.AddDefaultBinding(sourceOfInputs.p2ThrowKey);
            catchAction.AddDefaultBinding(sourceOfInputs.p2CatchKey);
            jumpAction.AddDefaultBinding(sourceOfInputs.p2JumpKey);
            lockMovementAction.AddDefaultBinding(sourceOfInputs.p2LockMovementKey);
            resetAction.AddDefaultBinding(sourceOfInputs.p2ResetKey);
            switchAction.AddDefaultBinding(sourceOfInputs.p2SwapKey);
            up.AddDefaultBinding(sourceOfInputs.p2UpKey);
            down.AddDefaultBinding(sourceOfInputs.p2DownKey);
            left.AddDefaultBinding(sourceOfInputs.p2LeftKey);
            right.AddDefaultBinding(sourceOfInputs.p2RightKey);
            altUp.AddDefaultBinding(sourceOfInputs.p2AltUpKey);
            altDown.AddDefaultBinding(sourceOfInputs.p2AltDownKey);
            altLeft.AddDefaultBinding(sourceOfInputs.p2AltLeftKey);
            altRight.AddDefaultBinding(sourceOfInputs.p2AltRightKey);
            confirmAction.AddDefaultBinding(sourceOfInputs.p2ConfirmKey);
            lockThrowAction.AddDefaultBinding(sourceOfInputs.p2LockThrowingKey);
        }
    }

}
