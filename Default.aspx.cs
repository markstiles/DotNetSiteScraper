using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SiteScraper
{
	public partial class _Default : System.Web.UI.Page
	{
		private static ASCIIEncoding _encoding = null;
		public static ASCIIEncoding encoding {
			get {
				if (_encoding == null) {
					_encoding = new System.Text.ASCIIEncoding();
				}
				return _encoding;
			}
		}

		#region Page Events
		
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				//xenu
				populateDropDown("xenulinks", ref ddlXenu);
				//site links
				populateDropDown("sitelinks", ref ddlFiles);
			}
		}

		protected void btnXenuRefresh_Click(object sender, EventArgs e) {
			populateDropDown("xenulinks", ref ddlXenu);
		}
		
		protected void btnXenu_Click(object sender, EventArgs e) {
			ltlXenu.Text = "";
			byte[] bytes = GetFileBytes(ddlXenu.SelectedValue);
			string data = encoding.GetString(bytes);

			//split urls by breaklines
			List<string> lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			//try to save each url to a file
			StringBuilder sb = new StringBuilder();
			List<string> links = new List<string>();
			StringBuilder sbImages = new StringBuilder();
			List<string> imageLinks = new List<string>();
			StringBuilder sbFiles = new StringBuilder();
			List<string> fileLinks = new List<string>();
			int count = 0;
			foreach (string str in lines) {
				//make sure it's a live link
				if (!str.Contains("200	ok"))
					continue;
				//split the line to just the url by getting the first item
				string s = str.Split(new char[] { '	' })[0];
				
				bool filterEmpty = txtXenuFilter.Text.Equals("");
				bool filterMatch = s.Contains(txtXenuFilter.Text);
				bool skipEmpty = txtXenuSkip.Text.Equals("");
				bool skipMatch = s.Contains(txtXenuSkip.Text);
				if (!(filterEmpty || filterMatch) || (!skipEmpty && skipMatch))
					continue;

				//if you want to separate by type
				if (chkXenuSeparate.Checked) {
					//if it's a file
					if (s.Contains(".pdf") || s.Contains(".flv") || s.Contains(".swf") || s.Contains(".css") || s.Contains(".js")) {
						if (!fileLinks.Contains(s)) {
							fileLinks.Add(s);
							sbFiles.AppendLine(s);
						}
					} else if (s.Contains(".jpg") || s.Contains(".jpeg") || s.Contains(".png") || s.Contains(".gif")) {
						if (!imageLinks.Contains(s)) {
							imageLinks.Add(s);
							sbImages.AppendLine(s);
						}
					} else {
						if (!links.Contains(s)) {
							links.Add(s);
							sb.AppendLine(s);
						}
					}
				} else {
					//just append to global
					if (!links.Contains(s)) {
						links.Add(s);
						sb.AppendLine(s);
					}
				}
				count++;
			}

			//messaging
			ltlXenu.Text = string.Format("{0} urls found with {1} usable.<br/>", lines.Count, count.ToString());

			WriteFileAndMessage("Links", sb);
			if (chkXenuSeparate.Checked) {
				//create url file
				WriteFileAndMessage("Images", sbImages);
				WriteFileAndMessage("Files", sbFiles);
			}
		}

		protected void btnRefresh_Click(object sender, EventArgs e) {
			populateDropDown("sitelinks", ref ddlFiles);
		}
		
		protected void btnSubmit_Click(object sender, EventArgs e) {
			ltlOutput.Text = "";
			byte[] fbytes = GetFileBytes(ddlFiles.SelectedValue);
			string data = encoding.GetString(fbytes);
			//split urls by breaklines
			List<string> lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			//messaging
			ltlOutput.Text += string.Format("<span class='info'>{0} urls processed.</span><br/>", lines.Count);
			//try to save each url to a file
			foreach (string str in lines) {

				ltlOutput.Text += string.Format("<br/><span class='info'>{0}</span>", str);

				//make a folder based on the domain and subfolders
				List<string> folders = str.Replace("http://", "").Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				folders.RemoveAt(folders.Count - 1);

				//make sure the directory exists
				StringBuilder dirPath = new StringBuilder(Request.PhysicalApplicationPath + @"sites\");
				foreach (string f in folders)
					dirPath.Append(f + @"\");

				byte[] bytes = GetUrlBytes(str.Replace("???", "").Replace("\r", ""));
				if (bytes == null)
					continue;

				//get starting page name from url or file name
				List<string> uri = str.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				List<string> parts = uri[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

				//set filename
				string fileName = string.Empty;
				fileName = parts[parts.Count - 1].Trim();
				//get querystring if there is one
				string qString = string.Empty;
				if (chkAppendQString.Checked && uri.Count > 1)
					qString = string.Format("_{0}_", uri[1]);

				//get extension and make sure there's a period
				string extType = (txtHtml.Text.StartsWith(".")) ? txtHtml.Text : string.Format(".{0}", txtHtml.Text);

				//if is page then build a new file name
				//check page name for exisiting extension type
				if(fileName.Equals("") && !txtDefaultFile.Text.Trim().Equals("")){
					fileName = string.Format("{0}{1}{2}", txtDefaultFile.Text, qString, extType);
					bytes = UpdateLinks(bytes, folders.Count);
				} else {
					List<string> fileTypes = new List<string> { ".aspx", ".asp", ".php", ".cf", ".jsp", ".html" };
					foreach (string s in fileTypes)
						if (fileName.Contains(s)) {
							fileName = string.Format("{0}{1}{2}", fileName.Replace(s, ""), qString, extType);
							bytes = UpdateLinks(bytes, folders.Count);
						}
				}
				
				//build page name from parts
				WriteBytesToFile(string.Format("{0}{1}", dirPath.ToString(), fileName), bytes);
			}
		}

		#endregion Page Events

		#region Helpers

		protected byte[] UpdateLinks(byte[] bytes, int folderCount) {
			if (chkLinkPaths.Checked) {
				string s = encoding.GetString(bytes);
				//must know the depth to know how many times to recursively add a ../ and if there's none then it removes the leading /
				int depth = folderCount - 1;
				string resPath = string.Empty;
				for (int i = depth; i > 0; i--) {
					resPath += "../";
				}
				List<string> replace = new List<string>() { "href=\"", "href='", "src=\"", "src='" };
				foreach (string r in replace) {
					s = s.Replace(r + "/", r + resPath);
				}
				bytes = encoding.GetBytes(s);
			}
			return bytes;
		}

		protected void populateDropDown(string folder, ref DropDownList ddl) {

			ddl.Items.Clear();

			//open directory
			DirectoryInfo di = new DirectoryInfo(Request.PhysicalApplicationPath + folder);
			FileInfo[] rgFiles = di.GetFiles("*.txt");
			//add each file to the dropdown
			foreach (FileInfo fi in rgFiles) {
				ddl.Items.Add(new ListItem(fi.Name, fi.FullName));
			}
		}

		protected byte[] GetFileBytes(string filePath) {
			//open the file selected
			FileInfo f = new FileInfo(filePath);
			FileStream s = f.OpenRead();
			byte[] bytes = new byte[s.Length];
			s.Position = 0;
			s.Read(bytes, 0, int.Parse(s.Length.ToString()));
			return bytes;
		}

		protected byte[] GetUrlBytes(string url) {

			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			if (request == null) {
				throw new NullReferenceException("request is not a http request");
			}
			try {
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				Stream stream = response.GetResponseStream();
				byte[] b = null;
				using (BinaryReader br = new BinaryReader(stream)) {
					b = br.ReadBytes(500000);
					br.Close();
				}
				response.Close();
				return b;
			} catch (WebException ex) {
				ltlOutput.Text += "<ul class='ex'><li class='exception'>" + ex.ToString() + "</li></ul>";
				return null;
			}
		}

		protected void WriteFileAndMessage(string type, StringBuilder fileContents) {
			string filePre = Request.PhysicalApplicationPath + @"sitelinks\" + DateTime.Now.ToString("yyyy.MM.dd") + "_";
			string pageName = System.IO.Path.GetFileName(ddlXenu.SelectedValue);
			string mFormat = "<br/><span class='note'>created {0}</span>";
			string file = string.Format("{0}{1}_{2}.txt", filePre, pageName.Replace(".txt", ""), type);
			WriteBytesToFile(file, encoding.GetBytes(fileContents.ToString()));
			ltlXenu.Text += string.Format(mFormat, file);
		}

		protected void WriteBytesToFile(string filePath, byte[] content) {

			//make a folder based on the domain and subfolders
			List<string> folders = filePath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			folders.RemoveAt(folders.Count - 1);

			//make sure the directory exists
			StringBuilder dirPath = new StringBuilder();
			foreach (string f in folders) {
				dirPath.Append(f + @"\");
				try {
					DirectoryInfo fd = new DirectoryInfo(dirPath.ToString());
					if (!fd.Exists) 
						System.IO.Directory.CreateDirectory(dirPath.ToString());
				} catch (Exception ex) {
					ltlOutput.Text += string.Format("<ul class='ex'><li class='error'>failed to create directory: {0}</li><li class='exception'>{1}</li></ul>", dirPath.ToString(), ex.ToString());
				}
			}

			FileStream fileStream = null;
			BinaryWriter w = null;
			string path = filePath.Replace("/", @"\").Replace(" ", "").Replace(@"\", "\\");
			string invalid = new string(Path.GetInvalidPathChars());
			//strip bad chars
			foreach (char c in invalid) 
				if (path.Contains(c)) 
					path = path.Replace(c.ToString(), "");
				
			try {
				fileStream = new FileStream(path, FileMode.Create);
				fileStream.Write(content, 0, content.Length);
			} catch (Exception ex) {
				ltlOutput.Text += string.Format("<ul class='ex'><li class='error'>failed to save file: {0}</li><li class='exception'>{1}</li></ul>", path, ex.ToString());
			} finally {
				if (w != null)
					w.Close();
				if (fileStream != null)
					fileStream.Close();
			}
		}

		#endregion Helpers

	}
}
