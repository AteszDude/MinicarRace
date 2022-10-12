using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    private float mainSpeed = 100.0f; //regular speed
    private float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    private float maxShift = 1000.0f; //Maximum speed when holdin gshift
    private float camSens = 0.25f; //How sensitive it with mouse
    private float totalRun = 1.0f;

    void Update() {
        //Keyboard commands
        var f = 0.0f;
        var p = GetBaseInput();
        if (p.sqrMagnitude <= 0) return; // only move while a direction key is pressed

        if (Input.GetKey(KeyCode.LeftShift)) {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.Space)) {
            //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else {
            transform.Translate(p);
        }
    }

    private static Vector3 GetBaseInput() {
        //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            p_Velocity += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.S)) {
            p_Velocity += new Vector3(0, 0, -1);
        }

        if (Input.GetKey(KeyCode.A)) {
            p_Velocity += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D)) {
            p_Velocity += new Vector3(1, 0, 0);
        }

        return p_Velocity;
    }
}