Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports COMPlusService
Imports System.Messaging
Imports System.Threading



Public Class ServiziFreezer

  Inherits MarshalByRefObject
  Implements IFreezer

    Delegate Function DelCreateFreezer(ByVal objDSAnagrafico As DataSet, ByVal blnConfigurazioneDich As Boolean) As Boolean
    Delegate Function DelCreateFreezerTerritorio(ByVal objDSAnagrafico As DataSet) As Boolean
	Delegate Function DelCalcoloMassivoICI(ByVal blnConfigurazioneDich As Boolean, ByVal blnRibaltaVersatoSuDovuto As Boolean, ByVal blnCalcolaArrotondamento As Boolean, ByVal TipoCalcolo As Integer) As Boolean

  ' GetDatiLiquidazioni() Function <a name="GetDatiLiquidazioni"></a>
  ' Il metodo GetDatiLiquidazioni ritorna un DataSet
  ' I parametri passati sono un oggetto di tipo Hashtable e la session aperta lato client
  ' Ritorna un oggetto dataset con i dati dei contribuenti trovati

	Public Function SetCalcoloICIAsync(ByVal objHashTable As System.Collections.Hashtable, ByVal blnConfigurazioneDich As Boolean, ByVal blnRibaltaVersatoSuDovuto As Boolean, ByVal blnCalcolaArrotondamento As Boolean, ByVal TipoCalcolo As Integer) As Boolean Implements ComPlusInterface.IFreezer.SetCalcoloICIAsync
		Try
			Dim strMessage As String = ViewCodaCalcoloICIMassivo(objHashTable)
			Dim strInizioElaborazione As String = "Elaborazione in Corso".ToUpper

			If StrComp(strMessage, strInizioElaborazione) = 0 Then
				Return False
				Exit Function
			End If

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			Dim RemoteDel As New DelCalcoloMassivoICI(AddressOf objCOMPlusBusinessObject.CalcoloICIcompletoAsync)
			Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(blnConfigurazioneDich, blnRibaltaVersatoSuDovuto, blnCalcolaArrotondamento, Nothing, Nothing, TipoCalcolo)

			Return True
		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function SetCalcoloICISync(ByVal objHashTable As System.Collections.Hashtable, ByVal blnConfigurazioneDich As Boolean, ByVal blnRibaltaVersatoSuDovuto As Boolean, ByVal blnCalcolaArrotondamento As Boolean, ByVal TipoCalcolo As Integer) As Boolean Implements ComPlusInterface.IFreezer.SetCalcoloICISync
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			Return objCOMPlusBusinessObject.CalcoloICIcompleto(blnConfigurazioneDich, blnRibaltaVersatoSuDovuto, blnCalcolaArrotondamento, TipoCalcolo)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function ViewCodaCalcoloICIMassivo(ByVal objHashTable As System.Collections.Hashtable) As String Implements ComPlusInterface.IFreezer.ViewCodaCalcoloICIMassivo

		Dim m As New Message
		Dim strID As String

		Dim strUSER As String = CType(objHashTable("USER"), String).ToUpper
		Const QUEUE_NAME As String = ".\private$\ElaborazioneCalcoloICIMassivo"
		Dim queue As MessageQueue

		Dim strInizioElaborazione As String = "Inizio Elaborazione".ToUpper
		Dim strFineElaborazione As String = "Fine Elaborazione".ToUpper
		Dim strErroreElaborazione As String = "Errore durante l'elaborazione del Calcolo ICI Massivo".ToUpper
		Dim PROGRESSIVO_ELABORAZIONE As Long

		ViewCodaCalcoloICIMassivo = ""

		Try
			If Not MessageQueue.Exists(QUEUE_NAME) Then
				queue = queue.Create(QUEUE_NAME, False)
				queue.Label = "Coda Elaborazione CalcoloICIMassivo"
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


	End Function



	Public Function SetFreezerDichiarazioni(ByVal objHashTable As System.Collections.Hashtable, ByVal objDSAnagrafico As System.Data.DataSet, ByVal blnConfigurazioneDich As Boolean) As Boolean Implements ComPlusInterface.IFreezer.SetFreezerDichiarazioni
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			Dim RemoteDel As New DelCreateFreezer(AddressOf objCOMPlusBusinessObject.CreateFreezer)
			Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(objDSAnagrafico, blnConfigurazioneDich, Nothing, Nothing)

			Return True


		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function SetFreezerDichiarazioniPuntuale(ByVal objHashTable As System.Collections.Hashtable, ByVal objDSAnagrafico As System.Data.DataSet, ByVal blnConfigurazioneDich As Boolean) As Boolean Implements ComPlusInterface.IFreezer.SetFreezerDichiarazioniPuntuale
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			SetFreezerDichiarazioniPuntuale = objCOMPlusBusinessObject.CreateFreezer(objDSAnagrafico, blnConfigurazioneDich)

			Return True


		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function GetAnagraficheFreezer(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IFreezer.GetAnagraficheFreezer


		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)
			'******************************************************************************************************
			'Modalita di rice
			'******************************************************************************************************
			GetAnagraficheFreezer = objCOMPlusBusinessObject.GetAnagraficheFreezer()

			Return GetAnagraficheFreezer

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function



	Public Function GetAnagraficheFreezerTerritorio(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IFreezer.GetAnagraficheFreezerTerritorio

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)
			'******************************************************************************************************
			'Modalita di rice
			'******************************************************************************************************
			GetAnagraficheFreezerTerritorio = objCOMPlusBusinessObject.GetAnagraficheFreezerTerritorio

			Return GetAnagraficheFreezerTerritorio

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function



	Public Function SetFreezerTerritorio(ByVal objHashTable As System.Collections.Hashtable, ByVal objDSAnagrafico As System.Data.DataSet) As Boolean Implements ComPlusInterface.IFreezer.SetFreezerTerritorio

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			Dim RemoteDel As New DelCreateFreezerTerritorio(AddressOf objCOMPlusBusinessObject.CreateFreezerTerritorio)
			Dim RemAr As IAsyncResult = RemoteDel.BeginInvoke(objDSAnagrafico, Nothing, Nothing)

			Return True


		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function


	Public Function CalcoloICI(ByVal objDSCalcoloICI As System.Data.DataSet, ByVal objHashTable As System.Collections.Hashtable, ByVal TipoCalcolo As Integer) As DataSet Implements ComPlusInterface.IFreezer.CalcoloICI
		Try

			Dim objCOMPLUSBaseCalcoloICI As New COMPLUSBaseCalcoloICI(objDSCalcoloICI, objHashTable)
			Return objCOMPLUSBaseCalcoloICI.getCALCOLO_ICI(0)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function GetSituazioneVirtualeDichiarazioni(ByVal dsAnagrafica As DataSet, ByVal objHashTable As Hashtable, ByVal CodContrib As String) As DataSet Implements ComPlusInterface.IFreezer.GetSituazioneVirtualeDichiarazioni
		Try

			Dim objDBICI As New DBIci
			Return objDBICI.GetSituazioneVirtualeDichiarazioni(dsAnagrafica, objHashTable, CodContrib)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function GetSituazioneVirtualeImmobili(ByVal objDSDichiaratoIci As DataSet, ByVal objHashTable As Hashtable, ByVal CodContrib As String) As DataSet Implements ComPlusInterface.IFreezer.GetSituazioneVirtualeImmobili
		Try

			Dim objDBICI As New DBIci
			Return objDBICI.GetSituazioneVirtualeImmobili(objDSDichiaratoIci, objHashTable, CodContrib)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function

	Public Function GetSituazioneVirtualeContitolari(ByVal objDSDichiaratoIci As DataSet, ByVal objHashTable As Hashtable, ByVal CodContrib As String) As DataSet Implements ComPlusInterface.IFreezer.GetSituazioneVirtualeContitolari
		Try

			Dim objDBICI As New DBIci
			Return objDBICI.GetSituazioneVirtualeContitolari(objDSDichiaratoIci, objHashTable, CodContrib)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try
	End Function


	Public Function getDATI_TASK_REPOSITORY_CALCOLO_ICI(ByVal objHashTable As Hashtable) As DataSet Implements IFreezer.getDATI_TASK_REPOSITORY_CALCOLO_ICI

		Try

			Dim objDBICI As New DBIci
			Return objDBICI.getDATI_TASK_REPOSITORY_CALCOLO_ICI(objHashTable)

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

End Class
