using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(KMSelectable))]
public sealed class Button : MonoBehaviour
{
	[SerializeField] private new KMAudio audio;
	private Animator _animator;
	public KMSelectable selectable;
	private static readonly int IsPressed = Animator.StringToHash("IsPressed");

	public event Action<char> OnInteract;

	public bool IsActive { get; set; } = true;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		selectable = GetComponent<KMSelectable>();

		selectable.OnInteract += InternalOnInteract;
		selectable.OnInteractEnded += InternalOnInteractEnded;
	}

	private bool InternalOnInteract()
	{
		_animator.SetBool(IsPressed, true);
		audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		selectable.AddInteractionPunch();
		if (!IsActive) return false;
		var text = GetComponentInChildren<TextMesh>().text.ToCharArray()[0];
		OnInteract?.Invoke(text);
		return false;
	}

	private void InternalOnInteractEnded()
	{
		_animator.SetBool(IsPressed, false);
		audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
	}
}