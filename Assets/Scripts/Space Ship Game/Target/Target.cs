using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultHeight;
    [SerializeField] private Vector2 _yAxisRange;
    [SerializeField] private Vector2 _moveSpeedRange;
    private float _moveSpeed;

    public event Action FellOnGround;

    private void Update()
    {
        Move(0, -1);
    }

    public void ResetTarget()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(_yAxisRange.x, _yAxisRange.y), _defaultHeight, 0.0f);
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
