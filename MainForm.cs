using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using XASuMaoUtils;

namespace Source2Doc
{
    public class MainForm : Form
    {
        private string[,] CodeTypeList = new string[5, 2]
        {
      {
        "C",
        "*.h;*.c;*.cu"
      },
      {
        "C++",
        "*.h;*.cpp;*.c;*.cu;*.hpp;*.cuh"
      },
      {
        "C#",
        "*.cs"
      },
      {
        "C#/WPF",
        "*.cs;*.xaml"
      },
      {
        "Java/Eclipse",
        "*.java;*.xml"
      }
        };
        private bool IsExit;
        private string[] files;
        private int lable1ClickCount;
        private string newfilename;
        private IContainer components;
        private Label lable1;
        private TextBox SystemName;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label5;
        private Button BtnAddFile;
        private Button btnDelete;
        private Button BtnQuit;
        private ListView FileList;
        private TextBox SystemVer;
        private TextBox AllPdfName;
        private ComboBox CodeType;
        private ProgressBar progressBar;
        private GroupBox 文档设置;
        private GroupBox 源文件列表;
        private Button btnDown;
        private Button btnUP;
        private CheckBox FilesAllCheck;
        private Button Btn_AddAllFiles;
        private Label labRegTip;
        private Label label6;
        private ToolTip toolTip;
        private Button btn_StartTxt;
        private BackgroundWorker backgroundWorkerCompute;

        public MainForm()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
            this.InitializeComponent();
            for (int index = 0; index < 5; ++index)
                this.CodeType.Items.Add((object)this.CodeTypeList[index, 0]);
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            if (this.btn_StartTxt.Enabled)
            {
                this.IsExit = true;
                this.Close();
            }
            else
            {
                this.backgroundWorkerCompute.CancelAsync();
                this.BtnQuit.Enabled = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.IsExit = false;
            this.CodeType.SelectedIndex = 0;
            this.FileList.Columns.Add("文件", 180, HorizontalAlignment.Left);
            this.FileList.Columns.Add("源程序全路径", 500, HorizontalAlignment.Left);
            //new Thread(new ThreadStart(this.Verification)).Start();
            IsExit = true;
            RegState.AlreadyReg = RegState.SoftwareState.Valid;
            RegState.expiredTime = DateTime.Now.AddDays(1);
            this.toolTip.SetToolTip((Control)this.SystemName, "软件作品的名称，与申请表必须完全一致");
            this.toolTip.SetToolTip((Control)this.SystemVer, "软件版本号，可以带V，也可以不带，但最终需与申请表一致");
            this.toolTip.SetToolTip((Control)this.AllPdfName, "生成全部源代码文档，留用备查");
            this.toolTip.SetToolTip((Control)this.BtnAddFile, "添加一个源代码文件到列表中");
            this.toolTip.SetToolTip((Control)this.Btn_AddAllFiles, "选择一个源代码文件，会将该目录及子目录下符合类型的文件自动添加到列表中");
            this.toolTip.SetToolTip((Control)this.FileList, "源文件列表清单");
            this.toolTip.SetToolTip((Control)this.btnUP, "将选中的一个文件向上调整顺序");
            this.toolTip.SetToolTip((Control)this.btnDown, "将选中的一个文件向下调整顺序");
            this.toolTip.SetToolTip((Control)this.labRegTip, "请测试可以正常生成后再注册以去掉水印文字");
            this.toolTip.SetToolTip((Control)this.CodeType, "选择源程序的编程语言类型");
            this.toolTip.SetToolTip((Control)this.btn_StartTxt, "列表中的文件均会被整理并生成源代码文档,格式为.doc");
            this.toolTip.SetToolTip((Control)this.btnDelete, "从源程序列表中删除选中的文件");
        }

        private void Verification()
        {
            //while (!this.IsExit)
            //{
            //  Thread.Sleep(500);
            //  bool HasReg = new ValidReg().onlineValid();
            //  this.ShowReg(HasReg);
            //  if (HasReg)
            //    this.MySleep(600);
            //  else
            //    this.MySleep(20);
            //}
        }

