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

namespace FileBro2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Load all drive letters into the Drive Explorer
            string[] ids = Directory.GetLogicalDrives();
            foreach (string id in ids)
            {
                treeView1.Nodes.Add(id);
                listView1.Items.Add(id, 8);
            }
        }

        private void assignIcons(string[] files)
        {
            foreach (string fl in files)
            {
                if (fl.EndsWith(".JPG") || fl.EndsWith(".jpg") || fl.EndsWith(".png") || fl.EndsWith(".PNG") || fl.EndsWith(".GIF") || fl.EndsWith(".gif"))
                    listView1.Items.Add(fl, 2);
                else if (fl.EndsWith(".MP4") || fl.EndsWith(".mp4") || fl.EndsWith(".AVI") || fl.EndsWith(".avi") || fl.EndsWith(".MPEG") || fl.EndsWith(".mpeg"))
                    listView1.Items.Add(fl, 3);
                else if (fl.EndsWith(".txt") || fl.EndsWith(".TXT"))
                    listView1.Items.Add(fl, 4);
                else if (fl.EndsWith(".doc") || fl.EndsWith(".DOC") || fl.EndsWith(".docx") || fl.EndsWith(".DOCX"))
                    listView1.Items.Add(fl, 5);
                else if (fl.EndsWith(".exe") || fl.EndsWith(".EXE"))
                    listView1.Items.Add(fl, 6);
                else if (fl.EndsWith(".msi") || fl.EndsWith(".MSI"))
                    listView1.Items.Add(fl, 7);
                else if (fl.EndsWith(".dll") || fl.EndsWith(".DLL"))
                    listView1.Items.Add(fl, 9);
                else if (fl.EndsWith(".mp3") || fl.EndsWith(".MP3") || fl.EndsWith(".wav") || fl.EndsWith(".WAV"))
                    listView1.Items.Add(fl, 10);
                else
                    listView1.Items.Add(fl, 1);
            }
        }

        private void ShowDriveDirects()
        {
            try
            {
                //Clear the ListView
                listView1.Clear();
                //Create a directory array and a file array
                string[] dirs = Directory.GetDirectories(treeView1.SelectedNode.Text);
                string[] files = Directory.GetFiles(treeView1.SelectedNode.Text);
                //Add the Directories to the ListView and the Drive Explorer
                foreach (string dir in dirs)
                {
                    listView1.Items.Add(dir, 0);
                    treeView1.SelectedNode.Nodes.Add(dir);
                }
                //Add the files to the ListView and give them the right icon
                assignIcons(files);
                //Refresh the Addressbar
                toolStripComboBox1.Text = treeView1.SelectedNode.Text;
            }
            catch (Exception ex)
            {
                //In case of Error? a messagebox will appear
                MessageBox.Show("Error - " + ex.Message);
            }
        }

        //This is the Function which is called when the user presses Enter in the addressbar
        private void GoToDirectory()
        {
            try
            {
                //This is almost the same as above. The only difference is that we dont add the directories to 
                //the Drive Explorer and we go to the directory in the addressbar this time.
                listView1.Clear();
                string[] dirs = Directory.GetDirectories(toolStripComboBox1.Text);
                string[] files = Directory.GetFiles(toolStripComboBox1.Text);
                foreach (string dir in dirs)
                {
                    listView1.Items.Add(dir, 0);
                }
                assignIcons(files);
                if (toolStripComboBox1.Items.Contains(toolStripComboBox1.Text) == false)
                    toolStripComboBox1.Items.Add(toolStripComboBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //When the User chooses Large View in the context menu, make the icons large
            listView1.View = View.LargeIcon;
        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //The same with the small,...
            listView1.View = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //...and the list icon view
            listView1.View = View.List;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //and when clicking the exit item, of cource close the application.
            this.Dispose();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            //When the user doubleclicks an item (folder/file) in the listview, do this:

            //Write the name of the clicked item into a string
            string direc = listView1.SelectedItems[0].Text;

            try
            {
                //We already had that...
                listView1.Clear();
                string[] dirs = Directory.GetDirectories(direc);
                string[] files = Directory.GetFiles(direc);
                foreach (string dir in dirs)
                {
                    listView1.Items.Add(dir, 0);
                }
                assignIcons(files);
                toolStripComboBox1.Text = direc;
                if (toolStripComboBox1.Items.Contains(toolStripComboBox1.Text) == false)
                    toolStripComboBox1.Items.Add(direc);
            }
            catch (Exception)
            {
                try
                {
                    toolStripComboBox1.Text = direc;
                    System.Diagnostics.Process.Start(direc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error - " + ex.Message);
                }
            }
        }

        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //when pressing Enter in the addressbar, call GoToDirectory
                GoToDirectory();
            }
        }
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Call the showDriveDirects Function
            ShowDriveDirects();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //This Function is for going one Directory up
            //First, you write the current path into a string and split it with \
            string path = toolStripComboBox1.Text;
            string[] pathSplit = path.Split(new Char[] { '\\' });
            //Then you can remove the last letters after the last \
            if (pathSplit.Length > 1)
            {
                try
                {
                    //The first string holds the last directory, so the directory youre in.
                    //The seconds String holds the directory which is above the one youre in, but still with a backslash.
                    string lastDir = pathSplit[pathSplit.Length - 1];
                    string withBackSlash = path.Replace(lastDir, null);

                    //This string holds the dir which is above the one youre in, without backslash this time.
                    string withoutBackSlash = withBackSlash.Remove(withBackSlash.Length - 1);

                    //If the path is a directory, do not show the backslash. If it is a drive, show it.
                    if (pathSplit.Length > 2)
                        toolStripComboBox1.Text = withoutBackSlash;
                    else
                        toolStripComboBox1.Text = withBackSlash;


                    try
                    {
                        //...
                        listView1.Clear();
                        string[] dirs = Directory.GetDirectories(toolStripComboBox1.Text);
                        string[] files = Directory.GetFiles(toolStripComboBox1.Text);
                        foreach (string dir in dirs)
                        {
                            listView1.Items.Add(dir, 0);
                        }
                        assignIcons(files);
                        if (toolStripComboBox1.Items.Contains(toolStripComboBox1.Text) == false)
                            toolStripComboBox1.Items.Add(toolStripComboBox1.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error - " + ex.Message);
                    }
                }
                catch (Exception)
                {

                }
            }
            if (pathSplit.Length == 1)
            {
                 MessageBox.Show("You cant move up");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //When clicking the home button, show the drives in the listview

            //Clear the listview
            listView1.Items.Clear();
            //Clear the treeview
            treeView1.Nodes.Clear();
            //get the drives and show them
            string[] ids = Directory.GetLogicalDrives();
            foreach (string id in ids)
            {
                treeView1.Nodes.Add(id);
                listView1.Items.Add(id, 8);
            }
            toolStripComboBox1.Text = "Home";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                //...
                listView1.Clear();
                string[] dirs = Directory.GetDirectories(toolStripComboBox1.Text);
                string[] files = Directory.GetFiles(toolStripComboBox1.Text);
                foreach (string dir in dirs)
                {
                    listView1.Items.Add(dir, 0);
                }
                assignIcons(files);
                if (toolStripComboBox1.Items.Contains(toolStripComboBox1.Text) == false)
                    toolStripComboBox1.Items.Add(toolStripComboBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].Text != "" && listView1.SelectedItems.Count == 1)
                Clipboard.SetText(listView1.SelectedItems[0].Text);
            else
                MessageBox.Show("You can only copy one element at a time.", "Cannot copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string path = Clipboard.GetText();
            char separator = '\\';
            string originalfilename = path.Split(separator)[path.Split(separator).Length - 1];
            string target = toolStripComboBox1.Text + "\\" + originalfilename;

            try
            {
                if (File.Exists(target))
                    if (MessageBox.Show("The file you want to copy already exists. Do you want to replace it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        File.Delete(target);
                        File.Copy(path, target, false);
                        GoToDirectory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    if (MessageBox.Show("Do you really want to delete this file?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        File.Delete(listView1.SelectedItems[0].Text);
                }
                else
                    MessageBox.Show("You can only delete one item at a time.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoToDirectory();
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
    }
}
