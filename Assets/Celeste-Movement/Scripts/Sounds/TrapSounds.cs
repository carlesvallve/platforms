using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarlesModules;

namespace Carles.Engine2D {

  public class TrapSounds : MonoBehaviour {

    [Header("Audio")]
    [Space]
    public AudioProps trigger;
    public AudioProps trap;
    // public AudioProps block;


    public void PlayTrigger() {
      PlaySound(trigger);
    }

    public void PlayTrap() {
      PlaySound(trap);
    }

    // public void PlayBlock() {
    //   PlaySound(block);
    // }

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
