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
			<h1>Import <a target="_blank" href="http://home.snafu.de/tilman/xenulink.html">Xenu</a> Links</h1>
			<ul>
                <li>
                    <asp:DropDownList ID="ddlXenu" runat="server"></asp:DropDownList>
			        <asp:Button ID="btnRefreshXenu" OnClick="btnXenuRefresh_Click" Text="refresh" runat="server" />
                </li>
                <li>
                    <asp:Label ID="lblXenuSeparate" runat="server" AssociatedControlID="chkXenuSeparate" Text="Separate links by type into separate files (links, images and files)"></asp:Label>
			        <asp:CheckBox ID="chkXenuSeparate" runat="server"></asp:CheckBox>
                </li>
                <li>
                    <asp:Label ID="lblXenuFilter" runat="server" AssociatedControlID="txtXenuFilter" Text="Keep entries containing..."></asp:Label>
			        <asp:TextBox ID="txtXenuFilter" Text="" runat="server"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblXenuSkip" runat="server" AssociatedControlID="txtXenuSkip" Text="Skip entries containing..."></asp:Label>
			        <asp:TextBox ID="txtXenuSkip" Text="" runat="server"></asp:TextBox>
                </li>
			</ul>
            <asp:Button ID="btnXenu" OnClick="btnXenu_Click" Text="submit" runat="server" />
            <br /><br />
            <asp:Literal ID="ltlXenu" runat="server"></asp:Literal>
		</div>
		<div class="section">
			<h1>Scrape Site Links</h1>
			<ul>
                <li>
                    <asp:DropDownList ID="ddlFiles" runat="server"></asp:DropDownList>
                    <asp:Button ID="btnRefresh" OnClick="btnRefresh_Click" Text="refresh" runat="server" />
                </li>
                <li>
                    <asp:Label ID="lblLinkPaths" runat="server" AssociatedControlID="chkLinkPaths" Text="Update Links to Relative Paths"></asp:Label>
			        <asp:CheckBox ID="chkLinkPaths" Checked="true" runat="server" />
			    </li>
                <li>
                    <asp:Label ID="lblHtml" runat="server" AssociatedControlID="txtHtml" Text="Save pages as..."></asp:Label>
			        <asp:TextBox ID="txtHtml" Text=".html" runat="server" />
			    </li>
                <li>
                    <asp:Label ID="lblDefaultFile" runat="server" AssociatedControlID="txtDefaultFile" Text="Default Page File Name (default, index, etc.)"></asp:Label>
			        <asp:TextBox ID="txtDefaultFile" Text="default" runat="server" />
			    </li>
                <li>
                    <asp:Label ID="lblAppendQString" runat="server" AssociatedControlID="chkAppendQString" Text="Append Querystring to Filename"></asp:Label>
			        <asp:CheckBox ID="chkAppendQString" runat="server" />
			    </li>
            </ul>
			<asp:Button ID="bntSubmit" OnClick="btnSubmit_Click" Text="submit" runat="server" />
			<div class="output">
                <asp:Literal ID="ltlOutput" runat="server"></asp:Literal>
            </div>
		</div>
	</div>
    </form>
</body>
</html>
