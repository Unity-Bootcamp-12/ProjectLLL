using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraZoomSetting : MonoBehaviour
{
    private CinemachineCamera _camera;

    private float _scrollSpeed = 2000.0f;
	private float _minFov = 60.0f;
	private float _maxFov = 80.0f;

	private void Awake()
    {
        _camera = this.GetComponent<CinemachineCamera>();
    }

	private void Update()
	{
		// ���콺 �� �� ���
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if(Mathf.Abs(scroll) > 0.001f)
		{
			// ���� ���� ���� ����
			var lens = _camera.Lens;

			// FOV ����(��ũ�� ��/�� �ΰ���, ��ŸŸ��, Ŭ������ ���� ����)
			lens.FieldOfView = Mathf.Clamp(
				lens.FieldOfView - scroll * _scrollSpeed * Time.deltaTime,
				_minFov, _maxFov
			);

			// ����� ���� ���� ����
			_camera.Lens = lens;
		}
	}
}
