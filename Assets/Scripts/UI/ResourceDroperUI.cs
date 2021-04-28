using Player;
using UnityEngine;

public class ResourceDroperUI : MonoBehaviour
{
    public ResourceDroper resourceDroper;

    public void Collect()
    {
        resourceDroper.CollectResource();
        Destroy(gameObject);
    }
}