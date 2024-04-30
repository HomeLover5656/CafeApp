using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CafeClasses;

public class ExhibitScript : MonoBehaviour
{
    //それ以外========================================
    public int i;
    public GameObject Panel,DellMenuPanel,TableName,Num,Menu,MenuScroll,MenuScrollGO,PriceTextGO,DMtextPrefab;
    public Button LeaveButton,DellMenuButton; 
    public Text CMenuText,CCashText,CDrinkText,OkaeriText,DellMenuText,ScrollTableText;
    Text TableText,NumText,MenuText,PriceText,PanelText;
    GameObject Table,Kitchen;
    TableScript table;
    KitchenScript kitchen;

    public void LeaveClick()
    {
        if(table.Data.OrderData[i].Number!=0){
            for(int j=0;j<table.Data.Ntable;j+=1){
                if(table.Data.OrderData[j].Number>table.Data.OrderData[i].Number){
                    table.Data.OrderData[j].Number-=1;
                }
            }


            //注文データを初期化.
            table.Data.OrderData[i].SetOrder.Clear();
            table.Data.OrderData[i].init();
            table.Data.OrderData[i].SetOrder.Add(new current_struct.OrderStruct());
            table.Data.OrderData[i].SetOrder[0].init(table.Data.Nmenu,table.Data.Ndisc);
            for(int j=0;j<table.Data.Nmenu;j++){
                table.Data.OrderData[i].SetOrder[0].Order.Add(new List<int>());
                for(int k=0;k<table.Data.Ndisc+1;k++){
                    table.Data.OrderData[i].SetOrder[0].Order[j].Add(0);
                }
            }
            
            NumText.text="";
            CMenuText.text="";
            CDrinkText.text="";
            CCashText.text="";
            MenuText.text =" ";

            table.Data.LastNum -= 1;
            //SCENEの再読込
            table.Save();
            SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        }
    }

    public void NoLeaveClick()
    {
        Panel.SetActive(false);
    }
    public void NoDMClick()
    {
        DellMenuPanel.SetActive(false);
    }

    public void OkaeriClick()
    {
        OkaeriText.text = "本当におかえりですか";
        if(table.Data.OrderData[i].Number!=0){
            Panel.SetActive(true);
        }
        LeaveButton.onClick.RemoveAllListeners();
        LeaveButton.onClick.AddListener(() => LeaveClick());

    }

    public void CheckMenuClick()
    {
        if(table.Data.OrderData[i].Number==0){
            CMenuText.text = "";
        }else if(table.Data.OrderData[i].GivMenu){
            table.Data.OrderData[i].GivMenu=false;
            CMenuText.text = "未";
        }else{
            table.Data.OrderData[i].GivMenu=true;
            CMenuText.text = "済";
        }
    }
    
    public void CheckDrinkClick()
    {
        if(table.Data.OrderData[i].Number==0){
            CDrinkText.text = "";
        }else if(table.Data.OrderData[i].GivDrink){
            table.Data.OrderData[i].GivDrink=false;
            CDrinkText.text = "未";
        }else{
            table.Data.OrderData[i].GivDrink=true;
            CDrinkText.text = "済";
        }
    }

    public void CheckCashClick()
    {
        if(table.Data.OrderData[i].Number==0){
            CCashText.text = "";
        }else if(table.Data.OrderData[i].GetCash){
            table.Data.OrderData[i].GetCash=false;
            CCashText.text = "未";
        }else{
            table.Data.OrderData[i].GetCash=true;
            CCashText.text = "済";
        }
    }

    public void ScrollInvisClick()
    {
        MenuScroll.SetActive(false);
    }
    public void ScrollVisClick()
    {
        if(table.Data.OrderData[i].Number!=0){
        MenuScroll.SetActive(true);
        DellMenuPanel.SetActive(false);
        }
    }


    //=============ver(7)================= 
    public void DMClick(int set,string setmenustr){
        DellMenuText.text = "セット「"+ setmenustr + "」を削除しますか";
        DellMenuPanel.SetActive(true);
        LeaveButton.onClick.RemoveAllListeners();
        DellMenuButton.onClick.AddListener(() => DelSetClick(set));
    }
    public void DelSetClick(int set){
        table.Data.OrderData[i].SetOrder.RemoveAt(set);
        table.Data.OrderData[i].Nset--;
        //SCENEの再読込.
        table.Save();
        SceneManager.LoadScene("KitchenScene");
    }


