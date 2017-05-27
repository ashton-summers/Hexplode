using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Hexagon> Hexagons = new List<Hexagon>();
    public GameObject hexagonPrefab;
    public GameObject chargePrefab;
    public CoreGameplay cg;

    //Color c = new Color(255, 51, 51);
    //mr.material.color = new Color32(255, 51, 51, 0);

    private void Start()
    {
        cg.PropertyChanged += new PropertyChangedEventHandler(eventHandler);
        cg.StartGame();
    }

    /// <summary>
    /// Proceduraly draws the board on the screen
    /// </summary>
    private void DrawBoard()
    {

        // Have at most 5 columns for a small board. Can extend to make larger ones in future.
        for (int i = 1; i < 6; i++)
        {
            switch (i)
            {
                case 1:
                    DrawVertical(i, -2, 2);
                    break;
                case 2:
                    DrawVertical(i, -1, 3);
                    break;
                case 3:
                    DrawVertical(i, 0, 4);
                    break;
                case 4:
                    DrawVertical(i - 2, 1, 3);
                    break;
                case 5:
                    DrawVertical(i - 4, 2, 2);
                    break;

            }
        }
        DrawCharges();
        AddAdjacentNeighbors();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawCharges()
    {
        MeshRenderer m = new MeshRenderer();
        for (int i = 0; i < Hexagons.Count; i++)
        {
            Hexagon temp = Hexagons[i];
            switch (i)
            {
                case 0:
                    DrawTwo(temp, 1);
                    break;
                case 1:
                   DrawTwo(temp, 2);
                    break;
                case 2:
                    DrawTwo(temp, 2);
                    break;
                case 3:
                    DrawThree(temp);
                    break;
                case 4:
                    DrawTwo(temp, 3);
                    break;
                case 5:
                    DrawThree(temp);
                    break;
                case 6:
                    DrawTwo(temp, 2);
                    break;
                case 7:
                    DrawTwo(temp, 2);
                    break;
                case 8:
                    DrawTwo(temp, 1);
                    break;

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="temp"> reference to the hexagon we are drawing charges for. Needed to add to list of hex's charges</param>
    /// <param name="numRows"> number of rows of two to draw </param>
    private void DrawTwo(Hexagon temp, int numRows)
    {
        float x = temp.x;
        float y = temp.y;
        for (int i = 0; i < numRows; i++)
        {
            GenerateCharge(temp, x - 0.1f, y + 0.3f);
            GenerateCharge(temp, x + 0.1f, y + 0.3f);
            y -= 0.3f;
        }
    }

    /// <summary>
    /// Draws three charges onto the given hexagon
    /// </summary>
    /// <param name="temp"></param>
    private void DrawThree(Hexagon temp)
    {
        float x = temp.x;
        float y = temp.y;
        GenerateCharge(temp, x, y - 0.05f);
        DrawTwo(temp, 1);
    }

   

    // Draws a vertical line of hexagons based on the x and y given. Draws number of hexagons specified.
    // This will be useful if we want to extend to make larger boards.
    // Starts with top hex first, then draws all below it
    private void DrawVertical(int numHexes, int x, int y)
    {
        // Generate the first hex (top hex) at x and y.
        GenerateHexagon(x, y);
        numHexes--;
        for (int i = 0; i < numHexes; i++)
        {
            GenerateHexagon(x, y - 2); // Generate all the hexes below the top one
            y -= 2;
        }
    }

    // Instantiates a hexagon at location x and y
    private void GenerateHexagon(int x, int y)
    {
        Vector3 vTemp = new Vector3();
        GameObject go;


        vTemp.Set(x, y, -1.5f);


        go = Instantiate(hexagonPrefab) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = vTemp;
        Hexagon h = new Hexagon();
        h = go.GetComponent<Hexagon>();
        h.x = x; h.y = y;

        Hexagons.Add(h);
    }

    /// <summary>
    /// Generates a charge on top of the referenced hexagon
    /// </summary>
    /// <param name="hex"> reference to the hexagon we are drawing on top of </param>
    /// <param name="x"> x position </param>
    /// <param name="y"> y position </param>
    private void GenerateCharge(Hexagon hex, float x, float y)
    {

        Vector3 chargeTemp = new Vector3();
        GameObject go;

        chargeTemp.Set(x, y, -1.54f);


        go = Instantiate(chargePrefab) as GameObject;
        go.transform.SetParent(transform);

        Hexagon h = go.GetComponent<Hexagon>();
        h.x = x; h.y = y;
        
        // Scale the charges down so they are small and set their position.
        go.transform.localScale = new Vector3(0.06f, 0.03f, 1f);
        go.transform.position = chargeTemp;

        // Set default color of the charge to dark gray.
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(32, 32, 32, 0);

        hex.Charges.Add(h);
       
    }

    /// <summary>
    /// Adds the adjacent neighbors to each of the lists of neighbors for each hexagon on the board
    /// Could make this more efficient by dynamically reading this information from an xml script. Not
    /// too worried about that though
    /// </summary>
    private void AddAdjacentNeighbors()
    {
        Hexagons[0].AdjacentNeighbors.Add(Hexagons[1]);
        Hexagons[0].AdjacentNeighbors.Add(Hexagons[2]);

        Hexagons[1].AdjacentNeighbors.Add(Hexagons[0]);
        Hexagons[1].AdjacentNeighbors.Add(Hexagons[2]);
        Hexagons[1].AdjacentNeighbors.Add(Hexagons[3]);
        Hexagons[1].AdjacentNeighbors.Add(Hexagons[4]);

        Hexagons[2].AdjacentNeighbors.Add(Hexagons[0]);
        Hexagons[2].AdjacentNeighbors.Add(Hexagons[1]);
        Hexagons[2].AdjacentNeighbors.Add(Hexagons[4]);
        Hexagons[2].AdjacentNeighbors.Add(Hexagons[5]);

        Hexagons[3].AdjacentNeighbors.Add(Hexagons[1]);
        Hexagons[3].AdjacentNeighbors.Add(Hexagons[4]);
        Hexagons[3].AdjacentNeighbors.Add(Hexagons[6]);

        Hexagons[4].AdjacentNeighbors.Add(Hexagons[1]);
        Hexagons[4].AdjacentNeighbors.Add(Hexagons[3]);
        Hexagons[4].AdjacentNeighbors.Add(Hexagons[6]);
        Hexagons[4].AdjacentNeighbors.Add(Hexagons[7]);
        Hexagons[4].AdjacentNeighbors.Add(Hexagons[5]);
        Hexagons[4].AdjacentNeighbors.Add(Hexagons[2]);

        Hexagons[5].AdjacentNeighbors.Add(Hexagons[4]);
        Hexagons[5].AdjacentNeighbors.Add(Hexagons[2]);
        Hexagons[5].AdjacentNeighbors.Add(Hexagons[7]);

        Hexagons[6].AdjacentNeighbors.Add(Hexagons[3]);
        Hexagons[6].AdjacentNeighbors.Add(Hexagons[4]);
        Hexagons[6].AdjacentNeighbors.Add(Hexagons[7]);
        Hexagons[6].AdjacentNeighbors.Add(Hexagons[8]);

        Hexagons[7].AdjacentNeighbors.Add(Hexagons[5]);
        Hexagons[7].AdjacentNeighbors.Add(Hexagons[4]);
        Hexagons[7].AdjacentNeighbors.Add(Hexagons[6]);
        Hexagons[7].AdjacentNeighbors.Add(Hexagons[8]);

        Hexagons[8].AdjacentNeighbors.Add(Hexagons[6]);
        Hexagons[8].AdjacentNeighbors.Add(Hexagons[7]);

    }

    /// <summary>
    /// Checks all of the hexagons to see if they have been clicked on or not.
    /// If a hex has been clicked, this method will return true and the gameplay class will
    /// call the board manager's helper method to change the color.
    /// Anything handling hexagons and attribute changes will happen in the board manager class
    /// This function returns a reference to the hexagon that was changed so we check for open charges from CoreGameplay
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private Hexagon CheckCollisionChangeColor(int x, int y, Player player)
    {
        Hexagon retValue = null;
        foreach (Hexagon hex in Hexagons)
        {
            // Check to see if x and y positions are at the location of the click
            // Also check to make sure the hex has been clicked by the correct player
            if (hex.x == x && hex.y == y)
            {
                if (hex.HexOwner == null)
                {
                    hex.HexOwner = player;
                }
                else if (hex.HexOwner.PlayerName != player.PlayerName)
                {
                    // Maybe display a prompt when we hit this case.
                    // Opposite player cannot click and take a hex that has already been taken
                    return hex;
                }
                    MeshRenderer mr = hex.GetComponent<MeshRenderer>();

                    if (string.Equals(player.PlayerName, "player1", StringComparison.InvariantCultureIgnoreCase)) // If the player is player 1
                    {
                        mr.material.color = new Color32(255, 51, 51, 0); // Change to red
                    }
                    else // Player 2
                    {
                        mr.material.color = new Color32(0, 128, 255, 0); // Change to blue
                    }
                retValue = hex;
            }
        }

        return retValue;

    }

    private void eventHandler(object sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == "Start")
        {
            DrawBoard();
        }
        else if (e.PropertyName == "Mouse Clicked")
        {
            int mouseX = cg.GetMouseXPos();
            int mouseY = cg.GetMouseYPos();
            int index = -1;
            Player currentPlayer = cg.GetCurrentPlayer();

            Hexagon currentHex = CheckCollisionChangeColor(mouseX, mouseY, currentPlayer);

            if(currentHex != null)
            {
                index = FindOpenCharge(currentHex);
            }

            if(index < 0) // There is no open charge. Means we should explode into adjacent neighbors
            {
                return;
            }
            else
            {
                Charge(index, currentHex);
            }
        }

    }

    /// <summary>
    /// Charges the charge at the specified index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="hex"></param>
    private void Charge(int index, Hexagon hex)
    {
        MeshRenderer mr = hex.Charges[index].GetComponent<MeshRenderer>();
        mr.material.color = new Color32(255, 255, 255, 0); // Change the charge to white.
        hex.Charges[index].IsCharged = true;
    }

    /// <summary>
    /// Finds the first open charge that we can charge up, returns index of that charge
    /// If this function return -1, then we need to explode this charge
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    private int FindOpenCharge(Hexagon hex)
    {
        for (int i = 0; i < hex.Charges.Count; i++)
        {
            if (!hex.Charges[i].IsCharged)
            {
                return i;
            }
        }

        return -1;
    }
}

