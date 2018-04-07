using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Manages the events that take place on the game board.
/// </summary>
public class BoardManager : MonoBehaviour, INotifyPropertyChanged
{

    private static XDocument _doc = XDocument.Load("GameBoards.xml");
    private XElement _smallBoard =_doc.Root.Element("SmallBoard");
    private XElement _bigBoard = _doc.Root.Element("BigBoard");
    private AI _ai;
    private AudioSource chargeSound = new AudioSource();
    public List<Hexagon> Hexagons = new List<Hexagon>();
    public GameObject hexagonPrefab;
    public GameObject chargePrefab;
    public CoreGameplay cg;

    public event PropertyChangedEventHandler PropertyChanged;

    private void Start()
    {
        cg.PropertyChanged += new PropertyChangedEventHandler(eventHandler); // Subscribe to CoreGameplay's event system
        LoadBoard(true);
        chargeSound = GetComponent<AudioSource>();
        chargeSound.time = 1.3f;
    }

    /// <summary>
    /// Reads data from a .xml file and dynamically loads the board
    /// </summary>
    /// <param name="size"></param>
    private void LoadBoard(bool bigSmall) // small = true, big = false
    {
        DrawHexagons(bigSmall);
        DrawCharges(bigSmall);
        AddAdjacentNeighbors(bigSmall);
    }

