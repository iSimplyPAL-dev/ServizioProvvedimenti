Imports log4net
Imports utility
''' <summary>
''' Classe per il calcolo del valore e della rendita di un oggetto
''' </summary>
Public Class FncICI
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(FncICI))

    Public Function CalcoloValore(DBType As String, ByVal sConGOV As String, ByVal sConICI As String, ByVal IdEnte As String, ByVal Anno As Integer, ByVal TipoRendita As String, ByVal Categoria As String, ByVal Classe As String, ByVal Zona As String, ByVal Rendita As Double, ByVal ValoreDich As Double, ByVal Consistenza As Double, ByVal Dal As DateTime, ByVal bIsColtivatoreDiretto As Boolean) As Double
        Dim sSQL As String = ""
        Dim myDataView As New DataView
        Dim RenditaRivalutata As Double
        Dim myReturn As Double = 0
        Dim nMoltiplicatore As Double = 0
        Dim Tariffa As Double = 0

        Try
            '*** se parto dalla rendita devo sempre rivalutare indipendentemente dall'anno ***
            ' La rivalutazione non deve avvenire per le aree fabbricabili e le categorie D da libri contabili
            Log.Debug("CalcoloValore::TipoRendita::" & TipoRendita)
            Select Case (TipoRendita)
                Case "AF", "LC"
                    '*** 20120709 - IMU per AF e LC devo usare il campo valore ***
                    RenditaRivalutata = ValoreDich
                Case "TA"
                    RenditaRivalutata = (Rendita * 1.25)
                Case Else
                    RenditaRivalutata = (Rendita * 1.05)
            End Select
            '*** ***
            If (TipoRendita = "AF") Or (TipoRendita = "LC") Then
                myReturn = RenditaRivalutata
                Tariffa = getEstimo(DBType, sConGOV, "TARIFFE_ESTIMO_CATASTALE_FAB", IdEnte, Categoria, Classe, Zona, Dal)
                If Tariffa > 0 Then
                    myReturn = Consistenza * Tariffa
                End If
            ElseIf RenditaRivalutata = 0 Then
                myReturn = RenditaRivalutata
                Tariffa = getEstimo(DBType, sConGOV, "TARIFFE_ESTIMO_CATASTALE", IdEnte, Categoria, Classe, Zona, Dal)
                If Tariffa > 0 Then
                    myReturn = Consistenza * Tariffa
                End If
            Else
                nMoltiplicatore = GetMoltiplicatoreRendita(DBType, sConICI, TipoRendita, Categoria, Anno, bIsColtivatoreDiretto)
                If (Anno = 2006) And Categoria <> "" Then
                    If Categoria.Substring(0, 1) = "B" Then
                        If (Dal.Month >= 10) Then
                            nMoltiplicatore = 140
                        Else
                            nMoltiplicatore = 100
                        End If
                    End If
                End If
                myReturn = (RenditaRivalutata * nMoltiplicatore)
            End If
            '*** 20130916 - se ho valore in dich e non sono riuscito a ricalcolarlo tengo buono quello di dich. ***
            If myReturn = 0 Then
                myReturn = ValoreDich
            End If
            '*** ***
        Catch ex As Exception
            Log.Debug(IdEnte & " - CalcoloValore.si è verificato il seguente errore", ex)
            Throw New Exception(IdEnte & " - CalcoloValore.si è verificato il seguente errore::" + ex.Message)
        End Try
        Return myReturn
    End Function
    ''' <summary>
    ''' cerco moltiplicatore per tipo rendita e categoria se non lo trovo prendo moltiplicatore di default per anno(ovvero in tabella senza tipo rendita e categoria)
    ''' </summary>
    ''' <param name="DBType"></param>
    ''' <param name="myStringConnection"></param>
    ''' <param name="TipoRendita"></param>
    ''' <param name="Categoria"></param>
    ''' <param name="Anno"></param>
    ''' <param name="bIsColtivatoreDiretto"></param>
    ''' <returns></returns>
    Private Function GetMoltiplicatoreRendita(DBType As String, ByVal myStringConnection As String, ByVal TipoRendita As String, ByVal Categoria As String, ByVal Anno As Integer, ByVal bIsColtivatoreDiretto As Boolean) As Double
        Dim myReturn As Double = 0

        Try
            myReturn = getMoltiplicatore(DBType, myStringConnection, "S", TipoRendita, Categoria, Anno, bIsColtivatoreDiretto)
            If myReturn = 0 Then
                myReturn = getMoltiplicatore(DBType, myStringConnection, "D", "", "", Anno, False)
            End If
        Catch ex As Exception
            Log.Debug("FncICI.GetMoltiplicatoreRendita.errore: ", ex)
            Throw New Exception("FncICI.GetMoltiplicatoreRendita.si è verificato il seguente errore." & ex.Message)
        End Try
        Return myReturn
    End Function
    ''' <summary>
    ''' Restituisce l'ultima tariffa estimo presente per i parametri in ingresso
    ''' </summary>
    ''' <param name="DBType"></param>
    ''' <param name="myStringConnection"></param>
    ''' <param name="myTable"></param>
    ''' <param name="IdEnte"></param>
    ''' <param name="Categoria"></param>
    ''' <param name="Classe"></param>
    ''' <param name="Zona"></param>
    ''' <param name="Dal"></param>
    ''' <returns></returns>
    Private Function getEstimo(DBType As String, ByVal myStringConnection As String, myTable As String, IdEnte As String, Categoria As String, Classe As String, Zona As String, Dal As DateTime) As Double
        Dim sSQL As String = ""
        Dim myDataView As New DataView
        Dim myReturn As Double = 0

        Try
            'Valorizzo la connessione
            Using ctx As New DBModel(DBType, myStringConnection)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_" + myTable + "_S", "ENTE", "CATEGORIA", "CLASSE", "ZONA", "DATADAL")
                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("ENTE", IdEnte) _
                            , ctx.GetParam("CATEGORIA", Categoria) _
                            , ctx.GetParam("CLASSE", Classe) _
                            , ctx.GetParam("ZONA", Zona) _
                            , ctx.GetParam("DATADAL", Dal)
                        )
                Catch ex As Exception
                    Log.Debug("ClsDBManager.getNewID.errore: ", ex)
                Finally
                    ctx.Dispose()
                End Try
                For Each myRow As DataRowView In myDataView
                    myReturn = StringOperation.FormatDouble(myRow("tariffa_euro"))
                Next
            End Using
        Catch ex As Exception
            Log.Debug(IdEnte + " - FncICI.getEstimo.errore: ", ex)
            Throw New Exception(IdEnte + " - FncICI.getEstimo.si è verificato il seguente errore." & ex.Message)
        End Try
        Return myReturn
    End Function
    ''' <summary>
    ''' Restituisce il moltiplicatore
    ''' </summary>
    ''' <param name="DBType"></param>
    ''' <param name="myStringConnection"></param>
    ''' <param name="Tipo">S=specifico, D=Default</param>
    ''' <param name="TipoRendita"></param>
    ''' <param name="Categoria"></param>
    ''' <param name="Anno"></param>
    ''' <param name="bIsColtivatoreDiretto"></param>
    ''' <returns></returns>
    Private Function getMoltiplicatore(DBType As String, ByVal myStringConnection As String, Tipo As String, ByVal TipoRendita As String, ByVal Categoria As String, ByVal Anno As Integer, ByVal bIsColtivatoreDiretto As Boolean) As Double
        Dim sSQL As String = ""
        Dim myDataView As New DataView
        Dim myReturn As Double = 0

        Try
            'Valorizzo la connessione
            Using ctx As New DBModel(DBType, myStringConnection)
                Try
                    sSQL = ctx.GetSQL(DBModel.TypeQuery.StoredProcedure, "prc_MOLTIPLICATORI_PER_CALCOLO_VALORE_S", "TIPO", "TIPORENDITA", "ISCOLTIVATOREDIRETTO", "CATEGORIA", "ANNO")
                    myDataView = ctx.GetDataView(sSQL, "TBL", ctx.GetParam("TIPO", Tipo) _
                            , ctx.GetParam("TIPORENDITA", TipoRendita) _
                            , ctx.GetParam("ISCOLTIVATOREDIRETTO", bIsColtivatoreDiretto) _
                            , ctx.GetParam("CATEGORIA", Categoria) _
                            , ctx.GetParam("ANNO", Anno)
                        )
                Catch ex As Exception
                    Log.Debug("FncICI.getMoltiplicatore.errore: ", ex)
                Finally
                    ctx.Dispose()
                End Try
                For Each myRow As DataRowView In myDataView
                    myReturn = StringOperation.FormatDouble(myRow("moltiplicatore"))
                Next
            End Using
        Catch ex As Exception
            Log.Debug("FncICI.getMoltiplicatore.errore: ", ex)
            Throw New Exception("FncICI.getMoltiplicatore.si è verificato il seguente errore." & ex.Message)
        End Try
        Return myReturn
    End Function
