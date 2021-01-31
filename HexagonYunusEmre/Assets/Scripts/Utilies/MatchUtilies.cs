using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchUtilies
{
    #region SearchMatch
    public static List<Cell> SearchUp(Cell item, Cell[,] cells, int column, int row)
    {
        List<Cell> upItems = new List<Cell> { item };
        int left = item.Column - 1;
        int right = item.Column + 1;
        int upRow=item.Row;
        if (item.Column%2==0)
        {
            //Debug.Log("Case2");
            upRow = item.Row + 1;
        }

        if (item.Row+1<row && left>=0 && cells[left, upRow].ColorType==item.ColorType )
        {
            if (cells[item.Column, item.Row+1].ColorType==item.ColorType)//Up
            {
                upItems.Add(cells[left, upRow]);
                upItems.Add(cells[item.Column, item.Row+1]);
                //Debug.Log("UpLeft");
            }
        }else if (item.Row+1<row && right<column && cells[right, upRow].ColorType==item.ColorType)
        {
            if (cells[item.Column, item.Row+1].ColorType==item.ColorType)
            {
                upItems.Add(cells[right, upRow]);
                upItems.Add(cells[item.Column, item.Row+1]);
                //Debug.Log("UpRight");
            }
        }

        return upItems;
    }
    
    public static List<Cell> SearchDown(Cell item, Cell[,] cells, int column, int row)
    {
        List<Cell> downItems = new List<Cell> { item };
        int left = item.Column - 1;
        int right = item.Column + 1;
        int downRow=item.Row-1;
        if (item.Column%2==0)
        {
            //Debug.Log("Case2");
            downRow = item.Row;
        }

        if (item.Row-1>=0 && left>=0 && cells[left, downRow].ColorType==item.ColorType)
        {
            if (cells[item.Column, item.Row-1].ColorType==item.ColorType)//Down
            {
                downItems.Add(cells[left, downRow]);
                downItems.Add(cells[item.Column, item.Row-1]);
                //Debug.Log("DownLeft");
            }
            
        }else if (item.Row-1>=0 && right<column && cells[right, downRow].ColorType==item.ColorType)
        {
            if (cells[item.Column, item.Row-1].ColorType==item.ColorType)
            {
                downItems.Add(cells[right, downRow]);
                downItems.Add(cells[item.Column, item.Row-1]);
                //Debug.Log("DownRight");
            }
        }
        
        return downItems;
    }
    
    public static List<Cell> SearchLeftRight(Cell item, Cell[,] cells, int column, int row)
    {
        List<Cell> leftRightItems = new List<Cell> { item };
        int left = item.Column - 1;
        int right = item.Column + 1;
        int rLRow=item.Row;
        if (item.Column%2==0)
        {
            //Debug.Log("Case2");
            rLRow = item.Row + 1;
        }

        if (rLRow < row && left >=0 &&  cells[left, rLRow].ColorType==item.ColorType )
        {
            if (rLRow-1 >= 0 && cells[left, rLRow-1].ColorType==item.ColorType)//Left
            {
                leftRightItems.Add(cells[left, rLRow]);
                leftRightItems.Add(cells[left, rLRow-1]);
                //Debug.Log("Left");
            }
        }else if (rLRow < row && right < column && cells[right, rLRow].ColorType==item.ColorType)
        {
            if (rLRow-1 >= 0 && cells[right, rLRow-1].ColorType==item.ColorType)//Right
            {
                leftRightItems.Add(cells[right, rLRow]);
                leftRightItems.Add(cells[right, rLRow-1]);
                //Debug.Log("Right");
            }
        }
        return leftRightItems;
    }
    
    #endregion
    
    #region AfterMatch

    public static void UnSelectHex(List<Cell> selectedHex)
    {
        foreach (var i in selectedHex)
        {
            i.SelectedBackground.SetActive(false);
        }
    }
    

    #endregion
}
