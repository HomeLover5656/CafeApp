using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuSetScript : MonoBehaviour
{
    //それ以外========================================
    public GameObject canvas,ButtonPrefab,GenrePrefab,GOInputField,GOInputGenreField,CanvasPanel,genreScrollView;
    public Text MenuWord,PriceWord,YenWord;
    public InputField NameField,PriceField,GenreField;
    public Button inputButton,DelButton,genreNameButton,genreDelButton;
    
    //===============ver5===============
    public RectTransform discountRect;
    public GameObject discountPanel;
    int Ndisc,AddDiscCount=0;
    List<GameObject> GOdiscButtons = new List<GameObject>();
    List<Button> discButtons = new List<Button>();
    List<EventTrigger> discTrigger = new List<EventTrigger>();
    List<EventTrigger.Entry> discEntryDrag = new List<EventTrigger.Entry>();
    List<RectTransform> DiscRect = new List<RectTransform>();
    List<Text[]> discButtText = new List<Text[]>();
    //===================================

    
    List<GameObject> genres = new List<GameObject>();
    List<GameObject> GOgenreButtons = new List<GameObject>();
    List<Button> genreButtons = new List<Button>();
    List<RectTransform> genreRect = new List<RectTransform>();
    List<Text[]> genreText = new List<Text[]>();
    
    List<GameObject> GObuttons = new List<GameObject>();
    List<Button> buttons = new List<Button>();
    List<EventTrigger> trigger = new List<EventTrigger>();
    List<EventTrigger.Entry> entryDrag = new List<EventTrigger.Entry>();
    List<RectTransform> MenuRect = new List<RectTransform>();
    List<Text[]> buttonText = new List<Text[]>();
    List<int> MenuGenre = new List<int>();
    
    
    GameObject Table;
    TableScript table;
    int Nmenu,Ngenre,nowGenre,AddCount=0;
    bool NoDrag=false;
    List<int> DelNum = new List<int>();
    List<int> DelGenre = new List<int>();
    List<int> DelDiscNum = new List<int>();

    RectTransform CanvasRect;
    

 
    //ボタン関係======================================================

    void buttonClick(int num)
    {
        if(NoDrag){
            int j=num;
            //名前入力
            inputButton.onClick.RemoveAllListeners();
            inputButton.onClick.AddListener(() => inputName(j));
            DelButton.onClick.RemoveAllListeners();
            DelButton.onClick.AddListener(() => inputDel(j));
            GOInputField.SetActive(true);
            MenuWord.text = "メニュー名を入力";
            NameField.text = buttonText[num][0].text;
            PriceWord.text = "値段を入力";
            PriceField.text = buttonText[num][1].text;
            YenWord.text = "円";
        }else{
            NoDrag=true;
        }
        
    }
    void buttonDrag(int num)
    {
        if(NoDrag) NoDrag=false;
        GObuttons[num].transform.position=Input.mousePosition;
    }
    void inputName(int num){
        //inputFieldの情報をbuttonText[][].textに入れる。
        buttonText[num][0].text = NameField.text;
        buttonText[num][1].text = PriceField.text;
        NameField.text = "";
        PriceField.text = "";
        GOInputField.SetActive(false);
    }
    public void inputReturn(){
        NameField.text = "";
        PriceField.text = "";
        GOInputField.SetActive(false);
    }
    public void inputDel(int num){
        //ListのRemoveはList番号が変わってめんどくさいからSaveでまとめてする。

        //削除するボタンの番号を保持
        DelNum.Add(num);
        //ボタンオブジェクトを非表示
        GObuttons[num].SetActive(false);

        GOInputField.SetActive(false);
    }


    public void AddClick()
    {
        AddCount+=1;
        Nmenu+=1;
        //GObuttons[]とbuttons[]の要素追加、インスタンス代入

        int j=Nmenu-1;
                
        GObuttons.Add(Instantiate(ButtonPrefab, canvas.transform));
        MenuGenre.Add(nowGenre);
        GObuttons[j].transform.parent = genres[nowGenre].transform;
        buttons.Add(GObuttons[j].GetComponent<Button> ());
        buttons[j].onClick.AddListener(() => buttonClick(j)); 
        GObuttons[j].AddComponent<EventTrigger>();
        trigger.Add(GObuttons[j].GetComponent<EventTrigger>());
        entryDrag.Add(new EventTrigger.Entry());
        entryDrag[j].eventID = EventTriggerType.Drag;
        entryDrag[j].callback.AddListener((eventDate) => { buttonDrag(j); });
        trigger[j].triggers.Add(entryDrag[j]);
        MenuRect.Add(GObuttons[j].GetComponent<RectTransform> ());
        MenuRect[j].localScale = new Vector3(1,1,1);
        buttonText.Add(GObuttons[j].GetComponentsInChildren<Text> ());
        buttonText[j][0].text="メニュー"+(j+1).ToString();
        
        GOInputField.GetComponent<RectTransform>().SetAsLastSibling();
    }
    public void AddGenreClick()
    {
        Ngenre = Ngenre+1;
        int j=Ngenre-1;
                
        genres.Add(new GameObject("Genre"+j.ToString()));
        genres[j].transform.parent = CanvasPanel.transform;
        genres[j].transform.localPosition = new Vector3(0,0,0);
        genres[j].transform.localScale = new Vector3(1,1,1);

        GOgenreButtons.Add(Instantiate(GenrePrefab, canvas.transform));
        GOgenreButtons[j].transform.parent = genreScrollView.transform;
        genreButtons.Add(GOgenreButtons[j].GetComponent<Button> ());
        genreButtons[j].onClick.AddListener(() => genreClick(j)); 
        genreRect.Add(GOgenreButtons[j].GetComponent<RectTransform> ());
        genreRect[j].transform.localScale = new Vector3(1,1,1);
        genreText.Add(GOgenreButtons[j].GetComponentsInChildren<Text> ());
        genreRect[j].anchoredPosition = new Vector3(90*j-500,185-75,0);
        genreText[j][0].text = "ジャンル"+(j+1).ToString();
        
        discountRect.anchoredPosition = new Vector3(90*Ngenre-500,185-75,0);

        GOInputField.GetComponent<RectTransform>().SetAsLastSibling();
        GOInputGenreField.GetComponent<RectTransform>().SetAsLastSibling();

    }
    public void inputGenreName(int num){
        //inputFieldの情報をgenreText[].textに入れる。
        genreText[num][0].text = GenreField.text;
        GenreField.text = "";
        GOInputGenreField.SetActive(false);
    }
    void DelGenreClick(int num)
    {
        //削除するジャンルの番号を保持
        DelGenre.Add(num);
        //オブジェクトを非表示
        genres[num].SetActive(false);
        GOgenreButtons[num].SetActive(false);
        GOInputGenreField.SetActive(false);

    }
    void genreClick(int num){
        nowGenre = num;
        for(int i=0;i<Ngenre;i++){
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
        discountRect.anchoredPosition = new Vector3(90*Ngenre-500,185-75,0);


        //名前入力
        genreNameButton.onClick.RemoveAllListeners();
        genreNameButton.onClick.AddListener(() => inputGenreName(num));
        genreDelButton.onClick.RemoveAllListeners();
        genreDelButton.onClick.AddListener(()=> DelGenreClick(num));
        GOInputGenreField.SetActive(true);
        GenreField.text = genreText[num][0].text;
    }

    //==============ver5==============
    public void discountClick(){
        nowGenre = -1;
        for(int i=0;i<Ngenre;i++){
            genres[i].SetActive(false);
            genreRect[i].transform.localScale = new Vector3(1,1,1);
            genreRect[i].anchoredPosition = new Vector3(90*i-500,185-75,0);
        }

        discountPanel.SetActive(true);
        discountRect.transform.localScale = new Vector3(1.5f,1.5f,1);
        discountRect.anchoredPosition = new Vector3(90*Ngenre-500,185-75*1.5f,0);
        

    }
    void discButtClick(int num){
        if(NoDrag){
            int j=num;
            //名前入力
            inputButton.onClick.RemoveAllListeners();
            inputButton.onClick.AddListener(() => inputDiscName(j));
            DelButton.onClick.RemoveAllListeners();
            DelButton.onClick.AddListener(() => inputDiscDel(j));
            GOInputField.SetActive(true);
            MenuWord.text = "割引名を入力";
            NameField.text = discButtText[num][0].text;
            PriceWord.text = "割引率を入力";
            PriceField.text = discButtText[num][1].text;
            YenWord.text = "割引";

        }else{
            NoDrag=true;
        }
    }
    void discButtDrag(int num){
        if(NoDrag) NoDrag=false;
        GOdiscButtons[num].transform.position=Input.mousePosition;
    }
    void inputDiscName(int num){
        //inputFieldの情報をdiscButtText[][].textに入れる。
        discButtText[num][0].text = NameField.text;
        discButtText[num][1].text = PriceField.text;
        NameField.text = "";
        PriceField.text = "";
        GOInputField.SetActive(false);
    }
    public void inputDiscDel(int num){
        //削除するボタンの番号を保持
        DelDiscNum.Add(num);
        //ボタンオブジェクトを非表示
        GOdiscButtons[num].SetActive(false);

        GOInputField.SetActive(false);
    }
    public void AddDiscClick()
    {
        AddDiscCount+=1;
        Ndisc+=1;
        //GOdiscButtons[]とdiscButtons[]の要素追加、インスタンス代入

        int j=Ndisc-1;
                
        GOdiscButtons.Add(Instantiate(ButtonPrefab, canvas.transform));
        GOdiscButtons[j].transform.parent = discountPanel.transform;
        discButtons.Add(GOdiscButtons[j].GetComponent<Button> ());
        discButtons[j].onClick.AddListener(() => discButtClick(j)); 
        GOdiscButtons[j].AddComponent<EventTrigger>();
        discTrigger.Add(GOdiscButtons[j].GetComponent<EventTrigger>());
        discEntryDrag.Add(new EventTrigger.Entry());
        discEntryDrag[j].eventID = EventTriggerType.Drag;
        discEntryDrag[j].callback.AddListener((eventDate) => { discButtDrag(j); });
        discTrigger[j].triggers.Add(discEntryDrag[j]);
        DiscRect.Add(GOdiscButtons[j].GetComponent<RectTransform> ());
        DiscRect[j].localScale = new Vector3(1,1,1);
        discButtText.Add(GOdiscButtons[j].GetComponentsInChildren<Text> ());
        discButtText[j][0].text="割引"+(j+1).ToString();
        discButtText[j][1].text="5";
        discButtText[j][2].text="割引";
        
        GOInputField.GetComponent<RectTransform>().SetAsLastSibling();
    }


    public void FinClick()
    {
        Destroy (Table);
        SceneManager.LoadScene("TableScene");
    }
    public void ResetClick()
    {
        //再読込で良さそう
        SceneManager.LoadScene("MenuSetScene");
    }
    public void SaveClick(){

        //ボタンの追加の反映（TableScriptのOrderData）==========================================
        for(int i=0;i<table.Data.Ntable;i++){
            for(int set=0;set<table.Data.OrderData[i].Nset;set++){
                for(int j=0; j<AddCount;j++){
                    //table.Data.A にリスト要素追加＆代入
                    //for(int k=0;k<table.Data.Ndisc+1;k++) table.Data.A[k][i].Add(0);
                    table.Data.OrderData[i].SetOrder[set].Order.Add(new List<int>());
                    for(int k=0;k<table.Data.Ndisc+1;k++) {
                        table.Data.OrderData[i].SetOrder[set].Order[table.Data.Nmenu+j].Add(0);
                    }
                }
            }    
        }

        //ボタンの削除の反映（このスクリプト内のListと、TableScriptのA[][]）==========================================
        int DelCount;
        DelCount=DelNum.Count;
        if(DelCount != 0){
            //List要素の削除（番号変わらないように、大きい順に）
            DelNum.Sort();
            Nmenu=Nmenu-DelCount;
            for(int i=DelCount-1;i>=0;i--){
                    
                buttonText.RemoveAt(DelNum[i]);
                MenuGenre.RemoveAt(DelNum[i]);
                MenuRect.RemoveAt(DelNum[i]);
                entryDrag.RemoveAt(DelNum[i]);
                trigger.RemoveAt(DelNum[i]);
                buttons.RemoveAt(DelNum[i]);
                GObuttons.RemoveAt(DelNum[i]);

                for(int j=0;j<table.Data.Ntable;j++){
                    for(int set=0;set<table.Data.OrderData[j].Nset;set++){
                        table.Data.OrderData[j].SetOrder[set].Order.RemoveAt(DelNum[i]);
                    }
                }
                
            }
        }

        //割引の追加の反映=================================-
        for(int i=0;i<table.Data.Ntable;i++){
            for(int set=0;set<table.Data.OrderData[i].Nset;set++){
                for(int j=0;j<table.Data.Nmenu;j++){
                    for(int k=0;k<AddDiscCount;k++){
                        table.Data.OrderData[i].SetOrder[set].Order[j].Add(0);
                    }
                }
            }
        }
        //割引の削除の反映==================================-
        DelCount=DelDiscNum.Count;
        if(DelCount != 0){
            //List要素の削除（番号変わらないように、大きい順に）
            DelDiscNum.Sort();
            Ndisc=Ndisc-DelCount;
            for(int i=DelCount-1;i>=0;i--){
                    
                discButtText.RemoveAt(DelDiscNum[i]);
                DiscRect.RemoveAt(DelDiscNum[i]);
                discEntryDrag.RemoveAt(DelDiscNum[i]);
                discTrigger.RemoveAt(DelDiscNum[i]);
                discButtons.RemoveAt(DelDiscNum[i]);
                GOdiscButtons.RemoveAt(DelDiscNum[i]);

                for(int l=0;l<table.Data.Ntable;l++){
                    for(int set=0;set<table.Data.OrderData[l].Nset;set++){
                        for(int j=0;j<table.Data.Nmenu;j++){
                                table.Data.OrderData[l].SetOrder[set].Order[j].RemoveAt(DelDiscNum[i]);
                        }
                    }
                }

            }
        }

        //ジャンルの削除の反映=======================================--
        DelCount = DelGenre.Count;
        if(DelCount !=0){
            DelGenre.Sort();
            Ngenre=Ngenre-DelCount;
            for(int i=DelCount-1;i>=0;i--){
                genreText.RemoveAt(DelGenre[i]);
                genreButtons.RemoveAt(DelGenre[i]);
                GOgenreButtons.RemoveAt(DelGenre[i]);

                for(int j=0;j<Nmenu;j++){ 
                    if(MenuGenre[j]==DelGenre[i]){
                        buttonText.RemoveAt(j);
                        MenuGenre.RemoveAt(j);
                        MenuRect.RemoveAt(j);
                        entryDrag.RemoveAt(j);
                        trigger.RemoveAt(j);
                        buttons.RemoveAt(j);
                        GObuttons.RemoveAt(j);
                        j-=1;
                        Nmenu-=1;
                    }
                    else if(MenuGenre[j]>DelGenre[i]){
                        MenuGenre[j]-=1;
                    }
                }
            }
        }
        
        
        // 残りの情報をTableScriptに反映========================================
        table.Data.Nmenu=Nmenu;
        table.Data.Ngenre=Ngenre;
        table.Data.Ndisc=Ndisc;
        table.Data.MenuName.Clear();
        table.Data.MenuPrice.Clear();
        table.Data.MenuGenre.Clear();
        table.Data.GenreName.Clear();
        table.Data.MenuVect.Clear();
        table.Data.DiscName.Clear();
        table.Data.DiscValue.Clear();
        table.Data.DiscVect.Clear();

        for(int i=0;i<Nmenu;i++){
            table.Data.MenuName.Add(buttonText[i][0].text);
            table.Data.MenuPrice.Add(int.Parse(buttonText[i][1].text));
            table.Data.MenuGenre.Add(MenuGenre[i]);
            table.Data.MenuVect.Add(MenuRect[i].anchoredPosition); 
        }

        for(int i=0;i<Ndisc;i++){
            table.Data.DiscName.Add(discButtText[i][0].text);
            table.Data.DiscValue.Add(int.Parse(discButtText[i][1].text));
            table.Data.DiscVect.Add(DiscRect[i].anchoredPosition);
        }

        for(int i=0;i<Ngenre;i++){
            table.Data.GenreName.Add(genreText[i][0].text);
        }

        //Save、シーン再読込
        table.Save();
        SceneManager.LoadScene("MenuSetScene");        
    }

    // Start is called before the first frame update
    void Start()
    {
        Table=GameObject.Find("GOTableScript");
        table=Table.GetComponent<TableScript>();
        Nmenu=table.Data.Nmenu;
        Ngenre=table.Data.Ngenre;
        Ndisc=table.Data.Ndisc;

        //==========================Canvasの編集==================================
            CanvasRect = CanvasPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = table.Data.CanPosition;
            CanvasRect.localScale = table.Data.CanScale;

        //==========================ジャンルの編集===============================
            for(int i=0;i<Ngenre;i++){
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
            genreText.Add(GOgenreButtons[i].GetComponentsInChildren<Text> ());
            genreText[i][0].text = table.Data.GenreName[i];
            genreRect[i].anchoredPosition = new Vector3(90*i-500,185-75,0);
            }

            nowGenre = 0;
            for(int i=0;i<Ngenre;i++){
                if(i == 0){
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

            discountRect.anchoredPosition = new Vector3(90*Ngenre-500,185-75,0);

        //==========================メニューの編集==================================

        for(int i=0;i<Nmenu;i++){

            int j=i;
                   
            GObuttons.Add(Instantiate(ButtonPrefab, canvas.transform));
            MenuGenre.Add(table.Data.MenuGenre[i]);
            GObuttons[i].transform.parent = genres[MenuGenre[i]].transform;
            buttons.Add(GObuttons[i].GetComponent<Button> ());
            buttons[i].onClick.AddListener(() => buttonClick(j)); 

            GObuttons[i].AddComponent<EventTrigger>();
            trigger.Add(GObuttons[i].GetComponent<EventTrigger>());
            entryDrag.Add(new EventTrigger.Entry());
            entryDrag[i].eventID = EventTriggerType.Drag;
            entryDrag[i].callback.AddListener((eventDate) => { buttonDrag(j); });
            trigger[i].triggers.Add(entryDrag[i]);


            MenuRect.Add(GObuttons[i].GetComponent<RectTransform> ());
            buttonText.Add(GObuttons[i].GetComponentsInChildren<Text> ()); 
            MenuRect[i].anchoredPosition = table.Data.MenuVect[i];
            MenuRect[i].localScale = new Vector3(1,1,1);
            buttonText[i][0].text = table.Data.MenuName[i];
            buttonText[i][1].text = table.Data.MenuPrice[i].ToString();

        };

        //=========================割引の編集===========================

        for(int i=0;i<Ndisc;i++){

            int j=i;
                   
            GOdiscButtons.Add(Instantiate(ButtonPrefab, canvas.transform));
            GOdiscButtons[i].transform.parent = discountPanel.transform;
            discButtons.Add(GOdiscButtons[i].GetComponent<Button>());
            discButtons[i].onClick.AddListener(() => discButtClick(j)); 

            GOdiscButtons[i].AddComponent<EventTrigger>();
            discTrigger.Add(GOdiscButtons[i].GetComponent<EventTrigger>());
            discEntryDrag.Add(new EventTrigger.Entry());
            discEntryDrag[i].eventID = EventTriggerType.Drag;
            discEntryDrag[i].callback.AddListener((eventDate) => { discButtDrag(j); });
            discTrigger[i].triggers.Add(discEntryDrag[i]);


            DiscRect.Add(GOdiscButtons[i].GetComponent<RectTransform> ());
            discButtText.Add(GOdiscButtons[i].GetComponentsInChildren<Text> ()); 
            DiscRect[i].anchoredPosition = table.Data.DiscVect[i];
            DiscRect[i].localScale = new Vector3(1,1,1);
            discButtText[i][0].text = table.Data.DiscName[i];
            discButtText[i][1].text = table.Data.DiscValue[i].ToString();
            discButtText[i][2].text = "割引";

        }

        GOInputField.GetComponent<RectTransform>().SetAsLastSibling();
        GOInputGenreField.GetComponent<RectTransform>().SetAsLastSibling();
    }


    // Update is called once per frame
    void Update()
    {
    }
}
