using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using CafeClasses;

public class TableSetScript : MonoBehaviour
{
    //それ以外========================================
    public GameObject canvas,ButtonPrefab,GOInputField,CanvasPanel;
    public InputField inputField;
    public Button inputButton,inputDelButton;

    List<GameObject> GObuttons = new List<GameObject>();
    List<Button> buttons = new List<Button>();
    List<EventTrigger> trigger = new List<EventTrigger>();
    List<EventTrigger.Entry> entryDrag = new List<EventTrigger.Entry>();
    List<RectTransform> TableRect = new List<RectTransform>();
    List<Text> buttonText = new List<Text>();
    
    GameObject Table;
    TableScript table;
    int Ntable,AddCount=0;
    bool NoDrag=false;
    List<int> DelNum = new List<int>();

    Vector3 screenPoint;
    Vector2 localPoint=Vector2.zero;
    RectTransform CanvasRect;


    private current_struct dummy;
 
    //ボタン関係======================================================

    void buttonClick(int num)
    {
        if(NoDrag){
            int j=num;
            //名前入力
            inputButton.onClick.RemoveAllListeners();
            inputButton.onClick.AddListener(() => inputName(j));
            inputDelButton.onClick.RemoveAllListeners();
            inputDelButton.onClick.AddListener(() => inputDel(j));
            GOInputField.SetActive(true);
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
        //inputFieldの情報をbuttonText[].textに入れる。
        buttonText[num].text = inputField.text;
        inputField.text = "";
        GOInputField.SetActive(false);
    }
    public void inputReturn(){
        GOInputField.SetActive(false);
    }


    public void inputDel(int num){
        //ListのRemoveはList番号が変わってめんどくさいからSaveClick()でまとめてする。

        //削除するボタンの番号を保持
        DelNum.Add(num);
        //ボタンオブジェクトを非表示
        GObuttons[num].SetActive(false);

        GOInputField.SetActive(false);
    }
    

    public void AddClick()
    {
        AddCount+=1;
        Ntable+=1;
        //GObuttons[]とbuttons[]の要素追加、インスタンス代入
        
        int j=Ntable-1;
                
        GObuttons.Add(Instantiate(ButtonPrefab, canvas.transform));
        GObuttons[j].transform.parent = CanvasPanel.transform;
        buttons.Add(GObuttons[j].GetComponent<Button> ());
        buttons[j].onClick.AddListener(() => buttonClick(j)); 
        GObuttons[j].AddComponent<EventTrigger>();
        trigger.Add(GObuttons[j].GetComponent<EventTrigger>());
        entryDrag.Add(new EventTrigger.Entry());
        entryDrag[j].eventID = EventTriggerType.Drag;
        entryDrag[j].callback.AddListener((eventDate) => { buttonDrag(j); });
        trigger[j].triggers.Add(entryDrag[j]);
        TableRect.Add(GObuttons[j].GetComponent<RectTransform> ());
        buttonText.Add(GObuttons[j].GetComponentInChildren<Text> ());
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
        SceneManager.LoadScene("TableSetScene");
    }


    public void SaveClick(){

        //ボタンの追加の反映（TableScriptのA[][],B[][]）==========================================
        int itable;
        for(int i=0;i<AddCount;i++){
            //table.(A,B) にリスト要素追加＆代入
            itable = table.Data.Ntable + i;
            table.Data.OrderData.Add(new current_struct.GuestStruct());
            table.Data.OrderData[itable].init();
            table.Data.OrderData[itable].SetOrder.Add(new current_struct.OrderStruct());
            for(int j=0;j<table.Data.Nmenu;j++){
                table.Data.OrderData[itable].SetOrder[0].Order.Add(new List<int>());
                for(int k=0;k<table.Data.Ndisc+1;k++){
                    table.Data.OrderData[itable].SetOrder[0].Order[j].Add(0);
                }
            }
        }

        //ボタンの削除の反映（このスクリプト内のListと、TableScriptのA[][],B[][]）==========================================
        int DelCount;
        DelCount=DelNum.Count;
        if(DelCount != 0){
            //List要素の削除（番号変わらないように、大きい順に）
            DelNum.Sort();
            Ntable=Ntable-DelCount;
            for(int i=DelCount-1;i>=0;i--){
                buttonText.RemoveAt(DelNum[i]);
                TableRect.RemoveAt(DelNum[i]);
                entryDrag.RemoveAt(DelNum[i]);
                trigger.RemoveAt(DelNum[i]);
                buttons.RemoveAt(DelNum[i]);
                GObuttons.RemoveAt(DelNum[i]);
                
                table.Data.OrderData.RemoveAt(DelNum[i]);
            }
        }
        
        // 残りの情報をTableScriptに反映========================================
        table.Data.Ntable=Ntable;
        table.Data.TableName.Clear();
        table.Data.TableVect.Clear();
        table.Data.ExhOrd.Clear();

        for(int i=0;i<Ntable;i++){
            table.Data.TableName.Add(buttonText[i].text);
            table.Data.TableVect.Add(TableRect[i].anchoredPosition); 
            table.Data.ExhOrd.Add(i);
        }
        
        int Ccopy;
        for(int i=0;i<Ntable-1;i++){
            for(int j=i+1;j<Ntable;j++){
                if(table.Data.TableVect[table.Data.ExhOrd[i]].y < table.Data.TableVect[table.Data.ExhOrd[j]].y){
                    Ccopy = table.Data.ExhOrd[i];
                    table.Data.ExhOrd[i]=table.Data.ExhOrd[j];
                    table.Data.ExhOrd[j]=Ccopy;
                }
            }
        }

        //Save、シーン再読込
        table.Save();
        SceneManager.LoadScene("TableSetScene");
        
    }
    


    // Start is called before the first frame update
    void Start()
    {
        Table=GameObject.Find("GOTableScript");
        table=Table.GetComponent<TableScript>();

        //==========================Canvasの編集==================================
            CanvasRect = CanvasPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = table.Data.CanPosition;
            CanvasRect.localScale = table.Data.CanScale;

        //==========================ボタンの編集==================================

        Ntable=table.Data.Ntable;
                
        for(int i=0;i<Ntable;i++){

            int j=i;
                   
            GObuttons.Add(Instantiate(ButtonPrefab, canvas.transform));
            GObuttons[i].transform.parent = CanvasPanel.transform;
            buttons.Add(GObuttons[i].GetComponent<Button> ());
            buttons[i].onClick.AddListener(() => buttonClick(j)); 

            GObuttons[i].AddComponent<EventTrigger>();
            trigger.Add(GObuttons[i].GetComponent<EventTrigger>());
            entryDrag.Add(new EventTrigger.Entry());
            entryDrag[i].eventID = EventTriggerType.Drag;
            entryDrag[i].callback.AddListener((eventDate) => { buttonDrag(j); });
            trigger[i].triggers.Add(entryDrag[i]);


            TableRect.Add(GObuttons[i].GetComponent<RectTransform> ());
            buttonText.Add(GObuttons[i].GetComponentInChildren<Text> ()); 
            TableRect[i].anchoredPosition = table.Data.TableVect[i];
            buttonText[i].text = table.Data.TableName[i];

            GOInputField.GetComponent<RectTransform>().SetAsLastSibling();

        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}
