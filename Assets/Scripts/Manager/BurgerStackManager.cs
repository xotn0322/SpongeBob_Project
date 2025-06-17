using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class BurgerStackManager : MonoBehaviour
{
    //private
    private List<Transform> ingredients = new List<Transform>();
    private int totalDamage;

    //public
    public Transform baseTransform; // Bottom Bun의 Transform
    public float stackOffsetY = 0.05f;

    //function
    public void AddIngredient(Transform ingredient)
    {
        GameObject stackPoint = new GameObject($"StackPoint_{ingredient.name}");
        stackPoint.transform.SetParent(this.transform);

        Vector3 newStackPointWorldPosition = baseTransform.position + Vector3.up * ((ingredients.Count + 1) * stackOffsetY);
        stackPoint.transform.position = newStackPointWorldPosition;

        // Disable the XRGrabInteractable component first to prevent interference with parenting
        if (ingredient.TryGetComponent<XRGrabInteractable>(out var grabInteractable))
        {
            grabInteractable.enabled = false;
        }

        // Parent the actual ingredient to this newly created stack point
        ingredient.SetParent(stackPoint.transform);
        ingredient.localPosition = Vector3.zero;
        ingredient.localRotation = Quaternion.identity;

        ingredients.Add(ingredient);

        if (ingredient.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Adjust the BoxCollider's Z-position to move with the stack
        if (TryGetComponent<BoxCollider>(out var boxCollider))
        {
            Vector3 currentCenter = boxCollider.center;
            currentCenter.y += ingredient.GetComponent<BoxCollider>().size.y;
            boxCollider.center = currentCenter;
        }

        // If the added ingredient is a Top_Bun, disable the BoxCollider to prevent further OnTriggerEnter calls
        if (ingredient.TryGetComponent<IngredientComponent>(out var ingredientComponent))
        {
            if (ingredientComponent.GetIngredientData()?.IngredientName == EIngredientName.Top_Bun)
            {
                if (TryGetComponent<BoxCollider>(out var burgerManagerCollider))
                {
                    burgerManagerCollider.enabled = false;
                }
            }
        }

        CalculateTotalDamage();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.TryGetComponent(out IngredientComponent ingredient))
        {
            if (ingredient == null || ingredient.GetIngredientData() == null)
            {
                Debug.LogWarning("IngredientComponent 또는 IngredientData가 null입니다.");
                return;
            }

            if (ingredient.GetIngredientData().IngredientName != EIngredientName.Bottom_Bun)
            {
                if (!ingredients.Contains(other.transform))
                {
                    AddIngredient(other.transform);
                }
            }
        }
    }

    private void CalculateTotalDamage()
    {
        foreach (var ingredient in ingredients)
        {
            totalDamage += ingredient.GetComponent<IngredientComponent>().GetIngredientData().Damage;
        }
    }

    public int GetTotalDamage()
    {
        return totalDamage;
    }
}
