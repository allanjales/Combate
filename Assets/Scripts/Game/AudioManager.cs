using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[Header("Audio Music Sources")]
	[SerializeField] AudioSource _Soundtrack;

	[Header("Audio Effects Sources")]
	[SerializeField] AudioSource _UnitsDistributionSound;
	[SerializeField] AudioSource _UnitMoveSound;
	[SerializeField] AudioSource _UnitKillSound;
	[SerializeField] AudioSource _UnitsSwapSound;

	[Header("Pitch range")]
	[Range(0f, .25f)]
	[SerializeField] float _RandomPitchRange;

	public static AudioManager Instance;

	private void Awake()
	{
		Instance = this;
		StartCoroutine(UnmuteSoundtrack());
	}

	IEnumerator UnmuteSoundtrack()
	{
		if (_Soundtrack == null)
			yield break;
		yield return new WaitForSeconds(.05f);
		_Soundtrack.mute = false;
	}

	/*
	 * Sounds Play
	 */

	private void SetRandomPitch(AudioSource Audio, float around = 1)
	{
		Audio.pitch = 1 + Random.Range(-_RandomPitchRange, _RandomPitchRange);
	}

	private void PlayEffect(AudioSource Audio)
	{
		SetRandomPitch(Audio);
		Audio.Play();
	}

	/*
	 * Sounds Call
	 */

	public void PlayUnitsDistributionSound()
	{
		PlayEffect(_UnitsDistributionSound);
	}

	public void PlayUnitMoveSound()
	{
		PlayEffect(_UnitMoveSound);
	}

	public void PlayUnitKillSound()
	{
		PlayEffect(_UnitKillSound);
	}

	public void PlayUnitsSwapSound()
	{
		PlayEffect(_UnitsSwapSound);
	}
}