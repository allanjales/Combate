using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviourPunCallbacks
{
	[SerializeField] private Text _nomeDaSala;
	[SerializeField] private Text _nomeDoJogador;

	[SerializeField] private GameObject _infoTextHolder;
	[SerializeField] private Text _infoText;

	private bool _canPressButton = true;

	private string GetInputErrors()
	{
		if (_nomeDaSala.text == "")
			return "Faltou o Nome da Sala";

		if (_nomeDoJogador.text == "")
			return "Faltou o Seu Apelido";

		return null;
	}

	public void EntraOuCriaSala()
	{
		string inputErrors = GetInputErrors();
		if (inputErrors != null)
		{
			MostrarInfoText(inputErrors, 1);
			return;
		}

		if (!_canPressButton)
			return;

		GestorDeRede.Instancia.MudaNick(_nomeDoJogador.text);
		GestorDeRede.Instancia.EntraOuCriaSala(_nomeDaSala.text);
		BloquearBotao();

		MostrarInfoText("Conectando...", 0);
		//OcultarInfoText();
	}

	public void MostrarInfoText(string text, int cor)
	{
		if (_infoText == null || _infoTextHolder == null)
			return;
		MudarInfoText(text, cor);
		_infoTextHolder.SetActive(true);
	}

	public void OcultarInfoText()
	{
		if (_infoText == null || _infoTextHolder == null)
			return;
		_infoTextHolder.SetActive(false);
	}

	private void MudarInfoText(string text, int cor = 0)
	{
		string hexcolor = null;
		switch (cor)
		{
			case 1:
				hexcolor = "#F00";
				break;
			case 2:
				hexcolor = "#FF0";
				break;
		}

		if (hexcolor == null)
			_infoText.text = text;
		else
			_infoText.text = $"<color={hexcolor}>{text}</color>";
	}

	public override void OnCreatedRoom()
	{
		OcultarInfoText();
		DesbloquearBotao();
	}

	public override void OnJoinedRoom()
	{
		OcultarInfoText();
		DesbloquearBotao();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		MostrarInfoText($"{returnCode}: {message}", 1);
		DesbloquearBotao();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		MostrarInfoText($"{returnCode}: {message}", 1);
		DesbloquearBotao();
	}

	private void BloquearBotao()
	{
		_canPressButton = false;
	}

	private void DesbloquearBotao()
	{
		_canPressButton = true;
	}
}
