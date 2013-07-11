﻿using System;
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

		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				//set files to choose from
				//xenu
				populateDropDown("xenulinks", ref ddlXenu);

				//site links
				populateDropDown("sitelinks", ref ddlFiles);
			}
		}

		protected void populateDropDown(string folder, ref DropDownList ddl) {
			
			//open directory
			DirectoryInfo di = new DirectoryInfo(Request.PhysicalApplicationPath + folder);
			FileInfo[] rgFiles = di.GetFiles("*.txt");
			//add each file to the dropdown
			foreach (FileInfo fi in rgFiles) {
				ddl.Items.Add(new ListItem(fi.Name, fi.FullName));
			}
		}
				
		protected string GetPageName(string path){
			//break up the url by slashes
			string pageName = "";

			StringBuilder qString = new StringBuilder();
			if (path.Contains("http")) {
				List<string> uri = path.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				List<string> parts = uri[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				
				//build the querystring into a string for the file name
				pageName = parts[parts.Count - 1];
				if (chkAppendQString.Checked && uri.Count > 1) {
					List<string> keyvals = uri[1].Split(new string[] {"&"}, StringSplitOptions.RemoveEmptyEntries).ToList();
					foreach (string param in keyvals) {
						List<string> kv = param.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries).ToList();
						if (kv.Count > 1) {
							qString.Append("_" + kv[1].Trim());
						}
					}
				}
			} else if(path.Contains(@"\")){
				pageName = System.IO.Path.GetFileName(path); 
			}
			
			//change to html if requested
			if (chkHtml.Checked) {
				List<string> fileTypes = new List<string>{".aspx", ".asp", ".php" };
				foreach(string s in fileTypes){
					if(pageName.Contains(s)){
						pageName = pageName.Replace(s, ".html");
					}
				}
			}
			//need to account for the homepage not just html ones
			if (pageName.Trim().Equals("")) {
				//TODO:: probably should have a field for a default value
				pageName = "default.html";
			}
			if (chkAppendQString.Checked) {
				List<string> pageParts = pageName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
				if (pageParts.Count > 1) {
					pageParts[pageParts.Count - 2] = pageParts[pageParts.Count - 2] + qString.ToString();
					StringBuilder sb2 = new StringBuilder();
					foreach (string part in pageParts) {
						if (sb2.Length > 0) {
							sb2.Append(".");
						}
						sb2.Append(part);
					}
					pageName = sb2.ToString();
				} else {
					ltlOutput.Text += "<br/><span class='error'>failed appending qstring: " + qString.ToString() + " to: " + pageName + "</span>";
				}
			}
			return pageName;
		}
		
		protected void WriteBytesToFile(string filePath, byte[] content) {

			//make a folder based on the domain and subfolders
			List<string> folders = filePath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			folders.RemoveAt(folders.Count - 1);

			//make sure the directory exists
			StringBuilder dirPath = new StringBuilder();
			foreach (string f in folders) {
				//if (!f.Equals("~")) {
					dirPath.Append(f + @"\");
					try {
						DirectoryInfo fd = new DirectoryInfo(dirPath.ToString());
						//if not create it
						if (!fd.Exists) {
							DirectoryInfo di = System.IO.Directory.CreateDirectory(dirPath.ToString());
						}
					}catch(Exception ex){
						ltlOutput.Text += "<br/><span class='error'>failed to create directory: " + dirPath.ToString() + "</span><br/><br/><div class='exception'>" + ex.ToString() + "</div><hr/>";
					}
				//}
			}
			
			FileStream fileStream = null;
			BinaryWriter w = null;
			string path = filePath.Replace("/", @"\").Replace(" ", "").Replace(@"\", "\\");
			string invalid = new string(Path.GetInvalidPathChars());
			//strip bad chars
			foreach (char c in invalid) {
				if (path.Contains(c)) {
					path = path.Replace(c.ToString(), "");
				}
			}

			try {

				fileStream = new FileStream(path, FileMode.Create);
				fileStream.Write(content, 0, content.Length);
				
				//w = new BinaryWriter(fileStream);
				//w.Write(content); 

			} catch (Exception ex) {
				ltlOutput.Text += "<br/><span class='error'>failed to save file: " + path + "</span><br/><br/><div class='exception'>" + ex.ToString() + "</div><hr/>";
			} finally {
				if(w != null)
					w.Close();
				if(fileStream != null)
					fileStream.Close();
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
				ltlOutput.Text += "<br/><br/><span class='error'>failed on: " + url + "</span><br/><hr/><br/>" + ex.ToString() + "<br/><hr/>";
				return null;
			}
		}
		
		protected void btnXenu_Click(object sender, EventArgs e) {
			ltlXenu.Text = "";
			byte[] bytes = GetFileBytes(ddlXenu.SelectedValue);
			string data = encoding.GetString(bytes);

			//split urls by breaklines
			List<string> lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			//messaging
			ltlOutput.Text = lines.Count + " urls found.<br/>";
			//try to save each url to a file
			StringBuilder sb = new StringBuilder();
			List<string> links = new List<string>();
			StringBuilder sbImages = new StringBuilder();
			List<string> imageLinks = new List<string>();
			StringBuilder sbFiles = new StringBuilder();
			List<string> fileLinks = new List<string>();
			foreach (string str in lines) {
				//make sure it's a live link
				if (str.Contains("200	ok")) {
					//split the line to just the url by getting the first item
					string s = str.Split(new char[] { '	' })[0];
					//check for filter
					if ((s.Contains(txtXenuFilter.Text) || txtXenuFilter.Text.Equals("")) && !s.Contains(txtXenuRemove.Text)) {
						//if you want to separate by type
						if (chkXenuSeparate.Checked) {
							//if it's a file
							if (s.Contains(".pdf") || s.Contains(".flv") || s.Contains(".swf")) {
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
					}
				}
			}

			string pageName = GetPageName(ddlXenu.SelectedValue);
			if (chkXenuSeparate.Checked) {
				string filePre = Request.PhysicalApplicationPath + @"sitelinks\" + DateTime.Now.ToString("yyyy.MM.dd") + "_";
				//create url file
				WriteBytesToFile(filePre + "_URLs_" + pageName, encoding.GetBytes(sb.ToString()));
				ltlXenu.Text += "<br/><span class='note'>created " + filePre + "_URLs_" + pageName + "</span>";
				WriteBytesToFile(filePre + "_Images_" + pageName, encoding.GetBytes(sbImages.ToString()));
				ltlXenu.Text += "<br/><span class='note'>created " + filePre + "_Images_" + pageName + "</span>";
				WriteBytesToFile(filePre + "_Files_" + pageName, encoding.GetBytes(sbFiles.ToString()));
				ltlXenu.Text += "<br/><span class='note'>created " + filePre + "_Files_" + pageName + "</span>";
			} else {
				string fileName = Request.PhysicalApplicationPath + @"sitelinks\" + DateTime.Now.ToString("yyyy.MM.dd") + "_" + pageName;
				WriteBytesToFile(fileName, encoding.GetBytes(sb.ToString()));
				ltlXenu.Text += "<br/><span class='note'>created " + fileName + "</span>";
			}
			
		}

		protected void btnSubmit_Click(object sender, EventArgs e) {
			ltlOutput.Text = "";
			byte[] fbytes = GetFileBytes(ddlFiles.SelectedValue);
			string data = encoding.GetString(fbytes);
			//split urls by breaklines
			List<string> lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			//messaging
			ltlOutput.Text += "<span class='info'>" + lines.Count + " urls found.</span><br/>";
			//try to save each url to a file
			foreach (string str in lines) {

				ltlOutput.Text += "<br/><span class='info'>" + str +"</span>";
				byte[] bytes = GetUrlBytes(str.Replace("???", ""));

				//make a folder based on the domain and subfolders
				List<string> folders = str.Replace("http://", "").Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				folders.RemoveAt(folders.Count - 1);

				//make sure the directory exists
				StringBuilder dirPath = new StringBuilder(Request.PhysicalApplicationPath + @"sites\");
				foreach (string f in folders) {
					//if (!f.Equals("~")) {
						dirPath.Append(f + @"\");
						DirectoryInfo fd = new DirectoryInfo(dirPath.ToString());
						//if not create it
						if (!fd.Exists) {
							DirectoryInfo di = System.IO.Directory.CreateDirectory(dirPath.ToString());
						}
					//}
				}
				WriteBytesToFile(dirPath.ToString() + GetPageName(str), bytes);
			}
		}

	}
}
