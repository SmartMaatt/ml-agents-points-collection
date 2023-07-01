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

    [Space]
    [SerializeField] private float _loseValidTargetPoints = -0.05f;
    [SerializeField] private float _loseInvalidTargetPoints = +0.05f;

    [Space]
    [SerializeField] private float _timePoints = -0.001f;


    // Setup methods
    private void Start()
    {
        _shipControl = GetComponent<SpaceShipControl>();
        _shipState = GetComponent<SpaceShipState>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (Target target in _validTargets)
        {
            target.FellOnGround += LostValidTarget;
        }

        foreach (Target target in _invalidTargets)
        {
            target.FellOnGround += LostInvalidTarget;
        }
    }

    public override void OnEpisodeBegin()
    {
        _shipControl.SetPositionToDefault();

        foreach (Target target in _validTargets)
        {
            target.ResetTarget();
        }

        foreach (Target target in _invalidTargets)
        {
            target.ResetTarget();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (Target target in _validTargets)
        {
            target.FellOnGround -= LostValidTarget;
        }

        foreach (Target target in _invalidTargets)
        {
            target.FellOnGround -= LostInvalidTarget;
        }
    }


    // Observations
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);       // three observations (x, y, z)

        // 2 x Valid target + 2 x Invalid target = 12 observations
        try
        {
            sensor.AddObservation(_validTargets[0].transform.position);
            sensor.AddObservation(_validTargets[1].transform.position);

            sensor.AddObservation(_invalidTargets[0].transform.position);
            sensor.AddObservation(_invalidTargets[1].transform.position);
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

    private void OnTriggerExit(Collider other)
    {
        // Fell from platform
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(_losePoints);
            FinishEpisode(false);
        }
    }

    public void LostValidTarget()
    {
        SetReward(_loseValidTargetPoints);
    }

    public void LostInvalidTarget()
    {
        SetReward(_loseInvalidTargetPoints);
    }

    private void Update()
    {
        // Time penalty on every tick
        AddReward(_timePoints);

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

        Debug.Log($"Episode ended: {wonEpisode}");
        EndEpisode();
    }


    // Testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }
}
