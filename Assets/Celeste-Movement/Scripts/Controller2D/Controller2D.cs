using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Controller2D : MonoBehaviour {

    public Rigidbody2D rb;

    // private InputControls controls;

    public Collision coll;
    public Movement mov;

    public Jump jump;
    public Dash dash;
    public WallSlide slide;

    public AnimationScript anim;
    public Sounds sounds;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    void Start() {
      rb = GetComponent<Rigidbody2D>();

      coll = GetComponent<Collision>();
      mov = GetComponent<Movement>();

      jump = GetComponent<Jump>();
      dash = GetComponent<Dash>();
      slide = GetComponent<WallSlide>();

      anim = GetComponentInChildren<AnimationScript>();
      sounds = GetComponentInChildren<Sounds>();
    }

  }



}
