using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AIController : MonoBehaviour
    {

    private Racer racer;
    
    [SerializeField] private int maxDepth = 7;
    [SerializeField] private int precision; // How much mistake can he make in the last move, 0 = none
    [SerializeField] private int[] edgeAvoidance = {4, 2, 1};
    
    void Start()
    {
        racer = GetComponent<Racer>();
        Assert.IsNotNull(racer);
    }
    
    public bool MakeMove()
    {
        (Vector2Int nextMove, bool canMove) = Move(racer);

        if (!canMove)
            return false;
        
        racer.Cell.RemoveRacer();
        GameGrid.Instance.getGrid(nextMove.x, nextMove.y).AssignRacer(racer);
        
        return true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private (Vector2Int, bool) Move(Racer racer)
    {
        HashSet <Step> steps = new HashSet<Step>();

        Assert.IsTrue(maxDepth > 0);
        
        //DEPTH 1

        var firstGrids = GameGrid.Instance.GetReachableGrids(racer.GetPosition(), racer.Speed, true);

        foreach (var grid in firstGrids)
        {
            Step step = new Step(grid.GetPosition(),
                racer.Speed + (grid.GetPosition() - racer.GetPosition()), grid.GetPosition());
            steps.Add(step);
            //Debug.Log(step);
        }

        if (steps.Count == 0)
            return (new Vector2Int(0, 0), false);
        
        //DEPTH 2 AND UP

        for (int depth = 2; depth <= maxDepth; depth++)
        {
            HashSet<Step> newSet = new HashSet<Step>();

            foreach (var step in steps)
            {
                var nextGrids = GameGrid.Instance.GetReachableGrids(step.pos, step.speed, false);

                foreach (var grid in nextGrids)
                {
                    if (grid.type == GridCell.GridType.Finish)
                        return (step.firstStep, true);

                    Step newStep = new Step(grid.GetPosition(), step.speed + (step.pos - grid.GetPosition()), step.firstStep);
                    newSet.Add(newStep);
                }
            }
            steps = new HashSet<Step>(newSet);
        }

        if (steps.Count > 0)
        {
            //Debug.Log(steps.Count);

            return (GameGrid.Instance.GETBestStep(Utility.Shuffle(steps.ToList()), precision, edgeAvoidance).firstStep, true);
        }

        return (new Vector2Int(0, 0), false);
        
    }
}
