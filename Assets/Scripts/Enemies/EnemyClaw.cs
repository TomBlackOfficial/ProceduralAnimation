using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.Mathematics;

public class EnemyClaw : BoidManager
{
    public Transform player;
    public Transform target;

    [Header("Attacking")]
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private float attackDuration;

    [Header("Returning")]
    [SerializeField] private AnimationCurve returnCurve;
    [SerializeField] private float returnDuration;

    [Header("VFX")]
    [SerializeField] private GameObject impactVFX;

    private float curveTimeElapsed = 0;
    private Vector3 curveStartPos;
    private Vector3 curveEndPos;

    protected override void Update()
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

        base.Update();
    }

    public override void DetatchAll()
    {
        Vector3 direction = new Vector3(curveEndPos.x, 0, curveEndPos.z) - new Vector3(transform.position.x, 0, transform.position.z).normalized;
        detatchOffset = (Vector3.up * 4) + (direction * 5);
        base.DetatchAll();

        StartCoroutine(WaitForSeconds(2, "ReturnAll"));
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
            currentState = EnemyState.Idle;
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
            Impact();
            Return();
            return;
        }

        float percent = Mathf.Clamp01(curveTimeElapsed / attackDuration);
        float curvePercent = attackCurve.Evaluate(percent);
        target.position = Vector3.LerpUnclamped(curveStartPos, curveEndPos, curvePercent);
    }

    private void Impact() 
    {
        CameraShakeManager.INSTANCE.ShakeOnce(12f, 0.35f, 0.25f);
        Instantiate(impactVFX, curveEndPos, Quaternion.identity);
    }

    IEnumerator WaitForSeconds(float time, string functionName)
    {
        yield return new WaitForSeconds(time);

        SendMessage(functionName);
    }
}
