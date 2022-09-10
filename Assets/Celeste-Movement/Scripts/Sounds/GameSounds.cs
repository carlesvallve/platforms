using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarlesModules;

// todo: generate general game sounds instead of current

namespace Carles.Engine2D {

  public class GameSounds : MonoBehaviour {

    [Header("Game SFX")]
    [Space]
    public AudioProps splash;

    public static GameSounds instance { get; private set; }

    private void Awake() {
      // If there is an instance, and it's not me, delete myself.
      if (instance != null && instance != this) {
        Destroy(this);
      } else {
        instance = this;
      }
    }

    public void PlaySplash() {
      PlaySound(splash);
    }

    private void PlaySound(AudioProps audioProps) {
      if (audioProps.clip) {
        cAudio.PlayClipAtPoint(
          transform, transform.position, audioProps.clip,
          audioProps.volume + Random.Range(-audioProps.volumeRandomDown, audioProps.volumeRandomUp),
          audioProps.pitch + Random.Range(-audioProps.pitchRandomDown, audioProps.pitchRandomUp),
          audioProps.loop
        );
      }
    }
  }

}