End Class
''' <summary>
''' Definizione oggetto calcolo situazione finale
''' </summary>
<Serializable()> Public Class objSituazioneFinale
    Private _Tributo As String = ""
    Private _IdEnte As String = ""
    Private _Anno As String = ""
    Private _TipoRendita As String = ""
    Private _Categoria As String = ""
    Private _Classe As String = ""
    Private _Zona As String = ""
    Private _TipoTasi As String = ""
    Private _DescrTipoTasi As String = ""
    Private _Foglio As String = ""
    Private _Numero As String = ""
    Private _Subalterno As String = ""
    Private _Provenienza As String = ""
    Private _Caratteristica As String = ""
    Private _Via As String = ""
    Private _NCivico As String = ""
    Private _Esponente As String = ""
    Private _Scala As String = ""
    Private _Interno As String = ""
    Private _Piano As String = ""
    Private _Barrato As String = ""
    Private _Sezione As String = ""
    Private _Protocollo As String = ""
    Private _DataScadenza As String = ""
    Private _DataInizio As String = ""
    Private _TipoOperazione As String = ""
    Private _TitPossesso As String = ""
    Private _Id As Integer = 0
    Private _Idcontribuente As Integer = 0
    Private _IdContribuenteCalcolo As Integer = 0
    Private _IdContribuenteDich As Integer = 0
    Private _IdProcedimento As Integer = 0
    Private _IdRiferimento As Integer = 0
    Private _IdLegame As Integer = 0
    Private _Progressivo As Integer = 0
    Private _IdVia As Integer = 0
    Private _NumeroFigli As Integer = 0
    Private _MesiPossesso As Integer = 0
    Private _Mesi As Integer = 0
    Private _IdTipoUtilizzo As Integer = 0
    Private _IdTipoPossesso As Integer = 0
    Private _NUtilizzatori As Integer = 0
    Private _FlagPrincipale As Integer = 0
    Private _FlagRiduzione As Integer = 0
    Private _FlagEsente As Integer = 0
    Private _FlagStorico As Integer = 0
    Private _FlagPosseduto As Integer = 0
    Private _FlagProvvisorio As Integer = 0
    Private _MesiRiduzione As Integer = 0
    Private _MesiEsenzione As Integer = 0
    Private _AccMesi As Integer = 0
    Private _IdImmobile As Integer = 0
    Private _IdImmobilePertinenza As Integer = 0
    Private _IdImmobileDichiarato As Integer = 0
    Private _MeseInizio As Integer = 0
    Private _AbitazionePrincipaleAttuale As Integer = 0
    Private _AccSenzaDetrazione As Double = 0
    Private _AccDetrazioneApplicata As Double = 0
    Private _AccDovuto As Double = 0
    Private _AccDetrazioneResidua As Double = 0
    Private _SalSenzaDetrazione As Double = 0
    Private _SalDetrazioneApplicata As Double = 0
    Private _SalDovuto As Double = 0
    Private _SalDetrazioneResidua As Double = 0
    Private _TotSenzaDetrazione As Double = 0
    Private _totDetrazioneApplicata As Double = 0
    Private _totDovuto As Double = 0
    Private _totDetrazioneResidua As Double = 0
    Private _IdAliquota As Double = 0
    Private _Aliquota As Double = 0
    Private _AliquotaStatale As Double = 0
    Private _PercentCaricoFigli As Double = 0
    Private _AccDovutoStatale As Double = 0
    Private _AccDetrazioneApplicataStatale As Double = 0
    Private _AccDetrazioneResiduaStatale As Double = 0
    Private _salDovutoStatale As Double = 0
    Private _salDetrazioneApplicataStatale As Double = 0
    Private _salDetrazioneResiduaStatale As Double = 0
    Private _TotDovutoStatale As Double = 0
    Private _totDetrazioneApplicataStatale As Double = 0
    Private _totDetrazioneResiduaStatale As Double = 0
    Private _PercPossesso As Double = 0
    Private _Rendita As Double = 0
    Private _Valore As Double = 0
    Private _ValoreReale As Double = 0
    Private _Consistenza As Double = 0
    Private _ImpDetrazione As Double = 0
    Private _DiffImposta As Double = 0
    Private _Totale As Double = 0
    Private _IsColtivatoreDiretto As Boolean = False
    Private _Dal As DateTime = DateTime.MaxValue
    Private _Al As DateTime = DateTime.MaxValue

    Public Property Tributo() As String
        Get
            Return _Tributo
        End Get
        Set(ByVal value As String)
            _Tributo = value
        End Set
    End Property
    Public Property IdEnte() As String
        Get
            Return _IdEnte
        End Get
        Set(ByVal value As String)
            _IdEnte = value
        End Set
    End Property
    Public Property Anno() As String
        Get
            Return _Anno
        End Get
        Set(ByVal value As String)
            _Anno = value
        End Set
    End Property
    Public Property TipoRendita() As String
        Get
            Return _TipoRendita
        End Get
        Set(ByVal value As String)
            _TipoRendita = value
        End Set
    End Property
    Public Property Categoria() As String
        Get
            Return _Categoria
        End Get
        Set(ByVal value As String)
            _Categoria = value
        End Set
    End Property
    Public Property Classe() As String
        Get
            Return _Classe
        End Get
        Set(ByVal value As String)
            _Classe = value
        End Set
    End Property
    Public Property Zona() As String
        Get
            Return _Zona
        End Get
        Set(ByVal value As String)
            _Zona = value
        End Set
    End Property
    Public Property TipoTasi() As String
        Get
            Return _TipoTasi
        End Get
        Set(ByVal value As String)
            _TipoTasi = value
        End Set
    End Property
    Public Property DescrTipoTasi() As String
        Get
            Return _DescrTipoTasi
        End Get
        Set(ByVal value As String)
            _DescrTipoTasi = value
        End Set
    End Property
    Public Property Foglio() As String
        Get
            Return _Foglio
        End Get
        Set(ByVal value As String)
            _Foglio = value
        End Set
    End Property
    Public Property Numero() As String
        Get
            Return _Numero
        End Get
        Set(ByVal value As String)
            _Numero = value
        End Set
    End Property
    Public Property Subalterno() As String
        Get
            Return _Subalterno
        End Get
        Set(ByVal value As String)
            _Subalterno = value
        End Set
    End Property
    Public Property Provenienza() As String
        Get
            Return _Provenienza
        End Get
        Set(ByVal value As String)
            _Provenienza = value
        End Set
    End Property
    Public Property Caratteristica() As String
        Get
            Return _Caratteristica
        End Get
        Set(ByVal value As String)
            _Caratteristica = value
        End Set
    End Property
    Public Property Via() As String
        Get
            Return _Via
        End Get
        Set(ByVal value As String)
            _Via = value
        End Set
    End Property
    Public Property NCivico() As String
        Get
            Return _NCivico
        End Get
        Set(ByVal value As String)
            _NCivico = value
        End Set
    End Property
    Public Property Esponente() As String
        Get
            Return _Esponente
        End Get
        Set(ByVal value As String)
            _Esponente = value
        End Set
    End Property
    Public Property Scala() As String
        Get
            Return _Scala
        End Get
        Set(ByVal value As String)
            _Scala = value
        End Set
    End Property
    Public Property Interno() As String
        Get
            Return _Interno
        End Get
        Set(ByVal value As String)
            _Interno = value
        End Set
    End Property
    Public Property Piano() As String
        Get
            Return _Piano
        End Get
        Set(ByVal value As String)
            _Piano = value
        End Set
    End Property
    Public Property Barrato() As String
        Get
            Return _Barrato
        End Get
        Set(ByVal value As String)
            _Barrato = value
        End Set
    End Property
    Public Property Sezione() As String
        Get
            Return _Sezione
        End Get
        Set(ByVal value As String)
            _Sezione = value
        End Set
    End Property
    Public Property Protocollo() As String
        Get
            Return _Protocollo
        End Get
        Set(ByVal value As String)
            _Protocollo = value
        End Set
    End Property
    Public Property DataScadenza() As String
        Get
            Return _DataScadenza
        End Get
        Set(ByVal value As String)
            _DataScadenza = value
        End Set
    End Property
    Public Property DataInizio() As String
        Get
            Return _DataInizio
        End Get
        Set(ByVal value As String)
            _DataInizio = value
        End Set
    End Property
    Public Property TipoOperazione() As String
        Get
            Return _TipoOperazione
        End Get
        Set(ByVal value As String)
            _TipoOperazione = value
        End Set
    End Property
    Public Property TitPossesso() As String
        Get
            Return _TitPossesso
        End Get
        Set(ByVal value As String)
            _TitPossesso = value
        End Set
    End Property
    Public Property Id() As Integer
        Get
            Return _Id
        End Get
        Set(ByVal value As Integer)
            _Id = value
        End Set
    End Property
    Public Property IdContribuente() As Integer
        Get
            Return _Idcontribuente
        End Get
        Set(ByVal value As Integer)
            _Idcontribuente = value
        End Set
    End Property
    Public Property IdContribuenteCalcolo() As Integer
        Get
            Return _IdContribuenteCalcolo
        End Get
        Set(ByVal value As Integer)
            _IdContribuenteCalcolo = value
        End Set
    End Property
    Public Property IdContribuenteDich() As Integer
        Get
            Return _IdContribuenteDich
        End Get
        Set(ByVal value As Integer)
            _IdContribuenteDich = value
        End Set
    End Property
    Public Property IdProcedimento() As Integer
        Get
            Return _IdProcedimento
        End Get
        Set(ByVal value As Integer)
            _IdProcedimento = value
        End Set
    End Property
    Public Property IdRiferimento() As Integer
        Get
            Return _IdRiferimento
        End Get
        Set(ByVal value As Integer)
            _IdRiferimento = value
        End Set
    End Property
    Public Property IdLegame() As Integer
        Get
            Return _IdLegame
        End Get
        Set(ByVal value As Integer)
            _IdLegame = value
        End Set
    End Property
    Public Property Progressivo() As Integer
        Get
            Return _Progressivo
        End Get
        Set(ByVal value As Integer)
            _Progressivo = value
        End Set
    End Property
    Public Property IdVia() As Integer
        Get
            Return _IdVia
        End Get
        Set(ByVal value As Integer)
            _IdVia = value
        End Set
    End Property
    Public Property NumeroFigli() As Integer
        Get
            Return _NumeroFigli
        End Get
        Set(ByVal value As Integer)
            _NumeroFigli = value
        End Set
    End Property
    Public Property MesiPossesso() As Integer
        Get
            Return _MesiPossesso
        End Get
        Set(ByVal value As Integer)
            _MesiPossesso = value
        End Set
    End Property
    Public Property Mesi() As Integer
        Get
            Return _Mesi
        End Get
        Set(ByVal value As Integer)
            _Mesi = value
        End Set
    End Property
    Public Property IdTipoUtilizzo() As Integer
        Get
            Return _IdTipoUtilizzo
        End Get
        Set(ByVal value As Integer)
            _IdTipoUtilizzo = value
        End Set
    End Property
    Public Property IdTipoPossesso() As Integer
        Get
            Return _IdTipoPossesso
        End Get
        Set(ByVal value As Integer)
            _IdTipoPossesso = value
        End Set
    End Property
    Public Property NUtilizzatori() As Integer
        Get
            Return _NUtilizzatori
        End Get
        Set(ByVal value As Integer)
            _NUtilizzatori = value
        End Set
    End Property
    Public Property FlagPrincipale() As Integer
        Get
            Return _FlagPrincipale
        End Get
        Set(ByVal value As Integer)
            _FlagPrincipale = value
        End Set
    End Property
    Public Property FlagRiduzione() As Integer
        Get
            Return _FlagRiduzione
        End Get
        Set(ByVal value As Integer)
            _FlagRiduzione = value
        End Set
    End Property
    Public Property FlagEsente() As Integer
        Get
            Return _FlagEsente
        End Get
        Set(ByVal value As Integer)
            _FlagEsente = value
        End Set
    End Property
    Public Property FlagStorico() As Integer
        Get
            Return _FlagStorico
        End Get
        Set(ByVal value As Integer)
            _FlagStorico = value
        End Set
    End Property
    Public Property FlagProvvisorio() As Integer
        Get
            Return _FlagProvvisorio
        End Get
        Set(ByVal value As Integer)
            _FlagProvvisorio = value
        End Set
    End Property
    Public Property FlagPosseduto() As Integer
        Get
            Return _FlagPosseduto
        End Get
        Set(ByVal value As Integer)
            _FlagPosseduto = value
        End Set
    End Property
    Public Property MesiRiduzione() As Integer
        Get
            Return _MesiRiduzione
        End Get
        Set(ByVal value As Integer)
            _MesiRiduzione = value
        End Set
    End Property
    Public Property MesiEsenzione() As Integer
        Get
            Return _MesiEsenzione
        End Get
        Set(ByVal value As Integer)
            _MesiEsenzione = value
        End Set
    End Property
    Public Property MeseInizio() As Integer
        Get
            Return _MeseInizio
        End Get
        Set(ByVal value As Integer)
            _MeseInizio = value
        End Set
    End Property
    Public Property AccMesi() As Integer
        Get
            Return _AccMesi
        End Get
        Set(ByVal value As Integer)
            _AccMesi = value
        End Set
    End Property
    Public Property IdImmobile() As Integer
        Get
            Return _IdImmobile
        End Get
        Set(ByVal value As Integer)
            _IdImmobile = value
        End Set
    End Property
    Public Property IdImmobilePertinenza() As Integer
        Get
            Return _IdImmobilePertinenza
        End Get
        Set(ByVal value As Integer)
            _IdImmobilePertinenza = value
        End Set
    End Property
    Public Property IdImmobileDichiarato() As Integer
        Get
            Return _IdImmobileDichiarato
        End Get
        Set(ByVal value As Integer)
            _IdImmobileDichiarato = value
        End Set
    End Property
    Public Property AbitazionePrincipaleAttuale() As Integer
        Get
            Return _AbitazionePrincipaleAttuale
        End Get
        Set(ByVal value As Integer)
            _AbitazionePrincipaleAttuale = value
        End Set
    End Property
    Public Property PercPossesso() As Double
        Get
            Return _PercPossesso
        End Get
        Set(ByVal value As Double)
            _PercPossesso = value
        End Set
    End Property
    Public Property Valore() As Double
        Get
            Return _Valore
        End Get
        Set(ByVal value As Double)
            _Valore = value
        End Set
    End Property
    Public Property ValoreReale() As Double
        Get
            Return _ValoreReale
        End Get
        Set(ByVal value As Double)
            _ValoreReale = value
        End Set
    End Property
    Public Property Rendita() As Double
        Get
            Return _Rendita
        End Get
        Set(ByVal value As Double)
            _Rendita = value
        End Set
    End Property
    Public Property Consistenza() As Double
        Get
            Return _Consistenza
        End Get
        Set(ByVal value As Double)
            _Consistenza = value
        End Set
    End Property
    Public Property ImpDetrazione() As Double
        Get
            Return _ImpDetrazione
        End Get
        Set(ByVal value As Double)
            _ImpDetrazione = value
        End Set
    End Property
    Public Property AccSenzaDetrazione() As Double
        Get
            Return _AccSenzaDetrazione
        End Get
        Set(ByVal value As Double)
            _AccSenzaDetrazione = value
        End Set
    End Property
    Public Property AccDetrazioneApplicata() As Double
        Get
            Return _AccDetrazioneApplicata
        End Get
        Set(ByVal value As Double)
            _AccDetrazioneApplicata = value
        End Set
    End Property
    Public Property AccDovuto() As Double
        Get
            Return _AccDovuto
        End Get
        Set(ByVal value As Double)
            _AccDovuto = value
        End Set
    End Property
    Public Property AccDetrazioneResidua() As Double
        Get
            Return _AccDetrazioneResidua
        End Get
        Set(ByVal value As Double)
            _AccDetrazioneResidua = value
        End Set
    End Property
    Public Property SalSenzaDetrazione() As Double
        Get
            Return _SalSenzaDetrazione
        End Get
        Set(ByVal value As Double)
            _SalSenzaDetrazione = value
        End Set
    End Property
    Public Property SalDetrazioneApplicata() As Double
        Get
            Return _SalDetrazioneApplicata
        End Get
        Set(ByVal value As Double)
            _SalDetrazioneApplicata = value
        End Set
    End Property
    Public Property SalDovuto() As Double
        Get
            Return _SalDovuto
        End Get
        Set(ByVal value As Double)
            _SalDovuto = value
        End Set
    End Property
    Public Property SalDetrazioneResidua() As Double
        Get
            Return _SalDetrazioneResidua
        End Get
        Set(ByVal value As Double)
            _SalDetrazioneResidua = value
        End Set
    End Property
    Public Property TotSenzaDetrazione() As Double
        Get
            Return _TotSenzaDetrazione
        End Get
        Set(ByVal value As Double)
            _TotSenzaDetrazione = value
        End Set
    End Property
    Public Property TotDetrazioneApplicata() As Double
        Get
            Return _totDetrazioneApplicata
        End Get
        Set(ByVal value As Double)
            _totDetrazioneApplicata = value
        End Set
    End Property
    Public Property TotDovuto() As Double
        Get
            Return _totDovuto
        End Get
        Set(ByVal value As Double)
            _totDovuto = value
        End Set
    End Property
    Public Property TotDetrazioneResidua() As Double
        Get
            Return _totDetrazioneResidua
        End Get
        Set(ByVal value As Double)
            _totDetrazioneResidua = value
        End Set
    End Property
    Public Property IdAliquota() As Double
        Get
            Return _IdAliquota
        End Get
        Set(ByVal value As Double)
            _IdAliquota = value
        End Set
    End Property
    Public Property Aliquota() As Double
        Get
            Return _Aliquota
        End Get
        Set(ByVal value As Double)
            _Aliquota = value
        End Set
    End Property
    Public Property AliquotaStatale() As Double
        Get
            Return _AliquotaStatale
        End Get
        Set(ByVal value As Double)
            _AliquotaStatale = value
        End Set
    End Property
    Public Property PercentCaricoFigli() As Double
        Get
            Return _PercentCaricoFigli
        End Get
        Set(ByVal value As Double)
            _PercentCaricoFigli = value
        End Set
    End Property
    Public Property AccDovutoStatale() As Double
        Get
            Return _AccDovutoStatale
        End Get
        Set(ByVal value As Double)
            _AccDovutoStatale = value
        End Set
    End Property
    Public Property AccDetrazioneApplicataStatale() As Double
        Get
            Return _AccDetrazioneApplicataStatale
        End Get
        Set(ByVal value As Double)
            _AccDetrazioneApplicataStatale = value
        End Set
    End Property
    Public Property AccDetrazioneResiduaStatale() As Double
        Get
            Return _AccDetrazioneResiduaStatale
        End Get
        Set(ByVal value As Double)
            _AccDetrazioneResiduaStatale = value
        End Set
    End Property
    Public Property SalDovutoStatale() As Double
        Get
            Return _salDovutoStatale
        End Get
        Set(ByVal value As Double)
            _salDovutoStatale = value
        End Set
    End Property
    Public Property SalDetrazioneApplicataStatale() As Double
        Get
            Return _salDetrazioneApplicataStatale
        End Get
        Set(ByVal value As Double)
            _salDetrazioneApplicataStatale = value
        End Set
    End Property
    Public Property SalDetrazioneResiduaStatale() As Double
        Get
            Return _salDetrazioneResiduaStatale
        End Get
        Set(ByVal value As Double)
            _salDetrazioneResiduaStatale = value
        End Set
    End Property
    Public Property TotDovutoStatale() As Double
        Get
            Return _TotDovutoStatale
        End Get
        Set(ByVal value As Double)
            _TotDovutoStatale = value
        End Set
    End Property
    Public Property TotDetrazioneApplicataStatale() As Double
        Get
            Return _totDetrazioneApplicataStatale
        End Get
        Set(ByVal value As Double)
            _totDetrazioneApplicataStatale = value
        End Set
    End Property
    Public Property TotDetrazioneResiduaStatale() As Double
        Get
            Return _totDetrazioneResiduaStatale
        End Get
        Set(ByVal value As Double)
            _totDetrazioneResiduaStatale = value
        End Set
    End Property
    Public Property DiffImposta() As Double
        Get
            Return _DiffImposta
        End Get
        Set(ByVal value As Double)
            _DiffImposta = value
        End Set
    End Property
    Public Property Totale() As Double
        Get
            Return _Totale
        End Get
        Set(ByVal value As Double)
            _Totale = value
        End Set
    End Property
    Public Property IsColtivatoreDiretto() As Boolean
        Get
            Return _IsColtivatoreDiretto
        End Get
        Set(ByVal value As Boolean)
            _IsColtivatoreDiretto = value
        End Set
    End Property
    Public Property Dal() As DateTime
        Get
            Return _Dal
        End Get
        Set(ByVal value As DateTime)
            _Dal = value
        End Set
    End Property
    Public Property Al() As DateTime
        Get
            Return _Al
        End Get
        Set(ByVal value As DateTime)
            _Al = value
        End Set
    End Property
End Class
'*** 201810 - Generazione Massiva Atti ***
''' <summary>
''' Definizione oggetto atto di accertamento
''' </summary>
<Serializable()>
Public Class OggettoAtto
    Public Class Fase
        Public Const VersamentiTardivi As Integer = 1
        Public Const VersatoDichiarato As Integer = 2
        Public Const DichiaratoAccertato As Integer = 3
        Public Const Ravvedimento As Integer = 4

        Public Sub New()
        End Sub
    End Class
    Public Class Procedimento
        Public Const Questionario As String = "Q"
        Public Const Liquidazione As String = "L"
        Public Const Accertamento As String = "A"
        Public Sub New()
        End Sub
    End Class
    Public Class Provvedimento
        Public Const NoAvviso As Integer = 0
        Public Const Questionario As Integer = 1
        Public Const Ingiunzione As Integer = 2
        Public Const AccertamentoUfficio As Integer = 3
        Public Const AccertamentoRettifica As Integer = 4
        Public Const Rimborso As Integer = 5
        Public Const AutotutelaRettifica As Integer = 6
        Public Const AutotutelaAnnullamento As Integer = 7
        Public Const Coattivo As Integer = 8
        Public Const Ravvedimento As Integer = 9

        Public Sub New()
        End Sub
    End Class
    Public Class BaseCalcolo
        Public Const Versato As String = "IV"
        Public Const VersatoAcconto As String = "IVA"
        Public Const VersatoSaldo As String = "IVS"
        Public Const VersatoUnicaSoluzione As String = "IVUS"

        Public Sub New()
        End Sub
    End Class
    Public Class Capitolo
        Public Const DifferenzaImposta As String = "0001"
        Public Const Sanzioni As String = "0002"
        Public Const Interessi As String = "0003"
        Public Const Spese As String = "0004"

        Public Sub New()
        End Sub
    End Class

    Dim _ID_PROVVEDIMENTO As Integer
        Dim _COD_ENTE As String
        Dim _NUMERO_AVVISO As String
        Dim _NUMERO_ATTO As String
        Dim _COD_TRIBUTO As String
        Dim _DescrTributo As String
        Dim _TipoProvvedimento As Integer
        Dim _COD_CONTRIBUENTE As Integer
        Dim _COGNOME As String
        Dim _NOME As String
        Dim _CODICE_FISCALE As String
        Dim _PARTITA_IVA As String
        Dim _VIA_RES As String
        Dim _POSIZIONE_CIVICO_RES As String
        Dim _CIVICO_RES As String
        Dim _ESPONENTE_CIVICO_RES As String
        Dim _CAP_RES As String
        Dim _FRAZIONE_RES As String
        Dim _CITTA_RES As String
        Dim _PROVINCIA_RES As String
        Dim _CO As String
        Dim _VIA_CO As String
        Dim _POSIZIONE_CIVICO_CO As String
        Dim _CIVICO_CO As String
        Dim _ESPONENTE_CIVICO_CO As String
        Dim _CAP_CO As String
        Dim _FRAZIONE_CO As String
        Dim _CITTA_CO As String
        Dim _PROVINCIA_CO As String
        Dim _IMPORTO_DIFFERENZA_IMPOSTA As Double
        Dim _IMPORTO_SANZIONI As Double
        Dim _IMPORTO_SANZIONI_RIDOTTO As Double
        Dim _IMPORTO_TOT_SANZIONI_RIDUCIBILI As Double
        Dim _IMPORTO_TOT_SANZIONI_RIDOTTE As Double
        Dim _IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI As Double
        Dim _IMPORTO_INTERESSI As Double
        Dim _IMPORTO_SPESE As Double
        Dim _IMPORTO_ALTRO As Double
        Dim _IMPORTO_TOTALE As Double
        Dim _IMPORTO_ARROTONDAMENTO As Double
        Dim _IMPORTO_TOTALE_RIDOTTO As Double
        Dim _IMPORTO_ARROTONDAMENTO_RIDOTTO As Double
        Dim _IMPORTO_SENZA_ARROTONDAMENTO As Double
        Dim _DATA_CONSEGNA_AVVISO As String
        Dim _DATA_NOTIFICA_AVVISO As String
        Dim _DATA_RETTIFICA_AVVISO As String
        Dim _DATA_ANNULLAMENTO_AVVISO As String
        Dim _DATA_PERVENUTO_IL As String
        Dim _DATA_SCADENZA_QUESTIONARIO As String
        Dim _DATA_RIMBORSO As String
        Dim _DATA_SOSPENSIONE_AVVISO_AUTOTUTELA As String
        Dim _DATA_PRESENTAZIONE_RICORSO As String
        Dim _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA As String
        Dim _DATA_SENTENZA As String
        Dim _DATA_ATTO_DEFINITIVO As String
        Dim _DATA_VERSAMENTO_SOLUZIONE_UNICA As String
        Dim _DATA_CONCESSIONE_RATEIZZAZIONE As String
        Dim _IMPORTO_PAGATO As Double
        Dim _DATA_ELABORAZIONE As String
        Dim _DATA_CONFERMA As String
        Dim _DATA_STAMPA As String
        Dim _DATA_SOLLECITO_BONARIO As String
        Dim _DATA_RUOLO_ORDINARIO_TARSU As String
        Dim _DATA_COATTIVO As String
        Dim _DATA_PRESENTAZIONE_RICORSO_REGIONALE As String
        Dim _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE As String
        Dim _DATA_SENTENZA_REGIONALE As String
        Dim _DATA_PRESENTAZIONE_RICORSO_CASSAZIONE As String
        Dim _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE As String
        Dim _DATA_SENTENZA_CASSAZIONE As String
        Dim _PROGRESSIVO_ELABORAZIONE As Integer
        Dim _NOTE_PROVINCIALE As String
        Dim _NOTE_REGIONALE As String
        Dim _NOTE_CASSAZIONE As String
        Dim _ESITO_ACCERTAMENTO As Integer
        Dim _TERMINE_RICORSO_ACC As String
        Dim _NOTE_ACCERTAMENTO As String
        Dim _NOTE_CONCILIAZIONE_G As String
        Dim _FLAG_ACCERTAMENTO As Boolean
        Dim _FLAG_CONCILIAZIONE_G As Boolean
        Dim _IMPORTO_RUOLO_COATTIVO As Double
        Dim _NOTE_GENERALI_ATTO As String
        Dim _IMPORTO_ADDCOM As Double
        Dim _IMPORTO_ADDPROV As Double
        Dim _IMPORTO_DICHIARATO_F2 As Double
        Dim _IMPORTO_VERSATO_F2 As Double
        Dim _IMPORTO_DIFFERENZA_IMPOSTA_F2 As Double
        Dim _IMPORTO_SANZIONI_F2 As Double
        Dim _IMPORTO_INTERESSI_F2 As Double
        Dim _IMPORTO_TOTALE_F2 As Double
        Dim _IMPORTO_ACCERTATO_ACC As Double
        Dim _IMPORTO_DIFFERENZA_IMPOSTA_ACC As Double
        Dim _IMPORTO_SANZIONI_ACC As Double
        Dim _IMPORTO_SANZIONI_RIDOTTE_ACC As Double
        Dim _IMPORTO_INTERESSI_ACC As Double
        Dim _IMPORTO_TOTALE_ACC As Double
        Dim _NOMEPDF As String
        Dim _DATA_RIENTRO As DateTime
        Dim _DATA_IRREPERIBILE As String
        Dim _IDRUOLO As Integer
        Dim _ANNO As String
        Dim _ListInteressi() As ObjInteressiSanzioni
        Dim _Provenienza As Integer

        Public Sub New()
            MyBase.New
            Me.Reset()
        End Sub
