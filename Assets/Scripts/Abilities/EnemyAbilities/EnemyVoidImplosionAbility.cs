using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyVoidImplosion", menuName = "EnemyAbilities/EnemyVoidImplosion")]
public class EnemyVoidImplosionAbility : Ability
{
    [SerializeField] float _attackRate = 0.15f;
    [SerializeField] float _aimOffset = 200f;
    [SerializeField] string _voidImplosionTag = "ThraxImplosion";

    public override void AbilityLogic(GameObject owner, Transform target, bool isUltimate = false)
    {
        // Spawn void implosions based on attackRate and amountToSpawn
        SetStatsBasedOnLevel();
        Debug.Log("Void Implosion Ability Starting...");
        owner.GetComponent<MonoBehaviour>().StartCoroutine(SpawnVoidImplosions(target));
        Debug.Log("Void Implosion Ability Started");


    }

    IEnumerator SpawnVoidImplosions(Transform target)
    {
        while (duration > 0)
        {
            Debug.Log("Spawning Void Implosion");
            Vector3 _finalOffset = new Vector3(Random.Range(-_aimOffset, _aimOffset), Random.Range(-_aimOffset, _aimOffset), 0f);
            GameObject _voidImplosion = ObjectPooler.Instance.SpawnFromPool(_voidImplosionTag, target.position + _finalOffset, Quaternion.identity);
            EnemyVoidImplosion voidImplosionScript = _voidImplosion.GetComponent<EnemyVoidImplosion>();
            yield return new WaitForSeconds(_attackRate);
            duration -= _attackRate;
        }
    }

    void SetStatsBasedOnLevel()
    {
        _aimOffset = _aimOffset - LevelManager.Instance.CurrentLevelIndex * 0.5f;
        if (_aimOffset < 100f)
        {
            _aimOffset = 100f;
        }
        duration = Mathf.Max(LevelManager.Instance.CurrentLevelIndex * 0.5f, 20f);
        _attackRate = _attackRate - LevelManager.Instance.CurrentLevelIndex * 0.01f;
        if (_attackRate < 0.1f)
        {
            _attackRate = 0.1f;
        }
    }

    public override void ResetStats()
    {
        _aimOffset = 150f;
        duration = 20f;
        _attackRate = 0.15f;
        cooldown = 40f;
        currentCooldown = cooldown;
        isUnlocked = false;
    }
}
