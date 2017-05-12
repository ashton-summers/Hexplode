using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Hexagon> Hexagons;
    public GameObject hexagonPrefab;

    private void Start()
    {
        DrawBoard();
    }

    // Draws the board onto the playspace
    private void DrawBoard()
    {
  
        // Have at most 5 columns for a small board. Can extend to make larger ones in future.
        // Just need to know start point of column (which can be hardcoded since it's not detrimental) to draw a column
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
        
        Hexagons.Add(h);
    }
}
