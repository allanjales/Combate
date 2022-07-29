using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviourPunCallbacks
{
	public static UnitManager Instance;

	[SerializeField] private Unit _unitPrefab;

	private readonly int[] num_units = { 0, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6, 1 };
	public readonly string[] unit_names =
	{
		"Vazio",
		"Espião",
		"Soldado",
		"Cabo-Armeiro",
		"Sargento",
		"Tenente",
		"Capitão",
		"Major",
		"Coronel",
		"General",
		"Marechal",
		"Bomba",
		"Bandeira"
	};

	public Unit SelectedUnit { get; private set; }
	private List<Unit> _UnitList = new();

	private void Awake()
	{
		Instance = this;
	}

	public void SpawnOwnUnits()
	{
		SpawnUnits(GameManager.Instance.playerArmy);
	}

	public void SpawnRedUnits()
	{
		SpawnUnits(0);
	}

	public void SpawnBlueUnits()
	{
		SpawnUnits(1);
	}

	private void SpawnUnits(int army)
	{
		for (int unit = 0; unit < num_units.Length; unit++)
		{
			for (int i = 0; i < num_units[unit]; i++)
			{
				var spawnTile = GridManager.Instance.GetSpawnTile((GameManager.Instance.IsMyArmy(army)) ? 0 : 1);
				photonView.RPC("SpawnUnit", RpcTarget.AllBuffered, army, unit, GridManager.Instance.GetPositionOfTile(spawnTile));
			}
		}
	}

	[PunRPC]
	private void SpawnUnit(int army, int unit, Vector2 pos)
	{
		Tile spawnTile = GridManager.Instance.GetTileAtPosition(GridManager.Instance.GetPosInMyTable(army, pos));
		var spawnedUnit = Instantiate(_unitPrefab, GridManager.Instance.GetPosInMyTable(army, new Vector3(5.5f, 0f)), Quaternion.identity); ;
		spawnedUnit.SetUnit(army, unit, unit_names[unit]);
		spawnedUnit.name = $"Unit{((army == 0) ? "Red" : "Blue")}:{unit_names[unit]}";
		spawnedUnit.transform.SetParent(this.transform);
		_UnitList.Add(spawnedUnit);

		spawnTile.SetUnit(spawnedUnit);
		AudioManager.Instance.PlayUnitsDistributionSound();
	}

	public void SetSelectedUnit(Unit unit)
	{
		if (GameManager.Instance.IsMyAttackTurn())
			return;

		SelectedUnit = unit;
		HUDManager.Instance.DrawSelectedUnitInfo(unit);
	}

	public void UpdateEveryUnitSpriteRenderer()
	{
		foreach (Unit unit in _UnitList)
			unit.UpdateSpriteRenderer();
	}

	public void RemoveUnitFromUnitList(Unit Unit)
	{
		_UnitList.Remove(Unit);
	}

	public bool ArmyHasUnitsToMove(int army)
	{
		foreach (Unit unit in _UnitList)
			if (unit.UnitArmy == army && unit.UnitNumber > 0 && unit.UnitNumber < 11 && unit.CanItMove())
				return true;

		return false;
	}

	public bool ArmyHasFlag(int army)
	{
		foreach (Unit unit in _UnitList)
			if (unit.UnitArmy == army && unit.UnitNumber == 12)
				return true;
		return false;
	}
}