using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnUtilies
{

    #region Prepare
    public static Vector3 GetMidPoint(List<Cell> selectedHex)
    {
        var pos1 = selectedHex[0].transform.position;
        var pos2 = selectedHex[1].transform.position;
        var pos3 = selectedHex[2].transform.position;
        
        var midPoint = new Vector3 {x = (pos1.x + pos2.x + pos3.x) / 3, 
            y = (pos1.y + pos2.y + pos3.y) / 3};
        return midPoint;
    }

    public static Cell[] GetTarget(float angle, List<Cell> selectedHex,GameObject selectedDomain, int column)
    {
        Cell[] targets = new Cell[3];
        targets[0]=null;
        targets[1]=null;
        targets[2]=null;
        

        byte columnMaxCounter=0;
        byte columnMinCounter=0;
        foreach (var item in selectedHex)
        {
            if (item.Column==column-1)
            {
                columnMaxCounter++;
            }
            if (item.Column==0)
            {
                columnMinCounter++;
            }
        }
        
        if (((selectedDomain.name =="Corner2" ||  selectedDomain.name =="Corner4" || 
              selectedDomain.name =="Corner6") && columnMaxCounter!=2) || columnMinCounter==2)
        {
            //Debug.Log("D");
            
            foreach (var selected in selectedHex)
            {
                if (selected.Column == MinMaxWork.GetMinimumX(selectedHex) && 
                    selected.Row == MinMaxWork.GetMaximumY(selectedHex))
                {
                    targets[0] = selected;
                    //Debug.Log("T1 selected to "+selected);
                }else if (selected.Column == MinMaxWork.GetMaximumX(selectedHex))
                {
                    if (angle>0 && targets[1]!=selected)
                    { targets[2] = selected; }
                    else
                    { targets[1] = selected; }
                }
                else
                {
                    if (angle>0 && targets[2]!=selected)
                    { targets[1] = selected;}
                    else
                    { targets[2] = selected;}
                }
            }
        }
        else
        {
            //Debug.Log("Reverse D");
            
            foreach (var selected in selectedHex)
            {
                if (selected.Column == MinMaxWork.GetMaximumX(selectedHex) && 
                    selected.Row == MinMaxWork.GetMaximumY(selectedHex))
                {
                    targets[0] = selected;
                }else if (selected.Column == MinMaxWork.GetMaximumX(selectedHex) && 
                          selected.Row == MinMaxWork.GetMinimumY(selectedHex))
                {
                    if (angle>0)
                    { targets[2] = selected; }
                    else
                    { targets[1] = selected; }
                }
                else
                {
                    if (angle>0)
                    { targets[1] = selected; }
                    else
                    { targets[2] = selected; }
                }
            }
            
        }
        
        //Debug.Log("0:"+selectedHex[0].name+"1:"+selectedHex[1].name+"2:"+selectedHex[2].name);
        //Debug.Log("T1:"+targets[0].name+"T2:"+targets[1].name+"T3:"+targets[2].name);
        return targets;
    }
    #endregion

    #region After
    public static void ChangeName(Cell[] selectedItems, Cell[,] cells)
    {
        //Debug.Log("T1:"+selectedItem[0].name+"T2:"+selectedItem[1].name+"T3:"+selectedItem[2].name);
        byte firstColumn = selectedItems[0].Column;
        byte firstRow = selectedItems[0].Row;
        
        selectedItems[0].OnChangedPosition(selectedItems[1].Column, selectedItems[1].Row);
        selectedItems[1].OnChangedPosition(selectedItems[2].Column, selectedItems[2].Row);
        selectedItems[2].OnChangedPosition(firstColumn, firstRow);

        ChangeCellsList(selectedItems, cells);
        //Debug.Log("T1:"+selectedItem[0].name+"T2:"+selectedItem[1].name+"T3:"+selectedItem[2].name);
    }

    private static void ChangeCellsList(Cell[] selectedItems, Cell[,] cells)
    {
        foreach (var item in selectedItems)
        {
            cells[item.Column, item.Row] = item;
            //Debug.Log(item.Column+","+item.Row +"/"+cells[item.Column, item.Row].name);
        }
    }
    #endregion

}
