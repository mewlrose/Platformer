﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target, leftBounds, rightBounds;

    public float smoothDampTime = 0.15f;
    public Vector3 smoothDampVelocity = Vector3.zero;

    private float camWidth, camHeight, levelMinX, levelMaxX;

    void Start()
    {
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = Camera.main.aspect * camHeight;

        float leftBoundsWidth = leftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightBoundsWidth = rightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;

        levelMinX = leftBounds.position.x + leftBoundsWidth + (camWidth/2);
        levelMaxX = rightBounds.position.x - rightBoundsWidth - (camWidth/2);
    }

    void Update()
    {
        if (target)
        {
            float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));
            float x = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothDampVelocity.x, smoothDampTime);

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
