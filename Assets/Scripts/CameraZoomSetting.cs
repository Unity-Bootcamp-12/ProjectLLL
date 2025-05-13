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
		// 마우스 휠 값 얻기
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if(Mathf.Abs(scroll) > 0.001f)
		{
			// 현재 렌즈 세팅 복사
			var lens = _camera.Lens;

			// FOV 조정(스크롤 앞/뒤 민감도, 델타타임, 클램프로 범위 제한)
			lens.FieldOfView = Mathf.Clamp(
				lens.FieldOfView - scroll * _scrollSpeed * Time.deltaTime,
				_minFov, _maxFov
			);

			// 변경된 렌즈 세팅 적용
			_camera.Lens = lens;
		}
	}
}
