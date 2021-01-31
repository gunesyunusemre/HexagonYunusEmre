using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class GameCell : MonoBehaviour
{
    public static GameCell gameCell;

    #region Variable

    #region Prepare Game
    [Header("Prepare Game")]
    [SerializeField] private List<GameObject> hexagons;
    [SerializeField] private List<GameObject> bombs;
    [SerializeField] private byte column = 8;
    [SerializeField] private byte row = 9;
    [SerializeField] private int bombScore = 1000;
    #endregion

    #region MidGame
    [Header("Mid Game")]
    [Tooltip("this variable is required for the wait time between matches.")]
    [SerializeField] private float delayBetweenMatches = 1f;
    [Tooltip("This variable required for select domain")]
    [SerializeField] [Range(0,1)]private float domainRadius=0;
    #endregion

    #region Core Variable
    private List<Cell> selectedHex = new List<Cell>();
    private Cell[,] cells;
    private GameObject selectedDomain;
    #endregion

    #region SwipeVariable
    private Vector2 startPosition;
    private Vector2 endPosition;
    //This variable working GetAngle Method
    #endregion

    #region Booleans
    private bool isSelect=false;
    private bool canPlay=true;
    private bool isGameOver=false;
    private bool isGameOpen = true;
    #endregion

    #region Others
    //Capsule Method************
    public byte Column { get { return column; } }
    public byte Row { get { return row; } }
    //**************************
    
    private byte turnCounter = 0;//This variable for Turn count-1  

    //*****Ground Vectors*************
    private Vector3[] groundsPos;
    //*******************************
    

    #endregion
    
    #endregion

    #region Events
    public delegate void OnMoveCountUp();
    public static event OnMoveCountUp OnMoveCountUpEventHandler;
    
    public delegate void OnScoreUp();
    public static event OnScoreUp OnScoreUpEventHandler;
    #endregion

    #region MonoBehaviour Function

    private void OnEnable()
    {
        if (gameCell==null)
        {
            gameCell = this;
        }
        
        Cell.GameOverEventHandler += GameOver;
        Cell.OnMouseOverItemEventHandler += OnMouseOverItem;
    }

    private void OnDisable()
    {
        Cell.OnMouseOverItemEventHandler -= OnMouseOverItem;
        Cell.GameOverEventHandler -= GameOver;

        SaveBoard();
    }


    private void Start()
    {
        groundsPos = new Vector3[column];

        LoadSystem();
        CamWork();
    }

    private void Update()
    {
        GetAngle();
    }

    #endregion

    #region Start Mechanics
    private void FillHexagons()
    {
        cells = new Cell[column, row];
        for (byte i = 0; i < column; i++)
        {
            for (byte j = 0; j < row; j++)
            {
                cells[i, j] = InstantiateCell(i, j);
                if (j==0)
                {
                    groundsPos[i] = cells[i, j].transform.position;
                }
            }
        }
    }
    
    private void FillResumeHexagons()
    {
        cells = new Cell[column, row];
        for (byte i = 0; i < column; i++)
        {
            for (byte j = 0; j < row; j++)
            {
                string location = i + "," + j;
                cells[i,j]=InstantiateResumeCell(i, j, PlayerPrefs.GetInt(location));
                if (j==0)
                {
                    groundsPos[i] = cells[i, j].transform.position;
                }
            }
        }
    }
    
    private void ClearMatchAllBoard()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                MatchInfo matchInfo = GetMatchInformation(cells[x, y]);
                if (matchInfo.validMatch)
                {
                    Destroy(cells[x, y].gameObject);
                    cells[x, y] = InstantiateCell(x, y);
                    y--;
                }
            }
        }
    }
    
    private void LoadSystem()
    {
        if (PlayerPrefs.HasKey("0,0"))
        {
            FillResumeHexagons();
        }
        else
        {
            FillHexagons();
            ClearMatchAllBoard();
        }
    }

    private void CamWork()
    {
        Vector3 camTarget =  (cells[column - 1, row - 1].transform.position - cells[0, 0].transform.position)/2;
        camTarget.y = cells[column - 1, row - 1].transform.position.y*2 / 3;
        camTarget.z = -10;
        MoveCenter.CamScript.SetCam(camTarget);
    }

    #endregion

    #region EndMechanics

    private void GameOver()
    {
        isGameOver = true;
        canPlay = false;
        StartCoroutine(BreakBoard());
    }

    private IEnumerator BreakBoard()
    {
        yield return new WaitForSeconds(2f);
        MatchUtilies.UnSelectHex(selectedHex);
        ChangeRigidBodyStatus(true);
    }
    
    private void SaveBoard()
    {
        if (isGameOver)
            return;

        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                string location = i + "," + j;
                PlayerPrefs.SetInt(location, cells[i, j].ColorType);
                if (cells[i, j].IsBomb)
                {
                    PlayerPrefs.SetInt(i+","+j+"isBomb", 1);
                    PlayerPrefs.SetInt(i+","+j+"bombCount", cells[i, j].BombCount);
                }
                else
                {
                    PlayerPrefs.SetInt(i+","+j+"isBomb", 0);
                }
                
            }
        }
    }

    #endregion
    
    #region CreatedDestroyMechanics
    private Cell InstantiateCell(int i, int j)
    {
        int colorType = Random.Range(0, hexagons.Count);
        Cell thisCell;
        
        if (Player._player.Score/bombScore > Player._player.BombCounter)
        {
            Player._player.BombCounter++;
            //Debug.Log("Bomb has the planted.");
            
            thisCell = Instantiate(bombs[colorType],
                i % 2 == 0 ? new Vector3(i / 1.1f, (j + 0.5f) / 1f) : new Vector3(i / 1.1f, j / 1f),
                quaternion.identity).GetComponent<Cell>();
            
        }
        else
        {
            
            thisCell = Instantiate(hexagons[colorType],
                i % 2 == 0 ? new Vector3(i / 1.1f, ((j + 0.5f) / 1f)) : new Vector3(i / 1.1f, (j / 1f)),
                quaternion.identity).GetComponent<Cell>();
        }
        thisCell.OnChangedPosition((byte)i,(byte)j);
        thisCell.transform.parent = this.transform;
        thisCell.ColorType = (byte) colorType;
        thisCell.BombCount=Random.Range(5, 8);
        
        return thisCell;
    }
    
    private Cell InstantiateResumeCell(int i, int j, int colorType)
    {
        Cell thisCell;
        
        if (PlayerPrefs.GetInt(i+","+j+"isBomb")==1)
        {
            thisCell = Instantiate(bombs[colorType],
                i % 2 == 0 ? new Vector3(i / 1.1f, (j + 0.5f) / 1f) : new Vector3(i / 1.1f, j / 1f),
                quaternion.identity).GetComponent<Cell>();
            
            thisCell.BombCount=PlayerPrefs.GetInt(i+","+j+"bombCount");

        }
        else
        {
            
            thisCell = Instantiate(hexagons[colorType],
                i % 2 == 0 ? new Vector3(i / 1.1f, ((j + 0.5f) / 1f)) : new Vector3(i / 1.1f, (j / 1f)),
                quaternion.identity).GetComponent<Cell>();
        }
        thisCell.OnChangedPosition((byte)i,(byte)j);
        thisCell.transform.parent = this.transform;
        thisCell.ColorType = (byte) colorType;
        
        return thisCell;
    }

    IEnumerator DestroyItems(List<Cell> items)
    {
        foreach (Cell i in items)
        {
            //Debug.Log(i.name+": "+i.ColorType);
            yield return StartCoroutine(i.transform.Scale(Vector3.zero, 0.095f));
        }
        foreach (var i in items) { Destroy(i.gameObject); }
    }
    #endregion

    #region SelectDomain
    private void OnMouseOverItem(Cell item)
    {
        if (!canPlay || isGameOver)
            return;

        //Debug.Log("("+item.Column+","+item.Row+")" +" Color: "+item.ColorType);
        
        SelectDomain(item);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(selectedDomain.transform.position, domainRadius);

        if (colliders.Length>3) return;

        foreach (var selected in selectedHex)
        {
            if (selected==null)
                break;
            
            selected.SelectedBackground.SetActive(false);
        }
        selectedHex.Clear();

        for (int i = 0; i <3 ; i++)
        {
            selectedHex.Add(colliders[i].GetComponent<Cell>());
            //selectedHex[i] = colliders[i].GetComponent<Cell>();
            selectedHex[i].SelectedBackground.SetActive(true);
            //Debug.Log(colliders[i].name);
        }
    }

    private void SelectDomain(Cell item)
    {
        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dis = 999f;
        foreach (var corner in item.Corners)
        {
            if (!(Vector2.Distance(mousePos, corner.position) < dis)) continue;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(corner.position, domainRadius);
            
            if (colliders.Length < 3 ) continue;
            
            dis = Vector2.Distance(mousePos, corner.position);
            selectedDomain = corner.gameObject;
            isSelect = true;
        }

        //Debug.Log(selectedDomain.name);
    }
    

    #endregion

    #region TurnMechanics
    private void GetAngle()
    {
        if (!isSelect || !canPlay || isGameOver) return;

        if (selectedHex.Count==0) return;

        if(Input.GetMouseButtonDown(0))
        {
            startPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dis = endPosition - startPosition;
            float angle = math.atan2(dis.y,dis.x) * Mathf.Rad2Deg;
            
            if (isGameOpen)
            {
                isGameOpen = false;
                dis = new Vector2(0, 0);
            }
            
            if (dis.magnitude > 2f)
            {
                turnCounter = 0;
                Player._player.MoveCounter++;
                //Debug.Log(Player._player.MoveCounter);
                if (OnMoveCountUpEventHandler != null) OnMoveCountUpEventHandler();

                StartCoroutine(TurnDomain(angle));
            }

            //Debug.Log(angle);
        }
    }
    
    
    private IEnumerator TurnDomain(float angle)
    {
        isSelect = false;
        canPlay = false;
        Cell[] targets = TurnUtilies.GetTarget(angle, selectedHex, selectedDomain, column);
        
        if (angle>0)
        {
            yield return StartCoroutine(targets[0].transform.Turn(targets[1],targets[2],500, 
                Vector3.forward, TurnUtilies.GetMidPoint(selectedHex),targets));
        }
        else
        {
            yield return StartCoroutine(targets[0].transform.Turn(targets[1],targets[2],500, 
                Vector3.back, TurnUtilies.GetMidPoint(selectedHex),targets));
        }

        TurnUtilies.ChangeName(targets, cells);
        yield return StartCoroutine(ControlMatch(angle));
    }
    
    private void ChangeRigidBodyStatus(bool status)
    {
        foreach (Cell g in cells)
        {
            if (g==null)
                continue;
            
            g.gameObject.GetComponent<Rigidbody2D>().isKinematic = !status;
        }
    }

    #endregion

    #region MatchWork
    
    IEnumerator ControlMatch(float angle)
    {
        //Debug.Log("Match control");
        MatchInfo matchA = GetMatchInformation(selectedHex[0]);
        MatchInfo matchB = GetMatchInformation(selectedHex[1]);
        MatchInfo matchC = GetMatchInformation(selectedHex[2]);
        //Debug.Log(matchA.validMatch +":"+ matchB.validMatch +":"+matchC.validMatch);
        if (!matchA.validMatch && !matchB.validMatch && !matchC.validMatch)
        {
            if (turnCounter<2)
            {
                turnCounter++;
                canPlay = false;
                yield return new WaitForSeconds(0.3f);
                yield return StartCoroutine(TurnDomain(angle));
            }
            else {canPlay = true; isSelect = true; }
            
        }
        if (matchA.validMatch)
        {
            //point gain
            Player._player.Score += matchA.match.Count * 5;
            if (OnScoreUpEventHandler != null) OnScoreUpEventHandler();
            //Debug.Log(Player._player.Score);
            
            MatchUtilies.UnSelectHex(selectedHex);
            yield return StartCoroutine(DestroyItems(matchA.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine(UpdateCellsAfterMatch(matchA));
        }
        else if (matchB.validMatch)
        {
            //point gain
            Player._player.Score += matchB.match.Count * 5;
            if (OnScoreUpEventHandler != null) OnScoreUpEventHandler();
            //Debug.Log(Player._player.Score);
            
            MatchUtilies.UnSelectHex(selectedHex);
            yield return StartCoroutine(DestroyItems(matchB.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine(UpdateCellsAfterMatch(matchB));
        }
        else if (matchC.validMatch)
        {
            //point gain
            Player._player.Score += matchC.match.Count * 5;
            if (OnScoreUpEventHandler != null) OnScoreUpEventHandler();
            //Debug.Log(Player._player.Score);
            
            MatchUtilies.UnSelectHex(selectedHex);
            yield return StartCoroutine(DestroyItems(matchC.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine(UpdateCellsAfterMatch(matchC));
        }
        yield return new WaitForSeconds(delayBetweenMatches);
    }
    
    private MatchInfo GetMatchInformation(Cell item)
    {
        MatchInfo m = new MatchInfo {match = null};
        List<Cell> upMatch = MatchUtilies.SearchUp(item, cells, column, row);
        List<Cell> downMatch = MatchUtilies.SearchDown(item, cells, column, row);
        List<Cell> leftRightMatch = MatchUtilies.SearchLeftRight(item, cells, column, row);
        //left
        //right
        
        if (upMatch.Count >= 3 && upMatch.Count > downMatch.Count)
        {
            //Debug.Log("Up!");
            //Up match information

            m.matchX1=MinMaxWork.GetMinimumX(upMatch);
            m.matchStartingY1 = MinMaxWork.GetMatchedMinimumY(upMatch, m.matchX1);
            m.matchEndingY1 = MinMaxWork.GetMatchedMaximumY(upMatch, m.matchX1);
            m.matchX2=MinMaxWork.GetMaximumX(upMatch);
            m.matchStartingY2 = MinMaxWork.GetMatchedMinimumY(upMatch, m.matchX2);
            m.matchEndingY2 = MinMaxWork.GetMatchedMaximumY(upMatch, m.matchX2);
            
            m.match = upMatch;
        }else if (downMatch.Count >= 3 && downMatch.Count >leftRightMatch.Count )
        {
            //Debug.Log("Down!");
            //Down match information

            m.matchX1=MinMaxWork.GetMinimumX(downMatch);
            m.matchStartingY1 = MinMaxWork.GetMatchedMinimumY(downMatch, m.matchX1);
            m.matchEndingY1 = MinMaxWork.GetMatchedMaximumY(downMatch, m.matchX1);
            m.matchX2=MinMaxWork.GetMaximumX(downMatch);
            m.matchStartingY2 = MinMaxWork.GetMatchedMinimumY(downMatch, m.matchX2);
            m.matchEndingY2 = MinMaxWork.GetMatchedMaximumY(downMatch, m.matchX2);
            
            m.match = downMatch;
        }else if (leftRightMatch.Count>=3)
        {
            //Debug.Log("Left or Right!");
            //Left Or Right match information

            m.matchX1=MinMaxWork.GetMinimumX(leftRightMatch);
            m.matchStartingY1 = MinMaxWork.GetMatchedMinimumY(leftRightMatch, m.matchX1);
            m.matchEndingY1 = MinMaxWork.GetMatchedMaximumY(leftRightMatch, m.matchX1);
            m.matchX2=MinMaxWork.GetMaximumX(leftRightMatch);
            m.matchStartingY2 = MinMaxWork.GetMatchedMinimumY(leftRightMatch, m.matchX2);
            m.matchEndingY2 = MinMaxWork.GetMatchedMaximumY(leftRightMatch, m.matchX2);
            
            m.match = leftRightMatch;
        }
        return m;
    }
    
    private IEnumerator ControlMatchAllBoard()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                MatchInfo matchInfo = GetMatchInformation(cells[x, y]);
                if (matchInfo.validMatch)
                {
                    //point gain

                    Player._player.Score += matchInfo.match.Count * 5;
                    if (OnScoreUpEventHandler != null) OnScoreUpEventHandler();
                    //Debug.Log(Player._player.Score);

                    yield return StartCoroutine(DestroyItems(matchInfo.match));
                    yield return new WaitForSeconds(delayBetweenMatches);
                    yield return StartCoroutine(UpdateCellsAfterMatch(matchInfo));
                }
            }
        }
    }
    
    #endregion

    #region Create New Hexagons

    IEnumerator UpdateCellsAfterMatch(MatchInfo match)
    {
        int[] matchX = new int[2];
        matchX[0] = match.matchX1;
        matchX[1] = match.matchX2;
        
        int[] matchStartY = new int[2];
        matchStartY[0] = match.matchStartingY1;
        matchStartY[1] = match.matchStartingY2;
        
        int[] matchEndY = new int[2];
        matchEndY[0] = match.matchEndingY1;
        matchEndY[1] = match.matchEndingY2;
        
        //Debug.Log(match.matchX1 + " - " +match.matchStartingY1 +"/"+match.matchEndingY1);
        //Debug.Log(match.matchX2 + " - " +match.matchStartingY2 +"/"+match.matchEndingY2);
        int matchHeight;
        for (int i = 0; i < matchX.Length; i++)
        {
            matchHeight = (matchEndY[i] - matchStartY[i]) + 1;
            //Debug.Log((matchStartY[i]+matchHeight)+"-"+row);
            
            for (int j = matchStartY[i]+matchHeight; j < row; j++)
            {
                cells[matchX[i], j-matchHeight] = cells[matchX[i], j];
                cells[matchX[i],j-matchHeight].OnChangedPosition((byte)matchX[i],(byte)(j-matchHeight));
                cells[matchX[i],j-matchHeight].ChangeCollider(false);
                if (j-matchHeight!=0)
                {
                    cells[matchX[i],j-matchHeight].ChangeBodyType(RigidbodyType2D.Dynamic);
                }
                else
                {
                    StartCoroutine(cells[matchX[i], j - matchHeight].transform.Move(groundsPos[matchX[i]], 0.5f));
                }
                
            }
            yield return new WaitForSeconds(0.5f);
            for (int j = 0; j <matchHeight; j++)
            {
                cells[matchX[i], (row-1) - j] = InstantiateCell(matchX[i], (row-1) - j);
                MatchInfo matchInfo = GetMatchInformation(cells[matchX[i], (row-1) - j]);
                if (matchInfo.validMatch)
                {
                    Destroy(cells[matchX[i], (row-1) - j].gameObject);
                    cells[matchX[i], (row-1) - j] = InstantiateCell(matchX[i], (row-1) - j);
                }
                cells[matchX[i], (row-1) - j].ChangeCollider(false);
            }
        }

        yield return new WaitForSeconds(0.3f);
        
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                cells[i,j].ChangeCollider(true);
                cells[i,j].ChangeBodyType(RigidbodyType2D.Static);
            }
        }
        
        yield return StartCoroutine(ControlMatchAllBoard());
        canPlay = true;
        isSelect = true;
    }

    #endregion
    
}
