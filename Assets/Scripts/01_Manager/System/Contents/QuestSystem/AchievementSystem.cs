using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class AchievementSystem : QuestSystem
{
    const int ACHIEVEMENT_START_INDEX = 3000;

    public void Init(JsonManager jsonManager)
    {
        // 업적데이터를 퀘스트로 만들어야 한다.
        List<Questable> achievements = new List<Questable>();
        foreach (var kvp in jsonManager.JsonDatas)
        {
            int idx = kvp.Key;
            var jsonData = kvp.Value;

            if (idx >= ACHIEVEMENT_START_INDEX)
            {
                var achievement = new Questable()
                {
                    Index = idx,
                    PurposeCount = (kvp.Value as Questable).PurposeCount
                };
                achievements.Add(achievement);
            }
                
        }

        // 퀘스트로 만들면 여기에 등록을 해주자.
        GameManager.AchievementSystem.Register(achievements);
    }
}
