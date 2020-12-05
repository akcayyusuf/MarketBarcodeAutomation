using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing.Text;

namespace yusufabi2
{
   
    public partial class Form1 : Form
    {
       
        MySqlConnection connect;
        MySqlDataAdapter adap;
        
        string br;
        double holdkdv;
        string plu, quan;
       
        public Form1()
        {

            InitializeComponent();
            timer1.Start();
            

            Control.CheckForIllegalCrossThreadCalls = false;

            connect = new MySqlConnection("Server=127.0.0.1;Database=yusa;Uid=root;Pwd='';");      //MYSQL BAĞLANTISI TANIMLANDI.
            connect.Open();                                 //BAĞLANTI AÇILDI
            dataGridView1.Columns[0].Width = 300;
            dataGridView1.Columns[1].Width = 110;
            dataGridView1.Columns[2].Width = 110;       //KOLUN GENİİŞLİK ÖLÇÜLERİ
            dataGridView1.Columns[3].Width = 110;
            dataGridView1.Columns[5].Width = 70;
            dataGridView1.Columns[4].Width = 70;
            dataGridView1.Columns[5].HeaderText = "";


        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
            textBox1.Focus();
            
        }
    
       

        public void textBox1_TextChanged(object sender, EventArgs e)
       {

            br = textBox1.Text.ToString();      //BARKOD DEĞERİNİ TUTAN DEĞİŞKEN
            
            if (textBox1.Text.Length == 13) //BARKOD 13 HANELİ OLDUĞU İÇİN
            {

                if (textBox1.Text.StartsWith("869"))        //HAZIR ÜRÜN MÜ?
                {
                    sec("hazir_urunler");
                    label2.Text = "Ürün Adı: " + dataGridView2.Rows[0].Cells[1].Value.ToString().ToUpper();
                    label3.Text = "Ürün Kodu: " + dataGridView2.Rows[0].Cells[2].Value.ToString();
                    label4.Text = "Miktar: " + "1" + " Adet";

                    label6.Text = "Br. Fiyat: " + dataGridView2.Rows[0].Cells[8].Value.ToString() + " ₺";
                    label5.Text = "Fiyat: " + (Convert.ToDouble(dataGridView2.Rows[0].Cells[8].Value.ToString()) + " ₺");
                    addtable(dataGridView2.Rows[0].Cells[1].Value.ToString(), "1",Convert.ToDouble( dataGridView2.Rows[0].Cells[8].Value.ToString()), Convert.ToDouble(dataGridView2.Rows[0].Cells[8].Value.ToString()), 0, Convert.ToInt32(dataGridView2.Rows[0].Cells[9].Value.ToString()));
                    textBox1.Clear();
                }
                else
                {
                    plu = br.Substring(2, 5);   //3. KARAKTERDEN SONRA GELEN 4 KARAKTER PLU KODUNU TUTUYOR
                    quan = br.Substring(8, 4);  //8. KARAKTERDEN SONRA GELEN 4 KARAKTER KİLO MİKTARINI TUTUYOR
                 


                    sec("cerezler");
                    label2.Text = "Ürün Adı: " + dataGridView2.Rows[0].Cells[1].Value.ToString().ToUpper();
                    label3.Text = "Ürün Kodu: " + dataGridView2.Rows[0].Cells[3].Value.ToString();
                    label4.Text = "Miktar: " + quan[0].ToString() + "," + quan.Substring(1).ToString() + " kg";

                    label6.Text = "Br. Fiyat: " + dataGridView2.Rows[0].Cells[2].Value.ToString() + " ₺";
                    label5.Text = "Fiyat: " + (Convert.ToDouble(dataGridView2.Rows[0].Cells[2].Value.ToString()) * Convert.ToDouble(quan[0].ToString() + "," + quan.Substring(1).ToString())).ToString() + " ₺";

                    addtable(dataGridView2.Rows[0].Cells[1].Value.ToString(), quan[0].ToString() + "," + quan.Substring(1).ToString(), (Convert.ToDouble(dataGridView2.Rows[0].Cells[2].Value.ToString()) * Convert.ToDouble(quan[0].ToString() + "," + quan.Substring(1).ToString())), Convert.ToDouble(dataGridView2.Rows[0].Cells[2].Value),1,18);
                    textBox1.Clear();
                }
            }
        }
        private void textbox1_KeyPress(object sender ,KeyPressEventArgs e) 
        {
              if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            var senderGrid = (DataGridView)sender;


            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewImageColumn && e.RowIndex >= 0)
            {



                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);
                dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
                total();

            }
        }           //TABLODAN SATIR KALDIR
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            var senderGrid = (DataGridView)sender;


            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewImageColumn && e.RowIndex >= 0)
            {

                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);
                dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
                total();

            }
        }   //TABLODAN SATIR KALDIR

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        public void sec(string nerden)                                                    //VERİ TABANINDAN ÜRÜNÜ SEÇ BARKODU SORGULA 
        {
            DataTable brstation = new DataTable();
            if (dataGridView2.Rows.Count != 0)          //SAÇMA AMA GİZLİ BİR TABLO VAR ÖNCE SATIRI ONA ATIYORUM ORDAN ASIL TABLOYA ÇEKİYORUM
            {
                brstation.Clear();
                //dataGridView2.Rows.RemoveAt(0);         //2. SATIR HİÇBİR ZAMAN EKLENMEMELİ HER ZAMAN MEVCUT SATIR OKUNUR ASIL TABLOYA EKLENİR
            }
            if (nerden == "cerezler")
            {
                string sorgu = "SELECT * FROM cerezler WHERE plu = " + plu.ToString() + ";";  //VERİ TABANI ARAŞTIRMASI
                adap = new MySqlDataAdapter(sorgu, connect);
                adap.Fill(brstation);                           //YALANCI TABLOYU DOLDUR
                dataGridView2.DataSource = brstation;
            }
            else if(nerden=="hazir_urunler")
            {
         
                string sorgu2 = "SELECT * FROM hazir_urunler WHERE barkod = " + textBox1.Text.ToString() + ";";  //VERİ TABANI ARAŞTIRMASI
             
                adap = new MySqlDataAdapter(sorgu2, connect);
                adap.Fill(brstation);                           //YALANCI TABLOYU DOLDUR
                dataGridView2.DataSource = brstation;
            }
         
            

        }
        public void addtable(string ad, string qnt, double price, double brprice,int hometown,int kdv)       //ASIL TABOYA ÜRÜN EKLE
        {
            if (hometown==1)
            {
                
                    dataGridView1.Rows.Add(ad, qnt + " kg", brprice + " ₺", price + " ₺", kdv, (System.Drawing.Image)Properties.Resources.rw3);
                    dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
                
            }
            else 
            {
                if (check(ad)==-1)
                {
                    dataGridView1.Rows.Add(ad, qnt + " Ad", brprice + " ₺",Convert.ToDouble(brprice)*Convert.ToDouble(qnt) + " ₺", kdv, (System.Drawing.Image)Properties.Resources.rw3);
                    dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
                }
                else 
                {
                    DataGridViewCell cell = dataGridView1.Rows[check(ad)].Cells[1];
                    DataGridViewCell costcell = dataGridView1.Rows[check(ad)].Cells[3];
                    cell.Value = (1 + Convert.ToInt32(cell.Value.ToString().Substring(0,cell.Value.ToString().Length-2 )))+" Ad";                    //ADETİN OLDUĞU CELL
                    costcell.Value = Convert.ToDouble(brprice) * Convert.ToDouble(cell.Value.ToString().Substring(0, cell.Value.ToString().Length - 2))+ " ₺";


                }
            }
            dataGridView1.Refresh();
            total();   
        }
        public void total()  //FİYATI HESAPLA
        {
            double toplam = 0;
            double tkdv=0;
            for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
            {
                toplam = toplam + Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString().Substring(0, dataGridView1.Rows[i].Cells[3].Value.ToString().Length - 2));
                tkdv=tkdv +( Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString().Substring(0, dataGridView1.Rows[i].Cells[3].Value.ToString().Length - 2))/100)*Convert.ToDouble( dataGridView1.Rows[i].Cells[4].Value);
            }
            holdkdv = tkdv;
            label1.Text = toplam.ToString() + " ₺";

        }
        private void print()        //FİŞ YAZDIRMA İŞLEMLERİ 1
        {

            PrintDialog pd = new PrintDialog();
            PrintDocument doc = new PrintDocument();

            pd.Document = doc;
            doc.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            
            doc.Print();
           

        }

        private void button3_Click(object sender, EventArgs e)  //FİŞ YAZDIRMA İŞLEMLERİ 2
        {
            timer1.Stop();
            print();
            dataGridView1.Rows.Clear();
            dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
            label1.Text = "0 ₺";
            timer1.Start();
        }

        private void button4_Click(object sender, EventArgs e)      //ÖDEME İPTALİ TÜM TABLOYU BOŞALTIR
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Height = Convert.ToInt32(dataGridView1.Rows.Count.ToString()) * 50 + 60;
            label1.Text = "0 ₺";
            textBox1.Focus();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

      
        void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)       //FİŞ YAZDIRMA İŞLEMLERİ 3
        {
            Graphics graphic = e.Graphics;
            Font font = new Font("Courier New", 8);
            float fontheihght = font.GetHeight();
            int startX = 10;
            int startY = 10;
            int offset = 40;

            graphic.DrawString("        YUŞA KURUYEMİŞ", new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY);

            graphic.DrawString("TARİH: ".PadRight(8) + DateTime.Now.ToString().PadLeft(30), font, new SolidBrush(Color.Black), startX, startY + offset - 15);
            offset = offset + 20;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                
                string name = dataGridView1.Rows[i].Cells[0].Value.ToString().Replace(" ",string.Empty);
                string price = dataGridView1.Rows[i].Cells[3].Value.ToString().PadLeft(7);
                string adet="";
                string line;
                if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains("Ad")) 
                {
                    adet = dataGridView1.Rows[i].Cells[1].Value.ToString().Substring(0, dataGridView1.Rows[i].Cells[1].Value.ToString().Length - 2);
                    name = name + " X" + adet;
                }
               
                if (name.Length > 30 &&name.Length<=60)
                {
                    string splitter = name.Substring(0, 30);
                    string rest = name.Substring(30, name.Length - 30);

                    graphic.DrawString(splitter, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + ((int)fontheihght)-3;

                    graphic.DrawString(rest.PadRight(31)+price, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + ((int)fontheihght) - 3;


                }
                else if(name.Length>60)
                {
                    string s1= name.Substring(0, 30);
                    string s2= name.Substring(30, 30);
                    string rest = name.Substring(60, name.Length - 60);

                    graphic.DrawString(s1, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + ((int)fontheihght) - 3;

                    graphic.DrawString(s2, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + ((int)fontheihght) - 3;

                    graphic.DrawString(rest.PadRight(31) + price, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + ((int)fontheihght) - 3;

                }
                else if(name.Length<=30)
                {
                    line = name.PadRight(31) + price;

                    graphic.DrawString(line, font, new SolidBrush(Color.Black), startX, startY + offset);

                    offset = offset + ((int)fontheihght) - 3;
                }
                offset = offset +3;
            }
            offset = offset + 20;
            graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);

            offset = offset + 10;
            graphic.DrawString("KDV:".PadRight(12) + (holdkdv + " ₺").ToString().PadLeft(26), font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 20;
            graphic.DrawString("TOPLAM:".PadRight(20) + label1.Text.ToString().PadLeft(18), font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 10;
            graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 20;
            graphic.DrawString("Yine Bekleriz...".PadRight(38), font, new SolidBrush(Color.Black), startX, startY + offset);

        }

    

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBox1.Focused==true)

            {
                panel4.BackColor = Color.LawnGreen;
                
            }
            else
            {
                timer1.Stop();
                panel4.BackColor = Color.Red;
                
                textBox1.Focus();
                timer1.Start();
            }
        }

        private void shop_Click(object sender, EventArgs e)
        {
            renkac(panel7);
        }

        private void storage_Click(object sender, EventArgs e)
        {
            renkac(panel8);
          
        }

        private void chart_Click(object sender, EventArgs e)
        {
           
            renkac(panel9);
           
        }

        private void add_Click(object sender, EventArgs e)
        {
            renkac(panel10);
        }

        private void settings_Click(object sender, EventArgs e)
        {
            renkac(panel11);
        }

        public int check(string searchfor) 
        {
            int isexist=-1;
            for(int i = 0; i < dataGridView1.Rows.Count; i++) 
            {
                if (searchfor == dataGridView1.Rows[i].Cells[0].Value.ToString()) 
                {
                    isexist = i;
                    break;
                }
                
            }
            return isexist;
        }
    public void renkac(Panel pnl) 
        {
            panel7.Visible = false;
            panel8.Visible = false;
            panel9.Visible = false;
            panel10.Visible = false;
            panel11.Visible = false;
            pnl.Visible = true;
            pnl.BackColor = Color.FromArgb(20, 130, 140);
        }

    }
}


