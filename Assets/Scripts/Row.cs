using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    private Cell[] cells;

    public bool IsRowComplete()
    {
        cells = GetComponentsInChildren<Cell>();

        int filledCells = 0;

        foreach (Cell cell in cells)
        {
            if (cell.IsFull())
            {
                filledCells++;
            }
        }

        if (filledCells == 10)
        {
            return true;
        }
        else return false;
    }
}