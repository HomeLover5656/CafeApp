using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasSetScript : MonoBehaviour
{
    public GameObject TopButton,BottomButton,RightButton,LeftButton,CanvasPanel;
    RectTransform TopRect,BottomRect,RightRect,LeftRect,CanvasRect;
    float CanTop,CanBottom,CanRight,CanLeft,buttonSizex,buttonSizey,ScreenH,ScreenW,RatioH,RatioW;
    Vector3 CanScale,CanPosition;

    GameObject Table;
    TableScript table;

    // できたらanchoredPositionかtransformPositionか統一させたほうがいいかも.

    // 端末の向きを取得する関数=====================================
        bool getPortrait() {
            bool result;

            if (Screen.width < Screen.height){
                
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

    // ボタン関連=================================================
        public void TopButtonDrag()
        {
            if(Input.mousePosition.y < Screen.height-buttonSizey ) TopButton.transform.position = new Vector3( Screen.width/2 , Input.mousePosition.y , 0 );
            else TopButton.transform.position = new Vector3( Screen.width/2 , Screen.height-buttonSizey , 0 );
            CanTop = TopRect.anchoredPosition.y /RatioH + buttonSizey;
            CanvasSet();
        }
        public void BottomButtonDrag()
        {
            if(Input.mousePosition.y > buttonSizey) BottomButton.transform.position = new Vector3( Screen.width/2 , Input.mousePosition.y , 0 );
            else BottomButton.transform.position = new Vector3( Screen.width/2 , buttonSizey , 0 );
            CanBottom = BottomRect.anchoredPosition.y /RatioH - buttonSizey;
            CanvasSet();
        }
        public void RightButtonDrag()
        {
            if(Input.mousePosition.x < Screen.width-buttonSizex) RightButton.transform.position = new Vector3( Input.mousePosition.x , Screen.height/2 , 0 );
            else RightButton.transform.position = new Vector3( Screen.width-buttonSizex , Screen.height/2 , 0 );
            CanRight = RightRect.anchoredPosition.x /RatioW + buttonSizex;
            CanvasSet();
        }
        public void LeftButtonDrag()
        {
            if(Input.mousePosition.x > buttonSizex) LeftButton.transform.position = new Vector3( Input.mousePosition.x , Screen.height/2 , 0 );
            else LeftButton.transform.position = new Vector3( buttonSizex , Screen.height/2 , 0 );
            CanLeft = LeftRect.anchoredPosition.x /RatioW - buttonSizex;
            CanvasSet();
        }

        void CanvasSet()
        {            
            CanScale = new Vector3((CanRight-CanLeft)/ScreenW , (CanTop-CanBottom)/ScreenH , 0);
            CanPosition = new Vector3((CanRight+CanLeft)/2 , (CanTop+CanBottom)/2 , 0);
            CanvasRect.anchoredPosition = CanPosition;
            CanvasRect.localScale = CanScale;  
        }

        public void Save()
        {
            CanvasSet();

            table.Data.CanScale = CanScale;
            table.Data.CanPosition = CanPosition;
            table.Save();

            SceneManager.LoadScene("CanvasSetScene");
        }
        public void Reset()
        {
            SceneManager.LoadScene("CanvasSetScene");
        }
        public void Fin()
        {
            Destroy (Table);
            SceneManager.LoadScene("TableScene");
        }


    // Start is called before the first frame update
    void Start()
    {
        Table=GameObject.Find("GOTableScript");
        table=Table.GetComponent<TableScript>();

        //ScreenW = Screen.width;
        //ScreenH = Screen.height;
        ScreenW = 720;
        ScreenH = 1280;
        RatioW = Screen.width/720;
        RatioH = Screen.height/1280;
        Debug.Log(Screen.height);
        buttonSizex = 125*RatioW;
        buttonSizey = 125*RatioH;
        //buttonSizex = 125;
        //buttonSizey = 125;

        TopRect = TopButton.GetComponent<RectTransform> ();
        BottomRect = BottomButton.GetComponent<RectTransform> ();
        RightRect = RightButton.GetComponent<RectTransform> ();
        LeftRect = LeftButton.GetComponent<RectTransform> ();
        CanvasRect = CanvasPanel.GetComponent<RectTransform>();
        
        
        CanTop = table.Data.CanScale.y*ScreenH/2+table.Data.CanPosition.y;
        CanBottom = -table.Data.CanScale.y*ScreenH/2+table.Data.CanPosition.y;
        CanRight = table.Data.CanScale.x*ScreenW/2 + table.Data.CanPosition.x;
        CanLeft = -table.Data.CanScale.x*ScreenW/2 + table.Data.CanPosition.x;

        TopRect.anchoredPosition = new Vector3 (0 , (CanTop - buttonSizey) , 0)*RatioH;
        BottomRect.anchoredPosition = new Vector3 (0 , (CanBottom + buttonSizey) , 0)*RatioH;
        RightRect.anchoredPosition = new Vector3 ((CanRight - buttonSizex) , 0 , 0)*RatioW;
        LeftRect.anchoredPosition = new Vector3 ((CanLeft + buttonSizex) , 0 , 0)*RatioW;
        
        

        CanvasSet();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
