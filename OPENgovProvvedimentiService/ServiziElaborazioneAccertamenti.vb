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
''' <summary>
''' Classe rende disponibili le interfacce di elaborazione accertamenti
''' </summary>
Public Class ServiziElaborazioneAccertamenti
    Inherits MarshalByRefObject
    Implements IElaborazioneAccertamenti

    Private Shared Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneAccertamenti))
    'Funzione che recupera tutte le dichiarazioni del contribuente
    Public Function getDatiAccertamenti(StringConnectionICI As String, StringConnectionGOV As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As System.Collections.Hashtable, ByRef ListSituazioneFinale() As objSituazioneFinale) As objUIICIAccert() Implements ComPlusInterface.IElaborazioneAccertamenti.GetDatiAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getDatiDichiarazioniAccertamenti(StringConnectionICI, StringConnectionGOV, IdEnte, IdContribuente, ListSituazioneFinale)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    '**** 201809 - Cartelle Insoluti ***
    Public Function getInteressi(IdEnte As String, ByVal IdTributo As String, ByVal CodVoce As String, ByVal TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, ScadenzaIntAcconto As String, ScadenzaIntSaldo As String, IdLegame As Integer, ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni() Implements ComPlusInterface.IElaborazioneAccertamenti.getInteressi
        Try
            getInteressi = New COMPlusBusinessObject().getInteressi(IdEnte, IdTributo, CodVoce, TipoProvvedimento, TipoProcedimento, Fase, DataElaborazione, ScadenzaIntAcconto, ScadenzaIntSaldo, IdLegame, ListToCalc, myStringConnection)
            Return getInteressi
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    '**** 201809 - Cartelle Insoluti ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myDBType"></param>
    ''' <param name="myHashTable"></param>
    ''' <param name="oCalcoloSanzInt"></param>
    ''' <param name="dsCalcoloSanzioni"></param>
    ''' <param name="ListInteressi"></param>
    ''' <param name="spese"></param>
    ''' <param name="ListSituazioneFinale"></param>
    ''' <param name="ListDichiarato"></param>
    ''' <param name="ListAccertato"></param>
    ''' <param name="dsSanzioniFase2"></param>
    ''' <param name="ListInteressiFase2"></param>
    ''' <param name="dsVersamentiF2"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    Public Function updateDBAccertamenti(myDBType As String, StringConnectionProvv As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As Hashtable, ByVal oCalcoloSanzInt As ObjBaseIntSanz, ByVal dsCalcoloSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal spese As Double, ByVal ListSituazioneFinale() As objSituazioneFinale, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal dsVersamentiF2 As System.Data.DataSet, Operatore As String) As Long Implements ComPlusInterface.IElaborazioneAccertamenti.updateDBAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(myHashTable)
            updateDBAccertamenti = objCOMPlusBusinessObject.updateDBAccertamenti(myDBType, StringConnectionProvv, IdEnte, IdContribuente, myHashTable, oCalcoloSanzInt, dsCalcoloSanzioni, ListInteressi, spese, ListDichiarato, ListAccertato, dsSanzioniFase2, ListInteressiFase2, dsVersamentiF2, Operatore)
            Return updateDBAccertamenti
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="objSituazioneBasePerSanzInt"></param>
    ''' <param name="objSanzioni"></param>
    ''' <param name="ObjInteressiSanzioni"></param>
    ''' <param name="oAtto"></param>
    ''' <param name="oDettaglioAtto"></param>
    ''' <param name="objDichiaratoTARSU"></param>
    ''' <param name="objAccertatoTARSU"></param>
    ''' <param name="spese"></param>
    ''' <param name="oAddizionali"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Public Function SetAtto(myDBType As String, ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt() As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ObjInteressiSanzioni() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento, Operatore As String) As Integer Implements ComPlusInterface.IElaborazioneAccertamenti.SetAtto
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.SetAtto(myDBType, objHashTable, objSituazioneBasePerSanzInt, objSanzioni, ObjInteressiSanzioni, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, spese, oAddizionali, Operatore)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetControlliAccertamento(StringConnectionProvv As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByRef objHashTable As System.Collections.Hashtable) As Integer Implements ComPlusInterface.IElaborazioneAccertamenti.GetControlliAccertamento
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetControlliAccertamento = objCOMPlusBusinessObject.GetControlliAccertamento(StringConnectionProvv, sAnno, strCodEnte, strCodContrib, strCodTributo, objHashTable)
            Return GetControlliAccertamento
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROCEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliAccertatiPerStampaAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getImmobiliAccertatiPerStampaAccertamenti = objCOMPlusBusinessObject.getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv, ID_PROCEDIMENTO)

            Return getImmobiliAccertatiPerStampaAccertamenti
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROCEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichiaratiPerStampaAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getImmobiliDichiaratiPerStampaAccertamenti = objCOMPlusBusinessObject.getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv, objHashTable, ID_PROCEDIMENTO)

            Return getImmobiliDichiaratiPerStampaAccertamenti
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    '*** 20140701 - IMU/TARES ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="objHashTableDati"></param>
    ''' <param name="oCalcoloSanzioni"></param>
    ''' <param name="objDSCalcoloSanzioniInteressiAppoggio"></param>
    ''' <param name="bConsentiSanzNeg"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="10/09/2019">passo direttamente la data di morte che è l'unico campo fisso che serve dall'anagrafica</revision></revisionHistory>
    Public Function getSanzioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable, ByVal objHashTableDati As System.Collections.Hashtable, ByRef oCalcoloSanzioni As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean, sDataMorte As String) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getSanzioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getSanzioni = objCOMPlusBusinessObject.getSanzioniICI(StringConnectionProvv, IdEnte, objHashTableDati, oCalcoloSanzioni, objDSCalcoloSanzioniInteressiAppoggio, bConsentiSanzNeg, sDataMorte)
            Return getSanzioni
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="oSituazioneBasePerSanzInt"></param>
    ''' <param name="objSanzioni"></param>
    ''' <param name="ListInteressi"></param>
    ''' <param name="oAtto"></param>
    ''' <param name="oDettaglioAtto"></param>
    ''' <param name="objDichiaratoTARSU"></param>
    ''' <param name="objAccertatoTARSU"></param>
    ''' <param name="spese"></param>
    ''' <param name="oAddizionali"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
    Public Function updateDBAccertamentiTARSU(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento, Operatore As String) As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento() Implements ComPlusInterface.IElaborazioneAccertamenti.updateDBAccertamentiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            updateDBAccertamentiTARSU = objCOMPlusBusinessObject.updateDBAccertamentiTARSU(myDBType, IdEnte, IdContribuente, objHashTable, oSituazioneBasePerSanzInt, objSanzioni, ListInteressi, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, spese, oAddizionali, Operatore)

            Return updateDBAccertamentiTARSU
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    '*** ***
    '*** 20130801 - accertamento OSAP ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="oSituazioneBasePerSanzInt"></param>
    ''' <param name="dsSanzioni"></param>
    ''' <param name="dsSanzioniImpDicVSImpPag"></param>
    ''' <param name="dsSanzioniScadDicVSDataPag"></param>
    ''' <param name="dsInteressi"></param>
    ''' <param name="dsInteressiImpDicVSImpPag"></param>
    ''' <param name="dsInteressiScadDicVSDataPag"></param>
    ''' <param name="oAtto"></param>
    ''' <param name="oDettaglioAtto"></param>
    ''' <param name="objDichiarato"></param>
    ''' <param name="objAccertato"></param>
    ''' <param name="spese"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Public Function updateDBAccertamentiOSAP(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As OggettoAttoOSAP, ByVal oDettaglioAtto() As OggettoDettaglioAtto, ByVal objDichiarato() As OSAPAccertamentoArticolo, ByVal objAccertato() As OSAPAccertamentoArticolo, ByVal spese As Double, Operatore As String) As OSAPAccertamentoArticolo() Implements ComPlusInterface.IElaborazioneAccertamenti.updateDBAccertamentiOSAP
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            updateDBAccertamentiOSAP = objCOMPlusBusinessObject.updateDBAccertamentiOSAP(myDBType, IdEnte, IdContribuente, objHashTable, oSituazioneBasePerSanzInt, dsSanzioni, dsSanzioniImpDicVSImpPag, dsSanzioniScadDicVSDataPag, dsInteressi, dsInteressiImpDicVSImpPag, dsInteressiScadDicVSDataPag, oAtto, oDettaglioAtto, objDichiarato, objAccertato, spese, Operatore)

            Return updateDBAccertamentiOSAP
        Catch ex As Exception
            Throw New Exception("updateDBAccertamentiOSAP::" & " " & ex.Message)
        End Try
    End Function

    Public Function getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getVersamentiPerStampaAccertamentiOSAP
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            getVersamentiPerStampaAccertamentiOSAP = objCOMPlusBusinessObject.getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv, ID_PROVVEDIMENTO)
            Return getVersamentiPerStampaAccertamentiOSAP
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal TipoRicerca As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichAccPerStampaAccertamentiOSAP
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            getImmobiliDichAccPerStampaAccertamentiOSAP = objCOMPlusBusinessObject.getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv, TipoRicerca, ID_PROVVEDIMENTO)
            Return getImmobiliDichAccPerStampaAccertamentiOSAP
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    '*** ***

    Public Function GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetElencoSanzioniPerStampaAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv, IdEnte, objHashTable, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetElencoInteressiPerStampaAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneAccertamenti.GetInteressiTotaliPerStampaAccertamenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliAccertatiPerStampaAccertamentiTARSU
        Try
            Log.Debug("OPENgovProvvedimentiService Dentro ServiziElaborazioneAccertamenti getImmobiliAccertatiPerStampaAccertamentiTARSU")
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Log.Debug("OPENgovProvvedimentiService ServiziElaborazioneAccertamenti.vb getImmobiliAccertatiPerStampaAccertamentiTARSU::" & ex.StackTrace)
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getAddizionaliPerStampaAccertamentiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getVersamentiPerStampaAccertamentiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichiaratiPerStampaAccertamentiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv, IdEnte, objHashTable, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAccertamenti.getImmobiliDichAccPerStampaAccertamentiTARSU
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
End Class
