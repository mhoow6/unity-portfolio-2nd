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

    [Header("# ��������"), Tooltip("�ʴ� �ö󰡴� ����ġ")]
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

        // ���ȭ��
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == result.WorldIdx && stage.StageIdx == result.StageIdx);
        StageBackground.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{stageData.StageImage}");

        // �������� �̸�
        StageName.text = stageData.StageName;

        // ���� ����
        int playerLevel = GameManager.PlayerData.Level;
        int playerExperience = GameManager.PlayerData.Experience;
        Level.text = $"LV.{playerLevel}";

        // ���� ����ġ
        var levelData = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == playerLevel);
        LevelExperience.text = $"{playerExperience} / {levelData.MaxExperience}";
        float experienceRatio = (playerExperience + playerGetExperience) / levelData.MaxExperience;
        float tweenDuration = Mathf.Min(4f * experienceRatio, 4f);

        // ���� ����ġ Ʈ����
        //LevelExperience.DOText($"{playerExperience + playerGetExperience} / {levelData.MaxExperience}", tweenDuration);
        StartCoroutine(IncreasePlayerLevelSliderCoroutine(LevelSlider, playerLevel, playerExperience, (playerExperience + playerGetExperience)));

        // ���� ��� ����ġ ǥ��
        ExperienceGain.text = $"+{playerGetExperience}<color=#02C3FE>Exp</color>";
        ExperienceGain.gameObject.DODisable(2f);
    }

    IEnumerator IncreasePlayerLevelSliderCoroutine(SplitSlider slider, int currentLevel, int currentExp, float endValue)
    {
        float desiredValue = endValue;
        int level = currentLevel;
        float getExperiencePerFrame = GetExperiencePerSecond / Application.targetFrameRate;

        // �����ؾ��ϴ� ����ġ, �� ������ �´� �ִ����ġ
        Queue<(float, int)> endValues = new Queue<(float, int)>();

        // endValues ����
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

        // ���� �����̴� �ʱ� ����
        (float, int) desired = endValues.Dequeue();
        LevelSlider.SetData(0, desired.Item2, (exp) =>
        {
            LevelExperience.text = $"{Mathf.Floor(exp)} / {desired.Item2}";
        });
        LevelSlider.Value = currentExp;

        // ����ġ ���� ����
        while (endValues.Count != 0)
        {
            if (slider.Value < desired.Item1)
                slider.Value += Time.deltaTime * 100f * getExperiencePerFrame;
            else
            {
                desired = endValues.Dequeue();
                slider.SetData(0, desired.Item2, (exp) =>
                {
                    LevelExperience.text = $"{Mathf.Floor(exp)} / {desired.Item2}";
                });
                slider.Value = 0;
                OnPlayerLevelUp(++level);
            }

            yield return null;
        }

        // ���� ����ġ ���� ����
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
}
