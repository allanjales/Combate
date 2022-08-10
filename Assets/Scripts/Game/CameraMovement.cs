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

	private Vector2 _MinCorner, _MaxCorner;

	private void Awake()
	{
		_Camera = GetComponent<Camera>();
		_maxZoom = _Camera.orthographicSize;
		_MinCorner = new(_Camera.transform.position.x -  _Camera.orthographicSize * _Camera.aspect, _Camera.transform.position.y -  _Camera.orthographicSize);
		_MaxCorner = new(_Camera.transform.position.x +  _Camera.orthographicSize * _Camera.aspect, _Camera.transform.position.y +  _Camera.orthographicSize);
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
			Vector3 difference = _DragOrigin - _Camera.ScreenToWorldPoint(Input.mousePosition);
			_Camera.transform.position = ClampCamera(_Camera.transform.position + difference);
		}
	}

	private void ZoomCamera()
	{
		if (Input.mouseScrollDelta.y != 0)
		{
			_Camera.orthographicSize = Mathf.Clamp(_Camera.orthographicSize - Input.mouseScrollDelta.y * _zoomStep, _minZoom, _maxZoom);
			_Camera.transform.position = ClampCamera(_Camera.transform.position);
		}
	}

	private Vector3 ClampCamera(Vector3 targetPosition)
	{
		float camHeight = _Camera.orthographicSize;
		float camWidth = _Camera.orthographicSize * _Camera.aspect;

		float newX = Mathf.Clamp(targetPosition.x, _MinCorner.x + camWidth, _MaxCorner.x - camWidth);
		float newY = Mathf.Clamp(targetPosition.y, _MinCorner.y + camHeight, _MaxCorner.y - camHeight);

		return new Vector3(newX, newY, targetPosition.z);
	}
}