#Region "Property"
        Public Property ID_PROVVEDIMENTO As Integer
            Get
                Return _ID_PROVVEDIMENTO
            End Get
            Set(ByVal value As Integer)
                _ID_PROVVEDIMENTO = value
            End Set
        End Property
        Public Property COD_ENTE As String
            Get
                Return _COD_ENTE
            End Get
            Set(ByVal value As String)
                _COD_ENTE = value
            End Set
        End Property
        Public Property NUMERO_AVVISO As String
            Get
                Return _NUMERO_AVVISO
            End Get
            Set(ByVal value As String)
                _NUMERO_AVVISO = value
            End Set
        End Property
        Public Property NUMERO_ATTO As String
            Get
                Return _NUMERO_ATTO
            End Get
            Set(ByVal value As String)
                _NUMERO_ATTO = value
            End Set
        End Property
        Public Property COD_TRIBUTO As String
            Get
                Return _COD_TRIBUTO
            End Get
            Set(ByVal value As String)
                _COD_TRIBUTO = value
            End Set
        End Property
        Public Property DescrTributo As String
            Get
                Return _DescrTributo
            End Get
            Set(ByVal value As String)
                _DescrTributo = value
            End Set
        End Property
        Public Property TipoProvvedimento As Integer
            Get
                Return _TipoProvvedimento
            End Get
            Set(ByVal value As Integer)
                _TipoProvvedimento = value
            End Set
        End Property
        Public Property COD_CONTRIBUENTE As Integer
            Get
                Return _COD_CONTRIBUENTE
            End Get
            Set(ByVal value As Integer)
                _COD_CONTRIBUENTE = value
            End Set
        End Property
        Public Property COGNOME As String
            Get
                Return _COGNOME
            End Get
            Set(ByVal value As String)
                _COGNOME = value
            End Set
        End Property
        Public Property NOME As String
            Get
                Return _NOME
            End Get
            Set(ByVal value As String)
                _NOME = value
            End Set
        End Property
        Public Property CODICE_FISCALE As String
            Get
                Return _CODICE_FISCALE
            End Get
            Set(ByVal value As String)
                _CODICE_FISCALE = value
            End Set
        End Property
        Public Property PARTITA_IVA As String
            Get
                Return _PARTITA_IVA
            End Get
            Set(ByVal value As String)
                _PARTITA_IVA = value
            End Set
        End Property
        Public Property VIA_RES As String
            Get
                Return _VIA_RES
            End Get
            Set(ByVal value As String)
                _VIA_RES = value
            End Set
        End Property
        Public Property POSIZIONE_CIVICO_RES As String
            Get
                Return _POSIZIONE_CIVICO_RES
            End Get
            Set(ByVal value As String)
                _POSIZIONE_CIVICO_RES = value
            End Set
        End Property
        Public Property CIVICO_RES As String
            Get
                Return _CIVICO_RES
            End Get
            Set(ByVal value As String)
                _CIVICO_RES = value
            End Set
        End Property
        Public Property ESPONENTE_CIVICO_RES As String
            Get
                Return _ESPONENTE_CIVICO_RES
            End Get
            Set(ByVal value As String)
                _ESPONENTE_CIVICO_RES = value
            End Set
        End Property
        Public Property CAP_RES As String
            Get
                Return _CAP_RES
            End Get
            Set(ByVal value As String)
                _CAP_RES = value
            End Set
        End Property
        Public Property FRAZIONE_RES As String
            Get
                Return _FRAZIONE_RES
            End Get
            Set(ByVal value As String)
                _FRAZIONE_RES = value
            End Set
        End Property
        Public Property CITTA_RES As String
            Get
                Return _CITTA_RES
            End Get
            Set(ByVal value As String)
                _CITTA_RES = value
            End Set
        End Property
        Public Property PROVINCIA_RES As String
            Get
                Return _PROVINCIA_RES
            End Get
            Set(ByVal value As String)
                _PROVINCIA_RES = value
            End Set
        End Property
        Public Property CO As String
            Get
                Return _CO
            End Get
            Set(ByVal value As String)
                _CO = value
            End Set
        End Property
        Public Property VIA_CO As String
            Get
                Return _VIA_CO
            End Get
            Set(ByVal value As String)
                _VIA_CO = value
            End Set
        End Property
        Public Property POSIZIONE_CIVICO_CO As String
            Get
                Return _POSIZIONE_CIVICO_CO
            End Get
            Set(ByVal value As String)
                _POSIZIONE_CIVICO_CO = value
            End Set
        End Property
        Public Property CIVICO_CO As String
            Get
                Return _CIVICO_CO
            End Get
            Set(ByVal value As String)
                _CIVICO_CO = value
            End Set
        End Property
        Public Property ESPONENTE_CIVICO_CO As String
            Get
                Return _ESPONENTE_CIVICO_CO
            End Get
            Set(ByVal value As String)
                _ESPONENTE_CIVICO_CO = value
            End Set
        End Property
        Public Property CAP_CO As String
            Get
                Return _CAP_CO
            End Get
            Set(ByVal value As String)
                _CAP_CO = value
            End Set
        End Property
        Public Property FRAZIONE_CO As String
            Get
                Return _FRAZIONE_CO
            End Get
            Set(ByVal value As String)
                _FRAZIONE_CO = value
            End Set
        End Property
        Public Property CITTA_CO As String
            Get
                Return _CITTA_CO
            End Get
            Set(ByVal value As String)
                _CITTA_CO = value
            End Set
        End Property
        Public Property PROVINCIA_CO As String
            Get
                Return _PROVINCIA_CO
            End Get
            Set(ByVal value As String)
                _PROVINCIA_CO = value
            End Set
        End Property
        Public Property IMPORTO_DIFFERENZA_IMPOSTA As Double
            Get
                Return _IMPORTO_DIFFERENZA_IMPOSTA
            End Get
            Set(ByVal value As Double)
                _IMPORTO_DIFFERENZA_IMPOSTA = value
            End Set
        End Property
        Public Property IMPORTO_SANZIONI As Double
            Get
                Return _IMPORTO_SANZIONI
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SANZIONI = value
            End Set
        End Property
        Public Property IMPORTO_SANZIONI_RIDOTTO As Double
            Get
                Return _IMPORTO_SANZIONI_RIDOTTO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SANZIONI_RIDOTTO = value
            End Set
        End Property
        Public Property IMPORTO_TOT_SANZIONI_RIDUCIBILI As Double
            Get
                Return _IMPORTO_TOT_SANZIONI_RIDUCIBILI
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOT_SANZIONI_RIDUCIBILI = value
            End Set
        End Property
        Public Property IMPORTO_TOT_SANZIONI_RIDOTTE As Double
            Get
                Return _IMPORTO_TOT_SANZIONI_RIDOTTE
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOT_SANZIONI_RIDOTTE = value
            End Set
        End Property
        Public Property IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI As Double
            Get
                Return _IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = value
            End Set
        End Property
        Public Property IMPORTO_INTERESSI As Double
            Get
                Return _IMPORTO_INTERESSI
            End Get
            Set(ByVal value As Double)
                _IMPORTO_INTERESSI = value
            End Set
        End Property
        Public Property IMPORTO_SPESE As Double
            Get
                Return _IMPORTO_SPESE
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SPESE = value
            End Set
        End Property
        Public Property IMPORTO_ALTRO As Double
            Get
                Return _IMPORTO_ALTRO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ALTRO = value
            End Set
        End Property
        Public Property IMPORTO_TOTALE As Double
            Get
                Return _IMPORTO_TOTALE
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOTALE = value
            End Set
        End Property
        Public Property IMPORTO_ARROTONDAMENTO As Double
            Get
                Return _IMPORTO_ARROTONDAMENTO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ARROTONDAMENTO = value
            End Set
        End Property
        Public Property IMPORTO_TOTALE_RIDOTTO As Double
            Get
                Return _IMPORTO_TOTALE_RIDOTTO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOTALE_RIDOTTO = value
            End Set
        End Property
        Public Property IMPORTO_ARROTONDAMENTO_RIDOTTO As Double
            Get
                Return _IMPORTO_ARROTONDAMENTO_RIDOTTO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ARROTONDAMENTO_RIDOTTO = value
            End Set
        End Property
        Public Property IMPORTO_SENZA_ARROTONDAMENTO As Double
            Get
                Return _IMPORTO_SENZA_ARROTONDAMENTO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SENZA_ARROTONDAMENTO = value
            End Set
        End Property
        Public Property DATA_CONSEGNA_AVVISO As String
            Get
                Return _DATA_CONSEGNA_AVVISO
            End Get
            Set(ByVal value As String)
                _DATA_CONSEGNA_AVVISO = value
            End Set
        End Property
        Public Property DATA_NOTIFICA_AVVISO As String
            Get
                Return _DATA_NOTIFICA_AVVISO
            End Get
            Set(ByVal value As String)
                _DATA_NOTIFICA_AVVISO = value
            End Set
        End Property
        Public Property DATA_RETTIFICA_AVVISO As String
            Get
                Return _DATA_RETTIFICA_AVVISO
            End Get
            Set(ByVal value As String)
                _DATA_RETTIFICA_AVVISO = value
            End Set
        End Property
        Public Property DATA_ANNULLAMENTO_AVVISO As String
            Get
                Return _DATA_ANNULLAMENTO_AVVISO
            End Get
            Set(ByVal value As String)
                _DATA_ANNULLAMENTO_AVVISO = value
            End Set
        End Property
        Public Property DATA_PERVENUTO_IL As String
            Get
                Return _DATA_PERVENUTO_IL
            End Get
            Set(ByVal value As String)
                _DATA_PERVENUTO_IL = value
            End Set
        End Property
        Public Property DATA_SCADENZA_QUESTIONARIO As String
            Get
                Return _DATA_SCADENZA_QUESTIONARIO
            End Get
            Set(ByVal value As String)
                _DATA_SCADENZA_QUESTIONARIO = value
            End Set
        End Property
        Public Property DATA_RIMBORSO As String
            Get
                Return _DATA_RIMBORSO
            End Get
            Set(ByVal value As String)
                _DATA_RIMBORSO = value
            End Set
        End Property
        Public Property DATA_SOSPENSIONE_AVVISO_AUTOTUTELA As String
            Get
                Return _DATA_SOSPENSIONE_AVVISO_AUTOTUTELA
            End Get
            Set(ByVal value As String)
                _DATA_SOSPENSIONE_AVVISO_AUTOTUTELA = value
            End Set
        End Property
        Public Property DATA_PRESENTAZIONE_RICORSO As String
            Get
                Return _DATA_PRESENTAZIONE_RICORSO
            End Get
            Set(ByVal value As String)
                _DATA_PRESENTAZIONE_RICORSO = value
            End Set
        End Property
        Public Property DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA As String
            Get
                Return _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA
            End Get
            Set(ByVal value As String)
                _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA = value
            End Set
        End Property
        Public Property DATA_SENTENZA As String
            Get
                Return _DATA_SENTENZA
            End Get
            Set(ByVal value As String)
                _DATA_SENTENZA = value
            End Set
        End Property
        Public Property DATA_ATTO_DEFINITIVO As String
            Get
                Return _DATA_ATTO_DEFINITIVO
            End Get
            Set(ByVal value As String)
                _DATA_ATTO_DEFINITIVO = value
            End Set
        End Property
        Public Property DATA_VERSAMENTO_SOLUZIONE_UNICA As String
            Get
                Return _DATA_VERSAMENTO_SOLUZIONE_UNICA
            End Get
            Set(ByVal value As String)
                _DATA_VERSAMENTO_SOLUZIONE_UNICA = value
            End Set
        End Property
        Public Property DATA_CONCESSIONE_RATEIZZAZIONE As String
            Get
                Return _DATA_CONCESSIONE_RATEIZZAZIONE
            End Get
            Set(ByVal value As String)
                _DATA_CONCESSIONE_RATEIZZAZIONE = value
            End Set
        End Property
        Public Property IMPORTO_PAGATO As Double
            Get
                Return _IMPORTO_PAGATO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_PAGATO = value
            End Set
        End Property
        Public Property DATA_ELABORAZIONE As String
            Get
                Return _DATA_ELABORAZIONE
            End Get
            Set(ByVal value As String)
                _DATA_ELABORAZIONE = value
            End Set
        End Property
        Public Property DATA_CONFERMA As String
            Get
                Return _DATA_CONFERMA
            End Get
            Set(ByVal value As String)
                _DATA_CONFERMA = value
            End Set
        End Property
        Public Property DATA_STAMPA As String
            Get
                Return _DATA_STAMPA
            End Get
            Set(ByVal value As String)
                _DATA_STAMPA = value
            End Set
        End Property
        Public Property DATA_SOLLECITO_BONARIO As String
            Get
                Return _DATA_SOLLECITO_BONARIO
            End Get
            Set(ByVal value As String)
                _DATA_SOLLECITO_BONARIO = value
            End Set
        End Property
        Public Property DATA_RUOLO_ORDINARIO_TARSU As String
            Get
                Return _DATA_RUOLO_ORDINARIO_TARSU
            End Get
            Set(ByVal value As String)
                _DATA_RUOLO_ORDINARIO_TARSU = value
            End Set
        End Property
        Public Property DATA_COATTIVO As String
            Get
                Return _DATA_COATTIVO
            End Get
            Set(ByVal value As String)
                _DATA_COATTIVO = value
            End Set
        End Property
        Public Property DATA_PRESENTAZIONE_RICORSO_REGIONALE As String
            Get
                Return _DATA_PRESENTAZIONE_RICORSO_REGIONALE
            End Get
            Set(ByVal value As String)
                _DATA_PRESENTAZIONE_RICORSO_REGIONALE = value
            End Set
        End Property
        Public Property DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE As String
            Get
                Return _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE
            End Get
            Set(ByVal value As String)
                _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE = value
            End Set
        End Property
        Public Property DATA_SENTENZA_REGIONALE As String
            Get
                Return _DATA_SENTENZA_REGIONALE
            End Get
            Set(ByVal value As String)
                _DATA_SENTENZA_REGIONALE = value
            End Set
        End Property
        Public Property DATA_PRESENTAZIONE_RICORSO_CASSAZIONE As String
            Get
                Return _DATA_PRESENTAZIONE_RICORSO_CASSAZIONE
            End Get
            Set(ByVal value As String)
                _DATA_PRESENTAZIONE_RICORSO_CASSAZIONE = value
            End Set
        End Property
        Public Property DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE As String
            Get
                Return _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE
            End Get
            Set(ByVal value As String)
                _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE = value
            End Set
        End Property
        Public Property DATA_SENTENZA_CASSAZIONE As String
            Get
                Return _DATA_SENTENZA_CASSAZIONE
            End Get
            Set(ByVal value As String)
                _DATA_SENTENZA_CASSAZIONE = value
            End Set
        End Property
        Public Property PROGRESSIVO_ELABORAZIONE As Integer
            Get
                Return _PROGRESSIVO_ELABORAZIONE
            End Get
            Set(ByVal value As Integer)
                _PROGRESSIVO_ELABORAZIONE = value
            End Set
        End Property
        Public Property NOTE_PROVINCIALE As String
            Get
                Return _NOTE_PROVINCIALE
            End Get
            Set(ByVal value As String)
                _NOTE_PROVINCIALE = value
            End Set
        End Property
        Public Property NOTE_REGIONALE As String
            Get
                Return _NOTE_REGIONALE
            End Get
            Set(ByVal value As String)
                _NOTE_REGIONALE = value
            End Set
        End Property
        Public Property NOTE_CASSAZIONE As String
            Get
                Return _NOTE_CASSAZIONE
            End Get
            Set(ByVal value As String)
                _NOTE_CASSAZIONE = value
            End Set
        End Property
        Public Property ESITO_ACCERTAMENTO As Integer
            Get
                Return _ESITO_ACCERTAMENTO
            End Get
            Set(ByVal value As Integer)
                _ESITO_ACCERTAMENTO = value
            End Set
        End Property
        Public Property TERMINE_RICORSO_ACC As String
            Get
                Return _TERMINE_RICORSO_ACC
            End Get
            Set(ByVal value As String)
                _TERMINE_RICORSO_ACC = value
            End Set
        End Property
        Public Property NOTE_ACCERTAMENTO As String
            Get
                Return _NOTE_ACCERTAMENTO
            End Get
            Set(ByVal value As String)
                _NOTE_ACCERTAMENTO = value
            End Set
        End Property
        Public Property NOTE_CONCILIAZIONE_G As String
            Get
                Return _NOTE_CONCILIAZIONE_G
            End Get
            Set(ByVal value As String)
                _NOTE_CONCILIAZIONE_G = value
            End Set
        End Property
        Public Property FLAG_ACCERTAMENTO As Boolean
            Get
                Return _FLAG_ACCERTAMENTO
            End Get
            Set(ByVal value As Boolean)
                _FLAG_ACCERTAMENTO = value
            End Set
        End Property
        Public Property FLAG_CONCILIAZIONE_G As Boolean
            Get
                Return _FLAG_CONCILIAZIONE_G
            End Get
            Set(ByVal value As Boolean)
                _FLAG_CONCILIAZIONE_G = value
            End Set
        End Property
        Public Property IMPORTO_RUOLO_COATTIVO As Double
            Get
                Return _IMPORTO_RUOLO_COATTIVO
            End Get
            Set(ByVal value As Double)
                _IMPORTO_RUOLO_COATTIVO = value
            End Set
        End Property
        Public Property NOTE_GENERALI_ATTO As String
            Get
                Return _NOTE_GENERALI_ATTO
            End Get
            Set(ByVal value As String)
                _NOTE_GENERALI_ATTO = value
            End Set
        End Property
        Public Property IMPORTO_ADDCOM As Double
            Get
                Return _IMPORTO_ADDCOM
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ADDCOM = value
            End Set
        End Property
        Public Property IMPORTO_ADDPROV As Double
            Get
                Return _IMPORTO_ADDPROV
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ADDPROV = value
            End Set
        End Property
        Public Property IMPORTO_DICHIARATO_F2 As Double
            Get
                Return _IMPORTO_DICHIARATO_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_DICHIARATO_F2 = value
            End Set
        End Property
        Public Property IMPORTO_VERSATO_F2 As Double
            Get
                Return _IMPORTO_VERSATO_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_VERSATO_F2 = value
            End Set
        End Property
        Public Property IMPORTO_DIFFERENZA_IMPOSTA_F2 As Double
            Get
                Return _IMPORTO_DIFFERENZA_IMPOSTA_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_DIFFERENZA_IMPOSTA_F2 = value
            End Set
        End Property
        Public Property IMPORTO_SANZIONI_F2 As Double
            Get
                Return _IMPORTO_SANZIONI_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SANZIONI_F2 = value
            End Set
        End Property
        Public Property IMPORTO_INTERESSI_F2 As Double
            Get
                Return _IMPORTO_INTERESSI_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_INTERESSI_F2 = value
            End Set
        End Property
        Public Property IMPORTO_TOTALE_F2 As Double
            Get
                Return _IMPORTO_TOTALE_F2
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOTALE_F2 = value
            End Set
        End Property
        Public Property IMPORTO_ACCERTATO_ACC As Double
            Get
                Return _IMPORTO_ACCERTATO_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_ACCERTATO_ACC = value
            End Set
        End Property
        Public Property IMPORTO_DIFFERENZA_IMPOSTA_ACC As Double
            Get
                Return _IMPORTO_DIFFERENZA_IMPOSTA_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_DIFFERENZA_IMPOSTA_ACC = value
            End Set
        End Property
        Public Property IMPORTO_SANZIONI_ACC As Double
            Get
                Return _IMPORTO_SANZIONI_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SANZIONI_ACC = value
            End Set
        End Property
        Public Property IMPORTO_SANZIONI_RIDOTTE_ACC As Double
            Get
                Return _IMPORTO_SANZIONI_RIDOTTE_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_SANZIONI_RIDOTTE_ACC = value
            End Set
        End Property
        Public Property IMPORTO_INTERESSI_ACC As Double
            Get
                Return _IMPORTO_INTERESSI_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_INTERESSI_ACC = value
            End Set
        End Property
        Public Property IMPORTO_TOTALE_ACC As Double
            Get
                Return _IMPORTO_TOTALE_ACC
            End Get
            Set(ByVal value As Double)
                _IMPORTO_TOTALE_ACC = value
            End Set
        End Property
        Public Property NOMEPDF As String
            Get
                Return _NOMEPDF
            End Get
            Set(ByVal value As String)
                _NOMEPDF = value
            End Set
        End Property
        Public Property DATA_RIENTRO As DateTime
            Get
                Return _DATA_RIENTRO
            End Get
            Set(ByVal value As DateTime)
                _DATA_RIENTRO = value
            End Set
        End Property
        Public Property DATA_IRREPERIBILE As String
            Get
                Return _DATA_IRREPERIBILE
            End Get
            Set(ByVal value As String)
                _DATA_IRREPERIBILE = value
            End Set
        End Property
        Public Property IDRUOLO As Integer
            Get
                Return _IDRUOLO
            End Get
            Set(ByVal value As Integer)
                _IDRUOLO = value
            End Set
        End Property
        Public Property ANNO As String
            Get
                Return _ANNO
            End Get
            Set(ByVal value As String)
                _ANNO = value
            End Set
        End Property
        Public Property ListInteressi As ObjInteressiSanzioni()
            Get
                Return _ListInteressi
            End Get
            Set(ByVal value As ObjInteressiSanzioni())
                _ListInteressi = value
            End Set
        End Property
        Public Property Provenienza As Integer
            Get
                Return _Provenienza
            End Get
            Set(ByVal value As Integer)
                _Provenienza = value
            End Set
        End Property
