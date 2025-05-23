using UnityEngine;
using Unity.Cinemachine;

public class CameraZoomSetting : MonoBehaviour
{
    private CinemachineCamera _camera;

    private float _scrollSpeed = 100.0f;

    [Header("Camera Settings")]
    [SerializeField] private float _minOffsetY = 0.0f;
    [SerializeField] private float _maxOffsetY = 7.0f;
    [SerializeField] private float _minOffsetZ = -7.0f;
    [SerializeField] private float _maxOffsetZ = 0.0f;

    private CinemachineOrbitalFollow _orbitalFollow;

    private void Awake()
    {
        _camera = this.GetComponent<CinemachineCamera>();
        _orbitalFollow = _camera.GetComponent<CinemachineOrbitalFollow>();
    }

    private void Update()
    {
        // 마우스 휠 값 얻기
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector3 offset = _orbitalFollow.TargetOffset;

            offset.y = Mathf.Clamp(offset.y - scroll * _scrollSpeed * Time.deltaTime, _minOffsetY, _maxOffsetY);
            offset.z = -offset.y;

            _orbitalFollow.TargetOffset = offset;
        }
    }
}
