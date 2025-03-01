using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboManager : MonoBehaviour
{
    float playerCurrentPostHealth;
    float playerPostHealth;
    ComboManager _comboManager;
    int comboCount;
    PlayerManager _playerManager;
    GameObject damageEffect;
    AudioSource _audioSource;
    [SerializeField] AudioClip _onComboAbilityExecute;

    // Track the last multiple of 500 where the AOE ability was used
    int _lastComboThresholdUsed = 0;

    // Track whether specific buffs have been activated
    Dictionary<int, bool> buffsActivated;

    void Start()
    {
        _comboManager = ComboManager.Instance;
        _playerManager = PlayerManager.Instance;
        _audioSource = GetComponent<AudioSource>();

        // Initialize dictionary to track activated buffs
        buffsActivated = new Dictionary<int, bool>
        {
            { 25, false },
            { 50, false },
            { 75, false },
            { 100, false },
            { 150, false },
            { 250, false }
        };
    }

    void Update()
    {
        comboCount = _comboManager.comboCount;  // Correctly assign the comboCount from the ComboManager

        List<int> comboActionsToActivate = new List<int>();

        // First pass: Determine which combo actions need to be activated
        foreach (var comboAction in buffsActivated.Keys)
        {
            if (comboCount >= comboAction && !buffsActivated[comboAction])
            {
                comboActionsToActivate.Add(comboAction);
            }
        }

        // Second pass: Activate those combo actions and update the dictionary
        foreach (var comboAction in comboActionsToActivate)
        {
            ExecuteComboAction(comboAction);
            buffsActivated[comboAction] = true; // Now it’s safe to modify the dictionary
        }

        // Check for the AOE combo ability (multiple of 500)
        if (comboCount >= 500 && CanUseAOECombo(comboCount))
        {
            // Activate the UI to show the combo ability is ready
            UIManager.Instance.ActivateComboKey();

            // Check for input to trigger the AOE ability
            if (Input.GetKeyDown(KeyCode.R))
            {
                ExecuteAOEComboAction();
                _lastComboThresholdUsed = comboCount / 500 * 500; // Update the last combo threshold
            }
        }
    }



    bool CanUseAOECombo(int comboCount)
    {
        // Check if the current threshold is a multiple of 500 and has not been used yet
        int currentComboThreshold = comboCount / 500 * 500;
        return currentComboThreshold > _lastComboThresholdUsed;
    }

    void ExecuteComboAction(int comboThreshold)
    {
        switch (comboThreshold)
        {
            case 25:
                IncreasePlayerSpeed();
                break;
            case 50:
                IncreasePlayerBulletSpeed();
                break;
            case 75:
                IncreasePlayerPickUpRadius();
                break;
            case 100:
                IncreasePlayerHealth();
                break;
            case 150:
                IncreasePlayerDamage();
                break;
            case 250:
                IncreaseBulletAmount();
                break;
        }
        ComboAnimation();
        AudioManager.Instance.PlaySound(GameManager.Instance._audioSource, _onComboAbilityExecute);
        _playerManager.ActivateBuffAnimations();
    }

    void ExecuteAOEComboAction()
    {
        UIManager.Instance.DeactivateComboKey();
        DealAOEDamage(); // Trigger the AOE ability
    }

    void IncreasePlayerSpeed()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 5;
        _playerManager.Movement().moveSpeed *= 1.5f;
        UIManager.Instance.movespeedBuff.SetActive(true);
    }

    void IncreasePlayerBulletSpeed()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 10;
        _playerManager.Weapon().bulletSpeed *= 1.5f;
        UIManager.Instance.bulletspeedBuff.SetActive(true);
    }

    void IncreasePlayerDamage()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 20;
        _playerManager.Weapon().bulletDamage *= 1.5f;
        UIManager.Instance.bulletdamageBuff.SetActive(true);
    }

    void IncreasePlayerPickUpRadius()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 15;
        _playerManager.PickUpBehaviour().PickUpRadius *= 2f;
        UIManager.Instance.pickupradiusBuff.SetActive(true);
    }

    void IncreaseBulletAmount()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 25;
        _playerManager.Weapon().amountOfBullets += 2;
        UIManager.Instance.bulletcountBuff.SetActive(true);
    }

    void IncreasePlayerHealth()
    {
        var emission = _playerManager.arrowEmission.emission;
        emission.rateOverTime = 20;
        playerPostHealth = (_playerManager.MaxHealth() * 1.25f) - _playerManager.MaxHealth();
        playerCurrentPostHealth = (_playerManager.CurrentHealth() * 1.25f) - _playerManager.CurrentHealth();
        _playerManager.SetMaxHealth(playerPostHealth);
        _playerManager.SetCurrentHealth(playerCurrentPostHealth);
        UIManager.Instance.healthBuff.SetActive(true);
    }

    void DealAOEDamage()
    {
        damageEffect = ObjectPooler.Instance.SpawnFromPool("ComboAOE", transform.position, Quaternion.identity);
        damageEffect.transform.rotation = Quaternion.Euler(-90, 0, 0);
        damageEffect.transform.SetParent(_playerManager.transform);
        damageEffect.transform.localScale = Vector3.zero;
        StartCoroutine(IncreaseAOEScale(damageEffect, 70f, 1.5f));
    }

    void ComboAnimation()
    {
        GameObject animEffect = ObjectPooler.Instance.SpawnFromPool("ComboPowerUp", transform.position, Quaternion.identity);
        animEffect.transform.localScale = Vector3.zero;
        StartCoroutine(IncreaseAOEScale(animEffect, 50f, 2f));
        StartCoroutine(FadeOut(animEffect, 2f));
    }

    IEnumerator IncreaseAOEScale(GameObject effect, float size, float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration; // Normalized value between 0 and 1

            // Scale the AOE effect
            effect.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(size, size, size), progress);

            yield return null;
        }

        effect.SetActive(false);
    }

    IEnumerator FadeOut(GameObject target, float duration)
    {
        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("GameObject does not have a SpriteRenderer component.");
            yield break;
        }

        Color originalColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // Interpolate from 1 to 0
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // Apply the new alpha

            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure it's fully transparent at the end
        target.SetActive(false); // Optionally deactivate the GameObject after fading
    }

    // Method to remove all buffs and reset combo thresholds
    public void RemoveAllBuffs()
    {
        _playerManager.DeactivateBuffAnimations();
        UIManager.Instance.DeactivateAllBuffIcons();

        // Copy the keys into a list to avoid modifying the collection while iterating
        List<int> keys = new List<int>(buffsActivated.Keys);

        // Reset all buffs activated for each threshold
        foreach (var key in keys)
        {
            if (buffsActivated[key])
            {
                RemoveBuff(key);
                buffsActivated[key] = false; // Mark the buff as deactivated
            }
        }

        // Reset the last combo threshold used for AOE
        _lastComboThresholdUsed = 0;
    }


    // Method to remove individual buffs based on combo threshold
    void RemoveBuff(int comboThreshold)
    {
        switch (comboThreshold)
        {
            case 25:
                _playerManager.Movement().moveSpeed /= 1.5f;
                break;
            case 50:
                _playerManager.Weapon().bulletSpeed /= 1.5f;
                break;
            case 75:
                _playerManager.PickUpBehaviour().PickUpRadius /= 2f;
                break;
            case 100:
                _playerManager.SetMaxHealth(-playerPostHealth);
                _playerManager.SetCurrentHealth(-playerCurrentPostHealth);
                break;
            case 150:
                _playerManager.Weapon().bulletDamage /= 1.5f;
                break;
            case 250:
                _playerManager.Weapon().amountOfBullets -= 2;
                break;
        }
    }
}














