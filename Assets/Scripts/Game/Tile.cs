using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour
{
	[SerializeField] private GameObject _HighlightHover, _Highlight;
	[SerializeField] private Color _SelectedHighlightColor, _WalkableHighlightColor, _AttackableHightlightColor, _SwapHighlightColor;

	private Color _OriginalHighlightHoverColor;
	public Unit OccupiedUnit;

	public bool Walkable => OccupiedUnit == null;

	//Iterables var
	private int _i;
	private Tile _iTile;

	private void Start()
	{
		_OriginalHighlightHoverColor = _HighlightHover.GetComponent<SpriteRenderer>().color;
		this.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	void OnMouseEnter()
	{
		//Change highlight hover color
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
			_HighlightHover.GetComponent<SpriteRenderer>().color = new Color(_SwapHighlightColor.r, _SwapHighlightColor.g, _SwapHighlightColor.b, _OriginalHighlightHoverColor.a);
		else
			_HighlightHover.GetComponent<SpriteRenderer>().color = _OriginalHighlightHoverColor;

		_HighlightHover.SetActive(true);
		HUDManager.Instance.ShowTileUnit(this);
	}

	void OnMouseExit()
	{
		_HighlightHover.SetActive(false);
		HUDManager.Instance.ShowTileUnit(null);
	}

	public void SetUnit(Unit unit)
	{
		if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
		unit.transform.position = transform.position;
		OccupiedUnit = unit;
		unit.OccupiedTile = this;
	}
	public void SwapUnits(Tile tile1)
	{
		if (tile1.OccupiedUnit == null || OccupiedUnit == null)
			return;

		Unit temp = tile1.OccupiedUnit;
		tile1.OccupiedUnit = OccupiedUnit;
		OccupiedUnit = temp;

		tile1.OccupiedUnit.transform.position = tile1.transform.position;
		OccupiedUnit.transform.position = transform.position;

		tile1.OccupiedUnit.OccupiedTile = tile1;
		OccupiedUnit.OccupiedTile = this;
	}

	public void HightLightTileUpdate()
	{
		//If this tile is selected
		if (UnitManager.Instance.SelectedUnit != null && UnitManager.Instance.SelectedUnit.OccupiedTile == this)
		{
			//If it is in positionate turn
			if (GameManager.Instance.GameState == GameState.PositionateUnits)
				_Highlight.GetComponent<SpriteRenderer>().color = _SwapHighlightColor;
			else
				_Highlight.GetComponent<SpriteRenderer>().color = _SelectedHighlightColor;
			_Highlight.SetActive(true);
			return;
		}

		//If is in positionate turn, ignore it
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
		{
			_Highlight.SetActive(false);
			return;
		}

		if (CanSelectedUnitMoveToThisTile())
		{
			_Highlight.GetComponent<SpriteRenderer>().color = _WalkableHighlightColor;
			_Highlight.SetActive(true);
			return;
		}

		if (CanSelectedUnitAttackThisTile())
		{
			_Highlight.GetComponent<SpriteRenderer>().color = _AttackableHightlightColor;
			_Highlight.SetActive(true);
			return;
		}

		_Highlight.SetActive(false);
	}

	private bool CanSelectedUnitMoveToThisTile()
	{
		//No unit selected
		if (UnitManager.Instance.SelectedUnit == null)
			return false;

		//Can't move due selected unit number
		if (UnitManager.Instance.SelectedUnit.UnitNumber < 1 || UnitManager.Instance.SelectedUnit.UnitNumber > 10)
			return false;

		//Can't move due there is a unit here
		if (OccupiedUnit != null)
			return false;

		//Can't move due direction is not allowed
		if (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.x != this.transform.position.x
			&& UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.y != this.transform.position.y)
			return false;

		//If is not a Soldier and it is 1 tile away
		if (UnitManager.Instance.SelectedUnit.UnitNumber != 2
			&& Vector3.Distance(this.transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position) > 1)
			return false;

		//If there is a player on the way for soldiers
		if (UnitManager.Instance.SelectedUnit.UnitNumber == 2)
		{
			float UnitDistance = Vector2.Distance(this.transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position);
			Vector2 UnitDirection = (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - this.transform.position);
			UnitDirection.Normalize();

			for (_i = 1; _i < UnitDistance; _i++)
			{
				_iTile = GridManager.Instance.GetTileAtPosition((Vector2)this.transform.position - new Vector2(0.5f, 0.5f) + _i * UnitDirection);

				//If there is no tile on the way
				if (_iTile == null)
					return false;

				//If there is some unit on the way
				if (_iTile.OccupiedUnit != null)
					return false;
			}
		}

		return true;
	}
	private bool CanSelectedUnitAttackThisTile()
	{
		//No attacker selected
		if (UnitManager.Instance.SelectedUnit == null)
			return false;

		//If there is no unit on this tile
		if (OccupiedUnit == null)
			return false;

		//If is this tile is occupied by ally
		if (OccupiedUnit.UnitColor == GameManager.Instance.PlayerSide)
			return false;

		//Can't attack due selected unit number
		if (UnitManager.Instance.SelectedUnit.UnitNumber < 1 || UnitManager.Instance.SelectedUnit.UnitNumber > 10)
			return false;

		//Can't attack due direction is not allowed
		if (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.x != this.transform.position.x
			&& UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.y != this.transform.position.y)
			return false;

		//If is 1 tile away
		if (Vector3.Distance(this.transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position) > 1)
			return false;

		return true;
	}

	private void SelectOrMoveUnitOnMouseDown()
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

		//If can't move unit to this tile, ignore it
		if (!CanSelectedUnitMoveToThisTile())
			return;

		//Move
		if (OccupiedUnit == null && UnitManager.Instance.SelectedUnit != null)
		{
			GridManager.Instance.photonView.RPC("MoveFromTileToTile", RpcTarget.OthersBuffered,
				(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)this.transform.position - new Vector2(0.5f, 0.5f));
			SetUnit(UnitManager.Instance.SelectedUnit);
			UnitManager.Instance.SetSelectedUnit(null);
			return;
		}
	}

	private void SwapOrSelectUnitsOnMouseDown()
	{
		//Unselect
		if (OccupiedUnit != null && OccupiedUnit == UnitManager.Instance.SelectedUnit)
		{
			UnitManager.Instance.SetSelectedUnit(null);
			return;
		}

		//Select
		if (OccupiedUnit != null && OccupiedUnit.UnitColor == GameManager.Instance.PlayerSide && UnitManager.Instance.SelectedUnit == null)
		{
			UnitManager.Instance.SetSelectedUnit((Unit)OccupiedUnit);
			return;
		}

		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//Swap
		GridManager.Instance.photonView.RPC("SwapUnitsBetweenTiles", RpcTarget.OthersBuffered,
			(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)this.transform.position - new Vector2(0.5f, 0.5f));
		SwapUnits(UnitManager.Instance.SelectedUnit.OccupiedTile);
		UnitManager.Instance.SetSelectedUnit(null);
	}

	private void OnMouseDown()
	{
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
			SwapOrSelectUnitsOnMouseDown();
		else
			SelectOrMoveUnitOnMouseDown();
		GridManager.Instance.HightLightTileUpdateEveryTile();
	}
}
