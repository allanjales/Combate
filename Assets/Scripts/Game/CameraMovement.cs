using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		return new Vector2(_OriginalCameraPosition.x - _originalOrthographicSize * _Camera.aspect, _OriginalCameraPosition.y - _originalOrthographicSize);
	}

	private Vector2 GetMaxCorner()
	{
		return new Vector2(_OriginalCameraPosition.x + _originalOrthographicSize * _Camera.aspect, _OriginalCameraPosition.y + _originalOrthographicSize);
	}

	void Update()
	{
		PanCamera();
		ZoomCamera();
	}

	private void PanCamera()
	{
		//Save position of mouse when drag starts
		if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			_DragOrigin = _Camera.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			Vector3 Difference = _DragOrigin - _Camera.ScreenToWorldPoint(Input.mousePosition);
			_Camera.transform.position = ClampCamera(_Camera.transform.position + Difference);
		}
	}

	private void ZoomCamera()
	{
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