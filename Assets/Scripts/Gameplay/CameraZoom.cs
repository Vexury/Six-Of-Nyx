using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float zoomStep = 1f;
    [SerializeField] private float minRadius = 4f;
    [SerializeField] private float maxRadius = 20f;

    private CinemachineOrbitalFollow _orbitalFollow;

    private void Awake()
    {
        _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable()
    {
        inputReader.ZoomEvent += OnZoom;
    }

    private void OnDisable()
    {
        inputReader.ZoomEvent -= OnZoom;
    }

    private void OnZoom(float scrollY)
    {
        if (scrollY == 0f) return;
        _orbitalFollow.Radius = Mathf.Clamp(
            _orbitalFollow.Radius - Mathf.Sign(scrollY) * zoomStep,
            minRadius,
            maxRadius
        );
    }
}
