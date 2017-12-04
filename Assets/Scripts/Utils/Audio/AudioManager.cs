﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages audio files that will be loaded from Resources folder

public class AudioManager : MonoSingleton <AudioManager> {

	public static GameObject container;
	public static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
	public static float generalVolume = 1.0f;


	void Start() {
		container = gameObject;
	}
	
	public static void Play(string wav, float volume = 1.0f, bool loop = false) {
		Play(wav, volume, 1.0f, loop);
	}


	public static void Play(string wav, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		AudioClip clip = Resources.Load(wav) as AudioClip;

		// Log error if clip could not be loaded
		if (!clip) {
			Debug.LogError("Error while loading audio clip --> " + wav);
			return;
		}

		// if sound plays in a loop, escape if is already playing
		if (loop && audioSources.ContainsKey(wav)) {
				return;
		}

		// Add the audio source component
		AudioSource source = container.AddComponent<AudioSource>();
		source.loop = loop;

		// Log error if source could not be created
		if(!source) {
			print("Error while creating audio source component!");
			return;
		}

		// Set audio source props
		source.clip = clip;
		source.volume = volume * generalVolume;
		source.pitch = pitch;

		// Play it
		source.Play();

		if (loop)  {
			// if sound is playing in a loop, add it to the audiosources dictionary
			audioSources.Add(wav, source);
		} else {
			// otherwise, destroy the source component after the sound has played
			Destroy(source, clip.length);
		}
	}


	public static void Stop(string wav) {
		if (audioSources.ContainsKey(wav)) {
			// Stop source and remove its component
			AudioSource source = audioSources[wav];
			source.Stop();
			Destroy(source);

			// Remove audio source from sources dictionary
			audioSources.Remove(wav);

		} else {

			// Log error if source could not be removed
			Debug.LogError("Error while stopping AudioSource --> " + wav);
		}
	}


	public static bool IsPlaying (string wav) {
		// Note: only sounds playing in a loop are added to the dictionary
		return audioSources.ContainsKey(wav);
	}


	/*private IEnumerator WaitForSoundToEnd(string wav, AudioSource source, float duration) {
		yield return new WaitForSeconds(duration);

		Destroy(source);
		audioSources.Remove(wav);

	}*/

	// public void Fade(string wav, float volume, float duration) {
	// 	StartCoroutine(FadeCoroutine(wav, volume, duration));
	// }
  // 
	// private IEnumerator FadeCoroutine (string wav, float volume, float duration) {
	// 	if (!audioSources.ContainsKey(wav)) { yield break; }
  // 
	// 	AudioSource source = audioSources[wav];
  // 
	// 	float t = 0;
	// 	float start = source.volume;
  // 
	// 	while (t <= 1) {
	// 		t += Time.deltaTime / duration;
	// 		source.volume = Mathf.Lerp(start, volume, t); // Mathf.SmoothStep(0f, 1f, t)
	// 		yield return null;
	// 	}
  // 
	// 	if (volume == 0) {
	// 		Stop(wav);
	// 	}
	// }

}
