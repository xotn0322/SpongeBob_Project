using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
// using UnityEngine.XR.Interaction.Toolkit;


public class BurgerStackManager : MonoBehaviour
{
    //private
    private List<Transform> ingredients = new List<Transform>();
    [SerializeField] private int totalDamage;
    // private XRGrabInteractable burgerGrabInteractable;
    // private Rigidbody burgerRigidbody;

    //public
    public Transform baseTransform; // Bottom Bun의 Transform
    public float stackOffsetY = 0.05f;

    // void Start()
    // {
    //     // Bottom_Bun의 XRGrabInteractable과 Rigidbody 가져오기
    //     burgerGrabInteractable = GetComponent<XRGrabInteractable>();
    //     burgerRigidbody = GetComponent<Rigidbody>();
    //     
    //     if (burgerGrabInteractable != null)
    //     {
    //         // 던질 때 호출될 이벤트 추가
    //         burgerGrabInteractable.selectExited.AddListener(OnBurgerThrown);
    //     }
    // }

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

    // // 햄버거가 던져질 때 호출되는 메서드
    // private void OnBurgerThrown(SelectExitEventArgs args)
    // {
    //     Debug.Log("Burger thrown! Activating all ingredient physics.");
    //     
    //     // 모든 재료의 Rigidbody를 활성화하여 물리 영향을 받도록 함
    //     foreach (var ingredient in ingredients)
    //     {
    //         if (ingredient != null && ingredient.TryGetComponent<Rigidbody>(out var rb))
    //         {
    //             rb.isKinematic = false;
    //             rb.useGravity = true;
    //             
    //             // Bottom_Bun의 속도를 모든 재료에 적용
    //             if (burgerRigidbody != null)
    //             {
    //                 rb.velocity = burgerRigidbody.velocity;
    //                 rb.angularVelocity = burgerRigidbody.angularVelocity;
    //             }
    //         }
    //     }
    // }

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
        totalDamage = GetComponent<IngredientComponent>().GetIngredientData().Damage; // Bottom_Bun으로 초기화
        foreach (var ingredient in ingredients)
        {
            if (ingredient.TryGetComponent<IngredientComponent>(out IngredientComponent ingredientComponent))
            {
                totalDamage += ingredientComponent.GetIngredientData().Damage;
            }
        }
        
        Debug.Log($"Total Damage: {totalDamage}"); // 디버깅을 위한 로그 추가
    }

    public int GetTotalDamage()
    {
        return totalDamage;
    }

    void OnDisable()
    {
        // 이벤트 리스너 정리
        // if (burgerGrabInteractable != null)
        // {
        //     burgerGrabInteractable.selectExited.RemoveListener(OnBurgerThrown);
        // }
    }
}
