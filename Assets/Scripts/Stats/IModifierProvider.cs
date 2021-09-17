using System.Collections.Generic;
namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);    // IEnumerable<int> потому что реализации этого метода могут иметь несколько yield return (несколько усилений урона для оружия, бафов для ХП и тд) и foreach'ом можно будет их все перебрать
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}