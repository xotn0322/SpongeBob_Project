using UnityEngine;

public class IngredientComponent : MonoBehaviour
{
    //public
    public EIngredientName ingredientName;

    //private
    private IngredientData ingredientData;

    //functions
    public void Init()
    {
        SetIngredientData();
    }

    private void SetIngredientData()
    {
        ingredientData = IngredientDataManager.Instance.GetData(ingredientName);
    }

    public IngredientData GetIngredientData()
    {
        return ingredientData;
    }
}