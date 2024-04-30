using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CafeClasses;


public class MenuScript : MonoBehaviour
{
    //===========変数宣言・定義===========================
        public GameObject canvas;
        public GameObject ButtonPrefab,GenrePrefab,CanvasPanel;
            
    //===============ver5===============
        public RectTransform discountRect;
        public GameObject discAppButtPrefab,ScrollSetPrefab,ScrollMenuPrefab,discountPanel,discAppPanel,discAppContent,SetPanel,scrollView,ScrollViewContent,genreScrollView;
        public Button discDetButton;
        public Text SelectText,SetPanelText,SetNumText;
        int Ndisc;
        int appNum;
        List<List<int>> appedNum = new List<List<int>>();
        List<GameObject> GOdiscButtons = new List<GameObject>();
        List<GameObject> GOdiscAppButtons = new List<GameObject>();
        List<List<Text[]>> discNumText = new List<List<Text[]>>();
        List<Button> discButtons = new List<Button>();
        List<Button> discAppButtons = new List<Button>();
        RectTransform DiscRect;
        Text[] discButtText = new Text[3];
    //===================================
    
        //private int[,] A;
        private current_struct Data = new current_struct();
        private current_struct.OrderStruct Order = new current_struct.OrderStruct();
        private int CurrentSet;
        private float SumPrice,SetPrice;
        private List<GameObject> genres = new List<GameObject>();
        private List<GameObject> GOgenreButtons = new List<GameObject>();
        private List<Button> genreButtons = new List<Button>();
        private List<RectTransform> genreRect = new List<RectTransform>();
        private string TableName,ScrollSetText;
        List<GameObject> GObuttons = new List<GameObject>();
        List<Button> buttons = new List<Button>();
        RectTransform MenuRect,CanvasRect;
        Text[] buttonText=new Text[2];
        public Text TitleText;
        public Text MenuText;
        public Text PriceText;
        GameObject Table;
        TableScript table;
        bool NextSetFrag = true , OrderdFrag = false , ReviseFrag = false;



    public void DetClick(){
        //注文1回目かつ無注文じゃないとき.
        if((Data.OrderData[Data.Num].Number==0) && (OrderdFrag)){
            Data.LastNum +=1;
            Data.OrderData[Data.Num].Number = Data.LastNum;
        }
        
        if(NextSetFrag){
            Data.OrderData[Data.Num].Nset --;
        }

        table.Data = Data;

        table.canvas.SetActive(true);
        SceneManager.LoadScene("TableScene");
    }
    public void CancelClick(){
        table.canvas.SetActive(true);
        SceneManager.LoadScene("TableScene");
    }
    public void ResetClick(){
        Data.copy(table.Data);
        Order.init(Data.Nmenu,Data.Ndisc);
        CurrentSet = Data.OrderData[Data.Num].Nset;
        Data.OrderData[Data.Num].AddSet(Order);
        NextSetFrag = true;
        updateDisplay();            
    }

    void buttonClick(int num)
    {
        OrderdFrag = true;
        //割引なしの注文にプラス1
        Order.Order[num][0] +=1;
        Data.OrderData[Data.Num].SetOrder[CurrentSet].copy(Order);
        
        updateDisplay();
        if(ReviseFrag){
            SetPanelText.text = (CurrentSet+1).ToString()+"人目の追加注文を続けますか";
        }
        else{
            if(NextSetFrag) NextSetFrag = false;
            SetPanelText.text = (CurrentSet+1).ToString()+"人目の注文を続けますか";
        }
        SetPanel.SetActive(true);
    }
    void DelButtonClick(int set,int num,int disc){
        Data.OrderData[Data.Num].SetOrder[set].Order[num][disc] = 0;
        Order.copy(Data.OrderData[Data.Num].SetOrder[CurrentSet]);
        HeadClick();
        HeadClick();
    }


    public void SetContinue(){
        SetPanel.SetActive(false);
        updateDisplay();
    }
    public void SetFin(){
        SetPanel.SetActive(false);

        if(ReviseFrag){
            CurrentSet = Data.OrderData[Data.Num].Nset-1;
            Order.copy(Data.OrderData[Data.Num].SetOrder[CurrentSet]);
            ReviseFrag = false;
        }
        else{
            //=======次のセットの準備・初期化=========
            NextSetFrag = true;
            Order.init(Data.Nmenu,Data.Ndisc);
            CurrentSet = Data.OrderData[Data.Num].Nset;
            Data.OrderData[Data.Num].AddSet(Order);//ここでNsetが更新されている点に注意.
        }
        SetNumText.text = (CurrentSet+1).ToString()+"人目\n\n注文中";
        updateDisplay();
    }
    void ReAddClick(int set){
        CurrentSet = set;
        ReviseFrag = true;
        SetNumText.text = (CurrentSet+1).ToString()+"人目\n\n追加\n注文中";
        Order.copy(Data.OrderData[Data.Num].SetOrder[CurrentSet]);
        HeadClick();
    }