#End Region
        Public Sub Reset()
            _ID_PROVVEDIMENTO = -1
            _COD_ENTE = ""
            _NUMERO_AVVISO = ""
            _NUMERO_ATTO = ""
            _COD_TRIBUTO = ""
            _DescrTributo = ""
            _COD_CONTRIBUENTE = -1
            _COGNOME = ""
            _NOME = ""
            _CODICE_FISCALE = ""
            _PARTITA_IVA = ""
            _VIA_RES = ""
            _POSIZIONE_CIVICO_RES = ""
            _CIVICO_RES = ""
            _ESPONENTE_CIVICO_RES = ""
            _CAP_RES = ""
            _FRAZIONE_RES = ""
            _CITTA_RES = ""
            _PROVINCIA_RES = ""
            _CO = ""
            _VIA_CO = ""
            _POSIZIONE_CIVICO_CO = ""
            _CIVICO_CO = ""
            _ESPONENTE_CIVICO_CO = ""
            _CAP_CO = ""
            _FRAZIONE_CO = ""
            _CITTA_CO = ""
            _PROVINCIA_CO = ""
            _IMPORTO_DIFFERENZA_IMPOSTA = 0
            _IMPORTO_SANZIONI = 0
            _IMPORTO_SANZIONI_RIDOTTO = 0
            _IMPORTO_TOT_SANZIONI_RIDUCIBILI = 0
            _IMPORTO_TOT_SANZIONI_RIDOTTE = 0
            _IMPORTO_TOT_SANZIONI_NON_RIDUCIBILI = 0
            _IMPORTO_INTERESSI = 0
            _IMPORTO_SPESE = 0
            _IMPORTO_ALTRO = 0
            _IMPORTO_TOTALE = 0
            _IMPORTO_ARROTONDAMENTO = 0
            _IMPORTO_TOTALE_RIDOTTO = 0
            _IMPORTO_ARROTONDAMENTO_RIDOTTO = 0
            _IMPORTO_SENZA_ARROTONDAMENTO = 0
            _DATA_CONSEGNA_AVVISO = ""
            _DATA_NOTIFICA_AVVISO = ""
            _DATA_RETTIFICA_AVVISO = ""
            _DATA_ANNULLAMENTO_AVVISO = ""
            _DATA_PERVENUTO_IL = ""
            _DATA_SCADENZA_QUESTIONARIO = ""
            _DATA_RIMBORSO = ""
            _DATA_SOSPENSIONE_AVVISO_AUTOTUTELA = ""
            _DATA_PRESENTAZIONE_RICORSO = ""
            _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA = ""
            _DATA_SENTENZA = ""
            _DATA_ATTO_DEFINITIVO = ""
            _DATA_VERSAMENTO_SOLUZIONE_UNICA = ""
            _DATA_CONCESSIONE_RATEIZZAZIONE = ""
            _IMPORTO_PAGATO = 0
            _DATA_ELABORAZIONE = ""
            _DATA_CONFERMA = ""
            _DATA_STAMPA = ""
            _DATA_SOLLECITO_BONARIO = ""
            _DATA_RUOLO_ORDINARIO_TARSU = ""
            _DATA_COATTIVO = ""
            _DATA_PRESENTAZIONE_RICORSO_REGIONALE = ""
            _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_REGIONALE = ""
            _DATA_SENTENZA_REGIONALE = ""
            _DATA_PRESENTAZIONE_RICORSO_CASSAZIONE = ""
            _DATA_SOSPENSIONE_DA_COMMISSIONE_TRIBUTARIA_CASSAZIONE = ""
            _DATA_SENTENZA_CASSAZIONE = ""
            _PROGRESSIVO_ELABORAZIONE = -1
            _NOTE_PROVINCIALE = ""
            _NOTE_REGIONALE = ""
            _NOTE_CASSAZIONE = ""
            _ESITO_ACCERTAMENTO = -1
            _TERMINE_RICORSO_ACC = ""
            _NOTE_ACCERTAMENTO = ""
            _NOTE_CONCILIAZIONE_G = ""
            _FLAG_ACCERTAMENTO = False
            _FLAG_CONCILIAZIONE_G = False
            _IMPORTO_RUOLO_COATTIVO = 0
            _NOTE_GENERALI_ATTO = ""
            _IMPORTO_ADDCOM = 0
            _IMPORTO_ADDPROV = 0
            _IMPORTO_DICHIARATO_F2 = 0
            _IMPORTO_VERSATO_F2 = 0
            _IMPORTO_DIFFERENZA_IMPOSTA_F2 = 0
            _IMPORTO_SANZIONI_F2 = 0
            _IMPORTO_INTERESSI_F2 = 0
            _IMPORTO_TOTALE_F2 = 0
            _IMPORTO_ACCERTATO_ACC = 0
            _IMPORTO_DIFFERENZA_IMPOSTA_ACC = 0
            _IMPORTO_SANZIONI_ACC = 0
            _IMPORTO_SANZIONI_RIDOTTE_ACC = 0
            _IMPORTO_INTERESSI_ACC = 0
            _IMPORTO_TOTALE_ACC = 0
            _NOMEPDF = ""
            _DATA_RIENTRO = DateTime.MaxValue
            _DATA_IRREPERIBILE = ""
            _IDRUOLO = -1
            _ANNO = ""
            _ListInteressi = CType(New ArrayList().ToArray(GetType(ObjInteressiSanzioni)), ObjInteressiSanzioni())
            _Provenienza = 1
        End Sub
    End Class
