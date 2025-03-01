using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometLevel : Level
{
    int _cometCount = 300;
    float _cometSpawnRate = 0.5f;
    bool _roundStarted = false;


    public CometLevel(int cometCount, float cometSpawnRate)
    {
        _cometCount = cometCount;
        _cometSpawnRate = cometSpawnRate;
    }
    public override void StartLevel()
    {
        SpawnerManager.Instance.StartCoroutine(SpawnerManager.Instance.SpawnCometsOverTime(_cometCount, _cometSpawnRate));
        GameManager.Instance.StartCoroutine(ActivateCometObjective());

    }

    IEnumerator ActivateCometObjective()
    {
        yield return new WaitForSeconds(3f);
        ObjectiveManager.Instance.ActivateSpecialObjective("Comet", this, _levelObjectives);
    }

    public override void UpdateLevel()
    {
        if (!_roundStarted)
        {
            _roundStarted = true;
            SpawnerManager.Instance.StartCoroutine(EndRoundAfter(60f));
        }
    }

    public override void CompleteLevel()
    {
        SpawnerManager.Instance.StopAllCoroutines();
        LevelManager.Instance.CompleteLevel();
    }

    IEnumerator EndRoundAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CompleteLevel();
    }
}
