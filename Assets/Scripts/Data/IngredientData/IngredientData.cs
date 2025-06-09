using System;
using System.Collections.Generic;

public class IngredientData
{
    public string id;
    public string Description;
    public int Damage;

    public EIngredientName IngredientName =>
        Enum.TryParse(id, out EIngredientName result) ? result : throw new Exception($"Unknown id: {id}");
}

public class IngredientDataList
{
    public List<IngredientData> ingredients;
}