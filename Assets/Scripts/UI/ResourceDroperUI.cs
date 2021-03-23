using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ResourceDroperUI : MonoBehaviour
{
    public ResourceDroper resourceDroper;
    public void           Collect()
    {
        resourceDroper.CollectResource();
        Destroy(this.gameObject);
    }
}
