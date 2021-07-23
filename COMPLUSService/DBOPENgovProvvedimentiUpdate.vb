Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports System.IO
'Imports ImexInterface
Imports System.ComponentModel
Imports System.Globalization
Imports COMPlusService
Imports log4net
Imports RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti
Imports RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti
Imports ComPlusInterface
Imports Utility

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la gestione di inserimenti e variazioni sul database
    ''' </summary>
    Public Class DBOPENgovProvvedimentiUpdate
        'Inherits ServicedComponent
        Protected objUtility As New MotoreProvUtility
        Protected objdbIci As DBIci

        Private Shared Log As ILog = LogManager.GetLogger(GetType(DBOPENgovProvvedimentiUpdate))

        Public Sub New()

        End Sub
#Region "SALVATAGGIO DATI GESTIONE ATTI"
        '<AutoComplete()> Public Function SetDATE_PROVVEDIMENTI(StringConnectionProvv As String, ByVal objDSDICHIARATO_ICI_PROVVEDIMENTO As DataSet, ByVal objHashTable As Hashtable) As Boolean

        '    Dim intRetVal As Integer
        '    Dim strSQL As String

        '    objUtility = New MotoreProvUtility

        '    SetDATE_PROVVEDIMENTI = False

        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)

        '    '*********************************************************************************
        '    'UPDATE PROVVEDIMENTI
        '    '*********************************************************************************

        '    strSQL = "UPDATE PROVVEDIMENTI SET "
        '    strSQL += "DATA_CONSEGNA_AVVISO =" & objUtility.CStrToDB(objHashTable("DATACONSEGNAAVVISO")) & vbCrLf
        '    strSQL += ",DATA_NOTIFICA_AVVISO=" & objUtility.CStrToDB(objHashTable("DATANOTIFICAAVVISO")) & vbCrLf
        '    strSQL += ",DATA_IRREPERIBILE=" & objUtility.CStrToDB(objHashTable("DATAIRREPERIBILE"))
        '    strSQL += ",DATA_PERVENUTO_IL=" & objUtility.CStrToDB(objHashTable("DATAPERVENUTOIL")) & vbCrLf

        '    '**** Aggiunta 15/01/2009
        '    strSQL += ",NOTE_GENERALI_ATTO=" & objUtility.CStrToDB(objHashTable("NOTEGENERALIATTO")) & vbCrLf
        '    '**** /Aggiunta 15/01/2009

        '    strSQL += "WHERE" & vbCrLf
        '    strSQL += "ID_PROVVEDIMENTO=" & objUtility.CIdToDB(objHashTable("IDPROVVEDIMENTO"))
        '    Log.Debug("SetDateProvv->" + strSQL)
        '    intRetVal = objDBManager.Execute(strSQL)

        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()

        '    End If


        '    If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '        Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetDATE_PROVVEDIMENTI::DBOPENgovProvvedimentiUpdate")
        '    End If

        '    'Dim objDBOPENgovICIUpdate As New COMPlusOPENgovProvvedimenti.DBICIUpdate
        '    'SetDATE_PROVVEDIMENTI = objDBOPENgovICIUpdate.setIDQUESTIONARIO_TBLTestata(objDSDICHIARATO_ICI_PROVVEDIMENTO, objHashTable)
        '    SetDATE_PROVVEDIMENTI = True

        '    Return SetDATE_PROVVEDIMENTI



        'End Function
        <AutoComplete()>
        Public Function SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByRef NUMERO_ATTO As String) As Boolean
            Dim intRetVal As Integer
            Dim strSQL As String


            objUtility = New MotoreProvUtility

            Dim objDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect

            SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA = False

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    '*** 20112008 Fabi modifica per assegnazione del numero_atto
                    '*** calcolare il numero_atto solo se il campo non è ancora valorizzato
                    If NUMERO_ATTO = "-1" Then
                        'Reperisco il numero atto da TblNumeroAtto
                        NUMERO_ATTO = objDBOPENgovProvvedimentiSelect.getNewNumeroAtto(StringConnectionProvv, objHashTable)
                    End If

                    strSQL = "UPDATE PROVVEDIMENTI SET "
                    strSQL += "DATA_STAMPA =" & objUtility.CStrToDB(DateTime.Now.ToString("yyyyMMdd")) & vbCrLf
                    strSQL += ",DATA_CONFERMA=" & objUtility.CStrToDB(DateTime.Now.ToString("yyyyMMdd")) & vbCrLf
                    If NUMERO_ATTO <> "-1" Then
                        strSQL += ",NUMERO_ATTO=" & objUtility.CStrToDB(NUMERO_ATTO) & vbCrLf
                    End If
                    strSQL += "WHERE" & vbCrLf
                    strSQL += "ID_PROVVEDIMENTO=" & objUtility.CIdToDB(objHashTable("IDPROVVEDIMENTO"))
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using
                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA::DBOPENgovProvvedimentiUpdate")
                End If
                SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA = True
            Catch ex As Exception
                Log.Debug("SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA::si è verificato il seguente errore::", ex)
                SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA = False
            End Try
            Return SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA
        End Function
        <AutoComplete()>
        Public Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv As String, ByVal objHashTable As Hashtable) As Boolean

            Dim intRetVal As Integer
            Dim strSQL As String

            objUtility = New MotoreProvUtility

            SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = False

            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                strSQL = "UPDATE PROVVEDIMENTI SET "
                strSQL += "DATA_ANNULLAMENTO_AVVISO =" & objUtility.CStrToDB(DateTime.Now.ToString("yyyyMMdd")) & vbCrLf
                strSQL += "WHERE" & vbCrLf
                strSQL += "ID_PROVVEDIMENTO=" & objUtility.CIdToDB(objHashTable("IDPROVVEDIMENTO"))
                intRetVal = ctx.ExecuteNonQuery(strSQL)
                ctx.Dispose()
            End Using

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO::DBOPENgovProvvedimentiUpdate")
            End If


            SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = True

            Return SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO



        End Function
        <AutoComplete()>
        Public Function setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal objSELEZIONE_DATASET As DataSet) As Long

            Dim intCount As Integer
            Dim intRetVal As Integer
            Dim intRecordAffect As Integer
            Dim intTOT_RECORD As Integer

            objUtility = New MotoreProvUtility
            Dim blnUPDATE As Boolean = False

            Dim strSQL_UPDATE As String = "UPDATE PROVVEDIMENTI SET " & vbCrLf

            Dim strSQL_DELETE As String = "UPDATE PROVVEDIMENTI SET " & vbCrLf
            Dim strDATA_AGGIORNAMENTO As String

            For intCount = 0 To objSELEZIONE_DATASET.Tables(0).Rows.Count - 1

                strSQL_UPDATE = "UPDATE PROVVEDIMENTI SET " & vbCrLf

                Dim rowPROVVEDIMENTI As DataRow = objSELEZIONE_DATASET.Tables(0).Rows(intCount)

                If objUtility.CToBool(objHashTable("AGGIORNA_A")) Then
                    blnUPDATE = True
                End If

                If blnUPDATE Then
                    '*******************************************************************
                    'UPDATE
                    '*******************************************************************
                    '*******************************************************************
                    strDATA_AGGIORNAMENTO = objUtility.CToStr(objUtility.GiraData(objHashTable("VALORE_DATA_AGGIORNAMENTO")))
                    'VERIFICA DEL TIPO DI DATA DA AGGIORNARE

                    If objUtility.CToBool(objHashTable("DATACONSEGNA")) Then

                        strSQL_UPDATE = strSQL_UPDATE & "DATA_CONSEGNA_AVVISO=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "WHERE" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_STAMPA IS NOT NULL AND DATA_STAMPA<>'')" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_STAMPA <=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & ")" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO >=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & " OR (DATA_NOTIFICA_AVVISO IS NULL OR DATA_NOTIFICA_AVVISO =''))" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_CONSEGNA_AVVISO IS NULL OR DATA_CONSEGNA_AVVISO ='')" & vbCrLf
                    End If

                    If objUtility.CToBool(objHashTable("DATANOTIFICA")) Then

                        strSQL_UPDATE = strSQL_UPDATE & "DATA_NOTIFICA_AVVISO=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "WHERE" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_CONSEGNA_AVVISO IS NOT NULL AND DATA_CONSEGNA_AVVISO <>'')" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_CONSEGNA_AVVISO <=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & ")" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_RETTIFICA_AVVISO >=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & " OR (DATA_RETTIFICA_AVVISO IS NULL OR DATA_RETTIFICA_AVVISO =''))" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_ANNULLAMENTO_AVVISO >=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & " OR (DATA_ANNULLAMENTO_AVVISO IS NULL OR DATA_ANNULLAMENTO_AVVISO =''))" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO IS NULL OR DATA_NOTIFICA_AVVISO ='')" & vbCrLf
                    End If
                    If objUtility.CToBool(objHashTable("DATAPERVENUTO")) Then

                        strSQL_UPDATE = strSQL_UPDATE & "DATA_PERVENUTO_IL=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "WHERE" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO IS NOT NULL AND DATA_NOTIFICA_AVVISO <>'')" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO <=" & objUtility.CStrToDB(strDATA_AGGIORNAMENTO) & ")" & vbCrLf

                    End If


                Else
                    '*******************************************************************
                    'ELIMINA
                    '*******************************************************************
                    'VERIFICA DEL TIPO DI DATA DA ELIMINARE
                    If objUtility.CToBool(objHashTable("DATACONSEGNA")) Then

                        strSQL_UPDATE = strSQL_UPDATE & "DATA_CONSEGNA_AVVISO=" & objUtility.CStrToDB("") & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "WHERE" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO IS NULL)" & vbCrLf

                    End If
                    If objUtility.CToBool(objHashTable("DATANOTIFICA")) Then

                        strSQL_UPDATE = strSQL_UPDATE & "DATA_NOTIFICA_AVVISO=" & objUtility.CStrToDB("") & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "WHERE" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_RETTIFICA_AVVISO IS NULL OR DATA_RETTIFICA_AVVISO='')" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_ANNULLAMENTO_AVVISO IS  NULL OR DATA_ANNULLAMENTO_AVVISO='')" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                        strSQL_UPDATE = strSQL_UPDATE & "(DATA_NOTIFICA_AVVISO IS NULL OR DATA_NOTIFICA_AVVISO ='')" & vbCrLf

                    End If


                End If

                strSQL_UPDATE = strSQL_UPDATE & "AND" & vbCrLf
                strSQL_UPDATE = strSQL_UPDATE & "ID_PROVVEDIMENTO= " & objUtility.CIdToDB(rowPROVVEDIMENTI("ID_PROVVEDIMENTO")) & vbCrLf

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    intRetVal = ctx.ExecuteNonQuery(strSQL_UPDATE)
                    ctx.Dispose()
                End Using

                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::setDATE_PROVVEDIMENTI_MASSIVA::DBOPENgovProvvedimentiUpdate")
                End If

                intTOT_RECORD = intTOT_RECORD + intRetVal
                strSQL_UPDATE = ""
            Next

            Return intTOT_RECORD


        End Function
#End Region

#Region "SAVE ELABORAZIONE LIQUIDAZIONI"
        ''' <summary>
        '''elimina un elaborazione di accertamento per singolo contribuente e per anno
        '''Queste sono le tabelle in gioco
        '''-----------accertamento-----------
        '''PROVVEDIMENTI
        '''TAB_PROCEDIMENTI
        '''DICHIARATO_ICI_ACCERTAMENTI
        '''tp_Immobili_ACCERTAMENTI
        '''tp_contitolari_ACCERTAMENTI
        '''tp_immobili_Accertati_ACCERTAMENTI
        '''tp_Legame_Accertamento
        '''DETTAGLIO_VOCI_ACCERTAMENTI
        '''TP_SITUAZIONE_FINALE_ICI
        '''-----------accertamento-----------
        '''-----------fase 2 pre accertamento-----------
        '''VERSAMENTI_ICI_LIQUIDAZIONI()
        '''DICHIARATO_ICI_LIQUIDAZIONI()
        '''tp_Immobili_LIQUIDAZIONI()
        '''tp_contitolari_LIQUIDAZIONI()
        '''DETTAGLIO_VOCI_LIQUIDAZIONI()
        '''-----------fase 2 pre accertamento-----------  
        ''' </summary>
        ''' <param name="objHashTable"></param>
        ''' <param name="ID_PROCEDIMENTO"></param>
        ''' <param name="ID_PROVVEDIMENTO"></param>
        ''' <returns></returns>
        <AutoComplete()>
        Public Function DeleteProvvedimentiAccertamento(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long, ByVal ID_PROVVEDIMENTO As Long) As Boolean

            Dim intRetVal As Integer
            Dim strSQL As String = ""

            Try
                'If blnRetVal = True Then

                strSQL += " DELETE FROM DICHIARATO_ICI_LIQUIDAZIONI "
                strSQL += " WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                'cancellando PROVVEDIMENTI, cancello a cascata (relazioni) anche tutte le tabelle degli accertamenti
                strSQL += " DELETE FROM PROVVEDIMENTI "
                strSQL += " WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE FROM DETTAGLIO_VOCI_LIQUIDAZIONI "
                strSQL += " WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE FROM VERSAMENTI_ICI_LIQUIDAZIONI "
                strSQL += " WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE FROM tp_Immobili_LIQUIDAZIONI "
                strSQL += " WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE FROM tp_contitolari_LIQUIDAZIONI "
                strSQL += " WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE FROM TP_SITUAZIONE_FINALE_ICI "
                strSQL += " WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                strSQL = ""
                strSQL += " DELETE from TP_PROVVEDIMENTI_RETTIFICATI "
                strSQL += " WHERE ID_PROVVEDIMENTO_FIGLIO=" & ID_PROVVEDIMENTO & ";"

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    strSQL = ctx.GetSQL(DBModel.TypeQuery.View, strSQL)
                    intRetVal = ctx.ExecuteNonQuery(strSQL)
                    ctx.Dispose()
                End Using

                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    'Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DeleteProvvedimentiLiquidazioni::DBOPENgovProvvedimentiUpdate")
                    Return False
                End If

                Return True
                'End If
            Catch ex As Exception
                Log.Debug("DeleteProvvedimentiAccertamento::si è verificato il seguente errore::", ex)
                Return False
            End Try
        End Function
        Private Function ImportoArrotondato(ByVal ImportoEuro As Double) As Long

            'Funzione che in base alla nuova finanziaria prevede
            'gli importi arrotondati
            'x= importo da arrotondare + 0.5
            'importo arrotondato = parte intera di x

            Dim X As Double
            Dim ImportoOut As Long

            If ImportoEuro > 0 Then

                X = ImportoEuro + 0.5
                If InStr(X, ",") > 0 Then
                    ImportoOut = Left(X, InStr(X, ",") - 1)
                Else
                    ImportoOut = X
                End If

            ElseIf ImportoEuro < 0 Then

                X = ImportoEuro - 0.5
                If InStr(X, ",") > 0 Then
                    ImportoOut = Left(X, InStr(X, ",") - 1)
                Else
                    ImportoOut = X
                End If

            End If

            Return ImportoOut

        End Function
        Public Function SetPROVVEDIMENTIRETTIFICATI(myConnectionString As String, ByVal intCODContribuente As Long, ByVal strCODTributo As String, ByVal strCODEnte As String, ByVal lngIDProvvedimento As Long, ByVal ID_PROVVEDIMENTO_OLD As Long, ByVal DATA_RETTIFICA As String, ByVal blnEffettuaInsert As Boolean, ByVal DATA_ANNULLAMENTO As String) As Long
            Dim cmdMyCommand As New SqlCommand
            Dim RetValue As Integer

            Try
                Dim strSQL As String

                cmdMyCommand.Connection = New SqlConnection(myConnectionString)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                If blnEffettuaInsert = True Then
                    'blnEffettuaInsert=true-->eseguo insert del nuovo provvedimento e update data rettifica del provvedimento rettificato 
                    'blnEffettuaInsert=false-->eseguo solo update data rettifica e note del provvedimento rettificato 
                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    cmdMyCommand.CommandText = "prc_TP_PROVVEDIMENTI_RETTIFICATI_IU"
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.AddWithValue("@ID_PROVVEDIMENTO", ID_PROVVEDIMENTO_OLD)
                    cmdMyCommand.Parameters.AddWithValue("@COD_CONTRIBUENTE", intCODContribuente)
                    cmdMyCommand.Parameters.AddWithValue("@COD_ENTE", strCODEnte)
                    cmdMyCommand.Parameters.AddWithValue("@COD_TRIBUTO", strCODTributo)
                    cmdMyCommand.Parameters.AddWithValue("@ID_PROVVEDIMENTO_FIGLIO", lngIDProvvedimento)
                    Log.Debug("setprovrett->" + Costanti.LogQuery(cmdMyCommand))
                    RetValue = cmdMyCommand.ExecuteNonQuery
                End If
                cmdMyCommand.CommandType = CommandType.Text
                strSQL = "UPDATE PROVVEDIMENTI SET "
                If DATA_RETTIFICA <> "" Then
                    strSQL += " DATA_RETTIFICA_AVVISO=" & objUtility.CStrToDB(DATA_RETTIFICA)
                End If
                If DATA_ANNULLAMENTO <> "" Then
                    strSQL += " DATA_ANNULLAMENTO_AVVISO=" & objUtility.CStrToDB(DATA_ANNULLAMENTO)
                End If
                'strSQL += ", NOTE_GENERALI_ATTO= '" & COSTANTValue.Costanti.NOTE_PRA_ACC_IMPORTO_INFERIORE_A_SOGLIA & "'"
                strSQL += " WHERE ID_PROVVEDIMENTO=" & objUtility.CIdToDB(ID_PROVVEDIMENTO_OLD) & ""
                cmdMyCommand.CommandText = strSQL
                Log.Debug("setprovret..->" + Costanti.LogQuery(cmdMyCommand))
                RetValue = cmdMyCommand.ExecuteNonQuery
                Return RetValue
            Catch ex As Exception
                Log.Debug("TARSU_SetProcedimento::si è verificato il seguente errore::", ex)
                Return 0
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function

        'Public Function SetTPCALCOLOFINALEICI(StringConnectionProvv As String, StringConnectionICI As String, ByVal objDSfinale() As objSituazioneFinale, ByVal objHashTable As Hashtable, ByVal lngIDelaborazione As Long, ByVal blnCalcolaArrotondamento As Boolean) As Long
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim intRetVal, lngID As Long
        '    Dim DBselect As New DBOPENgovProvvedimentiselect

        '    Try
        '        lngID = DBselect.getNewIDdbICI(StringConnectionProvv, "TP_CALCOLO_FINALE_ICI", objHashTable)
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionICI)
        '        If cmdMyCommand.Connection.State = ConnectionState.Closed Then
        '            cmdMyCommand.Connection.Open()
        '        End If
        '        'Valorizzo i parameters:
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = lngID
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdElaborazione", SqlDbType.Int)).Value = lngIDelaborazione
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdEnte", SqlDbType.NVarChar)).Value = objDSfinale(0).IdEnte
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Anno", SqlDbType.NVarChar)).Value = objDSfinale(0).Anno
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Tributo", SqlDbType.NVarChar)).Value = objDSfinale(0).Tributo
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IdContribuente", SqlDbType.Int)).Value = objDSfinale(0).IdContribuente
        '        cmdMyCommand.CommandText = "prc_TP_CALCOLO_FINALE_ICI_IU"
        '        Log.Debug("Set_TP_CALCOLO_FINALE_ICI_dbICI::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        intRetVal = cmdMyCommand.ExecuteNonQuery
        '        Log.Debug("Set_TP_CALCOLO_FINALE_ICI_dbICI::retval query::" + intRetVal.ToString())
        '        If intRetVal <= COSTANTValue.CostantiProv.VALUE_NUMBER_ZERO Then
        '            Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::Set_TP_CALCOLO_FINALE_ICI_dbICI::DBOPENgovProvvedimentiUpdate")
        '            Log.Error("Inserimento in TP_CALCOLO_FINALE_ICI fallito")
        '            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::Set_TP_CALCOLO_FINALE_ICI_dbICI::DBOPENgovProvvedimentiUpdate")
        '        End If
        '        Return intRetVal
        '    Catch ex As Exception
        '        Log.Debug("Application::COMPlusOPENgovProvvedimenti::Function::Set_TP_CALCOLO_FINALE_ICI_dbICI::" & ex.ToString)
        '    Finally
        '        'DBselect.Dispose()
        '        cmdMyCommand.Dispose()
        '        cmdMyCommand.Connection.Close()
        '    End Try
        'End Function
        Public Function Delete_SITUAZIONE_FINALE_ICI_dbICI(myStringConnection As String, ByVal ANNO As String, ByVal Tributo As String, ByVal ENTE As String, ByVal CONTRIB As Long) As Integer
            Dim sSQL As String
            Dim intRetVal As Integer = -1

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_TP_SITUAZIONE_FINALE_ICI_D", "CodEnte", "Anno", "Tributo", "CodContribuente")
                    intRetVal = ctx.ExecuteNonQuery(sSQL, ctx.GetParam("CodEnte", ENTE) _
                                    , ctx.GetParam("Anno", ANNO) _
                                    , ctx.GetParam("Tributo", Tributo) _
                                    , ctx.GetParam("CodContribuente", CONTRIB)
                                )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("Delete_SITUAZIONE_FINALE_ICI_dbICI::si è verificato il seguente errore::", ex)
                intRetVal = -1
            End Try
            Return intRetVal
        End Function
        Public Function Delete_TP_CALCOLO_FINALE_ICI_dbICI(myStringConnection As String, ByVal ANNO As String, ByVal Tributo As String, ByVal ENTE As String, ByVal CONTRIB As Long, ByVal objHashTable As Hashtable) As Long
            Dim sSQL As String
            Dim intRetVal As Long

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_TP_CALCOLO_FINALE_ICI_D", "CodEnte", "Anno", "Tributo", "CodContribuente")
                    intRetVal = ctx.ExecuteNonQuery(sSQL, ctx.GetParam("CodEnte", ENTE) _
                                    , ctx.GetParam("Anno", ANNO) _
                                    , ctx.GetParam("Tributo", Tributo) _
                                    , ctx.GetParam("CodContribuente", CONTRIB)
                                )
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("Delete_TP_CALCOLO_FINALE_ICI_dbICI::si è verificato il seguente errore::", ex)
                intRetVal = -1
            End Try
            Return intRetVal
        End Function
        Public Function SetSituazioneFinaleICI(ByVal objICI() As objSituazioneFinale, ByVal lngIDProcedimento As Long, myStringConnection As String) As Long
            Dim cmdMyCommand As New SqlCommand
            Dim intRetVal As Long

            Try
                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                For Each mySituazioneFinale As objSituazioneFinale In objICI
                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    'Valorizzo i parameters:
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_SITUAZIONE_FINALE", SqlDbType.BigInt)).Value = mySituazioneFinale.Id
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDPROCEDIMENTO", SqlDbType.Int)).Value = mySituazioneFinale.IdProcedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.Int)).Value = mySituazioneFinale.IdLegame
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Anno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = mySituazioneFinale.IdEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PROVENIENZA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Provenienza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CARATTERISTICA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Caratteristica
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@INDIRIZZO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Via & " " & mySituazioneFinale.NCivico
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEZIONE", SqlDbType.NVarChar)).Value = mySituazioneFinale.Sezione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FOGLIO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Foglio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Numero
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SUBALTERNO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Subalterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CATEGORIA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Categoria
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CLASSE", SqlDbType.NVarChar)).Value = mySituazioneFinale.Classe
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PROTOCOLLO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Protocollo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_STORICO", SqlDbType.Bit)).Value = mySituazioneFinale.FlagStorico
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VALORE", SqlDbType.Float)).Value = mySituazioneFinale.Valore
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VALORE_REALE", SqlDbType.Float)).Value = mySituazioneFinale.ValoreReale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_PROVVISORIO", SqlDbType.Bit)).Value = mySituazioneFinale.FlagProvvisorio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PERC_POSSESSO", SqlDbType.Float)).Value = mySituazioneFinale.PercPossesso
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESI_POSSESSO", SqlDbType.NVarChar)).Value = mySituazioneFinale.MesiPossesso
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESI_ESCL_ESENZIONE", SqlDbType.NVarChar)).Value = mySituazioneFinale.MesiEsenzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESI_RIDUZIONE", SqlDbType.NVarChar)).Value = mySituazioneFinale.MesiRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_DETRAZIONE", SqlDbType.Float)).Value = mySituazioneFinale.ImpDetrazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_POSSEDUTO", SqlDbType.Bit)).Value = mySituazioneFinale.FlagPosseduto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_ESENTE", SqlDbType.Bit)).Value = mySituazioneFinale.FlagEsente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_RIDUZIONE", SqlDbType.Bit)).Value = mySituazioneFinale.FlagRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_PRINCIPALE", SqlDbType.Int)).Value = mySituazioneFinale.FlagPrincipale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_CONTRIBUENTE", SqlDbType.Int)).Value = mySituazioneFinale.IdContribuente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_IMMOBILE_PERTINENZA", SqlDbType.Int)).Value = mySituazioneFinale.IdImmobilePertinenza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_IMMOBILE", SqlDbType.Int)).Value = mySituazioneFinale.IdImmobile
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DAL", SqlDbType.VarChar)).Value = mySituazioneFinale.Dal.ToString("yyyyMMdd")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AL", SqlDbType.VarChar)).Value = mySituazioneFinale.Al.ToString("yyyyMMdd")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_MESI_ACCONTO", SqlDbType.Int)).Value = mySituazioneFinale.AccMesi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_MESI_TOTALI", SqlDbType.Int)).Value = mySituazioneFinale.Mesi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_UTILIZZATORI", SqlDbType.Int)).Value = mySituazioneFinale.NUtilizzatori
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPO_RENDITA", SqlDbType.NVarChar)).Value = mySituazioneFinale.TipoRendita
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_SENZA_DETRAZIONE", SqlDbType.Float)).Value = mySituazioneFinale.AccSenzaDetrazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_APPLICATA", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneApplicata
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_ACCONTO", SqlDbType.Float)).Value = mySituazioneFinale.AccDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneResidua
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_SENZA_DETRAZIONE", SqlDbType.Float)).Value = mySituazioneFinale.TotSenzaDetrazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_APPLICATA", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneApplicata
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DOVUTA", SqlDbType.Float)).Value = mySituazioneFinale.TotDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneResidua
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_SALDO", SqlDbType.Float)).Value = mySituazioneFinale.SalDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_DETRAZIONE_SALDO", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneApplicata
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_SENZA_DETRAZIONE", SqlDbType.Float)).Value = mySituazioneFinale.SalSenzaDetrazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_DETRAZIONE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneResidua
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@RIDUZIONE", SqlDbType.Bit)).Value = mySituazioneFinale.FlagRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESE_INIZIO", SqlDbType.Int)).Value = mySituazioneFinale.MeseInizio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_SCADENZA", SqlDbType.NVarChar)).Value = mySituazioneFinale.DataScadenza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPO_OPERAZIONE", SqlDbType.VarChar)).Value = mySituazioneFinale.TipoOperazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@RITORNATA", SqlDbType.Bit)).Value = False
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_ELABORAZIONE", SqlDbType.NVarChar)).Value = Date.Now.ToString("yyyyMMdd")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@idtestata", SqlDbType.Int)).Value = DBNull.Value
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PROGRESSIVO_ELABORAZIONE", SqlDbType.Int)).Value = -1
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_FABBRICATI", SqlDbType.Int)).Value = 0
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_STATALE_APPLICATA", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_STATALE_CALCOLATA", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_STATALE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_SALDO_DETRAZIONE_STATALE_APPLICATA", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_SALDO_DETRAZIONE_STATALE_CALCOLATA", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_SALDO_DETRAZIONE_STATALE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_STATALE_APPLICATA", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_STATALE_CALCOLATA", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_STATALE_RESIDUA", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@consistenza", SqlDbType.Float)).Value = mySituazioneFinale.Consistenza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@AbitazionePrincipaleAttuale", SqlDbType.Int)).Value = mySituazioneFinale.AbitazionePrincipaleAttuale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COLTIVATOREDIRETTO", SqlDbType.Bit)).Value = mySituazioneFinale.IsColtivatoreDiretto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMEROFIGLI", SqlDbType.Int)).Value = mySituazioneFinale.NumeroFigli
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_VALORE_ALIQUOTA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.AliquotaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_ACCONTO_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.AccDovutoStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_APPLICATA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_ACCONTO_DETRAZIONE_RESIDUA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.AccDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DOVUTA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.TotDovutoStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_APPLICATA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_TOTALE_DETRAZIONE_RESIDUA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.TotDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_SALDO_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.SalDovutoStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_DETRAZIONE_SALDO_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneApplicataStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_DOVUTA_DETRAZIONE_RESIDUA_STATALE", SqlDbType.Float)).Value = mySituazioneFinale.SalDetrazioneResiduaStatale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_VALORE_ALIQUOTA", SqlDbType.Float)).Value = mySituazioneFinale.Aliquota
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PERCENTCARICOFIGLI", SqlDbType.Float)).Value = mySituazioneFinale.PercentCaricoFigli
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_ALIQUOTA", SqlDbType.Int)).Value = mySituazioneFinale.IdAliquota
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODTRIBUTO", SqlDbType.VarChar)).Value = mySituazioneFinale.Tributo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOUTILIZZO", SqlDbType.Int)).Value = mySituazioneFinale.IdTipoUtilizzo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOPOSSESSO", SqlDbType.Int)).Value = mySituazioneFinale.IdTipoPossesso
                    '*** 20150430 - TASI Inquilino ***
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOTASI", SqlDbType.NVarChar)).Value = mySituazioneFinale.TipoTasi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTECALCOLO", SqlDbType.Int)).Value = mySituazioneFinale.IdContribuenteCalcolo
                    '*** ***
                    cmdMyCommand.CommandText = "prc_TP_SITUAZIONE_FINALE_ICI_IU"
                    Log.Debug("Set_SITUAZIONE_FINALE_ICI_dbICI.query->" & cmdMyCommand.CommandText & " " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
                    intRetVal = cmdMyCommand.ExecuteNonQuery
                    '*** ***
                    Log.Debug("Set_SITUAZIONE_FINALE_ICI_dbICI:: query::prc_TP_SITUAZIONE_FINALE_ICI_IU::esito::" + intRetVal.ToString())
                    If intRetVal = -1 Then
                        Log.Error("Application::COMPlusOPENgovProvvedimenti::Function::Set_SITUAZIONE_FINALE_ICI_dbICI::DBOPENgovProvvedimentiUpdate")
                    End If
                    'Log.Debug("Inserito")
                Next
                Return intRetVal
            Catch ex As Exception

                Log.Error("Function::Set_SITUAZIONE_FINALE_ICI_dbICI::si è verificato il seguente errore::" & ex.Message)
                Return -1
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function

        '**** 201809 - Cartelle Insoluti ***
        Private Function SetDETTAGLIO_VOCI_LIQUIDAZIONI(ByVal IdEnte As String, ByVal IdProvvedimento As Integer, ByVal dsSanzioniFASE2 As DataSet, ByVal ListInteressiFASE2() As ObjInteressiSanzioni, myStringConnection As String) As Integer
            If DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT(IdProvvedimento, IdEnte, dsSanzioniFASE2, myStringConnection) <= 0 Then
                Return -1
            End If
            If DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT(IdEnte, IdProvvedimento, ListInteressiFASE2, myStringConnection) <= 0 Then
                Return -1
            End If
        End Function

        Private Function DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT(ByVal IDProvvedimento As Integer, ByVal IDEnte As String, ByVal objDataSet As DataSet, myStringConnection As String) As Integer
            Dim cmdMyCommand As New SqlCommand
            Dim nRet As Integer = 1

            Try
                cmdMyCommand.CommandType = CommandType.StoredProcedure
                'Valorizzo la connessione
                cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                If Not objDataSet Is Nothing Then
                    If objDataSet.Tables.Count > 0 Then
                        For Each MYrOW As DataRow In objDataSet.Tables(0).Rows
                            'Valorizzo i parameters:
                            cmdMyCommand.Parameters.Clear()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = -1
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.VarChar)).Value = IDEnte
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDPROVVEDIMENTO", SqlDbType.Int)).Value = IDProvvedimento
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVOCE", SqlDbType.VarChar)).Value = MYrOW("COD_VOCE")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Decimal)).Value = MYrOW("IMPORTO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTORIDOTTO", SqlDbType.Decimal)).Value = MYrOW("IMPORTO_RIDOTTO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Decimal)).Value = MYrOW("ACCONTO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Decimal)).Value = MYrOW("SALDO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAINIZIO", SqlDbType.VarChar)).Value = MYrOW("DATA_INIZIO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAFINE", SqlDbType.VarChar)).Value = MYrOW("DATA_FINE")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEMESTRIACCONTO", SqlDbType.Int)).Value = MYrOW("N_SEMESTRI_ACCONTO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEMESTRISALDO", SqlDbType.Int)).Value = MYrOW("N_SEMESTRI_SALDO").ToString
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TASSO", SqlDbType.Decimal)).Value = MYrOW("TASSO").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDFASE", SqlDbType.VarChar)).Value = MYrOW("GENERIC_ID")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@GIORNIACCONTO", SqlDbType.Int)).Value = MYrOW("N_GIORNI_ACCONTO").ToString
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@GIORNISALDO", SqlDbType.Int)).Value = MYrOW("N_GIORNI_SALDO").ToString
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOGIORNI", SqlDbType.Decimal)).Value = MYrOW("IMPORTO_GIORNI").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPACCONTOGIORNI", SqlDbType.Decimal)).Value = MYrOW("ACCONTO_GIORNI").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPSALDOGIORNI", SqlDbType.Decimal)).Value = MYrOW("SALDO_GIORNI").ToString()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOPROVVEDIMENTO", SqlDbType.Int)).Value = MYrOW("COD_TIPO_PROVVEDIMENTO")
                            cmdMyCommand.CommandText = "prc_DETTAGLIO_VOCI_LIQUIDAZIONI_IU"
                            cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                            Log.Debug("DettaglioVoci->" + Costanti.LogQuery(cmdMyCommand))
                            'eseguo la query
                            cmdMyCommand.ExecuteNonQuery()
                            nRet = cmdMyCommand.Parameters("@ID").Value
                            If nRet <= 0 Then
                                Exit For
                            End If
                        Next
                    End If
                    If Not objDataSet Is Nothing Then
                        objDataSet.Dispose()
                    End If
                End If
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT.errore::", ex)
                Log.Debug("DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
                nRet = -1
            Finally
                cmdMyCommand.Dispose()
                cmdMyCommand.Connection.Close()
            End Try
            Return nRet
        End Function
        Private Function DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT(ByVal IDEnte As String, ByVal IDProvvedimento As Integer, ByVal ListDati() As ObjInteressiSanzioni, myStringConnection As String) As Integer
            Dim cmdMyCommand As New SqlCommand
            Dim nRet As Integer

            Try
                cmdMyCommand.CommandType = CommandType.StoredProcedure
                'Valorizzo la connessione
                cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                For Each myItem As ObjInteressiSanzioni In ListDati
                    'Valorizzo i parameters:
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = myItem.ID
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.VarChar)).Value = IDEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDPROVVEDIMENTO", SqlDbType.Int)).Value = IDProvvedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVOCE", SqlDbType.VarChar)).Value = myItem.COD_VOCE
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Decimal)).Value = myItem.IMPORTO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTORIDOTTO", SqlDbType.Decimal)).Value = myItem.IMPORTO_RIDOTTO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Decimal)).Value = myItem.ACCONTO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Decimal)).Value = myItem.SALDO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAINIZIO", SqlDbType.VarChar)).Value = myItem.DATA_INIZIO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAFINE", SqlDbType.VarChar)).Value = myItem.DATA_FINE
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEMESTRIACCONTO", SqlDbType.Int)).Value = myItem.N_SEMESTRI_ACCONTO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEMESTRISALDO", SqlDbType.Int)).Value = myItem.N_SEMESTRI_SALDO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TASSO", SqlDbType.Decimal)).Value = myItem.TASSO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDFASE", SqlDbType.VarChar)).Value = myItem.IdFase
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@GIORNIACCONTO", SqlDbType.Int)).Value = myItem.N_GIORNI_ACCONTO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@GIORNISALDO", SqlDbType.Int)).Value = myItem.N_GIORNI_SALDO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOGIORNI", SqlDbType.Decimal)).Value = myItem.IMPORTO_GIORNI
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPACCONTOGIORNI", SqlDbType.Decimal)).Value = myItem.ACCONTO_GIORNI
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPSALDOGIORNI", SqlDbType.Decimal)).Value = myItem.SALDO_GIORNI
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOPROVVEDIMENTO", SqlDbType.Int)).Value = myItem.COD_TIPO_PROVVEDIMENTO
                    cmdMyCommand.CommandText = "prc_DETTAGLIO_VOCI_LIQUIDAZIONI_IU"
                    cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                    Log.Debug("DettaglioVOciInsert->" + Costanti.LogQuery(cmdMyCommand))
                    'eseguo la query
                    cmdMyCommand.ExecuteNonQuery()
                    nRet = cmdMyCommand.Parameters("@ID").Value
                    If nRet <= 0 Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.DETTAGLIO_VOCI_LIQUIDAZIONI_INSERT.errore::", ex)
                Log.Debug("DETTAGLIO_VOCI_LIQUIDAZIONI::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
            Finally
                cmdMyCommand.Dispose()
                cmdMyCommand.Connection.Close()
            End Try
            Return nRet
        End Function
        Private Function SetVERSAMENTI_LIQUIDAZIONI(ByVal IDEnte As String, ByVal objDSversamenti As DataSet, ByVal ID_PROCEDIMENTO As Long, ByVal ID_FASE As Long, myStringConnection As String) As Long
            Dim cmdMyCommand As New SqlCommand
            Dim nRet As Integer = 1

            Try
                cmdMyCommand.CommandType = CommandType.StoredProcedure
                'Valorizzo la connessione
                cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                If Not objDSversamenti Is Nothing Then
                    If objDSversamenti.Tables.Count > 0 Then
                        For Each myRow As DataRow In objDSversamenti.Tables(0).Rows
                            nRet = -1
                            'Valorizzo i parameters:
                            cmdMyCommand.Parameters.Clear()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = -1
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROCEDIMENTO", SqlDbType.Int)).Value = ID_PROCEDIMENTO
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_FASE", SqlDbType.VarChar)).Value = ID_FASE
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.VarChar)).Value = IDEnte
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDANAGRAFICO", SqlDbType.Int)).Value = myRow("idAnagrafico")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNORIFERIMENTO", SqlDbType.VarChar)).Value = myRow("annoRiferimento")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODICEFISCALE", SqlDbType.VarChar)).Value = "" 'myRow("codiceFiscale")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PARTITAIVA", SqlDbType.VarChar)).Value = "" 'myRow("partitaIva")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOPAGATO", SqlDbType.Decimal)).Value = myRow("importoPagato")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAPAGAMENTO", SqlDbType.VarChar)).Value = CDate(myRow("dataPagamento")).ToString("yyyyMMdd")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMEROFABBRICATIPOSSEDUTI", SqlDbType.Int)).Value = 0 'myRow("numeroFabbricatiPosseduti")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Bit)).Value = myRow("acconto")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Bit)).Value = myRow("saldo")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@RAVVEDIMENTOOPEROSO", SqlDbType.Bit)).Value = False 'myRow("ravvedimentoOperoso")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPOTERRENI", SqlDbType.Decimal)).Value = 0 'myRow("impoTerreni")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOAREEFABBRIC", SqlDbType.Decimal)).Value = 0 'myRow("importoAreeFabbric")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOABITAZPRINCIPALE", SqlDbType.Decimal)).Value = 0 'myRow("importoAbitazPrincipale")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTOALTRIFABBRIC", SqlDbType.Decimal)).Value = 0 'myRow("importoAltriFabbric")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DETRAZIONEABITAZPRINCIPALE", SqlDbType.Decimal)).Value = 0 'myRow("detrazioneAbitazPrincipale")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_VERSAMENTO_TARDIVO", SqlDbType.Bit)).Value = False 'myRow("FLAG_VERSAMENTO_TARDIVO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@GG", SqlDbType.Int)).Value = 0 'myRow("GG")
                            cmdMyCommand.CommandText = "prc_VERSAMENTI_ICI_LIQUIDAZIONI_IU"
                            cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                            'eseguo la query
                            Log.Debug("SetVersamenti->" + Costanti.LogQuery(cmdMyCommand))
                            cmdMyCommand.ExecuteNonQuery()
                            nRet = cmdMyCommand.Parameters("@ID").Value
                            If nRet <= 0 Then
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.SetVERSAMENTI_LIQUIDAZIONI.errore::", ex)
                Log.Debug("SetVERSAMENTI_LIQUIDAZIONI::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
            Finally
                cmdMyCommand.Dispose()
                cmdMyCommand.Connection.Close()
            End Try
            Return nRet
        End Function
#End Region

#Region "SAVE CONFIGURAZIONE"
        <AutoComplete()>
        Public Function SetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByRef strIDTIPOVOCE As String) As Boolean
            Log.Debug("SetTipoVoci::entrata")
            Dim cmdMyCommand As New SqlCommand

            Try
                cmdMyCommand.Connection = New SqlConnection(StringConnectionProvv)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                cmdMyCommand.CommandType = CommandType.StoredProcedure
                cmdMyCommand.CommandText = "prc_TIPO_VOCI_IU"
                cmdMyCommand.Parameters.Clear()
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOVOCE", SqlDbType.Int)).Value = objHashTable("IDTIPOVOCE")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.VarChar)).Value = IdEnte
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTRIBUTO", SqlDbType.VarChar)).Value = objHashTable("CODTRIBUTO")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CAPITOLO", SqlDbType.VarChar)).Value = objHashTable("CODCAPITOLO")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVOCE", SqlDbType.VarChar)).Value = objHashTable("CODVOCE")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOPROVVEDIMENTO", SqlDbType.Int)).Value = objHashTable("CODPROVVEDIMENTI")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MISURA", SqlDbType.VarChar)).Value = objHashTable("CODMISURA")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FASE", SqlDbType.VarChar)).Value = objHashTable("CODFASE")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DESCRVOCE", SqlDbType.VarChar)).Value = objHashTable("DESCVOCE")
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DESCRVOCEATTRIBUITA", SqlDbType.VarChar)).Value = objHashTable("VOCEATTRIBUITA")
                cmdMyCommand.Parameters("@IDTIPOVOCE").Direction = ParameterDirection.InputOutput
                'eseguo la query
                Log.Debug("SetTipoVoci->" + Costanti.LogQuery(cmdMyCommand))
                cmdMyCommand.ExecuteNonQuery()
                strIDTIPOVOCE = cmdMyCommand.Parameters("@IDTIPOVOCE").Value
                If strIDTIPOVOCE <= 0 Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetValoriVoci::DBOPENgovProvvedimentiUpdate")
                End If
            Catch ex As Exception
                Log.Debug("SetTipoVoci::errore::", ex)
                Throw New Exception("SetTipoVoci::" & ex.Message)
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        <AutoComplete()>
        Public Function DelTipoVoci(myStringConnection As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String = ""
            Dim intRetVal As Integer
            Dim strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODPROVVEDIMENTI, strIDTIPOVOCE As String

            Try
                strCODTRIBUTO = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
                strCODCAPITOLO = StringOperation.FormatString(objHashTable("CODCAPITOLO"))
                strCODVOCE = StringOperation.FormatString(objHashTable("CODVOCE"))
                strCODPROVVEDIMENTI = StringOperation.FormatString(objHashTable("CODPROVVEDIMENTI"))
                strIDTIPOVOCE = StringOperation.FormatString(objHashTable("IDTIPOVOCE"))
                Using ctx As New DBModel(MotoreProvUtility.DBType_SQL, myStringConnection)
                    sSQL = "DELETE FROM TIPO_VOCI "
                    sSQL += " WHERE COD_ENTE='" & strCODENTE & "'"
                    sSQL += " AND ID_TIPO_VOCE=" & strIDTIPOVOCE
                    sSQL += " AND COD_TRIBUTO='" & strCODTRIBUTO & "'"
                    sSQL += " AND COD_CAPITOLO='" & strCODCAPITOLO & "'"
                    sSQL += " AND COD_VOCE='" & strCODVOCE & "'"
                    sSQL += " AND COD_TIPO_PROVVEDIMENTO='" & strCODPROVVEDIMENTI
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    intRetVal = ctx.ExecuteNonQuery(sSQL)
                    ctx.Dispose()
                End Using
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.DelTipoVoci.errore::", ex)
                Throw New Exception("DBOPENgovProvvedimentiUpdate.DelTipoVoci.errore:: " & ex.Message)
            End Try

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelTipoVoci::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function
        <AutoComplete()>
        Public Function SetValoriVoci(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable, ByRef intRetVal As Integer) As Boolean
            Dim sSQL As String
            Dim myDataView As New DataView
            Dim strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODTIPOPROVVEDIMENTO, strCODCALCOLATO, strCODMISURA, strANNO_OLD, strIDTIPOVOCE As String
            Dim strID_VALORE_VOCI As String
            Dim strAnno, strValore, strMinimo, strCum, strRid, strInsUp As String
            Dim strCondizione, strParametro, strBaseRaffronto, strCalcolata, strTipoInteresse As String
            Dim strCondizione_intr, strParametro_intr, strBaseRaffronto_intr As String
            'Dim intRetVal As Integer
            objUtility = New MotoreProvUtility

            Try
                strCODTRIBUTO = objHashTable("CODTRIBUTO")
                strCODCAPITOLO = objHashTable("CODCAPITOLO")
                strCODVOCE = objHashTable("CODVOCE")
                strCODTIPOPROVVEDIMENTO = objHashTable("CODTIPOPROVVEDIMENTO")
                strID_VALORE_VOCI = objHashTable("ID_VALORE_VOCI")
                strIDTIPOVOCE = objHashTable("IDTIPOVOCE")


                strAnno = objHashTable("ANNO")
                strValore = objHashTable("VALORE")
                strMinimo = objHashTable("MINIMO")
                strCum = objHashTable("CUMULABILE")
                strRid = objHashTable("RIDUCIBILE")

                strCondizione = objHashTable("CONDIZIONE")
                strParametro = objHashTable("PARAMETRO")
                strBaseRaffronto = objHashTable("BASERAFFRONTO")
                strCalcolata = objHashTable("CALCOLATASU")
                strTipoInteresse = objHashTable("TIPOINTERESSE")

                strCondizione_intr = objHashTable("CONDIZIONE_INTR")
                strParametro_intr = objHashTable("PARAMETRO_INTR")
                strBaseRaffronto_intr = objHashTable("BASERAFFRONTO_INTR")

                strInsUp = objHashTable("INSUP")


                sSQL = " SELECT * FROM VALORE_VOCI"
                sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                sSQL += " and ID_TIPO_VOCE=" & strIDTIPOVOCE
                sSQL += " and COD_TRIBUTO=" & objUtility.CStrToDB(strCODTRIBUTO)
                sSQL += " and COD_CAPITOLO=" & objUtility.CStrToDB(strCODCAPITOLO)
                sSQL += " and COD_VOCE=" & objUtility.CStrToDB(strCODVOCE)
                sSQL += " and ANNO=" & objUtility.CStrToDB(strAnno)
                sSQL += " and COD_TIPO_PROVVEDIMENTO=" & objUtility.CStrToDB(strCODTIPOPROVVEDIMENTO)
                If IsNothing(strTipoInteresse) Then
                    sSQL += " and VALORE=" & objUtility.CStrToDB(strValore)
                    sSQL += " and MINIMO=" & objUtility.CStrToDB(strMinimo)
                    If strCum.CompareTo("False") = 0 Then
                        sSQL += " and CUMULABILE=" & objUtility.CStrToDB(0)
                    Else
                        sSQL += " and CUMULABILE=" & objUtility.CStrToDB(1)
                    End If
                    If strRid.CompareTo("False") = 0 Then
                        sSQL += " and RIDUCIBILE=" & objUtility.CStrToDB(0)
                    Else
                        sSQL += " and RIDUCIBILE=" & objUtility.CStrToDB(1)
                    End If
                    'sSQL+= " and RIDUCIBILE=" & objUtility.CStrToDB(strRid)
                    'sSQL+= " and CUMULABILE=" & objUtility.CStrToDB(strCum)
                    sSQL += " and CALCOLATA_SU=" & objUtility.CStrToDB(strCalcolata)
                    sSQL += " and CONDIZIONE=" & objUtility.CStrToDB(strCondizione)
                    sSQL += " and PARAMETRO=" & objUtility.CStrToDB(strParametro)
                    sSQL += " and BASE_RAFFRONTO=" & objUtility.CStrToDB(strBaseRaffronto)

                    sSQL += " and CONDIZIONE_INTR=" & objUtility.CStrToDB(strCondizione_intr)
                    sSQL += " and PARAMETRO_INTR=" & objUtility.CStrToDB(strParametro_intr)
                    sSQL += " and BASE_RAFFRONTO_INTR=" & objUtility.CStrToDB(strBaseRaffronto_intr)
                Else
                    sSQL += " and COD_TIPO_INTERESSE=" & objUtility.CStrToDB(strBaseRaffronto)
                End If

                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                    Try
                        sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                        myDataView = ctx.GetDataView(sSQL, "TBL")
                        If myDataView.Count > 0 Then
                            intRetVal = COSTANTValue.CostantiProv.INIT_CHIAVE_DUPLICATA
                            Return False
                            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetValoriVoci::DBOPENgovProvvedimentiUpdate::VALOREVOCEPRESENTE")
                        End If

                        Select Case strInsUp
                            Case "U"
                                strANNO_OLD = objHashTable("ANNO_OLD")
                                sSQL = "UPDATE VALORE_VOCI set "
                                sSQL += " ANNO=" & objUtility.CStrToDB(strAnno) & " ,"
                                sSQL += " VALORE=" & objUtility.CStrToDB(strValore) & " ,"
                                sSQL += " MINIMO=" & objUtility.CStrToDB(strMinimo) & " ,"

                                If strCum.CompareTo("False") = 0 Then
                                    sSQL += " CUMULABILE=" & objUtility.CStrToDB(0) & " ,"
                                Else
                                    sSQL += " CUMULABILE=" & objUtility.CStrToDB(1) & " ,"
                                End If
                                If strRid.CompareTo("False") = 0 Then
                                    sSQL += " RIDUCIBILE=" & objUtility.CStrToDB(0) & " ,"
                                Else
                                    sSQL += " RIDUCIBILE=" & objUtility.CStrToDB(1) & " ,"
                                End If

                                sSQL += " CALCOLATA_SU=" & objUtility.cToInt(strCalcolata) & " ,"
                                sSQL += " CONDIZIONE=" & objUtility.CStrToDB(strCondizione) & " ,"
                                sSQL += " PARAMETRO=" & objUtility.CStrToDB(strParametro) & " ,"
                                sSQL += " BASE_RAFFRONTO=" & objUtility.CStrToDB(strBaseRaffronto) & " ,"
                                sSQL += " COD_TIPO_INTERESSE=" & objUtility.CStrToDB(strTipoInteresse) & " ,"

                                sSQL += " CONDIZIONE_INTR=" & objUtility.CStrToDB(strCondizione_intr) & " ,"
                                sSQL += " PARAMETRO_INTR=" & objUtility.CStrToDB(strParametro_intr) & " ,"
                                sSQL += " BASE_RAFFRONTO_INTR=" & objUtility.CStrToDB(strBaseRaffronto_intr)
                                sSQL += " where ID_VALORE_VOCE=" & objUtility.cToInt(strID_VALORE_VOCI)
                                sSQL += " and ID_TIPO_VOCE=" & strIDTIPOVOCE
                                intRetVal = ctx.ExecuteNonQuery(sSQL)
                                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                                    Return False
                                End If
                                Return True
                            Case "I"
                                sSQL = "INSERT into VALORE_VOCI "
                                sSQL += " (COD_ENTE,COD_TRIBUTO,COD_CAPITOLO,ID_TIPO_VOCE,COD_VOCE,COD_TIPO_PROVVEDIMENTO,ANNO,VALORE,MINIMO,RIDUCIBILE,CUMULABILE,CALCOLATA_SU,CONDIZIONE,PARAMETRO,BASE_RAFFRONTO,COD_TIPO_INTERESSE,CONDIZIONE_INTR,PARAMETRO_INTR,BASE_RAFFRONTO_INTR)"
                                sSQL += " VALUES ("
                                sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                                sSQL += objUtility.CStrToDB(strCODTRIBUTO) & " ,"
                                sSQL += objUtility.CStrToDB(strCODCAPITOLO) & " ,"
                                sSQL += strIDTIPOVOCE & " ,"
                                sSQL += objUtility.CStrToDB(strCODVOCE) & " ,"
                                sSQL += objUtility.CStrToDB(strCODTIPOPROVVEDIMENTO) & " ,"
                                sSQL += objUtility.CStrToDB(strAnno) & " ,"
                                sSQL += objUtility.CStrToDB(strValore) & " ,"
                                sSQL += objUtility.CStrToDB(strMinimo) & " ,"

                                If strRid.CompareTo("False") = 0 Then
                                    sSQL += objUtility.CStrToDB(0) & " ,"
                                Else
                                    sSQL += objUtility.CStrToDB(1) & " ,"
                                End If
                                If strCum.CompareTo("False") = 0 Then
                                    sSQL += objUtility.CStrToDB(0) & " ,"
                                Else
                                    sSQL += objUtility.CStrToDB(1) & " ,"
                                End If

                                sSQL += objUtility.cToInt(strCalcolata) & " ,"
                                sSQL += objUtility.CStrToDB(strCondizione) & " ,"
                                sSQL += objUtility.CStrToDB(strParametro) & " ,"
                                sSQL += objUtility.CStrToDB(strBaseRaffronto) & " ,"
                                sSQL += objUtility.CStrToDB(strTipoInteresse) & " ,"

                                sSQL += objUtility.CStrToDB(strCondizione_intr) & " ,"
                                sSQL += objUtility.CStrToDB(strParametro_intr) & " ,"
                                sSQL += objUtility.CStrToDB(strBaseRaffronto_intr)

                                sSQL += " )"
                                Log.Debug("SetValorVoci..->" + sSQL)
                                intRetVal = ctx.ExecuteNonQuery(sSQL)
                                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                                    Return False
                                End If
                                If intRetVal = COSTANTValue.CostantiProv.INIT_CHIAVE_DUPLICATA Then
                                    Return False
                                End If
                                Return True
                            Case Else
                                Return False
                        End Select
                    Catch ex As Exception
                        Log.Debug("setValoriVoci.erroreQuery: ", ex)
                        Return Nothing
                    Finally
                        ctx.Dispose()
                    End Try
                End Using
            Catch ex As Exception
                intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER
                Return False
            End Try
        End Function
        <AutoComplete()>
        Public Function DelValoriVoci(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean

            Dim sSQL As String
            Dim strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strIDTIPOVOCE As String
            Dim strID_VALORE_VOCI As String
            Dim strAnno As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility

            strIDTIPOVOCE = objHashTable("IDTIPOVOCE")
            strCODTRIBUTO = objHashTable("CODTRIBUTO")
            strCODCAPITOLO = objHashTable("CODCAPITOLO")
            strCODVOCE = objHashTable("CODVOCE")
            strAnno = objHashTable("ANNO")
            strID_VALORE_VOCI = objHashTable("ID_VALORE_VOCI")

            sSQL = "DELETE FROM VALORE_VOCI "
            If Not IsNothing(strID_VALORE_VOCI) Then
                'Se elimino direttamente il valore voce
                sSQL += " where ID_VALORE_VOCE=" & objUtility.cToInt(strID_VALORE_VOCI)
            Else
                'Se elimino la voce generale, allora devo eliminare tutti i valore voce associati
                sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                sSQL += " and ID_TIPO_VOCE=" & strIDTIPOVOCE
                sSQL += " and COD_TRIBUTO=" & objUtility.CStrToDB(strCODTRIBUTO)
                sSQL += " and COD_CAPITOLO=" & objUtility.CStrToDB(strCODCAPITOLO)
                sSQL += " and COD_VOCE=" & objUtility.CStrToDB(strCODVOCE)
                If strAnno.CompareTo("") <> 0 Then
                    sSQL += " and ANNO=" & objUtility.CStrToDB(strAnno)
                End If

            End If
            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    intRetVal = ctx.ExecuteNonQuery(sSQL)
                Catch ex As Exception
                    Log.Debug("DelValoriVoci.errore: ", ex)
                    Return False
                Finally
                    ctx.Dispose()
                End Try
            End Using

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelValoriVoci::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function
        <AutoComplete()>
        Public Function SetTassiInteresse(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strCODTIPOINTERESSE, strDAL, strAL, strTASSO As String
            Dim strInsUp As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility


            strCODTIPOINTERESSE = objHashTable("CODTIPOINTERESSE")
            strDAL = objHashTable("DAL")
            strAL = objHashTable("AL")
            strTASSO = objHashTable("TASSO")


            strInsUp = objHashTable("INSUP")
            Select Case strInsUp

                Case "U"

                    'UPDATE
                    sSQL = "UPDATE TASSI_DI_INTERESSE set "
                    sSQL += " DAL=" & objUtility.CStrToDB(strDAL) & " ,"
                    If strAL <> "" Then
                        sSQL += " AL=" & objUtility.CStrToDB(strAL, True, True) & " ,"
                    Else
                        sSQL += " AL=null ,"
                    End If
                    sSQL += " TASSO_ANNUALE=" & objUtility.CDoubleToDB(strTASSO)

                    sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                    sSQL += " and COD_TIPO_INTERESSE=" & objUtility.CStrToDB(strCODTIPOINTERESSE)
                    sSQL += " and DAL=" & objUtility.CStrToDB(strDAL, , True)
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetTassiInteresse.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case "I"
                    'INSERT
                    sSQL = "INSERT into TASSI_DI_INTERESSE "
                    sSQL += " (COD_ENTE,COD_TIPO_INTERESSE,DAL,AL,TASSO_ANNUALE)"
                    sSQL += " values ("
                    sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                    sSQL += objUtility.CStrToDB(strCODTIPOINTERESSE) & " ,"
                    sSQL += objUtility.CStrToDB(strDAL) & " ,"
                    If strAL <> "" Then
                        sSQL += objUtility.CStrToDB(strAL, True, True) & " ,"
                    Else
                        sSQL += " null, "
                    End If
                    'sSQL+= objUtility.CStrToDB(strAL, True, True) & " ,"
                    sSQL += objUtility.CDoubleToDB(strTASSO)

                    sSQL += " )"
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetTassiInteresse.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case Else
                    Return False
            End Select
            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetValoriVoci::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function

        <AutoComplete()>
        Public Function SetScadenzaInteressi(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strANNO, strANNOOld, strDATA, strDATAOld, strTRIBUTO, strTRIBUTOOld, strNOTE As String
            Dim strInsUp As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility


            strANNO = objHashTable("ANNO")
            strANNOOld = objHashTable("ANNOOLD")
            strDATA = objHashTable("DATA")
            strDATAOld = objHashTable("DATAOLD")
            strTRIBUTO = objHashTable("CODTRIBUTO")
            strTRIBUTOOld = objHashTable("CODTRIBUTOOLD")
            strNOTE = objHashTable("NOTE")


            strInsUp = objHashTable("INSUP")
            Select Case strInsUp

                Case "U"

                    'UPDATE
                    sSQL = "UPDATE TAB_SCADENZA_INTERESSI set "
                    sSQL += " DATA_SCADENZA=" & objUtility.CStrToDB(strDATA, , True) & " ,"
                    sSQL += " ANNO=" & objUtility.CStrToDB(strANNO) & " ,"
                    sSQL += " NOTE=" & objUtility.CStrToDB(strNOTE) & ", "
                    sSQL += " COD_TRIBUTO=" & objUtility.CStrToDB(strTRIBUTO) & ""

                    sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                    sSQL += " and ANNO=" & objUtility.CStrToDB(strANNOOld)
                    sSQL += " and DATA_SCADENZA=" & objUtility.CStrToDB(strDATAOld, , True)
                    sSQL += " AND COD_TRIBUTO=" & objUtility.CStrToDB(strTRIBUTOOld) & ""
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetScadenzaInteressi.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case "I"
                    'INSERT
                    sSQL = "INSERT into TAB_SCADENZA_INTERESSI "
                    sSQL += " (COD_ENTE, ANNO, DATA_SCADENZA, NOTE, COD_TRIBUTO)"
                    sSQL += " values ("
                    sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                    sSQL += objUtility.CStrToDB(strANNO) & " ,"
                    sSQL += objUtility.CStrToDB(strDATA, , True) & " ,"
                    sSQL += objUtility.CStrToDB(strNOTE) & " ,"
                    sSQL += objUtility.CStrToDB(strTRIBUTO)

                    sSQL += " )"
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetScadenzaInteressi.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case Else
                    Return False
            End Select
            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetScadenzaInteressi::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function

        <AutoComplete()>
        Public Function SetAnniProvvedimenti(StringConnectionProvv As String, sIdEnte As String, ByVal objHashTable As Hashtable) As Boolean
            Dim strCODTRIBUTO, strANNO As String
            Dim nQuotaRiduzione As Integer = 4 'valore fisso che aveva prima dell'IMU
            Dim intRetVal, nCODTIPOPROVVEDIMENTO As Integer
            Dim impSogliaMinima As Double
            Dim cmdMyCommand As New SqlCommand

            Try
                cmdMyCommand.Connection = New SqlConnection(StringConnectionProvv)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                strCODTRIBUTO = objHashTable("CODTRIBUTO")
                nCODTIPOPROVVEDIMENTO = objHashTable("CODTIPOPROVVEDIMENTO")
                strANNO = objHashTable("ANNO")
                'originale
                'impSogliaMinima = CDbl(CStr(objHashTable("SOGLIAMINIMA")).Replace(",", "."))



                impSogliaMinima = CDbl(objHashTable("SOGLIAMINIMA"))
                'Dim tmp As Double
                'tmp = objHashTable("SOGLIAMINIMA") * 100
                'tmp = tmp / 100
                'impSogliaMinima = tmp



                '*** 20140701 - IMU/TARES ***
                nQuotaRiduzione = objHashTable("QUOTARIDUZIONESANZIONI")
                '*** ***

                cmdMyCommand.CommandType = CommandType.StoredProcedure
                cmdMyCommand.CommandText = "ANNI_PROVVEDIMENTI_IU"
                cmdMyCommand.Parameters.Clear()
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = sIdEnte
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = nCODTIPOPROVVEDIMENTO
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TRIBUTO", SqlDbType.NVarChar)).Value = strCODTRIBUTO
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = strANNO
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_MINIMO_ANNO", SqlDbType.Float)).Value = impSogliaMinima
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@QUOTARIDUZIONESANZIONI", SqlDbType.Int)).Value = nQuotaRiduzione
                Log.Debug("SEtAnniProv->" + Costanti.LogQuery(cmdMyCommand))
                intRetVal = cmdMyCommand.ExecuteNonQuery
                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetAnniProvvedimenti::DBOPENgovProvvedimentiUpdate::errore in inserimento")
                End If
                If intRetVal = COSTANTValue.CostantiProv.INIT_CHIAVE_DUPLICATA Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetAnniProvvedimenti::DBOPENgovProvvedimentiUpdate::ANNOPRESENTE")
                End If
                Return True
            Catch ex As Exception
                Log.Debug("SetAnniProvvedimenti::si è verificato il seguente errore::", ex)
                Throw New Exception("SetAnniProvvedimenti::si è verificato il seguente errore::", ex)
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function

        <AutoComplete()>
        Public Function DelTassiInteresse(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strCODTIPOINTERESSE, strDAL As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility

            strCODTIPOINTERESSE = objHashTable("CODTIPOINTERESSE")
            strDAL = objHashTable("DAL")

            sSQL = "DELETE FROM TASSI_DI_INTERESSE "
            sSQL += " WHERE COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
            sSQL += " AND COD_TIPO_INTERESSE=" & objUtility.CStrToDB(strCODTIPOINTERESSE)
            sSQL += " AND DAL=" & objUtility.CStrToDB(strDAL)
            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    intRetVal = ctx.ExecuteNonQuery(sSQL)
                Catch ex As Exception
                    Log.Debug("DelTassiInteresse.errore: ", ex)
                    Return False
                Finally
                    ctx.Dispose()
                End Try
            End Using

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelValoriVoci::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function

        <AutoComplete()>
        Public Function DelScadenzaInteressi(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strTRIBUTO, strDATA, strANNO As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility

            strTRIBUTO = objHashTable("CODTRIBUTO")
            strDATA = objHashTable("DATA")
            strANNO = objHashTable("ANNO")

            sSQL = "DELETE FROM TAB_SCADENZA_INTERESSI "
            sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
            sSQL += " and ANNO=" & objUtility.CStrToDB(strANNO)
            sSQL += " and COD_TRIBUTO=" & objUtility.CStrToDB(strTRIBUTO)
            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    intRetVal = ctx.ExecuteNonQuery(sSQL)
                Catch ex As Exception
                    Log.Debug("DelScadenzaInteressi.errore: ", ex)
                    Return False
                Finally
                    ctx.Dispose()
                End Try
            End Using

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelScadenzaInteressi::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function
        <AutoComplete()>
        Public Function DelAnniProvvedimenti(StringConnectionProvv As String, sIdEnte As String, ByVal objHashTable As Hashtable) As Boolean
            Dim strCODTRIBUTO, strANNO As String
            Dim intRetVal, nCODTIPOPROVVEDIMENTO As Integer
            Dim cmdMyCommand As New SqlCommand

            Try
                cmdMyCommand.Connection = New SqlConnection(StringConnectionProvv)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                strCODTRIBUTO = objHashTable("CODTRIBUTO")
                nCODTIPOPROVVEDIMENTO = objHashTable("CODTIPOPROVVEDIMENTO")
                strANNO = objHashTable("ANNO")

                cmdMyCommand.CommandType = CommandType.StoredProcedure
                cmdMyCommand.CommandText = "ANNI_PROVVEDIMENTI_D"
                cmdMyCommand.Parameters.Clear()
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = sIdEnte
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = nCODTIPOPROVVEDIMENTO
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TRIBUTO", SqlDbType.NVarChar)).Value = strCODTRIBUTO
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = strANNO
                Log.Debug("DelAnniProv->" + Costanti.LogQuery(cmdMyCommand))
                intRetVal = cmdMyCommand.ExecuteNonQuery
                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetAnniProvvedimenti::DBOPENgovProvvedimentiUpdate::errore in inserimento")
                End If
                Return True
            Catch ex As Exception
                Log.Debug("DelAnniProvvedimenti::si è verificato il seguente errore::", ex)
                Throw New Exception("DelAnniProvvedimenti::si è verificato il seguente errore::", ex)
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function

        <AutoComplete()>
        Public Function SetMotivazioni(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strInsUp As String
            Dim intRetVal As Integer

            objUtility = New MotoreProvUtility
            Dim lngID_MOTIVAZIONE As Long
            Dim strCOD_TRIBUTO, strCOD_VOCE, strCOD_MOTIVAZIONE, strDESC_MOTIVAZIONE, strCOD_TRIBUTO_OLD, strCOD_VOCE_OLD, strCOD_MOTIVAZIONE_OLD, strDESC_MOTIVAZIONE_OLD, strID_MOTIVAZIONE As String

            strCOD_TRIBUTO = objHashTable("COD_TRIBUTO")
            strCOD_VOCE = objHashTable("COD_VOCE")
            strCOD_MOTIVAZIONE = objHashTable("COD_MOTIVAZIONE")
            strDESC_MOTIVAZIONE = objHashTable("DESC_MOTIVAZIONE")

            strCOD_TRIBUTO_OLD = objHashTable("COD_TRIBUTO_OLD")
            strCOD_VOCE_OLD = objHashTable("COD_VOCE_OLD")
            strCOD_MOTIVAZIONE_OLD = objHashTable("COD_MOTIVAZIONE_OLD")
            strDESC_MOTIVAZIONE_OLD = objHashTable("DESC_MOTIVAZIONE_OLD")
            strID_MOTIVAZIONE = objHashTable("ID_MOTIVAZIONE")

            strInsUp = objHashTable("INSUP")
            Select Case strInsUp
                Case "U"
                    sSQL = "UPDATE TAB_MOTIVAZIONI set "
                    sSQL += " COD_TRIBUTO=" & objUtility.CStrToDB(strCOD_TRIBUTO) & " ,"
                    sSQL += " COD_VOCE=" & objUtility.CStrToDB(strCOD_VOCE) & " ,"
                    sSQL += " CODICE_MOTIVAZIONE=" & objUtility.CStrToDB(strCOD_MOTIVAZIONE) & " ,"
                    sSQL += " DESCRIZIONE_MOTIVAZIONE=" & objUtility.CStrToDB(strDESC_MOTIVAZIONE)
                    sSQL += " where ID_MOTIVAZIONE=" & objUtility.cToInt(strID_MOTIVAZIONE)
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetMotivazioni.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case "I"
                    Dim DBselect As New DBOPENgovProvvedimentiSelect
                    lngID_MOTIVAZIONE = DBselect.getNewID("TAB_MOTIVAZIONI", StringConnectionProvv)
                    'DBselect.Dispose()

                    sSQL = "INSERT INTO TAB_MOTIVAZIONI "
                    sSQL += " (COD_ENTE,COD_TRIBUTO,COD_VOCE,ID_MOTIVAZIONE,CODICE_MOTIVAZIONE,DESCRIZIONE_MOTIVAZIONE)"
                    sSQL += " VALUES ("
                    sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                    sSQL += objUtility.CStrToDB(strCOD_TRIBUTO) & " ,"
                    sSQL += objUtility.CStrToDB(strCOD_VOCE) & " ,"
                    sSQL += objUtility.cToInt(lngID_MOTIVAZIONE) & " ,"
                    sSQL += objUtility.CStrToDB(strCOD_MOTIVAZIONE) & " ,"
                    sSQL += objUtility.CStrToDB(strDESC_MOTIVAZIONE)
                    sSQL += " )"
                    Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                        Try
                            sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                            intRetVal = ctx.ExecuteNonQuery(sSQL)
                        Catch ex As Exception
                            Log.Debug("SetMotivazioni.errore: ", ex)
                            Return False
                        Finally
                            ctx.Dispose()
                        End Try
                    End Using

                Case Else
                    Return False
            End Select

            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetMotivazioni::DBOPENgovProvvedimentiUpdate")
            End If
            If intRetVal = COSTANTValue.CostantiProv.INIT_CHIAVE_DUPLICATA Then
                'Throw New Exception("Motivazione già presente")
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetMotivazioni::DBOPENgovProvvedimentiUpdate::MOTIVAZIONEPRESENTE")
            End If

            Return True
        End Function
        <AutoComplete()>
        Public Function DelMotivazioni(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strID_MOTIVAZIONE As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility

            strID_MOTIVAZIONE = objHashTable("ID_MOTIVAZIONE")

            sSQL = "DELETE FROM TAB_MOTIVAZIONI "
            sSQL += " WHERE ID_MOTIVAZIONE=" & objUtility.cToInt(strID_MOTIVAZIONE)
            sSQL += " AND COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    intRetVal = ctx.ExecuteNonQuery(sSQL)
                Catch ex As Exception
                    Log.Debug("DelMotivazioni.errore: ", ex)
                    Return False
                Finally
                    ctx.Dispose()
                End Try
            End Using
            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelMotivazioni::DBOPENgovProvvedimentiUpdate")
            End If
            Return True
        End Function

        <AutoComplete()>
        Public Function SetTipologieVoci(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strInsUp As String
            Dim intRetVal As Integer
            Dim strCOD_TRIBUTO, strCOD_VOCE, strDESC_SANZIONE, strCOD_TRIBUTO_OLD, strCOD_VOCE_OLD As String
            Dim NumTipoVociConf As String
            Dim objDSGetTipologieVoci As DataSet = Nothing
            Dim objDA As New SqlDataAdapter
            Dim cmdMyCommand As New SqlCommand

            Try
                objUtility = New MotoreProvUtility

                strCOD_TRIBUTO = objHashTable("COD_TRIBUTO")
                strCOD_VOCE = objHashTable("COD_VOCE")
                strDESC_SANZIONE = objHashTable("DESC_SANZIONE")

                strCOD_TRIBUTO_OLD = objHashTable("COD_TRIBUTO_OLD")
                strCOD_VOCE_OLD = objHashTable("COD_VOCE_OLD")

                strInsUp = objHashTable("INSUP")

                sSQL = "select * from TIPO_VOCI "
                sSQL &= " where COD_VOCE = " & objUtility.CStrToDB(strCOD_VOCE_OLD)
                sSQL &= " AND COD_TRIBUTO=" & objUtility.CStrToDB(strCOD_TRIBUTO_OLD)
                sSQL &= " AND COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "

                objDSGetTipologieVoci = New DataSet
                cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionProvv)
                cmdMyCommand.CommandTimeout = 0
                If cmdMyCommand.Connection.State = ConnectionState.Closed Then
                    cmdMyCommand.Connection.Open()
                End If
                cmdMyCommand.CommandType = CommandType.Text
                cmdMyCommand.CommandText = sSQL
                Log.Debug("settipovoci->" + Costanti.LogQuery(cmdMyCommand))
                objDA.SelectCommand = cmdMyCommand
                objDA.Fill(objDSGetTipologieVoci, "TIPO_VOCI")

                Select Case strInsUp
                    Case "U"
                        sSQL = "UPDATE TIPOLOGIE_SANZIONI SET "
                        sSQL += " COD_TRIBUTO=" & objUtility.CStrToDB(strCOD_TRIBUTO) & " ,"
                        sSQL += " COD_VOCE=" & objUtility.CStrToDB(strCOD_VOCE) & " ,"
                        sSQL += " DESCRIZIONE=" & objUtility.CStrToDB(strDESC_SANZIONE)
                        sSQL += " WHERE COD_TRIBUTO=" & objUtility.CStrToDB(strCOD_TRIBUTO_OLD)
                        sSQL += " AND COD_VOCE=" & objUtility.CStrToDB(strCOD_VOCE_OLD)
                        Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                            Try
                                sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                                intRetVal = ctx.ExecuteNonQuery(sSQL)
                            Catch ex As Exception
                                Log.Debug("SetTipologieVoci.errore: ", ex)
                                Return False
                            Finally
                                ctx.Dispose()
                            End Try
                        End Using
                    Case "I"
                        sSQL = "INSERT INTO TIPOLOGIE_SANZIONI "
                        sSQL += " (COD_ENTE,COD_TRIBUTO,COD_VOCE,DESCRIZIONE)"
                        sSQL += " VALUES ("
                        sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                        sSQL += objUtility.CStrToDB(strCOD_TRIBUTO) & " ,"
                        sSQL += objUtility.CStrToDB(strCOD_VOCE) & " ,"
                        sSQL += objUtility.CStrToDB(strDESC_SANZIONE)
                        sSQL += " )"
                        Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                            Try
                                sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                                intRetVal = ctx.ExecuteNonQuery(sSQL)
                            Catch ex As Exception
                                Log.Debug("SetTipologieVoci.errore: ", ex)
                                Return False
                            Finally
                                ctx.Dispose()
                            End Try
                        End Using

                    Case Else
                        Return False
                End Select

                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetTipologieVoci::DBOPENgovProvvedimentiUpdate")
                End If
                If intRetVal = COSTANTValue.CostantiProv.INIT_CHIAVE_DUPLICATA Then
                    Throw New Exception("Sanzione già presente")
                End If

                Return True
            Catch ex As Exception
                Log.Debug("SetTipologieVoci::errore->", ex)
            End Try
        End Function
        <AutoComplete()>
        Public Function DelTipologieVoci(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strCODTRIBUTO, strCODVOCE As String
            Dim intRetVal As Integer
            objUtility = New MotoreProvUtility
            Dim objDA As New SqlDataAdapter
            Dim objDSGetTipoVociConfig As DataSet = Nothing
            Dim cmdMyCommand As New SqlCommand
            Try
                'objDBManager = New DBManager
                'objDBManager.Initialize(StringConnectionProvv)

                strCODTRIBUTO = objHashTable("COD_TRIBUTO")
                strCODVOCE = objHashTable("COD_VOCE")

                sSQL = "select count(*) as contatore from TIPO_VOCI"
                sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                sSQL += " and COD_TRIBUTO=" & objUtility.CStrToDB(strCODTRIBUTO)
                sSQL += " and COD_VOCE=" & objUtility.CStrToDB(strCODVOCE)
                objDSGetTipoVociConfig = New DataSet
                cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionProvv)
                cmdMyCommand.CommandTimeout = 0
                If cmdMyCommand.Connection.State = ConnectionState.Closed Then
                    cmdMyCommand.Connection.Open()
                End If
                cmdMyCommand.CommandType = CommandType.Text
                cmdMyCommand.CommandText = sSQL
                Log.Debug("deltipovocicount->" + Costanti.LogQuery(cmdMyCommand))
                objDA.SelectCommand = cmdMyCommand
                objDA.Fill(objDSGetTipoVociConfig, "TIPO_VOCI")
                'objDBManager = New DBManager

                'objDBManager.Initialize(StringConnectionProvv)

                'objDA = objDBManager.GetPrivateDataAdapter(sSQL)

                If objDSGetTipoVociConfig.Tables(0).Rows(0).Item("contatore") <> 0 Then
                    'If Not IsNothing(objDBManager) Then
                    '    objDBManager.Kill()
                    '    objDBManager.Dispose()

                    'End If
                    Throw New Exception("TipologiaVoceUtilizzata")

                Else
                    sSQL = "DELETE FROM TIPOLOGIE_SANZIONI "
                    sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                    sSQL += " and COD_TRIBUTO=" & objUtility.CStrToDB(strCODTRIBUTO)
                    sSQL += " and COD_VOCE=" & objUtility.CStrToDB(strCODVOCE)
                    cmdMyCommand.CommandText = sSQL
                    Log.Debug("deltipovoci->" + Costanti.LogQuery(cmdMyCommand))
                    intRetVal = cmdMyCommand.ExecuteNonQuery
                    ' objDBManager = New DBManager
                    ' objDBManager.Initialize(StringConnectionProvv)
                    'intRetVal = objDBManager.Execute(sSQL)
                End If
                'If Not IsNothing(objDBManager) Then
                '    objDBManager.Kill()
                '    objDBManager.Dispose()

                'End If
                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DelValoriVoci::DBOPENgovProvvedimentiUpdate")
                End If
                Return True
            Catch ex As Exception
                Log.Debug("DelTipologieVoci::errore->", ex)
                Return False
            End Try
        End Function

        <AutoComplete()>
        Public Function SetGeneraleICI(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
            Dim sSQL As String
            Dim strAnno, strInsUp As String
            Dim strRIENTRO_LIQ_CONF_AVVISO, strRIENTRO_LIQ_ATTO_DEF, strINT_ACCONTO_SALDO, strINT_SALDO As String
            Dim intRetVal As Integer
            Dim objDSGetSanzioniConfig As DataSet = Nothing
            Dim objDA As SqlDataAdapter

            Dim strDATA_VERSAMENTO_ACCONTO As String

            Dim strDATA_VERSAMENTO_SALDO As String

            objUtility = New MotoreProvUtility

            strAnno = objHashTable("ANNO")

            strRIENTRO_LIQ_CONF_AVVISO = objHashTable("RIENTRO_LIQ_CONF_AVVISO")
            strRIENTRO_LIQ_ATTO_DEF = objHashTable("RIENTRO_LIQ_ATTO_DEF")
            strINT_ACCONTO_SALDO = objHashTable("INT_ACCONTO_SALDO")
            strINT_SALDO = objHashTable("INT_SALDO")
            strInsUp = objHashTable("INSUP")

            Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, StringConnectionProvv)
                Try
                    sSQL = "SELECT * FROM TP_GENERALE_ICI"
                    sSQL += " WHERE COD_ENTE = '" & strCODENTE & "' "
                    sSQL += " AND ANNO = '" & strAnno & "' "
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                    objDSGetSanzioniConfig = ctx.GetDataSet(sSQL, "TBL")
                    If objDSGetSanzioniConfig.Tables(0).Rows.Count = 0 Then
                        strInsUp = "0"
                    Else
                        strInsUp = "1"
                    End If
                    objDSGetSanzioniConfig.Dispose()
                    If strInsUp = Utility.Costanti.AZIONE_UPDATE Then
                        sSQL = "UPDATE TP_GENERALE_ICI set "
                        sSQL += " RIENTRO_LIQ_CONF_AVVISO=" & objUtility.CToBit(strRIENTRO_LIQ_CONF_AVVISO) & " ,"
                        sSQL += " RIENTRO_LIQ_ATTO_DEF=" & objUtility.CToBit(strRIENTRO_LIQ_ATTO_DEF) & " ,"
                        sSQL += " INT_ACCONTO_SALDO=" & objUtility.CToBit(strINT_ACCONTO_SALDO) & " ,"
                        sSQL += " INT_SALDO=" & objUtility.CToBit(strINT_SALDO)
                        sSQL += " WHERE COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
                        sSQL += " AND ANNO=" & objUtility.CStrToDB(strAnno)
                        sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                        intRetVal = ctx.ExecuteNonQuery(sSQL)
                    Else
                        If CInt(strAnno) < 2007 Then
                            strDATA_VERSAMENTO_ACCONTO = strAnno & "0630"
                            strDATA_VERSAMENTO_SALDO = strAnno & "1230"
                        Else
                            strDATA_VERSAMENTO_ACCONTO = strAnno & "0616"
                            strDATA_VERSAMENTO_SALDO = strAnno & "1216"
                        End If

                        'INSERT
                        sSQL = "INSERT INTO TP_GENERALE_ICI "
                        sSQL += " (COD_ENTE,ANNO,RIENTRO_LIQ_CONF_AVVISO,RIENTRO_LIQ_ATTO_DEF,INT_ACCONTO_SALDO,INT_SALDO,DATA_VERSAMENTO_ACCONTO,DATA_VERSAMENTO_SALDO)"
                        sSQL += " VALUES ("
                        sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
                        sSQL += objUtility.CStrToDB(strAnno) & " ,"
                        sSQL += objUtility.CToBit(strRIENTRO_LIQ_CONF_AVVISO) & " ,"
                        sSQL += objUtility.CToBit(strRIENTRO_LIQ_ATTO_DEF) & " ,"
                        sSQL += objUtility.CToBit(strINT_ACCONTO_SALDO) & " ,"
                        sSQL += objUtility.CToBit(strINT_SALDO) & " ,"
                        sSQL += objUtility.CStrToDB(strDATA_VERSAMENTO_ACCONTO) & " ,"
                        sSQL += objUtility.CStrToDB(strDATA_VERSAMENTO_SALDO)
                        sSQL += " )"
                        sSQL = ctx.GetSQL(DBModel.TypeQuery.View, sSQL)
                        intRetVal = ctx.ExecuteNonQuery(sSQL)
                    End If
                    If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                        Throw New Exception("DBOPENgovProvvedimentiUpdate.SetGeneraleICI.errore")
                    End If
                    Return True
                Catch ex As Exception
                    Log.Debug("SetGeneraleICI.errore: ", ex)
                    Return False
                Finally
                    ctx.Dispose()
                End Try
            End Using


        End Function
        '<AutoComplete()>
        'Public Function SetGeneraleICI(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean
        '    Dim sSQL As String
        '    Dim strAnno, strInsUp As String
        '    Dim strRIENTRO_LIQ_CONF_AVVISO, strRIENTRO_LIQ_ATTO_DEF, strINT_ACCONTO_SALDO, strINT_SALDO As String
        '    Dim intRetVal As Integer
        '    Dim objDSGetSanzioniConfig As DataSet = Nothing
        '    Dim objDA As SqlDataAdapter

        '    Dim strDATA_VERSAMENTO_ACCONTO As String

        '    Dim strDATA_VERSAMENTO_SALDO As String

        '    objUtility = New MotoreProvUtility

        '    strAnno = objHashTable("ANNO")

        '    strRIENTRO_LIQ_CONF_AVVISO = objHashTable("RIENTRO_LIQ_CONF_AVVISO")
        '    strRIENTRO_LIQ_ATTO_DEF = objHashTable("RIENTRO_LIQ_ATTO_DEF")
        '    strINT_ACCONTO_SALDO = objHashTable("INT_ACCONTO_SALDO")
        '    strINT_SALDO = objHashTable("INT_SALDO")
        '    strInsUp = objHashTable("INSUP")


        '    sSQL = "SELECT * FROM TP_GENERALE_ICI"
        '    sSQL += " WHERE COD_ENTE = '" & strCODENTE & "' "
        '    sSQL += " AND ANNO = '" & strAnno & "' "

        '    objDSGetSanzioniConfig = New DataSet
        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)
        '    Log.Debug("SetGEnICI->" + sSQL)
        '    objDA = objDBManager.GetPrivateDataAdapter(sSQL)

        '    objDA.Fill(objDSGetSanzioniConfig, "TAB_GENERALE_ICI")

        '    objDA.Dispose()
        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()

        '    End If

        '    If objDSGetSanzioniConfig.Tables(0).Rows.Count = 0 Then
        '        strInsUp = "I"
        '    Else
        '        strInsUp = "U"
        '    End If
        '    objDSGetSanzioniConfig.Dispose()
        '    Select Case strInsUp

        '        Case "U"

        '            'UPDATE
        '            sSQL = "UPDATE TP_GENERALE_ICI set "
        '            sSQL += " RIENTRO_LIQ_CONF_AVVISO=" & objUtility.CToBit(strRIENTRO_LIQ_CONF_AVVISO) & " ,"
        '            sSQL += " RIENTRO_LIQ_ATTO_DEF=" & objUtility.CToBit(strRIENTRO_LIQ_ATTO_DEF) & " ,"
        '            sSQL += " INT_ACCONTO_SALDO=" & objUtility.CToBit(strINT_ACCONTO_SALDO) & " ,"
        '            sSQL += " INT_SALDO=" & objUtility.CToBit(strINT_SALDO)
        '            sSQL += " where COD_ENTE=" & objUtility.CStrToDB(strCODENTE) & " "
        '            sSQL += " and ANNO=" & objUtility.CStrToDB(strAnno)
        '            Log.Debug("SetGeneraleICI->" + sSQL)
        '            objDBManager = New DBManager
        '            objDBManager.Initialize(StringConnectionProvv)
        '            intRetVal = objDBManager.Execute(sSQL)

        '            If Not IsNothing(objDBManager) Then
        '                objDBManager.Kill()
        '                objDBManager.Dispose()

        '            End If
        '            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetAnniProvvedimenti::DBOPENgovProvvedimentiUpdate")
        '            End If
        '            Return True
        '        Case "I"

        '            If CInt(strAnno) < 2007 Then
        '                strDATA_VERSAMENTO_ACCONTO = strAnno & "0630"
        '                strDATA_VERSAMENTO_SALDO = strAnno & "1230"
        '            Else
        '                strDATA_VERSAMENTO_ACCONTO = strAnno & "0616"
        '                strDATA_VERSAMENTO_SALDO = strAnno & "1216"
        '            End If


        '            'INSERT
        '            sSQL = "Insert Into TP_GENERALE_ICI "
        '            sSQL += " (COD_ENTE,ANNO,RIENTRO_LIQ_CONF_AVVISO,RIENTRO_LIQ_ATTO_DEF,INT_ACCONTO_SALDO,INT_SALDO,DATA_VERSAMENTO_ACCONTO,DATA_VERSAMENTO_SALDO)"
        '            sSQL += " values ("
        '            sSQL += objUtility.CStrToDB(strCODENTE) & " ,"
        '            sSQL += objUtility.CStrToDB(strAnno) & " ,"
        '            sSQL += objUtility.CToBit(strRIENTRO_LIQ_CONF_AVVISO) & " ,"
        '            sSQL += objUtility.CToBit(strRIENTRO_LIQ_ATTO_DEF) & " ,"
        '            sSQL += objUtility.CToBit(strINT_ACCONTO_SALDO) & " ,"
        '            sSQL += objUtility.CToBit(strINT_SALDO) & " ,"
        '            sSQL += objUtility.CStrToDB(strDATA_VERSAMENTO_ACCONTO) & " ,"
        '            sSQL += objUtility.CStrToDB(strDATA_VERSAMENTO_SALDO)
        '            sSQL += " )"

        '            Log.Debug("SetGeneraleICI..->" + sSQL)
        '            objDBManager = New DBManager
        '            objDBManager.Initialize(StringConnectionProvv)
        '            intRetVal = objDBManager.Execute(sSQL)

        '            If Not IsNothing(objDBManager) Then
        '                objDBManager.Kill()
        '                objDBManager.Dispose()

        '            End If
        '            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::SetAnniProvvedimenti::DBOPENgovProvvedimentiUpdate")
        '            End If
        '            Return True
        '        Case Else
        '            Return False
        '    End Select



        'End Function

#End Region

#Region "SAVE ELABORAZIONE ACCERTAMENTI"
        ''' <summary>
        ''' Funzione per l'inserimento del provvedimento, delle relative sanzioni, interessi e immobili dichiarati e accertati
        ''' </summary>
        ''' <param name="myAnag"></param>
        ''' <param name="myHashTable"></param>
        ''' <param name="dsSituazioneBasePerSanzInt"></param>
        ''' <param name="dsSanzioni"></param>
        ''' <param name="dsInteressi"></param>
        ''' <param name="ListDichiarato"></param>
        ''' <param name="ListAccertato"></param>
        ''' <param name="Spese"></param>
        ''' <param name="dsSanzioniFase2"></param>
        ''' <param name="ListInteressiFase2"></param>
        ''' <param name="dsVersamentiF2"></param>
        ''' <param name="Operatore">string utente</param>
        ''' <returns>int id provvedimento</returns>
        ''' <revisionHistory>
        ''' <revision date="05/12/2011">
        ''' <strong>le spese devono essere messe dopo l'arrotondamento</strong>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory>
        ''' <revision date="01/07/2014">
        ''' <strong>IMU/TARES</strong>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory>
        ''' <revision date="10/2018">
        ''' <strong>Generazione Massiva Atti</strong>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory><revision date="30/10/2019">per GC non bisogna fare il replace di "." con "," negli importi ma viceversa</revision></revisionHistory>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        <AutoComplete()> Public Function SetProvvedimentiAccertamenti(myDBType As String, StringConnectionProvv As String, IdEnte As String, IdContribuente As Integer, ByVal myAnag As AnagInterface.DettaglioAnagrafica, ByVal myHashTable As Hashtable, ByVal ListBasePerSanzInt As ObjBaseIntSanz, ByVal dsSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal Spese As Double, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal dsVersamentiF2 As DataSet, Operatore As String) As Integer
            Dim fncSelect As New DBOPENgovProvvedimentiSelect
            Dim myAtto As New OggettoAtto
            Dim TIPO_PROVVEDIMENTO As String

            Try
                Log.Debug("Inizio DBOPENgovProvvedimentiUpdate::SetProvvedimentiAccertamenti")
                objUtility = New MotoreProvUtility

                myAtto.COD_ENTE = IdEnte
                myAtto.DATA_ELABORAZIONE = DateTime.Now.ToString("yyyyMMdd")
                myAtto.COD_TRIBUTO = CType(myHashTable("CODTRIBUTO"), String)
                myAtto.COD_CONTRIBUENTE = IdContribuente
                myAtto.ANNO = myHashTable("ANNOACCERTAMENTO")
                myAtto.Provenienza = 1
                TIPO_PROVVEDIMENTO = myHashTable("TIPOPROVVEDIMENTO")

                '### tolto test perché passato sempre =0 ##############################################################################################################################
                'SE VALORE_RITORNO_ACCERTAMENTO=4 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO.
                'SE VALORE_RITORNO_ACCERTAMENTO=5 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO E UN ACCERTAMENTO NON DEFINITIVO (in questo caso cancello però solo l'accertamento e non il pre accertamento)
                'QUINDI SVUOTO I DATI PRESENTI E SALVO QUELLI DEL NUOVO ACCERTAMENTO
                Dim ID_PROCEDIMENTO As Long
                Dim ID_PROVVEDIMENTO As Long
                Dim DATA_CONFERMA As String = ""
                Log.Debug("Chiamata getIDProcedimentoDefinitivoPendenteContribuente")
                If fncSelect.getIDProcedimentoDefinitivoPendenteContribuente(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto.COD_ENTE, myAtto.COD_CONTRIBUENTE, myAtto.ANNO, "A", myAtto.COD_TRIBUTO, ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA) = False Then
                    Return -1
                End If
                Log.Debug("Fine getIDProcedimentoDefinitivoPendenteContribuente")
                'VALORE_RITORNO_ACCERTAMENTO=4 LA DATA CONFERMA E' PER FORZA VUOTA
                If DATA_CONFERMA.CompareTo("") = 0 Then
                    'vuol dire che ho trovato un atto non definitivo oppure nessun atto
                    If ID_PROVVEDIMENTO <> 0 Then
                        If DeleteProvvedimentiAccertamento(StringConnectionProvv, myHashTable, ID_PROCEDIMENTO, ID_PROVVEDIMENTO) = False Then
                            Return -1
                        End If
                    End If
                End If

                myAtto = SetImportiAtto(myAtto, ListBasePerSanzInt, Spese, TIPO_PROVVEDIMENTO, dsVersamentiF2)
                Log.Debug("Chiamata getNewID")
                myAtto.ID_PROVVEDIMENTO = fncSelect.getNewID("PROVVEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                Log.Debug("Fine getNewID:" & myAtto.ID_PROVVEDIMENTO)
                myAtto.NUMERO_AVVISO = objUtility.getNumeroAvviso(myHashTable("ANNOACCERTAMENTO"), myAtto.ID_PROVVEDIMENTO)
                myAtto.NUMERO_ATTO = ""
                Log.Debug("Chiamata getAnagraficaIndirizziSpedizione")
                myAtto.COD_CONTRIBUENTE = myAnag.COD_CONTRIBUENTE
                myAtto.COGNOME = myAnag.Cognome.ToUpper
                myAtto.NOME = myAnag.Nome.ToUpper
                myAtto.CODICE_FISCALE = myAnag.CodiceFiscale
                myAtto.PARTITA_IVA = myAnag.PartitaIva
                myAtto.VIA_RES = myAnag.ViaResidenza.ToUpper
                myAtto.POSIZIONE_CIVICO_RES = myAnag.PosizioneCivicoResidenza
                myAtto.CIVICO_RES = myAnag.CivicoResidenza
                myAtto.ESPONENTE_CIVICO_RES = myAnag.EsponenteCivicoResidenza
                myAtto.CAP_RES = myAnag.CapResidenza
                myAtto.FRAZIONE_RES = myAnag.FrazioneResidenza
                myAtto.CITTA_RES = myAnag.ComuneResidenza
                myAtto.PROVINCIA_RES = myAnag.ProvinciaResidenza
                If Not myAnag.ListSpedizioni Is Nothing Then
                    For Each mySped As AnagInterface.ObjIndirizziSpedizione In myAnag.ListSpedizioni
                        Try
                            myAtto.CO = objUtility.CStrToDB(mySped.CognomeInvio, True, True).ToUpper
                            myAtto.VIA_CO = objUtility.CStrToDB(mySped.ViaRCP, True, True).ToUpper
                            myAtto.POSIZIONE_CIVICO_CO = objUtility.CStrToDB(mySped.PosizioneCivicoRCP, True, True).ToUpper
                            myAtto.CIVICO_CO = objUtility.CStrToDB(mySped.CivicoRCP, True, True).ToUpper
                            myAtto.ESPONENTE_CIVICO_CO = objUtility.CStrToDB(mySped.EsponenteCivicoRCP, True, True).ToUpper
                            myAtto.CAP_CO = objUtility.CStrToDB(mySped.CapRCP, True, True).ToUpper
                            myAtto.FRAZIONE_CO = objUtility.CStrToDB(mySped.FrazioneRCP, True, True).ToUpper
                            myAtto.CITTA_CO = objUtility.CStrToDB(mySped.ComuneRCP, True, True).ToUpper
                            myAtto.PROVINCIA_CO = objUtility.CStrToDB(mySped.ProvinciaRCP, True, True).ToUpper
                        Catch ex As IndexOutOfRangeException
                            myAtto.CO = objUtility.CStrToDB("")
                            myAtto.VIA_CO = objUtility.CStrToDB("")
                            myAtto.POSIZIONE_CIVICO_CO = objUtility.CStrToDB("")
                            myAtto.CIVICO_CO = objUtility.CStrToDB("")
                            myAtto.ESPONENTE_CIVICO_CO = objUtility.CStrToDB("")
                            myAtto.CAP_CO = objUtility.CStrToDB("")
                            myAtto.FRAZIONE_CO = objUtility.CStrToDB("")
                            myAtto.CITTA_CO = objUtility.CStrToDB("")
                            myAtto.PROVINCIA_CO = objUtility.CStrToDB("")
                        End Try
                    Next
                Else
                    Log.Debug("objDSAnagraficaIndirizziSpedizione NULLO!!!")
                End If
                Log.Debug("Chiamata INSERT INTO PROVVEDIMENTI")
                If SetProvvedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, Operatore) <= 0 Then
                    Return -1
                End If
                Log.Debug("Chiamata INTO TAB_PROCEDIMENTI")
                Dim IdProcedimento As Integer
                IdProcedimento = fncSelect.getNewID("TAB_PROCEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                If SetProcedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, IdProcedimento, TIPO_PROVVEDIMENTO) <= 0 Then
                    Return -1
                End If
                Log.Debug("Chiamata SetDICHIARATO_ICI_ACCERTAMENTI")
                If SetUIDichAcc(IdProcedimento, ListDichiarato, ListAccertato, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) < 1 Then
                    Return -1
                End If
                Log.Debug("Chiamata setDETTAGLIO_VOCI_ACCERTAMENTI")
                If setDETTAGLIO_VOCI_ACCERTAMENTI(IdEnte, myAtto.COD_CONTRIBUENTE, myAtto.ID_PROVVEDIMENTO, dsSanzioni, ListInteressi, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) < 1 Then
                    Return -1
                End If
                'SALVO, SE PRESENTI, I DATI DEGLI IMMOBILI E DELLE VOCI SCATURITI DALLA FASE 2 DI PRE ACCERTAMENTO
                Log.Debug("Chiamata SetDETTAGLIO_VOCI_LIQUIDAZIONI")
                SetDETTAGLIO_VOCI_LIQUIDAZIONI(IdEnte, myAtto.ID_PROVVEDIMENTO, dsSanzioniFase2, ListInteressiFase2, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                Log.Debug("Chiamata SetVERSAMENTI_LIQUIDAZIONI")
                If SetVERSAMENTI_LIQUIDAZIONI(IdEnte, dsVersamentiF2, IdProcedimento, 2, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) < 1 Then
                    Return -1
                End If
                If myHashTable("TIPO_OPERAZIONE_RETTIFICA") = True Then
                    Log.Debug("Chiamata SetPROVVEDIMENTIRETTIFICATI")
                    SetPROVVEDIMENTIRETTIFICATI(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto.COD_CONTRIBUENTE, myAtto.COD_TRIBUTO, myAtto.COD_ENTE, myAtto.ID_PROVVEDIMENTO, myHashTable("ID_PROVVEDIMENTO_RETTIFICA"), myHashTable("DATA_RETTIFICA"), True, myHashTable("DATA_ANNULLAMENTO"))
                End If

                Log.Debug("Terminata  DBOPENgovProvvedimentiUpdate::SetProvvedimentiAccertamenti")
                Return myAtto.ID_PROVVEDIMENTO
            Catch ex As Exception
                Log.Error("DBOPENgovProvvedimentiUpdate::SetProvvedimentiAccertamenti::" & ex.Message)
                Return -1
            End Try
        End Function
        ''' <summary>
        ''' Funzione che valorizza tutti gli importi dell'atto.
        ''' La funzione ha come parametro il separatore decimale per la conversione corretta sia in ambiente nostro che GC.
        ''' </summary>
        ''' <param name="oAtto"></param>
        ''' <returns></returns>
        Private Function SetImportiAtto(ByVal oAtto As OggettoAtto, ByVal ListBaseSanzInt As ObjBaseIntSanz, ByVal impSpese As Double, ByVal sTipoProvvedimento As String, ByVal dsVersamentiF2 As DataSet) As OggettoAtto
            Dim myAtto As New OggettoAtto
            Dim IsFromPuntuale As Boolean = False
            Try
                myAtto = oAtto
                myAtto.IMPORTO_DIFFERENZA_IMPOSTA = 0
                myAtto.IMPORTO_SANZIONI = 0
                myAtto.IMPORTO_SANZIONI_RIDOTTO = 0
                myAtto.IMPORTO_INTERESSI = 0
                myAtto.IMPORTO_SPESE = impSpese
                myAtto.IMPORTO_ALTRO = 0

                myAtto.IMPORTO_DICHIARATO_F2 = 0
                myAtto.IMPORTO_ACCERTATO_ACC = 0
                IsFromPuntuale = False
                Log.Debug("dblIMPORTO_DIFF_IMPOSTA::" & StringOperation.FormatString(ListBaseSanzInt.DifferenzaImposta))
                Log.Debug("dblIMPORTO_SANZIONI::   " & StringOperation.FormatString(ListBaseSanzInt.Sanzioni))
                Log.Debug("dblIMPORTO_SANZIONI_RIDOTTO::" & StringOperation.FormatString(ListBaseSanzInt.SanzioniRidotto))
                Log.Debug("dblIMPORTO_INTERESSI::  " & StringOperation.FormatString(ListBaseSanzInt.Interessi))

                myAtto.IMPORTO_DIFFERENZA_IMPOSTA = ListBaseSanzInt.DifferenzaImposta
                myAtto.IMPORTO_SANZIONI = ListBaseSanzInt.Sanzioni
                myAtto.IMPORTO_SANZIONI_RIDOTTO = ListBaseSanzInt.SanzioniRidotto
                Log.Debug(" dblIMPORTO_SANZIONI - (dblIMPORTO_SANZIONI_RIDOTTO * CInt(myrow(QUOTARIDUZIONESANZIONI)))" & myAtto.IMPORTO_SANZIONI.ToString & ": " & myAtto.IMPORTO_SANZIONI_RIDOTTO.ToString & ":" & ListBaseSanzInt.QuotaRiduzione.ToString)
                myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = myAtto.IMPORTO_SANZIONI - (myAtto.IMPORTO_SANZIONI_RIDOTTO * ListBaseSanzInt.QuotaRiduzione)
                myAtto.IMPORTO_INTERESSI = ListBaseSanzInt.Interessi

                myAtto.IMPORTO_DICHIARATO_F2 = ListBaseSanzInt.Dichiarato
                myAtto.IMPORTO_ACCERTATO_ACC = ListBaseSanzInt.Accertato

                myAtto.IMPORTO_SANZIONI_F2 = ListBaseSanzInt.SanzioniF2
                myAtto.IMPORTO_INTERESSI_F2 = ListBaseSanzInt.InteressiF2
                myAtto.IMPORTO_SANZIONI_ACC = ListBaseSanzInt.SanzioniAcc
                myAtto.IMPORTO_INTERESSI_ACC = ListBaseSanzInt.InteressiAcc

                Log.Debug("(dblIMPORTO_SANZIONI_RIDOTTO * CInt(myrow(QUOTARIDUZIONESANZIONI)::" & (myAtto.IMPORTO_SANZIONI_RIDOTTO * ListBaseSanzInt.QuotaRiduzione).ToString)
                myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = myAtto.IMPORTO_SANZIONI - (myAtto.IMPORTO_SANZIONI_RIDOTTO * ListBaseSanzInt.QuotaRiduzione)

                If myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI < 0 Then
                    myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = 0
                End If
                myAtto.IMPORTO_SPESE = impSpese
                myAtto.IMPORTO_ALTRO = 0
                Log.Debug("dblIMPORTO_DIFF_IMPOSTA:: " & myAtto.IMPORTO_DIFFERENZA_IMPOSTA.ToString)
                Log.Debug("dblIMPORTO_SANZIONI::" & myAtto.IMPORTO_SANZIONI.ToString)
                Log.Debug("dblIMPORTO_SANZIONI_RIDOTTO::" & myAtto.IMPORTO_SANZIONI_RIDOTTO.ToString)
                Log.Debug("dblIMPORTO_INTERESSI::" & myAtto.IMPORTO_INTERESSI.ToString)
                'in caso di annullamento devono essere a zero
                If sTipoProvvedimento = 7 Then
                    myAtto.IMPORTO_DIFFERENZA_IMPOSTA = 0
                    myAtto.IMPORTO_SANZIONI = 0
                    myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = 0
                    myAtto.IMPORTO_SANZIONI_RIDOTTO = 0
                    myAtto.IMPORTO_INTERESSI = 0
                    myAtto.IMPORTO_ALTRO = 0
                    myAtto.IMPORTO_SPESE = 0
                End If
                'Importo versamenti
                Log.Debug("Importo versamenti")
                myAtto.IMPORTO_VERSATO_F2 = 0
                If Not dsVersamentiF2 Is Nothing Then
                    If dsVersamentiF2.Tables.Count > 0 Then
                        For Each myRow As DataRow In dsVersamentiF2.Tables(0).Rows
                            myAtto.IMPORTO_VERSATO_F2 += myRow("IMPORTOPAGATO")
                        Next
                    End If
                End If
                Log.Debug("dblIMPORTOPAGATO =" & myAtto.IMPORTO_VERSATO_F2)
                'importo totale e arrotondamento
                myAtto.IMPORTO_TOTALE = myAtto.IMPORTO_DIFFERENZA_IMPOSTA + myAtto.IMPORTO_SANZIONI + myAtto.IMPORTO_INTERESSI + myAtto.IMPORTO_ALTRO
                myAtto.IMPORTO_ARROTONDAMENTO = ImportoArrotondato(myAtto.IMPORTO_TOTALE) - myAtto.IMPORTO_TOTALE
                myAtto.IMPORTO_TOTALE = ImportoArrotondato(myAtto.IMPORTO_TOTALE) + myAtto.IMPORTO_SPESE
                'importo totale ridotto e arrotondamento ridotto
                myAtto.IMPORTO_TOTALE_RIDOTTO = myAtto.IMPORTO_DIFFERENZA_IMPOSTA + myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI + myAtto.IMPORTO_SANZIONI_RIDOTTO + myAtto.IMPORTO_INTERESSI + myAtto.IMPORTO_ALTRO
                myAtto.IMPORTO_ARROTONDAMENTO_RIDOTTO = ImportoArrotondato(myAtto.IMPORTO_TOTALE_RIDOTTO) - myAtto.IMPORTO_TOTALE_RIDOTTO
                myAtto.IMPORTO_TOTALE_RIDOTTO = ImportoArrotondato(myAtto.IMPORTO_TOTALE_RIDOTTO) + myAtto.IMPORTO_SPESE
                myAtto.IMPORTO_DIFFERENZA_IMPOSTA_F2 = myAtto.IMPORTO_DICHIARATO_F2 - myAtto.IMPORTO_VERSATO_F2
                myAtto.IMPORTO_TOTALE_F2 = (myAtto.IMPORTO_DICHIARATO_F2 - myAtto.IMPORTO_VERSATO_F2) + myAtto.IMPORTO_SANZIONI_F2 + myAtto.IMPORTO_INTERESSI_F2
                myAtto.IMPORTO_DIFFERENZA_IMPOSTA_ACC = myAtto.IMPORTO_ACCERTATO_ACC - myAtto.IMPORTO_DICHIARATO_F2
                myAtto.IMPORTO_TOTALE_ACC = (myAtto.IMPORTO_ACCERTATO_ACC - myAtto.IMPORTO_DICHIARATO_F2) + myAtto.IMPORTO_SANZIONI_ACC + myAtto.IMPORTO_INTERESSI_ACC
            Catch ex As Exception
                Throw New Exception("DBOPENgovProvvedimentiUpdate.SetImportiAtto.errore::", ex)
            End Try
            Return myAtto
        End Function
        ''' <summary>
        ''' Funzione per l'inserimento del provvedimento
        ''' </summary>
        ''' <param name="myDBType">string tipo di DB SQL/MySQL</param>
        ''' <param name="myStringConnection">string stringa di connessione</param>
        ''' <param name="myItem">OggettoAtto oggetto da gestire</param>
        ''' <param name="Operatore">string utente</param>
        ''' <returns>int identificativo riga</returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        Public Function SetProvvedimento(myDBType As String, myStringConnection As String, myItem As OggettoAtto, Operatore As String) As Integer
            Dim sSQL As String = ""
            Dim nRet As Integer = -1
            Dim myDataView As New DataView

            Try
                Dim oDbManagerRepository As New DBModel(myDBType, myStringConnection)
                Using ctx As DBModel = oDbManagerRepository
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_PROVVEDIMENTI_IU", "ID_PROVVEDIMENTO", "COD_ENTE", "NUMERO_AVVISO", "NUMERO_ATTO", "COD_TRIBUTO", "COD_CONTRIBUENTE" _
                        , "COGNOME", "NOME", "CODICE_FISCALE", "PARTITA_IVA", "VIA_RES", "POSIZIONE_CIVICO_RES", "CIVICO_RES", "ESPONENTE_CIVICO_RES", "CAP_RES", "FRAZIONE_RES", "CITTA_RES", "PROVINCIA_RES" _
                        , "CO", "VIA_CO", "POSIZIONE_CIVICO_CO", "CIVICO_CO", "ESPONENTE_CIVICO_CO", "CAP_CO", "FRAZIONE_CO", "CITTA_CO", "PROVINCIA_CO" _
                        , "DATA_ELABORAZIONE" _
                        , "IMPORTO_DIFFERENZA_IMPOSTA", "IMPORTO_SANZIONI", "IMPORTO_SANZIONI_RIDOTTO", "IMPORTO_INTERESSI", "IMPORTO_SPESE", "IMPORTO_ALTRO", "IMPORTO_SENZA_ARROTONDAMENTO", "IMPORTO_ARROTONDAMENTO", "IMPORTO_TOTALE", "IMPORTO_TOTALE_RIDOTTO", "IMPORTO_ARROTONDAMENTO_RIDOTTO" _
                        , "IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI", "IMPORTO_DICHIARATO_F2", "IMPORTO_VERSATO_F2", "IMPORTO_DIFFERENZA_IMPOSTA_F2", "IMPORTO_SANZIONI_F2", "IMPORTO_INTERESSI_F2", "IMPORTO_TOTALE_F2" _
                        , "IMPORTO_ACCERTATO_ACC", "IMPORTO_DIFFERENZA_IMPOSTA_ACC", "IMPORTO_SANZIONI_ACC", "IMPORTO_SANZIONI_RIDOTTE_ACC", "IMPORTO_INTERESSI_ACC", "IMPORTO_TOTALE_ACC" _
                        , "DATA_CONFERMA", "DATA_STAMPA" _
                        , "DATA_CONSEGNA_AVVISO", "DATA_NOTIFICA_AVVISO", "DATA_SOSPENSIONE_AVVISO_AUTOTUTELA", "DATA_PRESENTAZIONE_RICORSO", "DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA", "DATA_SENTENZA", "NOTE_PROVINCIALE" _
                        , "FLAG_CONCILIAZIONE_G", "NOTE_CONCILIAZIONE_G" _
                        , "DATA_PRESENTAZIONE_RICORSO_REGIONALE", "DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE" _
                        , "DATA_SENTENZA_REGIONALE", "NOTE_REGIONALE" _
                        , "DATA_PRESENTAZIONE_RICORSO_CASSAZIONE", "DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE" _
                        , "DATA_SENTENZA_CASSAZIONE", "NOTE_CASSAZIONE" _
                        , "DATA_VERSAMENTO_SOLUZIONE_UNICA", "DATA_CONCESSIONE_RATEIZZAZIONE" _
                        , "FLAG_ACCERTAMENTO", "ESITO_ACCERTAMENTO", "TERMINE_RICORSO_ACC", "NOTE_ACCERTAMENTO" _
                        , "NOTE_GENERALI_ATTO", "DATA_COATTIVO", "DATA_IRREPERIBILE" _
                        , "Provenienza", "IDELABORAZIONE", "OPERATORE")
                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("ID_PROVVEDIMENTO", myItem.ID_PROVVEDIMENTO) _
                        , ctx.GetParam("COD_ENTE", myItem.COD_ENTE) _
                        , ctx.GetParam("NUMERO_AVVISO", myItem.NUMERO_AVVISO) _
                        , ctx.GetParam("NUMERO_ATTO", myItem.NUMERO_ATTO) _
                        , ctx.GetParam("COD_TRIBUTO", myItem.COD_TRIBUTO) _
                        , ctx.GetParam("COD_CONTRIBUENTE", myItem.COD_CONTRIBUENTE) _
                        , ctx.GetParam("COGNOME", myItem.COGNOME) _
                        , ctx.GetParam("NOME", myItem.NOME) _
                        , ctx.GetParam("CODICE_FISCALE", myItem.CODICE_FISCALE) _
                        , ctx.GetParam("PARTITA_IVA", myItem.PARTITA_IVA) _
                        , ctx.GetParam("VIA_RES", myItem.VIA_RES) _
                        , ctx.GetParam("POSIZIONE_CIVICO_RES", myItem.POSIZIONE_CIVICO_RES) _
                        , ctx.GetParam("CIVICO_RES", myItem.CIVICO_RES) _
                        , ctx.GetParam("ESPONENTE_CIVICO_RES", myItem.ESPONENTE_CIVICO_RES) _
                        , ctx.GetParam("CAP_RES", myItem.CAP_RES) _
                        , ctx.GetParam("FRAZIONE_RES", myItem.FRAZIONE_RES) _
                        , ctx.GetParam("CITTA_RES", myItem.CITTA_RES) _
                        , ctx.GetParam("PROVINCIA_RES", myItem.PROVINCIA_RES) _
                        , ctx.GetParam("CO", myItem.CO) _
                        , ctx.GetParam("VIA_CO", myItem.VIA_CO) _
                        , ctx.GetParam("POSIZIONE_CIVICO_CO", myItem.POSIZIONE_CIVICO_CO) _
                        , ctx.GetParam("CIVICO_CO", myItem.CIVICO_CO) _
                        , ctx.GetParam("ESPONENTE_CIVICO_CO", myItem.ESPONENTE_CIVICO_CO) _
                        , ctx.GetParam("CAP_CO", myItem.CAP_CO) _
                        , ctx.GetParam("FRAZIONE_CO", myItem.FRAZIONE_CO) _
                        , ctx.GetParam("CITTA_CO", myItem.CITTA_CO) _
                        , ctx.GetParam("PROVINCIA_CO", myItem.PROVINCIA_CO) _
                        , ctx.GetParam("DATA_ELABORAZIONE", myItem.DATA_ELABORAZIONE) _
                        , ctx.GetParam("IMPORTO_DIFFERENZA_IMPOSTA", myItem.IMPORTO_DIFFERENZA_IMPOSTA) _
                        , ctx.GetParam("IMPORTO_SANZIONI", myItem.IMPORTO_SANZIONI) _
                        , ctx.GetParam("IMPORTO_SANZIONI_RIDOTTO", myItem.IMPORTO_SANZIONI_RIDOTTO) _
                        , ctx.GetParam("IMPORTO_INTERESSI", myItem.IMPORTO_INTERESSI) _
                        , ctx.GetParam("IMPORTO_SPESE", myItem.IMPORTO_SPESE) _
                        , ctx.GetParam("IMPORTO_ALTRO", myItem.IMPORTO_ALTRO) _
                        , ctx.GetParam("IMPORTO_SENZA_ARROTONDAMENTO", myItem.IMPORTO_SENZA_ARROTONDAMENTO) _
                        , ctx.GetParam("IMPORTO_ARROTONDAMENTO", myItem.IMPORTO_ARROTONDAMENTO) _
                        , ctx.GetParam("IMPORTO_TOTALE", myItem.IMPORTO_TOTALE) _
                        , ctx.GetParam("IMPORTO_TOTALE_RIDOTTO", myItem.IMPORTO_TOTALE_RIDOTTO) _
                        , ctx.GetParam("IMPORTO_ARROTONDAMENTO_RIDOTTO", myItem.IMPORTO_ARROTONDAMENTO_RIDOTTO) _
                        , ctx.GetParam("IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI", myItem.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI) _
                        , ctx.GetParam("IMPORTO_DICHIARATO_F2", myItem.IMPORTO_DICHIARATO_F2) _
                        , ctx.GetParam("IMPORTO_VERSATO_F2", myItem.IMPORTO_VERSATO_F2) _
                        , ctx.GetParam("IMPORTO_DIFFERENZA_IMPOSTA_F2", myItem.IMPORTO_DIFFERENZA_IMPOSTA_F2) _
                        , ctx.GetParam("IMPORTO_SANZIONI_F2", myItem.IMPORTO_SANZIONI_F2) _
                        , ctx.GetParam("IMPORTO_INTERESSI_F2", myItem.IMPORTO_INTERESSI_F2) _
                        , ctx.GetParam("IMPORTO_TOTALE_F2", myItem.IMPORTO_TOTALE_F2) _
                        , ctx.GetParam("IMPORTO_ACCERTATO_ACC", myItem.IMPORTO_ACCERTATO_ACC) _
                        , ctx.GetParam("IMPORTO_DIFFERENZA_IMPOSTA_ACC", myItem.IMPORTO_DIFFERENZA_IMPOSTA_ACC) _
                        , ctx.GetParam("IMPORTO_SANZIONI_ACC", myItem.IMPORTO_SANZIONI_ACC) _
                        , ctx.GetParam("IMPORTO_SANZIONI_RIDOTTE_ACC", myItem.IMPORTO_SANZIONI_RIDOTTE_ACC) _
                        , ctx.GetParam("IMPORTO_INTERESSI_ACC", myItem.IMPORTO_INTERESSI_ACC) _
                        , ctx.GetParam("IMPORTO_TOTALE_ACC", myItem.IMPORTO_TOTALE_ACC) _
                        , ctx.GetParam("DATA_CONSEGNA_AVVISO", myItem.DATA_CONSEGNA_AVVISO) _
                        , ctx.GetParam("DATA_CONFERMA", myItem.DATA_CONFERMA) _
                        , ctx.GetParam("DATA_STAMPA", myItem.DATA_STAMPA) _
                        , ctx.GetParam("DATA_NOTIFICA_AVVISO", myItem.DATA_NOTIFICA_AVVISO) _
                        , ctx.GetParam("DATA_SOSPENSIONE_AVVISO_AUTOTUTELA", myItem.DATA_SOSPENSIONE_AVVISO_AUTOTUTELA) _
                        , ctx.GetParam("DATA_PRESENTAZIONE_RICORSO", myItem.DATA_PRESENTAZIONE_RICORSO) _
                        , ctx.GetParam("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA", myItem.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA) _
                        , ctx.GetParam("DATA_SENTENZA", myItem.DATA_SENTENZA) _
                        , ctx.GetParam("NOTE_PROVINCIALE", myItem.NOTE_PROVINCIALE) _
                        , ctx.GetParam("FLAG_CONCILIAZIONE_G", myItem.FLAG_CONCILIAZIONE_G) _
                        , ctx.GetParam("NOTE_CONCILIAZIONE_G", myItem.NOTE_CONCILIAZIONE_G) _
                        , ctx.GetParam("DATA_PRESENTAZIONE_RICORSO_REGIONALE", myItem.DATA_PRESENTAZIONE_RICORSO_REGIONALE) _
                        , ctx.GetParam("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE", myItem.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE) _
                        , ctx.GetParam("DATA_SENTENZA_REGIONALE", myItem.DATA_SENTENZA_REGIONALE) _
                        , ctx.GetParam("NOTE_REGIONALE", myItem.NOTE_REGIONALE) _
                        , ctx.GetParam("DATA_PRESENTAZIONE_RICORSO_CASSAZIONE", myItem.DATA_PRESENTAZIONE_RICORSO_CASSAZIONE) _
                        , ctx.GetParam("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE", myItem.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE) _
                        , ctx.GetParam("DATA_SENTENZA_CASSAZIONE", myItem.DATA_SENTENZA_CASSAZIONE) _
                        , ctx.GetParam("NOTE_CASSAZIONE", myItem.NOTE_CASSAZIONE) _
                        , ctx.GetParam("DATA_VERSAMENTO_SOLUZIONE_UNICA", myItem.DATA_VERSAMENTO_SOLUZIONE_UNICA) _
                        , ctx.GetParam("DATA_CONCESSIONE_RATEIZZAZIONE", myItem.DATA_CONCESSIONE_RATEIZZAZIONE) _
                        , ctx.GetParam("FLAG_ACCERTAMENTO", myItem.FLAG_ACCERTAMENTO) _
                        , ctx.GetParam("ESITO_ACCERTAMENTO", myItem.ESITO_ACCERTAMENTO) _
                        , ctx.GetParam("TERMINE_RICORSO_ACC", myItem.TERMINE_RICORSO_ACC) _
                        , ctx.GetParam("NOTE_ACCERTAMENTO", myItem.NOTE_ACCERTAMENTO) _
                        , ctx.GetParam("NOTE_GENERALI_ATTO", myItem.NOTE_GENERALI_ATTO) _
                        , ctx.GetParam("DATA_COATTIVO", myItem.DATA_COATTIVO) _
                        , ctx.GetParam("DATA_IRREPERIBILE", myItem.DATA_IRREPERIBILE) _
                        , ctx.GetParam("Provenienza", myItem.Provenienza) _
                        , ctx.GetParam("IDELABORAZIONE", myItem.IDRUOLO) _
                        , ctx.GetParam("OPERATORE", Operatore)
                    )
                    ctx.Dispose()
                End Using
                For Each dtMyRow As DataRowView In myDataView
                    nRet = StringOperation.FormatInt(dtMyRow("ID"))
                Next
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.SetProvvedimento.errore::", ex)
                nRet = -1
            Finally
                myDataView.Dispose()
            End Try
            Return nRet
        End Function
        'Private Function SetProvvedimento(myStringConnection As String, myItem As OggettoAtto) As Integer
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim nRet As Integer

        '    Try
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
        '        cmdMyCommand.Connection.Open()
        '        'Valorizzo i parameters:
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.AddWithValue("@ID_PROVVEDIMENTO", myItem.ID_PROVVEDIMENTO)
        '        cmdMyCommand.Parameters.AddWithValue("@COD_ENTE", myItem.COD_ENTE)
        '        cmdMyCommand.Parameters.AddWithValue("@NUMERO_AVVISO", myItem.NUMERO_AVVISO)
        '        cmdMyCommand.Parameters.AddWithValue("@NUMERO_ATTO", myItem.NUMERO_ATTO)
        '        cmdMyCommand.Parameters.AddWithValue("@COD_TRIBUTO", myItem.COD_TRIBUTO)
        '        cmdMyCommand.Parameters.AddWithValue("@COD_CONTRIBUENTE", myItem.COD_CONTRIBUENTE)
        '        cmdMyCommand.Parameters.AddWithValue("@COGNOME", myItem.COGNOME)
        '        cmdMyCommand.Parameters.AddWithValue("@NOME", myItem.NOME)
        '        cmdMyCommand.Parameters.AddWithValue("@CODICE_FISCALE", myItem.CODICE_FISCALE)
        '        cmdMyCommand.Parameters.AddWithValue("@PARTITA_IVA", myItem.PARTITA_IVA)
        '        cmdMyCommand.Parameters.AddWithValue("@VIA_RES", myItem.VIA_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@POSIZIONE_CIVICO_RES", myItem.POSIZIONE_CIVICO_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@CIVICO_RES", myItem.CIVICO_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@ESPONENTE_CIVICO_RES", myItem.ESPONENTE_CIVICO_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@CAP_RES", myItem.CAP_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@FRAZIONE_RES", myItem.FRAZIONE_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@CITTA_RES", myItem.CITTA_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@PROVINCIA_RES", myItem.PROVINCIA_RES)
        '        cmdMyCommand.Parameters.AddWithValue("@CO", myItem.CO)
        '        cmdMyCommand.Parameters.AddWithValue("@VIA_CO", myItem.VIA_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@POSIZIONE_CIVICO_CO", myItem.POSIZIONE_CIVICO_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@CIVICO_CO", myItem.CIVICO_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@ESPONENTE_CIVICO_CO", myItem.ESPONENTE_CIVICO_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@CAP_CO", myItem.CAP_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@FRAZIONE_CO", myItem.FRAZIONE_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@CITTA_CO", myItem.CITTA_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@PROVINCIA_CO", myItem.PROVINCIA_CO)
        '        cmdMyCommand.Parameters.AddWithValue("@DATA_ELABORAZIONE", myItem.DATA_ELABORAZIONE)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_DIFFERENZA_IMPOSTA", myItem.IMPORTO_DIFFERENZA_IMPOSTA)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI", myItem.IMPORTO_SANZIONI)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_RIDOTTO", myItem.IMPORTO_SANZIONI_RIDOTTO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_INTERESSI", myItem.IMPORTO_INTERESSI)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SPESE", myItem.IMPORTO_SPESE)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_ALTRO", myItem.IMPORTO_ALTRO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SENZA_ARROTONDAMENTO", myItem.IMPORTO_SENZA_ARROTONDAMENTO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_ARROTONDAMENTO", myItem.IMPORTO_ARROTONDAMENTO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_TOTALE", myItem.IMPORTO_TOTALE)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_TOTALE_RIDOTTO", myItem.IMPORTO_TOTALE_RIDOTTO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_ARROTONDAMENTO_RIDOTTO", myItem.IMPORTO_ARROTONDAMENTO_RIDOTTO)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI", myItem.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_DICHIARATO_F2", myItem.IMPORTO_DICHIARATO_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_VERSATO_F2", myItem.IMPORTO_VERSATO_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_DIFFERENZA_IMPOSTA_F2", myItem.IMPORTO_DIFFERENZA_IMPOSTA_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_F2", myItem.IMPORTO_SANZIONI_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_INTERESSI_F2", myItem.IMPORTO_INTERESSI_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_TOTALE_F2", myItem.IMPORTO_TOTALE_F2)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_ACCERTATO_ACC", myItem.IMPORTO_ACCERTATO_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_DIFFERENZA_IMPOSTA_ACC", myItem.IMPORTO_DIFFERENZA_IMPOSTA_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_ACC", myItem.IMPORTO_SANZIONI_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_RIDOTTE_ACC", myItem.IMPORTO_SANZIONI_RIDOTTE_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_INTERESSI_ACC", myItem.IMPORTO_INTERESSI_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@IMPORTO_TOTALE_ACC", myItem.IMPORTO_TOTALE_ACC)
        '        cmdMyCommand.Parameters.AddWithValue("@NOTE_CASSAZIONE", myItem.NOTE_CASSAZIONE)
        '        cmdMyCommand.Parameters.AddWithValue("@Provenienza", myItem.Provenienza)
        '        cmdMyCommand.Parameters.AddWithValue("@IDELABORAZIONE", myItem.IDRUOLO)
        '        cmdMyCommand.CommandText = "prc_PROVVEDIMENTI_IU"
        '        cmdMyCommand.Parameters("@ID_PROVVEDIMENTO").Direction = ParameterDirection.InputOutput
        '        'eseguo la query
        '        cmdMyCommand.ExecuteNonQuery()
        '        nRet = cmdMyCommand.Parameters("@ID_PROVVEDIMENTO").Value
        '    Catch ex As Exception
        '        Log.Debug("DBOPENgovProvvedimentiUpdate.SetProvvedimento.errore::", ex)
        '        Log.Debug("SetProvvedimento::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        nRet = -1
        '    Finally
        '        cmdMyCommand.Dispose()
        '        cmdMyCommand.Connection.Close()
        '    End Try
        '    Return nRet
        'End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myDBType"></param>
        ''' <param name="myStringConnection"></param>
        ''' <param name="myItem"></param>
        ''' <param name="IdProcedimento"></param>
        ''' <param name="TipoProvvedimento"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        Private Function SetProcedimento(myDBType As String, myStringConnection As String, myItem As OggettoAtto, IdProcedimento As Integer, TipoProvvedimento As Integer) As Integer
            Dim sSQL As String = ""
            Dim nRet As Integer = -1
            Dim myDataView As New DataView

            Try
                Dim oDbManagerRepository As New DBModel(myDBType, myStringConnection)
                Using ctx As DBModel = oDbManagerRepository
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_TAB_PROCEDIMENTI_IU", "ID_PROCEDIMENTO", "COD_ENTE", "COD_TRIBUTO", "ANNO", "COD_CONTRIBUENTE", "ID_PROVVEDIMENTO" _
                        , "COD_TIPO_PROCEDIMENTO", "COD_TIPO_PROVVEDIMENTO", "DATA_INIZIO_PROCEDIMENTO", "TOTALE_DICHIARATO", "TOTALE_VERSATO", "TOTALE_ACCERTATO")
                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("ID_PROCEDIMENTO", IdProcedimento) _
                        , ctx.GetParam("COD_ENTE", myItem.COD_ENTE) _
                        , ctx.GetParam("COD_TRIBUTO", myItem.COD_TRIBUTO) _
                        , ctx.GetParam("ANNO", myItem.ANNO) _
                        , ctx.GetParam("COD_CONTRIBUENTE", myItem.COD_CONTRIBUENTE) _
                        , ctx.GetParam("ID_PROVVEDIMENTO", myItem.ID_PROVVEDIMENTO) _
                        , ctx.GetParam("COD_TIPO_PROCEDIMENTO", OggettoAtto.Procedimento.Accertamento) _
                        , ctx.GetParam("COD_TIPO_PROVVEDIMENTO", TipoProvvedimento) _
                        , ctx.GetParam("DATA_INIZIO_PROCEDIMENTO", myItem.DATA_ELABORAZIONE) _
                        , ctx.GetParam("TOTALE_DICHIARATO", myItem.IMPORTO_DICHIARATO_F2) _
                        , ctx.GetParam("TOTALE_VERSATO", myItem.IMPORTO_VERSATO_F2) _
                        , ctx.GetParam("TOTALE_ACCERTATO", myItem.IMPORTO_ACCERTATO_ACC)
                    )
                    ctx.Dispose()
                End Using
                For Each dtMyRow As DataRowView In myDataView
                    nRet = StringOperation.FormatInt(dtMyRow("ID"))
                Next
            Catch ex As Exception
                Log.Debug("DBOPENgovProvvedimentiUpdate.SetProcedimento.errore::", ex)
                nRet = -1
            Finally
                myDataView.Dispose()
            End Try
            Return nRet
        End Function
        'Private Function SetProcedimento(myStringConnection As String, myItem As OggettoAtto, IdProcedimento As Integer, TipoProvvedimento As Integer) As Integer
        '    Dim cmdMyCommand As New SqlCommand
        '    Dim nRet As Integer

        '    Try
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'Valorizzo la connessione
        '        cmdMyCommand.Connection = New SqlClient.SqlConnection(myStringConnection)
        '        cmdMyCommand.Connection.Open()
        '        'Valorizzo i parameters:
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROCEDIMENTO", SqlDbType.Int)).Value = IdProcedimento
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.VarChar)).Value = myItem.COD_ENTE
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TRIBUTO", SqlDbType.VarChar)).Value = myItem.COD_TRIBUTO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.VarChar)).Value = myItem.ANNO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_CONTRIBUENTE", SqlDbType.Int)).Value = myItem.COD_CONTRIBUENTE
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.VarChar)).Value = myItem.ID_PROVVEDIMENTO
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROCEDIMENTO", SqlDbType.VarChar)).Value = OggettoAtto.Procedimento.Accertamento
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = TipoProvvedimento
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO_PROCEDIMENTO", SqlDbType.VarChar)).Value = myItem.DATA_ELABORAZIONE
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TOTALE_DICHIARATO", SqlDbType.Decimal)).Value = myItem.IMPORTO_DICHIARATO_F2
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TOTALE_VERSATO", SqlDbType.Decimal)).Value = myItem.IMPORTO_VERSATO_F2
        '        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TOTALE_ACCERTATO", SqlDbType.Decimal)).Value = myItem.IMPORTO_ACCERTATO_ACC
        '        cmdMyCommand.CommandText = "prc_TAB_PROCEDIMENTI_IU"
        '        cmdMyCommand.Parameters("@ID_PROCEDIMENTO").Direction = ParameterDirection.InputOutput
        '        'eseguo la query
        '        cmdMyCommand.ExecuteNonQuery()
        '        nRet = cmdMyCommand.Parameters("@ID_PROCEDIMENTO").Value
        '    Catch ex As Exception
        '        Log.Debug("DBOPENgovProvvedimentiUpdate.SetProcedimento.errore::", ex)
        '        Log.Debug("SetProcedimento::query::" + cmdMyCommand.CommandText + Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        nRet = -1
        '    Finally
        '        cmdMyCommand.Dispose()
        '        cmdMyCommand.Connection.Close()
        '    End Try
        '    Return nRet
        'End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="IdEnte"></param>
        ''' <param name="intCODContribuente"></param>
        ''' <param name="lngIDProvvedimento"></param>
        ''' <param name="objSanzioni"></param>
        ''' <param name="ObjInteressiSanzioni"></param>
        ''' <param name="myStringConnection"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Private Function setDETTAGLIO_VOCI_ACCERTAMENTI(IdEnte As String, ByVal intCODContribuente As Integer, ByVal lngIDProvvedimento As Long, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, myStringConnection As String) As Long
            '*** 20140701 - IMU/TARES ***
            If DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, objSanzioni, myStringConnection) < 1 Then
                Return -1
            End If
            If DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, ListInteressi, myStringConnection) < 1 Then
                Return -1
            End If
            '*** ***
            Return 1
        End Function
        'Private Function setDETTAGLIO_VOCI_ACCERTAMENTI(IdEnte As String, ByVal intCODContribuente As Integer, ByVal lngIDProvvedimento As Long, ByVal objSanzioni As DataSet, ByVal ObjInteressiSanzioni As DataSet, myStringConnection As String) As Long
        '    '*** 20140701 - IMU/TARES ***
        '    If DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, objSanzioni, myStringConnection) < 1 Then
        '        Return -1
        '    End If
        '    If DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, ObjInteressiSanzioni, myStringConnection) < 1 Then
        '        Return -1
        '    End If
        '    '*** ***
        '    Return 1
        'End Function

        '*** 20140701 - IMU/TARES ***
        Private Function DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte As String, ByVal nIDProvvedimento As Long, ByVal objDataSet As DataSet, myStringConnection As String) As Long
            Dim intCount As Integer
            Dim intRetVal As Long
            Dim cmdMyCommand As New SqlCommand

            Try
                If Not objDataSet Is Nothing Then
                    If objDataSet.Tables.Count > 0 Then
                        cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                        cmdMyCommand.Connection.Open()
                        cmdMyCommand.CommandTimeout = 0
                        cmdMyCommand.CommandType = CommandType.Text

                        For intCount = 0 To objDataSet.Tables(0).Rows.Count - 1
                            cmdMyCommand.CommandType = CommandType.StoredProcedure
                            cmdMyCommand.CommandText = "prc_DETTAGLIO_VOCI_ACCERTAMENTI_IU"
                            cmdMyCommand.Parameters.Clear()
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = IdEnte
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = nIDProvvedimento
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_VOCE", SqlDbType.NVarChar)).Value = objDataSet.Tables(0).Rows(intCount).Item("COD_VOCE")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("IMPORTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.NVarChar)).Value = objDataSet.Tables(0).Rows(intCount).Item("DATA_INIZIO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.NVarChar)).Value = objDataSet.Tables(0).Rows(intCount).Item("DATA_FINE")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_IMMOBILE_PROGRESSIVO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("GENERIC_ID")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.NVarChar)).Value = objDataSet.Tables(0).Rows(intCount).Item("ID_LEGAME")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDOTTO", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("IMPORTO_RIDOTTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("ACCONTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("SALDO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_ACCONTO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("N_SEMESTRI_ACCONTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_SALDO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("N_SEMESTRI_SALDO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TASSO", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("TASSO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO_GIORNI", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("ACCONTO_GIORNI")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("COD_TIPO_PROVVEDIMENTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_GIORNI", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("IMPORTO_GIORNI")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MOTIVAZIONE", SqlDbType.NVarChar)).Value = objDataSet.Tables(0).Rows(intCount).Item("MOTIVAZIONI")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_ACCONTO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("N_GIORNI_ACCONTO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_SALDO", SqlDbType.Int)).Value = objDataSet.Tables(0).Rows(intCount).Item("N_GIORNI_SALDO")
                            cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO_GIORNI", SqlDbType.Float)).Value = objDataSet.Tables(0).Rows(intCount).Item("SALDO_GIORNI")
                            Log.Debug("DETTAGLIO_VOCI_ACCERTAMENTI_INSERT::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
                            intRetVal = cmdMyCommand.ExecuteNonQuery
                            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DETTAGLIO_ADDIZIONALI_TARSU_INSERT.DataSet::DBOPENgovProvvedimentiUpdate")
                            End If
                        Next
                    End If
                End If
                Return 1
            Catch ex As Exception
                Log.Debug("Si è verificato un errore in DBOPENgovProvvedimentiUpdate::DETTAGLIO_VOCI_ACCERTAMENTI_INSERT.DataSet::" & ex.Message)
                Return -1
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        Private Function DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(ByVal IDEnte As String, ByVal nIDProvvedimento As Long, ByVal ListVoci() As ObjInteressiSanzioni, ByVal myStringConnection As String) As Integer
            Dim intRetVal As Long
            Dim cmdMyCommand As New SqlCommand

            Try
                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0
                cmdMyCommand.CommandType = CommandType.Text

                If Not ListVoci Is Nothing Then
                    For Each myItem As ObjInteressiSanzioni In ListVoci
                        cmdMyCommand.CommandType = CommandType.StoredProcedure
                        cmdMyCommand.CommandText = "prc_DETTAGLIO_VOCI_ACCERTAMENTI_IU"
                        cmdMyCommand.Parameters.Clear()
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = IDEnte
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = nIDProvvedimento
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_VOCE", SqlDbType.NVarChar)).Value = myItem.COD_VOCE
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = myItem.IMPORTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.NVarChar)).Value = myItem.DATA_INIZIO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.NVarChar)).Value = myItem.DATA_FINE
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_IMMOBILE_PROGRESSIVO", SqlDbType.Int)).Value = myItem.IdFase
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.NVarChar)).Value = myItem.ID_LEGAME
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDOTTO", SqlDbType.Float)).Value = myItem.IMPORTO_RIDOTTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Float)).Value = myItem.ACCONTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Float)).Value = myItem.SALDO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_ACCONTO", SqlDbType.Int)).Value = myItem.N_SEMESTRI_ACCONTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_SALDO", SqlDbType.Int)).Value = myItem.N_SEMESTRI_SALDO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TASSO", SqlDbType.Float)).Value = myItem.TASSO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO_GIORNI", SqlDbType.Float)).Value = myItem.ACCONTO_GIORNI
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = myItem.COD_TIPO_PROVVEDIMENTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_GIORNI", SqlDbType.Float)).Value = myItem.IMPORTO_GIORNI
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MOTIVAZIONE", SqlDbType.NVarChar)).Value = myItem.MOTIVAZIONI
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_ACCONTO", SqlDbType.Int)).Value = myItem.N_GIORNI_ACCONTO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_SALDO", SqlDbType.Int)).Value = myItem.N_GIORNI_SALDO
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO_GIORNI", SqlDbType.Float)).Value = myItem.SALDO_GIORNI
                        Log.Debug("DETTAGLIO_VOCI_ACCERTAMENTI_INSERT:: query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
                        intRetVal = cmdMyCommand.ExecuteNonQuery
                        If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DETTAGLIO_ADDIZIONALI_TARSU_INSERT.ArrayList::DBOPENgovProvvedimentiUpdate")
                        End If
                    Next
                End If
                Return 1
            Catch ex As Exception
                Log.Debug("Si è verificato un errore in DBOPENgovProvvedimentiUpdate::DETTAGLIO_VOCI_ACCERTAMENTI_INSERT.ArrayList::" & ex.Message)
                Return -1
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        '*** ***
        'Private Function SET_RIDUZIONIDETASSAZIONI_ACCERTAMENTO(strCODEnte As String, ByVal intCODContribuente As Integer, ByVal lngIDProvvedimento As Long, ByVal objRid() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoRiduzione, ByVal objDet() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDetassazione, ByVal objDBManager As DBManager, ByVal objHashTable As Hashtable) As Long

        '    Dim intCount As Integer
        '    Dim strSQL As String
        '    Dim intRetVal As Long


        '    objUtility = New MotoreProvUtility


        '    If Not objRid Is Nothing Then
        '        For intCount = 0 To objRid.Length - 1
        '            strSQL = "INSERT INTO TBLACCERTATORIDUZIONE"
        '            strSQL += " (IDENTE, IDACCERTAMENTO, IDRIDUZIONE)"
        '            strSQL += " VALUES(" & vbCrLf

        '            strSQL += objUtility.CStrToDB(objRid(intCount).sIdEnte) & "," & vbCrLf
        '            strSQL += objUtility.CIdToDB(objRid(intCount).IdDettaglioTestata) & "," & vbCrLf
        '            strSQL += objUtility.CIdToDB(objRid(intCount).IdRiduzione) & "," & vbCrLf
        '            strSQL += ")"
        '            Log.Debug("setri->" + strSQL)
        '            intRetVal = objDBManager.Execute(strSQL)

        '            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '                Return 0
        '                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DETTAGLIO_VOCI_ACCERTAMENTI_INSERT::DBOPENgovProvvedimentiUpdate")
        '            End If

        '        Next
        '    End If

        '    If Not objDet Is Nothing Then
        '        For intCount = 0 To objDet.Length - 1
        '            strSQL = "INSERT INTO TBLACCERTATODETASSAZIONE"
        '            strSQL += " (IDENTE, IDACCERTAMENTO, IDRIDUZIONE)"
        '            strSQL += " VALUES(" & vbCrLf

        '            strSQL += objUtility.CStrToDB(objDet(intCount).sIdEnte) & "," & vbCrLf
        '            strSQL += objUtility.CIdToDB(objDet(intCount).IdDettaglioTestata) & "," & vbCrLf
        '            strSQL += objUtility.CIdToDB(objDet(intCount).IdDettaglioTestata) & "," & vbCrLf
        '            strSQL += ")"
        '            Log.Debug("setdet->" + strSQL)
        '            intRetVal = objDBManager.Execute(strSQL)

        '            If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '                Return 0
        '                Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DETTAGLIO_VOCI_ACCERTAMENTI_INSERT::DBOPENgovProvvedimentiUpdate")
        '            End If

        '        Next
        '    End If

        '    Return 1
        '    ''If Not objDataSet Is Nothing Then
        '    ''    objDataSet.Dispose()
        '    ''End If

        'End Function

        Private Function SetUIDichAcc(ByVal IDProcedimento As Integer, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, myStringConnection As String) As Integer
            Try
                If Not ListDichiarato Is Nothing Then
                    If SetUIDichAcc("DIC", myStringConnection, ListDichiarato, IDProcedimento) < 1 Then
                        Return -1
                    End If
                    For Each mySitFinale As objSituazioneFinale In ListDichiarato
                        mySitFinale.IdProcedimento = IDProcedimento
                        mySitFinale.TipoOperazione = "D"
                    Next
                    If SetSituazioneFinaleICI(ListDichiarato, IDProcedimento, myStringConnection) < 1 Then
                        Return -1
                    End If
                End If
                If SetUIDichAcc("ACC", myStringConnection, ListAccertato, IDProcedimento) < 0 Then
                    Return -1
                End If
                For Each mySitFinale As objSituazioneFinale In ListAccertato
                    mySitFinale.IdProcedimento = IDProcedimento
                    mySitFinale.TipoOperazione = "A"
                Next
                If SetSituazioneFinaleICI(ListAccertato, IDProcedimento, myStringConnection) < 1 Then
                    Return -1
                End If
                Return 1
            Catch ex As Exception
                Log.Debug("Si è verificato un errore in SetUIDichAcc::" & ex.Message)
                Return -1
            End Try
        End Function

        Private Function SetUIDichAcc(Type As String, ByVal StringConnection As String, ListUI() As objUIICIAccert, IDProcedimento As Integer) As Long
            Dim cmdMyCommand As New SqlCommand
            Dim intRetVal As Long

            Try
                cmdMyCommand.Connection = New SqlConnection(StringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                For Each mySituazioneFinale As objUIICIAccert In ListUI
                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    'Valorizzo i parameters:
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.BigInt)).Value = mySituazioneFinale.Id
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROCEDIMENTO", SqlDbType.Int)).Value = IDProcedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.Int)).Value = mySituazioneFinale.IdLegame
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PROGRESSIVO", SqlDbType.Int)).Value = mySituazioneFinale.Progressivo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ENTE", SqlDbType.NVarChar)).Value = mySituazioneFinale.IdEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@idtestata", SqlDbType.Int)).Value = DBNull.Value
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CodUI", SqlDbType.Int)).Value = mySituazioneFinale.IdImmobile
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FOGLIO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Foglio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Numero
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SUBALTERNO", SqlDbType.NVarChar)).Value = mySituazioneFinale.Subalterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CARATTERISTICA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Caratteristica
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SEZIONE", SqlDbType.NVarChar)).Value = mySituazioneFinale.Sezione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CATEGORIA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Categoria
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CLASSE", SqlDbType.NVarChar)).Value = mySituazioneFinale.Classe
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODRENDITA", SqlDbType.NVarChar)).Value = mySituazioneFinale.TipoRendita
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@STORICO", SqlDbType.Bit)).Value = mySituazioneFinale.FlagStorico
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VALORE", SqlDbType.Float)).Value = mySituazioneFinale.Valore
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_PROVVISORIO", SqlDbType.Bit)).Value = mySituazioneFinale.FlagProvvisorio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VIA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Via & " " & mySituazioneFinale.NCivico
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAINIZIO", SqlDbType.DateTime)).Value = mySituazioneFinale.Dal
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATAFINE", SqlDbType.DateTime)).Value = mySituazioneFinale.Al
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDIMMOBILEPERTINENTE", SqlDbType.Int)).Value = mySituazioneFinale.IdImmobilePertinenza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@consistenza", SqlDbType.Float)).Value = mySituazioneFinale.Consistenza
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PERCPOSSESSO", SqlDbType.Float)).Value = mySituazioneFinale.PercPossesso
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COLTIVATOREDIRETTO", SqlDbType.Bit)).Value = mySituazioneFinale.IsColtivatoreDiretto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMEROFIGLI", SqlDbType.Int)).Value = mySituazioneFinale.NumeroFigli
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOPOSSESSO", SqlDbType.Int)).Value = mySituazioneFinale.IdTipoPossesso
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOUTILIZZO", SqlDbType.Int)).Value = mySituazioneFinale.IdTipoUtilizzo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_RIDUZIONE", SqlDbType.Int)).Value = mySituazioneFinale.FlagRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESI_RIDUZIONE", SqlDbType.Int)).Value = mySituazioneFinale.MesiRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_PRINCIPALE", SqlDbType.Int)).Value = mySituazioneFinale.FlagPrincipale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ZONA", SqlDbType.NVarChar)).Value = mySituazioneFinale.Zona
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@RENDITA", SqlDbType.Float)).Value = mySituazioneFinale.Rendita
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_ESENTE", SqlDbType.Int)).Value = mySituazioneFinale.FlagEsente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MESI_ESCL_ESENZIONE", SqlDbType.Int)).Value = mySituazioneFinale.MesiEsenzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ICI_VALORE_ALIQUOTA", SqlDbType.Float)).Value = mySituazioneFinale.Aliquota
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPTOTALE", SqlDbType.Float)).Value = mySituazioneFinale.TotDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPACCONTO", SqlDbType.Float)).Value = mySituazioneFinale.AccDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPSALDO", SqlDbType.Float)).Value = mySituazioneFinale.SalDovuto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DIFFIMPOSTA", SqlDbType.Float)).Value = mySituazioneFinale.DiffImposta
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@INTERESSI", SqlDbType.Float)).Value = mySituazioneFinale.ImpInteressi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SANZIONI", SqlDbType.Float)).Value = mySituazioneFinale.ImpSanzioni
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TOTALE", SqlDbType.Float)).Value = mySituazioneFinale.Totale
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODTRIBUTO", SqlDbType.VarChar)).Value = mySituazioneFinale.Tributo
                    '*** 20150430 - TASI Inquilino ***
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOTASI", SqlDbType.NVarChar)).Value = mySituazioneFinale.TipoTasi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTECALCOLO", SqlDbType.Int)).Value = mySituazioneFinale.IdContribuenteCalcolo
                    '*** ***
                    If Type = "DIC" Then
                        cmdMyCommand.CommandText = "prc_TP_IMMOBILI_ACCERTAMENTI_IU"
                    Else
                        cmdMyCommand.CommandText = "prc_TP_IMMOBILI_ACCERTATI_ACCERTAMENTI_IU"
                    End If
                    Log.Debug("Function.SetUIDichAcc.QUERY::" + cmdMyCommand.CommandText + " " + Utility.Costanti.GetValParamCmd(cmdMyCommand))
                    intRetVal = cmdMyCommand.ExecuteNonQuery
                Next
                Return intRetVal
            Catch ex As Exception
                Log.Error("Function::SetUIDichAcc::si è verificato il seguente errore::" & ex.Message)
                Return -1
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
#End Region

#Region "GESTIONE RITORNO DICHIARAZIONI"
        '<AutoComplete()>
        'Public Function setDATA_ATTO_DEFINITIVO(StringConnectionProvv As String, strCODENTE As String, ByVal objHashTable As Hashtable) As Boolean

        '    Dim strSQL As String
        '    Dim objUtility As New MotoreProvUtility
        '    Dim intRetVal As Integer
        '    Dim objDS_PROVVEDIMENTI As DataSet
        '    Dim strDATAADD As String
        '    Dim strIDPROVVEDIMENTO As String
        '    Dim intCOUNT As Integer
        '    setDATA_ATTO_DEFINITIVO = False
        '    Dim objDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect

        '    objDBManager = New DBManager

        '    objDBManager.Initialize(StringConnectionProvv)

        '    objDS_PROVVEDIMENTI = objDBOPENgovProvvedimentiSelect.getDati_PROVVEDIMENTI(StringConnectionProvv, strCODENTE, objHashTable)

        '    For intCOUNT = 0 To objDS_PROVVEDIMENTI.Tables("PROVVEDIMENTI").Rows.Count - 1
        '        Dim rowPROVVEDIMENTI As DataRow = objDS_PROVVEDIMENTI.Tables("PROVVEDIMENTI").Rows(intCOUNT)
        '        strDATAADD = objUtility.GiraDataFromDB(rowPROVVEDIMENTI("DATA_NOTIFICA_AVVISO"))
        '        strIDPROVVEDIMENTO = objUtility.CToStr(rowPROVVEDIMENTI("ID_PROVVEDIMENTO"))

        '        strDATAADD = objUtility.GiraData(DateAdd(DateInterval.Day, 60, CDate(strDATAADD)))

        '        strSQL = "UPDATE PROVVEDIMENTI" & vbCrLf
        '        strSQL += "SET DATA_ATTO_DEFINITIVO =" & objUtility.CStrToDB(strDATAADD) & vbCrLf
        '        strSQL += "WHERE" & vbCrLf
        '        strSQL += "ID_PROVVEDIMENTO=" & strIDPROVVEDIMENTO & vbCrLf
        '        Log.Debug("setattodef->" + strSQL)
        '        intRetVal = objDBManager.Execute(strSQL)

        '        If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            If Not IsNothing(objDBManager) Then
        '                objDBManager.Kill()
        '                objDBManager.Dispose()

        '            End If
        '            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::setDATA_ATTO_DEFINITIVO::DBOPENgovProvvedimentiUpdate")
        '        End If

        '    Next

        '    If Not IsNothing(objDBManager) Then
        '        objDBManager.Kill()
        '        objDBManager.Dispose()

        '    End If

        '    setDATA_ATTO_DEFINITIVO = True

        '    Return setDATA_ATTO_DEFINITIVO

        'End Function
#End Region

#Region "GESTIONE FREEZER"
        '*** 20140509 - TASI ***
        Public Function DeleteFreezer(ByVal cmdMyCommand As SqlCommand, IdEnte As String, ByVal strCOD_CONTRIBUENTE As Long, ByVal objHashTable As Hashtable)
            Dim intRetVal As Integer

            Try
                cmdMyCommand.CommandType = CommandType.StoredProcedure
                'Valorizzo i parameters:
                cmdMyCommand.Parameters.Clear()
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CodEnte", SqlDbType.NVarChar)).Value = IdEnte
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Tributo", SqlDbType.NVarChar)).Value = objHashTable("TRIBUTOCALCOLO").ToString
                cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CodContribuente", SqlDbType.Int)).Value = strCOD_CONTRIBUENTE
                cmdMyCommand.CommandText = "prc_TP_SITUAZIONE_VIRTUALE_DICHIARATO_D"
                Log.Debug("delfre->" + Costanti.LogQuery(cmdMyCommand))
                intRetVal = cmdMyCommand.ExecuteNonQuery
                If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DeleteFreezer::DBOPENgovProvvedimentiUpdate")
                End If
            Catch ex As Exception
                Log.Error(" - Function:DeleteFreezer::" & ex.Message, ex)
                Return -1
            End Try
        End Function
        Public Function SetFREEZER(StringConnectionICI As String, ByVal cmdMyCommand As SqlCommand, ByVal objDSFreezerFINALE As DataSet, ByVal objHashTable As Hashtable) As Long
            'Dim cmdMyCommand As New SqlCommand
            Dim intCount As Integer
            Dim intRetVal As Integer
            Dim objDSFreezerFINALEClone As DataSet
            'Dim myTrans As SqlTransaction

            Try
                cmdMyCommand.CommandType = CommandType.StoredProcedure
                'Valorizzo la connessione
                cmdMyCommand.Connection = New SqlClient.SqlConnection(StringConnectionICI)
                If cmdMyCommand.Connection.State = ConnectionState.Closed Then
                    cmdMyCommand.Connection.Open()
                End If
                'myTrans = cmdMyCommand.Connection.BeginTransaction()
                'cmdMyCommand.Transaction = myTrans

                objDSFreezerFINALEClone = objDSFreezerFINALE.Copy
                For intCount = 0 To objDSFreezerFINALE.Tables(0).Rows.Count - 1
                    intRetVal = 0
                    'Valorizzo i parameters:
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Anno", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("ANNO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Cod_Contribuente", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_CONTRIBUENTE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_TESTATA", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("ID_TESTATA")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_IMMOBILE", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("ID_IMMOBILE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@Cod_Ente", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_ENTE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODTRIBUTO", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("CODTRIBUTO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROCEDIMENTO", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_TIPO_PROCEDIMENTO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_MESI_ACCONTO", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("NUMERO_MESI_ACCONTO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_MESI_TOTALI", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("NUMERO_MESI_TOTALI")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO_UTILIZZATORI", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("NUMERO_UTILIZZATORI")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_PRINCIPALE", SqlDbType.Bit)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("FLAG_PRINCIPALE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@PERC_POSSESSO", SqlDbType.Float)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("PERC_POSSESSO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VALORE", SqlDbType.Float)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("VALORE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@RIDUZIONE", SqlDbType.Bit)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("RIDUZIONE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@POSSESSO_FINE_ANNO", SqlDbType.Bit)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("POSSESSO_FINE_ANNO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ESENTE_ESCLUSO", SqlDbType.Bit)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("ESENTE_ESCLUSO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOUTILIZZO", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("IDTIPOUTILIZZO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTIPOPOSSESSO", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("IDTIPOPOSSESSO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_DETRAZIONE", SqlDbType.Float)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("IMPORTO_DETRAZIONE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_IMMOBILE_PERTINENZA", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_IMMOBILE_PERTINENZA")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_IMMOBILE_DA_ACCERTAMENTO", SqlDbType.NVarChar)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_IMMOBILE_DA_ACCERTAMENTO")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CONTITOLARE", SqlDbType.Bit)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("CONTITOLARE")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOTASI", SqlDbType.VarChar)).Value = "P" 'per ora default PROPRIETARIO
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTECALCOLO", SqlDbType.Int)).Value = objDSFreezerFINALEClone.Tables(0).Rows(intCount).Item("COD_CONTRIBUENTE") 'per ora default PROPRIETARIO
                    cmdMyCommand.CommandText = "prc_TP_SITUAZIONE_VIRTUALE_DICHIARATO_IU"
                    Dim sValParametri As String = Utility.Costanti.GetValParamCmd(cmdMyCommand)
                    Log.Debug("SetFreezer::query::" & cmdMyCommand.CommandText & "::param::" & sValParametri)
                    intRetVal = cmdMyCommand.ExecuteNonQuery
                    If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                        'myTrans.Rollback()
                        Log.Debug("Application::COMPlusFreezer.vb::Function::SetFREEZER::DBOPENgovProvvedimentiUpdate::" & cmdMyCommand.CommandText)
                        Throw New Exception("Application::COMPlusFreezer.vb::Function::SetFREEZER::DBOPENgovProvvedimentiUpdate")
                    End If
                Next
                If Not objDSFreezerFINALEClone Is Nothing Then
                    objDSFreezerFINALEClone.Dispose()
                End If
                'myTrans.Commit()
            Catch ex As Exception
                Log.Debug("Application::COMPlusFreezer.vb::Function::SetFREEZER::DBOPENgovProvvedimentiUpdate::" & ex.ToString)
                'myTrans.Rollback()
            End Try
        End Function
        '*** ***
#End Region
#Region "TARSU/TARES"
        '**** 201809 - Cartelle Insoluti ***
        Private Function CastToOggettoAtto(ItemIn As Object) As OggettoAtto
            Dim ItemOut As New OggettoAtto
            Dim myItem As New Object
            Try
                If ItemIn.GetType Is GetType(OggettoAttoTARSU) Then
                    myItem = New OggettoAttoTARSU
                    myItem = CType(ItemIn, OggettoAttoTARSU)
                ElseIf ItemIn.GetType Is GetType(OggettoAttoOSAP) Then
                    myItem = New OggettoAttoOSAP
                    myItem = CType(ItemIn, OggettoAttoOSAP)
                Else
                    ItemOut = Nothing
                End If
                If Not ItemOut Is Nothing Then
                    ItemOut.ID_PROVVEDIMENTO = myItem.ID_PROVVEDIMENTO
                    ItemOut.COD_ENTE = myItem.COD_ENTE
                    ItemOut.ANNO = myItem.anno
                    ItemOut.NUMERO_AVVISO = objUtility.getNumeroAvviso(ItemOut.ANNO, ItemOut.ID_PROVVEDIMENTO)
                    ItemOut.NUMERO_ATTO = myItem.NUMERO_ATTO
                    ItemOut.COD_TRIBUTO = myItem.COD_TRIBUTO
                    ItemOut.COD_CONTRIBUENTE = myItem.COD_CONTRIBUENTE
                    ItemOut.COGNOME = myItem.COGNOME
                    ItemOut.NOME = myItem.NOME
                    ItemOut.CODICE_FISCALE = myItem.CODICE_FISCALE
                    ItemOut.PARTITA_IVA = myItem.PARTITA_IVA
                    ItemOut.VIA_RES = myItem.VIA_RES
                    ItemOut.POSIZIONE_CIVICO_RES = myItem.POSIZIONE_CIVICO_RES
                    ItemOut.CIVICO_RES = myItem.CIVICO_RES
                    ItemOut.ESPONENTE_CIVICO_RES = myItem.ESPONENTE_CIVICO_RES
                    ItemOut.CAP_RES = myItem.CAP_RES
                    ItemOut.FRAZIONE_RES = myItem.FRAZIONE_RES
                    ItemOut.CITTA_RES = myItem.CITTA_RES
                    ItemOut.PROVINCIA_RES = myItem.PROVINCIA_RES
                    ItemOut.CO = myItem.CO
                    ItemOut.VIA_CO = myItem.VIA_CO
                    ItemOut.POSIZIONE_CIVICO_CO = myItem.POSIZIONE_CIVICO_CO
                    ItemOut.CIVICO_CO = myItem.CIVICO_CO
                    ItemOut.ESPONENTE_CIVICO_CO = myItem.ESPONENTE_CIVICO_CO
                    ItemOut.CAP_CO = myItem.CAP_CO
                    ItemOut.FRAZIONE_CO = myItem.FRAZIONE_CO
                    ItemOut.CITTA_CO = myItem.CITTA_CO
                    ItemOut.PROVINCIA_CO = myItem.PROVINCIA_CO
                    ItemOut.DATA_ELABORAZIONE = myItem.DATA_ELABORAZIONE
                    ItemOut.IMPORTO_DIFFERENZA_IMPOSTA = myItem.IMPORTO_DIFFERENZA_IMPOSTA
                    ItemOut.IMPORTO_SANZIONI = myItem.IMPORTO_SANZIONI
                    ItemOut.IMPORTO_SANZIONI_RIDOTTO = myItem.IMPORTO_SANZIONI_RIDOTTO
                    ItemOut.IMPORTO_INTERESSI = myItem.IMPORTO_INTERESSI
                    ItemOut.IMPORTO_SPESE = myItem.IMPORTO_SPESE
                    ItemOut.IMPORTO_ALTRO = myItem.IMPORTO_ALTRO
                    ItemOut.IMPORTO_SENZA_ARROTONDAMENTO = myItem.IMPORTO_SENZA_ARROTONDAMENTO
                    ItemOut.IMPORTO_ARROTONDAMENTO = myItem.IMPORTO_ARROTONDAMENTO
                    ItemOut.IMPORTO_TOTALE = myItem.IMPORTO_TOTALE
                    ItemOut.IMPORTO_TOTALE_RIDOTTO = myItem.IMPORTO_TOTALE_RIDOTTO
                    ItemOut.IMPORTO_ARROTONDAMENTO_RIDOTTO = myItem.IMPORTO_ARROTONDAMENTO_RIDOTTO
                    ItemOut.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = myItem.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI
                    ItemOut.IMPORTO_DICHIARATO_F2 = myItem.IMPORTO_DICHIARATO_F2
                    ItemOut.IMPORTO_VERSATO_F2 = myItem.IMPORTO_VERSATO_F2
                    ItemOut.IMPORTO_DIFFERENZA_IMPOSTA_F2 = myItem.IMPORTO_DIFFERENZA_IMPOSTA_F2
                    ItemOut.IMPORTO_SANZIONI_F2 = myItem.IMPORTO_SANZIONI_F2
                    ItemOut.IMPORTO_INTERESSI_F2 = myItem.IMPORTO_INTERESSI_F2
                    ItemOut.IMPORTO_TOTALE_F2 = myItem.IMPORTO_TOTALE_F2
                    ItemOut.IMPORTO_ACCERTATO_ACC = myItem.IMPORTO_ACCERTATO_ACC
                    ItemOut.IMPORTO_DIFFERENZA_IMPOSTA_ACC = myItem.IMPORTO_DIFFERENZA_IMPOSTA_ACC
                    ItemOut.IMPORTO_SANZIONI_ACC = myItem.IMPORTO_SANZIONI_ACC
                    ItemOut.IMPORTO_SANZIONI_RIDOTTE_ACC = myItem.IMPORTO_SANZIONI_RIDOTTE_ACC
                    ItemOut.IMPORTO_INTERESSI_ACC = myItem.IMPORTO_INTERESSI_ACC
                    ItemOut.IMPORTO_TOTALE_ACC = myItem.IMPORTO_TOTALE_ACC
                    ItemOut.NOTE_CASSAZIONE = myItem.NOTE_CASSAZIONE
                    ItemOut.Provenienza = myItem.Provenienza
                    ItemOut.IDRUOLO = myItem.IDRUOLO
                    ItemOut.TipoProvvedimento = myItem.TipoProvvedimento
                End If
            Catch ex As Exception
                Log.Debug("CastToOggettoAtto::si è verificato il seguente errore::", ex)
                ItemOut = Nothing
            End Try
            Return ItemOut
        End Function

        '*** 20140701 - IMU/TARES ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myHashTable"></param>
        ''' <param name="dsSanzioni"></param>
        ''' <param name="dsInteressi"></param>
        ''' <param name="ListInteressi"></param>
        ''' <param name="oAtto"></param>
        ''' <param name="ListDettaglioAtto"></param>
        ''' <param name="ListDichiaratoTARSU"></param>
        ''' <param name="ListAccertatoTARSU"></param>
        ''' <param name="ListAddizionali"></param>
        ''' <param name="Operatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory><revision date="12/12/2019">se arrivo da cartelle insoluti non devo eliminare l'eventuale atto precedente per stesso contribuente/anno non confermato</revision></revisionHistory>
        <AutoComplete()>
        Public Function TARSU_SetProvvedimenti(myDBType As String, ByVal myHashTable As Hashtable, ByVal dsSanzioni As DataSet, ByVal dsInteressi As DataSet, ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal ListDettaglioAtto() As OggettoDettaglioAtto, ByVal ListDichiaratoTARSU() As ObjArticoloAccertamento, ByVal ListAccertatoTARSU() As ObjArticoloAccertamento, ByVal ListAddizionali() As OggettoAddizionaleAccertamento, Operatore As String) As ObjArticoloAccertamento()
            Dim objDSAnagraficaIndirizziSpedizione As DataSet
            Dim lngIDProcedimento As Long
            Dim intRetVal As Integer
            Dim sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP As String
            Dim myAtto As New OggettoAtto

            Try
                objUtility = New MotoreProvUtility

                Dim intTemp As Integer = 0

                Dim FncDBSelect As New DBOPENgovProvvedimentiSelect

                Dim TIPO_OPERAZIONE_RETTIFICA As Boolean = myHashTable("TIPO_OPERAZIONE_RETTIFICA")  'NORMALE-RETTIFICA
                Dim DATA_RETTIFICA As String = myHashTable("DATA_RETTIFICA")
                Dim DATA_ANNULLAMENTO As String = myHashTable("DATA_ANNULLAMENTO")
                Dim ID_PROVVEDIMENTO_OLD As Long
                If TIPO_OPERAZIONE_RETTIFICA = True Then
                    ID_PROVVEDIMENTO_OLD = myHashTable("ID_PROVVEDIMENTO_RETTIFICA")
                End If

                Dim ObjRiepilogo() As ObjArticoloAccertamento

                '*** 201810 - Generazione Massiva Atti ***
                '### tolto test perché passato sempre =0 ##############################################################################################################################
                'SE VALORE_RITORNO_ACCERTAMENTO=4 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO.
                'SE VALORE_RITORNO_ACCERTAMENTO=5 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO E UN ACCERTAMENTO NON DEFINITIVO (in questo caso cancello però solo l'accertamento e non il pre accertamento)
                'QUINDI SVUOTO I DATI PRESENTI E SALVO QUELLI DEL NUOVO ACCERTAMENTO
                Dim blnControlloAttiDefinitivi As Boolean
                Dim ID_PROCEDIMENTO As Long
                Dim ID_PROVVEDIMENTO As Long
                Dim DATA_CONFERMA As String
                Dim blnDelete As Boolean

                If oAtto.Provenienza <> 2 Then 'se non arrivo da cartelle insoluti
                    blnControlloAttiDefinitivi = FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_ENTE, oAtto.COD_CONTRIBUENTE, oAtto.ANNO, "A", oAtto.COD_TRIBUTO, ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA)
                    'VALORE_RITORNO_ACCERTAMENTO=4 LA DATA CONFERMA E' PER FORZA VUOTA
                    If DATA_CONFERMA.CompareTo("") = 0 Then
                        'vuol dire che ho trovato un atto non definitivo oppure nessun atto
                        If ID_PROVVEDIMENTO <> 0 Then
                            blnDelete = TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), ID_PROCEDIMENTO, ID_PROVVEDIMENTO)
                        End If
                    End If
                End If
                intRetVal = 0

                objDSAnagraficaIndirizziSpedizione = FncDBSelect.getAnagraficaIndirizziSpedizione(myDBType, oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, myHashTable("CONNECTIONSTRINGANAGRAFICA"))
                Try
                    Dim rowIndirizziSpedizione As DataRow = objDSAnagraficaIndirizziSpedizione.Tables("TP_INDIRIZZI_SPEDIZIONE").Rows(0)

                    sCOGNOME_INVIO = (rowIndirizziSpedizione("COGNOME_INVIO")).ToUpper
                    sVIA_RCP = (rowIndirizziSpedizione("VIA_RCP")).ToUpper
                    sPOSIZIONE_CIV_RCP = (rowIndirizziSpedizione("POSIZIONE_CIV_RCP")).ToUpper
                    sCIVICO_RCP = (rowIndirizziSpedizione("CIVICO_RCP")).ToUpper
                    sESPONENTE_CIVICO_RCP = (rowIndirizziSpedizione("ESPONENTE_CIVICO_RCP")).ToUpper
                    sCAP_RCP = (rowIndirizziSpedizione("CAP_RCP")).ToUpper
                    sFRAZIONE_RCP = (rowIndirizziSpedizione("FRAZIONE_RCP")).ToUpper
                    sCOMUNE_RCP = (rowIndirizziSpedizione("COMUNE_RCP")).ToUpper
                    sPROVINCIA_RCP = (rowIndirizziSpedizione("PROVINCIA_RCP")).ToUpper
                Catch ex As IndexOutOfRangeException
                    sCOGNOME_INVIO = ""
                    sVIA_RCP = ""
                    sPOSIZIONE_CIV_RCP = ""
                    sCIVICO_RCP = ""
                    sESPONENTE_CIVICO_RCP = ""
                    sCAP_RCP = ""
                    sFRAZIONE_RCP = ""
                    sCOMUNE_RCP = ""
                    sPROVINCIA_RCP = ""
                End Try

                oAtto.ID_PROVVEDIMENTO = FncDBSelect.getNewID("PROVVEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                oAtto.DATA_ELABORAZIONE = DateTime.Now.ToString("yyyyMMdd") 'DateTime.Now

                myAtto = CastToOggettoAtto(oAtto)
                If myAtto Is Nothing Then
                    Log.Debug("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
                    Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
                End If
                If SetProvvedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, Operatore) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Log.Debug("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
                    Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
                Else
                    oAtto.ID_PROVVEDIMENTO = myAtto.ID_PROVVEDIMENTO
                End If

                lngIDProcedimento = FncDBSelect.getNewID("TAB_PROCEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                If SetProcedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, lngIDProcedimento, myAtto.TipoProvvedimento) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
                    TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
                    Log.Debug("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
                    Throw New Exception("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
                End If

                'inserisco ACCERTATO TARSU
                If TARSU_SetACCERTATO(ListAccertatoTARSU, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO) <> 1 Then
                    TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If
                'devo inserire RIEPILOGOACCERTATOTARSU
                ObjRiepilogo = TARSU_SetRIEPILOGO_ACCERTATO(myHashTable("TipoTassazione"), ListDichiaratoTARSU, ListAccertatoTARSU, oAtto.ID_PROVVEDIMENTO, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                If ObjRiepilogo Is Nothing Then
                    TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If

                If TIPO_OPERAZIONE_RETTIFICA = True Then
                    Log.Debug("sono in rettifica/annullamento")
                    SetPROVVEDIMENTIRETTIFICATI(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, ID_PROVVEDIMENTO_OLD, DATA_RETTIFICA, True, DATA_ANNULLAMENTO)
                End If

                intRetVal = TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioni, dsInteressi, ListInteressi, ListAddizionali, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                If intRetVal <> 1 Then
                    TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If

                'inserisco DICHIARATO TARSU
                intRetVal = TARSU_SetDICHIARATO(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myHashTable("CONNECTIONSTRINGOPENGOVTARSU"), ListDichiaratoTARSU, oAtto.ID_PROVVEDIMENTO)
                If intRetVal <> 1 Then
                    TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If
                Return ObjRiepilogo
            Catch ex As Exception
                Log.Debug("setprovvedimentiaccertamentitarsu::si è verificato il seguente errore::", ex)
                Return Nothing
            End Try
        End Function
        '<AutoComplete()>
        'Public Function TARSU_SetProvvedimenti(myDBType As String, ByVal myHashTable As Hashtable, ByVal dsSanzioni As DataSet, ByVal dsInteressi As DataSet, ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal ListDettaglioAtto() As OggettoDettaglioAtto, ByVal ListDichiaratoTARSU() As ObjArticoloAccertamento, ByVal ListAccertatoTARSU() As ObjArticoloAccertamento, ByVal ListAddizionali() As OggettoAddizionaleAccertamento, Operatore As String) As ObjArticoloAccertamento()
        '    Dim objDSAnagraficaIndirizziSpedizione As DataSet
        '    Dim lngIDProcedimento As Long
        '    Dim intRetVal As Integer
        '    Dim sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP As String
        '    Dim myAtto As New OggettoAtto

        '    Try
        '        objUtility = New MotoreProvUtility

        '        Dim intTemp As Integer = 0

        '        Dim FncDBSelect As New DBOPENgovProvvedimentiselect

        '        Dim TIPO_OPERAZIONE_RETTIFICA As Boolean = myHashTable("TIPO_OPERAZIONE_RETTIFICA")  'NORMALE-RETTIFICA
        '        Dim DATA_RETTIFICA As String = myHashTable("DATA_RETTIFICA")
        '        Dim DATA_ANNULLAMENTO As String = myHashTable("DATA_ANNULLAMENTO")
        '        Dim ID_PROVVEDIMENTO_OLD As Long
        '        If TIPO_OPERAZIONE_RETTIFICA = True Then
        '            ID_PROVVEDIMENTO_OLD = myHashTable("ID_PROVVEDIMENTO_RETTIFICA")
        '        End If

        '        Dim ObjRiepilogo() As ObjArticoloAccertamento

        '        '*** 201810 - Generazione Massiva Atti ***
        '        '### tolto test perché passato sempre =0 ##############################################################################################################################
        '        'SE VALORE_RITORNO_ACCERTAMENTO=4 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO.
        '        'SE VALORE_RITORNO_ACCERTAMENTO=5 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO E UN ACCERTAMENTO NON DEFINITIVO (in questo caso cancello però solo l'accertamento e non il pre accertamento)
        '        'QUINDI SVUOTO I DATI PRESENTI E SALVO QUELLI DEL NUOVO ACCERTAMENTO
        '        Dim blnControlloAttiDefinitivi As Boolean
        '        Dim ID_PROCEDIMENTO As Long
        '        Dim ID_PROVVEDIMENTO As Long
        '        Dim DATA_CONFERMA As String
        '        Dim blnDelete As Boolean
        '        blnControlloAttiDefinitivi = FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_ENTE, oAtto.COD_CONTRIBUENTE, oAtto.ANNO, "A", oAtto.COD_TRIBUTO, ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA)
        '        'VALORE_RITORNO_ACCERTAMENTO=4 LA DATA CONFERMA E' PER FORZA VUOTA
        '        If DATA_CONFERMA.CompareTo("") = 0 Then
        '            'vuol dire che ho trovato un atto non definitivo oppure nessun atto
        '            If ID_PROVVEDIMENTO <> 0 Then
        '                blnDelete = TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), ID_PROCEDIMENTO, ID_PROVVEDIMENTO)
        '            End If
        '        End If
        '        intRetVal = 0

        '        objDSAnagraficaIndirizziSpedizione = FncDBSelect.getAnagraficaIndirizziSpedizione(oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, myHashTable("CONNECTIONSTRINGANAGRAFICA"))
        '        Try
        '            Dim rowIndirizziSpedizione As DataRow = objDSAnagraficaIndirizziSpedizione.Tables("TP_INDIRIZZI_SPEDIZIONE").Rows(0)

        '            sCOGNOME_INVIO = (rowIndirizziSpedizione("COGNOME_INVIO")).ToUpper
        '            sVIA_RCP = (rowIndirizziSpedizione("VIA_RCP")).ToUpper
        '            sPOSIZIONE_CIV_RCP = (rowIndirizziSpedizione("POSIZIONE_CIV_RCP")).ToUpper
        '            sCIVICO_RCP = (rowIndirizziSpedizione("CIVICO_RCP")).ToUpper
        '            sESPONENTE_CIVICO_RCP = (rowIndirizziSpedizione("ESPONENTE_CIVICO_RCP")).ToUpper
        '            sCAP_RCP = (rowIndirizziSpedizione("CAP_RCP")).ToUpper
        '            sFRAZIONE_RCP = (rowIndirizziSpedizione("FRAZIONE_RCP")).ToUpper
        '            sCOMUNE_RCP = (rowIndirizziSpedizione("COMUNE_RCP")).ToUpper
        '            sPROVINCIA_RCP = (rowIndirizziSpedizione("PROVINCIA_RCP")).ToUpper
        '        Catch ex As IndexOutOfRangeException
        '            sCOGNOME_INVIO = ""
        '            sVIA_RCP = ""
        '            sPOSIZIONE_CIV_RCP = ""
        '            sCIVICO_RCP = ""
        '            sESPONENTE_CIVICO_RCP = ""
        '            sCAP_RCP = ""
        '            sFRAZIONE_RCP = ""
        '            sCOMUNE_RCP = ""
        '            sPROVINCIA_RCP = ""
        '        End Try

        '        oAtto.ID_PROVVEDIMENTO = FncDBSelect.getNewID("PROVVEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        oAtto.DATA_ELABORAZIONE = DateTime.Now.ToString("yyyyMMdd") 'DateTime.Now

        '        myAtto = CastToOggettoAtto(oAtto)
        '        If myAtto Is Nothing Then
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
        '        End If
        '        If SetProvvedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, Operatore) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
        '        Else
        '            oAtto.ID_PROVVEDIMENTO = myAtto.ID_PROVVEDIMENTO
        '        End If

        '        lngIDProcedimento = FncDBSelect.getNewID("TAB_PROCEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If SetProcedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, lngIDProcedimento, myAtto.TipoProvvedimento) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
        '        End If

        '        'inserisco ACCERTATO TARSU
        '        If TARSU_SetACCERTATO(ListAccertatoTARSU, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO) <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If
        '        'devo inserire RIEPILOGOACCERTATOTARSU
        '        ObjRiepilogo = TARSU_SetRIEPILOGO_ACCERTATO(myHashTable("TipoTassazione"), ListDichiaratoTARSU, ListAccertatoTARSU, oAtto.ID_PROVVEDIMENTO, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If ObjRiepilogo Is Nothing Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If

        '        If TIPO_OPERAZIONE_RETTIFICA = True Then
        '            Log.Debug("sono in rettifica/annullamento")
        '            SetPROVVEDIMENTIRETTIFICATI(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, ID_PROVVEDIMENTO_OLD, DATA_RETTIFICA, True, DATA_ANNULLAMENTO)
        '        End If

        '        intRetVal = TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioni, dsInteressi, ListInteressi, ListAddizionali, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If intRetVal <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If

        '        'inserisco DICHIARATO TARSU
        '        intRetVal = TARSU_SetDICHIARATO(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myHashTable("CONNECTIONSTRINGOPENGOVTARSU"), ListDichiaratoTARSU, oAtto.ID_PROVVEDIMENTO)
        '        If intRetVal <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If
        '        Return ObjRiepilogo
        '    Catch ex As Exception
        '        Log.Debug("setprovvedimentiaccertamentitarsu::si è verificato il seguente errore::", ex)
        '        Return Nothing
        '    Finally
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        'End Function
        '<AutoComplete()>
        'Public Function TARSU_SetProvvedimenti(ByVal myHashTable As Hashtable, ByVal dsSanzioni As DataSet, ByVal dsInteressi As DataSet, ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal ListDettaglioAtto() As OggettoDettaglioAtto, ByVal ListDichiaratoTARSU() As ObjArticoloAccertamento, ByVal ListAccertatoTARSU() As ObjArticoloAccertamento, ByVal ListAddizionali() As OggettoAddizionaleAccertamento) As ObjArticoloAccertamento()
        '    Dim objDSAnagraficaIndirizziSpedizione As DataSet
        '    Dim lngIDProcedimento As Long
        '    Dim intRetVal As Integer
        '    Dim sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP As String
        '    Dim myAtto As New OggettoAtto

        '    Try
        '        objUtility = New MotoreProvUtility

        '        Dim intTemp As Integer = 0

        '        Dim FncDBSelect As New DBOPENgovProvvedimentiselect 

        '        Dim TIPO_OPERAZIONE_RETTIFICA As Boolean = myHashTable("TIPO_OPERAZIONE_RETTIFICA")  'NORMALE-RETTIFICA
        '        Dim DATA_RETTIFICA As String = myHashTable("DATA_RETTIFICA")
        '        Dim DATA_ANNULLAMENTO As String = myHashTable("DATA_ANNULLAMENTO")
        '        Dim ID_PROVVEDIMENTO_OLD As Long
        '        If TIPO_OPERAZIONE_RETTIFICA = True Then
        '            ID_PROVVEDIMENTO_OLD = myHashTable("ID_PROVVEDIMENTO_RETTIFICA")
        '        End If

        '        Dim ObjRiepilogo() As ObjArticoloAccertamento

        '        '*** 201810 - Generazione Massiva Atti ***
        '        '### tolto test perché passato sempre =0 ##############################################################################################################################
        '        'SE VALORE_RITORNO_ACCERTAMENTO=4 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO.
        '        'SE VALORE_RITORNO_ACCERTAMENTO=5 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO E UN ACCERTAMENTO NON DEFINITIVO (in questo caso cancello però solo l'accertamento e non il pre accertamento)
        '        'QUINDI SVUOTO I DATI PRESENTI E SALVO QUELLI DEL NUOVO ACCERTAMENTO
        '        'If objHashTable("VALORE_RITORNO_ACCERTAMENTO") = 4 Or objHashTable("VALORE_RITORNO_ACCERTAMENTO") = 0 Or objHashTable("VALORE_RITORNO_ACCERTAMENTO") = 6 Then
        '        Dim blnControlloAttiDefinitivi As Boolean
        '        Dim ID_PROCEDIMENTO As Long
        '        Dim ID_PROVVEDIMENTO As Long
        '        Dim DATA_CONFERMA As String
        '        Dim blnDelete As Boolean
        '        'blnControlloAttiDefinitivi = FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA, objHashTable, intCODContribuente, strANNO, sCODEnte, "A", objHashTable("CODTRIBUTO"))
        '        blnControlloAttiDefinitivi = FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_ENTE, oAtto.COD_CONTRIBUENTE, oAtto.ANNO, "A", oAtto.COD_TRIBUTO, ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA)
        '        'VALORE_RITORNO_ACCERTAMENTO=4 LA DATA CONFERMA E' PER FORZA VUOTA
        '        If DATA_CONFERMA.CompareTo("") = 0 Then
        '            'vuol dire che ho trovato un atto non definitivo oppure nessun atto
        '            If ID_PROVVEDIMENTO <> 0 Then
        '                blnDelete = TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), ID_PROCEDIMENTO, ID_PROVVEDIMENTO)
        '            End If
        '        End If
        '        'End If

        '        'Dim culture As IFormatProvider
        '        'culture = New CultureInfo("it-IT", True)
        '        'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")
        '        intRetVal = 0

        '        objDSAnagraficaIndirizziSpedizione = FncDBSelect.getAnagraficaIndirizziSpedizione(oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, myHashTable("CONNECTIONSTRINGANAGRAFICA"))
        '        Try
        '            Dim rowIndirizziSpedizione As DataRow = objDSAnagraficaIndirizziSpedizione.Tables("TP_INDIRIZZI_SPEDIZIONE").Rows(0)

        '            sCOGNOME_INVIO = (rowIndirizziSpedizione("COGNOME_INVIO")).ToUpper
        '            sVIA_RCP = (rowIndirizziSpedizione("VIA_RCP")).ToUpper
        '            sPOSIZIONE_CIV_RCP = (rowIndirizziSpedizione("POSIZIONE_CIV_RCP")).ToUpper
        '            sCIVICO_RCP = (rowIndirizziSpedizione("CIVICO_RCP")).ToUpper
        '            sESPONENTE_CIVICO_RCP = (rowIndirizziSpedizione("ESPONENTE_CIVICO_RCP")).ToUpper
        '            sCAP_RCP = (rowIndirizziSpedizione("CAP_RCP")).ToUpper
        '            sFRAZIONE_RCP = (rowIndirizziSpedizione("FRAZIONE_RCP")).ToUpper
        '            sCOMUNE_RCP = (rowIndirizziSpedizione("COMUNE_RCP")).ToUpper
        '            sPROVINCIA_RCP = (rowIndirizziSpedizione("PROVINCIA_RCP")).ToUpper
        '        Catch ex As IndexOutOfRangeException
        '            sCOGNOME_INVIO = ""
        '            sVIA_RCP = ""
        '            sPOSIZIONE_CIV_RCP = ""
        '            sCIVICO_RCP = ""
        '            sESPONENTE_CIVICO_RCP = ""
        '            sCAP_RCP = ""
        '            sFRAZIONE_RCP = ""
        '            sCOMUNE_RCP = ""
        '            sPROVINCIA_RCP = ""
        '        End Try

        '        oAtto.ID_PROVVEDIMENTO = FncDBSelect.getNewID("PROVVEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        oAtto.DATA_ELABORAZIONE = DateTime.Now

        '        myAtto = CastToOggettoAtto(oAtto)
        '        If myAtto Is Nothing Then
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore in valorizzazione atto")
        '        End If
        '        If SetProvvedimento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            'If TARSU_SetProvvedimento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto, sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU:errore inserimento provvedimento")
        '        Else
        '            oAtto.ID_PROVVEDIMENTO = myAtto.ID_PROVVEDIMENTO
        '        End If

        '        lngIDProcedimento = FncDBSelect.getNewID("TAB_PROCEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If SetProcedimento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, lngIDProcedimento, myAtto.TipoProvvedimento) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            'If TARSU_SetProcedimento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto, lngIDProcedimento) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
        '            'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Log.Debug("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
        '            Throw New Exception("SetProvvedimentiAccertamentiTARSU::errore inserimento procedimento")
        '        End If

        '        'inserisco ACCERTATO TARSU
        '        If TARSU_SetACCERTATO(ListAccertatoTARSU, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO) <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If
        '        'devo inserire RIEPILOGOACCERTATOTARSU
        '        ObjRiepilogo = TARSU_SetRIEPILOGO_ACCERTATO(myHashTable("TipoTassazione"), ListDichiaratoTARSU, ListAccertatoTARSU, oAtto.ID_PROVVEDIMENTO, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If ObjRiepilogo Is Nothing Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If

        '        If TIPO_OPERAZIONE_RETTIFICA = True Then
        '            Log.Debug("sono in rettifica/annullamento")
        '            SetPROVVEDIMENTIRETTIFICATI(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.COD_CONTRIBUENTE, oAtto.COD_TRIBUTO, oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, ID_PROVVEDIMENTO_OLD, DATA_RETTIFICA, True, DATA_ANNULLAMENTO)
        '        End If

        '        intRetVal = TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioni, dsInteressi, ListInteressi, ListAddizionali, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
        '        If intRetVal <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If

        '        'inserisco DICHIARATO TARSU
        '        intRetVal = TARSU_SetDICHIARATO(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myHashTable("CONNECTIONSTRINGOPENGOVTARSU"), ListDichiaratoTARSU, oAtto.ID_PROVVEDIMENTO)
        '        If intRetVal <> 1 Then
        '            TARSU_DeleteProvvedimentiAccertamento(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), lngIDProcedimento, oAtto.ID_PROVVEDIMENTO)
        '            Return Nothing
        '        End If
        '        Return ObjRiepilogo
        '    Catch ex As Exception
        '        Log.Debug("setprovvedimentiaccertamentitarsu::si è verificato il seguente errore::", ex)
        '        Return Nothing
        '    Finally
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        'End Function
        Private Function TARSU_SetACCERTATO(ByVal oAccertato() As ObjArticoloAccertamento, ByVal myStringConnection As String, ByVal IdProvvedimento As Long) As Long
            Dim myAccertato As New ObjArticoloAccertamento
            Dim cmdMyCommand As New SqlCommand
            Dim nMyReturn As Integer

            Try
                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                For Each myAccertato In oAccertato
                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    cmdMyCommand.CommandText = "prc_TBLRUOLOACCERTATO_IU"
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = myAccertato.Id
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTE", SqlDbType.Int)).Value = myAccertato.IdContribuente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = myAccertato.IdEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = myAccertato.sAnno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVIA", SqlDbType.Int)).Value = myAccertato.nCodVia
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VIA", SqlDbType.NVarChar)).Value = myAccertato.sVia
                    If IsNumeric(myAccertato.sCivico) Then
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = myAccertato.sCivico
                    Else
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = 0
                    End If
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ESPONENTE", SqlDbType.NVarChar)).Value = myAccertato.sEsponente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@INTERNO", SqlDbType.NVarChar)).Value = myAccertato.sInterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SCALA", SqlDbType.NVarChar)).Value = myAccertato.sScala
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FOGLIO", SqlDbType.NVarChar)).Value = myAccertato.sFoglio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO", SqlDbType.NVarChar)).Value = myAccertato.sNumero
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SUBALTERNO", SqlDbType.NVarChar)).Value = myAccertato.sSubalterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCATEGORIA", SqlDbType.NVarChar)).Value = myAccertato.sCategoria
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTARIFFA", SqlDbType.Int)).Value = myAccertato.nIdTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_TARIFFA", SqlDbType.Float)).Value = myAccertato.impTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MQ", SqlDbType.Float)).Value = myAccertato.nMQ
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI", SqlDbType.Int)).Value = myAccertato.nComponenti
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI_PV", SqlDbType.Int)).Value = myAccertato.nComponentiPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FORZA_CALCOLAPV", SqlDbType.Bit)).Value = myAccertato.bForzaPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@BIMESTRI", SqlDbType.Int)).Value = myAccertato.nBimestri
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = myAccertato.impRuolo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDUZIONI", SqlDbType.Float)).Value = myAccertato.impRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_DETASSAZIONI", SqlDbType.Float)).Value = myAccertato.impDetassazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_NETTO", SqlDbType.Float)).Value = myAccertato.impNetto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_FORZATO", SqlDbType.Int)).Value = myAccertato.bIsImportoForzato
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ISTARSUGIORNALIERA", SqlDbType.Int)).Value = myAccertato.bIsTarsuGiornaliera
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.DateTime)).Value = CDate(myAccertato.tDataInizio).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    If myAccertato.tDataFine = DateTime.MinValue Then
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.DateTime)).Value = System.DBNull.Value
                    Else
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.DateTime)).Value = CDate(myAccertato.tDataFine).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    End If
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = IdProvvedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.NVarChar)).Value = myAccertato.IdLegame
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_RIENTRATO", SqlDbType.Bit)).Value = myAccertato.bIsRientrato
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_RIENTRO", SqlDbType.NVarChar)).Value = myAccertato.sDataRientro
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_SANZIONI", SqlDbType.Float)).Value = myAccertato.ImpSanzioni
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_INTERESSI", SqlDbType.Float)).Value = myAccertato.ImpInteressi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDDETTAGLIOTESTATA", SqlDbType.Int)).Value = myAccertato.IdDettaglioTestata
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTESTATA", SqlDbType.Int)).Value = -1
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NOTEARTICOLO", SqlDbType.NVarChar)).Value = myAccertato.sNote
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOPARTITA", SqlDbType.VarChar)).Value = myAccertato.TipoPartita
                    cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                    'eseguo la query
                    Log.Debug("tarsusetacc->" + Costanti.LogQuery(cmdMyCommand))
                    cmdMyCommand.ExecuteNonQuery()
                    nMyReturn = cmdMyCommand.Parameters("@ID").Value
                    If nMyReturn > 0 Then
                        If Not myAccertato.oRiduzioni Is Nothing Then
                            If myAccertato.oRiduzioni.Length > 0 Then
                                Dim myRid As ObjRidEseApplicati
                                For Each myRid In myAccertato.oRiduzioni
                                    cmdMyCommand.CommandText = "prc_TBLACCERTATORIDUZIONE_IU"
                                    cmdMyCommand.Parameters.Clear()
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = myAccertato.IdEnte
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDACCERTAMENTO", SqlDbType.BigInt)).Value = nMyReturn
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDRIDUZIONE", SqlDbType.BigInt)).Value = myRid.ID
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = IdProvvedimento
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.BigInt)).Value = myAccertato.IdLegame
                                    If cmdMyCommand.ExecuteNonQuery() = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                                        Log.Debug("SetAccertato_tarsu_accertamenti::si è verificato il seguente errore::errore insert TBLACCERTATORIDUZIONE")
                                        Return 0
                                    End If
                                Next
                            End If
                        End If

                        If Not myAccertato.oDetassazioni Is Nothing Then
                            If myAccertato.oDetassazioni.Length > 0 Then
                                Dim myRid As ObjRidEseApplicati
                                For Each myRid In myAccertato.oDetassazioni
                                    cmdMyCommand.CommandText = "prc_TBLACCERTATODETASSAZIONE_IU"
                                    cmdMyCommand.Parameters.Clear()
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = myAccertato.IdEnte
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDACCERTAMENTO", SqlDbType.BigInt)).Value = nMyReturn
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDDETASSAZIONE", SqlDbType.BigInt)).Value = myRid.ID
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = IdProvvedimento
                                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.BigInt)).Value = myAccertato.IdLegame
                                    If cmdMyCommand.ExecuteNonQuery() = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                                        Log.Debug("SetAccertato_tarsu_accertamenti::si è verificato il seguente errore::errore insert TBLACCERTATODETASSAZIONE")
                                        Return 0
                                    End If
                                Next
                            End If
                        End If
                    Else
                        Log.Debug("SetAccertato_tarsu_accertamenti::errore in inserimento TBLRUOLOACCERTATO")
                        Return 0
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log.Debug("SetAccertato_tarsu_accertamenti::si è verificato il seguente errore::", ex)
                Return 0
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        Private Function TARSU_SetDICHIARATO(ByVal myStringConnection As String, myStringConnectionTARSU As String, ByVal oDICHIARATO() As ObjArticoloAccertamento, ByVal IdProvvedimento As Long) As Long
            Dim myDichiarato As New ObjArticoloAccertamento
            Dim cmdMyCommand As New SqlCommand
            Dim nMyReturn As Integer = -1

            Try

                objUtility = New MotoreProvUtility

                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                'se non ho il dichiarato esco e ritorno 1
                If oDICHIARATO Is Nothing Then
                    Log.Debug("TARSU_SetDICHIARATO::dichiarato=nothing OK ritorno 1")
                    Return 1
                End If
                If oDICHIARATO.Length = 0 Then
                    Log.Debug("TARSU_SetDICHIARATO::dichiarato.Length=0 OK ritorno 1")
                    Return 1
                End If
                If oDICHIARATO(0) Is Nothing Then
                    Log.Debug("TARSU_SetDICHIARATO::dichiarato(0)=nothing OK ritorno 1")
                    Return 1
                End If
                If oDICHIARATO(0).IdContribuente = -1 Then
                    Log.Debug("TARSU_SetDICHIARATO::dichiarato(0).idcontribuente=-1 OK ritorno 1")
                    Return 1
                End If

                'DEVO SALVARE oDichiarato in tblruolodichiarato che mi serve poi per la stampa
                For Each myDichiarato In oDICHIARATO
                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    cmdMyCommand.CommandText = "prc_TBLRUOLODICHIARATO_IU"
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = myDichiarato.Id
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTE", SqlDbType.Int)).Value = myDichiarato.IdContribuente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = myDichiarato.IdEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = myDichiarato.sAnno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVIA", SqlDbType.Int)).Value = myDichiarato.nCodVia
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VIA", SqlDbType.NVarChar)).Value = myDichiarato.sVia
                    If IsNumeric(myDichiarato.sCivico) Then
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = myDichiarato.sCivico
                    Else
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = 0
                    End If
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ESPONENTE", SqlDbType.NVarChar)).Value = myDichiarato.sEsponente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@INTERNO", SqlDbType.NVarChar)).Value = myDichiarato.sInterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SCALA", SqlDbType.NVarChar)).Value = myDichiarato.sScala
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FOGLIO", SqlDbType.NVarChar)).Value = myDichiarato.sFoglio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO", SqlDbType.NVarChar)).Value = myDichiarato.sNumero
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SUBALTERNO", SqlDbType.NVarChar)).Value = myDichiarato.sSubalterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCATEGORIA", SqlDbType.NVarChar)).Value = myDichiarato.sCategoria
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTARIFFA", SqlDbType.Int)).Value = myDichiarato.nIdTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_TARIFFA", SqlDbType.Float)).Value = myDichiarato.impTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MQ", SqlDbType.Float)).Value = myDichiarato.nMQ
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI", SqlDbType.Int)).Value = myDichiarato.nComponenti
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI_PV", SqlDbType.Int)).Value = myDichiarato.nComponentiPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FORZA_CALCOLAPV", SqlDbType.Bit)).Value = myDichiarato.bForzaPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@BIMESTRI", SqlDbType.Int)).Value = myDichiarato.nBimestri
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = myDichiarato.impRuolo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDUZIONI", SqlDbType.Float)).Value = myDichiarato.impRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_DETASSAZIONI", SqlDbType.Float)).Value = myDichiarato.impDetassazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_NETTO", SqlDbType.Float)).Value = myDichiarato.impNetto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_FORZATO", SqlDbType.Int)).Value = myDichiarato.bIsImportoForzato
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ISTARSUGIORNALIERA", SqlDbType.Int)).Value = myDichiarato.bIsTarsuGiornaliera
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.DateTime)).Value = CDate(myDichiarato.tDataInizio).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    If myDichiarato.tDataFine = DateTime.MinValue Then
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.DateTime)).Value = System.DBNull.Value
                    Else
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.DateTime)).Value = CDate(myDichiarato.tDataFine).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    End If
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = IdProvvedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.NVarChar)).Value = myDichiarato.IdLegame
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FLAG_RIENTRATO", SqlDbType.Bit)).Value = myDichiarato.bIsRientrato
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_RIENTRO", SqlDbType.NVarChar)).Value = myDichiarato.sDataRientro
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_SANZIONI", SqlDbType.Float)).Value = myDichiarato.ImpSanzioni
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_INTERESSI", SqlDbType.Float)).Value = myDichiarato.ImpInteressi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDDETTAGLIOTESTATA", SqlDbType.Int)).Value = myDichiarato.IdDettaglioTestata
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTESTATA", SqlDbType.Int)).Value = -1
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NOTEARTICOLO", SqlDbType.NVarChar)).Value = myDichiarato.sNote
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOPARTITA", SqlDbType.VarChar)).Value = myDichiarato.TipoPartita
                    cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                    Log.Debug("TARSU_SetDICHIARATO.query->" + cmdMyCommand.CommandText + " VALUES " + Utility.Costanti.GetValParamCmd(cmdMyCommand))
                    'eseguo la query
                    cmdMyCommand.ExecuteNonQuery()
                    nMyReturn = cmdMyCommand.Parameters("@ID").Value
                    If nMyReturn <= 0 Then
                        Dim sValParametri As String = Utility.Costanti.GetValParamCmd(cmdMyCommand)
                        Log.Debug("SetDichiarato_TARSU_Accertamenti::si è verificato il seguente errore::errore insert prc_TBLRUOLODICHIARATO_IU " & sValParametri)
                        Return 0
                    End If
                Next

                'Try
                '    oTestataGet = objDichiarazione.GetDichiarazione(myStringConnectionTARSU, -1, oDICHIARATO(0).IdContribuente, oDICHIARATO(0).sOperatore, oDICHIARATO(0).sAnno)
                '    For intCount = 0 To oDICHIARATO.Length - 1
                '        IdLegameInserimento = -1
                '        For intTestata = 0 To oTestataGet.Length - 1
                '            For intDettaglioTestata = 0 To oTestataGet(intTestata).oDettaglioTestata.Length - 1
                '                For intOggetto = 0 To oTestataGet(intTestata).oDettaglioTestata(intDettaglioTestata).oOggetti.Length - 1
                '                    If oTestataGet(intTestata).oDettaglioTestata(intDettaglioTestata).IdDettaglioTestata = oDICHIARATO(intCount).IdDettaglioTestata Then
                '                        If oTestataGet(intTestata).oDettaglioTestata(intDettaglioTestata).oOggetti(intOggetto).IdCategoria = oDICHIARATO(intCount).sCategoria Then
                '                            oTestataGet(intTestata).oDettaglioTestata(intDettaglioTestata).oOggetti(intOggetto).IdLegame = oDICHIARATO(intCount).IdLegame
                '                        End If
                '                    End If
                '                Next
                '            Next
                '        Next
                '    Next

                '    For intTestata = 0 To oTestataGet.Length - 1
                '        If objDichiarazione.SetDichiarazione(oTestataGet(intTestata), myStringConnection, IdProvvedimento) <= 0 Then
                '            Log.Debug("TARSU_SetDICHIARATO::errore in SetDichiarazione")
                '        End If
                '    Next
                'Catch ex As Exception
                '    Log.Debug("TARSU_SetDICHIARATO.errore in lettura dichiarazione")
                'End Try
                Return 1
            Catch ex As Exception
                Log.Debug("setdichiarato_tarsu_accertamenti::si è verificato il seguente errore::", ex)
                Log.Debug("TARSU_SetDICHIARATO.query errore->" + cmdMyCommand.CommandText + " VALUES " + Utility.Costanti.GetValParamCmd(cmdMyCommand))
                Return 0
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        Private Function TARSU_SetRIEPILOGO_ACCERTATO(ByVal sTipoCalcolo As String, ByVal oDICHIARATO() As ObjArticoloAccertamento, ByVal oAccertato() As ObjArticoloAccertamento, ByVal IdProvvedimento As Long, ByVal myConnectionString As String) As ObjArticoloAccertamento()
            Dim intDichiarato As Integer
            Dim intAccertato As Integer
            Dim oListRiepilogoAccertamento() As ObjArticoloAccertamento
            Dim myRiepAcc As ObjArticoloAccertamento
            Dim myAccertato As New ObjArticoloAccertamento
            Dim cmdMyCommand As New SqlCommand
            Dim nMyReturn As Integer = -1

            Try
                cmdMyCommand.Connection = New SqlConnection(myConnectionString)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0

                intAccertato = -1
                For Each myAccertato In oAccertato
                    intAccertato += 1
                    ReDim Preserve oListRiepilogoAccertamento(intAccertato)
                    myRiepAcc = New ObjArticoloAccertamento
                    myRiepAcc = myAccertato
                    If sTipoCalcolo <> ObjRuolo.TipoCalcolo.TARES Then
                        For intDichiarato = 0 To oDICHIARATO.Length - 1
                            If oDICHIARATO(intDichiarato).IdLegame = myAccertato.IdLegame Then
                                myRiepAcc.impNetto = myRiepAcc.impNetto - oDICHIARATO(intDichiarato).impNetto
                            End If
                        Next
                    End If
                    myRiepAcc.impRuolo = myRiepAcc.impNetto + myAccertato.ImpSanzioni + myAccertato.ImpInteressi
                    myRiepAcc.bIsImportoForzato = True
                    myRiepAcc.Id = IdProvvedimento
                    oListRiepilogoAccertamento(intAccertato) = myRiepAcc

                    cmdMyCommand.CommandType = CommandType.StoredProcedure
                    cmdMyCommand.CommandText = "prc_TBLRIEPILOGOACCERTATOTARSU_IU"
                    cmdMyCommand.Parameters.Clear()
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID", SqlDbType.Int)).Value = myRiepAcc.Id
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCONTRIBUENTE", SqlDbType.Int)).Value = myRiepAcc.IdContribuente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDENTE", SqlDbType.NVarChar)).Value = myRiepAcc.IdEnte
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ANNO", SqlDbType.NVarChar)).Value = myRiepAcc.sAnno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CODVIA", SqlDbType.Int)).Value = myRiepAcc.nCodVia
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@VIA", SqlDbType.NVarChar)).Value = myRiepAcc.sVia
                    If IsNumeric(myAccertato.sCivico) Then
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = myAccertato.sCivico
                    Else
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@CIVICO", SqlDbType.Int)).Value = 0
                    End If
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ESPONENTE", SqlDbType.NVarChar)).Value = myRiepAcc.sEsponente
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@INTERNO", SqlDbType.NVarChar)).Value = myRiepAcc.sInterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SCALA", SqlDbType.NVarChar)).Value = myRiepAcc.sScala
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FOGLIO", SqlDbType.NVarChar)).Value = myRiepAcc.sFoglio
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NUMERO", SqlDbType.NVarChar)).Value = myRiepAcc.sNumero
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SUBALTERNO", SqlDbType.NVarChar)).Value = myRiepAcc.sSubalterno
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDCATEGORIA", SqlDbType.NVarChar)).Value = myRiepAcc.sCategoria
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IDTARIFFA", SqlDbType.Int)).Value = myRiepAcc.nIdTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_TARIFFA", SqlDbType.Float)).Value = myRiepAcc.impTariffa
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MQ", SqlDbType.Float)).Value = myRiepAcc.nMQ
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI", SqlDbType.Int)).Value = myRiepAcc.nComponenti
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@NCOMPONENTI_PV", SqlDbType.Int)).Value = myRiepAcc.nComponentiPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@FORZA_CALCOLAPV", SqlDbType.Bit)).Value = myRiepAcc.bForzaPV
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@BIMESTRI", SqlDbType.Int)).Value = myRiepAcc.nBimestri
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = myRiepAcc.impRuolo
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDUZIONI", SqlDbType.Float)).Value = myRiepAcc.impRiduzione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_DETASSAZIONI", SqlDbType.Float)).Value = myRiepAcc.impDetassazione
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_NETTO", SqlDbType.Float)).Value = myRiepAcc.impNetto
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_FORZATO", SqlDbType.Int)).Value = myRiepAcc.bIsImportoForzato
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ISTARSUGIORNALIERA", SqlDbType.Int)).Value = myRiepAcc.bIsTarsuGiornaliera
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.DateTime)).Value = CDate(myRiepAcc.tDataInizio).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.DateTime)).Value = CDate(myRiepAcc.tDataFine).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = IdProvvedimento
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_SANZIONI", SqlDbType.Float)).Value = myRiepAcc.ImpSanzioni
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_INTERESSI", SqlDbType.Float)).Value = myRiepAcc.ImpInteressi
                    cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TIPOPARTITA", SqlDbType.VarChar)).Value = myRiepAcc.TipoPartita
                    cmdMyCommand.Parameters("@ID").Direction = ParameterDirection.InputOutput
                    'eseguo la query
                    Log.Debug("tarsusetripiepilogo->" + Costanti.LogQuery(cmdMyCommand))
                    cmdMyCommand.ExecuteNonQuery()
                    nMyReturn = cmdMyCommand.Parameters("@ID").Value
                    If nMyReturn <= 0 Then
                        Log.Debug("SetRIEPILOGO_tarsu_accertamenti::si è verificato il seguente errore::errore insert TBLRIEPILOGOACCERTATOTARSU")
                        Return Nothing
                    End If
                Next
            Catch ex As Exception
                Log.Debug("Setriepilogo_accertato_tarsu::si è verificato il seguente errore::", ex)
                oListRiepilogoAccertamento = Nothing
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
            Return oListRiepilogoAccertamento
        End Function
        Private Function TARSU_SetDettaglioAddizionali(IdEnte As String, ByVal nIDProvvedimento As Long, ByVal oListAddizionali() As OggettoAddizionaleAccertamento, myStringConnection As String) As Long
            Dim intCount As Integer
            Dim intRetVal As Long
            Dim cmdMyCommand As New SqlCommand

            Try
                cmdMyCommand.Connection = New SqlConnection(myStringConnection)
                cmdMyCommand.Connection.Open()
                cmdMyCommand.CommandTimeout = 0
                cmdMyCommand.CommandType = CommandType.Text

                If Not oListAddizionali Is Nothing Then
                    For intCount = 0 To oListAddizionali.Length - 1
                        cmdMyCommand.CommandType = CommandType.StoredProcedure
                        cmdMyCommand.CommandText = "prc_DETTAGLIO_VOCI_ACCERTAMENTI_IU"
                        cmdMyCommand.Parameters.Clear()
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_ENTE", SqlDbType.NVarChar)).Value = IdEnte
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_PROVVEDIMENTO", SqlDbType.BigInt)).Value = nIDProvvedimento
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_VOCE", SqlDbType.NVarChar)).Value = oListAddizionali(intCount).CodiceCapitolo
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO", SqlDbType.Float)).Value = oListAddizionali(intCount).ImportoCalcolato
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_INIZIO", SqlDbType.NVarChar)).Value = ""
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@DATA_FINE", SqlDbType.NVarChar)).Value = ""
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_IMMOBILE_PROGRESSIVO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ID_LEGAME", SqlDbType.NVarChar)).Value = ""
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_RIDOTTO", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_ACCONTO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_SEMESTRI_SALDO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@TASSO", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@ACCONTO_GIORNI", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@COD_TIPO_PROVVEDIMENTO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@IMPORTO_GIORNI", SqlDbType.Float)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@MOTIVAZIONE", SqlDbType.NVarChar)).Value = ""
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_ACCONTO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@N_GIORNI_SALDO", SqlDbType.Int)).Value = 0
                        cmdMyCommand.Parameters.Add(New SqlClient.SqlParameter("@SALDO_GIORNI", SqlDbType.Float)).Value = 0
                        Log.Debug("tarsusetaddizionali->" + Costanti.LogQuery(cmdMyCommand))
                        intRetVal = cmdMyCommand.ExecuteNonQuery
                        If intRetVal = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                            Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DETTAGLIO_ADDIZIONALI_TARSU_INSERT::DBOPENgovProvvedimentiUpdate")
                        End If
                    Next
                End If
                Return 1
            Catch ex As Exception
                Log.Debug("Si è verificato un errore in DBOPENgovProvvedimentiUpdate::DETTAGLIO_ADDIZIONALI_TARSU_INSERT::" & ex.Message)
                Return -1
            Finally
                cmdMyCommand.Connection.Close()
                cmdMyCommand.Dispose()
            End Try
        End Function
        <AutoComplete()>
        Public Function TARSU_DeleteProvvedimentiAccertamento(ByVal myStringConnection As String, ByVal ID_PROCEDIMENTO As Long, ByVal ID_PROVVEDIMENTO As Long) As Boolean
            'Dim intRetVal As Integer
            Dim strSQL As String

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myStringConnection)
                    'CANCELLO TBLTESTATATARSU TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLTESTATATARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLRUOLODICHIARATO TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLRUOLODICHIARATO WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLOGGETTITARSU TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLOGGETTITARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLDETTAGLIOTESTATATARSU TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLDETTAGLIOTESTATATARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLDETTAGLIOTESTATARIDUZIONI TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLDETTAGLIOTESTATARIDUZIONI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLDETTAGLIOTESTATADETASSAZIONI TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLDETTAGLIOTESTATADETASSAZIONI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)

                    'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM PROVVEDIMENTI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TAB_PROCEDIMENTO TRAMITE ID_PROCEDIMENTO
                    strSQL = "DELETE FROM TAB_PROCEDIMENTI WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLRUOLOACCERTATO TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLRUOLOACCERTATO WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLRIEPILOGOACCERTATOTARSU TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLRIEPILOGOACCERTATOTARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO DETTAGLIO_VOCI_ACCERTAMENTI TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM DETTAGLIO_VOCI_ACCERTAMENTI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLACCERTATORIDUZIONE TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLACCERTATORIDUZIONE WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO TBLACCERTATODETASSAZIONE TRAMITE ID_PROVVEDIMENTO
                    strSQL = "DELETE FROM TBLACCERTATODETASSAZIONE WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)
                    'CANCELLO IL PROVVEDIMENTO RETTIFICATO LEGATO AL PROVVEDIMENTO
                    strSQL = "DELETE FROM TP_PROVVEDIMENTI_RETTIFICATI WHERE ID_PROVVEDIMENTO_FIGLIO=" & ID_PROVVEDIMENTO
                    ctx.ExecuteNonQuery(strSQL)

                    ctx.Dispose()
                End Using

                Return True
            Catch ex As Exception
                Log.Debug("DeleteProvvedimentiAccertamentoTARSU:: si è verificato il seguente errore::", ex)
            End Try
        End Function
        '<AutoComplete()>
        'Public Function TARSU_DeleteProvvedimentiAccertamento(ByVal myStringConnection As String, ByVal ID_PROCEDIMENTO As Long, ByVal ID_PROVVEDIMENTO As Long) As Boolean
        '    'Dim intRetVal As Integer
        '    Dim strSQL As String

        '    Try
        '        objDBManager = New DBManager

        '        objDBManager.Initialize(myStringConnection)

        '        'CANCELLO TBLTESTATATARSU TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLTESTATATARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLRUOLODICHIARATO TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLRUOLODICHIARATO WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLOGGETTITARSU TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLOGGETTITARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLDETTAGLIOTESTATATARSU TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLDETTAGLIOTESTATATARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLDETTAGLIOTESTATARIDUZIONI TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLDETTAGLIOTESTATARIDUZIONI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLDETTAGLIOTESTATADETASSAZIONI TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLDETTAGLIOTESTATADETASSAZIONI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)


        '        'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM PROVVEDIMENTI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TAB_PROCEDIMENTO TRAMITE ID_PROCEDIMENTO
        '        strSQL = "DELETE FROM TAB_PROCEDIMENTI WHERE ID_PROCEDIMENTO=" & ID_PROCEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLRUOLOACCERTATO TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLRUOLOACCERTATO WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLRIEPILOGOACCERTATOTARSU TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLRIEPILOGOACCERTATOTARSU WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO DETTAGLIO_VOCI_ACCERTAMENTI TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM DETTAGLIO_VOCI_ACCERTAMENTI WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLACCERTATORIDUZIONE TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLACCERTATORIDUZIONE WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO TBLACCERTATODETASSAZIONE TRAMITE ID_PROVVEDIMENTO
        '        strSQL = "DELETE FROM TBLACCERTATODETASSAZIONE WHERE ID_PROVVEDIMENTO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)
        '        'CANCELLO IL PROVVEDIMENTO RETTIFICATO LEGATO AL PROVVEDIMENTO
        '        strSQL = "DELETE from TP_PROVVEDIMENTI_RETTIFICATI WHERE ID_PROVVEDIMENTO_FIGLIO=" & ID_PROVVEDIMENTO
        '        objDBManager.Execute(strSQL)

        '        'If intRetVal = COSTANTValue.CostantiProv.init_value_number Then
        '        '    'Throw New Exception("Application::COMPlusOPENgovProvvedimenti::Function::DeleteProvvedimentiLiquidazioni::DBOPENgovProvvedimentiUpdate")
        '        '    Return False
        '        'End If

        '        Return True
        '    Catch ex As Exception
        '        Log.Debug("DeleteProvvedimentiAccertamentoTARSU:: si è verificato il seguente errore::", ex)
        '    Finally
        '        If Not IsNothing(objDBManager) Then
        '            objDBManager.Kill()
        '            objDBManager.Dispose()
        '        End If
        '    End Try
        'End Function
        Private Function TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(IdEnte As String, ByVal lngIDProvvedimento As Long, ByVal objSanzioni As DataSet, ByVal ObjInteressiSanzioni As DataSet, ListInteressi() As ObjInteressiSanzioni, ByVal objAddizionali() As OggettoAddizionaleAccertamento, myStringConnection As String) As Long
            Dim intRetVal As Integer = 0

            Try
                intRetVal = DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, objSanzioni, myStringConnection)
                If intRetVal <> 1 Then
                    Return 0
                End If
                If Not ObjInteressiSanzioni Is Nothing Then
                    intRetVal = DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, ObjInteressiSanzioni, myStringConnection)
                    If intRetVal <> 1 Then
                        Return 0
                    End If
                End If
                If Not ListInteressi Is Nothing Then
                    intRetVal = DETTAGLIO_VOCI_ACCERTAMENTI_INSERT(IdEnte, lngIDProvvedimento, ListInteressi, myStringConnection)
                    If intRetVal <> 1 Then
                        Return 0
                    End If
                End If
                '*** 20130801 - accertamento OSAP ***
                If Not IsNothing(objAddizionali) Then
                    intRetVal = TARSU_SetDettaglioAddizionali(IdEnte, lngIDProvvedimento, objAddizionali, myStringConnection)
                    If intRetVal <> 1 Then
                        Return 0
                    End If
                End If
                '*** ***
                Return 1
            Catch ex As Exception
                Log.Debug("setDETTAGLIO_VOCI_ACCERTAMENTI_TARSU::si è verificato il seguente errore::", ex)
                Return 0
            End Try
        End Function
        '*** ***
