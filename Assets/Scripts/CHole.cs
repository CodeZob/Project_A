using UnityEngine;
using System.Collections;

public enum EHoleState
{
    None,
    Open,
    Idle,
    Close,
    Catch
}

public class CHole : MonoBehaviour 
{
    // 애니메이션에 기능에 사용될 변수
    private string[] mSpriteNames;
    private UISprite mSprite;
    private UISpriteAnimation mAni;
    public UIAtlas[] mAtlases;

    // Hole 상태값
    public EHoleState mState = EHoleState.Close;

    // 좋은 몹 나쁜 몹 선택할 때 사용 하는 변수
    private bool mGoodEnemy = false;
    public int mGoodPer = 15;

    // 몹 스폰 스피드 결정 변수
    public float mMinWaitTime = 0.5f;
    public float mMaxWaitTime = 4.5f;

    // 사운드 플레이용 변수
    public AudioClip mOpenSound;
    public AudioClip mCatchSound;

    // 게임 플레이 매니저
    private CPlayManager mPlayManager;
   
    #region property
    public int goodPercent
    {
        get
        {
            return mGoodPer;
        }
        set
        {
            if (value > 100)
                mGoodPer = 100;
            else if (value < 0)
                mGoodPer = 0;
            else
                mGoodPer = value;
        }
    }
    #endregion

    void Awake()
    {
        mPlayManager = GameObject.Find("PlayManager").GetComponent<CPlayManager>();
        mSpriteNames = new string[4];
        mSprite = GetComponent<UISprite>();
        mAni = GetComponent<UISpriteAnimation>();
        mAni.Pause();
    }

    void Update()
    {
        AniCycle();
    }

    private void SetAtlas(UIAtlas atlas)
    {
        string spriteName;

        if (mGoodEnemy)
        {
            mSpriteNames[0] = "D_Dudigi_Open_";
            mSpriteNames[1] = "D_Dudigi_Idle_";
            mSpriteNames[2] = "D_Dudigi_Close_";
            mSpriteNames[3] = "D_Dudigi_Catch_";
            spriteName = mSpriteNames[0] + "000";
        }
        else
        {
            mSpriteNames[0] = "A_Dudugi_Open_";
            mSpriteNames[1] = "A_Dudugi_Idle_";
            mSpriteNames[2] = "A_Dudugi_Close_";
            mSpriteNames[3] = "A_Dudugi_Catch_";
            spriteName = mSpriteNames[0] + "002";
        }

        mSprite.atlas = atlas;
        mSprite.spriteName = spriteName;
    }

    private bool RandomInclination()
    {
        int result = Random.Range(0, 100 + 1);
        mGoodEnemy = (result <= mGoodPer) ? true : false;

        return mGoodEnemy;
    }

    private void ChoiceEnemy()
    {
        UIAtlas atlas = RandomInclination() ? mAtlases[0] : mAtlases[1];

        SetAtlas(atlas);
    }

    public void OnMouseDown()
    {
        if (mPlayManager.state == EPlayState.End)
            return;

        if(mState == EHoleState.Open || mState == EHoleState.Idle || mState == EHoleState.Close)
        {
            CatchOn();
        }
    }

    #region animation
    private void AniCycle()
    {
        if (mAni.isPlaying)
            return;

        switch (mState)
        {
            case EHoleState.Open:
                IdleOn();
                break;

            case EHoleState.Idle:
                CloseOn();
                break;

            case EHoleState.Close:
                NoneOn();
                break;

            case EHoleState.Catch:
                NoneOn();
                break;
        }
    }

    private void AniOn(EHoleState state, bool loop, int fps, ref string name)
    {
        mState = state;
        mAni.loop = loop;
        mAni.framesPerSecond = fps;
        mAni.namePrefix = name;
        mAni.ResetToBeginning();
    }

    public void OpenOn()
    {
        audio.clip = mOpenSound;
        audio.Play();
        AniOn(EHoleState.Open, false, 30, ref mSpriteNames[0]);

        if (mPlayManager.state == EPlayState.Ready)
        {
            mPlayManager.OnPlay();
        }
    }

    public void IdleOn()
    {
        AniOn(EHoleState.Idle, false, 30, ref mSpriteNames[1]);
    }

    public void CloseOn()
    {
        AniOn(EHoleState.Close, false, 30, ref mSpriteNames[2]);
    }

    public void CatchOn()
    {
        audio.clip = mCatchSound;
        audio.Play();
        AniOn(EHoleState.Catch, false, 30, ref mSpriteNames[3]);

        if (this.mGoodEnemy)
            mPlayManager.KillGoodEnemy();
        else
            mPlayManager.KillBadEnemy();
    }

    public void NoneOn()
    {
        mState = EHoleState.None;
        StartCoroutine("Wait");
    }

    public IEnumerator Wait()
    {
        if (mPlayManager.state == EPlayState.Ready)
        {
            yield return new WaitForSeconds(1.0f);
        }
        float waitTime = Random.Range(mMinWaitTime, mMaxWaitTime);
        yield return new WaitForSeconds(waitTime);
        ChoiceEnemy();
        OpenOn();
    }
    #endregion
}
