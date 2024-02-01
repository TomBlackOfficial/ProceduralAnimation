using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    List<Part> parts = new List<Part>();

    void Start () {
        parts = GetComponentsInChildren<Part>(false).ToList();
        foreach (Part b in parts) {
            b.Initialize (settings);
        }
    }

    void Update () {
        if (parts != null) {

            string functionName = null;
            Vector3 mousePos = Vector3.zero;

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit))
                {
                    mousePos = raycastHit.point;

                    GameObject testObj = Instantiate(new GameObject("Test"));
                    testObj.transform.position = mousePos;
                }
            }

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
                if (functionName != null)
                {
                    //if (functionName != "Return")
                        //parts[i].target = mousePos;

                    parts[i].SendMessage(functionName);
                }

                parts[i].avgFlockHeading = boidData[i].flockHeading;
                parts[i].centreOfFlockmates = boidData[i].flockCentre;
                parts[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                parts[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                parts[i].UpdateBoid();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData {
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