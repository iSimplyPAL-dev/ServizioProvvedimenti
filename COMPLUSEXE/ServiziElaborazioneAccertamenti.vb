Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports System.Messaging
Imports System.Threading
Imports log4net
Imports RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti

Public Class ServiziElaborazioneAccertamenti
    Inherits MarshalByRefObject
    Implements IElaborazioneAccertamenti

    Private Shared Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneAccertamenti))
    'Funzione che recupera tutte le dichiarazioni del contribuente
    Public Function getDatiAccertamenti(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.GetDatiAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getDatiAccertamenti = objCOMPlusBusinessObject.getDatiDichiarazioniAccertamenti()
            Return GetDatiAccertamenti
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    Public Function getImmobiliFromCatasto(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliFromCatasto

    End Function

    Public Function getImmobiliFromTerritorio(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliFromTerritorio
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getImmobiliFromTerritorio = objCOMPlusBusinessObject.getDatiImmobiliFromTerritorio()
            Return getImmobiliFromTerritorio
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    Public Function getInteressi(ByVal objHashTable As System.Collections.Hashtable, ByVal objHashTableDati As Hashtable, ByVal objDSCalcoloInteressi As System.Data.DataSet, ByVal Progressivo As Integer, ByVal idLegame As Integer) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getInteressi = objCOMPlusBusinessObject.getInteressiICI(objDSCalcoloInteressi, Progressivo, idLegame)
            Dim dt As DataTable
            dt = objDSCalcoloInteressi.Tables(0)
            objDSCalcoloInteressi.Dispose()
            getInteressi.Tables.Add(dt.Copy)
            Return getInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing

        End Try
    End Function

    Public Function getInteressiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal objHashTableDati As Hashtable, ByVal objDSCalcoloInteressi As System.Data.DataSet, ByVal Progressivo As Integer, ByVal idLegame As Integer) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getInteressiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getInteressiTARSU = objCOMPlusBusinessObject.getInteressiTARSU(objDSCalcoloInteressi, Progressivo, idLegame)
            Dim dt As DataTable
            dt = objDSCalcoloInteressi.Tables(0)
            objDSCalcoloInteressi.Dispose()
            getInteressiTARSU.Tables.Add(dt.Copy)
            Return getInteressiTARSU

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing

        End Try
    End Function


    Public Function getSanzioni(ByVal objHashTable As System.Collections.Hashtable, ByVal objHashTableDati As System.Collections.Hashtable, ByVal objDSCalcoloSanzioni As System.Data.DataSet, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getSanzioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getSanzioni = objCOMPlusBusinessObject.getSanzioniICI(objHashTableDati, objDSCalcoloSanzioni, objDSCalcoloSanzioniInteressiAppoggio)
            Dim dt As DataTable
            dt = objDSCalcoloSanzioni.Tables(0)
            objDSCalcoloSanzioni.Dispose()
            getSanzioni.Tables.Add(dt.Copy)
            Return getSanzioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing

        End Try
    End Function

    Public Function EliminaProvvedimentoAccertamento(ByVal objHashTable As Hashtable, ByVal sCodContribuente As String, ByVal sAnno As String, ByVal sCodEnte As String, ByVal sCOD_TIPO_PROCEDIMENTO As String) As Boolean Implements ComPlusInterface.IElaborazioneAccertamenti.EliminaProvvedimentoAccertamento
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            EliminaProvvedimentoAccertamento = objCOMPlusBusinessObject.EliminaProvvedimentoAccertamento(objHashTable, sCodContribuente, sAnno, sCodEnte, sCOD_TIPO_PROCEDIMENTO)
            Return EliminaProvvedimentoAccertamento
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return False
        End Try
    End Function

   

    Public Function updateDBAccertamenti(ByVal objHashTable As Hashtable, ByVal objDSCalcoloSanzioniInteressi As DataSet, ByVal objDSCalcoloSanzioni As DataSet, ByVal objDSCalcoloInteressi As DataSet, ByVal spese As Double, ByVal objICI As DataSet, ByVal dsImmobiliDichiarato As DataSet, ByVal dsImmobiliAccertato As DataSet, _
                                        Optional ByVal dsSanzioniFase2 As DataSet = Nothing, _
                                        Optional ByVal dsInteressiFase2 As DataSet = Nothing, _
                                        Optional ByVal objDSDichiaratoIciFase2 As System.Data.DataSet = Nothing, _
                                        Optional ByVal objDSImmobiliIciFase2 As System.Data.DataSet = Nothing, _
                                        Optional ByVal objDSContitolariIciFase2 As System.Data.DataSet = Nothing, _
                                        Optional ByVal objDSCversamentiF2 As System.Data.DataSet = Nothing) As Long Implements ComPlusInterface.IElaborazioneAccertamenti.updateDBAccertamenti

        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            updateDBAccertamenti = objCOMPlusBusinessObject.updateDBAccertamenti(objHashTable, objDSCalcoloSanzioniInteressi, objDSCalcoloSanzioni, objDSCalcoloInteressi, spese, objICI, dsImmobiliDichiarato, dsImmobiliAccertato, dsSanzioniFase2, dsInteressiFase2, objDSDichiaratoIciFase2, objDSImmobiliIciFase2, objDSContitolariIciFase2, objDSCversamentiF2)

            Return updateDBAccertamenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            'Return False
            Return -1
        End Try

    End Function

    Public Function GetImmobiliDichiaratoVirtualeICI(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.GetImmobiliDichiaratoVirtualeICI
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetImmobiliDichiaratoVirtualeICI = objCOMPlusBusinessObject.getDatiDichiarazioniICIVirtuale()
            Return GetImmobiliDichiaratoVirtualeICI
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
            Return Nothing
        End Try
    End Function


	Public Function GetICI(ByVal dsBaseCalcoloICI As DataSet, ByVal objHashTable As Hashtable, ByVal TipoCalcolo As Integer) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.GetICI
		Try

			Dim objCOMPlusICI As New COMPLUSBaseCalcoloICI(dsBaseCalcoloICI, objHashTable)

			GetICI = objCOMPlusICI.getCALCOLO_ICI(TipoCalcolo)
			Return GetICI
		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
			Return Nothing
		End Try
	End Function

	Public Function GetControlliAccertamento(ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByRef objHashTable As System.Collections.Hashtable) As Integer Implements ComPlusInterface.IElaborazioneAccertamenti.GetControlliAccertamento
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetControlliAccertamento = objCOMPlusBusinessObject.GetControlliAccertamento(sAnno, strCodEnte, strCodContrib, strCodTributo, objHashTable)
			Return GetControlliAccertamento
		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
			Return Nothing
		End Try

	End Function


	Public Function getImmobiliAccertatiPerStampaAccertamenti(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROCEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliAccertatiPerStampaAccertamenti

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliAccertatiPerStampaAccertamenti = objCOMPlusBusinessObject.getImmobiliAccertatiPerStampaAccertamenti(objHashTable, ID_PROCEDIMENTO)

			Return getImmobiliAccertatiPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function getImmobiliDichiaratiPerStampaAccertamenti(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROCEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichiaratiPerStampaAccertamenti

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliDichiaratiPerStampaAccertamenti = objCOMPlusBusinessObject.getImmobiliDichiaratiPerStampaAccertamenti(objHashTable, ID_PROCEDIMENTO)

			Return getImmobiliDichiaratiPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function


	Public Function GetInteressiPerStampaAccertamenti(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetInteressiPerStampaAccertamenti
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetInteressiPerStampaAccertamenti = objCOMPlusBusinessObject.GetInteressiPerStampaAccertamenti(objHashTable, ID_PROVVEDIMENTO)

			Return GetInteressiPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetElencoInteressiPerStampaAccertamenti(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetElencoInteressiPerStampaAccertamenti
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetElencoInteressiPerStampaAccertamenti = objCOMPlusBusinessObject.GetElencoInteressiPerStampaAccertamenti(objHashTable, ID_PROVVEDIMENTO)

			Return GetElencoInteressiPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetInteressiTotaliPerStampaAccertamenti(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetInteressiTotaliPerStampaAccertamenti
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetInteressiTotaliPerStampaAccertamenti = objCOMPlusBusinessObject.GetInteressiTotaliPerStampaAccertamenti(objHashTable, ID_PROVVEDIMENTO)

			Return GetInteressiTotaliPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function GetElencoSanzioniPerStampaAccertamenti(ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetElencoSanzioniPerStampaAccertamenti

		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			GetElencoSanzioniPerStampaAccertamenti = objCOMPlusBusinessObject.GetElencoSanzioniPerStampaAccertamenti(objHashTable, ID_PROVVEDIMENTO)

			Return GetElencoSanzioniPerStampaAccertamenti

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function updateDBAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal objSituazioneBasePerSanzInt As System.Data.DataSet, ByVal objSanzioni As System.Data.DataSet, ByVal objInteressi As System.Data.DataSet, ByVal oAtto As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento) As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo() Implements ComPlusInterface.IElaborazioneAccertamenti.updateDBAccertamentiTARSU
		Try
			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			updateDBAccertamentiTARSU = objCOMPlusBusinessObject.updateDBAccertamentiTARSU(objHashTable, objSituazioneBasePerSanzInt, objSanzioni, objInteressi, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, spese, oAddizionali)

			Return updateDBAccertamentiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
			Return Nothing
		End Try


	End Function

	Public Function getImmobiliAccertatiPerStampaAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliAccertatiPerStampaAccertamentiTARSU
		Try

			Log.Debug("COMPLUSEXE Dentro ServiziElaborazioneAccertamenti getImmobiliAccertatiPerStampaAccertamentiTARSU")
			Log.Warn("COMPLUSEXE Dentro ServiziElaborazioneAccertamenti getImmobiliAccertatiPerStampaAccertamentiTARSU")

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliAccertatiPerStampaAccertamentiTARSU = objCOMPlusBusinessObject.getImmobiliAccertatiPerStampaAccertamentiTARSU(objHashTable, ID_PROVVEDIMENTO)

			Return getImmobiliAccertatiPerStampaAccertamentiTARSU

		Catch ex As Exception
			Log.Debug("COMPLUSEXE ServiziElaborazioneAccertamenti.vb getImmobiliAccertatiPerStampaAccertamentiTARSU::" & ex.StackTrace)
			Log.Warn("COMPLUSEXE ServiziElaborazioneAccertamenti.vb getImmobiliAccertatiPerStampaAccertamentiTARSU::" & ex.StackTrace)

			Throw New Exception(ex.Message & "::" & ex.StackTrace)

		End Try

	End Function

	Public Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichiaratiPerStampaAccertamentiTARSU
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliDichiaratiPerStampaAccertamentiTARSU = objCOMPlusBusinessObject.getImmobiliDichiaratiPerStampaAccertamentiTARSU(objHashTable, ID_PROVVEDIMENTO)

			Return getImmobiliDichiaratiPerStampaAccertamentiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function
	Public Function getImmobiliDichAccPerStampaAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichAccPerStampaAccertamentiTARSU
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getImmobiliDichAccPerStampaAccertamentiTARSU = objCOMPlusBusinessObject.getImmobiliDichAccPerStampaAccertamentiTARSU(objHashTable, ID_PROVVEDIMENTO)

			Return getImmobiliDichAccPerStampaAccertamentiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function getVersamentiPerStampaAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getVersamentiPerStampaAccertamentiTARSU
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getVersamentiPerStampaAccertamentiTARSU = objCOMPlusBusinessObject.getVersamentiPerStampaAccertamentiTARSU(objHashTable, ID_PROVVEDIMENTO)

			Return getVersamentiPerStampaAccertamentiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function getAddizionaliPerStampaAccertamentiTARSU(ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getAddizionaliPerStampaAccertamentiTARSU
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getAddizionaliPerStampaAccertamentiTARSU = objCOMPlusBusinessObject.getAddizionaliPerStampaAccertamentiTARSU(objHashTable, ID_PROVVEDIMENTO)

			Return getAddizionaliPerStampaAccertamentiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function

	Public Function getPresenzaDecorrenzaInteressiTARSU(ByVal strANNO As String, ByVal strCODENTE As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneAccertamenti.getPresenzaDecorrenzaInteressiTARSU
		Try

			Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
			objCOMPlusBusinessObject.InizializeObject(objHashTable)

			getPresenzaDecorrenzaInteressiTARSU = objCOMPlusBusinessObject.getPresenzaDecorrenzaInteressiTARSU(strANNO, strCODENTE, objHashTable)

			Return getPresenzaDecorrenzaInteressiTARSU

		Catch ex As Exception
			Throw New Exception(ex.Message & "::" & ex.StackTrace)
		End Try

	End Function




End Class
