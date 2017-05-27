using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
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
        set { _name = value; }
    }

    public string PlayerColor
    {
        get { return _color; }
        set { _color = value; }
    }
}
