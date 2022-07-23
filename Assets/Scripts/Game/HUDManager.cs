using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	public static HUDManager Instance;
	[SerializeField] public Color RedArmyColor, BlueArmyColor;
	[SerializeField] private GameObject _SelectedUnitInfoObject, _AttackedUnitInfoObject, _TileUnitInfoObject;
	[SerializeField] private Text _SelectedUnitNumberText, _SelectedUnitNameText, _TileUnitNumberText, _TileUnitNameText, _AttackedUnitNumberText, _AttackedUnitNameText;
	[SerializeField] private Button _BtnReadyPositionate;
	[SerializeField] private Image _CheckImageBtnReadyPositionate;

	private readonly float _attackedUnitInfoShowTime = 5f;
	private float _AttackedUnitInfoShowingSince = -1f;

	private bool _positionateUnitsDoneState = false;

	private void Awake()
	{
		Instance = this;
		_SelectedUnitInfoObject.SetActive(false);
		_AttackedUnitInfoObject.SetActive(false);
		_TileUnitInfoObject.SetActive(false);
	}

	private void Update()
	{
		MoveTileUnitInfoToCursorPosition();
		TryToShowAttackedUnitInfo();
	}

	private void TryToShowAttackedUnitInfo()
	{
		if (_AttackedUnitInfoShowingSince == -1f)
			return;

		if (Time.time - _AttackedUnitInfoShowingSince > _attackedUnitInfoShowTime)
		{
			_AttackedUnitInfoShowingSince = -1f;
			_AttackedUnitInfoObject.SetActive(false);
		}
	}

	public void ShowSelectedUnit(Unit unit)
	{
		if (unit == null)
		{
			_SelectedUnitInfoObject.SetActive(false);
			return;
		}

		_SelectedUnitInfoObject.GetComponent<Image>().color = (unit.UnitArmy == 0) ? RedArmyColor : BlueArmyColor;
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
			_TileUnitInfoObject.GetComponent<Image>().color = (tile.OccupiedUnit.UnitArmy == 0) ? RedArmyColor : BlueArmyColor;
			if (tile.OccupiedUnit.UnitArmy == GameManager.Instance.playerArmy || GameManager.Instance.GodEye || GameManager.Instance.IsGameFinished())
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

	public void ShowAttackedUnitInfo(Unit unit)
	{
		_AttackedUnitInfoShowingSince = Time.time;
		_AttackedUnitInfoObject.SetActive(true);
		_AttackedUnitInfoObject.GetComponent<Image>().color = (unit.UnitArmy == 0) ? RedArmyColor : BlueArmyColor;
		_AttackedUnitNumberText.text = unit.GetUnitNumberString();
		_AttackedUnitNameText.text = unit.UnitName;
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

	public void ReadyStateSwitch()
	{
		_positionateUnitsDoneState = !_positionateUnitsDoneState;

		if (_positionateUnitsDoneState)
			GameManager.Instance.photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.RedMove, false);
		else
			GameManager.Instance.photonView.RPC("CancelReadyForChangeState", RpcTarget.AllBuffered);
		UpdateButtonAppearence();
		Debug.Log("Hm");
	}

	public void UpdateButtonShow()
	{
		if (GameManager.Instance.GameState == GameState.PositionateUnits)
			_BtnReadyPositionate.gameObject.SetActive(true);
		else
			_BtnReadyPositionate.gameObject.SetActive(false);
		_positionateUnitsDoneState = false;
		UpdateButtonAppearence();
	}

	private void UpdateButtonAppearence()
	{
		_CheckImageBtnReadyPositionate.gameObject.SetActive(_positionateUnitsDoneState);
		_CheckImageBtnReadyPositionate.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? RedArmyColor : BlueArmyColor;
		_BtnReadyPositionate.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? RedArmyColor : BlueArmyColor;
	}
}