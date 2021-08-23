using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    public GameObject Player;
    public GameObject Remy;
    public GameObject Joe;
    public GameObject Kachujin;

    public TextMeshProUGUI playerHp;
    public TextMeshProUGUI remyHp;
    public TextMeshProUGUI joeHp;
    public TextMeshProUGUI kachujinHp;
    public TextMeshProUGUI whoWon;
    public TextMeshProUGUI backToMainMenu;

    bool playerIsDead, remyIsDead, joeIsDead, kachuIsDead, gameOver;
    int childNum;

    // Start is called before the first frame update
    void Start()
    {
        childNum = 1;
        gameOver = false;
        playerIsDead = false;
        remyIsDead = false;
        joeIsDead = false;
        kachuIsDead = false;
        playerHp.text = Player.transform.name + '\n' + Player.transform.GetChild(1).GetComponent<PlayerMove>().hp.ToString();
        remyHp.text = Remy.name +'\n' +Remy.GetComponent<CommanderNPC>().hp.ToString();
        joeHp.text = Joe.name +'\n' +Joe.GetComponent<Follower>().hp.ToString();
        kachujinHp.text = Kachujin.name + '\n' + Kachujin.GetComponent<Follower>().hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.GetChild(0).gameObject.activeSelf)
            childNum = 0;
        else
            childNum = 1;
        if (Player.transform.GetChild(childNum).GetComponent<PlayerMove>().hp <= 0)
            playerIsDead = true;
        if (Remy.GetComponent<CommanderNPC>().hp <= 0)
            remyIsDead = true;
        if (Joe.GetComponent<Follower>().hp <= 0)
            joeIsDead = true;
        if (Kachujin.GetComponent<Follower>().hp <= 0)
            kachuIsDead = true;
        if (!playerIsDead)
            playerHp.text = Player.name + '\n' + Player.transform.GetChild(childNum).GetComponent<PlayerMove>().hp.ToString();
        else
            playerHp.text = Player.name +"\n0";
        if (!remyIsDead)
            remyHp.text = Remy.name + '\n' + Remy.GetComponent<CommanderNPC>().hp.ToString();
        else
            remyHp.text = Remy.name + "\n0";
        if (!joeIsDead)
            joeHp.text = Joe.name + '\n' + Joe.GetComponent<Follower>().hp.ToString();
        else
            joeHp.text = Joe.name + "\n0";
        if (!kachuIsDead)
            kachujinHp.text = Kachujin.name + '\n' + Kachujin.GetComponent<Follower>().hp.ToString();
        else
            kachujinHp.text = Kachujin.name + "\n0";
        
        if (remyIsDead && kachuIsDead)
        {
            whoWon.text = "Blue Team Wins!";
            whoWon.color = new Color(0, 0, 255);
            whoWon.gameObject.SetActive(true);
            gameOver = true;
        }
        else if (playerIsDead && joeIsDead)
        {
            whoWon.text = "Red Team Wins!";
            whoWon.color = new Color(255, 0, 0);
            whoWon.gameObject.SetActive(true);
            gameOver = true;
        }
        if (gameOver)
        {
            Time.timeScale = 0;
            transform.GetChild(6).gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
