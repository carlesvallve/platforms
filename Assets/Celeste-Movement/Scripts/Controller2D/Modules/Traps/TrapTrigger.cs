using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace Carles.Engine2D {

  public enum TrapMode {
    Spike,
    Boulder,
  }

  public class TrapTrigger : MonoBehaviour {

    public TrapMode trapMode;
    public Trap trap;

    void Start() {

    }

#if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }

    void _OnValidate() {
      Trap[] traps = transform.GetComponentsInChildren<Trap>(true);
      foreach (Trap trap in traps) {
        trap.gameObject.SetActive(false);
      }

      trap = GetTrap();
      trap.gameObject.SetActive(true);
    }
#endif

    private Trap GetTrap() {
      switch (trapMode) {
        case TrapMode.Spike: return transform.GetComponentInChildren<TrapSpike>(true);
        case TrapMode.Boulder: return transform.GetComponentInChildren<TrapBoulder>(true);
        default: return transform.GetComponentInChildren<TrapSpike>(true);
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      // if (collision.tag != "Player") return;
      // Debug.Log("Trap - OnTriggerEnter2D" + collision);
      trap.SetTarget(collision.gameObject);
      trap.Activate();
    }

    private void OnTriggerExit2D(Collider2D collision) {
      // if (collision.tag != "Player") return;
      // Debug.Log("Trap - OnTriggerExit2D" + collision);
      trap.SetTarget(null);
    }

  }
}
