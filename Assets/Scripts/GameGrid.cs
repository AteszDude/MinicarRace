using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

public class GameGrid : MonoBehaviour
{
    private static GameGrid _instance;
    public static GameGrid Instance => _instance;

    [SerializeField] private Texture2D[] maps;
    private int height;
    private int width;

    private float gridSpaceSize = 5f;

    [SerializeField]
    private int maxSpeed = 7;
    
    [SerializeField] private GameObject gridCellPrefab;
    private GridCell[,] gameGrid;

    //This caches distances, the key is an array of int
    private Dictionary<String, int> distances;
    private int lastDistance;
    
    //Pathfinding
    private List<GridCell> finishes;
    private List<GridCell> starts;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        CreateGrid();
    }

    /// <summary>
    ///Get or create a new key in the entry
    /// </summary>
    private int getDistanceKey(int[] edgeFilter)
    {
        if (edgeFilter == null || edgeFilter.Length == 0)
            return 0;
        
        String key = string.Join("-", edgeFilter);

        if (distances.ContainsKey(key))
            return distances[key];
        
        lastDistance++;
        distances.Add(key, lastDistance);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gameGrid[x, y].AddLayer();
            }
        }

        MakeDistances(edgeFilter, lastDistance);
        
        return lastDistance;
    }
    
    /// <summary>
    ///Creates a grid from texture and inits a layer of distances
    /// </summary>
    private void CreateGrid()
    {
        Texture2D map = maps[SceneInfo.Map];
        
        width = map.width;
        height = map.height;

        gameGrid = new GridCell[width, height];
        finishes = new List<GridCell>();
        distances = new Dictionary<String, int>();
        distances.Add("", 0);
        
        if (gridCellPrefab == null)
        {
            Debug.LogError("Eror!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelcolor = map.GetPixel(x, y);

                GridCell.GridType type = GridCell.GridType.Racetrack;

                if (pixelcolor == Color.red)
                    type = GridCell.GridType.Finish;
                else if (pixelcolor == Color.white)
                    type = GridCell.GridType.Unoccupiable;

                GameObject newGrid = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, y * gridSpaceSize),
                    Quaternion.identity);

                gameGrid[x, y] = newGrid.GetComponent<GridCell>();
                gameGrid[x, y].SetGrid(x, y, type);
                gameGrid[x, y].transform.parent = transform;
                gameGrid[x, y].AddLayer();
                
                if (type == GridCell.GridType.Finish)
                {
                    finishes.Add(gameGrid[x, y]);
                }
            }
        }
        MakeDistances(null, 0);
    }

    public GridCell getGrid(int x, int y)
    {
        return gameGrid[x, y].GetComponent<GridCell>();
    }
    
    public GridCell getGrid(Vector2Int pos)
    {
        return gameGrid[pos.x, pos.y].GetComponent<GridCell>();
    }


    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(x, 0, height);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * gridSpaceSize;
        float y = gridPos.y * gridSpaceSize;

        return new Vector3(x, 0, y);
    }

    public void SetPlayerPos(Racer[] players)
    {
        List<GridCell> startingLine = new List<GridCell>();

        for (int i = 0; i < width; i++)
        {
            GridCell grid = gameGrid[i, 0].GetComponent<GridCell>();
            if (grid.type != GridCell.GridType.Unoccupiable)
            {
                startingLine.Add(grid);
            }
        }
        Utility.Shuffle(startingLine);
        
        if (startingLine.Count < players.Length)
        {
            Debug.Log("Problem with start line");
            return;
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].setCell(startingLine[i], false);
            startingLine[i].AssignRacer(players[i]);
            players[i].Speed = new Vector2Int(0, 0);
        }
        
    }

    public void ResetGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gameGrid[x, y].GetComponent<GridCell>().ResetSelection();
            }
        }
    }

    public Step GETBestStep(List<Step> steps, int precision, int[] edgeFilter)
    {
        Assert.IsTrue(steps.Count > 0);
        Assert.IsTrue(finishes.Count > 0);

        int layer = getDistanceKey(edgeFilter);
        
        Step bestStep = steps[steps.Count - 1];
        int distance = getGrid(bestStep.pos).GETDistance(layer);

        steps.RemoveAt(steps.Count - 1);
        
        //Get distance from finish
        foreach (var step in steps)
        {
            //float dist = Vector2Int.Distance(step.pos, target.GetPosition());
            int dist = getGrid(step.pos).GETDistance(layer);
            if (dist < distance - precision)
            {
                distance = dist;
                bestStep = step;
            }
        }

        //Debug.Log(bestStep);
        
        return bestStep;
    }
    
    /// <summary>
    /// Creates a new layer of distance array
    /// </summary>
    private void MakeDistances([NotNull] IReadOnlyList<int>
        inc, int layer)
    {
        if (inc == null) throw new ArgumentNullException(nameof(inc));
        
        Debug.Log("Distances called");
        
        List<GridCell> nextPoints = new List<GridCell>(finishes);
        
        for (int distance = 0; nextPoints.Count > 0; distance++)
        {
            HashSet<GridCell> newPoints = new HashSet<GridCell>();
            
            foreach(var grid in nextPoints)
            {
                grid.SetDistance(layer,distance);
            }
            
            foreach(var grid in nextPoints)
            {
                Vector2Int pos = grid.GetPosition();
                
                if(pos.x > 0 && getGrid(pos.x - 1, pos.y).GETDistance(layer) < 0)
                    newPoints.Add(getGrid(pos.x - 1, pos.y));

                if(pos.y > 0 && getGrid(pos.x, pos.y - 1).GETDistance(layer) < 0)
                    newPoints.Add(getGrid(pos.x, pos.y - 1));

                if(pos.x < width - 1 && getGrid(pos.x + 1, pos.y).GETDistance(layer) < 0)
                    newPoints.Add(getGrid(pos.x + 1, pos.y));

                if(pos.y < height - 1 && getGrid(pos.x, pos.y + 1).GETDistance(layer) < 0)
                    newPoints.Add(getGrid(pos.x, pos.y + 1));

            }
            nextPoints = new List<GridCell>(newPoints);
        }

        if (inc == null || inc.Count == 0)
            return;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (getGrid(x, y).type == GridCell.GridType.Unoccupiable)
                {
                    for (int i = 0; x + i + 1 < width && i < inc.Count &&
                                        getGrid(x + i + 1, y).type == GridCell.GridType.Racetrack; i++)
                    {
                        getGrid(x + i + 1, y).AddDistance(layer, inc[i]);
                    }
                    for (int i = 0; x - i - 1> 0 && i < inc.Count &&
                                        getGrid(x - i - 1, y).type == GridCell.GridType.Racetrack; i++)
                    {
                        getGrid(x - i - 1, y).AddDistance(layer, inc[i]);
                    }
                    
                    for (int i = 0; y + i + 1< height && i < inc.Count &&
                                        getGrid(x, y + i + 1).type == GridCell.GridType.Racetrack; i++)
                    {
                        getGrid(x, y + i + 1).AddDistance(layer, inc[i]);
                    }
                    for (int i = 0; y - i - 1> 0 && i < inc.Count &&
                                        getGrid(x, y - i - 1).type == GridCell.GridType.Racetrack; i++)
                    {
                        getGrid(x, y - i - 1).AddDistance(layer, inc[i]);
                    }
                    
                }
            }
        }

    }
    
    /// <summary>
    /// Checks if the 9x9 field is free
    /// </summary>
    /// <param name="racerCounts">True if racers count as obstruction, otherwise false</param>
    public GridCell[] GetReachableGrids(Vector2Int pos, Vector2Int speed, bool racerCounts)
    {
        List<GridCell> result = new List<GridCell>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (speed.x > maxSpeed)
                    speed.x = maxSpeed;
                if (speed.y > maxSpeed)
                    speed.y = maxSpeed;
                
                int newx = pos.x + speed.x + i;
                int newy = pos.y + speed.y + j;

                //Found a new element
                if (newx >= 0 && newx < width && newy >= 0 && newy < height && !(newx == pos.x && newy == pos.y))
                {
                    if (gameGrid[newx, newy].GetComponent<GridCell>().CanRacerOccupy(racerCounts))
                    {
                        result.Add(gameGrid[newx, newy].GetComponent<GridCell>());
                    }
                }
            }
        }
        return result.ToArray();
    }
}