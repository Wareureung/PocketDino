using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dino_Food : MonoBehaviour
{
    //플레이어 머니
    State_Bar about_money;

    //먹이 상태바
    public Slider eat;

    //음식 오브젝트    
    GameObject[] food_object = new GameObject[5];
    //음식 종류별 갯수
    int[] food_object_number = new int[5];

    //UI
    GameObject food_text;
    GameObject food_left_arrow;
    GameObject food_right_arrow;

    GameObject[] food_image = new GameObject[5];
    GameObject[] food_image_price = new GameObject[5];
    //음식 순번
    int food_order_number = 0;
        
    //음식 복사용
    GameObject food_clone;
    
    //모드 확인
    UI_Touch uit;
    Dino_Making dino_m;    

    //디노, 디노 애니메이션
    GameObject dino_object;
    Animator Dino;

    //음식 구매 관련
    GameObject food_buy_ui;
    GameObject[] food_buy_number = new GameObject[10];
    int food_number_img_count = 0;
    bool food_buy_img_state = false;
    int food_price = 0;

    //먹이 상태
    public bool ami_eat = false;

    //이펙트, 사운드
    GameObject food_buy_particle;
    GameObject food_buy_done_sound;
    GameObject food_buy_fail_sound;

    void Start()
    {
        about_money = GameObject.Find("Canvas").GetComponent<State_Bar>();

        eat = GameObject.Find("food_silder").GetComponent<Slider>();

        //배고픔
        if (PlayerPrefs.HasKey("state_my_eat"))
        {
            eat.value = PlayerPrefs.GetFloat("state_my_eat");
        }
        //한번도 실행시킨적 없으면 0.5f로 초기화
        else
            eat.value = 0.5f;

        food_buy_done_sound = GameObject.Find("food_buy_done_sound");
        food_buy_fail_sound = GameObject.Find("food_buy_fail_sound");

        food_text = GameObject.Find("food_img_txt");
        food_text.SetActive(false);

        food_left_arrow = GameObject.Find("food_arrow_left");
        food_right_arrow = GameObject.Find("food_arrow_right");
        food_left_arrow.SetActive(false);
        food_right_arrow.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            food_image[i] = GameObject.Find("food_img_" + string.Format("{0:D2}", i));
            food_image[i].SetActive(false);
            
            //음식갯수 초기화
            if (!PlayerPrefs.HasKey("food_number_" + string.Format("{0:D2}", i)))
                food_object_number[i] = 3;
            else
                food_object_number[i] = PlayerPrefs.GetInt("food_number_" + string.Format("{0:D2}", i));

            //음식가격
            food_image_price[i] = GameObject.Find("food_" + string.Format("{0:D2}", i) + "_price");
            food_image_price[i].SetActive(false);
        }

        food_object[0] = GameObject.Find("ChickenLeg");
        food_object[1] = GameObject.Find("Cheese");
        food_object[2] = GameObject.Find("Croissant");
        food_object[3] = GameObject.Find("Fish");
        food_object[4] = GameObject.Find("Meat");


        uit = GameObject.Find("Canvas").GetComponent<UI_Touch>();
        dino_m = GameObject.Find("Dino").GetComponent<Dino_Making>();        

        dino_object = GameObject.Find("Dino");
        Dino = GameObject.Find("Dino").GetComponent<Animator>();

        food_buy_ui = GameObject.Find("food_buy_things");
        food_buy_ui.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            food_buy_number[i] = GameObject.Find("food_buy_number_" + string.Format("{0:D2}", i));
            food_buy_number[i].SetActive(false);
        }
        food_buy_particle = GameObject.Find("food_buy_boom");
        food_buy_particle.GetComponent<ParticleSystem>().Stop();
        food_buy_particle.GetComponent<AudioSource>().Stop();
    }

    void Update()
    {
        //[버튼 클릭 -> 함수호출]로 구성
    }

    public void change_food_mode()
    {
        if (uit.mode_ == 2 || uit.mode_ == 0)
        {
            uit.mode_ = 4;
            Dino.SetInteger("animation", 4);
            //화살표 활성화
            food_left_arrow.SetActive(true);
            food_right_arrow.SetActive(true);
            //음식 이미지 활성화
            food_order_number = 0;
            food_image[food_order_number].SetActive(true);
            //숫자 활성화
            food_text.SetActive(true);
            food_text.GetComponent<Text>().text = ("x" + string.Format("{0:D1}", food_object_number[food_order_number]));
        }
        else if (uit.mode_ == 4)
        {
            uit.mode_ = 2;
            Dino.SetInteger("animation", 1);

            //음식관련 ui 전부다 종료
            food_left_arrow.SetActive(false);
            food_right_arrow.SetActive(false);
            food_image[food_order_number].SetActive(false);
            food_text.SetActive(false);
            //음식 구매 관련
            food_buy_img_state = false;     //음식 구매 관련 조건 진입위해 초기화
            food_buy_ui.SetActive(false);
            food_buy_number[food_number_img_count].SetActive(false);
        }
    }

    //음식 소환
    public void food_img_touch()
    {
        food_buy_particle.transform.position = new Vector3(dino_object.transform.position.x, dino_object.transform.position.y + 2.2f, dino_object.transform.position.z);
        //음식 갯수 부족하면 구매 이미지로 넘어감
        if (food_object_number[food_order_number] == 0 && !food_buy_img_state)
        {
            food_number_img_count = 0;
            food_image_price[food_order_number].SetActive(true);
            food_buy_ui.SetActive(true);
            food_buy_img_state = true;
            food_buy_number[food_number_img_count].SetActive(true);
            food_left_arrow.SetActive(false);
            food_right_arrow.SetActive(false);
            food_text.SetActive(false);
        }
        //음식 갯수 충분시 애니메이션, 음식 클론 떨어뜨리기
        if (dino_m.food_happy_time == 0 && food_object_number[food_order_number] != 0 && !food_buy_img_state)
        {
            ami_eat = true;
            //게이지 올리기용
            food_buy_particle.GetComponent<ParticleSystem>().Play();
            food_buy_particle.GetComponent<AudioSource>().Play();

            food_clone = Instantiate(food_object[food_order_number]);
            food_clone.GetComponent<Rigidbody>().useGravity = true;
            food_clone.transform.position = new Vector3(dino_object.transform.position.x, dino_object.transform.position.y + 2.1f, dino_object.transform.position.z);

            food_object_number[food_order_number]--;
            food_text.GetComponent<Text>().text = ("x" + string.Format("{0:D1}", food_object_number[food_order_number]));
            //갯수 저장
            PlayerPrefs.SetInt("food_number_" + string.Format("{0:D2}", food_order_number), food_object_number[food_order_number]);
            PlayerPrefs.Save();
        }
    }    

    //좌측 화살표
    public void food_arrow_left()
    {
        //이전 이미지 끄기        
        food_image[food_order_number].SetActive(false);
        //숫자 감소 시키고 새로운 이미지 활성화
        food_order_number--;
        if (food_order_number < 0)
            food_order_number = 4;
        food_image[food_order_number].SetActive(true);
        food_text.GetComponent<Text>().text = ("x" + string.Format("{0:D1}",food_object_number[food_order_number]));
    }

    //우측 화살표
    public void food_arrow_right()
    {
        //이전 이미지 끄기        
        food_image[food_order_number].SetActive(false);
        //숫자 감소 시키고 새로운 이미지 활성화
        food_order_number++;
        if (food_order_number > 4)
            food_order_number = 0;
        food_image[food_order_number].SetActive(true);
        food_text.GetComponent<Text>().text = ("x" + string.Format("{0:D1}", food_object_number[food_order_number]));
    }

    //구매 갯수 증가
    public void food_buy_plus()
    {
        if (food_number_img_count >= 5)
        {
            food_number_img_count = 5;
            food_buy_number[5].SetActive(true);
        }
        else
        {
            food_buy_number[food_number_img_count].SetActive(false);
            food_number_img_count++;
            food_buy_number[food_number_img_count].SetActive(true);
        }
    }

    //구매 갯수 감소
    public void food_buy_min()
    {
        if (food_number_img_count <= 0)
        {
            food_number_img_count = 0;
            food_buy_number[0].SetActive(true);
        }
        else
        {
            food_buy_number[food_number_img_count].SetActive(false);
            food_number_img_count--;
            food_buy_number[food_number_img_count].SetActive(true);
        }
    }

    //음식 구매
    public void food_buy()
    {        
        //음식 종류별 가격
        if(food_order_number == 0)
        {
            food_price = 100 * food_number_img_count;            
        }
        if (food_order_number == 1)
        {
            food_price = 150 * food_number_img_count;            
        }
        if (food_order_number == 2)
        {
            food_price = 200 * food_number_img_count;            
        }
        if (food_order_number == 3)
        {
            food_price = 120 * food_number_img_count;            
        }
        if (food_order_number == 4)
        {
            food_price = 220 * food_number_img_count;            
        }

        //돈 부족시
        if (PlayerPrefs.GetInt("dino_money") < food_price)
            food_buy_fail_sound.GetComponent<AudioSource>().Play();
        //돈 충분시
        else
        {
            food_buy_done_sound.GetComponent<AudioSource>().Play();
            about_money.Total_Money_Set(food_price);
            food_buy_fuction();
        }
    }

    //구매 취소
    public void food_buy_cancle()
    {
        food_image_price[food_order_number].SetActive(false);
        food_buy_img_state = false;
        food_buy_ui.SetActive(false);
        food_left_arrow.SetActive(true);
        food_right_arrow.SetActive(true);
        food_text.SetActive(true);
        food_buy_number[food_number_img_count].SetActive(false);
    }

    //구매 완료
    void food_buy_fuction()
    {
        food_buy_img_state = false;
        food_object_number[food_order_number] = food_number_img_count;
        food_text.GetComponent<Text>().text = ("x" + string.Format("{0:D1}", food_object_number[food_order_number]));
        PlayerPrefs.SetInt("food_number_" + string.Format("{0:D2}", food_order_number), food_object_number[food_order_number]);
        PlayerPrefs.Save();

        food_buy_ui.SetActive(false);
        food_left_arrow.SetActive(true);
        food_right_arrow.SetActive(true);
        food_text.SetActive(true);
        food_buy_number[food_number_img_count].SetActive(false);
    }
}
