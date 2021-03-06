using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;

public class EnemyCharacter : MonoBehaviour
{
    private TMP_Text healthText;
    public int maxHealth;
    public int health;
    public int level;
    public int mana;
    private int attack;
    public int speed;
    System.Random rnd = new System.Random();
    GameObject enemyChar;
    GameObject playerChar;
    GameObject combatController;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    CombatControlScript controlscript;
    DatabaseReference DBreference;
    string userId;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get database reference
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        //Enemy max health
        maxHealth = 15;
        //Enemy starting health
        health = 15;
        //enemy starting mana
        mana = 0;
        //enemy possible damage
        attack = 4;
        //Match enemy and player levels
        level = PlayerCharacter.level;
        //get active game objects and their needed scripts
        //Objects tags are checked and then scripts are pulled from those objects
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        combatController = GameObject.FindGameObjectWithTag("Controller");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        controlscript = combatController.GetComponent<CombatControlScript>();
        
    }//end start method
   
    // Update is called once per frame
    void Update()
    {
        //set enemy status text
        healthText = GetComponentInChildren<TMP_Text>();
        healthText.SetText("HP: " + health + "\nMana:" + mana);
        //when enemy health hits 0 reset or "spawn" a new enemy
        if (health <= 0)
        {
            // add to player kills
            PlayerCharacter.kills += 1;
            StartCoroutine(UpdateKills(PlayerCharacter.kills));
            PlayerCharacter.highscore += 5;
            StartCoroutine(UpdateHighscore(PlayerCharacter.highscore));
	    
	    //reset enemy
            enemyscript.charReset(); 
        }
        

    }//end update method


    //enemy attacking functions
    public void Attack()
    {
        //calculate damage to player
        playerscript.health -= (int)((level*2)*1.6);
    }

    //enemy level up method
    public void enemylevelUp()
    {
        //increase level by 1
        level += 1;
        //check if level is below level cap for stat boost
        if (level <= 4)
        {
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
            //set health to max
            health += maxHealth - health;
            //increase damage by 1
            attack += rnd.Next(1,3);
        }
        else
        {
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
            //set health to max
            health += maxHealth - health;
        }

    }//end level up method
    
    
    //method to reset or "Spawn" a new enemy
    //could add a way to randomize eenemies here upon reset?
    public void charReset()
    {
        //set health to max
        enemyscript.health = enemyscript.maxHealth;
        //give player experience (could be randomized?)
        playerscript.exp += 80;
    }//end char reset method

    //enemy ult
    public void Ult()
    {
        //when enemy mana reaches 10 enemy uses ult or has chance to use ult?
        if (mana >= 10)
        {
            //calculate damage to player and update player health
            playerscript.health -= (int)((attack * level));
            //enemy gains health
            health += (int)(health * .45);
            
        }
    }//end ult method
    
    //enemy AI method
    public void enemyTakeTurn()
	{
		if (playerscript.health > 0){
			if (enemyscript.mana >= 10){
				enemyscript.Ult();
			}
			else {
				enemyscript.Attack();
			}
		}
	}//end AI method
    
        // Updates kills in database
       private IEnumerator UpdateKills(int kills)
    {
        //Set the currently logged in user kills
        var DBTask = DBreference.Child("users").Child(userId).Child("kills").SetValueAsync(kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }
    
        private IEnumerator UpdateHighscore(int newScore)
    {
        //Set the currently logged in user level
        var DBTask = DBreference.Child("users").Child(userId).Child("highscore").SetValueAsync(newScore);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
    }
    
}
