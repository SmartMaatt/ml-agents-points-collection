using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalTarget : Agent
{
    [Header("References")]
    [SerializeField] private Transform _goalTransform;

    [Header("Settings")]
    [Range(0.0f, 5.0f)][SerializeField] private float _moveSpeed = 1.0f;


    // Observations
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);      // three observations (x, y, z)
        sensor.AddObservation(_goalTransform.position);  // three observations (x, y, z)
    }

    // Actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * _moveSpeed;
    }

    // Rewards
    private void OnTriggerEnter(Collider other)
    {
        // Collected goal
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            EndEpisode();
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        // Fell from platform
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    // Episode management
    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
    }

    // Testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
}
