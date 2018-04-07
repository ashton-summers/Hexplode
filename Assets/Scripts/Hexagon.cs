using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{

    /// <summary>
    /// Backing field for x position
    /// </summary>
    private float _x;

    /// <summary>
    /// Backing field for y position.
    /// </summary>
    private float _y;

    /// <summary>
    /// Backing field for whether or not this hex is charged.
    /// </summary>
    private bool _isCharged;

    /// <summary>
    /// Which player owns this hex.
    /// </summary>
    private Player _owner;

    /// <summary>
    /// List of charges for this hex.
    /// </summary>
    public List<Hexagon> Charges;

    /// <summary>
    /// List of hexes that are adjacent to this.
    /// </summary>
    public List<Hexagon> AdjacentNeighbors;


    /// <summary>
    /// Gets or sets the x position
    /// </summary>
    public float x
    {
        get { return _x; }
        set { _x = value; }
    }

    /// <summary>
    /// Gets or sets the y position.
    /// </summary>
    public float y
    {
        get { return _y; }
        set { _y = value; }
    }

    /// <summary>
    /// Whether or not this hex is charged.
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
