using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Moveables : MonoBehaviour
{
    Slots currentSlot;
    public Transform whiteBoxArea;

    private void OnMouseDown()
    {
        currentSlot = SlotsHandler.Instance.GetEmptySlots();
        if (currentSlot != null)
        {
            //MoveToSlot();
            currentSlot.SetMoveables(this);
            MoveDirection dir = GetDirectionFromYRotation();
            MoveViaLPath(this.transform, dir, currentSlot.transform.position);
        }

    }
    private MoveDirection GetDirectionFromYRotation()
    {
        float angle = transform.eulerAngles.y % 360f;

        if (angle >= 337.5f || angle < 22.5f)
            return MoveDirection.Forward;
        else if (angle >= 22.5f && angle < 67.5f)
            return MoveDirection.TopRight;
        else if (angle >= 67.5f && angle < 112.5f)
            return MoveDirection.Right;
        else if (angle >= 112.5f && angle < 157.5f)
            return MoveDirection.BottomRight;
        else if (angle >= 157.5f && angle < 202.5f)
            return MoveDirection.Backward;
        else if (angle >= 202.5f && angle < 247.5f)
            return MoveDirection.BottomLeft;
        else if (angle >= 247.5f && angle < 292.5f)
            return MoveDirection.Left;
        else
            return MoveDirection.TopLeft;
    }


    private void MoveViaLPath(Transform movable, MoveDirection dir, Vector3 target)
    {
        Bounds bounds = whiteBoxArea.GetComponent<Collider>().bounds;
        float offset = 3f;

        Vector3 exit = movable.position;
        Vector3 turn = Vector3.zero;

        Vector3 whiteCenter = bounds.center;

        switch (dir)
        {
            case MoveDirection.Forward:
                exit = new Vector3(movable.position.x, movable.position.y, bounds.max.z + offset);
                turn = new Vector3(target.x, target.y, exit.z);
                break;

            case MoveDirection.Backward:
                // smart decision: check if on left or right
                bool isRightSide = movable.position.x > whiteCenter.x;
                exit = new Vector3(
                    isRightSide ? bounds.max.x + offset : bounds.min.x - offset,
                    movable.position.y,
                    bounds.min.z - offset
                );
                turn = new Vector3(target.x, target.y, exit.z);
                break;

            case MoveDirection.Left:
                exit = new Vector3(bounds.min.x - offset, movable.position.y, movable.position.z);
                turn = new Vector3(exit.x, target.y, target.z);
                break;

            case MoveDirection.Right:
                exit = new Vector3(bounds.max.x + offset, movable.position.y, movable.position.z);
                turn = new Vector3(exit.x, target.y, target.z);
                break;

            case MoveDirection.TopRight:
                exit = new Vector3(bounds.max.x + offset, movable.position.y, bounds.max.z + offset);
                turn = new Vector3(target.x, target.y, exit.z);
                break;

            case MoveDirection.BottomRight:
                exit = new Vector3(bounds.max.x + offset, movable.position.y, bounds.min.z - offset);
                turn = new Vector3(exit.x, target.y, target.z); // X first, then Z
                break;

            case MoveDirection.BottomLeft:
                exit = new Vector3(bounds.min.x - offset, movable.position.y, bounds.min.z - offset);
                turn = new Vector3(exit.x, target.y, target.z); // X first, then Z
                break;

            case MoveDirection.TopLeft:
                exit = new Vector3(bounds.min.x - offset, movable.position.y, bounds.max.z + offset);
                turn = new Vector3(target.x, target.y, exit.z);
                break;
        }

        //// Rotation Not Added
        //Sequence seq = DOTween.Sequence();
        //seq.Append(movable.DOMove(exit, 0.4f).SetEase(Ease.InOutQuad));
        //seq.Append(movable.DOMove(turn, 0.4f).SetEase(Ease.InOutQuad));
        //seq.Append(movable.DOMove(target, 0.4f).SetEase(Ease.InOutQuad));


        //Rotation Added
        Sequence seq = DOTween.Sequence();

        // Calculate rotation angles
        Quaternion rotToExit = Quaternion.LookRotation(exit - movable.position);
        Quaternion rotToTurn = Quaternion.LookRotation(turn - exit);
        Quaternion rotToTarget = Quaternion.LookRotation(target - turn);

        // Append rotate + move to exit
        seq.Append(movable.DORotateQuaternion(rotToExit, 0.3f));
        seq.Join(movable.DOMove(exit, 0.4f).SetEase(Ease.InOutQuad));

        // Rotate + move to turn point
        seq.Append(movable.DORotateQuaternion(rotToTurn, 0.3f));
        seq.Join(movable.DOMove(turn, 0.4f).SetEase(Ease.InOutQuad));

        // Rotate + move to final target
        seq.Append(movable.DORotateQuaternion(rotToTarget, 0.3f));
        seq.Join(movable.DOMove(target, 0.4f).SetEase(Ease.InOutQuad));
    }
    private void OnDrawGizmos()
    {
    }

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