
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo
// one way platforms -> Done!
// ladders -> Done!
// ropes -> Done!
// grappling hook -> Done!
// water  -> Done!

// melee -> OK
// shooting -> OK
// damage -> OK
// death -> OK

// inventory
// souls

// todo
// procedural levels
// - add variable corridor width -> OK
// - add enter and exit nodes -> OK
// - add platforms
// - add one way platforms
// - add movable platforms
// - add destroyable platforms
// - add traps
// - add ladders
// - add elevators
// - add doors
// - add teleporters

namespace Carles.Engine2D {

  public class CharController2D : MonoBehaviour {
    //
    [HideInInspector] public Rigidbody2D rb;
    //
    [HideInInspector] public CharSkin skin;
    [HideInInspector] public Collision coll;
    [HideInInspector] public Movement move;
    //
    [HideInInspector] public Jump jump;
    [HideInInspector] public Dash dash;
    [HideInInspector] public Combat combat;
    [HideInInspector] public Hook hook;
    [HideInInspector] public RopeClimb ropeClimb;
    [HideInInspector] public LadderClimb ladderClimb;
    //
    [HideInInspector] public CharAnimation anim;
    [HideInInspector] public Sounds sounds;
    [HideInInspector] public Particles particles;

    void Start() {
      rb = GetComponent<Rigidbody2D>();
      //
      skin = GetComponent<CharSkin>();
      coll = GetComponent<Collision>();
      move = GetComponent<Movement>();
      jump = GetComponent<Jump>();
      dash = GetComponent<Dash>();
      combat = GetComponent<Combat>();
      hook = GetComponent<Hook>();
      ropeClimb = GetComponent<RopeClimb>();
      ladderClimb = GetComponent<LadderClimb>();
      //
      anim = GetComponentInChildren<CharAnimation>();
      sounds = GetComponentInChildren<Sounds>();
      particles = GetComponentInChildren<Particles>();
    }

  }
}
