using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step
{
    public Vector2Int pos;
    public Vector2Int speed;
    public Vector2Int firstStep;

    public Step(Vector2Int pos, Vector2Int speed, Vector2Int firstStep)
    {
        this.pos = pos;
        this.speed = speed;
        this.firstStep = firstStep;
    }

    public override int GetHashCode()
    {
        return pos.x + (pos.y * 100) + ((int) speed.magnitude * 10000);
    }

    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || GetType() != obj.GetType())
            return false;

        Step other = obj as Step;

        //No need to check for the equality of first step!
        return other.pos.Equals(pos) && other.speed.Equals(speed);
    }

    public override string ToString()
    {
        return "Pos: " + pos + " Speed: " + speed + " First Step: " + firstStep;
    }
}
