using UnityEngine;

namespace CarlesModules {

  [System.Serializable]
  public class AudioProps {
    public AudioClip clip;
    public bool loop = false;

    [Space]
    [Range(0f, 5f)]
    public float volume = 1f;
    [Range(0, 3f)]
    public float volumeRandomDown = 0f;
    [Range(0f, 3f)]
    public float volumeRandomUp = 0f;

    [Space]
    [Range(0f, 5f)]
    public float pitch = 1f;
    [Range(0, 3f)]
    public float pitchRandomDown = 0f;
    [Range(0f, 3f)]
    public float pitchRandomUp = 0f;
  }

  // =============================================================================

  public class cAudio {

    public static void StopClipAtPoint(AudioSource source) {
      if (source) source.Stop();
    }

    public static AudioSource PlayClipAtPoint(
      Transform parent,
      Vector3 pos,
      AudioClip audioClip,
      float volume,
      float pitch,
      bool loop = false
    ) {
      // escape if there is no acudioClip to be played
      if (!audioClip) {
        Debug.LogError("cAudio - No audioClip was provided on " + parent.name);
        return null;
      }

      // Debug.Log("Playing AudioClip at volume " + volume);

      // create the temp object
      GameObject tempGO = new GameObject("Audio-Temp");
      tempGO.transform.SetParent(parent);
      tempGO.transform.position = pos; // set its position

      // add an audio source
      AudioSource tempASource = tempGO.AddComponent<AudioSource>();
      tempASource.clip = audioClip;
      tempASource.volume = volume; // Random.Range(0.025f, 0.1f) * volume; // volumeFactor;
      tempASource.pitch = pitch; // Random.Range(0f, 5f) * pitch;
      tempASource.loop = loop;

      tempASource.spatialBlend = 0.85f; // 0.7f; // 1f; // 0.3f;
      tempASource.rolloffMode = AudioRolloffMode.Logarithmic;
      tempASource.dopplerLevel = 1f; // tempASource.dopplerLevel;
      // tempASource.rolloffMode = AudioRolloffMode.Logarithmic; // AudioRolloffMode.Logarithmic; // tempASource.rolloffMode;
      // tempASource.maxDistance = 1f; //tempASource.maxDistance;

      // Debug.Log("audioClip " + audioClip);

      // tempASource.clip = audioSource.clip;
      // tempASource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
      // tempASource.mute = audioSource.mute;
      // tempASource.bypassEffects = audioSource.bypassEffects;
      // tempASource.bypassListenerEffects = audioSource.bypassListenerEffects;
      // tempASource.bypassReverbZones = audioSource.bypassReverbZones;
      // tempASource.playOnAwake = audioSource.playOnAwake;
      // tempASource.loop = audioSource.loop;
      // tempASource.priority = audioSource.priority;
      // tempASource.volume = audioSource.volume;
      // tempASource.pitch = audioSource.pitch;
      // tempASource.panStereo = audioSource.panStereo;
      // tempASource.spatialBlend = audioSource.spatialBlend;
      // tempASource.reverbZoneMix = audioSource.reverbZoneMix;
      // tempASource.dopplerLevel = audioSource.dopplerLevel;
      // tempASource.rolloffMode = audioSource.rolloffMode;
      // tempASource.minDistance = audioSource.minDistance;
      // tempASource.spread = audioSource.spread;
      // tempASource.maxDistance = audioSource.maxDistance;
      // set other aSource properties here, if desired

      // start the sound
      tempASource.Play();

      // destroy object after clip duration (this will not account for whether it is set to loop)
      if (!loop) MonoBehaviour.Destroy(tempGO, tempASource.clip.length);

      // return the AudioSource reference
      return tempASource;
    }
  }

}