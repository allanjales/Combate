using UnityEngine;

public class TouchHandler : MonoBehaviour
{
	[SerializeField] private float _holdTile = 0.2f;

	private Tile _firstTouchedTile = null;
	private Tile _lastTouchedTile = null;

	private Vector3 _touchPosWorld;
	private float _touchSince;

	private void Start()
	{
		Input.simulateMouseWithTouches = false;
	}

	private bool isLongTouch()
	{
		return (Time.time - _touchSince) > _holdTile;
	}

	void Update()
	{
		if (Input.touchCount == 1)
		{
			_touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

			Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
			RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

			if (hitInformation.collider != null && hitInformation.transform.gameObject.GetComponent<Tile>() != null)
			{
				Tile touchedTile = hitInformation.transform.gameObject.GetComponent<Tile>();

				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					_touchSince = Time.time;
					_firstTouchedTile = touchedTile;
				}

				if (_lastTouchedTile)
					_lastTouchedTile.Unhighlight();

				if (isLongTouch())
					touchedTile.Highlight();

				_lastTouchedTile = touchedTile;

				if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					if (_firstTouchedTile == touchedTile && !isLongTouch())
						touchedTile.Click();

					_lastTouchedTile.Unhighlight();
				}
			}
		}
	}
}