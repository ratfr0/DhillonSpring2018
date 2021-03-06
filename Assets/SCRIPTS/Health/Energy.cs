﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour, Initializable {

	private static Energy m_current;
	/// <summary>
	/// Return the Energy of the active scene.
	/// </summary>
	public static Energy current {
		get { return m_current; }
	}

	void OnDestroy () {
		m_current = null;
	}

	[SerializeField]
	private float m_maxEnergy = 100f;
	/// <summary>
	/// Dhillon's energy caps out at this value.
	/// </summary>
	public float maxEnergy {
		get { return m_maxEnergy; }
	}
	/// <summary>
	/// How much energy does Dhillon start the level with?
	/// </summary>
	[Range (0f, 1f)]
	[SerializeField]
	private float startingEnergyRatio;

	[SerializeField]
	private UIBar barToUpdate;

	private float m_currentEnergy;
	/// <summary>
	/// Current energy value. Assignment auto clamps.
	/// </summary>
	private float currentEnergy {
		get {
			return m_currentEnergy;
		}
		set {
			m_currentEnergy = Mathf.Clamp (value, 0f, maxEnergy);
		}
	}

	public float energyRatio {
		get {
			return currentEnergy / maxEnergy;
		}
		set {
			currentEnergy = maxEnergy * value;
		}
	}

	public void Initialize () {
		m_currentEnergy = maxEnergy * startingEnergyRatio;
		m_current = this;
	}

	void Update () {
		if (barToUpdate != null) {
			barToUpdate.ratio = energyRatio;
		}
	}

	public void IncreaseEnergyDiet (float increaseAmount) {
		IncreaseEnergy (increaseAmount, HealthCategory.Diet);
	}
	public void IncreaseEnergySocial (float increaseAmount) {
		IncreaseEnergy (increaseAmount, HealthCategory.Social);
	}
	public void IncreaseEnergyExercise (float increaseAmount) {
		IncreaseEnergy (increaseAmount, HealthCategory.Exercise);
	}
	public void IncreaseEnergyUncategorized (float increaseAmount) {
		IncreaseEnergy (increaseAmount, HealthCategory.Uncategorized);
	}
	public void IncreaseEnergySilently (float increaseAmount) {
		IncreaseEnergy (increaseAmount, HealthCategory.Silent);
	}

	/// <summary>
	/// Increase Dhillon's energy by this much. Use a negative number to decrease. Use the null health category to suppress the popup message.
	/// </summary>
	public void IncreaseEnergy (float increaseAmount, HealthCategory healthCategory) {
		currentEnergy += increaseAmount;
		if (healthCategory != HealthCategory.Silent) {
			SoundPlayer.PlayOneShot (increaseAmount > 0f ? ImportantAssets.soundLibrary.jump1 : ImportantAssets.soundLibrary.oof);
			string message = increaseAmount < 0f ? "-" : "+";
			message += Mathf.Abs (increaseAmount).ToString ();
			ShortMessageGenerator.current.GenerateShortMessage (message, healthCategory);
			if (healthCategory == HealthCategory.Social) {
				ImportantAssets.miscData.netPsychosocialScoreChange += increaseAmount;
			}
			else if (healthCategory == HealthCategory.Diet) {
				ImportantAssets.miscData.netDietScoreChange += increaseAmount;
			}
			else if (healthCategory == HealthCategory.Exercise) {
				ImportantAssets.miscData.netExertionScoreChange += increaseAmount;
			}
		}
	}
}
