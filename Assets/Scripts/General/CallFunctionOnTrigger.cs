using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallFunctionOnTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;

    [Space(10)]
    
    [SerializeField] private UnityEvent onCollision;

    private void OnTriggerEnter(Collider other) 
    {
        if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) 
        {
            onCollision.Invoke();
        }
    }
}
