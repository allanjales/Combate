using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
	public static GameOverScreen Instance;

	[Header("Army colors")]
	[SerializeField] private Color _RedArmyColor;
	[SerializeField] private Color _BlueArmyColor;

	[Header("Elements link")]
	[SerializeField] private GameObject _GameOverScreen;
	[SerializeField] private Text _WinnerText, _WinnerReasonText;

	private void Awake()
	{
		Instance = this;
	}

	public void SetWinnerText(int winner)
	{
		if (winner == 2)
			return;

		string playerName = GameManager.Instance.GetArmyOwnerNickName(winner);
		if (GameManager.Instance.IsMyArmy(winner))
			playerName = "Você";
		string colorHex = ColorUtility.ToHtmlStringRGB((winner == 0) ? _RedArmyColor : _BlueArmyColor);

		_WinnerText.GetComponent<Text>().text = $"<color=#{colorHex}>{playerName}</color> venceu";
	}

	public void SetWinnerReasonText(GameOverInfo goinfo)
	{
		if (goinfo.winner == 2)
			return;

		string playerName, colorHex;
		switch (goinfo.reason)
		{
			case GameOverReason.CaptureFlag:
				colorHex = ColorUtility.ToHtmlStringRGB((goinfo.winner == 0) ? _RedArmyColor : _BlueArmyColor);
				_WinnerReasonText.GetComponent<Text>().text = $"<color=#{colorHex}>{goinfo.CaptureUnit.UnitNumber} {goinfo.CaptureUnit.UnitName}</color> capturou a bandeira";
				break;
			case GameOverReason.NoMoves:
				playerName = GameManager.Instance.GetArmyOwnerNickName((goinfo.winner == 1) ? 0 : 1);
				if (!GameManager.Instance.IsMyArmy(goinfo.winner))
					playerName = "Você";
				colorHex = ColorUtility.ToHtmlStringRGB((goinfo.winner == 1) ? _RedArmyColor : _BlueArmyColor);
				_WinnerReasonText.GetComponent<Text>().text = $"<color=#{colorHex}>{playerName}</color> ficou sem movimentos";
				break;
			case GameOverReason.PlayerLeft:
				colorHex = ColorUtility.ToHtmlStringRGB((goinfo.winner == 1) ? _RedArmyColor : _BlueArmyColor);
				_WinnerReasonText.GetComponent<Text>().text = $"<color=#{colorHex}>{goinfo.PlayerLeft.NickName}</color> saiu da partida";
				break;
		}
	}

	public void OpenGameOverScreen()
	{
		_GameOverScreen.SetActive(true);
	}

	public void CloseGameOverScreen()
	{
		_GameOverScreen.SetActive(false);
	}

	public void GameOver(GameOverInfo goinfo)
	{
		SetWinnerText(goinfo.winner);
		SetWinnerReasonText(goinfo);
		OpenGameOverScreen();
	}
}

public enum GameOverReason
{
	NoMoves = 0,
	CaptureFlag = 1,
	PlayerLeft = 2
}

public struct GameOverInfo
{
	public int winner { private set; get; }
	public GameOverReason reason { private set; get; }
	public Unit CaptureUnit { private set; get; }
	public Player PlayerLeft { private set; get; }

	public GameOverInfo(int winner, GameOverReason reason)
	{
		this.winner = winner;
		this.reason = reason;
		this.CaptureUnit = null;
		this.PlayerLeft = null;
	}

	public GameOverInfo(int winner, GameOverReason reason, Player PlayerLeft)
	{
		this.winner = winner;
		this.reason = reason;
		this.CaptureUnit = null;
		this.PlayerLeft = PlayerLeft;
	}

	public GameOverInfo(int winner, GameOverReason reason, Unit CaptureUnit)
	{
		this.winner = winner;
		this.reason = reason;
		this.CaptureUnit = CaptureUnit;
		this.PlayerLeft = null;
	}
}