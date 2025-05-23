using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization; 
using CraftingSim.Model; 


namespace CraftingSim.Model
{
    /// <summary>
    /// Implementation of ICrafter. 
    /// </summary>
    public class Crafter : ICrafter
    {
        private readonly Inventory inventory;
        private readonly List<IRecipe> recipeList;

        public Crafter(Inventory inventory)
        {
            this.inventory = inventory;
            recipeList = new List<IRecipe>();
        }

        /// <summary>
        /// returns a read only list of loaded recipes.
        /// </summary>
        public IEnumerable<IRecipe> RecipeList => recipeList;

        /// <summary>
        /// Loads recipes from the files.
        /// Must parse the name, success rate, required materials and
        /// necessary quantities.
        /// </summary>
        /// <param name="recipeFiles">Array of file paths</param>
        /// 
        /// TODO
        public void LoadRecipesFromFile(string[] recipeFiles)
        {
            for (int i = 0; i < recipeFiles.Length; i++)
            {
                string filePath = recipeFiles[i];

                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');

                            string name = parts[0].Trim();
                            double successRate = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture); // Updated line
                            Dictionary<IMaterial, int> requiredMaterials = new Dictionary<IMaterial, int>();

                            for (int c = 2; c < parts.Length; c++)
                            {
                                string[] materialParts = parts[c].Trim().Split(',');

                                IMaterial material = new Material(materialParts[0].Trim().GetHashCode(), materialParts[0].Trim());
                                int quantity = int.Parse(materialParts[1].Trim());
                                requiredMaterials.Add(material, quantity);
                            }

                            IRecipe recipe = new Recipe(name, requiredMaterials, successRate);

                            recipeList.Add(recipe);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading recipes from file: " + ex.Message);
                }
            }
             

        }

        /// <summary>
        /// Attempts to craft an item from a given recipe. Consumes inventory 
        /// materials and returns the result message.
        /// </summary>
        /// <param name="recipeName">Name of the recipe to craft</param>
        /// <returns>A message indicating success, failure, or error</returns>
        public string CraftItem(string recipeName)
        {
            IRecipe selected = null;

            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Name.Equals(recipeName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    selected = recipeList[i];
                    break;
                }
            }
            
            if (selected == null)
                return "Recipe not found.";

            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
            {
                IMaterial material = required.Key;
                int need = required.Value;
                int have = inventory.GetQuantity(material);

                if (have < need)
                {
                    if (have == 0)
                    {
                        return "Missing material: " + material.Name;
                    }
                    return "Not enough " + material.Name +
                           " (need " + need +
                           ", have " + have + ")";
                }
            }

            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
                if (!inventory.RemoveMaterial(required.Key, required.Value))
                    return "Not enough materials";

            Random rng = new Random();
            if (rng.NextDouble() < selected.SuccessRate)
                return "Crafting '" + selected.Name + "' succeeded!";
            else
                return "Crafting '" + selected.Name + "' failed. Materials lost.";

        }
    }
}