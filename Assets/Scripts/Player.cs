using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    int waypointIndex;
    public List<Transform> waypoints;
    public int playerIndex;

    public override void FixedUpdateNetwork()
    {
        if (playerIndex == BasicSpawner.Instance.currentPlayerIndex)
        {
            if (GetInput(out NetworkInputData data))
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(data.mousePosition), out RaycastHit hit);
                if (hit.transform != null)
                {
                    if (hit.transform.GetComponent<DiceHolder>() != null)
                    {
                        if (hit.transform.GetComponent<DiceHolder>().diceIndex == playerIndex)
                        {
                            BasicSpawner.Instance.currentPlayerIndex++;
                            BasicSpawner.Instance.currentPlayerIndex %= BasicSpawner.Instance.spawnIndex;
                            int diceValue = Random.Range(1, 7);
                            StartCoroutine(Move(diceValue));
                        }
                    }
                }
            }
        }
    }
    public IEnumerator Move(int count)
    {
        while (count > 0)
        {
            waypointIndex++;
            if (waypointIndex < waypoints.Count)
            {
                transform.position = waypoints[waypointIndex].position;
                yield return new WaitForSeconds(.5f);
            }
            count--;
        }
    }
}