        private void MySleep(int second)
        {
            while (second > 0)
            {
                Thread.Sleep(1000);
                --second;
                if (this.IsExit)
                    break;
            }
        }

        private void ShowReg(bool HasReg)
        {
            if (!this.labRegTip.InvokeRequired)
            {
                if (HasReg)
                {
                    this.labRegTip.Text = "已注册！有效期到" + RegState.expiredTime.ToShortDateString();
                    this.labRegTip.Visible = true;
                    this.label6.Visible = true;
                }
                else if (RegState.AlreadyReg == RegState.SoftwareState.NoConnect)
                {
                    this.labRegTip.Text = "本机未联网(被防火墙阻断?)，无法注册！";
                    this.labRegTip.Click -= new EventHandler(this.labRegTip_Click);
                    this.labRegTip.Visible = true;
                }
                else
                {
                    this.labRegTip.Text = "未注册，点击注册！";
                    this.labRegTip.Visible = true;
                }
            }
            else
                this.Invoke((Delegate)new MainForm.ShowRegDelegate(this.ShowReg), (object)HasReg);
        }

        private void BtnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            int selectedIndex = this.CodeType.SelectedIndex;
            openFileDialog.Filter = this.CodeTypeList[selectedIndex, 0] + "文件(" + this.CodeTypeList[selectedIndex, 1] + ")|" + this.CodeTypeList[selectedIndex, 1] + "|所有文件|*.*";
            openFileDialog.ValidateNames = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "添加源代码文件，包括头文件、源程序文件等";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string[] safeFileNames = openFileDialog.SafeFileNames;
            string[] fileNames = openFileDialog.FileNames;
            for (int index = 0; index < openFileDialog.FileNames.Length; ++index)
                this.FileList.Items.Add(new ListViewItem()
                {
                    Text = safeFileNames[index],
                    SubItems = {
            fileNames[index]
          }
                });
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem checkedItem in this.FileList.CheckedItems)
                this.FileList.Items.RemoveAt(checkedItem.Index);
        }

        private void AllPdfName_Leave(object sender, EventArgs e)
        {
            if (this.AllPdfName.Text.IndexOf(':') < 0)
                return;
            int num = (int)MessageBox.Show("不能添加路径！");
        }

        private void FilesAllCheck_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.FilesAllCheck.Checked;
            int count = this.FileList.Items.Count;
            this.FileList.BeginUpdate();
            for (int index = 0; index < count; ++index)
                this.FileList.Items[index].Checked = flag;
            this.FileList.EndUpdate();
        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            if (this.FileList.CheckedItems.Count == 1)
            {
                ListViewItem checkedItem = this.FileList.CheckedItems[0];
                int index = checkedItem.Index;
                if (index <= 0)
                    return;
                this.FileList.BeginUpdate();
                this.FileList.Items.RemoveAt(index);
                this.FileList.Items.Insert(index - 1, checkedItem);
                this.FileList.EndUpdate();
            }
            else
            {
                int num = (int)MessageBox.Show("一次只能移动个文件的顺序！");
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.FileList.CheckedItems.Count == 1)
            {
                ListViewItem checkedItem = this.FileList.CheckedItems[0];
                int index = checkedItem.Index;
                if (index >= this.FileList.Items.Count - 1)
                    return;
                this.FileList.BeginUpdate();
                this.FileList.Items.RemoveAt(index);
                this.FileList.Items.Insert(index + 1, checkedItem);
                this.FileList.EndUpdate();
            }
            else
            {
                int num = (int)MessageBox.Show("一次只能移动个文件的顺序！");
            }
        }

        public void getFileName(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string codeType = this.CodeTypeList[this.CodeType.SelectedIndex, 1];
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Extension.Length > 0 && codeType.IndexOf(file.Extension) >= 0)
                    this.FileList.Items.Add(new ListViewItem()
                    {
                        Text = file.Name,
                        SubItems = {
              file.FullName
            }
                    });
            }
        }

        public void AddDirectoryFiles2List(string path)
        {
            this.getFileName(path);
            foreach (FileSystemInfo directory in new DirectoryInfo(path).GetDirectories())
                this.AddDirectoryFiles2List(directory.FullName);
        }

        private void Btn_AddAllFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            int selectedIndex = this.CodeType.SelectedIndex;
            openFileDialog.Filter = this.CodeTypeList[selectedIndex, 0] + "文件(" + this.CodeTypeList[selectedIndex, 1] + ")|" + this.CodeTypeList[selectedIndex, 1] + "|所有文件|*.*";
            openFileDialog.ValidateNames = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "添加源代码主文件,系统自动搜索其余文件......";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string safeFileName = openFileDialog.SafeFileName;
            string fileName = openFileDialog.FileName;
            int length = fileName.IndexOf(safeFileName);
            if (length == -1)
            {
                int num = (int)MessageBox.Show("选择的主文件错误，请重新选择！");
            }
            else
            {
                string path = fileName.Substring(0, length);
                this.FileList.BeginUpdate();
                this.AddDirectoryFiles2List(path);
                foreach (ListViewItem listViewItem in this.FileList.Items)
                {
                    if (fileName.Equals(listViewItem.SubItems[1].Text))
                    {
                        this.FileList.Items.RemoveAt(listViewItem.Index);
                        this.FileList.Items.Insert(0, listViewItem);
                        break;
                    }
                }
                this.FileList.EndUpdate();
            }
        }

        private void labRegTip_Click(object sender, EventArgs e)
        {
            new ValidReg().OpenValidPage();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.xasumao.cn/contact-us.asp");
        }

        private void btn_StartTxt_Click(object sender, EventArgs e)
        {
            int count = this.FileList.Items.Count;
            if (count < 1)
                return;
            this.btnDelete.Enabled = false;
            this.BtnAddFile.Enabled = false;
            this.btnDown.Enabled = false;
            this.btnUP.Enabled = false;
            this.Btn_AddAllFiles.Enabled = false;
            this.btn_StartTxt.Enabled = false;
            this.progressBar.Visible = true;
            this.BtnQuit.Text = "停  止";
            this.progressBar.Maximum = count;
            this.files = new string[count];
            for (int index = 0; index < count; ++index)
                this.files[index] = this.FileList.Items[index].SubItems[1].Text;
            this.backgroundWorkerCompute.RunWorkerAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.IsExit = true;
            base.OnClosing(e);
        }

        private void lable1_Click(object sender, EventArgs e)
        {
            if (this.lable1ClickCount < 10)
                ++this.lable1ClickCount;
            else
                WDLog.IsDebug = true;
        }

        private void backgroundWorkerCompute_DoWork(object sender, DoWorkEventArgs e)
        {
            SourceDeal sourceDeal = new SourceDeal(this.AllPdfName.Text.ToString(), RegState.AlreadyReg != RegState.SoftwareState.Valid ? this.SystemName.Text.ToString() + "-未注册版-" + this.SystemVer.Text.ToString() + "     源代码" : this.SystemName.Text.ToString() + "   " + this.SystemVer.Text.ToString() + "     源代码");
            int count = this.FileList.Items.Count;
            for (int index = 0; index < count; ++index)
            {
                string file = this.files[index];
                sourceDeal.AddFile2Word(file);
                this.backgroundWorkerCompute.ReportProgress(index + 1);
                if (this.backgroundWorkerCompute.CancellationPending)
                    break;
            }
            this.newfilename = sourceDeal.Finish();
        }

        private void backgroundWorkerCompute_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorkerCompute_RunWorkerCompleted(
          object sender,
          RunWorkerCompletedEventArgs e)
        {
            do
            {
                Thread.Sleep(20);
            }
            while (string.IsNullOrEmpty(this.newfilename));
            if (this.AllPdfName.Text != this.newfilename)
            {
                this.AllPdfName.Text = this.newfilename;
                int num = (int)MessageBox.Show("设定的文件打开冲突，系统新建了输出文件：" + this.newfilename, "提示");
            }
            Process.Start(this.newfilename);
            this.progressBar.Visible = false;
            this.btnDelete.Enabled = true;
            this.BtnAddFile.Enabled = true;
            this.btnDown.Enabled = true;
            this.btnUP.Enabled = true;
            this.Btn_AddAllFiles.Enabled = true;
            this.btn_StartTxt.Enabled = true;
            this.BtnQuit.Text = "退  出";
            this.BtnQuit.Enabled = true;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = (args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "")).Replace(".", "_");
            return name.EndsWith("_resources") ? (Assembly)null : Assembly.Load((byte[])new ResourceManager(Assembly.GetEntryAssembly().GetTypes()[0].Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly()).GetObject(name));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
            this.lable1 = new Label();
            this.SystemName = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label5 = new Label();
            this.CodeType = new ComboBox();
            this.BtnAddFile = new Button();
            this.btnDelete = new Button();
            this.BtnQuit = new Button();
            this.FileList = new ListView();
            this.SystemVer = new TextBox();
            this.AllPdfName = new TextBox();
            this.progressBar = new ProgressBar();
            this.文档设置 = new GroupBox();
            this.源文件列表 = new GroupBox();
            this.Btn_AddAllFiles = new Button();
            this.FilesAllCheck = new CheckBox();
            this.btnDown = new Button();
            this.btnUP = new Button();
            this.labRegTip = new Label();
            this.label6 = new Label();
            this.toolTip = new ToolTip(this.components);
            this.btn_StartTxt = new Button();
            this.backgroundWorkerCompute = new BackgroundWorker();
            this.文档设置.SuspendLayout();
            this.源文件列表.SuspendLayout();
            this.SuspendLayout();
            this.lable1.AutoSize = true;
            this.lable1.Location = new Point(15, 67);
            this.lable1.Margin = new Padding(6, 0, 6, 0);
            this.lable1.Name = "lable1";
            this.lable1.Size = new Size(88, 25);
            this.lable1.TabIndex = 0;
            this.lable1.Text = "软件名称";
            this.lable1.Click += new EventHandler(this.lable1_Click);
            this.SystemName.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.SystemName.Location = new Point(181, 59);
            this.SystemName.Margin = new Padding(6);
            this.SystemName.Name = "SystemName";
            this.SystemName.Size = new Size(318, 29);
            this.SystemName.TabIndex = 1;
            this.SystemName.Text = "机场信息集成系统";
            this.label1.AutoSize = true;
            this.label1.ForeColor = SystemColors.MenuHighlight;
            this.label1.Location = new Point(16, 46);
            this.label1.Margin = new Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(687, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "源程序请按顺序加入,最先加入的文件会优先生成文档,系统不会自动判断文件重复";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(15, 311);
            this.label2.Margin = new Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(164, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "输出全量备查文件";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(15, 433);
            this.label3.Margin = new Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(88, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "软件类别";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(15, 189);
            this.label5.Margin = new Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(107, 25);
            this.label5.TabIndex = 6;
            this.label5.Text = "软件版本号";
            this.CodeType.DisplayMember = "1";
            this.CodeType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.CodeType.FormattingEnabled = true;
            this.CodeType.Location = new Point(181, 431);
            this.CodeType.Margin = new Padding(6);
            this.CodeType.Name = "CodeType";
            this.CodeType.Size = new Size(318, 33);
            this.CodeType.TabIndex = 7;
            this.BtnAddFile.Location = new Point(335, 460);
            this.BtnAddFile.Margin = new Padding(6);
            this.BtnAddFile.Name = "BtnAddFile";
            this.BtnAddFile.Size = new Size(139, 41);
            this.BtnAddFile.TabIndex = 8;
            this.BtnAddFile.Text = "增加文件";
            this.BtnAddFile.UseVisualStyleBackColor = true;
            this.BtnAddFile.Click += new EventHandler(this.BtnAddFile_Click);
            this.btnDelete.Location = new Point(190, 460);
            this.btnDelete.Margin = new Padding(6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(133, 41);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Tag = (object)"";
            this.btnDelete.Text = "取消文件";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.BtnQuit.Location = new Point(304, 591);
            this.BtnQuit.Margin = new Padding(6);
            this.BtnQuit.Name = "BtnQuit";
            this.BtnQuit.Size = new Size(194, 56);
            this.BtnQuit.TabIndex = 10;
            this.BtnQuit.Text = "退  出";
            this.BtnQuit.UseVisualStyleBackColor = true;
            this.BtnQuit.Click += new EventHandler(this.BtnQuit_Click);
            this.FileList.CheckBoxes = true;
            this.FileList.GridLines = true;
            this.FileList.Location = new Point(20, 88);
            this.FileList.Margin = new Padding(6);
            this.FileList.Name = "FileList";
            this.FileList.Size = new Size(651, 360);
            this.FileList.TabIndex = 12;
            this.FileList.UseCompatibleStateImageBehavior = false;
            this.FileList.View = View.Details;
            this.SystemVer.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.SystemVer.Location = new Point(181, 183);
            this.SystemVer.Margin = new Padding(6);
            this.SystemVer.Name = "SystemVer";
            this.SystemVer.Size = new Size(318, 29);
            this.SystemVer.TabIndex = 13;
            this.SystemVer.Text = "V1.0";
            this.AllPdfName.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.AllPdfName.Location = new Point(181, 307);
            this.AllPdfName.Margin = new Padding(6);
            this.AllPdfName.Name = "AllPdfName";
            this.AllPdfName.Size = new Size(318, 29);
            this.AllPdfName.TabIndex = 14;
            this.AllPdfName.Text = "AllCode.docx";
            this.AllPdfName.Leave += new EventHandler(this.AllPdfName_Leave);
            this.progressBar.Location = new Point(18, 567);
            this.progressBar.Margin = new Padding(6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(1262, 12);
            this.progressBar.TabIndex = 16;
            this.progressBar.Visible = false;
            this.文档设置.Controls.Add((Control)this.AllPdfName);
            this.文档设置.Controls.Add((Control)this.SystemVer);
            this.文档设置.Controls.Add((Control)this.label5);
            this.文档设置.Controls.Add((Control)this.label2);
            this.文档设置.Controls.Add((Control)this.SystemName);
            this.文档设置.Controls.Add((Control)this.CodeType);
            this.文档设置.Controls.Add((Control)this.label3);
            this.文档设置.Controls.Add((Control)this.lable1);
            this.文档设置.Location = new Point(18, 37);
            this.文档设置.Margin = new Padding(6);
            this.文档设置.Name = "文档设置";
            this.文档设置.Padding = new Padding(6);
            this.文档设置.Size = new Size(531, 521);
            this.文档设置.TabIndex = 17;
            this.文档设置.TabStop = false;
            this.文档设置.Text = "文档设置";
            this.源文件列表.Controls.Add((Control)this.Btn_AddAllFiles);
            this.源文件列表.Controls.Add((Control)this.FilesAllCheck);
            this.源文件列表.Controls.Add((Control)this.btnDown);
            this.源文件列表.Controls.Add((Control)this.btnUP);
            this.源文件列表.Controls.Add((Control)this.FileList);
            this.源文件列表.Controls.Add((Control)this.label1);
            this.源文件列表.Controls.Add((Control)this.btnDelete);
            this.源文件列表.Controls.Add((Control)this.BtnAddFile);
            this.源文件列表.Location = new Point(568, 37);
            this.源文件列表.Name = "源文件列表";
            this.源文件列表.Size = new Size(712, 521);
            this.源文件列表.TabIndex = 18;
            this.源文件列表.TabStop = false;
            this.源文件列表.Text = "源文件列表";
            this.Btn_AddAllFiles.Location = new Point(486, 459);
            this.Btn_AddAllFiles.Margin = new Padding(6);
            this.Btn_AddAllFiles.Name = "Btn_AddAllFiles";
            this.Btn_AddAllFiles.Size = new Size(139, 41);
            this.Btn_AddAllFiles.TabIndex = 16;
            this.Btn_AddAllFiles.Text = "主文件+...";
            this.Btn_AddAllFiles.UseVisualStyleBackColor = true;
            this.Btn_AddAllFiles.Click += new EventHandler(this.Btn_AddAllFiles_Click);
            this.FilesAllCheck.AutoSize = true;
            this.FilesAllCheck.Location = new Point(47, 466);
            this.FilesAllCheck.Name = "FilesAllCheck";
            this.FilesAllCheck.Size = new Size(107, 29);
            this.FilesAllCheck.TabIndex = 15;
            this.FilesAllCheck.Text = "全选文件";
            this.FilesAllCheck.UseVisualStyleBackColor = true;
            this.FilesAllCheck.CheckedChanged += new EventHandler(this.FilesAllCheck_CheckedChanged);
            this.btnDown.Location = new Point(672, 324);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new Size(31, 105);
            this.btnDown.TabIndex = 14;
            this.btnDown.Text = "v";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new EventHandler(this.btnDown_Click);
            this.btnUP.Location = new Point(672, 141);
            this.btnUP.Name = "btnUP";
            this.btnUP.Size = new Size(31, 105);
            this.btnUP.TabIndex = 13;
            this.btnUP.Text = "^";
            this.btnUP.UseVisualStyleBackColor = true;
            this.btnUP.Click += new EventHandler(this.btnUP_Click);
            this.labRegTip.AutoSize = true;
            this.labRegTip.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.labRegTip.ForeColor = SystemColors.MenuHighlight;
            this.labRegTip.Location = new Point(996, 607);
            this.labRegTip.Margin = new Padding(6, 0, 6, 0);
            this.labRegTip.Name = "labRegTip";
            this.labRegTip.Size = new Size(106, 21);
            this.labRegTip.TabIndex = 19;
            this.labRegTip.Text = "本软件已注册";
            this.labRegTip.Visible = false;
            this.labRegTip.Click += new EventHandler(this.labRegTip_Click);
            this.label6.AutoSize = true;
            this.label6.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.label6.Location = new Point(20, 608);
            this.label6.Name = "label6";
            this.label6.Size = new Size(275, 20);
            this.label6.TabIndex = 20;
            this.label6.Text = "如果使用中有问题，请到官网留言，谢谢！";
            this.label6.Visible = false;
            this.label6.Click += new EventHandler(this.label6_Click);
            this.btn_StartTxt.Location = new Point(744, 590);
            this.btn_StartTxt.Margin = new Padding(6);
            this.btn_StartTxt.Name = "btn_StartTxt";
            this.btn_StartTxt.Size = new Size(194, 56);
            this.btn_StartTxt.TabIndex = 21;
            this.btn_StartTxt.Text = "开始生成";
            this.btn_StartTxt.UseVisualStyleBackColor = true;
            this.btn_StartTxt.Click += new EventHandler(this.btn_StartTxt_Click);
            this.backgroundWorkerCompute.WorkerReportsProgress = true;
            this.backgroundWorkerCompute.WorkerSupportsCancellation = true;
            this.backgroundWorkerCompute.DoWork += new DoWorkEventHandler(this.backgroundWorkerCompute_DoWork);
            this.backgroundWorkerCompute.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorkerCompute_ProgressChanged);
            this.backgroundWorkerCompute.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorkerCompute_RunWorkerCompleted);
            this.AutoScaleDimensions = new SizeF(11f, 25f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1292, 661);
            this.Controls.Add((Control)this.btn_StartTxt);
            this.Controls.Add((Control)this.label6);
            this.Controls.Add((Control)this.labRegTip);
            this.Controls.Add((Control)this.文档设置);
            this.Controls.Add((Control)this.progressBar);
            this.Controls.Add((Control)this.BtnQuit);
            this.Controls.Add((Control)this.源文件列表);
            this.Font = new Font("微软雅黑", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Margin = new Padding(6);
            this.MaximizeBox = false;
            this.Name = nameof(MainForm);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "西安苏茂软件著作权申请文件自动生成系统";
            this.Load += new EventHandler(this.MainForm_Load);
            this.文档设置.ResumeLayout(false);
            this.文档设置.PerformLayout();
            this.源文件列表.ResumeLayout(false);
            this.源文件列表.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private delegate void ShowRegDelegate(bool a);
    }
}
