using UnityEngine;

public class UnitRangeTrigger : MonoBehaviour
{
    [SerializeField] private UnitController _ownerUnit;

    private void OnTriggerEnter(Collider other)
    {
        _ownerUnit.OnTriggerEnterFromChild(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _ownerUnit.OnTriggerExitFromChild(other);
    }
}