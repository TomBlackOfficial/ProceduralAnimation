using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Part : Boid
{
    public enum PartStates
    {
        Idle,
        Returning
    }

    private PartStates currentState = PartStates.Idle;
    private Vector3 posEnd;
    private Quaternion rotEnd;
    private Transform parentJoint;
    private bool attached = true;
    private bool wentToBase;
    private float timeCount = 0.0f;

    private Vector3 lerpPosStart, lerpPosEnd;
    private Quaternion lerpRotStart, lerpRotEnd;


    protected override void Awake()
    {
        base.Awake();

        parentJoint = transform.parent.transform;

        posEnd = transform.localPosition;
        rotEnd = transform.localRotation;
    }

    public override void Initialize(BoidSettings settings, BoidManager manager)
    {
        base.Initialize(settings, manager);
    }

    public override void UpdateBoid()
    {
        if (!attached && currentState == PartStates.Returning) 
        {
            if (wentToBase) 
                target = parentJoint.position + posEnd;
            else if (target != manager.transform.position)
            {
                target = manager.transform.position;
            }
            
            base.UpdateBoid();
    
            if (Vector3.Distance(transform.position, target) <= 0.3f && wentToBase) 
            {
                Attach();
            }
            else if (Vector3.Distance(transform.position, target) <= 5f && !wentToBase) 
            {
                wentToBase = true;
            }
        }
        else if (!attached && currentState == PartStates.Idle) 
        {
            base.UpdateBoid();
            // if (timeCount <= 1) 
            // {
            //     if (transform.localPosition != posEnd)
            //         transform.localPosition = Vector3.Lerp(lerpPosStart, lerpPosEnd, timeCount);
            //     if (transform.localRotation != rotEnd)
            //         transform.localRotation = Quaternion.Lerp(lerpRotStart, lerpRotEnd, timeCount);

            //     timeCount = timeCount + Time.deltaTime * 0.5f;
            // }
            
        }
        else if (attached && timeCount <= 1)
        {
            if (transform.localPosition != posEnd)
                transform.localPosition = Vector3.Lerp(lerpPosStart, lerpPosEnd, timeCount);
            if (transform.localRotation != rotEnd)
                transform.localRotation = Quaternion.Lerp(lerpRotStart, lerpRotEnd, timeCount);

            timeCount = timeCount + Time.deltaTime;
        }
    }

    public void Detach(Vector3 offset = default)
    {
        if (!attached)
            return;

        attached = false;
        currentState = PartStates.Idle;

        transform.parent = null;
        target = transform.position + (Vector3.up * 5);
        targeting = true;

        lerpPosStart = transform.position;
        lerpPosEnd = transform.position + offset;
        lerpRotStart = transform.rotation;
        lerpRotEnd = transform.rotation;

        timeCount = 0;
    }

    public void Return()
    {
        if (attached)
            return;

        targeting = true;
        currentState = PartStates.Returning;
    }

    private void Attach()
    {
        currentState = PartStates.Idle;
        transform.parent = parentJoint;
        attached = true;
        wentToBase = false;
        
        lerpPosStart = transform.localPosition;
        lerpPosEnd = posEnd;
        lerpRotStart = transform.localRotation;
        lerpRotEnd = rotEnd;

        timeCount = 0;
    }
}
