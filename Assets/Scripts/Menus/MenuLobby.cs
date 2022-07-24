using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobby : MonoBehaviourPunCallbacks
{
	[SerializeField] private Text _listaDeJogadores, _PlayersCounter;
	[SerializeField] private Button _comecaJogo;
	[SerializeField] private InputField _nomeSala;

	[PunRPC]
	public void AtualizaLista()
	{
		_listaDeJogadores.text = GestorDeRede.Instancia.ObterListaDeJogadores();
		_comecaJogo.gameObject.SetActive(GestorDeRede.Instancia.DonoDaSala());
		_PlayersCounter.text = $"{PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
		_comecaJogo.interactable = PhotonNetwork.PlayerList.Length == 2;
	}

	public void UpdateRoomName()
	{
		_nomeSala.text = PhotonNetwork.CurrentRoom.Name;
	}
}