    // Start is called before the first frame update
    void Start()
    {
        float Price = 0;
        Table=GameObject.Find("GOTableScript");
        Kitchen=GameObject.Find("GOKitchenScript");
        table=Table.GetComponent<TableScript>();
        kitchen=Kitchen.GetComponent<KitchenScript>();
        
        //==========================テーブルテキストの編集==================================
                
        TableText = TableName.GetComponentInChildren<Text> (); 
        TableText.text = table.Data.TableName[i];
        ScrollTableText.text = table.Data.TableName[i] + "席";

        NumText = Num.GetComponentInChildren<Text> ();
        if(table.Data.OrderData[i].Number!=0){
            NumText.text = table.Data.OrderData[i].Number.ToString();
        }

        //==========================未済ボタンの編集==================================

        if(table.Data.OrderData[i].Number==0){
            CMenuText.text = "";
        }else if(table.Data.OrderData[i].GivMenu){
            CMenuText.text = "済";
        }else{
            CMenuText.text = "未";
        }

        if(table.Data.OrderData[i].Number==0){
            CDrinkText.text = "";
        }else if(table.Data.OrderData[i].GivDrink){
            CDrinkText.text = "済";
        }else{
            CDrinkText.text = "未";
        }

        if(table.Data.OrderData[i].Number==0){
            CCashText.text = "";
        }else if(table.Data.OrderData[i].GetCash){
            CCashText.text = "済";
        }else{
            CCashText.text = "未";
        }

        MenuText = Menu.GetComponentInChildren<Text> (); 
        PriceText = PriceTextGO.GetComponentInChildren<Text>();
        MenuText.text = "";

        //=====================メニュー内テキスト、値段テキストの編集========================================

        string menustr;
        List<string> setmenustr = new List<string>();

        //scrollの位置とサイズ

        RectTransform ScrollRect = MenuScroll.GetComponent<RectTransform> ();
        ScrollRect.anchoredPosition = new Vector3(0,0,0);
        ScrollRect.localScale = new Vector3(1,1,1);
        MenuScroll.GetComponent<RectTransform>().SetAsLastSibling();

        for(int l=0;l<table.Data.OrderData[i].Nset;l++){

            int L = l;
            int SetPrice = 0;
            setmenustr.Add("");

            for(int j=0;j<table.Data.Nmenu;j+=1){

                for(int k=0;k<table.Data.Ndisc+1;k+=1){

                    if(table.Data.OrderData[i].SetOrder[l].Order[j][k]!=0){
                    
                        //割引ありorなし.
                        if( k==0 ) {
                            menustr = table.Data.MenuName[j] + "✕" + table.Data.OrderData[i].SetOrder[l].Order[j][k];
                            SetPrice += table.Data.MenuPrice[j]*table.Data.OrderData[i].SetOrder[l].Order[j][k];
                        }
                        else{
                            menustr = table.Data.MenuName[j] +"("+ table.Data.DiscName[k-1] + ") ✕" + table.Data.OrderData[i].SetOrder[l].Order[j][k];
                            SetPrice += table.Data.MenuPrice[j]*table.Data.OrderData[i].SetOrder[l].Order[j][k]*(10-table.Data.DiscValue[k-1])/10; 
                        }

                        setmenustr[l] += menustr +"/";
                        MenuText.text += menustr + " ";//スクロールなしの表示.

                    }
                }
            }
            Price += SetPrice;

            //scroll内のボタン＋テキスト.
            GameObject GODMtext = Instantiate(DMtextPrefab, kitchen.canvas.transform);
            GameObject GODMbutton = GODMtext.transform.Find("DelButton").gameObject;
            GODMtext.transform.parent = MenuScrollGO.transform;
            Button DMbutton = GODMbutton.GetComponent<Button>();
            DMbutton.onClick.AddListener(() => DMClick(L,setmenustr[L]));
            RectTransform DMtrect = GODMtext.GetComponent<RectTransform>();
            Text Numtext = GODMtext.transform.Find("SetNum").GetComponent<Text>();
            Text DMtext = GODMtext.transform.Find("SetMenu").GetComponent<Text>();
            Text Pricetext = GODMtext.transform.Find("SetPrice").GetComponent<Text>();
            DMtrect.anchoredPosition = new Vector3(0,400-60*l,0);
            
            DMtrect.localScale = new Vector3(1,1,1);
            Numtext.text = (l+1).ToString()+"人目";
            DMtext.text = setmenustr[L];
            Pricetext.text = SetPrice.ToString()+"円";

        }
        PriceText.text = Price.ToString();

        //====================================================================================================

    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
