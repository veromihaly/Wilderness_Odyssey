using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {get;set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Json Project Save Path
    string jsonPathProject;

    // Json External/Real Save Path
    string jsonPathPersistant;

    // Binary Save Path
    string binaryPath;

    public bool isSavingToJson;

    public void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        jsonPathPersistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        binaryPath = Application.persistentDataPath + "/save_game.bin";
    }

    #region || ---- General Section ---- ||

    #region || ---- Saving ---- ||
    public void SaveGame()
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        SavingTypeSwitch(data);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentCalories;
        playerStats[2] = PlayerState.Instance.currentHydrationPercent;

        float[] playerPosandRot = new float[6];
        playerPosandRot[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPosandRot[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPosandRot[2] = PlayerState.Instance.playerBody.transform.position.z;
    
        playerPosandRot[3] = PlayerState.Instance.playerBody.transform.rotation.x;
        playerPosandRot[4] = PlayerState.Instance.playerBody.transform.rotation.y;
        playerPosandRot[5] = PlayerState.Instance.playerBody.transform.rotation.z;
    
        return new PlayerData(playerStats,playerPosandRot);
    }
    public void SavingTypeSwitch(AllGameData gameData)
    {
        if(isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData);
        }
    }
    #endregion

    #region || ---- Loading ---- ||

    public AllGameData LoadingTypeSwitch()
    {
        if(isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile();
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile();
            return gameData;
        }
    }

    public void LoadGame()
    {
        // Player Data
        SetPlayerData(LoadingTypeSwitch().playerData);

        //Enviroment Data
    }

    private void SetPlayerData(PlayerData playerData)
    {
        //Setting Player Stats
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentCalories = playerData.playerStats[1];
        PlayerState.Instance.currentHydrationPercent = playerData.playerStats[2];

        //Setting Player Position
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        //Setting Player Rotation
        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);
    }

    public void StartLoadedGame()
    {
        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading());
    }

    IEnumerator DelayedLoading()
    {
        yield return new WaitForSeconds(1f);

        LoadGame();
    }

    #endregion
    #endregion

    #region || ---- To Binary Section ---- ||

    public void SaveGameDataToBinaryFile(AllGameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binaryPath);
    }

    public AllGameData LoadGameDataFromBinaryFile()
    {
        if(File.Exists(binaryPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath,FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data loaded from" + binaryPath);

            return data;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region || ---- To Json Section ---- ||

    public void SaveGameDataToJsonFile(AllGameData gameData)
    {
       string json = JsonUtility.ToJson(gameData);

       string encrypted = EncryptionDecryption(json);

       using(StreamWriter writer = new StreamWriter(jsonPathProject))
       {
            writer.Write(encrypted);
            print("Saved Game to Json file at:" + jsonPathProject);
       };
    }

    public AllGameData LoadGameDataFromJsonFile()
    {
        using(StreamReader reader = new StreamReader(jsonPathProject))
       {
          string json = reader.ReadToEnd();
          string decrypted = EncryptionDecryption(json);

          AllGameData data = JsonUtility.FromJson<AllGameData>(decrypted);
          return data;  
       };
    }
    #endregion

    #region || ---- Settings Section ---- ||

    #region || ---- Volume Settings ---- ||
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SavedVolumeSettings(float _music, float _effects, float _master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = _music,
            effects = _effects,
            master = _master
        };

        PlayerPrefs.SetString("Volume",JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();
    }    

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }
    #endregion
    #endregion

    #region || ---- Encryption ---- ||

    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "1234567";
        string result = "";

        for(int i = 0; i < jsonString.Length;i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }

        return result; //Encrypted or Decrypted String

        // XOR = "is there a difference"
    }


    #endregion
}


