using System.Collections;
using System.Collections.Generic;
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
