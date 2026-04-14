public static class GameSessionState
{
    public static bool HasActiveRun { get; private set; }
    public static int CurrentLevelNumber { get; private set; } = 1;

    public static void StartNewRun()
    {
        HasActiveRun = true;
        CurrentLevelNumber = 1;
    }

    public static void ResumeLevel(int levelNumber)
    {
        HasActiveRun = true;
        CurrentLevelNumber = levelNumber < 1 ? 1 : levelNumber;
    }

    public static void AdvanceLevel(int maxLevelNumber)
    {
        HasActiveRun = true;
        CurrentLevelNumber++;
        if (CurrentLevelNumber > maxLevelNumber)
        {
            CurrentLevelNumber = maxLevelNumber;
        }
    }

    public static void ResetRun()
    {
        HasActiveRun = false;
        CurrentLevelNumber = 1;
    }
}
