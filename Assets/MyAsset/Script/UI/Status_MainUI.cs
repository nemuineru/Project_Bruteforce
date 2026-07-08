using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_MainUI : MonoBehaviour
{
    public HealthGaugeComponent Health;
    public EnergyGaugeComponent Energy;
    public ChargerGaugeComponent Charger;
    public GuardGaugeComponent Guard;

    public EquipmentUI EquipUI;

    public EquipmentStrings strsUI;

    public GameObject PreGameScreen;
    public GameObject OnGameScreen;
    public GameObject GameOverScreen;
    public GameObject GameClearScreen;
    public GameObject PauseScreen;

    public void Start()
    {
        SetComponent(gameState.self.Player);
    }

    //gameStateの状態に応じてUIを切り替える
    public void Update()
    {
        if (gameState.self != null)
        {
            // switch (gameState.self.menuStatus)
            // {
            //     case gameState._MenuStatus.PreStart:
            //         SetPreGameUIActive();
            //         break;
            //     case gameState._MenuStatus.OnGame:
            //         SetOnGameActive();
            //         break;
            //     case gameState._MenuStatus.Pause:
            //         SetGamePauseActive();
            //         break;
            //     case gameState._MenuStatus.GameClear:
            //         SetGameClearActive();
            //         break;
            //     case gameState._MenuStatus.GameOver:
            //         SetGameOverActive();
            //         break;
            // }
        }
    }

    //those GameUI comps are so crude.. but I need it to make them move
    public void SetComponent(Entity playerEntity)
    {
        Health.valueEntity = playerEntity;
        Energy.valueEntity = playerEntity;
        Charger.valueEntity = playerEntity;
        EquipUI.mainEntity = playerEntity;
        strsUI.mainEntity = playerEntity;
        // Guard.valueEntity = playerEntity;
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
