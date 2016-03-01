﻿namespace DS2_Backup_Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Media;
    using System.Windows.Forms;


     

    public partial class MainForm : Form
    {
        //private readonly string filePath =
        //    Path.Combine(
        //        (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
        //        "DarkSoulsII",
        //        "0110000102ee03bd", //TODO: Detect folder
        //        "DARKSII0000.sl2"); 

        //TODO: Remove global variables


      

        private readonly string BackupsPath;
        string vFileName; 
        string idFolder;
        private readonly Dictionary<int, string> dic;
        private readonly KeyboardHook hook = new KeyboardHook();
        private readonly SoundPlayer simpleSound;




         
        public MainForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            dic = new Dictionary<int, string>();
            BackupsPath = txtBackupPath.Text+"\\";
            simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
            openFileDialog1.InitialDirectory = Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),"DarkSoulsII");
            hook.KeyPressed += HookKeyPressed;
            hook.RegisterHotKey(Keys.F5, new ModifierKey());
            hook.RegisterHotKey(Keys.F8, new ModifierKey());
            hook.RegisterHotKey(Keys.Delete,new ModifierKey());
             
        }

        private string GetSavesLocation()
        {
            const string ds2s = "DS2SOFS0000.sl2";
            const string ds2o = "DARKSII0000.sl2";


            if (radioButtonDS2Orig.Checked)
                vFileName = ds2o;
            if (radioButtonDS2SOTFS.Checked)
                vFileName = ds2s;

            idFolder = Directory.GetDirectories(Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "DarkSoulsII"))[0];

            //  MessageBox.Show(idFolder);

            return Path.Combine(idFolder, vFileName);
        }
                    
        private void HookKeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F5:
                    BackupSave();
                    break;
                case Keys.F8:
                    LoadSave();
                    break;
                case Keys.Delete:
                    DeleteButton.PerformClick();
                    break;
            }
        }

        private void ShowFiles()
        {
            lstSaves.Items.Clear();
            dic.Clear();
            foreach (var file in Directory.GetFiles(BackupsPath))
            {
                lstSaves.Items.Add(Path.GetFileName(file) + "    " + File.GetLastWriteTime(file));
                if (lstSaves.Items.Count - 1 >= 0)
                    dic.Add(lstSaves.Items.Count - 1, file);
            }
            lstSaves.SelectedIndex = lstSaves.Items.Count - 1;
        }

        private void BackupSave()
        {
            Directory.GetFiles(BackupsPath);
            try
            {
                //File.Copy(GetSavesPath(),BackupsPath +"DARKSII0000.sl2" + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss,f"));
                File.Copy(GetSavesLocation(), BackupsPath +DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss,f") + " " +"DARKSII0000.sl2" );
                statusLabel.Text = @"The save was backed up";
            }
            catch (IOException ioException)
            {
                MessageBox.Show(ioException.Message);
            }
            ShowFiles();
           
        }

        private void LoadSave()
        {
            if (!File.Exists(GetSavesLocation()))
            {
                MessageBox.Show("Path "+GetSavesLocation()+" doesn't exist");
                return;
            }
            var count = lstSaves.Items.Count;
            if (count > 0)
            {
                File.Copy(dic[lstSaves.SelectedIndex], GetSavesLocation(), true);
                lstSaves.SelectedIndex = count - 1;
                //label2.Text = @"Save is restored  " + dic[savesListBox.SelectedIndex];
                statusLabel.Text = @"Save is restored  " + dic[lstSaves.SelectedIndex];
                simpleSound.Play();
            }
            else
            {
                MessageBox.Show(@"Backups not found in "+ BackupsPath);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void LoadButtonClick(object sender, EventArgs e)
        {
            //   MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"));
            LoadSave();
        }

        private void BackupButtonClick(object sender, EventArgs e)
        {
            BackupSave();
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            if (lstSaves.Items.Count > 0)
            {
                File.Delete(dic[lstSaves.SelectedIndex]);
                lstSaves.Items.Clear();
                ShowFiles();
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ShowFiles();
            lstSaves.SelectedIndex = lstSaves.Items.Count - 1;
            txtSavesPath.Text = GetSavesLocation();
        }

        private void BrowseSavesButtonClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txtSavesPath.Text = openFileDialog1.FileName;
        }

        private void BrowseBackupsButtonClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtBackupPath.Text = folderBrowserDialog1.SelectedPath;
        }


    }
}
// MessageBox.Show(new DirectoryInfo(Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "DarkSoulsII")).GetDirectories()[0].ToString());