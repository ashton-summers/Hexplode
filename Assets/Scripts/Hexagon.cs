using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    
    public List<Hexagon> Charges;
    public List<Hexagon> AdjacentNeighbors;
    public float x;
    public float y;
    public bool isCharged;

    private void Start()
    {
        Charges = new List<Hexagon>(); // charges are essentially just hexagons that have two states: charged or not charged
        AdjacentNeighbors = new List<Hexagon>();
        isCharged = false;
    }

 
}
