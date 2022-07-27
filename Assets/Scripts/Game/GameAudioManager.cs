using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
	[Header("Audio Sources")]
	[SerializeField] AudioSource _UnitsDistributionSound;
	[SerializeField] AudioSource _UnitMoveSound;
	[SerializeField] AudioSource _UnitKillSound;

	[Header("Pitch range")]
	[Range(0f, .10f)]
	[SerializeField] float _RandomPitchRange;

	public static GameAudioManager Instance;

	private void Awake()
	{
		Instance = this;
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

	public void PlayUnitDistributionSound()
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
}
