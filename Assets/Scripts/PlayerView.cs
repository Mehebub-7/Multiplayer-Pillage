using UnityEngine;
using Fusion;

public class PlayerView : NetworkBehaviour
{
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}