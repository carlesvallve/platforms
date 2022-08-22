using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles {

  public class PlayerControls : MonoBehaviour {

    [Header("Player Controls")]
    [Space]
    public GenericInput Horizontal = new GenericInput("Horizontal", "LeftAnalogHorizontal", "Horizontal");
    public GenericInput Vertical = new GenericInput("Vertical", "LeftAnalogVertical", "Vertical");

    public GenericInput Action = new GenericInput("E", "Y", "Y");
    public GenericInput Jump = new GenericInput("Space", "A", "A");
    public GenericInput Attack = new GenericInput("Mouse0", "X", "X");
    public GenericInput Weapon = new GenericInput("Tab", "RightStickClick", "RightStickClick");

    // public GenericInput Roll = new GenericInput("LeftShift", "LeftStickClick", "LeftStickClick");
    // public GenericInput Target = new GenericInput("Tab", "RightStickClick", "RightStickClick");
    // public GenericInput Block = new GenericInput("Mouse1", "B", "B");

    // [Space]
    // [Header("Game Controls")]
    // [Space]
    // public GenericInput AIGoal = new GenericInput("E", "LeftStickClick", "LeftStickClick");
  }
}


