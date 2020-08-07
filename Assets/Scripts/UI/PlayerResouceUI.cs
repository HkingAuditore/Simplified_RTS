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


    public void FixedUpdate()
    {
        foodText.text = "Food:" + player.Food;
        woodText.text = "Wood:" + player.Wood;
        goldText.text = "Gold:" + player.Gold;
    }
}
