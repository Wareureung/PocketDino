using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino_Making : MonoBehaviour
{
    //UI
    GameObject ok_button;
    GameObject circle_menu;

    GameObject dino_arrow_left;
    GameObject dino_arrow_right;
    GameObject dino_arrow_left_body;
    GameObject dino_arrow_right_body;

    //디노 텍스쳐들
    GameObject gm_dino_eyes;
    GameObject gm_dino_bodys;
    GameObject gm_dino_fins;

    //모드, 음식
    UI_Touch uit;
    Dino_Food dino_food_one;

    //모든 텍스쳐들 저장용
    Texture[] dino_eyes = new Texture[25];
    Texture[] dino_bodys = new Texture[25];
    Texture[] dino_fins = new Texture[25];

    //순서 저장용
    public int eyes_number;
    int eyes_number_for = 1;

    public int bodys_number;
    int bodys_number_for = 0;

    //애니메이션 돌아오기용 변수들
    bool trigger_food = false;
    public float food_happy_time = 0.0f;

    //디노 충돌검사용 (충돌했을때 메뉴 돌아가는거 방지용)
    public bool dino_check_crush = false;
    
    void Start()
    {
        dino_arrow_left = GameObject.Find("dino_arrow_left");
        dino_arrow_right = GameObject.Find("dino_arrow_right");
        dino_arrow_left_body = GameObject.Find("dino_arrow_left_body");
        dino_arrow_right_body = GameObject.Find("dino_arrow_right_body");

        gm_dino_eyes = GameObject.Find("Dino_Face");
        gm_dino_bodys = GameObject.Find("Dino_Body");
        gm_dino_fins = GameObject.Find("Dino_Back");

        uit = GameObject.Find("Canvas").GetComponent<UI_Touch>();
        dino_food_one = GameObject.Find("Dino_Food_Object").GetComponent<Dino_Food>();

        ok_button = GameObject.Find("ok_button");
        ok_button.SetActive(false);
        circle_menu = GameObject.Find("circle_menu");        

        //얼굴 텍스쳐 불러오기
        for (int i = 0; i < 25; i++)
        {            
            dino_eyes[i] = Resources.Load("Dino_Face" + string.Format("{0:D2}", eyes_number_for)) as Texture;
            eyes_number_for++;
        }
        //몸통 텍스쳐 불러오기
        for (int i = 0; i < 25; i++)
        {
            dino_bodys[i] = Resources.Load("Dino_" + string.Format("{0:D2}", bodys_number_for)) as Texture;
            dino_fins[i] = Resources.Load("Dino_" + string.Format("{0:D2}", bodys_number_for)) as Texture;
            bodys_number_for++;

        }
        
        //눈 저장확인
        if (!PlayerPrefs.HasKey("Dino_eyes"))
            eyes_number = 23;
        else
        {            
            eyes_number = PlayerPrefs.GetInt("Dino_eyes");
            gm_dino_eyes.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_eyes[eyes_number];
        }
        //몸통 저장확인
        if (!PlayerPrefs.HasKey("Dino_bodys"))
            bodys_number = 1;
        else
        {
            bodys_number = PlayerPrefs.GetInt("Dino_bodys");
            gm_dino_bodys.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_bodys[bodys_number];
            gm_dino_fins.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_fins[bodys_number];
        }

        dino_arrow_left.SetActive(false);
        dino_arrow_right.SetActive(false);
        dino_arrow_left_body.SetActive(false);
        dino_arrow_right_body.SetActive(false);
    }

    void Update()
    {
        if (uit.mode_ == 4)
            food_crush_check();
    }

    //음식 모션 변경
    void food_crush_check()
    {
        if(trigger_food)
        {
            food_happy_time += Time.deltaTime;
            if(food_happy_time > 1.3f)
            {
                gameObject.GetComponent<Animator>().SetInteger("animation", 2);
            }
            if(food_happy_time > 2.6f)
            {
                gameObject.GetComponent<Animator>().SetInteger("animation", 4);
                food_happy_time = 0.0f;
                trigger_food = false;
                dino_food_one.ami_eat = false;
            }
        }
    }

    //모드 변경
    public void change_dino_making_mode()
    {        
        if(uit.mode_ == 0 || uit.mode_ == 2)
        {
            uit.mode_ = 3;
            uit.menu_state = 2;

            ok_button.SetActive(true);
            circle_menu.SetActive(false);

            //디노 이미지 교체용 화살표
            dino_arrow_left.SetActive(true);
            dino_arrow_right.SetActive(true);
            dino_arrow_left_body.SetActive(true);
            dino_arrow_right_body.SetActive(true);
        }

    }
    
    //OK버튼 눌렀을때 데이터 저장
    public void save_dino_making()
    {
        if (uit.mode_ == 3)
        {
            uit.mode_ = 2;

            ok_button.SetActive(false);
            circle_menu.SetActive(true);

            //디노 이미지 교체용 화살표
            dino_arrow_left.SetActive(false);
            dino_arrow_right.SetActive(false);
            dino_arrow_left_body.SetActive(false);
            dino_arrow_right_body.SetActive(false);

            PlayerPrefs.SetInt("Dino_eyes", eyes_number);
            PlayerPrefs.SetInt("Dino_bodys", bodys_number);
            PlayerPrefs.SetInt("Dino_fins", bodys_number);
            PlayerPrefs.Save();
        }
    }

    //머리 좌측 화살표
    public void arrow_left()
    {
        eyes_number--;
        if (eyes_number == 1)
            eyes_number = 25;        
            
        gm_dino_eyes.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_eyes[eyes_number];
    }

    //머리 우측 화살표
    public void arrow_right()
    {
        eyes_number++;
        if (eyes_number == 25)
            eyes_number = 1;        
            
        gm_dino_eyes.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_eyes[eyes_number];
    }

    //몸통 좌측 화살표
    public void arrow_left_body()
    {
        bodys_number--;
        if (bodys_number == 1)
            bodys_number = 25;

        gm_dino_bodys.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_bodys[bodys_number];
        gm_dino_fins.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_fins[bodys_number];
    }

    //몸통 우측 화살표
    public void arrow_right_body()
    {
        bodys_number++;
        if (bodys_number == 25)
            bodys_number = 1;

        gm_dino_bodys.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_bodys[bodys_number];
        gm_dino_fins.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_fins[bodys_number];
    }

    //먹이와 충돌
    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.tag == "food")
        {
            gameObject.GetComponent<Animator>().SetInteger("animation", 5);
            dino_check_crush = false;
            trigger_food = true;
        }
        else
            dino_check_crush = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "food")
        {
            dino_check_crush = false;
        }
        else
            dino_check_crush = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "food")
        {
            dino_check_crush = false;
        }
        else
            dino_check_crush = false;
    }
}