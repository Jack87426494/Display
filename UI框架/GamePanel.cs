using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    //帽子的动画
    private Animator cupAnimator;

    //游戏结算动画
    private Animator gameSettleAni;

    //提示箭头左
    private GameObject arrowLeftObj;

    //提示箭头右
    private GameObject arrowRightObj;

    //提示箭头开
    private GameObject arrowKaiObj;

    //大棋子按钮
    private Button bigChessPieceBtn;

    //小棋子按钮
    private Button smallChessPieceBtn;

    //单棋子按钮
    private Button singleChessPieceBtn;

    //双棋子按钮
    private Button doubleChesePieceBtn;

    //指针的旋转中心
    private Transform needleCenterTra;

    //开按钮
    private Button openBtn;

    //杯子的图片
    private Image cupImg;

    //骰子放置的位置
    private Transform dicesPos;

    //黑色图层用户遮住底图
    private Image blackImage;

    //开骰合计图层
    private Image totalOpeningImg;

    //开骰子的点数文本
    private TextMeshProUGUI pointTxt;

    //退出游戏按钮
    private Button closeBtn;

    [Header("指针动画持续时间")]
    public float needleDuration = 1f;

    //金钱文本
    private Text moneyTxt;

    //金钱图片
    private Image moneyImg;

    //目前的游戏类型
    private EGameType nowGameType = EGameType.None;

    private Color OneColor = new Color(1, 1, 1, 1);
    private Color zeroColor = new Color(0, 0, 0, 0);

    //可以封装成一个类传出去
    //是否赢了
    private bool isWin;
    //是否赔率翻倍
    private bool isDouble;
    //合计三个骰子的点数
    int dicePointNum = 0;
    //骰子出结果的数字
    private int[] numberArray=new int[3];
  

    //游戏数据(需要存储，之后有成就等其它数据加进来)
    private DiceGameData diceGameData;
    //棋子列表
    private List<ChessData> chessDatasList=new List<ChessData>();

    //数字游戏模式:
    //左边数字按钮
    private Button leftNumBtn;

    //右边数字按钮
    private Button rightNumBtn;

    //数字文字
    private TextMeshProUGUI numberTxt;

    //中间的数字
    private TextMeshProUGUI numberChangeTxt;

    //数字控件组合
    private GameObject numberObj;

    //压数字压得是几
    private int numberSelect;

    //赔率
    private int odds=1;

    private void Start()
    {
        //获取控件
        cupAnimator = transform.Find("cup").gameObject.GetComponent<Animator>();
        arrowLeftObj = transform.Find("arrowLeft").gameObject;
        arrowRightObj = transform.Find("arrowRight").gameObject;
        arrowKaiObj = transform.Find("arrowKaiObj").gameObject;
        needleCenterTra = transform.Find("middlePoint");

        bigChessPieceBtn = GetUiControl<Button>("bigChessPiece");
        smallChessPieceBtn = GetUiControl<Button>("smallChessPiece");
        singleChessPieceBtn = GetUiControl<Button>("singleChessPiece");
        doubleChesePieceBtn = GetUiControl<Button>("doubleChessPiece");

        leftNumBtn = GetUiControl<Button>("leftNumBtn");
        rightNumBtn = GetUiControl<Button>("rightNumBtn");
        numberTxt = GetUiControl<TextMeshProUGUI>("numberTxt");
        numberChangeTxt = GetUiControl<TextMeshProUGUI>("numberChangeTxt");
        numberObj = transform.Find("number").gameObject;

        openBtn = GetUiControl<Button>("openBtn");
        closeBtn = GetUiControl<Button>("closeBtn");

        cupImg = GetUiControl<Image>("cup");
        dicesPos = transform.Find("dicesPos");
        blackImage = GetUiControl<Image>("blackImage");
        moneyImg = GetUiControl<Image>("moneyImg");
        totalOpeningImg = GetUiControl<Image>("totalOpeningImg");
       
        gameSettleAni = transform.Find("gameSettleAni").gameObject.GetComponent<Animator>();

        pointTxt = GetUiControl<TextMeshProUGUI>("pointTxt");
        moneyTxt = GetUiControl<Text>("moneyTxt");


        //注册棋子数据
        AddChessData();
        //得到游戏数据
        diceGameData = GameDataMgr.Instance.GetData<DiceGameData>();
        moneyTxt.text = diceGameData.money.ToString();

        //选择数字模式时为无
        numberChangeTxt.text = "无";
        numberTxt.text = "";

        //先隐藏开骰子的文本
        pointTxt.color = new Color(1,0,0,0);

        //先隐藏开骰子合计图层
        totalOpeningImg.gameObject.GetComponent<CanvasGroup>().DOFade(0f, 0f);

        //在选择游戏类型前不能点击开按钮
        openBtn.interactable = false;
        cupImg.raycastTarget = false;

        //隐藏箭头
        arrowLeftObj.SetActive(false);
        arrowRightObj.SetActive(false);
        arrowKaiObj.SetActive(false);

        //点击事件
        bigChessPieceBtn.onClick.AddListener(() =>
        {
            //选择游戏类型
            SelectGameType(EGameType.Big);
            if (!arrowEndTwo)
            {
                StartCoroutine(IArrowKaiMove());
            }
        });

        smallChessPieceBtn.onClick.AddListener(() =>
        {
            //选择游戏类型
            SelectGameType(EGameType.Small);
            if (!arrowEndTwo)
            {
                StartCoroutine(IArrowKaiMove());
            }
        });

        singleChessPieceBtn.onClick.AddListener(() =>
        {
            //选择游戏类型
            SelectGameType(EGameType.Single);
        });

        doubleChesePieceBtn.onClick.AddListener(() =>
        {
            //选择游戏类型
            SelectGameType(EGameType.Double);
        });

        leftNumBtn.onClick.AddListener(() =>
        {
            if(numberChangeTxt.text=="无" || numberSelect == 3)
            {
                numberSelect = 19;
            }
            --numberSelect;
            SetOddAndText();

            //选择游戏类型
            SelectGameType(EGameType.Number);
        });

        rightNumBtn.onClick.AddListener(() =>
        {
            if (numberChangeTxt.text == "无"||numberSelect==18)
            {
                numberSelect = 2;
            }
            ++numberSelect;
            SetOddAndText();

            //选择游戏类型
            SelectGameType(EGameType.Number);
        });

        openBtn.onClick.AddListener(() =>
        {
            
            if (nowGameType != EGameType.None)
            {
                leftNumBtn.interactable = false;
                rightNumBtn.interactable = false;
                arrowEndTwo = true;

                cupAnimator.SetTrigger("play");
                //避免再次点击按钮
                cupImg.raycastTarget = true;

                ////点击过后暂时不能再点击
                //openBtn.interactable = false;
                //smallChessPieceBtn.interactable = false;
                //bigChessPieceBtn.interactable = false;

                //生成骰子
                GenerateDices();
            }
        });

        closeBtn.onClick.AddListener(() =>
        {
            //清空对象池
            PoolMgr.Instance.Clear();
            //清空ui面板
            UIMgr.Instance.ClearDic();

            //回到主界面
            SceneMgr.Instance.LoadSceneAsync("Main");
        });

        if (GameMgr.Instance.gameIndex==1)
        {
            //第一次进入游戏
            FirstEnterGame();
        }
        else if(GameMgr.Instance.gameIndex==2)
        {
            //第二次进入游戏
            SecondEnterGame();
        }
        
    }

    //第一次进入游戏
    private void FirstEnterGame()
    {
        //moneyImg.gameObject.SetActive(false);
        singleChessPieceBtn.gameObject.SetActive(false);
        doubleChesePieceBtn.gameObject.SetActive(false);
        numberObj.SetActive(false);
        //oneChessPieceBtn.gameObject.SetActive(false);
        //twoChessPieceBtn.gameObject.SetActive(false);
        //threeChessPieceBtn.gameObject.SetActive(false);
        //fourChessPieceBtn.gameObject.SetActive(false);
        //fiveChessPieceBtn.gameObject.SetActive(false);
        //sixChessPieceBtn.gameObject.SetActive(false);
        if(GameMgr.Instance.reIndex==0)
        {
            //加载教学面板
            UIMgr.Instance.ShowPanel<TeachPanel>(false, EPanelLevel.Top);
        }
       
    }

    //第二次进入游戏
    private void SecondEnterGame()
    {
        numberObj.SetActive(false);
        //oneChessPieceBtn.gameObject.SetActive(false);
        //twoChessPieceBtn.gameObject.SetActive(false);
        //threeChessPieceBtn.gameObject.SetActive(false);
        //fourChessPieceBtn.gameObject.SetActive(false);
        //fiveChessPieceBtn.gameObject.SetActive(false);
        //sixChessPieceBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置赔率和文本
    /// </summary>
    private void SetOddAndText()
    {
        numberChangeTxt.text = numberSelect.ToString();
        switch (numberSelect)
        {
            case 3:
            case 18:
                odds = 50;
                break;
            case 4:
            case 17:
                odds = 35;
                break;
            case 5:
            case 16:
                odds = 27;
                break;
            case 6:
            case 15:
                odds = 21;
                break;
            case 7:
            case 14:
                odds = 16;
                break;
            case 8:
            case 13:
                odds = 11;
                break;
            case 9:
            case 12:
                odds = 8;
                break;
            case 10:
            case 11:
                odds = 6;
                break;
        }
        numberTxt.text = "1赔" + odds.ToString();
    }

    // 注册棋子数据
    private void AddChessData()
    {
        chessDatasList.Add(new ChessData(EGameType.Big, bigChessPieceBtn));
        chessDatasList.Add(new ChessData(EGameType.Small, smallChessPieceBtn));

        chessDatasList.Add(new ChessData(EGameType.Single, singleChessPieceBtn));
        chessDatasList.Add(new ChessData(EGameType.Double, doubleChesePieceBtn));

        //chessDatasList.Add(new ChessData(EGameType.Number, oneChessPieceBtn,1));
        //chessDatasList.Add(new ChessData(EGameType.Number, twoChessPieceBtn,2));
        //chessDatasList.Add(new ChessData(EGameType.Number, threeChessPieceBtn,3));
        //chessDatasList.Add(new ChessData(EGameType.Number, fourChessPieceBtn,4));
        //chessDatasList.Add(new ChessData(EGameType.Number, fiveChessPieceBtn,5));
        //chessDatasList.Add(new ChessData(EGameType.Number, sixChessPieceBtn,6));
    }

    //改变棋子的图片样式,和数字样式
    private void ChangeChessImg()
    {
        if(nowGameType!=EGameType.Number)
        {
            //选择数字模式时为无
            numberChangeTxt.text = "无";
            numberTxt.text = "";
            odds = 1;
        }

        foreach (ChessData chessData in chessDatasList)
        {
            chessData.ChangeChess(nowGameType);
        }
    }

    //骰子的位置
    private List<Vector3> diceVec = new List<Vector3> 
    {
        new Vector3(-30,0,0),
        new Vector3(120,0,0),

        new Vector3(50,-50,0),
        new Vector3(50,-161,0),

        new Vector3(0,-100,0),
        new Vector3(100,-100,0),
        
    };

    /// <summary>
    /// 生成骰子
    /// </summary>
    /// <param name="eGameType">游戏类型，根据游戏类型计算分数</param>
    private void GenerateDices()
    {
        int nowDiceNum = 0;
        int nowIndex = 0;

        while (nowDiceNum<3)
        {
            //一个骰子的点数
            numberArray[nowDiceNum] = Random.Range(1, 7);
      
            dicePointNum += numberArray[nowDiceNum];
            //生成骰子
            GameObject diceObj = ResMgr.Instance.Load<GameObject>("Dice");
            //设置图片
            diceObj.GetComponent<Dice>().SetInfo(numberArray[nowDiceNum]);
            //设置父对象
            diceObj.transform.SetParent(dicesPos,false);
            //设置骰子的位置
            //后面再优化先随便写一个
            nowIndex = numberArray[nowDiceNum] % 2 == nowIndex ? nowIndex+1 : nowIndex;
            diceObj.transform.localPosition = diceVec[nowIndex];
            ++nowIndex;
            ++nowDiceNum;
        }

        //计算输赢
        switch (nowGameType)
        {
            case EGameType.Big:

                if(dicePointNum >= 4&& dicePointNum <= 10)
                {
                    isWin = false;
                    isDouble = false;
                }
                else if(dicePointNum > 10&& dicePointNum <= 17)
                {
                    isWin = true;
                    isDouble = false;
                }
                else if(dicePointNum == 3)
                {
                    isWin = false;
                    isDouble = true;
                }
                else if(dicePointNum == 18)
                {
                    isWin = true;
                    isDouble = true;
                }
                break;

            case EGameType.Small:
                if (dicePointNum >= 4 && dicePointNum <= 10)
                {
                    isWin = true;
                    isDouble = false;
                }
                else if (dicePointNum > 10 && dicePointNum <= 17)
                {
                    isWin = false;
                    isDouble = false;
                }
                else if (dicePointNum == 3)
                {
                    isWin = true;
                    isDouble = true;
                }
                else if (dicePointNum == 18)
                {
                    isWin = false;
                    isDouble = true;
                }
                break;

            case EGameType.Single:
                if(dicePointNum==3)
                {
                    isWin = true;
                    isDouble = true;
                }
                else if(dicePointNum==18)
                {
                    isWin = false;
                    isDouble = true;
                }
                else if(dicePointNum%2==1)
                {
                    isDouble = false;
                    isWin = true;
                }
                else
                {
                    isDouble = false;
                    isWin = false;
                }

                break;

            case EGameType.Double:
                if (dicePointNum == 3)
                {
                    isWin = false;
                    isDouble = true;
                }
                else if (dicePointNum == 18)
                {
                    isWin = true;
                    isDouble = true;
                }
                else if (dicePointNum % 2 == 0)
                {
                    isDouble = false;
                    isWin = true;
                }
                else
                {
                    isDouble = false;
                    isWin = false;
                }
                break;

            case EGameType.Number:
                isWin = false;
                isDouble = false;
                //三个骰子点数一样输了赔偿双倍
                if (numberArray[0] == numberArray[1] && numberArray[2] == numberArray[1])
                {
                    isDouble = true;
                }
                else if(dicePointNum==numberSelect)
                {
                    //相同则赢
                    isWin = true;
                }

                break;
        }
    }

    /// <summary>
    /// 游戏结算
    /// </summary>
    public void GameSettle()
    {
        //使用黑檀色图层遮住底图
        blackImage.color = Color.clear;
        blackImage.DOColor(Color.black * 0.7f, 1f).onComplete += () =>
        {
            //显示开骰子合计图层
            totalOpeningImg.gameObject.GetComponent<CanvasGroup>().DOFade(1f, 1f).onComplete =
            () =>
            {
                //显示开骰子点数文本
                switch(dicePointNum)
                {
                    case 3:
                        pointTxt.text = "三点";
                        break;
                    case 4:
                        pointTxt.text = "四点";
                        break;
                    case 5:
                        pointTxt.text = "五点";
                        break;
                    case 6:
                        pointTxt.text = "六点";
                        break;
                    case 7:
                        pointTxt.text = "七点";
                        break;
                    case 8:
                        pointTxt.text = "八点";
                        break;
                    case 9:
                        pointTxt.text = "九点";
                        break;
                    case 10:
                        pointTxt.text = "十点";
                        break;
                    case 11:
                        pointTxt.text = "十一点";
                        break;
                    case 12:
                        pointTxt.text= "十二点";
                        break;
                    case 13:
                        pointTxt.text = "十三点";
                        break;
                    case 14:
                        pointTxt.text = "十四点";
                        break;
                    case 15:
                        pointTxt.text = "十五点";
                        break;
                    case 16:
                        pointTxt.text = "十六点";
                        break;
                    case 17:
                        pointTxt.text = "十七点";
                        break;
                    case 18:
                        pointTxt.text = "十八点";
                        break;
                }
                pointTxt.fontSize = 300f;
                StartCoroutine(IDoFontSize(pointTxt, 55, 1f));
                pointTxt.DOColor(Color.red, 1f).onComplete=
                ()=> 
                {
                    //播放输赢翻倍动画
                    if(isDouble)
                    {
                        gameSettleAni.SetTrigger("zhuang");
                        if(isWin)
                        {
                            diceGameData.money += 20;
                        }
                        else
                        {
                            if (GameMgr.Instance.gameIndex != 1)
                            {
                                diceGameData.money -= 20;
                            }
                            
                        }
                    }
                    else if(isWin)
                    {
                        gameSettleAni.SetTrigger("win");
                        diceGameData.money += 10*odds;
                    }
                    else if(!isWin)
                    {
                        gameSettleAni.SetTrigger("lose");
                        if (GameMgr.Instance.gameIndex != 1)
                        {
                            diceGameData.money -= 10;
                        }
                    }

                    //赔率重置
                    odds = 1;

                    diceGameData.money = Mathf.Max(0, diceGameData.money);
                    moneyTxt.text = diceGameData.money.ToString();
                    //保存游戏数据
                    GameDataMgr.Instance.SaveData<DiceGameData>(diceGameData);
                };
            };
        };
    }

    /// <summary>
    /// 旋转指针
    /// </summary>
    /// <param name="gameType">游戏类型</param>
    /// <param name="callBack">回调函数</param>
    private void RotateNeedle(EGameType gameType,UnityAction callBack)
    {
        switch(gameType)
        {
            case EGameType.Small:
                needleCenterTra.DORotate(new Vector3(0, 0, 135), needleDuration, RotateMode.Fast).onComplete=()=> { callBack(); };
                break;
            case EGameType.Big:
                needleCenterTra.DORotate(new Vector3(0, 0, -45), needleDuration, RotateMode.Fast).onComplete = () => { callBack(); };
                break;
            case EGameType.Single:
                needleCenterTra.DORotate(new Vector3(0, 0, -135), needleDuration, RotateMode.Fast).onComplete = () => { callBack(); };
                break;
            case EGameType.Double:
                needleCenterTra.DORotate(new Vector3(0, 0, 45), needleDuration, RotateMode.Fast).onComplete = () => { callBack(); };
                break;
            case EGameType.Number:
                callBack?.Invoke();
                break;
        }
        
    }

    /// <summary>
    /// 选择游戏类型
    /// </summary>
    /// <param name="gameType">游戏类型</param>
    private void SelectGameType(EGameType gameType)
    {
        arrowEndOne = true;
        //设置游戏类型
        nowGameType = gameType;
        //改变棋子的图片样式
        ChangeChessImg();

        //隐藏箭头
        arrowLeftObj.SetActive(false);
        arrowRightObj.SetActive(false);
        // 旋转指针
        RotateNeedle(gameType, () =>
        {
           
            //选择游戏类型后可以点击开按钮
            openBtn.interactable = true;
        });
    }

    //箭头第一次停止
    private bool arrowEndOne;
    //箭头第二次停止
    private bool arrowEndTwo;

    /// <summary>
    /// 箭头移动
    /// </summary>
    /// <returns></returns>
    public IEnumerator IArrowMove()
    {
        Arrow arrowLeft = arrowLeftObj.GetComponent<Arrow>();
        Image leftImg = arrowLeftObj.GetComponent<Image>();

        Arrow arrowRight = arrowRightObj.GetComponent<Arrow>();
        Image rightImg = arrowRightObj.GetComponent<Image>();

        arrowEndOne = false;
        arrowEndTwo = false;

        while (!arrowEndOne)
        {
            //左
            if(arrowLeftObj!=null)
            {
                arrowLeftObj.SetActive(true);
                leftImg.color = zeroColor;
                leftImg.DOColor(OneColor, 0.2f);
            }
            
            while (arrowLeft.arrowMoveCount < 2)
            {
                yield return new WaitForEndOfFrame();
            }
            arrowLeft.arrowMoveCount = 0;
            leftImg.DOColor(zeroColor, 0.2f).onComplete = () => { arrowLeftObj.SetActive(false); };
            //arrowLeftObj.SetActive(false);

            //消除残影
            yield return new WaitForSeconds(0.4f);

            //右
            if(arrowRightObj!=null)
            {
                arrowRightObj.SetActive(true);
                rightImg.color = zeroColor;
                rightImg.DOColor(OneColor, 0.2f);
            }
            
            while (arrowRight.arrowMoveCount < 2)
            {
                yield return new WaitForEndOfFrame();
            }
            arrowRight.arrowMoveCount = 0;
            rightImg.DOColor(zeroColor, 0.2f).onComplete = () => { arrowRightObj.SetActive(false); };
            //arrowRightObj.SetActive(false);

            //消除残影
            yield return new WaitForSeconds(0.4f);
        }

        //UIMgr.Instance.HidePanel<TeachPanel>();
        //StopCoroutine(IArrowMove());
    }

    private IEnumerator IArrowKaiMove()
    {
        arrowKaiObj.SetActive(true);
        arrowKaiObj.GetComponent<CanvasGroup>().DOFade(1f, 0.4f);
        while(!arrowEndTwo)
        {
            //消除残影
            yield return null;
        }
        arrowKaiObj.GetComponent<CanvasGroup>().DOFade(0f, 0.4f).onComplete =
            () =>
            {
                arrowKaiObj.SetActive(false);
            };
    }

    /// <summary>
    /// 改变字体大小
    /// </summary>
    /// <param name="txt">字体</param>
    /// <param name="targetSize">目标大小</param>
    /// <param name="duration">变化持续的时间</param>
    /// <returns></returns>
    private IEnumerator IDoFontSize(TextMeshProUGUI txt, float targetSize, float duration)
    {
        float time = 0;
        float nowSize = txt.fontSize;
        while (time < duration)
        {
            yield return null;
            txt.fontSize = (int)Mathf.Lerp(nowSize, targetSize, time / duration);
            time += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        //赔率重置
        odds = 1;
        //不启用箭头提示
        arrowEndOne = true;
        arrowEndTwo = true;
    }
    private void OnDestroy()
    {
        //赔率重置
        odds = 1;
    }
}