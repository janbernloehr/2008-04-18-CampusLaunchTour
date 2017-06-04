<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_05___Debugging_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Afterlaunch - Debugging</title>
    <link rel="Stylesheet" href="../style/common.css" />
    <script type="text/javascript">

    function pageLoad() {
        //debugger;
    }

    function btnAssert_onclick() {
        var n;
        if (false) n = 3;
        Sys.Debug.assert(n > 0, "n sollte 3 sein", true);
    }

    function btnFail_onclick() {
        var n;
        if (false) n = 3;
        if (isNaN(n)) Sys.Debug.fail("n sollte eine Zahl sein");
    }

    function btnTrace_onclick() {
        var v = $get("text1").value;
        Sys.Debug.trace("Etwas set to " + "\"" + v + "\".");
        //alert("Etwas: " + v + ".");
    }

    function btnDump_onclick() {
        Sys.Debug.traceDump($get("text1").value, "Name textbox");
        //alert("Hello " + form1.text1.value + ".");
    }

    function btnClear_onclick() {
        Sys.Debug.clearTrace()
        alert("Trace cleared.");
    }
    
    function btnWeb_onclick() {
        eBooksService.GetAllBooksXml(OnSucceededCallback, OnFailedCallback);
    }
    function OnSucceededCallback(result, userContext, methodName)
    { 
        if (result != null)
        {
            //alert(result.toString());
            $get("xml").innerXml = result;
        }
    }

    function OnFailedCallback(error, userContext, methodName) 
    {
        Sys.Debug.trace("Fehlermeldung:" + error.get_message());
        Sys.Debug.trace("ExceptionTyp:" + error.get_exceptionType());
        Sys.Debug.trace("StackTrace:" + error.get_stackTrace());           
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="scriptManager" ScriptMode="Debug">
        <Services>
            <asp:ServiceReference Path="~/services/eBooksService.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Ajax.Logging.ExceptionManager.js"/>
        </Scripts>
    </asp:ScriptManager>
    <h2>
        Afterlaunch - Sys.Debug</h2>
    <p>
        <b>assert() = Debugger, falls erste Bedingung false ausgewertet wird<br />
         fail() = Debugger, falls Fail aufgerufen wird<br /></b>
        <input id="btnAssert" type="button" value="Assert" style="width: 100px" onclick="return btnAssert_onclick()" />
        &nbsp
        <input id="btnFail" type="button" value="Fail" style="width: 100px" onclick="return btnFail_onclick()" />
    </p>
    <hr />
    <b>Tracing in &lt;textarea&gt; Element mit ID TraceConsole. </b>    
    <p style="float:left;width:300px">
        <br />Etwas eingeben:<br />
        <input id="text1" style="width:100px" maxlength="100" type="text" />
        <br />
        <br />
        <input id="btnTrace" type="button" value="Trace" style="width: 100px" onclick="return btnTrace_onclick()" /><br />
        <input id="btnDump" type="button" value="TraceDump" style="width: 100px" onclick="return btnDump_onclick()" /><br />
        <input id="btnClear" type="button" value="ClearTrace" style="width: 100px" onclick="return btnClear_onclick()" /><br /><input id="btnWebService" type="button" value="Webservice" style="width: 100px" onclick="return btnWeb_onclick()" />
  <div id="xml"></div>
        <br />
    </p>
    <p>
        TraceConsole:
        <br />
        <textarea id='TraceConsole' rows="10" cols="50" title="TraceConsole"></textarea>
    </p>
    </form>
</body>
</html>
