using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	[SerializeField] private Sprite[] _sprites_vermelhas;
	[SerializeField] private Sprite[] _sprites_azuis;

	public int UnitNumber { get; private set; } = -1;
	public int UnitColor { get; private set; } = -1;
	public string UnitName { get; private set; } = null;
	public Tile OccupiedTile;

	private SpriteRenderer _spriteRenderer;

	private void Start()
	{
		this.GetComponent<SpriteRenderer>().sortingOrder = 2;
	}

	public void SetUnit(int UnitColor, int UnitNumber, string UnitName)
	{
		if (this.UnitNumber != -1 || this.UnitColor != -1)
			return;

		this.UnitColor = UnitColor;
		this.UnitNumber = UnitNumber;
		this.UnitName = UnitName;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (GameManager.Instance.PlayerSide != UnitColor)
		{
			_spriteRenderer.sprite = (this.UnitColor == 0) ? _sprites_vermelhas[0] : _sprites_azuis[0];
			return;
		}

		_spriteRenderer.sprite = (this.UnitColor == 0) ? _sprites_vermelhas[this.UnitNumber ] : _sprites_azuis[this.UnitNumber ];
	}

	public string GetUnitNumberString()
	{
		if (UnitNumber > 0 && UnitNumber < 11)
			return $"{UnitNumber}";
		return "-";
	}
}