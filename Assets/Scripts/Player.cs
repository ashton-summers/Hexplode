using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all data relevant to a player in the game.
/// </summary>
public class Player
{ 
    /// <summary>
    /// Name of the player
    /// </summary>
    protected string _name;

    /// <summary>
    /// Color of the player
    /// </summary>
    protected string _color;

    /// <summary>
    /// Constructs a new player object
    /// </summary>
    /// <param name="newPlayerName"> New player name for created instance.</param>
    /// <param name="newPlayerColor"> New player color for the created instance.</param>
    public Player(string newPlayerName, string newPlayerColor)
    {
        _name = newPlayerName;
        _color = newPlayerColor;
    }

    /// <summary>
    /// Exposes the _name backing field
    /// </summary>
	public string PlayerName
    {
        get { return _name; }
    }

    /// <summary>
    /// Exposes the color backing field
    /// </summary>
    public string PlayerColor
    {
        get { return _color; }
    }
}
