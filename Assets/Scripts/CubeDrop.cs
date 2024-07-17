using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CubeDrop : MonoBehaviour
{
    private int score;

    [SerializeField] private TMP_Text scoreTxt;

    [SerializeField] private GameObject GameOverPanel;

    private void Start()
    {
        GameOverPanel.SetActive(false);
    }

    private void Update()
    {
        scoreTxt.text = "Score: " + score;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cube"))
        {
            Destroy(other.gameObject);
            score += 10;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
