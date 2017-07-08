using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CoreGameplay : MonoBehaviour, INotifyPropertyChanged
{

    
    private Vector2 _mouseOver;
    private Player _player1 = new Player("player1", "red");
    private Player _player2 = new Player("player2", "blue");
    private AI _ai = new AI("player2", "blue");
    private bool _tryAgain = false;
    private bool _aiGame = true;
    private bool _player1TurnOver = false;
    public Player _currentPlayer;
    public bool _player1Turn = true;
    public BoardManager boardManager;
    public event PropertyChangedEventHandler PropertyChanged;

    // Use this for initialization
    public void Start()
    {
        _mouseOver = new Vector2();

        // Subscribe to the board manager's event notification system
        boardManager.PropertyChanged += new PropertyChangedEventHandler(EventHandler);
        boardManager.SetAI(_ai);
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

        int x = (int)_mouseOver.x;
        int y = (int)_mouseOver.y;

        if(_aiGame)
        {
            if ((_player1Turn || _tryAgain) && _ai.turnIsOver) // If it's player one's turn or player 1 needs to try a move again
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentPlayer = _player1;
                    _player1TurnOver = false;
                    NotifyPropertyChanged(this, "Mouse Clicked");
                    if(!_tryAgain)
                    {
                        _player1Turn = false;
                    }
                    StartCoroutine(WaitForPlayer1Coroutine());
                }
            }
            else // It is player two's turn
            {
                if (!_tryAgain && _ai.turnIsOver && _player1TurnOver) // Make sure that the AI turn is already over and that player 1 does not need to try again
                {
                    _currentPlayer = _player2;
                    StartCoroutine(_ai.SelectMove());
                    _player1Turn = true;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            
            if(_player1Turn && !_tryAgain)
            {
                _currentPlayer = _player1;
                _player1Turn = false;
            }
            else // It is player two's turn
            {
                if (!_tryAgain)
                {
                    _currentPlayer = _player2;
                    _player1Turn = true;
                }
            }
            NotifyPropertyChanged(this, "Mouse Clicked");
        }

    }

    /// <summary>
    /// A routine that waits 3 seconds.
    /// This is used to stop the ai from choosing a hex when the player's
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForPlayer1Coroutine()
    {
        yield return new WaitForSeconds(2.0f);
        _player1TurnOver = true;

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
            _mouseOver.x = hit.point.x;
            _mouseOver.y = hit.point.y;
        }
        else
        {
            _mouseOver.x = -1;
            _mouseOver.y = -1;
        }
    }

    /// <summary>
    /// Returns the current mouse x pos
    /// </summary>
    /// <returns></returns>
    public float GetMouseXPos()
    {
        return _mouseOver.x;
    }

    /// <summary>
    /// Returns the current mouse y pos
    /// </summary>
    /// <returns></returns>
    public float GetMouseYPos()
    {
        return _mouseOver.y;
    }

    /// <summary>
    /// Returns the current player who is executing action
    /// </summary>
    /// <returns></returns>
    public Player GetCurrentPlayer()
    {
        return _currentPlayer;
    }

    /// <summary>
    /// If a player doesn't click on a hex or clicks on a hex
    /// that is not theirs, we need to switch back to their turn
    /// </summary>
    public void TryAgain()
    {
        _tryAgain = true;
      
    }

    public void ResetTryAgain()
    {
        _tryAgain = false;
    }

    /// <summary>
    /// Event handler for important events that may be sent from the board manager.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EventHandler(object sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, "blue wins", StringComparison.InvariantCultureIgnoreCase))
        {
            SceneManager.LoadScene("EndGameScene");
        }
    }

    /// <summary>
    /// This function provides a means of letting the AI change the mouse x and y pos
    /// so when an event is raised to the board manager, the correct coordinates are used.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AIChangeMousePos(float x, float y)
    {
        _mouseOver.x = x;
        _mouseOver.y = y;
    }

}
