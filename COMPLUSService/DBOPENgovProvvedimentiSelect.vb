Imports System
Imports System.Configuration
Imports System.Data
Imports System.EnterpriseServices
Imports System.Data.SqlClient
Imports log4net
Imports ComPlusInterface
Imports Utility

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe Business/Data Logic che incapsula tutti i dati logici necessari e reperibili da il database OPENgovProvvedimenti.
    ''' </summary>
    Public Class DBOPENgovProvvedimentiSelect
        'Inherits ServicedComponent

        Protected objUtility As New MotoreProvUtility
        Protected objdbIci As DBIci
        'Protected objdbCatasto As DBCatasto
        Protected objConst As COSTANTValue.CostantiProv
        Private Shared Log As ILog = LogManager.GetLogger(GetType(DBOPENgovProvvedimentiSelect))

#Region "Gestione ITER ELABORAZIONE LIQUIDAZIONI"
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
        <AutoComplete()>
        Public Function getVersamentiPerFase2(myStringConnection As String, IdEnte As String, ByVal IdContribuente As String, ByVal Anno As String, Tributo As String, sCodCartella As String) As DataSet
            objdbIci = New DBIci

            Return objdbIci.GetVersamentiFase2(myStringConnection, IdEnte, IdContribuente, Anno, Tributo, sCodCartella)
        End Function
        '<AutoComplete()>
        'Public Function getVersamentiPerFase2(myStringConnection As String, IdEnte As String, ByVal IdContribuente As String, ByVal Anno As String, Tributo As String) As DataSet
        '    objdbIci = New DBIci

        '    Return objdbIci.GetVersamentiFase2(myStringConnection, IdEnte, IdContribuente, Anno, Tributo)
        'End Function


        <AutoComplete()>
        Public Function GetSituazioneVirtual(Tipo As String, DBType As String, StringConnectionICI As String, IdEnte As String, ByVal strCOD_CONTRIBUENTE As String, Anno As String, AnnoDa As String, AnnoA As String) As DataSet

            Dim objDSDichiarazioniBonificate As DataSet = Nothing
            objdbIci = New DBIci

            objDSDichiarazioniBonificate = objdbIci.GetSituazioneVirtuali(Tipo, DBType, StringConnectionICI, IdEnte, strCOD_CONTRIBUENTE, Anno, AnnoDa, AnnoA)

            Return objDSDichiarazioniBonificate

        End Function
        '<AutoComplete()>
        'Public Function GetSituazioneVirtualeDichiarazioni(StringConnectionICI As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal strCOD_CONTRIBUENTE As String, Anno As String) As DataSet

        '    Dim objDSDichiarazioniBonificate As DataSet = Nothing
        '    objdbIci = New DBIci

        '    objDSDichiarazioniBonificate = objdbIci.GetSituazioneVirtualeDichiarazioni(StringConnectionICI, IdEnte, objHashTable, strCOD_CONTRIBUENTE, Anno)

        '    Return objDSDichiarazioniBonificate

        'End Function
        '<AutoComplete()>
        'Public Function GetSituazioneVirtualeImmobili(StringConnectionICI As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal strCOD_CONTRIBUENTE As String, ByVal strAnno As String) As DataSet
        '    Dim objDSDichiarazioniBonificate As DataSet = Nothing
        '    objdbIci = New DBIci

        '    objDSDichiarazioniBonificate = objdbIci.GetSituazioneVirtualeImmobili(StringConnectionICI, IdEnte, objHashTable, strCOD_CONTRIBUENTE, strAnno)

        '    Return objDSDichiarazioniBonificate
        'End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="StringConnectionProvv"></param>
        ''' <param name="strANNO"></param>
        ''' <param name="strCODTRIBUTO"></param>
        ''' <param name="strCODENTE"></param>
        ''' <param name="strCODTIPOPROVVEDIMENTO"></param>
        ''' <param name="objHashTable"></param>
        ''' <returns></returns>
        Public Function GetSogliaMinima(StringConnectionProvv As String, ByVal strANNO As String, ByVal strCODTRIBUTO As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal objHashTable As Hashtable) As Double
            Try
                Dim _oDbManager As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Dim sSQL As String = ""
                Dim objDR As SqlClient.SqlDataReader
                Using ctx As DBModel = _oDbManager
                    sSQL = "SELECT IMPORTO_MINIMO_ANNO" _
                        + " FROM ANNI_PROVVEDIMENTI" _
                        + " WHERE 1=1" _
                        + " AND COD_ENTE=@ENTE" _
                        + " AND (COD_TRIBUTO<>'' OR COD_TRIBUTO=@COD_TRIBUTO)" _
                        + " AND (ANNO<>'' OR ANNO=@ANNO)" _
                        + " AND (COD_TIPO_PROVVEDIMENTO<=0 OR COD_TIPO_PROVVEDIMENTO=@COD_TIPO_PROVVEDIMENTO" _
                        + " ORDER BY ID_TASK_REPOSITORY DESC,DATA_ELABORAZIONE DESC"
                    objDR = ctx.GetDataReader(sSQL, ctx.GetParam("ENTE", strCODENTE) _
                                , ctx.GetParam("COD_TRIBUTO", strCODTRIBUTO) _
                                , ctx.GetParam("ANNO", strANNO) _
                                , ctx.GetParam("COD_TIPO_PROVVEDIMENTO", strCODTIPOPROVVEDIMENTO)
                            )
                    While objDR.Read()
                        GetSogliaMinima = StringOperation.FormatDouble(objDR("IMPORTO_MINIMO_ANNO"))
                    End While
                    objDR.Close()
                    ctx.Dispose()
                End Using
            Catch Ex As Exception
                Log.Error("DBOPENgovProvvedimentiSelect.GetSogliaMinima.Errore::" & Ex.Message)
                GetSogliaMinima = 0
            End Try
            Return GetSogliaMinima
        End Function

        Public Function GetSpese(StringConnectionProvv As String, ByVal strANNO As String, ByVal strCODTRIBUTO As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal objHashTable As Hashtable, ByVal fase As String) As Double
            Try
                Dim _oDbManager As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Dim sSQL As String = ""
                Dim objDR As SqlClient.SqlDataReader
                Using ctx As DBModel = _oDbManager
                    sSQL = " SELECT VALORE_VOCI.ANNO, VALORE_VOCI.VALORE"
                    sSQL += " FROM TIPO_VOCI INNER JOIN TIPO_MISURA ON TIPO_VOCI.MISURA = TIPO_MISURA.COD_MISURA INNER JOIN"
                    sSQL += " VALORE_VOCI ON TIPO_VOCI.COD_TRIBUTO = VALORE_VOCI.COD_TRIBUTO AND TIPO_VOCI.COD_CAPITOLO = VALORE_VOCI.COD_CAPITOLO AND TIPO_VOCI.COD_VOCE = VALORE_VOCI.COD_VOCE"
                    sSQL += " AND TIPO_VOCI.COD_TIPO_PROVVEDIMENTO=VALORE_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " WHERE (TIPO_VOCI.COD_ENTE = " & objUtility.CStrToDB(strCODENTE) & ") "
                    sSQL += " AND (TIPO_VOCI.FASE IN (" & fase & "))  "
                    sSQL += " AND (VALORE_VOCI.COD_TIPO_PROVVEDIMENTO=" & strCODTIPOPROVVEDIMENTO & ") AND"
                    sSQL += " (TIPO_VOCI.COD_TRIBUTO = " & objUtility.CStrToDB(strCODTRIBUTO) & ") AND (TIPO_VOCI.COD_CAPITOLO = '" & OggettoAtto.Capitolo.Spese & "') AND (VALORE_VOCI.ANNO IN"
                    sSQL += "  (SELECT MAX(ANNO)"
                    sSQL += " FROM VALORE_VOCI "
                    sSQL += " WHERE ANNO <=" & objUtility.CStrToDB(strANNO) & " AND (COD_CAPITOLO = '" & OggettoAtto.Capitolo.Spese & "')"
                    sSQL += " AND COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & "))"
                    objDR = ctx.GetDataReader(sSQL)
                    While objDR.Read()
                        GetSpese = StringOperation.FormatDouble(objDR("VALORE"))
                    End While
                    objDR.Close()
                    ctx.Dispose()
                End Using
            Catch Ex As Exception
                Log.Error("DBOPENgovProvvedimentiSelect.GetSpese.Errore::" & Ex.Message)
                GetSpese = 0
            End Try
            Return GetSpese
        End Function

        Public Function GetSpese(StringConnectionProvv As String, ByVal strANNO As String, ByVal strCODTRIBUTO As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal objHashTable As Hashtable) As Double
            Try
                Dim _oDbManager As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Dim sSQL As String = ""
                Dim objDR As SqlClient.SqlDataReader
                Using ctx As DBModel = _oDbManager
                    sSQL = " SELECT VALORE_VOCI.ANNO, VALORE_VOCI.VALORE"
                    sSQL += " FROM TIPO_VOCI INNER JOIN TIPO_MISURA ON TIPO_VOCI.MISURA = TIPO_MISURA.COD_MISURA INNER JOIN"
                    sSQL += " VALORE_VOCI ON TIPO_VOCI.COD_TRIBUTO = VALORE_VOCI.COD_TRIBUTO AND TIPO_VOCI.COD_CAPITOLO = VALORE_VOCI.COD_CAPITOLO AND TIPO_VOCI.COD_VOCE = VALORE_VOCI.COD_VOCE"
                    sSQL += " AND TIPO_VOCI.COD_TIPO_PROVVEDIMENTO=VALORE_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " WHERE (TIPO_VOCI.COD_ENTE = " & objUtility.CStrToDB(strCODENTE) & ") "
                    sSQL += " AND (VALORE_VOCI.COD_TIPO_PROVVEDIMENTO=" & strCODTIPOPROVVEDIMENTO & ") AND"
                    sSQL += " (TIPO_VOCI.COD_TRIBUTO = " & objUtility.CStrToDB(strCODTRIBUTO) & ") AND (TIPO_VOCI.COD_CAPITOLO = '" & OggettoAtto.Capitolo.Spese & "') AND (VALORE_VOCI.ANNO IN"
                    sSQL += "  (SELECT MAX(ANNO)"
                    sSQL += " FROM VALORE_VOCI "
                    sSQL += " WHERE ANNO <=" & objUtility.CStrToDB(strANNO) & " AND (COD_CAPITOLO = '" & OggettoAtto.Capitolo.Spese & "')"
                    sSQL += " AND COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & "))"
                    objDR = ctx.GetDataReader(sSQL)
                    While objDR.Read()
                        GetSpese = FormatNumber(StringOperation.FormatDouble(objDR("VALORE")), 2)
                    End While
                    objDR.Close()
                    ctx.Dispose()
                End Using
            Catch Ex As Exception
                Log.Error("DBOPENgovProvvedimentiSelect.GetSpese.Errore::" & Ex.Message)
                GetSpese = 0
            End Try
            Return GetSpese
        End Function
#End Region
        '<AutoComplete()> Public Function getNewIDdbICI(StringConnectionProvv As String, ByVal strNomeTabella As String, ByVal objHashTable As Hashtable) As Long
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim myDataSet As New DataSet
        '    Dim myAdapter As New SqlDataAdapter
        '    Try
        '        objDBManager = New DBManager
        '        objConst = New COSTANTValue.CostantiProv
        '        objUtility = New MotoreProvUtility

        '        Dim sSQL As String
        '        Dim dr As SqlDataReader
        '        Dim lngMaxId As Long
        '        Dim intRetVal As Integer
        '        Log.Debug("inizio getnewid")
        '        'objDBManager.Initialize(StringConnectionICI)

        '        sSQL = "SELECT MAXID FROM CONTATORI  WHERE NOME_TABELLA =" & objUtility.CStrToDB(strNomeTabella)
        '        'dr = objDBManager.GetPrivateDataReaderCOMplus(sSQL)
        '        'If dr.Read Then
        '        '    lngMaxId = dr.Item("MAXID")
        '        '    lngMaxId = lngMaxId + objConst.VALUE_INCREMENT
        '        'End If
        '        'dr.Close()
        '        cmdMyCommand.Connection = New SqlConnection(StringConnectionProvv)
        '        cmdMyCommand.Connection.Open()
        '        cmdMyCommand.CommandTimeout = 0

        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.CommandText = sSQL
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDataSet, "NEWID")
        '        myAdapter.Dispose()
        '        If myDataSet.Tables(0).Rows.Count > 1 Then
        '            For Each myRow As DataRow In myDataSet.Tables(0).Rows
        '                lngMaxId = myRow("MAXID") + objConst.VALUE_INCREMENT
        '            Next
        '        Else
        '            lngMaxId = 1
        '        End If
        '        Log.Debug("prelevato getnewid")

        '        sSQL = "UPDATE CONTATORI SET MAXID=" & lngMaxId & " WHERE NOME_TABELLA ='" & strNomeTabella & "'"
        '        cmdMyCommand.CommandText = sSQL
        '        cmdMyCommand.ExecuteNonQuery()
        '        Log.Debug("aggiornato getnewid")
        '        'objDBManager.Execute(sSQL)
        '        'If intRetVal = objConst.INIT_VALUE_NUMBER Then
        '        '    If Not IsNothing(objDBManager) Then
        '        '        objDBManager.Kill()
        '        '        objDBManager.Dispose()
        '        '    End If
        '        '    Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::getNewIDdbICI::DBOPENgovProvvedimentiSelect:: Update Fallito")
        '        '    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewIDdbICI::DBOPENgovProvvedimentiSelect")
        '        'End If

        '        Return lngMaxId
        '    Catch ex As Exception
        '        Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::getNewIDdbICI::DBOPENgovProvvedimentiSelect " & ex.Message)
        '        Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewIDdbICI::DBOPENgovProvvedimentiSelect " & ex.Message)
        '    Finally
        '        'If Not IsNothing(objDBManager) Then
        '        '    objDBManager.Kill()
        '        '    objDBManager.Dispose()
        '        'End If
        '        If cmdMyCommand.Connection.State = ConnectionState.Open Then
        '            cmdMyCommand.Connection.Close()
        '        End If
        '    End Try
        'End Function

        '*** 20140509 - TASI ***
        <AutoComplete()>
        Public Function getNewID(ByVal strNomeTabella As String, ByVal myStringConnection As String) As Long
            Dim cmdMyCommand As New SqlCommand

            Try
                objConst = New COSTANTValue.CostantiProv
                objUtility = New MotoreProvUtility

                Dim sSQL As String = ""
                Dim dr As SqlDataReader
                Dim lngMaxId As Long
                Dim intRetVal As Integer
                Log.Debug("inizio getnewid")
                'sSQL = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED "
                sSQL += "SELECT MAXID FROM CONTATORI WHERE NOME_TABELLA =" & objUtility.CStrToDB(strNomeTabella)
                Log.Debug("query::" & sSQL)
                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.CommandTimeout = 0
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandText = sSQL
                Log.Debug("getNewID::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
                dr = cmdMyCommand.ExecuteReader()
                If dr.Read Then
                    lngMaxId = dr.Item("MAXID")
                    lngMaxId = lngMaxId + objConst.VALUE_INCREMENT
                End If
                dr.Close()
                Log.Debug("devo aggiornare getnewid")
                sSQL = "UPDATE CONTATORI SET MAXID=" & lngMaxId & " WHERE NOME_TABELLA ='" & strNomeTabella & "'"
                cmdMyCommand.CommandText = sSQL
                cmdMyCommand.ExecuteNonQuery()
                If intRetVal = objConst.INIT_VALUE_NUMBER Then
                    Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::getNewID::DBOPENgovProvvedimentiSelect")
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewID::DBOPENgovProvvedimentiSelect")
                End If
                Log.Debug("fine getnewid")
                Return lngMaxId
            Catch ex As Exception
                Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::getNewID::DBOPENgovProvvedimentiSelect:: " & ex.Message)
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewID::DBOPENgovProvvedimentiSelect:: " & ex.Message)
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        <AutoComplete()>
        Public Function getNewNumeroAtto(StringConnectionProvv As String, ByVal objHashTable As Hashtable) As String
            Try
                objConst = New COSTANTValue.CostantiProv
                objUtility = New MotoreProvUtility

                Dim ANNO, COD_ENTE As String
                Dim ANNOELABORAZIONE As String
                Dim sSQL As String
                Dim dr As SqlDataReader
                Dim iNUMERO_ATTO As Integer = -1
                Dim sNUMERO_ATTO As String = "-1"

                ANNO = objHashTable("ANNO")
                COD_ENTE = objHashTable("COD_ENTE")

                ANNOELABORAZIONE = objHashTable("ANNOELABORAZIONE")

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    sSQL = "SELECT * FROM TBLNUMEROATTO "
                    sSQL += " WHERE COD_ENTE='" & COD_ENTE & "'"
                    '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
                    sSQL += " AND ANNO='" & ANNOELABORAZIONE & "'"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    dr = ctx.GetDataReader(sSQL)
                    If dr.HasRows Then
                        'riga trovata 
                        'aumento di 1 il valore trovato e lo restituisco in output
                        dr.Read()
                        iNUMERO_ATTO = dr.Item("NUMERO_ATTO") + 1

                        dr.Close()

                        sSQL = "UPDATE TBLNUMEROATTO SET NUMERO_ATTO=" & iNUMERO_ATTO
                        sSQL += " WHERE COD_ENTE='" & COD_ENTE & "'"
                        '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
                        sSQL += " and ANNO='" & ANNOELABORAZIONE & "'"
                        ctx.ExecuteNonQuery(sSQL)
                    Else
                        'riga non trovata 
                        'inserisco nuovo valore (1) e lo restituisco in output
                        dr.Close()

                        iNUMERO_ATTO = 1
                        sSQL = "INSERT INTO TBLNUMEROATTO (NUMERO_ATTO,COD_ENTE,ANNO)"
                        sSQL += " VALUES("
                        sSQL += "" & iNUMERO_ATTO & ","
                        sSQL += "'" & COD_ENTE & "',"
                        '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
                        sSQL += "'" & ANNOELABORAZIONE & "'"
                        sSQL += " )"
                        ctx.ExecuteNonQuery(sSQL)
                    End If


                    ctx.Dispose()
                End Using

                If iNUMERO_ATTO = objConst.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewNumeroAtto::DBOPENgovProvvedimentiSelect")
                End If

                Dim LUNGHEZZA_STRINGA_ATTO As Integer
                LUNGHEZZA_STRINGA_ATTO = CType(ConfigurationSettings.AppSettings("LUNGHEZZA_STRINGA_ATTO").ToString, Integer)

                '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
                sNUMERO_ATTO = Right(objHashTable("ANNOELABORAZIONE"), 2) & "/" & CType(iNUMERO_ATTO, String).PadLeft(LUNGHEZZA_STRINGA_ATTO, "0")

                Return sNUMERO_ATTO

            Catch ex As Exception
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewNumeroAtto::DBOPENgovProvvedimentiSelect:: " & ex.Message)
            End Try
        End Function
        '<AutoComplete()>
        'Public Function getNewNumeroAtto(StringConnectionProvv As String, ByVal objHashTable As Hashtable) As String
        '    Try
        '        objDBManager = New DBManager
        '        objConst = New COSTANTValue.CostantiProv
        '        objUtility = New MotoreProvUtility

        '        Dim ANNO, COD_ENTE As String

        '        Dim ANNOELABORAZIONE As String


        '        Dim sSQL As String
        '        Dim dr As SqlDataReader
        '        Dim iNUMERO_ATTO As Integer = -1
        '        Dim sNUMERO_ATTO As String = "-1"

        '        ANNO = objHashTable("ANNO")
        '        COD_ENTE = objHashTable("COD_ENTE")

        '        ANNOELABORAZIONE = objHashTable("ANNOELABORAZIONE")

        '        objDBManager.Initialize(StringConnectionProvv)

        '        sSQL = "select * from TblNumeroAtto "
        '        sSQL += " where COD_ENTE='" & COD_ENTE & "'"
        '        'sSQL += " and ANNO='" & ANNO & "'"

        '        '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
        '        sSQL += " and ANNO='" & ANNOELABORAZIONE & "'"


        '        dr = objDBManager.GetPrivateDataReaderCOMplus(sSQL)

        '        If dr.HasRows Then
        '            'riga trovata 
        '            'aumento di 1 il valore trovato e lo restituisco in output
        '            dr.Read()
        '            iNUMERO_ATTO = dr.Item("NUMERO_ATTO") + 1
        '            sSQL = "update TblNumeroAtto set NUMERO_ATTO=" & iNUMERO_ATTO
        '            sSQL += " where COD_ENTE='" & COD_ENTE & "'"
        '            '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
        '            'sSQL += " and ANNO='" & ANNO & "'"
        '            sSQL += " and ANNO='" & ANNOELABORAZIONE & "'"

        '            objDBManager.Execute(sSQL)
        '        Else
        '            'riga non trovata 
        '            'inserisco nuovo valore (1) e lo restituisco in output
        '            iNUMERO_ATTO = 1
        '            sSQL = "insert into TblNumeroAtto "
        '            sSQL += "(NUMERO_ATTO,COD_ENTE,ANNO)"
        '            sSQL += " values("
        '            sSQL += "" & iNUMERO_ATTO & ","
        '            sSQL += "'" & COD_ENTE & "',"
        '            '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa
        '            'sSQL += "'" & ANNO & "'"
        '            sSQL += "'" & ANNOELABORAZIONE & "'"
        '            sSQL += " )"
        '            objDBManager.Execute(sSQL)
        '        End If

        '        dr.Close()


        '        If iNUMERO_ATTO = objConst.INIT_VALUE_NUMBER Then
        '            If Not IsNothing(objDBManager) Then
        '                objDBManager.Kill()
        '                objDBManager.Dispose()

        '            End If
        '            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewNumeroAtto::DBOPENgovProvvedimentiSelect")
        '        End If

        '        Dim LUNGHEZZA_STRINGA_ATTO As Integer
        '        LUNGHEZZA_STRINGA_ATTO = CType(ConfigurationSettings.AppSettings("LUNGHEZZA_STRINGA_ATTO").ToString, Integer)

        '        '*** 20112008 Fabi - invece dell'anno del provvedimento usare l'anno di stampa

        '        'sNUMERO_ATTO = Right(objHashTable("ANNO"), 2) & "/" & CType(iNUMERO_ATTO, String).PadLeft(LUNGHEZZA_STRINGA_ATTO, "0")
        '        sNUMERO_ATTO = Right(objHashTable("ANNOELABORAZIONE"), 2) & "/" & CType(iNUMERO_ATTO, String).PadLeft(LUNGHEZZA_STRINGA_ATTO, "0")

        '        Return sNUMERO_ATTO

        '    Catch ex As Exception
        '        Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::getNewNumeroAtto::DBOPENgovProvvedimentiSelect:: " & ex.Message)
        '    Finally
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()

        '        End If
        '    End Try
        'End Function
        <AutoComplete()> Public Function getAnagraficaIndirizziSpedizione(DBType As String, ByVal strCodContribuente As String, ByVal strCodTributo As String, ByVal myStringConnection As String) As DataSet
            Dim sSQL As String
            Dim myDataSet As New DataSet

            Try
                Using ctx As New DBModel(DBType, myStringConnection)
                    sSQL = "SELECT * "
                    sSQL += " FROM INDIRIZZI_SPEDIZIONE"
                    sSQL += " INNER JOIN DATA_VALIDITA_SPEDIZIONE ON INDIRIZZI_SPEDIZIONE.COD_TRIBUTO = DATA_VALIDITA_SPEDIZIONE.COD_TRIBUTO"
                    sSQL += " AND INDIRIZZI_SPEDIZIONE.COD_CONTRIBUENTE = DATA_VALIDITA_SPEDIZIONE.COD_CONTRIBUENTE"
                    sSQL += " AND INDIRIZZI_SPEDIZIONE.IDDATA = DATA_VALIDITA_SPEDIZIONE.IDDATA"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND DATA_VALIDITA_SPEDIZIONE.COD_TRIBUTO = " & objUtility.CStrToDB(strCodTributo)
                    sSQL += " AND DATA_VALIDITA_SPEDIZIONE.COD_CONTRIBUENTE = " & strCodContribuente
                    sSQL += " AND (INDIRIZZI_SPEDIZIONE.DATA_FINE_VALIDITA IS NULL OR INDIRIZZI_SPEDIZIONE.DATA_FINE_VALIDITA='')"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_INDIRIZZI_SPEDIZIONE")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getAnagraficaIndirizziSpedizione::si è verificato il seguente errore::", ex)
                myDataSet = Nothing
            End Try
            Return myDataSet
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="objHashTable"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function getLOAD_PROVVEDIMENTI(StringConnectionProvv As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetProvvedimento", "IDPROVVEDIMENTO")
                    myDataset = ctx.GetDataSet(sSQL, "PROVVEDIMENTO", ctx.GetParam("IDPROVVEDIMENTO", StringOperation.FormatInt(objHashTable("IDPROVVEDIMENTO"))))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getLOAD_PROVVEDIMENTI::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        '<AutoComplete()>
        'Public Function getLOAD_PROVVEDIMENTI(ByVal objHashTable As Hashtable) As DataSet
        '    Dim myDataSet As New DataSet
        '    Dim myAdapter As New SqlDataAdapter
        '    Dim cmdMyCommand As New SqlCommand
        '    Try
        '        Dim strIDPROVVEDIMENTO As String = CType(objHashTable("IDPROVVEDIMENTO"), String)
        '        cmdMyCommand.Connection = New SqlConnection(StringConnectionProvv)
        '        cmdMyCommand.Connection.Open()
        '        cmdMyCommand.CommandTimeout = 0
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.CommandText = "prc_GetProvvedimento"
        '        cmdMyCommand.Parameters.AddWithValue("@IdProvvedimento", strIDPROVVEDIMENTO)
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDataSet, "PROVVEDIMENTO")
        '        myAdapter.Dispose()
        '    Catch ex As Exception
        '        Log.Debug("getLOAD_PROVVEDIMENTI::si è verificato il seguente errore::" & ex.Message)
        '    Finally
        '        cmdMyCommand.Connection.Close()
        '        cmdMyCommand.Dispose()
        '    End Try
        '    Return myDataSet
        'End Function
        <AutoComplete()>
        Public Function GetSanzioni(ByVal dblIMPORTO As Double, ByVal sANNO As String, ByVal sCODTRIBUTO As String, ByVal sCODCAPITOLO As String, ByVal sCODVOCE As String, ByVal sIdEnte As String, ByVal sCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, COD_TIPO_PROCEDIMENTO As String, ByVal myConnectionString As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet

            'COD_TIPO_PROCEDIMENTO = L -->preaccertamento
            'COD_TIPO_PROCEDIMENTO = A -->accertamento
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myConnectionString)
                    If COD_TIPO_PROCEDIMENTO = "L" Then
                        sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetSanzioniPREAccertamento", "IDENTE", "CODTIPOPROVVEDIMENTO", "CODTRIBUTO", "ANNO", "CODCAPITOLO", "FASE")
                        myDataSet = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", sIdEnte) _
                            , ctx.GetParam("CODTIPOPROVVEDIMENTO", sCODTIPOPROVVEDIMENTO) _
                            , ctx.GetParam("CODTRIBUTO", sCODTRIBUTO) _
                            , ctx.GetParam("ANNO", sANNO) _
                            , ctx.GetParam("CODCAPITOLO", sCODCAPITOLO) _
                            , ctx.GetParam("FASE", lngGenericID)
                        )
                    Else
                        sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetSanzioniAccertamento", "IDENTE", "CODTIPOPROVVEDIMENTO", "CODTRIBUTO", "ANNO", "CODCAPITOLO", "CODVOCE")
                        myDataSet = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", sIdEnte) _
                            , ctx.GetParam("CODTIPOPROVVEDIMENTO", sCODTIPOPROVVEDIMENTO) _
                            , ctx.GetParam("CODTRIBUTO", sCODTRIBUTO) _
                            , ctx.GetParam("ANNO", sANNO) _
                            , ctx.GetParam("CODCAPITOLO", sCODCAPITOLO) _
                            , ctx.GetParam("CODVOCE", sCODVOCE)
                        )
                    End If
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("GetSanzioni::si è verificato il seguente errore::", ex)
                Return Nothing
            End Try
            Return myDataSet
        End Function
        '<AutoComplete()>
        'Public Function GetSanzioni(ByVal dblIMPORTO As Double, ByVal sANNO As String, ByVal sCODTRIBUTO As String, ByVal sCODCAPITOLO As String, ByVal sCODVOCE As String, ByVal sIdEnte As String, ByVal sCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, COD_TIPO_PROCEDIMENTO As String, ByVal myConnectionString As String) As DataSet
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim myDataSet As New DataSet
        '    Dim myAdapter As New SqlDataAdapter
        '    Dim sMyStoredProcedure As String

        '    Try
        '        'COD_TIPO_PROCEDIMENTO = L -->preaccertamento
        '        'COD_TIPO_PROCEDIMENTO = A -->accertamento
        '        cmdMyCommand.Connection = New SqlConnection(myConnectionString)
        '        cmdMyCommand.Connection.Open()
        '        cmdMyCommand.CommandTimeout = 0

        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = sIdEnte
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODTIPOPROVVEDIMENTO", SqlDbType.Int)).Value = sCODTIPOPROVVEDIMENTO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODTRIBUTO", SqlDbType.NVarChar)).Value = sCODTRIBUTO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = sANNO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODCAPITOLO", SqlDbType.NVarChar)).Value = sCODCAPITOLO
        '        If COD_TIPO_PROCEDIMENTO = "L" Then
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FASE", SqlDbType.NVarChar)).Value = lngGenericID
        '            sMyStoredProcedure = "prc_GetSanzioniPREAccertamento"
        '        Else
        '            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVOCE", SqlDbType.NVarChar)).Value = sCODVOCE
        '            sMyStoredProcedure = "prc_GetSanzioniAccertamento"
        '        End If
        '        cmdMyCommand.CommandText = sMyStoredProcedure
        '        Log.Debug("GetSanzioni::SQL::" & sMyStoredProcedure & "::@IDENTE::" & sIdEnte & "::@CODTIPOPROVVEDIMENTO::" & sCODTIPOPROVVEDIMENTO & "::@CODTRIBUTO::" & sCODTRIBUTO & "::@ANNO::" & sANNO & "::@CODCAPITOLO::" & sCODCAPITOLO & "::@FASE::" & lngGenericID.ToString & "::@CODVOCE::" & sCODVOCE)
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(myDataSet, "VALORE_VOCI")
        '        myAdapter.Dispose()
        '        Return myDataSet
        '    Catch ex As Exception
        '        Log.Debug("GetSanzioni::si è verificato il seguente errore::", ex)
        '        Return Nothing
        '    Finally
        '        cmdMyCommand.Connection.Close()
        '        cmdMyCommand.Dispose()
        '    End Try
        'End Function
        '*** ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myStringConnection"></param>
        ''' <param name="IdEnte"></param>
        ''' <param name="CodTributo"></param>
        ''' <param name="Anno"></param>
        ''' <param name="CodVoce"></param>
        ''' <param name="Fase"></param>
        ''' <param name="TipoProvvedimento"></param>
        ''' <param name="TipoProcedimento"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function GetInteressi(myStringConnection As String, ByVal IdEnte As String, ByVal CodTributo As String, ByVal Anno As String, ByVal CodVoce As String, ByVal Fase As Integer, ByVal TipoProvvedimento As String, TipoProcedimento As String) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetInteressi", "IDENTE", "CODTRIBUTO", "TIPOPROCEDIMENTO", "CAPITOLO", "TIPOPROVVEDIMENTO", "FASE", "ANNO")
                    myDataset = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", IdEnte) _
                        , ctx.GetParam("CODTRIBUTO", CodTributo) _
                        , ctx.GetParam("TIPOPROCEDIMENTO", TipoProcedimento) _
                        , ctx.GetParam("CAPITOLO", OggettoAtto.Capitolo.Interessi) _
                        , ctx.GetParam("TIPOPROVVEDIMENTO", TipoProvvedimento) _
                        , ctx.GetParam("FASE", Fase) _
                        , ctx.GetParam("ANNO", Anno)
                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug(IdEnte + " - DBOPENgovProvvedimentiSelect.GetInteressi.errore: ", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        '<AutoComplete()>
        'Public Function GetInteressi(myStringConnection As String, ByVal IdEnte As String, ByVal CodTributo As String, ByVal Anno As String, ByVal CodVoce As String, ByVal Fase As Integer, ByVal TipoProvvedimento As String, TipoProcedimento As String) As DataSet
        '    Dim myAdapter As New SqlClient.SqlDataAdapter
        '    Dim dsMyDati As New DataSet
        '    Dim cmdMyCommand As New SqlClient.SqlCommand

        '    Try
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
        '        cmdMyCommand.Connection.Open()
        '        cmdMyCommand.CommandTimeout = 0
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.CommandText = "prc_GetInteressi"
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.AddWithValue("@IDENTE", IdEnte)
        '        cmdMyCommand.Parameters.AddWithValue("@CODTRIBUTO", CodTributo)
        '        cmdMyCommand.Parameters.AddWithValue("@TIPOPROCEDIMENTO", TipoProcedimento)
        '        cmdMyCommand.Parameters.AddWithValue("@CAPITOLO", OggettoAtto.Capitolo.Interessi)
        '        cmdMyCommand.Parameters.AddWithValue("@TIPOPROVVEDIMENTO", TipoProvvedimento)
        '        cmdMyCommand.Parameters.AddWithValue("@FASE", Fase)
        '        cmdMyCommand.Parameters.AddWithValue("@ANNO", Anno)
        '        myAdapter.SelectCommand = cmdMyCommand
        '        myAdapter.Fill(dsMyDati, "GET_INTERESSI")
        '    Catch ex As Exception
        '        Log.Debug(IdEnte + " - DBOPENgovProvvedimentiSelect.GetInteressi.errore: ", ex)
        '        Log.Debug("GetTipologiaInteressi::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        dsMyDati = Nothing
        '    Finally
        '        myAdapter.Dispose()
        '        cmdMyCommand.Dispose()
        '        cmdMyCommand.Connection.Close()
        '    End Try
        '    Return dsMyDati
        'End Function
        '**** 201809 - Cartelle Insoluti ***
        <AutoComplete()>
        Public Function GetTipologiaInteressi(ByVal strANNO As String, ByVal CodTributo As String, ByVal strCODENTE As String, ByVal myStringConnection As String) As DataSet
            Dim dsMyDati As New DataSet
            Dim sSQL As String

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GETSCADENZAINTERESSI", "Ente", "Anno", "Tributo")
                    dsMyDati = ctx.GetDataSet(sSQL, "GET_TIPOLOGIA_INTERESSI", ctx.GetParam("Ente", strCODENTE) _
                                        , ctx.GetParam("Anno", strANNO) _
                                        , ctx.GetParam("Tributo", CodTributo)
                                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug(strCODENTE + " - DBOPENgovProvvedimentiSelect.GetTipologiaInteressi.errore: ", ex)
                dsMyDati = Nothing
            End Try
            Return dsMyDati
        End Function
        '<AutoComplete()>
        'Public Function GetTipologiaInteressiICI(ByVal strANNO As String, ByVal strCODENTE As String, ByVal myStringConnection As String) As DataSet
        '    Dim objDSGetTipologiaInteressi As DataSet = Nothing
        '    Dim sSQL As String
        '    Dim objDA As SqlDataAdapter
        '    objUtility = New MotoreProvUtility

        '    sSQL = " SELECT INT_ACCONTO_SALDO, INT_SALDO"
        '    sSQL += " FROM TP_GENERALE_ICI"
        '    sSQL += " WHERE (COD_ENTE = " & objUtility.CStrToDB(strCODENTE) & ") "
        '    sSQL += " AND (ANNO = " & objUtility.CStrToDB(strANNO) & ")"

        '    objDSGetTipologiaInteressi = New DataSet
        '    objDBManager = New DBManager

        '    objDBManager.Initialize(myStringConnection)
        '    Log.Debug("GET_TIPOLOGIA_INTERESSI::SQL::" & sSQL)
        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)
        '    objDA.Fill(objDSGetTipologiaInteressi, "GET_TIPOLOGIA_INTERESSI")
        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()
        '    End If
        '    Return objDSGetTipologiaInteressi
        'End Function
        '<AutoComplete()>
        'Public Function GetTipologiaInteressiTARSU(ByVal strANNO As String, ByVal CodTributo As String, ByVal strCODENTE As String, myStringConnection As String) As DataSet
        '    Dim objDSGetTipologiaInteressiTARSU As DataSet = Nothing
        '    'Dim sSQL As String
        '    Dim objDA As SqlDataAdapter
        '    Try
        '        objUtility = New MotoreProvUtility

        '        objDSGetTipologiaInteressiTARSU = New DataSet
        '        objDBManager = New DBManager

        '        objDBManager.Initialize(myStringConnection)

        '        '*** 20130801 - accertamento OSAP ***
        '        'sSQL = " SELECT DATA_SCADENZA"
        '        'sSQL += " FROM TAB_SCADENZA_INTERESSI"
        '        'sSQL += " WHERE (COD_ENTE = " & objUtility.CStrToDB(strCODENTE) & ") "
        '        'sSQL += " AND (ANNO = " & objUtility.CStrToDB(strANNO) & ")"
        '        'objDA = objDBManager.GetPrivateDataAdapter(sSQL)
        '        Dim cmdMyCommand As New SqlCommand
        '        cmdMyCommand.Connection = New SqlConnection(myStringConnection)
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        cmdMyCommand.CommandText = "prc_GETSCADENZAINTERESSI"
        '        cmdMyCommand.Parameters.AddWithValue("@Ente", strCODENTE)
        '        cmdMyCommand.Parameters.AddWithValue("@Anno", strANNO)
        '        cmdMyCommand.Parameters.AddWithValue("@Tributo", CodTributo)
        '        Log.Debug("GetTipologiaInteressiTARSU::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        objDA = objDBManager.GetPrivateDataAdapter(cmdMyCommand)
        '        '*** ***
        '        objDA.Fill(objDSGetTipologiaInteressiTARSU, "GET_TIPOLOGIA_INTERESSI_TARSU")
        '    Catch ex As Exception
        '        Log.Debug("GetTipologiaInteressiTARSU.errore::", ex)
        '        objDSGetTipologiaInteressiTARSU = Nothing
        '    Finally
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        '    Return objDSGetTipologiaInteressiTARSU
        'End Function
        '*** ***
#Region "Gestione ITER ATTI"
        <AutoComplete()>
        Public Function getATTIRicercaSemplice(StringConnectionProvv As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Log.Debug("getATTIRicercaSemplice::inizio")

            Dim objDSATTIRicercaSemplice As DataSet = Nothing
            Dim sSQL As String
            Dim objDA As New SqlDataAdapter
            objUtility = New MotoreProvUtility
            Dim strNumeroAvviso As String = ""
            Dim strNumeroAtto As String = ""
            Dim strCognome As String
            Dim strNome As String
            Dim strCodiceFiscale As String
            Dim strPartitaIVA As String

            Dim strCOD_TRIBUTO As String = ""

            Try
                strNumeroAtto = CType(objHashTable("NUMEROPROVVEDIMENTO"), String)
                strCOD_TRIBUTO = CType(objHashTable("CODTRIBUTO"), String)

                strCognome = objUtility.CToStr(objHashTable("COGNOME"))
                strNome = objUtility.CToStr(objHashTable("NOME"))
                strCodiceFiscale = objUtility.CToStr(objHashTable("CODICEFISCALE"))
                strPartitaIVA = objUtility.CToStr(objHashTable("PARTITAIVA"))
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    sSQL = "SELECT DISTINCT COD_CONTRIBUENTE, NOMINATIVO, CODICE_FISCALE, PARTITA_IVA"
                    sSQL += " FROM V_GETATTIRICERCASEMPLICE"
                    sSQL += " WHERE (COD_ENTE='" & strCOD_ENTE & "')"
                    If Trim(strCognome) <> "" Then
                        sSQL += " AND (COGNOME LIKE '" & Replace(Replace(Trim(strCognome), "'", "''"), "*", "%") & "%')"
                    End If
                    If Trim(strNome) <> "" Then
                        sSQL += " AND (NOME LIKE '" & Replace(Replace(Trim(strNome), "'", "''"), "*", "%") & "%')"
                    End If
                    If Trim(strCodiceFiscale) <> "" Then
                        sSQL += " AND (CODICE_FISCALE LIKE '" & Replace(Trim(strCodiceFiscale), "*", "%") & "%')"
                    End If
                    If Trim(strPartitaIVA) <> "" Then
                        sSQL += " AND (PARTITA_IVA LIKE '" & Replace(Trim(strPartitaIVA), "*", "%") & "%')"
                    End If
                    If Len(strNumeroAtto) > 0 Then
                        sSQL += "AND (NUMERO_ATTO=" & objUtility.CStrToDB(strNumeroAtto) & ")"
                    End If
                    If strCOD_TRIBUTO <> "-1" And strCOD_TRIBUTO <> "" Then
                        sSQL += " AND (COD_TRIBUTO='" & strCOD_TRIBUTO & "')"
                    End If
                    sSQL += " ORDER BY NOMINATIVO"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDSATTIRicercaSemplice = ctx.GetDataSet(sSQL, "TP_ATTI_RICERCA_SEMPLICE")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getAttiRicercaSemplice::si è verificato il seguente errore::", ex)
                objDSATTIRicercaSemplice = Nothing
            End Try
            Return objDSATTIRicercaSemplice
        End Function
        '<AutoComplete()>
        'Public Function getATTIRicercaSemplice(StringConnectionProvv As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable) As DataSet
        '    Log.Debug("getATTIRicercaSemplice::inizio")

        '    Dim objDSATTIRicercaSemplice As DataSet = Nothing
        '    Dim sSQL As String
        '    Dim objDA As SqlDataAdapter
        '    objUtility = New MotoreProvUtility
        '    Dim strNumeroAvviso As String = ""
        '    Dim strNumeroAtto As String = ""
        '    Dim strCognome As String
        '    Dim strNome As String
        '    Dim strCodiceFiscale As String
        '    Dim strPartitaIVA As String

        '    Dim strCOD_TRIBUTO As String = ""

        '    'strNumeroAvviso = CType(objHashTable("NUMEROPROVVEDIMENTO"), String)
        '    strNumeroAtto = CType(objHashTable("NUMEROPROVVEDIMENTO"), String)
        '    strCOD_TRIBUTO = CType(objHashTable("CODTRIBUTO"), String)

        '    strCognome = objUtility.CToStr(objHashTable("COGNOME"))
        '    strNome = objUtility.CToStr(objHashTable("NOME"))
        '    strCodiceFiscale = objUtility.CToStr(objHashTable("CODICEFISCALE"))
        '    strPartitaIVA = objUtility.CToStr(objHashTable("PARTITAIVA"))


        '    sSQL = "SELECT DISTINCT COD_CONTRIBUENTE, NOMINATIVO, CODICE_FISCALE, PARTITA_IVA"
        '    sSQL += " FROM V_GETATTIRICERCASEMPLICE"
        '    sSQL += " WHERE (COD_ENTE='" & strCOD_ENTE & "')"
        '    If Trim(strCognome) <> "" Then
        '        sSQL += " AND (COGNOME LIKE '" & Replace(Replace(Trim(strCognome), "'", "''"), "*", "%") & "%')"
        '    End If
        '    If Trim(strNome) <> "" Then
        '        sSQL += " AND (NOME LIKE '" & Replace(Replace(Trim(strNome), "'", "''"), "*", "%") & "%')"
        '    End If
        '    If Trim(strCodiceFiscale) <> "" Then
        '        sSQL += " AND (CODICE_FISCALE LIKE '" & Replace(Trim(strCodiceFiscale), "*", "%") & "%')"
        '    End If
        '    If Trim(strPartitaIVA) <> "" Then
        '        sSQL += " AND (PARTITA_IVA LIKE '" & Replace(Trim(strPartitaIVA), "*", "%") & "%')"
        '    End If
        '    If Len(strNumeroAtto) > 0 Then
        '        sSQL += "AND (NUMERO_ATTO=" & objUtility.CStrToDB(strNumeroAtto) & ")"
        '    End If
        '    If strCOD_TRIBUTO <> "-1" And strCOD_TRIBUTO <> "" Then
        '        sSQL += " AND (COD_TRIBUTO='" & strCOD_TRIBUTO & "')"
        '    End If
        '    sSQL += " ORDER BY NOMINATIVO"

        '    objDSATTIRicercaSemplice = New DataSet
        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)

        '    Log.Debug("getATTIRicercaSemplice::SQL::" & sSQL)

        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)

        '    objDA.Fill(objDSATTIRicercaSemplice, "TP_ATTI_RICERCA_SEMPLICE")


        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()

        '    End If

        '    Return objDSATTIRicercaSemplice

        'End Function

        '*** 201810 - Generazione Massiva Atti ***

        <AutoComplete()>
        Public Function getIDProcedimentoDefinitivoPendenteContribuente(myConnectionString As String, CodEnte As String, ByVal CodContribuente As String, ByVal Anno As String, ByVal CodTipoProcedimento As String, ByVal Tributo As String, ByRef IdProcedimento As Long, ByRef IdProvvedimento As Long, ByRef DataConferma As String) As Boolean
            'ritorno l'ID_PROCEDIMENTO del provvedimento in questione metodo utilizzato per eliminare una elaborazione di pre-accertamento per singolo contribuente questo metodo deve restituire sempre un solo ID_PROCEDIMENTO per contribuente, ente e anno
            Dim myDataView As New DataView
            Dim sSQL As String

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myConnectionString)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetProvvedimentoDefinitivo", "IDENTE", "ANNO", "IDCONTRIBUENTE", "CODTRIBUTO", "CODTIPOPROCEDIMENTO")
                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("IDENTE", CodEnte) _
                                        , ctx.GetParam("ANNO", Anno) _
                                        , ctx.GetParam("IDCONTRIBUENTE", CodContribuente) _
                                        , ctx.GetParam("CODTRIBUTO", Tributo) _
                                        , ctx.GetParam("CODTIPOPROCEDIMENTO", CodTipoProcedimento)
                                    )
                    ctx.Dispose()
                End Using
                DataConferma = ""
                For Each myRow As DataRowView In myDataView
                    IdProcedimento = myRow("ID_PROCEDIMENTO")
                    IdProvvedimento = myRow("ID_PROVVEDIMENTO")
                    If IsDBNull(myRow("DATA_CONFERMA")) Then
                        DataConferma = ""
                    Else
                        DataConferma = myRow("DATA_CONFERMA")
                    End If
                Next
                Return True
            Catch ex As Exception
                Log.Debug("getIDProcedimentoDefinitivoPendenteContribuente.errore::", ex)
                Return False
            Finally
                myDataView.Dispose()
            End Try
        End Function


        Public Function getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet

            Dim sSQL As String
            Dim objDS As New DataSet
            objUtility = New MotoreProvUtility
            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    sSQL = "SELECT *"
                    sSQL += " FROM V_GETICIIMMOBILIDICHIARATIXSTAMPA"
                    sSQL += " WHERE ID_PROCEDIMENTO = " & ID_PROCEDIMENTO
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDS = ctx.GetDataSet(sSQL, "IMMO_DICH_PER_STAMPA_ACCERTAMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getImmobiliDichiaratiPerStampaAccertamenti::si è verificato il seguente errore::", ex)
                objDS = Nothing
            End Try
            Return objDS
        End Function

        Public Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet

            Dim sSQL As String

            Dim NomeDbTarsu As String
            NomeDbTarsu = objUtility.CToStr(objHashTable("NomeDbTarsu"))
            Dim strCOD_CONTRIBUENTE As String = objUtility.CToStr(objHashTable("COD_CONTRIBUENTE"))

            Dim objDS As DataSet = Nothing
            objUtility = New MotoreProvUtility

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    sSQL = "SELECT * "
                    sSQL += " FROM TBLRUOLODICHIARATO INNER JOIN " & NomeDbTarsu & ".dbo.TBLTARIFFE On "
                    sSQL += " TBLRUOLODICHIARATO.IDCATEGORIA = " & NomeDbTarsu & ".dbo.TBLTARIFFE.IDCATEGORIA COLLATE Latin1_General_CI_AS And "
                    sSQL += " TBLRUOLODICHIARATO.IDENTE = " & NomeDbTarsu & ".dbo.TBLTARIFFE.IDENTE COLLATE Latin1_General_CI_AS And "
                    sSQL += " TBLRUOLODICHIARATO.ANNO = " & NomeDbTarsu & ".dbo.TBLTARIFFE.ANNO COLLATE SQL_Latin1_General_CP1_CI_AS INNER JOIN"
                    sSQL += " " & NomeDbTarsu & ".dbo.TBLCATEGORIE On "
                    sSQL += " " & NomeDbTarsu & ".dbo.TBLTARIFFE.IDCATEGORIA = " & NomeDbTarsu & ".dbo.TBLCATEGORIE.CODICE And "
                    sSQL += " " & NomeDbTarsu & ".dbo.TBLTARIFFE.IDENTE = " & NomeDbTarsu & ".dbo.TBLCATEGORIE.IDENTE"
                    sSQL += " WHERE (TBLRUOLODICHIARATO.ID_PROVVEDIMENTO = " & ID_PROVVEDIMENTO & ")"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDS = ctx.GetDataSet(sSQL, "IMMO_DICH_PER_STAMPA_ACCERTAMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getImmobiliDichiaratiPerStampaAccertamentiTARSU::si è verificato il seguente errore::", ex)
                objDS = Nothing
            End Try
            Return objDS

        End Function


        '*** 20140701 - IMU/TARES ***
        Public Function getAddizionaliPerStampaAccertamentiTARSU(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiAddizionali", "IdProvvedimento")
                    myDataSet = ctx.GetDataSet(sSQL, "ADDIZIONALI_PER_STAMPA_ACCERTAMENTO", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getAddizionaliPerStampaAccertamentiTARSU.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getAddizionaliPerStampaAccertamentiTARSU.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        Public Function getVersamentiPerStampaAccertamentiTARSU(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiVersamenti", "IdProvvedimento")
                    myDataSet = ctx.GetDataSet(sSQL, "PAGAMENTI_PER_STAMPA_ACCERTAMENTO", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getVersamentiPerStampaAccertamentiTARSU.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getVersamentiPerStampaAccertamentiTARSU.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '*** ***

        '*** 20140509 - TASI ***
        <AutoComplete()>
        Public Function getImmobiliAccertatiPerStampaAccertamenti(myStringConnection As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM V_GETIMMOBILIDICHACCPERSTAMPA_ICI"
                    sSQL += " WHERE ID_PROCEDIMENTO = " & ID_PROCEDIMENTO
                    sSQL += " ORDER BY ID_LEGAME, FOGLIO, NUMERO, SUBALTERNO"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "IMMO_ACCERTATI_PER_STAMPA_ACCERTAMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamenti.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamenti.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '*** ***

        <AutoComplete()>
        Public Function getImmobiliAccertatiPerStampaAccertamentiTARSU(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM V_GETIMMOBILIDICHACCPERSTAMPA"
                    sSQL += " WHERE ID_PROVVEDIMENTO = " & ID_PROVVEDIMENTO
                    sSQL += " ORDER BY ID_LEGAME, FOGLIO, NUMERO, SUBALTERNO"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "IMMO_ACCERTATI_PER_STAMPA_ACCERTAMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamentiTARSU.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamentiTARSU.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        '*** 20140701 - IMU/TARES ***
        <AutoComplete()>
        Public Function getImmobiliDichAccPerStampaAccertamentiTARSU(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiDichiarato", "IdProvvedimento")
                    myDataSet = ctx.GetDataSet(sSQL, "IMMO_DICH_ACC_PER_STAMPA_ACCERTAMENTO", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getImmobiliDichAccPerStampaAccertamentiTARSU.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getImmobiliDichAccPerStampaAccertamentiTARSU.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '*** ***
        <AutoComplete()>
        Public Function getVersamentiPerStampaLiquidazione(myStringConnection As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT * "
                    sSQL += " FROM VERSAMENTI_ICI_LIQUIDAZIONI "
                    sSQL += " WHERE (ID_PROCEDIMENTO = " & ID_PROCEDIMENTO & ") AND (ID_FASE = 1)"
                    sSQL += " UNION "
                    sSQL += " SELECT * "
                    sSQL += " FROM VERSAMENTI_ICI_LIQUIDAZIONI"
                    sSQL += " WHERE (ID_PROCEDIMENTO = " & ID_PROCEDIMENTO & ") AND (ID_FASE = 2)"
                    sSQL += " AND ID_ORIGINALE NOT IN"
                    sSQL += " (SELECT ID_ORIGINALE "
                    sSQL += " FROM VERSAMENTI_ICI_LIQUIDAZIONI"
                    sSQL += " WHERE (ID_PROCEDIMENTO = " & ID_PROCEDIMENTO & ") AND (ID_FASE = 1))"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "VERSAMENTI_PER_STAMPA")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getVersamentiPerStampaLiquidazione.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getVersamentiPerStampaLiquidazione.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        '*** 20140701 - IMU/TARES *** 
        <AutoComplete()>
        Public Function getProvvedimentoPerStampaLiquidazione(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiProvvedimento", "IdProvvedimento")
                    myDataSet = ctx.GetDataSet(sSQL, "PROVVEDIMENTO_PER_STAMPA", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getProvvedimentoPerStampaLiquidazione.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getProvvedimentoPerStampaLiquidazione.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '*** ***
        <AutoComplete()>
        Public Function getProvvedimentiContribuente(myStringConnection As String, strCOD_ENTE As String, strCOD_CONTRIBUENTE As String, strCOD_TRIBUTO As String, strAnno As String, ID_PROVVEDIMENTO_RETTIFICA As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetProvvedimentiContribuente", "IdEnte", "IdContribuente", "Anno", "Tributo", "IdProvRettifica")
                    myDataSet = ctx.GetDataSet(sSQL, "TP_ATTI_CONTRIBUENTE", ctx.GetParam("IdEnte", strCOD_ENTE) _
                                    , ctx.GetParam("IdContribuente", strCOD_CONTRIBUENTE) _
                                    , ctx.GetParam("Anno", strAnno) _
                                    , ctx.GetParam("Tributo", strCOD_TRIBUTO) _
                                    , ctx.GetParam("IdProvRettifica", ID_PROVVEDIMENTO_RETTIFICA)
                                )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getProvvedimentiContribuente.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getProvvedimentiContribuente.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '*************************************************************************************************
        'RITORNA I DATI IN BASE AL TIPO DI ELABORAZIONE ESEGUITA
        '*************************************************************************************************
        <AutoComplete()>
        Public Function getDatiProvvedimento_PerTipo(myStringConnection As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strPROGRESSIVO_ELABORAZIONE As String = Utility.StringOperation.FormatString(objHashTable("PROGRESSIVO_ELABORAZIONE"))
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT DISTINCT"
                    sSQL += " TAB_PROCEDIMENTI.ANNO, PROVVEDIMENTI.NUMERO_AVVISO,PROVVEDIMENTI.NUMERO_ATTO,TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO,'' AS STATO,"
                    sSQL += " TAB_TIPO_PROVVEDIMENTO.DESCRIZIONE + '  ' + TAB_TRIBUTI.DESCRIZIONE AS TRIBUTO, PROVVEDIMENTI.IMPORTO_TOTALE, "
                    sSQL += " PROVVEDIMENTI.COD_CONTRIBUENTE, PROVVEDIMENTI.ID_PROVVEDIMENTO,"
                    sSQL += " TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO, TAB_TRIBUTI.COD_TRIBUTO, PROVVEDIMENTI.COGNOME + ' ' + PROVVEDIMENTI.NOME AS NOMINATIVO, PROVVEDIMENTI.DATA_ELABORAZIONE,"
                    sSQL += " PROVVEDIMENTI.DATA_CONSEGNA_AVVISO, PROVVEDIMENTI.DATA_NOTIFICA_AVVISO, PROVVEDIMENTI.DATA_IRREPERIBILE, PROVVEDIMENTI.DATA_RETTIFICA_AVVISO, "
                    sSQL += " PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO,PROVVEDIMENTI.DATA_PERVENUTO_IL, "
                    sSQL += " PROVVEDIMENTI.DATA_SCADENZA_QUESTIONARIO, PROVVEDIMENTI.DATA_RIMBORSO, "
                    sSQL += " PROVVEDIMENTI.DATA_SOSPENSIONE_AVVISO_AUTOTUTELA, .PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO, "
                    sSQL += " PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA, PROVVEDIMENTI.DATA_SENTENZA, "
                    sSQL += " PROVVEDIMENTI.DATA_ATTO_DEFINITIVO"
                    sSQL += ", DATA_PAGAMENTO AS DATA_VERSAMENTO_SOLUZIONE_UNICA"
                    sSQL += ", PROVVEDIMENTI.DATA_CONCESSIONE_RATEIZZAZIONE,PROVVEDIMENTI.DATA_CONFERMA,PROVVEDIMENTI.DATA_STAMPA,"
                    sSQL += " PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO_REGIONALE, "
                    sSQL += " PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE, "
                    sSQL += " PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO_CASSAZIONE, "
                    sSQL += " PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE"
                    sSQL += " FROM TAB_PROCEDIMENTI"
                    sSQL += " INNER JOIN PROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " INNER JOIN TAB_TRIBUTI ON TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO"
                    sSQL += " INNER JOIN TAB_TIPO_PROVVEDIMENTO ON TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO"
                    sSQL += " LEFT JOIN ( 	SELECT ID_PROVVEDIMENTO, MIN(DATA_PAGAMENTO) AS DATA_PAGAMENTO, SUM(IMPORTO_PAGATO) AS PAGATO 	FROM V_GETPAGAMENTI 	GROUP BY ID_PROVVEDIMENTO ) P ON PROVVEDIMENTI.ID_PROVVEDIMENTO=P.ID_PROVVEDIMENTO"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO<>0"
                    sSQL += " AND PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "'"
                    sSQL += " AND PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE
                    sSQL += " ORDER BY TAB_PROCEDIMENTI.anno,TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO,PROVVEDIMENTI.COGNOME + ' ' + PROVVEDIMENTI.NOME,PROVVEDIMENTI.DATA_ELABORAZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_ATTI_RICERCA_SEMPLICE")
                    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE"
                    sSQL += " FROM PROVVEDIMENTI"
                    sSQL += " INNER JOINTAB_PROCEDIMENTI ON PROVVEDIMENTI.ID_PROVVEDIMENTO = TAB_PROCEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " INNER JOINTAB_TIPO_PROVVEDIMENTO ON TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND (PROVVEDIMENTI.DATA_RETTIFICA_AVVISO IS  NULL OR PROVVEDIMENTI.DATA_RETTIFICA_AVVISO = '')"
                    sSQL += " AND (PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO IS NULL OR PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO='')"
                    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO <>0"
                    sSQL += " AND PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "' "
                    sSQL += " AND PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_ATTI_RICERCA_SEMPLICE_TOTALE_RETTIFICATO")
                    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE"
                    sSQL += " FROM TAB_PROCEDIMENTI"
                    sSQL += " INNER JOINPROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " INNER JOINTAB_TIPO_PROVVEDIMENTO ON  TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND PROVVEDIMENTI.ID_PROVVEDIMENTO NOT IN (SELECT ID_PROVVEDIMENTO_FIGLIO  FROM TP_PROVVEDIMENTI_RETTIFICATI)"
                    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO <>0"
                    sSQL += " AND PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "' "
                    sSQL += " AND PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_ATTI_RICERCA_SEMPLICE_IMPORTO_TOTALE")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getDatiProvvedimento_PerTipo.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getDatiProvvedimento_PerTipo.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '<AutoComplete()>
        'Public Function getDatiProvvedimento_PerTipo(StringConnectionProvv As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable) As DataSet

        '    Dim objDSDatiProvvedimento_PerTipo As DataSet = Nothing
        '    Dim sSQL As String
        '    Dim objDA As SqlDataAdapter
        '    objUtility = New MotoreProvUtility
        '    Dim strNumeroAvviso As String = ""

        '    Dim strPROGRESSIVO_ELABORAZIONE As String = ""

        '    strPROGRESSIVO_ELABORAZIONE = objUtility.CToStr(objHashTable("PROGRESSIVO_ELABORAZIONE"))

        '    sSQL = ""
        '    'sSQL = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED" & vbCrLf
        '    sSQL += "SELECT DISTINCT" & vbCrLf
        '    sSQL += "TAB_PROCEDIMENTI.ANNO, PROVVEDIMENTI.NUMERO_AVVISO,PROVVEDIMENTI.NUMERO_ATTO,TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO,'' AS STATO," & vbCrLf
        '    sSQL += "TAB_TIPO_PROVVEDIMENTO.DESCRIZIONE + '  ' + TAB_TRIBUTI.DESCRIZIONE AS TRIBUTO, PROVVEDIMENTI.IMPORTO_TOTALE, "
        '    sSQL += "PROVVEDIMENTI.COD_CONTRIBUENTE, PROVVEDIMENTI.ID_PROVVEDIMENTO," & vbCrLf
        '    sSQL += "TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO, TAB_TRIBUTI.COD_TRIBUTO, PROVVEDIMENTI.COGNOME + ' ' + PROVVEDIMENTI.NOME AS NOMINATIVO, PROVVEDIMENTI.DATA_ELABORAZIONE," & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_CONSEGNA_AVVISO, PROVVEDIMENTI.DATA_NOTIFICA_AVVISO, PROVVEDIMENTI.DATA_IRREPERIBILE, PROVVEDIMENTI.DATA_RETTIFICA_AVVISO, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO,PROVVEDIMENTI.DATA_PERVENUTO_IL, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_SCADENZA_QUESTIONARIO, PROVVEDIMENTI.DATA_RIMBORSO, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_SOSPENSIONE_AVVISO_AUTOTUTELA, .PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA, PROVVEDIMENTI.DATA_SENTENZA, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_ATTO_DEFINITIVO"
        '    sSQL += ", DATA_PAGAMENTO AS DATA_VERSAMENTO_SOLUZIONE_UNICA"
        '    sSQL += ", PROVVEDIMENTI.DATA_CONCESSIONE_RATEIZZAZIONE,PROVVEDIMENTI.DATA_CONFERMA,PROVVEDIMENTI.DATA_STAMPA," & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO_REGIONALE, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_PRESENTAZIONE_RICORSO_CASSAZIONE, " & vbCrLf
        '    sSQL += "PROVVEDIMENTI.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE" & vbCrLf

        '    sSQL += " FROM TAB_PROCEDIMENTI"
        '    sSQL += " INNER JOIN PROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO"
        '    sSQL += " INNER JOIN TAB_TRIBUTI ON TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO"
        '    sSQL += " INNER JOIN TAB_TIPO_PROVVEDIMENTO ON TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO"
        '    sSQL += "  LEFT JOIN ( 	SELECT ID_PROVVEDIMENTO, MIN(DATA_PAGAMENTO) AS DATA_PAGAMENTO, SUM(IMPORTO_PAGATO) AS PAGATO 	FROM V_GETPAGAMENTI 	GROUP BY ID_PROVVEDIMENTO ) P ON PROVVEDIMENTI.ID_PROVVEDIMENTO=P.ID_PROVVEDIMENTO"

        '    sSQL += "WHERE"
        '    sSQL += " PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "'"
        '    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO <>0" & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE & vbCrLf
        '    sSQL += "ORDER BY TAB_PROCEDIMENTI.anno,TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO,PROVVEDIMENTI.COGNOME + ' ' + PROVVEDIMENTI.NOME,PROVVEDIMENTI.DATA_ELABORAZIONE" & vbCrLf

        '    objDSDatiProvvedimento_PerTipo = New DataSet
        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)

        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)

        '    objDA.Fill(objDSDatiProvvedimento_PerTipo, "TP_ATTI_RICERCA_SEMPLICE")

        '    sSQL = ""
        '    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE" & vbCrLf
        '    sSQL += "FROM PROVVEDIMENTI INNER JOIN" & vbCrLf
        '    sSQL += "TAB_PROCEDIMENTI ON PROVVEDIMENTI.ID_PROVVEDIMENTO = TAB_PROCEDIMENTI.ID_PROVVEDIMENTO INNER JOIN" & vbCrLf
        '    sSQL += "TAB_TIPO_PROVVEDIMENTO ON" & vbCrLf
        '    sSQL += "TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO" & vbCrLf
        '    sSQL += "WHERE PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "' "
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO <>0" & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "(PROVVEDIMENTI.DATA_RETTIFICA_AVVISO IS  NULL OR PROVVEDIMENTI.DATA_RETTIFICA_AVVISO = '')" & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "(PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO IS NULL OR PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO='')" & vbCrLf
        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)

        '    objDA.Fill(objDSDatiProvvedimento_PerTipo, "TP_ATTI_RICERCA_SEMPLICE_TOTALE_RETTIFICATO")

        '    sSQL = ""
        '    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE" & vbCrLf
        '    sSQL += "FROM TAB_PROCEDIMENTI INNER JOIN" & vbCrLf
        '    sSQL += "PROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO INNER JOIN" & vbCrLf
        '    sSQL += "TAB_TIPO_PROVVEDIMENTO ON " & vbCrLf
        '    sSQL += "TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO = TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO AND TAB_PROCEDIMENTI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO" & vbCrLf
        '    sSQL += "WHERE PROVVEDIMENTI.COD_ENTE='" & strCOD_ENTE & "' "
        '    sSQL += "AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO <>0" & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "PROVVEDIMENTI.PROGRESSIVO_ELABORAZIONE=" & strPROGRESSIVO_ELABORAZIONE & vbCrLf
        '    sSQL += "AND" & vbCrLf
        '    sSQL += "PROVVEDIMENTI.ID_PROVVEDIMENTO NOT IN (SELECT ID_PROVVEDIMENTO_FIGLIO  FROM TP_PROVVEDIMENTI_RETTIFICATI)" & vbCrLf
        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)
        '    objDA.Fill(objDSDatiProvvedimento_PerTipo, "TP_ATTI_RICERCA_SEMPLICE_IMPORTO_TOTALE")
        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()
        '    End If

        '    Return objDSDatiProvvedimento_PerTipo

        'End Function
        ''' <summary>
        ''' Ricerca Avanzata
        ''' </summary>
        ''' <param name="myStringConnection"></param>
        ''' <param name="strCOD_ENTE"></param>
        ''' <param name="objHashTable"></param>
        ''' <param name="strFilterData"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="20130116">aggiungere il parametro di pagato su rateizzo</revision></revisionHistory>
        ''' <revisionHistory><revision date="201511">Funzioni Sovracomunali</revision></revisionHistory>
        <AutoComplete()>
        Public Function GetDatiAttiRicercaAvanzata(myStringConnection As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable, ByVal strFilterData As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strTIPOLOGIAATTO As String = StringOperation.FormatString(objHashTable("TIPOPROVVEDIMENTO"))
            Dim strAmbiente As String = StringOperation.FormatString(objHashTable("AMBIENTE"))
            Dim strAnno As String = StringOperation.FormatString(objHashTable("ANNO"))
            Dim strCOD_TRIBUTO As String = StringOperation.FormatString(objHashTable("CODTRIBUTO"))

            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
                    sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
                    If strAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND (ANNO =" & objUtility.CStrToDB(strAnno) & ")"
                    End If
                    If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND (COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO) & ")"
                    End If
                    If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND (COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO & ")"
                    End If
                    sSQL += strFilterData
                    sSQL += " ORDER BY COGNOME + ' ' + NOME, ANNO DESC, COD_TIPO_PROVVEDIMENTO, DATA_ELABORAZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_ATTI_RICERCA_AVANZATA")
                    '**********************************SELEZIONE PER STAMPA*************************************
                    sSQL = "SELECT DESCRIZIONE_ENTE, COGNOME, NOME, STAMPA_CFPIVA, STAMPA_INDIRIZZO"
                    sSQL += " , STAMPA_NATTO, ANNO, TRIBUTO, STATO"
                    sSQL += " , STAMPA_DATA_ELABORAZIONE, STAMPA_DATA_STAMPA, STAMPA_DATA_CONSEGNA_AVVISO, STAMPA_DATA_NOTIFICA_AVVISO, STAMPA_DATA_ANNULLAMENTO_AVVISO, STAMPA_DATA_PAGAMENTO"
                    sSQL += " , NOTE_GENERALI_ATTO "
                    sSQL += " , STAMPA_MQDICH, STAMPA_MQACC"
                    sSQL += " , STAMPA_IMP_DIFIMP, STAMPA_IMP_SANZ, STAMPA_IMP_SANZNORID, STAMPA_IMP_SANZRID, STAMPA_IMP_INT, STAMPA_IMP_ALTRO, STAMPA_IMP_SPESE"
                    sSQL += " , STAMPA_IMP_ARR, STAMPA_IMP_TOT"
                    sSQL += " , STAMPA_IMP_ARRRID, STAMPA_IMP_TOT_RIDOTTO"
                    sSQL += " , STAMPA_PAGATO, RATEIZZATO"
                    sSQL += " FROM V_GET_RICERCAAVANZATAATTI_STAMPA"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
                    sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
                    If strAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND (ANNO =" & objUtility.CStrToDB(strAnno) & ")"
                    End If
                    If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND (COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO) & ")"
                    End If
                    If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND (COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO & ")"
                    End If
                    sSQL += strFilterData
                    sSQL += " ORDER BY DESCRIZIONE_ENTE, COGNOME, NOME, ANNO DESC, STAMPA_DATA_ELABORAZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_RICERCA_AVANZATA_PER_STAMPA")
                    '*************************************TOTALE AL NETTO DELLE RETTIFICHE E DEGLI ANNULLAMENTI**********************************************
                    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE"
                    sSQL += ", SUM(IMP_TOT_RIDOTTO) AS IMPORTO_TOTALE_RIDOTTO"
                    sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
                    sSQL += " WHERE (DATA_RETTIFICA_AVVISO IS NULL OR DATA_RETTIFICA_AVVISO='')"
                    sSQL += " AND (DATA_ANNULLAMENTO_AVVISO IS NULL OR DATA_ANNULLAMENTO_AVVISO='')"
                    sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
                    sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
                    If strAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND ANNO =" & objUtility.CStrToDB(strAnno)
                    End If
                    If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
                    End If
                    If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
                    End If
                    sSQL += strFilterData
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_TOTALE_RETTIFICATO")
                    '********************************** TOTALE *************************************
                    sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE, SUM(IMP_TOT_RIDOTTO) AS IMPORTO_TOTALE_RIDOTTO "
                    sSQL += " FROM V_GET_RICERCAAVANZATAATTI "
                    sSQL += " LEFT JOIN TP_PROVVEDIMENTI_RETTIFICATI ON V_GET_RICERCAAVANZATAATTI.ID_PROVVEDIMENTO=TP_PROVVEDIMENTI_RETTIFICATI.ID_PROVVEDIMENTO_FIGLIO"
                    sSQL += " WHERE  (TP_PROVVEDIMENTI_RETTIFICATI.ID_PROVVEDIMENTO_FIGLIO IS NULL)"
                    sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
                    sSQL += " AND ('" & strCOD_ENTE & "'='' OR V_GET_RICERCAAVANZATAATTI.COD_ENTE='" & strCOD_ENTE & "')"
                    If strAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND V_GET_RICERCAAVANZATAATTI.ANNO =" & objUtility.CStrToDB(strAnno)
                    End If
                    If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND V_GET_RICERCAAVANZATAATTI.COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
                    End If
                    If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND V_GET_RICERCAAVANZATAATTI.COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
                    End If
                    sSQL += strFilterData
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_TOTALE_GENERALE")
                    '********************* TOTALE CONTRIBUENTI *************************
                    sSQL = "SELECT DISTINCT COD_CONTRIBUENTE"
                    sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
                    sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
                    If strAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND ANNO =" & objUtility.CStrToDB(strAnno)
                    End If
                    If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
                    End If
                    If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
                    End If
                    sSQL += strFilterData
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_TOTALE_CONTRIBUENTI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetDatiAttiRicercaAvanzata.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetDatiAttiRicercaAvanzata.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        '<AutoComplete()>
        'Public Function GetDatiAttiRicercaAvanzata(StringConnectionProvv As String, strCOD_ENTE As String, ByVal objHashTable As Hashtable, ByVal strFilterData As String) As DataSet
        '    Dim objDSAttiRicercaAvanzata As DataSet = Nothing
        '    Dim sSQL As String
        '    Dim objDA As New SqlDataAdapter
        '    objUtility = New MotoreProvUtility
        '    Dim strNumeroAvviso As String = ""
        '    Dim strCodContribuente As String = ""
        '    Dim strAmbiente As String = ""
        '    Dim strCOD_TRIBUTO As String = ""
        '    Dim cmdMyCommand As New SqlClient.SqlCommand

        '    Try
        '        Dim strAnno As String = CType(objHashTable("ANNO"), String)
        '        Dim strTIPOLOGIAATTO As String = CType(objHashTable("TIPOPROVVEDIMENTO"), String)


        '        strCOD_TRIBUTO = CType(objHashTable("CODTRIBUTO"), String)
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        strAmbiente = CType(objHashTable("AMBIENTE"), String)
        '        '*** ***

        '        '*** 20140509 - TASI ***
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionProvv)
        '        cmdMyCommand.CommandTimeout = 0
        '        If cmdMyCommand.Connection.State = ConnectionState.Closed Then
        '            cmdMyCommand.Connection.Open()
        '        End If
        '        '*** 20130116 - aggiungere il parametro di pagato su rateizzo ***
        '        sSQL = "SELECT *"
        '        sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
        '        sSQL += " WHERE 1=1"
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
        '        sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
        '        '*** ***
        '        If strAnno.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (ANNO =" & objUtility.CStrToDB(strAnno) & ")"
        '        End If
        '        If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO) & ")"
        '        End If
        '        If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO & ")"
        '        End If
        '        sSQL += strFilterData
        '        sSQL += " ORDER BY COGNOME + ' ' + NOME, ANNO DESC, COD_TIPO_PROVVEDIMENTO, DATA_ELABORAZIONE"
        '        objDSAttiRicercaAvanzata = New DataSet
        '        Log.Debug("GetDatiAttiRicercaAvanzata::connessione::" & StringConnectionProvv)
        '        Log.Debug("GetDatiAttiRicercaAvanzata::TP_ATTI_RICERCA_AVANZATA ricerca::" & sSQL)

        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.CommandText = sSQL
        '        objDA.SelectCommand = cmdMyCommand
        '        objDA.Fill(objDSAttiRicercaAvanzata, "TP_ATTI_RICERCA_AVANZATA")
        '        '**********************************SELEZIONE PER STAMPA*************************************
        '        sSQL = "SELECT DESCRIZIONE_ENTE, COGNOME, NOME, STAMPA_CFPIVA, STAMPA_INDIRIZZO"
        '        sSQL += " , STAMPA_NATTO, ANNO, TRIBUTO, STATO"
        '        sSQL += " , STAMPA_DATA_ELABORAZIONE, STAMPA_DATA_STAMPA, STAMPA_DATA_CONSEGNA_AVVISO, STAMPA_DATA_NOTIFICA_AVVISO, STAMPA_DATA_ANNULLAMENTO_AVVISO, STAMPA_DATA_PAGAMENTO"
        '        sSQL += " , NOTE_GENERALI_ATTO "
        '        sSQL += " , STAMPA_MQDICH, STAMPA_MQACC"
        '        sSQL += " , STAMPA_IMP_DIFIMP, STAMPA_IMP_SANZ, STAMPA_IMP_SANZNORID, STAMPA_IMP_SANZRID, STAMPA_IMP_INT, STAMPA_IMP_ALTRO, STAMPA_IMP_SPESE"
        '        sSQL += " , STAMPA_IMP_ARR, STAMPA_IMP_TOT"
        '        sSQL += " , STAMPA_IMP_ARRRID, STAMPA_IMP_TOT_RIDOTTO"
        '        sSQL += " , STAMPA_PAGATO, RATEIZZATO"
        '        sSQL += " FROM V_GET_RICERCAAVANZATAATTI_STAMPA"
        '        sSQL += " WHERE 1=1"
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
        '        sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
        '        '*** ***
        '        If strAnno.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (ANNO =" & objUtility.CStrToDB(strAnno) & ")"
        '        End If
        '        If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO) & ")"
        '        End If
        '        If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND (COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO & ")"
        '        End If
        '        sSQL += strFilterData
        '        sSQL += " ORDER BY DESCRIZIONE_ENTE, COGNOME, NOME, ANNO DESC, STAMPA_DATA_ELABORAZIONE"
        '        Log.Debug("GetDatiAttiRicercaAvanzata::TP_RICERCA_AVANZATA_PER_STAMPA ricerca::" & sSQL)
        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.CommandText = sSQL
        '        objDA.SelectCommand = cmdMyCommand
        '        objDA.Fill(objDSAttiRicercaAvanzata, "TP_RICERCA_AVANZATA_PER_STAMPA")
        '        '**********************************FINE SELEZIONE PER STAMPA*************************************

        '        '*************************************TOTALE AL NETTO DELLE RETTIFICHE E DEGLI ANNULLAMENTI**********************************************
        '        sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE"
        '        sSQL += ", SUM(IMP_TOT_RIDOTTO) AS IMPORTO_TOTALE_RIDOTTO"
        '        sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
        '        sSQL += " WHERE (DATA_RETTIFICA_AVVISO IS NULL OR DATA_RETTIFICA_AVVISO='')"
        '        sSQL += " AND (DATA_ANNULLAMENTO_AVVISO IS NULL OR DATA_ANNULLAMENTO_AVVISO='')"
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
        '        sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
        '        '*** ***
        '        If strAnno.CompareTo("-1") <> 0 Then
        '            sSQL += " AND ANNO =" & objUtility.CStrToDB(strAnno)
        '        End If
        '        If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
        '        End If
        '        If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
        '        End If
        '        sSQL += strFilterData
        '        Log.Debug("GetDatiAttiRicercaAvanzata::TP_TOTALE_RETTIFICATO ricerca::" & sSQL)
        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.CommandText = sSQL
        '        objDA.SelectCommand = cmdMyCommand
        '        objDA.Fill(objDSAttiRicercaAvanzata, "TP_TOTALE_RETTIFICATO")
        '        '**********************************FINE TOTALE AL NETTO DELLE RETTIFICHE E DEGLI ANNULLAMENTI*************************************

        '        '********************************** TOTALE *************************************
        '        sSQL = "SELECT SUM(IMPORTO_TOTALE) AS IMPORTO_TOTALE, SUM(IMP_TOT_RIDOTTO) AS IMPORTO_TOTALE_RIDOTTO "
        '        sSQL += " FROM V_GET_RICERCAAVANZATAATTI "
        '        sSQL += " LEFT JOIN TP_PROVVEDIMENTI_RETTIFICATI ON V_GET_RICERCAAVANZATAATTI.ID_PROVVEDIMENTO=TP_PROVVEDIMENTI_RETTIFICATI.ID_PROVVEDIMENTO_FIGLIO"
        '        sSQL += " WHERE  (TP_PROVVEDIMENTI_RETTIFICATI.ID_PROVVEDIMENTO_FIGLIO IS NULL)"
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
        '        sSQL += " AND ('" & strCOD_ENTE & "'='' OR V_GET_RICERCAAVANZATAATTI.COD_ENTE='" & strCOD_ENTE & "')"
        '        '*** ***
        '        If strAnno.CompareTo("-1") <> 0 Then
        '            sSQL += " AND V_GET_RICERCAAVANZATAATTI.ANNO =" & objUtility.CStrToDB(strAnno)
        '        End If
        '        If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND V_GET_RICERCAAVANZATAATTI.COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
        '        End If
        '        If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND V_GET_RICERCAAVANZATAATTI.COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
        '        End If
        '        sSQL += strFilterData
        '        Log.Debug("GetDatiAttiRicercaAvanzata::TP_TOTALE_GENERALE ricerca::" & sSQL)
        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.CommandText = sSQL
        '        objDA.SelectCommand = cmdMyCommand
        '        objDA.Fill(objDSAttiRicercaAvanzata, "TP_TOTALE_GENERALE")
        '        '**********************************FINE TOTALE*************************************

        '        '********************* TOTALE CONTRIBUENTI *************************
        '        sSQL = "SELECT DISTINCT COD_CONTRIBUENTE"
        '        sSQL += " FROM V_GET_RICERCAAVANZATAATTI"
        '        sSQL += " WHERE 1=1"
        '        '*** 201511 - Funzioni Sovracomunali ***
        '        sSQL += " AND ('" & strAmbiente & "'='' OR AMBIENTE='" & strAmbiente & "')"
        '        sSQL += " AND ('" & strCOD_ENTE & "'='' OR COD_ENTE='" & strCOD_ENTE & "')"
        '        '*** ***
        '        If strAnno.CompareTo("-1") <> 0 Then
        '            sSQL += " AND ANNO =" & objUtility.CStrToDB(strAnno)
        '        End If
        '        If strCOD_TRIBUTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND COD_TRIBUTO =" & objUtility.CStrToDB(strCOD_TRIBUTO)
        '        End If
        '        If strTIPOLOGIAATTO.CompareTo("-1") <> 0 Then
        '            sSQL += " AND COD_TIPO_PROVVEDIMENTO =" & strTIPOLOGIAATTO
        '        End If
        '        sSQL += strFilterData
        '        Log.Debug("GetDatiAttiRicercaAvanzata::TP_TOTALE_CONTRIBUENTI ricerca::" & sSQL)
        '        cmdMyCommand.CommandType = CommandType.Text
        '        cmdMyCommand.CommandText = sSQL
        '        objDA.SelectCommand = cmdMyCommand
        '        objDA.Fill(objDSAttiRicercaAvanzata, "TP_TOTALE_CONTRIBUENTI")
        '        '********************* FINE TOTALE CONTRIBUENTI ********************
        '        Return objDSAttiRicercaAvanzata
        '    Catch Err As Exception
        '        Log.Debug("DBOPENgovProvvedimentiSelect::GetDatiAttiRicercaAvanzata::si è verificato il seguente errore::", Err)
        '        Return Nothing
        '    Finally
        '        cmdMyCommand.Dispose()
        '    End Try
        'End Function
#End Region

#Region "Gestione ACCERTAMENTI"
        'Ricavo gli immobili della dichiarazione da accertare 
        <AutoComplete()>
        Public Function getDatiDichiarazioniAccertamenti(myStringConnection As String, IdContribuente As Integer, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT TP_SITUAZIONE_FINALE_ICI.FOGLIO, TP_SITUAZIONE_FINALE_ICI.NUMERO, TP_SITUAZIONE_FINALE_ICI.SUBALTERNO,TP_SITUAZIONE_FINALE_ICI.CATEGORIA, '-1' AS LEGAME, '-1' AS PROGRESSIVO"
                    sSQL += " FROM TAB_PROCEDIMENTI"
                    sSQL += " INNER JOIN PROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " INNER JOIN TP_SITUAZIONE_FINALE_ICI ON TAB_PROCEDIMENTI.ID_PROCEDIMENTO = TP_SITUAZIONE_FINALE_ICI.ID_PROCEDIMENTO"
                    sSQL += " WHERE TP_SITUAZIONE_FINALE_ICI.ANNO = '" & objHashTable("ANNOACCERTAMENTO") & "'"
                    sSQL += " AND PROVVEDIMENTI.COD_CONTRIBUENTE  = " & IdContribuente.ToString
                    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO = 'L'"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TBL")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getDatiDichiarazioniAccertamenti.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getDatiDichiarazioniAccertamenti.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myDBType"></param>
        ''' <param name="myStringConnection"></param>
        ''' <param name="sAnno"></param>
        ''' <param name="strCodEnte"></param>
        ''' <param name="CodContrib"></param>
        ''' <param name="strCodTributo"></param>
        ''' <param name="IdProvvedimento"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function getControlloAccertamento(myDBType As String, myStringConnection As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal CodContrib As Integer, ByVal strCodTributo As String, ByVal IdProvvedimento As Integer) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(myDBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetControlloAccertamento", "IDENTE", "IDCONTRIBUENTE", "IDTRIBUTO", "ANNO", "IDPROVVEDIMENTO")
                    myDataset = ctx.GetDataSet(sSQL, "TBL", ctx.GetParam("IDENTE", strCodEnte) _
                        , ctx.GetParam("IDCONTRIBUENTE", CodContrib) _
                        , ctx.GetParam("IDTRIBUTO", strCodTributo) _
                        , ctx.GetParam("ANNO", sAnno) _
                        , ctx.GetParam("IDPROVVEDIMENTO", IdProvvedimento)
                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getControlloAccertamento.errore::", ex)
            End Try
            Return myDataset
        End Function
        '<AutoComplete()>
        'Public Function getControlloAccertamento(ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByVal objHashTable As Hashtable) As DataSet

        '    Dim sSQL As String

        '    Dim objDS As DataSet = Nothing
        '    Dim objDA As SqlDataAdapter
        '    objUtility = New MotoreProvUtility


        '    sSQL = "SELECT PROVVEDIMENTI.ID_PROVVEDIMENTO, PROVVEDIMENTI.NUMERO_AVVISO, PROVVEDIMENTI.DATA_CONFERMA, PROVVEDIMENTI.DATA_ELABORAZIONE "

        '    sSQL += " FROM PROVVEDIMENTI INNER JOIN "
        '    sSQL += " TAB_PROCEDIMENTI ON PROVVEDIMENTI.ID_PROVVEDIMENTO = TAB_PROCEDIMENTI.ID_PROVVEDIMENTO"

        '    sSQL += " WHERE PROVVEDIMENTI.COD_ENTE='" & strCodEnte & "' "
        '    sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO='A' "
        '    sSQL += " AND PROVVEDIMENTI.COD_CONTRIBUENTE=" & strCodContrib & ""
        '    sSQL += " AND PROVVEDIMENTI.COD_TRIBUTO='" & strCodTributo & "'"

        '    If sAnno.CompareTo("-1") <> 0 Then
        '        sSQL += " AND TAB_PROCEDIMENTI.ANNO = '" & sAnno & "'"
        '    End If
        '    If objHashTable.ContainsKey("ID_PROVVEDIMENTO_RETTIFICA") Then
        '        If objHashTable("ID_PROVVEDIMENTO_RETTIFICA").ToString <> "" And objHashTable("ID_PROVVEDIMENTO_RETTIFICA").ToString <> "0" Then
        '            sSQL += " AND TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = " & objHashTable("ID_PROVVEDIMENTO_RETTIFICA").ToString
        '        End If
        '    End If

        '    objDS = New DataSet
        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)
        '    Log.Debug("getControlloAccertamento::query::" & sSQL)
        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)

        '    objDA.Fill(objDS, "ControlloAccertamento")


        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()

        '    End If

        '    Return objDS


        'End Function

        <AutoComplete()>
        Public Function getControlloPreAccertamento(myStringConnection As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT PROVVEDIMENTI.ID_PROVVEDIMENTO, PROVVEDIMENTI.NUMERO_AVVISO, PROVVEDIMENTI.DATA_CONFERMA"
                    sSQL += " FROM PROVVEDIMENTI"
                    sSQL += " INNER JOIN TAB_PROCEDIMENTI ON PROVVEDIMENTI.ID_PROVVEDIMENTO = TAB_PROCEDIMENTI.ID_PROVVEDIMENTO "
                    sSQL += " WHERE (TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO='L') "
                    sSQL += " AND (PROVVEDIMENTI.COD_ENTE='" & strCodEnte & "')"
                    sSQL += " AND (PROVVEDIMENTI.COD_CONTRIBUENTE=" & strCodContrib & ")"
                    If sAnno.CompareTo("-1") <> 0 Then
                        sSQL += " AND  (TAB_PROCEDIMENTI.ANNO = '" & sAnno & "')"
                    End If
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "ControlloPreAccertamentoAttoDefinitivo")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getControlloPreAccertamento.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getControlloPreAccertamento.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
#End Region

#Region "Gestione CONFIGURAZIONE"
        <AutoComplete()>
        Public Function GetTipoVoci(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strCODTRIBUTO, strCODCAPITOLO, strCODPROVVEDIMENTI, strCODVOCE, strCODMISURA, strCODFASE As String
            Try
                strCODTRIBUTO = StringOperation.FormatString(objHashTable("COD_TRIBUTO"))
                strCODCAPITOLO = StringOperation.FormatString(objHashTable("COD_CAPITOLO"))
                strCODPROVVEDIMENTI = StringOperation.FormatString(objHashTable("COD_PROVVEDIMENTI"))
                strCODVOCE = StringOperation.FormatString(objHashTable("COD_VOCE"))
                strCODMISURA = StringOperation.FormatString(objHashTable("COD_MISURA"))
                strCODFASE = StringOperation.FormatString(objHashTable("COD_FASE"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT DISTINCT"
                    sSQL += " TAB_TRIBUTI.DESCRIZIONE AS DESCTRIBUTO, TAB_CAPITOLI.DESCRIZIONE AS DESCCAPITOLO, "
                    sSQL += " TAB_TIPO_PROVVEDIMENTO.DESCRIZIONE AS DESCTIPOPROVVEDIMENTO, TIPO_VOCI.ID_TIPO_VOCE, TIPO_VOCI.COD_VOCE, TIPO_VOCI.DESCRIZIONE_VOCE, "
                    sSQL += " TIPO_VOCI.DESCRIZIONE_VOCE_ATTRIBUITA, TIPO_MISURA.DESCRIZIONE AS DESCMISURA, "
                    sSQL += " TIPOLOGIE_SANZIONI.COD_VOCE + ' - ' + TIPOLOGIE_SANZIONI.DESCRIZIONE AS VOCE, TIPO_VOCI.COD_TRIBUTO, "
                    sSQL += " TIPO_VOCI.COD_CAPITOLO, TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO, TIPO_VOCI.MISURA, "
                    sSQL += " TAB_FASI.DESCRIZIONE AS DESCRIZIONE_FASE, TAB_FASI.COD_FASE AS COD_FASE"
                    sSQL += " FROM TAB_CAPITOLI"
                    sSQL += " INNER JOIN TAB_TIPO_PROVVEDIMENTO ON TAB_CAPITOLI.COD_TRIBUTO = TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO"
                    sSQL += " INNER JOIN TAB_TRIBUTI ON TAB_CAPITOLI.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO "
                    sSQL += " AND TAB_TIPO_PROVVEDIMENTO.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO"
                    sSQL += " INNER JOIN TIPO_VOCI "
                    sSQL += " ON TAB_CAPITOLI.COD_CAPITOLO = TIPO_VOCI.COD_CAPITOLO AND TAB_CAPITOLI.COD_TRIBUTO = TIPO_VOCI.COD_TRIBUTO "
                    sSQL += " AND TAB_TRIBUTI.COD_TRIBUTO = TIPO_VOCI.COD_TRIBUTO "
                    sSQL += " AND TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO = TIPO_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " INNER JOIN TIPO_MISURA ON TIPO_MISURA.COD_MISURA = TIPO_VOCI.MISURA"
                    sSQL += " LEFT OUTER JOIN TIPOLOGIE_SANZIONI ON TIPO_VOCI.COD_ENTE = TIPOLOGIE_SANZIONI.COD_ENTE AND TAB_TRIBUTI.COD_TRIBUTO = TIPOLOGIE_SANZIONI.COD_TRIBUTO "
                    sSQL += " AND TIPO_VOCI.COD_VOCE = TIPOLOGIE_SANZIONI.COD_VOCE "
                    sSQL += " AND TIPO_VOCI.COD_TRIBUTO = TIPOLOGIE_SANZIONI.COD_TRIBUTO"
                    sSQL += " LEFT OUTER JOIN TAB_FASI ON TIPO_VOCI.FASE = TAB_FASI.COD_FASE"
                    sSQL += " WHERE TIPO_VOCI.COD_ENTE='" & strCODENTE & "'"
                    If strCODTRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " and TIPO_VOCI.COD_TRIBUTO='" & strCODTRIBUTO & "'"
                    End If
                    If strCODCAPITOLO.CompareTo("-1") <> 0 And strCODCAPITOLO.CompareTo("") <> 0 Then
                        sSQL += " and TIPO_VOCI.COD_CAPITOLO='" & strCODCAPITOLO & "'"
                    End If
                    If strCODPROVVEDIMENTI.CompareTo("-1") <> 0 And strCODPROVVEDIMENTI.CompareTo("") <> 0 Then
                        sSQL += " and TAB_TIPO_PROVVEDIMENTO.COD_TIPO_PROVVEDIMENTO='" & strCODPROVVEDIMENTI & "'"
                    End If
                    If strCODVOCE.CompareTo("-1") <> 0 And strCODVOCE.CompareTo("") <> 0 Then
                        sSQL += " and TIPO_VOCI.COD_VOCE='" & strCODVOCE & "'"
                    End If
                    If strCODMISURA.CompareTo("-1") <> 0 And strCODMISURA.CompareTo("") <> 0 Then
                        sSQL += " and TIPO_MISURA.COD_MISURA='" & strCODMISURA & "'"
                    End If
                    If strCODFASE.CompareTo("-1") <> 0 And strCODFASE.CompareTo("") <> 0 Then
                        sSQL += " and TIPO_VOCI.FASE='" & strCODFASE & "'"
                    End If
                    sSQL += " ORDER BY DESCTRIBUTO ,DESCCAPITOLO,DESCTIPOPROVVEDIMENTO,VOCE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_VOCI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetTipoVoci.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetTipoVoci.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Public Function GetTipoInteresse(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strCODTIPOINTERESSE, strDAL, strAL, strTASSO, strTRIBUTO As String
            Try
                strCODTIPOINTERESSE = StringOperation.FormatString(objHashTable("CODTIPOINTERESSE"))
                strDAL = StringOperation.FormatString(objHashTable("DAL"))
                strAL = StringOperation.FormatString(objHashTable("AL"))
                strTASSO = StringOperation.FormatString(objHashTable("TASSO"))
                strTRIBUTO = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT COD_ENTE, TASSI_DI_INTERESSE.COD_TIPO_INTERESSE, TAB_TIPI_INTERESSE.DESCRIZIONE, "
                    sSQL += " DAL, AL, TASSO_ANNUALE, TAB_TIPI_INTERESSE.COD_TRIBUTO, TAB_TRIBUTI.DESCRIZIONE AS DESCRTRIBUTO"
                    sSQL += " FROM TAB_TIPI_INTERESSE"
                    sSQL += " INNER JOIN TASSI_DI_INTERESSE ON TAB_TIPI_INTERESSE.COD_TIPO_INTERESSE = TASSI_DI_INTERESSE.COD_TIPO_INTERESSE"
                    sSQL += " INNER JOIN TAB_TRIBUTI ON TAB_TRIBUTI.COD_TRIBUTO=TAB_TIPI_INTERESSE.COD_TRIBUTO"
                    sSQL += " WHERE COD_ENTE='" & strCODENTE & "'"
                    If strCODTIPOINTERESSE.CompareTo("-1") <> 0 And strCODTIPOINTERESSE.CompareTo("") <> 0 Then
                        sSQL += " AND TASSI_DI_INTERESSE.COD_TIPO_INTERESSE=" & strCODTIPOINTERESSE
                    End If
                    If strDAL.CompareTo("") <> 0 Then
                        sSQL += " AND DAL=" & strDAL
                    End If
                    If strTRIBUTO.CompareTo("") <> 0 And strTRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND TAB_TIPI_INTERESSE.COD_TRIBUTO=" & strTRIBUTO
                    End If
                    sSQL += " ORDER BY ISNULL(AL,'99991231') DESC, DAL DESC"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TASSI_INTERESSE")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetTipoInteresse.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetTipoInteresse.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Public Function GetScadenzaInteressi(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strAnno, strDATA, strNOTE, strTRIBUTO As String

            Try
                strAnno = StringOperation.FormatString(objHashTable("ANNO"))
                strDATA = StringOperation.FormatString(objHashTable("DATA"))
                strNOTE = StringOperation.FormatString(objHashTable("NOTE"))
                strTRIBUTO = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT TAB_SCADENZA_INTERESSI.COD_TRIBUTO, TAB_SCADENZA_INTERESSI.ANNO, TAB_SCADENZA_INTERESSI.DATA_SCADENZA, "
                    sSQL += " TAB_SCADENZA_INTERESSI.NOTE, TAB_TRIBUTI.DESCRIZIONE"
                    sSQL += ", ROW_NUMBER() OVER(ORDER BY TAB_SCADENZA_INTERESSI.COD_TRIBUTO, TAB_SCADENZA_INTERESSI.ANNO) AS PROGRESSIVO"
                    sSQL += " FROM TAB_SCADENZA_INTERESSI INNER JOIN TAB_TRIBUTI ON TAB_SCADENZA_INTERESSI.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO"
                    sSQL += " WHERE COD_ENTE=" & strCODENTE & ""
                    If strAnno.CompareTo("-1") <> 0 And strAnno.CompareTo("") <> 0 Then
                        sSQL += " AND TAB_SCADENZA_INTERESSI.ANNO=" & strAnno
                    End If
                    If strDATA.CompareTo("") <> 0 Then
                        sSQL += " AND DATA_SCADENZA=" & strDATA
                    End If
                    If strTRIBUTO.CompareTo("") <> 0 And strTRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND TAB_SCADENZA_INTERESSI.COD_TRIBUTO=" & strTRIBUTO
                    End If
                    If strNOTE.CompareTo("") <> 0 And strNOTE.CompareTo("-1") <> 0 Then
                        sSQL += " AND TAB_SCADENZA_INTERESSI.NOTE=" & strNOTE
                    End If
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TASSI_INTERESSE")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetScadenzaInteressi.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetScadenzaInteressi.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function


        <AutoComplete()>
        Public Function GetValoriVoci(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODTIPOPROVVEDIMENTO, strCODFASE, strIDTIPOVOCE As String
            Try
                strCODTRIBUTO = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
                strCODCAPITOLO = StringOperation.FormatString(objHashTable("CODCAPITOLO"))
                strCODVOCE = StringOperation.FormatString(objHashTable("CODVOCE"))
                strCODTIPOPROVVEDIMENTO = StringOperation.FormatString(objHashTable("CODTIPOPROVVEDIMENTO"))
                strCODFASE = StringOperation.FormatString(objHashTable("CODFASE"))
                strIDTIPOVOCE = StringOperation.FormatString(objHashTable("IDTIPOVOCE"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT  VALORE_VOCI.ID_VALORE_VOCE, VALORE_VOCI.COD_ENTE, VALORE_VOCI.COD_TRIBUTO, VALORE_VOCI.COD_CAPITOLO, VALORE_VOCI.COD_VOCE, "
                    sSQL += " VALORE_VOCI.ANNO, VALORE_VOCI.COD_TIPO_PROVVEDIMENTO, VALORE_VOCI.ID_TIPO_VOCE, VALORE_VOCI.VALORE, VALORE_VOCI.MINIMO, "
                    sSQL += " VALORE_VOCI.RIDUCIBILE, VALORE_VOCI.CUMULABILE, VALORE_VOCI.COD_TIPO_INTERESSE, VALORE_VOCI.CALCOLATA_SU, VALORE_VOCI.CONDIZIONE, "
                    sSQL += " VALORE_VOCI.PARAMETRO, VALORE_VOCI.BASE_RAFFRONTO, VALORE_VOCI.CONDIZIONE_INTR, VALORE_VOCI.PARAMETRO_INTR, "
                    sSQL += " VALORE_VOCI.BASE_RAFFRONTO_INTR, TAB_BASE_RAFFRONTO.DESC_BASE_RAFFRONTO, TAB_PARAMETRO.DESC_PARAMETRO, "
                    sSQL += " TIPO_BASE_CALCOLO.DESCRIZIONE AS DESC_BASE_CALCOLO, "
                    sSQL += " TAB_BASE_RAFFRONTO_INTRASMISSIBILITA.DESC_BASE_RAFFRONTO AS DESC_BASE_RAFFRONTO_INTR, "
                    sSQL += " TAB_PARAMETRO_INTR.DESC_PARAMETRO AS DESC_PARAMETRO_INTR"
                    sSQL += " FROM TAB_BASE_RAFFRONTO"
                    sSQL += " RIGHT OUTER JOIN TIPO_BASE_CALCOLO "
                    sSQL += " RIGHT OUTER JOIN TAB_PARAMETRO "
                    sSQL += " RIGHT OUTER JOIN VALORE_VOCI ON TAB_PARAMETRO.COD_PARAMETRO = VALORE_VOCI.PARAMETRO"
                    sSQL += " LEFT OUTER JOIN TAB_BASE_RAFFRONTO_INTRASMISSIBILITA ON VALORE_VOCI.BASE_RAFFRONTO_INTR = TAB_BASE_RAFFRONTO_INTRASMISSIBILITA.COD_BASE_RAFFRONTO "
                    sSQL += " LEFT OUTER JOIN TAB_PARAMETRO AS TAB_PARAMETRO_INTR ON VALORE_VOCI.PARAMETRO_INTR = TAB_PARAMETRO_INTR.COD_PARAMETRO "
                    sSQL += " ON TIPO_BASE_CALCOLO.TIPO = VALORE_VOCI.CALCOLATA_SU "
                    sSQL += " ON TAB_BASE_RAFFRONTO.COD_BASE_RAFFRONTO = VALORE_VOCI.BASE_RAFFRONTO"
                    sSQL += " WHERE COD_ENTE='" & strCODENTE & "'"
                    sSQL += " AND ID_TIPO_VOCE= " & strIDTIPOVOCE
                    If strCODTRIBUTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TRIBUTO='" & strCODTRIBUTO & "'"
                    End If
                    If strCODCAPITOLO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_CAPITOLO='" & strCODCAPITOLO & "'"
                    End If
                    If strCODVOCE.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_VOCE='" & strCODVOCE & "'"
                    End If
                    If strCODTIPOPROVVEDIMENTO.CompareTo("-1") <> 0 Then
                        sSQL += " AND COD_TIPO_PROVVEDIMENTO='" & strCODTIPOPROVVEDIMENTO & "'"
                    End If
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "VALORE_VOCI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetValoriVoci.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetValoriVoci.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Public Function GetAnniProvvedimenti(myStringConnection As String, sIdEnte As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing
            Dim sCODTRIBUTO, strANNO As String
            Dim nCODTIPOPROVVEDIMENTO As Integer = 0

            Try
                If StringOperation.FormatString(objHashTable("CODTRIBUTO")) <> "-1" Then
                    sCODTRIBUTO = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
                Else
                    sCODTRIBUTO = ""
                End If
                If IsNumeric(objHashTable("CODTIPOPROVVEDIMENTO")) Then
                    nCODTIPOPROVVEDIMENTO = StringOperation.FormatInt(objHashTable("CODTIPOPROVVEDIMENTO"))
                End If
                strANNO = StringOperation.FormatString(objHashTable("ANNO"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetAnniProvvedimenti", "IDENTE", "CODTIPOPROVVEDIMENTO", "CODTRIBUTO", "ANNO")
                    myDataset = ctx.GetDataSet(sSQL, "VALORE_VOCI", ctx.GetParam("IDENTE", sIdEnte) _
                        , ctx.GetParam("CODTIPOPROVVEDIMENTO", nCODTIPOPROVVEDIMENTO) _
                        , ctx.GetParam("CODTRIBUTO", sCODTRIBUTO) _
                        , ctx.GetParam("ANNO", strANNO)
                    )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetAnniProvvedimenti.errore::", ex)
            End Try
            Return myDataset
        End Function

        <AutoComplete()>
        Function GetMotivazioni(myStringConnection As String, strCODENTE As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT DISTINCT"
                    sSQL += " TAB_TRIBUTI.DESCRIZIONE, TAB_MOTIVAZIONI.ID_MOTIVAZIONE, TAB_MOTIVAZIONI.CODICE_MOTIVAZIONE, "
                    sSQL += " TAB_MOTIVAZIONI.DESCRIZIONE_MOTIVAZIONE, TAB_MOTIVAZIONI.COD_TRIBUTO, TAB_MOTIVAZIONI.COD_VOCE, "
                    sSQL += " TIPOLOGIE_SANZIONI.DESCRIZIONE AS DESCRIZIONE_VOCE "
                    sSQL += " FROM TAB_TRIBUTI "
                    sSQL += " RIGHT OUTER JOIN TIPOLOGIE_SANZIONI "
                    sSQL += " INNER JOIN TAB_MOTIVAZIONI ON TIPOLOGIE_SANZIONI.COD_ENTE = TAB_MOTIVAZIONI.COD_ENTE "
                    sSQL += " AND TIPOLOGIE_SANZIONI.COD_TRIBUTO = TAB_MOTIVAZIONI.COD_TRIBUTO AND TIPOLOGIE_SANZIONI.COD_VOCE = TAB_MOTIVAZIONI.COD_VOCE "
                    sSQL += " ON TAB_TRIBUTI.COD_TRIBUTO = TAB_MOTIVAZIONI.COD_TRIBUTO "
                    sSQL += " WHERE TAB_MOTIVAZIONI.COD_ENTE = '" & strCODENTE & "' "
                    sSQL += " ORDER BY TAB_TRIBUTI.DESCRIZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TAB_MOTIVAZIONI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetMotivazioni.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetMotivazioni.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        <AutoComplete()>
        Function GetTipologieVoci(myStringConnection As String, strCODENTE As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT DISTINCT "
                    sSQL += " TIPOLOGIE_SANZIONI.COD_ENTE, TIPOLOGIE_SANZIONI.COD_TRIBUTO, "
                    sSQL += " TIPOLOGIE_SANZIONI.COD_VOCE, TIPOLOGIE_SANZIONI.DESCRIZIONE, "
                    sSQL += " TAB_TRIBUTI.DESCRIZIONE AS DESCTRIBUTO"
                    sSQL += ", ROW_NUMBER() OVER(ORDER BY TIPOLOGIE_SANZIONI.COD_ENTE, TIPOLOGIE_SANZIONI.COD_TRIBUTO, TIPOLOGIE_SANZIONI.COD_VOCE) AS PROGRESSIVO"
                    sSQL += " FROM TIPOLOGIE_SANZIONI"
                    sSQL += " INNER JOIN TAB_TRIBUTI ON TIPOLOGIE_SANZIONI.COD_TRIBUTO = TAB_TRIBUTI.COD_TRIBUTO"
                    sSQL += " WHERE TIPOLOGIE_SANZIONI.COD_ENTE = '" & strCODENTE & "' "
                    sSQL += " ORDER BY TAB_TRIBUTI.DESCRIZIONE,TIPOLOGIE_SANZIONI.DESCRIZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TAB_SANZIONI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetTipologieVoci.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetTipologieVoci.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myConnectionString"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="14/804/2020">Sono cambiate le regole di applicazione sanzione</revision></revisionHistory>
        <AutoComplete()> Function GetSanzioniRavvedimentoOperoso(myConnectionString As String) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myConnectionString)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetSanzioniRavvedimento")
                    myDataset = ctx.GetDataSet(sSQL, "TBL")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("GetSanzioniRavvedimentoOperoso::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myStringConnection"></param>
        ''' <param name="strCODENTE"></param>
        ''' <param name="objHashTable"></param>
        ''' <param name="strAnnoLav"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="20090410">dipe aggiunto il controllo per aver passato l'anno di lavorazione alla funzione in quanto nel calcolo degli interessi se ci sono più anni viene prelevato sempre e solo l'anno di inizio e non l'anno in lavorazione</revision></revisionHistory>
        <AutoComplete()>
        Function GetGeneraleICI(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable, Optional ByVal strAnnoLav As String = "") As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strANNO As String
            Try
                If strAnnoLav = "" Then
                    strANNO = StringOperation.FormatString(objHashTable("ANNODA"))
                Else
                    strANNO = strAnnoLav
                End If
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM TP_GENERALE_ICI"
                    sSQL += " WHERE COD_ENTE = '" & strCODENTE & "' "
                    sSQL += " AND ANNO = '" & strANNO & "' "
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TAB_GENERALE_ICI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetGeneraleICI.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetGeneraleICI.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myStringConnection"></param>
        ''' <param name="ID_PROVVEDIMENTO"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="20100331">dipe aggiunte data_inizio e data_fine</revision></revisionHistory>
        <AutoComplete()>
        Function GetElencoInteressiPerStampaAccertamenti(myStringConnection As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT DATA_INIZIO,DATA_FINE,TMP.DESCRIZIONE, TMP.TASSO,"
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_SEMESTRI) AS IMPORTO_TOTALE_SEMESTRI,"
                    sSQL += " SUM(TMP.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,"
                    sSQL += " SUM(TMP.N_SEMESTRI_ACCONTO) AS N_SEMESTRI_ACCONTO ,"
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_GIORNI) AS IMPORTO_TOTALE_GIORNI,"
                    sSQL += " SUM(TMP.N_GIORNI_SALDO) AS N_GIORNI_SALDO,"
                    sSQL += " SUM(TMP.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO "
                    sSQL += " FROM ("
                    sSQL += " SELECT DISTINCT ID_IMMOBILE_PROGRESSIVO, DATA_INIZIO,DATA_FINE, TAB_TIPI_INTERESSE.DESCRIZIONE, DETTAGLIO_VOCI_ACCERTAMENTI.TASSO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.IMPORTO) AS IMPORTO_TOTALE_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_SEMESTRI_ACCONTO) AS N_SEMESTRI_ACCONTO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.IMPORTO_GIORNI) AS IMPORTO_TOTALE_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_GIORNI_SALDO) AS N_GIORNI_SALDO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO"
                    sSQL += " FROM DETTAGLIO_VOCI_ACCERTAMENTI"
                    sSQL += " INNER JOIN TIPO_VOCI ON DETTAGLIO_VOCI_ACCERTAMENTI.COD_ENTE=TIPO_VOCI.COD_ENTE AND DETTAGLIO_VOCI_ACCERTAMENTI.COD_VOCE=TIPO_VOCI.COD_VOCE AND DETTAGLIO_VOCI_ACCERTAMENTI.COD_TIPO_PROVVEDIMENTO=TIPO_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " INNER JOIN VALORE_VOCI ON VALORE_VOCI.ID_TIPO_VOCE = TIPO_VOCI.ID_TIPO_VOCE"
                    sSQL += " INNER JOIN TAB_TIPI_INTERESSE ON VALORE_VOCI.COD_TIPO_INTERESSE = TAB_TIPI_INTERESSE.COD_TIPO_INTERESSE"
                    sSQL += " WHERE (TIPO_VOCI.COD_CAPITOLO = '" & OggettoAtto.Capitolo.Interessi & "')"
                    sSQL += " AND (DETTAGLIO_VOCI_ACCERTAMENTI.ID_PROVVEDIMENTO = " & ID_PROVVEDIMENTO & " )"
                    sSQL += " ) TMP"
                    sSQL += " GROUP BY TMP.DESCRIZIONE, TMP.TASSO,DATA_INIZIO,DATA_FINE"
                    sSQL += " ORDER BY DATA_INIZIO,DATA_FINE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "ELENCO_INTERESSI_PROVVEDIMENTO_A")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaAccertamenti.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaAccertamenti.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function


        <AutoComplete()>
        Function GetInteressiTotaliPerStampaAccertamenti(myStringConnection As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet

            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT "
                    sSQL += " SUM(TMP.IMPORTO_ACC_SEMESTRI) AS IMPORTO_ACC_SEMESTRI,"
                    sSQL += " SUM(TMP.IMPORTO_SALDO_SEMESTRI) AS IMPORTO_SALDO_SEMESTRI,"
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_SEMESTRI) AS IMPORTO_TOTALE_SEMESTRI,"
                    sSQL += " SUM(TMP.N_SEMESTRI_ACC) AS N_SEMESTRI_ACC,"
                    sSQL += " SUM(TMP.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,   "
                    sSQL += " SUM(TMP.IMPORTO_ACC_GIORNI) AS IMPORTO_ACC_GIORNI,"
                    sSQL += " SUM(TMP.IMPORTO_SALDO_GIORNI) AS IMPORTO_SALDO_GIORNI,"
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_GIORNI) AS IMPORTO_TOTALE_GIORNI,"
                    sSQL += " SUM(TMP.N_GIORNI_SALDO) AS N_GIORNI_SALDO,"
                    sSQL += " SUM(TMP.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO "
                    sSQL += " FROM ("
                    sSQL += " SELECT DISTINCT "
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.ACCONTO) AS IMPORTO_ACC_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.SALDO) AS IMPORTO_SALDO_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.IMPORTO) AS IMPORTO_TOTALE_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_SEMESTRI_ACCONTO) AS N_SEMESTRI_ACC,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.ACCONTO_GIORNI) AS IMPORTO_ACC_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.SALDO_GIORNI) AS IMPORTO_SALDO_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.IMPORTO_GIORNI) AS IMPORTO_TOTALE_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_GIORNI_SALDO) AS N_GIORNI_SALDO,"
                    sSQL += " (DETTAGLIO_VOCI_ACCERTAMENTI.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO"
                    sSQL += " FROM DETTAGLIO_VOCI_ACCERTAMENTI"
                    sSQL += " INNER JOIN TIPO_VOCI ON DETTAGLIO_VOCI_ACCERTAMENTI.COD_ENTE=TIPO_VOCI.COD_ENTE AND DETTAGLIO_VOCI_ACCERTAMENTI.COD_VOCE=TIPO_VOCI.COD_VOCE AND DETTAGLIO_VOCI_ACCERTAMENTI.COD_TIPO_PROVVEDIMENTO=TIPO_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " INNER JOIN VALORE_VOCI ON VALORE_VOCI.ID_TIPO_VOCE = TIPO_VOCI.ID_TIPO_VOCE"
                    sSQL += " INNER JOIN TAB_TIPI_INTERESSE ON VALORE_VOCI.COD_TIPO_INTERESSE = TAB_TIPI_INTERESSE.COD_TIPO_INTERESSE"
                    sSQL += " WHERE (TIPO_VOCI.COD_CAPITOLO = '" & OggettoAtto.Capitolo.Interessi & "')"
                    sSQL += " AND (DETTAGLIO_VOCI_ACCERTAMENTI.ID_PROVVEDIMENTO = " & ID_PROVVEDIMENTO & " )"
                    sSQL += " ) TMP"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "ELENCO_INTERESSI_PROVVEDIMENTO_A")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaAccertamenti.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaAccertamenti.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function


        <AutoComplete()>
        Function GetElencoInteressiPerStampaLiquidazione(myStringConnection As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM V_GETELENCOINTERESSIPERSTAMPALIQUIDAZIONE"
                    sSQL += " WHERE (COD_CAPITOLO='" & OggettoAtto.Capitolo.Interessi & "')"
                    sSQL += " AND (ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO & ")"
                    sSQL += " ORDER BY DATA_INIZIO,DATA_FINE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "ELENCO_INTERESSI_PROVVEDIMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaLiquidazione.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaLiquidazione.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Function GetInteressiTotaliPerStampaLiquidazione(myStringConnection As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT "
                    sSQL += " SUM(TMP.IMPORTO_ACC_SEMESTRI) AS IMPORTO_ACC_SEMESTRI,  "
                    sSQL += " SUM(TMP.IMPORTO_SALDO_SEMESTRI) AS IMPORTO_SALDO_SEMESTRI,"
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_SEMESTRI) AS IMPORTO_TOTALE_SEMESTRI,  "
                    sSQL += " SUM(TMP.N_SEMESTRI_ACC) AS N_SEMESTRI_ACC,"
                    sSQL += " SUM(TMP.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,   "
                    sSQL += " SUM(TMP.IMPORTO_ACC_GIORNI) AS IMPORTO_ACC_GIORNI,  "
                    sSQL += " SUM(TMP.IMPORTO_SALDO_GIORNI) AS IMPORTO_SALDO_GIORNI,  "
                    sSQL += " SUM(TMP.IMPORTO_TOTALE_GIORNI) AS IMPORTO_TOTALE_GIORNI,  "
                    sSQL += " SUM(TMP.N_GIORNI_SALDO) AS N_GIORNI_SALDO,  "
                    sSQL += " SUM(TMP.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO "
                    sSQL += " FROM ("
                    sSQL += " SELECT DISTINCT "
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.ACCONTO) AS IMPORTO_ACC_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.SALDO) AS IMPORTO_SALDO_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.IMPORTO) AS IMPORTO_TOTALE_SEMESTRI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.N_SEMESTRI_ACCONTO) AS N_SEMESTRI_ACC,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.N_SEMESTRI_SALDO) AS N_SEMESTRI_SALDO,  "
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.ACCONTO_GIORNI) AS IMPORTO_ACC_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.SALDO_GIORNI) AS IMPORTO_SALDO_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.IMPORTO_GIORNI) AS IMPORTO_TOTALE_GIORNI,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.N_GIORNI_SALDO) AS N_GIORNI_SALDO,"
                    sSQL += " (DETTAGLIO_VOCI_LIQUIDAZIONI.N_GIORNI_ACCONTO) AS N_GIORNI_ACCONTO"
                    sSQL += " FROM DETTAGLIO_VOCI_LIQUIDAZIONI"
                    sSQL += " INNER JOIN TIPO_VOCI ON DETTAGLIO_VOCI_LIQUIDAZIONI.COD_ENTE=TIPO_VOCI.COD_ENTE AND DETTAGLIO_VOCI_LIQUIDAZIONI.COD_VOCE=TIPO_VOCI.COD_VOCE AND DETTAGLIO_VOCI_LIQUIDAZIONI.COD_TIPO_PROVVEDIMENTO=TIPO_VOCI.COD_TIPO_PROVVEDIMENTO"
                    sSQL += " INNER JOIN VALORE_VOCI ON VALORE_VOCI.ID_TIPO_VOCE = TIPO_VOCI.ID_TIPO_VOCE"
                    sSQL += " INNER JOIN TAB_TIPI_INTERESSE ON VALORE_VOCI.COD_TIPO_INTERESSE = TAB_TIPI_INTERESSE.COD_TIPO_INTERESSE"
                    sSQL += " WHERE (TIPO_VOCI.COD_CAPITOLO = '" & OggettoAtto.Capitolo.Interessi & "')"
                    sSQL += " AND (DETTAGLIO_VOCI_LIQUIDAZIONI.ID_PROVVEDIMENTO = " & ID_PROVVEDIMENTO & " )"
                    sSQL += " ) TMP"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "VERSAMENTI_PER_STAMPA")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaLiquidazione.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaLiquidazione.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Function GetElencoSanzioniPerStampaAccertamenti(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing
            Dim strANNO, strCODTRIBUTO, strRiducibile As String

            strRiducibile = objHashTable("riducibile")
            strANNO = objHashTable("ANNODA")
            strCODTRIBUTO = objHashTable("CODTRIBUTO")

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiSanzioni", "IDPROVVEDIMENTO", "IDTRIBUTO", "CAPITOLO", "RIDUCIBILE")
                    myDataset = ctx.GetDataSet(sSQL, "ELENCO_SANZIONI_PROVVEDIMENTO", ctx.GetParam("IDPROVVEDIMENTO", ID_PROVVEDIMENTO) _
                                    , ctx.GetParam("IDTRIBUTO", strCODTRIBUTO) _
                                    , ctx.GetParam("CAPITOLO", OggettoAtto.Capitolo.Sanzioni) _
                                    , ctx.GetParam("RIDUCIBILE", strRiducibile)
                                )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("GetElencoSanzioniPerStampaAccertamenti::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function

        <AutoComplete()>
        Function GetElencoSanzioniPerStampaLiquidazione(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Dim strANNO, strRiducibile, strCODTRIBUTO As String

            Try
                strANNO = objHashTable("ANNO")
                strRiducibile = objHashTable("riducibile")
                strCODTRIBUTO = objHashTable("CODTRIBUTO")
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT TV.DESCRIZIONE_VOCE_ATTRIBUITA, SUM(DV.IMPORTO) AS TOT_IMPORTO_SANZ, M.DESCRIZIONE_MOTIVAZIONE"
                    '*** 20170321 - tolto temporeamente perché duplica i record ***
                    'sSQL += " , FOGLIO, NUMERO, CASE WHEN SUBALTERNO<=0 THEN '' ELSE CAST(SUBALTERNO AS VARCHAR)END AS SUBALTERNO"
                    sSQL += " , '' AS FOGLIO, '' AS NUMERO, '' AS SUBALTERNO"
                    sSQL += " FROM DETTAGLIO_VOCI_LIQUIDAZIONI DV"
                    sSQL += " INNER JOIN TIPO_VOCI TV ON DV.COD_ENTE=TV.COD_ENTE AND DV.COD_VOCE=TV.COD_VOCE"
                    sSQL += " INNER JOIN TAB_PROCEDIMENTI P ON DV.ID_PROVVEDIMENTO=P.ID_PROVVEDIMENTO"
                    '*** 20170321 - tolto temporeamente perché duplica i record ***
                    'sSQL += " INNER JOIN TP_IMMOBILI_ACCERTATI_ACCERTAMENTI D ON P.ID_PROCEDIMENTO=D.ID_PROCEDIMENTO"
                    If strRiducibile <> "" Then
                        'sSQL += " INNER JOIN TAB_PROCEDIMENTI ON DV.ID_PROVVEDIMENTO=TAB_PROCEDIMENTI.ID_PROVVEDIMENTO"
                        'sSQL += " AND TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO=TV.COD_TIPO_PROVVEDIMENTO"
                        'sSQL += " AND FASE=2"
                        'sSQL += " INNER JOIN VALORE_VOCI ON VALORE_VOCI.ID_TIPO_VOCE=TV.ID_TIPO_VOCE "
                        'sSQL += " AND VALORE_VOCI.COD_ENTE=TV.COD_ENTE"
                        'sSQL += " AND VALORE_VOCI.COD_VOCE=TV.COD_VOCE"
                        'sSQL += " AND VALORE_VOCI.COD_TIPO_PROVVEDIMENTO=TAB_PROCEDIMENTI.COD_TIPO_PROVVEDIMENTO"
                        'sSQL += " AND VALORE_VOCI.RIDUCIBILE=" & strRiducibile
                        sSQL += " AND P.COD_TIPO_PROVVEDIMENTO=TV.COD_TIPO_PROVVEDIMENTO AND FASE=2"
                        sSQL += " INNER JOIN ("
                        sSQL += " SELECT DISTINCT ID_TIPO_VOCE, COD_ENTE, COD_VOCE, COD_TIPO_PROVVEDIMENTO, RIDUCIBILE"
                        sSQL += " FROM VALORE_VOCI"
                        sSQL += ") VV ON VV.ID_TIPO_VOCE=TV.ID_TIPO_VOCE "
                        sSQL += " AND VV.COD_ENTE=TV.COD_ENTE"
                        sSQL += " AND VV.COD_VOCE=TV.COD_VOCE"
                        sSQL += " AND VV.COD_TIPO_PROVVEDIMENTO=P.COD_TIPO_PROVVEDIMENTO"
                        sSQL += " AND VV.RIDUCIBILE=" & strRiducibile
                    Else
                        'INTRASMISSIBILITÀ
                        sSQL += " AND TV.COD_VOCE=97"
                    End If
                    sSQL += " LEFT JOIN TAB_MOTIVAZIONI M ON TV.COD_ENTE=M.COD_ENTE AND TV.COD_TRIBUTO=M.COD_TRIBUTO AND TV.COD_VOCE=M.COD_VOCE"
                    sSQL += " WHERE (TV.COD_CAPITOLO='" & OggettoAtto.Capitolo.Sanzioni & "') AND (DV.ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO & ")"
                    sSQL += " GROUP BY TV.DESCRIZIONE_VOCE_ATTRIBUITA, M.DESCRIZIONE_MOTIVAZIONE"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "ELENCO_SANZIONI_PROVVEDIMENTO")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.GetElencoSanzioniPerStampaLiquidazione.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.GetElencoSanzioniPerStampaLiquidazione.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
#End Region
#Region "GESTIONE RITORNO DICHIARAZIONI"
        <AutoComplete()>
        Public Function getDati_PROVVEDIMENTI(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT CASE WHEN NOT DATA_PAGAMENTO IS NULL THEN DATA_PAGAMENTO ELSE DATA_VERSAMENTO_SOLUZIONE_UNICA END AS DATA_VERSAMENTO_SOLUZIONE_UNICA"
                    sSQL += ", *"
                    sSQL += " FROM PROVVEDIMENTI"
                    sSQL += " INNER JOIN TAB_PROCEDIMENTI ON PROVVEDIMENTI.ID_PROVVEDIMENTO = TAB_PROCEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " LEFT JOIN ( 	SELECT ID_PROVVEDIMENTO, MIN(DATA_PAGAMENTO) AS DATA_PAGAMENTO, SUM(IMPORTO_PAGATO) AS PAGATO 	FROM V_GETPAGAMENTI 	GROUP BY ID_PROVVEDIMENTO ) P ON PROVVEDIMENTI.ID_PROVVEDIMENTO=P.ID_PROVVEDIMENTO"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND (PROVVEDIMENTI.COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & ")"
                    sSQL += " AND (PROVVEDIMENTI.DATA_NOTIFICA_AVVISO IS NOT NULL)"
                    sSQL += " AND (PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO IS NULL OR PROVVEDIMENTI.DATA_ANNULLAMENTO_AVVISO='')"
                    sSQL += " AND (TAB_PROCEDIMENTI.COD_TIPO_PROCEDIMENTO <> 'Q')"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "PROVVEDIMENTI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getDati_PROVVEDIMENTI.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getDati_PROVVEDIMENTI.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function

        <AutoComplete()>
        Public Function getDati_TP_GENERALE_ICI(myStringConnection As String, strCODENTE As String) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "SELECT *"
                    sSQL += " FROM TP_GENERALE_ICI"
                    sSQL += " WHERE COD_ENTE='" & strCODENTE & "'"
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_GENERALE_ICI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getDati_TP_GENERALE_ICI.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getDati_TP_GENERALE_ICI.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
        <AutoComplete()>
        Public Function getDATE_PROVVEDIMENTI(myStringConnection As String, strCODENTE As String, ByVal strID_PROCEDIMENTO As String, ByVal blnLIQUIDAZIONE As Boolean) As DataSet
            Dim sSQL As String = ""
            Dim myDataSet As New DataSet
            Try
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    If blnLIQUIDAZIONE Then
                        sSQL = "SELECT DISTINCT PROVVEDIMENTI.DATA_CONFERMA"
                    Else
                        sSQL = "SELECT DISTINCT PROVVEDIMENTI.DATA_ATTO_DEFINITIVO"
                    End If
                    sSQL += " FROM TAB_PROCEDIMENTI"
                    sSQL += " INNER JOINPROVVEDIMENTI ON TAB_PROCEDIMENTI.ID_PROVVEDIMENTO = PROVVEDIMENTI.ID_PROVVEDIMENTO"
                    sSQL += " WHERE 1=1"
                    sSQL += " AND TAB_PROCEDIMENTI.ID_PROCEDIMENTO = " & strID_PROCEDIMENTO
                    sSQL += " AND TAB_PROCEDIMENTI.COD_ENTE =" & strCODENTE
                    sSQL += " AND"
                    If blnLIQUIDAZIONE Then
                        sSQL += " (PROVVEDIMENTI.DATA_CONFERMA IS NOT NULL OR PROVVEDIMENTI.DATA_CONFERMA<>'')"
                    Else
                        sSQL += " (PROVVEDIMENTI.DATA_ATTO_DEFINITIVO IS NOT NULL OR PROVVEDIMENTI.DATA_ATTO_DEFINITIVO<>'')"
                    End If
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    myDataSet = ctx.GetDataSet(sSQL, "TP_DATE_PROVVEDIMENTI")
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiSelect.getDATE_PROVVEDIMENTI.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiSelect.getDATE_PROVVEDIMENTI.errore:: " & ex.Message)
            End Try
            Return myDataSet
        End Function
#End Region

        '*** 20130801 - accertamento OSAP ***
        Public Function getVersamentiPerStampaAccertamentiOSAP(myStringConnection As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_GetStampaAccertamentiVersamenti", "IdProvvedimento")
                    myDataset = ctx.GetDataSet(sSQL, "PAGAMENTI_PER_STAMPA_ACCERTAMENTO", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("getVersamentiPerStampaAccertamentiOSAP::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        Public Function getImmobiliDichAccPerStampaAccertamentiOSAP(myStringConnection As String, ByVal TipoRicerca As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Dim sSQL As String
            Dim myDataset As DataSet = Nothing
            Dim myProcedure As String = "prc_GetStampaAccertamentiAccertato"

            Try
                If TipoRicerca = "D" Then
                    myProcedure = "prc_GetStampaAccertamentiDichiarato"
                End If
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, myProcedure, "IdProvvedimento")
                    myDataset = ctx.GetDataSet(sSQL, "IMMO_DICH_PER_STAMPA_ACCERTAMENTO", ctx.GetParam("IdProvvedimento", ID_PROVVEDIMENTO))
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("GetSanzioniRavvedimentoOperoso::si è verificato il seguente errore::", ex)
                myDataset = Nothing
            End Try
            Return myDataset
        End Function
        '*** ***
    End Class
End Namespace