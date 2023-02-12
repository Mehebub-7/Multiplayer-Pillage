using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : NetworkBehaviour
{
    int waypointIndex;
    public List<Transform> waypoints;
    public int playerIndex;

    Player model = new Player();
    [SerializeField] PlayerView view;
    PlayerController controller;

    void Start()
    {
        Debug.Log(model);
        controller = new PlayerController(model, view);
    }

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
                controller.model.position = new Position(waypoints[waypointIndex].position.x, waypoints[waypointIndex].position.y, waypoints[waypointIndex].position.z);
                yield return new WaitForSeconds(.5f);
            }
            count--;
        }
    }
}