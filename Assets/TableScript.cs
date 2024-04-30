using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CafeClasses;

public class TableScript : MonoBehaviour
{
    //クラスの読み込み.===============================
        public current_struct Data = new current_struct();

    //==================================================

    //他のスクリプトと共有する変数=====================
        public string OldPath;
        public string FolderPath;
        public GameObject canvas,SetCanvas;

    //それ以外の変数===================================
        public GameObject ButtonPrefab,CanvasPanel,SetPanel,SetButton;
        int preVerNum;
        GameObject[] GObuttons;
        Button[] buttons;
        RectTransform TableRect,CanvasRect;
        Text buttonText;
        bool Portrait;
        bool PrePortrait;
    //===================================================


    //最新versionの初期化用データ(verごとの構造体に統合したい)==========================
        int VerNum = 7;
        string iniDATA = "=================テーブル関係======================="+
        "\n---テーブル数---\n8\n---テーブル名---\nA B C D E F G H\n---テーブル座標---\n200 -440 0\n200 -140 0\n200 160 0\n200 460 0\n-200 460 0\n-200 160 0\n-200 -140 0\n-200 -440 0\n"+
        "=================メニュー関係======================="+
        "\n---メニュー数---\n4\n---ジャンル数---\n1\n---メニュー名---\nHC CC T CT\n---価格---\n400 400 400 400\n---ジャンル---\n0 0 0 0\n---ジャンル名---\nドリンク\n---メニュー座標---\n-180 160 0\n-180 -20 0\n-180 -200 0\n-180 -380 0\n"+
        "=================割引関係======================="+
        "\n---割引種類数---\n0\n---割引名---\n\n---割引値---\n\n---割引座標---\n\n"+
        "=================各席情報OrderData==================="+
        "\n---table_0---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_1---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_2---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_3---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_4---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_5---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_6---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0"+
        "\n---table_7---\n--来客順--\n0\n--セット数--\n0\n--未済--\nfalse\nfalse\nfalse\n---注文---\n--set0--\n0 0 0 0\n"+
        "=================y座標順リストC======================="+
        "\n3 4 2 5 1 6 0 7\n"+
        "=================画面スケーリング======================="+
        "\n1 1 1\n0 0 0\n1 1 1\n0 0 0";

    //=================================================


