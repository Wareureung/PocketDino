using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground_block_place : MonoBehaviour
{
    //발판 갯수
    static int block_max_number = 7;
    //발판들
    GameObject[] ground_block = new GameObject[block_max_number];
    //발판 제거 체크 블럭
    GameObject check_block;
    //최상위 발판
    Vector3 top_block;
    //디노 상태(*set_timer)
    public bool mini_dino_state = false;
    //발판 밟으면 점수 증가(*set_timer)
    public int game_score = 0;
    //배경
    GameObject building_ground_00;
    GameObject building_ground_01;
    //디노
    GameObject dino_obj;

    //사운드
    GameObject background_sound;
    GameObject dino_fall;
    GameObject result_sound;


    void Start()
    {
        for (int i = 0; i < block_max_number; i++)
            ground_block[i] = GameObject.Find("ground_" + string.Format("{0:D2}", i));

        check_block = GameObject.Find("bottom_check_block");
        top_block = Vector3.zero;        

        building_ground_00 = GameObject.Find("building_ground_01");
        building_ground_01 = GameObject.Find("building_ground_02");

        dino_obj = GameObject.Find("Dino00");

        background_sound = GameObject.Find("Canvas");
        dino_fall = GameObject.Find("Dino_Fall");
        result_sound = GameObject.Find("Result_Sound");
    }

    void Update()
    {
        
    }

    void find_top_block()
    {
        //발판 사라지면 15점 추가
        game_score += 15;

        //최상단 발판 찾기
        for (int i=0; i< block_max_number; i++)
        {
            if (ground_block[i].transform.position.y > top_block.y)
            {
                top_block = ground_block[i].transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //디노 떨어지는거 확인용
        if(other.name == "Dino00")
        {
            mini_dino_state = true;
            dino_obj.SetActive(false);
            dino_fall.GetComponent<AudioSource>().Play();
            result_sound.GetComponent<AudioSource>().Play();
            background_sound.GetComponent<AudioSource>().Stop();
        }
        //첫발판 제거용
        if (other.name == "bottom_ground")
            other.gameObject.SetActive(false);

        //최상 발판 찾기
        find_top_block();

        //맨 밑에 블럭 체크하고 위로 올리기
        for (int i = 0; i < block_max_number; i++)
        {
            if (other.name == "ground_" + string.Format("{0:D2}", i))
            {
                if (top_block.x <= 0)
                    ground_block[i].transform.position = new Vector3(Random.Range(0.0f, 2.0f), top_block.y + 2, 0);
                if (top_block.x > 0)
                    ground_block[i].transform.position = new Vector3(Random.Range(-2.0f, 0.0f), top_block.y + 2, 0);                
            }
        }

        //배경 올리기
        if(other.name == "building_ground_01")
        {
            other.transform.position = new Vector3(other.transform.position.x, building_ground_01.transform.position.y + 13, other.transform.position.z);
        }
        if(other.name == "building_ground_02")
        {
            other.transform.position = new Vector3(other.transform.position.x, building_ground_00.transform.position.y + 13, other.transform.position.z);
        }
    }    
}