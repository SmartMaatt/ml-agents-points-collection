using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

[RequireComponent(typeof(SpaceShipControl))]
[RequireComponent(typeof(SpaceShipState))]
public class SpaceShipAgent : Agent
{
    [Header("Space Ship References")]
    private SpaceShipControl _shipControl;
    private SpaceShipState _shipState;

    [Header("Targets")]
    [SerializeField] private Target[] _validTargets = new Target[2];
    [SerializeField] private Target[] _invalidTargets = new Target[2];

    [Header("References")]
    [SerializeField] private EpisodeResultColorNotifier _colorNotifier;

    [Header("Rewards")]
    [SerializeField] private float _winPoints = 1.0f;
    [SerializeField] private float _losePoints = -1.0f;

    [Space]
    [SerializeField] private float _collectValidTargetPoints = 0.2f;
    [SerializeField] private float _collectInvalidTargetPoints = -0.2f;

    [Header("Settings")]
    [SerializeField] private Vector2 _playerXAxisLimits = new Vector2(-7, 7);


    // Setup methods
    private void Start()
    {
        _shipControl = GetComponent<SpaceShipControl>();
        _shipState = GetComponent<SpaceShipState>();
    }

    public override void OnEpisodeBegin()
    {
        _shipControl.SetPositionToDefault();
        _shipState.ResetPoints();

        foreach (Target target in _validTargets)
        {
            target.ResetTarget();
        }

        foreach (Target target in _invalidTargets)
        {
            target.ResetTarget();
        }
    }


    // Observations
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space ship position [1]
        sensor.AddObservation(Normalization.NormalizeFloat(
                transform.localPosition.x,
                _playerXAxisLimits.x,
                _playerXAxisLimits.y
            ));

        //Debug.Log(Normalization.NormalizeFloat(
        //        transform.localPosition.x,
        //        _playerXAxisLimits.x,
        //        _playerXAxisLimits.y
        //    ));

        // Current score [1]
        sensor.AddObservation(Normalization.NormalizeFloat(
                (float)_shipState.CollectedPoints,
                0.0f,
                (float)_shipState.MaxPoints
            ));

        // 2 x Valid target [2] + 2 x Invalid target [2] 
        // [8] Observations
        try
        {
            sensor.AddObservation(_validTargets[0].NormalizedPosition);
            //Debug.Log(_validTargets[0].NormalizedPosition.x);
            sensor.AddObservation(_validTargets[1].NormalizedPosition);

            sensor.AddObservation(_invalidTargets[0].NormalizedPosition);
            sensor.AddObservation(_invalidTargets[1].NormalizedPosition);
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.LogError(ex.Message);
        }
    }


    // Actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        _shipControl.Move(moveX);
    }


    // Rewards
    private void OnTriggerEnter(Collider other)
    {
        // Collect valid target
        if (other.TryGetComponent<PointTarget>(out PointTarget validTarget))
        {
            SetReward(_collectValidTargetPoints);
            validTarget.ResetTarget();
            _shipState.AddPoints(1);
        }

        // Collect invalid target
        if (other.TryGetComponent<VirusTarget>(out VirusTarget invalidTarget))
        {
            SetReward(_collectInvalidTargetPoints);
            FinishEpisode(false);
        }
    }

    private void Update()
    {
        // Won episode
        if (_shipState.IsPointsMax())
        {
            FinishEpisode(true);
        }
    }

    private void FinishEpisode(bool wonEpisode)
    {
        _shipState.ResetPoints();

        if (wonEpisode)
        {
            _colorNotifier.OnEpisodeWon();
            AddReward(_winPoints);
        }
        else
        {
            _colorNotifier.OnEpisodeLost();
            AddReward(_losePoints);
        }

        //Debug.Log(GetCumulativeReward());
        EndEpisode();
    }


    // Testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }
}
