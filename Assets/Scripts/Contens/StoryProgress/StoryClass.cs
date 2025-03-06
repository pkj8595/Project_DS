using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvent
{
    public string Title { get; set; }            // 이벤트 제목
    public string Description { get; set; }      // 이벤트 설명
    public int Difficulty { get; set; }          // 이벤트 난이도
    public ActionType EventType { get; set; }    // 이벤트 유형 (예: 전투, 상점, 특별한 이벤트 등)

    public StoryEvent(string title, string description, int difficulty, ActionType eventType)
    {
        Title = title;
        Description = description;
        Difficulty = difficulty;
        EventType = eventType;
    }

}

public enum ActionType
{
    Battle,
    Shop,
    Special
}

public class BattleResult
{
    public int EnemiesDefeated { get; set; }     // 처치한 적의 수
    public int AlliesLost { get; set; }          // 잃은 아군의 수
    public int ResourcesGained { get; set; }     // 전투로 얻은 자원
    public bool IsVictory { get; set; }          // 전투의 승패 여부

    public BattleResult(int enemiesDefeated, int alliesLost, int resourcesGained, bool isVictory)
    {
        EnemiesDefeated = enemiesDefeated;
        AlliesLost = alliesLost;
        ResourcesGained = resourcesGained;
        IsVictory = isVictory;
    }

    public string GetSummary()
    {
        return $"Enemies Defeated: {EnemiesDefeated}, Allies Lost: {AlliesLost}, Resources Gained: {ResourcesGained}, Victory: {IsVictory}";
    }
}