<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_07___ClientWebservice_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Afterlaunch - ClientWebservice</title>
    <link rel="Stylesheet" href="../style/common.css" />

    <script type="text/javascript">
        var displayResults;
        var loadingGif;
        var abortExecutor;
        
        function pageLoad() {
            // Event Handler registrieren
            
            $addHandler($get('btnGetBooks'), 'click', GetBooks);
            $addHandler($get('btnAbort'), 'click', AbortTransmission);
            $addHandler($get('btnPageMethod'), 'click', CallPageMethod);
            
            loadingGif = $get("loadingGif");
            displayResults = $get("displayResults");
            
            Sys.UI.DomElement.setVisible(loadingGif, false);
        }

        function pageUnload() {
            $removeHandler($get('btnGetBooks'), 'click', GetBooks);
            $removeHandler($get('btnAbort'), 'click', AbortTransmission);
        }

        function GetBooks() {
            Sys.UI.DomElement.setVisible(loadingGif,true);
            
            // Falls Abbrechen benötigt wird
            Sys.Net.WebRequestManager.add_invokingRequest(On_InvokingRequest); 
            
            eBooksService.GetAllBooksSlow(OnSucceededCallback, OnFailedCallback);
            
            Sys.Net.WebRequestManager.remove_invokingRequest(On_InvokingRequest); 
        }
        
        function On_InvokingRequest(executor, eventArgs)
        {
           // Hole Executor um den Aufruf ggf. abzubrechen
           abortExecutor = eventArgs.get_webRequest().get_executor();
        }
                
        function OnSucceededCallback(result, userContext, methodName)
        { 
            if (result != null)
            {
                var _book;
                var sb = new Sys.StringBuilder();
                for (_book in result)
                { 
                    sb.append('<div style=\"height: 60px;\"><img style=\"float:left\" src=\"../images/book.png\" />');
                    sb.append('<a href=\"'+result[_book].BookUrl+'\" target=\"_blank\">'+result[_book].Title+'</a>');
                    sb.append('</div>');                    
                }           
                displayResults.innerHTML = sb.toString();
                Sys.UI.DomElement.setVisible(loadingGif,false);                
            }
        }


        function OnFailedCallback(error, userContext, methodName) 
        {
            alert("Error: " + error.get_message() + " " + userContext +", " + methodName);
            Sys.UI.DomElement.setVisible(loadingGif,false);
        }
        
        function AbortTransmission(){
            abortExecutor.abort();
        }
        
        function CallPageMethod(){
            PageMethods.GetTime(OnSucceeded, OnFailedCallback);
        }

        function OnSucceeded(result, userContext, methodName) 
        {
            if (methodName == "GetTime")
            {
                displayResults.innerHTML = result;
            }
        }

    
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/eBooksService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div>
        <div>
            Afterlaunch - ClientWebservice
            <span id="loadingStatus"><img id="loadingGif" alt="Loading" src="../images/ajax-loader.gif" /></span>
        </div>
        <div id="item-container-1" style="float: left; border: dashed 2px #aaa; width: 700px;
            height: auto;margin:10px;padding:10px">
            <div id="displayResults">
            </div>                      
        </div>
        <div>
            <input type="button" id="btnGetBooks" value="Abrufen" />
            <input type="button" id="btnAbort" value="Abbrechen" />
            <input type="button" id="btnPageMethod" value="PageMethod" />
        </div>
    </div>
    </form>
</body>
</html>
