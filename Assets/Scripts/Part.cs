using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : Boid
{
    public enum PartStates
    {
        Attached,
        Detached,
        Returning
    }

    private Vector3 attackPoint = new Vector3(7f, 4f, -5f);

    public Vector3 posOffset;
    public Vector3 rotOffset;
    public Vector3 scaleOffset;
    public Vector3 desiredPos;

    public PartStates currentState = PartStates.Attached;

    private Transform parentJoint;

    protected override void Awake()
    {
        base.Awake();

        parentJoint = transform.parent.transform;

        posOffset = transform.localPosition;
        rotOffset = transform.localEulerAngles;
        scaleOffset = transform.localScale;
    }

    public override void Initialize(BoidSettings settings)
    {
        base.Initialize(settings);
    }

    public override void UpdateBoid()
    {
        if (currentState != PartStates.Attached)
            base.UpdateBoid();
    }

    public void Detach()
    {
        currentState = PartStates.Detached;

        transform.parent = null;
        desiredPos = attackPoint;
    }

    public void Attach()
    {
        currentState = PartStates.Attached;

        transform.parent = parentJoint;

        transform.localPosition = posOffset;
        transform.localEulerAngles = rotOffset;
        transform.localScale = scaleOffset;
    }

    public void Return()
    {
        currentState = PartStates.Returning;
    }

}
