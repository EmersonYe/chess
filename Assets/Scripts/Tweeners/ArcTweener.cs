using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] float speed;
    [SerializeField] float height;

    public void MoveTo(Transform transform, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        transform.DOJump(targetPosition, height, 1, distance / speed);
    }
}