    /// <summary>
    /// Will draw the hexagons given an XDocument to read from
    /// </summary>
    /// <param name="columns"></param>
    private void DrawHexagons(bool bigSmall)
    {
        int x = -1, y = -1, numHexes = -1;
        IEnumerable<XElement> boardColumns = null;

        try
        {
            if (bigSmall) // This means we are drawing the small board
            {
                boardColumns = _smallBoard.Descendants().Where(attribute => attribute.Name == "Column"); // Grab all descendants with the name 'Column' under 'SmallBoard' tag
            }

            // Iterate through the columns
            foreach (var column in boardColumns)
            {
                x = int.Parse(column.Element("x").Value.ToString());
                y = int.Parse(column.Element("y").Value.ToString());
                numHexes = int.Parse(column.Element("numHexes").Value.ToString());
                DrawVertical(numHexes, x, y);
            }
        }
        catch(Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Draww charges on the board
    /// </summary>
    /// <param name="bigSmall"> Whether to draw the big or small board</param>
    private void DrawCharges(bool bigSmall)
    {
        int hexIndex = -1, numRows = -1;
        string drawThree = string.Empty;
        IEnumerable<XElement> charges = null;

        try
        {
            if (bigSmall) // We are drawing the small board
            {
                charges = _smallBoard.Descendants().Where(attribute => attribute.Name == "Charge");
            }
            else // Big board 
            {
                charges = _bigBoard.Descendants().Where(attribute => attribute.Name == "Charge");
            }


            // Iterate through all the charge tags
            foreach (var chargesToDraw in charges)
            {
                hexIndex = int.Parse(chargesToDraw.Element("hexIndex").Value.ToString());
                numRows = int.Parse(chargesToDraw.Element("numRows").Value.ToString());
                drawThree = chargesToDraw.Element("draw3").Value.ToString();

                if (string.Equals(drawThree, "true", StringComparison.InvariantCultureIgnoreCase))
                {
                    DrawThree(Hexagons[hexIndex]);
                }
                else
                {
                    DrawTwo(Hexagons[hexIndex], numRows);
                }

            }
        }
        catch(Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Adds the adjacent neighbors to each of the lists of neighbors for each hexagon on the board.
    /// Reads the information from the xml file which gives an index to the hex to add neighbors to
    /// as well as the neighbors of that hexagon in a string that will be parsed
    /// </summary>
    private void AddAdjacentNeighbors(bool bigSmall)
    {
        XElement neighborElement = null;
        IEnumerable<XElement> hexesToAddNeighbors = null;
        IEnumerable<string> neighborsList = null;

        int index = -1;
        string neighborsString = string.Empty;

        if(bigSmall) // We are reading information for the small board
        {
            neighborElement = _smallBoard.Element("neighbors");
        }
        else // big board
        {
            neighborElement = _bigBoard.Element("neighbors");
        }

        // Grab the list of elements that contain the index of the hex to add neighbors to as well as the 
        // string containing the neighbors to add
        hexesToAddNeighbors = neighborElement.Descendants().Where(attribute => attribute.Name == "hex");

        // Iterate through list of hexes that we need to add neighbors to
        foreach(var hex in hexesToAddNeighbors)
        {
            index = int.Parse(hex.Element("index").Value.ToString());
            neighborsString = hex.Element("neighborList").Value.ToString();
            neighborsList = ParseNeigborsString(neighborsString);

            // Iterate through neighbors list. Max number of neighbors is 6
            foreach(var neighborIndex in neighborsList)
            {
                var neighbor = Hexagons[Convert.ToInt32(neighborIndex)];
                Hexagons[index].AdjacentNeighbors.Add(neighbor);
            }
        }

    }

    /// <summary>
    /// Will parse the string containing the neighbors to add to the hex, convert to ints, and return a list
    /// of those integers. Neighbors are comma-separated
    /// </summary>
    /// <param name="neighborsString"></param>
    /// <returns></returns>
    private IEnumerable<string> ParseNeigborsString(string neighborsString)
    {
        IEnumerable<string> values = neighborsString.Split(',');
        return values;
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

    /// <summary>
    /// Instantiates a hexagon at the given X and Y positions
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
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
    /// Checks all of the hexagons to see if they have been clicked on or not.
    /// If a hex has been clicked, this method will return true and the gameplay class will
    /// call the board manager's helper method to change the color.
    /// Anything handling hexagons and attribute changes will happen in the board manager class
    /// This function returns a reference to the hexagon that was changed so we check for open charges from CoreGameplay
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private Hexagon CheckCollisionChangeColor(float x, float y, Player player)
    {
        Hexagon retValue = null;
        bool tryAgain = false;
        bool clickedOnHex = false;
        float yUpperBound = 0, yLowerBound = 0, xLeftBound = 0, xRightBound = 0;
        foreach (Hexagon hex in Hexagons)
        {

            yUpperBound = (hex.y - 1f);
            yLowerBound = (hex.y + 0.5f);
            xLeftBound = (hex.x - 0.6f);
            xRightBound = (hex.x + 0.6f);

            // Check to see if x and y positions are at the location of the click
            // Also check to make sure the hex has been clicked by the correct player
            if ((x > xLeftBound && x < xRightBound) && (y > yUpperBound && y < yLowerBound))
            {
                clickedOnHex = true;
                if (hex.HexOwner == null)
                {
                    hex.HexOwner = player;
                }
                else if (hex.HexOwner.PlayerName != player.PlayerName)
                {
                    // Maybe display a prompt when we hit this case.
                    // Opposite player cannot click and take a hex that has already been taken
                    tryAgain = true;
                }

                if(!tryAgain)
                {

                    if (string.Equals(player.PlayerName, "player1", StringComparison.InvariantCultureIgnoreCase)) // If the player is player 1
                    {
                        ChangeToRed(hex); // Change to red
                        cg.ResetTryAgain(); // If we have tried a move again, must reset because the move is now successful
                    }
                    else // Player 2
                    {
                        ChangeToBlue(hex); // Change to blue
                        cg.ResetTryAgain();
                    }
                    retValue = hex;
                    tryAgain = false;
                }

            }
        }

        if(!clickedOnHex) // If the user clicked somewhere that is not a hex location
        {
            TryAgain();
        }
        else if (tryAgain) // If the user clicked on a hex that is not theirs
        {
            TryAgain();
        }

        return retValue;

    }

    /// <summary>
    /// Method that is called whenever an event happens
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void eventHandler(object sender, PropertyChangedEventArgs e)
    {
       
        if (string.Equals(e.PropertyName, "Mouse Clicked", StringComparison.InvariantCultureIgnoreCase))
        {
            float mouseX = cg.GetMouseXPos();
            float mouseY = cg.GetMouseYPos();
            int index = -5;
            Player currentPlayer = cg.GetCurrentPlayer();

            Hexagon currentHex = CheckCollisionChangeColor(mouseX, mouseY, currentPlayer);

            if (currentHex != null)
            {
                index = FindOpenCharge(currentHex);
                Charge(index, currentHex);
                index = FindOpenCharge(currentHex); // Check to see if there are no more open charges after charge
            }


            if (index == -1)
            {
                StartCoroutine (ExplodeAdjacentNeighbors(currentHex, currentPlayer));
                ResetExplodedHex(currentHex);
               
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
        chargeSound.Play();
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

    /// <summary>
    /// Changes all adjacent neighbors of exploding hex to that color of the player who
    /// exploded the hexagon
    /// </summary>
    /// <param name="hex"></param>
    private IEnumerator ExplodeAdjacentNeighbors(Hexagon hex, Player player)
    {
        int index = -1;

        if(string.Equals(hex.HexOwner.PlayerName, "player1", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return(StartCoroutine(FlashRedHexOnExplosion(hex)));
        }
        else
        {
            yield return (StartCoroutine(FlashBlueHexOnExplosion(hex)));
        }
        
        
        for (int i = 0; i < hex.AdjacentNeighbors.Count; i++)
        {
            hex.AdjacentNeighbors[i].IsCharged = true;
            hex.AdjacentNeighbors[i].HexOwner = player;
            index = FindOpenCharge(hex.AdjacentNeighbors[i]);


            if (string.Equals(player.PlayerName, "player1", StringComparison.InvariantCultureIgnoreCase))
            {
                ChangeToRed(hex.AdjacentNeighbors[i]);
                if (index >= 0) // We need to add 1 charge to all adjacent hexagons, so make sure there is an open charge
                {
                    Charge(index, hex.AdjacentNeighbors[i]);
                }
            }
            else
            {
                ChangeToBlue(hex.AdjacentNeighbors[i]);
                if (index >= 0) // We need to add 1 charge to all adjacent hexagons, so make sure there is an open charge
                {
                    Charge(index, hex.AdjacentNeighbors[i]);
                }
            }

        }

        CheckBoardForExplosion(); // After we explode into adjacent neighbors, see if we have other explosions
        CheckIfBlueWon();
        CheckIfRedWon();
    }

    /// <summary>
    /// Changes hex color to red
    /// </summary>
    /// <param name="hex"></param>
    private void ChangeToRed(Hexagon hex)
    {
        MeshRenderer mr = hex.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(204, 0, 0, 0);
    }

    /// <summary>
    /// Changes hex color to red
    /// </summary>
    /// <param name="hex"></param>
    private void ChangeToBlue(Hexagon hex)
    {
        MeshRenderer mr = hex.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(0, 76, 153, 0);
    }

    /// <summary>
    /// Changes hex color to light blue. WIll be
    /// </summary>
    /// <param name="hex"></param>
    private void ChangeToLightBlue(Hexagon hex)
    {
        MeshRenderer mr = hex.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(204, 255, 255, 0);
    }


    /// <summary>
    /// Changes hex color to light blue. WIll be
    /// </summary>
    /// <param name="hex"></param>
    private void ChangeToLightRed(Hexagon hex)
    {
        MeshRenderer mr = hex.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(255, 204, 204, 0);
    }



    /// <summary>
    /// Resets all of the charges to their default colors
    /// Also resets the hex that just exploded
    /// </summary>
    /// <param name="hex"></param>
    private void ResetExplodedHex(Hexagon hex)
    {
        MeshRenderer mr;
        mr = hex.GetComponent<MeshRenderer>();
        mr.material.color = new Color32(137, 137, 137, 0); // Change color back to default

        hex.HexOwner = null; // Nobody owns this hex anymore, so set to null

        // Reset all the charges back to default
        foreach (Hexagon charge in hex.Charges)
        {
            mr = charge.GetComponent<MeshRenderer>();
            mr.material.color = new Color32(32, 32, 32, 0); // Change back to grey
            charge.IsCharged = false;
        }

    }

    /// <summary>
    /// Will check the adjacent neighbors of a recently exploded hexagon
    /// to see if it needs to explode. If it does, then we will explode the adjacent neighbors of that hex
    /// </summary>
    /// <param name="hex"></param>
    private void CheckBoardForExplosion()
    {
        int index = -5;

        foreach(Hexagon hex in Hexagons)
        {
            index = FindOpenCharge(hex);

            if(index < 0) // If there is an explosion available
            {
                StartCoroutine(ExplodeAdjacentNeighbors(hex, cg.GetCurrentPlayer()));
            }
        }

    }

    

    /// <summary>
    /// If a player clicks on a hex that is not theirs, we do not want to switch turns.
    /// So we need to call a method in core gameplay that retries with the current player
    /// </summary>
    private void TryAgain()
    {
        cg.TryAgain();
    }


    /// <summary>
    /// Will change color from light red to red multiple times to simulate flashing
    /// before the hex explodes into adjacent neighbors
    /// </summary>
    /// <param name="hex"></param>
    private IEnumerator FlashRedHexOnExplosion(Hexagon hex)
    {
        ChangeToLightRed(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToRed(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToLightRed(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToRed(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToLightRed(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToRed(hex);
        yield return new WaitForSeconds(.09f);
        ResetExplodedHex(hex);

    }


    /// <summary>
    /// Will change color from light blue to blue multiple times to simulate flashing
    /// before the hex explodes into adjacent neighbors
    /// </summary>
    /// <param name="hex"></param>
    private IEnumerator FlashBlueHexOnExplosion(Hexagon hex)
    {
        ChangeToLightBlue(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToBlue(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToLightBlue(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToBlue(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToLightBlue(hex);
        yield return new WaitForSeconds(.09f);
        ChangeToBlue(hex);
        yield return new WaitForSeconds(.09f);
        ResetExplodedHex(hex);

    }

    /// <summary>
    /// Notifies subscribers when an important event happens.
    /// This will be used to tell the CoreGameplay class that the game has ended.
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
    /// Checks to see if the blue player won. If so, notify subscribers that this game state has changed
    /// </summary>
    private void CheckIfBlueWon()
    {
        foreach(Hexagon hex in Hexagons)
        {
            if(hex.HexOwner != null && string.Equals(hex.HexOwner.PlayerColor, "red", StringComparison.InvariantCultureIgnoreCase)) // found a red hex, blue player hasn't won
            {
                return;
            }
        }

        NotifyPropertyChanged(this, "blue wins");
    }

    /// <summary>
    /// Checks to see if the red player won. If so, notify subscribers that this game state has changed
    /// </summary>
    private void CheckIfRedWon()
    {
        foreach (Hexagon hex in Hexagons)
        {
            if (hex.HexOwner != null && string.Equals(hex.HexOwner.PlayerColor, "blue", StringComparison.InvariantCultureIgnoreCase)) // found a blue hex, red player hasn't won
            {
                return;
            }
        }

        NotifyPropertyChanged(this, "red wins");
    }

    /// <summary>
    /// Set the AI object used in the coregameplay object
    /// </summary>
    /// <param name="newAI"></param>
    public void SetAI(AI newAI)
    {
        _ai = newAI;
        _ai.PropertyChanged += new PropertyChangedEventHandler(eventHandler); // Subscribe to the AI's event system. May need to add check to see if ai is a part of game
    }

}


