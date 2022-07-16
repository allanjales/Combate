using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField] private Color _baseColor, _offSetColor;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private GameObject _highlight;

	public Unit OccupiedUnit;

	public bool Walkable => OccupiedUnit == null;

	private void Start()
	{
		this.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	public void SetColor(bool isOffset)
	{
		_renderer.color = isOffset ? _offSetColor : _baseColor;
	}

	void OnMouseEnter()
	{
		_highlight.SetActive(true);
		HUDManager.Instance.ShowTileUnit(this);
	}

	void OnMouseExit()
	{
		_highlight.SetActive(false);
		HUDManager.Instance.ShowTileUnit(null);
	}

	public void SetUnit(Unit unit)
	{
		if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
		unit.transform.position = transform.position;
		OccupiedUnit = unit;
		unit.OccupiedTile = this;
	}

	private void OnMouseDown()
	{
		//Unselect
		if (OccupiedUnit != null && OccupiedUnit == UnitManager.Instance.SelectedUnit)
		{
			UnitManager.Instance.SetSelectedUnit(null);
			return;
		}

		//Select
		if (OccupiedUnit != null && OccupiedUnit.UnitColor == GameManager.Instance.PlayerSide)
		{
			UnitManager.Instance.SetSelectedUnit((Unit)OccupiedUnit);
			return;
		}

		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//If is not my turn, ignore it
		if (!GameManager.Instance.IsMyMoveTurn())
			return;

		//If can't move due selected unit number, ignore it
		if (UnitManager.Instance.SelectedUnit.UnitNumber < 1 || UnitManager.Instance.SelectedUnit.UnitNumber > 10)
			return;

		//If can't move due direction is not allowed, ignore it
		if (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.x != this.transform.position.x
			&& UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.y != this.transform.position.y)
			return;

		//If is not a Soldier
		if (UnitManager.Instance.SelectedUnit.UnitNumber != 2
			&& Vector3.Distance(this.transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position) > 1)
			return;

		//If there is a player on the way for soldiers
		if (UnitManager.Instance.SelectedUnit.UnitNumber == 2)
		{

		}

		//Move
		if (OccupiedUnit == null && UnitManager.Instance.SelectedUnit != null)
		{
			SetUnit(UnitManager.Instance.SelectedUnit);
			UnitManager.Instance.SetSelectedUnit(null);
			return;
		}
	}
}
