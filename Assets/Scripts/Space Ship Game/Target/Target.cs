using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultHeight;
    [SerializeField] private Vector2 _xAxisRange;
    [SerializeField] private Vector2 _moveSpeedRange;

    private float _moveSpeed;
    private Vector2 _yAxisRange;

    public Vector2 NormalizedPosition
    {
        get
        {
            return Normalization.NormalizeVector2(
                    new Vector2(transform.localPosition.x, transform.localPosition.y),
                    new Vector2(_xAxisRange.x, _yAxisRange.x),
                    new Vector2(_xAxisRange.y, _yAxisRange.y)
                );
        }
    }

    public event Action FellOnGround;

    private void Start()
    {
        _yAxisRange = new Vector2(0, _defaultHeight);
    }

    private void Update()
    {
        Move(0, -1);
    }

    public void ResetTarget()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(_xAxisRange.x, _xAxisRange.y), _defaultHeight, 0.0f);
        _moveSpeed = UnityEngine.Random.Range(_moveSpeedRange.x, _moveSpeedRange.y);
    }

    public void Move(float x, float y)
    {
        transform.localPosition += new Vector3(x, y, 0) * Time.deltaTime * _moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Floor>(out Floor floor))
        {
            FellOnGround?.Invoke();
            ResetTarget();
        }
    }
}
