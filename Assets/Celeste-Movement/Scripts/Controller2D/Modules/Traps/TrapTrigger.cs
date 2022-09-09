using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class TrapTrigger : MonoBehaviour {

    public Trap trap;
    public float delayActivate = 0.3f;
    public float delayReset = 2f;
    public bool active = false;

    private TMP_Text label;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
      label = GetComponentInChildren<TMP_Text>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
      // if (collision.tag != "Player") return;

      // render click text and sound
      // start wait coroutine
      // tell trap to activate

    }

    private void OnTriggerExit2D(Collider2D collision) {
      // if (collision.tag != "Player") return;

    }

    public void SetLabel(string str) {
      label.text = str;
    }


    // public virtual IEnumerator SetLabel(string str) {
    //   if (!label) { yield break; }

    //   label.gameObject.SetActive(str != null);
    //   if (str == null) { yield break; }
    //   label.text = str;
    // }
  }
}
