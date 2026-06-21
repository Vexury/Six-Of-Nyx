using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineCamera closeCam;
    [SerializeField] private SphereRotator sphereRotator;

    private bool _isClose;

    private void OnEnable()
    {
        inputReader.SecondaryActionEvent += OnSecondaryAction;
    }

    private void OnDisable()
    {
        inputReader.SecondaryActionEvent -= OnSecondaryAction;
    }

    private void OnSecondaryAction(bool performed)
    {
        if (!performed) return;
        _isClose = !_isClose;
        closeCam.enabled = _isClose;
        sphereRotator.enabled = !_isClose;
    }
}
