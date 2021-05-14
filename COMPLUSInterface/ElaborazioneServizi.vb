''' <summary>
''' Definizione interfacce per il calcolo IMU/TARI e gestione accertamenti
''' </summary>
Public Interface IElaborazioneLiquidazioni
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
    ''' <param name="dsRiepilogo"></param>
    ''' <param name="dsVersamenti"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="10/12/2019">in caso di calcolo per Cartelle Insoluti devo prendere il pagato per singolo avviso</revision></revisionHistory>
    Function ProcessFase2(StringConnectionProvv As String, StringConnectionICI As String, IdEnte As String, IdContribuente As Integer, ByVal myHashTable As System.Collections.Hashtable, ByVal dsAnagrafica As System.Data.DataSet, ImpDichAcconto As Double, ImpDichSaldo As Double, ImpDichTotale As Double, sCodCartella As String, ByRef ListBaseCalcolo() As ObjBaseIntSanz, ByRef dsSanzioni As System.Data.DataSet, ByRef ListInteressi() As ObjInteressiSanzioni, ByRef dsRiepilogo As ObjBaseIntSanz, ByRef dsVersamenti As System.Data.DataSet) As Boolean
    Function GetProvvedimentoPerStampaLiquidazione(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function GetVersamentiPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long, ByVal ID_FASE As Integer) As DataSet
    Function GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
    Function GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
    Function GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
End Interface
Public Interface IElaborazioneAtti
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="myAtto">ref OggettoAtto oggetto da restituire popolato</param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Function GetDatiProvvedimenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByRef myAtto As OggettoAtto) As DataSet
    Function getDatiProvvedimento_PerTipo(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function getProvvedimentiContribuente(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myStringConnection"></param>
    ''' <param name="myAtto"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Function SetProvvedimentoAttoLiquidazione(myDBType As String, ByVal myStringConnection As String, myAtto As OggettoAtto, Operatore As String) As Integer
    'Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE(ByVal objHashTable As Hashtable) As Boolean
    Function GetDatiAttiRicercaAvanzata(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ParamSearch As ObjSearchAtti) As DataSet
    Function GetDatiAttiRicercaSemplice(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByRef NUMERO_ATTO As String) As Boolean
    Function setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal objSELEZIONE_DATASET As DataSet) As Long
    Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv As String, ByVal objHashTable As Hashtable) As Boolean
