using UnityEngine;

public class SpaceShipControl : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.0f, 5.0f)] [SerializeField] private float _moveSpeed = 1.0f;

    public void Move(float xDirection)
    {
        transform.localPosition += new Vector3(xDirection, 0, 0) * Time.deltaTime * _moveSpeed;
    }

    public void SetPositionToDefault()
    {
        transform.localPosition = Vector3.zero;
    }
}
