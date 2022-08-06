using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager Instance;
	public GameState GameState;

	[SerializeField] private GameObject _table;

	public int playerArmy;
	private int _readyToChangeState;
	public bool PositionateUnitsDoneState { get; private set; } = false;

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

		if (IsGameFinished())
			return;

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
		PositionateUnitsDoneState = false;
	}

	[PunRPC]
	public void CancelReadyForChangeState()
	{
		_readyToChangeState--;
		HUDManager.Instance.UpdatePositionatingDoneInfo();
	}

	public bool IsMyMoveTurn()
	{
		return (GameState == GameState.RedMove && (playerArmy == 0)) || GameState == GameState.BlueMove && (playerArmy == 1);
	}

	public bool IsMyAttackTurn()
	{
		return (GameState == GameState.RedAttack && (playerArmy == 0)) || (GameState == GameState.BlueAttack && (playerArmy == 1));
	}

	public void NextTurn()
	{
		List<GameState> SwitchGameStates = new() { GameState.RedMove, GameState.RedAttack, GameState.BlueMove, GameState.BlueAttack };
		int index = SwitchGameStates.IndexOf(GameManager.Instance.GameState);

		GameState newGameState = (GameState)((index + 1) % SwitchGameStates.Count + 3);

		photonView.RPC("ChangeState", RpcTarget.AllBuffered, newGameState, true);
	}

	public void FinishGame(GameOverInfo goinfo)
	{
		if (IsGameFinished())
			return;

		photonView.RPC("ChangeState", RpcTarget.AllBuffered, GameState.Finish, true);
		HUDManager.Instance.SetWinnerOnText(goinfo.winner);
		GameOverScreen.Instance.GameOver(goinfo);
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
		PositionateUnitsDoneState = !PositionateUnitsDoneState;

		if (PositionateUnitsDoneState)
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
		if (_readyToChangeState >= 1 + (PositionateUnitsDoneState ? 1 : 0))
			return true;
		return false;
	}

	public void CheckIfThereIsAWinner()
	{
		if (IsGameFinished())
			return;

		int winner = -1;

		if (!UnitManager.Instance.ArmyHasUnitsToMove(0) || !UnitManager.Instance.ArmyHasFlag(0))
			winner = 1;

		if (!UnitManager.Instance.ArmyHasUnitsToMove(1) || !UnitManager.Instance.ArmyHasFlag(1))
			winner += 1;

		if (winner != -1)
			FinishGame(new GameOverInfo(winner, GameOverReason.NoMoves));
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		FinishGame(new GameOverInfo(playerArmy, GameOverReason.PlayerLeft, otherPlayer));
	}

	public string GetArmyOwnerNickName(int army)
	{
		if (IsMyArmy(army))
			return PhotonNetwork.LocalPlayer.NickName;

		if (PhotonNetwork.PlayerList[0] != PhotonNetwork.LocalPlayer)
			return PhotonNetwork.PlayerList[0].NickName;

		if (PhotonNetwork.PlayerList[1] != PhotonNetwork.LocalPlayer)
			return PhotonNetwork.PlayerList[1].NickName;

		return null;
	}

	public string GetMyNickName()
	{
		return GetArmyOwnerNickName(playerArmy);
	}

	public string GetEnemyNickName()
	{
		return GetArmyOwnerNickName((playerArmy == 1) ? 0 : 1);
	}
}

public enum GameState
{
	GenerateGrid = 0,
	SpawnUnits = 1,
	PositionateUnits = 2,
	RedMove = 3,
	RedAttack = 4,
	BlueMove = 5,
	BlueAttack = 6,
	Finish = 7
}
