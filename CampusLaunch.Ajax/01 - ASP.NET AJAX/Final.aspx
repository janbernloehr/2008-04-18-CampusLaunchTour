<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Final.aspx.vb" Inherits="Demo_01_ASPNET_Final" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CampusLaunch Tour 08 - AJAX</title>
    <link rel="Stylesheet" href="../style/common.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <h1>
        Event Anmeldung</h1>
    <div>
        <asp:Label ID="Label1" runat="server" Text="Wählen Sie ein Datum"></asp:Label>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
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
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="Progress1">
            <ProgressTemplate>
                <asp:Image runat="server" ImageUrl="~/images/ajax-loading.gif" />
                lade ...
            </ProgressTemplate>
        </asp:UpdateProgress>
        <br />
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
            <ContentTemplate>
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
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="EventCalendar" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
