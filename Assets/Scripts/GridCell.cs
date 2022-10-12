using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour {
    public enum GridType {
        Racetrack,
        Unoccupiable,
        Finish
    }

    //Which fields can the user click
    public enum SelectState {
        Unselected,
        ReadyForSelection
    }

    public bool isOccupied;
    public GridType type;
    public SelectState state = SelectState.Unselected;

    private readonly List<int> distances = new List<int>();

    private int posX;
    private int posY;

    public void SetGrid(int x, int y, GridType type) {
        this.type = type;
        posX = x;
        posY = y;

        if (type == GridType.Unoccupiable) {
            transform.GetComponent<MeshRenderer>().enabled = false;
            transform.GetComponent<BoxCollider>().enabled = false;
        }
        else if (type == GridType.Finish) {
            transform.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        gameObject.name = "Grid Space (" + x + ", " + y + ")";
    }

    public bool AssignRacer(Racer racer) {
        if (racer == null) {
            Debug.Log("Racer can't be null!");
            return false;
        }

        isOccupied = true;
        racer.Speed = new Vector2Int(posX, posY) - racer.Cell.GetPosition();

        racer.setCell(this, true);

        var newPos = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z);
        racer.gameObject.transform.position = newPos;

        return type == GridType.Finish;
    }

    public void RemoveRacer() {
        isOccupied = false;
    }

    public bool CanRacerOccupy(bool racerCounts) {
        return type != GridType.Unoccupiable && (!isOccupied || !racerCounts);
    }

    public void Select() {
        if (state == SelectState.ReadyForSelection) {
        }
    }

    public bool CanPlayerSelect() {
        return state == SelectState.ReadyForSelection;
    }

    public void ResetSelection() {
        state = SelectState.Unselected;
        if (type == GridType.Finish)
            transform.GetComponent<MeshRenderer>().material.color = Color.red;
        else
            transform.GetComponent<MeshRenderer>().material.color = Color.blue;

#if UNITY_EDITOR

        if (isDebugColor) DebugColor();
#endif
    }

    public void ReadyForSelect() {
        state = SelectState.ReadyForSelection;
        transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public Vector2Int GetPosition() {
        return new Vector2Int(posX, posY);
    }

#if UNITY_EDITOR
    private void DebugColor() {
        if (type != GridType.Racetrack)
            return;

        Color color;

        var debugNum = distances[debugLayer] * 3;

        if (debugNum < 256)
            color = new Color(debugNum / 256.0f, 0.0f, 0.0f);
        else if (debugNum < 512)
            color = new Color(0.0f, (debugNum - 256.0f) / 256.0f, 0.0f);
        else if (debugNum < 768)
            color = new Color(0.0f, 0.0f, (debugNum - 512.0f) / 256.0f);
        else color = new Color(1.0f, 1.0f, 1.0f);

        transform.GetComponent<MeshRenderer>().material.color = color;
    }
#endif

    public int GETDistance(int layer) {
        return distances[layer];
    }

    public void AddLayer() {
        switch (type) {
            case GridType.Racetrack:
                distances.Add(-1);
                break;
            case GridType.Unoccupiable:
                distances.Add(int.MaxValue);
                break;
            case GridType.Finish:
                distances.Add(0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetDistance(int layer, int val) {
        distances[layer] = val;
    }

    public void AddDistance(int layer, int val) {
        distances[layer] += val;
    }

#if UNITY_EDITOR
    private int debugLayer;
    private readonly bool isDebugColor = false;
#endif
}