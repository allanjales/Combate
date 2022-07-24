using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GodMode : MonoBehaviourPunCallbacks
{
	public static GodMode Instance;

	[Header("God Mode info Text")]
	[SerializeField] private Text _godModeInfoText;

	[Header("God Mode")]
	[SerializeField] private bool _isGodModeActive = false;
	[SerializeField] private bool _godEye = false;
	[SerializeField] private bool _seeEnemyUnits = false;
	[SerializeField] private bool _canDeleteUnits = false;

	private void Awake()
	{
		Instance = this;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			_isGodModeActive = !_isGodModeActive;
			UpdateGodModeInfoText();
		}

		if (!_isGodModeActive)
			return;

		if (Input.GetKeyDown(KeyCode.N))
		{
			if ((int)GameManager.Instance.GameState < 2)
				return;

			GameManager.Instance.NextTurn();
			UpdateGodModeInfoText();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			_godEye = !_godEye;
			UpdateGodModeInfoText();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			_seeEnemyUnits = !_seeEnemyUnits;
			UnitManager.Instance.UpdateEveryUnitSpriteRenderer();
			UpdateGodModeInfoText();
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			_canDeleteUnits = !_canDeleteUnits;
			UpdateGodModeInfoText();
		}
	}

	public void UpdateGodModeInfoText()
	{
		if (!_isGodModeActive)
		{
			_godModeInfoText.gameObject.SetActive(false);
			return;
		}
		_godModeInfoText.gameObject.SetActive(true);

		string text = "God Mode Active [Ctrl+Shift+G]";
		text += "\nNext Turn [N]";
		text += "\nGod Eye [E]: " + _godEye;
		text += "\nSee Enemy [S]: " + _seeEnemyUnits;
		text += "\nAllow Delete [A]: " + _canDeleteUnits;

		_godModeInfoText.text = text;
	}

	public bool IsGodEyeActive()
	{
		return _isGodModeActive && _godEye;
	}

	public bool CanSeeEnemyUnits()
	{
		return _isGodModeActive && _seeEnemyUnits;
	}

	public bool CanDeleteUnits()
	{
		return _isGodModeActive && _canDeleteUnits;
	}

	public void TryToDeleteUnit(Unit Unit)
	{
		if (Unit == null)
			return;

		if (CanDeleteUnits())
			photonView.RPC("DeleteUnit", RpcTarget.AllBuffered, GameManager.Instance.playerArmy, (Vector2)Unit.OccupiedTile.transform.position);
	}

	[PunRPC]
	public void DeleteUnit(int sender, Vector2 pos)
	{
		GridManager.Instance.GetTileAtPosition(GridManager.Instance.GetPosInMyTable(sender, pos - new Vector2(.5f, .5f))).OccupiedUnit.DeleteItself();
	}
}
