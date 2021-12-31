using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace YazılımSınamaProje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
        public static string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        String Substition(String textBin)
        {
            byte[] pIndis = { 2, 8, 12, 5, 9, 0, 14, 4, 11, 1, 15, 6, 3, 10, 7, 13 };
            String pTextBin = "";
            for (int i = 0; i < textBin.Length; i++)
            {
                pTextBin += (textBin[pIndis[i]]);
            }
            return pTextBin;
        }

        String ReSubstition(String textBin)
        {
            byte[] rpIndis = { 5, 9, 0, 12, 7, 3, 11, 14, 1, 4, 13, 8, 2, 15, 6, 10 };
            String rpTextBin = "";
            for (int i = 0; i < textBin.Length; i++)
            {
                rpTextBin += (textBin[rpIndis[i]]);
            }
            return rpTextBin;
        }
     
        public String getXOR(String bin1,String bin2)
        {

            int a = Convert.ToInt32(bin1, 2) ^ Convert.ToInt32(bin2, 2);
            

            return (Convert.ToString(a, 2).PadLeft(16, '0')); 
        }

        //String XOR_BinText(St)
        String sifreliTextBin = "";
       
        
        public void encrypt()
        {

            String metin = "me";
            String key = "security";

            String binMetin = StringToBinary(metin);
            while (binMetin.Length % 16 != 0)
            {
                //ŞAYET 16'NIN KATI DEGILSE MESAJ UZUNLUGU SAGINA 0 EKLE (ÇÖZÜME ETKİ ETMEZ).
                binMetin = binMetin.PadRight(binMetin.Length + 8, '0');
            }
            String binKey = StringToBinary(key);

            textBox1.Text = binMetin;

            String[] xKey = new String[4];
            xKey[0] = binKey.Substring(0, 16);
            xKey[1] = binKey.Substring(16, 16);
            xKey[2] = binKey.Substring(32, 16);
            xKey[3] = binKey.Substring(48, 16);

            for (int i = 0; i < binMetin.Length; i += 16)
            {
                String xor_text = getXOR(binMetin.Substring(i, 16), xKey[0]);
                String subsText = Substition(xor_text);

                xor_text = getXOR(subsText, xKey[1]);
                subsText = Substition(xor_text);

                xor_text = getXOR(subsText, xKey[2]);
                xor_text = getXOR(xor_text, xKey[3]);
                sifreliTextBin += xor_text;

            }

            textBox2.Text = sifreliTextBin;

            textBox4.Text = BinaryToString(sifreliTextBin);
            String deCrpytTextBin = "";
            for (int i = 0; i < sifreliTextBin.Length; i += 16)
            {
                String reXor = getXOR(sifreliTextBin.Substring(i, 16), xKey[3]);
                reXor = getXOR(reXor, xKey[2]);

                String reSubsText = ReSubstition(reXor);

                reXor = getXOR(reSubsText, xKey[1]);
                reSubsText = ReSubstition(reXor);

                reXor = getXOR(reSubsText, xKey[0]);

                deCrpytTextBin += reXor;



            }

            textBox3.Text = deCrpytTextBin;
            textBox5.Text = (BinaryToString(deCrpytTextBin));
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //String text = "me";
            //String text2 = "se";

            //textBox1.Text = StringToBinary(text);
            //textBox2.Text = StringToBinary(text2);


            encrypt();
           


            //string hashData = ComputeStringToSha256Hash(metin);
            //MessageBox.Show(metin, hashData);

        }

       
        
        
        
        static string ComputeStringToSha256Hash(string plainText)
        {
            // Create a SHA256 hash from string   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computing Hash - returns here byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // now convert byte array to a string   
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }

      
    }
}