''' <summary>
''' Definizione oggetto coattivo
''' </summary>
<Serializable>
Public Class ObjCoattivo
    Inherits OggettoAtto
    Dim _Id As Integer
    Dim _IdFlusso As Integer
    Dim _ImportoCoattivo As Double
    Dim _InteressiCoattivo As Double
    Dim _SpeseCoattivo As Double
    Dim _ArrotondamentoCoattivo As Double
    Dim _TotaleCoattivo As Double
    Dim _DataInserimento As DateTime
    Dim _DataVariazione As DateTime
    Public Property Id As Integer
        Get
            Return _Id
        End Get
        Set(ByVal value As Integer)
            _Id = value
        End Set
    End Property
    Public Property IdFlusso As Integer
        Get
            Return _IdFlusso
        End Get
        Set(ByVal value As Integer)
            _IdFlusso = value
        End Set
    End Property
    Public Property ImportoCoattivo As Double
        Get
            Return _ImportoCoattivo
        End Get
        Set(ByVal value As Double)
            _ImportoCoattivo = value
        End Set
    End Property
    Public Property InteressiCoattivo As Double
        Get
            Return _InteressiCoattivo
        End Get
        Set(ByVal value As Double)
            _InteressiCoattivo = value
        End Set
    End Property
    Public Property SpeseCoattivo As Double
        Get
            Return _SpeseCoattivo
        End Get
        Set(ByVal value As Double)
            _SpeseCoattivo = value
        End Set
    End Property
    Public Property ArrotondamentoCoattivo As Double
        Get
            Return _ArrotondamentoCoattivo
        End Get
        Set(ByVal value As Double)
            _ArrotondamentoCoattivo = value
        End Set
    End Property
    Public Property TotaleCoattivo As Double
        Get
            Return _TotaleCoattivo
        End Get
        Set(ByVal value As Double)
            _TotaleCoattivo = value
        End Set
    End Property
    Public Property DataInserimento As DateTime
        Get
            Return _DataInserimento
        End Get
        Set(ByVal value As DateTime)
            _DataInserimento = value
        End Set
    End Property
    Public Property DataVariazione As DateTime
        Get
            Return _DataVariazione
        End Get
        Set(ByVal value As DateTime)
            _DataVariazione = value
        End Set
    End Property
    Public Sub New()
        MyBase.New
        Me.ResetCoattivo()
    End Sub
    Public Sub ResetCoattivo()
        _Id = -1
        _IdFlusso = -1
        _ImportoCoattivo = 0
        _InteressiCoattivo = 0
        _SpeseCoattivo = 0
        _ArrotondamentoCoattivo = 0
        _TotaleCoattivo = 0
        _DataInserimento = DateTime.MaxValue
        _DataVariazione = DateTime.MaxValue
    End Sub
End Class
''' <summary>
''' Definizione oggetto dati base calcolo interessi e sanzioni
''' </summary>
<Serializable()>
Public Class ObjBaseIntSanz
    Dim _COD_ENTE As String
    Dim _CONTRIBUENTE As Integer
    Dim _ANNO As String
    Dim _DICHIARATO As Double
    Dim _PAGATO As Double
    Dim _ACCERTATO As Double
    Dim _DIFFERENZAIMPOSTA As Double
    Dim _DIFFERENZAIMPOSTAACCONTO As Double
    Dim _DIFFERENZAIMPOSTASALDO As Double
    Dim _INTERESSI As Double
    Dim _SANZIONI As Double
    Dim _SANZIONIRIDOTTO As Double
    Dim _QUOTARIDUZIONE As Double
    Dim _INTERESSIF2 As Double
    Dim _SANZIONIF2 As Double
    Dim _INTERESSIACC As Double
    Dim _SANZIONIACC As Double
    Dim _SPESE As Double
    Dim _COD_TIPO_PROVVEDIMENTO As Integer
    Dim _MODALITAUNICASOLUZIONE As Boolean
    Dim _VERSAMENTOTARDIVO As Boolean
    Public Property IdEnte As String
        Get
            Return _COD_ENTE
        End Get
        Set(ByVal value As String)
            _COD_ENTE = value
        End Set
    End Property
    Public Property IdContribuente As Integer
        Get
            Return _CONTRIBUENTE
        End Get
        Set(ByVal value As Integer)
            _CONTRIBUENTE = value
        End Set
    End Property
    Public Property Anno As String
        Get
            Return _ANNO
        End Get
        Set(ByVal value As String)
            _ANNO = value
        End Set
    End Property
    Public Property Dichiarato As Double
        Get
            Return _DICHIARATO
        End Get
        Set(ByVal value As Double)
            _DICHIARATO = value
        End Set
    End Property
    Public Property Pagato As Double
        Get
            Return _PAGATO
        End Get
        Set(ByVal value As Double)
            _PAGATO = value
        End Set
    End Property
    Public Property Accertato As Double
        Get
            Return _ACCERTATO
        End Get
        Set(ByVal value As Double)
            _ACCERTATO = value
        End Set
    End Property
    Public Property DifferenzaImposta As Double
        Get
            Return _DIFFERENZAIMPOSTA
        End Get
        Set(ByVal value As Double)
            _DIFFERENZAIMPOSTA = value
        End Set
    End Property
    Public Property DifferenzaImpostaAcconto As Double
        Get
            Return _DIFFERENZAIMPOSTAACCONTO
        End Get
        Set(ByVal value As Double)
            _DIFFERENZAIMPOSTAACCONTO = value
        End Set
    End Property
    Public Property DifferenzaImpostaSaldo As Double
        Get
            Return _DIFFERENZAIMPOSTASALDO
        End Get
        Set(ByVal value As Double)
            _DIFFERENZAIMPOSTASALDO = value
        End Set
    End Property
    Public Property Interessi As Double
        Get
            Return _INTERESSI
        End Get
        Set(ByVal value As Double)
            _INTERESSI = value
        End Set
    End Property
    Public Property Sanzioni As Double
        Get
            Return _SANZIONI
        End Get
        Set(ByVal value As Double)
            _SANZIONI = value
        End Set
    End Property
    Public Property SanzioniRidotto As Double
        Get
            Return _SANZIONIRIDOTTO
        End Get
        Set(ByVal value As Double)
            _SANZIONIRIDOTTO = value
        End Set
    End Property
    Public Property QuotaRiduzione As Double
        Get
            Return _QUOTARIDUZIONE
        End Get
        Set(ByVal value As Double)
            _QUOTARIDUZIONE = value
        End Set
    End Property
    Public Property InteressiF2 As Double
        Get
            Return _INTERESSIF2
        End Get
        Set(ByVal value As Double)
            _INTERESSIF2 = value
        End Set
    End Property
    Public Property SanzioniF2 As Double
        Get
            Return _SANZIONIF2
        End Get
        Set(ByVal value As Double)
            _SANZIONIF2 = value
        End Set
    End Property
    Public Property InteressiAcc As Double
        Get
            Return _INTERESSIACC
        End Get
        Set(ByVal value As Double)
            _INTERESSIACC = value
        End Set
    End Property
    Public Property SanzioniAcc As Double
        Get
            Return _SANZIONIACC
        End Get
        Set(ByVal value As Double)
            _SANZIONIACC = value
        End Set
    End Property
    Public Property Spese As Double
        Get
            Return _SPESE
        End Get
        Set(ByVal value As Double)
            _SPESE = value
        End Set
    End Property
    Public Property COD_TIPO_PROVVEDIMENTO As Integer
        Get
            Return _COD_TIPO_PROVVEDIMENTO
        End Get
        Set(ByVal value As Integer)
            _COD_TIPO_PROVVEDIMENTO = value
        End Set
    End Property
    Public Property ModalitaUnicaSoluzione As Boolean
        Get
            Return _MODALITAUNICASOLUZIONE
        End Get
        Set(ByVal value As Boolean)
            _MODALITAUNICASOLUZIONE = value
        End Set
    End Property
    Public Property VersamentoTardivo As Boolean
        Get
            Return _VERSAMENTOTARDIVO
        End Get
        Set(ByVal value As Boolean)
            _VERSAMENTOTARDIVO = value
        End Set
    End Property
    Public Sub New()
        MyBase.New
        Me.Reset()
    End Sub
    Public Sub Reset()
        _COD_ENTE = ""
        _CONTRIBUENTE = -1
        _ANNO = ""
        _DICHIARATO = 0
        _PAGATO = 0
        _DIFFERENZAIMPOSTA = 0
        _DIFFERENZAIMPOSTAACCONTO = 0
        _DIFFERENZAIMPOSTASALDO = 0
        _INTERESSI = 0
        _SANZIONI = 0
        _SANZIONIRIDOTTO = 0
        _QUOTARIDUZIONE = 0
        _COD_TIPO_PROVVEDIMENTO = 0
        _MODALITAUNICASOLUZIONE = False
        _VERSAMENTOTARDIVO = False
    End Sub
