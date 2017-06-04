' VB9 kann Xml Schemas importieren und bietet dann IntelliSense für Xml Daten.
Imports <xmlns="http://schemas.linq.com/linqbooks/2008/">

''' <summary>
''' Linq To Xml Demo Code
''' </summary>
Module Program

    Sub Main()
        ' VB9 unterstütz die Definition von Xml Daten direkt im Source Code.
        Dim xmlBooks = _
        <?xml version="1.0" encoding="utf-8"?>
        <LinqBooks>
            <Book Id="11EB678E-D204-4e0b-8802-3C1EF4119793">
                <Title>Foundations of Linq in C#</Title>
                <Authors>
                    <Author>Joseph C. Rattz</Author>
                </Authors>
            </Book>
            <Book Id="033C204B-6BA9-4d04-82B2-802DFD84AB34">
                <Title>Microsoft LINQ - Crashkurs</Title>
                <Authors>
                    <Author>Paolo Pialorsi</Author>
                </Authors>
            </Book>
            <Book Id="1B9596C1-BEDA-4774-9EB7-FAEDEC974B9E">
                <Title>LINQ in Action</Title>
                <Authors>
                    <Author>Fabrice Marguerie</Author>
                    <Author>Steve Eichert</Author>
                    <Author>Jim Wooley</Author>
                </Authors>
            </Book>
        </LinqBooks>

        ' C# developer? It's time to change ;)


        ' Queries über Xml Daten lassen GENAU SO definieren wie Queries gegen Objekte
        ' oder Queries gegen Datenbanken.
        ' VB9 hat hier sogar IntelliSense für die Xml Elemente.

        ' In dieser Query werden die Xml Daten auf .NET Objekte projeziert.
        ' Das Xml-Attribut Id auf das Property Id, das Xml-Element Title auf das Property Title
        ' und die Xml Elemente von Authors in eine Liste von Strings.
        Dim query = From book In xmlBooks.<LinqBooks>.<Book> _
                    Select New With {.Id = book.@Id, _
                                     .Title = book.<Title>.Value, _
                                     .Authors = (From x In book.<Authors>.<Author> _
                                                 Select x.Value).ToList}

        ' Daten in den StringBuilder dumpen und auf der Console ausgeben.
        Dim template = New System.Text.StringBuilder

        For Each book In query
            ' Hinweis: Für das book Objekt haben wir IntelliSense.

            template.AppendLine(book.Title)

            For Each authorName In book.Authors
                template.AppendFormat(" {0}", authorName)
                template.AppendLine()
            Next

            template.AppendLine()
        Next

        Console.WriteLine(template.ToString)

        Console.ReadLine()
    End Sub

End Module
