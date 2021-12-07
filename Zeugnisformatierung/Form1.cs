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
            XPdfForm f = XPdfForm.FromFile(textBox1.Text);
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

                f.PageIndex = i;
                PdfPage p = output.AddPage();
                p.Width = pageWidth;//f.PixelWidth;
                p.Height = pageHeight;// f.PixelHeight;

                //  if (output.PageCount % 2 == 0) p.TrimMargins.Top = Convert.ToInt16(textBox2.Text);

                XGraphics g = XGraphics.FromPdfPage(p); g.DrawImage(f, 0, 0);

                // if (i != 0) p.MediaBox = new PdfRectangle(new XRect(5, 5, p.Width, p.Height));



                if (output.PageCount % 2 == 0 && checkBox2.Checked)
                {
                    
                        p.MediaBox = new PdfRectangle(new XRect(1, Convert.ToDouble(textBox2.Text), p.Width, p.Height));
                    
                }              
                else
                {
                    p.MediaBox = new PdfRectangle(new XRect(1, 1, p.Width, p.Height));
                }

                // Cut if it is not a cover
                progressBar1.Value = i;
            }
            const string filename = "tempfile.pdf";
            try { 
            output.Save(filename);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
