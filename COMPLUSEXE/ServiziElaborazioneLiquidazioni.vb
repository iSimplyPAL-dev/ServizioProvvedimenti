Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface

Imports COMPlusService
Imports System.Messaging
Imports System.Threading
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports log4net
'*******************************************************
'
' GetDatiLettere() Function <a name="GetDatiLettere"></a>
'
' La funzione GetDatiLettere
' sarà da utilizzare per la ricerca dei contribuenti con Dichiarazioni da Bonificare 
' Restituisce un dataset con i dati dei contribuenti  che saranno esposti in
'una griglia
'*******************************************************

Public Class ServiziElaborazioneLiquidazioni

  Inherits MarshalByRefObject
    Implements IElaborazioneLiquidazioni

	Delegate Function DelProcessLiquidazioni(ByVal objDSAnagrafico As DataSet) As Boolean
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneLiquidazioni))


  '**********************************************************************************************************
  ' GetDatiLiquidazioni() Function <a name="GetDatiLiquidazioni"></a>
  ' Il metodo GetDatiLiquidazioni ritorna un DataSet
  ' I parametri passati sono un oggetto di tipo Hashtable e la session aperta lato client
  ' Ritorna un oggetto dataset con i dati dei contribuenti trovati
  '**********************************************************************************************************


 
    Public Function GetDatiLiquidazioni(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements IElaborazioneLiquidazioni.GetDatiLiquidazioni

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            '******************************************************************************************************
            'Modalita di ricerca Manuale
            '******************************************************************************************************
            If CType(objHashTable("Manuale"), Boolean) Then
                GetDatiLiquidazioni = objCOMPlusBusinessObject.getLiquidazioniRicercaManualeDichiarazioni()
            End If
            '******************************************************************************************************
            'Modalita di ricerca Massiva
            '******************************************************************************************************
            If CType(objHashTable("Massiva"), Boolean) Then

                If CType(objHashTable("CHECKDAA"), Boolean) Then
                    GetDatiLiquidazioni = objCOMPlusBusinessObject.getLiquidazioniRicercaMassivaDichiarazioniDAA() 'getQuestionariRicercaMassivaDichiarazioniDaBonificareDAA()
                End If
                If CType(objHashTable("CHEKAREE"), Boolean) Then
                    GetDatiLiquidazioni = objCOMPlusBusinessObject.getLiquidazioniRicercaMassivaDichiarazioniAREE() 'getQuestionariRicercaMassivaDichiarazioniDaBonificareAREE()
                End If
                If CType(objHashTable("CHEKVIARESIDENZA"), Boolean) Then
                    GetDatiLiquidazioni = objCOMPlusBusinessObject.getLiquidazioniRicercaMassivaDichiarazioniVIA()  'getQuestionariRicercaMassivaDichiarazioniDaBonificareVIA()
                End If
                If CType(objHashTable("CHEKUBICAZIONE"), Boolean) Then
                    GetDatiLiquidazioni = objCOMPlusBusinessObject.getLiquidazioniRicercaMassivaDichiarazioniUBICAZIONE() 'getQuestionariRicercaMassivaDichiarazioniDaBonificareUBICAZIONE()
                End If

            End If

            Return GetDatiLiquidazioni
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


    '**********************************************************************************************************
    ' GetDatiLiquidazioniRettifiche() Function <a name="GetDatiLiquidazioni"></a>
    ' Il metodo GetDatiLiquidazioniRettifiche ritorna un DataSet
    ' Viene utilizzato quando viene effettuata una rettica su Liquidazione (Pre Accertamento)
    ' I parametri passati sono un oggetto di tipo Hashtable e la session aperta lato client
    ' Ritorna un oggetto dataset con i dati dei contribuenti trovati (1 contribuente)
    '**********************************************************************************************************
    Public Function GetDatiLiquidazioniRettifiche(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements IElaborazioneLiquidazioni.GetDatiLiquidazioniRettifiche

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            '******************************************************************************************************
            'Modalita di ricerca Manuale
            '******************************************************************************************************
            If CType(objHashTable("Manuale"), Boolean) Then
                GetDatiLiquidazioniRettifiche = objCOMPlusBusinessObject.getLiquidazioniRicercaManualeDichiarazioniRettifiche()
            End If


            Return GetDatiLiquidazioniRettifiche
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

	Public Function ProcessLiquidazione(ByVal objHashTable As System.Collections.Hashtable, ByVal objDSAnagrafico As System.Data.DataSet, ByVal TipoCalcolo As Integer) As Boolean Implements IElaborazioneLiquidazioni.ProcessLiquidazione

		Try
			Dim strMessage As String = ViewCodaElaborazioneLiquidazione(objHashTable, objDSAnagrafico)
			Dim strInizioElaborazione As String = "Elaborazione in Corso".ToUpper

			If StrComp(strMessage, strInizioElaborazione) = 0 Then
				Return False
				Exit Function
			End If

			Log.Debug("Inizio COMPLUSEXE::ServiziElaborazioneLiquidazioni::ProcessLiquidazione")

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			Dim RemoteDel As New DelProcessLiquidazioni(AddressOf objCOMPlusBusinessObject.ProcessLiquidazioni)
			Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(objDSAnagrafico, Nothing, Nothing)

			Log.Debug("Eseguo in modo asincrono il delegato di ProcessLiquidazioni.")

			Return True

		Catch ex As Exception
			Log.Debug("ProcessLiquidazione - Si è verificato un errore " & ex.Message)
			Log.Warn("ProcessLiquidazione - Si è verificato un errore " & ex.Message)

			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function ViewCodaElaborazioneLiquidazione(ByVal objHashTable As System.Collections.Hashtable, ByVal objDSAnagrafico As System.Data.DataSet) As String Implements IElaborazioneLiquidazioni.ViewCodaElaborazioneLiquidazione

		Dim m As New Message
		Dim strID As String

		Dim strUSER As String = CType(objHashTable("USER"), String)
		Const QUEUE_NAME As String = ".\private$\ElaborazioneLiquidazioni"
		Dim queue As MessageQueue

		Dim strInizioElaborazione As String = "Inizio Elaborazione".ToUpper
		Dim strFineElaborazione As String = "Fine Elaborazione".ToUpper
		Dim strErroreElaborazione As String = "Errore durante l'elaborazione delle Liquidazioni".ToUpper
		Dim PROGRESSIVO_ELABORAZIONE As Long

		ViewCodaElaborazioneLiquidazione = ""

		Try
			If Not MessageQueue.Exists(QUEUE_NAME) Then
				queue = queue.Create(QUEUE_NAME, False)
				queue.Label = "Coda Elaborazione Liquidazioni"
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
		Dim strMSG, strDESCRIPTION, strVALORE As String
		Dim arrayMsgCode() As String

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

		'For Each m In messaggi

		'  m.Formatter = New BinaryMessageFormatter

		'  arrayMsgCode = Split(CStr(m.Body), "|")
		'  If arrayMsgCode.Length > 1 Then
		'    strMSG = arrayMsgCode(0)
		'    strDESCRIPTION = arrayMsgCode(1)
		'    strVALORE = arrayMsgCode(2)
		'  End If

		'  If StrComp(m.Label, strUSER) = 0 Then
		'    If StrComp(UCase(CStr(strMSG)), strInizioElaborazione) = 0 Then
		'      Return "Elaborazione in Corso".ToUpper & "| | |"
		'    End If

		'    If StrComp(UCase(CStr(strMSG)), strFineElaborazione) = 0 Then

		'      strID = m.Id()
		'      'Prelevo il messaggio nella coda e lo elimino
		'      m = queue.ReceiveById(strID)

		'      If strDESCRIPTION = "PROGRESSIVO_ELABORAZIONE" Then
		'        Return "Elaborazione Terminata con successo".ToUpper & "|" & strDESCRIPTION & "|" & strVALORE & "|"
		'      End If

		'      'Return "Elaborazione Terminata con successo".ToUpper

		'    End If

		'    If StrComp(UCase(CStr(strMSG)), strErroreElaborazione) = 0 Then
		'      strID = m.Id()
		'      'Prelevo il messaggio nella coda e lo elimino
		'      m = queue.ReceiveById(strID)
		'      Return "Elaborazione Terminata con errori".ToUpper & "| | |"
		'    End If
		'  End If

		'  'm.Formatter = New BinaryMessageFormatter

		'  'If StrComp(m.Label, strUSER) = 0 Then
		'  '  If StrComp(UCase(CStr(m.Body)), strInizioElaborazione) = 0 Then
		'  '    Return "Elaborazione in Corso".ToUpper
		'  '  End If

		'  '  If StrComp(UCase(CStr(m.Body)), strFineElaborazione) = 0 Then

		'  '    strID = m.Id()
		'  '    'Prelevo il messaggio nella coda e lo elimino
		'  '    m = queue.ReceiveById(strID)

		'  '    Dim dum As Integer
		'  '    dum = InStr(m.Body, "&", CompareMethod.Text) 'Split(m.Body, "&")
		'  '    PROGRESSIVO_ELABORAZIONE = CLng(Right(m.Body, Len(m.Body) - dum))

		'  '    Return "Elaborazione Terminata con successo".ToUpper & "&" & PROGRESSIVO_ELABORAZIONE
		'  '    'Return "Elaborazione Terminata con successo".ToUpper


		'  '  End If
		'  '  If StrComp(UCase(CStr(m.Body)), strErroreElaborazione) = 0 Then

		'  '    strID = m.Id()
		'  '    'Prelevo il messaggio nella coda e lo elimino
		'  '    m = queue.ReceiveById(strID)
		'  '    Return "Elaborazione Terminata con errori".ToUpper
		'  '  End If
		'  'End If
		'Next

		''Return "Non ci sono elaborazioni in corso".ToUpper & "| | |"

	End Function

	Public Function GetProvvedimentoPerStampaLiquidazione(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements IElaborazioneLiquidazioni.GetProvvedimentoPerStampaLiquidazione

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetProvvedimentoPerStampaLiquidazione = objCOMPlusBusinessObject.getProvvedimentoPerStampaLiquidazione(ID_PROVVEDIMENTO)

			Return GetProvvedimentoPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function getVersamentiPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long, ByVal ID_FASE As Integer) As DataSet Implements IElaborazioneLiquidazioni.getVersamentiPerStampaLiquidazione

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getVersamentiPerStampaLiquidazione = objCOMPlusBusinessObject.getVersamentiPerStampaLiquidazione(objHashTable, ID_PROCEDIMENTO, ID_FASE)

			Return getVersamentiPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function getImmobiliDichiaratiPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet Implements IElaborazioneLiquidazioni.getImmobiliDichiaratiPerStampaLiquidazione

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliDichiaratiPerStampaLiquidazione = objCOMPlusBusinessObject.getImmobiliDichiaratiPerStampaLiquidazione(objHashTable, ID_PROCEDIMENTO)

			Return getImmobiliDichiaratiPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function getImmobiliCatastoPerStampaLiquidazione(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROCEDIMENTO As Long) As System.Data.DataSet Implements IElaborazioneLiquidazioni.getImmobiliCatastoPerStampaLiquidazione

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliCatastoPerStampaLiquidazione = objCOMPlusBusinessObject.getImmobiliCatastoPerStampaLiquidazione(objHashTable, ID_PROCEDIMENTO)

			Return getImmobiliCatastoPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetInteressiPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetInteressiPerStampaLiquidazione
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetInteressiPerStampaLiquidazione = objCOMPlusBusinessObject.GetInteressiPerStampaLiquidazione(objHashTable, ID_PROVVEDIMENTO)

			Return GetInteressiPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetElencoInteressiPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetElencoInteressiPerStampaLiquidazione
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetElencoInteressiPerStampaLiquidazione = objCOMPlusBusinessObject.GetElencoInteressiPerStampaLiquidazione(objHashTable, ID_PROVVEDIMENTO)

			Return GetElencoInteressiPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetInteressiTotaliPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetInteressiTotaliPerStampaLiquidazione
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetInteressiTotaliPerStampaLiquidazione = objCOMPlusBusinessObject.GetInteressiTotaliPerStampaLiquidazione(objHashTable, ID_PROVVEDIMENTO)

			Return GetInteressiTotaliPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetElencoSanzioniPerStampaLiquidazione(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetElencoSanzioniPerStampaLiquidazione

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetElencoSanzioniPerStampaLiquidazione = objCOMPlusBusinessObject.GetElencoSanzioniPerStampaLiquidazione(objHashTable, ID_PROVVEDIMENTO)

			Return GetElencoSanzioniPerStampaLiquidazione

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function getDatiProvvedimentiDaPreAccertamento(ByVal objHashTable As Hashtable) As DataSet Implements IElaborazioneLiquidazioni.getDatiProvvedimentiDaPreAccertamento

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getDatiProvvedimentiDaPreAccertamento = objCOMPlusBusinessObject.getDatiProvvedimentiDaPreAccertamento(objHashTable)

			Return getDatiProvvedimentiDaPreAccertamento

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function getDettaglioFasiPreAccertamento(ByVal objHashTable As Hashtable) As DataSet Implements IElaborazioneLiquidazioni.getDettaglioFasiPreAccertamento

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getDettaglioFasiPreAccertamento = objCOMPlusBusinessObject.getDettaglioFasiPreAccertamento()

			Return getDettaglioFasiPreAccertamento

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function
	Public Function getCronologiaInfo(ByVal objHashTable As Hashtable) As DataSet Implements IElaborazioneLiquidazioni.getCronologiaInfo

		Try
			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getCronologiaInfo = objCOMPlusBusinessObject.getCronologiaInfo()

			Return getCronologiaInfo

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function setTAB_STORICO_DOCUMENTI(ByVal objHashTable As Hashtable, ByVal dsDoc As DataSet) As Boolean Implements IElaborazioneLiquidazioni.setTAB_STORICO_DOCUMENTI

		Try
			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			setTAB_STORICO_DOCUMENTI = objCOMPlusBusinessObject.setTAB_STORICO_DOCUMENTI(objHashTable, dsDoc)

			Return setTAB_STORICO_DOCUMENTI

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function setPreAccertamentoDefinitivo(ByVal objHashTable As Hashtable, ByVal arrayIdProvvedimenti As DataSet) As Boolean Implements IElaborazioneLiquidazioni.setPreAccertamentoDefinitivo

		Try
			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			setPreAccertamentoDefinitivo = objCOMPlusBusinessObject.setPreAccertamentoDefinitivo(arrayIdProvvedimenti)

			Return setPreAccertamentoDefinitivo

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	'Public Function ProcessFase2PerAccertamento(ByVal objHashTable As System.Collections.Hashtable, ByRef dsCalcoliFase2 As System.Data.DataSet, ByRef dsSanzioniFase2 As System.Data.DataSet, ByRef dsInteressiFase2 As System.Data.DataSet, ByRef dsRiepilogo As System.Data.DataSet) As Boolean Implements ComPlusInterface.IElaborazioneLiquidazioni.ProcessFase2PerAccertamento
	Public Function ProcessFase2PerAccertamento(ByVal objHashTable As System.Collections.Hashtable, ByRef dsCalcoliFase2 As System.Data.DataSet, ByRef dsSanzioniFase2 As System.Data.DataSet, ByRef dsInteressiFase2 As System.Data.DataSet, ByRef dsRiepilogo As System.Data.DataSet, ByRef objDSDichiaratoIci As System.Data.DataSet, ByRef objDSImmobiliIci As System.Data.DataSet, ByRef objDSContitolariIci As System.Data.DataSet, ByRef objDSCversamentiF2 As System.Data.DataSet, ByVal objAnagrafica As System.Data.DataSet, ByVal TipoCalcolo As Integer) As Boolean Implements ComPlusInterface.IElaborazioneLiquidazioni.ProcessFase2PerAccertamento
		Try
			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			ProcessFase2PerAccertamento = objCOMPlusBusinessObject.ProcessFase2PerAccertamento(objHashTable, dsCalcoliFase2, dsSanzioniFase2, dsInteressiFase2, dsRiepilogo, objDSDichiaratoIci, objDSImmobiliIci, objDSContitolariIci, objDSCversamentiF2, objAnagrafica, TipoCalcolo)

			Return ProcessFase2PerAccertamento
		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Sub New()

	End Sub

End Class