    void genreClick(int num){
        for(int i=0;i<Data.Ngenre;i++){
            if(i == num){
                genres[i].SetActive(true);
                genreRect[i].transform.localScale = new Vector3(1.5f,1.5f,1);
                genreRect[i].anchoredPosition = new Vector3(90*i-500,185-75*1.5f,0);
            }
            else{
                genres[i].SetActive(false);
                genreRect[i].transform.localScale = new Vector3(1,1,1);
                genreRect[i].anchoredPosition = new Vector3(90*i-500,185-75,0);
            }
        }
        //===========ver5==========
        discountPanel.SetActive(false);
        discountRect.transform.localScale = new Vector3(1,1,1);
        discountRect.anchoredPosition = new Vector3(90*Data.Ngenre-500,185-75,0);
    }
    
    //==============割引関連ボタン==============
    public void discountClick(){
        for(int i=0;i<Data.Ngenre;i++){
            genres[i].SetActive(false);
            genreRect[i].transform.localScale = new Vector3(1,1,1);
            genreRect[i].anchoredPosition = new Vector3(90*i-500,185-75,0);
        }

        discountPanel.SetActive(true);
        discountRect.transform.localScale = new Vector3(1.5f,1.5f,1);
        discountRect.anchoredPosition = new Vector3(90*Data.Ngenre-500,185-75*1.5f,0);
    }
    
    void discButtClick(int num){
        SelectText.text = Data.DiscName[num]+"を適用する注文を選択";
        GOdiscAppButtons.Clear();
        discAppButtons.Clear();
        discNumText.Clear();
        appedNum.Clear();
        appNum=0;

        discDetButton.onClick.RemoveAllListeners();
        discDetButton.onClick.AddListener(() => discDetClick(num));

        for(int set=0;set<Data.OrderData[Data.Num].Nset;set++){
            appedNum.Add(new List<int>());
            discNumText.Add(new List<Text[]>());
            for(int i=0;i<Data.Nmenu;i++){
                appedNum[set].Add(0);
                discNumText[set].Add(new Text[5]);
                if(Data.OrderData[Data.Num].SetOrder[set].Order[i][0]!=0){

                    int j = i;
                    int jset = set;
                    GOdiscAppButtons.Add(Instantiate(discAppButtPrefab, canvas.transform));
                    GOdiscAppButtons[appNum].transform.parent = discAppContent.transform;
                    discAppButtons.Add(GOdiscAppButtons[appNum].GetComponent<Button> ());
                    discAppButtons[appNum].onClick.AddListener(() => discAppClick(jset,j)); 
                    MenuRect = GOdiscAppButtons[appNum].GetComponent<RectTransform> ();
                    MenuRect.anchoredPosition = new Vector3(0,850-170*appNum,0);
                    MenuRect.localScale = new Vector3(1,1,1);
                    discNumText[set][i]=GOdiscAppButtons[appNum].GetComponentsInChildren<Text> ();
                    discNumText[set][i][0].text = Data.MenuName[i]+"("+(set+1).ToString()+"人目)";
                    discNumText[set][i][1].text = Data.MenuPrice[i].ToString();
                    discNumText[set][i][3].text = appedNum[set][i].ToString();
                    discNumText[set][i][4].text = "✕      /" + Data.OrderData[Data.Num].SetOrder[set].Order[i][0].ToString();
                    buttonText[0].text = Data.MenuName[i];
                    buttonText[1].text = Data.MenuPrice[i].ToString();

                    appNum++;
                }
            }

        }
        discAppPanel.SetActive(true);
    }
    public void resetDiscButt(){
        for(int set=0;set<Data.OrderData[Data.Num].Nset;set++){
            for(int i=1;i<Data.Ndisc+1;i++){
                for(int j=0;j<Data.Nmenu;j++){
                    Data.OrderData[Data.Num].SetOrder[set].Order[j][0]+=Data.OrderData[Data.Num].SetOrder[set].Order[j][i];
                    Data.OrderData[Data.Num].SetOrder[set].Order[j][i]=0;
                }
            }
        }
        updateDisplay();
    }
    void discDetClick(int num){
        for(int set=0;set<Data.OrderData[Data.Num].Nset;set++){
            for(int i=0;i<Data.Nmenu;i++){
                Data.OrderData[Data.Num].SetOrder[set].Order[i][num+1]+=appedNum[set][i];
                Data.OrderData[Data.Num].SetOrder[set].Order[i][0]-=appedNum[set][i];
            } 
        }
        for(int i=0;i<appNum;i++){
            Destroy(GOdiscAppButtons[i]);
        }

        discAppPanel.SetActive(false);
        updateDisplay();
    }
    void discAppClick(int set,int j){
        
        if(appedNum[set][j]<Data.OrderData[Data.Num].SetOrder[set].Order[j][0]){
            appedNum[set][j]++;
        }
        else{
            appedNum[set][j]=0;
        }
        discNumText[set][j][3].text = appedNum[set][j].ToString();

    }
    //=================================-

