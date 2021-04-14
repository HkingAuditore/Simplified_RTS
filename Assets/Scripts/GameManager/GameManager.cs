using System.Collections.Generic;
using Pathfinding;
using Units;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public        Player      aSide;
        public        Player      bSide;
        public        Seeker      seeker;
        public        Camera      mainCamera;
        public        List<Unit>  unitsList = new List<Unit>();
        public        ResultUI    resultUI;
        public static GameManager GetManager { get; private set; }
        private void Awake()
        {
            GetManager = this;
        }


    }
}
