<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SiteScraper._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>site scraping tool</title>
    <style>
		body {
			margin:0px;
			padding:0px;
			font-family:Arial;
			font-size:12px;
			background-color:#DEDEDE;
		}
		a {
			color:#C20AFF;
			text-decoration:underline;
		}
		a:hover {
			text-decoration:none;
		}
		.wrapper {
			width:600px;
			margin:0px auto;
		}
		.section {
			padding:20px;
			margin:20px 0;
			background-color:#cccccc;
		}
		h1 {
			font-size:14px;
		}
		.error {
			color:#9933CC;
		}
		.exception {
			color:#9933CC;
			font-weight:bold;
		}
		.note {
			font-weight:bold;
		}
		.info {
		}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrapper">
		<div class="section">
			<h1>Import Xenu Links</h1>
			<asp:DropDownList ID="ddlXenu" runat="server"></asp:DropDownList>
			<br /><br />
			<asp:Label ID="lblXenuSeparate" runat="server" AssociatedControlID="chkXenuSeparate" Text="Separate links by type into separate files"></asp:Label>
			<asp:CheckBox ID="chkXenuSeparate" runat="server"></asp:CheckBox>
			<br /><br />
			<asp:Label ID="lblXenuFilter" runat="server" AssociatedControlID="txtXenuFilter" Text="Enter Domain Filter"></asp:Label>
			<asp:TextBox ID="txtXenuFilter" Text="" runat="server"></asp:TextBox>
			<br /><br />
			<asp:Label ID="lblXenuRemove" runat="server" AssociatedControlID="txtXenuRemove" Text="Remove URLs Containing:"></asp:Label>
			<asp:TextBox ID="txtXenuRemove" Text="" runat="server"></asp:TextBox>
			<br /><br />			
			<asp:Button ID="btnXenu" OnClick="btnXenu_Click" Text="submit" runat="server" />
			<br /><br />
			<asp:Literal ID="ltlXenu" runat="server"></asp:Literal>
		</div>
		<div class="section">
			<h1>Scrape Site Links</h1>
			<asp:Label ID="lblHtml" runat="server" AssociatedControlID="chkHtml" Text="Save pages as .html"></asp:Label>
			<asp:CheckBox ID="chkHtml" runat="server" />
			<br /><br />
			<asp:Label ID="lblAppendQString" runat="server" AssociatedControlID="chkAppendQString" Text="Append Querystring to Filename"></asp:Label>
			<asp:CheckBox ID="chkAppendQString" runat="server" />
			<br /><br />
			<asp:DropDownList ID="ddlFiles" runat="server"></asp:DropDownList>
			<br /><br />
			<asp:Button ID="bntSubmit" OnClick="btnSubmit_Click" Text="submit" runat="server" />
			<br /><br />
			<asp:Literal ID="ltlOutput" runat="server"></asp:Literal>
		</div>
	</div>
    </form>
</body>
</html>
