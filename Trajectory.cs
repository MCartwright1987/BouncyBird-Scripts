using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Trajectory : MonoBehaviour
{
    public static Trajectory instance;
    private float predictionTime = 4f;
    public GameObject trajectoryPointPrefab;

    private Rigidbody2D rb;
    public GameObject[] trajectoryPoints;
    private float[] pointDeactivationTimes = new float[100];

    private void Start()
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (trajectoryPoints.Any(p => p.activeSelf))
        {
            DeactivateTrajectoryPoints();
        }
    }

    public void DrawTrajectory()
    {       
        ClearTrajectory();
        
        Vector2 initialMovablePanelPositionion = transform.position;
        Vector2 initialVelocity = rb.velocity;

        //used to make screenshots.
        //initialVelocity = new Vector2(2.3f,11);

        float totalTime = predictionTime;
        float timeIncrement = totalTime / (float)trajectoryPoints.Length;

        float currentTime = Time.time;

        for (int i = 1; i < trajectoryPoints.Length; i++)
        {
            float time = i * timeIncrement;
            Vector2 newPosition = CalculatePositionAtTime(initialMovablePanelPositionion, initialVelocity, time);

            // Get a trajectory point from the array
            GameObject point = trajectoryPoints[i];

            point.transform.position = newPosition;
            point.SetActive(true);

            pointDeactivationTimes[i] = currentTime + time;
        }  
    }

    private Vector2 CalculatePositionAtTime(Vector2 initialMovablePanelPositionion, Vector2 initialVelocity, float time)
    {
        Vector2 gravity = Physics2D.gravity;
        Vector2 position = initialMovablePanelPositionion + initialVelocity * time + 0.5f * gravity * time * time;
        return position;
    }

    private void DeactivateTrajectoryPoints()
    {
        float currentTime = Time.time;

        for (int i = 0; i < pointDeactivationTimes.Length; i++)
        {
            if (trajectoryPoints[i].activeSelf && currentTime >= pointDeactivationTimes[i])
            {
                trajectoryPoints[i].SetActive(false);
                pointDeactivationTimes[i] = 0;
            }
        }
    }

    private void ClearTrajectory()
    {
        // Deactivate existing active trajectory points
        for (int i = 0; i < trajectoryPoints.Length; i++)
        {
            trajectoryPoints[i].SetActive(false);
            pointDeactivationTimes[i] = 0;
        }
    }
}