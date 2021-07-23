Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.EnterpriseServices
Imports System.Runtime.InteropServices
Imports System.Messaging
Imports Utility
Imports log4net
Imports System.Globalization
Imports ComPlusInterface

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per il confronto fra dichiarato e versato con eventuale creazione dei dati per l'atto
    ''' </summary>
    Public Class COMPLusProcessLiquidazioni

        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(COMPLusProcessLiquidazioni))

        Protected objUtility As New MotoreProvUtility
        Private m_strConnectionOPENgovICI As String = ""
        'Private objICI As DataSet
        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
        Dim objDBOPENgovProvvedimentiUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

        Dim sLogFase1, sLogFase2, sLogFase3 As String

        'boolean che servono per memorizzare lo stato di ogni singola fase
        'true--> fase terminata con anomalie 
        'false-->fase terminata senza anomalie 
        Dim blnFase1ANOMALA As Boolean = False
        Dim blnFase2ANOMALA As Boolean = False
        Dim blnFase3ANOMALA As Boolean = False

        Dim blnFASE1_TERMINATA_CON_ERRORI As Boolean = True
        Dim blnFASE2_TERMINATA_CON_ERRORI As Boolean = True
        Dim blnFASE3_TERMINATA_CON_ERRORI As Boolean = True

        '*** 201810 - Generazione Massiva Atti ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myHashTable"></param>
        ''' <param name="dsAnagrafica"></param>
        ''' <param name="ImpDichAcconto"></param>
        ''' <param name="ImpDichSaldo"></param>
        ''' <param name="ImpDichTotale"></param>
        ''' <param name="dsVersamenti"></param>
        ''' <param name="dsSanzioni"></param>
        ''' <param name="ListInteressi"></param>
        ''' <param name="ListCalcoli"></param>
        ''' <param name="dsRiepilogo"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="10/12/2019">in caso di calcolo per Cartelle Insoluti devo prendere il pagato per singolo avviso</revision></revisionHistory>
        Public Function ProcessFase2(StringConnectionProvv As String, StringConnectionICI As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As Hashtable, ByVal dsAnagrafica As DataSet, ImpDichAcconto As Double, ImpDichSaldo As Double, ImpDichTotale As Double, sCodCartella As String, ByRef dsVersamenti As DataSet, ByRef dsSanzioni As DataSet, ByRef ListInteressi As ObjInteressiSanzioni(), ByRef ListCalcoli() As ObjBaseIntSanz, ByRef dsRiepilogo As ObjBaseIntSanz) As Boolean
            Dim dsSituazioneBasePerSanzIntAppoggio As DataSet = Nothing
            Dim fncSanzInt As New SanzInt
            Dim CalcolaSanzInt As Boolean = False  'determino, in fase di perparazione al calcolo sanz-int, se "effettivamente" procedere col calcolo di sanz-int oppure no
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject

            Try
                myHashTable.Add("COD_TIPO_PROCEDIMENTO", OggettoAtto.Procedimento.Liquidazione)

                Dim IdTributo As String = CType(myHashTable("COD_TRIBUTO"), String)
                Dim AnnoDaLiquidare As String = CType(myHashTable("ANNODA"), String)
                'Imposto la data di elaborazione (data odierna) per evitare ad es. di considerare il 10/06 o il 17/12 come se il semestre solare fosse già scattato e quindi tolgo un mese se la data è il 31/12 tolgo anche un giorno altrimenti verrebbe 31/11
                Dim DataElaborazione As Date
                Try
                    If myHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
                        DataElaborazione = CDate(New MotoreProvUtility().GiraDataFromDB(myHashTable("DATA_ELABORAZIONE_PER_RETTIFICA")))
                    End If
                    If DataElaborazione.Date = Date.MinValue.Date Or DataElaborazione.Date = Date.MaxValue.Date Then
                        DataElaborazione = Date.Now
                    End If
                Catch
                    DataElaborazione = Date.Now
                End Try

                'FASE 2 CONFRONTO VERSATO-DICHIARATO
                If IdContribuente <= 0 Then
                    Log.Debug("Non sono state trovate dichiarazioni, non calcolo la fase 2")
                    ListCalcoli = Nothing
                    dsSanzioni = Nothing
                    ListInteressi = Nothing
                    dsRiepilogo = Nothing
                    Return True
                Else
                    Log.Debug("Controllata Fase 2")
                    sLogFase2 = sLogFase2 & "Controllata Fase 2$"
                    'prelevo i versamenti
                    dsVersamenti = New DBOPENgovProvvedimentiSelect().getVersamentiPerFase2(StringConnectionICI, IdEnte, IdContribuente, AnnoDaLiquidare, IdTributo, sCodCartella)
                    If Not dsVersamenti Is Nothing Then
                        Log.Debug("objDSVersamentiFase2.Tables(0).Rows.Count =" & dsVersamenti.Tables(0).Rows.Count)
                        Log.Debug("objDSVersamentiFase2.Tables(0).Rows.Count =" & dsVersamenti.Tables(0).Rows.Count)
                    End If

                    '*** 20140509 - TASI ***
                    If myHashTable.ContainsKey("TRIBUTOCALCOLO") = False Then
                        myHashTable.Add("TRIBUTOCALCOLO", IdTributo)
                    End If
                    '*** ***
                    myHashTable("ANNOA") = "-1"
                    myHashTable("ANNOA") = AnnoDaLiquidare

                    Log.Debug(IdEnte & " - devo confrontare versato/dichiarato contribuente::" & IdContribuente & " sull'anno::" & AnnoDaLiquidare)
                    'Dim objCOMPlusCalcoloICI As New COMPlusCalcoloICI
                    Log.Debug(IdEnte & "richiamo CreateDSperCalcoloICI")
                    Log.Debug(IdEnte & "richiamo SetObjDSAppoggioSanzioni")
                    dsSituazioneBasePerSanzIntAppoggio = SetObjDSAppoggioSanzioni()
                    Log.Debug(IdEnte & "richiamo ConfrontoVersatoDichiarato")
                    Dim ListBaseIntSanz() As ObjBaseIntSanz
                    ListBaseIntSanz = ConfrontoVersatoDichiarato(IdContribuente, AnnoDaLiquidare, dsVersamenti, ImpDichAcconto, ImpDichSaldo, ImpDichTotale, dsSituazioneBasePerSanzIntAppoggio, CalcolaSanzInt)

                    myHashTable("ID_FASE") = 2 'FASE 2

                    If CalcolaSanzInt = True Then
                        Log.Debug("DEVO CALCOLARE SANZIONI ED INTERESSI")
                        Dim strCodVoce As String
                        strCodVoce = "#" & ListBaseIntSanz(0).COD_TIPO_PROVVEDIMENTO
                        '*** 20140701 - IMU/TARES ***
                        ' Se il Codice Tributo è "TASI" lo cambio con "8852" per il calcolo delle Sanzioni e degli Interessi
                        Dim TribSanzInt As String
                        Dim IdFase As Integer = -1
                        TribSanzInt = IdTributo
                        If (TribSanzInt = Utility.Costanti.TRIBUTO_TASI) Then
                            TribSanzInt = Utility.Costanti.TRIBUTO_ICI
                        End If
                        If Not IsNothing(myHashTable("ID_FASE")) Then
                            IdFase = StringOperation.FormatInt(myHashTable("ID_FASE"))
                        End If
                        dsSanzioni = fncSanzInt.getCalcoloSanzioni(StringConnectionProvv, ListBaseIntSanz, dsSituazioneBasePerSanzIntAppoggio, TribSanzInt, strCodVoce, IdEnte, ListBaseIntSanz(0).COD_TIPO_PROVVEDIMENTO, 2, idfase, StringOperation.FormatString(myHashTable("COD_TIPO_PROCEDIMENTO")), myHashTable, dsAnagrafica, False, -1)
                        '*** ***
                        Log.Debug("Calcolo le sanzioni contribuente:: " & IdContribuente)
                        Log.Debug("ProcessFase2.DataDecorrenzaInteressi->")
                        ListInteressi = fncSanzInt.getCalcoloInteressi(IdEnte, TribSanzInt, "3", ListBaseIntSanz(0).COD_TIPO_PROVVEDIMENTO, OggettoAtto.Procedimento.Liquidazione, OggettoAtto.Fase.VersatoDichiarato, DataElaborazione, "", "", 1, ListBaseIntSanz, myHashTable("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI"))
                        Log.Debug("Calcolo gli interessi contribuente::" & IdContribuente)
                    Else
                        Log.Debug("non DEVO CALCOLARE SANZIONI ED INTERESSI")
                    End If

                    dsRiepilogo = CreateRiepilogoImporti(StringConnectionProvv, IdEnte, AnnoDaLiquidare, IdContribuente, Nothing, ListBaseIntSanz, Nothing, False, True, False, "", myHashTable, False, False)
                    ListCalcoli = ListBaseIntSanz

                    Log.Debug("ProcessFase2 effettuata con successo contribuente::" & IdContribuente)
                    If Not dsSituazioneBasePerSanzIntAppoggio Is Nothing Then
                        dsSituazioneBasePerSanzIntAppoggio.Dispose()
                    End If
                    Log.Debug("objDSVersamentiFase2.Tables(0).Rows.Count =" & dsVersamenti.Tables(0).Rows.Count & " contribuente::" & IdContribuente)

                    Return True
                    'FINE FASE 2 CONFRONTO VERSATO-DICHIARATO #################################################################################################################################################################################
                End If
                Return True
            Catch ex As Exception
                Log.Debug("ProcessFase2.Si è verificato un errore durante l'elaborazione della Fase 2 per Accertamento::" & ex.Message & "::" & ex.StackTrace)
                Return False
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strANNOdaLIQUIDARE"></param>
        ''' <param name="strCOD_CONTRIBUENTE"></param>
        ''' <param name="objDSSituazioneBasePerSanzIntFASE1"></param>
        ''' <param name="ListBaseSanzIntFase2"></param>
        ''' <param name="objDSSituazioneBasePerSanzIntFASE3"></param>
        ''' <param name="blnFase1"></param>
        ''' <param name="blnFase2"></param>
        ''' <param name="blnFase3"></param>
        ''' <param name="strSegno"></param>
        ''' <param name="objHashTable"></param>
        ''' <param name="blnImportoSottoSoglia"></param>
        ''' <param name="blnVerifica_Minimo_E_Spese"></param>
        ''' <returns></returns>
        Private Function CreateRiepilogoImporti(StringConnectionProvv As String, strCODEnte As String, ByVal strANNOdaLIQUIDARE As String, ByVal strCOD_CONTRIBUENTE As String, ByVal objDSSituazioneBasePerSanzIntFASE1 As DataSet, ByVal ListBaseSanzIntFase2() As ObjBaseIntSanz, ByVal objDSSituazioneBasePerSanzIntFASE3 As DataSet, ByVal blnFase1 As Boolean, ByVal blnFase2 As Boolean, ByVal blnFase3 As Boolean, ByVal strSegno As String, ByVal objHashTable As Hashtable, ByRef blnImportoSottoSoglia As Boolean, ByVal blnVerifica_Minimo_E_Spese As Boolean) As ObjBaseIntSanz
            Try
                blnImportoSottoSoglia = False

                Dim objectDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect
                Dim intCount As Integer = 0
                Dim oSituazioneBasePerSanzIntFINALE As New ObjBaseIntSanz
                Dim strCOD_CONTRIBUENTE_F1, strANNO_F1, strTIPOPROVV_F1 As String
                Dim strCOD_CONTRIBUENTE_F2, strANNO_F2, strTIPOPROVV_F2 As String
                Dim strCOD_CONTRIBUENTE_F3, strANNO_F3, strTIPOPROVV_F3 As String
                Dim DIFF_IMPOSTA_ACCONTO_F1, DIFF_IMPOSTA_SALDO_F1, DIFF_IMPOSTA_TOTALE_F1, IMPORTO_SANZIONI_F1, IMPORTO_SANZIONI_RIDOTTO_F1, IMPORTO_INTERESSI_F1 As Double
                Dim DIFF_IMPOSTA_ACCONTO_F2, DIFF_IMPOSTA_SALDO_F2, DIFF_IMPOSTA_TOTALE_F2, IMPORTO_SANZIONI_F2, IMPORTO_INTERESSI_F2, IMPORTO_SANZIONI_RIDOTTO_F2, IMPORTO_DICHIARATO_F2, IMPORTO_VERSATO_F2 As Double
                Dim DIFF_IMPOSTA_ACCONTO_F3, DIFF_IMPOSTA_SALDO_F3, DIFF_IMPOSTA_TOTALE_F3, IMPORTO_SANZIONI_F3, IMPORTO_SANZIONI_RIDOTTO_F3, IMPORTO_INTERESSI_F3 As Double
                Dim DIFF_IMPOSTA_ACCONTO_TOT, DIFF_IMPOSTA_SALDO_TOT, DIFF_IMPOSTA_TOTALE_TOT, IMPORTO_SANZIONI_TOT, IMPORTO_SANZIONI_RIDOTTO_TOT, IMPORTO_INTERESSI_TOT As Double
                Dim IMPORTO_DIFFERENZA_TOTALE_F1, IMPORTO_DIFFERENZA_TOTALE_F2, IMPORTO_DIFFERENZA_TOTALE_F3, IMPORTO_DIFFERENZA_TOTALE As Double
                Dim IMPORTO_SPESE As Double
                Dim IMPORTO_SOGLIA_MINIMA As Double
                Dim flagVERSAMENTO_TARDIVO As Boolean
                Dim objDSRiepilogoFase1() As DataRow
                Dim objDSRiepilogoFase3() As DataRow
                Dim COD_TIPO_PROVVEDIMENTO As Integer

                Dim strCODTributo As String = CType(objHashTable("COD_TRIBUTO"), String)

                If blnFase1 = True Then
                    If Not objDSSituazioneBasePerSanzIntFASE1 Is Nothing Then
                        objDSRiepilogoFase1 = objDSSituazioneBasePerSanzIntFASE1.Tables(0).Select("ANNO='" & strANNOdaLIQUIDARE & "'")
                        If objDSRiepilogoFase1.Length > 0 Then
                            strTIPOPROVV_F1 = CType(objDSRiepilogoFase1(intCount)("TIPO_PROVVEDIMENTO"), String)
                            strCOD_CONTRIBUENTE_F1 = CType(objDSRiepilogoFase1(intCount)("COD_CONTRIBUENTE"), String)
                            strANNO_F1 = CType(objDSRiepilogoFase1(intCount)("ANNO"), String)
                            DIFF_IMPOSTA_ACCONTO_F1 = 0                      ' CType(objDSRiepilogoFase1(intCount)("DIFFERENZA_IMPOSTA_ACCONTO"), Double)
                            DIFF_IMPOSTA_SALDO_F1 = 0                        'CType(objDSRiepilogoFase1(intCount)("DIFFERENZA_IMPOSTA_SALDO"), Double)
                            DIFF_IMPOSTA_TOTALE_F1 = 0                       'CType(objDSRiepilogoFase1(intCount)("DIFFERENZA_IMPOSTA_TOTALE"), Double)
                            IMPORTO_SANZIONI_F1 = FormatNumber(CType(objDSRiepilogoFase1(intCount)("IMPORTO_SANZIONI").Replace(",", "."), Double), 2)
                            IMPORTO_SANZIONI_RIDOTTO_F1 = FormatNumber(CType(objDSRiepilogoFase1(intCount)("IMPORTO_SANZIONI_RIDOTTO").Replace(",", "."), Double), 2)
                            IMPORTO_INTERESSI_F1 = FormatNumber(CType(objDSRiepilogoFase1(intCount)("IMPORTO_INTERESSI").Replace(",", "."), Double), 2)
                            flagVERSAMENTO_TARDIVO = CType(objDSRiepilogoFase1(intCount)("FLAG_VERSAMENTO_TARDIVO"), Boolean)
                            IMPORTO_DIFFERENZA_TOTALE_F1 = DIFF_IMPOSTA_TOTALE_F1 + IMPORTO_SANZIONI_F1 + IMPORTO_INTERESSI_F1
                        End If
                    Else
                        strTIPOPROVV_F1 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                      'strTIPOPROVV_F1 = ""
                        blnFase1ANOMALA = False
                        blnFASE1_TERMINATA_CON_ERRORI = False
                    End If
                Else
                    strTIPOPROVV_F1 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                'strTIPOPROVV_F1 = ""
                    blnFase1ANOMALA = False
                    blnFASE1_TERMINATA_CON_ERRORI = False
                End If

                If blnFase2 = True Then
                    If Not ListBaseSanzIntFase2 Is Nothing Then
                        For Each myItem As ObjBaseIntSanz In ListBaseSanzIntFase2
                            If myItem.Anno = strANNOdaLIQUIDARE Then
                                strTIPOPROVV_F2 = myItem.COD_TIPO_PROVVEDIMENTO
                                strCOD_CONTRIBUENTE_F2 = myItem.IdContribuente.ToString
                                strANNO_F2 = myItem.Anno
                                DIFF_IMPOSTA_ACCONTO_F2 += FormatNumber(myItem.DifferenzaImpostaAcconto, 2)
                                DIFF_IMPOSTA_SALDO_F2 += FormatNumber(myItem.DifferenzaImpostaSaldo, 2)
                                DIFF_IMPOSTA_TOTALE_F2 += FormatNumber(myItem.DifferenzaImposta, 2)
                                IMPORTO_SANZIONI_F2 += FormatNumber(myItem.Sanzioni, 2)
                                IMPORTO_SANZIONI_RIDOTTO_F2 += FormatNumber(myItem.SanzioniRidotto, 2)
                                IMPORTO_INTERESSI_F2 += FormatNumber(myItem.Interessi, 2)
                                IMPORTO_DICHIARATO_F2 += FormatNumber(myItem.Dichiarato, 2)
                                IMPORTO_VERSATO_F2 += FormatNumber(myItem.Pagato, 2)
                                IMPORTO_DIFFERENZA_TOTALE_F2 += DIFF_IMPOSTA_TOTALE_F2 + IMPORTO_SANZIONI_F2 + IMPORTO_INTERESSI_F2

                                Log.Debug("::DIFF_IMPOSTA_ACCONTO_F2::" & DIFF_IMPOSTA_ACCONTO_F2)
                                Log.Debug(":: DIFF_IMPOSTA_SALDO_F2::" & DIFF_IMPOSTA_SALDO_F2)
                                Log.Debug(":: DIFF_IMPOSTA_TOTALE_F2::" & DIFF_IMPOSTA_TOTALE_F2)
                                Log.Debug("::IMPORTO_SANZIONI_F2::" & IMPORTO_SANZIONI_F2)
                                Log.Debug("::IMPORTO_SANZIONI_RIDOTTO_F2::" & IMPORTO_SANZIONI_RIDOTTO_F2)
                                Log.Debug("::IMPORTO_INTERESSI_F2::" & IMPORTO_INTERESSI_F2)
                                Log.Debug("::IMPORTO_DICHIARATO_F2::" & IMPORTO_DICHIARATO_F2)
                                Log.Debug("::IMPORTO_VERSATO_F2::" & IMPORTO_VERSATO_F2)
                            End If
                        Next
                    Else
                        strTIPOPROVV_F2 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                      'strTIPOPROVV_F2 = ""
                        blnFase2ANOMALA = False
                        blnFASE2_TERMINATA_CON_ERRORI = False
                    End If
                Else
                    strTIPOPROVV_F2 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                'strTIPOPROVV_F2 = ""
                    blnFase2ANOMALA = False
                    blnFASE2_TERMINATA_CON_ERRORI = False
                End If

                If blnFase3 = True Then
                    If Not objDSSituazioneBasePerSanzIntFASE3 Is Nothing Then
                        objDSRiepilogoFase3 = objDSSituazioneBasePerSanzIntFASE3.Tables(0).Select("ANNO='" & strANNOdaLIQUIDARE & "'")
                        If objDSRiepilogoFase3.Length > 0 Then
                            strTIPOPROVV_F3 = CType(objDSRiepilogoFase3(intCount)("TIPO_PROVVEDIMENTO"), String)
                            strCOD_CONTRIBUENTE_F3 = CType(objDSRiepilogoFase3(intCount)("COD_CONTRIBUENTE"), String)
                            strANNO_F3 = CType(objDSRiepilogoFase3(intCount)("ANNO"), String)
                            DIFF_IMPOSTA_ACCONTO_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("DIFFERENZA_IMPOSTA_ACCONTO").Replace(",", "."), Double), 2)
                            DIFF_IMPOSTA_SALDO_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("DIFFERENZA_IMPOSTA_SALDO").Replace(",", "."), Double), 2)
                            DIFF_IMPOSTA_TOTALE_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("DIFFERENZA_IMPOSTA_TOTALE").Replace(",", "."), Double), 2)
                            IMPORTO_SANZIONI_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("IMPORTO_SANZIONI").Replace(",", "."), Double), 2)
                            IMPORTO_SANZIONI_RIDOTTO_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("IMPORTO_SANZIONI_RIDOTTO").Replace(",", "."), Double), 2)
                            IMPORTO_INTERESSI_F3 = FormatNumber(CType(objDSRiepilogoFase3(intCount)("IMPORTO_INTERESSI").Replace(",", "."), Double), 2)
                            IMPORTO_DIFFERENZA_TOTALE_F3 = DIFF_IMPOSTA_TOTALE_F3 + IMPORTO_SANZIONI_F3 + IMPORTO_INTERESSI_F3
                        End If
                    Else
                        strTIPOPROVV_F3 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                      'strTIPOPROVV_F3 = ""
                        blnFase3ANOMALA = False
                        blnFASE3_TERMINATA_CON_ERRORI = False
                    End If
                Else
                    strTIPOPROVV_F3 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK                 'strTIPOPROVV_F3 = ""
                    blnFase3ANOMALA = False
                    blnFASE3_TERMINATA_CON_ERRORI = False
                End If

                If (strTIPOPROVV_F1 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK) And (strTIPOPROVV_F2 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK) And (strTIPOPROVV_F3 = COSTANTValue.CostantiProv.TIPO_PROVVEDIMENTO_OK) And (blnFase1ANOMALA = False And blnFase2ANOMALA = False And blnFase3ANOMALA = False) Then
                    'TUTTE E TRE LE FASI HANNO DATO ESITO POSITIVO E TUTTE E TRE LE FASI NON HANNO DATO ANOMALIE NO AVVISO - NON CREO IL PROVVEDIMENTO NO FILE DI LOG
                    Return oSituazioneBasePerSanzIntFINALE
                End If
                DIFF_IMPOSTA_ACCONTO_TOT = DIFF_IMPOSTA_ACCONTO_F1 + DIFF_IMPOSTA_ACCONTO_F2 + DIFF_IMPOSTA_ACCONTO_F3
                DIFF_IMPOSTA_SALDO_TOT = DIFF_IMPOSTA_SALDO_F1 + DIFF_IMPOSTA_SALDO_F2 + DIFF_IMPOSTA_SALDO_F3
                DIFF_IMPOSTA_TOTALE_TOT = DIFF_IMPOSTA_TOTALE_F1 + DIFF_IMPOSTA_TOTALE_F2 + DIFF_IMPOSTA_TOTALE_F3
                IMPORTO_SANZIONI_TOT = IMPORTO_SANZIONI_F1 + IMPORTO_SANZIONI_F2 + IMPORTO_SANZIONI_F3
                IMPORTO_SANZIONI_RIDOTTO_TOT = IMPORTO_SANZIONI_RIDOTTO_F1 + IMPORTO_SANZIONI_RIDOTTO_F2 + IMPORTO_SANZIONI_RIDOTTO_F3
                IMPORTO_INTERESSI_TOT = IMPORTO_INTERESSI_F1 + IMPORTO_INTERESSI_F2 + IMPORTO_INTERESSI_F3
                IMPORTO_DIFFERENZA_TOTALE = IMPORTO_DIFFERENZA_TOTALE_F1 + IMPORTO_DIFFERENZA_TOTALE_F2 + IMPORTO_DIFFERENZA_TOTALE_F3
                Log.Debug("CreateRiepilogoImporti::DIFF_IMPOSTA_ACCONTO_TOT::" & DIFF_IMPOSTA_ACCONTO_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::DIFF_IMPOSTA_SALDO_TOT::" & DIFF_IMPOSTA_SALDO_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::DIFF_IMPOSTA_TOTALE_TOT::" & DIFF_IMPOSTA_TOTALE_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::IMPORTO_SANZIONI_TOT::" & IMPORTO_SANZIONI_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::IMPORTO_SANZIONI_RIDOTTO_TOT::" & IMPORTO_SANZIONI_RIDOTTO_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::IMPORTO_INTERESSI_TOT::" & IMPORTO_INTERESSI_TOT.ToString)
                Log.Debug("CreateRiepilogoImporti::IMPORTO_DIFFERENZA_TOTALE::" & IMPORTO_DIFFERENZA_TOTALE.ToString)

                oSituazioneBasePerSanzIntFINALE.Anno = strANNOdaLIQUIDARE            'strANNO_F1
                oSituazioneBasePerSanzIntFINALE.DifferenzaImpostaAcconto = DIFF_IMPOSTA_ACCONTO_TOT
                oSituazioneBasePerSanzIntFINALE.DifferenzaImpostaSaldo = DIFF_IMPOSTA_SALDO_TOT
                oSituazioneBasePerSanzIntFINALE.DifferenzaImposta = DIFF_IMPOSTA_TOTALE_TOT
                oSituazioneBasePerSanzIntFINALE.Sanzioni = IMPORTO_SANZIONI_TOT
                oSituazioneBasePerSanzIntFINALE.SanzioniRidotto = IMPORTO_SANZIONI_RIDOTTO_TOT
                oSituazioneBasePerSanzIntFINALE.Interessi = IMPORTO_INTERESSI_TOT

                oSituazioneBasePerSanzIntFINALE.Dichiarato = IMPORTO_DICHIARATO_F2
                oSituazioneBasePerSanzIntFINALE.Pagato = IMPORTO_VERSATO_F2

                If IMPORTO_DIFFERENZA_TOTALE > 0 Then
                    'AVVISO
                    'per determinare il tipo di avviso si prende "lo stato" più grave tra le 3 fasi
                    If (strTIPOPROVV_F2 = OggettoAtto.Provvedimento.AccertamentoUfficio) Or (strTIPOPROVV_F3 = OggettoAtto.Provvedimento.AccertamentoUfficio) Then
                        'se fase2 o fase 3 hanno scaturito un avviso di accertamento d'ufficio
                        oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoUfficio                     'AVVISO DI ACCERTAMENTO D'UFFICIO
                        COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoUfficio
                    ElseIf (strTIPOPROVV_F1 = OggettoAtto.Provvedimento.AccertamentoRettifica) Or (strTIPOPROVV_F2 = OggettoAtto.Provvedimento.AccertamentoRettifica) Or (strTIPOPROVV_F3 = OggettoAtto.Provvedimento.AccertamentoRettifica) Then
                        'se fase1 o fase2 o fase 3 hanno scaturito un avviso di accertamento in rettifica
                        oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoRettifica                 'AVVISO DI ACCERTAMENTO IN RETTIFICA
                        COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoRettifica
                    Else
                        oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoRettifica                 'AVVISO DI ACCERTAMENTO IN RETTIFICA
                        COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoRettifica
                    End If
                ElseIf IMPORTO_DIFFERENZA_TOTALE < 0 Then
                    oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.Rimborso                 'AVVISO DI ACCERTAMENTO IN RETTIFICA
                    COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.Rimborso
                ElseIf IMPORTO_DIFFERENZA_TOTALE = 0 Then
                    'TUTTE E TRE LE FASI HANNO DATO ESITO POSITIVO
                    'NO AVVISO - NON CREO IL PROVVEDIMENTO
                    oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.NoAvviso
                    COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.NoAvviso
                End If

                If blnVerifica_Minimo_E_Spese = True Then
                    IMPORTO_SOGLIA_MINIMA = objectDBOPENgovProvvedimentiSelect.GetSogliaMinima(StringConnectionProvv, strANNOdaLIQUIDARE, strCODTributo, strCODEnte, COD_TIPO_PROVVEDIMENTO, objHashTable)
                    If IMPORTO_SOGLIA_MINIMA > 0 Then
                        If Math.Abs(IMPORTO_DIFFERENZA_TOTALE) < IMPORTO_SOGLIA_MINIMA Then
                            blnImportoSottoSoglia = True
                            Return oSituazioneBasePerSanzIntFINALE
                        End If
                    End If
                    IMPORTO_SPESE = objectDBOPENgovProvvedimentiSelect.GetSpese(StringConnectionProvv, strANNOdaLIQUIDARE, strCODTributo, strCODEnte, oSituazioneBasePerSanzIntFINALE.COD_TIPO_PROVVEDIMENTO, objHashTable)
                    oSituazioneBasePerSanzIntFINALE.Spese = IMPORTO_SPESE
                Else
                    oSituazioneBasePerSanzIntFINALE.Spese = 0
                    blnImportoSottoSoglia = False
                End If
                'Next
                Return oSituazioneBasePerSanzIntFINALE
            Catch ex As Exception
                Log.Debug("CreateRiepilogoImporti:: si è verificato il seguente errore::", ex)
                Return Nothing
            End Try
        End Function

        Public Function ImportoArrotondato(ByVal importoInput As Object) As Double

            Dim importo As Double
            If importoInput > 0 Then
                importo = (Int((importoInput * 100) + 0.5)) / 100
            ElseIf importoInput < 0 Then
                importo = (Int((importoInput * 100) - 0.5)) / 100
            End If
            ImportoArrotondato = CDbl(importo)

        End Function
        '*** 201810 - Generazione Massiva Atti ***
        Private Function ConfrontoVersatoDichiarato(IdContribuente As Integer, ByVal Anno As String, ByVal dsVersamenti As DataSet, SUMImpDichACCONTO As Double, SUMImpDichSALDO As Double, SUMImpDichTOTALE As Double, ByRef dsSanzioni As DataSet, ByRef blnCalcolaSanzInt As Boolean) As ObjBaseIntSanz()
            'blnCalcolaSanzInt viene restituito a true se devo calcolare sanzioni e interessi
            'blnCalcolaSanzInt viene restituito a false se NON devo calcolare sanzioni e interessi
            'il dataset objICIDich viene riempito con il calcolo dell'ici degli immobili in questione
            Try
                Dim blnAcconto, blnSaldo As Boolean
                Dim ImpVersACCONTO, ImpVersSALDO As Double
                Dim SUMImpVersACCONTO, SUMImpVersSALDO, SUMImpVersTOTALE As Double
                Dim DiffImpostaACCONTO, DiffImpostaSALDO, DiffImpostaTOTALE As Double
                Dim objSanzioniInteressi As New COMPlusOPENgovProvvedimenti.SanzInt
                Dim sDescrStatoFase As String = ""
                Dim myRow As New ObjBaseIntSanz
                Dim myRowAppoggio As DataRow
                Dim blnModalitaUS As Boolean = False
                Dim ListBase As New ArrayList

                Log.Debug("inizio Confronto Versato Dichiarato")
                myRowAppoggio = dsSanzioni.Tables("TP_APPOGGIO_CALCOLO_SANZIONI").NewRow

                ImpVersACCONTO = 0 : SUMImpVersACCONTO = 0
                ImpVersSALDO = 0 : SUMImpVersSALDO = 0

                If Not IsNothing(dsVersamenti.Tables("VERSAMENTI")) Then
                    If dsVersamenti.Tables("VERSAMENTI").Rows.Count > 0 Then
                        Log.Debug("Sono presenti " & dsVersamenti.Tables("VERSAMENTI").Rows.Count & " Versamenti")
                        sLogFase2 = sLogFase2 & "Sono presenti " & dsVersamenti.Tables("VERSAMENTI").Rows.Count & " Versamenti$"

                        For Each myVersamento As DataRow In dsVersamenti.Tables("VERSAMENTI").Rows
                            blnAcconto = CType(myVersamento("ACCONTO"), String)
                            blnSaldo = CType(myVersamento("SALDO"), String)

                            If blnAcconto = True And blnSaldo = False Then
                                ImpVersACCONTO = CType(myVersamento("IMPORTOPAGATO"), Double)
                                SUMImpVersACCONTO = SUMImpVersACCONTO + ImpVersACCONTO
                                blnModalitaUS = False
                            ElseIf blnAcconto = False And blnSaldo = True Then
                                ImpVersSALDO = CType(myVersamento("IMPORTOPAGATO"), Double)
                                SUMImpVersSALDO = SUMImpVersSALDO + ImpVersSALDO
                                blnModalitaUS = False
                            Else
                                ImpVersACCONTO = CType(myVersamento("IMPORTOPAGATO"), Double)
                                SUMImpVersACCONTO = SUMImpVersACCONTO + ImpVersACCONTO
                                blnModalitaUS = True
                            End If
                        Next

                        SUMImpVersTOTALE = ImportoArrotondato(SUMImpVersACCONTO + SUMImpVersSALDO)
                    Else
                        'se non ci sono versamenti, segnalare nel file di log
                        Log.Debug("Non sono presenti Versamenti")
                        sLogFase2 = sLogFase2 & "Non sono presenti Versamenti$"
                        blnFase2ANOMALA = True
                    End If
                Else
                    'se non ci sono versamenti, segnalare nel file di log
                    Log.Debug("Non sono presenti Versamenti")
                    sLogFase2 = sLogFase2 & "Non sono presenti Versamenti$"
                    blnFase2ANOMALA = True
                End If

                DiffImpostaACCONTO = SUMImpDichACCONTO - SUMImpVersACCONTO
                DiffImpostaSALDO = SUMImpDichSALDO - SUMImpVersSALDO
                DiffImpostaTOTALE = SUMImpDichTOTALE - SUMImpVersTOTALE
                Log.Debug("DiffImpostaACCONTO=" & DiffImpostaACCONTO.ToString)
                Log.Debug("DiffImpostaSALDO=" & DiffImpostaSALDO.ToString)
                Log.Debug("DiffImpostaTOTALE=" & DiffImpostaTOTALE.ToString)
                Log.Debug("IMPORTO_TOTALE_DICHIARATO=" & SUMImpDichTOTALE.ToString)
                Log.Debug("IMPORTO_TOTALE_VERSATO=" & SUMImpVersTOTALE.ToString)

                myRowAppoggio.Item("IVA") = System.DBNull.Value
                myRowAppoggio.Item("IVS") = System.DBNull.Value
                myRowAppoggio.Item("IVUS") = System.DBNull.Value

                myRowAppoggio.Item("IV") = SUMImpVersTOTALE
                myRowAppoggio.Item("DI") = DiffImpostaTOTALE
                myRowAppoggio.Item("GG") = System.DBNull.Value
                myRowAppoggio.Item("ANNO") = Anno

                dsSanzioni.Tables("TP_APPOGGIO_CALCOLO_SANZIONI").Rows.Add(myRowAppoggio)
                dsSanzioni.AcceptChanges()

                myRow = New ObjBaseIntSanz

                myRow.IdContribuente = IdContribuente
                myRow.Anno = Anno
                myRow.DifferenzaImpostaAcconto = DiffImpostaACCONTO
                myRow.DifferenzaImpostaSaldo = DiffImpostaSALDO
                myRow.DifferenzaImposta = DiffImpostaTOTALE
                myRow.Dichiarato = SUMImpDichTOTALE
                myRow.Pagato = SUMImpVersTOTALE
                Select Case SUMImpDichTOTALE
                    Case > 0     'se CI SONO delle dichiarazioni/immobili               'se Dovuto > 0
                        If SUMImpVersTOTALE > 0 Then                      'se Versato > 0
                            If DiffImpostaTOTALE = 0 Then                            'Se la differenza = 0, FASE 2 TERMINATA OK
                                myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.NoAvviso
                                sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_OK
                                blnCalcolaSanzInt = False
                                sLogFase2 = sLogFase2 & "Sono presentidichiarazioni, Dichiarato maggiore di zero, Versato maggiore di zero e differenza tra Dichiarato e Versato uguale a zero$"

                                blnFASE2_TERMINATA_CON_ERRORI = False
                            ElseIf DiffImpostaTOTALE < 0 Then                            'la differenza < 0, POTENZIALE RIMBORSO
                                myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.Rimborso
                                sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_RIMBORSO
                                blnCalcolaSanzInt = True
                                sLogFase2 = sLogFase2 & "Sono presenti dichiarazioni, Dichiarato maggiore di zero, Versato maggiore di zero e differenza tra Dichiarato e Versato minore di zero$"
                            ElseIf DiffImpostaTOTALE > 0 Then                            'Se la differenza >0, AVVISO DI ACCERTAMENTO IN RETTIFICA
                                myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoRettifica
                                sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_IN_RETTIFICA
                                blnCalcolaSanzInt = True
                                sLogFase2 = sLogFase2 & "Sono presenti dichiarazioni, Dichiarato maggiore di zero, Versato maggiore di zero e differenza tra Dichiarato e Versato maggiore di zero$"
                            End If
                        ElseIf SUMImpVersTOTALE = 0 Then                      'se Versato = 0
                            myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.AccertamentoUfficio
                            sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_D_UFFICIO
                            blnCalcolaSanzInt = True
                            sLogFase2 = sLogFase2 & "Sono presenti dichiarazioni, Dichiarato maggiore di zero e Versato uguale a zero$"
                        End If
                    Case 0                    'se Dovuto = 0
                        If SUMImpVersTOTALE > 0 Then                      'se Versato > 0
                            myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.Rimborso
                            sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_RIMBORSO
                            blnCalcolaSanzInt = True
                            sLogFase2 = sLogFase2 & "Sono presenti dichiarazioni, Dichiarato uguale a zero e Versato maggiore di zero$"
                        ElseIf SUMImpVersTOTALE = 0 Then                      'se Versato = 0
                            myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.NoAvviso
                            sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_OK
                            blnCalcolaSanzInt = False
                            sLogFase2 = sLogFase2 & "Sono presenti dichiarazioni, Dichiarato uguale a zero e Versato uguale a zero$"
                        End If
                    Case Else               'se NON CI SONO dichiarazioni/immobili
                        If SUMImpVersTOTALE > 0 Then                 'se Versato > 0, POTENZIALE RIMBORSO
                            myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.Rimborso
                            sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_RIMBORSO
                            blnCalcolaSanzInt = True
                            sLogFase2 = sLogFase2 & "Non ci sono dichiarazioni e Versato maggiore di zero$"
                        ElseIf SUMImpVersTOTALE = 0 Then                   'se Versato = 0, FASE 2 TERMINATA OK
                            myRow.COD_TIPO_PROVVEDIMENTO = OggettoAtto.Provvedimento.NoAvviso
                            sDescrStatoFase = COSTANTValue.CostantiProv.DESCR_TIPO_PROVVEDIMENTO_OK
                            blnCalcolaSanzInt = False
                            sLogFase2 = sLogFase2 & "Non ci sono dichiarazioni e Versato uguale a zero$"
                        End If
                End Select

                sLogFase2 = sLogFase2 & "Stato FASE 2: " & sDescrStatoFase & "$"
                Log.Debug("ConfrontoVersatoDichiarato::" & sLogFase2)
                ListBase.Add(myRow)

                Log.Debug("Fine Confronto Versato Dichiarato")

                Return CType(ListBase.ToArray(GetType(ObjBaseIntSanz)), ObjBaseIntSanz())
            Catch ex As Exception
                Log.Debug("ConfrontoVersatoDichiarato::si è verificato il seguente errore::", ex)
                Throw New Exception("Function::ConfrontoVersatoDichiarato::COMPlusProcessLiquidazioni:: " & ex.Message)
            End Try
        End Function

        Private Function SetObjDSAppoggioSanzioni() As DataSet

            Try

                Dim objDS As New DataSet


                Dim newTableAppoggio As DataTable
                newTableAppoggio = New DataTable("TP_APPOGGIO_CALCOLO_SANZIONI")

                Dim NewColumn As New DataColumn
                NewColumn.ColumnName = "ANNO"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = ""
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IVA"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IVS"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IVUS"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "IV"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "DI"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                NewColumn = New DataColumn
                NewColumn.ColumnName = "GG"
                NewColumn.DataType = System.Type.GetType("System.String")
                NewColumn.DefaultValue = System.DBNull.Value
                newTableAppoggio.Columns.Add(NewColumn)

                objDS.Tables.Add(newTableAppoggio)

                Return objDS

            Catch ex As Exception
                Throw New Exception("Function::ObjDSAppoggioSanzioni::COMPlusService:: " & ex.Message)
            End Try


        End Function
        Public Sub New()

        End Sub

    End Class

End Namespace