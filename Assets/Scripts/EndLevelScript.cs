using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelScript : MonoBehaviour
{
    [SerializeField] GameObject EndScreen;
    [SerializeField] GameObject menuImage;
    [SerializeField] Sprite FadeImage;
    [SerializeField] private Image image;
    [SerializeField] private int SceneID;
    // Duration of the color change
    [SerializeField] private float duration1;

    // Target alpha values
    private float startAlpha1 = 0f;
    private float endAlpha1 = 1f;

    [SerializeField] private float duration2;

    private float startAlpha2 = 1f;
    private float endAlpha2 = 0f;

    private bool x = true;
    private bool y = true;
    private bool z = true;

    void Start()
    {
        //StartCoroutine(ChangeColor());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger && y)
        {
            y = false;
            EndScreen.SetActive(true);
            IEnumerator a = ChangeColor(duration1, startAlpha1, endAlpha1);
            StartCoroutine(a);
        }  
    }

    IEnumerator ChangeColor(float duration, float start, float end)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            // Calculate the proportion of the duration that has passed
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Update the alpha value
            color.a = Mathf.Lerp(start, end, t);
            image.color = color;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the final alpha is set
        color.a = end;
        image.color = color;
        if (x)
        {
            x = false;
            menuImage.SetActive(true);
            if (FadeImage != null)
            {
                menuImage.GetComponent<Image>().sprite = FadeImage;
            }
            IEnumerator a = ChangeColor(duration2, startAlpha2, endAlpha2);
            StartCoroutine(a);
        }
        else if (z)
        {
            z = false;
            SceneManager.LoadSceneAsync(SceneID);
        }
    }
}
