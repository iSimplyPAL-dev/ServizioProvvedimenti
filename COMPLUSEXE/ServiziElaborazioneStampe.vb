Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface


Class ServiziElaborazioneStampe
  Inherits MarshalByRefObject
	Implements IElaborazioneStampa

	Public Function GetStampa(ByVal arrayFileAspetta() As Object, ByVal strPErcorso As String, ByVal nomefilefionale As String, ByVal Errore As String) As String Implements ComPlusInterface.IElaborazioneStampa.GetStampa
    'Try
    '	Dim xmlDot As New GeneraXMLDOTNET.XMLDOT
    '	GetStampa = xmlDot.MerginFile(arrayFileAspetta, strPErcorso, nomefilefionale, Errore)
    'Catch ex As Exception

    '	Throw New Exception(ex.Message & "::" & ex.StackTrace)
    'End Try
    'Return GetStampa
	End Function
End Class