    void updateDisplay(){

        MenuText.text="";
        SumPrice=0;
        for(int set=0;set<Data.OrderData[Data.Num].Nset;set++){
            SetPrice=0;
            for(int j=0;j<Data.Nmenu;j+=1){
                //割り引きなしの注文.
                if(Data.OrderData[Data.Num].SetOrder[set].Order[j][0]!=0){
                    MenuText.text += Data.MenuName[j] + "✕" + Data.OrderData[Data.Num].SetOrder[set].Order[j][0].ToString() + " ";
                    SetPrice += Data.MenuPrice[j]*Data.OrderData[Data.Num].SetOrder[set].Order[j][0];
                }
                //割り引きありの注文.
                for(int k=1;k<Data.Ndisc+1;k+=1){
                    if(Data.OrderData[Data.Num].SetOrder[set].Order[j][k]!=0){
                        MenuText.text += Data.MenuName[j] +"("+ Data.DiscName[k-1] + ") ✕" + Data.OrderData[Data.Num].SetOrder[set].Order[j][k] + "  ";
                        SetPrice += Data.MenuPrice[j]*Data.OrderData[Data.Num].SetOrder[set].Order[j][k]*(10-Data.DiscValue[k-1])/10;
                    }
                }
            }
            SumPrice += SetPrice;
        }
        PriceText.text = SumPrice.ToString();
    }
    public void HeadClick(){

        if(scrollView.activeSelf){
            scrollView.SetActive(false);
            foreach (Transform child in ScrollViewContent.transform){
                GameObject.Destroy(child.gameObject);
            }
        }
        else{
            float length = 0.0f;
            for(int set=0;set<Data.OrderData[Data.Num].Nset;set++){
                //☑ScrollSetPrefab追加 
                //☑文字（○人目）編集　
                //☐PrefabのSelectボタン挙動定義
                //☐Nsetのときは追加注文ボタンナシ
                int L = set;
                int SetPrice = 0;

                GameObject GOScrollSet = Instantiate(ScrollSetPrefab, this.canvas.transform);
                GameObject GOAddButton = GOScrollSet.transform.Find("AddMenuButton").gameObject;
                GOScrollSet.transform.parent = ScrollViewContent.transform;
                Button AddButton = GOAddButton.GetComponent<Button>();

                AddButton.onClick.AddListener(() => ReAddClick(L));
                if(set == Data.OrderData[Data.Num].Nset-1){
                    GameObject.Destroy(GOAddButton);
                }
                
                RectTransform SetRect = GOScrollSet.GetComponent<RectTransform>();
                Text Numtext = GOScrollSet.transform.Find("SetNum").GetComponent<Text>();
                Text Pricetext = GOScrollSet.transform.Find("SetPrice").GetComponent<Text>();
                SetRect.anchoredPosition = new Vector3(0,900-60*length,0);
                SetRect.localScale = new Vector3(1,1,1);
                Numtext.text = (set+1).ToString()+"人目";
                
                length += 1.0f;


                for(int j=0;j<Data.Nmenu;j+=1){
                    for(int k=0;k<Data.Ndisc+1;k+=1){
                        int menu = j;
                        int disc = k;
                        if(Data.OrderData[Data.Num].SetOrder[set].Order[j][k]!=0){
                            GameObject GOScrollMenu = Instantiate(ScrollMenuPrefab, this.canvas.transform);
                            GameObject GODelButton = GOScrollMenu.transform.Find("DelButton").gameObject;
                            GOScrollMenu.transform.parent = ScrollViewContent.transform;
                            Button DelButton = GODelButton.GetComponent<Button>();

                            DelButton.onClick.AddListener(() => DelButtonClick(L,menu,disc));

                            RectTransform SetMenuRect = GOScrollMenu.GetComponent<RectTransform>();
                            Text Menutext = GOScrollMenu.transform.Find("SetMenu").GetComponent<Text>();
                            SetMenuRect.anchoredPosition = new Vector3(0,900-60*length,0);
                            SetMenuRect.localScale = new Vector3(1,1,1);
                
                            length += 1.0f;
                            
                            //割り引きナシ
                            if( k==0 ) {
                                Menutext.text = Data.MenuName[j] + "✕" + Data.OrderData[Data.Num].SetOrder[set].Order[j][k];
                                SetPrice += Data.MenuPrice[j]*Data.OrderData[Data.Num].SetOrder[set].Order[j][k];
                            }
                            //割り引きアリ
                            else{
                                Menutext.text = Data.MenuName[j] +"("+ Data.DiscName[k-1] + ") ✕" + Data.OrderData[Data.Num].SetOrder[set].Order[j][k];
                                SetPrice += Data.MenuPrice[j]*Data.OrderData[Data.Num].SetOrder[set].Order[j][k]*(10-Data.DiscValue[k-1])/10; 
                            }
                        }
                    }
                }
                length += 0.5f;

                Pricetext.text = SetPrice.ToString()+"円";
            }
            scrollView.SetActive(true);
        } 

    }


