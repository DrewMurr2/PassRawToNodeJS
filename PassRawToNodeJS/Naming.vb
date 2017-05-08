Public Class Naming
    Public Shared Characters() As String = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}

    Public Shared Function toCode(l As Long) As String
        Dim returnString As String = ""
        Do While l > 0
            Dim md = l Mod 36
            returnString = Characters(md) & returnString
            l = (l - md) / 36
        Loop
        Return returnString
    End Function
    Public Shared Function millisecondsSince2017Utc() As Long
        Return (Date.UtcNow().Ticks - New Date(2017, 1, 1).Ticks) / 10000
    End Function
    Public Shared Function newName() As String
        Return toCode(millisecondsSince2017Utc())
    End Function
End Class
