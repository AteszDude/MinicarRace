using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Racer : MonoBehaviour
{
    public GridCell Cell { get; set; }
    public Vector2Int Speed { get; set; }

    [SerializeField] private GameObject mesh;
    [SerializeField] private String name;

    private void Awake()
    {
        if (mesh == null)
        {
            mesh = gameObject;
        }
    }

    public override string ToString()
    {
        return name;
    }

    public Vector2Int GetPosition()
    {
        Assert.IsTrue(Cell != null);

        if (Cell == null)
        {
            return new Vector2Int(Int32.MaxValue, Int32.MaxValue);
        }

        return Cell.GetPosition();
    }

    public void setCell(GridCell gridcell, bool updateRot)
    {
        if (updateRot)
        {
            var direction = gridcell.GetPosition() - GetPosition();
            mesh.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, 0),
                new Vector3(0, 0, -1));
        }

        Cell = gridcell;
    }

    public void resetRotation()
    {
        mesh.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}