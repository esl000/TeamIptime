using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance = null;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                DataManager instance = FindObjectOfType<DataManager>();
                if (instance == null)
                    _instance = new GameObject("DataManager").AddComponent<DataManager>();
                else
                    _instance = instance;
            }
            return _instance;
        }
    }

    public static readonly string DBPath = Application.dataPath + "/GameDB.db";
    public static readonly string DBConnectionPath = "URI=file:" + Application.dataPath + "/GameDB.db";

    IDbConnection Connection { get; set; }
    bool IsConnect { get => Connection != null ? Connection.State == ConnectionState.Open : false; }

    void InitializeDBFile()
    {
        if (!File.Exists(DBPath))
        {
            File.Copy(Application.streamingAssetsPath + "/GameDB.db", DBPath);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        InitializeDBFile();
        DBConnect();
        Test();
        DontDestroyOnLoad(gameObject);
    }

    void DBConnect()
    {
        try
        {
            Connection = new SqliteConnection(DBConnectionPath);
            Connection.Open();

            if (Connection.State == ConnectionState.Open)
            {
                Debug.Log("Connection");
            }
            else
            {
                Connection = null;
                Debug.Log("Not Connection");
            }
        }
        catch(Exception e)
        {
            Connection = null;
            Debug.Log(e);
        }
    }

    void Test()
    {
        if(IsConnect)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = "Select * From Item";
            IDataReader dataReader = command.ExecuteReader();

            while(dataReader.Read())
            {
                Debug.Log(dataReader.GetString(0) + " , " + dataReader.GetInt32(1));
            }

            dataReader.Dispose();
            command.Dispose();
        }
    }

    private void OnDestroy()
    {
        if(Connection != null)
        {
            Connection.Close();
            Connection = null;
        }
    }
}
