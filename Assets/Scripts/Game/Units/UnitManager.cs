using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	public static UnitManager Instance;
	private int[] num_units = { 0, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6, 1 };
	public string[] unit_names =
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
	[SerializeField] private Unit _unitPrefab;

	public Unit SelectedUnit;

	private void Awake()
	{
		Instance = this;
	}

	public void SpawnRedUnits()
	{
		SpawnUnits(0);
	}

	public void SpawnBlueUnits()
	{
		SpawnUnits(1);
	}

	private void SpawnUnits(int exercito)
	{
		for (int unit = 0; unit < num_units.Length; unit++)
		{
			for (int i = 0; i < num_units[unit]; i++)
			{
				var spawnTile = GridManager.Instance.GetSpawnTile((exercito == GameManager.Instance.PlayerSide) ? 0 : 1);
				this.SpawnUnit(exercito, unit, GridManager.Instance.GetPositionOfTile(spawnTile));
			}
		}
	}

	private void SpawnUnit(int exercito, int unit, Vector2 pos)
	{
		Tile spawnTile = GridManager.Instance.GetTileAtPosition(pos);
		var spawnedUnit = Instantiate(_unitPrefab, new Vector3(0, 0), Quaternion.identity);
		spawnedUnit.SetUnit(exercito, unit, unit_names[unit]);
		spawnedUnit.name = $"Unit{((exercito == 0) ? "Red" : "Blue")}:{unit_names[unit]}";
		spawnedUnit.transform.SetParent(this.transform);

		spawnTile.SetUnit(spawnedUnit);
	}

	public void SetSelectedUnit(Unit unit)
	{
		SelectedUnit = unit;
		HUDManager.Instance.ShowSelectedUnit(unit);
	}
}