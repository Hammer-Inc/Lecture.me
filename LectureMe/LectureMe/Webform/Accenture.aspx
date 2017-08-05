<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Accenture.aspx.cs" Inherits="LectureMe.Webform.Accenture" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload" runat="server" />
            <asp:Button ID="btn_Upload" runat="server" Text="Button" OnClick="btn_Upload_Click"/>
        </div>
    </form>
</body>
</html>
