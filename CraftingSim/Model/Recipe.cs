using System;
using System.Collections.Generic;

namespace CraftingSim.Model
{
    public class Recipe : IRecipe
    {
        
        public string Name { get; }
        
        public IReadOnlyDictionary<IMaterial, int> RequiredMaterials { get; }
        
        public double SuccessRate { get; }

        public Recipe(string name, Dictionary<IMaterial, int> requiredMaterials, double successRate)
        {
            Name = name;
            RequiredMaterials = new Dictionary<IMaterial, int>(requiredMaterials);
            SuccessRate = successRate;
        }

        public int CompareTo(IRecipe other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}
