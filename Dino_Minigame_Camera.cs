using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dino_game_camera : MonoBehaviour
{
    //모바일 화면크기 확인
    bool check_screen_size;

    //디노 위치
    GameObject dino_;

    //디노 위치 저장용
    float dino_before_posy = 0.0f;

    void Start()
    {
        dino_ = GameObject.Find("Dino00");

        dino_before_posy = dino_.transform.position.y;

        check_screen_size = GameObject.Find("Game_Data").GetComponent<Game_Data_>().screen_size_check;

        //모바일 스크린 비율 확인
        if (check_screen_size)
            Camera.main.fieldOfView = 70f;
        else
            Camera.main.fieldOfView = 60f;
    }

    void Update()
    {
        //디노위치 확인
        if (dino_.GetComponent<dino_controller>().is_ground && dino_.transform.position.y >= dino_before_posy)
        {
            dino_before_posy = dino_.transform.position.y;
        }
        //디노 따라다니기
        if(dino_.transform.position.y >= dino_before_posy)
        {
            this.gameObject.transform.position = new Vector3(0, dino_.transform.position.y + 4.5f, -10);
        }        
    }
}
