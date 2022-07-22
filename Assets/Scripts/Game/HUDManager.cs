using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	public static HUDManager Instance;
	[SerializeField] private GameObject _SelectedUnitInfoObject, _AttackedUnitInfoObject, _TileUnitInfoObject;
	[SerializeField] private Text _SelectedUnitNumberText, _SelectedUnitNameText, _TileUnitNumberText, _TileUnitNameText;
	[SerializeField] public Color RedArmyColor, BlueArmyColor;

	private void Awake()
	{
		Instance = this;
		_SelectedUnitInfoObject.SetActive(false);
		_AttackedUnitInfoObject.SetActive(false);
		_TileUnitInfoObject.SetActive(false);
	}

	public void ShowSelectedUnit(Unit unit)
	{
		if (unit == null)
		{
			_SelectedUnitInfoObject.SetActive(false);
			return;
		}

		_SelectedUnitInfoObject.GetComponent<Image>().color = (unit.UnitColor == 0) ? RedArmyColor : BlueArmyColor;
		_SelectedUnitNumberText.text = unit.GetUnitNumberString();
		_SelectedUnitNameText.text = unit.UnitName;
		_SelectedUnitInfoObject.SetActive(true);
	}

	public void ShowTileUnit(Tile tile)
	{
		if (tile == null)
		{
			_TileUnitInfoObject.SetActive(false);
			return;
		}

		if (tile.OccupiedUnit)
		{
			_TileUnitInfoObject.GetComponent<Image>().color = (tile.OccupiedUnit.UnitColor == 0) ? RedArmyColor : BlueArmyColor;
			if (tile.OccupiedUnit.UnitColor == GameManager.Instance.PlayerSide || GameManager.Instance.GodEye)
			{
				_TileUnitNumberText.text = tile.OccupiedUnit.GetUnitNumberString();
				_TileUnitNameText.text = tile.OccupiedUnit.UnitName;
			}
			else
			{
				_TileUnitNumberText.text = "?";
				_TileUnitNameText.text = "?????";
			}
			_TileUnitInfoObject.SetActive(true);
		}
	}

	private void Update()
	{
		MoveTileUnitInfoToCursorPosition();
	}

	private void MoveTileUnitInfoToCursorPosition()
	{
		_TileUnitInfoObject.transform.position = new Vector3(Input.mousePosition.x + 10f, Input.mousePosition.y - 10f);
	}

	public void SairJogo()
	{
		GestorDeRede.Instancia.SairDoLobby();
		Destroy(GestorDeRede.Instancia.gameObject);
		SceneManager.LoadScene("Menu");
	}
}