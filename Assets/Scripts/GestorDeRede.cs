using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GestorDeRede : MonoBehaviourPunCallbacks
{
	public static GestorDeRede Instancia { get; private set; }

	private void Awake()
	{
		if (Instancia != null && Instancia != this)
		{
			gameObject.SetActive(false);
			return;
		}

		Instancia = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Conexão bem sucedida.");
	}

	public bool CriaSala(string nomeSala)
	{
		return PhotonNetwork.CreateRoom(nomeSala, new RoomOptions() { MaxPlayers = 2 }, null);
	}

	public bool EntraSala(string nomeSala)
	{
		return PhotonNetwork.JoinRoom(nomeSala);
	}

	public void MudaNick(string nickname)
	{
		PhotonNetwork.NickName = nickname;
	}

	public string ObterListaDeJogadores()
	{
		string lista = "";
		foreach (var player in PhotonNetwork.PlayerList)
		{
			lista += player.NickName + "\n";
		}
		return lista;
	}

	public bool DonoDaSala()
	{
		return PhotonNetwork.IsMasterClient;
	}

	public void SairDoLobby()
	{
		PhotonNetwork.LeaveRoom();
	}

	[PunRPC]
	public void ComecaJogo(string nomeCena)
	{
		if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
			PhotonNetwork.LoadLevel(nomeCena);
	}
}
