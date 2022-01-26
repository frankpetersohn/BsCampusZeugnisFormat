using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;


namespace Zeugnisformatierung
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                string[] files = openFileDialog1.FileNames;
                richTextBox1.Text = "";
                foreach (string file in files)
                {
                    richTextBox1.Text += file+"\n";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open the input files
            PdfDocument inputDocument = PdfReader.Open(textBox1.Text, PdfDocumentOpenMode.Import);
            progressBar1.Maximum = inputDocument.PageCount + 1;
            progressBar1.Value = 0;
            // Create the output document
            PdfDocument outputDocument = new PdfDocument();

            // Show consecutive pages facing. Requires Acrobat 5 or higher.
            outputDocument.PageLayout = PdfPageLayout.TwoColumnLeft;
/*
            XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Far;
            XGraphics gfx;
          XRect box; 
            int count = Math.Max(inputDocument1.PageCount, inputDocument2.PageCount);*/ 
            for (int idx = 0; idx < inputDocument.PageCount; idx++)
            {

              
                // Get page from 1st document
                PdfPage page = inputDocument.PageCount > idx ?
                  inputDocument.Pages[idx] : new PdfPage();
                PdfPage newPage = new PdfPage();
                newPage.TrimMargins.Top = 30;
                inputDocument.Pages[idx].TrimMargins.Top = 30;
                newPage = inputDocument.Pages[idx];
                
                if(checkBox1.Checked && idx == 0)
                {
                    continue;
                }
                page.TrimMargins.Top = 30;
                // Add both pages to the output document
                //outputDocument.AddPage(page);
                outputDocument.AddPage(newPage);
                progressBar1.Value = idx;
              
            }

            // Save the document...
            const string filename = "CompareDocument1_tempfile.pdf";
            outputDocument.Save(filename);
            progressBar1.Value = progressBar1.Maximum;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] files = richTextBox1.Text.Split('\n');
            foreach (string sourcefile in files)
            {
                if (File.Exists(sourcefile))
                {
                    XPdfForm f = XPdfForm.FromFile(sourcefile);
                    progressBar1.Maximum = f.PageCount + 1;
                    progressBar1.Value = 0;
                    double pageHeight = f.Size.Height;
                    double pageWidth = f.Size.Width;

                    PdfDocument output = new PdfDocument();
                    for (int i = 0; i < f.PageCount; i++)
                    {
                        if (checkBox1.Checked && i == 0)
                        {
                            continue;
                        }



                        PdfPage p = output.AddPage();
                        if (checkBox3.Checked)
                        {
                            p.Width = pageWidth * 2;//f.PixelWidth;
                        }
                        else
                        {
                            p.Width = pageWidth;//f.PixelWidth;
                        }

                        p.Height = pageHeight;// f.PixelHeight;

                        //  if (output.PageCount % 2 == 0) p.TrimMargins.Top = Convert.ToInt16(textBox2.Text);

                        f.PageIndex = i;

                        XGraphics g = XGraphics.FromPdfPage(p);
                        g.DrawImage(f, 0, 0);
                        if (checkBox3.Checked && f.PageCount > i + 1)
                        {
                            f.PageIndex = i + 1;
                            g.DrawImage(f, p.Width / 2, Convert.ToDouble(textBox2.Text));
                            i++;
                            p.MediaBox = new PdfRectangle(new XRect(1, 1, p.Width, p.Height));
                        }
                        else
                        {

                            if (output.PageCount % 2 == 0 && checkBox2.Checked)
                            {

                                p.MediaBox = new PdfRectangle(new XRect(1, Convert.ToDouble(textBox2.Text), p.Width, p.Height));

                            }
                            else
                            {
                                p.MediaBox = new PdfRectangle(new XRect(1, 1, p.Width, p.Height));
                            }
                        }
                        // Cut if it is not a cover
                        progressBar1.Value = i;
                        progressBar1.Value = 0;

                    }

                    string newfilename = Path.GetFileName(sourcefile);


                    string filename = textBox3.Text + "//" + newfilename;

                    try
                    {
                        output.Save(filename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
               textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            fileSystemWatcher1.Path = textBox3.Text;
            updateFileList();


        }
        private void updateFileList()
        {
            string[] fileEntries = Directory.GetFiles(fileSystemWatcher1.Path);
            listBox1.Items.Clear();
            foreach (string s in fileEntries)
            {
                listBox1.Items.Add(s);
            }
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            updateFileList();
           
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(listBox1.SelectedItem.ToString());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(printDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var item in listBox1.SelectedItems)
                {
                    
                    FileInfo fileInfo = new FileInfo(item.ToString());

                    if (!fileInfo.Exists)
                    {
                        throw new FileNotFoundException();
                    }

                    var printProcess = new Process();
                    printProcess.StartInfo.FileName = item.ToString();
                    printProcess.StartInfo.UseShellExecute = true;
                    printProcess.StartInfo.Verb = "print";
                    printProcess.Start();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = Properties.Settings.Default.Rand;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Rand = textBox2.Text;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            foreach(string item in listBox1.SelectedItems)
            {
               {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception ex) {
                        MessageBox.Show("Fehler beim löschen von: " + item);
                    }
                }
            }
            updateFileList();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
