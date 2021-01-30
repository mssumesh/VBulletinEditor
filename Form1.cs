using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using System.Web;

namespace VBEditor
    {
    public partial class Form1 : Form
        {
        static Form1 MyForm;
        private IHTMLDocument2 doc;

        static string post = null;
        static string postid = null;
        ThreadStart worker;
        Thread th;

        static int ver = 3;
        static MethodInvoker CallHtmlBindToTxt = new MethodInvoker(HtmlBindToTxt);
        static void HtmlBindToTxt()
            {
            MyForm.txtHTML.Text = post;
            MyForm.pictureBox1.Visible = false;
            MyForm.th.Abort();

            string temp = "<html><body>";
            temp = temp + post;
            temp = temp + "</body></html>";

            MyForm.htmlView.DocumentText = temp;
            //doc.

            }


        public Form1()
            {
            InitializeComponent();
            }

        private void btnFetch_Click(object sender, EventArgs e)
            {
            string tmp;
            int beg;
            if (txtPost.Text != "")
                {
                

                tmp = txtPost.Text;
                beg = tmp.IndexOf("p=");
                //http://www.warriorforum.com/warrior-special-offers-forum/467554-just-47-big-youtube-last-chance-get-warrior-special-offer-5.html#post5096810
                if ( beg <= 0 )
                    beg = tmp.IndexOf ("#post");

                if (beg > 0)
                    {
                    pictureBox1.Visible = true;
                    worker = new ThreadStart(GetWebPageProc);
                    th = new Thread(worker);
                    th.Name = "WebPageFetcher";
                    th.IsBackground = true;
                    th.Start();
                    }
                else
                    MessageBox.Show("invalid vbulletin post URL, please check syntax", "Invalid URL");

                }
            }
        public void GetWebPageProc()
            {
            MyWeb myweb = new MyWeb();
            StringBuilder sbtmp;
            int beg, end;
            beg = end = 0;
            string buff = null;
            string postmsg = "post_message_";
            string tmp = null;

            post = txtPost.Text;
            beg = post.IndexOf("p=");
            if (beg > 0)
                {
                postid = post.Substring(beg);
                end = postid.IndexOf("&");
                postid = postid.Substring(0, end);
                tmp = postid.Substring(2);
                }
            else
                {
                beg = post.IndexOf("#post");
                if (beg > 0)
                    {
                    postid = post.Substring(beg);
                    tmp = postid.Substring(5);
                    }
                }
            if (tmp != null)
                {

                postmsg = postmsg + tmp;

                try
                    {
                    sbtmp = myweb.GetWebPage(txtPost.Text);
                    post = sbtmp.ToString();
                    if (post.Contains("<meta name=\"generator\" content=\"vBulletin 4"))
                        ver = 4;

                    if (ver == 4)
                        {
                        beg = post.IndexOf(postmsg);
                        buff = post.Substring(beg);
                        beg = buff.IndexOf("\x09\x09\x09\x09\x09\x09\x09");
                        post = buff.Substring(beg + 7);
                        end = post.IndexOf("</div>");
                        buff = post.Substring(0, end);
                        post = buff;
                        }
                    else if (ver == 3)
                        {
                        beg = post.IndexOf(postid);
                        buff = post.Substring(beg);
                        beg = buff.IndexOf("<!-- message -->");
                        post = buff.Substring(beg);
                        beg = post.IndexOf("\x0d\x0a\x09\x09\x09\x0d\x0a\x09\x09\x09");
                        buff = post.Substring(beg + 10);
                        post = buff;
                        end = post.IndexOf("\x09\x09<!-- / message -->");
                        buff = post.Substring(0, end);
                        post = buff;
                        }


                    //strip unwanted stuff


                    MyForm.BeginInvoke(CallHtmlBindToTxt);
                    }
                catch (Exception web)
                    {
                    MessageBox.Show(web.ToString());
                    }
                }
            else
                {
                MessageBox.Show("invalid vbulletin post URL, please check syntax", "Invalid URL");
                return;
                }
            }

        private void Form1_Load(object sender, EventArgs e)
        {
            MyForm = this;


            this.htmlView.Navigate("about:blank");

            doc = (mshtml.IHTMLDocument2)this.htmlView.Document.DomDocument;
            doc.designMode = "On";
            doc.write("The quick fox jumps over the lazy dog\r\nThe quick fox jumps over the lazy dog\r\nThe quick fox jumps over the lazy dog\r\nThe quick fox jumps over the lazy dog\r\nThe quick fox jumps over the lazy dog\r\n");

            ToolTip ttip = new ToolTip();
            ttip.SetToolTip(btnUnformat, "Remove formatting");
            ttip.SetToolTip(btnBold, "Bold");
            ttip.SetToolTip(btnItalics, "Italics");
            ttip.SetToolTip(btnUline, "Underline");
            ttip.SetToolTip(btnLink, "Insert Hyperlink");
            ttip.SetToolTip(btnUnlink, "Remove Hyperlink");
            ttip.SetToolTip(btnEmail, "Insert Email");
            ttip.SetToolTip(btnImage, "Insert Image");
            ttip.SetToolTip(btnQuote, "Quote Region");
            ttip.SetToolTip(btnCode, "Code Region");
            ttip.SetToolTip(btnHtml, "Html Region");
            ttip.SetToolTip(btnPhp, "PHP Code Region");
            ttip.SetToolTip(btnJustLeft, "Left Justify");
            ttip.SetToolTip(btnJustCenter, "Center Justify");
            ttip.SetToolTip(btnJustRight, "Right Justify");
            ttip.SetToolTip(btnListNum, "Make Ordered List");
            ttip.SetToolTip(btnListBullet, "Make Unordered List");
            ttip.SetToolTip(btnIndent, "Indent Block");
            ttip.SetToolTip(btnOutdent, "Outdent Block");
            ttip.SetToolTip(btnUndo, "Undo");
            ttip.SetToolTip(btnRedo, "Redo");

            ttip.SetToolTip(btnJustRight, "Remove formatting");
            ttip.SetToolTip(btnJustRight, "Remove formatting");
            
            ttip.AutomaticDelay = 2000;
        }

        private void btnBold_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Bold", false, null);
            }

        private void groupBox2_Enter(object sender, EventArgs e)
            {

            }

        private void btnItalics_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Italic", false, null);
            }

        private void btnUline_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Underline", false, null);
            }

        private void cboFont_SelectedIndexChanged(object sender, EventArgs e)
            {
            string font;
            if ( cboFont.SelectedIndex != -1)
                {
                font = cboFont.SelectedItem.ToString();
               htmlView.Document.ExecCommand("FontName", false, font);

                }
            }

        private void cboFsize_SelectedIndexChanged(object sender, EventArgs e)
            {
            if (cboFont.SelectedIndex != -1)
                {
                string size = cboFsize.SelectedItem.ToString();
                htmlView.Document.ExecCommand("FontSize", false, size );
                }
  
            }

        private void btnUndo_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Undo", false, null);
            //htmlView.ve
            }

        private void btnOutdent_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Outdent", false, null);
            }

        private void btnIndent_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Indent", false, null);
            }

        private void btnJustLeft_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("JustifyLeft", false, null);
            }

        private void btnJustCenter_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("JustifyCenter", false, null);
            
            }

        private void btnJustRight_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("JustifyRight", false, null);
            }

        private void btnLink_Click(object sender, EventArgs e)
            {
            string value = "Input URL";
            
            if (InputBox("Insert URL", "Please enter the URL of your link:", ref value) == DialogResult.OK)
                {
                htmlView.Document.ExecCommand("CreateLink", false, value);
                }
            }

        private void btnImage_Click(object sender, EventArgs e)
            {
            string value = "Input image link";

            if (InputBox("Insert Image", "Please enter the URL of your image:", ref value) == DialogResult.OK)
                {
                htmlView.Document.ExecCommand("InsertImage", false, value);
                }
           
            }

        private void btnListBullet_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertUnorderedList", false, null);
            }

        private void btnListNum_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertOrderedList", false, null);
            }

      

        public static DialogResult InputBox(string title, string promptText, ref string value)
            {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
            }

        private void btnUnlink_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("Unlink", false, null);
            }

        private void btnColor1_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Black");
            }

        private void btnColor40_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "White");
            }

        private void btnColor17_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Red");
            }

        private void btnColor2_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Sienna");
            }

        private void btnColor3_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkOliveGreen");
            }

        private void btnColor4_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkGreen");
            
            }

        private void btnColor5_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkSlateBlue");
            }

        private void btnColor6_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Navy");
            }

        private void btnColor8_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Indigo");
            }

        private void button7_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkSlateGray");
            }

        private void btnColor9_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkRed");
            }

        private void btnColor10_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkOrange");
            }

       private void btnColor11_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Olive");
            }

        private void btnColor12_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Green");
            }

        private void btnColor13_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Teal");
            }

        private void btnColor14_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Blue");
            }

        private void btnColor15_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "SlateGray");
            }

        private void btnColor16_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DimGray");
            }

        private void btnColor18_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "SandyBrown");
            }

        private void btnColor19_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "YellowGreen");
            }

        private void btnColor20_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "SeaGreen");
            }

        private void btnColor21_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "MediumTurquoise");
            }

        private void btnColor22_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "RoyalBlue");
            }

        private void btnColor23_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Purple");
            }

        private void btnColor24_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Gray");
            }

        private void btnColor25_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Magenta");
            }

        private void btnColor26_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Orange");
            }

        private void btnColor27_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Yellow");
            }

        private void btnColor28_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Lime");
            }

        private void btnColor29_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Cyan");
            }

        private void btnColor30_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DeepSkyBlue");
            }

        private void btnColor31_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "DarkOrchid");
            }

        private void btnColor32_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Silver");
            }

        private void btnColor33_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Pink");
            }

        private void btnColor34_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Wheat");
            }

        private void btnColor35_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "LemonChiffon");
            }
        private void btnColor36_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "PaleGreen");
            }

        private void btnColor37_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "PaleTurquoise");
            }

        private void btnColor38_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "LightBlue");
            }

        private void btnColor39_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("ForeColor", false, "Plum");
            }

        private void btnUnformat_Click(object sender, EventArgs e)
            {

            htmlView.Document.ExecCommand("RemoveFormat", false, null);

            }

        private void btnQuote_Click(object sender, EventArgs e)
            {
            string open = "<div style=\"margin:20px; margin-top:5px; \"> <div class=\"smallfont\" style=\"margin-bottom:2px\">Quote:</div> <table cellpadding=\"6\" cellspacing=\"0\" border=\"0\" width=\"100%\"> <tr> <td class=\"alt2\" style=\"border:1px inset\">";
			string close = "</td> </tr> </table> </div>";
            string replace;
	
            IHTMLTxtRange range = (IHTMLTxtRange)doc.selection.createRange();
            replace = open + range.text + close;
            range.pasteHTML(replace);
          
            }

        private void btnEmail_Click(object sender, EventArgs e)
            {
            string value = "Input email";
            string open = "<a href=\"mailto:";//">asdfa
            string close = "</a>";
            string replace;

            if (InputBox("Insert Email id", "Please enter the email address for link:", ref value) == DialogResult.OK)
                {
                IHTMLTxtRange range = (IHTMLTxtRange)doc.selection.createRange();
                replace = open + value + "\">" + range.text + close;
                range.pasteHTML(replace);
                }
            }

        private void btnCode_Click(object sender, EventArgs e)
            {
            string open = "<div style=\"margin:20px; margin-top:5px\"><div class=\"smallfont\" style=\"margin-bottom:2px\">Code:</div><pre class=\"alt2\" dir=\"ltr\" style=\"margin: 0px;padding: 6px;border: 1px inset;width: 640px;height: 34px;text-align: left;overflow: auto\">";
            string close = "</pre></div>";
            string replace;

            IHTMLTxtRange range = (IHTMLTxtRange)doc.selection.createRange();
            replace = open + range.text + close;
            range.pasteHTML(replace);

            }

        private void btnHtml_Click(object sender, EventArgs e)
            {
            string open = "<div style=\"margin:20px; margin-top:5px\"><div class=\"smallfont\" style=\"margin-bottom:2px\">HTML Code:</div><pre class=\"alt2\" dir=\"ltr\" style=\"margin: 0px;padding: 6px;border: 1px inset;width: 640px;height: 34px;text-align: left;overflow: auto\">";
            string close = "</pre></div>";
            string replace;

            IHTMLTxtRange range = (IHTMLTxtRange)doc.selection.createRange();
            replace = open + range.text + close;
            range.pasteHTML(replace);
            }

        private void btnPhp_Click(object sender, EventArgs e)
            {
            string open = "<div style=\"margin:20px; margin-top:5px\"><div class=\"smallfont\" style=\"margin-bottom:2px\">PHP Code:</div><div class=\"alt2\" dir=\"ltr\" style=\"margin: 0px;padding: 6px;border: 1px inset;width: 640px;height: 34px;text-align: left;overflow: auto\"><code style=\"white-space:nowrap\"><!-- php buffer start --><code><span style=\"color: #000000\"><span style=\"color: #0000BB\">";   
            string close = "<br /></span></span></code><!-- php buffer end --></code></div></div>";
            string replace;

            IHTMLTxtRange range = (IHTMLTxtRange)doc.selection.createRange();
            replace = open + range.text + close;
            range.pasteHTML(replace);
            }

        private void btnSm1_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/wink.gif");
            }

        private void btnSm2_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/smile.gif");
            }

        private void btnSm3_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/frown.gif");
            }

        private void btnSm4_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/biggrin.gif");
            }

        private void btnSm5_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/mad.gif");
            }

        private void btnSm6_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/tongue.gif");
            }

        private void btnSm7_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/confused.gif");
            }

        private void btnSm8_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/redface.gif");
            }

        private void btnSm9_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/rolleyes.gif");
            }

        private void btnSm10_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/cool.gif");
            }

        private void btnSm11_Click(object sender, EventArgs e)
            {
            htmlView.Document.ExecCommand("InsertImage", false, "http://cdn.warriorforum.com/images/smilies/eek.gif");
            }



        private void tabControl1_Selected(object sender, TabControlEventArgs e)
            {
            string html = "<HTML><HEAD><TITLE></TITLE></HEAD><BODY></BODY></HTML>";
            int indx1;
            int indx2;
            if (tabControl1.SelectedIndex == 2)
                {
          html = htmlView.DocumentText;
                indx1 = html.IndexOf("<BODY>");
                indx1 = indx1 + 6;
                indx2 = html.IndexOf("</BODY>");
                html = html.Substring(indx1, indx2 - indx1);
                txtHTML.Text = html;
                }
            else if (tabControl1.SelectedIndex == 0) //going to editor
                {
                IHTMLDocument2 doc2 = (IHTMLDocument2)doc;
                doc2.open("about:blank", null, null, null);
                doc2.expando = true;
                if (this.txtHTML.Text != string.Empty)
                    {
                    html = this.txtHTML.Text;
                    }
                doc2.write(html);
                doc2.close();
 
                }
            else if (tabControl1.SelectedIndex == 1)
                {
                html = VBConvert();
                txtVB.Text = html;
                }

            }


        public static string VBConvert()
            {
            string output = "<HTML><HEAD><TITLE></TITLE></HEAD><BODY></BODY></HTML>";
            int indx1;
            int indx2;
            output = MyForm.htmlView.DocumentText;
            indx1 = output.IndexOf("<BODY>");
            indx1 = indx1 + 6;
            indx2 = output.IndexOf("</BODY>");
            output = output.Substring(indx1, indx2 - indx1);

            output = output.Replace("\r\n", string.Empty );
            output = output.Replace("<STRONG>", "[B]");
            output = output.Replace("</STRONG>", "[/B]");
            output = output.Replace("<EM>", "[I]");
            output = output.Replace("</EM>", "[/I]");
            output = output.Replace("<U>", "[U]");
            output = output.Replace("</U>", "[/U]");
            output = output.Replace("&nbsp;", " fff");

            output = output.Replace("<OL>", "[LIST=1]");
            output = output.Replace("</OL>", "[/LIST]");
            output = output.Replace("<UL>", "[LIST]");
            output = output.Replace("</UL>", "[/LIST]");
            output = output.Replace("<LI>", "[*]");
            output = output.Replace("</LI>", "\r\n");
            
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/wink.gif\">", ";)");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/smile.gif\">", ":)");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/frown.gif\">", ":(");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/biggrin.gif\">", ":D");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/mad.gif\">", ":mad:");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/tongue.gif\">", ":p");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/confused.gif\">", ":confused:");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/redface.gif\">", ":o");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/rolleyes.gif\">", ":rolleyes:");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/cool.gif\">", ":cool:");
            output = output.Replace("<IMG src=\"http://cdn.warriorforum.com/images/smilies/eek.gif\">", ":eek:");

            output = ReplaceHtml(output, "<FONT color=black>", "</FONT>", "[COLOR=\"Black\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=sienna>", "</FONT>", "[COLOR=\"Sienna\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkolivegreen>", "</FONT>", "[COLOR=\"DarkOliveGreen\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkgreen>", "</FONT>", "[COLOR=\"DarkGreen\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkslateblue>", "</FONT>", "[COLOR=\"DarkSlateBlue\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=navy>", "</FONT>", "[COLOR=\"Navy\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=indigo>", "</FONT>", "[COLOR=\"Indigo\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkslategray>", "</FONT>", "[COLOR=\"DarkSlateGray\"]", "[/COLOR]");

            output = ReplaceHtml(output, "<FONT color=darkred>", "</FONT>", "[COLOR=\"DarkRed\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkorange>", "</FONT>", "[COLOR=\"DarkOrange\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=olive>", "</FONT>", "[COLOR=\"Olive\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=green>", "</FONT>", "[COLOR=\"Green\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=teal>", "</FONT>", "[COLOR=\"Teal\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=blue>", "</FONT>", "[COLOR=\"Blue\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=slategray>", "</FONT>", "[COLOR=\"SlateGray\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=dimgray>", "</FONT>", "[COLOR=\"DimGray\"]", "[/COLOR]");

            output = ReplaceHtml(output, "<FONT color=red>", "</FONT>", "[COLOR=\"Red\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=sandybrown>", "</FONT>", "[COLOR=\"SandyBrown\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=yellowgreen>", "</FONT>", "[COLOR=\"YellowGreen\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=seagreen>", "</FONT>", "[COLOR=\"SeaGreen\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=mediumturquoise>", "</FONT>", "[COLOR=\"MediumTurquoise\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=royalblue>", "</FONT>", "[COLOR=\"RoyalBlue\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=purple>", "</FONT>", "[COLOR=\"Purple\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=gray>", "</FONT>", "[COLOR=\"Gray\"]", "[/COLOR]");


            output = ReplaceHtml(output, "<FONT color=magenta>", "</FONT>", "[COLOR=\"Magenta\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=orange>", "</FONT>", "[COLOR=\"Orange\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=yellow>", "</FONT>", "[COLOR=\"Yellow\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=lime>", "</FONT>", "[COLOR=\"Lime\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=cyan>", "</FONT>", "[COLOR=\"Cyan\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=deepskyblue>", "</FONT>", "[COLOR=\"DeepSkyBlue\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=darkorchid>", "</FONT>", "[COLOR=\"DarkOrchid\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=silver>", "</FONT>", "[COLOR=\"Silver\"]", "[/COLOR]");

            output = ReplaceHtml(output, "<FONT color=pink>", "</FONT>", "[COLOR=\"Pink\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=wheat>", "</FONT>", "[COLOR=\"Wheat\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=lemonchiffon>", "</FONT>", "[COLOR=\"LemonChiffon\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=palegreen>", "</FONT>", "[COLOR=\"PaleGreen\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=paleturquoise>", "</FONT>", "[COLOR=\"PaleTurquoise\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=lightblue>", "</FONT>", "[COLOR=\"LightBlue\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=plum>", "</FONT>", "[COLOR=\"Plum\"]", "[/COLOR]");
            output = ReplaceHtml(output, "<FONT color=white>", "</FONT>", "[COLOR=\"White\"]", "[/COLOR]");

            output = ReplaceHtml(output, "<FONT size=1>", "</FONT>", "[SIZE=\"1\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=2>", "</FONT>", "[SIZE=\"2\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=3>", "</FONT>", "[SIZE=\"3\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=4>", "</FONT>", "[SIZE=\"4\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=5>", "</FONT>", "[SIZE=\"5\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=6>", "</FONT>", "[SIZE=\"6\"]", "[/SIZE]");
            output = ReplaceHtml(output, "<FONT size=7>", "</FONT>", "[SIZE=\"7\"]", "[/SIZE]");

            
            output = ReplaceHtml(output, "<FONT face=Arial>", "</FONT>", "[FONT=\"Arial\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Arial Black\">", "</FONT>", "[FONT=\"Arial Black\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Arial Narrow\">", "</FONT>", "[FONT=\"Arial Narrow\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Book Antiqua\">", "</FONT>", "[FONT=\"Book Antiqua\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Century Gothik\">", "</FONT>", "[FONT=\"Century Gothik\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Comic Sans MS\">", "</FONT>", "[FONT=\"Comic Sans MS\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Courier New\">", "</FONT>", "[FONT=\"Courier New\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Fixedsys>", "</FONT>", "[FONT=\"Fixedsys\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Franklin Gothik\">", "</FONT>", "[FONT=\"Franklin Gothik\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Garamond>", "</FONT>", "[FONT=\"Garamond\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Georgia>", "</FONT>", "[FONT=\"Georgia\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Impact>", "</FONT>", "[FONT=\"Impact\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Lucida Console\">", "</FONT>", "[FONT=\"Lucida Console\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Microsoft Sans Serif\">", "</FONT>", "[FONT=\"Microsoft Sans Serif\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Palatino Linotype\">", "</FONT>", "[FONT=\"Palatino Linotype\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=System>", "</FONT>", "[FONT=\"System\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Tahoma>", "</FONT>", "[FONT=\"Tahoma\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Times New Roman\">", "</FONT>", "[FONT=\"Times New Roman\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=\"Trebuchet MS\">", "</FONT>", "[FONT=\"Trebuchet MS\"]", "[/FONT]");
            output = ReplaceHtml(output, "<FONT face=Verdana>", "</FONT>", "[FONT=\"Verdana\"]", "[/FONT]");




            output = ReplaceHtml(output, "<DIV style=\"MARGIN: 5px 20px 20px\"><DIV class=smallfont style=\"MARGIN-BOTTOM: 2px\">Quote:</DIV><TABLE cellSpacing=0 cellPadding=6 width=\"100%\" border=0>  <TBODY>  <TR>    <TD class=alt2     style=\"BORDER-RIGHT: 1px inset; BORDER-TOP: 1px inset; BORDER-LEFT: 1px inset; BORDER-BOTTOM: 1px inset\">", "</TD></TR></TBODY></TABLE></DIV>", "[QUOTE]", "[/QUOTE]");
            
            output = ReplaceHtml(output, "<DIV style=\"MARGIN: 5px 20px 20px\"><DIV class=smallfont style=\"MARGIN-BOTTOM: 2px\">Code:</DIV><PRE class=alt2 dir=ltr style=\"BORDER-RIGHT: 1px inset; PADDING-RIGHT: 6px; BORDER-TOP: 1px inset; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px; MARGIN: 0px; OVERFLOW: auto; BORDER-LEFT: 1px inset; WIDTH: 640px; PADDING-TOP: 6px; BORDER-BOTTOM: 1px inset; HEIGHT: 34px; TEXT-ALIGN: left\">","</PRE></DIV>","[CODE]","[/CODE]");

            output = ReplaceHtml(output, "<DIV style=\"MARGIN: 5px 20px 20px\"><DIV class=smallfont style=\"MARGIN-BOTTOM: 2px\">HTML Code:</DIV><PRE class=alt2 dir=ltr style=\"BORDER-RIGHT: 1px inset; PADDING-RIGHT: 6px; BORDER-TOP: 1px inset; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px; MARGIN: 0px; OVERFLOW: auto; BORDER-LEFT: 1px inset; WIDTH: 640px; PADDING-TOP: 6px; BORDER-BOTTOM: 1px inset; HEIGHT: 34px; TEXT-ALIGN: left\">", "</PRE></DIV>", "[HTML]", "[/HTML]");
            
            output = ReplaceHtml(output, "<DIV style=\"MARGIN: 5px 20px 20px\"><DIV class=smallfont style=\"MARGIN-BOTTOM: 2px\">PHP Code:</DIV><DIV class=alt2 dir=ltr style=\"BORDER-RIGHT: 1px inset; PADDING-RIGHT: 6px; BORDER-TOP: 1px inset; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px; MARGIN: 0px; OVERFLOW: auto; BORDER-LEFT: 1px inset; WIDTH: 640px; PADDING-TOP: 6px; BORDER-BOTTOM: 1px inset; HEIGHT: 34px; TEXT-ALIGN: left\"><CODE style=\"WHITE-SPACE: nowrap\"><!-- php buffer start --><CODE><SPAN style=\"COLOR: #000000\"><SPAN style=\"COLOR: #0000bb\">","<BR></SPAN></SPAN></CODE><!-- php buffer end --></CODE></DIV></DIV>", "[PHP]", "[/PHP]");

            output = ReplaceHtml(output, "<P align=center>", "</P>", "[CENTER]", "[/CENTER]");
            output = ReplaceHtml(output, "<P align=left>", "</P>", "[LEFT]", "[/LEFT]");
            output = ReplaceHtml(output, "<P align=right>", "</P>", "[RIGHT]", "[/RIGHT]");

            output = ReplaceHtml(output, "<BLOCKQUOTE dir=ltr style=\"MARGIN-RIGHT: 0px\">", "</BLOCKQUOTE>", "[INDENT]", "[/INDENT]");

            output = ReplaceHtml(output, "<A href=\"mailto:", "</A>", "[EMAIL=\"", "[/EMAIL]");
            output = ReplaceHtml(output, "<IMG src=\"", "\">", "[IMG]", "[/IMG]");

            output = ReplaceHtml(output, "<A href=", "</A>", "[URL=", "[/URL]");
            
            //after everything remove paras with CRLF
            output = output.Replace("\">", "\"]");
            output = output.Replace("<P>", " ");
            output = output.Replace("</P>", "\r\n");
            return output;
            }

        public static string ReplaceHtml(string input, string tag1, string tag2, string repl1, string repl2)
            {
            int indxbeg1, indxend1, indxbeg2, indxend2, i;
            string op = string.Empty;
            string tmp;
            string exp = input;

            i = 0;
            if ( exp.Contains (tag1 ) )
                {
                indxbeg1 = exp.IndexOf(tag1);

                do
                    {
                    
                    indxend1 = indxbeg1 + tag1.Length;
                    tmp = exp.Substring(i, indxbeg1 - i);
                    op = op + tmp;
                    i = indxend1;
                    op = op + repl1;
                    indxbeg2 = exp.IndexOf(tag2, i);
                    indxend2 = indxbeg2 + tag2.Length;
                    tmp = exp.Substring(indxend1, indxbeg2 - indxend1);
                    op = op + tmp;
                    i = indxend2;
                    op = op + repl2;
                    indxbeg1 = exp.IndexOf(tag1, i );
                    } while (indxbeg1 != -1);
                tmp = exp.Substring(i);
                op = op + tmp;
                }
            else
                op = input;

            return op;

            }

        


        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
            {
            string html = htmlView.DocumentText;
            int indx1;
            int indx2;
            if (tabControl1.SelectedIndex == 0)
                {
                
                indx1 = html.IndexOf("<BODY>");
                if (indx1 != -1)
                    {
                    indx1 = indx1 + 6;
                    indx2 = html.IndexOf("</BODY>");
                    html = html.Substring(indx1, indx2 - indx1);
                    txtHTML.Text = html;
                    }
            

                }
            }


 
        }
    }