    //オブジェクトを破壊させないための処理=========================
        public static TableScript Instance {
            get; private set;
        }
        void Awake(){
            if (Instance != null) {
                Destroy (gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad (gameObject);
        }  

    

    //version確認用関数（更新不要）===========================================================
        void VerCheck(){
            // CafeAppDATAフォルダが存在しないなら、CafeAppDataフォルダ,versionファイル,DATAファイルを作成
            //  かつ、ver1のDATAファイルが存在するなら、versionファイルをver=1に書き換え
            if (Directory.Exists(FolderPath) == false){

                Directory.CreateDirectory(FolderPath);
                using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(iniDATA);
                using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(VerNum.ToString());

                if(System.IO.File.Exists(OldPath)) using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("1");
            }

            // versionが古ければ、現ver形式まで変換する. 未来のversionなら、現ver形式で初期化.
            using (var fs = new StreamReader(FolderPath + "/version.txt", System.Text.Encoding.GetEncoding("UTF-8"))) preVerNum = int.Parse(fs.ReadLine());
            if (preVerNum < VerNum) for (int i=preVerNum;i < VerNum;i++) VerChange(i);
            else if (preVerNum > VerNum) {
                using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(iniDATA);
                using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(VerNum.ToString());
            }
        }

    //DATAファイルをversion(n)形式からversion(n+1)形式に変換する関数(versionが上がり次第更新)===
    //新方針：ver_N-1_read()でver_N-1_struct作成→ver_N-1_structをver_N_structに変換→ver_N_save().
        void VerChange(int n){
            //==========================変数宣言===================================
                string data_txt;
                string[] array=new string[3];
                int Ntable1,Nmenu1,Ngenre1,Ndisc1,LastNum1=0;
                List<string> TableName1=new List<string>();
                List<int> TableNum1 = new List<int>();
                List<string> MenuName1=new List<string>();
                List<int> MenuPrice1=new List<int>();
                List<Vector3> TableVect1=new List<Vector3>();
                List<Vector3> MenuVect1=new List<Vector3>();
                List<string> GenreName1=new List<string>();
                List<int> MenuGenre1=new List<int>();
                List<List<int>> A1=new List<List<int>>();
                List<List<List<int>>> A2=new List<List<List<int>>>();   //ver5以降
                List<List<bool>> B1=new List<List<bool>>();
                List<int> C1=new List<int>();
                List<string> DiscName1 = new List<string>();
                List<int> DiscValue1 = new List<int>();
                List<Vector3> DiscVect1 =new List<Vector3>();
                Vector3 CanScale1,CanPosition1,KCanScale1,KCanPosition1;
            switch(n){
                
                case 1:
                    //==================================ver1ファイル読込=================================
                        using (var fs = new StreamReader(OldPath, System.Text.Encoding.GetEncoding("UTF-8")))
                        {

                            //==========================table関係の読込=============================
                            Ntable1=int.Parse(fs.ReadLine());

                            //TableName1
                            TableName1.AddRange(fs.ReadLine().Split(' '));

                            //TableVector1
                            for(int i=0;i<Ntable1;i++){
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                TableVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                            }

                            //============================menu関係の読込==============================
                            Nmenu1=int.Parse(fs.ReadLine());

                            //MenuName1
                            MenuName1.AddRange(fs.ReadLine().Split(' '));

                            //MenuVector1
                            for(int i=0;i<Nmenu1;i++){
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                MenuVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                            }

                            //========================リストA1[][],B1[][]の読込==================================

                            Array.Resize(ref array, Nmenu1 + 1);
                            for(int i=0;i<Ntable1;i++){
                                A1.Add(new List<int>());
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1 + 1);
                                for(int j=0;j<Nmenu1 + 1;j++){
                                    A1[i].Add(int.Parse(array[j]));
                                }
                                if(A1[i][0] > LastNum1)LastNum1 = A1[i][0];
                            }

                            Array.Resize(ref array, 2);
                            for(int i=0;i<Ntable1;i++){
                                B1.Add(new List<bool>());
                                Array.Copy(fs.ReadLine().Split(' '), array, 2);
                                for(int j=0;j<2;j++){
                                    B1[i].Add(bool.Parse(array[j]));
                                }
                            }

                            //====================MenuPrice1 , C1[]の作成(TableVector1.yでバブルソート)=============
                            int Ccopy;
                            for(int i=0;i<Nmenu1;i++) MenuPrice1.Add(0);
                            for(int i=0;i<Ntable1;i++) C1.Add(i);//まずは番号順に入れていく.
                            for(int i=0;i<Ntable1-1;i++){
                                for(int j=i+1;j<Ntable1;j++){
                                    if(TableVect1[C1[i]].y < TableVect1[C1[j]].y){
                                        Ccopy = C1[i];
                                        C1[i]=C1[j];
                                        C1[j]=Ccopy;
                                    }
                                    /*
                                    if(dummy[i] < dummy[j]){
                                        Ccopy = dummy[i];
                                        dummy[i]=dummy[j];
                                        dummy[j]=Ccopy;
                                    }
                                    */
                                }
                            }

                        
                            
                        }
                    //======================ver2形式の文字列を作成=========================================
                        //Table関連==================================================
                        data_txt=Ntable1.ToString()+"\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableName1[i]+" ";
                        data_txt+="\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableVect1[i].x.ToString()+" "+TableVect1[i].y.ToString()+" "+TableVect1[i].z.ToString()+"\n";

                        //Menu関連===================================================
                        data_txt+=Nmenu1.ToString()+"\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuName1[i]+" ";
                        data_txt+="\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuPrice1[i].ToString()+" ";
                        data_txt+="\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuVect1[i].x.ToString()+" "+MenuVect1[i].y.ToString()+" "+MenuVect1[i].z.ToString()+"\n";
                    
                        //リストA[]====================================================
                        for(int i=0;i<Ntable1;i++){
                            if(A1[i][0] <= 0) for(int j=0;j<Nmenu1 + 1;j++) A1[i][j]=0;
                            for(int j=0;j<Nmenu1 + 1;j++) data_txt+=A1[i][j].ToString()+" ";
                            data_txt+="\n";
                        }

                        //リストB[]====================================================
                        for(int i=0;i<Ntable1;i++){
                            if(A1[i][0] == 0) for(int j=0;j<2;j++) B1[i][j]=false;
                            for(int j=0;j<2;j++) data_txt+=B1[i][j].ToString()+" ";
                            data_txt+="\n";
                        }

                        //リストC[]====================================================
                        for(int i=0;i<Ntable1;i++) data_txt+=C1[i].ToString()+" ";
                        data_txt+="\n";
                    //======================DATAファイルを新規作成=========================================
                        using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);

                    //======================version情報の書き換え===============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("2");
                
                    break;

                case 2:
                    //=====ファイル末尾にCanScale,CanPosition,KCanSlace,KCanPositionを加えるだけ=========
                    //======================DATAファイル追記=========================================
                        using (var fs = new StreamWriter(FolderPath + "/DATA.txt", true, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("1 1 1\n0 0 0\n1 1 1\n0 0 0\n");
                    //======================version情報の書き換え===============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("3");

                    break;

                case 3:
                    //==================================ver3ファイル読込=================================
                    
                        using (var fs = new StreamReader(FolderPath + "/DATA.txt", System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            //==========================table関係の読込=============================
                                Ntable1=int.Parse(fs.ReadLine());

                                //TableName
                                TableName1.AddRange(fs.ReadLine().Split(' '));

                                //TableVector
                                for(int i=0;i<Ntable1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    TableVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //============================menu関係の読込==============================
                                Nmenu1=int.Parse(fs.ReadLine());

                                //MenuName
                                MenuName1.AddRange(fs.ReadLine().Split(' '));

                                //MenuPrice
                                Array.Resize(ref array, Nmenu1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                for(int i=0;i<Nmenu1;i++) MenuPrice1.Add(int.Parse(array[i]));

                                //MenuVector
                                Array.Resize(ref array, 3);
                                for(int i=0;i<Nmenu1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    MenuVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //========================リストA[][],B[][],ExhOrd[]の読込==================================
                                Array.Resize(ref array, Nmenu1 +1);
                                for(int i=0;i<Ntable1;i++){
                                    A1.Add(new List<int>());
                                    Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1 +1);
                                    for(int j=0;j<Nmenu1 +1;j++) A1[i].Add(int.Parse(array[j]));
                                    if(A1[i][0] > LastNum1) LastNum1 = A1[i][0];
                                }

                                Array.Resize(ref array, 2);
                                for(int i=0;i<Ntable1;i++){
                                    B1.Add(new List<bool>());
                                    Array.Copy(fs.ReadLine().Split(' '), array, 2);
                                    for(int j=0;j<2;j++) B1[i].Add(bool.Parse(array[j]));
                                }

                                Array.Resize(ref array, Ntable1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Ntable1);
                                for(int i=0;i<Ntable1;i++) C1.Add(int.Parse(array[i]));
                            //========================CanvasScaleなどの読込========================================
                                Array.Resize(ref array, 3);
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));
                        }
                    //======================ver4形式の文字列を作成=========================================
                        //menu genre,genre name を追加する

                        //Table関連==================================================
                            data_txt=Ntable1.ToString()+"\n";
                            for(int i=0;i<Ntable1;i++) data_txt+=TableName1[i]+" ";
                            data_txt+="\n";
                            for(int i=0;i<Ntable1;i++) data_txt+=TableVect1[i].x.ToString()+" "+TableVect1[i].y.ToString()+" "+TableVect1[i].z.ToString()+"\n";

                        //Menu関連===================================================
                            data_txt+=Nmenu1.ToString()+"\n";
                            data_txt+=1+"\n";//ver4要素
                            for(int i=0;i<Nmenu1;i++) data_txt+=MenuName1[i]+" ";
                            data_txt+="\n";
                            for(int i=0;i<Nmenu1;i++) data_txt+=MenuPrice1[i].ToString()+" ";
                            data_txt+="\n";
                            
                            for(int i=0;i<Nmenu1;i++) data_txt+="0 ";//ver4要素
                            data_txt+="\n";
                            for(int i=0;i<1;i++) data_txt+="ドリンク ";//ver4要素
                            data_txt+="\n";

                            for(int i=0;i<Nmenu1;i++) data_txt+=MenuVect1[i].x.ToString()+" "+MenuVect1[i].y.ToString()+" "+MenuVect1[i].z.ToString()+"\n";
                    
                        //リストA[]====================================================
                            for(int i=0;i<Ntable1;i++){
                                if(A1[i][0] <= 0) for(int j=0;j<Nmenu1+1;j++) A1[i][j]=0;
                                for(int j=0;j<Nmenu1+1;j++) data_txt+=A1[i][j].ToString()+" ";
                                data_txt+="\n";
                            }

                        //リストB[]====================================================
                            for(int i=0;i<Ntable1;i++){
                                if(A1[i][0] == 0) for(int j=0;j<2;j++) B1[i][j]=false;
                                for(int j=0;j<2;j++) data_txt+=B1[i][j].ToString()+" ";
                                data_txt+="\n";
                            }

                        //リストC[]====================================================
                            for(int i=0;i<Ntable1;i++) data_txt+=C1[i].ToString()+" ";
                            data_txt+="\n";

                        //CanvasScaleなど==============================================
                            data_txt+=CanScale1.x.ToString()+" "+CanScale1.y.ToString()+" "+CanScale1.z.ToString()+"\n";
                            data_txt+=CanPosition1.x.ToString()+" "+CanPosition1.y.ToString()+" "+CanPosition1.z.ToString()+"\n";
                            data_txt+=KCanScale1.x.ToString()+" "+KCanScale1.y.ToString()+" "+KCanScale1.z.ToString()+"\n";
                            data_txt+=KCanPosition1.x.ToString()+" "+KCanPosition1.y.ToString()+" "+KCanPosition1.z.ToString()+"\n";
                        
                    //======================DATAファイル書き換え=========================================
                        using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);
                    //======================version情報の書き換え===============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("4");
                    break;

                case 4:
                    //==============DATAファイル(ver4)の読込===============
                        using (var fs = new StreamReader(FolderPath + "/DATA.txt", System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            //==========================table関係の読込=============================
                                Ntable1=int.Parse(fs.ReadLine());

                                //TableName
                                TableName1.AddRange(fs.ReadLine().Split(' '));

                                //TableVector
                                for(int i=0;i<Ntable1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    TableVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //============================menu・ジャンル関係の読込==============================
                                Nmenu1=int.Parse(fs.ReadLine());
                                Ngenre1=int.Parse(fs.ReadLine());

                                //MenuName
                                MenuName1.AddRange(fs.ReadLine().Split(' '));

                                //MenuPrice
                                Array.Resize(ref array, Nmenu1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                for(int i=0;i<Nmenu1;i++) MenuPrice1.Add(int.Parse(array[i]));

                                //MenuGenre
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                for(int i=0;i<Nmenu1;i++) MenuGenre1.Add(int.Parse(array[i]));
                                
                                //GenreName
                                GenreName1.AddRange(fs.ReadLine().Split(' '));

                                //MenuVector
                                Array.Resize(ref array, 3);
                                for(int i=0;i<Nmenu1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    MenuVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //========================リストA[][],B[][],ExhOrd[]の読込==================================
                                Array.Resize(ref array, Nmenu1+1);
                                for(int i=0;i<Ntable1;i++){
                                    A1.Add(new List<int>());
                                    Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1+1);
                                    for(int j=0;j<Nmenu1+1;j++) A1[i].Add(int.Parse(array[j]));
                                    if(A1[i][0] > LastNum1)LastNum1 = A1[i][0];
                                }

                                Array.Resize(ref array, 2);
                                for(int i=0;i<Ntable1;i++){
                                    B1.Add(new List<bool>());
                                    Array.Copy(fs.ReadLine().Split(' '), array, 2);
                                    for(int j=0;j<2;j++) B1[i].Add(bool.Parse(array[j]));
                                }

                                Array.Resize(ref array, Ntable1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Ntable1);
                                for(int i=0;i<Ntable1;i++) C1.Add(int.Parse(array[i]));
                            //========================CanvasScaleなどの読込========================================
                                Array.Resize(ref array, 3);
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));
                            //============================================================
                        }
                    //======================ver5形式の文字列を作成=========================================
                        //Ndisc,DiscName[],DiscValue[] を追加する
                        //Table関連==================================================
                        data_txt=Ntable1.ToString()+"\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableName1[i]+" ";
                        data_txt+="\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=A1[i][0].ToString()+" ";
                        data_txt+="\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableVect1[i].x.ToString()+" "+TableVect1[i].y.ToString()+" "+TableVect1[i].z.ToString()+"\n";

                        //Menu・ジャンル関連===================================================
                        data_txt+=Nmenu1.ToString()+"\n";
                        data_txt+=Ngenre1.ToString()+"\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuName1[i]+" ";
                        data_txt+="\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuPrice1[i].ToString()+" ";
                        data_txt+="\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuGenre1[i].ToString()+" ";
                        data_txt+="\n";
                        for(int i=0;i<Ngenre1;i++) data_txt+=GenreName1[i]+" ";
                        data_txt+="\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuVect1[i].x.ToString()+" "+MenuVect1[i].y.ToString()+" "+MenuVect1[i].z.ToString()+"\n";
                    
                        //割引関連=================================================
                        data_txt+="0 \n \n \n";
                        
                        //リストA[]====================================================
                        for(int i=0;i<Ntable1;i++){
                            if(A1[i][0] <= 0) for(int j=0;j<Nmenu1+1;j++) A1[i][j]=0;
                            for(int j=1;j<Nmenu1+1;j++) data_txt+=A1[i][j].ToString()+" ";
                            data_txt+="\n";
                        }

                        //リストB[]====================================================
                        for(int i=0;i<Ntable1;i++){
                            if(A1[i][0] == 0) for(int j=0;j<2;j++) B1[i][j]=false;
                            for(int j=0;j<2;j++) data_txt+=B1[i][j].ToString()+" ";
                            data_txt+="\n";
                        }

                        //リストC[]====================================================
                        for(int i=0;i<Ntable1;i++) data_txt+=C1[i].ToString()+" ";
                        data_txt+="\n";

                        //CanvasScaleなど==============================================
                        data_txt+=CanScale1.x.ToString()+" "+CanScale1.y.ToString()+" "+CanScale1.z.ToString()+"\n";
                        data_txt+=CanPosition1.x.ToString()+" "+CanPosition1.y.ToString()+" "+CanPosition1.z.ToString()+"\n";
                        data_txt+=KCanScale1.x.ToString()+" "+KCanScale1.y.ToString()+" "+KCanScale1.z.ToString()+"\n";
                        data_txt+=KCanPosition1.x.ToString()+" "+KCanPosition1.y.ToString()+" "+KCanPosition1.z.ToString()+"\n";

                    //======================DATAファイル書き換え=========================================
                        using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);
                    //======================version情報の書き換え===============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("5");
                    break;

                case 5:
                
                    //==============DATAファイル(ver5)の読込===============
                        using (var fs = new StreamReader(FolderPath + "/DATA.txt", System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            //==========================table関係の読込=============================
                                Ntable1=int.Parse(fs.ReadLine());

                                //TableName
                                TableName1.AddRange(fs.ReadLine().Split(' '));

                                //TableNum
                                Array.Resize(ref array, Ntable1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Ntable1);
                                for(int i=0;i<Ntable1;i++) TableNum1.Add(int.Parse(array[i]));
                                LastNum1 = TableNum1.Max();

                                //TableVector
                                for(int i=0;i<Ntable1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    TableVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //============================menu・ジャンル関係の読込==============================
                                Nmenu1=int.Parse(fs.ReadLine());
                                Ngenre1=int.Parse(fs.ReadLine());

                                //MenuName
                                MenuName1.AddRange(fs.ReadLine().Split(' '));

                                //MenuPrice
                                Array.Resize(ref array, Nmenu1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                for(int i=0;i<Nmenu1;i++) MenuPrice1.Add(int.Parse(array[i]));

                                //MenuGenre
                                Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                for(int i=0;i<Nmenu1;i++) MenuGenre1.Add(int.Parse(array[i]));
                                
                                //GenreName
                                GenreName1.AddRange(fs.ReadLine().Split(' '));

                                //MenuVector
                                Array.Resize(ref array, 3);
                                for(int i=0;i<Nmenu1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    MenuVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //========================= 割引関係の読み込み===================================
                                Ndisc1=int.Parse(fs.ReadLine());
                                DiscName1.AddRange(fs.ReadLine().Split(' '));
                                //DiscValue
                                Array.Resize(ref array, Ndisc1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Ndisc1);
                                for(int i=0;i<Ndisc1;i++) DiscValue1.Add(int.Parse(array[i]));
                                //DiscVector
                                Array.Resize(ref array, 3);
                                for(int i=0;i<Ndisc1;i++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                    DiscVect1.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                                }
                            //========================リストA[][],B[][],ExhOrd[]の読込==================================
                                Array.Resize(ref array, Nmenu1);
                                for(int k=0;k<Ndisc1 +1;k++){
                                    A2.Add(new List<List<int>>());
                                    for(int i=0;i<Ntable1;i++){
                                        A2[k].Add(new List<int>());
                                        Array.Copy(fs.ReadLine().Split(' '), array, Nmenu1);
                                        for(int j=0;j<Nmenu1;j++) A2[k][i].Add(int.Parse(array[j]));
                                    }
                                }

                                Array.Resize(ref array, 2);
                                for(int i=0;i<Ntable1;i++){
                                    B1.Add(new List<bool>());
                                    Array.Copy(fs.ReadLine().Split(' '), array, 2);
                                    for(int j=0;j<2;j++) B1[i].Add(bool.Parse(array[j]));
                                    B1[i].Add(false);
                                }

                                Array.Resize(ref array, Ntable1);
                                Array.Copy(fs.ReadLine().Split(' '), array, Ntable1);
                                for(int i=0;i<Ntable1;i++) C1.Add(int.Parse(array[i]));
                            //========================CanvasScaleなどの読込========================================
                                Array.Resize(ref array, 3);
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                CanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanScale1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                KCanPosition1 = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));
                            //=======================================================================================    
                        }
                    //======================ver6形式の文字列を作成=======================================
                        //Table関連==================================================
                        data_txt = "=================テーブル関係=======================\n---テーブル数---\n";
                        data_txt+=Ntable1.ToString();
                        data_txt+="\n---テーブル名---\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableName1[i]+" ";
                        data_txt+="\n---来客順---\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableNum1[i].ToString()+" ";
                        data_txt+="\n---テーブル座標---\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=TableVect1[i].x.ToString()+" "+TableVect1[i].y.ToString()+" "+TableVect1[i].z.ToString()+"\n";

                        //Menu・ジャンル関連===================================================
                        data_txt+= "=================メニュー関係=======================\n---メニュー数---\n";
                        data_txt+=Nmenu1.ToString();
                        data_txt+="\n---ジャンル数---\n";
                        data_txt+=Ngenre1.ToString();
                        data_txt+="\n---メニュー名---\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuName1[i]+" ";
                        data_txt+="\n---価格---\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuPrice1[i].ToString()+" ";
                        data_txt+="\n---ジャンル---\n";
                        Debug.Log("ok");
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuGenre1[i].ToString()+" ";
                        data_txt+="\n---ジャンル名---\n";
                        for(int i=0;i<Ngenre1;i++) data_txt+=GenreName1[i]+" ";
                        data_txt+="\n---メニュー座標---\n";
                        for(int i=0;i<Nmenu1;i++) data_txt+=MenuVect1[i].x.ToString()+" "+MenuVect1[i].y.ToString()+" "+MenuVect1[i].z.ToString()+"\n";
                    
                        //割引関連=====================================================
                        data_txt+="=================割引関係=======================\n---割引種類数---\n";
                        data_txt+=Ndisc1.ToString();
                        data_txt+="\n---割引名---\n";
                        for(int i=0;i<Ndisc1;i++) data_txt+=DiscName1[i]+" ";
                        data_txt+="\n---割引値---\n";
                        for(int i=0;i<Ndisc1;i++) data_txt+=DiscValue1[i].ToString()+" ";
                        data_txt+="\n---割引座標---\n";
                        if(Ndisc1==0){data_txt+="\n";}
                        else{for(int i=0;i<Ndisc1;i++) data_txt+=DiscVect1[i].x.ToString()+" "+DiscVect1[i].y.ToString()+" "+DiscVect1[i].z.ToString()+"\n";}
                    

                        //リストA[]====================================================
                        data_txt+="================注文数リストA===========================\n";
                        for(int i=0;i<Ntable1;i++){
                            if(TableNum1[i] <= 0){
                                for(int j=0;j<Nmenu1;j++)for(int k=0;k<Ndisc1 +1;k++) A2[k][i][j]=0;
                                TableNum1[i] = 0;
                            } 
                        }
                        for(int k=0;k<Ndisc1 +1;k++){
                            for(int i=0;i<Ntable1;i++){
                                for(int j=0;j<Nmenu1;j++) data_txt+=A2[k][i][j].ToString()+" ";
                                data_txt+="\n";
                            }
                        }

                        //リストB[]====================================================
                        data_txt+="================未済リストB===========================\n";
                        for(int i=0;i<Ntable1;i++){
                            if(TableNum1[i] == 0) for(int j=0;j<3;j++) B1[i][j]=false;
                            for(int j=0;j<3;j++) data_txt+=B1[i][j].ToString()+" ";
                            data_txt+="\n";
                        }
                        Debug.Log("ok");

                        //リストC[]====================================================
                        data_txt+="================y座標順リストC===========================\n";
                        for(int i=0;i<Ntable1;i++) data_txt+=C1[i].ToString()+" ";
                        data_txt+="\n";

                        //CanvasScaleなど==============================================
                        data_txt+="================画面スケーリング=========================\n";
                        data_txt+=CanScale1.x.ToString()+" "+CanScale1.y.ToString()+" "+CanScale1.z.ToString()+"\n";
                        data_txt+=CanPosition1.x.ToString()+" "+CanPosition1.y.ToString()+" "+CanPosition1.z.ToString()+"\n";
                        data_txt+=KCanScale1.x.ToString()+" "+KCanScale1.y.ToString()+" "+KCanScale1.z.ToString()+"\n";
                        data_txt+=KCanPosition1.x.ToString()+" "+KCanPosition1.y.ToString()+" "+KCanPosition1.z.ToString()+"\n";
                        
                    //======================DATAファイル書き換え=========================================
                        using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);
                    //======================version情報の書き換え===============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("6");
                    
                    break;

                case 6:
                        
                    //==============DATAファイル(ver6)の読込===============
                        ver_6_struct read_data = new ver_6_struct();
                        read_data.read(FolderPath);
                    //======================ver7形式のデータに変換===========
                        ver_7_struct write_data = new ver_7_struct();
                        write_data.Ntable = read_data.Ntable;
                        write_data.Nmenu = read_data.Nmenu;
                        write_data.Ngenre = read_data.Ngenre;
                        write_data.Ndisc = read_data.Ndisc;
                        write_data.Num = read_data.Num;
                        write_data.LastNum = read_data.LastNum;
                        write_data.TableName = read_data.TableName;
                        write_data.MenuName = read_data.MenuName;
                        write_data.GenreName = read_data.GenreName;
                        write_data.DiscName = read_data.DiscName;
                        write_data.MenuPrice = read_data.MenuPrice;
                        write_data.MenuGenre = read_data.MenuGenre;
                        write_data.DiscValue = read_data.DiscValue;
                        write_data.TableVect = read_data.TableVect;
                        write_data.MenuVect = read_data.MenuVect;
                        write_data.DiscVect = read_data.DiscVect;

                        write_data.OrderData = new List<ver_7_struct.GuestStruct>();
                        for(int i=0;i<write_data.Ntable;i++){
                            write_data.OrderData.Add(new ver_7_struct.GuestStruct());
                            write_data.OrderData[i].init();
                            write_data.OrderData[i].SetOrder.Add(new ver_7_struct.OrderStruct());
                            write_data.OrderData[i].SetOrder[0].init(read_data.Nmenu,read_data.Ndisc);
                            
                            write_data.OrderData[i].Number = read_data.TableNum[i];
                            write_data.OrderData[i].GivMenu = read_data.B[i][0];
                            write_data.OrderData[i].GivDrink = read_data.B[i][2];
                            write_data.OrderData[i].GetCash = read_data.B[i][1];


                            for(int j=0;j<write_data.Nmenu;j++){
                                write_data.OrderData[i].SetOrder[0].Order.Add(new List<int>());
                                for(int k=0;k<write_data.Ndisc+1;k++){
                                write_data.OrderData[i].SetOrder[0].Order[j].Add(read_data.A[k][i][j]);
                                }
                            }                               
                        }
                        
                        write_data.ExhOrd = read_data.ExhOrd;
                        write_data.CanScale = read_data.CanScale;
                        write_data.CanPosition = read_data.CanPosition;
                        write_data.KCanScale = read_data.KCanScale;
                        write_data.KCanPosition = read_data.KCanPosition;

                    //======================DATAファイル書き換え==============================
                        write_data.save(FolderPath);

                    //======================version情報の書き換え=============================
                        using (var fs = new StreamWriter(FolderPath + "/version.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write("7");
                    
                    break; 
            }
        }

    //
    //データセーブ用関数（今は他スクリプトとの整合性を合わせるためだけ）======================
        public void Save()
        {
            Data.save(FolderPath);
        }

    //
    // 端末の向きを取得する関数=====================================
        bool getPortrait() {
            bool result;

            if (Screen.width < Screen.height){
                
                result = true;
            }
            else{
                result = false;
            }
            return result;
        }
    //
    //ボタン関係======================================================
        void buttonClick(int num){
            canvas.SetActive(false);
            Data.Num=num;
            //numを引き渡してSceneを移動.
            SceneManager.LoadScene("MenuScene");
        }
        public void SetClick(){
            SetCanvas.SetActive(true);
        }
        public void SetTableClick(){
            SetCanvas.SetActive(false);
            canvas.SetActive(false);
            SceneManager.LoadScene("TableSetScene");
        }
        public void SetMenuClick(){
            SetCanvas.SetActive(false);
            canvas.SetActive(false);
            SceneManager.LoadScene("MenuSetScene");
        }
        public void SetCanvasClick(){
            SetCanvas.SetActive(false);
            canvas.SetActive(false);
            SceneManager.LoadScene("CanvasSetScene");
        }

        public void SetReturnClick(){
            SetCanvas.SetActive(false);
        }

    //===================================================================


    // Start is called before the first frame update
    void Start()
    {
        PrePortrait=true;
        string[] array=new string[3];

        OldPath = Application.persistentDataPath + "/DATA.txt";
        FolderPath = Application.persistentDataPath + "/CafeAppDATA";
        bool isAppend = false; // 上書きfalse or 追記true.

        //==========================versionの確認=================================
            VerCheck();
        //==============DATAファイルの読込(versionが上がり次第更新)===============
            Data.read(FolderPath);
        //==========================Canvasの編集==================================
            CanvasRect = CanvasPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = Data.CanPosition;
            CanvasRect.localScale = Data.CanScale;

            CanvasRect = SetPanel.GetComponent<RectTransform>();
            CanvasRect.anchoredPosition = Data.CanPosition;
            CanvasRect.localScale = Data.CanScale;
        //==========================ボタンの編集==================================
            Array.Resize(ref array, 3);
            GObuttons = new GameObject[Data.Ntable];
            buttons=new Button[Data.Ntable];      
                    
            for(int i=0;i<Data.Ntable;i++){

                int j=i;
                    
                GObuttons[i]=Instantiate(ButtonPrefab, canvas.transform);
                GObuttons[i].transform.parent = CanvasPanel.transform;
                buttons[i]=GObuttons[i].GetComponent<Button> ();
                buttons[i].onClick.AddListener(() => buttonClick(j)); 
                TableRect = GObuttons[i].GetComponent<RectTransform> ();
                buttonText = GObuttons[i].GetComponentInChildren<Text> (); 
                TableRect.anchoredPosition = Data.TableVect[i];
                TableRect.localScale = new Vector3(1,1,1);
                buttonText.text = Data.TableName[i];
            }

            SetButton.GetComponent<RectTransform>().SetAsLastSibling();
        //========================================================================

    }


    // Update is called once per frame
    void Update()
    {
        Portrait = getPortrait();
        if(PrePortrait != Portrait){
            if((Portrait==false)&&(SceneManager.GetActiveScene().name=="TableScene")){
                Save();
                canvas.SetActive(false);
                SceneManager.LoadScene("KitchenScene");
            }else if(Portrait&&(SceneManager.GetActiveScene().name=="KitchenScene")){
                canvas.SetActive(true);
                SceneManager.LoadScene("TableScene");
            }
        }
        PrePortrait = Portrait;
    }
    
}