End Interface
Public Interface IElaborazioneAccertamenti
    Function GetDatiAccertamenti(StringConnectionICI As String, StringConnectionGOV As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByRef ListSituazioneFinale() As objSituazioneFinale) As objUIICIAccert()
    '*** 20140701 - IMU/TARES ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <param name="objHashTableDati"></param>
    ''' <param name="oCalcoloSanzioni"></param>
    ''' <param name="objDSCalcoloSanzioniInteressiAppoggio"></param>
    ''' <param name="bConsentiSanzNeg"></param>
    ''' <param name="sDataMorte"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="10/09/2019">passo direttamente la data di morte che è l'unico campo fisso che serve dall'anagrafica</revision></revisionHistory>
    Function getSanzioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByRef oCalcoloSanzioni As ObjBaseIntSanz, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean, sDataMorte As String) As DataSet
    'Function getSanzioni(ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByVal objDSCalcoloSanzioni As DataSet, ByVal objDSCalcoloSanzioniInteressiAppoggio As DataSet, ByVal bConsentiSanzNeg As Boolean, sDataMorte As String) As DataSet
    '*** ***
    '**** 201809 - Cartelle Insoluti ***
    ''' <summary>
    ''' Funzione unica per il calcolo degli interessi
    ''' </summary>
    ''' <param name="IdEnte"></param>
    ''' <param name="IdTributo"></param>
    ''' <param name="CodVoce"></param>
    ''' <param name="TipoProvvedimento"></param>
    ''' <param name="TipoProcedimento"></param>
    ''' <param name="Fase"></param>
    ''' <param name="DataElaborazione"></param>
    ''' <param name="ScadenzaIntAcconto">non valorizzato in caso di 0434/0453</param>
    ''' <param name="ScadenzaIntSaldo"></param>
    ''' <param name="IdLegame"></param>
    ''' <param name="ListToCalc"></param>
    ''' <param name="myStringConnection"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
    Function getInteressi(IdEnte As String, ByVal IdTributo As String, ByVal CodVoce As String, ByVal TipoProvvedimento As String, TipoProcedimento As String, Fase As Integer, DataElaborazione As Date, ScadenzaIntAcconto As String, ScadenzaIntSaldo As String, IdLegame As Integer, ListToCalc() As ObjBaseIntSanz, myStringConnection As String) As ObjInteressiSanzioni()
    'Function getInteressiICI(ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByVal objDSCalcoloInteressi As DataSet, ByVal Progressivo As Integer, ByVal idLegame As Integer) As DataSet
    'Function getInteressiTARSU(ByVal objHashTable As Hashtable, ByVal objHashTableDati As Hashtable, ByVal objDSCalcoloInteressi As DataSet, ByVal Progressivo As Integer, ByVal idLegame As Integer) As DataSet
    '*** ***
    '**** 201809 - Cartelle Insoluti ***'*** 20130801 - accertamento OSAP ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myDBType"></param>
    ''' <param name="objHashTable"></param>
    ''' <param name="oCalcoloSanzioniInteressi"></param>
    ''' <param name="objDSCalcoloSanzioni"></param>
    ''' <param name="ListInteressi"></param>
    ''' <param name="spese"></param>
    ''' <param name="objICI"></param>
    ''' <param name="ListDichiarato"></param>
    ''' <param name="ListAccertato"></param>
    ''' <param name="dsSanzioniFase2"></param>
    ''' <param name="ListInteressiFase2"></param>
    ''' <param name="objDSCversamentiF2"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    ''' <revisionHistory><revision date="12/11/2019">il calcolo interessi 8852/TASI deve essere fatto in acconto/saldo o in unica soluzione in base alla configurazione di TP_GENERALE_ICI</revision></revisionHistory>
    Function updateDBAccertamenti(myDBType As String, StringConnectionProvv As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oCalcoloSanzioniInteressi As ObjBaseIntSanz, ByVal objDSCalcoloSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal spese As Double, ByVal objICI() As objSituazioneFinale, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal objDSCversamentiF2 As DataSet, Operatore As String) As Long
    'Function updateDBAccertamenti(myDBType As String, ByVal objHashTable As Hashtable, ByVal objDSCalcoloSanzioniInteressi As DataSet, ByVal objDSCalcoloSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal spese As Double, ByVal objICI() As objSituazioneFinale, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal objDSCversamentiF2 As DataSet, Operatore As String) As Long
    'Function updateDBAccertamenti(myDBType As String, ByVal objHashTable As Hashtable, ByVal objDSCalcoloSanzioniInteressi As DataSet, ByVal objDSCalcoloSanzioni As DataSet, ByVal objDSCalcoloInteressi As DataSet, ByVal spese As Double, ByVal objICI() As objSituazioneFinale, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal objDSCversamentiF2 As DataSet, Operatore As String) As Long
    'Function updateDBAccertamenti(ByVal objHashTable As Hashtable, ByVal objDSCalcoloSanzioniInteressi As DataSet, ByVal objDSCalcoloSanzioni As DataSet, ByVal objDSCalcoloInteressi As DataSet, ByVal spese As Double, ByVal objICI() As objSituazioneFinale, ByVal ListDichiarato() As objUIICIAccert, ByVal ListAccertato() As objUIICIAccert, ByVal dsSanzioniFase2 As DataSet, ByVal ListInteressiFase2() As ObjInteressiSanzioni, ByVal objDSCversamentiF2 As DataSet) As Long
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myDBType"></param>
    ''' <param name="objHashTable"></param>
    ''' <param name="objSituazioneBasePerSanzInt"></param>
    ''' <param name="objSanzioni"></param>
    ''' <param name="objInteressi"></param>
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
    Function SetAtto(myDBType As String, ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt() As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal objInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento, Operatore As String) As Integer
    'Function SetAtto(ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt() As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal objInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento) As Integer

    Function getImmobiliDichiaratiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet
    Function getImmobiliAccertatiPerStampaAccertamenti(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROCEDIMENTO As Long) As DataSet
    Function GetElencoSanzioniPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
    Function GetElencoInteressiPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet
    Function GetInteressiTotaliPerStampaAccertamenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet

    Function getImmobiliDichiaratiPerStampaAccertamentiTARSU(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function getImmobiliAccertatiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function getImmobiliDichAccPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function getAddizionaliPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function getVersamentiPerStampaAccertamentiTARSU(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet

    '*** 20140701 - IMU/TARES ***
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myDBType"></param>
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
    Function updateDBAccertamentiTARSU(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento, Operatore As String) As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento()
    'Function updateDBAccertamentiTARSU(myDBType As String, ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt As DataSet, ByVal objSanzioni As DataSet, ByVal ListInteressi() As ObjInteressiSanzioni, ByVal oAtto As OggettoAttoTARSU, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiaratoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal objAccertatoTARSU() As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento, ByVal spese As Double, ByVal oAddizionali() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoAddizionaleAccertamento, Operatore As String) As RemotingInterfaceMotoreTarsu.MotoreTarsuVARIABILE.Oggetti.ObjArticoloAccertamento()
    '*** ***

    Function GetControlliAccertamento(StringConnectionProvv As String, ByVal sAnno As String, ByVal strCodEnte As String, ByVal strCodContrib As String, ByVal strCodTributo As String, ByRef objHashTable As Hashtable) As Integer
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
    Function updateDBAccertamentiOSAP(myDBType As String, IdEnte As String, IdContribuente As Integer, ByVal objHashTable As Hashtable, ByVal oSituazioneBasePerSanzInt As ObjBaseIntSanz, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As OggettoAttoOSAP, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiarato() As OSAPAccertamentoArticolo, ByVal objAccertato() As OSAPAccertamentoArticolo, ByVal spese As Double, Operatore As String) As OSAPAccertamentoArticolo()
    'Function updateDBAccertamentiOSAP(myDBType As String, ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt As DataSet, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As OggettoAttoOSAP, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiarato() As OSAPAccertamentoArticolo, ByVal objAccertato() As OSAPAccertamentoArticolo, ByVal spese As Double, Operatore As String) As OSAPAccertamentoArticolo()
    'Function updateDBAccertamentiOSAP(ByVal objHashTable As Hashtable, ByVal objSituazioneBasePerSanzInt As DataSet, ByVal dsSanzioni As DataSet, ByVal dsSanzioniImpDicVSImpPag As DataSet, ByVal dsSanzioniScadDicVSDataPag As DataSet, ByVal dsInteressi As DataSet, ByVal dsInteressiImpDicVSImpPag As DataSet, ByVal dsInteressiScadDicVSDataPag As DataSet, ByVal oAtto As OggettoAttoOSAP, ByVal oDettaglioAtto() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoDettaglioAtto, ByVal objDichiarato() As OSAPAccertamentoArticolo, ByVal objAccertato() As OSAPAccertamentoArticolo, ByVal spese As Double) As OSAPAccertamentoArticolo()
    Function getVersamentiPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    Function getImmobiliDichAccPerStampaAccertamentiOSAP(StringConnectionProvv As String, ByVal TipoRicerca As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As Long) As DataSet
    '*** ***
End Interface
Public Interface IGestioneConfigurazione
    Function GetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByRef strIDTIPOVOCE As String) As Boolean
    Function DelTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByRef intRetVal As Integer) As Boolean
    Function DelValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetTipoInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetTassiInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function DelTassiInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function DelAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function DelMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function DelTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetGeneraleICI(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetGeneraleICI(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean
    Function GetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As DataSet
    Function SetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable) As Boolean

    Function GetSanzioniRavvedimentoOperoso(ByVal myConnectionString As String) As DataSet

    Function GetListTipoRendita(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
    Function GetListClasse(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
    Function GetListCategorie(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
    Function GetListTipoPossesso(StringConnectionICI As String, ByVal objHashTable As Hashtable) As DataSet
End Interface
Public Interface IFreezer
    ''' <summary>
    ''' Interfaccia per il calcolo degli importi dovuti su una lista di immobili in ingresso
    ''' </summary>
    ''' <param name="StringConnectionGOV"></param>
    ''' <param name="StringConnectionICI"></param>
    ''' <param name="IdEnte"></param>
    ''' <param name="ListUI"></param>
    ''' <param name="TipoCalcolo"></param>
    ''' <param name="ListSituazioneFinale"></param>
    ''' <returns></returns>
    Function CalcoloFromUI(StringConnectionGOV As String, StringConnectionICI As String, IdEnte As String, ListUI As ArrayList, ByVal TipoCalcolo As Integer, ByRef ListSituazioneFinale() As objSituazioneFinale) As Boolean
    ''' <summary>
    ''' Interfaccia per il calcolo degli importi dovuti dei soggetti in ingresso
    ''' </summary>
    ''' <param name="StringConnectionGOV"></param>
    ''' <param name="StringConnectionICI"></param>
    ''' <param name="IdEnte"></param>
    ''' <param name="nIdContribuente"></param>
    ''' <param name="TributoCalcolo"></param>
    ''' <param name="Tributo"></param>
    ''' <param name="AnnoDa"></param>
    ''' <param name="AnnoA"></param>
    ''' <param name="IsMassivo"></param>
    ''' <param name="blnConfigurazioneDich"></param>
    ''' <param name="blnRibaltaVersatoSuDovuto"></param>
    ''' <param name="blnCalcolaArrotondamento"></param>
    ''' <param name="TipoCalcolo"></param>
    ''' <param name="TipoTASI"></param>
    ''' <param name="TASIAProprietario"></param>
    ''' <param name="TipoOperazione"></param>
    ''' <param name="Operatore"></param>
    ''' <param name="ListSituazioneFinale"></param>
    ''' <returns></returns>
    Function CalcoloFromSoggetto(StringConnectionGOV As String, StringConnectionICI As String, IdEnte As String, nIdContribuente As Integer, TributoCalcolo As String, Tributo As String, AnnoDa As String, AnnoA As String, IsMassivo As Boolean, ByVal blnConfigurazioneDich As Boolean, ByVal blnRibaltaVersatoSuDovuto As Boolean, ByVal blnCalcolaArrotondamento As Boolean, ByVal TipoCalcolo As Integer, TipoTASI As String, TASIAProprietario As String, TipoOperazione As String, Operatore As String, ByRef ListSituazioneFinale() As objSituazioneFinale) As Boolean
    ''' <summary>
    ''' Interfaccia per la determinazione dei mesi IMU applicabili
    ''' </summary>
    ''' <param name="dInizio"></param>
    ''' <param name="dFine"></param>
    ''' <param name="nAnno"></param>
    ''' <returns></returns>
    Function CalcolaMesi(dInizio As Date, dFine As Date, nAnno As Integer) As Integer
    ''' <summary>
    ''' Interfaccia per la visualizzazione della progressione del calcolo massivo
    ''' </summary>
    ''' <param name="StringConnectionICI"></param>
    ''' <param name="IdEnte"></param>
    ''' <returns></returns>
    Function ViewCodaCalcoloICIMassivo(StringConnectionICI As String, IdEnte As String) As String
    ''' <summary>
    ''' Interfaccia per la visualizzazione dei calcoli massivi pregressi
    ''' </summary>
    ''' <param name="StringConnectionICI"></param>
    ''' <param name="IdEnte"></param>
    ''' <returns></returns>
    Function getDATI_TASK_REPOSITORY_CALCOLO_ICI(StringConnectionICI As String, IdEnte As String) As DataSet
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myConnectionString"></param>
    ''' <param name="IdEnte"></param>
    ''' <param name="objICI"></param>
    ''' <param name="nIDElaborazione"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    Function SetSituazioneFinale(myConnectionString As String, IdEnte As String, ByVal objICI As objSituazioneFinale(), ByVal nIDElaborazione As Long, Operatore As String) As Long
End Interface
