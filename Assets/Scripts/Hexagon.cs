using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{

    public List<Hexagon> Charges;
    public List<Hexagon> AdjacentNeighbors;
    private float _x;
    private float _y;
    private bool _isCharged;
    private Player _owner;

  

    /// <summary>
    /// 
    /// </summary>
    public float x
    {
        get { return _x; }
        set { _x = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public float y
    {
        get { return _y; }
        set { _y = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCharged
    {
        get { return _isCharged; }
        set { _isCharged = value; }
    }

 

    /// <summary>
    /// Reference to the player who has captured the hex
    /// </summary>
    public Player HexOwner
    {
        get { return _owner; }
        set { _owner = value; }
    }


}
