using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResouceUI : MonoBehaviour
{
    public Player player;

    public Text foodText;
    public Text woodText;
    public Text goldText;

    public Text dispatchableFarmersText;
    public Text maxFarmersText;


    public void FixedUpdate()
    {
        foodText.text = "食物:" + player.Food;
        woodText.text = "木材:" + player.Wood;
        goldText.text = "黄金:" + player.Gold;
        
        dispatchableFarmersText.text = "可调遣农夫:" + player.DispatchableFarmer;
        maxFarmersText.text = "最大农夫人口数:" + player.MaxFarmerNumber;

        #if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q))
        {
            player.ChangeResource(Resource.Gold,10);
        }
        #endif
    }
}
