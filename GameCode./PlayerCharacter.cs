using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCharacter : MonoBehaviour
{
    //create player variables
    public int health;
    public int maxHealth;
    public int mana;
    static public int level = 1;
    int attack;
    public int speed;
    private TMP_Text healthText;
    public int exp = 0;
    //new random generator
    System.Random rnd = new System.Random();
    //create object and script variables
    GameObject enemyChar;
    GameObject playerChar;
    GameObject combatController;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    CombatControlScript controlscript;
    // Start is called before the first frame update
    void Start()
    {
        //Player max health
        maxHealth = 20;
        //amount of possible damage
        attack = 4;
        //starting mana
        mana = 10;
        //current health
        health = 20;
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        combatController = GameObject.FindGameObjectWithTag("Controller");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        controlscript = combatController.GetComponent<CombatControlScript>();
    }//end start function


    //player attack function
    public void Attack()
    {
        //reduce enemy health by attack value
        enemyscript.health -= (attack);
    }

    //fireball function
    public void Fire()
    {
        //check if current mana is 6 or more 
        if (mana >= 6)
        {
            //calculate fireball damage
            enemyscript.health -= (int)((attack * level) / 1.5);
            mana -= 6;
            // changes turn
            controlscript.playerturn = false;
        }
    }

    //player ult function
    public void Ult()
    {
        //check if current mana is 9 or more 
        if (mana >= 9)
        {
            
            enemyscript.health -= (int)((attack * (2.5 * level)));
            if (health <= (maxHealth - 5))
            {
                health += (5*level);
            }
            
            mana -= 9;
            // changes turn
            controlscript.playerturn = false;
        }
    }//end ult function


    void Update()
    {
        //set player status text
        healthText = GetComponentInChildren<TMP_Text>();
        healthText.SetText("HP: " + health + "\nMana:" + mana + "\nAttack:" + attack);
        //check if player has 100 experience
        if (exp >= 100)
        {
            //remove 100 experience from player for level up
            exp -= 100;
            levelUp();
            // changes turn
            enemyscript.enemylevelUp();
            

        }
    }//end update function

    //player level up sequence
   void levelUp()
    {
        //increase level by 1
        level += 1;
        //check if level is below level cap for stat boost
        if (level <= 4)
        {
            //check if level is divisible by 2 for attack increase
            if (level % 2 == 0)
            {
                //increase posible damage by a range of 1-3
                attack = (attack + rnd.Next(1, 3));
            }
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
        }
        //if player is above max level for stat increase just improve health
        else
        {
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
        }
        //set health back to max upon level up
        health += maxHealth-health;
        //increase mana by 3 upon level up
        mana += 3;
    }//end levelup function

    
}