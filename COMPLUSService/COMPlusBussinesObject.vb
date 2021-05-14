Imports System
Imports System.EnterpriseServices
Imports System.Diagnostics
Imports RIBESFrameWork
Imports System.Messaging
Imports log4net
Imports ComPlusInterface
Imports RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti
Imports RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' La classe sarà da utilizzare per la gestione degli Oggetti necessari a OPENgovProvvedimenti
    ''' </summary>
    Public Class COMPlusBusinessObject
        '*******************************************************************
        'variabili di istanza oggetti provenienti dal client
        'Hashtable
        'RIBESFrameWork.Session (sessione del FrameWork
        '*******************************************************************
        Public m_objHashTable As Hashtable
        Public m_objSession As RIBESFrameWork.Session
        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(COMPlusBusinessObject))

#Region "Gestione ITER LIQUIDAZIONI"
        <AutoComplete()>
        Public Function getProvvedimentoPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet

            Try

                Dim objCOMPlusLiquidazioni As New COMPlusLiquidazioni(m_objHashTable)


                getProvvedimentoPerStampaLiquidazione = Nothing
                getProvvedimentoPerStampaLiquidazione = objCOMPlusLiquidazioni.getProvvedimentoPerStampaLiquidazione(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getProvvedimentoPerStampaLiquidazione

            Catch ex As Exception
                Throw New Exception("Function::getProvvedimentoPerStampaLiquidazione::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getVersamentiPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet

            Try

                Dim objCOMPlusLiquidazioni As New COMPlusLiquidazioni(m_objHashTable)


                getVersamentiPerStampaLiquidazione = Nothing
                getVersamentiPerStampaLiquidazione = objCOMPlusLiquidazioni.getVersamentiPerStampaLiquidazione(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getVersamentiPerStampaLiquidazione

            Catch ex As Exception
                Throw New Exception("Function::getProvvedimentoPerStampaLiquidazione::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                GetElencoInteressiPerStampaAccertamenti = Nothing
                GetElencoInteressiPerStampaAccertamenti = objCOMPlusAccertamenti.GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return GetElencoInteressiPerStampaAccertamenti

            Catch ex As Exception
                Throw New Exception("Function::GetElencoInteressiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                GetInteressiTotaliPerStampaAccertamenti = Nothing
                GetInteressiTotaliPerStampaAccertamenti = objCOMPlusAccertamenti.GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return GetInteressiTotaliPerStampaAccertamenti

            Catch ex As Exception
                Throw New Exception("Function::GetInteressiTotaliPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusLiquidazioni As New COMPlusLiquidazioni(m_objHashTable)


                GetElencoInteressiPerStampaLiquidazione = Nothing
                GetElencoInteressiPerStampaLiquidazione = objCOMPlusLiquidazioni.GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return GetElencoInteressiPerStampaLiquidazione

            Catch ex As Exception
                Throw New Exception("Function::GetElencoInteressiPerStampaLiquidazione::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusLiquidazioni As New COMPlusLiquidazioni(m_objHashTable)


                GetInteressiTotaliPerStampaLiquidazione = Nothing
                GetInteressiTotaliPerStampaLiquidazione = objCOMPlusLiquidazioni.GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return GetInteressiTotaliPerStampaLiquidazione

            Catch ex As Exception
                Throw New Exception("Function::GetInteressiTotaliPerStampaLiquidazione::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusLiquidazioni As New COMPlusLiquidazioni(m_objHashTable)


                GetElencoSanzioniPerStampaLiquidazione = Nothing
                GetElencoSanzioniPerStampaLiquidazione = objCOMPlusLiquidazioni.GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv, IdEnte, m_objHashTable, ID_PROVVEDIMENTO)

                Return GetElencoSanzioniPerStampaLiquidazione

            Catch ex As Exception
                Throw New Exception("Function::GetElencoSanzioniPerStampaLiquidazione::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getImmobiliDichiaratiPerStampaAccertamenti = Nothing
                getImmobiliDichiaratiPerStampaAccertamenti = objCOMPlusAccertamenti.getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv, objHashTable, ID_PROCEDIMENTO)

                Return getImmobiliDichiaratiPerStampaAccertamenti

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getImmobiliDichiaratiPerStampaAccertamentiTARSU = Nothing
                getImmobiliDichiaratiPerStampaAccertamentiTARSU = objCOMPlusAccertamenti.getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv, IdEnte, objHashTable, ID_PROCEDIMENTO)

                Return getImmobiliDichiaratiPerStampaAccertamentiTARSU

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getImmobiliDichAccPerStampaAccertamentiTARSU = Nothing
                getImmobiliDichAccPerStampaAccertamentiTARSU = objCOMPlusAccertamenti.getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getImmobiliDichAccPerStampaAccertamentiTARSU

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getAddizionaliPerStampaAccertamentiTARSU = Nothing
                getAddizionaliPerStampaAccertamentiTARSU = objCOMPlusAccertamenti.getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getAddizionaliPerStampaAccertamentiTARSU

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliDichiaratiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getVersamentiPerStampaAccertamentiTARSU = Nothing
                getVersamentiPerStampaAccertamentiTARSU = objCOMPlusAccertamenti.getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getVersamentiPerStampaAccertamentiTARSU

            Catch ex As Exception
                Throw New Exception("Function::getVersamentiPerStampaAccertamentiTARSU::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getImmobiliAccertatiPerStampaAccertamenti = Nothing
                getImmobiliAccertatiPerStampaAccertamenti = objCOMPlusAccertamenti.getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getImmobiliAccertatiPerStampaAccertamenti

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliAccertatiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti


                getImmobiliAccertatiPerStampaAccertamentiTARSU = Nothing
                getImmobiliAccertatiPerStampaAccertamentiTARSU = objCOMPlusAccertamenti.getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getImmobiliAccertatiPerStampaAccertamentiTARSU

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliAccertatiPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti

                GetElencoSanzioniPerStampaAccertamenti = Nothing
                GetElencoSanzioniPerStampaAccertamenti = objCOMPlusAccertamenti.GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv, IdEnte, m_objHashTable, ID_PROVVEDIMENTO)

                Return GetElencoSanzioniPerStampaAccertamenti

            Catch ex As Exception
                Throw New Exception("Function::GetElencoSanzioniPerStampaAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
#End Region

#Region "Gestione ITER ACCERTAMENTI"
        Public Function getDatiDichiarazioniAccertamenti(StringConnectionICI As String, StringConnectionGOV As String, IdEnte As String, IdContribuente As Integer, ByRef ListSituazioneFinale() As objSituazioneFinale) As objUIICIAccert()
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti
                Return objCOMPlusAccertamenti.getDichiarazioniCOMPlusAccertamenti(StringConnectionICI, StringConnectionGOV, IdEnte, IdContribuente, m_objHashTable, ListSituazioneFinale)
            Catch ex As Exception
                Throw New Exception("Function::getDatiDichiarazioniAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function
        Public Function GetControlliAccertamento(StringConnectionProvv As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByRef objHashTable As Hashtable) As Integer
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti

                GetControlliAccertamento = objCOMPlusAccertamenti.GetControlliPerElaborazioneAccertamento(StringConnectionProvv, sAnno, strCodEnte, strCodContrib, strCodTributo, objHashTable)
                Return GetControlliAccertamento
            Catch ex As Exception
                Throw New Exception("Function::GetControlliAccertamento::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function
        Public Function getSanzioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTableDati As Hashtable, ByRef ListBaseCalcolo() As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean) As DataSet
            Try
                Return New COMPlusAccertamenti().getSanzioniCOMPlusAccertamenti(StringConnectionProvv, IdEnte, m_objHashTable, objHashTableDati, ListBaseCalcolo, objDSCalcoloSanzioniInteressiAppoggio, bConsentiSanzNeg)
            Catch ex As Exception
                Throw New Exception("Function::getSanzioni::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function
        Public Function getInteressi(IdEnte As String, IdTributo As String, CodVoce As String, TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, ScadenzaAcconto As String, ScadenzaSaldo As String, IdLegame As Integer, ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni()
            Try
                Return New COMPlusAccertamenti().getInteressi(IdEnte, IdTributo, CodVoce, TipoProvvedimento, TipoProcedimento, Fase, DataElaborazione, ScadenzaAcconto, ScadenzaSaldo, IdLegame, ListToCalc, myStringConnection)
            Catch ex As Exception
                Throw New Exception("COMPlusBusinessObject.getInteressi.errore::", ex)
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="objHashTableDati"></param>
        ''' <param name="oCalcoloSanzioni"></param>
        ''' <param name="objDSCalcoloSanzioniInteressiAppoggio"></param>
        ''' <param name="bConsentiSanzNeg"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="10/09/2019">passo direttamente la data di morte che è l'unico campo fisso che serve dall'anagrafica</revision></revisionHistory>
        Public Function getSanzioniICI(StringConnectionProvv As String, IdEnte As String, ByVal objHashTableDati As Hashtable, ByRef oCalcoloSanzioni As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean, sDataMorte As String) As DataSet
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti

                getSanzioniICI = objCOMPlusAccertamenti.getSanzioniICICOMPlusAccertamenti(StringConnectionProvv, IdEnte, m_objHashTable, objHashTableDati, oCalcoloSanzioni, objDSCalcoloSanzioniInteressiAppoggio, bConsentiSanzNeg, sDataMorte)

                Return getSanzioniICI
            Catch ex As Exception
                Throw New Exception("Function::getSanzioniICI::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' Funzione che richiamala funzione per l'inserimento del provvedimento e relativi dati accessori
        ''' </summary>
        ''' <param name="myDBType"></param>
        ''' <param name="myHashTable"></param>
        ''' <param name="ListCalcoloSanzInt"></param>
        ''' <param name="dsCalcoloSanzioni"></param>
        ''' <param name="dsCalcoloInteressi"></param>
        ''' <param name="spese"></param>
        ''' <param name="ListDichiarato"></param>
        ''' <param name="ListAccertato"></param>
        ''' <param name="dsSanzioniFase2"></param>
        ''' <param name="ListInteressiFase2"></param>
        ''' <param name="dsVersamentiF2"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Public Function updateDBAccertamenti(myDBType As String, StringConnectionProvv As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As System.Collections.Hashtable, ByVal ListCalcoloSanzInt As ObjBaseIntSanz, ByVal dsCalcoloSanzioni As System.Data.DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal spese As Double, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal dsVersamentiF2 As System.Data.DataSet, Operatore As String) As Long
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti
                updateDBAccertamenti = objCOMPlusAccertamenti.updateDBCOMPlusAccertamenti(myDBType, StringConnectionProvv, IdEnte, IdContribuente, m_objHashTable, ListCalcoloSanzInt, dsCalcoloSanzioni, ListInteressi, spese, ListDichiarato, ListAccertato, dsSanzioniFase2, ListInteressiFase2, dsVersamentiF2, Operatore)
                Return updateDBAccertamenti
            Catch ex As Exception
                Throw New Exception("Function::updateDBAccertamenti::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myDBType"></param>
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
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti
                Return objCOMPlusAccertamenti.SetAtto(myDBType, m_objHashTable, objSituazioneBasePerSanzInt, objSanzioni, ObjInteressiSanzioni, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, spese, oAddizionali, Operatore)
            Catch ex As Exception
                Throw New Exception("Function::updateDBAccertamentiTARSU::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        '*** 20140701 - IMU/TARES ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myDBType"></param>
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
        ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
        Public Function updateDBAccertamentiTARSU(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As ObjArticoloAccertamento, ByVal objAccertatoTARSU() As ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As OggettoAddizionaleAccertamento, Operatore As String) As ObjArticoloAccertamento()
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti

                updateDBAccertamentiTARSU = objCOMPlusAccertamenti.updateDBCOMPlusAccertamentiTARSU(myDBType, IdEnte, IdContribuente, m_objHashTable, oSituazioneBasePerSanzInt, objSanzioni, ListInteressi, oAtto, oDettaglioAtto, objDichiaratoTARSU, objAccertatoTARSU, spese, oAddizionali, Operatore)

                Return updateDBAccertamentiTARSU
            Catch ex As Exception
                Throw New Exception("Function::updateDBAccertamentiTARSU::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function
        '*** ***
        '*** 20130801 - accertamento OSAP ***
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myDBType"></param>
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
        Public Function updateDBAccertamentiOSAP(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As OggettoAttoOSAP, ByVal oDettaglioAtto() As OggettoDettaglioAtto, ByVal objDichiarato() As OSAPAccertamentoArticolo, ByVal objAccertato() As OSAPAccertamentoArticolo, ByVal spese As Double, Operatore As String) As OSAPAccertamentoArticolo()
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti

                updateDBAccertamentiOSAP = objCOMPlusAccertamenti.updateDBCOMPlusAccertamentiOSAP(myDBType, IdEnte, IdContribuente, m_objHashTable, oSituazioneBasePerSanzInt, dsSanzioni, dsSanzioniImpDicVSImpPag, dsSanzioniScadDicVSDataPag, dsInteressi, dsInteressiImpDicVSImpPag, dsInteressiScadDicVSDataPag, oAtto, oDettaglioAtto, objDichiarato, objAccertato, spese, Operatore)

                Return updateDBAccertamentiOSAP
            Catch ex As Exception
                Throw New Exception("updateDBAccertamentiOSAP::" & " " & ex.Message)
            End Try
        End Function
        <AutoComplete()> Public Function getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti
                getVersamentiPerStampaAccertamentiOSAP = Nothing
                getVersamentiPerStampaAccertamentiOSAP = objCOMPlusAccertamenti.getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv, ID_PROCEDIMENTO)

                Return getVersamentiPerStampaAccertamentiOSAP

            Catch ex As Exception
                Throw New Exception("Function::getVersamentiPerStampaAccertamentiOSAP::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        <AutoComplete()> Public Function getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal TipoRicerca As String, ByVal ID_PROCEDIMENTO As Long) As DataSet
            Try
                Dim objCOMPlusAccertamenti As New COMPlusAccertamenti
                getImmobiliDichAccPerStampaAccertamentiOSAP = Nothing
                getImmobiliDichAccPerStampaAccertamentiOSAP = objCOMPlusAccertamenti.getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv, TipoRicerca, ID_PROCEDIMENTO)
                Return getImmobiliDichAccPerStampaAccertamentiOSAP

            Catch ex As Exception
                Throw New Exception("Function::getImmobiliDichAccPerStampaAccertamentiOSAP::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        '*** ***
#End Region

#Region "ITER GESTIONE ATTI"
        <AutoComplete()>
        Public Function getAttiRicercaSemplice(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)

                getAttiRicercaSemplice = Nothing
                getAttiRicercaSemplice = objCOMPGestioniAtti.getAttiRicercaSemplice(StringConnectionProvv, IdEnte)

                Return getAttiRicercaSemplice
            Catch ex As Exception
                Log.Debug("COMPlusBusinessObject::GetDatiAttiRicercaSemplice::errore::" & ex.Message)
                Throw New Exception("Function::getAttiRicercaSemplice::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function getProvvedimentiContribuente(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)

                getProvvedimentiContribuente = Nothing
                getProvvedimentiContribuente = objCOMPGestioniAtti.getProvvedimentiContribuente(StringConnectionProvv, IdEnte)

                Return getProvvedimentiContribuente
            Catch ex As Exception
                Throw New Exception("Function::getProvvedimentiContribuente::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function getDatiProvvedimento_PerTipo(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)

                getDatiProvvedimento_PerTipo = Nothing
                getDatiProvvedimento_PerTipo = objCOMPGestioniAtti.getDatiProvvedimento_PerTipo(StringConnectionProvv, IdEnte)

                Return getDatiProvvedimento_PerTipo
            Catch ex As Exception
                Throw New Exception("Function::getAttiRicercaSemplice::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myAtto"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function LoadProvvedimenti(StringConnectionProvv As String, ByRef myAtto As OggettoAtto) As DataSet
            Try
                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)
                Return objCOMPGestioniAtti.GetDatiProvvedimenti(StringConnectionProvv, myAtto)
            Catch ex As Exception
                Throw New Exception("Function::LOADPROVVEDIMENTI::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function


        <AutoComplete()>
        Public Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv As String, ByRef NUMERO_ATTO As String) As Boolean
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)


                setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA = objCOMPGestioniAtti.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv, NUMERO_ATTO)

                Return setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA

            Catch ex As Exception
                Throw New Exception("Function::setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        <AutoComplete()>
        Public Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv As String) As Boolean
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)


                SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = objCOMPGestioniAtti.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv)

                Return SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO

            Catch ex As Exception
                Throw New Exception("Function::SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Operatore"></param>
        ''' <returns></returns>
        ''' <revisionHistory>
        ''' <revision date="12/04/2019">
        ''' <strong>Qualificazione AgID-analisi_rel01</strong>
        ''' <em>Analisi eventi</em>
        ''' </revision>
        ''' </revisionHistory>
        <AutoComplete()>
        Public Function setProvvedimentoAttoLiquidazione(myDBType As String, ByVal myStringConnection As String, myAtto As OggettoAtto, Operatore As String) As Integer
            Try
                Return New DBOPENgovProvvedimentiUpdate().SetProvvedimento(myDBType, myStringConnection, myAtto, Operatore)
            Catch ex As Exception
                Throw New Exception("COMPlusBusinessObject.setProvvedimentoAttoLiquidazione.errore::", ex)
            End Try
        End Function


        <AutoComplete()>
        Public Function GetDatiAttiRicercaAvanzata(StringConnectionProvv As String, IdEnte As String, ParamSearch As ObjSearchAtti) As DataSet
            Try
                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)

                GetDatiAttiRicercaAvanzata = Nothing
                GetDatiAttiRicercaAvanzata = objCOMPGestioniAtti.GetDatiAttiRicercaAvanzata(StringConnectionProvv, IdEnte, ParamSearch)

                Return GetDatiAttiRicercaAvanzata

            Catch ex As Exception
                Throw New Exception("Function::GetDatiAttiRicercaAvanzata::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

        <AutoComplete()>
        Public Function setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv As String, ByVal objSELEZIONE_DATASET As DataSet) As Long
            Try

                Dim objCOMPGestioniAtti As New COMPlusGestioneAtti(m_objHashTable)

                setDATE_PROVVEDIMENTI_MASSIVA = 0
                setDATE_PROVVEDIMENTI_MASSIVA = objCOMPGestioniAtti.setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv, objSELEZIONE_DATASET)

                Return setDATE_PROVVEDIMENTI_MASSIVA

            Catch ex As Exception
                Throw New Exception("Function::setDATE_PROVVEDIMENTI_MASSIVA::COMPlusBusinessObject:: " & ex.Message)
            End Try
        End Function

#End Region

#Region "ITER GESTIONE CONFIGURAZIONE"
        <AutoComplete()>
        Public Function getTipoVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                getTipoVoci = Nothing
                getTipoVoci = objCOMPGestioniConfigurazione.getTipoVoci(StringConnectionProvv, IdEnte)

                Return getTipoVoci
            Catch ex As Exception
                Throw New Exception("Function::getTipoVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function SetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByRef strIDTIPOVOCE As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetTipoVoci = Nothing
                SetTipoVoci = objCOMPGestioniConfigurazione.SetTipoVoci(StringConnectionProvv, IdEnte, strIDTIPOVOCE)

                Return SetTipoVoci
            Catch ex As Exception
                Throw New Exception("Function::SetTipoVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function DelTipoVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelTipoVoci = Nothing
                DelTipoVoci = objCOMPGestioniConfigurazione.DelTipoVoci(StringConnectionProvv, IdEnte)

                Return DelTipoVoci
            Catch ex As Exception
                Throw New Exception("Function::DelTipoVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function GetValoriVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetValoriVoci = Nothing
                GetValoriVoci = objCOMPGestioniConfigurazione.GetValoriVoci(StringConnectionProvv, IdEnte)

                Return GetValoriVoci
            Catch ex As Exception
                Throw New Exception("Function::GetValoriVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetTipoInteresse(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetTipoInteresse = Nothing
                GetTipoInteresse = objCOMPGestioniConfigurazione.GetTipoInteresse(StringConnectionProvv, IdEnte)

                Return GetTipoInteresse
            Catch ex As Exception
                Throw New Exception("Function::GetTipoInteresse::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetScadenzaInteressi = Nothing
                GetScadenzaInteressi = objCOMPGestioniConfigurazione.GetScadenzaInteressi(StringConnectionProvv, IdEnte)

                Return GetScadenzaInteressi
            Catch ex As Exception
                Throw New Exception("Function::GetTipoInteresse::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetAnniProvvedimenti = Nothing
                GetAnniProvvedimenti = objCOMPGestioniConfigurazione.GetAnniProvvedimenti(StringConnectionProvv, IdEnte)

                Return GetAnniProvvedimenti
            Catch ex As Exception
                Throw New Exception("Function::GetAnniProvvedimenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function SetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByRef intRetVal As Integer) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetValoriVoci = Nothing
                SetValoriVoci = objCOMPGestioniConfigurazione.SetValoriVoci(StringConnectionProvv, IdEnte, intRetVal)

                Return SetValoriVoci
            Catch ex As Exception
                Throw New Exception("Function::SetValoriVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function DelValoriVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelValoriVoci = Nothing
                DelValoriVoci = objCOMPGestioniConfigurazione.DelValoriVoci(StringConnectionProvv, IdEnte)

                Return DelValoriVoci
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function DelAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelAnniProvvedimenti = Nothing
                DelAnniProvvedimenti = objCOMPGestioniConfigurazione.DelAnniProvvedimenti(StringConnectionProvv, IdEnte)

                Return DelAnniProvvedimenti
            Catch ex As Exception
                Throw New Exception("Function::DelAnniProvvedimenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function SetTassiInteresse(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetTassiInteresse = Nothing
                SetTassiInteresse = objCOMPGestioniConfigurazione.SetTassiInteresse(StringConnectionProvv, IdEnte)

                Return SetTassiInteresse
            Catch ex As Exception
                Throw New Exception("Function::SetTassiInteresse::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function SetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetScadenzaInteressi = Nothing
                SetScadenzaInteressi = objCOMPGestioniConfigurazione.SetScadenzaInteressi(StringConnectionProvv, IdEnte)

                Return SetScadenzaInteressi
            Catch ex As Exception
                Throw New Exception("Function::SetScadenzaInteressi::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function SetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetAnniProvvedimenti = Nothing
                SetAnniProvvedimenti = objCOMPGestioniConfigurazione.SetAnniProvvedimenti(StringConnectionProvv, IdEnte)

                Return SetAnniProvvedimenti
            Catch ex As Exception
                Throw New Exception("Function::SetAnniProvvedimenti::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function DelTassiInteresse(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelTassiInteresse = Nothing
                DelTassiInteresse = objCOMPGestioniConfigurazione.DelTassiInteresse(StringConnectionProvv, IdEnte)

                Return DelTassiInteresse
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function DelScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelScadenzaInteressi = Nothing
                DelScadenzaInteressi = objCOMPGestioniConfigurazione.DelScadenzaInteressi(StringConnectionProvv, IdEnte)

                Return DelScadenzaInteressi
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function GetMotivazioni(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetMotivazioni = Nothing
                GetMotivazioni = objCOMPGestioniConfigurazione.GetMotivazioni(StringConnectionProvv, IdEnte)

                Return GetMotivazioni
            Catch ex As Exception
                Throw New Exception("Function::GetMotivazioni::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function SetMotivazioni(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetMotivazioni = Nothing
                SetMotivazioni = objCOMPGestioniConfigurazione.SetMotivazioni(StringConnectionProvv, IdEnte)

                Return SetMotivazioni
            Catch ex As Exception
                Throw New Exception("Function::SetMotivazioni::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function DelMotivazioni(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelMotivazioni = Nothing
                DelMotivazioni = objCOMPGestioniConfigurazione.DelMotivazioni(StringConnectionProvv, IdEnte)

                Return DelMotivazioni
            Catch ex As Exception
                Throw New Exception("Function::DelMotivazioni::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        <AutoComplete()>
        Public Function GetTipologieVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetTipologieVoci = Nothing
                GetTipologieVoci = objCOMPGestioniConfigurazione.GetTipologieVoci(StringConnectionProvv, IdEnte)

                Return GetTipologieVoci
            Catch ex As Exception
                Throw New Exception("Function::GetTipologieVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myConnectionString"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="14/804/2020">Sono cambiate le regole di applicazione sanzione</revision></revisionHistory>
        <AutoComplete()>
        Public Function GetSanzioniRavvedimentoOperoso(myConnectionString As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetSanzioniRavvedimentoOperoso = Nothing
                GetSanzioniRavvedimentoOperoso = objCOMPGestioniConfigurazione.GetSanzioniRavvedimentoOperoso(myConnectionString)

                Return GetSanzioniRavvedimentoOperoso
            Catch ex As Exception
                Throw New Exception("Function::GetSanzioniRavvedimentoOperoso::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function SetTipologieVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetTipologieVoci = Nothing
                SetTipologieVoci = objCOMPGestioniConfigurazione.SetTipologieVoci(StringConnectionProvv, IdEnte)

                Return SetTipologieVoci
            Catch ex As Exception
                Throw New Exception("Function::SetTipologieVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function


        <AutoComplete()>
        Public Function DelTipologieVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                DelTipologieVoci = Nothing
                DelTipologieVoci = objCOMPGestioniConfigurazione.DelTipologieVoci(StringConnectionProvv, IdEnte)

                Return DelTipologieVoci
            Catch ex As Exception
                Throw New Exception("Function::DelTipologieVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetGeneraleICI(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                GetGeneraleICI = Nothing
                GetGeneraleICI = objCOMPGestioniConfigurazione.GetGeneraleICI(StringConnectionProvv, IdEnte)

                Return GetGeneraleICI
            Catch ex As Exception
                Throw New Exception("Function::GetTipologieVoci::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function SetGeneraleICI(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                Dim objCOMPGestioniConfigurazione As New COMPlusGestioneConfigurazione(m_objHashTable)

                SetGeneraleICI = Nothing
                SetGeneraleICI = objCOMPGestioniConfigurazione.SetGeneraleICI(StringConnectionProvv, IdEnte)

                Return SetGeneraleICI
            Catch ex As Exception
                Throw New Exception("Function::SetGeneraleICI::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

#End Region


#Region "Configurazione ACCERTAMENTO"
        <AutoComplete()>
        Public Function GetListClasse(StringConnectionICI As String) As DataSet
            Try

                Dim objDbIci As New DBIci

                GetListClasse = Nothing
                GetListClasse = objDbIci.GetListClasse(StringConnectionICI, m_objHashTable)

                Return GetListClasse

            Catch ex As Exception
                Throw New Exception("Function::GetListClasse::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetListTipoRendita(StringConnectionICI As String) As DataSet
            Try

                Dim objDbIci As New DBIci

                GetListTipoRendita = Nothing
                GetListTipoRendita = objDbIci.GetListTipoRendita(StringConnectionICI, m_objHashTable)

                Return GetListTipoRendita

            Catch ex As Exception
                Throw New Exception("Function::GetListTipoRendita::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetListCategorie(StringConnectionICI As String) As DataSet
            Try

                Dim objDbIci As New DBIci

                GetListCategorie = Nothing
                GetListCategorie = objDbIci.GetListCategorie(StringConnectionICI, m_objHashTable)

                Return GetListCategorie

            Catch ex As Exception
                Throw New Exception("Function::GetListCategorie::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function

        <AutoComplete()>
        Public Function GetListTipoPossesso(StringConnectionICI As String) As DataSet
            Try

                Dim objDbIci As New DBIci

                GetListTipoPossesso = Nothing
                GetListTipoPossesso = objDbIci.GetListTipoPossesso(StringConnectionICI, m_objHashTable)

                Return GetListTipoPossesso

            Catch ex As Exception
                Throw New Exception("Function::GetListTipoPossesso::COMPlusBusinessObject:: " & ex.Message)
            End Try

        End Function
#End Region


        Public Sub New()

        End Sub

        Public Overloads Sub InizializeObject(ByVal objHashTable As Hashtable)
            m_objHashTable = objHashTable
        End Sub
    End Class

End Namespace