    // Start is called before the first frame update
    void Start()
    {
        //====================ClassScriptsの読み込み=====================--
        //========================tableスクリプトの読み込み======================
            Table=GameObject.Find("GOTableScript");
            table=Table.GetComponent<TableScript>();


            Data.copy(table.Data);;
            TableName = Data.TableName[Data.Num];
            Order.init(Data.Nmenu,Data.Ndisc);
            CurrentSet = Data.OrderData[Data.Num].Nset;
            Data.OrderData[Data.Num].AddSet(Order);//ここでNsetが+1されている点に注意.
            SetNumText.text = (CurrentSet+1).ToString()+"人目\n\n注文中";

        //==========================Canvasの編集==================================
            CanvasRect = CanvasPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = Data.CanPosition;
            CanvasRect.localScale = Data.CanScale;

        //==========================テキストの編集================================
            TitleText.text="注文 ("+TableName+"席)";
            updateDisplay();

        //==========================ジャンルの編集================================
            for(int i=0;i<Data.Ngenre;i++){
                int j = i;
                genres.Add(new GameObject("Genre"+i.ToString()));
                genres[i].transform.parent = CanvasPanel.transform;
                genres[i].transform.localPosition = new Vector3(0,0,0);
                genres[i].transform.localScale = new Vector3(1,1,1);

                GOgenreButtons.Add(Instantiate(GenrePrefab, canvas.transform));
                GOgenreButtons[i].transform.parent = genreScrollView.transform;
                genreButtons.Add(GOgenreButtons[i].GetComponent<Button> ());
                genreButtons[i].onClick.AddListener(() => genreClick(j)); 
                genreRect.Add(GOgenreButtons[i].GetComponent<RectTransform> ());
                buttonText = GOgenreButtons[i].GetComponentsInChildren<Text> ();
                buttonText[0].text = Data.GenreName[i];
            }
            genreClick(0);

        //==========================メニューの編集==================================
                    
            for(int i=0;i<Data.Nmenu;i++){
                int j = i;
                GObuttons.Add(Instantiate(ButtonPrefab, canvas.transform));
                GObuttons[i].transform.parent = genres[Data.MenuGenre[i]].transform;
                buttons.Add(GObuttons[i].GetComponent<Button> ());
                buttons[i].onClick.AddListener(() => buttonClick(j)); 
                MenuRect = GObuttons[i].GetComponent<RectTransform> ();
                buttonText = GObuttons[i].GetComponentsInChildren<Text> (); 
                MenuRect.anchoredPosition = Data.MenuVect[i];
                MenuRect.localScale = new Vector3(1,1,1);
                buttonText[0].text = Data.MenuName[i];
                buttonText[1].text = Data.MenuPrice[i].ToString();
            }
        //==========================割引の編集==========================================        
            
                    
            for(int i=0;i<Data.Ndisc;i++){
                int j = i;

                GOdiscButtons.Add(Instantiate(ButtonPrefab, canvas.transform));
                GOdiscButtons[i].transform.parent = discountPanel.transform;
                discButtons.Add(GOdiscButtons[i].GetComponent<Button> ());
                discButtons[i].onClick.AddListener(() => discButtClick(j)); 
                DiscRect = GOdiscButtons[i].GetComponent<RectTransform> ();
                discButtText = GOdiscButtons[i].GetComponentsInChildren<Text> (); 
                DiscRect.anchoredPosition = Data.DiscVect[i];
                DiscRect.localScale = new Vector3(1,1,1);
                discButtText[0].text = Data.DiscName[i];
                discButtText[1].text = Data.DiscValue[i].ToString();
                discButtText[2].text = "割引";
            }
            scrollView.GetComponent<RectTransform>().SetAsLastSibling();
        //========================================================================
    }
    // Update is called once per frame
    void Update()
    {
    }
}
