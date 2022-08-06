using UnityEngine;

public class Credits : MonoBehaviour
{
	public void OpenCredits()
	{
		gameObject.SetActive(true);
	}

	public void CloseCredits()
	{
		gameObject.SetActive(false);
	}
}
