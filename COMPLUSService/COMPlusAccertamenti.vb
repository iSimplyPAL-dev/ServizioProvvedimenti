Imports System
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports log4net
Imports RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti
Imports RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti
Imports Utility
Imports ComPlusInterface

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la visualizzazione e gestione degli atti di accertamento
    ''' </summary>
    Friend Class COMPlusAccertamenti
        Private objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
        Private FncProvUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate
        Private objDBOPENgovIci As DBIci
        Private Shared Log As ILog = LogManager.GetLogger(GetType(COMPlusAccertamenti))

        '*** 20140509 - TASI ***
        Public Function getDichiarazioniCOMPlusAccertamenti(StringConnectionICI As String, StringConnectionGOV As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByRef ListSituazioneFinale() As objSituazioneFinale) As objUIICIAccert()
            Try
                Dim oReplace As New MotoreProvUtility()
                Dim myArray As New ArrayList()
                Dim ListUIAcc() As objUIICIAccert
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

                ''commentato Ale 07/09/2007
                ''Controllo se per il contribuente non ci sono Liquidazioni che devono
                ''ancora entrare in circolo perchè non sono passati 60gg. 
                ''Se ci sono blocco l'accertamento altrimenti ricerco in DB ICI se la posizione
                ''del contribuente è stata liquidata
                'objDs = objDBOPENgovProvvedimentiSelect.getDatiDichiarazioniAccertamenti(objHashTable)

                ''Cerco in DB ICI
                ''If objDs.Tables(0).DefaultView.Count > 0 Then
                ''	Return Nothing
                ''End If

                'Ale 07/09/2007
                'eseguo il freezer per annualizzare la situazione degli immobili
                Log.Debug("getDichiarazioniCOMPlusAccertamenti.richiamo il calcolo")
                Try
                    Dim bVersatoNelDovuto As Boolean = False
                    Dim bCalcolaArrotondamento As Boolean = False
                    Dim TipoCalcolo As Integer = 0 'calcolo standard '=1 calcolo netto versato
                    Dim remObjectFreezer As ComPlusInterface.IFreezer
                    remObjectFreezer = Activator.GetObject(GetType(ComPlusInterface.IFreezer), objHashTable("URLServiziFreezer"))
                    'Dim iRetValFreezer As Boolean = remObjectFreezer.CalcoloFromSoggetto(False, objHashTable, objHashTable("CONFIGURAZIONE_DICHIARAZIONE"), bVersatoNelDovuto, bCalcolaArrotondamento, TipoCalcolo, "", ListSituazioneFinale)
                    Dim iRetValFreezer As Boolean = remObjectFreezer.CalcoloFromSoggetto(StringConnectionGOV, StringConnectionICI, IdEnte, IdContribuente, objHashTable("TRIBUTOCALCOLO"), objHashTable("COD_TRIBUTO"), objHashTable("ANNODA"), objHashTable("ANNOA"), False, objHashTable("CONFIGURAZIONE_DICHIARAZIONE"), bVersatoNelDovuto, bCalcolaArrotondamento, TipoCalcolo, Utility.Costanti.TIPOTASI_PROPRIETARIO, objHashTable("TASIAPROPRIETARIO").ToString, "A", CType(objHashTable("USER"), String).ToUpper, ListSituazioneFinale)
                Catch ex As Exception
                    Log.Debug("getDichiarazioniCOMPlusAccertamenti::errore in freezer::", ex)
                End Try
                Log.Debug("getDichiarazioniCOMPlusAccertamenti.calcolo fatto")
                'Se trovo dichiarazioni che derivano dal processo di liquidazione (Pre Accertamento)
                'procedo con l'accertamento. Viene controllato nel DB ICI dalla
                'tabella TP_SITUAZIONE_VIRTUALE_DICHIARATO con tipologia 'L'

                objDBOPENgovIci = New DBIci             'DBIci 'COMPlusOPENgovProvvedimenti.DBIci
                Dim objDs As DataSet = objDBOPENgovIci.getSituazioneFinale(StringConnectionICI, IdEnte, IdContribuente, objHashTable)
                If objDs.Tables(0).DefaultView.Count <= 0 Then
                    Return Nothing
                Else
                    For Each myRow As DataRow In objDs.Tables(0).Rows
                        Dim myUI As New objUIICIAccert
                        If Not IsDBNull(myRow("id_situazione_finale")) Then
                            myUI.Id = myRow("id_situazione_finale")
                        End If
                        If Not IsDBNull(myRow("idlegame")) Then
                            myUI.IdLegame = myRow("idlegame")
                        End If
                        If Not IsDBNull(myRow("progressivo")) Then
                            myUI.Progressivo = myRow("progressivo")
                        End If
                        If Not IsDBNull(myRow("COD_CONTRIBUENTE")) Then
                            myUI.IdContribuente = myRow("COD_CONTRIBUENTE")
                        End If
                        If Not IsDBNull(myRow("ANNO")) Then
                            myUI.Anno = myRow("ANNO")
                        End If
                        If Not IsDBNull(myRow("CODTRIBUTO")) Then
                            myUI.Tributo = myRow("CODTRIBUTO")
                        End If
                        If Not IsDBNull(myRow("MESI_possesso")) Then
                            myUI.MesiPossesso = myRow("MESI_possesso")
                        End If
                        If Not IsDBNull(myRow("NUMERO_MESI_ACCONTO")) Then
                            myUI.AccMesi = myRow("NUMERO_MESI_ACCONTO")
                        End If
                        If Not IsDBNull(myRow("NUMERO_MESI_TOTALI")) Then
                            myUI.Mesi = myRow("NUMERO_MESI_TOTALI")
                        End If
                        If Not IsDBNull(myRow("NUMERO_UTILIZZATORI")) Then
                            myUI.NUtilizzatori = myRow("NUMERO_UTILIZZATORI")
                        End If
                        myUI.FlagPrincipale = 0
                        If Not IsDBNull(myRow("FLAG_PRINCIPALE")) Then
                            If myRow("FLAG_PRINCIPALE").ToString <> "0" Then
                                myUI.FlagPrincipale = 1
                            Else
                                If Not IsDBNull(myRow("COD_IMMOBILE_PERTINENZA")) Then
                                    If myRow("COD_IMMOBILE_PERTINENZA").ToString.Length > 0 Then
                                        If myRow("COD_IMMOBILE_PERTINENZA").ToString <> "-1" Then
                                            myUI.FlagPrincipale = 2
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        If Not IsDBNull(myRow("PERC_POSSESSO")) Then
                            myUI.PercPossesso = myRow("PERC_POSSESSO")
                        End If
                        If Not IsDBNull(myRow("COD_ENTE")) Then
                            myUI.IdEnte = myRow("COD_ENTE")
                        End If
                        If Not IsDBNull(myRow("CARATTERISTICA")) Then
                            myUI.Caratteristica = myRow("CARATTERISTICA")
                        End If
                        If Not IsDBNull(myRow("VIA")) Then
                            myUI.Via = myRow("VIA")
                        End If
                        If Not IsDBNull(myRow("NUMEROCIVICO")) Then
                            myUI.NCivico += " " & myRow("NUMEROCIVICO")
                        End If
                        If Not IsDBNull(myRow("SEZIONE")) Then
                            myUI.Sezione = myRow("SEZIONE")
                        End If
                        If Not IsDBNull(myRow("FOGLIO")) Then
                            myUI.Foglio = myRow("FOGLIO")
                        End If
                        If Not IsDBNull(myRow("NUMERO")) Then
                            myUI.Numero = myRow("NUMERO")
                        End If
                        If Not IsDBNull(myRow("SUBALTERNO")) Then
                            myUI.Subalterno = myRow("SUBALTERNO")
                        End If
                        If Not IsDBNull(myRow("CATEGORIA")) Then
                            myUI.Categoria = myRow("CATEGORIA")
                        End If
                        If Not IsDBNull(myRow("CLASSE")) Then
                            myUI.Classe = myRow("CLASSE")
                        End If
                        If Not IsDBNull(myRow("FLAG_STORICO")) Then
                            myUI.FlagStorico = myRow("FLAG_STORICO")
                        End If
                        If Not IsDBNull(myRow("FLAG_PROVVISORIO")) Then
                            myUI.FlagProvvisorio = myRow("FLAG_PROVVISORIO")
                        End If
                        If Not IsDBNull(myRow("MESI_POSSESSO")) Then
                            myUI.MesiPossesso = myRow("MESI_POSSESSO")
                        End If
                        If Not IsDBNull(myRow("MESI_ESCL_ESENZIONE")) Then
                            myUI.MesiEsenzione = myRow("MESI_ESCL_ESENZIONE")
                        End If
                        If Not IsDBNull(myRow("MESI_RIDUZIONE")) Then
                            myUI.MesiRiduzione = myRow("MESI_RIDUZIONE")
                        End If
                        If Not IsDBNull(myRow("IMPORTO_DETRAZIONE")) Then
                            myUI.ImpDetrazione = myRow("IMPORTO_DETRAZIONE")
                        End If
                        If Not IsDBNull(myRow("FLAG_POSSEDUTO")) Then
                            myUI.FlagPosseduto = myRow("FLAG_POSSEDUTO")
                        End If
                        If Not IsDBNull(myRow("FLAG_ESENTE")) Then
                            myUI.FlagEsente = myRow("FLAG_ESENTE")
                        End If
                        If Not IsDBNull(myRow("FLAG_RIDUZIONE")) Then
                            myUI.FlagRiduzione = myRow("FLAG_RIDUZIONE")
                        End If
                        If Not IsDBNull(myRow("ID")) Then
                            myUI.IdImmobile = myRow("ID")
                        End If
                        If Not IsDBNull(myRow("COD_IMMOBILE_PERTINENZA")) Then
                            myUI.IdImmobilePertinenza = myRow("COD_IMMOBILE_PERTINENZA")
                        End If
                        If Not IsDBNull(myRow("DAL")) Then
                            myUI.Dal = CDate(New MotoreProvUtility().GiraDataFromDB(myRow("DAL"))) 'oReplace.FormattaData(myRow("DAL"), "G")
                            myUI.DataInizio = New MotoreProvUtility().GiraDataFromDB(myRow("DAL"))
                        End If
                        If Not IsDBNull(myRow("AL")) Then
                            myUI.Al = CDate(New MotoreProvUtility().GiraDataFromDB(myRow("AL"))) 'oReplace.FormattaData(myRow("AL"), "G")
                        End If
                        If Not IsDBNull(myRow("TIPO_RENDITA")) Then
                            myUI.TipoRendita = myRow("TIPO_RENDITA")
                        End If
                        If Not IsDBNull(myRow("IDTIPOUTILIZZO")) Then
                            myUI.IdTipoUtilizzo = myRow("IDTIPOUTILIZZO")
                        End If
                        If Not IsDBNull(myRow("IDTIPOPOSSESSO")) Then
                            myUI.IdTipoPossesso = myRow("IDTIPOPOSSESSO")
                        End If
                        If Not IsDBNull(myRow("TITPOSSESSO")) Then
                            myUI.TitPossesso = myRow("TITPOSSESSO")
                        End If
                        '*** ***
                        If Not IsDBNull(myRow("ZONA")) Then
                            myUI.Zona = myRow("ZONA")
                        End If
                        If Not IsDBNull(myRow("consistenza")) Then
                            myUI.Consistenza = myRow("consistenza")
                        End If
                        If Not IsDBNull(myRow("ABITAZIONEPRINCIPALEATTUALE")) Then
                            myUI.AbitazionePrincipaleAttuale = myRow("ABITAZIONEPRINCIPALEATTUALE")
                        End If
                        If Not IsDBNull(myRow("RENDITA")) Then
                            myUI.Rendita = myRow("RENDITA")
                        End If
                        Dim FncValore As New ComPlusInterface.FncICI
                        If Not IsDBNull(myRow("COLTIVATOREDIRETTO")) Then
                            myUI.IsColtivatoreDiretto = myRow("COLTIVATOREDIRETTO")
                        Else
                            myUI.IsColtivatoreDiretto = False
                        End If
                        If Not IsDBNull(myRow("NUMEROFIGLI")) Then
                            myUI.NumeroFigli = myRow("NUMEROFIGLI")
                        End If
                        If Not IsDBNull(myRow("PERCENTCARICOFIGLI")) Then
                            myUI.PercentCaricoFigli = myRow("PERCENTCARICOFIGLI")
                        End If
                        Dim nValoreDich As Double = 0
                        If Not IsDBNull(myRow("valore")) Then
                            nValoreDich = myRow("valore")
                        End If
                        myUI.Valore = FncValore.CalcoloValore("SQL", StringConnectionGOV, StringConnectionICI, myUI.IdEnte, myUI.Anno, myUI.TipoRendita, myUI.Categoria, myUI.Classe, myUI.Zona, myUI.Rendita, nValoreDich, myUI.Consistenza, myUI.Dal, myUI.IsColtivatoreDiretto)
                        myUI.ValoreReale = myUI.Valore
                        If Not IsDBNull(myRow("TIPOTASI")) Then
                            myUI.TipoTasi = myRow("TIPOTASI")
                        End If
                        If Not IsDBNull(myRow("DESCRTIPOTASI")) Then
                            myUI.DescrTipoTasi = myRow("DESCRTIPOTASI")
                        End If
                        If Not IsDBNull(myRow("IDCONTRIBUENTECALCOLO")) Then
                            myUI.IdContribuenteCalcolo = myRow("IDCONTRIBUENTECALCOLO")
                        End If
                        If Not IsDBNull(myRow("ICI_ACCONTO_SENZA_DETRAZIONE")) Then
                            myUI.AccSenzaDetrazione = myRow("ICI_ACCONTO_SENZA_DETRAZIONE")
                        End If
                        If Not IsDBNull(myRow("ICI_ACCONTO_DETRAZIONE_APPLICATA")) Then
                            myUI.AccDetrazioneApplicata = myRow("ICI_ACCONTO_DETRAZIONE_APPLICATA")
                        End If
                        If Not IsDBNull(myRow("ICICALCOLATOACCONTO")) Then
                            myUI.AccDovuto = myRow("ICICALCOLATOACCONTO")
                        End If
                        If Not IsDBNull(myRow("ICI_ACCONTO_DETRAZIONE_RESIDUA")) Then
                            myUI.AccDetrazioneResidua = myRow("ICI_ACCONTO_DETRAZIONE_RESIDUA")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_SENZA_DETRAZIONE")) Then
                            myUI.SalSenzaDetrazione = myRow("ICI_TOTALE_SENZA_DETRAZIONE")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_DETRAZIONE_APPLICATA")) Then
                            myUI.SalDetrazioneApplicata = myRow("ICI_TOTALE_DETRAZIONE_APPLICATA")
                        End If
                        If Not IsDBNull(myRow("ICICALCOLATOSALDO")) Then
                            myUI.SalDovuto = myRow("ICICALCOLATOSALDO")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_DETRAZIONE_RESIDUA")) Then
                            myUI.SalDetrazioneResidua = myRow("ICI_TOTALE_DETRAZIONE_RESIDUA")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_SENZA_DETRAZIONE")) Then
                            myUI.TotSenzaDetrazione = myRow("ICI_DOVUTA_SENZA_DETRAZIONE")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_DETRAZIONE_SALDO")) Then
                            myUI.TotDetrazioneApplicata = myRow("ICI_DOVUTA_DETRAZIONE_SALDO")
                        End If
                        If Not IsDBNull(myRow("ICICALCOLATO")) Then
                            myUI.TotDovuto = myRow("ICICALCOLATO")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_DETRAZIONE_RESIDUA")) Then
                            myUI.TotDetrazioneResidua = myRow("ICI_DOVUTA_DETRAZIONE_RESIDUA")
                        End If
                        If Not IsDBNull(myRow("ID_ALIQUOTA")) Then
                            myUI.IdAliquota = myRow("ID_ALIQUOTA")
                        End If
                        If Not IsDBNull(myRow("ICI_VALORE_ALIQUOTA")) Then
                            myUI.Aliquota = myRow("ICI_VALORE_ALIQUOTA")
                        End If
                        If Not IsDBNull(myRow("ICI_VALORE_ALIQUOTA_STATALE")) Then
                            myUI.AliquotaStatale = myRow("ICI_VALORE_ALIQUOTA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_ACCONTO_STATALE")) Then
                            myUI.AccDovutoStatale = myRow("ICI_DOVUTA_ACCONTO_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_ACCONTO_DETRAZIONE_APPLICATA_STATALE")) Then
                            myUI.AccDetrazioneApplicataStatale = myRow("ICI_ACCONTO_DETRAZIONE_APPLICATA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_ACCONTO_DETRAZIONE_RESIDUA_STATALE")) Then
                            myUI.AccDetrazioneResiduaStatale = myRow("ICI_ACCONTO_DETRAZIONE_RESIDUA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_DOVUTA_STATALE")) Then
                            myUI.SalDovutoStatale = myRow("ICI_TOTALE_DOVUTA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_DETRAZIONE_APPLICATA_STATALE")) Then
                            myUI.SalDetrazioneApplicataStatale = myRow("ICI_TOTALE_DETRAZIONE_APPLICATA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_TOTALE_DETRAZIONE_RESIDUA_STATALE")) Then
                            myUI.SalDetrazioneResiduaStatale = myRow("ICI_TOTALE_DETRAZIONE_RESIDUA_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_SALDO_STATALE")) Then
                            myUI.TotDovutoStatale = myRow("ICI_DOVUTA_SALDO_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_DETRAZIONE_SALDO_STATALE")) Then
                            myUI.TotDetrazioneApplicataStatale = myRow("ICI_DOVUTA_DETRAZIONE_SALDO_STATALE")
                        End If
                        If Not IsDBNull(myRow("ICI_DOVUTA_DETRAZIONE_RESIDUA_STATALE")) Then
                            myUI.TotDetrazioneResiduaStatale = myRow("ICI_DOVUTA_DETRAZIONE_RESIDUA_STATALE")
                        End If
                        If Not IsDBNull(myRow("DIFFIMPOSTA")) Then
                            myUI.DiffImposta = myRow("DIFFIMPOSTA")
                        End If
                        If Not IsDBNull(myRow("TOTALE")) Then
                            myUI.Totale = myRow("TOTALE")
                        End If
                        myUI.TipoOperazione = objHashTable("TIPO_OPERAZIONE")
                        myUI.IdProcedimento = 0
                        myUI.IdRiferimento = 0
                        myUI.Provenienza = ""
                        myUI.Protocollo = "0"
                        myUI.MeseInizio = 0
                        myUI.DataScadenza = ""
                        myArray.Add(myUI)
                    Next
                    ListUIAcc = CType(myArray.ToArray(GetType(objUIICIAccert)), objUIICIAccert())

                    Return ListUIAcc
                End If
                'objDs = objDBOPENgovIci.getSituazioneVirtualeDichiaratoICI(objHashTable)
                ''Se ho una riga posso procedere nell'accertamento perchè il contribuente è stato liquidato per l'anno scelto da accertare
                'If objDs.Tables(0).DefaultView.Count > 0 Then
                '    Log.Debug("prelevo da getDatiImmobili")
                '    'Prelevo i dati della dichiarazione
                '    'Foglio, numero, subalterno...
                '    Dim n As Integer
                '    Dim arrayIDImmobili() As Integer
                '    ReDim arrayIDImmobili(objDs.Tables(0).DefaultView.Count - 1)

                '    For n = 0 To objDs.Tables(0).DefaultView.Count - 1
                '        arrayIDImmobili(n) = objDs.Tables(0).Rows(n).Item("ID_IMMOBILE")
                '    Next

                '    'Prelevo tutti i dati dell'immobile
                '    objDs = objDBOPENgovIci.getDatiImmobili(arrayIDImmobili, objHashTable)

                '    'Ritorno il DataSet con i dati della dichiarazione da caricare
                '    'nella griglia
                '    Return objDs
                'Else
                '    Log.Debug("prelevo da getSituazioneVirtualeDichiaratoICI")
                '    'Rimuovo il tipo provvedimento per la query perchè devo cercare se c'è stata
                '    'almeno una dichirazione per il contribuente. Se c'è stata non procedo all'
                '    'accertamento
                '    '?????
                '    'objHashTable.Remove("CODTIPOPROVVEDIMENTO")

                '    '?????
                '    objHashTable.Item("CODTIPOPROCEDIMENTO") = ""

                '    objDs = Nothing
                '    objDs = objDBOPENgovIci.getSituazioneVirtualeDichiaratoICI(objHashTable)

                '    'objDBOPENgovProvvedimentiSelect.DisposeObject(objDBOPENgovProvvedimentiSelect)
                '    'objDBOPENgovIci.DisposeObject(objDBOPENgovIci)
                '    'non posso procedere all'accertmento
                '    If objDs.Tables(0).DefaultView.Count > 0 Then
                '        Return Nothing
                '    Else
                '        Return objDs
                '    End If
                'End If
            Catch Err As Exception
                'Return Nothing
                Log.Debug("Function::ElaborazioneAccertamenti::COMPlusElaboraAccertamenti:: " & Err.Message)
                Throw New Exception("Function::ElaborazioneAccertamenti::COMPlusElaboraAccertamenti:: " & Err.Message)
            End Try
        End Function
        '*** ***
        ''' <summary>
        ''' verifico se per contrib e anno è stato effettuato un accertamento con data di conferma presente (definitivo)
        '''RETURN 0
        '''
        ''' verifico se per contrib e anno è stato effettuato un pre-accertamento con data conferma settata(definitivo)
        '''RETURN 1
        '''
        ''' verifico se per contrib e anno è stato effettuato un pre-accertamento con numero avviso non presente (atto potenziale)
        '''RETURN 2
        '''
        '''se non cado in uno di questi 3 casi, vuol dire che non ho effettuato nè ACCERTAMENTO nè PRE-ACCERTAMENTO
        '''RETURN 3
        '''
        ''' verifico se per contrib e anno è stato effettuato un accertamento con data di conferma NON presente (NON definitivo)
        '''RETURN 4
        '''
        ''' verifico se per contrib e anno è stato effettuato un accertamento con data di conferma NON presente (NON definitivo) e un preaccertamento con data di conferma NON presente  (atto potenziale)
        '''RETURN 5
        '''
        ''' verifico se per contrib e anno è stato effettuato un accertamento con data di conferma NON presente (NON definitivo) e un preaccertamento con data di conferma presente (definitivo)
        '''RETURN 6
        ''' </summary>
        ''' <param name="sAnno"></param>
        ''' <param name="strCodEnte"></param>
        ''' <param name="strCodContrib"></param>
        ''' <param name="strCodTributo"></param>
        ''' <param name="objHashTable"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        Public Function GetControlliPerElaborazioneAccertamento(StringConnectionProvv As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByRef objHashTable As Hashtable) As Integer
            Dim dsACC As New DataSet
            Dim dsPREACC As New DataSet
            Dim iACC As Integer
            Dim iPREACC As Integer
            Dim blnAccertamento_NON_DEFINITIVO As Boolean = False
            Dim IdProvvedimentoRettifica As Integer
            Try
                If objHashTable.ContainsKey("ID_PROVVEDIMENTO_RETTIFICA") Then
                    IdProvvedimentoRettifica = StringOperation.FormatInt(objHashTable("ID_PROVVEDIMENTO_RETTIFICA"))
                End If

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

                Log.Debug("inizio GetControlliPerElaborazioneAccertamento")
                If strCodTributo = Costanti.TRIBUTO_ICI Or strCodTributo = Costanti.TRIBUTO_TASI Then              'se il tributo è ICI
                    Log.Debug("tributo ici/TASI")
                    'verifico se per contrib e anno è stato effettuato un accertamento 
                    dsACC = objDBOPENgovProvvedimentiSelect.getControlloAccertamento(COSTANTValue.CostantiProv.DBType, StringConnectionProvv, sAnno, strCodEnte, strCodContrib, strCodTributo, IdProvvedimentoRettifica)
                    If dsACC.Tables(0).Rows.Count > 0 Then
                        'per contrib e anno sono stati effettuati ACCERTAMENTI...quindi:
                        For iACC = 0 To dsACC.Tables(0).Rows.Count - 1
                            If objHashTable.ContainsKey("ID_PROVVEDIMENTO_RETTIFICA") Then
                                If objHashTable("ID_PROVVEDIMENTO_RETTIFICA").ToString() <> "" Then
                                    If objHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
                                        objHashTable.Remove("DATA_ELABORAZIONE_PER_RETTIFICA")
                                    End If
                                End If
                            End If
                            If IsDBNull(dsACC.Tables(0).Rows(iACC)("DATA_ELABORAZIONE")) Then
                                objHashTable.Add("DATA_ELABORAZIONE_PER_RETTIFICA", "")
                            Else
                                objHashTable.Add("DATA_ELABORAZIONE_PER_RETTIFICA", dsACC.Tables(0).Rows(iACC)("DATA_ELABORAZIONE"))
                            End If

                            'se ACCERTAMENTO NON E' DEFINITIVO, ovvero
                            'se la data conferma è null
                            If IsDBNull(dsACC.Tables(0).Rows(iACC)("DATA_CONFERMA")) Then
                                'proseguo controllando se per contrib e anno 
                                'sono stati effettuati PREACCERTAMENTI
                                blnAccertamento_NON_DEFINITIVO = True
                            ElseIf CStr(dsACC.Tables(0).Rows(iACC)("DATA_CONFERMA")).CompareTo("") = 0 Then
                                'proseguo controllando se per contrib e anno 
                                'sono stati effettuati PREACCERTAMENTI
                                blnAccertamento_NON_DEFINITIVO = True
                            Else
                                'accertamento DEFINITIVO, esco e ritorno 0
                                dsACC.Dispose()
                                dsACC = Nothing
                                Return 0
                            End If
                        Next
                    End If

                    'per contrib e anno non sono stati effettuati ACCERTAMENTI...quindi
                    dsACC.Dispose()
                    dsACC = Nothing

                    '... verifico se per contrib e anno è stato effettuato un pre-accertamento                    
                    'se definitivo --> RETURN 1
                    'se potenziale --> RETURN 2

                    dsPREACC = objDBOPENgovProvvedimentiSelect.getControlloPreAccertamento(StringConnectionProvv, sAnno, strCodEnte, strCodContrib, objHashTable)
                    If dsPREACC.Tables(0).Rows.Count = 0 Then
                        'per contrib e anno non sono stati effettuati nè ACCERTAMENTI ne PREACCERTAMENTI, ritorno 3
                        dsPREACC.Dispose()
                        dsPREACC = Nothing
                        If blnAccertamento_NON_DEFINITIVO = True Then
                            Return 4
                        Else
                            Return 3
                        End If

                    Else

                        For iPREACC = 0 To dsPREACC.Tables(0).Rows.Count - 1
                            ''se il numero avviso è null
                            'If dsPREACC.Tables(0).Rows(iPREACC)("NUMERO_AVVISO") = "" Then
                            'se la data di conferma non è stata settata
                            If IsDBNull(dsPREACC.Tables(0).Rows(iPREACC)("DATA_CONFERMA")) Then
                                'pre accertamento NON DEFINITIVO, ritorno 2
                                dsPREACC.Dispose()
                                dsPREACC = Nothing
                                If blnAccertamento_NON_DEFINITIVO = True Then
                                    Return 5
                                Else
                                    Return 2
                                End If
                            ElseIf CStr(dsPREACC.Tables(0).Rows(iPREACC)("DATA_CONFERMA")).CompareTo("") = 0 Then
                                dsPREACC.Dispose()
                                dsPREACC = Nothing
                                If blnAccertamento_NON_DEFINITIVO = True Then
                                    Return 5
                                Else
                                    Return 2
                                End If
                            Else
                                'pre accertamento DEFINITIVO, ritorno 1
                                dsPREACC.Dispose()
                                dsPREACC = Nothing
                                If blnAccertamento_NON_DEFINITIVO = True Then
                                    Return 6
                                Else
                                    Return 1
                                End If
                            End If
                        Next
                    End If
                ElseIf strCodTributo = Costanti.TRIBUTO_TARSU Or strCodTributo = Costanti.TRIBUTO_OSAP Then                '*** 20130801 - accertamento OSAP ***				 ' se il tributo è TARSU
                    Log.Debug("tributo tarsu")
                    'verifico se per contrib e anno è stato effettuato solo un accertamento 
                    dsACC = objDBOPENgovProvvedimentiSelect.getControlloAccertamento(COSTANTValue.CostantiProv.DBType, StringConnectionProvv, sAnno, strCodEnte, strCodContrib, strCodTributo, IdProvvedimentoRettifica)
                    If dsACC.Tables(0).Rows.Count > 0 Then
                        'per contrib e anno sono stati effettuati ACCERTAMENTI...quindi:
                        For iACC = 0 To dsACC.Tables(0).Rows.Count - 1
                            If objHashTable.ContainsKey("ID_PROVVEDIMENTO_RETTIFICA") Then
                                If objHashTable("ID_PROVVEDIMENTO_RETTIFICA").ToString() <> "" Then
                                    If objHashTable.ContainsKey("DATA_ELABORAZIONE_PER_RETTIFICA") Then
                                        objHashTable.Remove("DATA_ELABORAZIONE_PER_RETTIFICA")
                                    End If
                                End If
                            End If
                            If IsDBNull(dsACC.Tables(0).Rows(iACC)("DATA_ELABORAZIONE")) Then
                                objHashTable.Add("DATA_ELABORAZIONE_PER_RETTIFICA", "")
                            Else
                                objHashTable.Add("DATA_ELABORAZIONE_PER_RETTIFICA", dsACC.Tables(0).Rows(iACC)("DATA_ELABORAZIONE"))
                            End If

                            'se ACCERTAMENTO NON E' DEFINITIVO, ovvero
                            'se la data conferma è null
                            If IsDBNull(dsACC.Tables(0).Rows(iACC)("DATA_CONFERMA")) Then
                                'proseguo controllando se per contrib e anno 
                                'sono stati effettuati PREACCERTAMENTI
                                Return 4
                            ElseIf CStr(dsACC.Tables(0).Rows(iACC)("DATA_CONFERMA")).CompareTo("") = 0 Then
                                'proseguo controllando se per contrib e anno 
                                'sono stati effettuati PREACCERTAMENTI
                                Return 4
                            Else
                                'accertamento DEFINITIVO, esco e ritorno 0
                                dsACC.Dispose()
                                dsACC = Nothing
                                Return 0
                            End If
                        Next
                    Else
                        Return 3
                    End If
                End If
                Log.Debug("fine")
            Catch Err As Exception
                'Return Nothing
                Log.Error("Function::GetControlliAccertamento::COMPlusElaboraAccertamenti:: " & Err.Message)
                Throw New Exception("Function::GetControlliAccertamento::COMPlusElaboraAccertamenti:: " & Err.Message)
            End Try
        End Function
        Public Function getSanzioniCOMPlusAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByRef ListBaseSanzInt() As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean) As DataSet
            Dim dsAnagrafica As New DataSet
            Dim sTributo As String = ""
            Dim CodTipoProc As String = ""
            Dim IdFase As Integer = -1
            If objHashTable.ContainsKey("CODTRIBUTO") Then
                sTributo = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
            ElseIf objHashTable.ContainsKey("COD_TRIBUTO") Then
                sTributo = StringOperation.FormatString(objHashTable("COD_TRIBUTO"))
            End If
            CodTipoProc = StringOperation.FormatString(objHashTable("COD_TIPO_PROCEDIMENTO"))
            If Not IsNothing(objHashTable("ID_FASE")) Then
                IdFase = StringOperation.FormatInt(objHashTable("ID_FASE"))
            End If

            Return New SanzInt().getCalcoloSanzioni(StringConnectionProvv, ListBaseSanzInt, objDSCalcoloSanzioniInteressiAppoggio, sTributo, objHashTableDati("IDSANZIONI"), IdEnte, objHashTable("TIPOPROVVEDIMENTO"), objHashTableDati("IDIMMOBILE"), idfase, CodTipoProc, objHashTable, dsAnagrafica, bConsentiSanzNeg, objHashTableDati("IDLEGAME"))
        End Function
        Public Function getInteressi(IdEnte As String, ByVal IdTributo As String, ByVal CodVoce As String, ByVal TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, ScadenzaAcconto As String, ScadenzaSaldo As String, IdLegame As Integer, ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni()
            Return New SanzInt().getCalcoloInteressi(IdEnte, IdTributo, CodVoce, TipoProvvedimento, TipoProcedimento, Fase, DataElaborazione, ScadenzaAcconto, ScadenzaSaldo, IdLegame, ListToCalc, myStringConnection)
        End Function
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
        Public Function getSanzioniICICOMPlusAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByRef oCalcoloSanzioni As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean, sDataMorte As String) As DataSet
            Dim objSanzInt As New SanzInt
            'alep 17012008
            Dim sTributo As String = ""
            If objHashTable.ContainsKey("CODTRIBUTO") Then
                sTributo = StringOperation.FormatString(objHashTable("CODTRIBUTO"))
            ElseIf objHashTable.ContainsKey("COD_TRIBUTO") Then
                sTributo = StringOperation.FormatString(objHashTable("COD_TRIBUTO"))
            End If
            getSanzioniICICOMPlusAccertamenti = objSanzInt.getCalcoloSanzioniICI(StringConnectionProvv, oCalcoloSanzioni, objDSCalcoloSanzioniInteressiAppoggio, sTributo, OggettoAtto.Capitolo.Sanzioni, objHashTableDati("IDSANZIONI"), IdEnte, objHashTable("TIPOPROVVEDIMENTO"), objHashTableDati("IDIMMOBILE"), objHashTable, sDataMorte, bConsentiSanzNeg, objHashTableDati("IDLEGAME"))

            Return getSanzioniICICOMPlusAccertamenti
        End Function
        ''' <summary>
        ''' Funzione che preleva l'anagrafica e richiama la funzione di inserimento
        ''' </summary>
        ''' <param name="myHashTable"></param>
        ''' <param name="ListCalcoloSanzioniInteressi"></param>
        ''' <param name="dsCalcoloSanzioni"></param>
        ''' <param name="ListInteressi"></param>
        ''' <param name="Spese"></param>
        ''' <param name="ListDichiarato"></param>
        ''' <param name="ListAccertato"></param>
        ''' <param name="dsSanzioniFase2"></param>
        ''' <param name="ListInteressiFase2"></param>
        ''' <param name="dsVersamentiF2"></param>
        ''' <param name="Operatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Public Function updateDBCOMPlusAccertamenti(myDBType As String, StringConnectionProvv As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As Hashtable, ByVal ListCalcoloSanzioniInteressi As ObjBaseIntSanz, ByVal dsCalcoloSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal Spese As Double, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal dsVersamentiF2 As DataSet, Operatore As String) As Integer
            Try
                If myHashTable.ContainsKey("DA") Then
                    myHashTable("DA") = ""
                Else
                    myHashTable.Add("DA", "")
                End If
                If myHashTable.ContainsKey("A") Then
                    myHashTable("A") = ""
                Else
                    myHashTable.Add("A", "")
                End If
                If myHashTable.ContainsKey("PARAMETROORDINAMENTOGRIGLIA") Then
                    myHashTable("PARAMETROORDINAMENTOGRIGLIA") = ""
                Else
                    myHashTable.Add("PARAMETROORDINAMENTOGRIGLIA", "")
                End If
                If myHashTable.ContainsKey("CodContribuente") Then
                    myHashTable("CodContribuente") = IdContribuente
                Else
                    myHashTable.Add("CodContribuente", IdContribuente)
                End If
                'Per dichiarato ICI
                If myHashTable.ContainsKey("ANNODA") Then
                    myHashTable("ANNODA") = myHashTable("ANNOACCERTAMENTO")
                Else
                    myHashTable.Add("ANNODA", myHashTable("ANNOACCERTAMENTO"))
                End If
                If myHashTable.ContainsKey("ANNOA") Then
                    myHashTable("ANNOA") = myHashTable("ANNOACCERTAMENTO")
                Else
                    myHashTable.Add("ANNOA", myHashTable("ANNOACCERTAMENTO"))
                End If

                Log.Debug("Prima della chiamata all'anagrafica IdContribuente = " + IdContribuente.ToString())

                Dim myAnag As New AnagInterface.DettaglioAnagrafica
                Dim FncAnag As New Anagrafica.DLL.GestioneAnagrafica
                myAnag = FncAnag.GetAnagrafica(IdContribuente, -1, "", myHashTable("DBType"), myHashTable("CONNECTIONSTRINGANAGRAFICA"), False)

                Log.Debug("Dopo la chiamata all'anagrafica IdContribuente = " + IdContribuente.ToString())

                FncProvUpdate = New DBOPENgovProvvedimentiUpdate

                updateDBCOMPlusAccertamenti = FncProvUpdate.SetProvvedimentiAccertamenti(myDBType, StringConnectionProvv, IdEnte, IdContribuente, myAnag, myHashTable, ListCalcoloSanzioniInteressi, dsCalcoloSanzioni, ListInteressi, ListDichiarato, ListAccertato, Spese, dsSanzioniFase2, ListInteressiFase2, dsVersamentiF2, Operatore)

                Return updateDBCOMPlusAccertamenti
            Catch ex As Exception
                Log.Error("COMPlusAccertamenti::updateDBCOMPlusAccertamenti::" & ex.Message)
                Return -1
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
        Public Function SetAtto(myDBType As String, ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt() As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ObjInteressiSanzioni() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As ObjArticoloAccertamento, ByVal objAccertatoTARSU() As ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As OggettoAddizionaleAccertamento, Operatore As String) As Integer
            Try
                Log.Debug("inizio")

                '********************************************************
                'DATI ANAGRAFICI DEL CONTRIBUENTE
                '********************************************************
                objHashTable.Add("DA", "")
                objHashTable.Add("A", "")
                objHashTable.Add("PARAMETROORDINAMENTOGRIGLIA", "")
                objHashTable.Add("CodContribuente", oAtto.COD_CONTRIBUENTE)
                If objHashTable.ContainsKey("CodENTE") = True Then
                    objHashTable.Remove("CodENTE")
                End If
                objHashTable.Add("CodENTE", oAtto.COD_ENTE)
                'Per dichiarato ICI
                objHashTable.Add("ANNODA", oAtto.ANNO)
                objHashTable.Add("ANNOA", oAtto.ANNO)

                Dim myAnag As New AnagInterface.DettaglioAnagrafica
                Dim FncAnag As New Anagrafica.DLL.GestioneAnagrafica
                myAnag = FncAnag.GetAnagrafica(oAtto.COD_CONTRIBUENTE, -1, "", myDBType, objHashTable("CONNECTIONSTRINGANAGRAFICA"), False)

                'con ListAnagrafica popolato devo aggiornare l'oggetto atto con i dati anagrafici
                'che mi servono per insert into PROVVEDIMENTI

                oAtto.CAP_RES = myAnag.CapResidenza
                oAtto.CITTA_RES = myAnag.ComuneResidenza
                oAtto.CIVICO_RES = myAnag.CivicoResidenza
                oAtto.CODICE_FISCALE = myAnag.CodiceFiscale
                oAtto.COGNOME = myAnag.Cognome
                oAtto.ESPONENTE_CIVICO_RES = myAnag.EsponenteCivicoResidenza
                oAtto.FRAZIONE_RES = myAnag.FrazioneResidenza
                oAtto.NOME = myAnag.Nome
                oAtto.PARTITA_IVA = myAnag.PartitaIva
                oAtto.POSIZIONE_CIVICO_RES = myAnag.PosizioneCivicoResidenza
                oAtto.PROVINCIA_RES = myAnag.ProvinciaResidenza
                oAtto.VIA_RES = myAnag.ViaResidenza

                Dim objDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect

                objHashTable.Remove("ANNODA")
                objHashTable.Remove("ANNOA")

                Dim ListRet() As ObjArticoloAccertamento
                ListRet = New DBOPENgovProvvedimentiUpdate().TARSU_SetProvvedimenti(myDBType, objHashTable, objSanzioni, Nothing, ObjInteressiSanzioni, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, oAddizionali, Operatore)
                If Not ListRet Is Nothing Then
                    Return ListRet(0).Id
                Else
                    Return -1
                End If
            Catch ex As Exception
                Log.Debug("COMPlusAccertamenti.SetAtto", ex)
                Return -1
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
        Public Function updateDBCOMPlusAccertamentiTARSU(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As ObjArticoloAccertamento, ByVal objAccertatoTARSU() As ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As OggettoAddizionaleAccertamento, Operatore As String) As ObjArticoloAccertamento()
            Try
                Log.Debug("inizio")
                objHashTable.Add("DA", "")
                objHashTable.Add("A", "")
                objHashTable.Add("PARAMETROORDINAMENTOGRIGLIA", "")
                objHashTable.Add("CodContribuente", IdContribuente)
                If objHashTable.ContainsKey("CodENTE") = True Then
                    objHashTable.Remove("CodENTE")
                End If
                objHashTable.Add("CodENTE", IdEnte)
                'Per dichiarato ICI
                objHashTable.Add("ANNODA", objHashTable("ANNOACCERTAMENTO"))
                objHashTable.Add("ANNOA", objHashTable("ANNOACCERTAMENTO"))

                Dim myAnag As New AnagInterface.DettaglioAnagrafica
                Dim FncAnag As New Anagrafica.DLL.GestioneAnagrafica
                myAnag = FncAnag.GetAnagrafica(IdContribuente, -1, "", myDBType, objHashTable("CONNECTIONSTRINGANAGRAFICA"), False)

                'con ListAnagrafica popolato devo aggiornare l'oggetto atto con i dati anagrafici
                'che mi servono per insert into PROVVEDIMENTI
                oAtto.CAP_RES = myAnag.CapResidenza
                oAtto.CITTA_RES = myAnag.ComuneResidenza
                oAtto.CIVICO_RES = myAnag.CivicoResidenza
                oAtto.CODICE_FISCALE = myAnag.CodiceFiscale
                oAtto.COGNOME = myAnag.Cognome
                oAtto.ESPONENTE_CIVICO_RES = myAnag.EsponenteCivicoResidenza
                oAtto.FRAZIONE_RES = myAnag.FrazioneResidenza
                oAtto.NOME = myAnag.Nome
                oAtto.PARTITA_IVA = myAnag.PartitaIva
                oAtto.POSIZIONE_CIVICO_RES = myAnag.PosizioneCivicoResidenza
                oAtto.PROVINCIA_RES = myAnag.ProvinciaResidenza
                oAtto.VIA_RES = myAnag.ViaResidenza

                Dim objDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect

                objHashTable.Remove("ANNODA")
                objHashTable.Remove("ANNOA")

                FncProvUpdate = New DBOPENgovProvvedimentiUpdate
                updateDBCOMPlusAccertamentiTARSU = FncProvUpdate.TARSU_SetProvvedimenti(myDBType, objHashTable, objSanzioni, Nothing, ListInteressi, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, oAddizionali, Operatore)

                Return updateDBCOMPlusAccertamentiTARSU
            Catch ex As Exception
                Log.Debug("Function::updateDBCOMPlusAccertamentiTARSU::COMPlusAccertamenti::" & " " & ex.Message)
                Throw New Exception("Function::updateDBCOMPlusAccertamentiTARSU::COMPlusAccertamenti::" & " " & ex.Message)
            End Try
        End Function

        Public Function getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv, objHashTable, ID_PROCEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try

        End Function

        Public Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv, IdEnte, objHashTable, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getAddizionaliPerStampaAccertamentiTARSU::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getAddizionaliPerStampaAccertamentiTARSU::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getVersamentiPerStampaAccertamentiTARSU::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getVersamentiPerStampaAccertamentiTARSU::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv, ID_PROCEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getImmobiliAccertatiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getImmobiliAccertatiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("COMPlusAccertamenti getImmobiliAccertatiPerStampaAccertamentiTARSU::" & ex.StackTrace)
                Throw New Exception("Function::getImmobiliAccertatiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try
                Log.Debug("Dentro COMPlusAccertamenti getImmobiliDichAccPerStampaAccertamentiTARSU")

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("COMPlusAccertamenti getImmobiliDichAccPerStampaAccertamentiTARSU::" & ex.StackTrace)
                Throw New Exception("Function::getImmobiliDichAccPerStampaAccertamentiTARSU::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv, IdEnte, objHashTable, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::GetElencoSanzioniPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::GetElencoSanzioniPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function


        Public Function GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::GetElencoInteressiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::GetElencoInteressiPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::GetInteressiTotaliPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::GetInteressiTotaliPerStampaAccertamenti::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function


        Public Sub New()

        End Sub
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
        ''' <param name="objDichiaratoOSAP"></param>
        ''' <param name="objAccertatoOSAP"></param>
        ''' <param name="spese"></param>
        ''' <param name="Operatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        Public Function updateDBCOMPlusAccertamentiOSAP(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As ComPlusInterface.OggettoAttoOSAP, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoOSAP() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal objAccertatoOSAP() As ComPlusInterface.OSAPAccertamentoArticolo, ByVal spese As Double, Operatore As String) As ComPlusInterface.OSAPAccertamentoArticolo()
            Try
                '********************************************************
                'DATI ANAGRAFICI DEL CONTRIBUENTE
                '********************************************************
                objHashTable.Add("DA", "")
                objHashTable.Add("A", "")
                objHashTable.Add("PARAMETROORDINAMENTOGRIGLIA", "")
                objHashTable.Add("CodContribuente", IdContribuente)
                If objHashTable.ContainsKey("CodENTE") = True Then
                    objHashTable.Remove("CodENTE")
                End If
                objHashTable.Add("CodENTE", IdEnte)
                'Per dichiarato ICI
                objHashTable.Add("ANNODA", objHashTable("ANNOACCERTAMENTO"))
                objHashTable.Add("ANNOA", objHashTable("ANNOACCERTAMENTO"))

                'ListAnagrafica = objAnagrafica.GetListAnagragrafica(objHashTable)
                Dim myAnag As New AnagInterface.DettaglioAnagrafica
                Dim FncAnag As New Anagrafica.DLL.GestioneAnagrafica
                myAnag = FncAnag.GetAnagrafica(IdContribuente, -1, "", myDBType, objHashTable("CONNECTIONSTRINGANAGRAFICA"), False)

                'con ListAnagrafica popolato devo aggiornare l'oggetto atto con i dati anagrafici
                'che mi servono per insert into PROVVEDIMENTI

                oAtto.CAP_RES = myAnag.CapResidenza
                oAtto.CITTA_RES = myAnag.ComuneResidenza
                oAtto.CIVICO_RES = myAnag.CivicoResidenza
                oAtto.CODICE_FISCALE = myAnag.CodiceFiscale
                oAtto.COGNOME = myAnag.Cognome
                oAtto.ESPONENTE_CIVICO_RES = myAnag.EsponenteCivicoResidenza
                oAtto.FRAZIONE_RES = myAnag.FrazioneResidenza
                oAtto.NOME = myAnag.Nome
                oAtto.PARTITA_IVA = myAnag.PartitaIva
                oAtto.POSIZIONE_CIVICO_RES = myAnag.PosizioneCivicoResidenza
                oAtto.PROVINCIA_RES = myAnag.ProvinciaResidenza
                oAtto.VIA_RES = myAnag.ViaResidenza

                Dim objDBOPENgovProvvedimentiSelect As New DBOPENgovProvvedimentiSelect

                objHashTable.Remove("ANNODA")
                objHashTable.Remove("ANNOA")

                'alep 23112007
                'ciclo il dataset degli immobili dichiarati ricevuto dal frontend (dsImmobiliDichiarato)
                'se trovo (per forza) lo stesso immobile nel dataset degli immobili dichiarati caricato da query
                'aggiorno l'id legame, che deve essere salvato nella tabella tp_Immobili_ACCERTAMENTI

                FncProvUpdate = New DBOPENgovProvvedimentiUpdate
                Dim myObj As ComPlusInterface.OSAPAccertamentoArticolo()
                Log.Debug("objHashTable:" + objHashTable.Count.ToString)
                If dsSanzioni.Tables.Count > 0 Then
                    Log.Debug("dsSanzioni:" + dsSanzioni.Tables(0).Rows.Count.ToString)
                End If
                If dsSanzioniImpDicVSImpPag.Tables.Count > 0 Then
                    Log.Debug("dsSanzioniImpDicVSImpPag:" + dsSanzioniImpDicVSImpPag.Tables(0).Rows.Count.ToString)
                End If
                If dsSanzioniScadDicVSDataPag.Tables.Count > 0 Then
                    Log.Debug("dsSanzioniScadDicVSDataPag:" + dsSanzioniScadDicVSDataPag.Tables(0).Rows.Count.ToString)
                End If
                If dsInteressi.Tables.Count > 0 Then
                    Log.Debug("dsInteressi:" + dsInteressi.Tables(0).Rows.Count.ToString)
                End If
                If dsInteressiImpDicVSImpPag.Tables.Count > 0 Then
                    Log.Debug("dsInteressiImpDicVSImpPag:" + dsInteressiImpDicVSImpPag.Tables(0).Rows.Count.ToString)
                End If
                If dsInteressiScadDicVSDataPag.Tables.Count > 0 Then
                    Log.Debug("dsInteressiScadDicVSDataPag:" + dsInteressiScadDicVSDataPag.Tables(0).Rows.Count.ToString)
                End If
                Log.Debug("oAtto:" + oAtto.IMPORTO_ACCERTATO_ACC.ToString())
                Log.Debug("oDettaglioAtto:" + oDettaglioAtto.Length.ToString)
                Log.Debug("objDichiaratoOSAP:" + objDichiaratoOSAP.Length.ToString)
                Log.Debug("objAccertatoOSAP:" + objAccertatoOSAP.Length.ToString)
                Log.Debug("spese:" + spese.ToString)
                myObj = FncProvUpdate.OSAP_SetProvvedimenti(myDBType, IdEnte, IdContribuente, objHashTable, dsSanzioni, dsSanzioniImpDicVSImpPag, dsSanzioniScadDicVSDataPag, dsInteressi, dsInteressiImpDicVSImpPag, dsInteressiScadDicVSDataPag, oAtto, objDichiaratoOSAP, objAccertatoOSAP, Operatore)

                Return myObj
            Catch ex As Exception
                Log.Debug("updateDBCOMPlusAccertamentiOSAP::si è verificato il seguente errore::" & ex.Message)
                Throw New Exception("updateDBCOMPlusAccertamentiOSAP::" & " " & ex.Message)
            End Try
        End Function


        Public Function getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getVersamentiPerStampaAccertamentiOSAP::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getVersamentiPerStampaAccertamentiOSAP::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function

        Public Function getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal TipoRicerca As String, ByVal ID_PROVVEDIMENTO As Long) As DataSet
            Try
                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv, TipoRicerca, ID_PROVVEDIMENTO)
                Return ds
            Catch ex As Exception
                Log.Debug("Function::getImmobiliDichAccPerStampaAccertamentiOSAP::COMPlusLiquidazioni::" & " " & ex.Message)
                Throw New Exception("Function::getImmobiliDichAccPerStampaAccertamentiOSAP::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try
        End Function
        '*** ***
    End Class
End Namespace
