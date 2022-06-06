using DatabaseSystem;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
public class SavefileEditor : EditorWindow
{
    public static SavefileEditor Instance { get; private set; }

    PlayerData m_PlayerData;
    string m_PlayerDataString;
    bool m_PlayerDataSave;
    string m_textAreaString;
    ObjectCode m_SelectedCharacter;
    StageRecordData m_AddStageRecord = new StageRecordData();
    int m_updateStageRecordWorldIndex;
    int m_updateStageRecordStageIndex;

    [MenuItem("Custom Tools/Editor/Savefile Editor")]

    public static void Init()
    {
        const int width = 800;
        const int height = 800;

        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        GetWindow<SavefileEditor>().position = new Rect(x, y, width, height);

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
            m_textAreaString = EditorGUILayout.TextArea(m_textAreaString, new GUILayoutOption[] { GUILayout.Height(300) });

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
                    var newData = new CharacterData()
                    {
                        Code = m_SelectedCharacter,
                        Hp = data.BaseHp,
                        Sp = data.BaseSp,
                        Speed = data.BaseSpeed,
                        Level = 1,
                        Critical = data.BaseCritical,
                        Damage = data.BaseDamage,
                        Defense = data.BaseDefense,
                        EquipWeaponData = new WeaponData()
                        {
                            Code = ObjectCode.NONE,
                            Critical = 0,
                            Damage = 0
                        }
                    };

                    m_PlayerData.CharacterDatas.Add(newData);
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
#endif