using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Touch : MonoBehaviour
{
    //화면크기 받아오기
    int screen_size_width;
    int screen_size_height;
    int screen_size_width_ratio;
    int max_;
    int min_;
    int temp_;
    public bool check_screen_size;

    //타이틀
    GameObject title_touch;
    GameObject title_back;
    GameObject title_main;
    GameObject title_string;
    bool title_set = false;
    float title_alpha = 1.0f;

    //사운드
    GameObject menu_down_sound;
    GameObject title_touch_sound;
    //라이트
    GameObject light_left;
    GameObject light_right;
    //가구 UI
    GameObject placement_ui;
    GameObject furniture_buy_obj;
    //가구 UI 위에 오브젝트들
    GameObject[] placement_object = new GameObject[4];
    GameObject[] place_menu_page_lock = new GameObject[4];
    //카메라
    Camera  main_cam;
    //밑부분 원형 UI
    GameObject rotate_menu;
    GameObject rotate_halr_circle;
    //상태바
    GameObject all_state_bar;
    //마당
    GameObject obejct_greed;
    //게임 데이터
    GameObject gamedata_;

    //메뉴 UI
    //메뉴 하위 아이콘
    GameObject  menu_static;        //메뉴00 - 메뉴버튼
    GameObject  menu_01;            //메뉴01 - 상점
    GameObject  menu_02;            //메뉴02 - 잠김
    GameObject  menu_03;            //메뉴03 - 잠김
    //아래 원메뉴 터치구역
    GameObject touch_place;

    public int menu_state = 0;             //메뉴 상태 (내리기, 올리기)
    float scale_value = 0.0f;       //메뉴가 커지고, 작아지는 속도
    float max_scale = 1.5f;     //메뉴 최대 크기
    float next_scale = 0.6f;    //다음 메뉴 아이콘 커지는 시기

    //UI 터치할때 가구 안움직이게 하기 위함
    public bool ui_bool = false;


    //mode_ == 0 : 관찰모드(좌우 회전가능), mode_ == 1 : 가구모드, mode_ == 2 : 관찰모드로 돌아오기(좌우 회전불가)
    public int mode_ = 0;
    public bool warning_img_check = false;

    float move_cam_speed = 2.5f;

    public bool menu_touch_check = false;

    //rot_menu
    public bool check_touch_down = false;   //터치할때 카메라 안움직이게 하기 위함    

    //배치모드일때 천장
    GameObject top_block;
    //배치모드일때 벽
    GameObject left_block;
    GameObject right_block;
    GameObject back_block;
    float fall_down_block = 2.425f;

    //배치모드 첫 가구배치용
    UI_Touch_Object_Place about_first_menu_out;

    //디노
    GameObject Dino_;

    //디노 머니 위치 수정용
    GameObject dino_total_money;
    public float dino_money_x_original;
    public float dino_money_y_original;
    public float dino_money_y;

    //디노 샤워
    Vector3 dino_shower_before_pos;    
    GameObject boom_effect;
    GameObject dino_shower;
    GameObject dino_shower_touch;
    GameObject shower_back_home;
    GameObject menu_pivot;
    GameObject circle_menu;

    //사운드
    AudioSource click_sound;
    AudioSource shower_sound;

    private void Awake()
    {
        //게임 데이터
        gamedata_ = GameObject.Find("Game_Data");

        placement_ui = GameObject.Find("Canvas_cam_overlay");

        place_menu_page_lock[0] = GameObject.Find("place_menu_page1_lock");
        place_menu_page_lock[1] = GameObject.Find("place_menu_page2_lock");
        place_menu_page_lock[2] = GameObject.Find("splace_menu_page1_lock");
        place_menu_page_lock[3] = GameObject.Find("tplace_menu_page1_lock");

        //크기 보정용
        placement_object[0] = GameObject.Find("place_object");
        placement_object[1] = GameObject.Find("place_object2");
        placement_object[2] = GameObject.Find("splace_object");
        placement_object[3] = GameObject.Find("tplace_object");

        furniture_buy_obj = GameObject.Find("furniture_buy");

        //스크린 사이즈 받아와서 가구 배치 메뉴 조정
        //screen size set
        screen_size_width = Screen.width;
        screen_size_height = Screen.height;

        if (screen_size_width < screen_size_height)
        {
            max_ = screen_size_height;
            min_ = screen_size_width;
        }
        else if (screen_size_width > screen_size_height)
        {
            min_ = screen_size_height;
            max_ = screen_size_width;
        }
        while (max_ % min_ != 0)
        {
            temp_ = max_ % min_;
            max_ = min_;
            min_ = temp_;
        }
        screen_size_width_ratio = screen_size_width / min_;

        //노트8 사이즈 18.5 : 9
        if (screen_size_width_ratio == 18)
        {
            check_screen_size = true;
            gamedata_.GetComponent<Game_Data_>().screen_size_check = true;
            placement_ui.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            for (int i = 0; i < placement_ui.transform.childCount; i++)
            {
                placement_ui.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(placement_ui.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x,
                    placement_ui.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y - 20);
            }
            //잠구는건 z축이 위로 올라와서 오브젝트를 가리기 때문에
            //anchoredPostion이 아닌 anchoredPostion3D를 사용하여 z축을 변경함
            for (int i = 0; i < 4; i++)
            {
                place_menu_page_lock[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 18.7f, -201f);
                placement_object[i].transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);
            }
            //구매버튼은 따로 위치 잡아줘야함
            furniture_buy_obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -0.5f, -211);
        }

    }
    void Start()
    {
        //사운드
        menu_down_sound = GameObject.Find("menu_back_sound");
        title_touch_sound = GameObject.Find("title_start");
        
        about_first_menu_out = GameObject.Find("Canvas_cam_overlay").GetComponent<UI_Touch_Object_Place>();

        //title
        title_touch = GameObject.Find("title_touch");
        title_back = GameObject.Find("title_back");
        title_main = GameObject.Find("title_main");
        title_string = GameObject.Find("title_string");

        if (PlayerPrefs.HasKey("mini_game_check"))
        {
            title_touch.SetActive(false);
            title_back.SetActive(false);
            title_main.SetActive(false);
            title_string.SetActive(false);            
        }
        else if (!PlayerPrefs.HasKey("mini_game_check"))
        {
            title_touch.SetActive(true);
            title_back.SetActive(true);
            title_main.SetActive(true);
            title_string.SetActive(true);            
        }

        light_left = GameObject.Find("Directional_Light_left");
        light_right = GameObject.Find("Directional_Light_right");

        rotate_menu = GameObject.Find("menu_pivot");
        rotate_halr_circle = GameObject.Find("main_half_circle");

        all_state_bar = GameObject.Find("State_Bar");

        obejct_greed = GameObject.Find("Object_greed");
        
        placement_ui = GameObject.Find("Canvas_cam_overlay");
        furniture_buy_obj = GameObject.Find("furniture_buy");

        //크기 보정용
        placement_object[0] = GameObject.Find("place_object");
        placement_object[1] = GameObject.Find("place_object2");
        placement_object[2] = GameObject.Find("splace_object");
        placement_object[3] = GameObject.Find("tplace_object");

        place_menu_page_lock[0] = GameObject.Find("place_menu_page1_lock");
        place_menu_page_lock[1] = GameObject.Find("place_menu_page2_lock");
        place_menu_page_lock[2] = GameObject.Find("splace_menu_page1_lock");
        place_menu_page_lock[3] = GameObject.Find("tplace_menu_page1_lock");


        main_cam = Camera.main;       

        menu_static = GameObject.Find("menu_back");
        menu_01 = GameObject.Find("menu_01");
        menu_02 = GameObject.Find("menu_02");
        menu_03 = GameObject.Find("menu_03");
        touch_place = GameObject.Find("touch_place");

        top_block = GameObject.Find("top_side");
        left_block = GameObject.Find("left_side");
        right_block = GameObject.Find("right_side");
        back_block = GameObject.Find("back_side");

        Dino_ = GameObject.Find("Dino");

        menu_01.transform.localScale = new Vector3(0, 0, 0);
        menu_02.transform.localScale = new Vector3(0, 0, 0);
        menu_03.transform.localScale = new Vector3(0, 0, 0);

        //사운드 초기화
        menu_static.GetComponent<AudioSource>().Stop();

        //게임 데이터
        gamedata_ = GameObject.Find("Game_Data");

        //디노 머니
        dino_total_money = GameObject.Find("dino_money_show");
        dino_money_x_original = dino_total_money.GetComponent<RectTransform>().anchoredPosition.x;
        dino_money_y_original = dino_total_money.GetComponent<RectTransform>().anchoredPosition.y;        
        dino_money_y = dino_total_money.GetComponent<RectTransform>().anchoredPosition.y + 250;

        
        //안쪽에 넣지 않은 이유 - 스크린 비율이 어찌됬든 이건 비활성화 시켜야 하기 때문
        //UI_Touch_Object_Place 에서 초기화 하지 않은 이유 - 오브젝트를 비활성화 시키기전에 스크린 사이즈를 확인하고 비활성화 시키기 위함
        for(int i=0; i<4; i++)
            place_menu_page_lock[i].SetActive(false);
        //하위 자식들 찾아서 변경하고 꺼야하기 때문
        placement_ui.SetActive(false);

        //디노 샤워
        boom_effect = GameObject.Find("fade_effect_back_home");        
        dino_shower = GameObject.Find("Shower_Things");
        dino_shower.SetActive(false);
        dino_shower_touch = GameObject.Find("dino_shower_touch_place");
        dino_shower_touch.SetActive(false);
        shower_back_home = GameObject.Find("shower_back_home");
        shower_back_home.SetActive(false);
        menu_pivot = GameObject.Find("menu_bar_all");
        circle_menu = GameObject.Find("circle_menu");

        click_sound = GameObject.Find("click_sound").GetComponent<AudioSource>();
        shower_sound = GameObject.Find("go_to_shower").GetComponent<AudioSource>();
    }
        
    void Update()
    {   
        //title
        if (title_set)
        {
            if (title_alpha <= 0)
            {
                title_set = false;
                title_touch.SetActive(false);
                title_back.SetActive(false);
                title_main.SetActive(false); 
                title_string.SetActive(false);
                //초반에 연속으로 터치하면 화면회전 안되는거 버그수정
                ui_bool = false;
            }
            else
                title_alpha -= 0.02f;

            title_back.GetComponent<Image>().color = new Color(title_back.GetComponent<Image>().color.r, title_back.GetComponent<Image>().color.g, title_back.GetComponent<Image>().color.b, title_alpha);
            title_main.GetComponent<Image>().color = new Color(title_main.GetComponent<Image>().color.r, title_main.GetComponent<Image>().color.g, title_main.GetComponent<Image>().color.b, title_alpha);
            title_string.GetComponent<Image>().color = new Color(title_string.GetComponent<Image>().color.r, title_string.GetComponent<Image>().color.g, title_string.GetComponent<Image>().color.b, title_alpha);            
        }

        //모드 구분
        if (mode_ == 1)
        {
            cam_pos_station();
        }
        if (mode_ == 2)
        {
            cam_pos_looking();
        }
        if(mode_ == 3)
        {
            cam_pos_making();
        }
        if(mode_ == 4)
        {
            cam_pos_eat_food();
        }
        if(mode_ == 5)
        {            
            cam_pos_shower();
        }

        //상단 메뉴
        if (menu_state == 1)
            menu_coming_down();
        if (menu_state == 2)
            menu_coming_up();        
    }

    //모드변경용
    public void mode_station()
    {
        //모드 변경하면 메뉴 올려주기 위함
        menu_state = 2;

        //모드 변경하는 부분        
        if (mode_ == 0)
        {
            main_cam.transform.position = new Vector3(0.0f, main_cam.transform.position.y, main_cam.transform.position.z);
            mode_ = 1;
            //스크린 비율 확인해서 오브젝트 위치 새롭게 하기 위함
            if (check_screen_size)
            {
                Debug.Log("note8");
                about_first_menu_out.down_object_pos_z = -0.3f;
            }
            else
            {
                Debug.Log("not note8");
                about_first_menu_out.down_object_pos_z = 0f;
            }
        }
        else if (mode_ == 1)
        {            
            warning_img_check = true;
            about_first_menu_out.first_menu_out = false;            
        }
    }

    //가구배치 모드
    void cam_pos_station()
    {
        //y값 위로 200만큼 올려주기 위함
        dino_total_money.GetComponent<RectTransform>().anchoredPosition = new Vector3(dino_money_x_original, dino_money_y, 0);

        rotate_menu.SetActive(false);
        rotate_halr_circle.SetActive(false);
        obejct_greed.SetActive(false);
        all_state_bar.SetActive(false);
        top_block.SetActive(false);
        touch_place.SetActive(false);

        //light
        light_left.SetActive(false);
        light_right.SetActive(false);

        //좌,우,뒤 벽 올라가고 내려가게 하는 효과
        if (back_block.transform.position.y > -2)
            fall_down_block -= 1.5f * Time.deltaTime;
        else if (back_block.transform.position.y <= -2)
            fall_down_block = -2;
        left_block.transform.position = new Vector3(left_block.transform.position.x, fall_down_block, left_block.transform.position.z);
        right_block.transform.position = new Vector3(right_block.transform.position.x, fall_down_block, right_block.transform.position.z);
        back_block.transform.position = new Vector3(back_block.transform.position.x, fall_down_block, back_block.transform.position.z);

        //모드변경하고나서 확실한 값으로 변경하기 위함
        if (main_cam.transform.rotation.eulerAngles.x > 89.9f)
        {
            mode_ = 1;
            main_cam.transform.position = new Vector3(0, 8.5f, -1.5f);
            main_cam.transform.rotation = Quaternion.Euler(90, 0, 0);            
        }
        if(main_cam.transform.rotation.eulerAngles.x > 89.5f)
            placement_ui.SetActive(true);

        //메인 카메라 이동
        main_cam.transform.position = Vector3.Lerp(main_cam.transform.position, new Vector3(0, 8.5f, -1.5f), move_cam_speed * Time.deltaTime);
        main_cam.transform.rotation = Quaternion.Lerp(main_cam.transform.rotation, Quaternion.Euler(90, 0, 0), move_cam_speed * Time.deltaTime);   

        main_cam.fieldOfView = 70.0f;
    }

    //메인 모드
    void cam_pos_looking()
    {
        //디노 머니 위치 초기화
        dino_total_money.GetComponent<RectTransform>().anchoredPosition = new Vector3(dino_money_x_original, dino_money_y_original, 0);

        for (int i = 0; i < 4; i++)
            placement_object[i].transform.position = new Vector3(10, 0, 0);
        
        rotate_menu.SetActive(true);
        rotate_halr_circle.SetActive(true);
        obejct_greed.SetActive(true);
        all_state_bar.SetActive(true);
        touch_place.SetActive(true);

        light_left.SetActive(true);
        light_right.SetActive(true);

        if (back_block.transform.position.y < 2.425f)
            fall_down_block += 1.5f * Time.deltaTime;
        else if (back_block.transform.position.y >= 2.425f)
            fall_down_block = 2.425f;
        left_block.transform.position = new Vector3(left_block.transform.position.x, fall_down_block, left_block.transform.position.z);
        right_block.transform.position = new Vector3(right_block.transform.position.x, fall_down_block, right_block.transform.position.z);
        back_block.transform.position = new Vector3(back_block.transform.position.x, fall_down_block, back_block.transform.position.z);
        //모드변경하고나서 확실한 값으로 변경하기 위함
        if (main_cam.transform.rotation.eulerAngles.x < 0.1f)
        {
            mode_ = 0;
            back_block.transform.position = new Vector3(0, 2.425f, 2.66f);
            left_block.transform.position = new Vector3(-2.66f, 2.425f, 0);
            right_block.transform.position = new Vector3(2.66f, 2.425f, 0);
            main_cam.transform.position = new Vector3(0, 1.0f, -4.5f);
            main_cam.transform.rotation = Quaternion.Euler(0, 0, 0);
            top_block.SetActive(true);
        }

        //메인 카메라 이동
        placement_ui.SetActive(false);
        main_cam.transform.position = Vector3.Lerp(main_cam.transform.position, new Vector3(0, 1.0f, -4.5f), move_cam_speed * Time.deltaTime);
        main_cam.transform.rotation = Quaternion.Lerp(main_cam.transform.rotation, Quaternion.Euler(0, 0, 0), move_cam_speed * Time.deltaTime);
                
        main_cam.fieldOfView = 60.0f;
    }

    //커스터 마이징 모드
    void cam_pos_making()
    {
        main_cam.transform.position = new Vector3(Dino_.transform.position.x, Dino_.transform.position.y + 0.8f, Dino_.transform.position.z - 3.5f);
        main_cam.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    //먹이 모드
    void cam_pos_eat_food()
    {
        main_cam.transform.position = new Vector3(Dino_.transform.position.x, Dino_.transform.position.y + 1.5f, Dino_.transform.position.z - 3.8f);
        main_cam.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    
    //샤워 모드
    void cam_pos_shower()
    {
        main_cam.fieldOfView = 60.0f;

        main_cam.transform.position = new Vector3(-3.75f, 1.4f, 3.6f);
        main_cam.transform.eulerAngles = new Vector3(0f, 0f, 0f);

        Dino_.transform.position = new Vector3(-3.8f, 0.33f, 8.76f);
    }

    //샤워모드 진입
    public void Set_Shower_Mode()
    {
        shower_sound.Play();

        //다시 돌아가기용
        dino_shower_before_pos = Dino_.transform.position;

        dino_shower_touch.SetActive(true);
        dino_shower.SetActive(true);
        shower_back_home.SetActive(true);
        mode_ = 5;

        //다른 메뉴 넘어가지 못하게
        menu_pivot.SetActive(false);
        circle_menu.SetActive(false);
    }

    //샤워모드 나오기
    public void Shower_Back()
    {
        //디노 원래 위치로
        Dino_.transform.position = dino_shower_before_pos;
        //이펙트 위치        
        boom_effect.GetComponent<ParticleSystem>().Play();

        dino_shower_touch.SetActive(false);
        dino_shower.SetActive(false);
        shower_back_home.SetActive(false);

        menu_pivot.SetActive(true);
        circle_menu.SetActive(true);
        mode_ = 2;
    }

    //메뉴 올리기, 내리기
    public void menu_come_down()
    {
        //UI 터치 했을때 반응하는 부분 (내리기, 올리기 변경)
        //menu_state == 1 : 메뉴 내리기, menu_state == 2 : 메뉴 올리기
        if (menu_state == 0)
            menu_static.GetComponent<AudioSource>().Play();
        if (menu_state > 1)
        {
            menu_state = 0;
            menu_static.GetComponent<AudioSource>().Play();
        }

        if (menu_state == 1)
        {
            menu_state = 2;
            menu_down_sound.GetComponent<AudioSource>().Play();
        }
        else
            menu_state++;
    }

    //메뉴 내리기
    void menu_coming_down()
    {        
        //메뉴가 순서대로 커지는 부분
        if (menu_01.transform.localScale.x >= max_scale)
        {
            menu_01.transform.localScale = new Vector3(max_scale, max_scale, max_scale);            
        }
        if (menu_02.transform.localScale.x >= max_scale)
        {
            menu_02.transform.localScale = new Vector3(max_scale, max_scale, max_scale);
        }
        if (menu_03.transform.localScale.x >= max_scale)
        {
            scale_value = 0.0f;
            menu_03.transform.localScale = new Vector3(max_scale, max_scale, max_scale);
        }
        else
            scale_value = 0.2f;

        //순서대로 커짐
        if (menu_01.transform.localScale.x < max_scale)
            menu_01.transform.localScale = new Vector3(menu_01.transform.localScale.x + scale_value, menu_01.transform.localScale.y + scale_value, menu_01.transform.localScale.z + scale_value);
        if (menu_01.transform.localScale.x > next_scale && menu_02.transform.localScale.x < max_scale)
            menu_02.transform.localScale = new Vector3(menu_02.transform.localScale.x + scale_value, menu_02.transform.localScale.y + scale_value, menu_02.transform.localScale.z + scale_value);
        if (menu_02.transform.localScale.x > next_scale && menu_03.transform.localScale.x < max_scale)
            menu_03.transform.localScale = new Vector3(menu_03.transform.localScale.x + scale_value, menu_03.transform.localScale.y + scale_value, menu_03.transform.localScale.z + scale_value);
    }

    //메뉴 올리기
    void menu_coming_up()
    {
        //메뉴가 역순으로 작아지는 부분
        if (menu_03.transform.localScale.x <= 0.0f)
        {
            menu_03.transform.localScale = new Vector3(0, 0, 0);
        }
        if (menu_02.transform.localScale.x <= 0.0f)
        {
            menu_02.transform.localScale = new Vector3(0, 0, 0);
        }
        if (menu_01.transform.localScale.x <= 0.0f)
        {
            scale_value = 0.0f;
            menu_01.transform.localScale = new Vector3(0, 0, 0);
        }
        else
            scale_value = 0.2f;

        //순서대로 작아짐
        if (menu_03.transform.localScale.x > 0.0f)
            menu_03.transform.localScale = new Vector3(menu_03.transform.localScale.x - scale_value, menu_03.transform.localScale.y - scale_value, menu_03.transform.localScale.z - scale_value);
        if (menu_03.transform.localScale.x <= next_scale && menu_02.transform.localScale.x > 0.0f)
            menu_02.transform.localScale = new Vector3(menu_02.transform.localScale.x - scale_value, menu_02.transform.localScale.y - scale_value, menu_02.transform.localScale.z - scale_value);
        if (menu_02.transform.localScale.x <= next_scale && menu_01.transform.localScale.x > 0.0f)
            menu_01.transform.localScale = new Vector3(menu_01.transform.localScale.x - scale_value, menu_01.transform.localScale.y - scale_value, menu_01.transform.localScale.z - scale_value);
    }

    //메뉴 터치시 커졌다 작아졌다
    public void OnPointerDown()
    {
        check_touch_down = true;

        menu_touch_check = true;
        menu_static.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    //메뉴 터치시 커졌다 작아졌다
    public void OnPointerUp()
    {
        check_touch_down = false;

        menu_touch_check = false;
        menu_static.transform.localScale = new Vector3(max_scale, max_scale, max_scale);
    }

    //카메라 터치, UI터치 겹치지 않게 구분
    public void BlockMovePlacement_Down()
    {
        ui_bool = true;
        click_sound.Play();
    }

    //카메라 터치, UI터치 겹치지 않게 구분
    public void BlockMovePlacement_Up()
    {
        ui_bool = false;
    }

    //타이틀 터치
    public void TItle_Touch()
    {        
        title_touch_sound.GetComponent<AudioSource>().Play();
        title_set = true;
    }

    //게임 종료
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("mini_game_check");        
        PlayerPrefs.Save();
    }
}