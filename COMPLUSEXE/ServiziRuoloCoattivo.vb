Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports COMPlusService
Imports ComPlusInterface


Public Class ServiziRuoloCoattivo
    Inherits MarshalByRefObject
    Implements IRuoloCoattivo



    Public Function GetDsRuoloCoattivo(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRuoloCoattivo.GetDsRuoloCoattivo
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDsRuoloCoattivo = objCOMPlusBusinessObject.GetDsRuoloCoattivo()

            Return GetDsRuoloCoattivo

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetDsProvvedimentoFiglio(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRuoloCoattivo.GetDsProvvedimentoFiglio
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDsProvvedimentoFiglio = objCOMPlusBusinessObject.GetDsProvvedimentoFiglio()

            Return GetDsProvvedimentoFiglio

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function ElaboraRuoloCoattivo(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataTable Implements ComPlusInterface.IRuoloCoattivo.ElaboraRuoloCoattivo
        Try
            Dim objComplusBusinessObject As New COMPlusBusinessObject
            objComplusBusinessObject.InizializeObject(objHashTable)

            ElaboraRuoloCoattivo = objComplusBusinessObject.ElaboraRuoloCoattivo()
            Return ElaboraRuoloCoattivo
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
End Class
