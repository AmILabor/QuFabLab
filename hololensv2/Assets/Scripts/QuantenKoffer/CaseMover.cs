using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class CaseMover : MonoBehaviour
{
    public float minMoveValue = 1;
    public float maxMoveValue = 10;
    private float currentMoveValue = 1;

    public GameObject toMove;

    // Start is called before the first frame update
    void Start()
    {
        currentMoveValue = CalculateMoveValue(0.5f);
    }

    private float CalculateMoveValue(float moveValue)
    {
        return minMoveValue + ((maxMoveValue - minMoveValue) * moveValue);
    }

    public void OnMoveValueChanged(SliderEventData evtDAta)
    {
        if (evtDAta.NewValue != evtDAta.OldValue)
        {
            AMI.Util.Console.Log(evtDAta.NewValue);
            currentMoveValue = CalculateMoveValue(evtDAta.NewValue);
        }
    }

    private void Move(Vector3 move)
    {
        AMI.Util.Console.Log("Moving " + toMove.transform.position + " to " + (toMove.transform.position + move));

        toMove.transform.position += move;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-currentMoveValue, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(currentMoveValue, 0, 0));
    }

    public void MoveUp()
    {
        Move(new Vector3(0, currentMoveValue, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -currentMoveValue, 0));
    }

    public void MoveFowrward()
    {
        Move(new Vector3(0, 0, currentMoveValue));
    }

    public void MoveBackward()
    {
        Move(new Vector3(0, 0, -currentMoveValue));
    }

    // Update is called once per frame
    void Update()
    {
    }
}