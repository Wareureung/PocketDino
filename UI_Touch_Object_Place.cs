using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Touch_Object_Place : MonoBehaviour
{
    //스크린 사이즈 확인하고 가구오브젝트 z축 내리기 위한 변수
    public float down_object_pos_z;

    //메뉴 번호 확인
    //*UI_TOUCH
    public int menu_number = 0;

    //메뉴 오브젝트들
    GameObject[] menu_category = new GameObject[3];

    //메뉴 하위 오브젝트들(가구들)
    GameObject[] menu_category1_obj = new GameObject[2];
    GameObject[] menu_category2_obj = new GameObject[2];
    GameObject[] menu_category3_obj = new GameObject[2];

    //메뉴 하위
    GameObject[] menu_category1_page = new GameObject[2];
    GameObject[] menu_category2_page = new GameObject[2];
    GameObject[] menu_category3_page = new GameObject[2];

    //페이지 좌,우 화살표
    GameObject menu_left_arrow;
    GameObject menu_right_arrow;

    //페이지 닷
    GameObject menu01_dot_all;
    GameObject[] menu01_dot_y = new GameObject[2];

    //메뉴 페이지
    //*UI_TOUCH
    public int menu_category1_number = 0;
    public int menu_category2_number = 0;
    public int menu_category3_number = 0;

    //초기화 한번만 하기용
    bool menu_number_return = false;
    public bool first_menu_out = false;
    UI_Touch about_mode;

    //카테고리별 모음
    GameObject[] furniture_menu = new GameObject[3];

    //카테고리 하위 잠금 페이지 1
    GameObject[] place_menu_01_page = new GameObject[2];
    //카테고리 하위 잠금 페이지 2
    GameObject[] place_menu_02_page = new GameObject[2];
    //카테고리 하위 잠금 페이지 3
    GameObject[] place_menu_03_page = new GameObject[2];

    //클릭할때마다 언락 최신화
    State_Bar check_unlock;

    void Start()
    {
        about_mode = GameObject.Find("Canvas").GetComponent<UI_Touch>();
        check_unlock = GameObject.Find("Canvas").GetComponent<State_Bar>();

        //menu dot
        menu01_dot_all = GameObject.Find("menu01_page_dot");
        menu01_dot_y[0] = GameObject.Find("page_number1_y");
        menu01_dot_y[1] = GameObject.Find("page_number2_y");
        menu01_dot_y[1].SetActive(false);

        menu_category[0] = GameObject.Find("store_first");
        menu_category[1] = GameObject.Find("store_second");
        menu_category[2] = GameObject.Find("store_third");

        menu_left_arrow = GameObject.Find("arrow_left");
        menu_right_arrow = GameObject.Find("arrow_right");

        //카테고리
        furniture_menu[0] = GameObject.Find("furniture_menu_01");
        furniture_menu[1] = GameObject.Find("furniture_menu_02");
        furniture_menu[2] = GameObject.Find("furniture_menu_03");

        //카테고리1 가구
        menu_category1_obj[0] = GameObject.Find("place_object");
        menu_category1_obj[1] = GameObject.Find("place_object2");
        //카테고리2 가구
        menu_category2_obj[0] = GameObject.Find("splace_object");
        menu_category2_obj[1] = GameObject.Find("splace_object2");
        //카테고리3 가구
        menu_category3_obj[0] = GameObject.Find("tplace_object");
        menu_category3_obj[1] = GameObject.Find("tplace_object2");

        //카테고리1 페이지
        menu_category1_page[0] = GameObject.Find("page1");
        menu_category1_page[1] = GameObject.Find("page2");
        menu_category1_page[1].SetActive(false);
        //카테고리2 페이지
        menu_category2_page[0] = GameObject.Find("spage1");
        menu_category2_page[1] = GameObject.Find("spage2");
        menu_category2_page[0].SetActive(false);
        menu_category2_page[1].SetActive(false);
        //카테고리3 페이지
        menu_category3_page[0] = GameObject.Find("tpage1");
        menu_category3_page[1] = GameObject.Find("tpage2");
        menu_category3_page[0].SetActive(false);
        menu_category3_page[1].SetActive(false);

        //카테고리1 페이지 잠금
        place_menu_01_page[0] = GameObject.Find("place_menu_page1_lock");
        place_menu_01_page[1] = GameObject.Find("place_menu_page2_lock");
        //place_menu_01_page[1].SetActive(false);
        //카테고리2 페이지 잠금
        place_menu_02_page[0] = GameObject.Find("splace_menu_page1_lock");
        place_menu_02_page[1] = GameObject.Find("splace_menu_page2_lock");
        //place_menu_02_page[0].SetActive(false);
        place_menu_02_page[1].SetActive(false);
        //카테고리2 페이지 잠금
        place_menu_03_page[0] = GameObject.Find("tplace_menu_page1_lock");
        place_menu_03_page[1] = GameObject.Find("tplace_menu_page2_lock");
        //place_menu_03_page[0].SetActive(false);
        place_menu_03_page[1].SetActive(false);
    }

    void Update()
    {
        placement_one_();
    }

    public void menu_number_set(int number)
    {
        menu_number = number;        

        //언락 최신화
        check_unlock.check_mode_just_one = false;

        //메뉴 오브젝트, ui 활/비활성화용
        menu_number_return = true;
        //카테고리
        for (int i = 0; i < 3; i++)
            menu_category[i].SetActive(false);

        menu_category[menu_number].SetActive(true);

        //카테고리 안 모든 메뉴
        for (int i = 0; i < 3; i++)
            furniture_menu[i].SetActive(false);

        furniture_menu[menu_number].SetActive(true);
        return_menu_category();
    }

    void placement_one_()
    {
        //메뉴 상태 돌리면 첫화면 보일수 있도록 다시 false로 만들어줌
        if (about_mode.mode_ == 0 || about_mode.mode_ == 2)
        {
            first_menu_out = false;
            //메뉴 처음부터 보여주기 위함
            menu_category1_number = 0;
            menu_category2_number = 0;
            menu_category3_number = 0;
            menu_number = 0;
            menu_number_set(0);
        }
        //가구배치 메뉴에서 첫가구, ui 나오도록 설정
        if (about_mode.mode_ == 1 && !first_menu_out)
        {
            if (Camera.main.transform.eulerAngles.x >= 89.9f)
            {                              
                menu_category1_obj[0].transform.position = new Vector3(0, 0, down_object_pos_z);
                place_menu_01_page[0].SetActive(true);
                //첫화면에서 모든 가구 잠겨있는거 방지하기용
                menu_arrow_left();
                first_menu_out = true;
            }
        }
    }

    void return_menu_category()
    {
        if (menu_number_return)
        {
            menu_left_arrow.SetActive(true);
            menu_right_arrow.SetActive(true);

            if (menu_number == 0)
            {
                menu01_dot_all.SetActive(true);
                //전부다 끄고 필요한 것만 키기 위함
                for (int i = 0; i < 2; i++)
                {
                    menu_category1_page[i].SetActive(false);                    
                    place_menu_01_page[i].SetActive(false);

                    menu_category2_page[i].SetActive(false);
                    place_menu_02_page[i].SetActive(false);
                    menu_category2_obj[i].transform.position = new Vector3(10, 0, 0);

                    menu_category3_page[i].SetActive(false);
                    place_menu_03_page[i].SetActive(false);
                    menu_category3_obj[i].transform.position = new Vector3(10, 0, 0);
                }
                menu_category1_page[menu_category1_number].SetActive(true);
                menu_category1_obj[menu_category1_number].transform.position = new Vector3(0, 0, down_object_pos_z);
                place_menu_01_page[menu_category1_number].SetActive(true);
            }
            if (menu_number == 1)
            {
                menu01_dot_all.SetActive(false);

                menu_left_arrow.SetActive(false);
                menu_right_arrow.SetActive(false);

                //전부다 끄고 필요한 것만 키기 위함
                for (int i = 0; i < 2; i++)
                {
                    menu_category1_page[i].SetActive(false);
                    place_menu_01_page[i].SetActive(false);
                    menu_category1_obj[i].transform.position = new Vector3(10, 0, 0);

                    menu_category2_page[i].SetActive(false);
                    place_menu_02_page[i].SetActive(false);

                    menu_category3_page[i].SetActive(false);
                    place_menu_03_page[i].SetActive(false);
                    menu_category3_obj[i].transform.position = new Vector3(10, 0, 0);
                }
                menu_category2_page[menu_category2_number].SetActive(true);
                menu_category2_obj[menu_category2_number].transform.position = new Vector3(0, 0, down_object_pos_z);
                place_menu_02_page[menu_category2_number].SetActive(true);
            }
            if (menu_number == 2)
            {
                menu01_dot_all.SetActive(false);

                menu_left_arrow.SetActive(false);
                menu_right_arrow.SetActive(false);

                //전부다 끄고 필요한 것만 키기 위함
                for (int i = 0; i < 2; i++)
                {
                    menu_category1_page[i].SetActive(false);
                    place_menu_01_page[i].SetActive(false);
                    menu_category1_obj[i].transform.position = new Vector3(10, 0, 0);

                    menu_category2_page[i].SetActive(false);
                    place_menu_02_page[i].SetActive(false);
                    menu_category2_obj[i].transform.position = new Vector3(10, 0, 0);

                }
                menu_category3_page[menu_category3_number].SetActive(true);
                menu_category3_obj[menu_category3_number].transform.position = new Vector3(0, 0, down_object_pos_z);
                place_menu_03_page[menu_category3_number].SetActive(true);
            }
        }
        menu_number_return = false;
    }

    public void menu_arrow_left()
    {
        if (menu_number == 0)
        {
            //이전 닷 비활성화
            menu01_dot_y[menu_category1_number].SetActive(false);

            //언락 최신화
            check_unlock.check_mode_just_one = false;

            //이전 오브젝트, ui 비활성화하고
            menu_category1_page[menu_category1_number].SetActive(false);
            place_menu_01_page[menu_category1_number].SetActive(false);
            menu_category1_obj[menu_category1_number].transform.position = new Vector3(10, 0, 0);
            //값을 감소 시키고
            menu_category1_number--;
            //최소수 안넘게 방지하고
            if (menu_category1_number <= 0)
                menu_category1_number = 0;
            //다음꺼 활성화            
            menu01_dot_y[menu_category1_number].SetActive(true);
            menu_category1_page[menu_category1_number].SetActive(true);
            place_menu_01_page[menu_category1_number].SetActive(true);
            menu_category1_obj[menu_category1_number].SetActive(true);
            menu_category1_obj[menu_category1_number].transform.position = new Vector3(0, 0, down_object_pos_z);
        }        
    }

    public void menu_arrow_right()
    {
        if (menu_number == 0)
        {
            //이전 닷 비활성화
            menu01_dot_y[menu_category1_number].SetActive(false);

            //언락 최신화
            check_unlock.check_mode_just_one = false;

            //이전 오브젝트, ui 비활성화하고
            menu_category1_page[menu_category1_number].SetActive(false);
            place_menu_01_page[menu_category1_number].SetActive(false);
            menu_category1_obj[menu_category1_number].transform.position = new Vector3(10, 0, 0);
            //값을 증가 시키고
            menu_category1_number++;
            //최대수 안넘게 방지하고
            if (menu_category1_number >= 1)
                menu_category1_number = 1;
            //다음꺼 활성화
            menu01_dot_y[menu_category1_number].SetActive(true);
            menu_category1_page[menu_category1_number].SetActive(true);
            place_menu_01_page[menu_category1_number].SetActive(true);
            menu_category1_obj[menu_category1_number].SetActive(true);
            menu_category1_obj[menu_category1_number].transform.position = new Vector3(0, 0, down_object_pos_z);
        }        
    }
}
