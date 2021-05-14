Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports System.Messaging
Imports System.Threading
'*******************************************************
'
' ServiziElaborazioneQuestionari() Class <a name="ServiziElaborazioneQuestionari"></a>
'
' La classe ServiziElaborazioneQuestionari 
' sarà da utilizzare per la gestione dell'iter dei questionari da parte di OPENgovProvvedimenti
' 
'*******************************************************
Public Class ServiziElaborazioneQuestionari
  Inherits MarshalByRefObject
  Implements IElaborazioneQuestionari
  Delegate Function DelCreateQuestionari(ByVal objDSAnagrafico As DataSet) As Boolean
  '*******************************************************
  '
  ' GetDatiQuestionari() Function <a name="GetDatiQuestionari"></a>
  '
  ' La funzione GetDatiQuestionari
  ' sarà da utilizzare per la ricerca dei contribuenti con Dichiarazioni da Bonificare 
  ' Restituisce un dataset con i dati dei contribuenti  che saranno esposti in
  'una griglia
  '*******************************************************
  Public Function GetDatiQuestionari(ByVal objHashTable As Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneQuestionari.GetDatiQuestionari
    Try

      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)
      '******************************************************************************************************
      'Modalita di ricerca Manuale
      '******************************************************************************************************
      If CType(objHashTable("Manuale"), Boolean) Then
        GetDatiQuestionari = objCOMPlusBusinessObject.getQuestionariRicercaManualeDichiarazioniDaBonificare()
      End If
      '******************************************************************************************************
      'Modalita di ricerca Massiva
      '******************************************************************************************************
      If CType(objHashTable("Massiva"), Boolean) Then

        If CType(objHashTable("CHECKDAA"), Boolean) Then
          GetDatiQuestionari = objCOMPlusBusinessObject.getQuestionariRicercaMassivaDichiarazioniDaBonificareDAA()
        End If
        If CType(objHashTable("CHEKAREE"), Boolean) Then
          GetDatiQuestionari = objCOMPlusBusinessObject.getQuestionariRicercaMassivaDichiarazioniDaBonificareAREE()
        End If
        If CType(objHashTable("CHEKVIARESIDENZA"), Boolean) Then
          GetDatiQuestionari = objCOMPlusBusinessObject.getQuestionariRicercaMassivaDichiarazioniDaBonificareVIA()
        End If
        If CType(objHashTable("CHEKUBICAZIONE"), Boolean) Then
          GetDatiQuestionari = objCOMPlusBusinessObject.getQuestionariRicercaMassivaDichiarazioniDaBonificareUBICAZIONE()
        End If

      End If

      Return GetDatiQuestionari
    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function
  '***********************************************************************************************************
  ' CreateQuestionari() Function <a name="CreateQuestionari"></a>
  ' La funzione CreateQuestionari
  ' sarà da utilizzare per la creazione dei Questionari per i contribuenti con Dichiarazioni da Bonificare 
  '***********************************************************************************************************
  Public Function CreateQuestionari(ByVal objHashTable As System.Collections.Hashtable, _
                                    ByVal objDSAnagrafico As System.Data.DataSet) As Boolean Implements ComPlusInterface.IElaborazioneQuestionari.CreateQuestionari

    Dim strMessage As String = ViewCodaElaborazioneQuestionari(objHashTable, objDSAnagrafico)
    Dim strInizioElaborazione As String = "Elaborazione in Corso".ToUpper

    If StrComp(strMessage, strInizioElaborazione) = 0 Then
      Return False
      Exit Function
    End If

    Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
    objCOMPlusBusinessObject.InizializeObject(objHashTable)

    Dim RemoteDel As New DelCreateQuestionari(AddressOf objCOMPlusBusinessObject.CreateQuestionari)
    Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(objDSAnagrafico, Nothing, Nothing)

    Return True

  End Function

  Public Function ViewCodaElaborazioneQuestionari(ByVal objHashTable As System.Collections.Hashtable, _
                                                  ByVal objDSAnagrafico As System.Data.DataSet) As String Implements ComPlusInterface.IElaborazioneQuestionari.ViewCodaElaborazioneQuestionari


    Dim m As New Message
    Dim strID As String

    Dim strUSER As String = CType(objHashTable("USER"), String)
    Const QUEUE_NAME As String = ".\private$\ElaborazioneQuestionari"
    Dim queue As MessageQueue

    Dim strInizioElaborazione As String = "Inizio Elaborazione".ToUpper
    Dim strFineElaborazione As String = "Fine Elaborazione".ToUpper
    Dim strErroreElaborazione As String = "Errore durante l'elaborazione dei questionari".ToUpper

    ViewCodaElaborazioneQuestionari = ""


        Try
            If Not MessageQueue.Exists(QUEUE_NAME) Then
                queue = queue.Create(QUEUE_NAME, False)
                queue.Label = "Coda Elaborazione Questionari"
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

            If StrComp(m.Label, strUSER & objHashTable("CodENTE")) = 0 Then
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



    Public Function GetErroriTestataQuestionari(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long, ByVal ID_DICHIARAZIONE As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneQuestionari.GetErroriTestataQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetErroriTestataQuestionari = objCOMPlusBusinessObject.GetErroriTestataQuestionari(objHashTable, ID_PROVVEDIMENTO, ID_DICHIARAZIONE)

            Return GetErroriTestataQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetErroriImmobiliQuestionari(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long, ByVal ID_IMMOBILE As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneQuestionari.GetErroriImmobiliQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetErroriImmobiliQuestionari = objCOMPlusBusinessObject.GetErroriImmobiliQuestionari(objHashTable, ID_PROVVEDIMENTO, ID_IMMOBILE)

            Return GetErroriImmobiliQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetErroriDettagliotestataQuestionari(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long, ByVal ID_DICHIARAZIONE As Long, ByVal COD_CONTRIBUENTE As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneQuestionari.GetErroriDettagliotestataQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetErroriDettagliotestataQuestionari = objCOMPlusBusinessObject.GetErroriDettagliotestataQuestionari(objHashTable, ID_PROVVEDIMENTO, ID_DICHIARAZIONE, COD_CONTRIBUENTE)

            Return GetErroriDettagliotestataQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


End Class
