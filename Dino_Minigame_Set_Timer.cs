using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class set_timer : MonoBehaviour
{
    //디노 상태
    ground_block_place dino_state_check;

    //사운드
    GameObject enter_sound;

    //게임 종료용
    bool game_done = false;

    //페이드인
    GameObject fade_in;
    GameObject[] count_down = new GameObject[3];
    bool fade_in_state = false;    
    float fade_in_speed = 1.0f;    
    float fade_in_timer = 0.0f;
    int fade_number = 0;


    public bool start_game = false;

    //타이머
    GameObject[] timer_one = new GameObject[10];
    GameObject[] timer_two = new GameObject[10];
    GameObject[] timer_th = new GameObject[10];
    GameObject[] timer_fo = new GameObject[10];

    //시간 받아오기
    float game_timer = 0.0f;
    //일초
    int game_one = 0;
    //십초
    int game_two = 0;
    //일분
    int game_th = 0;
    //십분
    int game_fo = 0;

    //Score_Timer
    GameObject[] scroe_timer_one = new GameObject[10];
    GameObject[] scroe_timer_two = new GameObject[10];
    GameObject[] scroe_timer_th = new GameObject[10];
    GameObject[] scroe_timer_fo = new GameObject[10];

    GameObject dotdot_;

    //점수
    GameObject[] scroe_one = new GameObject[10];
    GameObject[] scroe_two = new GameObject[10];
    GameObject[] scroe_th = new GameObject[10];
    GameObject[] scroe_fo = new GameObject[10];

    int total_score = 0;
    int four_number;
    int three_number;
    int two_number;
    int one_number;

    //점수 이미지
    GameObject score_background;
    GameObject score_enter;

    void Start()
    {
        dino_state_check = GameObject.Find("bottom_check_block").GetComponent<ground_block_place>();

        enter_sound = GameObject.Find("enter_sound");

        score_background = GameObject.Find("result_back");
        score_enter = GameObject.Find("result_enter");

        score_background.SetActive(false);
        score_enter.SetActive(false);

        count_down[0] = GameObject.Find("count_00");
        count_down[1] = GameObject.Find("count_01");
        count_down[2] = GameObject.Find("count_02");

        count_down[0].SetActive(false);
        count_down[1].SetActive(false);
        count_down[2].SetActive(false);

        fade_in = GameObject.Find("fade_out_img");
        fade_in.SetActive(true);
        fade_in_state = false;

        //타이머
        for (int i=0; i<10; i++)
        {
            timer_one[i] = GameObject.Find("go_time_number_00").transform.GetChild(i).gameObject;
            timer_two[i] = GameObject.Find("go_time_number_01").transform.GetChild(i).gameObject;
            timer_th[i] = GameObject.Find("go_time_number_02").transform.GetChild(i).gameObject;
            timer_fo[i] = GameObject.Find("go_time_number_03").transform.GetChild(i).gameObject;

            timer_one[i].SetActive(false);
            timer_two[i].SetActive(false);
            timer_th[i].SetActive(false);
            timer_fo[i].SetActive(false);

            scroe_timer_one[i] = GameObject.Find("time_number_00").transform.GetChild(i).gameObject;
            scroe_timer_two[i] = GameObject.Find("time_number_01").transform.GetChild(i).gameObject;
            scroe_timer_th[i] = GameObject.Find("time_number_02").transform.GetChild(i).gameObject;
            scroe_timer_fo[i] = GameObject.Find("time_number_03").transform.GetChild(i).gameObject;

            scroe_timer_one[i].SetActive(false);
            scroe_timer_two[i].SetActive(false);
            scroe_timer_th[i].SetActive(false);
            scroe_timer_fo[i].SetActive(false);

            scroe_one[i] = GameObject.Find("result_numbers_00").transform.GetChild(i).gameObject;
            scroe_two[i] = GameObject.Find("result_numbers_01").transform.GetChild(i).gameObject;
            scroe_th[i] = GameObject.Find("result_numbers_02").transform.GetChild(i).gameObject;
            scroe_fo[i] = GameObject.Find("result_numbers_03").transform.GetChild(i).gameObject;

            scroe_one[i].SetActive(false);
            scroe_two[i].SetActive(false);
            scroe_th[i].SetActive(false);
            scroe_fo[i].SetActive(false);
        }
        timer_one[0].SetActive(true);
        timer_two[0].SetActive(true);
        timer_th[0].SetActive(true);
        timer_fo[0].SetActive(true);

        dotdot_ = GameObject.Find("dotdot_1");
        dotdot_.SetActive(false);
    }

    void Update()
    {
        //시작 페이드인
        if (!start_game && !game_done)
            Go_Fade_in();
        //시작하면 게임 타이머 카운트
        if (!dino_state_check.mini_dino_state && start_game)
            Timer();
        //끝나면 점수 보여주기
        if (dino_state_check.mini_dino_state && !game_done)
            Score_Show();
        //게임종료 페이드아웃
        if (fade_in_state)
            Go_Fade_Out();
    }

    //게임 타이머
    void Timer()
    {        
        game_timer += Time.deltaTime;
        
        game_one = (int)game_timer / 1;

        if (game_one != 0 && game_one > 0)
        {            
            timer_one[game_one - 1].SetActive(false);
        }
        if (game_one >= 10)
        {
            game_timer = 0.0f;
            game_one = 0;
            if (game_two < 6)
            {
                timer_two[game_two].SetActive(false);
                game_two++;
            }
        }        
        if (game_two == 6)
        {
            timer_two[game_two].SetActive(false);
            game_two = 0;
            timer_th[game_th].SetActive(false);
            game_th++;
        }
        timer_one[game_one].SetActive(true);
        timer_two[game_two].SetActive(true);
        timer_th[game_th].SetActive(true);
    }

    //점수 보여주기
    void Score_Show()
    {
        score_background.SetActive(true);
        score_enter.SetActive(true);
        dotdot_.SetActive(true);

        scroe_timer_one[game_one].SetActive(true);
        scroe_timer_two[game_two].SetActive(true);
        scroe_timer_th[game_th].SetActive(true);
        scroe_timer_fo[game_fo].SetActive(true);

        //score
        total_score = dino_state_check.game_score;

        four_number = total_score / 1000;
        three_number = (total_score % 1000) / 100;
        two_number = ((total_score % 1000) % 100) / 10;
        one_number = ((total_score % 1000) % 100) % 10;

        scroe_one[one_number].SetActive(true);
        scroe_two[two_number].SetActive(true);
        scroe_th[three_number].SetActive(true);
        scroe_fo[four_number].SetActive(true);
                
        start_game = false;
        game_done = true;        
    }

    //점수만큼 가진돈 추가
    public void Set_Money()
    {
        enter_sound.GetComponent<AudioSource>().Play();

        total_score += PlayerPrefs.GetInt("dino_money");
        PlayerPrefs.SetInt("dino_money", total_score);
        PlayerPrefs.Save();

        fade_in_state = true;
        fade_in.SetActive(true);
    }

    //페이드인, 카운트다운
    void Go_Fade_in()
    {
        if (fade_in.transform.localScale.x > 0)
        {
            fade_in.transform.localScale = new Vector3(fade_in.transform.localScale.x - fade_in_speed, fade_in.transform.localScale.y - fade_in_speed, fade_in.transform.localScale.z - fade_in_speed);
        }
        else if (fade_in.transform.localScale.x <= 0 && start_game == false)
        {            
            fade_in.SetActive(false);

            if (fade_number == 3)
                start_game = true;
            else
            {
                fade_in_timer += Time.deltaTime;
                count_down[fade_number].SetActive(true);
                if (fade_in_timer < 1.0f)
                {
                    count_down[fade_number].transform.localScale = new Vector3(count_down[fade_number].transform.localScale.x - 0.01f, count_down[fade_number].transform.localScale.y - 0.01f, count_down[fade_number].transform.localScale.z - 0.01f);
                }
                else if (fade_in_timer >= 1.0f)
                {
                    fade_in_timer = 0.0f;
                    count_down[fade_number].SetActive(false);

                    fade_number++;
                }
            }
        }
    }

    //페이드 아웃
    void Go_Fade_Out()
    {
        if (fade_in.transform.localScale.x > 30)
        {
            PlayerPrefs.SetInt("mini_game_check", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("main_");
        }

        fade_in.transform.localScale = new Vector3(fade_in.transform.localScale.x + fade_in_speed, fade_in.transform.localScale.y + fade_in_speed, fade_in.transform.localScale.z + fade_in_speed);
    }
}
