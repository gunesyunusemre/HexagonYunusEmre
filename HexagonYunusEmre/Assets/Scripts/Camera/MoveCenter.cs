using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCenter : MonoBehaviour
{
    public static  MoveCenter CamScript;

    [SerializeField] private float tolerance=0.2f;

    #region MonoBehaviour
    private void Start()
    {
        CamScript = this;
    }
    #endregion

    #region CameraSettings
    public void SetCam(Vector3 targetPos)
    {
        transform.position = targetPos;

        Camera.main.orthographicSize = ((720 * (16f / 9f) / 2) / 100) + tolerance;
 
        Camera.main.aspect = 9f / 16f;
    }
    #endregion
    
    
    
}
