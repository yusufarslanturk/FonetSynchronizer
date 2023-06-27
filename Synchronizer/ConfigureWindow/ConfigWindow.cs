﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AZLog;
using Dic = AZDictionary.Dictionary;
using Type = AZLog.Type;

namespace Synchronizer
{
    public partial class ConfigWindow : Form
    {
        public ConfigWindow()
        {
            Logger.Log(this.Name,"Initializing..", AZLog.Type.Loading);
            InitializeComponent();

            Logger.Log(this.Name, "Reading Config file..", AZLog.Type.Loading);
            StreamReader reader = new StreamReader(Dic.SyncConfigFile);
            string lineOfConfig = reader.ReadToEnd();
            string[] linesOfConfig = lineOfConfig.Split(',');

            // Cleanup strings
            foreach (string line in linesOfConfig)
            {
                string[] splitted = line.Split('=');
                if (splitted.Length > 1)
                {
                    if (splitted[0] == "src")
                    {
                        this.srcTxtBox.Text = splitted[1];
                        Logger.Log(this.Name, "Line of ConfigFile: " + splitted[0] + "=" + splitted[1]);
                    }
                    else if (splitted[0] == "trgt")
                    {
                        this.targetTxtBox.Text = splitted[1];
                        Logger.Log(this.Name, "Line of ConfigFile: " + splitted[0] + "=" + splitted[1]);
                    }
                    else
                    {
                        Logger.Log(this.Name, "Empty or Wrong line at Config File", Type.Warning);
                    }
                }
            }
            Logger.Log(this.Name, "Closing Config file.", AZLog.Type.Closing);
            reader.Close();
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            Logger.Log(this.Name, "Opening FolderBrowserDialog().", AZLog.Type.Opening);
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
                    Logger.Log(this.Name, "sourceBtn clicked.");
                    break;
                case ("targetBtn"):
                    targetTxtBox.Text = path;
                    Logger.Log(this.Name, "targetBtn clicked.");
                    break;
                default:
                    Logger.Log(this.Name, "Default triggered, something went wrong.", AZLog.Type.Warning);
                    break;
            }

            Logger.Log(this.Name, "Closing FolderBrowserDialog().", AZLog.Type.Closing);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Logger.Log(this.Name, "Creating Config File.", Type.Opening);
            StreamWriter writer = new StreamWriter(Dic.SyncConfigFile);
            writer.WriteLine("src=" + srcTxtBox.Text + ",");
            writer.WriteLine("trgt=" + targetTxtBox.Text + ",");
            writer.Flush();
            Logger.Log(this.Name, "Saving Config File.", Type.Saving);
            writer.Close();
            Logger.Log(this.Name, "Closing Config File.", Type.Closing);
            Logger.Log(this.Name, "Showing 'Config is Created' message.");
            MessageBox.Show(StringTable.ConfigConfigIsSaved);
            Logger.Log(this.Name, "Closing Configuration Dialog", Type.Closing);
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Logger.Log(this.Name, "Closing Configuration Dialog", Type.Closing);
            this.Close();
        }
    }
}
