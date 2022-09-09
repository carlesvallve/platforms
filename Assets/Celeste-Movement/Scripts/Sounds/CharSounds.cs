using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarlesModules;

namespace Carles.Engine2D {

  public class CharSounds : MonoBehaviour {

    [Header("Audio")]
    [Space]
    public AudioProps footstep;
    public AudioProps slide;
    public AudioProps jump;
    public AudioProps dash;
    public AudioProps splash;
    [Space]
    public AudioProps attack;
    public AudioProps block;
    public AudioProps damage;
    public AudioProps die;

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

    public void PlaySplash() {
      PlaySound(splash);
    }

    public void PlayAttack() {
      PlaySound(attack);
    }

    public void PlayBlock() {
      PlaySound(block);
    }

    public void PlayDamage() {
      PlaySound(damage);
    }

    public void PlayDie() {
      PlaySound(die);
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