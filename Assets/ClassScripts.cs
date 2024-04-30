using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CafeClasses
{
    //verごとのデータ構造クラス.

    public class current_struct : ver_7_struct{
        public void copy(current_struct original){
            this.init();
            this.Ntable = original.Ntable;
            this.Nmenu = original.Nmenu;
            this.Ngenre = original.Ngenre;
            this.Ndisc = original.Ndisc;
            this.Num = original.Num;
            this.LastNum = original.LastNum;
            this.TableName = original.TableName;
            this.MenuName = original.MenuName;
            this.GenreName = original.GenreName;
            this.DiscName = original.DiscName;
            this.MenuPrice = original.MenuPrice;
            this.MenuGenre = original.MenuGenre;
            this.DiscValue = original.DiscValue;
            this.TableVect = original.TableVect;
            this.MenuVect = original.MenuVect;
            this.DiscVect = original.DiscVect;
            this.ExhOrd = original.ExhOrd;
            this.CanScale = original.CanScale;
            this.CanPosition = original.CanPosition;
            this.KCanScale = original.KCanScale;
            this.KCanPosition = original.KCanPosition;
            this.OrderData.Clear();

            for(int i=0;i<this.Ntable;i++){
                this.OrderData.Add(new current_struct.GuestStruct());
                this.OrderData[i].init();
                this.OrderData[i].Number = original.OrderData[i].Number;
                this.OrderData[i].Nset = original.OrderData[i].Nset;
                this.OrderData[i].GivMenu = original.OrderData[i].GivMenu;
                this.OrderData[i].GivDrink = original.OrderData[i].GivDrink;
                this.OrderData[i].GetCash = original.OrderData[i].GetCash;

                for(int j=0;j<this.OrderData[i].Nset;j++ ){

                    this.OrderData[i].SetOrder.Add(new current_struct.OrderStruct());
                    this.OrderData[i].SetOrder[j].init(this.Nmenu,this.Ndisc);
                    for(int k=0;k<this.Nmenu;k++){
                        for(int l=0;l<this.Ndisc+1;l++){
                            this.OrderData[i].SetOrder[j].Order[k][l] = original.OrderData[i].SetOrder[j].Order[k][l];
                        }
                    }
                }
            }

        }
    }


    public class ver_7_struct{
        public int Ntable;
        public int Nmenu;
        public int Ngenre;
        public int Ndisc;
        public int Num;
        public int LastNum;
        public List<string> TableName;
        public List<string> MenuName;
        public List<string> GenreName;
        public List<string> DiscName;
        public List<int> MenuPrice;
        public List<int> MenuGenre;
        public List<int> DiscValue;
        public List<Vector3> TableVect;
        public List<Vector3> MenuVect;
        public List<Vector3> DiscVect;
        public List<GuestStruct> OrderData;
        public List<int> ExhOrd;
        public Vector3 CanScale,CanPosition,KCanScale,KCanPosition;

        public void init(){
            this.Ntable = 0;
            this.Nmenu = 0;
            this.Ngenre = 0;
            this.Ndisc = 0;
            this.Num = 0;
            this.LastNum = 0;
            this.TableName = new List<string>();
            this.MenuName = new List<string>();
            this.GenreName = new List<string>();
            this.DiscName = new List<string>();
            this.MenuPrice = new List<int>();
            this.MenuGenre = new List<int>();
            this.DiscValue = new List<int>();
            this.TableVect = new List<Vector3>();
            this.MenuVect = new List<Vector3>();
            this.DiscVect = new List<Vector3>();
            this.OrderData = new List<GuestStruct>();
            

            this.ExhOrd = new List<int>();
            this.CanScale = new Vector3();
            this.CanPosition = new Vector3();
            this.KCanScale = new Vector3();
            this.KCanPosition = new Vector3();

        }

        public class OrderStruct{
            public List<List<int>> Order;

            public void init(int Nmenu, int Ndisc){
                this.Order = new List<List<int>>();
                for(int j=0;j<Nmenu;j++){
                    this.Order.Add(new List<int>());
                    for(int k=0;k<Ndisc+1;k++){
                        this.Order[j].Add(0);
                    }
                }
            }

            public void copy(OrderStruct original){
                this.init(original.Order.Count,original.Order[0].Count);
                for(int j=0;j<original.Order.Count;j++){
                    for(int k=0;k<original.Order[0].Count;k++){
                        this.Order[j][k] = original.Order[j][k];
                    }
                }
            }
        }
        public class GuestStruct{
            public List<OrderStruct> SetOrder;
            public int Number;
            public int Nset;
            public bool GivMenu;
            public bool GivDrink;
            public bool GetCash;

            public void init(){
                this.Number = 0;
                this.Nset = 0;
                this.GivMenu = false;
                this.GivDrink = false;
                this.GetCash = false;
                this.SetOrder = new List<OrderStruct>();
            }

            public void AddSet(OrderStruct original){
                this.SetOrder.Add(new OrderStruct());
                SetOrder[Nset].copy(original);
                this.Nset ++;
            }
        }

        public void read(string FolderPath){
            this.init();
            string[] array=new string[3];

            //==============DATAファイル(ver7)の読込===============
                using (var fs = new StreamReader(FolderPath + "/DATA.txt", System.Text.Encoding.GetEncoding("UTF-8"))){
                    //==========================table関係の読込=============================
                        fs.ReadLine();
                        fs.ReadLine();//---テーブル数---
                        this.Ntable=int.Parse(fs.ReadLine());
                        
                        fs.ReadLine();//---テーブル名---
                        this.TableName.AddRange(fs.ReadLine().Split(' '));

                        fs.ReadLine();//---テーブル座標---
                        for(int i=0;i<this.Ntable;i++){
                            Array.Copy(fs.ReadLine().Split(' '), array, 3);
                            this.TableVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                        }
                    //============================menu・ジャンル関係の読込==============================
                        fs.ReadLine();
                        fs.ReadLine();//---メニュー数---
                        this.Nmenu=int.Parse(fs.ReadLine());
                        fs.ReadLine();//---ジャンル数---
                        this.Ngenre=int.Parse(fs.ReadLine());

                        fs.ReadLine();//---メニュー名---
                        this.MenuName.AddRange(fs.ReadLine().Split(' '));

                        fs.ReadLine();//---価格---
                        Array.Resize(ref array, this.Nmenu);
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Nmenu);
                        for(int i=0;i<this.Nmenu;i++) this.MenuPrice.Add(int.Parse(array[i]));

                        fs.ReadLine();//---ジャンル---
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Nmenu);
                        for(int i=0;i<this.Nmenu;i++) this.MenuGenre.Add(int.Parse(array[i]));
                        
                        fs.ReadLine();//---ジャンル名---
                        this.GenreName.AddRange(fs.ReadLine().Split(' '));
                        fs.ReadLine();//---メニュー座標---
                        Array.Resize(ref array, 3);
                        for(int i=0;i<this.Nmenu;i++){
                            Array.Copy(fs.ReadLine().Split(' '), array, 3);
                            this.MenuVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                        }
                    //========================= 割引関係の読み込み===================================

                        fs.ReadLine();
                        fs.ReadLine();//--- 割引種類数---
                        this.Ndisc=int.Parse(fs.ReadLine());

                        if(this.Ndisc!=0){
                            fs.ReadLine();//---割引名---
                            this.DiscName.AddRange(fs.ReadLine().Split(' '));
                            
                            fs.ReadLine();//---割引値---
                            Array.Resize(ref array, this.Ndisc);
                            Array.Copy(fs.ReadLine().Split(' '), array, this.Ndisc);
                            for(int i=0;i<this.Ndisc;i++) this.DiscValue.Add(int.Parse(array[i]));
                            Debug.Log(this.DiscValue[0]);
                            
                            fs.ReadLine();//---割引座標---
                            Array.Resize(ref array, 3);
                            for(int i=0;i<this.Ndisc;i++){
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                this.DiscVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                            }
                        }
                        else{
                            fs.ReadLine();//---割引名---
                            fs.ReadLine();
                            fs.ReadLine();//---割引値---
                            fs.ReadLine();
                            fs.ReadLine();//---割引座標---
                            fs.ReadLine();
                        }

                        fs.ReadLine(); //各席情報OrderData================================================

                        for(int i=0;i<this.Ntable;i++){
                            this.OrderData.Add(new ver_7_struct.GuestStruct());
                            this.OrderData[i].init();
                            fs.ReadLine();  //---table_n---
                            fs.ReadLine();  //--来客順--
                            this.OrderData[i].Number = int.Parse(fs.ReadLine());
                            fs.ReadLine();  //--セット数--
                            this.OrderData[i].Nset = int.Parse(fs.ReadLine());
                            fs.ReadLine();  //--未済--
                            this.OrderData[i].GivMenu = bool.Parse(fs.ReadLine());
                            this.OrderData[i].GivDrink = bool.Parse(fs.ReadLine());
                            this.OrderData[i].GetCash = bool.Parse(fs.ReadLine());

                            Array.Resize(ref array, this.Ndisc+1);
                            fs.ReadLine();  //---注文---
                            for(int j=0;j<this.OrderData[i].Nset;j++ ){

                                this.OrderData[i].SetOrder.Add(new ver_7_struct.OrderStruct());
                                this.OrderData[i].SetOrder[j].init(this.Nmenu,this.Ndisc);
                                fs.ReadLine();  //--set_n--
                                for(int k=0;k<this.Nmenu;k++){
                                    Array.Copy(fs.ReadLine().Split(' '), array, this.Ndisc+1);
                                    for(int l=0;l<this.Ndisc+1;l++){
                                        this.OrderData[i].SetOrder[j].Order[k][l] = int.Parse(array[l]);
                                    }
                                }
                            }

                            if(this.OrderData[i].Number >= this.LastNum ) this.LastNum = this.OrderData[i].Number;
                        }

                        fs.ReadLine();//---y座標順リストExhOrd---
                        Array.Resize(ref array, this.Ntable);
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Ntable);
                        for(int i=0;i<this.Ntable;i++) this.ExhOrd.Add(int.Parse(array[i]));

                    
                    //========================CanvasScaleなどの読込========================================
                        fs.ReadLine();//---画面スケーリング---
                        Array.Resize(ref array, 3);
                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.CanScale = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.CanPosition = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.KCanScale = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.KCanPosition = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));
                }
        }

        public void save(string FolderPath){
            //Table関連==================================================
            string data_txt;
            data_txt = "=================テーブル関係=======================\n---テーブル数---\n";
            data_txt+=this.Ntable.ToString();
            data_txt+="\n---テーブル名---\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.TableName[i]+" ";
            data_txt+="\n---テーブル座標---\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.TableVect[i].x.ToString()+" "+this.TableVect[i].y.ToString()+" "+this.TableVect[i].z.ToString()+"\n";

            //Menu・ジャンル関連===================================================
            data_txt+= "=================メニュー関係=======================\n---メニュー数---\n";
            data_txt+=this.Nmenu.ToString();
            data_txt+="\n---ジャンル数---\n";
            data_txt+=this.Ngenre.ToString();
            data_txt+="\n---メニュー名---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuName[i]+" ";
            data_txt+="\n---価格---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuPrice[i].ToString()+" ";
            data_txt+="\n---ジャンル---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuGenre[i].ToString()+" ";
            data_txt+="\n---ジャンル名---\n";
            for(int i=0;i<this.Ngenre;i++) data_txt+=this.GenreName[i]+" ";
            data_txt+="\n---メニュー座標---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuVect[i].x.ToString()+" "+this.MenuVect[i].y.ToString()+" "+this.MenuVect[i].z.ToString()+"\n";
        
            //割引関連=====================================================
            data_txt+="=================割引関係=======================\n---割引種類数---\n";
            data_txt+=this.Ndisc.ToString();
            data_txt+="\n---割引名---\n";
            for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscName[i]+" ";
            data_txt+="\n---割引値---\n";
            for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscValue[i].ToString()+" ";
            data_txt+="\n---割引座標---\n";
            if(this.Ndisc==0){data_txt+="\n";}
            else{for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscVect[i].x.ToString()+" "+this.DiscVect[i].y.ToString()+" "+this.DiscVect[i].z.ToString()+"\n";}
        

            //各席情報OrderData================================================
            data_txt+="================各席情報OrderData=============================\n";
            for(int i=0;i<this.Ntable;i++){
                data_txt+="---table_"+i.ToString()+"---\n";
                //来客順がマイナスのとき、注文データを初期化.
                if(this.OrderData[i].Number <= 0){
                    this.OrderData[i].SetOrder.Clear();
                    this.OrderData[i].init();
                    this.OrderData[i].SetOrder.Add(new ver_7_struct.OrderStruct());
                    this.OrderData[i].SetOrder[0].init(this.Nmenu,this.Ndisc);
                }
                data_txt+="--来客順--\n";
                data_txt+=this.OrderData[i].Number.ToString()+"\n";
                data_txt+="--セット数--\n";
                data_txt+=this.OrderData[i].Nset.ToString()+"\n";
                data_txt+="--未済--\n";
                data_txt+=this.OrderData[i].GivMenu.ToString()+"\n";
                data_txt+=this.OrderData[i].GivDrink.ToString()+"\n";
                data_txt+=this.OrderData[i].GetCash.ToString()+"\n";
                data_txt+="---注文---\n";
                
                for(int j=0;j<this.OrderData[i].Nset;j++ ){
                    data_txt+="--set"+j.ToString()+"--\n";
                    for(int k=0;k<this.Nmenu;k++){
                        for(int l=0;l<this.Ndisc+1;l++){
                            data_txt+= this.OrderData[i].SetOrder[j].Order[k][l].ToString()+" ";
                        }
                        data_txt+="\n";
                    }
                }
            }


            //リストExhOrd[]====================================================
            data_txt+="================y座標順リストExhOrd===========================\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.ExhOrd[i].ToString()+" ";
            data_txt+="\n";

            //CanvasScaleなど==============================================
            data_txt+="================画面スケーリング=========================\n";
            data_txt+=this.CanScale.x.ToString()+" "+this.CanScale.y.ToString()+" "+this.CanScale.z.ToString()+"\n";
            data_txt+=this.CanPosition.x.ToString()+" "+this.CanPosition.y.ToString()+" "+this.CanPosition.z.ToString()+"\n";
            data_txt+=this.KCanScale.x.ToString()+" "+this.KCanScale.y.ToString()+" "+this.KCanScale.z.ToString()+"\n";
            data_txt+=this.KCanPosition.x.ToString()+" "+this.KCanPosition.y.ToString()+" "+this.KCanPosition.z.ToString()+"\n";

            //書き込み======================================
            using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);

            Debug.Log("セーブしたで");

        }

    }

    public class ver_6_struct{
        public int Ntable;
        public int Nmenu;
        public int Ngenre;
        public int Ndisc;
        public int Num;
        public int LastNum;
        public List<string> TableName;
        public List<string> MenuName;
        public List<string> GenreName;
        public List<string> DiscName;
        public List<int> TableNum;
        public List<int> MenuPrice;
        public List<int> MenuGenre;
        public List<int> DiscValue;
        public List<Vector3> TableVect;
        public List<Vector3> MenuVect;
        public List<Vector3> DiscVect;
        public List<List<List<int>>> A;
        public List<List<bool>> B;
        public List<int> ExhOrd;
        public Vector3 CanScale,CanPosition,KCanScale,KCanPosition;
        
        public void init(){
            this.Ntable = 0;
            this.Nmenu = 0;
            this.Ngenre = 0;
            this.Ndisc = 0;
            this.Num = 0;
            this.LastNum = 0;
            this.TableName = new List<string>();
            this.MenuName = new List<string>();
            this.GenreName = new List<string>();
            this.DiscName = new List<string>();
            this.TableNum = new List<int>();
            this.MenuPrice = new List<int>();
            this.MenuGenre = new List<int>();
            this.DiscValue = new List<int>();
            this.TableVect = new List<Vector3>();
            this.MenuVect = new List<Vector3>();
            this.DiscVect = new List<Vector3>();
            this.A = new List<List<List<int>>>();
            this.B = new List<List<bool>>();
            this.ExhOrd = new List<int>();
            this.CanScale = new Vector3();
            this.CanPosition = new Vector3();
            this.KCanScale = new Vector3();
            this.KCanPosition = new Vector3();

        }

        public void read(string FolderPath){
            this.init();
            string[] array=new string[3];

            //==============DATAファイル(ver6)の読込===============
                using (var fs = new StreamReader(FolderPath + "/DATA.txt", System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    //==========================table関係の読込=============================
                        fs.ReadLine();
                        fs.ReadLine();//---テーブル数---
                        this.Ntable=int.Parse(fs.ReadLine());
                        
                        fs.ReadLine();//---テーブル名---
                        this.TableName.AddRange(fs.ReadLine().Split(' '));

                        fs.ReadLine();//---来客順---
                        Array.Resize(ref array, this.Ntable);
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Ntable);
                        for(int i=0;i<this.Ntable;i++) this.TableNum.Add(int.Parse(array[i]));
                        this.LastNum = this.TableNum.Max();

                        fs.ReadLine();//---テーブル座標---
                        for(int i=0;i<this.Ntable;i++){
                            Array.Copy(fs.ReadLine().Split(' '), array, 3);
                            this.TableVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                        }
                    //============================menu・ジャンル関係の読込==============================
                        fs.ReadLine();
                        fs.ReadLine();//---メニュー数---
                        this.Nmenu=int.Parse(fs.ReadLine());
                        fs.ReadLine();//---ジャンル数---
                        this.Ngenre=int.Parse(fs.ReadLine());

                        fs.ReadLine();//---メニュー名---
                        this.MenuName.AddRange(fs.ReadLine().Split(' '));

                        fs.ReadLine();//---価格---
                        Array.Resize(ref array, this.Nmenu);
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Nmenu);
                        for(int i=0;i<this.Nmenu;i++) this.MenuPrice.Add(int.Parse(array[i]));

                        fs.ReadLine();//---ジャンル---
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Nmenu);
                        for(int i=0;i<this.Nmenu;i++) this.MenuGenre.Add(int.Parse(array[i]));
                        
                        fs.ReadLine();//---ジャンル名---
                        this.GenreName.AddRange(fs.ReadLine().Split(' '));
                        fs.ReadLine();//---メニュー座標---
                        Array.Resize(ref array, 3);
                        for(int i=0;i<this.Nmenu;i++){
                            Array.Copy(fs.ReadLine().Split(' '), array, 3);
                            this.MenuVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                        }
                    //========================= 割引関係の読み込み===================================

                        fs.ReadLine();
                        fs.ReadLine();//--- 割引種類数---
                        this.Ndisc=int.Parse(fs.ReadLine());

                        if(this.Ndisc!=0){
                            fs.ReadLine();//---割引名---
                            this.DiscName.AddRange(fs.ReadLine().Split(' '));
                            
                            fs.ReadLine();//---割引値---
                            Array.Resize(ref array, this.Ndisc);
                            Array.Copy(fs.ReadLine().Split(' '), array, this.Ndisc);
                            for(int i=0;i<this.Ndisc;i++) this.DiscValue.Add(int.Parse(array[i]));
                            Debug.Log(this.DiscValue[0]);
                            
                            fs.ReadLine();//---割引座標---
                            Array.Resize(ref array, 3);
                            for(int i=0;i<this.Ndisc;i++){
                                Array.Copy(fs.ReadLine().Split(' '), array, 3);
                                this.DiscVect.Add(new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]))); 
                            }
                        }
                        else{
                            fs.ReadLine();//---割引名---
                            fs.ReadLine();
                            fs.ReadLine();//---割引値---
                            fs.ReadLine();
                            fs.ReadLine();//---割引座標---
                            fs.ReadLine();
                        }
                    //========================リストA[][],B[][],ExhOrd[]の読込==================================
                        Debug.Log(fs.ReadLine());//---注文数リストA---
                        Array.Resize(ref array, this.Nmenu);
                        for(int k=0;k<this.Ndisc+1;k++){
                            this.A.Add(new List<List<int>>());
                            for(int i=0;i<this.Ntable;i++){
                                
                                Array.Copy(fs.ReadLine().Split(' '), array, this.Nmenu);
                                this.A[k].Add(new List<int>());

                                for(int j=0;j<this.Nmenu;j++){
                                    this.A[k][i].Add(int.Parse(array[j]));
                                }
                            }
                        }

                        fs.ReadLine();//---未済リストB---
                        Array.Resize(ref array, 3);
                        for(int i=0;i<this.Ntable;i++){
                            this.B.Add(new List<bool>());
                            Array.Copy(fs.ReadLine().Split(' '), array, 3);
                            for(int j=0;j<3;j++) this.B[i].Add(bool.Parse(array[j]));
                        }

                        fs.ReadLine();//---y座標順リストC---
                        Array.Resize(ref array, this.Ntable);
                        Array.Copy(fs.ReadLine().Split(' '), array, this.Ntable);
                        for(int i=0;i<this.Ntable;i++) this.ExhOrd.Add(int.Parse(array[i]));
                    
                    //========================CanvasScaleなどの読込========================================
                        fs.ReadLine();//---画面スケーリング---
                        Array.Resize(ref array, 3);
                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.CanScale = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.CanPosition = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.KCanScale = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));

                        Array.Copy(fs.ReadLine().Split(' '), array, 3);
                        this.KCanPosition = new Vector3(float.Parse(array[0]),float.Parse(array[1]),float.Parse(array[2]));
                }

        }

        public void save(string FolderPath){
            //Table関連==================================================
            string data_txt;
            data_txt = "=================テーブル関係=======================\n---テーブル数---\n";
            data_txt+=this.Ntable.ToString();
            data_txt+="\n---テーブル名---\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.TableName[i]+" ";
            data_txt+="\n---来客順---\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.TableNum[i].ToString()+" ";
            data_txt+="\n---テーブル座標---\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.TableVect[i].x.ToString()+" "+this.TableVect[i].y.ToString()+" "+this.TableVect[i].z.ToString()+"\n";

            //Menu・ジャンル関連===================================================
            data_txt+= "=================メニュー関係=======================\n---メニュー数---\n";
            data_txt+=this.Nmenu.ToString();
            data_txt+="\n---ジャンル数---\n";
            data_txt+=this.Ngenre.ToString();
            data_txt+="\n---メニュー名---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuName[i]+" ";
            data_txt+="\n---価格---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuPrice[i].ToString()+" ";
            data_txt+="\n---ジャンル---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuGenre[i].ToString()+" ";
            data_txt+="\n---ジャンル名---\n";
            for(int i=0;i<this.Ngenre;i++) data_txt+=this.GenreName[i]+" ";
            data_txt+="\n---メニュー座標---\n";
            for(int i=0;i<this.Nmenu;i++) data_txt+=this.MenuVect[i].x.ToString()+" "+this.MenuVect[i].y.ToString()+" "+this.MenuVect[i].z.ToString()+"\n";
        
            //割引関連=====================================================
            data_txt+="=================割引関係=======================\n---割引種類数---\n";
            data_txt+=this.Ndisc.ToString();
            data_txt+="\n---割引名---\n";
            for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscName[i]+" ";
            data_txt+="\n---割引値---\n";
            for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscValue[i].ToString()+" ";
            data_txt+="\n---割引座標---\n";
            if(this.Ndisc==0){data_txt+="\n";}
            else{for(int i=0;i<this.Ndisc;i++) data_txt+=this.DiscVect[i].x.ToString()+" "+this.DiscVect[i].y.ToString()+" "+this.DiscVect[i].z.ToString()+"\n";}
        

            //リストA[]====================================================
            data_txt+="================注文数リストA===========================\n";
            for(int i=0;i<this.Ntable;i++){
                if(this.TableNum[i] <= 0){
                    for(int j=0;j<this.Nmenu;j++)for(int k=0;k<this.Ndisc+1;k++) this.A[k][i][j]=0;
                    this.TableNum[i] = 0;
                } 
            }
            for(int k=0;k<this.Ndisc+1;k++){
                for(int i=0;i<this.Ntable;i++){
                    for(int j=0;j<this.Nmenu;j++) data_txt+=this.A[k][i][j].ToString()+" ";
                    data_txt+="\n";
                }
            }

            //リストB[]====================================================
            data_txt+="================未済リストB===========================\n";
            for(int i=0;i<this.Ntable;i++){
                if(this.TableNum[i] == 0) for(int j=0;j<3;j++) this.B[i][j]=false;
                for(int j=0;j<3;j++) data_txt+=this.B[i][j].ToString()+" ";
                data_txt+="\n";
            }

            //リストC[]====================================================
            data_txt+="================y座標順リストC===========================\n";
            for(int i=0;i<this.Ntable;i++) data_txt+=this.ExhOrd[i].ToString()+" ";
            data_txt+="\n";

            //CanvasScaleなど==============================================
            data_txt+="================画面スケーリング=========================\n";
            data_txt+=this.CanScale.x.ToString()+" "+this.CanScale.y.ToString()+" "+this.CanScale.z.ToString()+"\n";
            data_txt+=this.CanPosition.x.ToString()+" "+this.CanPosition.y.ToString()+" "+this.CanPosition.z.ToString()+"\n";
            data_txt+=this.KCanScale.x.ToString()+" "+this.KCanScale.y.ToString()+" "+this.KCanScale.z.ToString()+"\n";
            data_txt+=this.KCanPosition.x.ToString()+" "+this.KCanPosition.y.ToString()+" "+this.KCanPosition.z.ToString()+"\n";

            //書き込み======================================
            using (var fs = new StreamWriter(FolderPath + "/DATA.txt", false, System.Text.Encoding.GetEncoding("UTF-8"))) fs.Write(data_txt);

            Debug.Log("セーブしたで");
        }
    }

}
