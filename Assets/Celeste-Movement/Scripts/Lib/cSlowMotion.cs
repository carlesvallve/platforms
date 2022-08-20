using UnityEngine;
// using Invector.vCharacterController;

namespace CarlesModules {

  public class cSlowMotion : MonoBehaviour {

    // [Space]
    // public GenericInput inputSlowMotion1 = new GenericInput("LeftControl", "LB", "LB");
    // public GenericInput inputSlowMotion2 = new GenericInput("T", "A", "A");

    public float slowDownFactor = 0.05f;
    public float slowDownDuration = 5f;

    void Start() { }

    void Update() {
      // if (cUtils.instance.isInventoryOpen) {
      //   Time.timeScale = 0;
      //   return;
      // }

      // if (inputSlowMotion1.GetButton() && inputSlowMotion2.GetButtonDown()) DoSlowMotion();

      Time.timeScale += (1f / slowDownDuration) * Time.unscaledDeltaTime;
      Time.fixedDeltaTime += (0.01f / slowDownDuration) * Time.unscaledDeltaTime;
      Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
      Time.fixedDeltaTime = Mathf.Clamp(Time.fixedDeltaTime, 0f, 0.01f);

    }

    public void DoSlowMotion() {

      Time.timeScale = slowDownFactor;
      Time.fixedDeltaTime = Time.fixedDeltaTime * slowDownFactor;

    }

  }
}