End Class
''' <summary>
''' Definizione oggetto appoggio calcolco interessi e sanzioni
''' </summary>
<Serializable()>
Public Class ObjAppoggioIntSanz
    Dim _ANNO As String
    Dim _IVA As String
    Dim _IVS As String
    Dim _IVUS As String
    Dim _IV As String
    Dim _DI As String
    Dim _GG As String
    Public Property Anno As String
        Get
            Return _ANNO
        End Get
        Set(ByVal value As String)
            _ANNO = value
        End Set
    End Property
    Public Property IVA As String
        Get
            Return _IVA
        End Get
        Set(ByVal value As String)
            _IVA = value
        End Set
    End Property
    Public Property IVS As String
        Get
            Return _IVS
        End Get
        Set(ByVal value As String)
            _IVS = value
        End Set
    End Property
    Public Property IVUS As String
        Get
            Return _IVUS
        End Get
        Set(ByVal value As String)
            _IVUS = value
        End Set
    End Property
    Public Property IV As String
        Get
            Return _IV
        End Get
        Set(ByVal value As String)
            _IV = value
        End Set
    End Property
    Public Property DI As String
        Get
            Return _DI
        End Get
        Set(ByVal value As String)
            _DI = value
        End Set
    End Property
    Public Property GG As String
        Get
            Return _GG
        End Get
        Set(ByVal value As String)
            _GG = value
        End Set
    End Property
    Public Sub New()
        MyBase.New
        Me.Reset()
    End Sub
    Public Sub Reset()
        _ANNO = ""
        _IVA = ""
        _IVS = ""
        _IVUS = ""
        _IV = ""
        _DI = ""
        _GG = ""
    End Sub
End Class
''' <summary>
''' Definizione oggetto calcolo interessi e sanzioni
''' </summary>
<Serializable()>
Public Class ObjInteressiSanzioni
    Dim _ID As Integer
    Dim _COD_ENTE As String
    Dim _ANNO As String
    Dim _COD_VOCE As String
    Dim _IMPORTO As Double
    Dim _IMPORTO_GIORNI As Double
    Dim _IMPORTO_RIDOTTO As Double
    Dim _ACCONTO As Double
    Dim _ACCONTO_GIORNI As Double
    Dim _SALDO As Double
    Dim _SALDO_GIORNI As Double
    Dim _DATA_INIZIO As String
    Dim _DATA_FINE As String
    Dim _N_SEMESTRI_ACCONTO As Integer
    Dim _N_SEMESTRI_SALDO As Integer
    Dim _N_SEMESTRI_TOTALI As Integer
    Dim _TASSO As Double
    Dim _IdFase As Integer
    Dim _ID_LEGAME As Integer
    Dim _MOTIVAZIONI As String
    Dim _N_GIORNI_ACCONTO As Integer
    Dim _N_GIORNI_SALDO As Integer
    Dim _N_GIORNI_TOTALI As Integer
    Dim _COD_TIPO_PROVVEDIMENTO As Integer
    Dim _QUOTARIDUZIONE As Double
    Public Property ID As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property
    Public Property COD_ENTE As String
        Get
            Return _COD_ENTE
        End Get
        Set(ByVal value As String)
            _COD_ENTE = value
        End Set
    End Property
    Public Property ANNO As String
        Get
            Return _ANNO
        End Get
        Set(ByVal value As String)
            _ANNO = value
        End Set
    End Property
    Public Property COD_VOCE As String
        Get
            Return _COD_VOCE
        End Get
        Set(ByVal value As String)
            _COD_VOCE = value
        End Set
    End Property
    Public Property IMPORTO As Double
        Get
            Return _IMPORTO
        End Get
        Set(ByVal value As Double)
            _IMPORTO = value
        End Set
    End Property
    Public Property IMPORTO_GIORNI As Double
        Get
            Return _IMPORTO_GIORNI
        End Get
        Set(ByVal value As Double)
            _IMPORTO_GIORNI = value
        End Set
    End Property
    Public Property IMPORTO_RIDOTTO As Double
        Get
            Return _IMPORTO_RIDOTTO
        End Get
        Set(ByVal value As Double)
            _IMPORTO_RIDOTTO = value
        End Set
    End Property
    Public Property ACCONTO As Double
        Get
            Return _ACCONTO
        End Get
        Set(ByVal value As Double)
            _ACCONTO = value
        End Set
    End Property
    Public Property ACCONTO_GIORNI As Double
        Get
            Return _ACCONTO_GIORNI
        End Get
        Set(ByVal value As Double)
            _ACCONTO_GIORNI = value
        End Set
    End Property
    Public Property SALDO As Double
        Get
            Return _SALDO
        End Get
        Set(ByVal value As Double)
            _SALDO = value
        End Set
    End Property
    Public Property SALDO_GIORNI As Double
        Get
            Return _SALDO_GIORNI
        End Get
        Set(ByVal value As Double)
            _SALDO_GIORNI = value
        End Set
    End Property
    Public Property DATA_INIZIO As String
        Get
            Return _DATA_INIZIO
        End Get
        Set(ByVal value As String)
            _DATA_INIZIO = value
        End Set
    End Property
    Public Property DATA_FINE As String
        Get
            Return _DATA_FINE
        End Get
        Set(ByVal value As String)
            _DATA_FINE = value
        End Set
    End Property
    Public Property N_SEMESTRI_ACCONTO As Integer
        Get
            Return _N_SEMESTRI_ACCONTO
        End Get
        Set(ByVal value As Integer)
            _N_SEMESTRI_ACCONTO = value
        End Set
    End Property
    Public Property N_SEMESTRI_SALDO As Integer
        Get
            Return _N_SEMESTRI_SALDO
        End Get
        Set(ByVal value As Integer)
            _N_SEMESTRI_SALDO = value
        End Set
    End Property
    Public Property N_SEMESTRI_TOTALI As Integer
        Get
            Return _N_SEMESTRI_TOTALI
        End Get
        Set(ByVal value As Integer)
            _N_SEMESTRI_TOTALI = value
        End Set
    End Property
    Public Property TASSO As Double
        Get
            Return _TASSO
        End Get
        Set(ByVal value As Double)
            _TASSO = value
        End Set
    End Property
    Public Property IdFase As Integer
        Get
            Return _IdFase
        End Get
        Set(ByVal value As Integer)
            _IdFase = value
        End Set
    End Property
    Public Property ID_LEGAME As Integer
        Get
            Return _ID_LEGAME
        End Get
        Set(ByVal value As Integer)
            _ID_LEGAME = value
        End Set
    End Property
    Public Property MOTIVAZIONI As String
        Get
            Return _MOTIVAZIONI
        End Get
        Set(ByVal value As String)
            _MOTIVAZIONI = value
        End Set
    End Property
    Public Property N_GIORNI_ACCONTO As Integer
        Get
            Return _N_GIORNI_ACCONTO
        End Get
        Set(ByVal value As Integer)
            _N_GIORNI_ACCONTO = value
        End Set
    End Property
    Public Property N_GIORNI_SALDO As Integer
        Get
            Return _N_GIORNI_SALDO
        End Get
        Set(ByVal value As Integer)
            _N_GIORNI_SALDO = value
        End Set
    End Property
    Public Property N_GIORNI_TOTALI As Integer
        Get
            Return _N_GIORNI_TOTALI
        End Get
        Set(ByVal value As Integer)
            _N_GIORNI_TOTALI = value
        End Set
    End Property
    Public Property COD_TIPO_PROVVEDIMENTO As Integer
        Get
            Return _COD_TIPO_PROVVEDIMENTO
        End Get
        Set(ByVal value As Integer)
            _COD_TIPO_PROVVEDIMENTO = value
        End Set
    End Property
    Public Property QuotaRiduzione As Double
        Get
            Return _QUOTARIDUZIONE
        End Get
        Set(ByVal value As Double)
            _QUOTARIDUZIONE = value
        End Set
    End Property
    Public Sub New()
        MyBase.New
        Me.Reset()
    End Sub
    Public Sub Reset()
        _ID = -1
        _COD_ENTE = ""
        _ANNO = ""
        _COD_VOCE = ""
        _IMPORTO = 0
        _IMPORTO_GIORNI = 0
        _IMPORTO_RIDOTTO = 0
        _ACCONTO = 0
        _ACCONTO_GIORNI = 0
        _SALDO = 0
        _SALDO_GIORNI = 0
        _DATA_INIZIO = ""
        _DATA_FINE = ""
        _N_SEMESTRI_ACCONTO = 0
        _N_SEMESTRI_SALDO = 0
        _N_SEMESTRI_TOTALI = 0
        _TASSO = 0
        _IdFase = 0
        _ID_LEGAME = 0
        _MOTIVAZIONI = ""
        _N_GIORNI_ACCONTO = 0
        _N_GIORNI_SALDO = 0
        _N_GIORNI_TOTALI = 0
        _COD_TIPO_PROVVEDIMENTO = 0
        _QUOTARIDUZIONE = 1
    End Sub
