using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_MainUI : MonoBehaviour
{
    public HealthGaugeComponent Health;
    public EnergyGaugeComponent Energy;
    public ChargerGaugeComponent Charger;
    public GuardGaugeComponent Guard;
    public void SetComponent(Entity playerEntity)
    {
        Health.valueEntity = playerEntity;
        Energy.valueEntity = playerEntity;
        Charger.valueEntity = playerEntity;
        Guard.valueEntity = playerEntity;
    }
    
    public void SetPreGameUIActive()
    {
        // PreGameUI.SetActive(true);
    }

    public void SetOnGameActive()
    {
        // Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, 0.3f);
        // GameOverCams.enabled = false;
        // pauseGameUI.SetActive(false);
        // PreGameUI.SetActive(false);
        // InGameUI.SetActive(true);
        // if(inGameAuds != null)
        // inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 1f, 0.08f);
        // elapsedTime += Time.deltaTime;
    }

    public void SetGameoverActive()
    {
        // InGameUI.SetActive(false);
        // pauseGameUI.SetActive(false);
        // GameOverCams.enabled = true;
        // Time.timeScale = Mathf.Lerp(Time.timeScale, 0.001f, 0.025f);
        // if (!isGameOverUIShown)
        // {
        //     GameOverUI.SetActive(true);
        //     isGameOverUIShown = true;
        // }
        // if(inGameAuds != null)
        // inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 0.001f, 0.025f);
    }
}
