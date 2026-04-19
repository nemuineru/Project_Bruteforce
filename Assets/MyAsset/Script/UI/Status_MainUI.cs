using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_MainUI : MonoBehaviour
{
    public HealthGaugeComponent Health;
    public EnergyGaugeComponent Energy;
    public ChargerGaugeComponent Charger;
    public GuardGaugeComponent Guard;

    public GameObject PreGameScreen;
    public GameObject OnGameScreen;
    public GameObject GameOverScreen;
    public GameObject GameClearScreen;
    public GameObject PauseScreen;

    //those GameUI comps are so crude.. but I need it to make them move
    public void SetComponent(Entity playerEntity)
    {
        Health.valueEntity = playerEntity;
        Energy.valueEntity = playerEntity;
        Charger.valueEntity = playerEntity;
        Guard.valueEntity = playerEntity;
    }
    
    public void SetPreGameUIActive()
    {
        PreGameScreen.SetActive(true);
        OnGameScreen.SetActive(false);
    }

    public void SetOnGameActive()
    {
        PauseScreen.SetActive(false);
        PreGameScreen.SetActive(false);
        OnGameScreen.SetActive(true);
    }

    public void SetGamePauseActive()
    {
        OnGameScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void SetGameClearActive()
    {
        OnGameScreen.SetActive(false);
        GameClearScreen.SetActive(true);
    }

    public void SetGameOverActive()
    {
        OnGameScreen.SetActive(false);
        GameOverScreen.SetActive(true);
    }
}
