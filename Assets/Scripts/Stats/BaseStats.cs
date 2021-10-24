using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 100)]
        [SerializeField] int startLvl = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] CharacterProgression progression = null;
        [SerializeField] GameObject levelUpParticles = null;
        [SerializeField] bool haveModifiersUsingPermission = false;

        public event Action onLvlUp;

        Experience experience = null;
        int currentLvl = 0;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        private void Start()
        {
            currentLvl = CalculateLvl();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceIncrease += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceIncrease -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLvl();
            if (newLevel > currentLvl)
            {
                currentLvl = newLevel;
                LevelUpEffect();
                onLvlUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticles, transform);
        }

        public int GetLevel()
        {
            if (currentLvl < 1)
            {
                currentLvl = CalculateLvl();
            }
            return currentLvl;
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!haveModifiersUsingPermission) return 0;
            float res = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    res += modifier;
                }
            }
            return res;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!haveModifiersUsingPermission) return 0;
            float res = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    res += modifier;
                }
            }
            return res;
        }

        public int CalculateLvl()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null)
            {
                return startLvl;
            }
            float currentXp = experience.GetXpVal();
            int penultimateLvl = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLvl; level++)
            {
                int XpToLvlUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XpToLvlUp > currentXp)
                {
                    return level;
                }
            }
            return penultimateLvl;
        }
    }
}