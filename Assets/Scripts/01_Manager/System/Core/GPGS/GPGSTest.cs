using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPGSTest : MonoBehaviour
{
    string _log;

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(new Vector3(50, 50, 0), Quaternion.identity, Vector3.one * 3);

        if (GUILayout.Button("ClearLog"))
            _log = "";

        if (GUILayout.Button("Login"))
        {
            GPGSBinder.Inst.Login((success, localUser) =>
            {
                _log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}";
            });
        }

        if (GUILayout.Button("Logout"))
            GPGSBinder.Inst.Logout();

        // ------------------------------

        if (GUILayout.Button("SaveCloud"))
            GPGSBinder.Inst.SaveCloud("mysave", "example", (success) => _log = $"{success}");

        if (GUILayout.Button("LoadCloud"))
            GPGSBinder.Inst.LoadCloud("mysave", (success, data) => _log = $"{success}, {data}");

        if (GUILayout.Button("DeleteCloud"))
            GPGSBinder.Inst.DeleteCloud("mysave", (success) => _log = $"{success}");

        // ------------------------------

        //if (GUILayout.Button("ShowAchievementUI"))
        //    GPGSBinder.Inst.ShowAchievementUI();

        //if (GUILayout.Button("UnlockAchievement_one"))
        //    GPGSBinder.Inst.UnlockAchievement(GPGSIds.achievement_one, (success) => _log = $"{success}");

        //// ps.achievement_two ¼û±ä ¾÷Àû
        //if (GUILayout.Button("UnlockAchievement_two"))
        //    GPGSBinder.Inst.UnlockAchievement(GPGSIds.achievement_two, (success) => _log = $"{success}");

        //if (GUILayout.Button("UnlockAchievement_three"))
        //    GPGSBinder.Inst.IncrementAchievement(GPGSIds.achievement_three, 1, (success) => _log = $"{success}");

        //// ------------------------------

        //if (GUILayout.Button("ShowAllLeaderBoardUI"))
        //    GPGSBinder.Inst.ShowAllLeaderboardUI();

        //if (GUILayout.Button("ShowAllLeaderBoardUI_num"))
        //    GPGSBinder.Inst.ShowTargetLeaderboardUI(GPGSIds.leaderboard_num);

        //if (GUILayout.Button("ReportLeaderBoard_num"))
        //    GPGSBinder.Inst.ReportLeaderboard(GPGSIds.leaderboard_num, 1000, (success) => _log = $"{success}");

        //if (GUILayout.Button("LoadAllLeaderboardArray_num"))
        //    GPGSBinder.Inst.LoadAllLeaderboardArray(GPGSIds.leaderboard_num, (scores) =>
        //    {
        //        _log = "";
        //        for (int i = 0; i < scores.Length; i++)
        //            _log += $"{i}, {scores[i].rank}, {scores[i].value}, {scores[i].userID}, {scores[i].date}\n";
        //    });

        //if (GUILayout.Button("LoadCustomLeaderboardArray_num"))
        //    GPGSBinder.Inst.LoadCustomLeaderboardArray(GPGSIds.leaderboard_num, 10,
        //        GooglePlayGames.BasicApi.LeaderboardStart.PlayerCentered, GooglePlayGames.BasicApi.LeaderboardTimeSpan.Daily, (success, scoreData) =>
        //         {
        //             _log = $"{success}\n";
        //             var scores = scoreData.Scores;
        //             for (int i = 0; i < scores.Length; i++)
        //                 _log += $"{i}, {scores[i].rank}, {scores[i].value}, {scores[i].userID}, {scores[i].date}\n";
        //         });

        //// ------------------------------

        //if (GUILayout.Button("IncrementEvent_event"))
        //    GPGSBinder.Inst.IncrementEvent(GPGSIds.event_event, 1);

        //if (GUILayout.Button("LoadEvent_event"))
        //    GPGSBinder.Inst.LoadEvent(GPGSIds.event_event, (success, iEvent) =>
        //    {
        //        _log = $"{success}, {iEvent.Name}, {iEvent.CurrentCount}";
        //    });

        //if (GUILayout.Button("LoadAllEvent"))
        //    GPGSBinder.Inst.LoadAllEvent((success, iEvents) =>
        //    {
        //        _log = $"{success}\n";
        //        foreach (var iEvent in iEvents)
        //            _log += $"{iEvent.Name}, {iEvent.CurrentCount}, {iEvent.Description}\n";
        //    });



        GUILayout.Label(_log);
    }
}
