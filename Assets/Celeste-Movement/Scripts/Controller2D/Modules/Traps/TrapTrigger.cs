using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class TrapTrigger : MonoBehaviour {

    public Trap trap;
    public float delayActivate = 0.3f;
    public float delayReset = 2f;
    public bool active = false;

    private TextMesh info;

    // private CharController2D c;

    void Start() {
      // c = GetComponent<CharController2D>();
    }
  }
}
