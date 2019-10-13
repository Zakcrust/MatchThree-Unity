using UnityEngine;
public abstract class TileEvent
{
    //Abstract class untuk base event dari tile
    public abstract int setValue(int value);
    //Apa yang terjadi jika tile match
    public abstract void OnMatch();
    //Check jika persyaratn event telah terpenuhi
    public abstract bool AchievementCompleted();
}

public class CookieTileEvent : TileEvent
{
    int value;
    int currentTileStreak;
    bool isPass;
    public CookieTileEvent(int value)
    {
        this.value = setValue(value);
        currentTileStreak = 0;
        isPass = false;
    }
    public override int setValue(int value)
    {
        return value;
    }
    public override void OnMatch()
    {
        currentTileStreak++;
        Debug.Log(currentTileStreak);
        if (currentTileStreak >= value)
            isPass = true;
    }
    public override bool AchievementCompleted()
    {
        return isPass;
    }
}

public class CakeTileEvent : TileEvent
{
    int value;
    int currentTileStreak;
    bool isPass;
    public CakeTileEvent(int value)
    {
        this.value = setValue(value);
        currentTileStreak = 0;
        isPass = false;
    }
    public override int setValue(int value)
    {
        return value;
    }

    public override void OnMatch()
    {
        currentTileStreak++;
        Debug.Log(currentTileStreak);
        if(currentTileStreak >= value)
            isPass = true;
    }

    public override bool AchievementCompleted()
    {
        return isPass;
    }
}


public class StarTileEvent : TileEvent
{
    int value;
    int currentTileStreak;
    bool isPass;
    public StarTileEvent(int value)
    {
        this.value = setValue(value);
        currentTileStreak = 0;
        isPass = false;
    }
    public override int setValue(int value)
    {
        return value;
    }
    public override void OnMatch()
    {
        currentTileStreak++;
        Debug.Log(currentTileStreak);
        if (currentTileStreak >= value)
            isPass = true;
    }

    public override  bool AchievementCompleted()
    {
        return isPass;
    }
}

