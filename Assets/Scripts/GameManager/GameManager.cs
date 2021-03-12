using Pathfinding;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public        Player      aSide;
        public        Player      bSide;
        public        Seeker      seeker;
        public static GameManager GetManager { get; private set; }
        private void Awake()
        {
            GetManager = this;
        }


    }
}
