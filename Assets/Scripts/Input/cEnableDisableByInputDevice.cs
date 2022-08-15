using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles {
  public class cEnableDisableByInputDevice : MonoBehaviour {
    public InputDevice inputDevice;
    public CheckMethod methodToCheck;
    public enum CheckMethod {
      Equals,
      Different,
    }
    void Start() {
      cInput.instance.onChangeInputType -= OnChangeInput;
      cInput.instance.onChangeInputType += OnChangeInput;
      OnChangeInput(cInput.instance.inputDevice);
    }

    public void OnChangeInput(InputDevice type) {
      bool validate = methodToCheck == CheckMethod.Different ? type != inputDevice : type == inputDevice;
      if (gameObject.activeSelf != validate) gameObject.SetActive(validate);
    }

  }
}
