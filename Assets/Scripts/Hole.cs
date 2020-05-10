using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hole : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("Player").GetComponent<Player>().score = 0;
            SceneManager.LoadScene("Level1");
        }

        if (other.tag == "Enemy" || other.tag == "Enemy (1)")
        {
            Destroy(this.gameObject);
        }
    }
}
