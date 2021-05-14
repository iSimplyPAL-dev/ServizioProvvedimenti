Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports COMPlusService
Imports System.Messaging
Imports System.Threading

Imports log4net

''' <summary>
''' Classe rende disponibili le interfacce di elaborazione liquidazioni
''' </summary>
Public Class ServiziElaborazioneLiquidazioni

    Inherits MarshalByRefObject
    Implements IElaborazioneLiquidazioni
    Delegate Function DelProcessLiquidazioni(ByVal objDSAnagrafico As DataSet) As Boolean

    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneLiquidazioni))



    Public Function GetProvvedimentoPerStampaLiquidazione(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal ID_PROVVEDIMENTO As Long) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneLiquidazioni.GetProvvedimentoPerStampaLiquidazione

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetProvvedimentoPerStampaLiquidazione = objCOMPlusBusinessObject.getProvvedimentoPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

            Return GetProvvedimentoPerStampaLiquidazione

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getVersamentiPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long, ByVal ID_FASE As Integer) As DataSet Implements ComPlusInterface.IElaborazioneLiquidazioni.GetVersamentiPerStampaLiquidazione

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getVersamentiPerStampaLiquidazione = objCOMPlusBusinessObject.getVersamentiPerStampaLiquidazione(StringConnectionProvv, ID_PROCEDIMENTO)

            Return getVersamentiPerStampaLiquidazione

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetElencoInteressiPerStampaLiquidazione
        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetElencoInteressiPerStampaLiquidazione = objCOMPlusBusinessObject.GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

            Return GetElencoInteressiPerStampaLiquidazione

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetInteressiTotaliPerStampaLiquidazione
        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetInteressiTotaliPerStampaLiquidazione = objCOMPlusBusinessObject.GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

            Return GetInteressiTotaliPerStampaLiquidazione

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet Implements IElaborazioneLiquidazioni.GetElencoSanzioniPerStampaLiquidazione

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetElencoSanzioniPerStampaLiquidazione = objCOMPlusBusinessObject.GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv, IdEnte, objHashTable, ID_PROVVEDIMENTO)

            Return GetElencoSanzioniPerStampaLiquidazione

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


    '*** 201810 - Generazione Massiva Atti ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myHashTable"></param>
    ''' <param name="dsAnagrafica"></param>
    ''' <param name="ImpDichAcconto"></param>
    ''' <param name="ImpDichSaldo"></param>
    ''' <param name="ImpDichTotale"></param>
    ''' <param name="sCodCartella"></param>
    ''' <param name="ListBaseCalcolo"></param>
    ''' <param name="dsSanzioni"></param>
    ''' <param name="ListInteressi"></param>
    ''' <param name="oRiepilogo"></param>
    ''' <param name="dsVersamenti"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="10/12/2019">in caso di calcolo per Cartelle Insoluti devo prendere il pagato per singolo avviso</revision></revisionHistory>
    Public Function ProcessFase2(StringConnectionProvv As String, StringConnectionICI As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As System.Collections.Hashtable, ByVal dsAnagrafica As System.Data.DataSet, ImpDichAcconto As Double, ImpDichSaldo As Double, ImpDichTotale As Double, sCodCartella As String, ByRef ListBaseCalcolo() As ObjBaseIntSanz, ByRef dsSanzioni As System.Data.DataSet, ByRef ListInteressi() As ObjInteressiSanzioni, ByRef oRiepilogo As ObjBaseIntSanz, ByRef dsVersamenti As System.Data.DataSet) As Boolean Implements ComPlusInterface.IElaborazioneLiquidazioni.ProcessFase2
        Try
            Dim objCOMPlusProcessLiquidazioni As New COMPLusProcessLiquidazioni
            Return objCOMPlusProcessLiquidazioni.ProcessFase2(StringConnectionProvv, StringConnectionICI, IdEnte, IdContribuente, myHashTable, dsAnagrafica, ImpDichAcconto, ImpDichSaldo, ImpDichTotale, sCodCartella, dsVersamenti, dsSanzioni, ListInteressi, ListBaseCalcolo, oRiepilogo)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
End Class
