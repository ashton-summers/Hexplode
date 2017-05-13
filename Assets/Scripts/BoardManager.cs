using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Hexagon> Hexagons;
    public GameObject hexagonPrefab;
    public GameObject chargePrefab;

    //Color c = new Color(255, 51, 51);
    //mr.material.color = new Color32(255, 51, 51, 0);

 
    private void Start()
    {
        DrawBoard();
        DrawCharges();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawBoard()
    {

        // Have at most 5 columns for a small board. Can extend to make larger ones in future.
        for (int i = 1; i < 6; i++)
        {
            switch (i)
            {
                case 1:
                    DrawVertical(i, -4, 2);
                    break;
                case 2:
                    DrawVertical(i, -3, 3);
                    break;
                case 3:
                    DrawVertical(i, -2, 4);
                    break;
                case 4:
                    DrawVertical(i - 2, -1, 3);
                    break;
                case 5:
                    DrawVertical(i - 4, 0, 2);
                    break;

            }
        }
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
        Hexagon h = go.GetComponent<Hexagon>();
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

}

