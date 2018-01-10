using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
namespace GameClient
{
    public partial class FormPlaying : Form
    {
        private int tableIndex;
        private int side;
        private DotColor[,] grid = new DotColor[15, 15]; //������ɫ����������ʱ�����ж�
        private Bitmap blackBitmap;
        private Bitmap whiteBitmap;
        private int hold = 0;              //��ɫ�ȳ���
        //�����Ƿ����Է�����
        private bool isReceiveCommand = false;
        private Service service;
        delegate void LabelDelegate(Label label, string str);
        delegate void ButtonDelegate(Button button, bool flag);
        delegate void RadioButtonDelegate(RadioButton radioButton, bool flag);
        delegate void SetDotDelegate(int i, int j, int dotColor);
        LabelDelegate labelDelegate;
        ButtonDelegate buttonDelegate;
        RadioButtonDelegate radioButtonDelegate;
        public FormPlaying(int TableIndex, int Side, StreamWriter sw)
        {
            InitializeComponent();
            this.tableIndex = TableIndex;
            this.side = Side;
            labelDelegate = new LabelDelegate(SetLabel);
            buttonDelegate = new ButtonDelegate(SetButton);
            radioButtonDelegate = new RadioButtonDelegate(SetRadioButton);
            blackBitmap = new Bitmap(Properties.Resources.black);
            whiteBitmap = new Bitmap(Properties.Resources.white);
            service = new Service(listBox1, sw);
        }
        /// <summary>���봰��ʱ�������¼�</summary>
        private void FormPlaying_Load(object sender, EventArgs e)
        {
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = DotColor.None;
                }
            }
            labelSide0.Text = "";
            labelSide1.Text = "";
        }
        /// <summary>���ñ�ǩ��ʾ��Ϣ</summary>
        /// <param name="Label">Ҫ���õ�Label</param>
        /// <param name="string">Ҫ���õ���Ϣ</param>
        public void SetLabel(Label label, string str)
        {
            if (label.InvokeRequired)
            {
                this.Invoke(labelDelegate, label, str);
            }
            else
            {
                label.Text = str;
            }
        }
        /// <summary>����button�Ƿ����</summary>
        /// <param name="Button">Ҫ���õ�Button</param>
        /// <param name="flag">�Ƿ����</param>
        private void SetButton(Button button, bool flag)
        {
            if (button.InvokeRequired)
            {
                this.Invoke(buttonDelegate, button, flag);
            }
            else
            {
                button.Enabled = flag;
            }
        }
        /// <summary>����radioButtonѡ��״̬</summary>
        /// <param name="radioButton">Ҫ���õ�RadioButton</param>
        /// <param name="flag">�Ƿ�ѡ��</param>
        private void SetRadioButton(RadioButton radioButton, bool flag)
        {
            if (radioButton.InvokeRequired)
            {
                this.Invoke(radioButtonDelegate, radioButton, flag);
            }
            else
            {
                radioButton.Checked = flag;
            }
        }
        /// <summary>��������״̬</summary>
        /// <param name="i">�ڼ���</param>
        /// <param name="j">�ڼ���</param>
        /// <param name="dotColor">������ɫ</param>
        public void SetDot(int i, int j, DotColor dotColor)
        {
            service.AddItemToListBox(string.Format("{0},{1},{2}", i, j, dotColor));
            grid[i, j] = dotColor;
            pictureBox1.Invalidate();
        }
        /// <summary>���¿�ʼ����Ϸ</summary>
        /// <param name="str">������Ϣ</param>
        public void Restart(string str)
        {
            MessageBox.Show(str, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ResetGrid();
            SetButton(buttonStart, true);
        }
        /// <summary>��������</summary>
        public void ResetGrid()
        {
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = DotColor.None;
                }
            }
            pictureBox1.Invalidate();
        }
        /// <summary>ȡ������</summary>
        /// <param name="x">x����</param>
        /// <param name="y">y����</param>
        public void UnsetDot(int x, int y)
        {
            grid[x / 20 - 1, y / 20 - 1] = DotColor.None;
            pictureBox1.Invalidate();
        }
        /// <summary>����ͼ��</summary>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    if (grid[i, j] != DotColor.None)
                    {
                        if (grid[i, j] == DotColor.Black)
                        {
                            g.DrawImage(blackBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else
                        {
                            g.DrawImage(whiteBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                    }
                }
            }
        }

        /// <summary>�������������Ϣ</summary>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //�ַ�����ʽ��Talk,����,�Ի�����
            service.SendToServer(string.Format("Talk,{0},{1}", tableIndex, textBox1.Text));
        }
        /// <summary>�Ի����ݸı�ʱ�������¼�</summary>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                //�ַ�����ʽ��Talk,����,�Ի�����
                service.SendToServer(string.Format("Talk,{0},{1}", tableIndex, textBox1.Text));
            }
        }
        /// <summary>���������ťʱ�������¼�</summary>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string str =
                "�������������岻����ô�����������\n";
            MessageBox.Show(str, "������Ϣ");
        }
        /// <summary>�����ʼ��ťʱ�������¼�</summary>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            service.SendToServer(string.Format("Start,{0},{1}", tableIndex, side));
            this.buttonStart.Enabled = false;
        }
        /// <summary>����˳���ťʱ�������¼�</summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>�رմ���ʱ�������¼�</summary>
        private void FormPlaying_FormClosing(object sender, FormClosingEventArgs e)
        {
            //��ʽ��GetUp,����,��λ��
            service.SendToServer(string.Format("GetUp,{0},{1}", tableIndex, side));
        }
        /// <summary>FormRoom�е��̵߳��ô˷����رմ˴���</summary>
        public void StopFormPlaying()
        {
            Application.Exit();
        }
        /// <summary>��pictureBox1�а�����괥�����¼�</summary>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (hold == side)
            {
                int x = e.X / 20;
                int y = e.Y / 20;
                if (!(x < 1 || x > 15 || y < 1 || y > 15))
                {
                    if (grid[x - 1, y - 1] == DotColor.None)
                    {
                        int color = (int)grid[x - 1, y - 1];
                        //���͸�ʽ��SetDot,����,��λ��,��,��,��ɫ
                        service.SendToServer(string.Format(
                           "setDot,{0},{1},{2},{3}", tableIndex, side, x - 1, y - 1));
                    }
                }
            }
            else
            {
                MessageBox.Show("���ڲ�������ӡ�");
            }
            
        }
        //�ı䵱ǰ���ӷ�
        public void changeHold()
        {
            hold = (hold + 1) % 2;
        }
        public void sendRestart()
        {
            service.SendToServer(string.Format("restar,{0}",tableIndex));
        }
        /// <summary>
        /// ���������Ϣ����ʽ����λ�ţ�labelSide��ʾ����Ϣ��listbox��ʾ����Ϣ
        /// </summary>
        /// <param name="sideString">ָ�����</param>
        /// <param name="labelSideString">labelSide��ʾ����Ϣ</param>
        /// <param name="listBoxString">listbox��ʾ����Ϣ</param>
        public void SetTableSideText(string sideString, string labelSideString, string listBoxString)
        {
            string s = "�׷�";
            if (sideString == "0")
            {
                s = "�ڷ���";
            }
            //�ж��Լ��Ǻڷ����ǰ׷�
            if (sideString == side.ToString())
            {
                SetLabel(labelSide1, s + labelSideString);
            }
            else
            {
                SetLabel(labelSide0, s + labelSideString);
            }
            service.AddItemToListBox(listBoxString);
        }
        /// <summary>��ʾ̸����Ϣ</summary>
        /// <param name="talkMan"≯����</param>
        /// <param name="str">Ҫ��ʾ����Ϣ</param>
        public void ShowTalk(string talkMan, string str)
        {
            service.AddItemToListBox(string.Format("{0}˵��{1}", talkMan, str));
        }
        /// <summary>
        /// ��ʾ��Ϣ
        /// </summary>
        /// <param name="str">Ҫ��ʾ����Ϣ</param>
        public void ShowMessage(string str)
        {
            service.AddItemToListBox(str);
        }

        private void buttonTie_Click(object sender, EventArgs e)
        {
            if (side == 0)
            {
                service.SendToServer(string.Format(
                                           "Tie,{0},{1}", tableIndex, side));
            }
            if (side == 1)
            {
                service.SendToServer(string.Format("Tie,{0},{1}", tableIndex, side));
            }
        }


    }
}