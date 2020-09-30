using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Start is called before the first frame update

    // prize of the towers
    public float prizeTower1 = 100;
    public float prizeTower2 = 350;
    public Text txt_prize1, txt_prize2;

    // text to display the score
    public Text txt_gold;

    //actual gold
    public float gold=0;
    public float initialGold=100;

    
    //this is the health of the player
    public float playerHealth=0;
    public float initialHealth = 100;
    public Image im_health;


    void Start()
    {
        //initialization
        txt_prize1.text = "" + prizeTower1;
        txt_prize2.text = "" + prizeTower2;

        gold = initialGold;
        playerHealth = initialHealth;

        GameObject.FindGameObjectWithTag("scoreCanvas").GetComponent<Canvas>().enabled=true;

    }

    // Update is called once per frame
    void Update()
    {
        txt_gold.text= "" + gold;

        im_health.fillAmount = playerHealth / initialHealth;
    }

    public void updateGold(float a)
    {
        gold += a;
    }

    public void takeHit(float a)
    {
        playerHealth -= a;
    }
}
