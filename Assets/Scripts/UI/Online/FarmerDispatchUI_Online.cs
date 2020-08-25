using Photon.Pun;

namespace UI.Online
{
    public class FarmerDispatchUIOnline : MonoBehaviourPun
    {
        public Player player;
    

        public void DispatchFood(bool isAdd)
        {
            if (isAdd)
            {
                player.AddFarmer(Resource.Food);
            }
            else
            {
                player.SubtractFarmer(Resource.Food);
            }
        }
    
        public void DispatchGold(bool isAdd)
        {
            if (isAdd)
            {
                player.AddFarmer(Resource.Gold);
            }
            else
            {
                player.SubtractFarmer(Resource.Gold);
            }
        }
    
        public void DispatchWood(bool isAdd)
        {
            if (isAdd)
            {
                player.AddFarmer(Resource.Wood);
            }
            else
            {
                player.SubtractFarmer(Resource.Wood);
            }
        }
    }
}