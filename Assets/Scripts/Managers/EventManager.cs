using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{

    public static event Action<GameObject> OnEnemyDestroyed;
    public static event Action<FactionType> OnFactionInvasionWon;

    public static event Action<PrestigeType> OnPrestigeChange;
    public static event Action<PrestigeType> OnPrestigeClick;
    public static event Action OnCometDestruction;
    public static event Action<float> OnPlayerDamageDealt;
    public static event Action OnObjectiveCompleted;
    public static event Action<float> OnBulletDamage;
    public static event Action OnTeleport;
    public static event Action<float> OnShieldAbsorb;
    public static event Action OnLaserHit;
    public static event Action OnCoreEnergize;
    public static event Action OnGameOver;
    public static event Action OnNextLevel;
    public static event Action OnLevelStart;
    public static event Action OnLevelComplete;
    public static event Action<Ability> OnUltimateReady;
    public static event Action<Ability> OnUltimateUnready;

    public static event System.Action<float> OnCurrencyChange;

    public static void CometDestructionEvent()
    {
        OnCometDestruction?.Invoke();
    }
    public static void PlayerDamageDealtEvent(float damage)
    {
        OnPlayerDamageDealt?.Invoke(damage);
    }

    public static void ObjectiveCompletedEvent()
    {
        OnObjectiveCompleted?.Invoke();
    }
    public static void BulletDamageEvent(float damage)
    {
        OnBulletDamage?.Invoke(damage);
    }
    public static void TeleportEvent()
    {
        OnTeleport?.Invoke();
    }

    public static void ShieldAbsorbEvent(float damage)
    {
        OnShieldAbsorb?.Invoke(damage);
    }

    public static void LaserHitEvent()
    {
        OnLaserHit?.Invoke();
    }
    public static void CoreEnergizeEvent()
    {
        OnCoreEnergize?.Invoke();
    }
    public static void PrestigeClickEvent(PrestigeType prestige)
    {
        OnPrestigeClick?.Invoke(prestige);
    }
    public static void PrestigeChangeEvent(PrestigeType prestige)
    {
        OnPrestigeChange?.Invoke(prestige);
    }
    public static void UltimateReadyEvent(Ability ability)
    {
        OnUltimateReady?.Invoke(ability);
    }

    public static void UltimateUnreadyEvent(Ability ability)
    {
        OnUltimateUnready?.Invoke(ability);
    }

    public static void CurrencyChangeEvent(float currency)
    {
        OnCurrencyChange?.Invoke(currency);
    }
    public static void EnemyShipDestroyedEvent(GameObject go)
    {
        OnEnemyDestroyed?.Invoke(go);
    }
    public static void GameOverEvent()
    {
        OnGameOver?.Invoke();
    }

    public static void NextLevelEvent()
    {
        OnNextLevel?.Invoke();
    }

    public static void LevelStartEvent()
    {
        OnLevelStart?.Invoke();
    }

    public static void LevelCompleteEvent()
    {
        OnLevelComplete?.Invoke();
    }

    public static void FactionInvasionWonEvent(FactionType faction)
    {
        OnFactionInvasionWon?.Invoke(faction);
    }


}
