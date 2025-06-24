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

    private void OnDestroy()
    {
        // Ingredient가 파괴될 때 IngredientManager에서 해당 Ingredient를 제거합니다.
        if (IngredientManager.Instance != null)
        {
            IngredientManager.Instance.RemoveIngredient(gameObject.GetInstanceID());
        }
    }
}