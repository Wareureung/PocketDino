using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotate_Menu_04 : MonoBehaviour
{
    //아이콘들
    GameObject[] menu_icon = new GameObject[4];

    //회전할때 Pivot
    GameObject menu_pivot;

    //터치 위치
    Rotate_Touch rot_place_touch;

    //touch
    //터치할떄 어느방향으로 드래그 했는지 구분하기 위한 변수들
    Vector2 first_touch_pos;
    Vector2 last_touch_pos;

    //touch
    public float touch_value_check = 0.0f;    //터치 나중 위치값에서 처음 위치값 뺸 값 저장용
    public int rot_vector_state = 0;     //0이면 정지, 1이면 좌, 2이면 우      
    float touch_drag_value = 90.0f; //터치 드래그 길이 확인

    //rotate
    [SerializeField] float rotate_value = 0.0f;

    //메뉴 회전속도
    float menu_rot_speed = 170.0f;

    //메뉴 회전 사운드
    float menu_rot_sound = 0;

    void Start()
    {
        menu_icon[0] = GameObject.Find("menu_love"); ;
        menu_icon[1] = GameObject.Find("menu_shower"); ;
        menu_icon[2] = GameObject.Find("menu_eat"); ;
        menu_icon[3] = GameObject.Find("menu_game"); ;

        menu_pivot = GameObject.Find("menu_pivot");

        rot_place_touch = GameObject.Find("touch_place").GetComponent<Rotate_Touch>();
    }

    // Update is called once per frame
    void Update()
    {
        touch_check();
        Set_Icon_Pos();
        Go_Rot();
    }

    //터치 방향 확인
    void touch_check()
    {
        if (Input.touchCount == 1 && rot_place_touch.rotate_menu_touch)
        {            
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                first_touch_pos = touch.position;             
            }
            if (touch.phase == TouchPhase.Ended)
            {
                last_touch_pos = touch.position;

                touch_value_check = last_touch_pos.x - first_touch_pos.x;

                rot_place_touch.rotate_menu_touch = false;
            }
        }
    }

    //좌, 우 확인
    void Set_Icon_Pos()
    {
        if (touch_value_check > touch_drag_value)
        {
            rot_vector_state = 1;            
            menu_rot_speed = 170.0f;
            menu_rot_sound += Time.deltaTime;
        }
        if (touch_value_check < -touch_drag_value)
        {
            rot_vector_state = 2;
            menu_rot_speed = 170.0f;
            menu_rot_sound += Time.deltaTime;
        }
    }

    //회전
    void Go_Rot()
    {
        //사운드
        if (menu_rot_sound >= 0.2f)
        {
            menu_pivot.GetComponent<AudioSource>().Play();
            menu_rot_sound = 0.0f;
        }       

        //우측으로        
        if (rot_vector_state == 1)
        {            
            if (menu_pivot.transform.eulerAngles.z == 0)
                rotate_value = 270;
            if (menu_pivot.transform.eulerAngles.z == 270)
                rotate_value = 180;
            if (menu_pivot.transform.eulerAngles.z == 180)
                rotate_value = 90;
            if (menu_pivot.transform.eulerAngles.z == 90)
                rotate_value = 0;            
            //한바퀴 돌았을때
            //355가 부드럽게 되는 값
            if (rotate_value == 0 && menu_pivot.transform.eulerAngles.z <= 5)
            {
                rot_vector_state = 0;
                touch_value_check = 0.0f;
                menu_rot_speed = 0.0f;
                rotate_value = 0.0f;
                menu_pivot.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            //메뉴 하나에 도달했을때
            if (menu_pivot.transform.eulerAngles.z <= rotate_value && menu_pivot.transform.eulerAngles.z != 0)
            {
                rot_vector_state = 0;
                touch_value_check = 0.0f;
                menu_rot_speed = 0.0f;
                menu_pivot.transform.eulerAngles = new Vector3(0, 0, rotate_value);
                for (int i = 0; i < 4; i++)
                    menu_icon[i].transform.eulerAngles = new Vector3(0, 0, 0);
                menu_pivot.GetComponent<AudioSource>().Stop();
            }
            //돌아가는 부분
            else
            {
                menu_pivot.transform.Rotate(0, 0, -menu_rot_speed * Time.deltaTime);
                for (int i = 0; i < 4; i++)
                    menu_icon[i].transform.Rotate(0, 0, menu_rot_speed * Time.deltaTime);
            }
        }

        //좌측으로
        if (rot_vector_state == 2)
        {            
            if (menu_pivot.transform.eulerAngles.z == 0)
                rotate_value = 90;
            if (menu_pivot.transform.eulerAngles.z == 90)
                rotate_value = 180;
            if (menu_pivot.transform.eulerAngles.z == 180)
                rotate_value = 270;
            if (menu_pivot.transform.eulerAngles.z == 270)
                rotate_value = 360;
            //한바퀴 돌았을때
            //355가 부드럽게 되는 값
            if (rotate_value >= 360 && menu_pivot.transform.eulerAngles.z >= 355)
            {
                rot_vector_state = 0;
                touch_value_check = 0.0f;
                menu_rot_speed = 0.0f;
                rotate_value = 0.0f;
                menu_pivot.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            //메뉴 하나에 도달했을때
            if (menu_pivot.transform.eulerAngles.z >= rotate_value)
            {
                rot_vector_state = 0;
                touch_value_check = 0.0f;
                menu_rot_speed = 0.0f;
                menu_pivot.transform.eulerAngles = new Vector3(0, 0, rotate_value);
                for (int i = 0; i < 4; i++)
                    menu_icon[i].transform.eulerAngles = new Vector3(0, 0, 0);
                menu_pivot.GetComponent<AudioSource>().Stop();
            }
            //돌아가는 부분
            else
            {
                menu_pivot.transform.Rotate(0, 0, menu_rot_speed * Time.deltaTime);
                for (int i = 0; i < 4; i++)
                    menu_icon[i].transform.Rotate(0, 0, -menu_rot_speed * Time.deltaTime);
            }
        }
    }
}
