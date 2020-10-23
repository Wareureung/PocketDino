using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dino_controller : MonoBehaviour
{
    //디노 애니메이터
    Animator dino_ani;
    //게임시작 확인
    set_timer start_state;

    //저장된 디노 커스터마이징 상태 가져오기
    Texture[] dino_eyes = new Texture[25];
    Texture[] dino_bodys = new Texture[25];
    Texture[] dino_fins = new Texture[25];

    //커스터 마이징 정보
    GameObject gm_dino_eyes;
    GameObject gm_dino_bodys;
    GameObject gm_dino_fins;

    //커스터 마이징 정보
    int eyes_number;
    int eyes_number_for = 1;
    int bodys_number;
    int bodys_number_for = 0;

    //디노 속도
    float dino_move_= 4.0f;
    float dino_move_speed = 4.0f;

    //디노 방향(좌, 우 구분)
    float dino_angle = 90.0f;

    //땅확인
    public bool is_ground = false;

    //좌, 우 충돌 확인
    bool dino_crash_left = false;
    bool dino_crash_right = false;

    //터치 시간 확인
    float touchDuration = 0.0f;
    
    //디노 원래 정보 저장용
    float ori_dp;
    float ori_angle;

    //방향 변경
    bool flip_way = false;

    void Start()
    {
        dino_ani = GameObject.Find("Dino00").GetComponent<Animator>();
        dino_ani.SetInteger("animation", 18);

        start_state = GameObject.Find("Canvas").GetComponent<set_timer>();

        if (SceneManager.GetActiveScene().name == "dino_game_")
        {
            Physics.gravity = new Vector3(0, -20.0f, 0);
        }

        gm_dino_eyes = GameObject.Find("Dino_Face");
        gm_dino_bodys = GameObject.Find("Dino_Body");
        gm_dino_fins = GameObject.Find("Dino_Back");

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
        {
            eyes_number = 23;            
        }
        else
        {
            eyes_number = PlayerPrefs.GetInt("Dino_eyes");            
        }
        gm_dino_eyes.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_eyes[eyes_number];
        //몸통 저장확인
        if (!PlayerPrefs.HasKey("Dino_bodys"))
        {
            bodys_number = 1;            
        }
        else
        {
            bodys_number = PlayerPrefs.GetInt("Dino_bodys");            
        }
        gm_dino_bodys.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_bodys[bodys_number];
        gm_dino_fins.GetComponent<SkinnedMeshRenderer>().material.mainTexture = dino_fins[bodys_number];
    }

    void Update()
    {
        Touch_();                
        dino_move_go();        
    }

    //디노 좌,우 자동이동    
    void dino_move_go()
    {
        //좌,우로만 움직이면 되서 x,z축은 0도
        this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);

        //좌,우 벽 부딪히면 방향 바꾸기
        if (dino_crash_right)
        {
            dino_move_ = -dino_move_speed;
            this.gameObject.transform.eulerAngles = new Vector3(0, -dino_angle, 0);
        }
        else if (dino_crash_left)
        {
            dino_move_ = dino_move_speed;
            this.gameObject.transform.eulerAngles = new Vector3(0, dino_angle, 0);
        }

        //이동
        this.gameObject.transform.position = new Vector3((this.gameObject.transform.position.x) + dino_move_ * Time.deltaTime, this.gameObject.transform.position.y, 0);
    }
    
    //디노 발판 통과용
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
            this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "ground")
            this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //바닥이랑 충돌 검사 (점프 1번하기 용도)
        if (collision.collider.tag == "ground")
        {
            is_ground = true;
        }
        //벽이랑 충돌검사 (반대 방향으로 전환)
        if (collision.collider.tag == "left_wall")
            dino_crash_left = true;
        if (collision.collider.tag == "right_wall")
            dino_crash_right = true;
    }

    private void OnCollisionStay(Collision collision)
    {        
        //바닥이랑 충돌 검사 (점프 1번하기 용도)
        if (collision.collider.tag == "ground")
        {
            is_ground = true;
        }
        //벽이랑 충돌검사 (반대 방향으로 전환)
        if (collision.collider.tag == "left_wall")
            dino_crash_left = true;
        if (collision.collider.tag == "right_wall")
            dino_crash_right = true;
    }
    private void OnCollisionExit(Collision collision)
    {        
        is_ground = false;
        dino_crash_left = false;
        dino_crash_right = false;
    }

    //터치
    void Touch_()
    {
        if (Input.touchCount > 0 && start_state.start_game)
        {
            touchDuration += Time.deltaTime;

            Touch touch = Input.GetTouch(0);

            if(touch.tapCount == 1)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    //원래 디노 스피드
                    if (dino_move_ == 4 || dino_move_ == -4)
                        ori_dp = dino_move_;

                    ori_angle = transform.eulerAngles.y;
                }
                if (touch.phase == TouchPhase.Stationary && touchDuration > 0.18f && dino_move_ != 0)
                {
                    //멈춤
                    dino_move_ = 0;                    
                    //원래값에서 반대로 돌림
                    flip_way = true;
                    //방향 반전
                    transform.eulerAngles = new Vector3(0, -ori_angle, 0);
                }
                if(touch.phase == TouchPhase.Ended)
                {
                    if (is_ground)
                    {
                        if (flip_way)
                        {
                            dino_move_ = -ori_dp;
                        }
                        else if (!flip_way)
                        {
                            dino_move_ = ori_dp;                            
                        }
                        dino_move_go();

                        //디노 점프
                        this.gameObject.GetComponent<AudioSource>().Play();
                        this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 10.0f, ForceMode.Impulse);
                    }
                    //-인 이유 : Stationary를 거쳤을때만 dino_move 속도가 0이 되기 때문
                    if (dino_move_ == 0 && flip_way)
                        dino_move_ = -4;

                    touchDuration = 0.0f;
                    flip_way = false;                    
                }
            }            
        }        
    }
}
