using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        //public delegate void ExperienceIncreaseDelegate();
        //public event ExperienceIncreaseDelegate onExperienceIncrease;
        public event Action onExperienceIncrease;

        public void IncrExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceIncrease();
        }
        public float GetXpVal()
        {
            return experiencePoints;
        }


        public object CaptureState()
        {
            return experiencePoints;
        }              

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
