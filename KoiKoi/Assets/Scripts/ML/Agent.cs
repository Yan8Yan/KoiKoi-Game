using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using KoiKoiProject;

public class KoiKoiAgent : Agent
{
    [Header("Test Data")]
    [SerializeField] private List<Card> handCards = new List<Card>();
    [SerializeField] private List<Card> tableCards = new List<Card>();

    [Header("Database")]
    [SerializeField] private CardDatabase cardDatabase;

    [Header("Settings")]
    [SerializeField] private int handSize = 3;
    [SerializeField] private int tableSize = 2;

    public override void OnEpisodeBegin()
    {
        GenerateRandomSituation();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // HAND
        for (int i = 0; i < handSize; i++)
        {
            if (i < handCards.Count)
            {
                sensor.AddObservation((int)handCards[i].month);
            }
            else
            {
                sensor.AddObservation(-1);
            }
        }

        // TABLE
        for (int i = 0; i < tableSize; i++)
        {
            if (i < tableCards.Count)
            {
                sensor.AddObservation((int)tableCards[i].month);
            }
            else
            {
                sensor.AddObservation(-1);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int chosenCardIndex = actions.DiscreteActions[0];

        // защита от выхода за границы
        if (chosenCardIndex < 0 || chosenCardIndex >= handCards.Count)
        {
            AddReward(-1f);
            EndEpisode();
            return;
        }

        Card selectedCard = handCards[chosenCardIndex];

        bool foundMatch = false;

        foreach (Card tableCard in tableCards)
        {
            if (tableCard.month == selectedCard.month)
            {
                foundMatch = true;
                break;
            }
        }

        if (foundMatch)
        {
            Debug.Log("GOOD MOVE");
            AddReward(+1f);
        }
        else
        {
            Debug.Log("BAD MOVE");
            AddReward(-1f);
        }

        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            discreteActions[0] = 2;
        }
    }

    private void GenerateRandomSituation()
    {
        handCards.Clear();
        tableCards.Clear();

        List<Card> tempDeck = new List<Card>(cardDatabase.allCards);

        Shuffle(tempDeck);

        // HAND
        for (int i = 0; i < handSize; i++)
        {
            handCards.Add(tempDeck[0]);
            tempDeck.RemoveAt(0);
        }

        // TABLE
        for (int i = 0; i < tableSize; i++)
        {
            tableCards.Add(tempDeck[0]);
            tempDeck.RemoveAt(0);
        }

        DebugSituation();
    }

    private void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);

            Card temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    private void DebugSituation()
    {
        Debug.Log("===== NEW EPISODE =====");

        Debug.Log("HAND:");

        for (int i = 0; i < handCards.Count; i++)
        {
            Debug.Log(i + ": " + handCards[i].month);
        }

        Debug.Log("TABLE:");

        for (int i = 0; i < tableCards.Count; i++)
        {
            Debug.Log(tableCards[i].month);
        }
    }
}