using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Camera_move : MonoBehaviour
{
    //중심 Pivot
    GameObject camera_rot_pivot;
    //터치모드 확인
    UI_Touch uitouch;

    //벽들
    Renderer left_side_wall;
    Renderer right_side_wall;
    Renderer back_side_wall;

    //회전 속도
    public float   rot_speed;

    //카메라 회전
    public Vector2 touch_deltapos;
    float camera_rot_value = 1.0f;
    public float campitch;
    public float camyaw;

    //멀티 터치
    Touch touch_two;
    Touch touch_one;

    Vector2 before_one_pos;
    Vector2 before_two_pos;

    float bpower_pos;
    float apower_pos;
    float check_dis;
    float zoomspeed = 0.1f; 

    void Start()
    {
        camera_rot_pivot = GameObject.Find("camera_rot_pivot");
        uitouch = GameObject.Find("Canvas").GetComponent<UI_Touch>();

        left_side_wall = GameObject.Find("left_side").GetComponent<Renderer>();
        right_side_wall = GameObject.Find("right_side").GetComponent<Renderer>();
        back_side_wall = GameObject.Find("back_side").GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {   
        //벽 반투명
        HalfWallView();
        //카메라 회전
        if (uitouch.mode_ == 0)
        {
            SingleTouch();
            MoveCam();
        }
        //줌 인아웃
        if (uitouch.mode_ == 0)
            MultiTouch();
    }

    void HalfWallView()
    {
        if (transform.rotation.eulerAngles.y > 36 && transform.rotation.eulerAngles.y < 139)
            left_side_wall.gameObject.SetActive(false);        
        else
            left_side_wall.gameObject.SetActive(true);

        if (transform.rotation.eulerAngles.y > 136 && transform.rotation.eulerAngles.y < 233)
            back_side_wall.gameObject.SetActive(false);
        else
            back_side_wall.gameObject.SetActive(true);

        if (transform.rotation.eulerAngles.y > 213 && transform.rotation.eulerAngles.y < 316)
            right_side_wall.gameObject.SetActive(false);
        else
            right_side_wall.gameObject.SetActive(true);
    }

    void MoveCam()
    {   
        //카메라가 멀어질때
        if(Vector3.Distance(camera_rot_pivot.transform.position, this.transform.position) > 4.7f)
        {
            if (transform.position.z < 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f * Time.deltaTime);
            if (transform.position.z > 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f * Time.deltaTime);
        }
        //카메라가 가까워질때
        if (Vector3.Distance(camera_rot_pivot.transform.position, this.transform.position) < 4.7f)
        {
            if (transform.position.z < 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f * Time.deltaTime);
            if (transform.position.z > 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f * Time.deltaTime);
        }

        //위치값 y가 1밑으로 내려가지 않도록 함(내려가면 땅 뚫음)
        if (transform.position.y < 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);            
        }
        else
        {
            //회전값 z가 움직이지 않도록 고정(움직이면 기울어짐)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            transform.RotateAround(camera_rot_pivot.transform.position, touch_deltapos, rot_speed);
        }
        transform.LookAt(camera_rot_pivot.transform.position);

        //부드럽게 회전하기 위함
        rot_speed -= 0.05f;
        if (rot_speed <= 0.0f)
        {
            rot_speed = 0.0f;
        }
    }

    void SingleTouch()
    {
        //터치모드, uI터치 확인
        if (!uitouch.check_touch_down && !uitouch.ui_bool && uitouch.mode_ == 0)
        {
            if (Input.touchCount == 1)
            {
                Touch touch_one = Input.GetTouch(0);

                if (touch_one.phase == TouchPhase.Moved)
                {
                    if (uitouch.mode_ == 0)
                    {                        
                        rot_speed = 1.7f;

                        touch_deltapos = touch_one.deltaPosition * camera_rot_value;

                        //상하좌우 터치 방향 보정                        
                        touch_deltapos = new Vector2(touch_deltapos.y * -1f, touch_deltapos.x);
                    }
                }
            }
        }
    }    

    void MultiTouch()
    {
        //멀티터치(줌인아웃)
        if (Input.touchCount == 2)
        {
            touch_two = Input.GetTouch(1);
            touch_one = Input.GetTouch(0);

            //터치 차이 확인
            before_one_pos = touch_one.position - touch_one.deltaPosition;
            before_two_pos = touch_two.position - touch_two.deltaPosition;

            //차이들 정규화
            bpower_pos = (before_one_pos - before_two_pos).magnitude;
            apower_pos = (touch_one.position - touch_two.position).magnitude;

            //거리 체크
            check_dis = bpower_pos - apower_pos;

            //거리에 속도 곱한만큼의 속도로 줌 인/아웃
            Camera.main.fieldOfView += check_dis * zoomspeed;

            //Min 45 ~ Max 100
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 45.0f, 100.0f);
        }
    }
}
