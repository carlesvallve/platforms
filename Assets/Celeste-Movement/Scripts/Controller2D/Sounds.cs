using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarlesModules;

namespace Carles.Engine2D {

  public class Sounds : MonoBehaviour {

    [Space]
    [Header("Audio")]
    public AudioProps footstep;
    public AudioProps slide;
    public AudioProps jump;
    public AudioProps dash;

    public void PlayFootstep() {
      PlaySound(footstep);
    }

    public void PlaySlide() {
      PlaySound(slide);
    }

    public void PlayJump() {
      PlaySound(jump);
    }

    public void PlayDash() {
      PlaySound(dash);
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
