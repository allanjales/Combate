using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;

public class GridManager : MonoBehaviourPunCallbacks
{
	public static GridManager Instance;

	[SerializeField] private Tile _tilePrefab;

	public Dictionary<Vector2, Tile> _tiles;

	private void Awake()
	{
		Instance = this;
	}

	public void GenerateGrid()
	{
		_tiles = new Dictionary<Vector2, Tile>();
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				if ((x == 2 || x == 3 || x == 6 || x == 7) && (y == 4 || y == 5))
					continue;

				this.SpawnTile(x, y);
			}
		}
	}

	private void SpawnTile(int x, int y)
	{
		Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x + .5f, y + .5f), Quaternion.identity);
		spawnedTile.name = $"Tile {x} {y}";
		spawnedTile.transform.SetParent(this.transform);

		_tiles[new Vector2(x, y)] = spawnedTile;
	}

	public Tile GetTileAtPosition(Vector2 pos)
	{
		if (_tiles.TryGetValue(pos, out var tile))
			return tile;

		return null;
	}

	public Vector2 GetPositionOfTile(Tile tile)
	{
		return _tiles.FirstOrDefault(x => x.Value == tile).Key;
	}

	public Tile GetSpawnTile(int side)
	{
		/*
		 * side = 0 -> bottom
		 * side = 1 -> top
		 */
		if (side == 0)
			return _tiles.Where(t => t.Key.y < 4 && t.Value.Walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
		else
			return _tiles.Where(t => t.Key.y > 5 && t.Value.Walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
	}

	public void HightLightTileUpdateEveryTile()
	{
		foreach (var tile in _tiles)
			tile.Value.HightLightTileUpdate();
	}

	public void DisableEveryLastMoveHighlight()
	{
		foreach (var tile in _tiles)
			tile.Value.DisableLastMoveHighlight();
	}

	public bool CanSelectedUnitAttackAnyTile()
	{
		foreach (var tile in _tiles)
		{
			if (tile.Value.CanSelectedUnitAttackThisTile() == true)
				return true;
		}

		return false;
	}

	[PunRPC]
	public void MoveFromTileToTile(Vector2 From, Vector2 To)
	{
		Tile TileFrom = GetTileAtPosition(new Vector2(9, 9) - From);
		Tile TileTo = GetTileAtPosition(new Vector2(9, 9) - To);
		TileTo.SetUnit(TileFrom.OccupiedUnit);
		TileFrom.HighlightLastMove(0);
		TileTo.HighlightLastMove(0);
	}

	[PunRPC]
	public void SwapUnitsBetweenTiles(int sender, Vector2 pos1, Vector2 pos2)
	{
		Tile tile1 = GetTileAtPosition((GameManager.Instance.playerArmy == sender) ? pos1 : (new Vector2(9, 9) - pos1));
		Tile tile2 = GetTileAtPosition((GameManager.Instance.playerArmy == sender) ? pos2 : (new Vector2(9, 9) - pos2));

		if (tile1.OccupiedUnit == null || tile2.OccupiedUnit == null)
			return;

		Unit temp = tile1.OccupiedUnit;
		tile1.OccupiedUnit = tile2.OccupiedUnit;
		tile2.OccupiedUnit = temp;

		tile1.OccupiedUnit.MoveTo(tile1.transform.position);
		tile2.OccupiedUnit.MoveTo(tile2.transform.position);

		tile1.OccupiedUnit.OccupiedTile = tile1;
		tile2.OccupiedUnit.OccupiedTile = tile2;
	}

	public Dictionary<Unit, bool> WhoSurvivesOnAttack(Unit AttackerUnit, Unit TargetUnit)
	{
		Dictionary<Unit, bool> Survivors = new();
		Survivors[AttackerUnit] = true;
		Survivors[TargetUnit] = true;

		if (TargetUnit.UnitNumber == 12)
		{
			Survivors[TargetUnit] = false;
			return Survivors;
		}

		if (TargetUnit.UnitNumber == 11)
		{
			Survivors[TargetUnit] = false;
			if (AttackerUnit.UnitNumber != 3)
				Survivors[AttackerUnit] = false;
			return Survivors;
		}

		if (AttackerUnit.UnitNumber == 1 && TargetUnit.UnitNumber == 10)
		{
			Survivors[TargetUnit] = false;
			return Survivors;
		}

		if (AttackerUnit.UnitNumber == TargetUnit.UnitNumber)
		{
			Survivors[AttackerUnit] = false;
			Survivors[TargetUnit] = false;
			return Survivors;
		}

		Survivors[(AttackerUnit.UnitNumber > TargetUnit.UnitNumber) ? TargetUnit : AttackerUnit] = false;
		return Survivors;
	}

	[PunRPC]
	public void AttackTile(int sender, Vector2 AttackerPos, Vector2 TargetPos)
	{
		Unit AttackerUnit = GetTileAtPosition((GameManager.Instance.playerArmy == sender) ? AttackerPos : (new Vector2(9, 9) - AttackerPos)).OccupiedUnit;
		Unit TargetUnit = GetTileAtPosition((GameManager.Instance.playerArmy == sender) ? TargetPos : (new Vector2(9, 9) - TargetPos)).OccupiedUnit;

		HUDManager.Instance.ShowUnitInfoOnAttack(AttackerUnit, TargetUnit);

		AttackerUnit.OccupiedTile.HighlightLastMove(1);
		TargetUnit.OccupiedTile.HighlightLastMove(2);

		if (TargetUnit.UnitNumber == 12)
		{
			GameManager.Instance.FinishGame(AttackerUnit.UnitArmy);
			return;
		}

		Dictionary<Unit, bool> Survivors = WhoSurvivesOnAttack(AttackerUnit, TargetUnit);
		if (Survivors[AttackerUnit] == false) AttackerUnit.DeleteItself();
		if (Survivors[TargetUnit] == false) TargetUnit.DeleteItself();
	}

	public Vector2 GetPosInMyTable(int senderArmy, Vector2 pos)
	{
		return (GameManager.Instance.playerArmy == senderArmy) ? pos : (new Vector2(9, 9) - pos);
	}
}