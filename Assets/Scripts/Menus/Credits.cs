using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(false);
	}

	public void OpenCredits()
	{
		gameObject.SetActive(true);
	}

	public void CloseCredits()
	{
		gameObject.SetActive(false);
	}
}
