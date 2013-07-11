<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SiteScraper._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>site scraping tool</title>
    <link href="/presentation/css/styles.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrapper">
		<div class="section">
			<h1>Import Xenu Links</h1>
			<asp:DropDownList ID="ddlXenu" runat="server"></asp:DropDownList>
			<asp:Button ID="btnRefreshXenu" OnClick="btnXenuRefresh_Click" Text="refresh" runat="server" />
            <br /><br />
			<asp:Label ID="lblXenuSeparate" runat="server" AssociatedControlID="chkXenuSeparate" Text="Separate links by type into separate files"></asp:Label>
			<asp:CheckBox ID="chkXenuSeparate" runat="server"></asp:CheckBox>
			<br /><br />
			<asp:Label ID="lblXenuFilter" runat="server" AssociatedControlID="txtXenuFilter" Text="Enter Domain You Want to Keep"></asp:Label>
			<asp:TextBox ID="txtXenuFilter" Text="" runat="server"></asp:TextBox>
			<br /><br />
			<asp:Label ID="lblXenuSkip" runat="server" AssociatedControlID="txtXenuSkip" Text="Skip Entries Containing:"></asp:Label>
			<asp:TextBox ID="txtXenuSkip" Text="" runat="server"></asp:TextBox>
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
            <asp:Button ID="btnRefresh" OnClick="btnRefresh_Click" Text="refresh" runat="server" />
            <br /><br />
			<asp:Button ID="bntSubmit" OnClick="btnSubmit_Click" Text="submit" runat="server" />
			<br /><br />
			<asp:Literal ID="ltlOutput" runat="server"></asp:Literal>
		</div>
	</div>
    </form>
</body>
</html>
