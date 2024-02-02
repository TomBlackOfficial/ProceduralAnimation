using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class BoidManager : MonoBehaviour 
{
    public enum EnemyState
    {
        Idle,
        Attacking,
        Returning
    }


    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    List<Part> parts = new List<Part>();

    protected EnemyState currentState = EnemyState.Idle;

    protected Vector3 detatchOffset = new Vector3(0, 0, 0);

    void Start () 
    {
        parts = GetComponentsInChildren<Part>(false).ToList();
        foreach (Part b in parts) {
            b.Initialize (settings, this);
        }
    }

    protected virtual void Update () 
    {
        if (parts != null) 
        {
            int numBoids = parts.Count;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < parts.Count; i++) {
                boidData[i].position = parts[i].position;
                boidData[i].direction = parts[i].forward;
            }

            var boidBuffer = new ComputeBuffer (numBoids, BoidData.Size);
            boidBuffer.SetData (boidData);

            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", parts.Count);
            compute.SetFloat ("viewRadius", settings.perceptionRadius);
            compute.SetFloat ("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            boidBuffer.GetData (boidData);

            for (int i = 0; i < parts.Count; i++) {
                parts[i].avgFlockHeading = boidData[i].flockHeading;
                parts[i].centreOfFlockmates = boidData[i].flockCentre;
                parts[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                parts[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                parts[i].UpdateBoid();
            }

            boidBuffer.Release ();
        }
    }

    public virtual void DetatchAll() 
    {
        if (currentState != EnemyState.Attacking)
            return;  
            
        for (int i = 0; i < parts.Count; i++) {
                parts[i].Detach(detatchOffset);
        }
    }

    public virtual void ReturnAll() 
    {
        if (currentState == EnemyState.Attacking)
            return;  
        
        for (int i = 0; i < parts.Count; i++) {
                parts[i].Return();
        }
    }

    public struct BoidData 
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}