using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    Player player;

    public Transform characterBody;
    public Transform cameraPosition;

    float mv;
    float camera_dist = 0f;
    float camera_width = -5f;
    float camera_height = 2f;

    Vector3 dir;
    Vector3 offset; //카메라와 캐릭터의 일정거리 유지를 위한 벡터값

    RaycastHit hit;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInChildren<Player>();
        offset = cameraPosition.position - characterBody.position;
        anim = characterBody.GetComponent<Animator>();
        camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);//camera_width * camera_width + camera_height* camera_height
        dir = new Vector3(0, camera_height, camera_width).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.ST != StateType.Die && player.AT == AnimType.Idle)
        {
            LookAround();
            Move();
        }
    }

    private void Move()
    {
        float mv = player.stats.moveSpeed;
        //캐릭터 움직임 및 카메라가 바라보는 방향으로 w 누를 시 그 방향으로 회전 및 이동
        Vector2 moveInput= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        anim.SetBool("isWalk", isMove);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraPosition.forward.x, 0f, cameraPosition.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraPosition.right.x, 0f, cameraPosition.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            characterBody.position += moveDir * Time.deltaTime * player.stats.moveSpeed;
        }
    }

    private void LookAround()
    {
        Vector3 ray_target = characterBody.up * cameraPosition.position.y + cameraPosition.forward * cameraPosition.position.z;
        Physics.Raycast(new Vector3(characterBody.position.x, 2, characterBody.position.z), ray_target, out hit, camera_dist);
        Debug.DrawRay(characterBody.position, ray_target, Color.red);
        if (hit.point != Vector3.zero) //벽과 카메라가 충돌할 때
        {
            if(hit.collider != characterBody)
            {
                cameraPosition.transform.position = hit.point; //오브젝트와 충돌한 위치로 이동
                Debug.Log(hit.rigidbody);
                Debug.DrawRay(characterBody.position, ray_target, Color.green);
            }
        }
        else // 벽과 카메라가 충돌하지 않을 때
        {
            cameraPosition.transform.localPosition = characterBody.position + offset; //카메라 일정 간격 유지
            cameraPosition.Translate(dir * camera_dist);//이 위치로 카메라 이동
        }

        //카메라 회전
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPosition.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if(x < 180f) // 윗 방향으로 75도 제한
        {
            x = Mathf.Clamp(x, -1f, 75f);
        }
        else//아래 방향으로 -45도 제한
        {
            x = Mathf.Clamp(x, 315f, 361f);
        }

        cameraPosition.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