#End Region

#Region "OSAP"
        '*** 20130801 - accertamento OSAP ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myHashTable"></param>
        ''' <param name="dsSanzioni"></param>
        ''' <param name="dsSanzioniImpDicVSImpPag"></param>
        ''' <param name="dsSanzioniScadDicVSDataPag"></param>
        ''' <param name="dsInteressi"></param>
        ''' <param name="dsInteressiImpDicVSImpPag"></param>
        ''' <param name="dsInteressiScadDicVSDataPag"></param>
        ''' <param name="oAtto"></param>
        ''' <param name="objDichiaratoOSAP"></param>
        ''' <param name="objAccertatoOSAP"></param>
        ''' <param name="Operatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function OSAP_SetProvvedimenti(myDBType As String, sCODEnte As String, IdContribuente As Integer, myHashTable As Hashtable, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As ComPlusInterface.OggettoAttoOSAP, ByVal objDichiaratoOSAP() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal objAccertatoOSAP() As ComPlusInterface.OSAPAccertamentoArticolo, Operatore As String) As ComPlusInterface.OSAPAccertamentoArticolo()
            Try
                Log.Debug("OSAP_SetProvvedimenti::inizio")
                Dim oToReturn() As ComPlusInterface.OSAPAccertamentoArticolo
                'Dim sSQL as string
                Dim objDSAnagraficaIndirizziSpedizione As DataSet
                'Dim intCount As Integer
                Dim lngIDProcedimento As Long
                Dim sCODTributo As String = CType(myHashTable("CODTRIBUTO"), String)
                Dim intRetVal As Integer
                Dim sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP As String
                Dim myAtto As New OggettoAtto
                Dim sANNO, TIPO_PROVVEDIMENTO As String
                Dim dblIMPORTODICHIARATO, dblIMPORTOACCERTATO As Double

                objUtility = New MotoreProvUtility

                Dim FncDBSelect As New DBOPENgovProvvedimentiSelect

                Dim TIPO_OPERAZIONE_RETTIFICA As Boolean = myHashTable("TIPO_OPERAZIONE_RETTIFICA")                 'NORMALE-RETTIFICA
                Dim DATA_RETTIFICA As String = myHashTable("DATA_RETTIFICA")
                Dim DATA_ANNULLAMENTO As String = myHashTable("DATA_ANNULLAMENTO")
                Dim ID_PROVVEDIMENTO_OLD As Long
                If TIPO_OPERAZIONE_RETTIFICA = True Then
                    ID_PROVVEDIMENTO_OLD = myHashTable("ID_PROVVEDIMENTO_RETTIFICA")
                End If

                TIPO_PROVVEDIMENTO = myHashTable("TIPOPROVVEDIMENTO")
                sANNO = myHashTable("ANNOACCERTAMENTO")

                Log.Debug("letto i primi objhastable")
                '*** 201810 - Generazione Massiva Atti ***
                '#### tolto test perché passato sempre =0 #############################################################################################################################
                'SE VALORE_RITORNO_ACCERTAMENTO=4 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO.
                'SE VALORE_RITORNO_ACCERTAMENTO=5 VUOL DIRE CHE NELLA BASE DATI E' PRESENTE, PER CONTRIB E ANNO, UN ACCERTAMENTO NON DEFINITIVO E UN ACCERTAMENTO NON DEFINITIVO  (in questo caso cancello però solo l'accertamento e non il pre accertamento)
                'QUINDI SVUOTO I DATI PRESENTI E SALVO QUELLI DEL NUOVO ACCERTAMENTO
                'If myHashTable("VALORE_RITORNO_ACCERTAMENTO") = 4 Or myHashTable("VALORE_RITORNO_ACCERTAMENTO") = 0 Or myHashTable("VALORE_RITORNO_ACCERTAMENTO") = 6 Then
                Log.Debug("devo ripulire pendente")
                Dim ID_PROCEDIMENTO As Long
                Dim ID_PROVVEDIMENTO As Long
                Dim DATA_CONFERMA As String = ""
                'FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA, myHashTable, intCODContribuente, sANNO, sCODEnte, "A", myHashTable("CODTRIBUTO"))
                FncDBSelect.getIDProcedimentoDefinitivoPendenteContribuente(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), sCODEnte, IdContribuente, sANNO, "A", myHashTable("CODTRIBUTO"), ID_PROCEDIMENTO, ID_PROVVEDIMENTO, DATA_CONFERMA)
                'VALORE_RITORNO_ACCERTAMENTO=4 LA DATA CONFERMA E' PER FORZA VUOTA
                If DATA_CONFERMA.CompareTo("") = 0 Then
                    'vuol dire che ho trovato un atto non definitivo oppure nessun atto
                    If ID_PROVVEDIMENTO <> 0 Then
                        'OSAP_DeleteProvvedimentiAccertamento(objDBManager, ID_PROVVEDIMENTO)
                        DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), ID_PROVVEDIMENTO, Operatore)
                    End If
                End If
                'End If
                Log.Debug("sommo imp dich e imp acc")
                '*** 20101001 - gli importi sono già tutti regisati nell'oggetto ***
                If Not IsNothing(objDichiaratoOSAP) Then
                    For Each oMyDic As ComPlusInterface.OSAPAccertamentoArticolo In objDichiaratoOSAP
                        If Not IsNothing(oMyDic.Calcolo) Then
                            dblIMPORTODICHIARATO += oMyDic.Calcolo.ImportoCalcolato
                        End If
                    Next
                End If
                If Not IsNothing(objAccertatoOSAP) Then
                    For Each oMyAcc As ComPlusInterface.OSAPAccertamentoArticolo In objAccertatoOSAP
                        If Not IsNothing(oMyAcc.Calcolo) Then
                            dblIMPORTOACCERTATO += oMyAcc.Calcolo.ImportoCalcolato
                        End If
                    Next
                End If
                intRetVal = 0
                Log.Debug("prelevo anagrafe")
                objDSAnagraficaIndirizziSpedizione = FncDBSelect.getAnagraficaIndirizziSpedizione(myDBType, IdContribuente, Utility.Costanti.TRIBUTO_OSAP, myHashTable("CONNECTIONSTRINGANAGRAFICA"))
                If objDSAnagraficaIndirizziSpedizione Is Nothing Then
                    sCOGNOME_INVIO = ""
                    sVIA_RCP = ""
                    sPOSIZIONE_CIV_RCP = ""
                    sCIVICO_RCP = ""
                    sESPONENTE_CIVICO_RCP = ""
                    sCAP_RCP = ""
                    sFRAZIONE_RCP = ""
                    sCOMUNE_RCP = ""
                    sPROVINCIA_RCP = ""
                Else
                    Try
                        Dim rowIndirizziSpedizione As DataRow = objDSAnagraficaIndirizziSpedizione.Tables("TP_INDIRIZZI_SPEDIZIONE").Rows(0)
                        sCOGNOME_INVIO = objUtility.CStrToDB(rowIndirizziSpedizione("COGNOME_INVIO")).ToUpper
                        sVIA_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("VIA_RCP")).ToUpper
                        sPOSIZIONE_CIV_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("POSIZIONE_CIV_RCP")).ToUpper
                        sCIVICO_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("CIVICO_RCP")).ToUpper
                        sESPONENTE_CIVICO_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("ESPONENTE_CIVICO_RCP")).ToUpper
                        sCAP_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("CAP_RCP")).ToUpper
                        sFRAZIONE_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("FRAZIONE_RCP")).ToUpper
                        sCOMUNE_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("COMUNE_RCP")).ToUpper
                        sPROVINCIA_RCP = objUtility.CStrToDB(rowIndirizziSpedizione("PROVINCIA_RCP")).ToUpper
                    Catch ex As IndexOutOfRangeException
                        sCOGNOME_INVIO = ""
                        sVIA_RCP = ""
                        sPOSIZIONE_CIV_RCP = ""
                        sCIVICO_RCP = ""
                        sESPONENTE_CIVICO_RCP = ""
                        sCAP_RCP = ""
                        sFRAZIONE_RCP = ""
                        sCOMUNE_RCP = ""
                        sPROVINCIA_RCP = ""
                    End Try
                End If
                Log.Debug("valorizzo atto")
                oAtto.COD_ENTE = sCODEnte
                oAtto.ID_PROVVEDIMENTO = FncDBSelect.getNewID("PROVVEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                oAtto.ANNO = myHashTable("ANNOACCERTAMENTO")
                oAtto.COD_TRIBUTO = sCODTributo
                oAtto.TipoProvvedimento = TIPO_PROVVEDIMENTO
                Log.Debug("richiamo setprovvedimento")

                myAtto = CastToOggettoAtto(oAtto)
                If SetProvvedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, Operatore) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    'If OSAP_SetProvvedimento(objDBManager, oAtto, sCOGNOME_INVIO, sVIA_RCP, sPOSIZIONE_CIV_RCP, sCIVICO_RCP, sESPONENTE_CIVICO_RCP, sCAP_RCP, sFRAZIONE_RCP, sCOMUNE_RCP, sPROVINCIA_RCP) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    Log.Debug("SetProvvedimentiAccertamentiOSAP:errore inserimento provvedimento")
                    Throw New Exception("SetProvvedimentiAccertamentiOSAP")
                    Return Nothing
                Else
                    oAtto.ID_PROVVEDIMENTO = myAtto.ID_PROVVEDIMENTO
                End If

                lngIDProcedimento = FncDBSelect.getNewID("TAB_PROCEDIMENTI", myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                If SetProcedimento(myDBType, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), myAtto, lngIDProcedimento, myAtto.TipoProvvedimento) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    'If OSAP_SetProcedimento(objDBManager, oAtto, lngIDProcedimento, TIPO_PROVVEDIMENTO, dblIMPORTOACCERTATO, dblIMPORTODICHIARATO) = COSTANTValue.CostantiProv.INIT_VALUE_NUMBER Then
                    'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
                    DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                    Log.Debug("SetProvvedimentiAccertamentiOSAP::errore inserimento procedimento")
                    Throw New Exception("SetProvvedimentiAccertamentiOSAP")
                    Return Nothing
                End If

                'inserisco DICHIARATO OSAP
                If Not IsNothing(objDichiaratoOSAP) Then
                    If OSAP_SetDichiaratoListArticoli(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), COSTANTValue.CostantiProv.AMBITO_DICHIARATO, oAtto, objDichiaratoOSAP, oAtto.ID_PROVVEDIMENTO) <> 1 Then
                        'CANCELLO PROVVEDIMENTI e PROCEDIMEnTI TRAMITE ID_PROVVEDIMENTO
                        DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                        Log.Debug("SetProvvedimentiAccertamentiOSAP::errore inserimento accertato")
                        Throw New Exception("SetProvvedimentiAccertamentiOSAP")
                        Return Nothing
                    End If
                End If
                'inserisco ACCERTATO OSAP
                If OSAP_SetDichiaratoListArticoli(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), COSTANTValue.CostantiProv.AMBITO_ACCERTATO, oAtto, objAccertatoOSAP, oAtto.ID_PROVVEDIMENTO) <> 1 Then
                    'CANCELLO PROVVEDIMENTI e PROCEDIMEnTI TRAMITE ID_PROVVEDIMENTO
                    DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                    Log.Debug("SetProvvedimentiAccertamentiOSAP::errore inserimento accertato")
                    Throw New Exception("SetProvvedimentiAccertamentiOSAP")
                    Return Nothing
                End If

                'inserisco sanzioni ed interessi di fase confronto dichiarato-accertato
                If TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioni, dsInteressi, Nothing, Nothing, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) <> 1 Then
                    DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If
                'inserisco sanzioni ed interessi di fase confronto importo dichiarato-importo pagato
                If TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioniImpDicVSImpPag, dsInteressiImpDicVSImpPag, Nothing, Nothing, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) <> 1 Then
                    DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If
                'inserisco sanzioni ed interessi di fase confronto scad dichiarato-data pagato
                If TARSU_SetDETTAGLIO_VOCI_ACCERTAMENTI(oAtto.COD_ENTE, oAtto.ID_PROVVEDIMENTO, dsSanzioniScadDicVSDataPag, dsInteressiScadDicVSDataPag, Nothing, Nothing, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI")) <> 1 Then
                    DeleteAtto(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), oAtto.ID_PROVVEDIMENTO, Operatore) 'OSAP_DeleteProvvedimentiAccertamento(objDBManager, oAtto.ID_PROVVEDIMENTO)
                    Return Nothing
                End If

                If TIPO_OPERAZIONE_RETTIFICA = True Then
                    SetPROVVEDIMENTIRETTIFICATI(myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"), IdContribuente, sCODTributo, sCODEnte, oAtto.ID_PROVVEDIMENTO, ID_PROVVEDIMENTO_OLD, DATA_RETTIFICA, True, DATA_ANNULLAMENTO)
                End If
                oToReturn = objAccertatoOSAP
                oToReturn(0).IdProvvedimento = oAtto.ID_PROVVEDIMENTO
                Return oToReturn
            Catch ex As Exception
                Log.Debug("SetProvvedimentiAccertamentiOSAP::si è verificato il seguente errore::" & ex.Message)
                Throw New Exception("SetProvvedimentiAccertamentiOSAP::" & " " & ex.Message)
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MyDBManager"></param>
        ''' <param name="nIdProvvedimento"></param>
        ''' <param name="sOperatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function DeleteAtto(myConnectionString As String, ByVal nIdProvvedimento As Integer, sOperatore As String) As Boolean
            Dim sSQL As String

            Try
                Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myConnectionString)
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_DeleteProvvedimento", "IDPROVVEDIMENTO", "OPERATORE")
                    ctx.ExecuteNonQuery(sSQL, ctx.GetParam("IDPROVVEDIMENTO", nIdProvvedimento), ctx.GetParam("OPERATORE", sOperatore))
                    ctx.Dispose()
                End Using
                Log.Debug("Cancellato IdProvvedimento:" & nIdProvvedimento)
                Return True
            Catch ex As Exception
                Log.Debug("DeleteAtto.errore: ", ex)
                Throw New Exception("DeleteAtto::" & ex.Message)
            End Try
        End Function
        '<AutoComplete()>
        'Public Function OSAP_DeleteProvvedimentiAccertamento(ByVal MyDBManager As DBManager, ByVal IDProvvedimento As Long) As Boolean
        '    Try
        '        Dim cmdMyCommand As New SqlCommand
        '        cmdMyCommand.Connection = New SqlConnection(MyDBManager.GetConnection.ConnectionString)
        '        cmdMyCommand.CommandType = CommandType.StoredProcedure
        '        'CANCELLO PROVVEDIMENTI TRAMITE ID_PROVVEDIMENTO
        '        cmdMyCommand.CommandText = "prc_DeleteProvvedimento"
        '        cmdMyCommand.Parameters.Clear()
        '        cmdMyCommand.Parameters.AddWithValue("@IdProvvedimento", IDProvvedimento)
        '        Log.Debug("OSAP_DeleteProvvedimentiAccertamento::query:: " & cmdMyCommand.CommandText & " ::param:: " & Utility.Costanti.GetValParamCmd(cmdMyCommand))
        '        objDBManager.Execute(cmdMyCommand)

        '        Return True
        '    Catch ex As Exception
        '        Log.Debug("DeleteProvvedimentiAccertamentoOSAP::si è verificato il seguente errore::" & ex.Message)
        '        Throw New Exception("DeleteProvvedimentiAccertamentoOSAP::" & ex.Message)
        '    End Try
        'End Function
        Private Function OSAP_SetDichiaratoListArticoli(ByVal myConnectionString As String, ByVal Ambito As Integer, ByVal oAtto As ComPlusInterface.OggettoAttoOSAP, ByVal oListArticoli() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal IdProvvedimento As Long) As Integer
            Dim x, y As Integer
            Dim intRetVal As Integer
            Dim myIdentity As Long
            Dim sSQL As String
            Dim myDataView As New DataView

            objUtility = New MotoreProvUtility

            Try
                For x = 0 To oListArticoli.Length - 1
                    If Not IsNothing(oListArticoli(x).Calcolo) Then
                        intRetVal = 0
                        Using ctx As New DBModel(COSTANTValue.CostantiProv.DBType, myConnectionString)
                            Try
                                If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
                                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_SetOSAPDichiarato", "IDPOSIZIONE" _
                                        , "IDENTE" _
                                        , "ANNO" _
                                        , "IDCONTRIBUENTE" _
                                        , "IDDICHIARAZIONE" _
                                        , "IDARTICOLO" _
                                        , "IDTRIBUTO" _
                                        , "CODVIA" _
                                        , "CIVICO" _
                                        , "ESPONENTE" _
                                        , "INTERNO" _
                                        , "SCALA" _
                                        , "IDCATEGORIA" _
                                        , "IDTIPOLOGIAOCCUPAZIONE" _
                                        , "CONSISTENZA" _
                                        , "IDTIPOCONSISTENZA" _
                                        , "DATAINIZIOOCCUPAZIONE" _
                                        , "DATAFINEOCCUPAZIONE" _
                                        , "IDDURATA" _
                                        , "DURATAOCCUPAZIONE" _
                                        , "MAGGIORAZIONE_IMPORTO" _
                                        , "MAGGIORAZIONE_PERC" _
                                        , "NOTE" _
                                        , "DETRAZIONE_IMPORTO" _
                                        , "ATTRAZIONE" _
                                        , "OPERATORE" _
                                        , "DATA_INSERIMENTO" _
                                        , "TARIFFA_APPLICATA" _
                                        , "IMPORTO_LORDO" _
                                        , "IMPORTO" _
                                        , "ID_PROVVEDIMENTO" _
                                        , "ID_LEGAME")
                                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("IDPOSIZIONE", -1) _
                                            , ctx.GetParam("IDENTE", oAtto.COD_ENTE) _
                                            , ctx.GetParam("ANNO", oAtto.ANNO) _
                                            , ctx.GetParam("IDCONTRIBUENTE", oAtto.COD_CONTRIBUENTE) _
                                            , ctx.GetParam("IDDICHIARAZIONE", oListArticoli(x).IdDichiarazione) _
                                            , ctx.GetParam("IDARTICOLO", oListArticoli(x).IdArticolo) _
                                            , ctx.GetParam("IDTRIBUTO", oListArticoli(x).IdTributo) _
                                            , ctx.GetParam("CODVIA", oListArticoli(x).CodVia) _
                                            , ctx.GetParam("CIVICO", oListArticoli(x).Civico) _
                                            , ctx.GetParam("ESPONENTE", oListArticoli(x).Esponente) _
                                            , ctx.GetParam("INTERNO", oListArticoli(x).Interno) _
                                            , ctx.GetParam("SCALA", oListArticoli(x).Scala) _
                                            , ctx.GetParam("IDCATEGORIA", oListArticoli(x).Categoria.IdCategoria) _
                                            , ctx.GetParam("IDTIPOLOGIAOCCUPAZIONE", oListArticoli(x).TipologiaOccupazione.IdTipologiaOccupazione) _
                                            , ctx.GetParam("CONSISTENZA", oListArticoli(x).Consistenza) _
                                            , ctx.GetParam("IDTIPOCONSISTENZA", oListArticoli(x).TipoConsistenzaTOCO.IdTipoConsistenza) _
                                            , ctx.GetParam("DATAINIZIOOCCUPAZIONE", oListArticoli(x).DataInizioOccupazione) _
                                            , ctx.GetParam("DATAFINEOCCUPAZIONE", oListArticoli(x).DataFineOccupazione) _
                                            , ctx.GetParam("IDDURATA", oListArticoli(x).TipoDurata.IdDurata) _
                                            , ctx.GetParam("DURATAOCCUPAZIONE", oListArticoli(x).DurataOccupazione) _
                                            , ctx.GetParam("MAGGIORAZIONE_IMPORTO", oListArticoli(x).MaggiorazioneImporto) _
                                            , ctx.GetParam("MAGGIORAZIONE_PERC", oListArticoli(x).MaggiorazionePerc) _
                                            , ctx.GetParam("NOTE", oListArticoli(x).Note) _
                                            , ctx.GetParam("DETRAZIONE_IMPORTO", oListArticoli(x).DetrazioneImporto) _
                                            , ctx.GetParam("ATTRAZIONE", oListArticoli(x).Attrazione) _
                                            , ctx.GetParam("OPERATORE", oListArticoli(x).Operatore) _
                                            , ctx.GetParam("DATA_INSERIMENTO", oListArticoli(x).DataInserimento) _
                                            , ctx.GetParam("TARIFFA_APPLICATA", oListArticoli(x).Calcolo.TariffaApplicata) _
                                            , ctx.GetParam("IMPORTO_LORDO", oListArticoli(x).Calcolo.ImportoLordo) _
                                            , ctx.GetParam("IMPORTO", oListArticoli(x).Calcolo.ImportoCalcolato) _
                                            , ctx.GetParam("ID_PROVVEDIMENTO", IdProvvedimento) _
                                            , ctx.GetParam("ID_LEGAME", oListArticoli(x).IdLegame))
                                Else
                                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_SetOSAPAccertato", "IDPOSIZIONE" _
                                        , "IDENTE" _
                                        , "ANNO" _
                                        , "IDCONTRIBUENTE" _
                                        , "IDDICHIARAZIONE" _
                                        , "IDARTICOLO" _
                                        , "IDTRIBUTO" _
                                        , "CODVIA" _
                                        , "CIVICO" _
                                        , "ESPONENTE" _
                                        , "INTERNO" _
                                        , "SCALA" _
                                        , "IDCATEGORIA" _
                                        , "IDTIPOLOGIAOCCUPAZIONE" _
                                        , "CONSISTENZA" _
                                        , "IDTIPOCONSISTENZA" _
                                        , "DATAINIZIOOCCUPAZIONE" _
                                        , "DATAFINEOCCUPAZIONE" _
                                        , "IDDURATA" _
                                        , "DURATAOCCUPAZIONE" _
                                        , "MAGGIORAZIONE_IMPORTO" _
                                        , "MAGGIORAZIONE_PERC" _
                                        , "NOTE" _
                                        , "DETRAZIONE_IMPORTO" _
                                        , "ATTRAZIONE" _
                                        , "OPERATORE" _
                                        , "DATA_INSERIMENTO" _
                                        , "TARIFFA_APPLICATA" _
                                        , "IMPORTO_LORDO" _
                                        , "IMPORTO" _
                                        , "IMPORTO_DIFFIMPOSTA" _
                                        , "IMPORTO_SANZIONI" _
                                        , "IMPORTO_SANZIONI_RIDOTTO" _
                                        , "IMPORTO_INTERESSI" _
                                        , "ID_PROVVEDIMENTO" _
                                        , "ID_LEGAME")
                                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("IDPOSIZIONE", -1) _
                                            , ctx.GetParam("IDENTE", oAtto.COD_ENTE) _
                                            , ctx.GetParam("ANNO", oAtto.ANNO) _
                                            , ctx.GetParam("IDCONTRIBUENTE", oAtto.COD_CONTRIBUENTE) _
                                            , ctx.GetParam("IDDICHIARAZIONE", oListArticoli(x).IdDichiarazione) _
                                            , ctx.GetParam("IDARTICOLO", oListArticoli(x).IdArticolo) _
                                            , ctx.GetParam("IDTRIBUTO", oListArticoli(x).IdTributo) _
                                            , ctx.GetParam("CODVIA", oListArticoli(x).CodVia) _
                                            , ctx.GetParam("CIVICO", oListArticoli(x).Civico) _
                                            , ctx.GetParam("ESPONENTE", oListArticoli(x).Esponente) _
                                            , ctx.GetParam("INTERNO", oListArticoli(x).Interno) _
                                            , ctx.GetParam("SCALA", oListArticoli(x).Scala) _
                                            , ctx.GetParam("IDCATEGORIA", oListArticoli(x).Categoria.IdCategoria) _
                                            , ctx.GetParam("IDTIPOLOGIAOCCUPAZIONE", oListArticoli(x).TipologiaOccupazione.IdTipologiaOccupazione) _
                                            , ctx.GetParam("CONSISTENZA", oListArticoli(x).Consistenza) _
                                            , ctx.GetParam("IDTIPOCONSISTENZA", oListArticoli(x).TipoConsistenzaTOCO.IdTipoConsistenza) _
                                            , ctx.GetParam("DATAINIZIOOCCUPAZIONE", oListArticoli(x).DataInizioOccupazione) _
                                            , ctx.GetParam("DATAFINEOCCUPAZIONE", oListArticoli(x).DataFineOccupazione) _
                                            , ctx.GetParam("IDDURATA", oListArticoli(x).TipoDurata.IdDurata) _
                                            , ctx.GetParam("DURATAOCCUPAZIONE", oListArticoli(x).DurataOccupazione) _
                                            , ctx.GetParam("MAGGIORAZIONE_IMPORTO", oListArticoli(x).MaggiorazioneImporto) _
                                            , ctx.GetParam("MAGGIORAZIONE_PERC", oListArticoli(x).MaggiorazionePerc) _
                                            , ctx.GetParam("NOTE", oListArticoli(x).Note) _
                                            , ctx.GetParam("DETRAZIONE_IMPORTO", oListArticoli(x).DetrazioneImporto) _
                                            , ctx.GetParam("ATTRAZIONE", oListArticoli(x).Attrazione) _
                                            , ctx.GetParam("OPERATORE", oListArticoli(x).Operatore) _
                                            , ctx.GetParam("DATA_INSERIMENTO", oListArticoli(x).DataInserimento) _
                                            , ctx.GetParam("TARIFFA_APPLICATA", oListArticoli(x).Calcolo.TariffaApplicata) _
                                            , ctx.GetParam("IMPORTO_LORDO", oListArticoli(x).Calcolo.ImportoLordo) _
                                            , ctx.GetParam("IMPORTO", oListArticoli(x).Calcolo.ImportoCalcolato) _
                                            , ctx.GetParam("IMPORTO_DIFFIMPOSTA", oListArticoli(x).ImpDiffImposta) _
                                            , ctx.GetParam("IMPORTO_SANZIONI", oListArticoli(x).ImpSanzioni) _
                                            , ctx.GetParam("IMPORTO_SANZIONI_RIDOTTO", oListArticoli(x).ImpSanzioniRidotto) _
                                            , ctx.GetParam("IMPORTO_INTERESSI", oListArticoli(x).ImpInteressi) _
                                            , ctx.GetParam("ID_PROVVEDIMENTO", IdProvvedimento) _
                                            , ctx.GetParam("ID_LEGAME", oListArticoli(x).IdLegame))
                                End If
                            Catch ex As Exception
                                Log.Debug("OSAP_SetDichiaratoListArticoli::si è verificato il seguenet errore::" & ex.Message)
                                Return Nothing
                            Finally
                                ctx.Dispose()
                            End Try
                            For Each myRow As DataRowView In myDataView
                                myIdentity = myRow(0)
                            Next
                            For y = 0 To oListArticoli(x).ListAgevolazioni.GetUpperBound(0)
                                If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
                                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_SetDichiaratoVSAgevolazione", "IDARTICOLO", "IDAGEVOLAZIONE")
                                Else
                                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_SetAccertatoVSAgevolazione", "IDARTICOLO", "IDAGEVOLAZIONE")
                                End If
                                ctx.ExecuteNonQuery(sSQL, "TBL", ctx.GetParam("IDARTICOLO", myIdentity) _
                                            , ctx.GetParam("IDAGEVOLAZIONE", oListArticoli(x).ListAgevolazioni(y).IdAgevolazione)
                                        )
                            Next
                        End Using
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log.Debug("OSAP_SetDichiaratoListArticoli::si è verificato il seguenet errore::" & ex.Message)
                Return 0
            End Try
        End Function
        'Private Function OSAP_SetDichiaratoListArticoli(ByVal myConnectionString As String, ByVal Ambito As Integer, ByVal oAtto As ComPlusInterface.OggettoAttoOSAP, ByVal oListArticoli() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal IdProvvedimento As Long) As Integer
        '    Dim MyCommand As New SqlClient.SqlCommand
        '    Dim x, y As Integer
        '    Dim intRetVal As Integer
        '    Dim myIdentity As Long


        '    objUtility = New MotoreProvUtility

        '    Try
        '        MyCommand.Connection = New SqlConnection(myConnectionString)
        '        MyCommand.CommandType = CommandType.StoredProcedure
        '        For x = 0 To oListArticoli.Length - 1
        '            If Not IsNothing(oListArticoli(x).Calcolo) Then
        '                intRetVal = 0
        '                MyCommand.Parameters.Clear()
        '                If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
        '                    MyCommand.CommandText = "prc_SetOSAPDichiarato"
        '                Else
        '                    MyCommand.CommandText = "prc_SetOSAPAccertato"
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_DIFFIMPOSTA", oListArticoli(x).ImpDiffImposta)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI", oListArticoli(x).ImpSanzioni)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_RIDOTTO", oListArticoli(x).ImpSanzioniRidotto)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_INTERESSI", oListArticoli(x).ImpInteressi)
        '                End If
        '                MyCommand.Parameters.AddWithValue("@IDPOSIZIONE", -1)
        '                MyCommand.Parameters.AddWithValue("@IDENTE", oAtto.COD_ENTE)
        '                MyCommand.Parameters.AddWithValue("@ANNO", oAtto.ANNO)
        '                MyCommand.Parameters.AddWithValue("@IDCONTRIBUENTE", oAtto.COD_CONTRIBUENTE)
        '                MyCommand.Parameters.AddWithValue("@IDDICHIARAZIONE", oListArticoli(x).IdDichiarazione)
        '                MyCommand.Parameters.AddWithValue("@IDARTICOLO", oListArticoli(x).IdArticolo)
        '                'MyCommand.Parameters.AddWithValue("@IDARTICOLOPADRE", oListArticoli(x).IdArticoloPadre)
        '                MyCommand.Parameters.AddWithValue("@IDTRIBUTO", oListArticoli(x).IdTributo)
        '                MyCommand.Parameters.AddWithValue("@CODVIA", oListArticoli(x).CodVia)
        '                MyCommand.Parameters.AddWithValue("@CIVICO", oListArticoli(x).Civico)
        '                MyCommand.Parameters.AddWithValue("@ESPONENTE", oListArticoli(x).Esponente)
        '                MyCommand.Parameters.AddWithValue("@INTERNO", oListArticoli(x).Interno)
        '                MyCommand.Parameters.AddWithValue("@SCALA", oListArticoli(x).Scala)
        '                MyCommand.Parameters.AddWithValue("@IDCATEGORIA", oListArticoli(x).Categoria.IdCategoria)
        '                MyCommand.Parameters.AddWithValue("@IDTIPOLOGIAOCCUPAZIONE", oListArticoli(x).TipologiaOccupazione.IdTipologiaOccupazione)
        '                MyCommand.Parameters.AddWithValue("@CONSISTENZA", oListArticoli(x).Consistenza)
        '                MyCommand.Parameters.AddWithValue("@IDTIPOCONSISTENZA", oListArticoli(x).TipoConsistenzaTOCO.IdTipoConsistenza)
        '                MyCommand.Parameters.AddWithValue("@DATAINIZIOOCCUPAZIONE", oListArticoli(x).DataInizioOccupazione)
        '                MyCommand.Parameters.AddWithValue("@DATAFINEOCCUPAZIONE", oListArticoli(x).DataFineOccupazione)
        '                MyCommand.Parameters.AddWithValue("@IDDURATA", oListArticoli(x).TipoDurata.IdDurata)
        '                MyCommand.Parameters.AddWithValue("@DURATAOCCUPAZIONE", oListArticoli(x).DurataOccupazione)
        '                MyCommand.Parameters.AddWithValue("@MAGGIORAZIONE_IMPORTO", oListArticoli(x).MaggiorazioneImporto)
        '                MyCommand.Parameters.AddWithValue("@MAGGIORAZIONE_PERC", oListArticoli(x).MaggiorazionePerc)
        '                MyCommand.Parameters.AddWithValue("@NOTE", oListArticoli(x).Note)
        '                MyCommand.Parameters.AddWithValue("@DETRAZIONE_IMPORTO", oListArticoli(x).DetrazioneImporto)
        '                MyCommand.Parameters.AddWithValue("@ATTRAZIONE", oListArticoli(x).Attrazione)
        '                MyCommand.Parameters.AddWithValue("@OPERATORE", oListArticoli(x).Operatore)
        '                MyCommand.Parameters.AddWithValue("@DATA_INSERIMENTO", oListArticoli(x).DataInserimento)
        '                MyCommand.Parameters.AddWithValue("@TARIFFA_APPLICATA", oListArticoli(x).Calcolo.TariffaApplicata)
        '                MyCommand.Parameters.AddWithValue("@IMPORTO_LORDO", oListArticoli(x).Calcolo.ImportoLordo)
        '                MyCommand.Parameters.AddWithValue("@IMPORTO", oListArticoli(x).Calcolo.ImportoCalcolato)
        '                MyCommand.Parameters.AddWithValue("@ID_PROVVEDIMENTO", IdProvvedimento)
        '                MyCommand.Parameters.AddWithValue("@ID_LEGAME", oListArticoli(x).IdLegame)
        '                Dim sValParametri As String = Utility.Costanti.GetValParamCmd(MyCommand)
        '                Log.Debug("osapsetdicart->" + Costanti.LogQuery(MyCommand))
        '                'eseguo la query
        '                Dim DrReturn As SqlClient.SqlDataReader
        '                DrReturn = MyDBManager.GetPrivateDataReaderCOMplus(MyCommand)
        '                Do While DrReturn.Read
        '                    myIdentity = DrReturn(0)
        '                Loop
        '                DrReturn.Close()

        '                For y = 0 To oListArticoli(x).ListAgevolazioni.GetUpperBound(0)
        '                    If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
        '                        MyCommand.CommandText = "prc_SetDichiaratoVSAgevolazione"
        '                    Else
        '                        MyCommand.CommandText = "prc_SetAccertatoVSAgevolazione"
        '                    End If
        '                    MyCommand.Parameters.Clear()
        '                    MyCommand.Parameters.AddWithValue("@IDARTICOLO", myIdentity)
        '                    MyCommand.Parameters.AddWithValue("@IDAGEVOLAZIONE", oListArticoli(x).ListAgevolazioni(y).IdAgevolazione)
        '                    'eseguo la query
        '                    Log.Debug("osapsetdicartagev->" + Costanti.LogQuery(MyCommand))
        '                    If MyDBManager.Execute(MyCommand) <> 1 Then
        '                        Log.Debug("OSAP_SetDichiaratoListArticoli::errore in inserimento agevolazioni::procedure::" & MyCommand.CommandText & "::@IDARTICOLO" & myIdentity & "::@IDAGEVOLAZIONE::" & oListArticoli(x).ListAgevolazioni(y).IdAgevolazione)
        '                    End If
        '                Next
        '            End If
        '        Next
        '        Return 1
        '    Catch ex As Exception
        '        Log.Debug("OSAP_SetDichiaratoListArticoli::si è verificato il seguenet errore::" & ex.Message)
        '        Return 0
        '    End Try
        'End Function
        'Private Function OSAP_SetDichiaratoListArticoli(ByVal MyDBManager As DBManager, ByVal Ambito As Integer, ByVal oAtto As ComPlusInterface.OggettoAttoOSAP, ByVal oListArticoli() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal IdProvvedimento As Long) As Integer
        '    Dim MyCommand As New SqlClient.SqlCommand
        '    Dim x, y As Integer
        '    Dim intRetVal As Integer
        '    Dim myIdentity As Long


        '    objUtility = New MotoreProvUtility

        '    Try
        '        MyCommand.Connection = New SqlConnection(objDBManager.GetConnection.ConnectionString)
        '        MyCommand.CommandType = CommandType.StoredProcedure
        '        For x = 0 To oListArticoli.Length - 1
        '            If Not IsNothing(oListArticoli(x).Calcolo) Then
        '                intRetVal = 0
        '                MyCommand.Parameters.Clear()
        '                If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
        '                    MyCommand.CommandText = "prc_SetOSAPDichiarato"
        '                Else
        '                    MyCommand.CommandText = "prc_SetOSAPAccertato"
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_DIFFIMPOSTA", oListArticoli(x).ImpDiffImposta)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI", oListArticoli(x).ImpSanzioni)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_SANZIONI_RIDOTTO", oListArticoli(x).ImpSanzioniRidotto)
        '                    MyCommand.Parameters.AddWithValue("@IMPORTO_INTERESSI", oListArticoli(x).ImpInteressi)
        '                End If
        '                MyCommand.Parameters.AddWithValue("@IDPOSIZIONE", -1)
        '                MyCommand.Parameters.AddWithValue("@IDENTE", oAtto.COD_ENTE)
        '                MyCommand.Parameters.AddWithValue("@ANNO", oAtto.ANNO)
        '                MyCommand.Parameters.AddWithValue("@IDCONTRIBUENTE", oAtto.COD_CONTRIBUENTE)
        '                MyCommand.Parameters.AddWithValue("@IDDICHIARAZIONE", oListArticoli(x).IdDichiarazione)
        '                MyCommand.Parameters.AddWithValue("@IDARTICOLO", oListArticoli(x).IdArticolo)
        '                'MyCommand.Parameters.AddWithValue("@IDARTICOLOPADRE", oListArticoli(x).IdArticoloPadre)
        '                MyCommand.Parameters.AddWithValue("@IDTRIBUTO", oListArticoli(x).IdTributo)
        '                MyCommand.Parameters.AddWithValue("@CODVIA", oListArticoli(x).CodVia)
        '                MyCommand.Parameters.AddWithValue("@CIVICO", oListArticoli(x).Civico)
        '                MyCommand.Parameters.AddWithValue("@ESPONENTE", oListArticoli(x).Esponente)
        '                MyCommand.Parameters.AddWithValue("@INTERNO", oListArticoli(x).Interno)
        '                MyCommand.Parameters.AddWithValue("@SCALA", oListArticoli(x).Scala)
        '                MyCommand.Parameters.AddWithValue("@IDCATEGORIA", oListArticoli(x).Categoria.IdCategoria)
        '                MyCommand.Parameters.AddWithValue("@IDTIPOLOGIAOCCUPAZIONE", oListArticoli(x).TipologiaOccupazione.IdTipologiaOccupazione)
        '                MyCommand.Parameters.AddWithValue("@CONSISTENZA", oListArticoli(x).Consistenza)
        '                MyCommand.Parameters.AddWithValue("@IDTIPOCONSISTENZA", oListArticoli(x).TipoConsistenzaTOCO.IdTipoConsistenza)
        '                MyCommand.Parameters.AddWithValue("@DATAINIZIOOCCUPAZIONE", oListArticoli(x).DataInizioOccupazione)
        '                MyCommand.Parameters.AddWithValue("@DATAFINEOCCUPAZIONE", oListArticoli(x).DataFineOccupazione)
        '                MyCommand.Parameters.AddWithValue("@IDDURATA", oListArticoli(x).TipoDurata.IdDurata)
        '                MyCommand.Parameters.AddWithValue("@DURATAOCCUPAZIONE", oListArticoli(x).DurataOccupazione)
        '                MyCommand.Parameters.AddWithValue("@MAGGIORAZIONE_IMPORTO", oListArticoli(x).MaggiorazioneImporto)
        '                MyCommand.Parameters.AddWithValue("@MAGGIORAZIONE_PERC", oListArticoli(x).MaggiorazionePerc)
        '                MyCommand.Parameters.AddWithValue("@NOTE", oListArticoli(x).Note)
        '                MyCommand.Parameters.AddWithValue("@DETRAZIONE_IMPORTO", oListArticoli(x).DetrazioneImporto)
        '                MyCommand.Parameters.AddWithValue("@ATTRAZIONE", oListArticoli(x).Attrazione)
        '                MyCommand.Parameters.AddWithValue("@OPERATORE", oListArticoli(x).Operatore)
        '                MyCommand.Parameters.AddWithValue("@DATA_INSERIMENTO", oListArticoli(x).DataInserimento)
        '                MyCommand.Parameters.AddWithValue("@TARIFFA_APPLICATA", oListArticoli(x).Calcolo.TariffaApplicata)
        '                MyCommand.Parameters.AddWithValue("@IMPORTO_LORDO", oListArticoli(x).Calcolo.ImportoLordo)
        '                MyCommand.Parameters.AddWithValue("@IMPORTO", oListArticoli(x).Calcolo.ImportoCalcolato)
        '                MyCommand.Parameters.AddWithValue("@ID_PROVVEDIMENTO", IdProvvedimento)
        '                MyCommand.Parameters.AddWithValue("@ID_LEGAME", oListArticoli(x).IdLegame)
        '                Dim sValParametri As String = Utility.Costanti.GetValParamCmd(MyCommand)
        '                Log.Debug("osapsetdicart->" + Costanti.LogQuery(MyCommand))
        '                'eseguo la query
        '                Dim DrReturn As SqlClient.SqlDataReader
        '                DrReturn = MyDBManager.GetPrivateDataReaderCOMplus(MyCommand)
        '                Do While DrReturn.Read
        '                    myIdentity = DrReturn(0)
        '                Loop
        '                DrReturn.Close()

        '                For y = 0 To oListArticoli(x).ListAgevolazioni.GetUpperBound(0)
        '                    If Ambito = COSTANTValue.CostantiProv.AMBITO_DICHIARATO Then
        '                        MyCommand.CommandText = "prc_SetDichiaratoVSAgevolazione"
        '                    Else
        '                        MyCommand.CommandText = "prc_SetAccertatoVSAgevolazione"
        '                    End If
        '                    MyCommand.Parameters.Clear()
        '                    MyCommand.Parameters.AddWithValue("@IDARTICOLO", myIdentity)
        '                    MyCommand.Parameters.AddWithValue("@IDAGEVOLAZIONE", oListArticoli(x).ListAgevolazioni(y).IdAgevolazione)
        '                    'eseguo la query
        '                    Log.Debug("osapsetdicartagev->" + Costanti.LogQuery(MyCommand))
        '                    If MyDBManager.Execute(MyCommand) <> 1 Then
        '                        Log.Debug("OSAP_SetDichiaratoListArticoli::errore in inserimento agevolazioni::procedure::" & MyCommand.CommandText & "::@IDARTICOLO" & myIdentity & "::@IDAGEVOLAZIONE::" & oListArticoli(x).ListAgevolazioni(y).IdAgevolazione)
        '                    End If
        '                Next
        '            End If
        '        Next
        '        Return 1
        '    Catch ex As Exception
        '        Log.Debug("OSAP_SetDichiaratoListArticoli::si è verificato il seguenet errore::" & ex.Message)
        '        Return 0
        '    End Try
        'End Function
        '*** ***
#End Region
    End Class
End Namespace