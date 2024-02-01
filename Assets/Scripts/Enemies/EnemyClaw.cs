using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class EnemyClaw : BoidManager
{
    public Transform player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Attack();
        }
    }

    public void Attack()
    {

    }
}
