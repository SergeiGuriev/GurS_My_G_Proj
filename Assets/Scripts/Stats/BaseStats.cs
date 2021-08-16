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
        public event Action onLvlUp;

        int currentLvl = 0;

        private void Start()
        {
            currentLvl = CalculateLvl();
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.onExperienceIncrease += UpdateLevel;
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

        //private void Update()
        //{
        //    int newLevel = CalculateLvl();  // каждый кадр должен запускать СНОВА И СНОВА CalculateLvl() метод для того чтоб просто узнать
        //    if (newLevel > currentLvl)      // обновился он или нет
        //    {
        //        currentLvl = newLevel;
        //        print("Lvl Up!");
        //    }
        //}
        public int GetLevel()
        {
            if (currentLvl < 1)
            {
                currentLvl = CalculateLvl();
            }
            return currentLvl;
        }

        public int GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int CalculateLvl()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null)
            {
                return startLvl;    // у врагов нету повышения уровня после убийства ГГ
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
            return penultimateLvl + 1;
        }
    }
}