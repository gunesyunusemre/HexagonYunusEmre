using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour
{
    #region Variables

    #region ID
    private byte _column;
    private byte _row;
    private byte _colorType;
    private int _bombCount;
    [SerializeField] private bool isBomb;
    #endregion

    #region MyChildren
    [SerializeField]private List<Transform> corners;
    [SerializeField]private GameObject selectedBackground;
    #endregion

    #region My Component
    [SerializeField]private CircleCollider2D _circleCollider2D;
    [SerializeField]private BoxCollider2D _boxCollider2D;
    [SerializeField]private Rigidbody2D _rigidbody2D;
    [SerializeField]private Text bombMoveDownText;
    #endregion

    #region SwipeVariables
    private Vector2 startPosition;
    private Vector2 endPosition;
    #endregion
    
    #endregion

    #region ID

    public byte Column  { get { return _column; } }
    public byte Row { get { return _row; } }
    public byte ColorType
    {
        get  =>  _colorType; 
        set  => _colorType = value; 
    }
    public bool IsBomb => isBomb;

    public int BombCount
    {
        get => _bombCount;
        set => _bombCount = value;
    }

    #endregion

    #region MyChildren
    public List<Transform> Corners { get { return corners; } }
    
    public GameObject SelectedBackground { get { return selectedBackground; } }

    #endregion

    #region MonobehaviourFunction

    private void Start()
    {
        if (isBomb)
        {
            bombMoveDownText.text = _bombCount.ToString();
            GameCell.OnMoveCountUpEventHandler += OnBombCountDown;
        }
        
    }

    private void OnDisable()
    {
        if (isBomb)
        {
            GameCell.OnMoveCountUpEventHandler -= OnBombCountDown;
        }
    }

    #endregion

    #region ID Proceses

    public void OnChangedPosition(byte newColumn, byte newRow)
    {
        _column = newColumn;
        _row = newRow;
        this.gameObject.name = "(" + _column + "," + _row + ")";
    }

    #endregion

    #region Physics Work
    
    public void ChangeCollider(bool circle)
    {
        _circleCollider2D.enabled = circle;
        _boxCollider2D.enabled = !circle;
    }

    public void ChangeBodyType(RigidbodyType2D rigidbodyType2D)
    {
        _rigidbody2D.bodyType = rigidbodyType2D;
    }

    #endregion

    #region BombReactions

    private void OnBombCountDown()
    {
        if (!isBomb)
            return;

        _bombCount--;
        bombMoveDownText.text = _bombCount.ToString();

        if (_bombCount<=0)
        {
            //Debug.Log("GameOver");
            if (GameOverEventHandler != null) GameOverEventHandler();
        }
        
    }

    public delegate void GameOver();
    public static event GameOver GameOverEventHandler;
    
    #endregion

    #region Mouse Event

    private void OnMouseDown()
    {
        //Debug.Log("("+_column+","+_row+")" +" Color: "+_colorType);

        startPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        /*if (OnMouseOverItemEventHandler!=null)
        {
            OnMouseOverItemEventHandler(this);
        }*/
    }

    private void OnMouseUp()
    {
        
        endPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = endPosition - startPosition  ;

        if (distance.magnitude<1f)
        {
            if (OnMouseOverItemEventHandler!=null)
            {
                OnMouseOverItemEventHandler(this);
                //Debug.Log(distance.magnitude);
            }
        }

    }


    public delegate void OnMouseOverItem(Cell item);
    public static event OnMouseOverItem OnMouseOverItemEventHandler;
    #endregion
    
}
