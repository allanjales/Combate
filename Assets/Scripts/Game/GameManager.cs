using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager Instance;
	public GameState GameState;

	[SerializeField] private GameObject _table;

	public int playerArmy;
	private int _readyToChangeState;
	public bool positionateUnitsDoneState { get; private set; } = false;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			SetPlayerArmy(Random.Range(0, 2));
			if (PhotonNetwork.PlayerList.Length > 1)
				photonView.RPC("SetPlayerArmy", PhotonNetwork.PlayerList[1], (playerArmy == 0) ? 1 : 0);
		}
	}

	[PunRPC]
	private void SetPlayerArmy(int army)
	{
		playerArmy = army;
		_table.transform.Rotate(0f, 0f, (float)playerArmy * 180f, Space.World);
		photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.GenerateGrid, false);
	}

	[PunRPC]
	public void ChangeState(GameState newState, bool doNotWaitOthersToBeReady = false)
	{
		_readyToChangeState++;
		HUDManager.Instance.UpdatePositionatingDoneInfo();
		if (!doNotWaitOthersToBeReady && _readyToChangeState < PhotonNetwork.PlayerList.Length)
			return;
		_readyToChangeState = 0;

		GameState = newState;
		switch (newState)
		{
			case GameState.GenerateGrid:
				GridManager.Instance.GenerateGrid();
				photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.SpawnUnits, false);
				break;
			case GameState.SpawnUnits:
				UnitManager.Instance.SpawnOwnUnits();
				photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.PositionateUnits, false);
				break;
			case GameState.PositionateUnits:
				break;
			case GameState.Finish:
				UnitManager.Instance.UpdateEveryUnitSpriteRenderer();
				break;
		}

		GridManager.Instance.HightLightTileUpdateEveryTile();
		HUDManager.Instance.UpdateTurnInfo();
		HUDManager.Instance.UpdateButtonsShow();
		positionateUnitsDoneState = false;
	}

	[PunRPC]
	public void CancelReadyForChangeState()
	{
		_readyToChangeState--;
		HUDManager.Instance.UpdatePositionatingDoneInfo();
	}

	public bool IsMyMoveTurn()
	{
		return (GameManager.Instance.GameState == GameState.RedMove && (GameManager.Instance.playerArmy == 0))
			|| GameManager.Instance.GameState == GameState.BlueMove && (GameManager.Instance.playerArmy == 1);
	}

	public bool IsMyAttackTurn()
	{
		return (GameManager.Instance.GameState == GameState.RedAttack && (GameManager.Instance.playerArmy == 0))
			|| (GameManager.Instance.GameState == GameState.BlueAttack && (GameManager.Instance.playerArmy == 1));
	}

	public int GetPlayerTurn()
	{
		switch (GameManager.Instance.GameState)
		{
			case GameState.RedMove:
				return 0;
			case GameState.RedAttack:
				return 0;
			case GameState.BlueMove:
				return 1;
			case GameState.BlueAttack:
				return 1;
			default:
				return -1;
		}
	}

	public void NextTurn()
	{
		List<GameState> SwitchGameStates = new List<GameState> { GameState.RedMove, GameState.RedAttack, GameState.BlueMove, GameState.BlueAttack };
		int index = SwitchGameStates.IndexOf(GameManager.Instance.GameState);

		GameState newGameState = (GameState)((index + 1) % SwitchGameStates.Count + 3);

		photonView.RPC("ChangeState", RpcTarget.AllBuffered, newGameState, true);
	}

	public void FinishGame(int winner)
	{
		photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.Finish, true);
	}

	public bool IsMyArmy(int army)
	{
		return playerArmy == army;
	}

	public bool IsGameFinished()
	{
		return GameState == GameState.Finish;
	}

	public void DonePositioning()
	{
		positionateUnitsDoneState = !positionateUnitsDoneState;

		if (positionateUnitsDoneState)
			photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.RedMove, false);
		else
			photonView.RPC("CancelReadyForChangeState", RpcTarget.AllBuffered);
		HUDManager.Instance.UpdateBtnDonePositioningAppearence();
	}

	public void DoneAttack()
	{
		NextTurn();
	}

	public void ExitGame()
	{
		GestorDeRede.Instancia.SairDoLobby();
		Destroy(GestorDeRede.Instancia.gameObject);
		SceneManager.LoadScene("Menu");
	}

	public bool IsOtherPlayerDonePositioningUnits()
    {
		if (_readyToChangeState >= 1 + (positionateUnitsDoneState ? 1 : 0))
			return true;
		return false;
    }

}

public enum GameState
{
	GenerateGrid = 0,
	SpawnUnits = 1,
	PositionateUnits = 2,
	RedMove = 3,
	RedAttack= 4,
	BlueMove = 5,
	BlueAttack = 6,
	Finish = 7
}
