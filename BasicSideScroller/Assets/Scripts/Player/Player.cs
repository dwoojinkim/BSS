﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //Remove this once I no longer need the debug text

public class Player : MonoBehaviour
{
    public GameObject DebugTextObj;
    public GameObject Hitbox;

    public float jumpVelocity = 5f;
    public float jumpGravity = 1f;
    public float normalGravity = 10f;
    public float extraGravity = 20f;

    private Rigidbody2D playerRigidbody;

    private Text debugText;
    private bool jumping = false;
    private bool slashRequest = false;
    private bool slashing = false;
    private bool smashRequest = false;
    private bool smashing = false;
    private float hitboxStartup = 0.05f;
    private float hitboxDuration = 0.05f;
    private float attackCooldown = 1f;
    private float attackTimer = 0f;
    private float playerMovespeed = 5;


    // Start is called before the first frame update
    void Start()
    {
        if (DebugTextObj != null)
        {
            debugText = DebugTextObj.GetComponent<Text>();
        }
        
        playerRigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //MovePlayer();

        if (slashRequest)
        {  
            if (attackTimer >= hitboxStartup)
            {
                slashing = true;
                slashRequest = false;

                Hitbox.GetComponent<SpriteRenderer>().enabled = true;
                Hitbox.GetComponent<BoxCollider2D>().enabled = true;
                attackTimer = 0f;
            }
            attackTimer += Time.deltaTime;
        }
        if (slashing)
        {
            if (attackTimer >= hitboxDuration)
            {
                slashing = false;
                Hitbox.GetComponent<SpriteRenderer>().enabled = false;
                Hitbox.GetComponent<BoxCollider2D>().enabled = false;
                attackTimer = 0f;
            }

            attackTimer += Time.deltaTime;
        }

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.tag == "Obstacle" && obj.GetComponent<Collider2D>().enabled)
        {
            Debug.Log("Game Over");
            Kill();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            jumping = false;
        }
    }
    

    public void SetDebugText(string text)
    {
        debugText.text = text;
    }

    public void MovePlayer()
    {
        transform.position += transform.right * playerMovespeed * Time.fixedDeltaTime;
    }

    public void Jump()
    {
        SetDebugText("Jumping");

        //If not already in a jumping state, then do below
        if (!jumping)
        {
            playerRigidbody.velocity = transform.up * jumpVelocity;
            playerRigidbody.gravityScale = jumpGravity;
            jumping = true;
        }

    }

    public void ReleaseJump()
    {
        playerRigidbody.gravityScale = normalGravity;
    }

    public void Down()
    {
        playerRigidbody.gravityScale = extraGravity;
    }

    public void Slash()
    {
        if (!slashing)
            slashRequest = true;
    }

    public void Smash()
    {
        smashRequest = true;
    }

    public void Kill()
    {
        GameManager.instance.ResetGame();
    }
}
