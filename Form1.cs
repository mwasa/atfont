using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace at_font
{

    public partial class Form1 : Form
    {
        int offset;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

     
        }

        private void LoadFont(string path)
        {
            listView1.BeginUpdate();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.CheckBoxes = true;

            String str;

            StreamReader sr = new StreamReader(path, Encoding.UTF32);

            str = sr.ReadToEnd();

            sr.Close();

            listView1.Columns.Add("seq");
            listView1.Columns.Add("char");
            listView1.Columns.Add("hex");
            listView1.Columns.Add("h");

            listView1.Columns.Add("w");
            listView1.Columns.Add("x");
            listView1.Columns.Add("y");

            byte[] aa = Encoding.UTF32.GetBytes(str);

            byte[] fileBytes = null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);
            }

            byte[] aa2 = fileBytes;

            offset = Convert.ToInt32(boxoffset.Text);

            for (int i = 0; i < ((aa.Length - offset) / 12); i++)
            {
                ListViewItem item = new ListViewItem(i.ToString());



                String str2 = Encoding.UTF32.GetString(aa, (i * 12) + offset + 8, 4);

                String str3 = string.Format("{0:X2}", aa[(i * 12) + offset + 8 + 3]) + string.Format("{0:X2}", aa[(i * 12) + offset + 8 + 2])
                            + string.Format("{0:X2}", aa[(i * 12) + offset + 8 + 1]) + string.Format("{0:X2}", aa[(i * 12) + offset + 8]);

                int w, h, x, y;
                item.SubItems.Add(str2);
                item.SubItems.Add(str3);
                if ((i * 12) + offset + 8 + 11 > aa.Length) break;

                byte[] bytes_w = { aa2[(i * 12) + offset + 8 + 4], aa2[(i * 12) + offset + 8 + 5], 0, 0 };
                byte[] bytes_h = { aa2[(i * 12) + offset + 8 + 6], aa2[(i * 12) + offset + 8 + 7], 0, 0 };
                byte[] bytes_x = { aa2[(i * 12) + offset + 8 + 8], aa2[(i * 12) + offset + 8 + 9], 0, 0 };
                byte[] bytes_y = { aa2[(i * 12) + offset + 8 + 10], aa2[(i * 12) + offset + 8 + 11], 0, 0 };

                item.SubItems.Add(BitConverter.ToInt32(bytes_w, 0).ToString());
                item.SubItems.Add(BitConverter.ToInt32(bytes_h, 0).ToString());
                item.SubItems.Add(BitConverter.ToInt32(bytes_x, 0).ToString());
                item.SubItems.Add(BitConverter.ToInt32(bytes_y, 0).ToString());

                listView1.Items.Add(item);

            }

            listView1.EndUpdate();
        }

        public string str2hex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.UTF32.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:X2}", byteStr);

            return resultHex;
        }
        public static int byteToInt(byte[] source)
        {

            int result =

                source[3] << 24 |

                (source[2] & 0xff) << 16 |

                (source[1] & 0xff) << 8 |

                (source[0] & 0xff);

            return result;

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string pathSource = textBox2.Text;
            string pathNew = textBox3.Text;
            int offset = 81;

            try
            {
                using (FileStream fsSource = new FileStream(pathSource,
                    FileMode.Open, FileAccess.Read))
                {
                    // Read the source file into a byte array.
                    byte[] bytes = new byte[20];
                    byte[] outputbytes = new byte[0];

                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    byte[] bytestemp = new byte[offset];
                    fsSource.Read(bytestemp, 0, offset);

                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(bytes, 0, 20);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;


                        byte[] bytes12 = { bytes[0], bytes[1], bytes[2], bytes[3],
                        bytes[8], bytes[9], bytes[10], bytes[11],
                        bytes[4], bytes[5], bytes[6], bytes[7] };

                        outputbytes = Combine(outputbytes, bytes12);
                    }
                    numBytesToRead = outputbytes.Length;

                  using (FileStream fsNew = new FileStream(pathNew,
                        FileMode.Create, FileAccess.Write))
                    {
                        fsNew.Write(outputbytes, 0, numBytesToRead);
                    }
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Console.WriteLine(ioEx.Message);
            }

        }

        private byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c;
            if (a == null)
            {
                c = new byte[b.Length];
            }
            else c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string fileFullName = ofd.FileName;
                LoadFont(fileFullName);
            }
            else if (dr == DialogResult.Cancel)
            {
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {


        }
    }
}
