using TMPro;
using UnityEngine;

public class ScoreLabelDisplayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpaceShipState _shipState;
    [SerializeField] private TMP_Text _label;

    private void Update()
    {
        _label.text = $"{_shipState.CollectedPoints}/{_shipState.MaxPoints}";
    }
}
