Namespace COSTANTValue

    ''' <summary>
    ''' Classe che incapsula tutte le costanti necessarie
    ''' </summary>
    Public Class CostantiProv
        Public Const DBType As String = "SQL"

        Public Const INIT_VALUE_NUMBER As Integer = -1
        Public Const INIT_VALUE_STRING As String = "-1"
        Public Const INIT_VALUE_BOOL As Boolean = False
        Public Const VALUE_NUMBER_ZERO As Integer = 0
        Public Const VALUE_NUMBER_UNO As Integer = 1
        Public Const VALUE_INCREMENT As Integer = 1

        'Public Const ID_PROVVEDIMENTO_ACCERTAMENTO As Integer = 4
        Public Const DATA_FINE_INTERESSI_SEMESTRI As String = "20061231" '"31/12/2006"
        Public Const DATA_INIZIO_INTERESSI_GIORNALIERI As String = "01/01/2007"
        Public Const DATA_ACCONTO_ICI As String = "30/06"
        Public Const DATA_SALDO_ICI As String = "20/12"
        Public Const DATA_US_ICI As String = "30/06"
        Public Const INIT_CHIAVE_DUPLICATA As Integer = 2627

        'dati di default dalla tabella TP_GENERALE_ICI per il calcolo degli interessi
        Public Const MODALITA_CALCOLO_INTERESSI_SEMESTRI As String = "S"
        Public Const MODALITA_CALCOLO_INTERESSI_GIORNI As String = "G"
        Public Const DATA_VERSAMENTO_ACCONTO As String = "0630"
        Public Const DATA_VERSAMENTO_SALDO As String = "1220"
        Public Const DATA_SCADENZA_TARSU As String = "0531"

        'dati caratteristica immobile per eseguire fase 3.1 confronto catasto-dichiarato
        Public Const CARATTERISTICA_TA As String = "1" 'Terreno agricolo
        Public Const CARATTERISTICA_AF As String = "2" 'Area Fabbricabile
        Public Const CARATTERISTICA_RE As String = "3" 'Fabbricato con valore determinato da rendita catastale
        Public Const CARATTERISTICA_LC As String = "4" 'Fabbricato con valore determinato sulla base delle scritture contabili

        'dati per determinare il tipo di provvedimento nel preaccertamento/accertamento
        Public Const TIPO_PROVVEDIMENTO_OK As String = "0" 'Public Const TIPO_PROVVEDIMENTO_OK As String = "OK"
        Public Const TIPO_PROVVEDIMENTO_RIMBORSO As String = "RIMBORSO"
        Public Const TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_IN_RETTIFICA As String = "AVVISOACCERTAMENTORETTIFICA"
        Public Const TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_D_UFFICIO As String = "AVVISOACCERTAMENTOUFFICIO"

        Public Const DESCR_TIPO_PROVVEDIMENTO_OK As String = "OK"
        Public Const DESCR_TIPO_PROVVEDIMENTO_RIMBORSO As String = "RIMBORSO"
        Public Const DESCR_TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_IN_RETTIFICA As String = "AVVISO DI ACCERTAMENTO IN RETTIFICA"
        Public Const DESCR_TIPO_PROVVEDIMENTO_AVVISO_ACCERTAMENTO_D_UFFICIO As String = "AVVISO DI ACCERTAMENTO D'UFFICIO"

        'dati per determinare l'esito di fase 3.1 di preaccertamento
        Public Const DATA_CONFRONTO_NOTIFICA_CATASTO As String = "01/01/2000"

        'DATI PER GESTIRE I DOCUMENTI INVIATI AI CONTRIBUENTI
        Public Const TIPO_DOC_LETTERA As String = "LETTERA" 'LETTERA GENERICA DA PROVVEDIMENTI
        Public Const TIPO_DOC_LETTERA_CONTATTO As String = "CONTATTO" ' LETTERA SPECIFICA DA PRE ACCERTAMENTO
        Public Const TIPO_DOC_RICHIESTAINFO As String = "RICHIESTAINFO" 'RICHIESTA INFO DA PRE ACCERTAMENTO
        Public Const TIPO_DOC_INFORMATIVA_ICI As String = "INFORMATIVAICI" 'INFORMATIVA ICI
        Public Const TIPO_DOC_INFORMATIVA_TARSU As String = "INFORMATIVATARSU" 'INFORMATIVA TARSU

        'Public Const COD_DOCUMENTO_PREACCERTAMENTO As String = "6"
        'Public Const COD_DOCUMENTO_PREACCERTAMENTO_BOZZA As String = "7"
        Public Const COD_DOC_LETTERA As String = "2"
        Public Const COD_DOC_LETTERA_CONTATTO As String = "3"
        Public Const COD_DOC_RICHIESTAINFO As String = "4"

        ' DATI TIPO INTERESSE
        Public Const TIPO_INTERESSI_LEGALI As Integer = 1
        Public Const TIPO_INTERESSI_MORATORI As Integer = 2
        Public Const TIPO_INTERESSI_ERARIALI As Integer = 3

        'NOTE ATTO PRE ACCERTAMENTO SE IMPORTO INFERIORE A SOGLIA
        Public Const NOTE_PRA_ACC_IMPORTO_INFERIORE_A_SOGLIA As String = "IMPORTO TOTALE ATTO INFERIORE A IMPORTO SOGLIA MINIMA CONFIGURATO"
        '*** 20130801 - accertamento OSAP ***
        Public Const AMBITO_DICHIARATO As Integer = 1
        Public Const AMBITO_ACCERTATO As Integer = 2
        '*** ***
    End Class
End Namespace