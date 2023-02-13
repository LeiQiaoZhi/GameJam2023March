using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject levelEndScreen;
    [SerializeField] GameObject pauseScreen;

    private void Awake()
    {
        SetEnableGameOverScreen(false);
        SetEnableLevelEndScreen(false);
    }

    public void SetEnableGameOverScreen(bool enable)
    {
        gameOverScreen.SetActive(enable);
    }

    public void SetEnableLevelEndScreen(bool enable)
    {
        levelEndScreen.SetActive(enable);
    }


    public void DisplayAchievementUnlockMessage(int i)
    {
        AchievementManager achievementManager = AchievementManager.instance;
        if (achievementManager.IsAchievementUnlocked(i))
        {
            LoggerX.Log(Category.Achievement,$"Achivement {achievementManager.achievementNames[i]} is already unlocked");
            return;
        }
        achievementManager.UnlockAchievement(i);
        MessageManager.Instance.DisplayMessage($"Achievement Unlock: {achievementManager.achievementNames[i].ToUpper()}");
    }

    public void Pause()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
  }
