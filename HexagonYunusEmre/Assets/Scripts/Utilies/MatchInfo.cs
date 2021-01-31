using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo
{

    #region Variables
    public List<Cell> match;
    
    #region X1 Location
    public int matchX1;
    public int matchStartingY1;
    public int matchEndingY1;
    #endregion
    
    #region X2 Location
    public int matchX2;
    public int matchStartingY2;
    public int matchEndingY2;
    #endregion

    #endregion

    public bool validMatch
    {
        get { return match != null; }
    }
}
