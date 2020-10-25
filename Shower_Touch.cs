using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shower_Touch : MonoBehaviour
{
    //디노 애니메이션
    Animator Dino;

    //샤워 이펙트
    GameObject shower_effect;

    //시간
    bool shower_stop = false;
    float shower_time;

    //샤워 상태바
    public Slider shower;

    //사운드
    AudioSource shower_sound;
    AudioSource dino_voice;

    void Start()
    {
        Dino = GameObject.Find("Dino").GetComponent<Animator>();

        shower = GameObject.Find("shower_silder").GetComponent<Slider>();

        shower_effect = GameObject.Find("WaterFall");

        //샤워
        if (PlayerPrefs.HasKey("state_my_shower"))
        {
            shower.value = PlayerPrefs.GetFloat("state_my_shower");
        }
        //한번도 실행시킨적 없으면 0.5f로 초기화
        else
            shower.value = 0.5f;

        shower_sound = GameObject.Find("shower_water_sound").GetComponent<AudioSource>();
        dino_voice = GameObject.Find("dino_shower_voice").GetComponent<AudioSource>();
    }

    
    void Update()
    {
        //샤워 시간에 따라 애니메이션 변경
        if (shower_stop)
        {
            shower_time += Time.deltaTime;

            Dino.SetInteger("animation", 12);

            if (shower_time < 2f)
                Dino.SetInteger("animation", 12);
            if (shower_time >= 2f)
            {
                shower_sound.Stop();

                Dino.SetInteger("animation", 2);
                shower_effect.GetComponent<ParticleSystem>().Stop();
            }
            if (shower_time > 3f)
            {
                Dino.SetInteger("animation", 1);
                shower_stop = false;
                shower_time = 0f;
            }
        }
    }

    //샤워 이펙트, 사운드
    public void start_shower_effect()
    {
        shower_sound.Play();
        dino_voice.Play();

        shower_effect = GameObject.Find("WaterFall");
        shower_effect.GetComponent<ParticleSystem>().Play();
        shower_stop = true;

        //gage up
        shower.value += 0.1f;
        PlayerPrefs.SetFloat("state_my_shower", shower.value);      //값이 증가하면 바로 저장
        PlayerPrefs.Save(); //저장한 값 디스크에 쓰기        
    }
}
