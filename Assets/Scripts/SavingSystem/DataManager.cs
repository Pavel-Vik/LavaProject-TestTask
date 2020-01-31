using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public Text text;
    public int jumpCount;

    public void DisplayJumpCount()
    {
        text.text = jumpCount.ToString();
    }

    public void SaveData()
    {
        SaveSystem.SavePlayerData(this);
    }


    private void Start()
    {
        LoadData();
        text.text = jumpCount.ToString();
    }

    public void LoadData()
    {
        PlayerData data = SaveSystem.LoadPlayerData();

        jumpCount = data.jumpCount;
    }
}
