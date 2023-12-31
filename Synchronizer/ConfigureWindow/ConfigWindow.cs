﻿using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AZLog;
using Microsoft.Win32;
using Dic = AZDictionary.Dictionary;
using Type = AZLog.Type;

namespace az.Synchronizer
{
    public partial class ConfigWindow : Form
    {
        public ConfigWindow()
        {
            Logger.Log("ConfigWindow.ConfigWindow","Initializing..", AZLog.Type.Loading);

            InitializeComponent();

            // Cleanup strings and fill Config area.
            foreach (string key in Functions.ConfigurationDictionary.Keys)
            {
                if (key == "src")
                {
                    Functions.ConfigurationDictionary.TryGetValue(key, out string text);
                    this.srcTxtBox.Text = text;
                    Logger.Log(this.Name + ".ConfigWindow", "Line of ConfigFile: " + key + "=" + text);
                }
                else if (key == "trgt")
                {
                    Functions.ConfigurationDictionary.TryGetValue(key, out string text);
                    this.targetTxtBox.Text = text;
                    Logger.Log(this.Name + ".ConfigWindow", "Line of ConfigFile: " + key + "=" + text);
                }
                else
                {
                    Logger.Log(this.Name + ".ConfigWindow", "Empty or Wrong line at Config File", Type.Warning);
                }
            }

            // Fill Settings area.
            foreach (string key in Functions.SettingsDictionary.Keys)
            {
                Functions.SettingsDictionary.TryGetValue(key, out string value);
                if (key == "startWithWindows")
                {
                    this.startAutoBox.Checked = Convert.ToBoolean(value);
                    Logger.Log(this.Name+".ConfigWindow", "Adding "+ Convert.ToBoolean(value)+ " To startAutoBox.", Type.Saving);
                }
                else if (key == "settingShowWarning")
                {
                    this.settingShowWarning.Checked = Convert.ToBoolean(value);
                    Logger.Log(this.Name + ".ConfigWindow", "Adding " + Convert.ToBoolean(value) + " To settingShowWarning.", Type.Saving);
                }
                else if (key == "showCurrentStatusInBalloonTip")
                {
                    this.showCurrentProgressAsTooltip.Checked = Convert.ToBoolean(value);
                    Logger.Log(this.Name + ".ConfigWindow", "Adding " + Convert.ToBoolean(value) + " To showCurrentStatusInBalloonTip.", Type.Saving);
                }
                else if (key == "showWindowOnChanges")
                {
                    this.showWindowOnChanges.Checked = Convert.ToBoolean(value);
                    Logger.Log(this.Name + ".ConfigWindow", "Adding " + Convert.ToBoolean(value) + " To showWindowOnChanges.", Type.Saving);
                }
                else if (key == "popUpWhenExternalConnected")
                {
                    this.popUpWhenExternalConnected.Checked = Convert.ToBoolean(value);
                    Logger.Log(this.Name + ".ConfigWindow", "Adding " + Convert.ToBoolean(value) + " To popUpWhenExternalConnected.", Type.Saving);
                }
            }

            Logger.Log("ConfigWindow.ConfigWindow", "Initialization completed.", AZLog.Type.Success);
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            Logger.Log(this.Name + ".OpenFileDialog", "Opening FolderBrowserDialog().", AZLog.Type.Opening);
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog()
            {
                Description = StringTable.SetupFolderBrowserDescription,
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = ((Button)sender).Name == "sourceBtn" ? srcTxtBox.Text : targetTxtBox.Text,
                ShowNewFolderButton = true
            };

            folderBrowser.ShowDialog();

            string path = folderBrowser.SelectedPath;

            switch (((Button)sender).Name)
            {
                case ("sourceBtn"):
                    srcTxtBox.Text = path;
                    Logger.Log(this.Name + ".OpenFileDialog", "sourceBtn clicked.");
                    break;
                case ("targetBtn"):
                    targetTxtBox.Text = path;
                    Logger.Log(this.Name + ".OpenFileDialog", "targetBtn clicked.");
                    break;
                default:
                    Logger.Log(this.Name + ".OpenFileDialog", "Default triggered, something went wrong.", AZLog.Type.Warning);
                    break;
            }

            Logger.Log(this.Name + ".OpenFileDialog", "Closing FolderBrowserDialog().", AZLog.Type.Closing);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            #region Config file

            Logger.Log(this.Name + ".saveBtn_Click", "Updating Config File.", Type.Opening);
            StreamWriter writerConf = new StreamWriter(Dic.SyncConfigFile);
            writerConf.WriteLine("src=" + srcTxtBox.Text + ","+ "trgt=" + targetTxtBox.Text + ",");
            writerConf.Flush();
            Logger.Log(this.Name + ".saveBtn_Click", "Saving Config File.", Type.Saving);
            writerConf.Close();
            Logger.Log(this.Name + ".saveBtn_Click", "Closing Config File.", Type.Closing);
            Logger.Log(this.Name + ".saveBtn_Click", "Showing 'Config is updated' message.");
            MessageBox.Show(StringTable.ConfigConfigIsSaved);
            writerConf.Close();
            
            #endregion

            #region Settings

            Logger.Log(this.Name + ".saveBtn_Click", "Updating Settings file.", Type.Opening);
            StreamWriter writerSetting = new StreamWriter(Dic.SyncSettingFile);
            writerSetting.WriteLine("startWithWindows=" + startAutoBox.Checked.ToString()+";");
            writerSetting.WriteLine("settingShowWarning=" + settingShowWarning.Checked.ToString()+";");
            writerSetting.WriteLine("showCurrentStatusInBalloonTip=" + showCurrentProgressAsTooltip.Checked.ToString()+";");
            writerSetting.WriteLine("showWindowOnChanges=" + showWindowOnChanges.Checked.ToString() + ";");
            writerSetting.WriteLine("popUpWhenExternalConnected=" + popUpWhenExternalConnected.Checked.ToString() + ";");
            Logger.Log(this.Name + ".saveBtn_Click", "Saving Setting File.", Type.Saving);
            writerSetting.Flush();
            Logger.Log(this.Name + ".saveBtn_Click", "Closing Setting File.", Type.Closing);
            Logger.Log(this.Name + ".saveBtn_Click", "Showing 'Setting is updated' message.");
            MessageBox.Show(StringTable.ConfigSettingIsSaved);
            writerSetting.Close();

            #endregion

            Logger.Log(this.Name + ".saveBtn_Click", "Closing Configuration Dialog", Type.Closing);

            Functions.GetConfigStrings();

            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Logger.Log(this.Name + ".cancelBtn_Click", "Closing Configuration Dialog", Type.Closing);
            this.Close();
        }

