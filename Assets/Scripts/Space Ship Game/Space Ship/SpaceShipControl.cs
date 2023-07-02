using UnityEngine;

public class SpaceShipControl : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.0f, 5.0f)] [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private Vector2 _xAxisLimit = new Vector2(-7, 7);

    public void Move(float xDirection)
    {
        transform.localPosition += new Vector3(xDirection, 0, 0) * Time.deltaTime * _moveSpeed;
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, _xAxisLimit.x, _xAxisLimit.y), transform.localPosition.y, transform.localPosition.z);
    }

    public void SetPositionToDefault()
    {
        transform.localPosition = Vector3.zero;
    }
}