End Class
''' <summary>
''' Definizione oggetto versamento
''' </summary>
<Serializable> Public Class VersamentiRow
    Dim _ID As Integer = 0
    Dim _Ente As String = ""
    Dim _IdAnagrafico As Integer = 0
    Dim _CodTributo As String = ""
    Dim _AnnoRiferimento As String = ""
    Dim _CodiceFiscale As String = ""
    Dim _PartitaIva As String = ""
    Dim _ImportoPagato As Double = 0
    Dim _DataPagamento As DateTime = DateTime.MaxValue
    Dim _NumeroBollettino As String = ""
    Dim _NumeroFabbricatiPosseduti As Integer = 0
    Dim _Acconto As Boolean = False
    Dim _Saldo As Boolean = False
    Dim _RavvedimentoOperoso As Boolean = False
    Dim _ImportoTerreni As Double = 0
    Dim _ImportoAreeFabbric As Double = 0
    Dim _ImportoAbitazPrincipale As Double = 0
    Dim _ImportoAltriFabbric As Double = 0
    Dim _DetrazioneAbitazPrincipale As Double = 0
    Dim _ImportoTerreniStatale As Double = 0
    Dim _ImportoAreeFabbricStatale As Double = 0
    Dim _ImportoAltriFabbricStatale As Double = 0
    Dim _ImportoFabRurUsoStrum As Double = 0
    Dim _ImportoFabRurUsoStrumStatale As Double = 0
    Dim _ImportoUsoProdCatD As Double = 0
    Dim _ImportoUsoProdCatDStatale As Double = 0
    Dim _ContoCorrente As String = ""
    Dim _ComuneUbicazioneImmobile As String = ""
    Dim _ComuneIntestatario As String = ""
    Dim _Bonificato As Boolean = False
    Dim _DataInizioValidità As DateTime = DateTime.MaxValue
    Dim _DataFineValidità As DateTime = DateTime.MaxValue
    Dim _Operatore As String = ""
    Dim _Annullato As Boolean = False
    Dim _ImportoSoprattassa As Double = 0
    Dim _ImportoPenaPecuniaria As Double = 0
    Dim _Interessi As Double = 0
    Dim _Violazione As Boolean = False
    Dim _IDProvenienza As Integer = 0
    Dim _NumeroAttoAccertamento As String = ""
    Dim _DataProvvedimentoViolazione As DateTime = DateTime.MaxValue
    Dim _ImportoPagatoArrotondamento As Double = 0
    Dim _DataRiversamento As DateTime = DateTime.MaxValue
    Dim _FlagFabbricatiExRurali As Boolean = False
    Dim _NumeroProvvedimentoViolazione As String = ""
    Dim _ImportoImposta As Double = 0
    Dim _Note As String = ""
    Dim _DetrazioneStatale As Double = 0
    Dim _Provenienza As String = ""
    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property
    Public Property Ente() As String
        Get
            Return _Ente
        End Get
        Set(ByVal value As String)
            _Ente = value
        End Set
    End Property
    Public Property IdAnagrafico() As Integer
        Get
            Return _IdAnagrafico
        End Get
        Set(ByVal value As Integer)
            _IdAnagrafico = value
        End Set
    End Property
    Public Property CodTributo() As String
        Get
            Return _CodTributo
        End Get
        Set(ByVal value As String)
            _CodTributo = value
        End Set
    End Property
    Public Property AnnoRiferimento() As String
        Get
            Return _AnnoRiferimento
        End Get
        Set(ByVal value As String)
            _AnnoRiferimento = value
        End Set
    End Property
    Public Property CodiceFiscale() As String
        Get
            Return _CodiceFiscale
        End Get
        Set(ByVal value As String)
            _CodiceFiscale = value
        End Set
    End Property
    Public Property PartitaIva() As String
        Get
            Return _PartitaIva
        End Get
        Set(ByVal value As String)
            _PartitaIva = value
        End Set
    End Property
    Public Property ImportoPagato() As Double
        Get
            Return _ImportoPagato
        End Get
        Set(ByVal value As Double)
            _ImportoPagato = value
        End Set
    End Property
    Public Property DataPagamento() As DateTime
        Get
            Return _DataPagamento
        End Get
        Set(ByVal value As DateTime)
            _DataPagamento = value
        End Set
    End Property
    Public Property NumeroBollettino() As String
        Get
            Return _NumeroBollettino
        End Get
        Set(ByVal value As String)
            _NumeroBollettino = value
        End Set
    End Property
    Public Property NumeroFabbricatiPosseduti() As Integer
        Get
            Return _NumeroFabbricatiPosseduti
        End Get
        Set(ByVal value As Integer)
            _NumeroFabbricatiPosseduti = value
        End Set
    End Property
    Public Property Acconto() As Boolean
        Get
            Return _Acconto
        End Get
        Set(ByVal value As Boolean)
            _Acconto = value
        End Set
    End Property
    Public Property Saldo() As Boolean
        Get
            Return _Saldo
        End Get
        Set(ByVal value As Boolean)
            _Saldo = value
        End Set
    End Property
    Public Property RavvedimentoOperoso() As Boolean
        Get
            Return _RavvedimentoOperoso
        End Get
        Set(ByVal value As Boolean)
            _RavvedimentoOperoso = value
        End Set
    End Property
    Public Property ImportoTerreni() As Double
        Get
            Return _ImportoTerreni
        End Get
        Set(ByVal value As Double)
            _ImportoTerreni = value
        End Set
    End Property
    Public Property ImportoAreeFabbric() As Double
        Get
            Return _ImportoAreeFabbric
        End Get
        Set(ByVal value As Double)
            _ImportoAreeFabbric = value
        End Set
    End Property
    Public Property ImportoAbitazPrincipale() As Double
        Get
            Return _ImportoAbitazPrincipale
        End Get
        Set(ByVal value As Double)
            _ImportoAbitazPrincipale = value
        End Set
    End Property
    Public Property ImportoAltriFabbric() As Double
        Get
            Return _ImportoAltriFabbric
        End Get
        Set(ByVal value As Double)
            _ImportoAltriFabbric = value
        End Set
    End Property
    Public Property DetrazioneAbitazPrincipale() As Double
        Get
            Return _DetrazioneAbitazPrincipale
        End Get
        Set(ByVal value As Double)
            _DetrazioneAbitazPrincipale = value
        End Set
    End Property
    Public Property ImportoTerreniStatale() As Double
        Get
            Return _ImportoTerreniStatale
        End Get
        Set(ByVal value As Double)
            _ImportoTerreniStatale = value
        End Set
    End Property
    Public Property ImportoAreeFabbricStatale() As Double
        Get
            Return _ImportoAreeFabbricStatale
        End Get
        Set(ByVal value As Double)
            _ImportoAreeFabbricStatale = value
        End Set
    End Property
    Public Property ImportoAltriFabbricStatale() As Double
        Get
            Return _ImportoAltriFabbricStatale
        End Get
        Set(ByVal value As Double)
            _ImportoAltriFabbricStatale = value
        End Set
    End Property
    Public Property ImportoFabRurUsoStrum() As Double
        Get
            Return _ImportoFabRurUsoStrum
        End Get
        Set(ByVal value As Double)
            _ImportoFabRurUsoStrum = value
        End Set
    End Property
    Public Property ImportoFabRurUsoStrumStatale() As Double
        Get
            Return _ImportoFabRurUsoStrumStatale
        End Get
        Set(ByVal value As Double)
            _ImportoFabRurUsoStrumStatale = value
        End Set
    End Property
    Public Property ImportoUsoProdCatD() As Double
        Get
            Return _ImportoUsoProdCatD
        End Get
        Set(ByVal value As Double)
            _ImportoUsoProdCatD = value
        End Set
    End Property
    Public Property ImportoUsoProdCatDStatale() As Double
        Get
            Return _ImportoUsoProdCatDStatale
        End Get
        Set(ByVal value As Double)
            _ImportoUsoProdCatDStatale = value
        End Set
    End Property
    Public Property ContoCorrente() As String
        Get
            Return _ContoCorrente
        End Get
        Set(ByVal value As String)
            _ContoCorrente = value
        End Set
    End Property
    Public Property ComuneUbicazioneImmobile() As String
        Get
            Return _ComuneUbicazioneImmobile
        End Get
        Set(ByVal value As String)
            _ComuneUbicazioneImmobile = value
        End Set
    End Property
    Public Property ComuneIntestatario() As String
        Get
            Return _ComuneIntestatario
        End Get
        Set(ByVal value As String)
            _ComuneIntestatario = value
        End Set
    End Property
    Public Property Bonificato() As Boolean
        Get
            Return _Bonificato
        End Get
        Set(ByVal value As Boolean)
            _Bonificato = value
        End Set
    End Property
    Public Property DataInizioValidità() As DateTime
        Get
            Return _DataInizioValidità
        End Get
        Set(ByVal value As DateTime)
            _DataInizioValidità = value
        End Set
    End Property
    Public Property DataFineValidità() As DateTime
        Get
            Return _DataFineValidità
        End Get
        Set(ByVal value As DateTime)
            _DataFineValidità = value
        End Set
    End Property
    Public Property Operatore() As String
        Get
            Return _Operatore
        End Get
        Set(ByVal value As String)
            _Operatore = value
        End Set
    End Property
    Public Property Annullato() As Boolean
        Get
            Return _Annullato
        End Get
        Set(ByVal value As Boolean)
            _Annullato = value
        End Set
    End Property
    Public Property ImportoSoprattassa() As Double
        Get
            Return _ImportoSoprattassa
        End Get
        Set(ByVal value As Double)
            _ImportoSoprattassa = value
        End Set
    End Property
    Public Property ImportoPenaPecuniaria() As Double
        Get
            Return _ImportoPenaPecuniaria
        End Get
        Set(ByVal value As Double)
            _ImportoPenaPecuniaria = value
        End Set
    End Property
    Public Property Interessi() As Double
        Get
            Return _Interessi
        End Get
        Set(ByVal value As Double)
            _Interessi = value
        End Set
    End Property
    Public Property Violazione() As Boolean
        Get
            Return _Violazione
        End Get
        Set(ByVal value As Boolean)
            _Violazione = value
        End Set
    End Property
    Public Property IDProvenienza() As Integer
        Get
            Return _IDProvenienza
        End Get
        Set(ByVal value As Integer)
            _IDProvenienza = value
        End Set
    End Property
    Public Property NumeroAttoAccertamento() As String
        Get
            Return _NumeroAttoAccertamento
        End Get
        Set(ByVal value As String)
            _NumeroAttoAccertamento = value
        End Set
    End Property
    Public Property DataProvvedimentoViolazione() As DateTime
        Get
            Return _DataProvvedimentoViolazione
        End Get
        Set(ByVal value As DateTime)
            _DataProvvedimentoViolazione = value
        End Set
    End Property
    Public Property ImportoPagatoArrotondamento() As Double
        Get
            Return _ImportoPagatoArrotondamento
        End Get
        Set(ByVal value As Double)
            _ImportoPagatoArrotondamento = value
        End Set
    End Property
    Public Property DataRiversamento() As DateTime
        Get
            Return _DataRiversamento
        End Get
        Set(ByVal value As DateTime)
            _DataRiversamento = value
        End Set
    End Property
    Public Property FlagFabbricatiExRurali() As Boolean
        Get
            Return _FlagFabbricatiExRurali
        End Get
        Set(ByVal value As Boolean)
            _FlagFabbricatiExRurali = value
        End Set
    End Property
    Public Property NumeroProvvedimentoViolazione() As String
        Get
            Return _NumeroProvvedimentoViolazione
        End Get
        Set(ByVal value As String)
            _NumeroProvvedimentoViolazione = value
        End Set
    End Property
    Public Property ImportoImposta() As Double
        Get
            Return _ImportoImposta
        End Get
        Set(ByVal value As Double)
            _ImportoImposta = value
        End Set
    End Property
    Public Property Note() As String
        Get
            Return _Note
        End Get
        Set(ByVal value As String)
            _Note = value
        End Set
    End Property
    Public Property DetrazioneStatale() As Double
        Get
            Return _DetrazioneStatale
        End Get
        Set(ByVal value As Double)
            _DetrazioneStatale = value
        End Set
    End Property
    Public Property Provenienza() As String
        Get
            Return _Provenienza
        End Get
        Set(ByVal value As String)
            _Provenienza = value
        End Set
    End Property
End Class
''' <summary>
''' Definizione oggetto riepilogo importi atto
''' </summary>
<Serializable> Public Class ObjRiepilogoImpAtto
    Dim _COD_CONTRIBUENTE As Integer = 0
    Dim _ANNO As String = "0"
    Dim _DIFFERENZA_IMPOSTA_ACCONTO As Double = 0
    Dim _DIFFERENZA_IMPOSTA_SALDO As Double = 0
    Dim _DIFFERENZA_IMPOSTA_TOTALE As Double = 0
    Dim _IMPORTO_SANZIONI As Double = 0
    Dim _IMPORTO_SANZIONI_RIDOTTO As Double = 0
    Dim _IMPORTO_INTERESSI As Double = 0
    Dim _TIPO_PROVVEDIMENTO As String = "0"
    Dim _IMPORTO_TOTALE_DICHIARATO As Double = 0
    Dim _IMPORTO_TOTALE_VERSATO As Double = 0
    Dim _FASE_RIFERIMENTO As Integer = 0
    Dim _FASE1 As Boolean = 0
    Dim _FASE2 As Boolean = 0
    Dim _FASE3 As Boolean = 0
    Dim _FLAG_VERSAMENTO_TARDIVO As Boolean = 0
    Dim _RAFF_CATASTO_DICHIARATO As String = 0
    Dim _IMPORTO_SPESE As Double = 0
    Dim _LOG_FASE1 As String = 0
    Dim _LOG_FASE2 As String = 0
    Dim _LOG_FASE3 As String = 0
    Dim _LOG_FASE1_ANOMALIE As Boolean = 0
    Dim _LOG_FASE2_ANOMALIE As Boolean = 0
    Dim _LOG_FASE3_ANOMALIE As Boolean = 0
    Dim _FLAG_ATTO_POTENZIALE As Boolean = 0
    Dim _QUOTARIDUZIONESANZIONI As Integer = 0
    Public Property CodContribuente() As Integer
        Get
            Return _COD_CONTRIBUENTE
        End Get
        Set(ByVal value As Integer)
            _COD_CONTRIBUENTE = value
        End Set
    End Property
    Public Property Anno() As Integer
        Get
            Return _ANNO
        End Get
        Set(ByVal value As Integer)
            _ANNO = value
        End Set
    End Property
    Public Property DifferenzaImpostaAcconto() As Integer
        Get
            Return _DIFFERENZA_IMPOSTA_ACCONTO
        End Get
        Set(ByVal value As Integer)
            _DIFFERENZA_IMPOSTA_ACCONTO = value
        End Set
    End Property
    Public Property DifferenzaImpostaSaldo() As Integer
        Get
            Return _DIFFERENZA_IMPOSTA_SALDO
        End Get
        Set(ByVal value As Integer)
            _DIFFERENZA_IMPOSTA_SALDO = value
        End Set
    End Property
    Public Property DifferenzaImpostaTotale() As Integer
        Get
            Return _DIFFERENZA_IMPOSTA_TOTALE
        End Get
        Set(ByVal value As Integer)
            _DIFFERENZA_IMPOSTA_TOTALE = value
        End Set
    End Property
    Public Property ImportoSanzioni() As Integer
        Get
            Return _IMPORTO_SANZIONI
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_SANZIONI = value
        End Set
    End Property
    Public Property ImportoSanzioniRidotto() As Integer
        Get
            Return _IMPORTO_SANZIONI_RIDOTTO
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_SANZIONI_RIDOTTO = value
        End Set
    End Property
    Public Property ImportoInteressi() As Integer
        Get
            Return _IMPORTO_INTERESSI
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_INTERESSI = value
        End Set
    End Property
    Public Property TipoProvvedimento() As Integer
        Get
            Return _TIPO_PROVVEDIMENTO
        End Get
        Set(ByVal value As Integer)
            _TIPO_PROVVEDIMENTO = value
        End Set
    End Property
    Public Property ImportoTotaleDichiarato() As Integer
        Get
            Return _IMPORTO_TOTALE_DICHIARATO
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_TOTALE_DICHIARATO = value
        End Set
    End Property
    Public Property ImportoTotaleVersato() As Integer
        Get
            Return _IMPORTO_TOTALE_VERSATO
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_TOTALE_VERSATO = value
        End Set
    End Property
    Public Property FaseRiferimento() As Integer
        Get
            Return _FASE_RIFERIMENTO
        End Get
        Set(ByVal value As Integer)
            _FASE_RIFERIMENTO = value
        End Set
    End Property
    Public Property Fase1() As Integer
        Get
            Return _FASE1
        End Get
        Set(ByVal value As Integer)
            _FASE1 = value
        End Set
    End Property
    Public Property Fase2() As Integer
        Get
            Return _FASE2
        End Get
        Set(ByVal value As Integer)
            _FASE2 = value
        End Set
    End Property
    Public Property Fase3() As Integer
        Get
            Return _FASE3
        End Get
        Set(ByVal value As Integer)
            _FASE3 = value
        End Set
    End Property
    Public Property FlagVersamentoTardivo() As Integer
        Get
            Return _FLAG_VERSAMENTO_TARDIVO
        End Get
        Set(ByVal value As Integer)
            _FLAG_VERSAMENTO_TARDIVO = value
        End Set
    End Property
    Public Property RaffCatastoDichiarato() As Integer
        Get
            Return _RAFF_CATASTO_DICHIARATO
        End Get
        Set(ByVal value As Integer)
            _RAFF_CATASTO_DICHIARATO = value
        End Set
    End Property
    Public Property ImportoSpese() As Integer
        Get
            Return _IMPORTO_SPESE
        End Get
        Set(ByVal value As Integer)
            _IMPORTO_SPESE = value
        End Set
    End Property
    Public Property LogFase1() As Integer
        Get
            Return _LOG_FASE1
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE1 = value
        End Set
    End Property
    Public Property LogFase2() As Integer
        Get
            Return _LOG_FASE2
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE2 = value
        End Set
    End Property
    Public Property LogFase3() As Integer
        Get
            Return _LOG_FASE3
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE3 = value
        End Set
    End Property
    Public Property LogFase1Anomalie() As Integer
        Get
            Return _LOG_FASE1_ANOMALIE
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE1_ANOMALIE = value
        End Set
    End Property
    Public Property LogFase2Anomalie() As Integer
        Get
            Return _LOG_FASE2_ANOMALIE
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE2_ANOMALIE = value
        End Set
    End Property
    Public Property LogFase3Anomalie() As Integer
        Get
            Return _LOG_FASE3_ANOMALIE
        End Get
        Set(ByVal value As Integer)
            _LOG_FASE3_ANOMALIE = value
        End Set
    End Property
    Public Property FlagAttoPotenziale() As Integer
        Get
            Return _FLAG_ATTO_POTENZIALE
        End Get
        Set(ByVal value As Integer)
            _FLAG_ATTO_POTENZIALE = value
        End Set
    End Property
    Public Property Quotariduzionesanzioni() As Integer
        Get
            Return _QUOTARIDUZIONESANZIONI
        End Get
        Set(ByVal value As Integer)
            _QUOTARIDUZIONESANZIONI = value
        End Set
    End Property
End Class
#Region "ProvvedimentiICI"
''' <summary>
''' Definizione oggetto atto di accertamento IMU/TASI
''' </summary>
<Serializable()>
Public Class OggettoAttoICI
    Inherits OggettoAtto
    Dim _ListUIDich() As objUIICIAccert
    Dim _ListUIAcc() As objUIICIAccert

    Public Property ListUIDich() As objUIICIAccert()
        Get
            Return _ListUIDich
        End Get
        Set(ByVal Value As objUIICIAccert())
            _ListUIDich = Value
        End Set
    End Property
    Public Property ListUIAcc() As objUIICIAccert()
        Get
            Return _ListUIAcc
        End Get
        Set(ByVal Value As objUIICIAccert())
            _ListUIAcc = Value
        End Set
    End Property
End Class
''' <summary>
''' Definizione oggetto immobili accertamento IMU/TASI
''' </summary>
<Serializable()> Public Class objUIICIAccert
    Inherits objSituazioneFinale
    Private _IdSanzioni As String = ""
    Private _DescrSanzioni As String = ""
    Private _CalcolaInteressi As Boolean = True
    Private _Interessi As Double = 0
    Private _Sanzioni As Double = 0
    Private _SanzioniRidotto As Double = 0

    Public Property IdSanzioni As String
        Get
            Return _IdSanzioni
        End Get
        Set(value As String)
            _IdSanzioni = value
        End Set
    End Property
    Public Property DescrSanzioni As String
        Get
            Return _DescrSanzioni
        End Get
        Set(value As String)
            _DescrSanzioni = value
        End Set
    End Property
    Public Property CalcolaInteressi As Boolean
        Get
            Return _CalcolaInteressi
        End Get
        Set(value As Boolean)
            _CalcolaInteressi = value
        End Set
    End Property
    Public Property ImpInteressi As Double
        Get
            Return _Interessi
        End Get
        Set(value As Double)
            _Interessi = value
        End Set
    End Property
    Public Property ImpSanzioni As Double
        Get
            Return _Sanzioni
        End Get
        Set(value As Double)
            _Sanzioni = value
        End Set
    End Property
    Public Property ImpSanzioniRidotto As Double
        Get
            Return _SanzioniRidotto
        End Get
        Set(value As Double)
            _SanzioniRidotto = value
        End Set
    End Property
End Class
#End Region
#Region "ProvvedimentiTARSU"
''' <summary>
''' Definizione oggetto atto di accertamento TARI
''' </summary>
<Serializable()>
Public Class OggettoAttoTARSU
    Inherits OggettoAtto
    Dim _ListUIDich() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo
    Dim _ListUIAcc() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo

    Public Property ListUIDich() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo()
        Get
            Return _ListUIDich
        End Get
        Set(ByVal Value As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo())
            _ListUIDich = Value
        End Set
    End Property
    Public Property ListUIAcc() As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo()
        Get
            Return _ListUIAcc
        End Get
        Set(ByVal Value As RemotingInterfaceMotoreTarsu.MotoreTarsu.Oggetti.OggettoArticoloRuolo())
            _ListUIAcc = Value
        End Set
    End Property
End Class
#End Region
#Region "ProvvedimentiOSAP"
''' <summary>
''' Definizione oggetto atto di accertamento OSAP
''' </summary>
<Serializable()> _
Public Class OggettoAttoOSAP
    Inherits OggettoAtto
    Dim _ListUIDich() As IRemInterfaceOSAP.Articolo
    Dim _ListUIAcc() As IRemInterfaceOSAP.Articolo

    Public Property ListUIDich() As IRemInterfaceOSAP.Articolo()
        Get
            Return _ListUIDich
        End Get
        Set(ByVal Value As IRemInterfaceOSAP.Articolo())
            _ListUIDich = Value
        End Set
    End Property
    Public Property ListUIAcc() As IRemInterfaceOSAP.Articolo()
        Get
            Return _ListUIAcc
        End Get
        Set(ByVal Value As IRemInterfaceOSAP.Articolo())
            _ListUIAcc = Value
        End Set
    End Property
