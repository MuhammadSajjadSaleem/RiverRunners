using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RunnerAgent : Agent
{
    public Transform target;
    public float moveSpeed = 5f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        if (target != null)
            target.localPosition = new Vector3(0, 0, 10); // move target forward
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // agent pos
        if (target != null)
            sensor.AddObservation(target.localPosition); // target pos
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 movement = new Vector3(moveX, 0, moveZ);
        transform.localPosition += movement * moveSpeed * Time.deltaTime;

        if (target != null)
        {
            float distance = Vector3.Distance(transform.localPosition, target.localPosition);
            if (distance < 1.5f)
            {
                SetReward(1f);
                EndEpisode();
            }
        }

        SetReward(-0.001f); // tiny penalty each frame
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var act = actionsOut.ContinuousActions;
        act[0] = Input.GetAxis("Horizontal");
        act[1] = Input.GetAxis("Vertical");
    }
}
