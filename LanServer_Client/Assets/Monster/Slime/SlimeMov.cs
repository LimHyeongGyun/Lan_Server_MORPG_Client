#pragma warning disable IDE0032 // auto
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public sealed class SlimeMov : MonoBehaviour
{
    [SerializeField] private string jumpStateName;
    private Animator animator;
    private NavMeshAgent agent;
    private bool isJumping;
    public bool IsJumping => isJumping;
    private bool doJump;

    public void Init(Animator animator, NavMeshAgent agent)
    {
        this.animator = animator;
        this.agent = agent;
    }
    private void Awake()
    {
        var animator = GetComponent<Animator>();
        var agent = GetComponent<NavMeshAgent>();
        Init(animator, agent);
    }

    public void AnimEvent(int i)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(jumpStateName))
        {
            switch ((ISlimeController.AnimEventType)i)
            {
                case ISlimeController.AnimEventType.Start:
                    isJumping = true;
                    break;
                case ISlimeController.AnimEventType.End:
                    isJumping = false;
                    animator.SetFloat("Speed", 0f);
                    break;
            }
        }
    }

    private Vector3 pre;
    public void Jump(Vector3 dest)
    {
        agent.destination = dest;
        if (!doJump)
        {
            pre = transform.position;
        }
        if (!isJumping)
        {
            NavMeshPath path = agent.path;
            Vector3[] corners = path.corners;
            if (corners.Length >= 2)
            {
                Vector3 dir = corners[1] - corners[0];
                dir.y = 0;
                dir.Normalize();

                Vector3 dir2 = transform.forward;
                dir2.y = 0;
                dir2.Normalize();

                float dot = Vector3.Dot(dir, dir2); // cos theta

                Vector3 dir10 = transform.position - pre;
                dir10.y = 0;
                dir10.Normalize();

                dest = corners[corners.Length - 1]; // .... 뭔 해결이 안되

                Vector3 dir11 = dest - pre;
                dir11.y = 0;
                dir11.Normalize();

                Vector4 dir12 = dest - transform.position;
                dir12.y = 0;
                dir12.Normalize();

                float dotn = Vector3.Dot(dir10, dir12);
                float dotp = Vector3.Dot(dir10, dir11);

                if (dot > 0.99f)
                {
                    doJump = true;
                    isJumping = true;
                    animator.SetFloat("Speed", 1f);
                }
                else if (doJump && dotn < 0 && dotp > 0)
                //else if (doJump && dot < 0)
                {
                    doJump = false;
                }
            }
        }
    }

    private bool input;
    private Vector3 dest;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                input = true;
                dest = hit.point;
            }
        }
    }
    private void FixedUpdate()
    {
        if (input)
        {
            Jump(dest);
        }
    }

    private void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        agent.nextPosition = transform.position = position;
    }
}