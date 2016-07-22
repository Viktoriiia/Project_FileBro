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

namespace FileManager_V2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
        }

        private string tempFolderPath = @"D:\Temp";

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            //DriveInfo[] allDrives = DriveInfo.GetDrives();
            //foreach (DriveInfo drive in allDrives)
            //{
            //    DirectoryInfo info = new DirectoryInfo(drive.Name);
                DirectoryInfo info = new DirectoryInfo(@"D:\");
            if (info.Exists)
                {
                    rootNode = new TreeNode(info.Name);
                    rootNode.Tag = info;
                    GetDirectories(info.GetDirectories(), rootNode);
                    treeView1.Nodes.Add(rootNode);
                }
            //}
        }

        private void GetDirectories(DirectoryInfo[] subDirs,TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                try
                {
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch(UnauthorizedAccessException ex)
                {
                    continue;
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "Directory"),new ListViewItem.ListViewSubItem(item,dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
                listView1.Items[listView1.Items.IndexOf(item)].Tag = dir;
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                          { new ListViewItem.ListViewSubItem(item, "File"),
                   new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()),  new ListViewItem.ListViewSubItem(item,file.Length.ToString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
                listView1.Items[listView1.Items.IndexOf(item)].Tag=file;
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
               if ((listView1.SelectedItems[0].Tag.GetType() == typeof(FileInfo)))
                {
                    if ((listView1.SelectedItems[0].Tag as FileInfo).Exists)
                    {
                        Clipboard.Clear();
                        string fileNameFull = (listView1.SelectedItems[0].Tag as FileInfo).FullName;
                        System.Collections.Specialized.StringCollection filePath = new System.Collections.Specialized.StringCollection();
                        filePath.Add((listView1.SelectedItems[0].Tag as FileInfo).FullName);
                        Clipboard.SetFileDropList(filePath);
                        MessageBox.Show("File Copied To Clipboard"); //Inform the user
                    }
                }
                if ((listView1.SelectedItems[0].Tag.GetType() == typeof(DirectoryInfo)))
                {
                    if ((listView1.SelectedItems[0].Tag as DirectoryInfo).Exists)
                    {
                        Clipboard.Clear();
                        string folderPathFull = (listView1.SelectedItems[0].Tag as DirectoryInfo).FullName;
                        System.Collections.Specialized.StringCollection folderPath = new System.Collections.Specialized.StringCollection();
                        folderPath.Add((listView1.SelectedItems[0].Tag as DirectoryInfo).FullName);
                        Clipboard.SetFileDropList(folderPath);
                        MessageBox.Show("File Copied To Clipboard"); //Inform the user // add permission!!!
                    }
                }
            }
        }

        private void toolStripMenuItemPast_Click(object sender, EventArgs e)
        {

            try
            {
                if ((listView1.SelectedItems[0].Tag.GetType() == typeof(FileInfo)) && Clipboard.ContainsFileDropList())
                {
                    string targetDir = (listView1.SelectedItems[0].Tag as FileInfo).DirectoryName;
                    File.Copy(Clipboard.GetFileDropList()[0], targetDir + "\\" + Path.GetFileName(Clipboard.GetFileDropList()[0]));
                    Clipboard.Clear();

                    MessageBox.Show((listView1.SelectedItems[0].Tag as FileInfo).FullName);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("The file which you try to copy is already exists in this directory.", "Error");
            }

            try
            {
                if ((listView1.SelectedItems[0].Tag.GetType() == typeof(DirectoryInfo)) && Clipboard.ContainsFileDropList())
                {
                    string targetDir = (listView1.SelectedItems[0].Tag as DirectoryInfo).FullName;
                    File.Copy(Clipboard.GetFileDropList()[0], targetDir + "\\" + Path.GetFileName(Clipboard.GetFileDropList()[0]));
                    Clipboard.Clear();

                    MessageBox.Show((listView1.SelectedItems[0].Tag as DirectoryInfo).FullName);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("The file which you try to copy is already exists in this directory.", "Error");
            }

            if (Directory.Exists(tempFolderPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(@"D:\Temp");

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
                Directory.Delete(tempFolderPath);
                //delete file from curent Directory
            }
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {

              if (listView1.SelectedItems[0].Tag.GetType() == typeof(DirectoryInfo))
                {
                    if ((listView1.SelectedItems[0].Tag as DirectoryInfo).Exists)
                    File.Delete((listView1.SelectedItems[0].Tag as DirectoryInfo).FullName);
                    MessageBox.Show((listView1.SelectedItems[0].Tag as DirectoryInfo).FullName + " " + @"file was Deleted.");
                }

                if (listView1.SelectedItems[0].Tag.GetType() == typeof(FileInfo))
                {

                    if ((listView1.SelectedItems[0].Tag as FileInfo).Exists)
                    {
                        File.Delete((listView1.SelectedItems[0].Tag as FileInfo).FullName);
                        MessageBox.Show((listView1.SelectedItems[0].Tag as FileInfo).FullName + " " + @"file was Deleted.");
                    }
                }
            }
        }

        private void toolStripMenuItemCut_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {

                if(listView1.SelectedItems[0].Tag.GetType() == typeof(FileInfo))
                {
                    if ((listView1.SelectedItems[0].Tag as FileInfo).Exists)
                    {
                        string fileNameFull = (listView1.SelectedItems[0].Tag as FileInfo).FullName;
                        System.Collections.Specialized.StringCollection filePath = new System.Collections.Specialized.StringCollection();
                        filePath.Add((listView1.SelectedItems[0].Tag as FileInfo).FullName);
                        Clipboard.SetFileDropList(filePath); // copy file to cilboard

                        if (!Directory.Exists(tempFolderPath))
                        {
                            DirectoryInfo templetedir = Directory.CreateDirectory(tempFolderPath);
                        }
                        // add verifying if this file does not located at the temp folder
                        //if yes - delete this file
                        File.Copy(Clipboard.GetFileDropList()[0], tempFolderPath + "\\" + Path.GetFileName(Clipboard.GetFileDropList()[0]));
                        MessageBox.Show(Clipboard.GetFileDropList()[0]);
                    }
                }
            }
        }
    }
}
