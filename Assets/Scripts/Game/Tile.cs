using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
	[Header("Buttons link")]
	[SerializeField] private GameObject _HighlightHover;
	[SerializeField] private GameObject _HighlightPossibilities;
	[SerializeField] private GameObject _HighlightLastAttack;
	[SerializeField] private GameObject _HighlightLastMove;

	[Header("Highlight colors")]
	[SerializeField] private Color _SelectedHighlightColor;
	[SerializeField] private Color _WalkableHighlightColor;
	[SerializeField] private Color _AttackableHightlightColor;
	[SerializeField] private Color _SwapHighlightColor;
	[SerializeField] private Color _LastAttackerHighlightColor;
	[SerializeField] private Color _LastTargetHighlightColor;

	private Color _OriginalHighlightHoverColor;
	public Unit OccupiedUnit;

	public bool Walkable => OccupiedUnit == null;

	//Iterables var
	private int _i;
	private Tile _iTile;

	private void Start()
	{
		_OriginalHighlightHoverColor = _HighlightHover.GetComponent<SpriteRenderer>().color;
		GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	void OnMouseEnter()
	{
		if (!Application.isFocused)
			return;

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (Input.touchCount > 0)
			return;

		Highlight();
	}

	public void Highlight()
	{
		if (GameManager.Instance.GameState == GameState.PositionateUnits && OccupiedUnit != null && OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy)
			_HighlightHover.GetComponent<SpriteRenderer>().color = new Color(_SwapHighlightColor.r, _SwapHighlightColor.g, _SwapHighlightColor.b, _OriginalHighlightHoverColor.a);
		else
			_HighlightHover.GetComponent<SpriteRenderer>().color = _OriginalHighlightHoverColor;

		_HighlightHover.SetActive(true);
		HUDManager.Instance.DrawTileUnitInfo(this);
	}

	public void Unhighlight()
	{
		_HighlightHover.SetActive(false);
		HUDManager.Instance.DrawTileUnitInfo(null);
	}


	private void OnMouseOver()
	{
		if (!Application.isFocused)
			return;

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (Input.GetKey(KeyCode.Delete))
			GodMode.Instance.TryToDeleteUnit(OccupiedUnit);
	}

	void OnMouseExit()
	{
		Unhighlight();
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
		/*
		 * Last Move
		 */

		_HighlightLastMove.SetActive(false);
		//_HighlightLastMove.transform.rotation = Quaternion.Euler(0,0,0);
		_HighlightLastAttack.SetActive(false);
		if (GridManager.Instance.lastActionTiles.ContainsKey(this))
		{
			switch (GridManager.Instance.lastActionTiles[this])
			{
				case LastAction.MoveFrom:
					_HighlightLastMove.SetActive(true);
					Tile TileTo = GridManager.Instance.lastActionTiles.Where(t => t.Value == LastAction.MoveTo).First().Key;
					Vector2 VecDirection = TileTo.transform.position - transform.position;
					VecDirection.Normalize();
					_HighlightLastMove.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(VecDirection.y, VecDirection.x) + 90); ;
					break;
				case LastAction.AttackFrom:
					_HighlightLastAttack.SetActive(true);
					_HighlightLastAttack.GetComponent<SpriteRenderer>().color = _LastAttackerHighlightColor;
					break;
				case LastAction.AttackTo:
					_HighlightLastAttack.SetActive(true);
					_HighlightLastAttack.GetComponent<SpriteRenderer>().color = _LastTargetHighlightColor;
					break;
				default:
					break;
			}
		}

		/*
		 * Selection
		 */

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

		/*
		 * Possibilities of Movement or Attack
		 */

		//If is in positionate turn, ignore it
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
		{
			_HighlightPossibilities.SetActive(false);
			return;
		}

		//If can move and is not my attack turn
		if (CanUnitMoveToThisTile(UnitManager.Instance.SelectedUnit) && !GameManager.Instance.IsMyAttackTurn())
		{
			_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _WalkableHighlightColor;
			_HighlightPossibilities.SetActive(true);
			return;
		}

		//If can attack
		if (CanUnitAttackThisTile(UnitManager.Instance.SelectedUnit))
		{
			_HighlightPossibilities.GetComponent<SpriteRenderer>().color = _AttackableHightlightColor;
			_HighlightPossibilities.SetActive(true);
			return;
		}

		_HighlightPossibilities.SetActive(false);
	}

	public bool CanUnitMoveToThisTile(Unit unit)
	{
		//No unit selected
		if (unit == null)
			return false;

		//Can't move due selected unit number
		if (unit.UnitNumber < 1 || unit.UnitNumber > 10)
			return false;

		//Can't move due there is a unit here
		if (OccupiedUnit != null)
			return false;

		//Can't move due direction is not allowed
		if (unit.OccupiedTile.transform.position.x != transform.position.x
			&& unit.OccupiedTile.transform.position.y != transform.position.y)
			return false;

		//If is not a Soldier and it is 1 tile away
		if (unit.UnitNumber != 2
			&& Vector3.Distance(transform.position, unit.OccupiedTile.transform.position) > 1)
			return false;

		//If there is a player on the way for soldiers
		if (unit.UnitNumber == 2)
		{
			float UnitDistance = Vector2.Distance(transform.position, unit.OccupiedTile.transform.position);
			Vector2 UnitDirection = (unit.OccupiedTile.transform.position - transform.position);
			UnitDirection.Normalize();

			for (_i = 1; _i < UnitDistance; _i++)
			{
				_iTile = GridManager.Instance.GetTileAtPosition(GetPositionInGrid() + _i * UnitDirection);

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

	public bool CanUnitAttackThisTile(Unit unit)
	{
		//No attacker selected
		if (unit == null)
			return false;

		//If there is no unit on this tile
		if (OccupiedUnit == null)
			return false;

		//If is this tile is occupied by ally
		if (OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy)
			return false;

		//Can't attack due selected unit number
		if (unit.UnitNumber < 1 || unit.UnitNumber > 10)
			return false;

		//Can't attack due direction is not allowed
		if (unit.OccupiedTile.transform.position.x != transform.position.x
			&& unit.OccupiedTile.transform.position.y != transform.position.y)
			return false;

		//If is 1 tile away
		if (Vector3.Distance(transform.position, unit.OccupiedTile.transform.position) > 1)
			return false;

		return true;
	}

	private void MouseDownOnMoveTurn()
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
			UnitManager.Instance.SetSelectedUnit(OccupiedUnit);
			return;
		}

		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//If is not my turn, ignore it
		if (!GameManager.Instance.IsMyMoveTurn())
			return;

		//Attack
		if (CanUnitAttackThisTile(UnitManager.Instance.SelectedUnit))
		{
			GridManager.Instance.photonView.RPC("AttackTile", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
				UnitManager.Instance.SelectedUnit.OccupiedTile.GetPositionInGrid(), GetPositionInGrid());
			UnitManager.Instance.SetSelectedUnit(null);
			GameManager.Instance.NextTurn();
			GameManager.Instance.NextTurn();
			return;
		}

		//If can't move unit to this tile, ignore it
		if (!CanUnitMoveToThisTile(UnitManager.Instance.SelectedUnit))
			return;

		//Move
		if (OccupiedUnit == null && UnitManager.Instance.SelectedUnit != null)
		{
			Vector2 OldPos = UnitManager.Instance.SelectedUnit.OccupiedTile.transform.position;
			GridManager.Instance.photonView.RPC("MoveFromTileToTile", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
				UnitManager.Instance.SelectedUnit.OccupiedTile.GetPositionInGrid(), GetPositionInGrid());
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

	private void MouseDownOnPositioningTurn()
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
			UnitManager.Instance.SetSelectedUnit(OccupiedUnit);
			return;
		}

		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//If there is no unit on tile, ignore it
		if (OccupiedUnit == null)
			return;

		//If selected unit is a diferent army, ignore it
		if (UnitManager.Instance.SelectedUnit.UnitArmy != GameManager.Instance.playerArmy)
			return;

		//If occupied unit is a diferent army, ignore it
		if (OccupiedUnit.UnitArmy != GameManager.Instance.playerArmy)
			return;

		//Swap
		GridManager.Instance.photonView.RPC("SwapUnitsBetweenTiles", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
			UnitManager.Instance.SelectedUnit.OccupiedTile.GetPositionInGrid(), GetPositionInGrid());
		UnitManager.Instance.SetSelectedUnit(null);
	}

	private void MouseDownOnAttackTurn()
	{
		//If there is no unit selected, ignore it
		if (UnitManager.Instance.SelectedUnit == null)
			return;

		//Attack
		if (CanUnitAttackThisTile(UnitManager.Instance.SelectedUnit))
		{
			GridManager.Instance.photonView.RPC("AttackTile", RpcTarget.AllBuffered, GameManager.Instance.playerArmy,
				UnitManager.Instance.SelectedUnit.OccupiedTile.GetPositionInGrid(), GetPositionInGrid());
			GameManager.Instance.NextTurn();
			UnitManager.Instance.SetSelectedUnit(null);
		}
	}

	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (Input.touchCount > 0)
			return;

		Click();
	}

	public void Click()
	{
		if (GameManager.Instance.IsMyAttackTurn())
			MouseDownOnAttackTurn();
		else if (GameManager.Instance.GameState == GameState.PositionateUnits)
			MouseDownOnPositioningTurn();
		else
			MouseDownOnMoveTurn();

		GridManager.Instance.HightLightTileUpdateEveryTile();
	}

	private Vector2 GetPositionInGrid()
	{
		return GridManager.Instance.GetPositionOfTile(this);
	}
}
