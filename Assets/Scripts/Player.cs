﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float jumpVelocity, bounceVelocity, gravity;

    public Vector2 velocity;
    public Vector2 originalVelocity;
    public LayerMask wallMask, floorMask;

    private bool walk, walkLeft, walkRight, jump;

    public Text scoreText;
    public int score = 0;

    public int playerHP = 5;

    public enum PlayerState
    {
        jumping,
        idle,
        walking,
        bouncing
    }
    private PlayerState playerState = PlayerState.idle;

    private bool grounded = false;
    private bool bounce = false;

    void Start()
    {
        originalVelocity.x = velocity.x;
        originalVelocity.y = velocity.y;
        InvokeRepeating("IncrementScore", 1.0f, 1.0f);
    }

    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPosition();
        UpdateAnimationStates();
    }

    void IncrementScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void UpdatePlayerPosition()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;

        if (walk)
        {
            if (walkLeft)
            {
                pos.x -= (velocity.x * Time.deltaTime);
                scale.x = -1;
            }
            if (walkRight)
            {
                pos.x += (velocity.x * Time.deltaTime);
                scale.x = 1;
            }
            pos = CheckWallRays(pos, scale.x);
        }

        if (jump && playerState != PlayerState.jumping)
        {
            playerState = PlayerState.jumping;
            velocity = new Vector2(velocity.x, jumpVelocity);
        }

        if (playerState == PlayerState.jumping)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if (bounce && playerState != PlayerState.bouncing)
        {
            playerState = PlayerState.bouncing;
            velocity = new Vector2 (velocity.x, bounceVelocity);
        }

        if (playerState == PlayerState.bouncing)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if (velocity.y <= 0)
            pos = CheckFloorRays(pos);

        if (velocity.y >= 0)
            pos = CheckCeilingRays(pos);

        transform.localPosition = pos;
        transform.localScale = scale;
    }

    void UpdateAnimationStates()
    {
        if (grounded && !walk && !bounce)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }

        if (grounded && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }

        if (playerState == PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    void CheckPlayerInput()
    {
        bool inputLeft = Input.GetKey(KeyCode.A);
        bool inputRight = Input.GetKey(KeyCode.D);
        bool inputSpace = Input.GetKey(KeyCode.W);

        walk = inputLeft || inputRight;
        walkLeft = inputLeft && !inputRight;
        walkRight = !inputLeft && inputRight;
        jump = inputSpace;
    }

    Vector3 CheckWallRays (Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * 0.4f, pos.y + 1f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 0.4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * 0.4f, pos.y - 1f + 0.2f);

        RaycastHit2D wallTop = Physics2D.Raycast(   originTop, new Vector2(direction, 0), 
                                                    velocity.x * Time.deltaTime, wallMask);

        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), 
                                                    velocity.x * Time.deltaTime, wallMask);

        RaycastHit2D wallBottom = Physics2D.Raycast(originBottom, new Vector2(direction, 0), 
                                                    velocity.x * Time.deltaTime, wallMask);
        
        if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
            pos.x -= velocity.x * Time.deltaTime * direction;

        return pos;
    }

    Vector3 CheckFloorRays (Vector3 pos)
    {
        Vector2 originLeft = new Vector2 (pos.x - 0.5f + 0.2f, pos.y - 1f);
        Vector2 originMid = new Vector2 (pos.x, pos.y - 1f);
        Vector2 originRight = new Vector2 (pos.x + 0.5f - 0.2f, pos.y - 1f);

        RaycastHit2D floorLeft = Physics2D.Raycast( originLeft, Vector2.down, 
                                                    velocity.y * Time.deltaTime, floorMask);

        RaycastHit2D floorMid = Physics2D.Raycast(  originMid, Vector2.down, 
                                                    velocity.y * Time.deltaTime, floorMask);

        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, 
                                                    velocity.y * Time.deltaTime, floorMask);
        
        if (floorLeft.collider != null || floorMid.collider != null || floorRight.collider != null)
        {
            RaycastHit2D hitRay = floorRight;
            if (floorLeft)
            {
                hitRay = floorLeft;
            } else if (floorMid)
            {
                hitRay = floorMid;
            } else if (floorRight)
            {
                hitRay = floorRight;
            }

            if (hitRay.collider.tag == "Enemy")
            {
                bounce = true;
                hitRay.collider.GetComponent<EnemyAI>().Crush();
                score += 20;
                scoreText.text = score.ToString();
            }

            playerState = PlayerState.idle;
            grounded = true;
            velocity.y = 0;
            pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y/2 + 1;
        }
        else
        {
            if (playerState != PlayerState.jumping)
                Fall();
        }
        return pos;
    }

    Vector3 CheckCeilingRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2 (pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 originMid = new Vector2 (pos.x, pos.y + 1f);
        Vector2 originRight = new Vector2 (pos.x + 0.5f - 0.2f, pos.y + 1f);

        RaycastHit2D ceilLeft = Physics2D.Raycast( originLeft, Vector2.up, 
                                                    velocity.y * Time.deltaTime, floorMask);

        RaycastHit2D ceilMid = Physics2D.Raycast(  originMid, Vector2.up, 
                                                    velocity.y * Time.deltaTime, floorMask);

        RaycastHit2D ceilRight = Physics2D.Raycast(originRight, Vector2.up, 
                                                    velocity.y * Time.deltaTime, floorMask);

        if (ceilLeft.collider != null || ceilMid.collider != null || ceilRight.collider != null)
        {
            RaycastHit2D hitRay = ceilLeft;
            if (ceilLeft)
            {
                hitRay = ceilLeft;
            } else if (ceilMid)
            {
                hitRay = ceilMid;
            } else if (ceilRight)
            {
                hitRay = ceilRight;
            }

            if (hitRay.collider.tag == "QuestionBlock")
            {
                hitRay.collider.GetComponent<QuestionBlock>().QuestionBlockBounce();
                score += 10;
                scoreText.text = score.ToString();
            }

            if (hitRay.collider.tag == "YellowQuestionBlock")
            {
                hitRay.collider.GetComponent<QuestionBlock>().QuestionBlockBounce();
                velocity = originalVelocity;
                Debug.Log("Player is back to original speed");
            }

            pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y/2-1;
            Fall();
        }
        return pos;
    }

    void Fall()
    {
        velocity.y = 0;
        playerState = PlayerState.jumping;
        bounce = false;
        grounded = false;
    }
}
