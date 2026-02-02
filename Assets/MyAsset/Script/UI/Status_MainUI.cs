using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_MainUI : MonoBehaviour
{
    public HealthGaugeComponent Health;
    public EnergyGaugeComponent Energy;
    public ChargerGaugeComponent Charger;
    public void SetComponent(Entity playerEntity)
    {
        Health.valueEntity = playerEntity;
        Energy.valueEntity = playerEntity;
        Charger.valueEntity = playerEntity;
    }
}
