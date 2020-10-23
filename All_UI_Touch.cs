using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class All_UI_Touch : MonoBehaviour
{
    //가구 데이터 저장 함수
    Data_Save_Function DSF;
    //디노 충돌확인용
    Dino_Making dino_crush_check;

    //가구 한번에 한개만
    GameObject furniture_block_img;
    public bool furniture_place_state;

    //RayCast 충돌 오브젝트 이름, 위치, 회전
    public string hit_object_name;
    public Vector3 hit_object_pos;
    public Vector3 hit_object_rot;
    
    //가구 찾기 위함
    string find_all_data;
    string find_all_data2;
    string find_all_data3;
    int all_placement_number = 1;
    int all_child_count = 0;
    public int childcount_ = 0;

    //터치 시간
    public float placement_touch_time = 0.0f;
    //터치 상태
    public bool placement_touch_state = false;

    //ui터치시 가구 오브젝트 변경하는거 막기용
    GameObject before_ui_touch;
    bool ui_touch_check = false;

    //가구배치 마지막 ui
    GameObject select_ui;   //3ui 받아오는 게임오브젝트
    GameObject select_ui_object = null;    //가구 터치하면 오브젝트 정보 저장
    GameObject[] select_before =  new GameObject[6];   //배치 완료전 UI에 있는 가구 모음들
    GameObject select_before2;   //배치 완료전 UI에 있는 가구 모음들
    GameObject select_done_;    //배치 완료된 가구 모드 바꿔도 없어지지 않게 이동

    

    //가구 움직일때 범위 확장용
    GameObject obj_move_support;
    GameObject before_moveing_pos;
    
    //그리드에서 쓰기위함
    public Touch touch;
    public float grid_size = 0.2f;

    UI_Touch menu_ui;
        
    //가구충돌 확인
    public bool select_object_red_state = false;    //&UI_Touch

    //RayCast 길이
    float draw_ray_dis = 10.0f;

    //가구 상태
    public bool select_ui_state = false;    //&UI_Touch

    //가구 움직일때 겹쳐도 원래 움직이던것만 움직이도록
    [SerializeField] bool placement_move_just_one = false;
    [SerializeField] bool now_ui_touching = false;

    //사운드
    GameObject tui_done;

    //이펙트
    GameObject placement_particle;

    void Start()
    {
        DSF = GameObject.Find("Game_Data").GetComponent<Data_Save_Function>();
        dino_crush_check = GameObject.Find("Dino").GetComponent<Dino_Making>();

        obj_move_support = GameObject.Find("for_move_object");
        obj_move_support.SetActive(false);

        select_ui = GameObject.Find("3ui_set");
        select_ui.SetActive(select_ui_state);

        select_before[0] = GameObject.Find("place_object");
        select_before[1] = GameObject.Find("place_object2");
        select_before[2] = GameObject.Find("splace_object");
        select_before[3] = GameObject.Find("splace_object2");
        select_before[4] = GameObject.Find("tplace_object");
        select_before[5] = GameObject.Find("tplace_object2");

        select_done_ = GameObject.Find("place_done_object");

        menu_ui = GameObject.Find("Canvas").GetComponent<UI_Touch>();

        if (DSF.placement_number != 0)
        {
            all_child_count = select_before[0].transform.childCount + select_before[1].transform.childCount 
                + select_before[2].transform.childCount + select_before[3].transform.childCount 
                + select_before[4].transform.childCount + select_before[5].transform.childCount
                + select_done_.transform.childCount;

            //+1하는 이유는 00번이 아니라 01번부터 시작해서 갯수로 따지면 1개가 더 많아야하기 떄문
            for (; all_placement_number < all_child_count + 1; all_placement_number++)
            {
                //모든 가구 이름 확인(카테고리1)
                find_all_data = string.Format("fobject_{0:D2}", all_placement_number);                
                //모든 가구 중에서 배치완료(int값이 1임)된것만 정보 가져옴
                if (PlayerPrefs.GetInt(find_all_data + string.Format("state")) == 1)
                    DSF.GetPosRot(find_all_data);

                find_all_data2 = string.Format("sobject_{0:D2}", all_placement_number);
                if (PlayerPrefs.GetInt(find_all_data2 + string.Format("state")) == 1)
                    DSF.GetPosRot(find_all_data2);

                find_all_data3 = string.Format("tobject_{0:D2}", all_placement_number);
                if (PlayerPrefs.GetInt(find_all_data3 + string.Format("state")) == 1)
                    DSF.GetPosRot(find_all_data3);
            }
        }
        if (PlayerPrefs.HasKey("Dino"))
            DSF.GetPosRot("Dino");

        //사운드
        tui_done = GameObject.Find("done_sound");
        //이펙트
        placement_particle = GameObject.Find("food_buy_boom");
    }

    void Update()
    {
        //가구배치 모드일 때
        if (menu_ui.mode_ == 1)
        {
            //가구 충돌 확인
            Furniture_Crash_Check();
            //가구 터치
            Obejct_Place_Touch();
        }
    }

    void Furniture_Crash_Check()
    {
        //디노 충돌 체크
        if (dino_crush_check.dino_check_crush)
            select_object_red_state = true;
        else if (!dino_crush_check.dino_check_crush)
            select_object_red_state = false;

        //회전할때도 충돌검사 해야함(충돌했을때 색상 변경), 스케일 체크는 ui모드에서 빠간색으로 변하는거 방지위함
        if (select_ui_object != null && select_ui_object.transform.localScale.x > 0.9f)
        {            
            //하위오브젝트를 가진 가구들
            if (select_ui_object.transform.childCount > 0)
            {
                for (int i = 0; i < select_ui_object.transform.childCount; i++)
                {
                    if (select_ui_object.transform.GetChild(i).GetComponent<MeshRenderer>().material.color == Color.red)
                    {
                        select_object_red_state = true;
                    }
                    else
                        select_object_red_state = false;
                }
            }
            //하위오브젝트가 없는 가구들
            else if (select_ui_object.transform.childCount == 0)
            {
                if (select_ui_object.GetComponent<MeshRenderer>().materials[0].color == Color.red)
                {
                    select_object_red_state = true;
                }
                else
                    select_object_red_state = false;
            }
        }
    }

    void Obejct_Place_Touch()
    {
        //ui를 터치할때가 아닐때만 RayCast발사
        if (Input.touchCount == 1 && !now_ui_touching)
        {
            touch = Input.GetTouch(0);

            //RayCast 길이
            draw_ray_dis = 10.0f;

            //RayCast
            RaycastHit hit;
            Ray touchray = Camera.main.ScreenPointToRay(touch.position);
            Physics.Raycast(touchray, out hit);
            Debug.DrawRay(touchray.origin, touchray.direction * draw_ray_dis, Color.red);

            //충돌 오브젝트 확인
            if (hit.collider != null)
            {
                //가구 오브젝트 터치했을때 (가구배치 모드)
                if (menu_ui.mode_ == 1 && !menu_ui.ui_bool)
                {
                    //오브젝트인지 확인하고, 배치가 UI가 아니라 3d에 위치해 있는지 확인하기 위함
                    //그리고 구매상태 확인
                    if (hit.collider.tag == "object" && hit.collider.transform.localScale.x > 0.8f)
                    {
                        if (!placement_move_just_one)
                        {
                            if (ui_touch_check)
                            {
                                //ui 터치했을때 오브젝트 바뀌는것 방지
                                select_ui_object = before_ui_touch;
                                hit_object_name = select_ui_object.gameObject.name; //충돌한 오브젝트 이름 저장
                                ui_touch_check = false;
                            }
                            else if (!ui_touch_check)
                            {
                                select_ui_object = hit.collider.gameObject; //터치한 오브젝트를 빈오브젝트에 저장                                                
                                hit_object_name = hit.collider.gameObject.name; //충돌한 오브젝트 이름 저장
                            }
                            

                            //가구 터치 시간 확인
                            if (!placement_touch_state)
                                placement_touch_time += Time.deltaTime;

                            //일정 터치 시간 이상일 때 가구 움직일 수 있게
                            if (!placement_touch_state && placement_touch_time > 0.5f)
                            {
                                if (furniture_place_state == true)
                                    furniture_place_state = true;
                                if (furniture_place_state == false)
                                {
                                    furniture_place_state = true;
                                    furniture_block_img = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
                                    furniture_block_img.SetActive(true);
                                }

                                placement_touch_state = true;
                                select_ui_object.transform.position = new Vector3(select_ui_object.transform.position.x, 0.1f, select_ui_object.transform.position.z);
                                select_ui_object.GetComponent<Object_Data>().object_moving = true;
                                placement_move_just_one = select_ui_object.GetComponent<Object_Data>().object_moving;
                                placement_touch_time = 0.0f;

                                before_moveing_pos = select_ui_object.transform.parent.gameObject;
                                //가구 무빙 지원 오브젝트 정렬                                
                                obj_move_support.SetActive(true);
                                obj_move_support.transform.position = select_ui_object.transform.position;
                                select_ui_object.transform.parent = obj_move_support.transform;
                                //가구 타입 확인
                                whatami = PlayerPrefs.GetInt(select_ui_object.name);
                            }
                        }
                        else if (placement_move_just_one)
                        {
                            if (touch.phase == TouchPhase.Began)
                            {
                                //3ui 위치 보정용
                                placement_particle.GetComponent<ParticleSystem>().Play();
                                tui_done.GetComponent<AudioSource>().Play();
                            }

                            if (touch.phase == TouchPhase.Moved)
                            {
                                //가구 이동
                                if (placement_touch_state && placement_move_just_one)
                                {
                                    obj_move_support.transform.position = select_ui_object.transform.position;
                                    select_ui_object.transform.position = new Vector3((Mathf.Floor(hit.point.x / grid_size) * grid_size) + 0.1f, 0.1f, Mathf.Floor(hit.point.z / grid_size) * grid_size);
                                }
                                select_ui_state = false;
                                select_ui.SetActive(select_ui_state);
                            }

                            if (touch.phase == TouchPhase.Stationary)
                            {
                                //가구 터치 유지 상태
                                if (placement_touch_state && placement_move_just_one)
                                {
                                    obj_move_support.transform.position = select_ui_object.transform.position;
                                    select_ui_object.transform.position = new Vector3((Mathf.Floor(hit.point.x / grid_size) * grid_size) + 0.1f, 0.1f, Mathf.Floor(hit.point.z / grid_size) * grid_size);
                                }
                                select_ui_state = false;
                                select_ui.SetActive(select_ui_state);
                            }

                            if (touch.phase == TouchPhase.Ended)
                            {
                                tui_done.GetComponent<AudioSource>().Play();

                                placement_touch_time = 0.0f;

                                //터치가 끝나면 y위칙 원래대로
                                select_ui_object.transform.position = new Vector3(select_ui_object.transform.position.x, 0.01f, select_ui_object.transform.position.z);

                                //마지막으로 배치된 가구 위치 데이터 저장용
                                hit_object_pos = select_ui_object.transform.position;
                                hit_object_rot = select_ui_object.transform.eulerAngles;
                                
                                select_ui_state = true;
                                select_ui.SetActive(select_ui_state);

                                //y좌표에 + 하는 값은 오브젝트 위치에서 오른쪽에 위치하게 하기 위함
                                //오브젝트 밑에 3ui를 위치시키는 것이 화면을 안벗어나는 적합한 위치임
                                select_ui.transform.position = new Vector3(Camera.main.WorldToScreenPoint(select_ui_object.transform.position).x, Camera.main.WorldToScreenPoint(select_ui_object.transform.position).y - 250);

                                //내려놓으면 다시 들어야하니
                                placement_touch_state = false;
                                select_ui_object.GetComponent<Object_Data>().object_moving = false;
                                placement_move_just_one = false;

                                //가구 무빙 지원 원상복구                                
                                select_ui_object.transform.parent = before_moveing_pos.transform;                              
                                before_moveing_pos.transform.parent = null;
                                obj_move_support.SetActive(false);
                            }
                        }
                    }

                    //디노 이동
                    if (hit.collider.tag == "ImDino")
                    {
                        if (touch.phase == TouchPhase.Moved)
                        {
                            hit.collider.transform.position = new Vector3(Mathf.Floor(hit.point.x / grid_size) * grid_size, 0.01f, Mathf.Floor(hit.point.z / grid_size) * grid_size);
                        }

                        if (touch.phase == TouchPhase.Stationary)
                        {
                            hit.collider.transform.position = new Vector3(Mathf.Floor(hit.point.x / grid_size) * grid_size, 0.01f, Mathf.Floor(hit.point.z / grid_size) * grid_size);
                        }

                        if (touch.phase == TouchPhase.Ended)
                        {
                            hit_object_pos = hit.collider.transform.position;
                            hit_object_rot = hit.collider.transform.eulerAngles;
                            DSF.SetPosRot("Dino", hit_object_pos, hit.collider.transform.eulerAngles);
                            PlayerPrefs.Save();
                        }
                    }
                }
            }
        }
    }

    //3 UI 중 가구 배치 완료
    public void Place_Object_Done()
    {
        if (!select_object_red_state)
        {
            //터치를 안하면 저장이 안되서 여기서 한번더 저장
            hit_object_rot = select_ui_object.transform.eulerAngles;

            furniture_place_state = false;

            //가구 블락용
            furniture_block_img = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
            furniture_block_img.SetActive(false);

            before_ui_touch = select_ui_object;
            ui_touch_check = true;

            now_ui_touching = false;
            placement_touch_state = false;
            select_ui_state = false;
            select_ui.SetActive(select_ui_state);
            
            //가구 배치 완료시 데이터 저장            
            DSF.SetPosRot(hit_object_name, hit_object_pos, hit_object_rot);            
            PlayerPrefs.Save();

            //가구 터치 비우기
            select_ui_object = null;
        }
    }

    //3 UI 중 가구 배치 취소
    public void Place_Object_Cancel()
    {
        furniture_place_state = false;
        //가구 블락용
        furniture_block_img = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
        furniture_block_img.SetActive(false);

        before_ui_touch = select_ui_object;
        ui_touch_check = true;

        now_ui_touching = false;
        DSF.DelPosRot(hit_object_name);
        DSF.GetFirstPosRot(hit_object_name);
        PlayerPrefs.Save();
        //가구 터치 비우기
        select_ui_object = null;
        select_ui_state = false;
        select_ui.SetActive(select_ui_state);  //ui 제거

        //가구 터치 비우기
        select_ui_object = null;

        placement_touch_state = false;
    }

    //3 UI 중 가구 배치 회전
    public void Place_Object_Rotate()
    {
        before_ui_touch = select_ui_object;
        ui_touch_check = true;

        //오브젝트 하위 차일드 있는지 없는지 확인
        //차일드 있는지, 없는지에 따라 회전 방향이 다르기 때문에 구분해야함
        if (select_ui_object.transform.childCount == 0)
        {
            select_ui_object.transform.Rotate(Vector3.up, 90.0f);
        }
        else if (select_ui_object.transform.childCount > 0)
        {
            select_ui_object.transform.Rotate(Vector3.forward, 90.0f);
        }
        hit_object_rot = select_ui_object.transform.eulerAngles;        
    }

    //중복 터치 방지용
    public void Block_Touch_With_Ui_Object_(bool state)
    {        
        now_ui_touching = state;
    }

    //3 UI 사운드
    public void threeui_sound_Down()
    {
        select_ui.GetComponent<AudioSource>().Play();
    }

    public void threeui_sound_Up()
    {
        select_ui.GetComponent<AudioSource>().Play();
    }
}
