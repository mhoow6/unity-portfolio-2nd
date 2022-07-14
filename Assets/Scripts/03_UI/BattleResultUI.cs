using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using DG.Tweening;

public class BattleResultUI : UI
{
    public override UIType Type => UIType.BattleResult;

    [Header("# Left")]
    public Image StageBackground;
    public Text StageName;

    public Text Level;
    public Text LevelExperience;
    public Text ExperienceGain;
    public SplitSlider LevelSlider;

    public List<SelectCharacterUI> SelectCharacterUIs = new List<SelectCharacterUI>(3);
    public List<Text> CharacterExperienceGains = new List<Text>(3);
    public List<SplitSlider> CharacterExperiences = new List<SplitSlider>(3);

    [Header("# Right")]
    public Text GoldGain;
    public List<ItemUI> Items = new List<ItemUI>(5);

    [Header("# 수동기입"), Tooltip("초당 올라가는 경험치")]
    public float GetExperiencePerSecond;

    public override void OnClosed()
    {
        SelectCharacterUIs.Clear();
        CharacterExperienceGains.Clear();
        Items.Clear();
    }

    public override void OnOpened()
    {
        
    }

    public void SetData(StageResultData result)
    {
        float playerGetExperience = result.Score * 0.1f;
        float characterGetExperience = result.Score * 0.5f;

        // 배경화면
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == result.WorldIdx && stage.StageIdx == result.StageIdx);
        StageBackground.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{stageData.StageImage}");

        // 스테이지 이름
        StageName.text = stageData.StageName;

        // 함장 레벨
        int playerLevel = GameManager.PlayerData.Level;
        int playerExperience = GameManager.PlayerData.Experience;
        Level.text = $"LV.{playerLevel}";

