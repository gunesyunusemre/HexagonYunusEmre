using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectBackGroundUI : MonoBehaviour
{
    [SerializeField]private Button left;
    [SerializeField]private Button right;
    void Start()
    {
        left.onClick.AddListener(SelectLeft);
        right.onClick.AddListener(SelectRight);
    }

    private void SelectLeft()
    {
        SceneManager.LoadScene("LevelScene");
    }
    
    private void SelectRight()
    {
        SceneManager.LoadScene("AnotherBackgroundLevelScene");
    }
}
