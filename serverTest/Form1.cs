using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;

namespace serverTest
{
    public partial class Form1 : Form
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        String deCrpytTextBin = "";
        String sifreliTextBin = "";

        public Form1()
        {
            InitializeComponent();
        }
        public string ComputeStringToSha256Hash(string plainText)
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
        public string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
        public  string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

      public  String Substition(String textBin)
        {
            byte[] pIndis = { 2, 8, 12, 5, 9, 0, 14, 4, 11, 1, 15, 6, 3, 10, 7, 13 };
            String pTextBin = "";
            for (int i = 0; i < textBin.Length; i++)
            {
                pTextBin += (textBin[pIndis[i]]);
            }
            return pTextBin;
        }

      public  String ReSubstition(String textBin)
        {
            byte[] rpIndis = { 5, 9, 0, 12, 7, 3, 11, 14, 1, 4, 13, 8, 2, 15, 6, 10 };
            String rpTextBin = "";
            for (int i = 0; i < textBin.Length; i++)
            {
                rpTextBin += (textBin[rpIndis[i]]);
            }
            return rpTextBin;
        }

        public String getXOR(String bin1, String bin2)
        {

            int a = Convert.ToInt32(bin1, 2) ^ Convert.ToInt32(bin2, 2);


            return (Convert.ToString(a, 2).PadLeft(16, '0'));
        }

        //String XOR_BinText(St)

        public void encrypt()
        {

            String metin = textBox3.Text;
            String key = keyBox.Text;

            String binMetin = StringToBinary(metin);
            while (binMetin.Length % 16 != 0)
            {
                //ŞAYET 16'NIN KATI DEGILSE MESAJ UZUNLUGU SAGINA 0 EKLE (ÇÖZÜME ETKİ ETMEZ).
                binMetin = binMetin.PadRight(binMetin.Length + 8, '0');
            }
            String binKey = StringToBinary(key);

            //textBox1.Text = binMetin;

            String[] xKey = new String[4];
            xKey[0] = binKey.Substring(0, 16);
            xKey[1] = binKey.Substring(16, 16);
            xKey[2] = binKey.Substring(32, 16);
            xKey[3] = binKey.Substring(48, 16);

            for (int i = 0; i < binMetin.Length; i += 16)
            {
                String xor_text = getXOR(binMetin.Substring(i, 16), xKey[0]);
                String subsText = Substition(xor_text);
                textBox4.Text = xor_text;
                xor_text = getXOR(subsText, xKey[1]);
                subsText = Substition(xor_text);

                xor_text = getXOR(subsText, xKey[2]);
                xor_text = getXOR(xor_text, xKey[3]);
                sifreliTextBin += xor_text;

            }

            //textBox2.Text = sifreliTextBin;



            //textBox4.Text = BinaryToString(sifreliTextBin);

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

            // textBox3.Text = deCrpytTextBin;
            // textBox5.Text = (BinaryToString(deCrpytTextBin));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (spnBox.Checked)
            {
                encrypt();
            }
            else if (shaBox.Checked)
            {
               String hashMetin = ComputeStringToSha256Hash(textBox3.Text);
                textBox3.Text = hashMetin;
            }
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox3.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            readData = "Conected to Chat Server ...";
            msg("security", true);
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox1.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();

        }

        byte ilkMesaj = 0;

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                //msg();
                if (ilkMesaj < 1)
                {
                    ilkMesaj++;
                    msg(keyBox.Text, true);
                }
                else
                    msg(keyBox.Text);
                




            }
        }

        private void msg(string key, bool direkYaz = false)
        {
            if (key == "security"  || direkYaz==true)
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(() => msg(key,true)));
                else
                    textBox2.Text = textBox2.Text + Environment.NewLine + " >> " + readData;
            }
            else
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(() => msg(key)));
                else
                    textBox2.Text = textBox2.Text + Environment.NewLine + " >> " + "kilitli mesdaj var";
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {

            
        }
    }
}
