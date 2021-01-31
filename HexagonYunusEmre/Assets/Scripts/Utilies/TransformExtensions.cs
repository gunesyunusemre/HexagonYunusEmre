using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class TransformExtensions
{

    public static IEnumerator Turn(this Transform t, Cell target, Cell other, float speed, 
        Vector3 direction, Vector3 midPoint, Cell[] selectedItems)
    {
        var tPos = t.transform.position;//targets[0]
        var targetPos = target.transform.position;//targets[1]
        var nextPos = other.transform.position;//targets[2]


        var distance = Vector2.Distance(targetPos, t.transform.position);
        
        while(distance >0.1f)
        {
            distance = Vector2.Distance(targetPos, t.transform.position);

            foreach (var item in selectedItems)
            {
                item.transform.RotateAround(midPoint, direction,  speed*Time.deltaTime);
            }
            
            yield return null;
        }
        
        foreach (var t1 in selectedItems)
        {
            t1.transform.rotation = quaternion.Euler(0,0,0);
        }

        t.transform.position = targetPos;
        target.transform.position = nextPos;
        other.transform.position = tPos;
        
        yield return null;
    }
    
    public static IEnumerator Scale (this Transform t, Vector3 target, float duration)
    {
        Vector3 diffVector = (target - t.localScale);
        float diffLength = diffVector.magnitude;
        diffVector.Normalize();
        float counter = 0;
        while (counter < duration)
        {
            float movAmount = (Time.deltaTime * diffLength) / duration;
            t.localScale += diffVector * movAmount;
            counter += Time.deltaTime;
            yield return null;
        }

        t.localScale = target;
    }
    
    public static IEnumerator Move(this Transform t, Vector3 target, float duration)
    {
        Vector3 diffVector = (target - t.position);
        float diffLength = diffVector.magnitude;
        diffVector.Normalize();
        float counter = 0;
        while(counter < duration)
        {
            float movAmount=(Time.deltaTime*diffLength)/duration;
            t.position += diffVector * movAmount;
            counter += Time.deltaTime;
            yield return null;
        }

        t.position = target;
    }
    

}
