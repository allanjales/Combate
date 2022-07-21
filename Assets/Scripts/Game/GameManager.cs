using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public GameState GameState;

	[SerializeField] private GameObject _table;

	public int PlayerSide;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		PlayerSide = UnityEngine.Random.Range(0, 2);
		_table.transform.Rotate(0f, 0f, (float)PlayerSide*180f, Space.World);

		this.ChangeState(GameState.GenerateGrid);
	}

	public void ChangeState(GameState newState)
	{
		GameState = newState;
		switch (newState)
		{
			case GameState.GenerateGrid:
				GridManager.Instance.GenerateGrid();
				this.ChangeState(GameState.SpawnUnits);
				break;
			case GameState.SpawnUnits:
				UnitManager.Instance.SpawnRedUnits();
				UnitManager.Instance.SpawnBlueUnits();
				this.ChangeState(GameState.RedMove);
				break;
			case GameState.RedMove:
				HUDManager.Instance.UpdateTurnInfo();
				break;
			case GameState.RedAttack:
				HUDManager.Instance.UpdateTurnInfo();
				break;
			case GameState.BlueMove:
				HUDManager.Instance.UpdateTurnInfo();
				break;
			case GameState.BlueAttack:
				HUDManager.Instance.UpdateTurnInfo();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
		}
	}

	public bool IsMyMoveTurn()
	{
		return (GameManager.Instance.GameState == GameState.RedMove && (GameManager.Instance.PlayerSide == 0))
			|| GameManager.Instance.GameState == GameState.BlueMove && (GameManager.Instance.PlayerSide == 1);
	}
	void Update()
	{

		if (Input.GetKeyDown("n"))
		{
			if ((int)GameManager.Instance.GameState < 2)
				return;

			GameManager.Instance.GameState = (GameState)(((int)GameManager.Instance.GameState + 1) % Enum.GetNames(typeof(GameState)).Length);

			if ((int)GameManager.Instance.GameState < 2)
				GameManager.Instance.GameState = GameState.RedMove;

			HUDManager.Instance.UpdateTurnInfo();
		}
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
}

public enum GameState
{
	GenerateGrid = 0,
	SpawnUnits = 1,
	RedMove = 2,
	RedAttack= 3,
	BlueMove = 4,
	BlueAttack = 5
}
