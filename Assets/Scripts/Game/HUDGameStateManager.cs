using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDGameStateManager : MonoBehaviour
{
	public static HUDGameStateManager Instance;
	[SerializeField] private List<GameObject> _ListGameStateOwnTurnInfo;
	[SerializeField] private List<GameObject> _ListGameStateEnemyTurnInfo;


	private void Awake()
	{
		Instance = this;
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
			GameStateTurnInfo.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? HUDManager.Instance.RedArmyColor : HUDManager.Instance.BlueArmyColor;

		foreach (GameObject GameStateTurnInfo in _ListGameStateEnemyTurnInfo)
			GameStateTurnInfo.GetComponent<Image>().color = (GameManager.Instance.playerArmy == 0) ? HUDManager.Instance.BlueArmyColor : HUDManager.Instance.RedArmyColor;
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
}
