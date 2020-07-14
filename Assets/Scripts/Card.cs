using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SWNetwork;
using System;

public class Card : MonoBehaviour
{
    public int attack { get; set; }
    public int health { get; set; }
    public int card_id { get; set; }
    public string cardName { get; set; }
    public int manaCost { get; set; }

    public Animator anim;
    public string owner;

    public bool dead;
    public bool canAttack = true;

    public GameObject attackBtn;
    public GameObject targetBtn;
    public GameObject model;

    public void showAnimation(string animation)
    {
        anim.GetComponent<Animator>();
        switch (animation)
        {
            case "attack":
                {
                    Debug.Log("show attack animation");
                    anim.SetBool("isAttack", true);
                    break;
                }
            case "hit":
                {
                    anim.SetBool("isHit", true);
                    break;
                }
            case "dead":
                {
                    anim.SetBool("isDead", true);
                    break;
                }
        }
    }

    public void setDead(bool isDead)
    {
        dead = isDead;
    }

    public bool getDead()
    {
        return dead;
    }

    public void toggleAttackEvent()
    {
        if (canAttack)
            canAttack = false;
        else
            canAttack = true;
    }
}
