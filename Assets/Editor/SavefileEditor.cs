using DatabaseSystem;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SavefileEditor : EditorWindow
{
    public static SavefileEditor Instance { get; private set; }

    PlayerData m_PlayerData;
    string m_PlayerDataString;
    bool m_PlayerDataSave;

    string m_textAreaString;
    Vector3 m_PlayerDataScrollPos;

    ObjectCode m_SelectedCharacter;

    StageRecordData m_AddStageRecord = new StageRecordData();
    int m_updateStageRecordWorldIndex;
    int m_updateStageRecordStageIndex;

    const int WINDOW_WIDTH = 800;
    const int WINDOW_HEIGHT = 800;

    [MenuItem("Custom Tools/Editor/Savefile Editor")]

    public static void Init()
    {
        var x = (Screen.currentResolution.width - WINDOW_WIDTH) / 2;
        var y = (Screen.currentResolution.height - WINDOW_HEIGHT) / 2;

        GetWindow<SavefileEditor>().position = new Rect(x, y, WINDOW_WIDTH, WINDOW_HEIGHT);

        EditorStyles.boldLabel.fontSize = 14;
        EditorStyles.textField.fontSize = 14;
        EditorStyles.textField.wordWrap = true;

        if (!TableManager.Instance && !JsonManager.Instance)
        {
            TableManager.Instance.LoadTable();
            JsonManager.Instance.LoadJson();
        }
        
    }

    private void Awake()
    {
        m_PlayerData = PlayerData.GetData($"{Application.persistentDataPath}/PlayerData.json");
        m_PlayerDataSave = false;

        if (m_PlayerData != null)
            UpdatePlayerData();

        Instance = this;
    }

    // The actual window code goes here
    void OnGUI()
    {
        #region ���̺� ����
        GUILayout.Label("���̺� ����", EditorStyles.boldLabel);
        GUILayout.Space(2);
        if (m_PlayerData != null)
        {
            m_PlayerDataScrollPos = EditorGUILayout.BeginScrollView(m_PlayerDataScrollPos, GUILayout.Width(WINDOW_WIDTH), GUILayout.Height(400));
            m_textAreaString = EditorGUILayout.TextArea(m_textAreaString.BeautifyJson());
            GUILayout.EndScrollView();
        }
            

        m_PlayerDataSave = EditorGUILayout.Toggle("����� ���̺� ���� ����", m_PlayerDataSave, new GUILayoutOption[] { GUILayout.Width(100) });
        #endregion

        GUILayout.Space(10);

        #region ĳ����
        GUILayout.Label("ĳ����", EditorStyles.boldLabel);
        GUILayout.Space(2);

        m_SelectedCharacter = (ObjectCode)EditorGUILayout.EnumPopup("�߰��� ĳ����:", m_SelectedCharacter);
        if (GUILayout.Button("ĳ���� �߰��ϱ�", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            if (m_SelectedCharacter.ToString().StartsWith("CHAR_"))
            {
                var exist = m_PlayerData.CharacterDatas.Find(c => c.Code == m_SelectedCharacter);
                if (exist == null)
                {
                    var data = TableManager.Instance.CharacterTable.Find(c => c.Code == m_SelectedCharacter);

                    m_PlayerData.CharacterDatas.Add(new CharacterRecordData()
                    {
                        Code = m_SelectedCharacter,
                        Level = 1,
                        EquipWeaponIndex = 0,
                        Experience = 0,
                    });
                    UpdatePlayerData();
                }
            }
            else
                Debug.LogError("CHAR_*** �� �����ϴ� ���� �߰����Ѿ� ĳ���Ͱ� �߰��˴ϴ�.");
        }

        if (GUILayout.Button("ĳ���� ��ü ����", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            m_PlayerData.CharacterDatas.Clear();
            UpdatePlayerData();
        }
        #endregion

        GUILayout.Space(10);

        #region �������� ���
        GUILayout.Label("�������� ���", EditorStyles.boldLabel);
        GUILayout.Space(2);

        if (GUILayout.Button("�������� ��� �߰��ϱ�", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            StageRecordCreateWizard.SetData(m_PlayerData);
            StageRecordCreateWizard.Open();
        }

        GUILayout.Space(1);

        if (GUILayout.Button("�������� ��� ��ü����", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            m_PlayerData.StageRecords.Clear();
            UpdatePlayerData();
        }

        #endregion

    }

    private void OnDestroy()
    {
        if (m_PlayerDataSave)
        {
            PlayerData deserialized = JsonConvert.DeserializeObject<PlayerData>(m_textAreaString);
            Debug.Log($"{m_textAreaString}");

            m_PlayerData = deserialized;
            m_PlayerData.Save();
        }
            
    }

    public void UpdatePlayerData()
    {
        m_PlayerDataString = JsonConvert.SerializeObject(m_PlayerData);
        m_textAreaString = m_PlayerDataString;
    }
}