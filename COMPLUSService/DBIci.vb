Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports log4net
Imports ComPlusInterface
Imports Utility

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe Business/Data Logic che incapsula tutti i dati logici necessari e reperibili da il database OPENgovICI.
    ''' </summary>
    Public Class DBIci
        'Inherits ServicedComponent
        Protected objUtility As New MotoreProvUtility
        Protected objCostanti As New COSTANTValue.CostantiProv
        Dim myUtility As New MotoreProvUtility

        Private Shared Log As ILog = LogManager.GetLogger(GetType(DBIci))

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myStringConnection"></param>
        ''' <param name="IdEnte"></param>
        ''' <param name="IdContribuente"></param>
        ''' <param name="Anno"></param>
        ''' <param name="Tributo"></param>
        ''' <param name="sCodCartella"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="10/12/2019">in caso di calcolo per Cartelle Insoluti devo prendere il pagato per singolo avviso</revision></revisionHistory>
        Public Function GetVersamentiFase2(myStringConnection As String, ByVal IdEnte As String, ByVal IdContribuente As String, ByVal Anno As String, Tributo As String, sCodCartella As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetVersamentiFase2", "IDENTE", "IDCONTRIBUENTE", "ANNO", "TRIBUTO", "CODCARTELLA")
                    myDataSet = ctx.GetDataSet(sSQL, "VERSAMENTI", ctx.GetParam("IDENTE", IdEnte) _
                        , ctx.GetParam("IDCONTRIBUENTE", IdContribuente) _
                        , ctx.GetParam("ANNO", Anno) _
                        , ctx.GetParam("TRIBUTO", Tributo) _
                        , ctx.GetParam("CODCARTELLA", sCodCartella)
                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBICI.GetVersamentiFase2.errore::", ex)
                Throw New Exception("DBICI.GetVersamentiFase2.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        'Public Function GetVersamentiFase2(myStringConnection As String, ByVal IdEnte As String, ByVal IdContribuente As String, ByVal Anno As String, Tributo As String) As DataSet
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim myAdapter As New SqlDataAdapter
        '    Dim myDsDati As New DataSet

        '    Try
        '        cmdMyCommand.Connection = New SqlConnection(myStringConnection)
        '        If cmdMyCommand.Connection.State = ConnectionState.Closed Then
        '            cmdMyCommand.Connection.Open()
        '        End If
        '        cmdMyCommand.CommandTimeout = 0
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.CommandText = "prc_GetVersamentiFase2"
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlParameter("@IDENTE", SqlDbType.VarChar)).Value = IdEnte
        '        cmdMyCommand.Parameters.Add(New SqlParameter("@IDCONTRIBUENTE", SqlDbType.Int)).Value = IdContribuente
        '        cmdMyCommand.Parameters.Add(New SqlParameter("@ANNO", SqlDbType.VarChar)).Value = Anno
        '        cmdMyCommand.Parameters.Add(New SqlParameter("@TRIBUTO", SqlDbType.VarChar)).Value = Tributo
        '        Log.Debug("GetVersamentiFase2.query::" & cmdMyCommand.CommandText & "  " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDsDati, "VERSAMENTI")

        '        Return myDsDati
        '    Catch ex As Exception
        '        Log.Error("DBICI.GetVersamentiFase2.errore::", ex)
        '        Throw New Exception("DBICI.GetVersamentiFase2.errore::" & " " & ex.Message)
        '    Finally
        '        cmdMyCommand.Connection.Close()
        '        cmdMyCommand.Dispose()
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        'End Function
        Public Function getSituazioneFinale(StringConnectionICI As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetSituazioneFinale", "IDENTE", "ANNO", "IDCONTRIBUENTE", "TRIBUTO", "TIPOOPERAZIONE")
                    myDataset = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", IdEnte) _
                                        , ctx.GetParam("ANNO", StringOperation.FormatString(objHashTable("ANNOACCERTAMENTO"))) _
                                        , ctx.GetParam("IDCONTRIBUENTE", IdContribuente) _
                                        , ctx.GetParam("TRIBUTO", StringOperation.FormatString(objHashTable("TRIBUTOCALCOLO"))) _
                                        , ctx.GetParam("TIPOOPERAZIONE", "A")
                                )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getSituazioneFinale::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        '*** 20140509 - TASI ***
        Public Function getDatiImmobili(DBType As String, StringConnectionICI As String, IdEnte As String, IdContribuente As Integer, ByVal arrayIDImmobili() As Integer, ByVal objHashTable As Hashtable) As DataSet
            Dim myDsDati As New DataSet
            Dim sSQL As String

            Try
                Using ctx As New DBModel(DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetDatiImmobili", "IDENTE", "IDCONTRIBUENTE", "LISTIMMOBILI")
                    myDsDati = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", IdEnte) _
                                        , ctx.GetParam("IDCONTRIBUENTE", IdContribuente) _
                                        , ctx.GetParam("LISTIMMOBILI", GetINListForQuery(arrayIDImmobili))
                                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Throw New Exception("Function::getSituazioneVirtualeDichiratoICI::COMPlusService:: " & ex.Message)
            End Try
            Return myDsDati
        End Function
        '*** ***
        'Funzione che da un array crea La stringa In per la query
        Public Function GetINListForQuery(ByVal array() As Integer) As String

            Dim strCodDICHIARAZIONIList As String = ""
            Dim sTmp As String
            Dim intCount As Integer

            Try

                For intCount = -1 To UBound(array) - 1
                    strCodDICHIARAZIONIList = strCodDICHIARAZIONIList & CStr(array(intCount + 1)) & ","
                Next



                If Len(strCodDICHIARAZIONIList) = 0 Then
                    Throw New Exception("Function::GetAnolmalieList::COMPlusService:: " & "Non sono state Trovate Dichiarazioni Da Bonificare")
                Else
                    '*********************************************************
                    'Pulizia dell'Ultima "Virgola" inserita
                    '*********************************************************
                    sTmp = Mid(strCodDICHIARAZIONIList, 1, Len(strCodDICHIARAZIONIList) - 1)
                End If

                Return sTmp

            Catch ex As Exception
                Throw New Exception("Function::GetCodDICHIARAZIONIList::COMPlusService:: " & ex.Message)
            End Try

        End Function

        '*** 20140509 - TASI ***
        Public Function GetSituazioneVirtuali(Tipo As String, DBType As String, StringConnectionICI As String, IdEnte As String, ByVal strCOD_CONTRIBUENTE As String, Anno As String, AnnoDa As String, AnnoA As String) As DataSet
            Dim myDsDati As New DataSet
            Dim sSQL As String
            Dim myProcedure As String

            Try
                If Tipo = "D" Then
                    myProcedure = "prc_GETSITUAZIONEVIRTUALEDICHIARAZIONI"
                Else
                    myProcedure = ""
                End If
                If Anno = "" Then
                    Anno = AnnoDa
                End If
                Using ctx As New DBModel(DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, myProcedure, "IdEnte", "IdContribuente", "AnnoDa", "AnnoA")
                    myDsDati = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IdEnte", IdEnte) _
                                        , ctx.GetParam("IdContribuente", strCOD_CONTRIBUENTE) _
                                        , ctx.GetParam("AnnoDa", Anno) _
                                        , ctx.GetParam("AnnoA", AnnoA)
                                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Error("DBIci.GetSituazioneVirtuali.errore.", ex)
                Throw New Exception("DBICI.GetSituazioneVirtuali.errore." & ex.Message)
            End Try
            Return myDsDati
        End Function
        'Public Function GetSituazioneVirtualeDichiarazioni(StringConnectionICI As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal strCOD_CONTRIBUENTE As String, Anno As String) As DataSet
        '    Dim myAdapter As New SqlDataAdapter
        '    Dim myDsDati As New DataSet
        '    Dim cmdMyCommand As New SqlCommand

        '    Try
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionICI)
        '        If cmdMyCommand.Connection.State = ConnectionState.Closed Then
        '            cmdMyCommand.Connection.Open()
        '        End If
        '        'Valorizzo i parameters:
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdEnte", SqlDbType.NVarChar)).Value = IdEnte
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdContribuente", SqlDbType.Int)).Value = strCOD_CONTRIBUENTE
        '        If Anno <> "" Then
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoDa", SqlDbType.NVarChar)).Value = Anno
        '        Else
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoDa", SqlDbType.NVarChar)).Value = objHashTable("ANNODA").ToString
        '        End If
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoA", SqlDbType.NVarChar)).Value = objHashTable("ANNOA").ToString
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CodTributo", SqlDbType.NVarChar)).Value = objHashTable("TRIBUTOCALCOLO").ToString
        '        cmdMyCommand.CommandText = "prc_GETSITUAZIONEVIRTUALEDICHIARAZIONI"
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDsDati, "SITUAZIONE_VIRTUALE_DICHIARATO_ICI")
        '        Return myDsDati
        '    Catch ex As Exception
        '        Log.Error("Function::GetSituazioneVirtualeDichiarazioni::COMPlusService::" & ex.Message)
        '        Throw New Exception("Function::GetSituazioneVirtualeDichiarazioni::COMPlusService::" & ex.Message)
        '    Finally
        '        myAdapter.Dispose()
        '        If cmdMyCommand.Connection.State = ConnectionState.Open Then
        '            cmdMyCommand.Connection.Close()
        '        End If
        '    End Try
        'End Function
        'Public Function GetSituazioneVirtualeImmobili(StringConnectionICI As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal strCOD_CONTRIBUENTE As String, Anno As String) As DataSet
        '    Dim myAdapter As New SqlDataAdapter
        '    Dim myDsDati As New DataSet
        '    Dim cmdMyCommand As New SqlCommand

        '    Try
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionICI)
        '        If cmdMyCommand.Connection.State = ConnectionState.Closed Then
        '            cmdMyCommand.Connection.Open()
        '        End If
        '        'Valorizzo i parameters:
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdEnte", SqlDbType.NVarChar)).Value = IdEnte
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdContribuente", SqlDbType.Int)).Value = strCOD_CONTRIBUENTE
        '        If Anno <> "" Then
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoDa", SqlDbType.NVarChar)).Value = Anno
        '        Else
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoDa", SqlDbType.NVarChar)).Value = objHashTable("ANNODA").ToString
        '        End If
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AnnoA", SqlDbType.NVarChar)).Value = objHashTable("ANNOA").ToString
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CodTributo", SqlDbType.NVarChar)).Value = objHashTable("TRIBUTOCALCOLO").ToString
        '        cmdMyCommand.CommandText = "prc_GETSITUAZIONEVIRTUALEIMMOBILI"
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDsDati, "SITUAZIONE_VIRTUALE_IMMOBILI_ICI")
        '        Return myDsDati
        '    Catch ex As Exception
        '        Log.Error("Function::GetSituazioneVirtualeImmobili::COMPlusService::" & ex.Message)
        '        Throw New Exception("Function::GetSituazioneVirtualeImmobili::COMPlusService::" & ex.Message)
        '    Finally
        '        myAdapter.Dispose()
        '        If cmdMyCommand.Connection.State = ConnectionState.Open Then
        '            cmdMyCommand.Connection.Close()
        '        End If
        '    End Try
        'End Function
        'Public Function GetDatiTutteDichiarazioniFreezer(myStringConnection As String, IdEnte As String, ByVal strCodContribuente As Long, ByVal strAnno As String, Tributo As String) As DataSet
        '    Dim myAdapter As New SqlClient.SqlDataAdapter
        '    Dim myDataSet As New DataSet
        '    Dim cmdMyCommand As New SqlCommand
        '    Try
        '        Log.Debug("GetDatiTutteDichiarazioniFreezer inizio")

        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
        '        cmdMyCommand.Connection.Open()
        '        cmdMyCommand.CommandTimeout = 0
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.CommandText = "prc_GetDatiTutteDichiarazioniFreezer"
        '        cmdMyCommand.Parameters.AddWithValue("@Ente", IdEnte)
        '        cmdMyCommand.Parameters.AddWithValue("@IdContribuente", strCodContribuente)
        '        cmdMyCommand.Parameters.AddWithValue("@Anno", strAnno)
        '        cmdMyCommand.Parameters.AddWithValue("@IDTRIBUTO", Tributo)
        '        'objDBManager = New DBManager
        '        'objDBManager.Initialize(myStringConnection)
        '        Log.Debug("GetDatiTutteDichiarazioniFreezer::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        'objDSDatiTutteDichiarazioniFreezer = objDBManager.GetPrivateDataSet(cmdMyCommand)
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDataSet, "Create DataView")
        '        myAdapter.Dispose()

        '        Return myDataSet
        '    Catch ex As Exception
        '        Log.Error("Function::GetDatiDichiarazioniFreezer::COMPlusService:: " & ex.Message)
        '        Throw New Exception("Function::GetDatiDichiarazioniFreezer::COMPlusService:: " & ex.Message)
        '    Finally
        '        cmdMyCommand.Connection.Close()
        '        cmdMyCommand.Dispose()
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        'End Function
        '*** ***

        <AutoComplete()>
        Public Function getDATI_TASK_REPOSITORY_CALCOLO_ICI(StringConnectionICI As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
            Try
                Dim _oDbManager As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                Dim sSQL As String = ""
                Dim objDR As New DataSet
                Dim strTipoElaborazione As String
                Dim strUTENTE As String

                strTipoElaborazione = objUtility.CToStr(objHashTable("TIPO_ELABORAZIONE"))
                strUTENTE = objUtility.CToStr(objHashTable("USER"))

                Using ctx As DBModel = _oDbManager
                    sSQL += " SELECT * FROM TP_TASK_REPOSITORY"
                    sSQL += " WHERE TIPO_ELABORAZIONE=" & objUtility.CStrToDB(strTipoElaborazione)
                    sSQL += " AND COD_ENTE='" & IdEnte & "'"
                    sSQL += " ORDER BY DATA_ELABORAZIONE DESC"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDR = ctx.GetDataSet(sSQL, "TASK_REPOSITORY_CALCOLO_ICI")

                    ctx.Dispose()
                End Using
                getDATI_TASK_REPOSITORY_CALCOLO_ICI = objDR

                Return getDATI_TASK_REPOSITORY_CALCOLO_ICI
            Catch ex As Exception
                Log.Debug("Function::getDATI_TASK_REPOSITORY_CALCOLO_ICI::COMPlusService:: " & ex.Message)
                Throw New Exception("Function::getDATI_TASK_REPOSITORY_CALCOLO_ICI::COMPlusService:: " & ex.Message)
            End Try
        End Function

        Public Function GetListTipoPossesso(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
            Try
                Dim sSQL As String = ""
                Dim objDR As New DataSet
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                    '*** 20140509 - TASI ***
                    sSQL = "SELECT ID AS TIPOPOSSESSO,DESCRIZIONE"
                    sSQL += " FROM TBLTIPOUTILIZZO"
                    sSQL += " ORDER BY DESCRIZIONE"
                    '*** ***
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDR = ctx.GetDataSet(sSQL, "TBL")

                    ctx.Dispose()
                End Using
                Return objDR
            Catch ex As Exception
                Throw New Exception("Function::GetTipoPossesso::COMPlusService:: " & ex.Message)
            End Try
        End Function

        Public Function GetListCategorie(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
            Try
                Dim sSQL As String = ""
                Dim objDR As New DataSet
                sSQL = "SELECT CATEGORIACATASTALE FROM TBLCATEGORIACATASTALE"
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDR = ctx.GetDataSet(sSQL, "TBL")

                    ctx.Dispose()
                End Using
                Return objDR
            Catch ex As Exception
                Throw New Exception("Function::GetListCategorie::COMPlusService:: " & ex.Message)
            End Try
        End Function

        Public Function GetListClasse(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
            Try
                Dim sSQL As String = ""
                Dim objDR As New DataSet
                sSQL = "SELECT CLASSE,CLASSE + ' - ' + DESCRIZIONE AS DESCR FROM TblClasse"
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDR = ctx.GetDataSet(sSQL, "TBL")

                    ctx.Dispose()
                End Using
                Return objDR
            Catch ex As Exception
                Throw New Exception("Function::GetListClasse::COMPlusService:: " & ex.Message)
            End Try
        End Function

        Public Function GetListTipoRendita(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
            Try
                Dim sSQL As String = ""
                Dim objDR As New DataSet
                sSQL = "SELECT COD_RENDITA, SIGLA + ' - ' + DESCRIZIONE AS DESCR FROM TIPO_RENDITA ORDER BY SIGLA"
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionICI)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDR = ctx.GetDataSet(sSQL, "TBL")

                    ctx.Dispose()
                End Using
                Return objDR

            Catch ex As Exception
                Throw New Exception("Function::GetListTipoRendita::COMPlusService:: " & ex.Message)
            End Try
        End Function
    End Class
End Namespace