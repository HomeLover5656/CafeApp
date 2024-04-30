using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class KitchenScript : MonoBehaviour
{
    //それ以外========================================
    public GameObject canvas,CanvasPanel;
    public GameObject RightPrefab;
    public GameObject LeftPrefab;
    GameObject[] GObuttons;
    RectTransform KitchenRect,CanvasRect;
    Text buttonText;
    GameObject Table;
    TableScript table;
    ExhibitScript Exhibit;


    // Start is called before the first frame update
    void Start()
    {
        Table=GameObject.Find("GOTableScript");
        table=Table.GetComponent<TableScript>();
        
        //==========================Canvasの編集==================================
            float RatioY = 0.8f ;

            RatioY = ((float)Screen.height/(float)Screen.width) / (720.0f/1280.0f);

            CanvasRect = CanvasPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = new Vector3(-table.Data.CanPosition.y,table.Data.CanPosition.x,table.Data.CanPosition.z);

            Vector3 CanvasVect = table.Data.CanScale;
            CanvasVect.x = CanvasVect.x*RatioY;
            CanvasRect.localScale = new Vector3(CanvasVect.y,CanvasVect.x,CanvasVect.z);
            

        //==========================ボタンの編集==================================
        string[] array=new string[3];        
        GObuttons = new GameObject[table.Data.Ntable];
                
        for(int i=0;i<table.Data.Ntable;i++){

            int j = table.Data.ExhOrd[i];

            //   生成したplaneのスクリプトを参照し、直接iを与える。
                   
            if(table.Data.TableVect[j].x < 0){
                GObuttons[j]=Instantiate(LeftPrefab, canvas.transform);
                GObuttons[j].transform.parent = CanvasPanel.transform;
                Exhibit=GObuttons[j].GetComponent<ExhibitScript>();
                Exhibit.i=j;
                KitchenRect = GObuttons[j].GetComponent<RectTransform> ();
                KitchenRect.anchoredPosition = new Vector3(-320,(table.Data.TableVect[j].y-table.Data.CanPosition.y)/table.Data.CanScale.y*0.49f-50,0);
                KitchenRect.localScale = new Vector3(1,1,1);
            }else{
                GObuttons[j]=Instantiate(RightPrefab, canvas.transform);
                GObuttons[j].transform.parent = CanvasPanel.transform;
                Exhibit=GObuttons[j].GetComponent<ExhibitScript>();
                Exhibit.i=j;
                KitchenRect = GObuttons[j].GetComponent<RectTransform> ();
                KitchenRect.anchoredPosition = new Vector3(320,(table.Data.TableVect[j].y-table.Data.CanPosition.y)/table.Data.CanScale.y*0.49f-50,0);
                KitchenRect.localScale = new Vector3(1,1,1);
            }

            //scrollの編集
            Exhibit.MenuScroll.transform.parent = CanvasPanel.transform;
        }
        //=========================================================================        

    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
