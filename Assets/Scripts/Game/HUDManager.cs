using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	public static HUDManager Instance;
	[SerializeField] private GameObject _SelectedUnitObject, _TileUnitObject, _GameStateInfoObject, _GameStateInfoObjectHolder;
	[SerializeField] private Text _SelectedUnitNumberText, _SelectedUnitNameText, _TileUnitNumberText, _TileUnitNameText;
	[SerializeField] private Color _RedArmy, _BlueArmy;
	[SerializeField] private Sprite _PositionateUnitesIconSprite, _MoveTurnIconSprite, _AttackTurnIconSprite;

	private void Awake()
	{
		Instance = this;
		_SelectedUnitObject.SetActive(false);
		_TileUnitObject.SetActive(false);
	}

	public void ShowSelectedUnit(Unit unit)
	{
		if (unit == null)
		{
			_SelectedUnitObject.SetActive(false);
			return;
		}

		_SelectedUnitObject.GetComponent<Image>().color = (unit.UnitColor == 0) ? _RedArmy : _BlueArmy;
		_SelectedUnitNumberText.text = unit.GetUnitNumberString();
		_SelectedUnitNameText.text = unit.UnitName;
		_SelectedUnitObject.SetActive(true);
	}

	public void ShowTileUnit(Tile tile)
	{
		if (tile == null)
		{
			_TileUnitObject.SetActive(false);
			return;
		}

		if (tile.OccupiedUnit)
		{
			_TileUnitObject.GetComponent<Image>().color = (tile.OccupiedUnit.UnitColor == 0) ? _RedArmy : _BlueArmy;
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
			_TileUnitObject.SetActive(true);
		}
	}

	public void UpdateTurnInfo()
	{
		_GameStateInfoObjectHolder.SetActive(true);
		switch (GameManager.Instance.GameState)
		{
			case GameState.PositionateUnits:
				_GameStateInfoObject.GetComponent<Image>().sprite = _PositionateUnitesIconSprite;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = (GameManager.Instance.PlayerSide == 0) ? _RedArmy : _BlueArmy;
				TranslateTurnInfoToSide();
				break;
			case GameState.RedMove:
				_GameStateInfoObject.GetComponent<Image>().sprite = _MoveTurnIconSprite;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = _RedArmy;
				TranslateTurnInfoToSide();
				break;
			case GameState.RedAttack:
				_GameStateInfoObject.GetComponent<Image>().sprite = _AttackTurnIconSprite;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = _RedArmy;
				TranslateTurnInfoToSide();
				break;
			case GameState.BlueMove:
				_GameStateInfoObject.GetComponent<Image>().sprite = _MoveTurnIconSprite;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = _BlueArmy;
				TranslateTurnInfoToSide();
				break;
			case GameState.BlueAttack:
				_GameStateInfoObject.GetComponent<Image>().sprite = _AttackTurnIconSprite;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = _BlueArmy;
				TranslateTurnInfoToSide();
				break;
			default:
				_GameStateInfoObject.GetComponent<Image>().sprite = null;
				_GameStateInfoObjectHolder.GetComponent<Image>().color = new Color(0, 0, 0, 50);
				_GameStateInfoObjectHolder.SetActive(false);
				break;
		}
	}

	private void TranslateTurnInfoToCenter()
	{
		RectTransform RectTransformComponent = _GameStateInfoObjectHolder.GetComponent<RectTransform>();
		RectTransformComponent.anchorMin = new Vector2(0.5f, 0f);
		RectTransformComponent.anchorMax = new Vector2(0.5f, 0f);
		RectTransformComponent.pivot = new Vector2(0.5f, 0f);
		RectTransformComponent.anchoredPosition = new Vector2(0f, 10f);
	}

	private void TranslateTurnInfoToSide()
	{
		RectTransform RectTransformComponent = _GameStateInfoObjectHolder.GetComponent<RectTransform>();

		if (GameManager.Instance.PlayerSide == GameManager.Instance.GetPlayerTurn() || GameManager.Instance.GameState == GameState.PositionateUnits)
		{
			RectTransformComponent.anchorMin = new Vector2(0f, 0f);
			RectTransformComponent.anchorMax = new Vector2(0f, 0f);
			RectTransformComponent.pivot = new Vector2(0f, 0f);
			RectTransformComponent.anchoredPosition = new Vector2(10f, 70f);
		}
		else
		{
			RectTransformComponent.anchorMin = new Vector2(1f, 0f);
			RectTransformComponent.anchorMax = new Vector2(1f, 0f);
			RectTransformComponent.pivot = new Vector2(1f, 0f);
			RectTransformComponent.anchoredPosition = new Vector2(-10f, 70f);
		}
	}

	public void SairJogo()
	{
		GestorDeRede.Instancia.SairDoLobby();
		Destroy(GestorDeRede.Instancia.gameObject);
		SceneManager.LoadScene("Menu");
	}
}