End Class
''' <summary>
''' Definizione oggetto immobili accertamento OSAP
''' </summary>
<Serializable()> _
Public Class OSAPAccertamentoArticolo
	Inherits IRemInterfaceOSAP.Articolo
	Dim _Calcolo As IRemInterfaceOSAP.CalcoloResult
	Dim _IdProvvedimento As Integer = 0
	Dim _Anno As String = ""
	Dim _IdLegame As Integer = 0
	Dim _Progressivo As Integer = 0
	Dim _Sanzioni As String = ""
	Dim _Interessi As String = ""
	Dim _Calcola_Interessi As Boolean = False
	Dim _ImpDiffImposta As Double
	Dim _ImpSanzioni As Double
	Dim _ImpSanzioniRidotto As Double
	Dim _ImpInteressi As Double
	Dim _DescrSanzioni As String

	Public Property Calcolo() As IRemInterfaceOSAP.CalcoloResult
		Get
			Return _Calcolo
		End Get
		Set(ByVal Value As IRemInterfaceOSAP.CalcoloResult)
			_Calcolo = Value
		End Set
	End Property
	Public Property IdProvvedimento() As Integer
		Get
			Return _IdProvvedimento
		End Get
		Set(ByVal Value As Integer)
			_IdProvvedimento = Value
		End Set
	End Property
	Public Property Anno() As String
		Get
			Return _Anno
		End Get
		Set(ByVal Value As String)
			_Anno = Value
		End Set
	End Property
	Public Property IdLegame() As Integer
		Get
			Return _IdLegame
		End Get
		Set(ByVal Value As Integer)
			_IdLegame = Value
		End Set
	End Property
	Public Property Progressivo() As Integer
		Get
			Return _Progressivo
		End Get
		Set(ByVal Value As Integer)
			_Progressivo = Value
		End Set
	End Property
	Public Property Sanzioni() As String
		Get
			Return _Sanzioni
		End Get
		Set(ByVal Value As String)
			_Sanzioni = Value
		End Set
	End Property
	Public Property Interessi() As String
		Get
			Return _Interessi
		End Get
		Set(ByVal Value As String)
			_Interessi = Value
		End Set
	End Property
	Public Property Calcola_Interessi() As Boolean
		Get
			Return _Calcola_Interessi
		End Get
		Set(ByVal Value As Boolean)
			_Calcola_Interessi = Value
		End Set
	End Property
	Public Property ImpDiffImposta() As Double
		Get
			Return _ImpDiffImposta
		End Get
		Set(ByVal Value As Double)
			_ImpDiffImposta = Value
		End Set
	End Property
	Public Property DescrSanzioni() As String
		Get
			Return _DescrSanzioni
		End Get
		Set(ByVal Value As String)
			_DescrSanzioni = Value
		End Set
	End Property
	Public Property ImpSanzioni() As Double
		Get
			Return _ImpSanzioni
		End Get
		Set(ByVal Value As Double)
			_ImpSanzioni = Value
		End Set
	End Property
	Public Property ImpSanzioniRidotto() As Double
		Get
			Return _ImpSanzioniRidotto
		End Get
		Set(ByVal Value As Double)
			_ImpSanzioniRidotto = Value
		End Set
	End Property
	Public Property ImpInteressi() As Double
		Get
			Return _ImpInteressi
		End Get
		Set(ByVal Value As Double)
			_ImpInteressi = Value
		End Set
	End Property
End Class
#End Region
''' <summary>
''' Definizione oggetto rettifica accertamento
''' </summary>
<Serializable()>
Public Class ObjRettifica
    Dim _TIPO_OPERAZIONE_RETTIFICA As Boolean
    Dim _DATA_RETTIFICA As String
    Dim _DATA_ANNULLAMENTO As String
    Dim _ID_PROVVEDIMENTO_OLD As Integer

    Public Property TipoOperazione As Boolean
        Get
            Return _TIPO_OPERAZIONE_RETTIFICA
        End Get
        Set(ByVal value As Boolean)
            _TIPO_OPERAZIONE_RETTIFICA = value
        End Set
    End Property
    Public Property DataRettifica As String
        Get
            Return _DATA_RETTIFICA
        End Get
        Set(ByVal value As String)
            _DATA_RETTIFICA = value
        End Set
    End Property
    Public Property DataAnnullamento As String
        Get
            Return _DATA_ANNULLAMENTO
        End Get
        Set(ByVal value As String)
            _DATA_ANNULLAMENTO = value
        End Set
    End Property
    Public Property IdOld As Integer
        Get
            Return _ID_PROVVEDIMENTO_OLD
        End Get
        Set(ByVal value As Integer)
            _ID_PROVVEDIMENTO_OLD = value
        End Set
    End Property
    Public Sub Reset()
        _TIPO_OPERAZIONE_RETTIFICA = False
        _DATA_RETTIFICA = ""
        _DATA_ANNULLAMENTO = ""
        _ID_PROVVEDIMENTO_OLD = -1
    End Sub
End Class
#Region "Ricerca Atti"
''' <summary>
''' Definizione oggetto ricerca accertamento
''' </summary>
<Serializable()> Public Class ObjSearchAtti
    Dim _IdEnte As String
    Dim _Tributo As String
    Dim _Anno As String
    Dim _TipoProv As String
    Dim _Generazione As ObjSearchAttiAvanzataDate
    Dim _ConfermaAvviso As ObjSearchAttiAvanzataDate
    Dim _StampaAvviso As ObjSearchAttiAvanzataDate
    Dim _ConsegnaAvviso As ObjSearchAttiAvanzataDate
    Dim _NotificaAvviso As ObjSearchAttiAvanzataDate
    Dim _RettificaAvviso As ObjSearchAttiAvanzataDate
    Dim _AnnullamentoAvviso As ObjSearchAttiAvanzataDate
    Dim _SospensioneAutotutela As ObjSearchAttiAvanzataDate
    Dim _Irreperibile As ObjSearchAttiAvanzataDate
    Dim _RicorsoProvinciale As ObjSearchAttiAvanzataDate
    Dim _SospensioneProvinciale As ObjSearchAttiAvanzataDate
    Dim _SentenzaProvinciale As ObjSearchAttiAvanzataDate
    Dim _RicorsoRegionale As ObjSearchAttiAvanzataDate
    Dim _SospensioneRegionale As ObjSearchAttiAvanzataDate
    Dim _SentenzaRegionale As ObjSearchAttiAvanzataDate
    Dim _RicorsoCassazione As ObjSearchAttiAvanzataDate
    Dim _SospensioneCassazione As ObjSearchAttiAvanzataDate
    Dim _SentenzaCassazione As ObjSearchAttiAvanzataDate
    Dim _AttoDefinitivo As ObjSearchAttiAvanzataDate
    Dim _Pagamento As ObjSearchAttiAvanzataDate
    Dim _SollecitoBonario As ObjSearchAttiAvanzataDate
    Dim _RuoloOrdinario As ObjSearchAttiAvanzataDate
    Dim _Coattivo As ObjSearchAttiAvanzataDate
    Public Property IdEnte As String
        Get
            Return _IdEnte
        End Get
        Set(ByVal Value As String)
            _IdEnte = Value
        End Set
    End Property
    Public Property Tributo As String
        Get
            Return _Tributo
        End Get
        Set(ByVal Value As String)
            _Tributo = Value
        End Set
    End Property
    Public Property Anno As String
        Get
            Return _Anno
        End Get
        Set(ByVal Value As String)
            _Anno = Value
        End Set
    End Property
    Public Property TipoProv As String
        Get
            Return _TipoProv
        End Get
        Set(ByVal Value As String)
            _TipoProv = Value
        End Set
    End Property
    Public Property Generazione As ObjSearchAttiAvanzataDate
        Get
            Return _Generazione
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _Generazione = Value
        End Set
    End Property
    Public Property ConfermaAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _ConfermaAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _ConfermaAvviso = Value
        End Set
    End Property
    Public Property StampaAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _StampaAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _StampaAvviso = Value
        End Set
    End Property
    Public Property ConsegnaAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _ConsegnaAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _ConsegnaAvviso = Value
        End Set
    End Property
    Public Property NotificaAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _NotificaAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _NotificaAvviso = Value
        End Set
    End Property
    Public Property RettificaAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _RettificaAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _RettificaAvviso = Value
        End Set
    End Property
    Public Property AnnullamentoAvviso As ObjSearchAttiAvanzataDate
        Get
            Return _AnnullamentoAvviso
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _AnnullamentoAvviso = Value
        End Set
    End Property
    Public Property SospensioneAutotutela As ObjSearchAttiAvanzataDate
        Get
            Return _SospensioneAutotutela
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SospensioneAutotutela = Value
        End Set
    End Property
    Public Property Irreperibile As ObjSearchAttiAvanzataDate
        Get
            Return _Irreperibile
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _Irreperibile = Value
        End Set
    End Property
    Public Property RicorsoProvinciale As ObjSearchAttiAvanzataDate
        Get
            Return _RicorsoProvinciale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _RicorsoProvinciale = Value
        End Set
    End Property
    Public Property SospensioneProvinciale As ObjSearchAttiAvanzataDate
        Get
            Return _SospensioneProvinciale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SospensioneProvinciale = Value
        End Set
    End Property
    Public Property SentenzaProvinciale As ObjSearchAttiAvanzataDate
        Get
            Return _SentenzaProvinciale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SentenzaProvinciale = Value
        End Set
    End Property
    Public Property RicorsoRegionale As ObjSearchAttiAvanzataDate
        Get
            Return _RicorsoRegionale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _RicorsoRegionale = Value
        End Set
    End Property
    Public Property SospensioneRegionale As ObjSearchAttiAvanzataDate
        Get
            Return _SospensioneRegionale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SospensioneRegionale = Value
        End Set
    End Property
    Public Property SentenzaRegionale As ObjSearchAttiAvanzataDate
        Get
            Return _SentenzaRegionale
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SentenzaRegionale = Value
        End Set
    End Property
    Public Property RicorsoCassazione As ObjSearchAttiAvanzataDate
        Get
            Return _RicorsoCassazione
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _RicorsoCassazione = Value
        End Set
    End Property
    Public Property SospensioneCassazione As ObjSearchAttiAvanzataDate
        Get
            Return _SospensioneCassazione
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SospensioneCassazione = Value
        End Set
    End Property
    Public Property SentenzaCassazione As ObjSearchAttiAvanzataDate
        Get
            Return _SentenzaCassazione
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SentenzaCassazione = Value
        End Set
    End Property
    Public Property AttoDefinitivo As ObjSearchAttiAvanzataDate
        Get
            Return _AttoDefinitivo
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _AttoDefinitivo = Value
        End Set
    End Property
    Public Property Pagamento As ObjSearchAttiAvanzataDate
        Get
            Return _Pagamento
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _Pagamento = Value
        End Set
    End Property
    Public Property SollecitoBonario As ObjSearchAttiAvanzataDate
        Get
            Return _SollecitoBonario
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _SollecitoBonario = Value
        End Set
    End Property
    Public Property RuoloOrdinario As ObjSearchAttiAvanzataDate
        Get
            Return _RuoloOrdinario
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _RuoloOrdinario = Value
        End Set
    End Property
    Public Property Coattivo As ObjSearchAttiAvanzataDate
        Get
            Return _Coattivo
        End Get
        Set(ByVal Value As ObjSearchAttiAvanzataDate)
            _Coattivo = Value
        End Set
    End Property

    Sub New()
        IdEnte = ""
        Tributo = ""
        Anno = ""
        TipoProv = ""
        Generazione = New ObjSearchAttiAvanzataDate()
        ConfermaAvviso = New ObjSearchAttiAvanzataDate()
        StampaAvviso = New ObjSearchAttiAvanzataDate()
        ConsegnaAvviso = New ObjSearchAttiAvanzataDate()
        NotificaAvviso = New ObjSearchAttiAvanzataDate()
        RettificaAvviso = New ObjSearchAttiAvanzataDate()
        AnnullamentoAvviso = New ObjSearchAttiAvanzataDate()
        SospensioneAutotutela = New ObjSearchAttiAvanzataDate()
        Irreperibile = New ObjSearchAttiAvanzataDate()
        RicorsoProvinciale = New ObjSearchAttiAvanzataDate()
        SospensioneProvinciale = New ObjSearchAttiAvanzataDate()
        SentenzaProvinciale = New ObjSearchAttiAvanzataDate()
        RicorsoRegionale = New ObjSearchAttiAvanzataDate()
        SospensioneRegionale = New ObjSearchAttiAvanzataDate()
        SentenzaRegionale = New ObjSearchAttiAvanzataDate()
        RicorsoCassazione = New ObjSearchAttiAvanzataDate()
        SospensioneCassazione = New ObjSearchAttiAvanzataDate()
        SentenzaCassazione = New ObjSearchAttiAvanzataDate()
        AttoDefinitivo = New ObjSearchAttiAvanzataDate()
        Pagamento = New ObjSearchAttiAvanzataDate()
        SollecitoBonario = New ObjSearchAttiAvanzataDate()
        RuoloOrdinario = New ObjSearchAttiAvanzataDate()
        Coattivo = New ObjSearchAttiAvanzataDate()
    End Sub
End Class
''' <summary>
''' Definizione oggetto ricerca accertamento avanzata per date
''' </summary>
<Serializable()> Public Class ObjSearchAttiAvanzataDate
    Public Const DateNoSelezionato As Integer = 0
    Public Const DateSelezione As Integer = 1
    Public Const DateNessuna As Integer = 2
    Dim _TipoRic As Integer
    Dim _Dal As Date
    Dim _Al As Date
    Public Property TipoRic As Integer
        Get
            Return _TipoRic
        End Get
        Set(value As Integer)
            _TipoRic = value
        End Set
    End Property
    Public Property Dal As Date
        Get
            Return _Dal
        End Get
        Set(value As Date)
            _Dal = value
        End Set
    End Property
    Public Property Al As Date
        Get
            Return _Al
        End Get
        Set(value As Date)
            _Al = value
        End Set
    End Property
    Sub New()
        TipoRic = DateNoSelezionato
        Dal = Date.MaxValue
        Al = Date.MaxValue
    End Sub
End Class
#End Region