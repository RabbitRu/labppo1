﻿using labppo1.InnerStruct;
using labppo1.Fileworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using labppo1.Command;

namespace labppo1
{
    public partial class Form1 : Form
    {
        private DataTree dt;
        private ILoader xmlworker;
        private Reciever<DataTree> rcv;
        private int grpchng;

        public Form1()
        {
            InitializeComponent();
            rcv = new Reciever<DataTree>();
            dt = new DataTree();
            xmlworker = new XMLLoader();
            grpchng = -1;

            this.treeView1.AfterSelect += (s, arg) => treeView1_AfterSelect();


        }

        private void treeView1_AfterSelect()
        {
            int group = treeView1.SelectedNode.Index;
            if (treeView1.SelectedNode.Level == 1)
            {
                group = treeView1.SelectedNode.Parent.Index;
                int student = treeView1.SelectedNode.Index;
                UpdateStudentCard(group, student);
            }
            else
            {
                ClearStudentCard();
            }

            UpdateGroupCard(group);
        }

        private void ClearStudentCard()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void UpdateStudentCard(int group, int student)
        {
            StudentInfo st = dt[group][student];
            textBox1.Text = st.Surname;
            textBox2.Text = st.Firstname;
            textBox3.Text = st.Middlename;
            textBox4.Text = st.Rating.ToString();
            textBoxAvatar.Text = st.Avatar;
            Bitmap image1;
            try
            {
                image1 = new Bitmap(st.Avatar);
            }
            catch
            {
                image1 = new Bitmap(100, 100);
                using (Graphics graph = Graphics.FromImage(image1))
                {
                    Rectangle ImageSize = new Rectangle(0, 0, 100, 100);
                    graph.FillRectangle(Brushes.White, ImageSize);
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Image = image1;
        }

        private void UpdateGroupCard(int group)
        {
            GroupInfo gr = dt[group];
            textBox5.Text = gr.Groupname;
            textBox6.Text = gr.Count.ToString();
            textBox7.Text = gr.MaxRating.ToString();
            textBox8.Text = gr.AvRating.ToString();
            textBox9.Text = gr.MinRating.ToString();
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = openFileDialog1.FileName;

            dt.Clear();
            grpchng = -1;
            dt = xmlworker.LoadFromFile(filename);
            UpdateTree();
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = saveFileDialog1.FileName;
            xmlworker.WriteToFile(filename, dt);

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Level == 1)
                {
                    int gind = treeView1.SelectedNode.Parent.Index;
                    int sind = treeView1.SelectedNode.Index;
                    dt = rcv.Do(new DeleteStudentCommand(gind, sind), dt);
                }
                else
                {
                    int gind = treeView1.SelectedNode.Index;
                    dt = rcv.Do(new DeleteGroupCommand(gind), dt);
                    grpchng = gind;
                }
                UpdateTree();
            }
        }

        private void AddStudentButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                int rating;
                try
                {
                    rating = int.Parse(textBox4.Text);
                }
                catch
                {
                    rating = 0;
                }
                StudentInfo st = new StudentInfo(
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    rating,
                    textBoxAvatar.Text);
                if (treeView1.SelectedNode.Level == 1)
                {
                    int gind = treeView1.SelectedNode.Parent.Index;
                    int sind = treeView1.SelectedNode.Index;
                    dt = rcv.Do(new AddStudentCommand(st, gind, sind), dt);
                }
                else
                {
                    int sind = 0;
                    int gind = treeView1.SelectedNode.Index;
                    dt = rcv.Do(new AddStudentCommand(st, gind, sind), dt);
                }
                UpdateTree();
            }
        }

        private void EditStudentButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                int rating;
                try
                {
                    rating = int.Parse(textBox4.Text);
                }
                catch
                {
                    rating = 0;
                }
                StudentInfo st = new StudentInfo(
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    rating,
                    textBoxAvatar.Text);

                int gind = treeView1.SelectedNode.Parent.Index;
                int sind = treeView1.SelectedNode.Index;
                dt = rcv.Do(new EditStudentCommand(st, gind, sind), dt);

                UpdateTree();
            }
        }

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Level == 1) { }
                else
                {
                    int gind = treeView1.SelectedNode.Index;
                    GroupInfo gr = new GroupInfo(textBox5.Text);
                    dt = rcv.Do(new AddGroupCommand(gr, gind), dt);
                    grpchng = gind;
                }
                UpdateTree();
            }
            else
            {
                if(dt.Count == 0)
                {
                    int gind = 0;
                    GroupInfo gr = new GroupInfo(textBox5.Text);
                    dt = rcv.Do(new AddGroupCommand(gr, gind), dt);
                    UpdateTree();
                    grpchng = 0;
                }
            }
        }

        private void EditGroupButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 0)
            {
                int gind = treeView1.SelectedNode.Index;
                GroupInfo gr = new GroupInfo(textBox5.Text);
                dt = rcv.Do(new EditGroupCommand(gr, gind), dt);

                UpdateTree();
            }
        }

        private void UpdateTree()
        {
            List<Boolean> oldn = new List<bool>();
            for (int i = 0; i < treeView1.Nodes.Count; i++)
                oldn.Add(treeView1.Nodes[i].IsExpanded);
            if (grpchng > -1 && treeView1.Nodes.Count > dt.Count)
            {
                oldn.RemoveAt(grpchng);
            }
            if(grpchng > -1 && treeView1.Nodes.Count < dt.Count)
            {
                oldn.Insert(grpchng, false);
            }
            bool saveExpand = dt.Count == oldn.Count;
            treeView1.Nodes.Clear();
            for (int i = 0; i < dt.Count; i++)
            {
                treeView1.Nodes.Add(new TreeNode(dt[i].Groupname));
                for (int j = 0; j < dt[i].Count; j++)
                {
                    treeView1.Nodes[i].Nodes.Add(new TreeNode(dt[i][j].Surname + " " + dt[i][j].Firstname + " " + dt[i][j].Middlename));
                }
                if(saveExpand && oldn[i])
                    treeView1.Nodes[i].Expand();
            }
            grpchng = -1;
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            rcv.Redo(dt);
            UpdateTree();

        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            rcv.Undo(dt);
            UpdateTree();
        }
    }
}