        // 함장 경험치
        var levelData = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == playerLevel);
        LevelExperience.text = $"{playerExperience} / {levelData.MaxExperience}";

        // 함장 경험치 트위닝
        StartCoroutine(IncreasePlayerLevelSliderCoroutine(LevelSlider, LevelExperience, playerLevel, playerExperience, (playerExperience + playerGetExperience)));

        // 함장 얻는 경험치 표시
        ExperienceGain.text = $"+{playerGetExperience}<color=#02C3FE>Exp</color>";
        ExperienceGain.gameObject.DODisable(2f);

        // 파티 프리셋 보여주기
        var playerStageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == result.WorldIdx && stage.StageIdx == result.StageIdx);
        SelectCharacterUIs[0].SetData(playerStageRecord.CharacterLeader);
        SelectCharacterUIs[0].Raycastable = false;
        SelectCharacterUIs[0].IsLeaderSlot = true;
        SelectCharacterUIs[1].SetData(playerStageRecord.CharacterSecond);
        SelectCharacterUIs[1].Raycastable = false;
        SelectCharacterUIs[1].IsLeaderSlot = false;
        SelectCharacterUIs[2].SetData(playerStageRecord.CharacterThird);
        SelectCharacterUIs[2].Raycastable = false;
        SelectCharacterUIs[2].IsLeaderSlot = false;

        // 리더 캐릭터 경험치
        var leaderCharacterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == SelectCharacterUIs[0].DisplayedCharacter);
        StartCoroutine(IncreaseCharacterLevelSliderCoroutine(
            CharacterExperiences[0],
            SelectCharacterUIs[0],
            leaderCharacterRecord.Level,
            leaderCharacterRecord.Experience,
            (leaderCharacterRecord.Experience + characterGetExperience)));

        // 리더 캐릭터 얻는 경험치 표시
        CharacterExperienceGains[0].text = $"+{characterGetExperience}<color=#02C3FE>Exp</color>";
        CharacterExperienceGains[0].gameObject.DODisable(2f);

        // 두번째 캐릭터 경험치
        if (SelectCharacterUIs[1].DisplayedCharacter != ObjectCode.NONE)
        {
            CharacterExperiences[1].gameObject.SetActive(true);
            var secondCharacterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == SelectCharacterUIs[1].DisplayedCharacter);
            StartCoroutine(IncreaseCharacterLevelSliderCoroutine(
                CharacterExperiences[1],
                SelectCharacterUIs[1],
                secondCharacterRecord.Level,
                secondCharacterRecord.Experience,
                (secondCharacterRecord.Experience + characterGetExperience)));

            // 두번째 캐릭터 얻는 경험치 표시
            CharacterExperienceGains[1].gameObject.SetActive(true);
            CharacterExperienceGains[1].text = $"+{characterGetExperience}<color=#02C3FE>Exp</color>";
            CharacterExperienceGains[1].gameObject.DODisable(2f);
        }
        else
        {
            CharacterExperiences[1].gameObject.SetActive(false);
            CharacterExperienceGains[1].gameObject.SetActive(false);
        }


        // 세번째 캐릭터 경험치
        if (SelectCharacterUIs[2].DisplayedCharacter != ObjectCode.NONE)
        {
            CharacterExperiences[2].gameObject.SetActive(true);
            var thirdCharacterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == SelectCharacterUIs[2].DisplayedCharacter);
            StartCoroutine(IncreaseCharacterLevelSliderCoroutine(
                CharacterExperiences[2],
                SelectCharacterUIs[2],
                thirdCharacterRecord.Level,
                thirdCharacterRecord.Experience,
                (thirdCharacterRecord.Experience + characterGetExperience)));

            // 두번째 캐릭터 얻는 경험치 표시
            CharacterExperienceGains[2].gameObject.SetActive(true);
            CharacterExperienceGains[2].text = $"+{characterGetExperience}<color=#02C3FE>Exp</color>";
            CharacterExperienceGains[2].gameObject.DODisable(2f);
        }
        else
        {
            CharacterExperiences[2].gameObject.SetActive(false);
            CharacterExperienceGains[2].gameObject.SetActive(false);
        }
    }

    IEnumerator IncreasePlayerLevelSliderCoroutine(SplitSlider slider, Text representText, int currentLevel, int currentExp, float endValue)
    {
        float desiredValue = endValue;
        int level = currentLevel;
        float getExperiencePerFrame = GetExperiencePerSecond / Application.targetFrameRate;

        // 도달해야하는 경험치, 그 레벨에 맞는 최대경험치
        Queue<(float, int)> endValues = new Queue<(float, int)>();

        // endValues 세팅
        var levelData = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == level);
        while (levelData.MaxExperience < desiredValue)
        {
            endValues.Enqueue((levelData.MaxExperience, levelData.MaxExperience));
            desiredValue -= levelData.MaxExperience;
            levelData = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == level + 1);
            level = level + 1;
        }
        endValues.Enqueue((desiredValue, levelData.MaxExperience));

        level = currentLevel;

        // 레벨 슬라이더 초기 세팅
        (float, int) desired = endValues.Dequeue();
        slider.SetData(0, desired.Item2, (exp) =>
        {
            representText.text = $"{Mathf.Floor(exp)} / {desired.Item2}";
        });
        slider.Value = currentExp;

        // 경험치 증가 연출
        while (endValues.Count != 0)
        {
            if (slider.Value < desired.Item1)
                slider.Value += Time.deltaTime * 100f * getExperiencePerFrame;
            else
            {
                desired = endValues.Dequeue();
                slider.SetData(0, desired.Item2, (exp) =>
                {
                    representText.text = $"{Mathf.Floor(exp)} / {desired.Item2}";
                });
                slider.Value = 0;
                OnPlayerLevelUp(++level);
            }

            yield return null;
        }

        // 남은 경험치 증가 연출
        while (true)
        {
            if (slider.Value < desired.Item1)
                slider.Value += Time.deltaTime * 100f * getExperiencePerFrame;
            else
                yield break;

            yield return null;
        }
    }

    void OnPlayerLevelUp(float level)
    {
        Level.text = $"LV.{level}";
    }

    IEnumerator IncreaseCharacterLevelSliderCoroutine(SplitSlider slider, SelectCharacterUI characterUI, int currentLevel, int currentExp, float endValue)
    {
        float desiredValue = endValue;
        int level = currentLevel;
        float getExperiencePerFrame = GetExperiencePerSecond / Application.targetFrameRate * 5f;

        // 도달해야하는 경험치, 그 레벨에 맞는 최대경험치
        Queue<(float, int)> endValues = new Queue<(float, int)>();

        // endValues 세팅
        var levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level);
        while (levelData.MaxExperience < desiredValue)
        {
            endValues.Enqueue((levelData.MaxExperience, levelData.MaxExperience));
            desiredValue -= levelData.MaxExperience;
            levelData = TableManager.Instance.CharacterLevelExperienceTable.Find(row => row.Level == level + 1);
            level = level + 1;
        }
        endValues.Enqueue((desiredValue, levelData.MaxExperience));

        level = currentLevel;

        // 레벨 슬라이더 초기 세팅
        (float, int) desired = endValues.Dequeue();
        slider.SetData(0, desired.Item2);
        slider.Value = currentExp;

        // 경험치 증가 연출
        while (endValues.Count != 0)
        {
            if (slider.Value < desired.Item1)
                slider.Value += Time.deltaTime * 100f * getExperiencePerFrame;
            else
            {
                desired = endValues.Dequeue();
                slider.SetData(0, desired.Item2);
                slider.Value = 0;
                OnCharacterLevelUp(characterUI, ++level);
            }

            yield return null;
        }

        // 남은 경험치 증가 연출
        while (true)
        {
            if (slider.Value < desired.Item1)
                slider.Value += Time.deltaTime * 100f * getExperiencePerFrame;
            else
                yield break;

            yield return null;
        }
    }

    void OnCharacterLevelUp(SelectCharacterUI characterUI, float level)
    {
        characterUI.CharacterLevel.text = $"Lv. {level}";
    }
}
