using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinMaxWork
{
    #region GetMinMaxIndex
    public static int GetMinimumX(List<Cell> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].Column;
        }
        return (int)Mathf.Min(indices);
    }

    public static int GetMaximumX(List<Cell> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].Column;
        }
        return (int)Mathf.Max(indices);
    }

    public static int GetMinimumY(List<Cell> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].Row;
        }
        return (int)Mathf.Min(indices);
    }

    public static int GetMaximumY(List<Cell> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].Row;
        }
        return (int)Mathf.Max(indices);
    }
    #endregion

    #region MatchedItem'sGetMinMaxIndex 

    public static int GetMatchedMinimumY(List<Cell> items, int column)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            if (items[i].Column == column)
            {
                indices[i] = items[i].Row;   
            }
            else
            {
                indices[i] = 99f;
            }
        }
        return (int)Mathf.Min(indices);
    }

    public static int GetMatchedMaximumY(List<Cell> items, int column)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            if (items[i].Column == column)
            {
                indices[i] = items[i].Row;
            }
            else
            {
                indices[i] = 0;
            }
        }
        return (int)Mathf.Max(indices);
    }
    

    #endregion
    
}
