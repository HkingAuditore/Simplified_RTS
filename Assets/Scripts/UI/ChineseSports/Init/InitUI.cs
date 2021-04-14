using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitUI : MonoBehaviour
{
   public void InitToMap()
   {
      SceneManager.LoadScene("Map");
   }
}
