using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class TrapTrigger : MonoBehaviour {

    public Trap trap;

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
