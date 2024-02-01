using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class EnemyClaw : BoidManager
{
    public enum EnemyState
    {
        Idle,
        Targeting,
        Attacking,
        Returning,
        Dead
    }

    public Transform player;
    public Transform target;

    [Header("Attacking")]
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private float attackDuration;

    [Header("Returning")]
    [SerializeField] private AnimationCurve returnCurve;
    [SerializeField] private float returnDuration;

    private float curveTimeElapsed = 0;
    private Vector3 curveStartPos;
    private Vector3 curveEndPos;
    private EnemyState currentState = EnemyState.Targeting;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Attack();
        }

        if (currentState == EnemyState.Attacking)
        {
            Attacking();
        }
        else if (currentState == EnemyState.Returning)
        {
            Returning();
        }
    }

    public void Attack()
    {
        currentState = EnemyState.Attacking;
        curveTimeElapsed = 0;
        curveStartPos = target.position;
        curveEndPos = player.position;
    }

    private void Return()
    {
        currentState = EnemyState.Returning;
        curveTimeElapsed = 0;
        curveEndPos = curveStartPos;
        curveStartPos = target.position;
    }

    public void Returning()
    {
        curveTimeElapsed += Time.deltaTime;

        if (curveTimeElapsed >= returnDuration)
        {
            currentState = EnemyState.Targeting;
            return;
        }

        float percent = Mathf.Clamp01(curveTimeElapsed / returnDuration);
        float curvePercent = returnCurve.Evaluate(percent);
        target.position = Vector3.LerpUnclamped(curveStartPos, curveEndPos, curvePercent);
    }

    public void Attacking()
    {
        curveTimeElapsed += Time.deltaTime;

        if (curveTimeElapsed >= attackDuration)
        {
            Return();
            return;
        }

        float percent = Mathf.Clamp01(curveTimeElapsed / attackDuration);
        float curvePercent = attackCurve.Evaluate(percent);
        target.position = Vector3.LerpUnclamped(curveStartPos, curveEndPos, curvePercent);
    }
}
