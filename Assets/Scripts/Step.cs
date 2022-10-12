using UnityEngine;

public readonly struct Step {
    public Vector2Int pos { get; }
    public Vector2Int speed { get; }
    public Vector2Int firstStep { get; }

    public Step(Vector2Int pos, Vector2Int speed, Vector2Int firstStep) {
        this.pos = pos;
        this.speed = speed;
        this.firstStep = firstStep;
    }

    public override int GetHashCode() {
        return pos.x + (pos.y * 100) + ((int) speed.magnitude * 10000);
    }

    public override bool Equals(object obj) {
        if (!(obj is Step other))
            return false;

        //No need to check for the equality of first step!
        return other.pos.Equals(pos) && other.speed.Equals(speed);
    }

    public override string ToString() {
        return "Pos: " + pos + " Speed: " + speed + " First Step: " + firstStep;
    }
}