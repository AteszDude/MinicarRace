using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public Racer player;
    
    [SerializeField] private List<Racer> players; // This stores the order of players and it could be shuffled
    private List<Racer> activePlayers; // Currently in race
    private List<Racer> winners;

    private Racer[] endResult;
    private int firstPlace;
    private int lastPlace;
    
    private int nextPlayer = -1;

    [SerializeField] private GameObject menuUI;
    [SerializeField] private TMP_Text menuText;


    private void Start()
    {
        if (SceneInfo.CustomAINUM)
        {
            int diff = players.Count - SceneInfo.AI - 2;
            Assert.IsTrue(diff >= 0);

            if (diff > 0)
            {
                players.RemoveRange(players.Count - diff, diff);
            }
        }

        activePlayers = new List<Racer>(players);
        endResult = new Racer[players.Count];
        GameGrid.Instance.SetPlayerPos(activePlayers.ToArray());

        lastPlace = players.Count - 1;
        
        //TO DO: refactor this small hack 
        foreach(var player in players)
        {
            if(player.GetComponent<AIController>() != null)
                player.transform.Rotate(new Vector3(-90, 0, 0));
            else
            {
                player.transform.GetChild(0).Rotate(new Vector3(-90, 0, 0));
            }
        }
        
        continueTurn();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void continueTurn()
    {
        //True for AI players, false for players or when callback is needed
        Boolean repeat = true;

        while(repeat)
        {
            
            if (activePlayers.Count == 0)
            {
                EndGame();
                return;
            }            
            
            nextPlayer++;

            if (nextPlayer == activePlayers.Count)
                nextPlayer = 0;
            

            AIController ai = activePlayers[nextPlayer].GetComponent<AIController>();

            if (ai != null)
            {
                if (!ai.MakeMove())
                {
                    RemovePlayer(activePlayers[nextPlayer], false);
                }
                else if (activePlayers[nextPlayer].Cell.type == GridCell.GridType.Finish)
                    RemovePlayer(activePlayers[nextPlayer], true);

            }
            else
            {
                repeat = false;
                movePlayer();
            }
        }
    }

    private void movePlayer()
    {
        var reachableGrids = GameGrid.Instance.GetReachableGrids(player.GetPosition(), player.Speed, true);

        if (reachableGrids.Length == 0)
        {
            Debug.Log("No reachable grids!");
            RemovePlayer(player, false);
            continueTurn();
        }
        else
        {
            foreach (var rgrid in reachableGrids)
            {
                rgrid.ReadyForSelect();
            }
        }
    }

    // TO DO refactor
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayerMoved(GridCell toCell)
    {
        GameGrid.Instance.ResetGrid();
        
        player.Cell.RemoveRacer();
        if (toCell.AssignRacer(player))
        {
            RemovePlayer(player, true);
        }
        
        continueTurn();
    }

    private void EndGame()
    {
        menuUI.SetActive(true);
        Debug.Log("Game ended!");
    }

    private void SetPositionText(String text)
    {
        menuText.text = text;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
    }
    
    private void RemovePlayer(Racer racer, bool didFinish)
    {
        if (didFinish)
        {

            endResult[firstPlace] = racer;
            SetPositionText(racer + " finished " + (firstPlace + 1));
            firstPlace++;
        }
        else
        {
            endResult[lastPlace] = racer;
            SetPositionText(racer + " finished " + (lastPlace + 1));
            lastPlace--;
        }
        activePlayers.RemoveAt(nextPlayer);
        nextPlayer--;
    }
}