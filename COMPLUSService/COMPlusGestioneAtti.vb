Imports System
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports log4net
Imports ComPlusInterface

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la gestione degli atti di accertamento
    ''' </summary>
    Friend Class COMPlusGestioneAtti
        Protected objUtility As New MotoreProvUtility
        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(COMPlusGestioneAtti))
        '*******************************************************************
        'variabili di istanza oggetti provenienti dal client
        'Hashtable
        '*******************************************************************
        Public m_objHashTable As Hashtable
        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect

        Public Function getAttiRicercaSemplice(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                getAttiRicercaSemplice = objDBOPENgovProvvedimentiSelect.getATTIRicercaSemplice(StringConnectionProvv, IdEnte, m_objHashTable)

                Return getAttiRicercaSemplice
            Catch ex As Exception
                Log.Debug("COMPlusGestioneAtti::GetAttiRicercaSemplice::errore::" & ex.Message)
                Throw New Exception("Function::getAttiRicercaSemplice::COMPlusGestioneAtti:: " & ex.Message)
            End Try
        End Function

        Public Function getProvvedimentiContribuente(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                getProvvedimentiContribuente = objDBOPENgovProvvedimentiSelect.getProvvedimentiContribuente(StringConnectionProvv, IdEnte, Utility.StringOperation.FormatString(m_objHashTable("COD_CONTRIBUENTE")), Utility.StringOperation.FormatString(m_objHashTable("ANNO")), Utility.StringOperation.FormatString(m_objHashTable("CODTRIBUTO")), Utility.StringOperation.FormatString(m_objHashTable("ID_PROVVEDIMENTO_RETTIFICA")))

                Return getProvvedimentiContribuente

            Catch ex As Exception
                Throw New Exception("Function::getProvvedimentiContribuente::COMPlusGestioneAtti:: " & ex.Message)
            End Try

        End Function

        Public Function getDatiProvvedimento_PerTipo(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                getDatiProvvedimento_PerTipo = objDBOPENgovProvvedimentiSelect.getDatiProvvedimento_PerTipo(StringConnectionProvv, IdEnte, m_objHashTable)

                Return getDatiProvvedimento_PerTipo

            Catch ex As Exception
                Throw New Exception("Function::getAttiRicercaSemplice::COMPlusGestioneAtti:: " & ex.Message)
            End Try

        End Function
        ''' <summary>
        ''' Funzione che preleva l'atto di accertamento
        ''' </summary>
        ''' <param name="myAtto">OggettoAtto oggetto da restituire</param>
        ''' <returns>DataSet con il record selezionato</returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        Public Function GetDatiProvvedimenti(StringConnectionProvv As String, ByRef myAtto As OggettoAtto) As DataSet
            myAtto = New OggettoAtto
            Try
                Dim dsDati As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                dsDati = objDBOPENgovProvvedimentiSelect.getLOAD_PROVVEDIMENTI(StringConnectionProvv, m_objHashTable)
                If Not dsDati Is Nothing Then
                    For Each myRow As DataRow In dsDati.Tables(0).Rows
                        myAtto.ID_PROVVEDIMENTO = Utility.StringOperation.FormatInt(myRow("ID_PROVVEDIMENTO"))
                        myAtto.COD_ENTE = Utility.StringOperation.FormatString(myRow("COD_ENTE"))
                        myAtto.NUMERO_AVVISO = Utility.StringOperation.FormatString(myRow("NUMERO_AVVISO"))
                        myAtto.NUMERO_ATTO = Utility.StringOperation.FormatString(myRow("NUMERO_ATTO"))
                        myAtto.COD_TRIBUTO = Utility.StringOperation.FormatString(myRow("COD_TRIBUTO"))
                        myAtto.DescrTributo = Utility.StringOperation.FormatString(myRow("DescrTributo"))
                        myAtto.TipoProvvedimento = Utility.StringOperation.FormatInt(myRow("TipoProvvedimento"))
                        myAtto.COD_CONTRIBUENTE = Utility.StringOperation.FormatInt(myRow("COD_CONTRIBUENTE"))
                        myAtto.COGNOME = Utility.StringOperation.FormatString(myRow("COGNOME"))
                        myAtto.NOME = Utility.StringOperation.FormatString(myRow("NOME"))
                        myAtto.CODICE_FISCALE = Utility.StringOperation.FormatString(myRow("CODICE_FISCALE"))
                        myAtto.PARTITA_IVA = Utility.StringOperation.FormatString(myRow("PARTITA_IVA"))
                        myAtto.VIA_RES = Utility.StringOperation.FormatString(myRow("VIA_RES"))
                        myAtto.POSIZIONE_CIVICO_RES = Utility.StringOperation.FormatString(myRow("POSIZIONE_CIVICO_RES"))
                        myAtto.CIVICO_RES = Utility.StringOperation.FormatString(myRow("CIVICO_RES"))
                        myAtto.ESPONENTE_CIVICO_RES = Utility.StringOperation.FormatString(myRow("ESPONENTE_CIVICO_RES"))
                        myAtto.CAP_RES = Utility.StringOperation.FormatString(myRow("CAP_RES"))
                        myAtto.FRAZIONE_RES = Utility.StringOperation.FormatString(myRow("FRAZIONE_RES"))
                        myAtto.CITTA_RES = Utility.StringOperation.FormatString(myRow("CITTA_RES"))
                        myAtto.PROVINCIA_RES = Utility.StringOperation.FormatString(myRow("PROVINCIA_RES"))
                        myAtto.CO = Utility.StringOperation.FormatString(myRow("CO"))
                        myAtto.VIA_CO = Utility.StringOperation.FormatString(myRow("VIA_CO"))
                        myAtto.POSIZIONE_CIVICO_CO = Utility.StringOperation.FormatString(myRow("POSIZIONE_CIVICO_CO"))
                        myAtto.CIVICO_CO = Utility.StringOperation.FormatString(myRow("CIVICO_CO"))
                        myAtto.ESPONENTE_CIVICO_CO = Utility.StringOperation.FormatString(myRow("ESPONENTE_CIVICO_CO"))
                        myAtto.CAP_CO = Utility.StringOperation.FormatString(myRow("CAP_CO"))
                        myAtto.FRAZIONE_CO = Utility.StringOperation.FormatString(myRow("FRAZIONE_CO"))
                        myAtto.CITTA_CO = Utility.StringOperation.FormatString(myRow("CITTA_CO"))
                        myAtto.PROVINCIA_CO = Utility.StringOperation.FormatString(myRow("PROVINCIA_CO"))
                        myAtto.IMPORTO_DIFFERENZA_IMPOSTA = Utility.StringOperation.FormatDouble(myRow("IMPORTO_DIFFERENZA_IMPOSTA"))
                        myAtto.IMPORTO_SANZIONI = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SANZIONI"))
                        myAtto.IMPORTO_SANZIONI_RIDOTTO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SANZIONI_RIDOTTO"))
                        myAtto.IMPORTO_TOT_SANZIONI_RIDUCIBILI = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOT_SANZIONI_RIDUCIBILI"))
                        myAtto.IMPORTO_TOT_SANZIONI_RIDOTTE = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOT_SANZIONI_RIDOTTE"))
                        myAtto.IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI"))
                        myAtto.IMPORTO_INTERESSI = Utility.StringOperation.FormatDouble(myRow("IMPORTO_INTERESSI"))
                        myAtto.IMPORTO_SPESE = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SPESE"))
                        myAtto.IMPORTO_ALTRO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ALTRO"))
                        myAtto.IMPORTO_TOTALE = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOTALE"))
                        myAtto.IMPORTO_ARROTONDAMENTO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ARROTONDAMENTO"))
                        myAtto.IMPORTO_TOTALE_RIDOTTO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOTALE_RIDOTTO"))
                        myAtto.IMPORTO_ARROTONDAMENTO_RIDOTTO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ARROTONDAMENTO_RIDOTTO"))
                        myAtto.IMPORTO_SENZA_ARROTONDAMENTO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SENZA_ARROTONDAMENTO"))
                        myAtto.DATA_CONSEGNA_AVVISO = Utility.StringOperation.FormatString(myRow("DATA_CONSEGNA_AVVISO"))
                        myAtto.DATA_NOTIFICA_AVVISO = Utility.StringOperation.FormatString(myRow("DATA_NOTIFICA_AVVISO"))
                        myAtto.DATA_RETTIFICA_AVVISO = Utility.StringOperation.FormatString(myRow("DATA_RETTIFICA_AVVISO"))
                        myAtto.DATA_ANNULLAMENTO_AVVISO = Utility.StringOperation.FormatString(myRow("DATA_ANNULLAMENTO_AVVISO"))
                        myAtto.DATA_PERVENUTO_IL = Utility.StringOperation.FormatString(myRow("DATA_PERVENUTO_IL"))
                        myAtto.DATA_SCADENZA_QUESTIONARIO = Utility.StringOperation.FormatString(myRow("DATA_SCADENZA_QUESTIONARIO"))
                        myAtto.DATA_RIMBORSO = Utility.StringOperation.FormatString(myRow("DATA_RIMBORSO"))
                        myAtto.DATA_SOSPENSIONE_AVVISO_AUTOTUTELA = Utility.StringOperation.FormatString(myRow("DATA_SOSPENSIONE_AVVISO_AUTOTUTELA"))
                        myAtto.DATA_PRESENTAZIONE_RICORSO = Utility.StringOperation.FormatString(myRow("DATA_PRESENTAZIONE_RICORSO"))
                        myAtto.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA = Utility.StringOperation.FormatString(myRow("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA"))
                        myAtto.DATA_SENTENZA = Utility.StringOperation.FormatString(myRow("DATA_SENTENZA"))
                        myAtto.DATA_ATTO_DEFINITIVO = Utility.StringOperation.FormatString(myRow("DATA_ATTO_DEFINITIVO"))
                        myAtto.DATA_VERSAMENTO_SOLUZIONE_UNICA = Utility.StringOperation.FormatString(myRow("DATA_VERSAMENTO_SOLUZIONE_UNICA"))
                        myAtto.DATA_CONCESSIONE_RATEIZZAZIONE = Utility.StringOperation.FormatString(myRow("DATA_CONCESSIONE_RATEIZZAZIONE"))
                        myAtto.IMPORTO_PAGATO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_PAGATO"))
                        myAtto.DATA_ELABORAZIONE = Utility.StringOperation.FormatString(myRow("DATA_ELABORAZIONE"))
                        myAtto.DATA_CONFERMA = Utility.StringOperation.FormatString(myRow("DATA_CONFERMA"))
                        myAtto.DATA_STAMPA = Utility.StringOperation.FormatString(myRow("DATA_STAMPA"))
                        myAtto.DATA_SOLLECITO_BONARIO = Utility.StringOperation.FormatString(myRow("DATA_SOLLECITO_BONARIO"))
                        myAtto.DATA_RUOLO_ORDINARIO_TARSU = Utility.StringOperation.FormatString(myRow("DATA_RUOLO_ORDINARIO_TARSU"))
                        myAtto.DATA_COATTIVO = Utility.StringOperation.FormatString(myRow("DATA_COATTIVO"))
                        myAtto.DATA_PRESENTAZIONE_RICORSO_REGIONALE = Utility.StringOperation.FormatString(myRow("DATA_PRESENTAZIONE_RICORSO_REGIONALE"))
                        myAtto.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE = Utility.StringOperation.FormatString(myRow("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE"))
                        myAtto.DATA_SENTENZA_REGIONALE = Utility.StringOperation.FormatString(myRow("DATA_SENTENZA_REGIONALE"))
                        myAtto.DATA_PRESENTAZIONE_RICORSO_CASSAZIONE = Utility.StringOperation.FormatString(myRow("DATA_PRESENTAZIONE_RICORSO_CASSAZIONE"))
                        myAtto.DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE = Utility.StringOperation.FormatString(myRow("DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE"))
                        myAtto.DATA_SENTENZA_CASSAZIONE = Utility.StringOperation.FormatString(myRow("DATA_SENTENZA_CASSAZIONE"))
                        myAtto.PROGRESSIVO_ELABORAZIONE = Utility.StringOperation.FormatInt(myRow("PROGRESSIVO_ELABORAZIONE"))
                        myAtto.NOTE_PROVINCIALE = Utility.StringOperation.FormatString(myRow("NOTE_PROVINCIALE"))
                        myAtto.NOTE_REGIONALE = Utility.StringOperation.FormatString(myRow("NOTE_REGIONALE"))
                        myAtto.NOTE_CASSAZIONE = Utility.StringOperation.FormatString(myRow("NOTE_CASSAZIONE"))
                        myAtto.ESITO_ACCERTAMENTO = Utility.StringOperation.FormatInt(myRow("ESITO_ACCERTAMENTO"))
                        myAtto.TERMINE_RICORSO_ACC = Utility.StringOperation.FormatString(myRow("TERMINE_RICORSO_ACC"))
                        myAtto.NOTE_ACCERTAMENTO = Utility.StringOperation.FormatString(myRow("NOTE_ACCERTAMENTO"))
                        myAtto.NOTE_CONCILIAZIONE_G = Utility.StringOperation.FormatString(myRow("NOTE_CONCILIAZIONE_G"))
                        myAtto.FLAG_ACCERTAMENTO = Utility.StringOperation.FormatBool(myRow("FLAG_ACCERTAMENTO"))
                        myAtto.FLAG_CONCILIAZIONE_G = Utility.StringOperation.FormatBool(myRow("FLAG_CONCILIAZIONE_G"))
                        myAtto.IMPORTO_RUOLO_COATTIVO = Utility.StringOperation.FormatDouble(myRow("IMPORTO_RUOLO_COATTIVO"))
                        myAtto.NOTE_GENERALI_ATTO = Utility.StringOperation.FormatString(myRow("NOTE_GENERALI_ATTO"))
                        myAtto.IMPORTO_ADDCOM = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ADDCOM"))
                        myAtto.IMPORTO_ADDPROV = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ADDPROV"))
                        myAtto.IMPORTO_DICHIARATO_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_DICHIARATO_F2"))
                        myAtto.IMPORTO_VERSATO_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_VERSATO_F2"))
                        myAtto.IMPORTO_DIFFERENZA_IMPOSTA_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_DIFFERENZA_IMPOSTA_F2"))
                        myAtto.IMPORTO_SANZIONI_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SANZIONI_F2"))
                        myAtto.IMPORTO_INTERESSI_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_INTERESSI_F2"))
                        myAtto.IMPORTO_TOTALE_F2 = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOTALE_F2"))
                        myAtto.IMPORTO_ACCERTATO_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_ACCERTATO_ACC"))
                        myAtto.IMPORTO_DIFFERENZA_IMPOSTA_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_DIFFERENZA_IMPOSTA_ACC"))
                        myAtto.IMPORTO_SANZIONI_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SANZIONI_ACC"))
                        myAtto.IMPORTO_SANZIONI_RIDOTTE_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_SANZIONI_RIDOTTE_ACC"))
                        myAtto.IMPORTO_INTERESSI_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_INTERESSI_ACC"))
                        myAtto.IMPORTO_TOTALE_ACC = Utility.StringOperation.FormatDouble(myRow("IMPORTO_TOTALE_ACC"))
                        myAtto.NOMEPDF = Utility.StringOperation.FormatString(myRow("NOMEPDF"))
                        myAtto.DATA_RIENTRO = Utility.StringOperation.FormatDateTime(myRow("DATA_RIENTRO"))
                        myAtto.DATA_IRREPERIBILE = Utility.StringOperation.FormatString(myRow("DATA_IRREPERIBILE"))
                        myAtto.IDRUOLO = Utility.StringOperation.FormatInt(myRow("IDRUOLO"))
                        myAtto.ANNO = Utility.StringOperation.FormatString(myRow("ANNO"))
                        myAtto.Provenienza = Utility.StringOperation.FormatInt(myRow("Provenienza"))
                    Next
                End If
                Return dsDati
            Catch ex As Exception
                Throw New Exception("Function::GetDatiProvvedimenti::COMPlusGestioneAtti:: " & ex.Message)
            End Try
        End Function



        Public Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv As String, ByRef NUMERO_ATTO As String) As Boolean
            Try
                setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA = False
                Dim objDBOPENgovProvvedimentiUpdate As New COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

                setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA = objDBOPENgovProvvedimentiUpdate.SetPROVVEDIMENTOATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv, m_objHashTable, NUMERO_ATTO)

                Return setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA
            Catch ex As Exception
                Throw New Exception("Function::setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA::COMPlusGestioneAtti:" & ex.Message)
            End Try
        End Function
        Public Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv As String) As Boolean
            Try


                SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = False



                Dim objDBOPENgovProvvedimentiUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate
                objDBOPENgovProvvedimentiUpdate = New COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

                SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = objDBOPENgovProvvedimentiUpdate.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv, m_objHashTable)

                Return SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO

            Catch ex As Exception
                Throw New Exception("Function::SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO::COMPlusGestioneAtti:: " & ex.Message)
            End Try

        End Function
        Public Function setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv As String, ByVal objSELEZIONE_DATASET As DataSet) As Long
            Try


                setDATE_PROVVEDIMENTI_MASSIVA = 0



                Dim objDBOPENgovProvvedimentiUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate
                objDBOPENgovProvvedimentiUpdate = New COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

                setDATE_PROVVEDIMENTI_MASSIVA = objDBOPENgovProvvedimentiUpdate.setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv, m_objHashTable, objSELEZIONE_DATASET)

                Return setDATE_PROVVEDIMENTI_MASSIVA

            Catch ex As Exception
                Throw New Exception("Function::setPROVVEDIMENTO_ATTO_LIQUIDAZIONE::COMPlusGestioneAtti:: " & ex.Message)
            End Try

        End Function


        Public Function GetDatiAttiRicercaAvanzata(StringConnectionProvv As String, IdEnte As String, ParamSearch As ComPlusInterface.ObjSearchAtti) As DataSet
            Try
                Dim strSQL As String
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect

                strSQL = generaSQLRicercaAvanzata(ParamSearch)

                GetDatiAttiRicercaAvanzata = objDBOPENgovProvvedimentiSelect.GetDatiAttiRicercaAvanzata(StringConnectionProvv, IdEnte, m_objHashTable, strSQL)

                Return GetDatiAttiRicercaAvanzata
            Catch ex As Exception
                Throw New Exception("Function::GetDatiAttiRicercaAvanzata::COMPlusGestioneAtti:: " & ex.Message)
            End Try

        End Function
        Private Function generaSQLRicercaAvanzata(ByVal mySearch As ComPlusInterface.ObjSearchAtti) As String
            Dim strSQL As String
            Try
                strSQL = getStringSQLRicercaAvanzata(mySearch.Generazione.TipoRic, mySearch.Generazione.Dal, mySearch.Generazione.Al, "DATA_ELABORAZIONE")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.ConfermaAvviso.TipoRic, mySearch.ConfermaAvviso.Dal, mySearch.ConfermaAvviso.Al, "DATA_CONFERMA")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.StampaAvviso.TipoRic, mySearch.StampaAvviso.Dal, mySearch.StampaAvviso.Al, "DATA_STAMPA")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.ConsegnaAvviso.TipoRic, mySearch.ConsegnaAvviso.Dal, mySearch.ConsegnaAvviso.Al, "DATA_CONSEGNA_AVVISO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.NotificaAvviso.TipoRic, mySearch.NotificaAvviso.Dal, mySearch.NotificaAvviso.Al, "DATA_NOTIFICA_AVVISO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.RettificaAvviso.TipoRic, mySearch.RettificaAvviso.Dal, mySearch.RettificaAvviso.Al, "DATA_RETTIFICA_AVVISO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.AnnullamentoAvviso.TipoRic, mySearch.AnnullamentoAvviso.Dal, mySearch.AnnullamentoAvviso.Al, "DATA_ANNULLAMENTO_AVVISO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.SospensioneAutotutela.TipoRic, mySearch.SospensioneAutotutela.Dal, mySearch.SospensioneAutotutela.Al, "DATA_SOSPENSIONE_AVVISO_AUTOTUTELA")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.AttoDefinitivo.TipoRic, mySearch.AttoDefinitivo.Dal, mySearch.AttoDefinitivo.Al, "DATA_ATTO_DEFINITIVO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.Pagamento.TipoRic, mySearch.Pagamento.Dal, mySearch.Pagamento.Al, "DATA_PAGAMENTO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.SollecitoBonario.TipoRic, mySearch.SollecitoBonario.Dal, mySearch.SollecitoBonario.Al, "DATA_SOLLECITO_BONARIO")
                strSQL = strSQL & getStringSQLRicercaAvanzata(mySearch.Coattivo.TipoRic, mySearch.Coattivo.Dal, mySearch.Coattivo.Al, "DATA_COATTIVO")
            Catch ex As Exception
                Log.Debug("generaSQLRicercaAvanzata.si è verificato un errore::   ", ex)
                strSQL = ""
            End Try
            Return strSQL
        End Function
        Private Function getStringSQLRicercaAvanzata(TipoRicerca As Integer, ByVal Dal As Date, ByVal Al As Date, ByVal FieldName As String) As String
            Dim strSql As String
            Dim oReplace As New MotoreProvUtility()
            strSql = ""
            Try
                Log.Debug("getStringSQLRicercaAvanzata::strFIELDName::" & FieldName)
                If TipoRicerca = ComPlusInterface.ObjSearchAttiAvanzataDate.DateNessuna Then
                    '*****************************NESSUNA DATA*************************************
                    strSql += " AND (" & FieldName & " IS NULL OR " & FieldName & " = ''" & ")"
                Else
                    If Dal = Date.MaxValue And Al <> Date.MaxValue Then
                        strSql += " AND (" & FieldName & "<='" & oReplace.FormattaData(Al, "A") & "')"
                    End If
                    If Dal <> Date.MaxValue And Al <> Date.MaxValue Then
                        strSql += " AND (" & FieldName & ">='" & oReplace.FormattaData(Dal, "A") & "' AND " & FieldName & "<='" & oReplace.FormattaData(Al, "A") & "')"
                    End If
                    If Dal <> Date.MaxValue And Al = Date.MaxValue Then
                        strSql += " AND (" & FieldName & ">='" & oReplace.FormattaData(Dal, "A") & "')"
                    End If
                End If
                Log.Debug("getStringSQLRicercaAvanzata::filtro::" & strSql)
            Catch ex As Exception
                Log.Debug("getStringSQLRicercaAvanzata.si è verificato un errore::", ex)
                strSql = ""
            End Try
            Return strSql
        End Function

        Public Sub New(ByVal objHashTable As Hashtable)
            m_objHashTable = objHashTable
        End Sub
    End Class
End Namespace
