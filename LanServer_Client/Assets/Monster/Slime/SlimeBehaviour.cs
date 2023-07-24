using UnityEngine;

[RequireComponent(typeof(ISlimeController))]
public class SlimeBehaviour : MonoBehaviour
{
    private ISlimeController m_Controller;

    private void Awake()
    {
        m_Controller = GetComponent<ISlimeController>();
    }

    private void Update()
    {
        m_Controller.OnUpdate();
    }

    private void FixedUpdate()
    {
        m_Controller.OnFixedUpdate();
    }
}