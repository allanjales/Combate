using UnityEngine;

public class GameSettings : MonoBehaviour
{
	public static GameSettings Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			gameObject.SetActive(false);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = Screen.currentResolution.refreshRate;
	}
}
