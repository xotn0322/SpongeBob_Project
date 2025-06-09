using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections; // For Coroutines

public class IngredientGenerator : MonoBehaviour
{
    // 유니티 에디터에서 할당할 재료 프리팹의 이름
    public EIngredientName ingredientName; 
    private GameObject currentIngredient;
    private XRGrabInteractable grabInteractable;
    private Rigidbody ingredientRigidbody; // 재료의 Rigidbody
    private Transform controllerTransform; // 컨트롤러의 Transform을 저장

    // 컨트롤러가 트리거 영역에 들어왔을 때 호출됩니다.
    void OnTriggerEnter(Collider other)
    {
        // 'XRDirectInteractor' 컴포넌트를 사용하여 컨트롤러를 식별합니다.
        if (other.GetComponent<XRDirectInteractor>() != null)
        {
            controllerTransform = other.transform; // 컨트롤러의 Transform 저장

            // 현재 생성된 재료가 없을 때만 새로운 재료를 생성합니다.
            if (currentIngredient == null) 
            {
                // IngredientManager를 통해 재료를 생성합니다.
                currentIngredient = IngredientManager.Instance.SpawnPrefab(ingredientName, controllerTransform.position);
                // 생성된 재료에서 XRGrabInteractable 컴포넌트와 Rigidbody 컴포넌트를 가져옵니다.
                grabInteractable = currentIngredient.GetComponent<XRGrabInteractable>();
                ingredientRigidbody = currentIngredient.GetComponent<Rigidbody>();

                if (grabInteractable == null)
                {
                    Debug.LogWarning("Ingredient prefab does not have an XRGrabInteractable component! Please add one.");
                }
                if (ingredientRigidbody == null)
                {
                    Debug.LogWarning("Ingredient prefab does not have a Rigidbody component! Please add one.");
                }

                if (grabInteractable != null)
                {
                    // selectExited 이벤트에 구독하여 재료가 해제될 때 Rigidbody kinematic 상태를 처리합니다.
                    grabInteractable.selectExited.AddListener(OnIngredientReleased);
                    // 재료가 잡히기 전까지 컨트롤러를 따라다니도록 코루틴을 시작합니다.
                    StartCoroutine(FollowController());
                }
            }
        }
    }

    // 컨트롤러가 트리거 영역에서 나갔을 때 호출됩니다.
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() != null) // 나가는 오브젝트가 컨트롤러인지 확인
        {
            if (currentIngredient != null)
            {
                // 현재 실행 중인 모든 코루틴 (FollowController 포함)을 중지합니다.
                StopAllCoroutines();

                // 재료가 현재 선택/잡혔는지 확인합니다.
                if (grabInteractable != null && grabInteractable.isSelected) 
                {
                    // 재료가 잡혔다면, OnTriggerExit에서 특별히 할 일은 없습니다.
                    // XRGrabInteractable이 해제될 때 OnIngredientReleased가 호출되어 처리됩니다.
                    Debug.Log("Ingredient grabbed, not destroying on OnTriggerExit, letting XRGrabInteractable handle release.");
                }
                else
                {
                    // 잡히지 않았다면 재료를 파괴합니다.
                    Destroy(currentIngredient);
                    currentIngredient = null; 
                    grabInteractable = null; 
                    ingredientRigidbody = null; 
                    controllerTransform = null; 
                    Debug.Log("Ingredient not grabbed, destroying.");
                }
            }
        }
    }

    // 재료가 잡히기 전까지 컨트롤러를 따라다니게 하는 코루틴입니다.
    private IEnumerator FollowController()
    {
        // 재료가 컨트롤러에 자식으로 설정되어 컨트롤러를 따라다니게 합니다.
        currentIngredient.transform.parent = controllerTransform;
        // 재료의 Rigidbody를 kinematic으로 설정하여 물리적인 영향을 받지 않도록 합니다.
        if (ingredientRigidbody != null)
        {
            ingredientRigidbody.isKinematic = true;
        }

        // 재료가 존재하고, 아직 잡히지 않았다면 계속 반복합니다.
        while (currentIngredient != null && (grabInteractable == null || !grabInteractable.isSelected))
        {
            yield return null; 
        }

        // 루프가 재료가 잡혔기 때문에 종료되었다면, 재료의 부모를 해제하고 Rigidbody를 non-kinematic으로 설정합니다.
        // 이제 XRGrabInteractable이 재료의 움직임을 관리하도록 합니다.
        if (currentIngredient != null && grabInteractable != null && grabInteractable.isSelected)
        {
            currentIngredient.transform.parent = null; 
            if (ingredientRigidbody != null)
            {
                ingredientRigidbody.isKinematic = false; 
                Debug.Log("Ingredient grabbed, setting Rigidbody to non-kinematic and unparenting for XRGrabInteractable control.");
            }
        }
    }

    // XRGrabInteractable이 해제될 때 호출되는 이벤트 핸들러입니다.
    private void OnIngredientReleased(SelectExitEventArgs args)
    {
        // Rigidbody를 non-kinematic으로 설정하여 던지기 동작이 원활하게 이루어지도록 합니다.
        if (ingredientRigidbody != null)
        {
            ingredientRigidbody.isKinematic = false;
        }
        // 재료 참조를 정리합니다.
        currentIngredient.transform.parent = null;
        currentIngredient = null;
        grabInteractable = null;
        ingredientRigidbody = null;
        controllerTransform = null;
        Debug.Log("Ingredient released, ensuring non-kinematic and cleaning up references.");
    }

    // 스크립트가 비활성화되거나 파괴될 때 이벤트 구독을 해제하여 메모리 누수를 방지합니다.
    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnIngredientReleased);
        }
    }
}
