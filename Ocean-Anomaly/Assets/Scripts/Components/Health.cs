using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Attributes;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	/// <summary>
	/// Used to modify the health in a fancy way. Negative valued status effects will effect health overtime regardless of what is modifying health.
	/// </summary>
	public enum StatusEffect
	{
		/// <summary>
		/// Makes you loose health till there is no more hp.
		/// </summary>
		Bleeding = -3,
		/// <summary>
		/// Makes you take loose health till 1 hp.
		/// </summary>
		Poisoned = -2,
		/// <summary>
		/// Makes you regenerate a certain percentage till full.
		/// </summary>
		Regeneration = -1,
		/// <summary>
		/// Pretty self explanitory this one is.
		/// </summary>
		None,
		/// <summary>
		/// Makes you take percentage additional damage.
		/// </summary>
		Vulnerable,
		/// <summary>
		/// Makes you take a percentage of damage less.
		/// </summary>
		Resilient,
		/// <summary>
		/// Makes you take 0 damage instead.
		/// </summary>
		Invulnerable,
		/// <summary>
		/// Makes damage you would take increase the health instead of subtract it.
		/// This will also ignore the <seealso cref="maxHealth"/>.
		/// </summary>
		Adsorption
	}
	[Serializable]
	public class Health : MonoBehaviour
	{
		public float startHealth = 100;
		public float maxHealth = 100;
		public float regenRate = 5f;
		public float bleedRate = 2f;
		public float poisonRate = 5f;
		public float vulnerablePercentage = 0.1f;
		public float resilientPercentage = 0.1f;
		/// <summary>
		/// In seconds for the Coroutine to update.
		/// </summary>
		public float updateHealthRate = 1f;
		public List<StatusEffect> activeStatusEffects { get; private set; }
		public float currentHealth { get; private set; }
		public UnityEvent noHealthEvent;
		public UnityEvent changeHealthEvent;
		public UnityEvent changeStatusEffectEvent;
		private IEnumerator updateHealthRoutine;
		private bool updatingHealth = false;

		private void Awake()
		{
			activeStatusEffects = new List<StatusEffect>();
			currentHealth = startHealth;
		}

		/// <summary>
		/// Used to add a status effect to the active status effects on this component.
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		public bool AddStatusEffect(StatusEffect effect)
		{
			if (activeStatusEffects.Contains(effect))
			{
				return false;
			}
			if (currentHealth <= 0)
			{
				return false;
			}
			// If we aren't updating health we wanna start doing that with the addition
			// status effects that are less than 0 in the enum.
			if (!updatingHealth & (effect < 0))
			{
				StartUpdatingHealth();
			}
			activeStatusEffects.Add(effect);
			// After we add the effect we should call whatever subscribers we have.
			if (changeStatusEffectEvent != null)
			{
				changeStatusEffectEvent.Invoke();
			}
			return true;
		}
		/// <summary>
		/// Used to remove a status effect from this component.
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		public bool RemoveStatusEffect(StatusEffect effect)
		{
			bool removed = activeStatusEffects.Remove(effect);

			// We should iterate through the list of statusEffects to check if any existing ones need health updating
			bool keepUpdatingHealth = false;
			foreach (StatusEffect activeEffect in activeStatusEffects)
			{
				if (activeEffect < 0)
				{
					keepUpdatingHealth = true;
				}
			}
			// Do we need to keep updating health?
			if (!keepUpdatingHealth)
			{
				StopUpdatedHealth();
			}

			// After we remove the effect we should call whatever subscribers we have.
			if (changeStatusEffectEvent != null)
			{
				changeStatusEffectEvent.Invoke();
			}
			return removed;
		}
		/// <summary>
		/// Adding negative health means to damage the player instead.
		/// </summary>
		/// <param name="amount"></param>
		public void ModifyHealth(float amount, bool allowOverheal = false)
		{
			// This is a potentially expensive function. Lets bail if we do nothing useful.
			if (amount == 0)
			{
				return;
			}
			// We might do nothing 
			if (changeHealthEvent != null)
			{
				changeHealthEvent.Invoke();
			}

			float totalModifyAmount = amount;
			// If we are loosing health, checking for Invulnerable should be at the end.
			if (amount < 0)
			{
				// Should add additional percentage damage
				if (activeStatusEffects.Contains(StatusEffect.Vulnerable))
				{
					totalModifyAmount += (amount * vulnerablePercentage);
				}
				// Should reduce the damage by a percentage
				if (activeStatusEffects.Contains(StatusEffect.Resilient))
				{
					totalModifyAmount -= (amount * resilientPercentage);
				}
				// If we are Invulernable we can't take damage
				if (activeStatusEffects.Contains(StatusEffect.Invulnerable))
				{
					totalModifyAmount = 0;
				}
			} else
			{
				float amountToIncrease = totalModifyAmount + currentHealth;
				// We want to add health only if we aren't allowing overheal and it won't
				// increase our health over our max health.
				if (!allowOverheal & amountToIncrease <= maxHealth)
				{
					amountToIncrease = maxHealth - currentHealth;
				}
				totalModifyAmount = amountToIncrease;
			}
			// However, if we have Absorption we want to make sure this health amount will be positive.
			if (activeStatusEffects.Contains(StatusEffect.Adsorption))
			{
				totalModifyAmount = Mathf.Abs(amount);
			}
			currentHealth += totalModifyAmount;
			// Basically if we added health and we aren't updating health, lets do that again.
			if (currentHealth > 0 & !updatingHealth)
			{
				StartUpdatingHealth();
			}
			// If we have no health we kinda just want to stop things for a bit.
			if (currentHealth <= 0)
			{
				currentHealth = 0;
				activeStatusEffects.Clear();
				StopUpdatedHealth();
				if (noHealthEvent != null)
				{
					noHealthEvent.Invoke();
				}
			}
		}
		private void StartUpdatingHealth()
		{
			updateHealthRoutine = UpdateHealth();
			StartCoroutine(updateHealthRoutine);
		}
		private void StopUpdatedHealth()
		{
			updatingHealth = false;
			StopCoroutine(updateHealthRoutine);
		}
		private IEnumerator UpdateHealth()
		{
			updatingHealth = true;
			while (true)
			{
				if (activeStatusEffects.Contains(StatusEffect.Bleeding))
				{
					ModifyHealth(-bleedRate);
				}
				if (activeStatusEffects.Contains(StatusEffect.Regeneration))
				{
					ModifyHealth(regenRate);
				}
				// Poison is a fancy one that needs to consider vulnerable in its calculation to not accidentally kill the player
				if (activeStatusEffects.Contains(StatusEffect.Poisoned))
				{
					float totalPoisonAmount = poisonRate;
					if (activeStatusEffects.Contains(StatusEffect.Vulnerable))
					{
						totalPoisonAmount += (poisonRate * vulnerablePercentage);
					}
					// If the poison causes us to dip below 
					if ((currentHealth - totalPoisonAmount) >= 1)
					{
						ModifyHealth(-poisonRate);
					}
				}
				yield return new WaitForSeconds(updateHealthRate);
			}
		}
	}
}