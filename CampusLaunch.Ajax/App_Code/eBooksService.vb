Option Strict On

Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Web.Script.Services
Imports System.ComponentModel
Imports Microsoft.Web.Preview.Services

<WebService(Namespace:="http://www.afterlaunch.de/services/Ajax_eBooks/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ScriptService(), GenerateScriptType(GetType(eBook))> _
Public Class eBooksService
    Inherits System.Web.Services.WebService

    <WebMethod(), ScriptMethod(UseHttpGet:=True), DataObjectMethod(DataObjectMethodType.Select)> _
    Public Function GetAllBooks() As eBook()
        Return New eBook() { _
            New eBook With { _
                .Title = "What's Ajax?", _
                .BookUrl = "../ebooks/WhatsAjax.pdf", _
                .ThumbnailUrl = "../ebooks/WhatsAjax.png" _
                }, _
                New eBook With { _
                .Title = "Ajax DomElement", _
                .BookUrl = "../ebooks/DomElement.pdf", _
                .ThumbnailUrl = "../ebooks/DomElement.png" _
                }, _
                New eBook With { _
                .Title = "Ajax Client Life-Cycle", _
                .BookUrl = "../ebooks/ClientLifeCycle.pdf", _
                .ThumbnailUrl = "../ebooks/ClientLifeCycle.png" _
                }, _
                New eBook With { _
                .Title = "Ajax Array Extension", _
                .BookUrl = "../ebooks/ArrayExtension.pdf", _
                .ThumbnailUrl = "../ebooks/ArrayExtension.png" _
                }, _
                New eBook With { _
                .Title = "Ajax String & Object Extension", _
                .BookUrl = "../ebooks/StringAndObjectExtension.pdf", _
                .ThumbnailUrl = "../ebooks/StringAndObjectExtension.png" _
                } _
         }
    End Function

    <WebMethod(), ScriptMethod(UseHttpGet:=True)> _
    Public Function GetAllBooksSlow() As eBook()
        System.Threading.Thread.Sleep(3000)
        Return GetAllBooks()
    End Function

    <WebMethod(), ScriptMethod(ResponseFormat:=ResponseFormat.Xml)> _
    Public Function GetAllBooksXml() As eBook()
        Throw New IO.IOException("Mist gebaut")
        Return GetAllBooks()
    End Function
End Class
