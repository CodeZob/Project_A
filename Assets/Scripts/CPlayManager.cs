using UnityEngine;
using System.Collections;

public enum EPlayState
{
    Ready,
    Play,
    End
}

public class CPlayManager : MonoBehaviour 
{
    // 플레이 상태 변수
    private EPlayState mState;

    // 플레이에 사용되는 사운드
    public AudioClip mReadySound;
    public AudioClip mGoSound;
    public AudioClip mFinishSound;
    public AudioClip mBGM;

    // 게임 제한 시간에 사용될 변수
    public float mLimitTime = 60.0f;
    private float mConverter;
    private CProgressBar mProgress;

    // 몹 잡은 횟수와 점수 변수
    public UILabel mScoreLabel;
    public int mScore = 0;
    public int mBadCount = 0;
    public int mGoodCount = 0;
    public int mBadPoint = 1;
    public int mGoodPoint = -1;

    // Result UI 변수들
    public GameObject mUIResult;
    public UILabel mLabelResultScore;

    #region property
    public EPlayState state
    {
        get
        {
            return mState;
        }
        set
        {
            mState = value;
        }
    }
    #endregion


    void Awake()
    {
        mState = EPlayState.Ready;
        mProgress = GameObject.Find("LimitTimeGauge").GetComponent<CProgressBar>();
        mConverter = 1 / mLimitTime;
    }

	// Use this for initialization
	void Start () 
    {
        OnReady();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (mState == EPlayState.Play)
        {
            Playing();
        }
	}

    private void Playing()
    {
        float convertValue;

        if ((mLimitTime -= RealTime.deltaTime) <= 0)
        {
            mLimitTime = 0;
            OnEnd();
        }

        convertValue = mLimitTime * mConverter;
        mProgress.SetFill(convertValue);
    }

    public void OnRestartButton()
    {
        Application.LoadLevel("Play");
    }

    public void OnMainButton()
    {
        Application.LoadLevel("Title");
    }

    public void KillBadEnemy()
    {
        mBadCount++;
        mScore += mBadPoint;
        mScoreLabel.text = mScore.ToString();
    }

    public void KillGoodEnemy()
    {
        mGoodCount++;
        mScore += mGoodPoint;
        mScoreLabel.text = mScore.ToString();
    }

    public void OnReady()
    {
        mState = EPlayState.Ready;
        audio.PlayOneShot(mReadySound);
        Time.timeScale = 1.0f;
    }

    public void OnPlay()
    {
        mState = EPlayState.Play;
        audio.clip = mBGM;
        audio.loop = true;
        audio.Play();
        audio.PlayOneShot(mGoSound);
    }

    public void OnEnd()
    {
        mState = EPlayState.End;
        audio.Stop();
        audio.PlayOneShot(mFinishSound);
        Time.timeScale = 0.0f;
        ShowResultUI();
    }

    public void ShowResultUI()
    {
        mLabelResultScore.text = mScore.ToString();
        mUIResult.SetActive(true);
    }
}
