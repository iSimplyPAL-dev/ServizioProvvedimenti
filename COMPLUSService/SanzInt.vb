Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports ComPlusInterface
Imports log4net

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per il calcolo delle sanzioni ed interessi
    ''' </summary>
    Public Class SanzInt
        Private Shared Log As ILog = LogManager.GetLogger(GetType(SanzInt))
        Dim myUtility As New MotoreProvUtility

        '****  -  ***'***  -  ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="oSituazioneBasePerSanzInt"></param>
        ''' <param name="objDSSituazioneBasePerSanzIntAppoggio"></param>
        ''' <param name="strCODTRIBUTO"></param>
        ''' <param name="strCODCAPITOLO"></param>
        ''' <param name="strCODVOCE"></param>
        ''' <param name="strCODENTE"></param>
        ''' <param name="strCODTIPOPROVVEDIMENTO"></param>
        ''' <param name="lngGenericID"></param>
        ''' <param name="objHashTable"></param>
        ''' <param name="sDataMorte"></param>
        ''' <param name="bConsentiSanzNeg"></param>
        ''' <param name="idLegame"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="01/07/2014">IMU/TARES</revision></revisionHistory>
        ''' <revisionHistory><revision date="09/2018">Cartelle Insoluti</revision></revisionHistory>
        ''' <revisionHistory><revision date="10/09/2019">passo direttamente la data di morte che è l'unico campo fisso che serve dall'anagrafica</revision></revisionHistory>
        Public Function getCalcoloSanzioniICI(StringConnectionProvv As String, ByRef oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal objDSSituazioneBasePerSanzIntAppoggio As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODCAPITOLO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, ByVal objHashTable As Hashtable, ByVal sDataMorte As String, ByVal bConsentiSanzNeg As Boolean, ByVal idLegame As Integer) As DataSet
            'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
            Try
                Dim culture As IFormatProvider
                culture = New CultureInfo("it-IT", True)
                System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")
                Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
                Dim objDSGetDatiSanzioni As DataSet
                Dim objDSGetCalcoloSanzioni As New DataSet
                Dim intCount, intCountSanzioni, intCountCondizioneSanzioni, intCountSanzSplit As Integer
                Dim TipoMisura As String
                Dim blnRiducibile, blnCumulabile As Boolean
                Dim dblValore, dblMinimo, dblImportoSanzione, dblImportoPieno, dblImportoRidotto, dblImportoRiducibile, dblImportoNonRiducibile As Double
                Dim dblIMPORTO As Double                'importo scaturito dal confronto tra versato e dichiarato
                Dim TotaleSanzioni As Double
                Dim TotaleSanzioniRidotto As Double

                Dim StrCondizione, strParametro, strBaseRaffronto, strCalcolataSu, strCOD_VOCE, strCOD_VOCEold As String
                Dim StrCondizione_intr, strParametro_intr, strBaseRaffronto_intr As String
                Dim strBaseRaffrontoAPPOGGIO, strCalcolataSuAPPOGGIO As String
                Dim strBaseRaffrontoAPPOGGIO_intr As String
                Dim objDRSituazioneBasePerSanzIntAppoggio() As DataRow
                Dim nQuotaRiduzione As Integer = 4 'valore fisso che aveva prima dell'IMU
                Dim impCalcolataSuAppoggio As Double
                'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI

                Dim newTable As DataTable
                newTable = New DataTable("SANZIONI")

                Dim NewColumn As New DataColumn
                NewColumn.ColumnName = "COD_ENTE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                Dim NewColumn1 As New DataColumn
                NewColumn1.ColumnName = "ANNO"
                NewColumn1.DataType = System.Type.GetType("System.String")
                NewColumn1.DefaultValue = "0"
                newTable.Columns.Add(NewColumn1)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "COD_VOCE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO_RIDOTTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ACCONTO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "SALDO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "SALDO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "DATA_INIZIO"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "DATA_FINE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_SEMESTRI_SALDO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "TASSO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "GENERIC_ID"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ID_LEGAME"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "MOTIVAZIONI"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = ""
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_GIORNI_ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_GIORNI_SALDO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)
                '*** 20140701 - IMU/TARES ***
                NewColumn = New DataColumn
                NewColumn.ColumnName = "QUOTARIDUZIONESANZIONI"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 1
                newTable.Columns.Add(NewColumn)
                '*** ***
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

                Dim MyVsaEngine As Microsoft.JScript.Vsa.VsaEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine
                Dim objExecFormula, objExecFormula_intr As Object
                '*** 20130801 - accertamento OSAP ***
                If Not IsNothing(objHashTable("ID_FASE")) Then
                    lngGenericID = objHashTable("ID_FASE")
                End If
                '*** ***
                Log.Debug("getCalcoloSanzioni::devo ciclare su objSituazioneBasePerSanzInt")
                Dim MinSanzione As Double = 0
                Dim MinImportoRidotto As Double = 0
                Dim arrSanz As Array
                Dim arrSanzvalue As Array
                Dim Row1 As DataRow
                objDRSituazioneBasePerSanzIntAppoggio = objDSSituazioneBasePerSanzIntAppoggio.Tables(0).Select("ANNO='" & oSituazioneBasePerSanzInt.Anno & "'")
                'ciclo sul dataset appoggio
                For intCountCondizioneSanzioni = 0 To objDRSituazioneBasePerSanzIntAppoggio.Length - 1
                    If strCODVOCE = "-1" Then
                        strCODVOCE = strCODVOCE & "#" & strCODTIPOPROVVEDIMENTO
                    End If
                    Log.Debug("getCalcoloSanzioni::COD_VOCE::" & strCODVOCE)
                    'Split strCODVOCE con "," -> cod_voce@cod_provv
                    If strCODVOCE <> "" Then
                        strCOD_VOCEold = "-1"
                        arrSanz = Split(strCODVOCE, ",")
                        For intCountSanzSplit = 0 To arrSanz.Length - 1
                            Try
                                arrSanzvalue = Split(arrSanz(intCountSanzSplit), "#")
                                strCODVOCE = arrSanzvalue(0)
                                strCODTIPOPROVVEDIMENTO = arrSanzvalue(1)

                                objDSGetDatiSanzioni = objDBOPENgovProvvedimentiSelect.GetSanzioni(dblIMPORTO, oSituazioneBasePerSanzInt.Anno, strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODENTE, strCODTIPOPROVVEDIMENTO, lngGenericID, objHashTable("COD_TIPO_PROCEDIMENTO"), StringConnectionProvv)
                                'Ciclo sulle Sanzioni
                                Log.Debug("ho trovato " & objDSGetDatiSanzioni.Tables(0).Rows.Count.ToString() & " sanzioni")
                                For intCountSanzioni = 0 To objDSGetDatiSanzioni.Tables(0).Rows.Count - 1
                                    strCOD_VOCE = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("COD_VOCE"), String)
                                    Log.Debug("strCOD_VOCE=" & strCOD_VOCE)
                                    '*** 20140701 - IMU/TARES ***
                                    If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI")) Then
                                        nQuotaRiduzione = CInt(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI"))
                                    End If
                                    '*** ***
                                    Log.Debug("nQuotaRiduzione=" & nQuotaRiduzione.ToString)
                                    If strCOD_VOCEold <> strCOD_VOCE And strCOD_VOCEold <> "-1" Then
                                        TotaleSanzioni = TotaleSanzioni + MinSanzione                                    'MinImportoRidotto
                                        TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto

                                        Row1 = newTable.NewRow()
                                        Row1.Item("COD_ENTE") = strCODENTE
                                        Row1.Item("ANNO") = oSituazioneBasePerSanzInt.Anno
                                        Row1.Item("COD_VOCE") = strCOD_VOCEold
                                        'giulia 17082005
                                        Row1.Item("IMPORTO") = MinSanzione
                                        Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
                                        Row1.Item("IMPORTO_GIORNI") = 0
                                        Row1.Item("ACCONTO") = 0
                                        Row1.Item("SALDO") = 0
                                        Row1.Item("ACCONTO_GIORNI") = 0
                                        Row1.Item("SALDO_GIORNI") = 0
                                        Row1.Item("DATA_INIZIO") = ""
                                        Row1.Item("DATA_FINE") = ""
                                        Row1.Item("N_SEMESTRI_ACCONTO") = 0
                                        Row1.Item("N_SEMESTRI_SALDO") = 0
                                        Row1.Item("N_GIORNI_ACCONTO") = 0
                                        Row1.Item("N_GIORNI_SALDO") = 0
                                        Row1.Item("TASSO") = 0
                                        Row1.Item("GENERIC_ID") = lngGenericID
                                        Row1.Item("ID_LEGAME") = idLegame
                                        Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
                                        '*** 20140701 - IMU/TARES ***
                                        Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
                                        '*** ***
                                        newTable.Rows.Add(Row1)
                                        MinSanzione = 0
                                        MinImportoRidotto = 0
                                    End If

                                    dblImportoPieno = 0
                                    dblImportoRidotto = 0
                                    dblImportoRiducibile = 0
                                    dblImportoNonRiducibile = 0
                                    TipoMisura = ""
                                    blnRiducibile = False
                                    blnCumulabile = False
                                    dblValore = 0
                                    dblMinimo = 0

                                    StrCondizione = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE"), String)
                                    Log.Debug("StrCondizione=" & StrCondizione)
                                    strParametro = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO"), String)
                                    Log.Debug("strParametro=" & strParametro)
                                    If strParametro = "=" Then
                                        strParametro = "=="
                                    End If
                                    If strParametro = "<>" Then
                                        strParametro = "!="
                                    End If
                                    strBaseRaffronto = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO"), String)
                                    Log.Debug("strBaseRaffronto=" & strBaseRaffronto)
                                    strCalcolataSu = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("DESC_BASE_CALCOLO"), String)
                                    Log.Debug("strCalcolataSu=" & strCalcolataSu)
                                    'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
                                    If strBaseRaffronto <> "-1" Then
                                        If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)) Then
                                            strBaseRaffrontoAPPOGGIO = 0
                                        Else
                                            strBaseRaffrontoAPPOGGIO = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)).ToString()
                                        End If
                                    Else
                                        strBaseRaffrontoAPPOGGIO = 0
                                    End If
                                    Log.Debug("strBaseRaffrontoAPPOGGIO=" & strBaseRaffrontoAPPOGGIO)
                                    'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
                                    If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu)) Then
                                        strCalcolataSuAPPOGGIO = 0
                                    Else
                                        strCalcolataSuAPPOGGIO = CType(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu), String)
                                        Try
                                            impCalcolataSuAppoggio = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu))
                                            Log.Debug("impCalcolataSuAppoggio::" & impCalcolataSuAppoggio.ToString)
                                        Catch ex As Exception
                                            Log.Debug("errore conversione impCalcolataSuAppoggio:: da " & objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu).ToString)
                                        End Try
                                    End If
                                    Log.Debug("strCalcolataSuAPPOGGIO=" & strCalcolataSuAPPOGGIO)
                                    'recupero valori per instrasmissibilità
                                    Try
                                        If (IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
                                            objExecFormula_intr = False
                                        Else
                                            If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"))) Then
                                                StrCondizione_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"), String)
                                                If StrCondizione_intr = "" Or StrCondizione_intr = "NULL" Then
                                                    StrCondizione_intr = "''"
                                                End If
                                            Else
                                                StrCondizione_intr = "''"
                                            End If
                                            If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"))) Then
                                                If CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String) <> "-1" Then
                                                    strParametro_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String)
                                                    If strParametro_intr = "=" Then
                                                        strParametro_intr = "=="
                                                    End If
                                                    If strParametro_intr = "<>" Then
                                                        strParametro_intr = "!="
                                                    End If
                                                Else
                                                    strParametro_intr = "!="
                                                End If
                                            Else
                                                strParametro_intr = "!="
                                            End If
                                            If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
                                                strBaseRaffronto_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"), String)
                                                If strBaseRaffronto_intr <> "-1" Then
                                                    If Utility.StringOperation.FormatDateTime(sDataMorte).ToShortDateString <> DateTime.MaxValue.ToShortDateString Then
                                                        strBaseRaffrontoAPPOGGIO_intr = sDataMorte
                                                    Else
                                                        Log.Debug("datamorte='' quindi non calcolo")
                                                        objExecFormula_intr = False
                                                        strBaseRaffrontoAPPOGGIO_intr = "''"
                                                    End If
                                                Else
                                                    strBaseRaffrontoAPPOGGIO_intr = "''"
                                                End If
                                            Else
                                                strBaseRaffrontoAPPOGGIO_intr = "''"
                                            End If
                                            objExecFormula_intr = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr, MyVsaEngine)
                                            Log.Debug("devo eseguire intrasmissibilita?" & strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr)
                                        End If
                                    Catch exIntr As Exception
                                        Log.Debug("SanzInt.getCalcoloSanzioni.intrasmissibilita.errore::", exIntr)
                                    End Try
                                    Log.Debug("strBaseRaffrontoAPPOGGIO_intr=" & strBaseRaffrontoAPPOGGIO_intr)
                                    objExecFormula = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione, MyVsaEngine)
                                    Log.Debug("devo eseguire formula?" & strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione)

                                    If CType(objExecFormula_intr, Boolean) = True Then
                                        Log.Debug("sono morto e la sanzione è intrasmissibile quindi non calcolo")
                                        'Se la condizione di Intrasmissibilità è verificata le sanzioni vanno a 0
                                        dblImportoPieno = 0
                                        dblImportoRidotto = 0
                                        dblImportoRiducibile = 0
                                        dblImportoNonRiducibile = 0
                                        'forzo il valore della voce come instramissibilità agli eredi
                                        'data_morte<>''
                                        strCOD_VOCE = "97"
                                    Else
                                        If CType(objExecFormula, Boolean) = True Then
                                            Log.Debug("calcolo sanzione")
                                            If Not IsDBNull(strCalcolataSuAPPOGGIO) Then
                                                Log.Debug("strCalcolataSuAPPOGGIO::" & strCalcolataSuAPPOGGIO)
                                                'per CMGC NO REPLACE
                                                dblIMPORTO = impCalcolataSuAppoggio 'CDbl(strCalcolataSuAPPOGGIO.Replace(".", ","))
                                                If dblIMPORTO >= 0 Or bConsentiSanzNeg Then
                                                    TipoMisura = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MISURA"), String)

                                                    If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE")) Then
                                                        blnRiducibile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE"), String)
                                                    Else
                                                        blnRiducibile = False
                                                    End If

                                                    If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE")) Then
                                                        blnCumulabile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE"), String)
                                                    Else
                                                        blnCumulabile = False
                                                    End If

                                                    If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE")).CompareTo("") = 0 Then
                                                        dblValore = 0 'SanzInt.getCalcoloInteressi.errore
                                                    Else
                                                        dblValore = CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE"), ".", ","), Double)
                                                    End If

                                                    If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO")).CompareTo("") = 0 Then
                                                        dblMinimo = 0
                                                    Else
                                                        dblMinimo = FormatNumber(CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO"), ".", ","), Double), 2)
                                                    End If

                                                    If UCase(TipoMisura) = "F" Then
                                                        dblImportoSanzione = dblValore                                                  'dblMinimo
                                                    ElseIf UCase(TipoMisura) = "P" Then
                                                        Log.Debug("dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)::" & dblIMPORTO.ToString & "::" & dblValore.ToString)
                                                        dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)
                                                        Log.Debug("dblImportoSanzione::" & dblImportoSanzione.ToString)
                                                        Log.Debug("dblMinimo::" & dblMinimo.ToString)
                                                        'GIULIA 08082005 aggiunto controllo
                                                        If bConsentiSanzNeg = False And dblImportoSanzione < dblMinimo Then
                                                            dblImportoSanzione = dblMinimo
                                                        End If
                                                    End If
                                                    dblImportoPieno = FormatNumber(dblImportoSanzione, 2)

                                                    If blnRiducibile = True Then
                                                        '*** 20140701 - IMU/TARES ***
                                                        dblImportoRidotto = FormatNumber((dblImportoSanzione / nQuotaRiduzione), 2)
                                                        '*** ***
                                                        dblImportoRiducibile = dblImportoPieno
                                                        dblImportoNonRiducibile = 0
                                                    Else
                                                        'giulia 17082005
                                                        dblImportoRidotto = 0
                                                        dblImportoRiducibile = 0
                                                        dblImportoNonRiducibile = 0
                                                    End If
                                                Else
                                                    dblImportoPieno = 0
                                                    dblImportoRidotto = 0
                                                    dblImportoRiducibile = 0
                                                    dblImportoNonRiducibile = 0
                                                End If
                                            Else
                                                Log.Debug("strCalcolataSuAPPOGGIO null quindi non calcolo")
                                                dblImportoPieno = 0
                                                dblImportoRidotto = 0
                                                dblImportoRiducibile = 0
                                                dblImportoNonRiducibile = 0
                                            End If
                                        Else
                                            Log.Debug("non eseguo formula")
                                        End If
                                    End If

                                    'giulia 17082005
                                    'a parità di codice sanzione se sono vere più condizioni, al contribuente viene applicata la sanzione minore
                                    If MinImportoRidotto > dblImportoRidotto Or MinImportoRidotto = 0 Then
                                        MinSanzione = dblImportoPieno
                                        MinImportoRidotto = dblImportoRidotto
                                    End If
                                    strCOD_VOCEold = strCOD_VOCE
                                    Log.Debug("getCalcoloSanzioni::passo a riga successiva")
                                Next
                            Catch ErrSanz As Exception
                                Log.Debug("getCalcoloSanzioni::arrsanz vuoto")
                            End Try
                        Next
                        '*********** fine ciclo nuovo

                        'giulia 17082005
                        TotaleSanzioni = TotaleSanzioni + MinSanzione                        'MinImportoRidotto
                        TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto
                        If objDSGetDatiSanzioni.Tables(0).Rows.Count > 0 Then
                            Row1 = newTable.NewRow()
                            Row1.Item("COD_ENTE") = strCODENTE
                            Row1.Item("ANNO") = oSituazioneBasePerSanzInt.Anno
                            Row1.Item("COD_VOCE") = strCOD_VOCE
                            'giulia 17082005
                            Row1.Item("IMPORTO") = MinSanzione
                            Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
                            Row1.Item("IMPORTO_GIORNI") = dblMinimo 'usato come appoggio per restituire importo minimo 0
                            Row1.Item("ACCONTO") = 0
                            Row1.Item("SALDO") = 0
                            Row1.Item("ACCONTO_GIORNI") = 0
                            Row1.Item("SALDO_GIORNI") = 0
                            Row1.Item("DATA_INIZIO") = ""
                            Row1.Item("DATA_FINE") = ""
                            Row1.Item("N_SEMESTRI_ACCONTO") = 0
                            Row1.Item("N_SEMESTRI_SALDO") = 0
                            Row1.Item("N_GIORNI_ACCONTO") = 0
                            Row1.Item("N_GIORNI_SALDO") = 0
                            Row1.Item("TASSO") = 0
                            Row1.Item("GENERIC_ID") = lngGenericID
                            Row1.Item("ID_LEGAME") = idLegame
                            Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
                            '*** 20140701 - IMU/TARES ***
                            Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
                            '*** ***
                            newTable.Rows.Add(Row1)
                        End If
                    End If
                Next
                'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
                oSituazioneBasePerSanzInt.Sanzioni = TotaleSanzioni
                oSituazioneBasePerSanzInt.SanzioniRidotto = TotaleSanzioniRidotto
                '*** 20140701 - IMU/TARES ***
                If TotaleSanzioni > 0 Then
                    oSituazioneBasePerSanzInt.QuotaRiduzione = nQuotaRiduzione
                    Log.Debug("ho calcolato TotaleSanzioni::" & TotaleSanzioni.ToString)
                    Log.Debug("ho calcolato TotaleSanzioniRidotto::" & TotaleSanzioniRidotto.ToString)
                    Log.Debug("nQuotaRiduzione::" & nQuotaRiduzione.ToString)
                End If
                '*** ***
                objDSGetCalcoloSanzioni.Tables.Add(newTable)
                Return objDSGetCalcoloSanzioni
            Catch ex As Exception
                Log.Debug("Function::getCalcoloSanzioni::COMPlusService::si è verificato il seguente errore::", ex)
                Return Nothing
            End Try
        End Function
        'Public Function getCalcoloSanzioniICI(ByRef objSituazioneBasePerSanzInt As DataSet, ByVal objDSSituazioneBasePerSanzIntAppoggio As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODCAPITOLO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, ByVal objHashTable As Hashtable, ByVal sDataMorte As String, ByVal bConsentiSanzNeg As Boolean, ByVal idLegame As Integer) As DataSet
        '    'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
        '    Try
        '        Dim culture As IFormatProvider
        '        culture = New CultureInfo("it-IT", True)
        '        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")
        '        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect          
        '        Dim objDSGetDatiSanzioni As DataSet
        '        Dim objDSGetCalcoloSanzioni As New DataSet
        '        Dim intCount, intCountSanzioni, intCountCondizioneSanzioni, intCountSanzSplit As Integer
        '        Dim TipoMisura As String
        '        Dim blnRiducibile, blnCumulabile As Boolean
        '        Dim dblValore, dblMinimo, dblImportoSanzione, dblImportoPieno, dblImportoRidotto, dblImportoRiducibile, dblImportoNonRiducibile As Double
        '        Dim strANNO As String               'anno di liquidazione (Pre Accertamento)
        '        Dim dblIMPORTO As Double                'importo scaturito dal confronto tra versato e dichiarato
        '        Dim TotaleSanzioni As Double
        '        Dim TotaleSanzioniRidotto As Double

        '        Dim StrCondizione, strParametro, strBaseRaffronto, strCalcolataSu, strCOD_VOCE, strCOD_VOCEold As String
        '        Dim StrCondizione_intr, strParametro_intr, strBaseRaffronto_intr As String
        '        Dim strBaseRaffrontoAPPOGGIO, strCalcolataSuAPPOGGIO As String
        '        Dim strBaseRaffrontoAPPOGGIO_intr As String
        '        Dim objDRSituazioneBasePerSanzIntAppoggio() As DataRow
        '        Dim nQuotaRiduzione As Integer = 4 'valore fisso che aveva prima dell'IMU
        '        Dim impCalcolataSuAppoggio As Double
        '        'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI

        '        Dim newTable As DataTable
        '        newTable = New DataTable("SANZIONI")

        '        Dim NewColumn As New DataColumn
        '        NewColumn.ColumnName = "COD_ENTE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        Dim NewColumn1 As New DataColumn
        '        NewColumn1.ColumnName = "ANNO"
        '        NewColumn1.DataType = System.Type.GetType("System.String")
        '        NewColumn1.DefaultValue = "0"
        '        newTable.Columns.Add(NewColumn1)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_VOCE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_RIDOTTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_INIZIO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_FINE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "TASSO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "GENERIC_ID"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ID_LEGAME"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "MOTIVAZIONI"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = ""
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)
        '        '*** 20140701 - IMU/TARES ***
        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "QUOTARIDUZIONESANZIONI"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 1
        '        newTable.Columns.Add(NewColumn)
        '        '*** ***
        '        objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

        '        Dim MyVsaEngine As Microsoft.JScript.Vsa.VsaEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine
        '        Dim objExecFormula, objExecFormula_intr As Object
        '        '*** 20130801 - accertamento OSAP ***
        '        If Not IsNothing(objHashTable("ID_FASE")) Then
        '            lngGenericID = objHashTable("ID_FASE")
        '        End If
        '        '*** ***
        '        Log.Debug("getCalcoloSanzioni::devo ciclare su objSituazioneBasePerSanzInt")
        '        For intCount = 0 To objSituazioneBasePerSanzInt.Tables(0).Rows.Count - 1
        '            Log.Debug("getCalcoloSanzioni:: giro " & intCount.ToString)
        '            strANNO = CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("ANNO"), Double)
        '            Dim MinSanzione As Double = 0
        '            Dim MinImportoRidotto As Double = 0
        '            Dim arrSanz As Array
        '            Dim arrSanzvalue As Array
        '            Dim Row1 As DataRow
        '            objDRSituazioneBasePerSanzIntAppoggio = objDSSituazioneBasePerSanzIntAppoggio.Tables(0).Select("ANNO='" & strANNO & "'")
        '            'ciclo sul dataset appoggio
        '            For intCountCondizioneSanzioni = 0 To objDRSituazioneBasePerSanzIntAppoggio.Length - 1
        '                If strCODVOCE = "-1" Then
        '                    strCODVOCE = strCODVOCE & "#" & strCODTIPOPROVVEDIMENTO
        '                End If
        '                Log.Debug("getCalcoloSanzioni::COD_VOCE::" & strCODVOCE)
        '                'Split strCODVOCE con "," -> cod_voce@cod_provv
        '                If strCODVOCE <> "" Then
        '                    strCOD_VOCEold = "-1"
        '                    arrSanz = Split(strCODVOCE, ",")
        '                    For intCountSanzSplit = 0 To arrSanz.Length - 1
        '                        Try
        '                            arrSanzvalue = Split(arrSanz(intCountSanzSplit), "#")
        '                            strCODVOCE = arrSanzvalue(0)
        '                            strCODTIPOPROVVEDIMENTO = arrSanzvalue(1)

        '                            objDSGetDatiSanzioni = objDBOPENgovProvvedimentiSelect.GetSanzioni(dblIMPORTO, strANNO, strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODENTE, strCODTIPOPROVVEDIMENTO, lngGenericID, objHashTable("COD_TIPO_PROCEDIMENTO"), StringConnectionProvv)
        '                            'Ciclo sulle Sanzioni
        '                            Log.Debug("ho trovato " & objDSGetDatiSanzioni.Tables(0).Rows.Count.ToString() & " sanzioni")
        '                            For intCountSanzioni = 0 To objDSGetDatiSanzioni.Tables(0).Rows.Count - 1
        '                                strCOD_VOCE = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("COD_VOCE"), String)
        '                                Log.Debug("strCOD_VOCE=" & strCOD_VOCE)
        '                                '*** 20140701 - IMU/TARES ***
        '                                If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI")) Then
        '                                    nQuotaRiduzione = CInt(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI"))
        '                                End If
        '                                '*** ***
        '                                Log.Debug("nQuotaRiduzione=" & nQuotaRiduzione.ToString)
        '                                If strCOD_VOCEold <> strCOD_VOCE And strCOD_VOCEold <> "-1" Then
        '                                    TotaleSanzioni = TotaleSanzioni + MinSanzione                                    'MinImportoRidotto
        '                                    TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto

        '                                    Row1 = newTable.NewRow()
        '                                    Row1.Item("COD_ENTE") = strCODENTE
        '                                    Row1.Item("ANNO") = strANNO
        '                                    Row1.Item("COD_VOCE") = strCOD_VOCEold
        '                                    'giulia 17082005
        '                                    Row1.Item("IMPORTO") = MinSanzione
        '                                    Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
        '                                    Row1.Item("IMPORTO_GIORNI") = 0
        '                                    Row1.Item("ACCONTO") = 0
        '                                    Row1.Item("SALDO") = 0
        '                                    Row1.Item("ACCONTO_GIORNI") = 0
        '                                    Row1.Item("SALDO_GIORNI") = 0
        '                                    Row1.Item("DATA_INIZIO") = ""
        '                                    Row1.Item("DATA_FINE") = ""
        '                                    Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                                    Row1.Item("N_SEMESTRI_SALDO") = 0
        '                                    Row1.Item("N_GIORNI_ACCONTO") = 0
        '                                    Row1.Item("N_GIORNI_SALDO") = 0
        '                                    Row1.Item("TASSO") = 0
        '                                    Row1.Item("GENERIC_ID") = lngGenericID
        '                                    Row1.Item("ID_LEGAME") = idLegame
        '                                    Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
        '                                    '*** 20140701 - IMU/TARES ***
        '                                    Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
        '                                    '*** ***
        '                                    newTable.Rows.Add(Row1)
        '                                    MinSanzione = 0
        '                                    MinImportoRidotto = 0
        '                                End If

        '                                dblImportoPieno = 0
        '                                dblImportoRidotto = 0
        '                                dblImportoRiducibile = 0
        '                                dblImportoNonRiducibile = 0
        '                                TipoMisura = ""
        '                                blnRiducibile = False
        '                                blnCumulabile = False
        '                                dblValore = 0
        '                                dblMinimo = 0

        '                                StrCondizione = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE"), String)
        '                                Log.Debug("StrCondizione=" & StrCondizione)
        '                                strParametro = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO"), String)
        '                                Log.Debug("strParametro=" & strParametro)
        '                                If strParametro = "=" Then
        '                                    strParametro = "=="
        '                                End If
        '                                If strParametro = "<>" Then
        '                                    strParametro = "!="
        '                                End If
        '                                strBaseRaffronto = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO"), String)
        '                                Log.Debug("strBaseRaffronto=" & strBaseRaffronto)
        '                                strCalcolataSu = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("DESC_BASE_CALCOLO"), String)
        '                                Log.Debug("strCalcolataSu=" & strCalcolataSu)
        '                                'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
        '                                If strBaseRaffronto <> "-1" Then
        '                                    If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)) Then
        '                                        strBaseRaffrontoAPPOGGIO = 0
        '                                    Else
        '                                        strBaseRaffrontoAPPOGGIO = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)).ToString()
        '                                    End If
        '                                Else
        '                                    strBaseRaffrontoAPPOGGIO = 0
        '                                End If
        '                                Log.Debug("strBaseRaffrontoAPPOGGIO=" & strBaseRaffrontoAPPOGGIO)
        '                                'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
        '                                If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu)) Then
        '                                    strCalcolataSuAPPOGGIO = 0
        '                                Else
        '                                    strCalcolataSuAPPOGGIO = CType(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu), String)
        '                                    Try
        '                                        impCalcolataSuAppoggio = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu))
        '                                        Log.Debug("impCalcolataSuAppoggio::" & impCalcolataSuAppoggio.ToString)
        '                                    Catch ex As Exception
        '                                        Log.Debug("errore conversione impCalcolataSuAppoggio:: da " & objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu).ToString)
        '                                    End Try
        '                                End If
        '                                Log.Debug("strCalcolataSuAPPOGGIO=" & strCalcolataSuAPPOGGIO)
        '                                'recupero valori per instrasmissibilità
        '                                Try
        '                                    If (IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
        '                                        objExecFormula_intr = False
        '                                    Else
        '                                        If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"))) Then
        '                                            StrCondizione_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"), String)
        '                                            If StrCondizione_intr = "" Or StrCondizione_intr = "NULL" Then
        '                                                StrCondizione_intr = "''"
        '                                            End If
        '                                        Else
        '                                            StrCondizione_intr = "''"
        '                                        End If
        '                                        If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"))) Then
        '                                            If CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String) <> "-1" Then
        '                                                strParametro_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String)
        '                                                If strParametro_intr = "=" Then
        '                                                    strParametro_intr = "=="
        '                                                End If
        '                                                If strParametro_intr = "<>" Then
        '                                                    strParametro_intr = "!="
        '                                                End If
        '                                            Else
        '                                                strParametro_intr = "!="
        '                                            End If
        '                                        Else
        '                                            strParametro_intr = "!="
        '                                        End If
        '                                        If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
        '                                            strBaseRaffronto_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"), String)
        '                                            If strBaseRaffronto_intr <> "-1" Then
        '                                                If Utility.StringOperation.FormatDateTime(sDataMorte).ToShortDateString <> DateTime.MaxValue.ToShortDateString Then
        '                                                    strBaseRaffrontoAPPOGGIO_intr = sDataMorte
        '                                                Else
        '                                                    Log.Debug("datamorte='' quindi non calcolo")
        '                                                    objExecFormula_intr = False
        '                                                    strBaseRaffrontoAPPOGGIO_intr = "''"
        '                                                End If
        '                                            Else
        '                                                strBaseRaffrontoAPPOGGIO_intr = "''"
        '                                            End If
        '                                        Else
        '                                            strBaseRaffrontoAPPOGGIO_intr = "''"
        '                                        End If
        '                                        objExecFormula_intr = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr, MyVsaEngine)
        '                                        Log.Debug("devo eseguire intrasmissibilita?" & strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr)
        '                                    End If
        '                                Catch exIntr As Exception
        '                                    Log.Debug("SanzInt.getCalcoloSanzioni.intrasmissibilita.errore::", exIntr)
        '                                End Try
        '                                Log.Debug("strBaseRaffrontoAPPOGGIO_intr=" & strBaseRaffrontoAPPOGGIO_intr)
        '                                objExecFormula = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione, MyVsaEngine)
        '                                Log.Debug("devo eseguire formula?" & strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione)

        '                                If CType(objExecFormula_intr, Boolean) = True Then
        '                                    Log.Debug("sono morto e la sanzione è intrasmissibile quindi non calcolo")
        '                                    'Se la condizione di Intrasmissibilità è verificata le sanzioni vanno a 0
        '                                    dblImportoPieno = 0
        '                                    dblImportoRidotto = 0
        '                                    dblImportoRiducibile = 0
        '                                    dblImportoNonRiducibile = 0
        '                                    'forzo il valore della voce come instramissibilità agli eredi
        '                                    'data_morte<>''
        '                                    strCOD_VOCE = "97"
        '                                Else
        '                                    If CType(objExecFormula, Boolean) = True Then
        '                                        Log.Debug("calcolo sanzione")
        '                                        If Not IsDBNull(strCalcolataSuAPPOGGIO) Then
        '                                            Log.Debug("strCalcolataSuAPPOGGIO::" & strCalcolataSuAPPOGGIO)
        '                                            'per CMGC NO REPLACE
        '                                            dblIMPORTO = impCalcolataSuAppoggio 'CDbl(strCalcolataSuAPPOGGIO.Replace(".", ","))
        '                                            If dblIMPORTO >= 0 Or bConsentiSanzNeg Then
        '                                                TipoMisura = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MISURA"), String)

        '                                                If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE")) Then
        '                                                    blnRiducibile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE"), String)
        '                                                Else
        '                                                    blnRiducibile = False
        '                                                End If

        '                                                If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE")) Then
        '                                                    blnCumulabile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE"), String)
        '                                                Else
        '                                                    blnCumulabile = False
        '                                                End If

        '                                                If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE")).CompareTo("") = 0 Then
        '                                                    dblValore = 0 'SanzInt.getCalcoloInteressi.errore
        '                                                Else
        '                                                    dblValore = CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE"), ".", ","), Double)
        '                                                End If

        '                                                If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO")).CompareTo("") = 0 Then
        '                                                    dblMinimo = 0
        '                                                Else
        '                                                    dblMinimo = FormatNumber(CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO"), ".", ","), Double), 2)
        '                                                End If

        '                                                If UCase(TipoMisura) = "F" Then
        '                                                    dblImportoSanzione = dblValore                                                  'dblMinimo
        '                                                ElseIf UCase(TipoMisura) = "P" Then
        '                                                    Log.Debug("dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)::" & dblIMPORTO.ToString & "::" & dblValore.ToString)
        '                                                    dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)
        '                                                    Log.Debug("dblImportoSanzione::" & dblImportoSanzione.ToString)
        '                                                    Log.Debug("dblMinimo::" & dblMinimo.ToString)
        '                                                    'GIULIA 08082005 aggiunto controllo
        '                                                    If bConsentiSanzNeg = False And dblImportoSanzione < dblMinimo Then
        '                                                        dblImportoSanzione = dblMinimo
        '                                                    End If
        '                                                End If
        '                                                dblImportoPieno = FormatNumber(dblImportoSanzione, 2)

        '                                                If blnRiducibile = True Then
        '                                                    '*** 20140701 - IMU/TARES ***
        '                                                    dblImportoRidotto = FormatNumber((dblImportoSanzione / nQuotaRiduzione), 2)
        '                                                    '*** ***
        '                                                    dblImportoRiducibile = dblImportoPieno
        '                                                    dblImportoNonRiducibile = 0
        '                                                Else
        '                                                    'giulia 17082005
        '                                                    dblImportoRidotto = 0
        '                                                    dblImportoRiducibile = 0
        '                                                    dblImportoNonRiducibile = 0
        '                                                End If
        '                                            Else
        '                                                dblImportoPieno = 0
        '                                                dblImportoRidotto = 0
        '                                                dblImportoRiducibile = 0
        '                                                dblImportoNonRiducibile = 0
        '                                            End If
        '                                        Else
        '                                            Log.Debug("strCalcolataSuAPPOGGIO null quindi non calcolo")
        '                                            dblImportoPieno = 0
        '                                            dblImportoRidotto = 0
        '                                            dblImportoRiducibile = 0
        '                                            dblImportoNonRiducibile = 0
        '                                        End If
        '                                    Else
        '                                        Log.Debug("non eseguo formula")
        '                                    End If
        '                                End If

        '                                'giulia 17082005
        '                                'a parità di codice sanzione se sono vere più condizioni, al contribuente viene applicata la sanzione minore
        '                                If MinImportoRidotto > dblImportoRidotto Or MinImportoRidotto = 0 Then
        '                                    MinSanzione = dblImportoPieno
        '                                    MinImportoRidotto = dblImportoRidotto
        '                                End If
        '                                strCOD_VOCEold = strCOD_VOCE
        '                                Log.Debug("getCalcoloSanzioni::passo a riga successiva")
        '                            Next
        '                        Catch ErrSanz As Exception
        '                            Log.Debug("getCalcoloSanzioni::arrsanz vuoto")
        '                        End Try
        '                    Next
        '                    '*********** fine ciclo nuovo

        '                    'giulia 17082005
        '                    TotaleSanzioni = TotaleSanzioni + MinSanzione                        'MinImportoRidotto
        '                    TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto
        '                    If objDSGetDatiSanzioni.Tables(0).Rows.Count > 0 Then
        '                        Row1 = newTable.NewRow()
        '                        Row1.Item("COD_ENTE") = strCODENTE
        '                        Row1.Item("ANNO") = strANNO
        '                        Row1.Item("COD_VOCE") = strCOD_VOCE
        '                        'giulia 17082005
        '                        Row1.Item("IMPORTO") = MinSanzione
        '                        Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
        '                        Row1.Item("IMPORTO_GIORNI") = dblMinimo 'usato come appoggio per restituire importo minimo 0
        '                        Row1.Item("ACCONTO") = 0
        '                        Row1.Item("SALDO") = 0
        '                        Row1.Item("ACCONTO_GIORNI") = 0
        '                        Row1.Item("SALDO_GIORNI") = 0
        '                        Row1.Item("DATA_INIZIO") = ""
        '                        Row1.Item("DATA_FINE") = ""
        '                        Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                        Row1.Item("N_SEMESTRI_SALDO") = 0
        '                        Row1.Item("N_GIORNI_ACCONTO") = 0
        '                        Row1.Item("N_GIORNI_SALDO") = 0
        '                        Row1.Item("TASSO") = 0
        '                        Row1.Item("GENERIC_ID") = lngGenericID
        '                        Row1.Item("ID_LEGAME") = idLegame
        '                        Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
        '                        '*** 20140701 - IMU/TARES ***
        '                        Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
        '                        '*** ***
        '                        newTable.Rows.Add(Row1)
        '                    End If
        '                End If
        '            Next
        '            'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
        '            objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("IMPORTO_SANZIONI") = TotaleSanzioni
        '            objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("IMPORTO_SANZIONI_RIDOTTO") = TotaleSanzioniRidotto
        '            '*** 20140701 - IMU/TARES ***
        '            If TotaleSanzioni > 0 Then
        '                objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
        '                Log.Debug("ho calcolato TotaleSanzioni::" & TotaleSanzioni.ToString)
        '                Log.Debug("ho calcolato TotaleSanzioniRidotto::" & TotaleSanzioniRidotto.ToString)
        '                Log.Debug("nQuotaRiduzione::" & nQuotaRiduzione.ToString)
        '            End If
        '            '*** ***
        '            objSituazioneBasePerSanzInt.AcceptChanges()
        '            objDSGetCalcoloSanzioni.Tables.Add(newTable)
        '        Next
        '        Return objDSGetCalcoloSanzioni
        '    Catch ex As Exception
        '        Log.Debug("Function::getCalcoloSanzioni::COMPlusService::si è verificato il seguente errore::", ex)
        '        Return Nothing
        '    End Try
        'End Function
        Public Function getCalcoloSanzioni(StringConnectionProvv As String, ByRef objSituazioneBasePerSanzInt() As ObjBaseIntSanz, ByVal objDSSituazioneBasePerSanzIntAppoggio As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, IdFase As Integer, CodTipoProcedimento As String, ByVal obj_HashTable As Hashtable, ByVal dsAnagrafica As System.Data.DataSet, ByVal bConsentiSanzNeg As Boolean, ByVal idLegame As Integer) As DataSet
            'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
            Try
                Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
                Dim objDSGetDatiSanzioni As DataSet
                Dim objDSGetCalcoloSanzioni As New DataSet
                Dim intCountSanzioni, intCountCondizioneSanzioni, intCountSanzSplit As Integer
                Dim TipoMisura As String
                Dim blnRiducibile, blnCumulabile As Boolean
                Dim dblValore, dblMinimo, dblImportoSanzione, dblImportoPieno, dblImportoRidotto, dblImportoRiducibile, dblImportoNonRiducibile As Double
                Dim dblIMPORTO As Double                'importo scaturito dal confronto tra versato e dichiarato
                Dim TotaleSanzioni As Double
                Dim TotaleSanzioniRidotto As Double

                Dim StrCondizione, strParametro, strBaseRaffronto, strCalcolataSu, strCOD_VOCE, strCOD_VOCEold As String
                Dim StrCondizione_intr, strParametro_intr, strBaseRaffronto_intr As String
                Dim strBaseRaffrontoAPPOGGIO, strCalcolataSuAPPOGGIO As String
                Dim strBaseRaffrontoAPPOGGIO_intr As String
                Dim objDRSituazioneBasePerSanzIntAppoggio() As DataRow
                Dim nQuotaRiduzione As Integer = 4 'valore fisso che aveva prima dell'IMU
                Dim impCalcolataSuAppoggio As Double
                'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI

                Dim newTable As DataTable
                newTable = New DataTable("SANZIONI")

                Dim NewColumn As New DataColumn
                NewColumn.ColumnName = "COD_ENTE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                Dim NewColumn1 As New DataColumn
                NewColumn1.ColumnName = "ANNO"
                NewColumn1.DataType = System.Type.GetType("System.String")
                NewColumn1.DefaultValue = "0"
                newTable.Columns.Add(NewColumn1)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "COD_VOCE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IMPORTO_RIDOTTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ACCONTO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "SALDO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "SALDO_GIORNI"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "DATA_INIZIO"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "DATA_FINE"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_SEMESTRI_SALDO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "TASSO"
                NewColumn.DataType = System.Type.GetType("System.Double")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "GENERIC_ID"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "ID_LEGAME"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = False
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "MOTIVAZIONI"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = ""
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_GIORNI_ACCONTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "N_GIORNI_SALDO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 0
                newTable.Columns.Add(NewColumn)
                '*** 20140701 - IMU/TARES ***
                NewColumn = New DataColumn
                NewColumn.ColumnName = "QUOTARIDUZIONESANZIONI"
                NewColumn.DataType = System.Type.GetType("System.Int64")
                NewColumn.DefaultValue = 1
                newTable.Columns.Add(NewColumn)
                '*** ***
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

                Dim MyVsaEngine As Microsoft.JScript.Vsa.VsaEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine
                Dim objExecFormula, objExecFormula_intr As Object
                '*** 20130801 - accertamento OSAP ***
                If IdFase > 0 Then
                    lngGenericID = IdFase
                End If
                '*** ***
                Log.Debug("getCalcoloSanzioni::devo ciclare su objSituazioneBasePerSanzInt")
                For Each myBaseSanz As ObjBaseIntSanz In objSituazioneBasePerSanzInt
                    Dim MinSanzione As Double = 0
                    Dim MinImportoRidotto As Double = 0
                    Dim arrSanz As Array
                    Dim arrSanzvalue As Array
                    Dim Row1 As DataRow
                    objDRSituazioneBasePerSanzIntAppoggio = objDSSituazioneBasePerSanzIntAppoggio.Tables(0).Select("ANNO='" & myBaseSanz.Anno & "'")
                    'ciclo sul dataset appoggio
                    For intCountCondizioneSanzioni = 0 To objDRSituazioneBasePerSanzIntAppoggio.Length - 1
                        If strCODVOCE = "-1" Then
                            strCODVOCE = strCODVOCE & "#" & strCODTIPOPROVVEDIMENTO
                        End If
                        Log.Debug("getCalcoloSanzioni::COD_VOCE::" & strCODVOCE)
                        'Split strCODVOCE con "," -> cod_voce@cod_provv
                        If strCODVOCE <> "" Then
                            strCOD_VOCEold = "-1"
                            arrSanz = Split(strCODVOCE, ",")
                            For intCountSanzSplit = 0 To arrSanz.Length - 1
                                Try
                                    arrSanzvalue = Split(arrSanz(intCountSanzSplit), "#")
                                    strCODVOCE = arrSanzvalue(0)
                                    strCODTIPOPROVVEDIMENTO = arrSanzvalue(1)

                                    objDSGetDatiSanzioni = objDBOPENgovProvvedimentiSelect.GetSanzioni(dblIMPORTO, myBaseSanz.Anno, strCODTRIBUTO, OggettoAtto.Capitolo.Sanzioni, strCODVOCE, strCODENTE, strCODTIPOPROVVEDIMENTO, lngGenericID, CodTipoProcedimento, StringConnectionProvv)
                                    'Ciclo sulle Sanzioni
                                    Log.Debug("ho trovato " & objDSGetDatiSanzioni.Tables(0).Rows.Count.ToString() & " sanzioni")
                                    For intCountSanzioni = 0 To objDSGetDatiSanzioni.Tables(0).Rows.Count - 1
                                        strCOD_VOCE = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("COD_VOCE"), String)
                                        Log.Debug("strCOD_VOCE=" & strCOD_VOCE)
                                        '*** 20140701 - IMU/TARES ***
                                        If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI")) Then
                                            nQuotaRiduzione = CInt(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("QUOTARIDUZIONESANZIONI"))
                                        End If
                                        '*** ***
                                        Log.Debug("nQuotaRiduzione=" & nQuotaRiduzione.ToString)
                                        If strCOD_VOCEold <> strCOD_VOCE And strCOD_VOCEold <> "-1" Then
                                            TotaleSanzioni = TotaleSanzioni + MinSanzione                                    'MinImportoRidotto
                                            TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto

                                            Row1 = newTable.NewRow()
                                            Row1.Item("COD_ENTE") = strCODENTE
                                            Row1.Item("ANNO") = myBaseSanz.Anno
                                            Row1.Item("COD_VOCE") = strCOD_VOCEold
                                            'giulia 17082005
                                            Row1.Item("IMPORTO") = MinSanzione
                                            Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
                                            Row1.Item("IMPORTO_GIORNI") = 0
                                            Row1.Item("ACCONTO") = 0
                                            Row1.Item("SALDO") = 0
                                            Row1.Item("ACCONTO_GIORNI") = 0
                                            Row1.Item("SALDO_GIORNI") = 0
                                            Row1.Item("DATA_INIZIO") = ""
                                            Row1.Item("DATA_FINE") = ""
                                            Row1.Item("N_SEMESTRI_ACCONTO") = 0
                                            Row1.Item("N_SEMESTRI_SALDO") = 0
                                            Row1.Item("N_GIORNI_ACCONTO") = 0
                                            Row1.Item("N_GIORNI_SALDO") = 0
                                            Row1.Item("TASSO") = 0
                                            Row1.Item("GENERIC_ID") = lngGenericID
                                            Row1.Item("ID_LEGAME") = idLegame
                                            Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
                                            '*** 20140701 - IMU/TARES ***
                                            Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
                                            '*** ***
                                            newTable.Rows.Add(Row1)
                                            MinSanzione = 0
                                            MinImportoRidotto = 0
                                        End If

                                        dblImportoPieno = 0
                                        dblImportoRidotto = 0
                                        dblImportoRiducibile = 0
                                        dblImportoNonRiducibile = 0
                                        TipoMisura = ""
                                        blnRiducibile = False
                                        blnCumulabile = False
                                        dblValore = 0
                                        dblMinimo = 0

                                        StrCondizione = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE"), String)
                                        Log.Debug("StrCondizione=" & StrCondizione)
                                        strParametro = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO"), String)
                                        Log.Debug("strParametro=" & strParametro)
                                        If strParametro = "=" Then
                                            strParametro = "=="
                                        End If
                                        If strParametro = "<>" Then
                                            strParametro = "!="
                                        End If
                                        strBaseRaffronto = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO"), String)
                                        Log.Debug("strBaseRaffronto=" & strBaseRaffronto)
                                        strCalcolataSu = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("DESC_BASE_CALCOLO"), String)
                                        Log.Debug("strCalcolataSu=" & strCalcolataSu)
                                        'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
                                        If strBaseRaffronto <> "-1" Then
                                            If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)) Then
                                                strBaseRaffrontoAPPOGGIO = 0
                                            Else
                                                strBaseRaffrontoAPPOGGIO = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strBaseRaffronto)).ToString()
                                            End If
                                        Else
                                            strBaseRaffrontoAPPOGGIO = 0
                                        End If
                                        Log.Debug("strBaseRaffrontoAPPOGGIO=" & strBaseRaffrontoAPPOGGIO)
                                        'DIPE 03/08/2005 Aggiunto controllo su campo se valore null
                                        If IsDBNull(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu)) Then
                                            strCalcolataSuAPPOGGIO = 0
                                        Else
                                            strCalcolataSuAPPOGGIO = CType(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu), String)
                                            Try
                                                impCalcolataSuAppoggio = CDbl(objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu))
                                                Log.Debug("impCalcolataSuAppoggio::" & impCalcolataSuAppoggio.ToString)
                                            Catch ex As Exception
                                                Log.Debug("errore conversione impCalcolataSuAppoggio:: da " & objDRSituazioneBasePerSanzIntAppoggio(intCountCondizioneSanzioni)(strCalcolataSu).ToString)
                                            End Try
                                        End If
                                        Log.Debug("strCalcolataSuAPPOGGIO=" & strCalcolataSuAPPOGGIO)
                                        'recupero valori per instrasmissibilità
                                        Try
                                            If (IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR")) And IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
                                                objExecFormula_intr = False
                                            Else
                                                If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"))) Then
                                                    StrCondizione_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CONDIZIONE_INTR"), String)
                                                    If StrCondizione_intr = "" Or StrCondizione_intr = "NULL" Then
                                                        StrCondizione_intr = "''"
                                                    End If
                                                Else
                                                    StrCondizione_intr = "''"
                                                End If
                                                If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"))) Then
                                                    If CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String) <> "-1" Then
                                                        strParametro_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("PARAMETRO_INTR"), String)
                                                        If strParametro_intr = "=" Then
                                                            strParametro_intr = "=="
                                                        End If
                                                        If strParametro_intr = "<>" Then
                                                            strParametro_intr = "!="
                                                        End If
                                                    Else
                                                        strParametro_intr = "!="
                                                    End If
                                                Else
                                                    strParametro_intr = "!="
                                                End If
                                                If (Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"))) Then
                                                    strBaseRaffronto_intr = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("BASE_RAFFRONTO_INTR"), String)
                                                    If strBaseRaffronto_intr <> "-1" Then
                                                        If dsAnagrafica.Tables.Count > 0 Then
                                                            If dsAnagrafica.Tables(0).Rows.Count > 0 Then
                                                                If IsDBNull(dsAnagrafica.Tables(0).Rows(0)(strBaseRaffronto_intr)) Then
                                                                    strBaseRaffrontoAPPOGGIO_intr = "''"
                                                                Else
                                                                    strBaseRaffrontoAPPOGGIO_intr = CType(dsAnagrafica.Tables(0).Rows(0)(strBaseRaffronto_intr), String)
                                                                    If strBaseRaffrontoAPPOGGIO_intr = "" Then
                                                                        strBaseRaffrontoAPPOGGIO_intr = "''"
                                                                    End If
                                                                End If
                                                            Else
                                                                Log.Debug("objAnagrafica.Tables(0).Rows.Count=0 quindi non calcolo")
                                                                objExecFormula_intr = False
                                                            End If
                                                        Else
                                                            Log.Debug("objAnagrafica.Tables.Count=0 quindi non calcolo")
                                                            objExecFormula_intr = False
                                                        End If
                                                    Else
                                                        strBaseRaffrontoAPPOGGIO_intr = "''"
                                                    End If
                                                Else
                                                    strBaseRaffrontoAPPOGGIO_intr = "''"
                                                End If
                                                objExecFormula_intr = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr, MyVsaEngine)
                                                Log.Debug("devo eseguire intrasmissibilita?" & strBaseRaffrontoAPPOGGIO_intr & strParametro_intr & StrCondizione_intr)
                                            End If
                                        Catch exIntr As Exception
                                            Log.Debug("SanzInt.getCalcoloSanzioni.intrasmissibilita.errore::", exIntr)
                                        End Try
                                        Log.Debug("strBaseRaffrontoAPPOGGIO_intr=" & strBaseRaffrontoAPPOGGIO_intr)
                                        objExecFormula = Microsoft.JScript.Eval.JScriptEvaluate(strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione, MyVsaEngine)
                                        Log.Debug("devo eseguire formula?" & strBaseRaffrontoAPPOGGIO & strParametro & StrCondizione)

                                        If CType(objExecFormula_intr, Boolean) = True Then
                                            Log.Debug("sono morto e la sanzione è intrasmissibile quindi non calcolo")
                                            'Se la condizione di Intrasmissibilità è verificata le sanzioni vanno a 0
                                            dblImportoPieno = 0
                                            dblImportoRidotto = 0
                                            dblImportoRiducibile = 0
                                            dblImportoNonRiducibile = 0
                                            'forzo il valore della voce come instramissibilità agli eredi data_morte<>''
                                            strCOD_VOCE = "97"
                                        Else
                                            If CType(objExecFormula, Boolean) = True Then
                                                Log.Debug("calcolo sanzione")
                                                If Not IsDBNull(strCalcolataSuAPPOGGIO) Then
                                                    Log.Debug("strCalcolataSuAPPOGGIO::" & strCalcolataSuAPPOGGIO)
                                                    'per CMGC NO REPLACE
                                                    dblIMPORTO = impCalcolataSuAppoggio 'dblIMPORTO = CDbl(strCalcolataSuAPPOGGIO.Replace(".", ","))
                                                    If dblIMPORTO >= 0 Or bConsentiSanzNeg Then
                                                        TipoMisura = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MISURA"), String)

                                                        If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE")) Then
                                                            blnRiducibile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("RIDUCIBILE"), String)
                                                        Else
                                                            blnRiducibile = False
                                                        End If

                                                        If Not IsDBNull(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE")) Then
                                                            blnCumulabile = CType(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("CUMULABILE"), String)
                                                        Else
                                                            blnCumulabile = False
                                                        End If

                                                        If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE")).CompareTo("") = 0 Then
                                                            dblValore = 0
                                                        Else
                                                            dblValore = CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("VALORE"), ".", ","), Double)
                                                        End If

                                                        If CStr(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO")).CompareTo("") = 0 Then
                                                            dblMinimo = 0
                                                        Else
                                                            dblMinimo = FormatNumber(CType(Replace(objDSGetDatiSanzioni.Tables(0).Rows(intCountSanzioni).Item("MINIMO"), ".", ","), Double), 2)
                                                        End If

                                                        If UCase(TipoMisura) = "F" Then
                                                            dblImportoSanzione = dblValore                                                  'dblMinimo
                                                        ElseIf UCase(TipoMisura) = "P" Then
                                                            Log.Debug("dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)::" & dblIMPORTO.ToString & "::" & dblValore.ToString)
                                                            dblImportoSanzione = FormatNumber(((dblIMPORTO * dblValore) / 100), 2)
                                                            Log.Debug("dblImportoSanzione::" & dblImportoSanzione.ToString)
                                                            Log.Debug("dblMinimo::" & dblMinimo.ToString)
                                                            'GIULIA 08082005 aggiunto controllo
                                                            If bConsentiSanzNeg = False And dblImportoSanzione < dblMinimo Then
                                                                dblImportoSanzione = dblMinimo
                                                            End If
                                                        End If
                                                        dblImportoPieno = FormatNumber(dblImportoSanzione, 2)

                                                        If blnRiducibile = True Then
                                                            '*** 20140701 - IMU/TARES ***
                                                            dblImportoRidotto = FormatNumber((dblImportoSanzione / nQuotaRiduzione), 2)
                                                            '*** ***
                                                            dblImportoRiducibile = dblImportoPieno
                                                            dblImportoNonRiducibile = 0
                                                        Else
                                                            dblImportoRidotto = 0
                                                            dblImportoRiducibile = 0
                                                            dblImportoNonRiducibile = 0
                                                        End If
                                                    Else
                                                        dblImportoPieno = 0
                                                        dblImportoRidotto = 0
                                                        dblImportoRiducibile = 0
                                                        dblImportoNonRiducibile = 0
                                                    End If
                                                Else
                                                    Log.Debug("strCalcolataSuAPPOGGIO null quindi non calcolo")
                                                    dblImportoPieno = 0
                                                    dblImportoRidotto = 0
                                                    dblImportoRiducibile = 0
                                                    dblImportoNonRiducibile = 0
                                                End If
                                            Else
                                                Log.Debug("non eseguo formula")
                                            End If
                                        End If

                                        'giulia 17082005
                                        'If MinSanzione > dblImportoPieno Or MinSanzione = 0 Then
                                        '  MinSanzione = dblImportoPieno
                                        '  MinImportoRidotto = dblImportoRidotto
                                        'End If
                                        'a parità di codice sanzione se sono vere più condizioni, al contribuente viene applicata la sanzione minore
                                        If MinImportoRidotto > dblImportoRidotto Or MinImportoRidotto = 0 Then
                                            MinSanzione = dblImportoPieno
                                            MinImportoRidotto = dblImportoRidotto
                                        End If
                                        strCOD_VOCEold = strCOD_VOCE
                                        Log.Debug("getCalcoloSanzioni::passo a riga successiva")
                                    Next
                                Catch ErrSanz As Exception
                                    Log.Debug("getCalcoloSanzioni::arrsanz vuoto")
                                End Try
                            Next
                            '*********** fine ciclo nuovo

                            'giulia 17082005
                            'TotaleSanzioni = TotaleSanzioni + MinSanzione
                            TotaleSanzioni = TotaleSanzioni + MinSanzione                        'MinImportoRidotto
                            TotaleSanzioniRidotto = TotaleSanzioniRidotto + MinImportoRidotto
                            If objDSGetDatiSanzioni.Tables(0).Rows.Count > 0 Then
                                Row1 = newTable.NewRow()
                                Row1.Item("COD_ENTE") = strCODENTE
                                Row1.Item("ANNO") = myBaseSanz.Anno
                                Row1.Item("COD_VOCE") = strCOD_VOCE
                                'giulia 17082005
                                Row1.Item("IMPORTO") = MinSanzione
                                Row1.Item("IMPORTO_RIDOTTO") = MinImportoRidotto
                                Row1.Item("IMPORTO_GIORNI") = dblMinimo 'usato come appoggio per restituire importo minimo 0
                                Row1.Item("ACCONTO") = 0
                                Row1.Item("SALDO") = 0
                                Row1.Item("ACCONTO_GIORNI") = 0
                                Row1.Item("SALDO_GIORNI") = 0
                                Row1.Item("DATA_INIZIO") = ""
                                Row1.Item("DATA_FINE") = ""
                                Row1.Item("N_SEMESTRI_ACCONTO") = 0
                                Row1.Item("N_SEMESTRI_SALDO") = 0
                                Row1.Item("N_GIORNI_ACCONTO") = 0
                                Row1.Item("N_GIORNI_SALDO") = 0
                                Row1.Item("TASSO") = 0
                                Row1.Item("GENERIC_ID") = lngGenericID
                                Row1.Item("ID_LEGAME") = idLegame
                                Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO
                                '*** 20140701 - IMU/TARES ***
                                Row1.Item("QUOTARIDUZIONESANZIONI") = nQuotaRiduzione
                                '*** ***
                                newTable.Rows.Add(Row1)
                            End If
                        End If
                    Next
                    'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
                    myBaseSanz.Sanzioni = TotaleSanzioni
                    myBaseSanz.SanzioniRidotto = TotaleSanzioniRidotto
                    '*** 20140701 - IMU/TARES ***
                    If TotaleSanzioni > 0 Then
                        myBaseSanz.QuotaRiduzione = nQuotaRiduzione
                        Log.Debug("ho calcolato TotaleSanzioni::" & TotaleSanzioni.ToString)
                        Log.Debug("ho calcolato TotaleSanzioniRidotto::" & TotaleSanzioniRidotto.ToString)
                        Log.Debug("nQuotaRiduzione::" & nQuotaRiduzione.ToString)
                    End If
                    '*** ***
                    objDSGetCalcoloSanzioni.Tables.Add(newTable)
                Next
                Return objDSGetCalcoloSanzioni
            Catch ex As Exception
                Log.Debug("Function::getCalcoloSanzioni::COMPlusService::si è verificato il seguente errore::", ex)
                Return Nothing
            End Try
        End Function
        '*** ***

        '**** 201809 - Cartelle Insoluti ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="IdEnte"></param>
        ''' <param name="IdTributo"></param>
        ''' <param name="CodVoce"></param>
        ''' <param name="TipoProvvedimento"></param>
        ''' <param name="TipoProcedimento"></param>
        ''' <param name="Fase"></param>
        ''' <param name="DataElaborazione"></param>
        ''' <param name="sDataScadenzaSaldo"></param>
        ''' <param name="IdLegame"></param>
        ''' <param name="ListToCalc"></param>
        ''' <param name="myStringConnection"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="30/10/2019">
        ''' la datadi scadenza degli interessi deve arrivare da db nel formato AAAAMMGG altrimenti su GC la conversione in data non funziona
        ''' </revision></revisionHistory>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Public Function getCalcoloInteressi(ByVal IdEnte As String, ByVal IdTributo As String, ByVal CodVoce As String, ByVal TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, sDataScadenzaAcconto As String, sDataScadenzaSaldo As String, IdLegame As Integer, ByRef ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni()
            Try
                Dim fncSel As New DBOPENgovProvvedimentiSelect
                Dim dsInteressi As DataSet
                Dim dsTipoInteressi As New DataSet
                Dim strDAL_Interesse, strAL_Interesse As String
                Dim TassoInteressi As Double
                Dim Semestri As Integer
                Dim Giorni, GiorniAcconto, GiorniSaldo As Integer
                Dim ImportoTotale, ImportoGGTotale, ImportoGGAcconto, ImportoGGSaldo As Double
                Dim myList As New ArrayList
                Dim DATA_AL As Date
                Dim datamod, strDATA_AL As String


                For Each myItem As ObjBaseIntSanz In ListToCalc
                    '*** 20130801 - accertamento OSAP ***
                    dsTipoInteressi = fncSel.GetTipologiaInteressi(myItem.Anno, IdTributo, IdEnte, myStringConnection)
                    '*** ***
                    If dsTipoInteressi.Tables(0).Rows.Count > 0 Then
                        For Each myRowTipoInt As DataRow In dsTipoInteressi.Tables(0).Rows
                            Log.Debug("ho scadenza interessi")
                            If TipoProvvedimento <> OggettoAtto.Provvedimento.Coattivo Then
                                sDataScadenzaAcconto = Utility.StringOperation.FormatString(myRowTipoInt("DATA_SCADENZA"))
                                sDataScadenzaSaldo = Utility.StringOperation.FormatString(myRowTipoInt("DATA_SCADENZA_SALDO"))
                            End If
                            Log.Debug("sDataScadenzaAcconto->" + sDataScadenzaAcconto + " giradata->" + myUtility.GiraDataFromDB(sDataScadenzaAcconto))
                            Log.Debug("sDataScadenzaSaldo->" + sDataScadenzaSaldo + " giradata->" + myUtility.GiraDataFromDB(sDataScadenzaSaldo))
                            If myItem.DifferenzaImposta <> 0 Then
                                Log.Debug("ho importo " + myItem.DifferenzaImposta.ToString)
                                Log.Debug("devo richiamare getinteressi")
                                '*** 201408026 - l'anno di paragone deve essere l'anno di scadenza del tributo e non l'anno di imposta ***
                                myItem.Anno = sDataScadenzaAcconto.Substring(0, 4)
                                '*** ***
                                dsInteressi = fncSel.GetInteressi(myStringConnection, IdEnte, IdTributo, myItem.Anno, CodVoce, Fase, TipoProvvedimento, TipoProcedimento)
                                For Each myRow As DataRow In dsInteressi.Tables(0).Rows
                                    ImportoTotale = 0
                                    ImportoGGTotale = 0

                                    Semestri = 0 : Giorni = 0

                                    TassoInteressi = FormatNumber(CType(myRow.Item("TASSO_ANNUALE"), Double), 2)
                                    strDAL_Interesse = CType(myRow.Item("DAL"), String)
                                    If Not IsDBNull(myRow.Item("AL")) Then
                                        strAL_Interesse = CType(myRow.Item("AL"), String)
                                    Else
                                        strAL_Interesse = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
                                    End If
                                    sDataScadenzaAcconto = Utility.StringOperation.FormatString(myRow("SCADENZAACCONTO"))
                                    sDataScadenzaSaldo = Utility.StringOperation.FormatString(myRow("SCADENZASALDO"))
                                    '*** 20201230 - tolto perché non giustificato ***
                                    'If Month(DataElaborazione) = 6 Or Month(DataElaborazione) = 12 Then
                                    '    If Month(DataElaborazione) = 12 And Day(DataElaborazione) = 31 Then
                                    '        datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione) - 1, "00")
                                    '    Else
                                    '        datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione), "00")
                                    '    End If
                                    'Else
                                    '    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")
                                    'End If
                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")
                                    '*** ***

                                    'If myItem.Anno >= Left(strDAL_Interesse, 4) Then
                                    '    strDAL_Interesse = sDataScadenzaAcconto
                                    'End If
                                    Log.Debug("datamod->" + datamod + " giradata->" + myUtility.GiraDataFromDB(datamod))
                                    Log.Debug("strAL_Interesse->" + strAL_Interesse + " giradata->" + myUtility.GiraDataFromDB(strAL_Interesse))
                                    If DateDiff(DateInterval.Month, CDate(myUtility.GiraDataFromDB(datamod)), CDate(myUtility.GiraDataFromDB(strAL_Interesse)), FirstDayOfWeek.Monday) < 0 Then
                                        DATA_AL = (myUtility.GiraDataFromDB(strAL_Interesse))
                                        strDATA_AL = strAL_Interesse
                                    Else
                                        DATA_AL = (myUtility.GiraDataFromDB(datamod))
                                        strDATA_AL = datamod
                                    End If
                                    'Questa è la logica per determinare se il calcolo degli interessi deve essere giornaliero, semestrale oppure mezzo e mezzo...
                                    'se DAL è maggiore di AL
                                    Log.Debug("strDAL_Interesse->" + strDAL_Interesse + " giradata->" + myUtility.GiraDataFromDB(strDAL_Interesse))
                                    Log.Debug("COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI->" + COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI + " giradata->" + myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI))
                                    ImportoGGTotale = 0 : ImportoGGAcconto = 0 : ImportoGGSaldo = 0 : ImportoTotale = 0
                                    If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, FirstDayOfWeek.Monday) > 0 Then
                                        'se DAL è maggiore di 31/12/2006...calcolo solo gli interessi in GIORNI
                                        If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) < 0 Then
                                            Log.Debug("SanzInt.getCalcoloInteressi.DAL è maggiore di 31/12/2006...calcolo solo gli interessi in GIORNI")
                                            If Calcolo_Giorni(myUtility.GiraDataFromDB(strDAL_Interesse), DATA_AL, myUtility.GiraDataFromDB(sDataScadenzaAcconto), myUtility.GiraDataFromDB(sDataScadenzaSaldo), Giorni, GiorniAcconto, GiorniSaldo) = True Then
                                                If myItem.DifferenzaImposta <> 0 Then
                                                    If Utility.StringOperation.FormatInt(myRow.Item("TipoCalcoloInteressi")) = 2 Then 'ACCONTO-SALDO
                                                        If GiorniAcconto > 0 Then
                                                            ImportoGGAcconto = FormatNumber((((myItem.DifferenzaImpostaAcconto * TassoInteressi) / 100) / 365) * GiorniAcconto, 2)
                                                            Log.Debug("SanzInt.getCalcoloInteressi.ACCONTO. ((DifferenzaImpostaAcconto[" + myItem.DifferenzaImpostaAcconto.ToString + "]*(TassoInteressi[" + TassoInteressi.ToString + "]/100))/365)*GiorniAcconto[" + GiorniAcconto.ToString + "]=ImportoGGAcconto[" + ImportoGGAcconto.ToString + "]")
                                                        End If
                                                        If GiorniSaldo > 0 Then
                                                            ImportoGGSaldo = FormatNumber((((myItem.DifferenzaImpostaSaldo * TassoInteressi) / 100) / 365) * GiorniSaldo, 2)
                                                            Log.Debug("SanzInt.getCalcoloInteressi.SALDO. ((DifferenzaImpostaSaldo[" + myItem.DifferenzaImpostaSaldo.ToString + "]*(TassoInteressi[" + TassoInteressi.ToString + "]/100))/365)*GiorniSaldo[" + GiorniSaldo.ToString + "]=ImportoGGSaldo[" + ImportoGGSaldo.ToString + "]")
                                                        End If
                                                        ImportoGGTotale = ImportoGGAcconto + ImportoGGSaldo
                                                    Else 'SOLO SALDO
                                                        ImportoGGTotale = FormatNumber((((myItem.DifferenzaImposta * TassoInteressi) / 100) / 365) * Giorni, 2)
                                                        Log.Debug("SanzInt.getCalcoloInteressi.TOTALE. ((DifferenzaImposta[" + myItem.DifferenzaImposta.ToString + "]*(TassoInteressi[" + TassoInteressi.ToString + "]/100))/365)*Giorni[" + Giorni.ToString + "]=ImportoGGTotale[" + ImportoGGTotale.ToString + "]")
                                                    End If
                                                End If
                                            Else
                                                Log.Debug("SanzInt.getCalcoloInteressi.errore in calcolo giorni")
                                            End If
                                        Else                                        'altrimenti se DAL è minore di 31/12/2006...
                                            '...e se AL minore di 31/12/2006...calcolo solo gli interessi in SEMESTRI
                                            Log.Debug("SanzInt.getCalcoloInteressi.DAL è maggiore di 31/12/2006...calcolo solo gli interessi in SEMESTRI tolto perchè in prescritto")
                                        End If
                                    Else
                                        'se dal minore di al
                                        Semestri = 0
                                        Giorni = 0
                                    End If

                                    'mantenere questa procedura e implementare quella del calcolo in giorni. prevedere la scelta da front end testare la il campo nella tabella TP_GENERALE_ICI                            
                                    Dim myInteresse As New ObjInteressiSanzioni
                                    myInteresse.COD_ENTE = IdEnte
                                    myInteresse.ANNO = myItem.Anno
                                    myInteresse.COD_VOCE = myRow("CODICEVOCE").ToString
                                    myInteresse.IMPORTO = ImportoTotale
                                    myInteresse.IMPORTO_GIORNI = ImportoGGTotale
                                    myInteresse.IMPORTO_RIDOTTO = 0
                                    myInteresse.ACCONTO = ImportoGGAcconto
                                    myInteresse.SALDO = ImportoGGSaldo
                                    myInteresse.ACCONTO_GIORNI = GiorniAcconto
                                    myInteresse.SALDO_GIORNI = GiorniSaldo
                                    myInteresse.DATA_INIZIO = strDAL_Interesse
                                    myInteresse.DATA_FINE = strDATA_AL
                                    myInteresse.N_SEMESTRI_ACCONTO = 0
                                    myInteresse.N_SEMESTRI_SALDO = Semestri
                                    myInteresse.N_SEMESTRI_TOTALI = Semestri
                                    myInteresse.N_GIORNI_ACCONTO = 0
                                    myInteresse.N_GIORNI_SALDO = Giorni
                                    myInteresse.N_GIORNI_TOTALI = Giorni
                                    myInteresse.TASSO = TassoInteressi
                                    myInteresse.IdFase = Fase
                                    myInteresse.ID_LEGAME = IdLegame
                                    myInteresse.COD_TIPO_PROVVEDIMENTO = TipoProvvedimento
                                    myList.Add(myInteresse)
                                    myItem.Interessi += ImportoGGTotale
                                Next
                            Else
                                Log.Debug("non ho importo")
                            End If
                        Next
                    Else
                        Log.Debug("non ho scadenza interessi")
                        Dim myInteresse As New ObjInteressiSanzioni
                        myInteresse.COD_ENTE = IdEnte
                        myInteresse.ANNO = myItem.Anno
                        myInteresse.COD_VOCE = 0
                        myInteresse.IMPORTO = ImportoTotale
                        myInteresse.IMPORTO_GIORNI = ImportoGGTotale
                        myInteresse.IMPORTO_RIDOTTO = 0
                        myInteresse.ACCONTO = ImportoGGAcconto
                        myInteresse.SALDO = ImportoGGSaldo
                        myInteresse.ACCONTO_GIORNI = GiorniAcconto
                        myInteresse.SALDO_GIORNI = GiorniSaldo
                        myInteresse.DATA_INIZIO = ""
                        myInteresse.DATA_FINE = ""
                        myInteresse.N_SEMESTRI_ACCONTO = 0
                        myInteresse.N_SEMESTRI_SALDO = 0
                        myInteresse.N_SEMESTRI_TOTALI = 0
                        myInteresse.N_GIORNI_ACCONTO = 0
                        myInteresse.N_GIORNI_SALDO = 0
                        myInteresse.N_GIORNI_TOTALI = 0
                        myInteresse.TASSO = 0
                        myInteresse.IdFase = Fase
                        myInteresse.ID_LEGAME = IdLegame
                        myInteresse.COD_TIPO_PROVVEDIMENTO = TipoProvvedimento
                        myList.Add(myInteresse)
                    End If
                Next

                Return CType(myList.ToArray(GetType(ObjInteressiSanzioni)), ObjInteressiSanzioni())
            Catch ex As Exception
                Log.Debug("SanzInt.getCalcoloInteressi.errore:: ", ex)
                Return Nothing
            End Try
        End Function
        'Public Function getCalcoloInteressi(ByVal IdEnte As String, ByVal IdTributo As String, ByVal CodVoce As String, ByVal TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, sDataDecorrenzaScadenza As String, IdLegame As Integer, ByRef ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni()
        '    Try
        '        Dim fncSel As New DBOPENgovProvvedimentiSelect
        '        Dim dsInteressi As DataSet
        '        Dim dsTipoInteressi As New DataSet
        '        Dim strDAL_Interesse, strAL_Interesse As String
        '        Dim TassoInteressi As Double
        '        Dim Semestri As Integer
        '        Dim Giorni As Integer
        '        Dim ImportoTotale As Double
        '        Dim ImportoTotaleGG As Double
        '        Dim myList As New ArrayList
        '        Dim sal_acc_div As Boolean
        '        Dim DATA_AL As Date
        '        Dim datamod, strDATA_AL As String


        '        For Each myItem As ObjBaseIntSanz In ListToCalc
        '            '*** 20130801 - accertamento OSAP ***
        '            dsTipoInteressi = fncSel.GetTipologiaInteressi(myItem.Anno, IdTributo, IdEnte, myStringConnection)
        '            '*** ***
        '            If dsTipoInteressi.Tables(0).Rows.Count > 0 Then
        '                For Each myRowTipoInt As DataRow In dsTipoInteressi.Tables(0).Rows
        '                    Log.Debug("ho scadenza interessi")
        '                    If TipoProvvedimento <> OggettoAtto.Provvedimento.Coattivo Then
        '                        sDataDecorrenzaScadenza = Utility.StringOperation.FormatString(myRowTipoInt("DATA_SCADENZA"))
        '                    End If
        '                    Log.Debug("sDataDecorrenzaScadenza->" + sDataDecorrenzaScadenza + " giradata->" + myUtility.GiraDataFromDB(sDataDecorrenzaScadenza))
        '                    If myItem.DifferenzaImposta <> 0 Then
        '                        Log.Debug("ho importo " + myItem.DifferenzaImposta.ToString)
        '                        Log.Debug("devo richiamare getinteressi")
        '                        dsInteressi = fncSel.GetInteressi(myStringConnection, IdEnte, IdTributo, myItem.Anno, CodVoce, Fase, TipoProvvedimento, TipoProcedimento)
        '                        For Each myRow As DataRow In dsInteressi.Tables(0).Rows
        '                            '*** 201408026 - l'anno di paragone deve essere l'anno di scadenza del tributo e non l'anno di imposta ***
        '                            myItem.Anno = sDataDecorrenzaScadenza.Substring(0, 4)
        '                            '*** ***
        '                            ImportoTotale = 0
        '                            ImportoTotaleGG = 0

        '                            Semestri = 0 : Giorni = 0

        '                            TassoInteressi = FormatNumber(CType(myRow.Item("TASSO_ANNUALE"), Double), 2)
        '                            strDAL_Interesse = CType(myRow.Item("DAL"), String)
        '                            If Not IsDBNull(myRow.Item("AL")) Then
        '                                strAL_Interesse = CType(myRow.Item("AL"), String)
        '                            Else
        '                                strAL_Interesse = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
        '                            End If

        '                            If Month(DataElaborazione) = 6 Or Month(DataElaborazione) = 12 Then
        '                                If Month(DataElaborazione) = 12 And Day(DataElaborazione) = 31 Then
        '                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione) - 1, "00")
        '                                Else
        '                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione), "00")
        '                                End If
        '                            Else
        '                                datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")
        '                            End If

        '                            If myItem.Anno >= Left(strDAL_Interesse, 4) Then
        '                                strDAL_Interesse = sDataDecorrenzaScadenza
        '                                'prevedere di prendere la data di acconto che sarà configurata nella tabella TP_GENERALE_ICI 
        '                                sal_acc_div = True
        '                            Else
        '                                sal_acc_div = False
        '                            End If
        '                            Log.Debug("datamod->" + datamod + " giradata->" + myUtility.GiraDataFromDB(datamod))
        '                            Log.Debug("strAL_Interesse->" + strAL_Interesse + " giradata->" + myUtility.GiraDataFromDB(strAL_Interesse))
        '                            If DateDiff(DateInterval.Month, CDate(myUtility.GiraDataFromDB(datamod)), CDate(myUtility.GiraDataFromDB(strAL_Interesse)), FirstDayOfWeek.Monday) < 0 Then
        '                                DATA_AL = (myUtility.GiraDataFromDB(strAL_Interesse))
        '                                strDATA_AL = strAL_Interesse
        '                            Else
        '                                DATA_AL = (myUtility.GiraDataFromDB(datamod))
        '                                strDATA_AL = datamod
        '                            End If
        '                            'Questa è la logica per determinare se il calcolo degli interessi deve essere giornaliero, semestrale oppure mezzo e mezzo...
        '                            'se DAL è maggiore di AL
        '                            Log.Debug("strDAL_Interesse->" + strDAL_Interesse + " giradata->" + myUtility.GiraDataFromDB(strDAL_Interesse))
        '                            Log.Debug("COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI->" + COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI + " giradata->" + myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI))
        '                            If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, FirstDayOfWeek.Monday) > 0 Then
        '                                'se DAL è maggiore di 31/12/2006...calcolo solo gli interessi in GIORNI
        '                                If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) < 0 Then
        '                                    Calcolo_GiorniTARSU(myUtility.GiraDataFromDB(strDAL_Interesse), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), Giorni, 0, sal_acc_div)
        '                                    If myItem.DifferenzaImposta <> 0 Then
        '                                        'calcolo interesse unica soluzione
        '                                        ImportoTotaleGG = FormatNumber((((myItem.DifferenzaImposta * TassoInteressi) / 100) / 365) * Giorni, 2)
        '                                    Else
        '                                        ImportoTotaleGG = 0
        '                                        ImportoTotale = 0
        '                                    End If
        '                                Else                                        'altrimenti se DAL è minore di 31/12/2006...
        '                                    '...e se AL minore di 31/12/2006...calcolo solo gli interessi in SEMESTRI
        '                                    If DateDiff(DateInterval.Day, DATA_AL, CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) >= 0 Then
        '                                        Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, Semestri, 0, sal_acc_div)
        '                                        If myItem.DifferenzaImposta <> 0 Then
        '                                            'calcolo interesse unica soluzione
        '                                            ImportoTotale = FormatNumber(((myItem.DifferenzaImposta * TassoInteressi) / 100) * Semestri, 2)
        '                                        Else
        '                                            ImportoTotale = 0
        '                                        End If
        '                                    Else
        '                                        '...e se AL maggiore di 31/12/2006...calcolo gli interessi in SEMESTRI da DAL al 31/12/2006calcolo gli interessi in GIORNI da 01/01/2007 a AL
        '                                        Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), Semestri, 0, sal_acc_div)
        '                                        If myItem.DifferenzaImposta <> 0 Then
        '                                            'calcolo interesse unica soluzione
        '                                            ImportoTotale = FormatNumber(((myItem.DifferenzaImposta * TassoInteressi) / 100) * Semestri, 2)
        '                                        Else
        '                                            ImportoTotale = 0
        '                                        End If
        '                                        sal_acc_div = False
        '                                        Calcolo_GiorniTARSU(CDate(COSTANTValue.CostantiProv.DATA_INIZIO_INTERESSI_GIORNALIERI), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), Giorni, 0, sal_acc_div)
        '                                        If myItem.DifferenzaImposta <> 0 Then
        '                                            'calcolo interesse unica soluzione
        '                                            ImportoTotaleGG = FormatNumber((((myItem.DifferenzaImposta * TassoInteressi) / 100) / 365) * Giorni, 2)
        '                                        Else
        '                                            ImportoTotaleGG = 0
        '                                            ImportoTotale = 0
        '                                        End If
        '                                    End If
        '                                End If
        '                            Else
        '                                'se dal minore di al
        '                                ImportoTotaleGG = 0
        '                                ImportoTotale = 0

        '                                Semestri = 0
        '                                Giorni = 0
        '                            End If

        '                            'mantenere questa procedura e implementare quella del calcolo in giorni. prevedere la scelta da front end testare la il campo nella tabella TP_GENERALE_ICI                            
        '                            Dim myInteresse As New ObjInteressiSanzioni
        '                            myInteresse.COD_ENTE = IdEnte
        '                            myInteresse.ANNO = myItem.Anno
        '                            myInteresse.COD_VOCE = myRow("CODICEVOCE").ToString
        '                            myInteresse.IMPORTO = ImportoTotale
        '                            myInteresse.IMPORTO_GIORNI = ImportoTotaleGG
        '                            myInteresse.IMPORTO_RIDOTTO = 0
        '                            myInteresse.ACCONTO = 0
        '                            myInteresse.SALDO = 0
        '                            myInteresse.ACCONTO_GIORNI = 0
        '                            myInteresse.SALDO_GIORNI = 0
        '                            myInteresse.DATA_INIZIO = strDAL_Interesse
        '                            myInteresse.DATA_FINE = strDATA_AL
        '                            myInteresse.N_SEMESTRI_ACCONTO = 0
        '                            myInteresse.N_SEMESTRI_SALDO = Semestri
        '                            myInteresse.N_SEMESTRI_TOTALI = Semestri
        '                            myInteresse.N_GIORNI_ACCONTO = 0
        '                            myInteresse.N_GIORNI_SALDO = Giorni
        '                            myInteresse.N_GIORNI_TOTALI = Giorni
        '                            myInteresse.TASSO = TassoInteressi
        '                            myInteresse.IdFase = Fase
        '                            myInteresse.ID_LEGAME = IdLegame
        '                            myInteresse.COD_TIPO_PROVVEDIMENTO = TipoProvvedimento
        '                            myList.Add(myInteresse)
        '                            myItem.Interessi += ImportoTotaleGG
        '                        Next
        '                    Else
        '                        Log.Debug("non ho importo")
        '                    End If
        '                Next
        '            Else
        '                Log.Debug("non ho scadenza interessi")
        '                Dim myInteresse As New ObjInteressiSanzioni
        '                myInteresse.COD_ENTE = IdEnte
        '                myInteresse.ANNO = myItem.Anno
        '                myInteresse.COD_VOCE = 0
        '                myInteresse.IMPORTO = ImportoTotale
        '                myInteresse.IMPORTO_GIORNI = ImportoTotaleGG
        '                myInteresse.IMPORTO_RIDOTTO = 0
        '                myInteresse.ACCONTO = 0
        '                myInteresse.SALDO = 0
        '                myInteresse.ACCONTO_GIORNI = 0
        '                myInteresse.SALDO_GIORNI = 0
        '                myInteresse.DATA_INIZIO = ""
        '                myInteresse.DATA_FINE = ""
        '                myInteresse.N_SEMESTRI_ACCONTO = 0
        '                myInteresse.N_SEMESTRI_SALDO = 0
        '                myInteresse.N_SEMESTRI_TOTALI = 0
        '                myInteresse.N_GIORNI_ACCONTO = 0
        '                myInteresse.N_GIORNI_SALDO = 0
        '                myInteresse.N_GIORNI_TOTALI = 0
        '                myInteresse.TASSO = 0
        '                myInteresse.IdFase = Fase
        '                myInteresse.ID_LEGAME = IdLegame
        '                myInteresse.COD_TIPO_PROVVEDIMENTO = TipoProvvedimento
        '                myList.Add(myInteresse)
        '            End If
        '        Next

        '        Return CType(myList.ToArray(GetType(ObjInteressiSanzioni)), ObjInteressiSanzioni())
        '    Catch ex As Exception
        '        Log.Debug("SanzInt.getCalcoloInteressi.errore::", ex)
        '        Return Nothing
        '    End Try
        'End Function
        'Public Function getCalcoloInteressiICI(ByRef objSituazioneBasePerSanzInt As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODCAPITOLO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, ByVal intTipoBaseCalcolo As Integer, ByVal objHashTable As Hashtable, Optional ByVal idLegame As Integer = -1, Optional ByVal dsVersTard As DataSet = Nothing) As DataSet

        '    'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
        '    'intTipoBaseCalcolo=1 --> DIFFERENZA IMPOSTA 
        '    'intTipoBaseCalcolo=2 --> IMPOSTA VERSATA (VERSAMENTO TARDIVO)

        '    Try

        '        Dim culture As IFormatProvider

        '        culture = New CultureInfo("it-IT", True)

        '        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")

        '        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect          
        '        Dim objDSGetTipologiaDatiInteressi As DataSet
        '        Dim objDSGetDatiInteressi As DataSet
        '        Dim objDSGetCalcoloInteressi As New DataSet
        '        Dim objDSGetGeneraleICI As DataSet
        '        Dim intCount, intCountTassi As Integer
        '        Dim strDAL, strAL, strDAL_ORIG As String
        '        Dim dblTassoInteressi As Double
        '        Dim intSemestriACCONTO, intSemestriSALDO As Integer
        '        Dim intGiorniACCONTO, intGiorniSALDO As Integer

        '        Dim strANNO As String               'anno di liquidazione (Pre Accertamento)
        '        Dim dblIMPORTO, dblIMPORTOACCONTO, dblIMPORTOSALDO As Double                'importo scaturito dal confronto tra versato e dichiarato
        '        Dim dblIMPORTOINTERESSETOTALE, dblIMPORTOINTERESSEACCONTO, dblIMPORTOINTERESSESALDO As Double
        '        Dim dblIMPORTOINTERESSETOTALE_GG, dblIMPORTOINTERESSEACCONTO_GG, dblIMPORTOINTERESSESALDO_GG As Double
        '        Dim dblIMPORTOFINALEINTERESSI, dblIMPORTOFINALEINTERESSI_GG As Double
        '        Dim sal_acc_div As Boolean
        '        Dim DATA_AL As Date
        '        Dim datamod, strDATA_AL As String
        '        Dim intTipoCalcoloInteressi As Integer              'intTipoCalcoloInteressi=1 ACCONTO - intTipoCalcoloInteressi=2 SALDO - intTipoCalcoloInteressi=3 UNICA SOLUZIONE

        '        Dim sDataVersamentoAcconto As String
        '        Dim sDataVersamentoSaldo As String
        '        Dim objCostanti As COSTANTValue.CostantiProv

        '        Dim strCOD_VOCE As String
        '        Dim blnModalitaUS As Boolean = False
        '        Dim DataElaborazione As Date

        '        'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI

        '        Dim newTable As DataTable
        '        newTable = New DataTable("INTERESSI")

        '        Dim NewColumn As DataColumn

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_ENTE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ANNO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = "0"
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_VOCE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_RIDOTTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_INIZIO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_FINE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "TASSO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "GENERIC_ID"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ID_LEGAME"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "MOTIVAZIONI"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = System.DBNull.Value
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect           

        '        For intCount = 0 To objSituazioneBasePerSanzInt.Tables(0).Rows.Count - 1
        '            dblIMPORTOFINALEINTERESSI = 0 : dblIMPORTOFINALEINTERESSI_GG = 0

        '            strANNO = CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("ANNO"), Double)
        '            dblIMPORTO = FormatNumber(CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_TOTALE").Replace(".", ","), Double), 2)
        '            dblIMPORTOACCONTO = FormatNumber(CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_ACCONTO").Replace(".", ","), Double), 2)
        '            dblIMPORTOSALDO = FormatNumber(CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_SALDO").Replace(".", ","), Double), 2)
        '            blnModalitaUS = CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("FLAG_MODALITA_UNICA_SOLUZIONE"), Boolean)
        '            Log.Debug("getCalcoloInteressi::differenzaimposta::" & CStr(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_TOTALE")))
        '            Log.Debug("getCalcoloInteressi::differenzaimposta acconto::" & CStr(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_ACCONTO")))
        '            Log.Debug("getCalcoloInteressi::differenzaimposta saldo::" & CStr(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_SALDO")))
        '            Log.Debug("getCalcoloInteressi::dblimporto::" & dblIMPORTO.ToString)
        '            Log.Debug("getCalcoloInteressi::dblimporto acconto::" & dblIMPORTOACCONTO.ToString)
        '            Log.Debug("getCalcoloInteressi::dblimporto saldo::" & dblIMPORTOSALDO.ToString)

        '            'dblIMPORTO = Math.Abs(dblIMPORTO)
        '            'dblIMPORTOACCONTO = Math.Abs(dblIMPORTOACCONTO)
        '            'dblIMPORTOSALDO = Math.Abs(dblIMPORTOSALDO)

        '            ''DA RICONTROLLARE E METTERE A POSTO
        '            'If intTipoBaseCalcolo = 1 Then
        '            '  If dblIMPORTO > 0 Then
        '            '    strCODVOCE = "3"
        '            '  ElseIf dblIMPORTO = 0 Then
        '            '    strCODVOCE = "4"
        '            '  End If
        '            'End If
        '            'strCODVOCE = "0"

        '            If dblIMPORTO <> 0 Then

        '                objDSGetTipologiaDatiInteressi = objDBOPENgovProvvedimentiSelect.GetTipologiaInteressiICI(strANNO, strCODENTE, StringConnectionProvv)
        '                If objDSGetTipologiaDatiInteressi.Tables(0).Rows.Count > 0 Then
        '                    If CType(objDSGetTipologiaDatiInteressi.Tables(0).Rows(intCountTassi).Item("INT_SALDO"), Double) = True Then
        '                        intTipoCalcoloInteressi = 1
        '                    ElseIf CType(objDSGetTipologiaDatiInteressi.Tables(0).Rows(intCountTassi).Item("INT_ACCONTO_SALDO"), Double) = True Then
        '                        intTipoCalcoloInteressi = 2
        '                    End If
        '                End If
        '                '**** 201809 - Cartelle Insoluti ***
        '                'objDSGetDatiInteressi = objDBOPENgovProvvedimentiSelect.GetInteressi(dblIMPORTO, strANNO, strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODENTE, strCODTIPOPROVVEDIMENTO, lngGenericID, objHashTable)
        '                objDSGetDatiInteressi = objDBOPENgovProvvedimentiSelect.GetInteressi(StringConnectionProvv, strCODENTE, strCODTRIBUTO, strANNO, strCODVOCE, lngGenericID, strCODTIPOPROVVEDIMENTO, objHashTable("COD_TIPO_PROCEDIMENTO"))
        '                For intCountTassi = 0 To objDSGetDatiInteressi.Tables(0).Rows.Count - 1

        '                    strCOD_VOCE = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi)("CODICEVOCE"), String)

        '                    dblIMPORTOINTERESSETOTALE = 0 : dblIMPORTOINTERESSEACCONTO = 0 : dblIMPORTOINTERESSESALDO = 0
        '                    dblIMPORTOINTERESSETOTALE_GG = 0 : dblIMPORTOINTERESSEACCONTO_GG = 0 : dblIMPORTOINTERESSESALDO_GG = 0

        '                    intSemestriACCONTO = 0 : intSemestriSALDO = 0
        '                    intGiorniACCONTO = 0 : intGiorniSALDO = 0

        '                    dblTassoInteressi = FormatNumber(CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("TASSO_ANNUALE"), Double), 2)
        '                    strDAL = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("DAL"), String)
        '                    strDAL_ORIG = strDAL
        '                    If Not IsDBNull(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("AL")) Then
        '                        strAL = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("AL"), String)
        '                    Else
        '                        strAL = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
        '                    End If

        '                    objDSGetGeneraleICI = New DataSet
        '                    objDSGetGeneraleICI = objDBOPENgovProvvedimentiSelect.GetGeneraleICI(objHashTable, strANNO)

        '                    If objDSGetGeneraleICI.Tables(0).Rows.Count > 0 Then
        '                        sDataVersamentoAcconto = objDSGetGeneraleICI.Tables(0).Rows(0)("DATA_VERSAMENTO_ACCONTO")
        '                        sDataVersamentoSaldo = objDSGetGeneraleICI.Tables(0).Rows(0)("DATA_VERSAMENTO_SALDO")
        '                    Else
        '                        sDataVersamentoAcconto = strANNO & objCostanti.DATA_VERSAMENTO_ACCONTO
        '                        sDataVersamentoSaldo = strANNO & objCostanti.DATA_VERSAMENTO_SALDO
        '                    End If

        '                    'Imposto la data di elaborazione (data odierna) per evitare ad es. di considerare
        '                    'il 10/06 o il 17/12 come se il semestre solare fosse già scattato e quindi tolgo un
        '                    'mese se la data è il 31/12 tolgo anche un giorno altrimenti verrebbe 31/11
        '                    'If Month(Date.Now) = 6 Or Month(Date.Now) = 12 Then
        '                    '    If Month(Date.Now) = 12 And Day(Date.Now) = 31 Then
        '                    '        'datamod = Day(Date.Now) - 1 & "/" & Month(Date.Now) - 1 & "/" & Year(Date.Now)
        '                    '        datamod = Year(Date.Now) & Format(Month(Date.Now) - 1, "00") & Format(Day(Date.Now) - 1, "00")
        '                    '    Else
        '                    '        'datamod = Day(Date.Now) & "/" & Month(Date.Now) - 1 & "/" & Year(Date.Now)
        '                    '        datamod = Year(Date.Now) & Format(Month(Date.Now) - 1, "00") & Format(Day(Date.Now), "00")
        '                    '    End If
        '                    'Else
        '                    'datamod = Day(Date.Now) & "/" & Month(Date.Now) & "/" & Year(Date.Now)
        '                    'datamod = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
        '                    If objHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
        '                        If Not IsNothing(objHashTable("DATA_ELABORAZIONE_PER_RETTIFICA")) Then
        '                            DataElaborazione = objHashTable("DATA_ELABORAZIONE_PER_RETTIFICA")
        '                        Else
        '                            DataElaborazione = Date.Now
        '                        End If
        '                    Else
        '                        DataElaborazione = Date.Now
        '                    End If
        '                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")

        '                    'End If

        '                    If intTipoBaseCalcolo = 1 Then
        '                        If strANNO >= Left(strDAL, 4) Then
        '                            'strDAL = strANNO & "0630"
        '                            strDAL = sDataVersamentoAcconto
        '                            'prevedere di prendere la data di acconto che sarà configurata
        '                            'nella tabella TP_GENERALE_ICI 
        '                            sal_acc_div = True
        '                        Else
        '                            sal_acc_div = False
        '                        End If
        '                        'End If

        '                        If DateDiff(DateInterval.Month, CDate(myUtility.GiraDataFromDB(datamod)), CDate(myUtility.GiraDataFromDB(strAL)), FirstDayOfWeek.Monday) < 0 Then
        '                            DATA_AL = (myUtility.GiraDataFromDB(strAL))
        '                            strDATA_AL = strAL
        '                        Else
        '                            DATA_AL = (myUtility.GiraDataFromDB(datamod))
        '                            strDATA_AL = datamod
        '                        End If

        '                        'Questa è la logica per determinare se il calcolo degli interessi
        '                        'deve essere giornaliero, semestrale oppure mezzo e mezzo...

        '                        'se DAL è maggiore di AL
        '                        If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL)), DATA_AL, FirstDayOfWeek.Monday) > 0 Then

        '                            'se DAL è maggiore di 31/12/2006...
        '                            '....calcolo solo gli interessi in GIORNI
        '                            If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) < 0 Then

        '                                Calcolo_Giorni(myUtility.GiraDataFromDB(strDAL), DATA_AL, myUtility.GiraDataFromDB(sDataVersamentoAcconto), myUtility.GiraDataFromDB(sDataVersamentoSaldo), intGiorniACCONTO, intGiorniSALDO, 0, sal_acc_div)

        '                                If dblIMPORTO <> 0 Then
        '                                    'calcolo interesse
        '                                    'unica soluzione
        '                                    If intTipoCalcoloInteressi = 1 Then                                     'SOLO SALDO

        '                                        dblIMPORTOINTERESSESALDO_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorniSALDO, 2)
        '                                        dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSESALDO_GG

        '                                    ElseIf intTipoCalcoloInteressi = 2 Then                                     'ACCONTO-SALDO

        '                                        If blnModalitaUS = True Then

        '                                            dblIMPORTOINTERESSEACCONTO_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorniACCONTO, 2)
        '                                            dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSEACCONTO_GG

        '                                        Else

        '                                            dblIMPORTOINTERESSEACCONTO_GG = FormatNumber((((dblIMPORTOACCONTO * dblTassoInteressi) / 100) / 365) * intGiorniACCONTO, 2)
        '                                            dblIMPORTOINTERESSESALDO_GG = FormatNumber((((dblIMPORTOSALDO * dblTassoInteressi) / 100) / 365) * intGiorniSALDO, 2)

        '                                            dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSEACCONTO_GG + dblIMPORTOINTERESSESALDO_GG

        '                                        End If

        '                                    End If

        '                                Else

        '                                    dblIMPORTOINTERESSETOTALE_GG = 0
        '                                    dblIMPORTOINTERESSEACCONTO_GG = 0
        '                                    dblIMPORTOINTERESSESALDO_GG = 0

        '                                End If

        '                            Else                            'altrimenti se DAL è minore di 31/12/2006...

        '                                '...e se AL minore di 31/12/2006...
        '                                '....calcolo solo gli interessi in SEMESTRI
        '                                If DateDiff(DateInterval.Day, DATA_AL, CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) >= 0 Then

        '                                    Calcolo_Semestri((myUtility.GiraDataFromDB(strDAL)), DATA_AL, intSemestriACCONTO, intSemestriSALDO, 0, sal_acc_div)

        '                                    If dblIMPORTO <> 0 Then
        '                                        'calcolo interesse
        '                                        'unica soluzione
        '                                        If intTipoCalcoloInteressi = 1 Then                                        'SOLO SALDO

        '                                            dblIMPORTOINTERESSESALDO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriSALDO, 2)
        '                                            'dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                            dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSESALDO

        '                                        ElseIf intTipoCalcoloInteressi = 2 Then                                        'ACCONTO-SALDO

        '                                            If blnModalitaUS = True Then

        '                                                dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                                dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSEACCONTO

        '                                            Else

        '                                                dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTOACCONTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                                dblIMPORTOINTERESSESALDO = FormatNumber(((dblIMPORTOSALDO * dblTassoInteressi) / 100) * intSemestriSALDO, 2)

        '                                                dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSEACCONTO + dblIMPORTOINTERESSESALDO
        '                                            End If

        '                                        End If

        '                                    Else

        '                                        dblIMPORTOINTERESSETOTALE = 0
        '                                        dblIMPORTOINTERESSEACCONTO = 0
        '                                        dblIMPORTOINTERESSESALDO = 0

        '                                    End If
        '                                Else
        '                                    '...e se AL maggiore di 31/12/2006...
        '                                    '....calcolo gli interessi in SEMESTRI da DAL al 31/12/2006
        '                                    '....calcolo gli interessi in GIORNI da 01/01/2007 a AL

        '                                    Calcolo_Semestri((myUtility.GiraDataFromDB(strDAL)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), intSemestriACCONTO, intSemestriSALDO, 0, sal_acc_div)

        '                                    If dblIMPORTO <> 0 Then
        '                                        'calcolo interesse
        '                                        'unica soluzione
        '                                        If intTipoCalcoloInteressi = 1 Then                                        'SOLO SALDO

        '                                            dblIMPORTOINTERESSESALDO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriSALDO, 2)
        '                                            'dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                            dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSESALDO

        '                                        ElseIf intTipoCalcoloInteressi = 2 Then                                        'ACCONTO-SALDO


        '                                            If blnModalitaUS = True Then

        '                                                dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                                dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSEACCONTO

        '                                            Else

        '                                                dblIMPORTOINTERESSEACCONTO = FormatNumber(((dblIMPORTOACCONTO * dblTassoInteressi) / 100) * intSemestriACCONTO, 2)
        '                                                dblIMPORTOINTERESSESALDO = FormatNumber(((dblIMPORTOSALDO * dblTassoInteressi) / 100) * intSemestriSALDO, 2)

        '                                                dblIMPORTOINTERESSETOTALE = dblIMPORTOINTERESSEACCONTO + dblIMPORTOINTERESSESALDO
        '                                            End If

        '                                        End If

        '                                    Else

        '                                        dblIMPORTOINTERESSETOTALE = 0
        '                                        dblIMPORTOINTERESSEACCONTO = 0
        '                                        dblIMPORTOINTERESSESALDO = 0

        '                                    End If
        '                                    'Modifica lobo da verificare
        '                                    sal_acc_div = False
        '                                    Calcolo_Giorni(CDate(COSTANTValue.CostantiProv.DATA_INIZIO_INTERESSI_GIORNALIERI), DATA_AL, myUtility.GiraDataFromDB(sDataVersamentoAcconto), myUtility.GiraDataFromDB(sDataVersamentoSaldo), intGiorniACCONTO, intGiorniSALDO, 0, sal_acc_div)

        '                                    If dblIMPORTO <> 0 Then
        '                                        'calcolo interesse
        '                                        'unica soluzione
        '                                        If intTipoCalcoloInteressi = 1 Then                                        'SOLO SALDO

        '                                            dblIMPORTOINTERESSESALDO_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorniSALDO, 2)
        '                                            dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSESALDO_GG

        '                                        ElseIf intTipoCalcoloInteressi = 2 Then                                        'ACCONTO-SALDO

        '                                            If blnModalitaUS = True Then

        '                                                dblIMPORTOINTERESSEACCONTO_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorniACCONTO, 2)
        '                                                dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSEACCONTO_GG

        '                                            Else

        '                                                dblIMPORTOINTERESSEACCONTO_GG = FormatNumber((((dblIMPORTOACCONTO * dblTassoInteressi) / 100) / 365) * intGiorniACCONTO, 2)
        '                                                dblIMPORTOINTERESSESALDO_GG = FormatNumber((((dblIMPORTOSALDO * dblTassoInteressi) / 100) / 365) * intGiorniSALDO, 2)

        '                                                dblIMPORTOINTERESSETOTALE_GG = dblIMPORTOINTERESSEACCONTO_GG + dblIMPORTOINTERESSESALDO_GG

        '                                            End If

        '                                        End If

        '                                    Else

        '                                        dblIMPORTOINTERESSETOTALE_GG = 0
        '                                        dblIMPORTOINTERESSEACCONTO_GG = 0
        '                                        dblIMPORTOINTERESSESALDO_GG = 0

        '                                    End If

        '                                End If

        '                            End If
        '                        Else

        '                            'se dal minore di al


        '                            dblIMPORTOINTERESSETOTALE_GG = 0
        '                            dblIMPORTOINTERESSEACCONTO_GG = 0
        '                            dblIMPORTOINTERESSESALDO_GG = 0

        '                            dblIMPORTOINTERESSETOTALE = 0
        '                            dblIMPORTOINTERESSEACCONTO = 0
        '                            dblIMPORTOINTERESSESALDO = 0

        '                            intSemestriACCONTO = 0
        '                            intSemestriSALDO = 0
        '                            intGiorniACCONTO = 0
        '                            intGiorniSALDO = 0

        '                        End If


        '                    ElseIf intTipoBaseCalcolo = 2 Then
        '                        'su importo tardivamente versato
        '                        'valido per fase 1

        '                        If strANNO >= Left(strDAL, 4) And strANNO <= Left(strAL, 4) Then

        '                            Dim importoTot, importoTotFinaleACC, importoTotFinaleSAL As Double

        '                            Dim intTIPOVERS As Integer
        '                            For ii As Integer = 0 To dsVersTard.Tables(0).Rows.Count - 1

        '                                Dim importoCalcolo As Double = FormatNumber(CType(dsVersTard.Tables(0).Rows(intCount).Item("IV"), Double), 2)
        '                                Dim GGritardo As Integer = FormatNumber(CType(dsVersTard.Tables(0).Rows(intCount).Item("GG"), Integer), 2)
        '                                intTIPOVERS = FormatNumber(CType(dsVersTard.Tables(0).Rows(intCount).Item("DI"), Integer), 2)

        '                                importoTot = (((importoCalcolo * dblTassoInteressi) / 100) * GGritardo) / 365

        '                                If intTIPOVERS = 1 Then
        '                                    importoTotFinaleACC = importoTotFinaleACC + importoTot
        '                                    intGiorniACCONTO = intGiorniACCONTO + GGritardo
        '                                Else
        '                                    importoTotFinaleSAL = importoTotFinaleSAL + importoTot
        '                                    intGiorniSALDO = intGiorniSALDO + GGritardo
        '                                End If

        '                            Next


        '                            dblIMPORTOINTERESSETOTALE_GG = importoTotFinaleACC + importoTotFinaleSAL
        '                            dblIMPORTOINTERESSEACCONTO_GG = importoTotFinaleACC
        '                            dblIMPORTOINTERESSESALDO_GG = importoTotFinaleSAL

        '                        End If

        '                    End If

        '                    'mantenere questa procedura e implementare quella del calcolo in giorni.
        '                    'prevedere la scelta da front end
        '                    'testare la il campo nella tabella TP_GENERALE_ICI                            

        '                    dblIMPORTOFINALEINTERESSI = dblIMPORTOFINALEINTERESSI + dblIMPORTOINTERESSETOTALE
        '                    dblIMPORTOFINALEINTERESSI_GG = dblIMPORTOFINALEINTERESSI_GG + dblIMPORTOINTERESSETOTALE_GG
        '                    Log.Debug("dblIMPORTOINTERESSETOTALE_GG::" & dblIMPORTOINTERESSETOTALE_GG.ToString)
        '                    Dim Row1 As DataRow
        '                    Row1 = newTable.NewRow()

        '                    Row1.Item("COD_ENTE") = strCODENTE
        '                    Row1.Item("ANNO") = strANNO
        '                    Row1.Item("COD_VOCE") = strCOD_VOCE
        '                    Row1.Item("IMPORTO") = dblIMPORTOINTERESSETOTALE
        '                    Row1.Item("IMPORTO_GIORNI") = dblIMPORTOINTERESSETOTALE_GG
        '                    Row1.Item("IMPORTO_RIDOTTO") = 0
        '                    Row1.Item("ACCONTO") = dblIMPORTOINTERESSEACCONTO
        '                    Row1.Item("SALDO") = dblIMPORTOINTERESSESALDO
        '                    Row1.Item("ACCONTO_GIORNI") = dblIMPORTOINTERESSEACCONTO_GG
        '                    Row1.Item("SALDO_GIORNI") = dblIMPORTOINTERESSESALDO_GG
        '                    Row1.Item("DATA_INIZIO") = strDAL_ORIG                    'strDAL
        '                    Row1.Item("DATA_FINE") = strDATA_AL
        '                    Row1.Item("N_SEMESTRI_ACCONTO") = intSemestriACCONTO
        '                    Row1.Item("N_SEMESTRI_SALDO") = intSemestriSALDO
        '                    Row1.Item("N_GIORNI_ACCONTO") = intGiorniACCONTO
        '                    Row1.Item("N_GIORNI_SALDO") = intGiorniSALDO
        '                    Row1.Item("TASSO") = dblTassoInteressi
        '                    Row1.Item("GENERIC_ID") = lngGenericID
        '                    'Solo x accertamenti
        '                    Row1.Item("ID_LEGAME") = idLegame
        '                    Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO

        '                    newTable.Rows.Add(Row1)

        '                Next

        '                objDSGetCalcoloInteressi.Tables.Add(newTable)

        '            End If
        '            'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
        '            objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("IMPORTO_INTERESSI") = dblIMPORTOFINALEINTERESSI + dblIMPORTOFINALEINTERESSI_GG
        '            objSituazioneBasePerSanzInt.AcceptChanges()

        '        Next

        '        culture = New CultureInfo("en-US", True)

        '        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")

        '        Return objDSGetCalcoloInteressi

        '    Catch ex As Exception
        '        Log.Debug("getCalcoloInteressi::si è verificato il seguente errore::", ex)
        '        Throw New Exception("Function::getCalcoloInteressi::COMPlusService:: " & ex.Message)
        '    End Try
        'End Function
        'Public Function getCalcoloInteressiTARSU(ByRef objSituazioneBasePerSanzInt As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODCAPITOLO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, ByVal intTipoBaseCalcolo As Integer, ByVal objHashTable As Hashtable, ByVal idLegame As Integer, ByVal dsVersTard As DataSet) As DataSet
        '    'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
        '    'intTipoBaseCalcolo=1 --> DIFFERENZA IMPOSTA 
        '    'intTipoBaseCalcolo=2 --> IMPOSTA VERSATA (VERSAMENTO TARDIVO)

        '    Try

        '        Dim culture As IFormatProvider
        '        culture = New CultureInfo("it-IT", True)
        '        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")

        '        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
        '        Dim objDSGetDatiInteressi As DataSet
        '        Dim objDSGetCalcoloInteressi As New DataSet
        '        Dim objDSGetGeneraleTARSU As DataSet
        '        Dim strDAL_Interesse, strAL_Interesse As String
        '        Dim dblTassoInteressi As Double
        '        Dim intSemestri As Integer
        '        Dim intGiorni As Integer

        '        Dim strANNO As String               'anno di liquidazione (Pre Accertamento)
        '        Dim dblIMPORTO As Double                'importo scaturito dal confronto tra versato e dichiarato
        '        Dim dblIMPORTOINTERESSETOTALE As Double
        '        Dim dblIMPORTOINTERESSETOTALE_GG As Double
        '        Dim dblIMPORTOFINALEINTERESSI, dblIMPORTOFINALEINTERESSI_GG As Double
        '        Dim sal_acc_div As Boolean
        '        Dim DATA_AL As Date
        '        Dim datamod, strDATA_AL As String
        '        Dim DataElaborazione As Date
        '        Dim intTipoCalcoloInteressi As Integer              'intTipoCalcoloInteressi=1 ACCONTO - intTipoCalcoloInteressi=2 SALDO - intTipoCalcoloInteressi=3 UNICA SOLUZIONE

        '        Dim sDataDecorrenzaScadenza As String

        '        Dim strCOD_VOCE As String
        '        Dim blnModalitaUS As Boolean = False

        '        'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI
        '        Dim newTable As DataTable
        '        newTable = New DataTable("INTERESSI")

        '        Dim NewColumn As DataColumn

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_ENTE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ANNO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = "0"
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_VOCE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_RIDOTTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_INIZIO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_FINE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_TOTALI"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "TASSO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "GENERIC_ID"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ID_LEGAME"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "MOTIVAZIONI"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = System.DBNull.Value
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_TOTALI"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

        '        '*** 20130801 - accertamento OSAP ***
        '        If Not IsNothing(objHashTable("ID_FASE")) Then
        '            lngGenericID = objHashTable("ID_FASE")
        '        End If
        '        '*** ***
        '        If objSituazioneBasePerSanzInt.Tables.Count > 0 Then
        '            Dim x As Integer
        '            For Each myRow As DataRow In objSituazioneBasePerSanzInt.Tables(0).Rows
        '                x += 1
        '                Log.Debug("ciclo su objSituazioneBasePerSanzInt sono " + x.ToString)
        '                dblIMPORTOFINALEINTERESSI = 0 : dblIMPORTOFINALEINTERESSI_GG = 0

        '                strANNO = CType(myRow.Item("ANNO"), Double)
        '                dblIMPORTO = FormatNumber(CType(myRow.Item("DIFFERENZA_IMPOSTA_TOTALE"), Double), 2)
        '                blnModalitaUS = CType(myRow.Item("FLAG_MODALITA_UNICA_SOLUZIONE"), Boolean)

        '                objDSGetGeneraleTARSU = New DataSet
        '                '*** 20130801 - accertamento OSAP ***
        '                objDSGetGeneraleTARSU = objDBOPENgovProvvedimentiSelect.GetTipologiaInteressiTARSU(strANNO, strCODTRIBUTO, strCODENTE, StringConnectionProvv)
        '                '*** ***
        '                If objDSGetGeneraleTARSU.Tables.Count > 0 Then
        '                    If objDSGetGeneraleTARSU.Tables(0).Rows.Count > 0 Then
        '                        Log.Debug("ho scadenza interessi")
        '                        sDataDecorrenzaScadenza = myUtility.GiraData(objDSGetGeneraleTARSU.Tables(0).Rows(0)("DATA_SCADENZA"))
        '                        If dblIMPORTO <> 0 Then
        '                            Log.Debug("ho importo " + dblIMPORTO.ToString)
        '                            intTipoCalcoloInteressi = 1
        '                            Log.Debug("devo richiamare getinteressi per tarsu")
        '                            '**** 201809 - Cartelle Insoluti ***
        '                            objDSGetDatiInteressi = objDBOPENgovProvvedimentiSelect.GetInteressi(StringConnectionProvv, strCODENTE, strCODTRIBUTO, strANNO, strCODVOCE, lngGenericID, strCODTIPOPROVVEDIMENTO, objHashTable("COD_TIPO_PROCEDIMENTO"))
        '                            For Each drTassi As DataRow In objDSGetDatiInteressi.Tables(0).Rows

        '                                strCOD_VOCE = CType(drTassi("CODICEVOCE"), String)
        '                                '*** 201408026 - l'anno di paragone deve essere l'anno di scadenza del tributo e non l'anno di imposta ***
        '                                strANNO = sDataDecorrenzaScadenza.Substring(0, 4)
        '                                '*** ***
        '                                dblIMPORTOINTERESSETOTALE = 0
        '                                dblIMPORTOINTERESSETOTALE_GG = 0

        '                                intSemestri = 0 : intGiorni = 0

        '                                dblTassoInteressi = FormatNumber(CType(drTassi.Item("TASSO_ANNUALE"), Double), 2)
        '                                strDAL_Interesse = CType(drTassi.Item("DAL"), String)
        '                                If Not IsDBNull(drTassi.Item("AL")) Then
        '                                    strAL_Interesse = CType(drTassi.Item("AL"), String)
        '                                Else
        '                                    strAL_Interesse = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
        '                                End If

        '                                'Imposto la data di elaborazione (data odierna) per evitare ad es. di considerare
        '                                'il 10/06 o il 17/12 come se il semestre solare fosse già scattato e quindi tolgo un
        '                                'mese se la data è il 31/12 tolgo anche un giorno altrimenti verrebbe 31/11
        '                                If objHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
        '                                    DataElaborazione = objHashTable("DATA_ELABORAZIONE_PER_RETTIFICA")
        '                                Else
        '                                    DataElaborazione = Date.Now
        '                                End If

        '                                If Month(DataElaborazione) = 6 Or Month(DataElaborazione) = 12 Then
        '                                    If Month(DataElaborazione) = 12 And Day(DataElaborazione) = 31 Then
        '                                        datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione) - 1, "00")
        '                                    Else
        '                                        datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione), "00")
        '                                    End If
        '                                Else
        '                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")
        '                                End If

        '                                If intTipoBaseCalcolo = 1 Then
        '                                    If strANNO >= Left(strDAL_Interesse, 4) Then
        '                                        strDAL_Interesse = sDataDecorrenzaScadenza
        '                                        'prevedere di prendere la data di acconto che sarà configurata nella tabella TP_GENERALE_ICI 
        '                                        sal_acc_div = True
        '                                    Else
        '                                        sal_acc_div = False
        '                                    End If

        '                                    If DateDiff(DateInterval.Month, CDate(myUtility.GiraDataFromDB(datamod)), CDate(myUtility.GiraDataFromDB(strAL_Interesse)), FirstDayOfWeek.Monday) < 0 Then
        '                                        DATA_AL = (myUtility.GiraDataFromDB(strAL_Interesse))
        '                                        strDATA_AL = strAL_Interesse
        '                                    Else
        '                                        DATA_AL = (myUtility.GiraDataFromDB(datamod))
        '                                        strDATA_AL = datamod
        '                                    End If
        '                                    Log.Debug("getCalcoloInteressiTARSU.parametri calcolo->strDAL_Interesse=" + strDAL_Interesse + ",DATA_AL=" + DATA_AL.ToString + ",dblIMPORTO=" + dblIMPORTO.ToString + ",dblTassoInteressi=" + dblTassoInteressi.ToString + ",intTipoCalcoloInteressi=" + intTipoCalcoloInteressi.ToString)
        '                                    'Questa è la logica per determinare se il calcolo degli interessi deve essere giornaliero, semestrale oppure mezzo e mezzo...

        '                                    'se DAL è maggiore di AL
        '                                    If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, FirstDayOfWeek.Monday) > 0 Then
        '                                        'If DateDiff(DateInterval.Day, CDate(strDAL_Interesse), DATA_AL, FirstDayOfWeek.Monday) > 0 Then

        '                                        'se DAL è maggiore di 31/12/2006...
        '                                        '....calcolo solo gli interessi in GIORNI
        '                                        If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) < 0 Then
        '                                            Calcolo_Giorni(myUtility.GiraDataFromDB(strDAL_Interesse), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), intGiorni, 0, sal_acc_div)
        '                                            Log.Debug("getCalcoloInteressiTARSU.parametri calcolo->intGiorni=" + intGiorni.ToString)
        '                                            If dblIMPORTO <> 0 Then
        '                                                'calcolo interesse
        '                                                'unica soluzione
        '                                                If intTipoCalcoloInteressi = 1 Then
        '                                                    dblIMPORTOINTERESSETOTALE_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                                End If
        '                                            Else
        '                                                dblIMPORTOINTERESSETOTALE_GG = 0
        '                                                dblIMPORTOINTERESSETOTALE = 0
        '                                            End If
        '                                        Else                                        'altrimenti se DAL è minore di 31/12/2006...

        '                                            '...e se AL minore di 31/12/2006...
        '                                            '....calcolo solo gli interessi in SEMESTRI
        '                                            If DateDiff(DateInterval.Day, DATA_AL, CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), FirstDayOfWeek.Monday) >= 0 Then
        '                                                Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, intSemestri, 0, sal_acc_div)
        '                                                If dblIMPORTO <> 0 Then
        '                                                    'calcolo interesse
        '                                                    'unica soluzione
        '                                                    If intTipoCalcoloInteressi = 1 Then                                                 'SOLO SALDO
        '                                                        dblIMPORTOINTERESSETOTALE = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestri, 2)
        '                                                    End If
        '                                                Else
        '                                                    dblIMPORTOINTERESSETOTALE = 0
        '                                                End If
        '                                            Else
        '                                                '...e se AL maggiore di 31/12/2006...
        '                                                '....calcolo gli interessi in SEMESTRI da DAL al 31/12/2006
        '                                                '....calcolo gli interessi in GIORNI da 01/01/2007 a AL
        '                                                Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(myUtility.GiraDataFromDB(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI)), intSemestri, 0, sal_acc_div)
        '                                                If dblIMPORTO <> 0 Then
        '                                                    'calcolo interesse
        '                                                    'unica soluzione
        '                                                    If intTipoCalcoloInteressi = 1 Then                                                 'SOLO SALDO
        '                                                        dblIMPORTOINTERESSETOTALE = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestri, 2)
        '                                                    End If
        '                                                Else
        '                                                    dblIMPORTOINTERESSETOTALE = 0
        '                                                End If
        '                                                'Modifica lobo da verificare
        '                                                sal_acc_div = False
        '                                                Calcolo_Giorni(CDate(COSTANTValue.CostantiProv.DATA_INIZIO_INTERESSI_GIORNALIERI), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), intGiorni, 0, sal_acc_div)
        '                                                If dblIMPORTO <> 0 Then
        '                                                    'calcolo interesse
        '                                                    'unica soluzione
        '                                                    If intTipoCalcoloInteressi = 1 Then
        '                                                        dblIMPORTOINTERESSETOTALE_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                                    End If
        '                                                Else
        '                                                    dblIMPORTOINTERESSETOTALE_GG = 0
        '                                                    dblIMPORTOINTERESSETOTALE = 0
        '                                                End If
        '                                            End If
        '                                        End If
        '                                    Else
        '                                        'se dal minore di al
        '                                        dblIMPORTOINTERESSETOTALE_GG = 0
        '                                        dblIMPORTOINTERESSETOTALE = 0

        '                                        intSemestri = 0
        '                                        intGiorni = 0
        '                                    End If
        '                                End If
        '                                'mantenere questa procedura e implementare quella del calcolo in giorni. prevedere la scelta da front end testare la il campo nella tabella TP_GENERALE_ICI                            
        '                                dblIMPORTOFINALEINTERESSI = dblIMPORTOFINALEINTERESSI + dblIMPORTOINTERESSETOTALE
        '                                dblIMPORTOFINALEINTERESSI_GG = dblIMPORTOFINALEINTERESSI_GG + dblIMPORTOINTERESSETOTALE_GG

        '                                Dim Row1 As DataRow
        '                                Row1 = newTable.NewRow()

        '                                Row1.Item("COD_ENTE") = strCODENTE
        '                                Row1.Item("ANNO") = strANNO
        '                                Row1.Item("COD_VOCE") = strCOD_VOCE
        '                                Row1.Item("IMPORTO") = dblIMPORTOINTERESSETOTALE
        '                                Row1.Item("IMPORTO_GIORNI") = dblIMPORTOINTERESSETOTALE_GG
        '                                Row1.Item("IMPORTO_RIDOTTO") = 0
        '                                Row1.Item("ACCONTO") = 0
        '                                Row1.Item("SALDO") = 0
        '                                Row1.Item("ACCONTO_GIORNI") = 0
        '                                Row1.Item("SALDO_GIORNI") = 0
        '                                Row1.Item("DATA_INIZIO") = strDAL_Interesse
        '                                Row1.Item("DATA_FINE") = strDATA_AL
        '                                Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                                Row1.Item("N_SEMESTRI_SALDO") = intSemestri
        '                                Row1.Item("N_SEMESTRI_TOTALI") = intSemestri
        '                                Row1.Item("N_GIORNI_ACCONTO") = 0
        '                                Row1.Item("N_GIORNI_SALDO") = intGiorni
        '                                Row1.Item("N_GIORNI_TOTALI") = intGiorni
        '                                Row1.Item("TASSO") = dblTassoInteressi
        '                                Row1.Item("GENERIC_ID") = lngGenericID
        '                                'Solo x accertamenti
        '                                Row1.Item("ID_LEGAME") = idLegame
        '                                Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO

        '                                newTable.Rows.Add(Row1)
        '                            Next
        '                            objDSGetCalcoloInteressi.Tables.Add(newTable)
        '                        Else
        '                            Log.Debug("non ho importo")
        '                        End If
        '                    Else
        '                        Log.Debug("non ho scadenza interessi")
        '                        Dim Row1 As DataRow
        '                        Row1 = newTable.NewRow()

        '                        Row1.Item("COD_ENTE") = strCODENTE
        '                        Row1.Item("ANNO") = strANNO
        '                        Row1.Item("COD_VOCE") = 0
        '                        Row1.Item("IMPORTO") = dblIMPORTOINTERESSETOTALE
        '                        Row1.Item("IMPORTO_GIORNI") = dblIMPORTOINTERESSETOTALE_GG
        '                        Row1.Item("IMPORTO_RIDOTTO") = 0
        '                        Row1.Item("ACCONTO") = 0
        '                        Row1.Item("SALDO") = 0
        '                        Row1.Item("ACCONTO_GIORNI") = 0
        '                        Row1.Item("SALDO_GIORNI") = 0
        '                        Row1.Item("DATA_INIZIO") = ""
        '                        Row1.Item("DATA_FINE") = ""
        '                        Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                        Row1.Item("N_SEMESTRI_SALDO") = 0
        '                        Row1.Item("N_SEMESTRI_TOTALI") = 0
        '                        Row1.Item("N_GIORNI_ACCONTO") = 0
        '                        Row1.Item("N_GIORNI_SALDO") = 0
        '                        Row1.Item("N_GIORNI_TOTALI") = 0
        '                        Row1.Item("TASSO") = 0
        '                        Row1.Item("GENERIC_ID") = lngGenericID
        '                        'Solo x accertamenti
        '                        Row1.Item("ID_LEGAME") = idLegame
        '                        Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO

        '                        newTable.Rows.Add(Row1)

        '                        objDSGetCalcoloInteressi.Tables.Add(newTable)
        '                    End If
        '                End If
        '                'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
        '                myRow.Item("IMPORTO_INTERESSI") = dblIMPORTOFINALEINTERESSI + dblIMPORTOFINALEINTERESSI_GG
        '                objSituazioneBasePerSanzInt.AcceptChanges()
        '            Next
        '        End If
        '        Return objDSGetCalcoloInteressi
        '    Catch ex As Exception
        '        Log.Debug("getCalcoloInteressiTARSU.errore::", ex)
        '        Throw New Exception("Function::getCalcoloInteressi::COMPlusService:: " & ex.Message)
        '    End Try
        'End Function
        'Public Function getCalcoloInteressiTARSU(ByRef objSituazioneBasePerSanzInt As DataSet, ByVal strCODTRIBUTO As String, ByVal strCODCAPITOLO As String, ByVal strCODVOCE As String, ByVal strCODENTE As String, ByVal strCODTIPOPROVVEDIMENTO As String, ByVal lngGenericID As Long, ByVal intTipoBaseCalcolo As Integer, ByVal objHashTable As Hashtable, Optional ByVal idLegame As Integer = -1, Optional ByVal dsVersTard As DataSet = Nothing) As DataSet
        '    'lngGenericID è ID_FASE per LIQUIDAZIONE (Pre Accertamento), ID_IMMOBILE per ACCERTAMENTO
        '    'intTipoBaseCalcolo=1 --> DIFFERENZA IMPOSTA 
        '    'intTipoBaseCalcolo=2 --> IMPOSTA VERSATA (VERSAMENTO TARDIVO)

        '    Try

        '        Dim culture As IFormatProvider
        '        culture = New CultureInfo("it-IT", True)
        '        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")

        '        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect          
        '        Dim objDSGetDatiInteressi As DataSet
        '        Dim objDSGetCalcoloInteressi As New DataSet
        '        Dim objDSGetGeneraleTARSU As DataSet
        '        Dim intCount, intCountTassi As Integer
        '        Dim strDAL_Interesse, strAL_Interesse As String
        '        Dim dblTassoInteressi As Double
        '        Dim intSemestri As Integer
        '        Dim intGiorni As Integer

        '        Dim strANNO As String               'anno di liquidazione (Pre Accertamento)
        '        Dim dblIMPORTO As Double                'importo scaturito dal confronto tra versato e dichiarato
        '        Dim dblIMPORTOINTERESSETOTALE As Double
        '        Dim dblIMPORTOINTERESSETOTALE_GG As Double
        '        Dim dblIMPORTOFINALEINTERESSI, dblIMPORTOFINALEINTERESSI_GG As Double
        '        Dim sal_acc_div As Boolean
        '        Dim DATA_AL As Date
        '        Dim datamod, strDATA_AL As String
        '        Dim DataElaborazione As Date
        '        Dim intTipoCalcoloInteressi As Integer              'intTipoCalcoloInteressi=1 ACCONTO - intTipoCalcoloInteressi=2 SALDO - intTipoCalcoloInteressi=3 UNICA SOLUZIONE

        '        'Dim sDataVersamentoTARSU As String
        '        Dim sDataDecorrenzaScadenza As String
        '        'Dim sDataVersamentoSaldo As String

        '        Dim strCOD_VOCE As String
        '        Dim blnModalitaUS As Boolean = False

        '        'creo la struttura del dataset per tabella DETTAGLIO_VOCI_LIQUIDAZIONI

        '        Dim newTable As DataTable
        '        newTable = New DataTable("INTERESSI")

        '        Dim NewColumn As DataColumn

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_ENTE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ANNO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = "0"
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_VOCE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "IMPORTO_RIDOTTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ACCONTO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "SALDO_GIORNI"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_INIZIO"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "DATA_FINE"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_SEMESTRI_TOTALI"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "TASSO"
        '        NewColumn.DataType = System.Type.GetType("System.Double")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "GENERIC_ID"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "ID_LEGAME"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = False
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "MOTIVAZIONI"
        '        NewColumn.DataType = System.Type.GetType("System.String")
        '        NewColumn.DefaultValue = System.DBNull.Value
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_ACCONTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_SALDO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "N_GIORNI_TOTALI"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        NewColumn = New DataColumn
        '        NewColumn.ColumnName = "COD_TIPO_PROVVEDIMENTO"
        '        NewColumn.DataType = System.Type.GetType("System.Int64")
        '        NewColumn.DefaultValue = 0
        '        newTable.Columns.Add(NewColumn)

        '        objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect           

        '        '*** 20130801 - accertamento OSAP ***
        '        If Not IsNothing(objHashTable("ID_FASE")) Then
        '            lngGenericID = objHashTable("ID_FASE")
        '        End If
        '        '*** ***
        '        If objSituazioneBasePerSanzInt.Tables.Count > 0 Then
        '            For intCount = 0 To objSituazioneBasePerSanzInt.Tables(0).Rows.Count - 1
        '                Log.Debug("ciclo su objSituazioneBasePerSanzInt sono " + intCount.ToString)
        '                dblIMPORTOFINALEINTERESSI = 0 : dblIMPORTOFINALEINTERESSI_GG = 0

        '                strANNO = CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("ANNO"), Double)
        '                dblIMPORTO = FormatNumber(CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("DIFFERENZA_IMPOSTA_TOTALE"), Double), 2)
        '                blnModalitaUS = CType(objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("FLAG_MODALITA_UNICA_SOLUZIONE"), Boolean)

        '                'dblIMPORTO = Math.Abs(dblIMPORTO)
        '                'dblIMPORTOACCONTO = Math.Abs(dblIMPORTOACCONTO)
        '                'dblIMPORTOSALDO = Math.Abs(dblIMPORTOSALDO)

        '                ''DA RICONTROLLARE E METTERE A POSTO
        '                'If intTipoBaseCalcolo = 1 Then
        '                '  If dblIMPORTO > 0 Then
        '                '    strCODVOCE = "3"
        '                '  ElseIf dblIMPORTO = 0 Then
        '                '    strCODVOCE = "4"
        '                '  End If
        '                'End If
        '                'strCODVOCE = "0"

        '                objDSGetGeneraleTARSU = New DataSet
        '                '*** 20130801 - accertamento OSAP ***
        '                'objDSGetGeneraleTARSU = objDBOPENgovProvvedimentiSelect.GetTipologiaInteressiTARSU(strANNO, strCODENTE, objHashTable)
        '                objDSGetGeneraleTARSU = objDBOPENgovProvvedimentiSelect.GetTipologiaInteressiTARSU(strANNO, strCODTRIBUTO, strCODENTE, StringConnectionProvv)
        '                '*** ***
        '                If objDSGetGeneraleTARSU.Tables(0).Rows.Count > 0 Then
        '                    Log.Debug("ho scadenza interessi")
        '                    sDataDecorrenzaScadenza = myUtility.GiraData(objDSGetGeneraleTARSU.Tables(0).Rows(0)("DATA_SCADENZA"))
        '                    'Else
        '                    '    sDataVersamentoAcconto = strANNO & objCostanti.DATA_VERSAMENTO_ACCONTO
        '                    ''    sDataVersamentoSaldo = strANNO & objCostanti.DATA_VERSAMENTO_SALDO
        '                    'sDataDecorrenzaScadenza = Utility.GiraDataFromDB(sDataDecorrenzaScadenza)
        '                    If dblIMPORTO <> 0 Then
        '                        Log.Debug("ho importo " + dblIMPORTO.ToString)
        '                        intTipoCalcoloInteressi = 1
        '                        Log.Debug("devo richiamare getinteressi")
        '                        '**** 201809 - Cartelle Insoluti ***
        '                        'objDSGetDatiInteressi = objDBOPENgovProvvedimentiSelect.GetInteressi(dblIMPORTO, strANNO, strCODTRIBUTO, strCODCAPITOLO, strCODVOCE, strCODENTE, strCODTIPOPROVVEDIMENTO, lngGenericID, objHashTable)
        '                        objDSGetDatiInteressi = objDBOPENgovProvvedimentiSelect.GetInteressi(StringConnectionProvv, strCODENTE, strCODTRIBUTO, strANNO, strCODVOCE, lngGenericID, strCODTIPOPROVVEDIMENTO, objHashTable("COD_TIPO_PROCEDIMENTO"))
        '                        For intCountTassi = 0 To objDSGetDatiInteressi.Tables(0).Rows.Count - 1

        '                            strCOD_VOCE = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi)("CODICEVOCE"), String)
        '                            '*** 201408026 - l'anno di paragone deve essere l'anno di scadenza del tributo e non l'anno di imposta ***
        '                            strANNO = sDataDecorrenzaScadenza.Substring(0, 4)
        '                            '*** ***
        '                            dblIMPORTOINTERESSETOTALE = 0
        '                            dblIMPORTOINTERESSETOTALE_GG = 0

        '                            intSemestri = 0 : intGiorni = 0

        '                            dblTassoInteressi = FormatNumber(CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("TASSO_ANNUALE"), Double), 2)
        '                            strDAL_Interesse = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("DAL"), String)
        '                            If Not IsDBNull(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("AL")) Then
        '                                strAL_Interesse = CType(objDSGetDatiInteressi.Tables(0).Rows(intCountTassi).Item("AL"), String)
        '                            Else
        '                                strAL_Interesse = Year(Date.Now) & Format(Month(Date.Now), "00") & Format(Day(Date.Now), "00")
        '                            End If

        '                            'Imposto la data di elaborazione (data odierna) per evitare ad es. di considerare
        '                            'il 10/06 o il 17/12 come se il semestre solare fosse già scattato e quindi tolgo un
        '                            'mese se la data è il 31/12 tolgo anche un giorno altrimenti verrebbe 31/11
        '                            If objHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
        '                                DataElaborazione = objHashTable("DATA_ELABORAZIONE_PER_RETTIFICA")
        '                            Else
        '                                DataElaborazione = Date.Now
        '                            End If

        '                            If Month(DataElaborazione) = 6 Or Month(DataElaborazione) = 12 Then
        '                                If Month(DataElaborazione) = 12 And Day(DataElaborazione) = 31 Then
        '                                    'datamod = Day(DataElaborazione) - 1 & "/" & Month(DataElaborazione) - 1 & "/" & Year(DataElaborazione)
        '                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione) - 1, "00")
        '                                Else
        '                                    'datamod = Day(DataElaborazione) & "/" & Month(DataElaborazione) - 1 & "/" & Year(DataElaborazione)
        '                                    datamod = Year(DataElaborazione) & Format(Month(DataElaborazione) - 1, "00") & Format(Day(DataElaborazione), "00")
        '                                End If
        '                            Else
        '                                'datamod = Day(DataElaborazione) & "/" & Month(DataElaborazione) & "/" & Year(DataElaborazione)
        '                                datamod = Year(DataElaborazione) & Format(Month(DataElaborazione), "00") & Format(Day(DataElaborazione), "00")
        '                            End If

        '                            If intTipoBaseCalcolo = 1 Then
        '                                If strANNO >= Left(strDAL_Interesse, 4) Then
        '                                    'strDAL = strANNO & "0630"
        '                                    strDAL_Interesse = sDataDecorrenzaScadenza
        '                                    'prevedere di prendere la data di acconto che sarà configurata
        '                                    'nella tabella TP_GENERALE_ICI 
        '                                    sal_acc_div = True
        '                                Else
        '                                    sal_acc_div = False
        '                                End If
        '                                'End If

        '                                If DateDiff(DateInterval.Month, CDate(myUtility.GiraDataFromDB(datamod)), CDate(myUtility.GiraDataFromDB(strAL_Interesse)), FirstDayOfWeek.Monday) < 0 Then
        '                                    DATA_AL = (myUtility.GiraDataFromDB(strAL_Interesse))
        '                                    strDATA_AL = strAL_Interesse
        '                                Else
        '                                    DATA_AL = (myUtility.GiraDataFromDB(datamod))
        '                                    strDATA_AL = datamod
        '                                End If
        '                                Log.Debug("getCalcoloInteressiTARSU.parametri calcolo->strDAL_Interesse=" + strDAL_Interesse + ",DATA_AL=" + DATA_AL.ToString + ",dblIMPORTO=" + dblIMPORTO.ToString + ",dblTassoInteressi=" + dblTassoInteressi.ToString + ",intTipoCalcoloInteressi=" + intTipoCalcoloInteressi.ToString)

        '                                'Questa è la logica per determinare se il calcolo degli interessi
        '                                'deve essere giornaliero, semestrale oppure mezzo e mezzo...

        '                                'se DAL è maggiore di AL
        '                                If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, FirstDayOfWeek.Monday) > 0 Then
        '                                    'If DateDiff(DateInterval.Day, CDate(strDAL_Interesse), DATA_AL, FirstDayOfWeek.Monday) > 0 Then

        '                                    'se DAL è maggiore di 31/12/2006...
        '                                    '....calcolo solo gli interessi in GIORNI
        '                                    If DateDiff(DateInterval.Day, CDate(myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI), FirstDayOfWeek.Monday) < 0 Then
        '                                        '    If DateDiff(DateInterval.Day, CDate(strDAL_Interesse), CDate(COSTANTValue.Costanti.DATA_FINE_INTERESSI_SEMESTRI), FirstDayOfWeek.Monday) < 0 Then

        '                                        Calcolo_GiorniTARSU(myUtility.GiraDataFromDB(strDAL_Interesse), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), intGiorni, 0, sal_acc_div)
        '                                        'Calcolo_GiorniTARSU(strDAL_Interesse, DATA_AL, sDataDecorrenzaScadenza, intGiorni, 0, sal_acc_div)
        '                                        Log.Debug("getCalcoloInteressiTARSU.parametri calcolo->intGiorni=" + intGiorni.ToString)
        '                                        If dblIMPORTO <> 0 Then
        '                                            'calcolo interesse
        '                                            'unica soluzione
        '                                            If intTipoCalcoloInteressi = 1 Then
        '                                                dblIMPORTOINTERESSETOTALE_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                                'dblIMPORTOINTERESSETOTALE = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                            End If
        '                                        Else
        '                                            dblIMPORTOINTERESSETOTALE_GG = 0
        '                                            dblIMPORTOINTERESSETOTALE = 0
        '                                        End If

        '                                    Else                                        'altrimenti se DAL è minore di 31/12/2006...

        '                                        '...e se AL minore di 31/12/2006...
        '                                        '....calcolo solo gli interessi in SEMESTRI
        '                                        If DateDiff(DateInterval.Day, DATA_AL, CDate(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI), FirstDayOfWeek.Monday) >= 0 Then
        '                                            Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), DATA_AL, intSemestri, 0, sal_acc_div)
        '                                            'Calcolo_SemestriTARSU(strDAL_Interesse, DATA_AL, intSemestri, 0, sal_acc_div)

        '                                            If dblIMPORTO <> 0 Then
        '                                                'calcolo interesse
        '                                                'unica soluzione
        '                                                If intTipoCalcoloInteressi = 1 Then                                                 'SOLO SALDO
        '                                                    dblIMPORTOINTERESSETOTALE = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestri, 2)
        '                                                End If

        '                                            Else
        '                                                dblIMPORTOINTERESSETOTALE = 0
        '                                            End If
        '                                        Else
        '                                            '...e se AL maggiore di 31/12/2006...
        '                                            '....calcolo gli interessi in SEMESTRI da DAL al 31/12/2006
        '                                            '....calcolo gli interessi in GIORNI da 01/01/2007 a AL

        '                                            Calcolo_SemestriTARSU((myUtility.GiraDataFromDB(strDAL_Interesse)), CDate(COSTANTValue.CostantiProv.DATA_FINE_INTERESSI_SEMESTRI), intSemestri, 0, sal_acc_div)
        '                                            'Calcolo_SemestriTARSU(strDAL_Interesse, CDate(COSTANTValue.Costanti.DATA_FINE_INTERESSI_SEMESTRI), intSemestri, 0, sal_acc_div)

        '                                            If dblIMPORTO <> 0 Then
        '                                                'calcolo interesse
        '                                                'unica soluzione
        '                                                If intTipoCalcoloInteressi = 1 Then                                                 'SOLO SALDO
        '                                                    dblIMPORTOINTERESSETOTALE = FormatNumber(((dblIMPORTO * dblTassoInteressi) / 100) * intSemestri, 2)
        '                                                End If
        '                                            Else
        '                                                dblIMPORTOINTERESSETOTALE = 0
        '                                            End If
        '                                            'Modifica lobo da verificare
        '                                            sal_acc_div = False
        '                                            Calcolo_GiorniTARSU(CDate(COSTANTValue.CostantiProv.DATA_INIZIO_INTERESSI_GIORNALIERI), DATA_AL, myUtility.GiraDataFromDB(sDataDecorrenzaScadenza), intGiorni, 0, sal_acc_div)
        '                                            'Calcolo_GiorniTARSU(CDate(COSTANTValue.Costanti.DATA_INIZIO_INTERESSI_GIORNALIERI), DATA_AL, sDataDecorrenzaScadenza, intGiorni, 0, sal_acc_div)

        '                                            If dblIMPORTO <> 0 Then
        '                                                'calcolo interesse
        '                                                'unica soluzione
        '                                                If intTipoCalcoloInteressi = 1 Then

        '                                                    dblIMPORTOINTERESSETOTALE_GG = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                                    'dblIMPORTOINTERESSETOTALE = FormatNumber((((dblIMPORTO * dblTassoInteressi) / 100) / 365) * intGiorni, 2)
        '                                                End If
        '                                            Else
        '                                                dblIMPORTOINTERESSETOTALE_GG = 0
        '                                                dblIMPORTOINTERESSETOTALE = 0
        '                                            End If
        '                                        End If
        '                                    End If
        '                                Else
        '                                    'se dal minore di al
        '                                    dblIMPORTOINTERESSETOTALE_GG = 0
        '                                    dblIMPORTOINTERESSETOTALE = 0

        '                                    intSemestri = 0
        '                                    intGiorni = 0
        '                                End If
        '                            End If

        '                            'mantenere questa procedura e implementare quella del calcolo in giorni.
        '                            'prevedere la scelta da front end
        '                            'testare la il campo nella tabella TP_GENERALE_ICI                            

        '                            dblIMPORTOFINALEINTERESSI = dblIMPORTOFINALEINTERESSI + dblIMPORTOINTERESSETOTALE
        '                            dblIMPORTOFINALEINTERESSI_GG = dblIMPORTOFINALEINTERESSI_GG + dblIMPORTOINTERESSETOTALE_GG

        '                            Dim Row1 As DataRow
        '                            Row1 = newTable.NewRow()

        '                            Row1.Item("COD_ENTE") = strCODENTE
        '                            Row1.Item("ANNO") = strANNO
        '                            Row1.Item("COD_VOCE") = strCOD_VOCE
        '                            Row1.Item("IMPORTO") = dblIMPORTOINTERESSETOTALE
        '                            Row1.Item("IMPORTO_GIORNI") = dblIMPORTOINTERESSETOTALE_GG
        '                            Row1.Item("IMPORTO_RIDOTTO") = 0
        '                            Row1.Item("ACCONTO") = 0
        '                            Row1.Item("SALDO") = 0
        '                            Row1.Item("ACCONTO_GIORNI") = 0
        '                            Row1.Item("SALDO_GIORNI") = 0
        '                            Row1.Item("DATA_INIZIO") = strDAL_Interesse
        '                            Row1.Item("DATA_FINE") = strDATA_AL
        '                            Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                            Row1.Item("N_SEMESTRI_SALDO") = intSemestri
        '                            Row1.Item("N_SEMESTRI_TOTALI") = intSemestri
        '                            Row1.Item("N_GIORNI_ACCONTO") = 0
        '                            Row1.Item("N_GIORNI_SALDO") = intGiorni
        '                            Row1.Item("N_GIORNI_TOTALI") = intGiorni
        '                            Row1.Item("TASSO") = dblTassoInteressi
        '                            Row1.Item("GENERIC_ID") = lngGenericID
        '                            'Solo x accertamenti
        '                            Row1.Item("ID_LEGAME") = idLegame
        '                            Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO

        '                            newTable.Rows.Add(Row1)
        '                        Next
        '                        objDSGetCalcoloInteressi.Tables.Add(newTable)
        '                    Else
        '                        Log.Debug("non ho importo")
        '                    End If
        '                Else
        '                    Log.Debug("non ho scadenza interessi")
        '                    Dim Row1 As DataRow
        '                    Row1 = newTable.NewRow()

        '                    Row1.Item("COD_ENTE") = strCODENTE
        '                    Row1.Item("ANNO") = strANNO
        '                    Row1.Item("COD_VOCE") = 0
        '                    Row1.Item("IMPORTO") = dblIMPORTOINTERESSETOTALE
        '                    Row1.Item("IMPORTO_GIORNI") = dblIMPORTOINTERESSETOTALE_GG
        '                    Row1.Item("IMPORTO_RIDOTTO") = 0
        '                    Row1.Item("ACCONTO") = 0
        '                    Row1.Item("SALDO") = 0
        '                    Row1.Item("ACCONTO_GIORNI") = 0
        '                    Row1.Item("SALDO_GIORNI") = 0
        '                    Row1.Item("DATA_INIZIO") = ""
        '                    Row1.Item("DATA_FINE") = ""
        '                    Row1.Item("N_SEMESTRI_ACCONTO") = 0
        '                    Row1.Item("N_SEMESTRI_SALDO") = 0
        '                    Row1.Item("N_SEMESTRI_TOTALI") = 0
        '                    Row1.Item("N_GIORNI_ACCONTO") = 0
        '                    Row1.Item("N_GIORNI_SALDO") = 0
        '                    Row1.Item("N_GIORNI_TOTALI") = 0
        '                    Row1.Item("TASSO") = 0
        '                    Row1.Item("GENERIC_ID") = lngGenericID
        '                    'Solo x accertamenti
        '                    Row1.Item("ID_LEGAME") = idLegame
        '                    Row1.Item("COD_TIPO_PROVVEDIMENTO") = strCODTIPOPROVVEDIMENTO

        '                    newTable.Rows.Add(Row1)

        '                    objDSGetCalcoloInteressi.Tables.Add(newTable)
        '                End If
        '                'update IMPORTO_SANZIONI sul dataset che contiene gli importi totale per il provvedimento
        '                objSituazioneBasePerSanzInt.Tables(0).Rows(intCount).Item("IMPORTO_INTERESSI") = dblIMPORTOFINALEINTERESSI + dblIMPORTOFINALEINTERESSI_GG
        '                objSituazioneBasePerSanzInt.AcceptChanges()
        '            Next
        '        End If
        '        ''culture = New CultureInfo("en-US", True)

        '        ''System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")

        '        Return objDSGetCalcoloInteressi
        '    Catch ex As Exception
        '        Log.Debug("getCalcoloInteressiTARSU.errore::", ex)
        '        Throw New Exception("Function::getCalcoloInteressi::COMPlusService:: " & ex.Message)
        '    End Try
        'End Function
        '*** ***
        ''' <summary>
        '''Scopo :    Questa procedura determinare il numero dei semestri in acconto e saldo compresi in un periodo
        '''Input :    dataverifca = data sino alla quale si deve effettuare il conteggio
        '''           annorif = anno di riferimento dal quale parte in controllo
        '''           sal_acc_div = (se true) Indica se i semestri a saldo e acconto devono essere diversi
        '''Output :   semacconto = numero di semestri relativi all'acconto
        '''           semsaldo = numero di semestri relativi a saldo
        ''' </summary>
        ''' <param name="dal"></param>
        ''' <param name="al"></param>
        ''' <param name="semacconto"></param>
        ''' <param name="semsaldo"></param>
        ''' <param name="errore"></param>
        ''' <param name="sal_acc_div"></param>
        Sub Calcolo_Semestri(ByVal dal As String, ByVal al As String, ByRef semacconto As Integer, ByRef semsaldo As Integer, ByVal errore As Integer, ByVal sal_acc_div As Boolean)
            Dim semestri As Integer
            Dim strDAL, strAL As Date
            'strDAL = Utility.GiraDataFromDB(dal)
            'strAL = Utility.GiraDataFromDB(al)
            strDAL = CDate(dal)
            strAL = CDate(al)

            'semestri = Int((DateDiff("M", Dal, Al, vbMonday) + 1) / 6)
            semestri = Int((DateDiff(DateInterval.Month, strDAL, strAL, FirstDayOfWeek.Monday)) / 6)
            If semestri > 0 Then
                If sal_acc_div = True Then
                    semacconto = semestri
                    semsaldo = semestri - 1
                Else
                    semacconto = semestri
                    semsaldo = semestri
                End If
            Else
                semacconto = 0
                semsaldo = 0
            End If
        End Sub
        ''' <summary>
        '''Scopo :    Questa procedura determinare il numero dei semestri in acconto e saldo compresi in un periodo
        '''Input :    dataverifca = data sino alla quale si deve effettuare il conteggio
        '''           annorif = anno di riferimento dal quale parte in controllo
        '''           sal_acc_div = (se true) Indica se i semestri a saldo e acconto devono essere diversi
        '''Output :   semacconto = numero di semestri relativi all'acconto
        '''           semsaldo = numero di semestri relativi a saldo
        ''' </summary>
        ''' <param name="dal"></param>
        ''' <param name="al"></param>
        ''' <param name="semTARSU"></param>
        ''' <param name="errore"></param>
        ''' <param name="sal_acc_div"></param>
        Sub Calcolo_SemestriTARSU(ByVal dal As String, ByVal al As String, ByRef semTARSU As Integer, ByVal errore As Integer, ByVal sal_acc_div As Boolean)
            Dim semestri As Integer
            Dim strDAL, strAL As Date
            Try
                'strDAL = Utility.GiraDataFromDB(dal)
                'strAL = Utility.GiraDataFromDB(al)
                strDAL = CDate(dal)
                strAL = CDate(al)

                'semestri = Int((DateDiff("M", Dal, Al, vbMonday) + 1) / 6)
                semestri = Int((DateDiff(DateInterval.Month, strDAL, strAL, FirstDayOfWeek.Monday)) / 6)
                If semestri > 0 Then
                    semTARSU = semestri
                Else
                    semTARSU = 0
                End If
            Catch ex As Exception
                Log.Debug("Calcolo_SemestriTARSU.errore::", ex)
            End Try
        End Sub
        ''' <summary>
        '''Scopo :    Questa procedura determinare il numero dei giorni in acconto e saldo compresi in un periodo
        '''Input :    dataverifca = data sino alla quale si deve effettuare il conteggio
        '''           annorif = anno di riferimento dal quale parte in controllo
        '''           sal_acc_div = (se true) Indica se i giorni a saldo e acconto devono essere diversi
        '''Output :   ggcconto = numero di giorni relativi all'acconto
        '''           ggsaldo = numero di giorni relativi a saldo
        ''' </summary>
        ''' <param name="dal"></param>
        ''' <param name="al"></param>
        ''' <param name="dataVersACCONTO"></param>
        ''' <param name="dataVersSALDO"></param>
        ''' <param name="GG"></param>
        ''' <param name="GGacconto"></param>
        ''' <param name="GGsaldo"></param>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Private Function Calcolo_Giorni(ByVal dal As String, ByVal al As String, ByVal dataVersACCONTO As String, ByVal dataVersSALDO As String, ByRef GG As Integer, ByRef GGacconto As Integer, ByRef GGsaldo As Integer) As Boolean
            Dim giorni As Integer
            Dim strDAL, strAL, strDataVersACC, strDataVersSAL As Date

            Try
                GG = 0 : GGacconto = 0 : GGsaldo = 0
                strDAL = CDate(dal)
                strAL = CDate(al)

                strDataVersACC = CDate(dataVersACCONTO)
                strDataVersSAL = CDate(dataVersSALDO)

                Log.Debug("SanzInt.Calcolo_Giorni.strDAL->" + strDAL.ToShortDateString + "- strAL->" + strAL.ToShortDateString)
                giorni = Int(DateDiff(DateInterval.Day, strDataVersACC, strAL, FirstDayOfWeek.Monday) + 1)
                If giorni > 0 Then
                    GGacconto = giorni
                    GGsaldo = Int(DateDiff(DateInterval.Day, strDataVersSAL, strAL, FirstDayOfWeek.Monday) + 1)
                End If
                GG = giorni
                Log.Debug("SanzInt.Calcolo_Giorni.GG=" + GG.ToString + " GGacconto=" + GGacconto.ToString + " GGsaldo=" + GGsaldo.ToString)
                Return True
            Catch ex As Exception
                Log.Debug("SanzInt.Calcolo_Giorni.errore::", ex)
                Return False
            End Try
        End Function
        '''' <summary>
        ''''Scopo :    Questa procedura determinare il numero dei giorni in acconto e saldo compresi in un periodo
        ''''Input :    dataverifca = data sino alla quale si deve effettuare il conteggio
        ''''           annorif = anno di riferimento dal quale parte in controllo
        ''''           sal_acc_div = (se true) Indica se i giorni a saldo e acconto devono essere diversi
        ''''Output :   ggcconto = numero di giorni relativi all'acconto
        ''''           ggsaldo = numero di giorni relativi a saldo
        '''' </summary>
        '''' <param name="dal"></param>
        '''' <param name="al"></param>
        '''' <param name="dataVersTARSU"></param>
        '''' <param name="GG"></param>
        '''' <param name="errore"></param>
        '''' <param name="sal_acc_div"></param>
        'Sub Calcolo_GiorniTARSU(ByVal dal As String, ByVal al As String, ByVal dataVersTARSU As String, ByRef GG As Integer, ByVal errore As Integer, ByVal sal_acc_div As Boolean)
        '    Dim giorni As Integer
        '    Dim strDAL, strAL, strDataVersTARSU As Date

        '    Try
        '        strDAL = CDate(dal)
        '        strAL = CDate(al)

        '        strDataVersTARSU = CDate(dataVersTARSU)
        '        'strDataVersSAL = CDate(dataVersSALDO)
        '        giorni = Int(DateDiff(DateInterval.Day, strDAL, strAL, FirstDayOfWeek.Monday) + 1)
        '        If giorni > 0 Then
        '            If sal_acc_div = True Then
        '                GG = giorni
        '                'GGsaldo = giorni - diff(data versamento acconto e data versamento in saldo)
        '                'GGsaldo = giorni - Int(DateDiff(DateInterval.Day, strDataVersACC, strDataVersSAL, FirstDayOfWeek.Monday))
        '                'campi della tabella TP_GENERALE_ICI 
        '            Else
        '                GG = giorni
        '                ''GGacconto = giorni
        '                ''GGsaldo = giorni
        '            End If
        '        Else
        '            ''GGacconto = 0
        '            ''GGsaldo = 0
        '        End If
        '    Catch ex As Exception
        '        Log.Debug("Calcolo_GiorniTARSU.errore::", ex)
        '    End Try
        'End Sub
    End Class
End Namespace

