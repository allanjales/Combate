using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAnimation : MonoBehaviour
{
	[SerializeField] private float _startDelaySecs;
	private Animator _animator;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_animator.speed = 0;
		StartCoroutine(IDelayedAnimation());
	}

	IEnumerator IDelayedAnimation()
	{
		yield return new WaitForSeconds(_startDelaySecs);
		_animator.speed = 1;
	}
}