using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            GameObject.Find("Player").GetComponent<Player>().velocity *= 500f/1000f;
            Debug.Log("Player is slowed");
        }
    }
}
