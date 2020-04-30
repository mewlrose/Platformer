using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;
    private Vector2 originalPos;
    private bool canBounce = true;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void QuestionBlockBounce()
    {
        if (canBounce)
        {
            canBounce = false;
            StartCoroutine(Bounce());
        }
    }

    void Update()
    {
        
    }

    IEnumerator Bounce ()
    {
        while (true)
        {
            transform.localPosition = new Vector2 ( transform.localPosition.x, 
                                                    transform.localPosition.y + bounceSpeed * Time.deltaTime);
                                                    
            if (transform.localPosition.y >= originalPos.y + bounceHeight)
                break;
        }

        while (true)
        {

        }
    }

}
