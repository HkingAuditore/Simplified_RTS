using Photon.Pun;

namespace UI.Online
{
    public class FarmerDispatchUIOnline : MonoBehaviourPun
    {
        public Player player;


        public void DispatchFood(bool isAdd)
        {
            if (isAdd)
                player.AddFarmer(GameResourceType.Food);
            else
                player.SubtractFarmer(GameResourceType.Food);
        }

        public void DispatchGold(bool isAdd)
        {
            if (isAdd)
                player.AddFarmer(GameResourceType.Gold);
            else
                player.SubtractFarmer(GameResourceType.Gold);
        }

        public void DispatchWood(bool isAdd)
        {
            if (isAdd)
                player.AddFarmer(GameResourceType.Wood);
            else
                player.SubtractFarmer(GameResourceType.Wood);
        }
    }
}