using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CoreGameplay : MonoBehaviour, INotifyPropertyChanged
{

    
    private Vector2 mouseOver;
    private Player _player1 = new Player("player1", "Red");
    private Player _player2 = new Player("player2", "blue");
    private Player _currentPlayer;
    public event PropertyChangedEventHandler PropertyChanged;

    // Use this for initialization
    public void StartGame ()
    {

        NotifyPropertyChanged(this, "Start");
        mouseOver = new Vector2();
		
	}

    /// <summary>
    /// Notifies subsribers of the event that happened.
    /// This way, the core gameplay class does not have to know anything about a BoardManager object
    /// Also allows the board manager class to be self contained
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

    /// <summary>
    /// Main game loop
    /// </summary>
    private void Update()
    {
        UpdateMouseOver();

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;

        // TODO: add logic for two player game later
        // Maybe add AI
        if (Input.GetMouseButtonDown(0))
        {
            _currentPlayer = _player1;
            NotifyPropertyChanged(this, "Mouse Clicked");

           
        }

    }


    /// <summary>
    /// 
    /// </summary>
    private void UpdateMouseOver()
    {
        
        RaycastHit hit;
        // If we are interacting with the board, update the Vector2 for mouse position
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, LayerMask.GetMask("Hex")))
        {
            mouseOver.x = (int)hit.point.x;
            mouseOver.y = (int)hit.point.y;
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    /// <summary>
    /// Returns the current mouse x pos
    /// </summary>
    /// <returns></returns>
    public int GetMouseXPos()
    {
        return Convert.ToInt32(mouseOver.x);
    }

    /// <summary>
    /// Returns the current mouse y pos
    /// </summary>
    /// <returns></returns>
    public int GetMouseYPos()
    {
        return Convert.ToInt32(mouseOver.y);
    }

    /// <summary>
    /// Returns the current player who is executing action
    /// </summary>
    /// <returns></returns>
    public Player GetCurrentPlayer()
    {
        return _currentPlayer;
    }

}
