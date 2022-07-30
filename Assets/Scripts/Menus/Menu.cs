using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
	[SerializeField] private MenuEntrada _menuEntrada;
	[SerializeField] private GameObject _telaCarregando;
	[SerializeField] private MenuLobby _menuLobby;
	[SerializeField] private GameObject _btnsHolder;

	[SerializeField] private Text _appVersion;

	void Start()
	{
		_telaCarregando.SetActive(true);
		_menuEntrada.gameObject.SetActive(false);
		_menuLobby.gameObject.SetActive(false);
		_btnsHolder.SetActive(false);
		_appVersion.text = $"v{PhotonNetwork.AppVersion}";
	}

	public override void OnConnectedToMaster()
	{
		_telaCarregando.SetActive(false);
		_menuEntrada.gameObject.SetActive(true);
		_btnsHolder.SetActive(true);
	}

	public override void OnJoinedRoom()
	{
		MudaMenu(_menuLobby.gameObject);
		_menuLobby.photonView.RPC("AtualizaLista", RpcTarget.All);
		_menuLobby.UpdateRoomName();
	}

	public void MudaMenu(GameObject menu)
	{
		_telaCarregando.SetActive(false);
		_menuEntrada.gameObject.SetActive(false);
		_menuLobby.gameObject.SetActive(false);

		menu.SetActive(true);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		_menuLobby.AtualizaLista();
	}

	public void SairDoLobby()
	{
		GestorDeRede.Instancia.SairDoLobby();
		MudaMenu(_menuEntrada.gameObject);
	}

	public void ComecaJogo(string nomeCena)
	{
		GestorDeRede.Instancia.photonView.RPC("ComecaJogo", RpcTarget.All, nomeCena);
	}

	public void FecharJogo()
	{
		Application.Quit();
	}
}
