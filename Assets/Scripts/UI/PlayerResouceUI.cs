using Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResouceUI : MonoBehaviour
{
    public Player.Player player;

    public Text foodText;
    public Text woodText;
    public Text goldText;

    public Text dispatchableFarmersText;
    public Text maxFarmersText;


    public void FixedUpdate()
    {
        foodText.text = player.Food.ToString();
        woodText.text = player.Wood.ToString();
        goldText.text = player.Gold.ToString();

        dispatchableFarmersText.text = "可调遣农夫:"   + player.DispatchableFarmer;
        maxFarmersText.text          = "最大农夫人口数:" + player.MaxFarmerNumber;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q)) player.ChangeResource(GameResourceType.Gold, 10);
#endif
    }
}