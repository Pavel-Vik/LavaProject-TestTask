using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Возможность сохранения в файл
[System.Serializable]
public class PlayerData
{
    public int jumpCount;

    public PlayerData (DataManager playerData)
    {
        jumpCount = playerData.jumpCount;
    }
}
