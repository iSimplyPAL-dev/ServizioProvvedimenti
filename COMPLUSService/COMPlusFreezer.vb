Imports System.Globalization
Imports System
Imports System.EnterpriseServices
Imports System.Diagnostics
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.InteropServices
Imports log4net
Imports Utility

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la gestione delle detrazioni
    ''' </summary>
    Friend Class COMPlusFreezer

        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(COMPlusFreezer))

        Const s_DETRAZ As String = "D"

        Private Function GestioneDetrazioni(ByVal dsDetraz As DataSet, ByVal ImportoDetrazDichiarato As Double, ByVal strAnnoOrig As String, ByVal strAnnoFreezer As String) As Double

            Dim objDetrazAnnoOrig() As DataRow
            Dim objDetrazAnnoFreezer() As DataRow
            Dim importoDetrazAnnoOrig As Double
            Dim importoDetrazAnnoFreezer As Double

            Dim ImportoDetrazioneRet As Double

            objDetrazAnnoOrig = dsDetraz.Tables(0).Select("ANNO='" & strAnnoOrig & "'")
            If objDetrazAnnoOrig.Length > 0 Then
                importoDetrazAnnoOrig = objDetrazAnnoOrig(0).Item("VALORE")
            Else
                importoDetrazAnnoOrig = 0
            End If

            objDetrazAnnoFreezer = dsDetraz.Tables(0).Select("ANNO='" & strAnnoFreezer & "'")
            If objDetrazAnnoFreezer.Length > 0 Then
                importoDetrazAnnoFreezer = objDetrazAnnoFreezer(0).Item("VALORE")
            Else
                importoDetrazAnnoFreezer = 0
            End If


            If importoDetrazAnnoOrig = 0 Or importoDetrazAnnoFreezer = 0 Then
                Return ImportoDetrazDichiarato
            End If

            ImportoDetrazioneRet = (ImportoDetrazDichiarato * importoDetrazAnnoFreezer) / importoDetrazAnnoOrig

            Return ImportoDetrazioneRet

        End Function


        '*** 20140509 - TASI ***
        Private Function getDetrazioni(DBType As String, StringConnectionICI As String, IdEnte As String, ByVal Tributo As String) As DataSet
            Dim myDataSet As New DataSet
            Dim sSQL As String

            Try
                Using ctx As New DBModel(DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_TP_ALIQUOTE_ICI_S", "IdEnte", "Tributo", "Anno", "Tipo")
                    myDataSet = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IdEnte", IdEnte) _
                                        , ctx.GetParam("Tributo", Tributo) _
                                        , ctx.GetParam("Anno", "") _
                                        , ctx.GetParam("Tipo", s_DETRAZ)
                                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Error("COMPlusFreeser.getDetrazioni.errore.", ex)
            End Try
            Return myDataSet
        End Function
    End Class
End Namespace