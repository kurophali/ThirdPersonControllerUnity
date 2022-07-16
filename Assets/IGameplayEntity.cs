using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameplayEntity : MonoBehaviour
{
    #region STATES
    [SerializeField] IGameplayState mStatePrefab;
    public IGameplayState mState { get; private set; }
    void InitState()
    {
        mState = Instantiate(mStatePrefab);
        mState.OnStartInit(this);
    }

    void UpdateStates()
    {
        mState.OnFixedUpdate();
    }
    #endregion

    #region ABILITIES
    [SerializeField] List<IGameplayAbility> mAbilityPrefabs;
    List<IGameplayAbility> mAbilityInstances;
    void InitAbilities()
    {
        mAbilityInstances = new List<IGameplayAbility>();

        foreach (IGameplayAbility abilityPrefab in mAbilityPrefabs)
        {
            IGameplayAbility abilityInstance = Instantiate(abilityPrefab);
            mAbilityInstances.Add(abilityInstance);
            abilityInstance.OnStartInit(this);
        }
    }
    void UpdateAbilities()
    {
        foreach (IGameplayAbility ability in mAbilityInstances)
        {
            ability.OnFixedUpdate();
        }
    }
    public void TriggerAbility(int abilityIdx, Vector4 triggerVector)
    {
        mAbilityInstances[abilityIdx].Trigger(triggerVector);
    }
    #endregion

    #region CALLBACKS
    void Start()
    {
        InitState();
        InitAbilities();
    }

    void FixedUpdate()
    {
        UpdateStates();
        UpdateAbilities();
    }
    #endregion
}
