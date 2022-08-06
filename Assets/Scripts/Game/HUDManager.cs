using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	public static HUDManager Instance;
	[Header("Army colors")]
	[SerializeField] private Color _RedArmyColor;
	[SerializeField] private Color _BlueArmyColor;

	[Header("Players Name")]
	[SerializeField] private Text _OwnNameText;
	[SerializeField] private Text _EnemyNameText;

	[Header("Unit Infos on Screen")]
	[SerializeField] private GameObject _SelectedUnitInfoObject;
	[SerializeField] private GameObject _TileUnitInfoObject;
	[SerializeField] private GameObject _EnemyUnitInfoOnAttack;
	[SerializeField] private GameObject _OwnUnitInfoOnAttack;

	[Header("Positionate Done Button")]
	[SerializeField] private Button _BtnDonePositionating;
	[SerializeField] private Image _CheckImageBtnReadyPositionate;

	[Header("Attack Done Button")]
	[SerializeField] private Button _BtnDoNotAttack;

	[Space(20)]
	[SerializeField] private Text _WinnerText;

	[Space(20)]
	[SerializeField] private List<GameObject> _ListGameStateOwnTurnInfo;
	[SerializeField] private List<GameObject> _ListGameStateEnemyTurnInfo;

	private readonly float _showTimeUnitInfoOnAttack = 5f;
	private float _ShowingSinceUnitInfoOnAttack = -1f;

	private void Awake()
	{
		Instance = this;
		_SelectedUnitInfoObject.SetActive(false);
		_TileUnitInfoObject.SetActive(false);
		_EnemyUnitInfoOnAttack.SetActive(false);
		_OwnUnitInfoOnAttack.SetActive(false);
		_WinnerText.gameObject.SetActive(false);

		_OwnNameText.text = GameManager.Instance.GetMyNickName();
		_EnemyNameText.text = GameManager.Instance.GetEnemyNickName();
	}

	private void Update()
	{
		MoveTileUnitInfoToCursorPosition();
		DrawUnitsInfoOnAttack();
	}

	private void DrawUnitsInfoOnAttack()
	{
		if (_ShowingSinceUnitInfoOnAttack == -1f)
			return;

		if (Time.time - _ShowingSinceUnitInfoOnAttack > _showTimeUnitInfoOnAttack)
		{
			_ShowingSinceUnitInfoOnAttack = -1f;
			_EnemyUnitInfoOnAttack.SetActive(false);
			_OwnUnitInfoOnAttack.SetActive(false);
		}
	}

	public void DrawSelectedUnitInfo(Unit unit)
	{
		if (unit == null)
		{
			_SelectedUnitInfoObject.SetActive(false);
			return;
		}

		_SelectedUnitInfoObject.GetComponent<Image>().color = (unit.UnitArmy == 0) ? _RedArmyColor : _BlueArmyColor;
		_SelectedUnitInfoObject.transform.Find("UnitNumber").GetComponent<Text>().text = unit.GetUnitNumberString();
		_SelectedUnitInfoObject.transform.Find("UnitName").GetComponent<Text>().text = unit.UnitName;
		_SelectedUnitInfoObject.SetActive(true);
	}

	public void DrawTileUnitInfo(Tile tile)
	{
		if (tile == null)
		{
			_TileUnitInfoObject.SetActive(false);
			return;
		}

		if (tile.OccupiedUnit)
		{
			_TileUnitInfoObject.GetComponent<Image>().color = (tile.OccupiedUnit.UnitArmy == 0) ? _RedArmyColor : _BlueArmyColor;
			if (tile.OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy || GodMode.Instance.IsGodEyeActive() || GameManager.Instance.IsGameFinished())
			{
				_TileUnitInfoObject.transform.Find("UnitNumber").GetComponent<Text>().text = tile.OccupiedUnit.GetUnitNumberString();
				_TileUnitInfoObject.transform.Find("UnitName").GetComponent<Text>().text = tile.OccupiedUnit.UnitName;
			}
			else
			{
				_TileUnitInfoObject.transform.Find("UnitNumber").GetComponent<Text>().text = "?";
				_TileUnitInfoObject.transform.Find("UnitName").GetComponent<Text>().text = "?????";
			}
			_TileUnitInfoObject.SetActive(true);
		}
	}

	public void ShowUnitInfoOnAttack(Unit Attacker, Unit Target)
	{
		Unit Enemy = GameManager.Instance.IsMyArmy(Target.UnitArmy) ? Attacker : Target;
		Unit Ally = GameManager.Instance.IsMyArmy(Attacker.UnitArmy) ? Attacker : Target;

		Dictionary<Unit, bool> Survivors = GridManager.Instance.WhoSurvivesOnAttack(Attacker, Target);

		_ShowingSinceUnitInfoOnAttack = Time.time;
		_EnemyUnitInfoOnAttack.SetActive(true);
		_EnemyUnitInfoOnAttack.GetComponent<Image>().color = ((Enemy.UnitArmy == 0) ? _RedArmyColor : _BlueArmyColor)
			* (Survivors[Enemy] ? new Color(1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f));
		_EnemyUnitInfoOnAttack.transform.Find("UnitNumber").GetComponent<Text>().text = Enemy.GetUnitNumberString();
		_EnemyUnitInfoOnAttack.transform.Find("UnitName").GetComponent<Text>().text = Enemy.UnitName;

		_OwnUnitInfoOnAttack.SetActive(true);
		_OwnUnitInfoOnAttack.GetComponent<Image>().color = ((Ally.UnitArmy == 0) ? _RedArmyColor : _BlueArmyColor)
			* (Survivors[Ally] ? new Color(1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f));
		_OwnUnitInfoOnAttack.transform.Find("UnitNumber").GetComponent<Text>().text = Ally.GetUnitNumberString();
		_OwnUnitInfoOnAttack.transform.Find("UnitName").GetComponent<Text>().text = Ally.UnitName;
	}

	private void MoveTileUnitInfoToCursorPosition()
	{
		_TileUnitInfoObject.transform.position = new Vector3(Input.mousePosition.x + 10f, Input.mousePosition.y - 10f);
	}

	public void UpdateButtonsShow()
	{
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
			_BtnDonePositionating.gameObject.SetActive(true);
		else
			_BtnDonePositionating.gameObject.SetActive(false);

		if (GameManager.Instance.IsMyAttackTurn())
			_BtnDoNotAttack.gameObject.SetActive(true);
		else
			_BtnDoNotAttack.gameObject.SetActive(false);

		UpdateBtnDonePositioningAppearence();
		UpdateBtnDoNotAttackAppearence();
	}

	private void UpdateBtnDoNotAttackAppearence()
	{
		_BtnDoNotAttack.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? _RedArmyColor : _BlueArmyColor;
	}

	public void UpdateBtnDonePositioningAppearence()
	{
		_CheckImageBtnReadyPositionate.gameObject.SetActive(GameManager.Instance.PositionateUnitsDoneState);
		_CheckImageBtnReadyPositionate.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? _RedArmyColor : _BlueArmyColor;
		_BtnDonePositionating.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? _RedArmyColor : _BlueArmyColor;

	}

	public void UpdatePositionatingDoneInfo()
	{
		_ListGameStateOwnTurnInfo[3].SetActive(false);
		_ListGameStateEnemyTurnInfo[3].SetActive(false);
		if (GameManager.Instance.IsOtherPlayerDonePositioningUnits())
			_ListGameStateEnemyTurnInfo[3].SetActive(true);
		if (GameManager.Instance.PositionateUnitsDoneState == true)
			_ListGameStateOwnTurnInfo[3].SetActive(true);
	}

	private void HideEveryGameStateTurnInfo()
	{
		foreach (GameObject GameStateTurnInfo in _ListGameStateOwnTurnInfo)
			GameStateTurnInfo.SetActive(false);

		foreach (GameObject GameStateTurnInfo in _ListGameStateEnemyTurnInfo)
			GameStateTurnInfo.SetActive(false);
	}

	private void SetColors()
	{
		foreach (GameObject GameStateTurnInfo in _ListGameStateOwnTurnInfo)
			GameStateTurnInfo.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? _RedArmyColor : _BlueArmyColor;

		foreach (GameObject GameStateTurnInfo in _ListGameStateEnemyTurnInfo)
			GameStateTurnInfo.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? _BlueArmyColor : _RedArmyColor;
	}

	private List<GameObject> GetListGameStateTurnInfoByArmy(int army)
	{
		return (GameManager.Instance.playerArmy == army) ? _ListGameStateOwnTurnInfo : _ListGameStateEnemyTurnInfo;
	}

	public void UpdateTurnInfo()
	{
		HideEveryGameStateTurnInfo();
		SetColors();

		switch (GameManager.Instance.GameState)
		{
			case GameState.PositionateUnits:
				_ListGameStateOwnTurnInfo[0].SetActive(true);
				_ListGameStateEnemyTurnInfo[0].SetActive(true);
				break;
			case GameState.RedMove:
				GetListGameStateTurnInfoByArmy(0)[1].SetActive(true);
				GetListGameStateTurnInfoByArmy(0)[2].SetActive(true);
				break;
			case GameState.RedAttack:
				GetListGameStateTurnInfoByArmy(0)[2].SetActive(true);
				break;
			case GameState.BlueMove:
				GetListGameStateTurnInfoByArmy(1)[1].SetActive(true);
				GetListGameStateTurnInfoByArmy(1)[2].SetActive(true);
				break;
			case GameState.BlueAttack:
				GetListGameStateTurnInfoByArmy(1)[2].SetActive(true);
				break;
			default:
				break;
		}
	}

	public void SetWinnerOnText(int winner)
	{
		_WinnerText.gameObject.SetActive(true);
		if (winner == 2)
			return;

		string playerName = GameManager.Instance.GetArmyOwnerNickName(winner);
		string colorName = (winner == 0) ? "vermelho" : "azul";
		string colorHex = ColorUtility.ToHtmlStringRGB((winner == 0) ? _RedArmyColor : _BlueArmyColor);

		_WinnerText.GetComponent<Text>().text = $"<color=#{colorHex}>{colorName}</color> (<color=#{colorHex}>{playerName}</color>) é o vencedor";
	}
}