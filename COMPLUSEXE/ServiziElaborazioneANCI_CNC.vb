Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports System.Messaging
Imports System.Threading
Public Class ServiziElaborazioneANCI_CNC
  Inherits MarshalByRefObject
  Implements IElaborazioneANCI_CNC
  Delegate Function DelANCI_CNC_() As Boolean
  Public Function CreateANCI_CNC(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneANCI_CNC.CreateANCI_CNC

    Dim strMessage As String = ViewCodaElaborazioneANCI_CNC(objHashTable)
    Dim strInizioElaborazione As String = "Elaborazione in Corso".ToUpper

    'If StrComp(strMessage, strInizioElaborazione) = 0 Then
    '  Return False
    '  Exit Function
    'End If

    Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
    objCOMPlusBusinessObject.InizializeObject(objHashTable)

    Dim RemoteDel As New DelANCI_CNC_(AddressOf objCOMPlusBusinessObject.CreateANCI_CNC)
    Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(Nothing, Nothing)

    Return True
  End Function

  Public Function ViewCodaElaborazioneANCI_CNC(ByVal objHashTable As System.Collections.Hashtable) As String Implements ComPlusInterface.IElaborazioneANCI_CNC.ViewCodaElaborazioneANCI_CNC
    Dim m As New Message
    Dim strID As String

    Dim strUSER As String = CType(objHashTable("USER"), String)
    Const QUEUE_NAME As String = ".\private$\ElaborazioneANCICNC"
    Dim queue As MessageQueue

    Dim strInizioElaborazione As String = "Inizio Elaborazione".ToUpper
    Dim strFineElaborazione As String = "Fine Elaborazione".ToUpper
    Dim strErroreElaborazione As String = "Errore durante l'elaborazione dei File ANCI_CNC".ToUpper

    ViewCodaElaborazioneANCI_CNC = ""


        Try
            If Not MessageQueue.Exists(QUEUE_NAME) Then
                queue = queue.Create(QUEUE_NAME, False)
                queue.Label = "Coda Elaborazione ANCICNC"
            Else
                queue = New MessageQueue(QUEUE_NAME)
            End If
            queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
            queue.SetPermissions("Administrator", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
            queue.UseJournalQueue = False
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Dim messaggi() As Message = queue.GetAllMessages()


        For Each m In messaggi

            m.Formatter = New BinaryMessageFormatter

            If StrComp(m.Label, strUSER) = 0 Then
                If StrComp(UCase(CStr(m.Body)), strInizioElaborazione) = 0 Then
                    Return "Elaborazione in Corso".ToUpper
                End If

                If StrComp(UCase(CStr(m.Body)), strFineElaborazione) = 0 Then
                    strID = m.Id()
                    'Prelevo il messaggio nella coda e lo elimino
                    m = queue.ReceiveById(strID)
                    Return "Elaborazione Terminata con successo".ToUpper

                End If
                If StrComp(UCase(CStr(m.Body)), strErroreElaborazione) = 0 Then

                    strID = m.Id()
                    'Prelevo il messaggio nella coda e lo elimino
                    m = queue.ReceiveById(strID)
                    Return "Elaborazione Terminata con errori".ToUpper
                End If
            End If
        Next

        Return "Non ci sono elaborazioni in corso".ToUpper
  End Function
End Class
