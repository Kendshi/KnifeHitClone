using UnityEngine;

public class KnifeFly : MonoBehaviour
{//Когда происходит победа, ножи в бревне уничтожаются, а на их месте создаются анимированные, устремляющиеся в полет
    [SerializeField] private GameObject animationKnifePrefab;

    private void Start()
    {
        GameManager.manager.WinLevel += GoToFly;
    }

    private void GoToFly()
    {
        Instantiate(animationKnifePrefab, gameObject.transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z));
        GameManager.manager.WinLevel -= GoToFly;
    }
}
