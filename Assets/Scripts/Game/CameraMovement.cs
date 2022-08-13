using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
	private Camera _Camera;
	private Vector3 _DragOrigin;

	[SerializeField][Range(0.1f, 3f)] private float _zoomStep = 1;
	[SerializeField] private float _minZoom = 1;
	private float _maxZoom;

	private Vector2 _OriginalCameraPosition;
	private float _originalOrthographicSize;

	private void Awake()
	{
		_Camera = GetComponent<Camera>();
		_maxZoom = _Camera.orthographicSize;
		_OriginalCameraPosition = _Camera.transform.position;
		_originalOrthographicSize = _Camera.orthographicSize;
	}

	private Vector2 GetMinCorner()
	{
		return new Vector2(_OriginalCameraPosition.x - _originalOrthographicSize / ((_Camera.aspect < 1) ? _Camera.aspect : 1) * _Camera.aspect,
			_OriginalCameraPosition.y - _originalOrthographicSize / ((_Camera.aspect < 1) ? _Camera.aspect : 1));
	}

	private Vector2 GetMaxCorner()
	{
		return new Vector2(_OriginalCameraPosition.x + _originalOrthographicSize / ((_Camera.aspect < 1) ? _Camera.aspect : 1) * _Camera.aspect,
			_OriginalCameraPosition.y + _originalOrthographicSize / ((_Camera.aspect < 1) ? _Camera.aspect : 1));
	}

	void Update()
	{
		float oldMaxZoom = _maxZoom;
		_maxZoom = _originalOrthographicSize / ((_Camera.aspect < 1) ? _Camera.aspect : 1);
		if (_maxZoom != oldMaxZoom)
			_Camera.orthographicSize = _maxZoom;

		if (Input.touchCount > 0)
		{
			TouchCameraControl();
			return;
		}

		PanMouseCamera();
		ZoomMouseCamera();
	}

	private void TouchCameraControl()
	{
		if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
			{
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

				_Camera.orthographicSize = Mathf.Clamp(prevMagnitude / currentMagnitude * _Camera.orthographicSize, _minZoom, _maxZoom);

				Vector2 touchPrevMedPos = (touchZeroPrevPos + touchOnePrevPos) / 2;
				Vector2 touchCurrentMedPos = (touchZero.position + touchOne.position) / 2;

				Vector3 DifferencePos = _Camera.ScreenToWorldPoint(touchCurrentMedPos) - _Camera.ScreenToWorldPoint(touchPrevMedPos);
				_Camera.transform.position = ClampCamera(_Camera.transform.position - DifferencePos);
			}
		}
	}

	private void PanMouseCamera()
	{
		if (!Application.isFocused)
			return;

		//Save position of mouse when drag starts
		if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			_DragOrigin = _Camera.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			Vector3 Difference = _DragOrigin - _Camera.ScreenToWorldPoint(Input.mousePosition);
			_Camera.transform.position = ClampCamera(_Camera.transform.position + Difference);
		}
	}

	private void ZoomMouseCamera()
	{
		if (!Application.isFocused)
			return;

		if (Input.mouseScrollDelta.y != 0)
		{
			Vector3 OldCursorPosition = _Camera.ScreenToWorldPoint(Input.mousePosition);
			_Camera.orthographicSize = Mathf.Clamp(_Camera.orthographicSize - Input.mouseScrollDelta.y * _zoomStep, _minZoom, _maxZoom);
			_Camera.transform.position = ClampCamera(_Camera.transform.position - (_Camera.ScreenToWorldPoint(Input.mousePosition) - OldCursorPosition));
		}
	}

	private Vector3 ClampCamera(Vector3 targetPosition)
	{
		float camHeight = _Camera.orthographicSize;
		float camWidth = _Camera.orthographicSize * _Camera.aspect;

		Vector2 MinCorner = GetMinCorner();
		Vector2 MaxCorner = GetMaxCorner();

		float newX = Mathf.Clamp(targetPosition.x, MinCorner.x + camWidth, MaxCorner.x - camWidth);
		float newY = Mathf.Clamp(targetPosition.y, MinCorner.y + camHeight, MaxCorner.y - camHeight);

		return new Vector3(newX, newY, targetPosition.z);
	}
}