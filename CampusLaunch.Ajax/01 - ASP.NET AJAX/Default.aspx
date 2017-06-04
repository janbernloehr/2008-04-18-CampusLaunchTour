<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Demo_01_ASPNET_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CampusLaunch Tour 08 - AJAX</title>
    <link rel="Stylesheet" href="../style/common.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" />
    <h1>
        Event Anmeldung</h1>
    <div>
        <asp:Label ID="Label1" runat="server" Text="Wählen Sie ein Datum"></asp:Label>
        <asp:Calendar ID="EventCalendar" runat="server" BackColor="White" BorderColor="#999999"
            CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt"
            ForeColor="Black" Height="180px" Width="200px">
            <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
            <SelectorStyle BackColor="#CCCCCC" />
            <WeekendDayStyle BackColor="#FFFFCC" />
            <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
            <OtherMonthDayStyle ForeColor="#808080" />
            <NextPrevStyle VerticalAlign="Bottom" />
            <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
            <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
        </asp:Calendar>
        <br />
        <asp:Label runat="server" ID="EventLabel" Text="Gefundene Events" Visible="false" /><br />
        <br />
        <asp:Repeater runat="server" ID="EventRepeater">
            <ItemTemplate>
                Ort: <b>
                    <%#Eval("City")%></b>
                <asp:HyperLink ID="HyperLink1" runat="server" Text="Anmelden" NavigateUrl="#" />
                <br />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
