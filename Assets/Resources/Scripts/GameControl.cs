using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameControl : MonoBehaviour {


    public static GameControl control;

    public float highScore;

    // Use this for initialization
    void Awake() {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        if (Load().Equals("Failure"))
        {
            highScore = 1.0f;
        }
        
    }

    private void OnDestroy()
    {
        Save();
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 40), "High Score: " + highScore);

    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        SaveData data = new SaveData();

        data.highScore = highScore;
        bf.Serialize(file, data);
        file.Close();
    }

    public string Load()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + 
                "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            highScore = data.highScore;
            file.Close();
            return ("Success");

        }
        else
        {
            return ("Failure");
        }
    } 



    [Serializable]
    class SaveData
    {
        public float highScore;
    }
}
