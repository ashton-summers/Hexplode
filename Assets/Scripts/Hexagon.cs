using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    
    public int Charged;
    public Color Color;
    public List<Hexagon> Charges;
    public List<Hexagon> AdjacentNeighbors;


    private void Start()
    {
        Charges = new List<Hexagon>(); // charges are essentially just hexagons that have two states: charged or not charged
        AdjacentNeighbors = new List<Hexagon>();
        Charged = 0; // 0 means hex is not charged. 
        Color = new Color(255, 0, 255);
    }
}
