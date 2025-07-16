using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Moveables : MonoBehaviour
{
    Slots currentSlot;
    public Transform whiteBoxArea;

    [Header("Box Area & Corners")]
    public Transform topLeftCorner;
    public Transform topRightCorner;
    public Transform bottomLeftCorner;
    public Transform bottomRightCorner;


    //private void OnMouseDown()
    //{
    //    currentSlot = SlotsHandler.Instance.GetEmptySlots();
    //    if (currentSlot != null)
    //    {
    //        //MoveToSlot();
    //        currentSlot.SetMoveables(this);
    //        MoveDirection dir = GetMoveDirectionBasedOnYRotation();
    //        MoveViaLPath(this.transform, dir, currentSlot.transform.position);
    //    }

    //}
    //MoveDirection GetMoveDirectionBasedOnYRotation()
    //{
    //    float yRot = transform.eulerAngles.y;

    //    if (yRot >= 315 || yRot < 45) return MoveDirection.Forward;
    //    if (yRot >= 45 && yRot < 90) return MoveDirection.TopRight;
    //    if (yRot >= 90 && yRot < 135) return MoveDirection.Right;
    //    if (yRot >= 135 && yRot < 180) return MoveDirection.BottomRight;
    //    if (yRot >= 180 && yRot < 225) return MoveDirection.Backward;
    //    if (yRot >= 225 && yRot < 270) return MoveDirection.BottomLeft;
    //    if (yRot >= 270 && yRot < 315) return MoveDirection.Left;

    //    return MoveDirection.Forward;
    //}


    //private void MoveViaLPath(Transform movable, MoveDirection dir, Vector3 target)
    //{
    //    Bounds bounds = whiteBoxArea.GetComponent<Collider>().bounds;
    //    float offset = 1.2f;

    //    Vector3 exit = movable.position;
    //    Vector3 turn = Vector3.zero;
    //    Vector3 whiteCenter = bounds.center;

    //    switch (dir)
    //    {
    //        case MoveDirection.Forward:
    //            exit = new Vector3(movable.position.x, movable.position.y, bounds.max.z + offset);
    //            turn = new Vector3(target.x, target.y, exit.z);
    //            break;

    //        case MoveDirection.Backward:
    //            bool isRightSide = movable.position.x > whiteCenter.x;
    //            exit = new Vector3(
    //                isRightSide ? bounds.max.x + offset : bounds.min.x - offset,
    //                movable.position.y,
    //                bounds.min.z - offset
    //            );
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.Left:
    //            exit = new Vector3(bounds.min.x - offset, movable.position.y, movable.position.z);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.Right:
    //            exit = new Vector3(bounds.max.x + offset, movable.position.y, movable.position.z);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.TopRight:
    //            exit = new Vector3(bounds.max.x + offset, movable.position.y, bounds.max.z + offset);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.BottomRight:
    //            exit = new Vector3(bounds.max.x + offset, movable.position.y, bounds.min.z - offset);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.BottomLeft:
    //            exit = new Vector3(bounds.min.x - offset, movable.position.y, bounds.min.z - offset);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;

    //        case MoveDirection.TopLeft:
    //            exit = new Vector3(bounds.min.x - offset, movable.position.y, bounds.max.z + offset);
    //            turn = new Vector3(exit.x, target.y, target.z);
    //            break;
    //    }

    //    // Rotation targets
    //    Quaternion rotToExit = Quaternion.LookRotation(exit - movable.position);
    //    Quaternion rotToTurn = Quaternion.LookRotation(turn - exit);

    //    // DOTween movement + rotation sequence
    //    Sequence seq = DOTween.Sequence();
    //    seq.Append(movable.DORotateQuaternion(rotToExit, 0.3f));
    //    seq.Join(movable.DOMove(exit, 0.4f).SetEase(Ease.InOutQuad));

    //    seq.Append(movable.DORotateQuaternion(rotToTurn, 0.3f));
    //    seq.Join(movable.DOMove(turn, 0.4f).SetEase(Ease.InOutQuad));

    //    // Move to final slot and reset rotation to zero
    //    seq.Append(movable.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.3f));
    //    seq.Join(movable.DOMove(target, 0.4f).SetEase(Ease.OutBack));
    //}
    //private void OnDrawGizmos()
    //{
    //}

    private void OnMouseDown()
    {
        currentSlot = SlotsHandler.Instance.GetEmptySlots();
        if (currentSlot != null)
        {
            MoveViaOuterPath(this.transform);
        }
    }

    public void MoveViaOuterPath(Transform movable)
    {
        Vector3 startPos = movable.position;
        float yRotation = movable.eulerAngles.y;

        Vector3 corner1 = Vector3.zero;
        Vector3 corner2 = Vector3.zero;

        // Decide movement direction based on Y rotation
        if (IsFacingUp(yRotation))
        {
            // Directly go up and then in
            corner1 = new Vector3(startPos.x, startPos.y, topLeftCorner.position.z);
        }
        else if (IsFacingDown(yRotation))
        {
            // Choose left or right path based on X position
            if (startPos.x < whiteBoxArea.position.x)
            {
                corner1 = bottomLeftCorner.position;
                corner2 = topLeftCorner.position;
            }
            else
            {
                corner1 = bottomRightCorner.position;
                corner2 = topRightCorner.position;
            }
        }
        else if (IsFacingLeft(yRotation))
        {
            corner1 = bottomLeftCorner.position;
            corner2 = topLeftCorner.position;
        }
        else if (IsFacingRight(yRotation))
        {
            corner1 = bottomRightCorner.position;
            corner2 = topRightCorner.position;
        }
        else if (IsFacingBottomLeft(yRotation))
        {
            corner1 = bottomLeftCorner.position;
            corner2 = topLeftCorner.position;
        }
        else if (IsFacingBottomRight(yRotation))
        {
            corner1 = bottomRightCorner.position;
            corner2 = topRightCorner.position;
        }
        else if (IsFacingTopLeft(yRotation))
        {
            corner1 = topLeftCorner.position;
        }
        else if (IsFacingTopRight(yRotation))
        {
            corner1 = topRightCorner.position;
        }

        Vector3 finalTarget = currentSlot.transform.position;

        // Movement Sequence
        Sequence seq = DOTween.Sequence();
        if (corner1 != Vector3.zero)
            seq.Append(movable.DOMove(corner1, 0.4f).SetEase(Ease.InOutSine));

        if (corner2 != Vector3.zero)
            seq.Append(movable.DOMove(corner2, 0.4f).SetEase(Ease.InOutSine));

        // Final move into slot and rotate to zero
        seq.Append(movable.DOMove(finalTarget, 0.4f).SetEase(Ease.OutBack));
        seq.Join(movable.DORotate(Vector3.zero, 0.3f));
    }

    #region Rotation Checkers

    bool IsFacingUp(float y) => Mathf.Approximately(NormalizeAngle(y), 0f);
    bool IsFacingDown(float y) => Mathf.Approximately(NormalizeAngle(y), 180f);
    bool IsFacingLeft(float y) => Mathf.Approximately(NormalizeAngle(y), 270f);
    bool IsFacingRight(float y) => Mathf.Approximately(NormalizeAngle(y), 90f);
    bool IsFacingBottomLeft(float y) => Mathf.Approximately(NormalizeAngle(y), 225f);
    bool IsFacingBottomRight(float y) => Mathf.Approximately(NormalizeAngle(y), 135f);
    bool IsFacingTopLeft(float y) => Mathf.Approximately(NormalizeAngle(y), 315f);
    bool IsFacingTopRight(float y) => Mathf.Approximately(NormalizeAngle(y), 45f);

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return angle < 0 ? angle + 360f : angle;
    }

    #endregion
}
public enum MoveDirection
{
    Forward,       // 0°
    TopRight,      // 45°
    Right,         // 90°
    BottomRight,   // 135°
    Backward,      // 180°
    BottomLeft,    // 225°
    Left,          // 270°
    TopLeft        // 315°
}
