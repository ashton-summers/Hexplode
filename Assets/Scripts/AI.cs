﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

/// <summary>
/// AI class that plays against another player.
/// </summary>
public class AI : MonoBehaviour, INotifyPropertyChanged
{
    /// <summary>
    /// Color of the player or AI in this case
    /// </summary>
    private string _playerColor;

    /// <summary>
    /// Name of the player, or AI in this case
    /// </summary>
    private string _playerName;
    
    /// <summary>
    /// Whether or not the AI's turn is over
    /// </summary>
    public bool turnIsOver = true;

    /// <summary>
    /// Event handler to notify subscribers when things happen
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    
    /// <summary>
    /// Constructs a new AI object
    /// </summary>
    /// <param name="newPlayerName"></param>
    /// <param name="newPlayerColor"></param>
    public AI(string newPlayerName, string newPlayerColor)
    {
        _playerColor = newPlayerColor;
        _playerName = newPlayerName;
    }

    /// <summary>
    /// Gets the player (in this case the AI) color
    /// </summary>
    public string PlayerColor
    {
        get
        {
            return _playerColor;
        }
    }

    /// <summary>
    /// Gets the name of the player
    /// </summary>
    public string PlayerName
    {
        get
        {
            return _playerName;
        }

    }

    /// <summary>
    /// Selects a move to make from the board.
    /// Implements the interface this way we can add different AI later if needed/wanted.
    /// </summary>
    public IEnumerator SelectMove()
    {
        System.Random r = new System.Random();
        BoardManager boardManager = GameObject.FindGameObjectWithTag("board").GetComponent<BoardManager>();
        CoreGameplay coreGameplay = GameObject.FindGameObjectWithTag("coreGame").GetComponent<CoreGameplay>();
        Hexagon hex = null;
        
        int numHexesOnBoard = boardManager.Hexagons.Count;
        int randNum = r.Next(0, numHexesOnBoard);
        bool aiHasFoundSpot = false;
        float x, y;

        turnIsOver = false;

        while (!aiHasFoundSpot)
        {
            // If the hex is not yet occupied or the player name is set to player 2. AI will ALWAYS be second player
            if (boardManager.Hexagons[randNum].HexOwner == null || boardManager.Hexagons[randNum].HexOwner.PlayerName == "player2")
            {
                // Add logic here to change mouse position using the selected hex and then notify subscribers
                hex = boardManager.Hexagons[randNum];
                aiHasFoundSpot = true;
            }
            else
            {
                randNum = r.Next(0, numHexesOnBoard);
            }
        }
        
        yield return new WaitForSeconds(3.0f);
        x = boardManager.Hexagons[randNum].x;
        y = boardManager.Hexagons[randNum].y;
        coreGameplay.AIChangeMousePos(x, y);
        NotifyPropertyChanged(this, "Mouse Clicked"); // AI has 'clicked' on a hexagon, tell the board manager
        turnIsOver = true;

    }

    /// <summary>
    /// Notifies all subscribers of the event system for the AI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventHappened"></param>
    private void NotifyPropertyChanged(object sender, string eventHappened)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(sender, new PropertyChangedEventArgs(eventHappened));
        }
    }
}
