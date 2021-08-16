using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Progression", order = 0)]
    public class CharacterProgression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;


        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public int[] levels;
        }

        Dictionary<CharacterClass, Dictionary<Stat, int[]>> lookupTable = null;

        public int GetStat(Stat stats, CharacterClass characterClass, int lvl)
        {
            BuildLookup();
            int[] levels = lookupTable[characterClass][stats];
            if (levels.Length < lvl)
            {
                return 0;
            }
            return levels[lvl - 1];
            //foreach (ProgressionCharacterClass progressionClass in characterClasses)
            //{
            //    if (progressionClass.characterClass != characterClass) continue;
            //    foreach (ProgressionStat progressionStat in progressionClass.stats)
            //    {
            //        if (progressionStat.stat != stats) continue;
            //        if (progressionStat.levels.Length < lvl) continue; 
            //        return progressionStat.levels[lvl - 1];
            //    }
            //}
            //return 0;
        }

        public int GetLevels(Stat stats, CharacterClass characterClass)
        {
            BuildLookup();
            int[] levels = lookupTable[characterClass][stats];
            return levels.Length;
        }



        private void BuildLookup()
        {
            // сохранение в словарь всех данных, записанных в файле Core/Progression
            if (lookupTable != null) return;    // СОХРАНИЛ 1 РАЗ В СЛОВАРЬ ВСЕ ЗНАЧЕНИЯ И ПОСЛЕ ЗАПИСИ RETURN. НЕ НУЖНО КАЖДЫЙ РАЗ ДЛЯ ПОЛУЧЕНИЯ ДАННЫХ
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, int[]>>();    // ЗАПУСКАТЬ 2 foreach'а
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, int[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }
    }
}