using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages audio clips embedded in the component through the inspector

public class AudioClipManager : MonoBehaviour {

	public List<AudioClip> sfxList;

	public static Dictionary<string, AudioClip> sfx = new Dictionary<string, AudioClip>();
	public static GameObject container;
	public static float audioLevel = 1.0f;


	void Start() {
		container = gameObject;

    sfx = new Dictionary<string, AudioClip>();
		foreach(AudioClip clip in sfxList) {
			// Debug.Log(clip.name);
			sfx.Add(clip.name, clip);
		}
	}


	public static AudioSource Play(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		if (!clip) {
			Debug.LogError("Error while loading audio clip! --> " + clip);
			return null;
		}

		// create an empty game object at given pos
		GameObject go = new GameObject("Audio: " + clip.name);
		go.transform.parent = container.transform;

		// Create the audio source
		AudioSource source = go.AddComponent<AudioSource>(); //(AudioSource); // as AudioSource;
		source.loop = loop;

		if(!source) {
			Debug.Log("Error while creating audio source component!");
			return null;
		}

		// set audio source props
		source.clip = clip;
		source.volume = volume * audioLevel;
		source.pitch = pitch;

		// play it
		source.Play();

		// destroy it
		if (!loop) Destroy(go, clip.length);

		// return it in case we need it for something
		return source;
	}
}