        private void startAutoBox_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (startAutoBox.Checked)
            {
                rk.SetValue("ContextApplication", Application.ExecutablePath);
            }
            else
            {
                rk.DeleteValue("ContextApplication", false);
            }
            Logger.Log(this.Name + ".startAutoBox_CheckedChanged", "Automatic start with windows set to: " + startAutoBox.Checked + ".", Type.Saving);
        }

        private void settingShowWarning_CheckedChanged(object sender, EventArgs e)
        {
            if (settingShowWarning.Checked)
            {
                Functions.ShowSizeWarning = true;
            }
            else
            {
                Functions.ShowSizeWarning = false;
            }

            Logger.Log(this.Name + ".settingShowWarning_CheckedChanged", "Show warning when backup size >10GB: " + settingShowWarning.Checked + ".", Type.Saving);
        }

        private void showCurrentProgressAsTooltip_CheckedChanged(object sender, EventArgs e)
        {
            if (showCurrentProgressAsTooltip.Checked)
            {
                Functions.ShowCurrentStatusProgressWindow = true;
            }
            else
            {
                Functions.ShowCurrentStatusProgressWindow = false;
            }

            Logger.Log(this.Name + ".showCurrentProgressAsTooltip_CheckedChanged", "Show current progress of backup as tooltip: " + showCurrentProgressAsTooltip.Checked + ".", Type.Saving);
        }

        private void showWindowOnChanges_CheckedChanged(object sender, EventArgs e)
        {
            Functions.ShowWindowOnChanges = showWindowOnChanges.Checked;
            if (Functions.ShowWindowOnChanges)
            {
                //TODO Add Code that runs when true.
            }

            Logger.Log(this.Name + ".showCurrentProgressAsTooltip_CheckedChanged", "Show popup window on changes: " + showWindowOnChanges.Checked + ".", Type.Saving);
        }

        private void popUpWhenExternalConnected_CheckedChanged(object sender, EventArgs e)
        {
            Functions.AutoStartBackupOnChange = popUpWhenExternalConnected.Checked;

            if (Functions.AutoStartBackupOnChange)
            {
                Thread externalDriveThread = new Thread(Functions.WaitForExternalDrive)
                {
                    Name = "WaitForExternalDriveThread"
                };
                externalDriveThread.Start();
            }

            Logger.Log(this.Name + ".popUpWhenExternalConnected_CheckedChanged", "Show popup when external is connected: " + popUpWhenExternalConnected.Checked + ".", Type.Saving);
        }
    }
}
