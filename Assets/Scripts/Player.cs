using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IPlayer
{ 
    private string _name;
    private string _color;

    public Player(string newPlayerName, string newPlayerColor)
    {
        _name = newPlayerName;
        _color = newPlayerColor;
    }


	public string PlayerName
    {
        get { return _name; }
    }

    public string PlayerColor
    {
        get { return _color; }
    }
}
