using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviour
{
	[SerializeField] private Text _nomeDaSala;
	[SerializeField] private Text _nomeDoJogador;

	private bool IsInputAllowed()
	{
		if (_nomeDoJogador.text == "")
			return false;

		if (_nomeDaSala.text == "")
			return false;

		return true;
	}

	public void CriaSala()
	{
		if (!IsInputAllowed())
			return;

		GestorDeRede.Instancia.MudaNick(_nomeDoJogador.text);
		GestorDeRede.Instancia.CriaSala(_nomeDaSala.text);
	}

	public void EntraSala()
	{
		if (!IsInputAllowed())
			return;

		GestorDeRede.Instancia.MudaNick(_nomeDoJogador.text);
		GestorDeRede.Instancia.EntraSala(_nomeDaSala.text);
	}
}
