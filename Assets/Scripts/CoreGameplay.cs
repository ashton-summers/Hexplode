using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main game class that includes main logic for the game.
/// </summary>
public class CoreGameplay : MonoBehaviour, INotifyPropertyChanged
{

    /// <summary>
    /// Vector containing mouse position.
    /// </summary>
    private Vector2 _mouseOver;

    /// <summary>
    /// 1st player instance.
    /// </summary>
    private Player _player1 = new Player("player1", "red");

    /// <summary>
    /// Second player instance. 
    /// </summary>
    private Player _player2 = new Player("player2", "blue");

    /// <summary>
    /// An AI to use if applicable.
    /// </summary>
    private AI _ai = new AI("player2", "blue");

    /// <summary>
    /// Bool to tell whether a move needs to be tried again.
    /// </summary>
    private bool _tryAgain = false;

    /// <summary>
    /// Whether or not we are playing against AI.
    /// </summary>
    private bool _aiGame = true;

    /// <summary>
    /// Whether or not player 1's turn is over.
    /// </summary>
    private bool _player1TurnOver = false;

    /// <summary>
    /// Instance of the current player whose turn it is.
    /// </summary>
    public Player _currentPlayer;

    /// <summary>
    /// Whether or not it is player 1's turn.
    /// </summary>
    public bool _player1Turn = true;

    /// <summary>
    /// Reference to the board manager that runs the game logic.
    /// </summary>
    public BoardManager boardManager;

    /// <summary>
    /// Event notifying the board manager and other subscribers when events take place
    /// and what those events were.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// UI element for whose turn it is.
    /// </summary>
    public Text playerTurnText;

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
    /// <param name="sender"> Who invoked the event</param>
    /// <param name="eventHappened">A string describing the event that took place</param>
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
                playerTurnText.text = "Player 1";
                playerTurnText.color = new Color32(204, 0, 0, 255);
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
                playerTurnText.text = "Player 2"; // Change the player turn text to match whose turn it is
                playerTurnText.color = new Color32(0, 76, 153, 255); // Change the color of the text to blue
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
    /// <returns>An enumerator as required by yield keyword</returns>
    private IEnumerator WaitForPlayer1Coroutine()
    {
        yield return new WaitForSeconds(2.0f);
        _player1TurnOver = true;

    }

    /// <summary>
    /// Calculates position of mouse using raycasts.
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

    /// <summary>
    /// Resets the _tryAgain variable.
    /// </summary>
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
            SceneManager.LoadScene("BlueWinsEndGameScene");
        }
        else if (string.Equals(e.PropertyName, "red wins", StringComparison.InvariantCultureIgnoreCase))
        {
            SceneManager.LoadScene("RedWinsEndGameScene");
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
