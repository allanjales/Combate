using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviourPunCallbacks
{
	public static GridManager Instance;

	[SerializeField] private Tile _tilePrefab;

	public Dictionary<Vector2, Tile> _tiles = new(92);
	public Dictionary<Tile, LastAction> lastActionTiles = new(2);

	public Tile TouchBeganTile = null;

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

				SpawnTile(x, y);
			}
		}
	}

	private void SpawnTile(int x, int y)
	{
		Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x + .5f, y + .5f), Quaternion.identity);
		spawnedTile.name = $"Tile {x} {y}";
		spawnedTile.transform.SetParent(transform);

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
			return _tiles.Where(t => t.Key.y < 4 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
		else
			return _tiles.Where(t => t.Key.y > 5 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
	}

	public void HightLightTileUpdateEveryTile()
	{
		foreach (var tile in _tiles)
			tile.Value.HightLightTileUpdate();
	}

	public bool CanSelectedUnitAttackAnyTile()
	{
		foreach (var tile in _tiles)
		{
			if (tile.Value.CanUnitAttackThisTile(UnitManager.Instance.SelectedUnit) == true)
				return true;
		}

		return false;
	}

	[PunRPC]
	public void MoveFromTileToTile(int sender, Vector2 From, Vector2 To)
	{
		Tile TileFrom = GetTileAtPosition(GetPosInMyTable(sender, From));
		Tile TileTo = GetTileAtPosition(GetPosInMyTable(sender, To));

		lastActionTiles.Clear();
		lastActionTiles.Add(TileFrom, LastAction.MoveFrom);
		lastActionTiles.Add(TileTo, LastAction.MoveTo);
		HightLightTileUpdateEveryTile();

		TileTo.SetUnit(TileFrom.OccupiedUnit);
		AudioManager.Instance.PlayUnitMoveSound();
	}

	[PunRPC]
	public void SwapUnitsBetweenTiles(int sender, Vector2 pos1, Vector2 pos2)
	{
		Tile tile1 = GetTileAtPosition(GetPosInMyTable(sender, pos1));
		Tile tile2 = GetTileAtPosition(GetPosInMyTable(sender, pos2));

		if (tile1.OccupiedUnit == null || tile2.OccupiedUnit == null)
			return;

		(tile1.OccupiedUnit, tile2.OccupiedUnit) = (tile2.OccupiedUnit, tile1.OccupiedUnit);

		tile1.OccupiedUnit.MoveTo(tile1.transform.position);
		tile2.OccupiedUnit.MoveTo(tile2.transform.position);

		tile1.OccupiedUnit.OccupiedTile = tile1;
		tile2.OccupiedUnit.OccupiedTile = tile2;
		AudioManager.Instance.PlayUnitsSwapSound();
	}

	public Dictionary<Unit, bool> WhoSurvivesOnAttack(Unit AttackerUnit, Unit TargetUnit)
	{
		Dictionary<Unit, bool> Survivors = new(2)
		{
			[AttackerUnit] = true,
			[TargetUnit] = true
		};

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
		Unit AttackerUnit = GetTileAtPosition(GetPosInMyTable(sender, AttackerPos)).OccupiedUnit;
		Unit TargetUnit = GetTileAtPosition(GetPosInMyTable(sender, TargetPos)).OccupiedUnit;

		HUDManager.Instance.ShowUnitInfoOnAttack(AttackerUnit, TargetUnit);

		lastActionTiles.Clear();
		lastActionTiles.Add(AttackerUnit.OccupiedTile, LastAction.AttackFrom);
		lastActionTiles.Add(TargetUnit.OccupiedTile, LastAction.AttackTo);
		HightLightTileUpdateEveryTile();


		if (TargetUnit.UnitNumber == 12)
		{
			GameManager.Instance.FinishGame(new GameOverInfo(AttackerUnit.UnitArmy, GameOverReason.CaptureFlag, AttackerUnit));
			return;
		}

		Dictionary<Unit, bool> Survivors = WhoSurvivesOnAttack(AttackerUnit, TargetUnit);
		if (Survivors[AttackerUnit] == false) AttackerUnit.DeleteItself();
		if (Survivors[TargetUnit] == false) TargetUnit.DeleteItself();
		AudioManager.Instance.PlayUnitKillSound();
	}

	public Vector2 GetPosInMyTable(int senderArmy, Vector2 pos)
	{
		return (GameManager.Instance.playerArmy == senderArmy) ? pos : (new Vector2(9, 9) - pos);
	}
}

public enum LastAction
{
	MoveFrom = 0,
	MoveTo = 1,
	AttackFrom = 2,
	AttackTo = 3
}