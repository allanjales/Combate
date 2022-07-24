using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour
{
	[Header("Buttons link")]
	[SerializeField] private GameObject _HighlightHover;
	[SerializeField] private GameObject _HighlightPossibilities;
	[SerializeField] private GameObject _HighlightLastMove;

	[Header("Highlight colors")]
	[SerializeField] private Color _SelectedHighlightColor;
	[SerializeField] private Color _WalkableHighlightColor;
	[SerializeField] private Color _AttackableHightlightColor;
	[SerializeField] private Color _SwapHighlightColor;
	[SerializeField] private Color _LastMoveHighlightColor ;
	[SerializeField] private Color _LastAttackerHighlightColor ;
	[SerializeField] private Color _LastTargetHighlightColor ;

	private Color _OriginalHighlightHoverColor;
	public Unit OccupiedUnit;

	private readonly float _showTimeLastMoveHighlight = 5f;
	private float _ShowingSinceLastMoveHighlight = -1f;

	public bool Walkable => OccupiedUnit == null;

	//Iterables var
	private int _i;
	private Tile _iTile;

	private void Start()
	{
		_OriginalHighlightHoverColor = _HighlightHover.GetComponent<SpriteRenderer>().color;
		this.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	private void Update()
	{
		ShowHighlightLastMove();
	}

	private void ShowHighlightLastMove()
	{
		if (_ShowingSinceLastMoveHighlight == -1f)
			return;

		if (Time.time - _ShowingSinceLastMoveHighlight > _showTimeLastMoveHighlight)
		{
			_ShowingSinceLastMoveHighlight = -1f;
			_HighlightLastMove.SetActive(false);
		}
	}

	public void DisableLastMoveHighlight()
	{
		_ShowingSinceLastMoveHighlight = -1f;
		_HighlightLastMove.SetActive(false);
	}

	public void HighlightLastMove(int moveType)
	{
		_ShowingSinceLastMoveHighlight = Time.time;
		_HighlightLastMove.SetActive(true);
		switch (moveType)
		{
			case 0:
				_HighlightLastMove.GetComponent<SpriteRenderer>().color = _LastMoveHighlightColor;
				break;
			case 1:
				_HighlightLastMove.GetComponent<SpriteRenderer>().color = _LastAttackerHighlightColor;
				break;
			case 2:
				_HighlightLastMove.GetComponent<SpriteRenderer>().color = _LastTargetHighlightColor;
				break;
			default:
				break;
		}
	}

	void OnMouseEnter()
	{
		//Change highlight hover color
		if (GameManager.Instance.GameState == GameState.PositionateUnits && OccupiedUnit != null && OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy)
			_HighlightHover.GetComponent<SpriteRenderer>().color = new Color(_SwapHighlightColor.r, _SwapHighlightColor.g, _SwapHighlightColor.b, _OriginalHighlightHoverColor.a);
		else
			_HighlightHover.GetComponent<SpriteRenderer>().color = _OriginalHighlightHoverColor;

		_HighlightHover.SetActive(true);
		HUDManager.Instance.ShowTileUnit(this);
	}

	private void OnMouseOver()
	{
		if (Input.GetKey(KeyCode.Delete))
			GodMode.Instance.TryToDeleteUnit(OccupiedUnit);
	}

	void OnMouseExit()
	{
		_HighlightHover.SetActive(false);
		HUDManager.Instance.ShowTileUnit(null);
	}

	public void SetUnit(Unit unit)
	{
		if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
		unit.MoveTo(transform.position);
		OccupiedUnit = unit;
		unit.OccupiedTile = this;
	}

	public void HightLightTileUpdate()
	{
		//If this tile is selected
		if (UnitManager.Instance.SelectedUnit != null && UnitManager.Instance.SelectedUnit.OccupiedTile == this)
		{
			//If it is in positionate turn
			if (GameManager.Instance.GameState == GameState.PositionateUnits)
				_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _SwapHighlightColor;
			else
				_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _SelectedHighlightColor;
			_HighlightPossibilities.SetActive(true);
			return;
		}

		//If is in positionate turn, ignore it
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
		{
			_HighlightPossibilities.SetActive(false);
			return;
		}

		if (CanSelectedUnitMoveToThisTile() && !GameManager.Instance.IsMyAttackTurn())
		{
			_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _WalkableHighlightColor;
			_HighlightPossibilities.SetActive(true);
			return;
		}

		if (CanSelectedUnitAttackThisTile())
		{
			_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _AttackableHightlightColor;
			_HighlightPossibilities.SetActive(true);
			return;
		}

		_HighlightPossibilities.SetActive(false);
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
		if (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.x != transform.position.x
			&& UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.y != transform.position.y)
			return false;

		//If is not a Soldier and it is 1 tile away
		if (UnitManager.Instance.SelectedUnit.UnitNumber != 2
			&& Vector3.Distance(transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position) > 1)
			return false;

		//If there is a player on the way for soldiers
		if (UnitManager.Instance.SelectedUnit.UnitNumber == 2)
		{
			float UnitDistance = Vector2.Distance(transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position);
			Vector2 UnitDirection = (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - transform.position);
			UnitDirection.Normalize();

			for (_i = 1; _i < UnitDistance; _i++)
			{
				_iTile = GridManager.Instance.GetTileAtPosition((Vector2)transform.position - new Vector2(0.5f, 0.5f) + _i * UnitDirection);

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
	public bool CanSelectedUnitAttackThisTile()
	{
		//No attacker selected
		if (UnitManager.Instance.SelectedUnit == null)
			return false;

		//If there is no unit on this tile
		if (OccupiedUnit == null)
			return false;

		//If is this tile is occupied by ally
		if (OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy)
			return false;

		//Can't attack due selected unit number
		if (UnitManager.Instance.SelectedUnit.UnitNumber < 1 || UnitManager.Instance.SelectedUnit.UnitNumber > 10)
			return false;

		//Can't attack due direction is not allowed
		if (UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.x != transform.position.x
			&& UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position.y != transform.position.y)
			return false;

		//If is 1 tile away
		if (Vector3.Distance(transform.position, UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position) > 1)
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
		if (OccupiedUnit != null && OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy)
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

		//Attack
		if (CanSelectedUnitAttackThisTile())
		{
			GridManager.Instance.photonView.RPC("AttackTile", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
				(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)transform.position - new Vector2(0.5f, 0.5f));
			UnitManager.Instance.SetSelectedUnit(null);
			GameManager.Instance.NextTurn();
			GameManager.Instance.NextTurn();
			return;
		}

		//If can't move unit to this tile, ignore it
		if (!CanSelectedUnitMoveToThisTile())
			return;

		//Move
		if (OccupiedUnit == null && UnitManager.Instance.SelectedUnit != null)
		{
			Vector2 OldPos = UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position;
			UnitManager.Instance.SelectedUnit.OccupiedTile.HighlightLastMove(0);
			HighlightLastMove(0);
			GridManager.Instance.photonView.RPC("MoveFromTileToTile", RpcTarget.OthersBuffered,
				(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)transform.position - new Vector2(0.5f, 0.5f));
			SetUnit(UnitManager.Instance.SelectedUnit);
			GameManager.Instance.NextTurn();

			//If is a soldier and moved more than 1 tile
			if (UnitManager.Instance.SelectedUnit.UnitNumber == 2 &&
				Vector2.Distance(transform.position, OldPos) > 1)
			{
				GameManager.Instance.NextTurn();
				UnitManager.Instance.SetSelectedUnit(null);
				return;
			}

			//If can't attack anything
			if (!GridManager.Instance.CanSelectedUnitAttackAnyTile())
			{
				GameManager.Instance.NextTurn();
				UnitManager.Instance.SetSelectedUnit(null);
				return;
			}
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
		if (OccupiedUnit != null && OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy && UnitManager.Instance.SelectedUnit == null)
		{
			UnitManager.Instance.SetSelectedUnit((Unit)OccupiedUnit);
			return;
		}

		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//Swap
		GridManager.Instance.photonView.RPC("SwapUnitsBetweenTiles", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
			(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)transform.position - new Vector2(0.5f, 0.5f));
		UnitManager.Instance.SetSelectedUnit(null);
	}

	private void AttackOnMouseDown()
	{
		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//Attack
		if (CanSelectedUnitAttackThisTile())
		{
			GridManager.Instance.photonView.RPC("AttackTile", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
				(Vector2)UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position - new Vector2(0.5f, 0.5f), (Vector2)transform.position - new Vector2(0.5f, 0.5f));
			GameManager.Instance.NextTurn();
			UnitManager.Instance.SetSelectedUnit(null);
		}
	}

	private void OnMouseDown()
	{
		if (GameManager.Instance.IsMyAttackTurn())
			AttackOnMouseDown();
		else if (GameManager.Instance.GameState == GameState.PositionateUnits)
			SwapOrSelectUnitsOnMouseDown();
		else
			SelectOrMoveUnitOnMouseDown();

		GridManager.Instance.HightLightTileUpdateEveryTile();
	}
}
