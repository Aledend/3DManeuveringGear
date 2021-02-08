using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerInput", menuName ="ScriptableObjects/Player/PlayerInput")]
public class PlayerInput : ScriptableObject
{
    public KeyCode Jump = KeyCode.Space;
    public KeyCode Gas = KeyCode.LeftShift;
    public KeyCode Slide = KeyCode.LeftControl;


    public KeyCode GasDirRight = KeyCode.D;
    public KeyCode GasDirLeft = KeyCode.A;
    public KeyCode GasDirForw = KeyCode.W;
    public KeyCode GasDirBack = KeyCode.S;
    public KeyCode GasDirUp = KeyCode.Space;
    public KeyCode GasDirDown = KeyCode.LeftControl;

    public float GetGasRight => System.Convert.ToSingle(Input.GetKey(GasDirRight)) + -System.Convert.ToSingle(Input.GetKey(GasDirLeft));
    public float GetGasForw => System.Convert.ToSingle(Input.GetKey(GasDirForw)) + -System.Convert.ToSingle(Input.GetKey(GasDirBack));
    public float GetGasUp => System.Convert.ToSingle(Input.GetKey(GasDirUp)) + -System.Convert.ToSingle(Input.GetKey(GasDirDown));


    public KeyCode GrapplingLeft = KeyCode.Mouse0;
    public KeyCode GrapplingRight = KeyCode.Mouse1;
}
