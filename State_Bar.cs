using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class State_Bar : MonoBehaviour
{
    UI_Touch check_mode;
    UI_Touch_Object_Place check_store_number;
    public bool check_mode_just_one = false;

    //머니 표시
    GameObject[] score_one = new GameObject[10];
    GameObject[] score_two = new GameObject[10];
    GameObject[] score_th = new GameObject[10];
    GameObject[] score_fo = new GameObject[10];

    //디노머니
    //전체금액
    int total_money = 0;
    //four - 1000자리, three - 100자리, two - 10자리, one - 1자리
    int four_number;
    int three_number;
    int two_number;
    int one_number;

    //가구 가격
    int my_price;
    //가구 구매상태
    bool am_i_sale;
    //어떤 가구인지 체크
    public String check_furniture;
    //구매 UI
    GameObject furniture_buy_ui;
    GameObject furniture_block;
    //가구 담기용 빈오브젝트
    GameObject empty_obj;
    //사운드
    GameObject tui_done;

    //구매 금액
    GameObject back_dan;
    GameObject ship_dan;
    GameObject li_dan;
    int back_dan_number;
    int ship_dan_number;
    int li_dan_number;

    //구매하면 lock 해제
    GameObject[] page1_menu1 = new GameObject[9];
    GameObject[] page2_menu1 = new GameObject[9];
    int furniture_number;

    //사운드
    AudioSource buy_done;
    AudioSource buy_fail;

    void Start()
    {
        check_mode = GameObject.Find("Canvas").GetComponent<UI_Touch>();        

        for (int i=0; i<10; i++)
        {
            score_one[i] = GameObject.Find("result_numbers_00").transform.GetChild(i).gameObject;
            score_two[i] = GameObject.Find("result_numbers_01").transform.GetChild(i).gameObject;
            score_th[i] = GameObject.Find("result_numbers_02").transform.GetChild(i).gameObject;
            score_fo[i] = GameObject.Find("result_numbers_03").transform.GetChild(i).gameObject;

            score_one[i].SetActive(false);
            score_two[i].SetActive(false);
            score_th[i].SetActive(false);
            score_fo[i].SetActive(false);
        }

        if (!PlayerPrefs.HasKey("dino_money"))
        {
            score_one[0].SetActive(true);
            score_two[0].SetActive(true);
            score_th[0].SetActive(true);
            score_fo[0].SetActive(true);
        }
        if(PlayerPrefs.HasKey("dino_money"))
        {
            total_money = PlayerPrefs.GetInt("dino_money");

            four_number = total_money / 1000;
            three_number = (total_money % 1000) / 100;
            two_number = ((total_money % 1000) % 100) / 10;
            one_number = ((total_money % 1000) % 100) % 10;

            score_one[one_number].SetActive(true);
            score_two[two_number].SetActive(true);
            score_th[three_number].SetActive(true);
            score_fo[four_number].SetActive(true);
        }

        furniture_buy_ui = GameObject.Find("furniture_buy_things");
        furniture_block = GameObject.Find("furniture_block");

        //시작할때 가구 구매 여부 확인
        if (PlayerPrefs.GetInt(this.gameObject.name) == 1)
            am_i_sale = true;
        else
            am_i_sale = false;

        buy_done = GameObject.Find("furniture_purchase").GetComponent<AudioSource>();
        buy_fail = GameObject.Find("cant_buy_item").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!check_mode_just_one)
        {
            if (check_mode.mode_ == 1)
            {                
                //게임을 다시 켰을때 잠금해제된것 확인하기용
                for (int i = 0; i < 9; i++)
                {
                    //카테고리1
                    check_furniture = "fobject_" + string.Format(("{0:D2}"), i + 1);
                    page1_menu1[i] = GameObject.Find("place_menu_" + string.Format("{0:D2}", i + 1) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page1_menu1[i]);

                    check_furniture = "fobject_" + string.Format(("{0:D2}"), i + 10);
                    page2_menu1[i] = GameObject.Find("place_menu_" + string.Format("{0:D2}", i + 10) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page2_menu1[i]);

                    //카테고리2
                    check_furniture = "sobject_" + string.Format(("{0:D2}"), i + 1);
                    page1_menu1[i] = GameObject.Find("splace_menu_" + string.Format("{0:D2}", i + 1) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page1_menu1[i]);

                    check_furniture = "sobject_" + string.Format(("{0:D2}"), i + 10);
                    page2_menu1[i] = GameObject.Find("splace_menu_" + string.Format("{0:D2}", i + 10) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page2_menu1[i]);

                    //카테고리3
                    check_furniture = "tobject_" + string.Format(("{0:D2}"), i + 1);
                    page1_menu1[i] = GameObject.Find("tplace_menu_" + string.Format("{0:D2}", i + 1) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page1_menu1[i]);

                    check_furniture = "tobject_" + string.Format(("{0:D2}"), i + 10);
                    page2_menu1[i] = GameObject.Find("tplace_menu_" + string.Format("{0:D2}", i + 10) + "_lock");

                    if (PlayerPrefs.GetInt(check_furniture + "buy_state") == 1)
                        Destroy(page2_menu1[i]);
                }
                check_mode_just_one = true;
            }
        }
    }

    //디노 전체 돈
    public void Total_Money_Set(int num)
    {
        //숫자 안겹치게 먼저 초기화
        for (int i = 0; i < 10; i++)
        {
            score_one[i].SetActive(false);
            score_two[i].SetActive(false);
            score_th[i].SetActive(false);
            score_fo[i].SetActive(false);
        }

        total_money = PlayerPrefs.GetInt("dino_money");
        total_money -= num;

        four_number = total_money / 1000;
        three_number = (total_money % 1000) / 100;
        two_number = ((total_money % 1000) % 100) / 10;
        one_number = ((total_money % 1000) % 100) % 10;

        score_one[one_number].SetActive(true);
        score_two[two_number].SetActive(true);
        score_th[three_number].SetActive(true);
        score_fo[four_number].SetActive(true);

        PlayerPrefs.SetInt("dino_money", total_money);
        PlayerPrefs.Save();
    }

    //가구 배치
    public void Furniture_Place(int obj_number)
    {
        check_store_number = GameObject.Find("Canvas_cam_overlay").GetComponent<UI_Touch_Object_Place>();
        if (check_store_number.menu_number == 0)
            check_furniture = "fobject_" + string.Format(("{0:D2}"), obj_number);
        if (check_store_number.menu_number == 1)
            check_furniture = "sobject_" + string.Format(("{0:D2}"), obj_number);
        if (check_store_number.menu_number == 2)
            check_furniture = "tobject_" + string.Format(("{0:D2}"), obj_number);

        //가구 배치 부분
        //가구 배치 소리
        tui_done = GameObject.Find("done_sound");
        tui_done.GetComponent<AudioSource>().Play();

        empty_obj = GameObject.Find(check_furniture);

        if (empty_obj.GetComponent<Object_Data>().object_state == 0)
        {
            empty_obj.transform.position = new Vector3(0.1f, 0.01f, 0);
            empty_obj.transform.localScale = new Vector3(1, 1, 1);
            if (empty_obj.transform.childCount > 0)
                empty_obj.transform.rotation = Quaternion.Euler(-90, 0, 0);
            else if (empty_obj.transform.childCount == 0)
                empty_obj.transform.rotation = Quaternion.Euler(0, 0, 0);

            empty_obj.GetComponent<Object_Data>().object_state = 1;
        }
        //ui 비활성화용
        furniture_block = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
        furniture_block.SetActive(false);
        furniture_buy_ui = GameObject.Find("furniture_buy").transform.Find("furniture_buy_things").gameObject;
        furniture_buy_ui.SetActive(false);
    }

    //가구 번호 확인
    public void My_Name_(int obj_name)
    {
        furniture_number = obj_name;
    }

    //가구 가격 확인
    public void My_Price_is(int price)
    {
        my_price = price;

        check_store_number = GameObject.Find("Canvas_cam_overlay").GetComponent<UI_Touch_Object_Place>();
        if (check_store_number.menu_number == 0)
            check_furniture = "fobject_" + string.Format(("{0:D2}"), furniture_number);
        if (check_store_number.menu_number == 1)
            check_furniture = "sobject_" + string.Format(("{0:D2}"), furniture_number);
        if (check_store_number.menu_number == 2)
            check_furniture = "tobject_" + string.Format(("{0:D2}"), furniture_number);
                
        if (PlayerPrefs.GetInt(check_furniture + "buy_state") != 1)
        {
            //얼마인지 계산
            back_dan_number = my_price / 100;
            ship_dan_number = (my_price % 100) / 10;
            li_dan_number = ((my_price % 100) % 10);

            //얼마인지 찾아서 출력            
            GameObject.Find("furniture_buy").transform.Find("buy_numbers_00").gameObject.SetActive(true);
            GameObject.Find("furniture_buy").transform.Find("buy_numbers_01").gameObject.SetActive(true);
            GameObject.Find("furniture_buy").transform.Find("buy_numbers_02").gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {               
                GameObject.Find("furniture_buy").transform.Find("buy_numbers_00").transform.Find("back_one_" + string.Format("{0:D2}", i)).gameObject.SetActive(false);
                GameObject.Find("furniture_buy").transform.Find("buy_numbers_01").transform.Find("back_two_" + string.Format("{0:D2}", i)).gameObject.SetActive(false);
                GameObject.Find("furniture_buy").transform.Find("buy_numbers_02").transform.Find("back_th_" + string.Format("{0:D2}", i)).gameObject.SetActive(false);
            }
            back_dan = GameObject.Find("furniture_buy").transform.Find("buy_numbers_00").transform.Find("back_one_" + string.Format("{0:D2}", back_dan_number)).gameObject;
            ship_dan = GameObject.Find("furniture_buy").transform.Find("buy_numbers_01").transform.Find("back_two_" + string.Format("{0:D2}", ship_dan_number)).gameObject;
            li_dan = GameObject.Find("furniture_buy").transform.Find("buy_numbers_02").transform.Find("back_th_" + string.Format("{0:D2}", li_dan_number)).gameObject;
            back_dan.SetActive(true);
            ship_dan.SetActive(true);
            li_dan.SetActive(true);

            furniture_block = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
            furniture_block.SetActive(true);
            furniture_buy_ui = GameObject.Find("furniture_buy").transform.Find("furniture_buy_things").gameObject;
            furniture_buy_ui.SetActive(true);
        }
    }

    //가구 구매
    public void My_Price()
    {
        //돈 부족
        if (PlayerPrefs.GetInt("dino_money") < my_price)
        {            
            buy_fail.Play();
        }
        //돈 충분
        else
        {
            buy_done.Play();

            //돈 줄이는 부분
            Total_Money_Set(my_price);
            am_i_sale = true;
            if (check_store_number.menu_number == 0)
                check_furniture = "fobject_" + string.Format(("{0:D2}"), furniture_number);
            if (check_store_number.menu_number == 1)
                check_furniture = "sobject_" + string.Format(("{0:D2}"), furniture_number);
            if (check_store_number.menu_number == 2)
                check_furniture = "tobject_" + string.Format(("{0:D2}"), furniture_number);

            PlayerPrefs.SetInt(check_furniture + "buy_state", 1);
            PlayerPrefs.Save();

            //가구 잠금해재
            check_store_number = GameObject.Find("Canvas_cam_overlay").GetComponent<UI_Touch_Object_Place>();
            if (check_store_number.menu_number == 0)
            {
                if (check_store_number.menu_category1_number == 0)
                {
                    page1_menu1[furniture_number] = GameObject.Find("place_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page1_menu1[furniture_number]);
                }
                else if (check_store_number.menu_category1_number == 1)
                {
                    page2_menu1[furniture_number-10] = GameObject.Find("place_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page2_menu1[furniture_number-10]);
                }
            }
            if (check_store_number.menu_number == 1)
            {
                if (check_store_number.menu_category2_number == 0)
                {
                    page1_menu1[furniture_number] = GameObject.Find("splace_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page1_menu1[furniture_number]);
                }
                else if (check_store_number.menu_category2_number == 1)
                {
                    page2_menu1[furniture_number - 10] = GameObject.Find("splace_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page2_menu1[furniture_number - 10]);
                }
            }
            if (check_store_number.menu_number == 2)
            {
                if (check_store_number.menu_category3_number == 0)
                {
                    page1_menu1[furniture_number] = GameObject.Find("tplace_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page1_menu1[furniture_number]);
                }
                else if (check_store_number.menu_category2_number == 1)
                {
                    page2_menu1[furniture_number - 10] = GameObject.Find("tplace_menu_" + string.Format("{0:D2}", furniture_number) + "_lock");
                    Destroy(page2_menu1[furniture_number - 10]);
                }
            }

        }
        //가구 구매 ui 비활성화 부분
        furniture_buy_ui = GameObject.Find("furniture_buy").transform.Find("furniture_buy_things").gameObject;
        furniture_buy_ui.SetActive(false);
        //가구 블락 비활성화
        furniture_block = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
        furniture_block.SetActive(false);
        //숫자 비활성화
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_00").gameObject.SetActive(false);
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_01").gameObject.SetActive(false);
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_02").gameObject.SetActive(false);
    }
    //구매 취소
    public void Cancle_Button()
    {
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_00").gameObject.SetActive(false);
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_01").gameObject.SetActive(false);
        GameObject.Find("furniture_buy").transform.Find("buy_numbers_02").gameObject.SetActive(false);

        furniture_buy_ui = GameObject.Find("furniture_buy").transform.Find("furniture_buy_things").gameObject;
        furniture_buy_ui.SetActive(false);
        furniture_block = GameObject.Find("furniture_buy").transform.Find("furniture_block").gameObject;
        furniture_block.SetActive(false);
    }
}
