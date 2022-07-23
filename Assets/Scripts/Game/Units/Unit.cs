using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	[SerializeField] private Sprite[] _sprites_vermelhas;
	[SerializeField] private Sprite[] _sprites_azuis;

	private Vector3 Velocity = Vector3.zero;
	private Vector2 TargetPosition;
	private float moveTime = 0.05f;

	public int UnitNumber { get; private set; } = -1;
	public int UnitArmy { get; private set; } = -1;
	public string UnitName { get; private set; } = null;
	public Tile OccupiedTile;

	private SpriteRenderer _spriteRenderer;

	private void Start()
	{
		this.GetComponent<SpriteRenderer>().sortingOrder = 2;
	}

	public void SetUnit(int UnitArmy, int UnitNumber, string UnitName)
	{
		if (this.UnitNumber != -1 || this.UnitArmy != -1)
			return;

		this.UnitArmy = UnitArmy;
		this.UnitNumber = UnitNumber;
		this.UnitName = UnitName;

		UpdateSpriteRenderer();
	}

	public void MoveTo(Vector2 NewPos)
	{
		TargetPosition = NewPos;
	}

    private void Update()
	{
		transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, moveTime);
	}

    public void UpdateSpriteRenderer()
	{

		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (GameManager.Instance.playerArmy == UnitArmy || GameManager.Instance.GameState == GameState.Finish)
		{
			_spriteRenderer.sprite = (this.UnitArmy == 0) ? _sprites_vermelhas[this.UnitNumber] : _sprites_azuis[this.UnitNumber];
			return;
		}

		_spriteRenderer.sprite = (this.UnitArmy == 0) ? _sprites_vermelhas[0] : _sprites_azuis[0];
	}

	public void DeleteItself()
    {
		OccupiedTile.OccupiedUnit = null;
		if (UnitManager.Instance.SelectedUnit == this) UnitManager.Instance.SetSelectedUnit(null);
		Destroy(gameObject);
    }

	public string GetUnitNumberString()
	{
		if (UnitNumber > 0 && UnitNumber < 11)
			return $"{UnitNumber}";
		return "-";
	}

	public bool IsMyUnit()
    {
		if (GameManager.Instance.playerArmy == UnitArmy)
			return true;

		return false;
    }
}