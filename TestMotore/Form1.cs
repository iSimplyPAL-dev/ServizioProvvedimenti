using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using RIBESFrameWork;
using log4net;
using log4net.Config;
using System.Configuration;
using ComPlusInterface;
using System.IO;

namespace TestMotore
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
//		private static readonly ILog Log = LogManager.GetLogger(typeof(Form1));
		private System.Windows.Forms.Button btnCalcoloICI;
		private System.Windows.Forms.TextBox txtAnno;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtCodContribuente;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtEnte;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCalcoloICI = new System.Windows.Forms.Button();
			this.txtAnno = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtCodContribuente = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtEnte = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnCalcoloICI
			// 
			this.btnCalcoloICI.Location = new System.Drawing.Point(8, 112);
			this.btnCalcoloICI.Name = "btnCalcoloICI";
			this.btnCalcoloICI.Size = new System.Drawing.Size(136, 23);
			this.btnCalcoloICI.TabIndex = 3;
			this.btnCalcoloICI.Text = "Calcolo ICI Puntuale";
			this.btnCalcoloICI.Click += new System.EventHandler(this.btnCalcoloICI_Click);
			// 
			// txtAnno
			// 
			this.txtAnno.Location = new System.Drawing.Point(80, 32);
			this.txtAnno.Name = "txtAnno";
			this.txtAnno.Size = new System.Drawing.Size(80, 20);
			this.txtAnno.TabIndex = 1;
			this.txtAnno.Text = "2013";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Anno";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Contrib.";
			// 
			// txtCodContribuente
			// 
			this.txtCodContribuente.Location = new System.Drawing.Point(80, 56);
			this.txtCodContribuente.Name = "txtCodContribuente";
			this.txtCodContribuente.Size = new System.Drawing.Size(80, 20);
			this.txtCodContribuente.TabIndex = 2;
			this.txtCodContribuente.Text = "6827";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Ente";
			// 
			// txtEnte
			// 
			this.txtEnte.Location = new System.Drawing.Point(80, 8);
			this.txtEnte.Name = "txtEnte";
			this.txtEnte.Size = new System.Drawing.Size(80, 20);
			this.txtEnte.TabIndex = 0;
			this.txtEnte.Text = "050027";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtEnte);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtCodContribuente);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtAnno);
			this.Controls.Add(this.btnCalcoloICI);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnCalcoloICI_Click(object sender, System.EventArgs e)
		{
				try 
				{ 
					Hashtable objHashTable = new Hashtable(); 
					CreateSessione objSessione; 
					string strWFErrore=string.Empty; 
					//DataSet objDSAnagrafica; 
					string strConnectionStringOPENgovICI; 
					string strConnectionStringOPENgovProvvedimenti; 
					string strConnectionStringAnagrafica; 
					string strConnectionStringOPENgovTerritorio; 
					string strConnectionStringOPENgovCatasto; 
					bool bVersatoNelDovuto = false;
					bool bCalcolaArrotondamento = false;
				
//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::inizio");
					objSessione = new CreateSessione(ConfigurationSettings.AppSettings["PARAMETROENV"].ToString(), ConfigurationSettings.AppSettings["username"].ToString(), ConfigurationSettings.AppSettings["IDENTIFICATIVOAPPLICAZIONE"].ToString()); 
					if (!(objSessione.CreaSessione(ConfigurationSettings.AppSettings["username"].ToString(), ref strWFErrore ))) 
					{ 
						throw new Exception("Errore durante l'apertura della sessione di WorkFlow"); 
					} 
//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::aperto sessione");
					string strIdSottoAppAnag=ConfigurationSettings.AppSettings["OPENGOVA"];
					objHashTable.Add("IDSOTTOAPPLICAZIONEANAGRAFICA",strIdSottoAppAnag ); 

					string strIdSottoAppTerr=ConfigurationSettings.AppSettings["OPENGOVT"];
					objHashTable.Add("IDSOTTOAPPLICAZIONETERRITORIO", strIdSottoAppTerr); 

					string strIdSottoAppUtilita=ConfigurationSettings.AppSettings["OPENGOVU"];
					objHashTable.Add("IDSOTTOAPPLICAZIONEUTILITA", strIdSottoAppUtilita); 

					//				string strIdSottoAppIci=ConfigurationSettings.AppSettings["OPENGOVI"];
					//				objHashTable.Add("IDSOTTOAPPLICAZIONEICI", strIdSottoAppIci); 

					string strIdSottoAppProvv=ConfigurationSettings.AppSettings["OPENGOVp"];
					objHashTable.Add("IDSOTTOAPPLICAZIONEICI", strIdSottoAppProvv); 

					string strIdSottoAppCatasto=ConfigurationSettings.AppSettings["OPENGOVC"];
					objHashTable.Add("IDSOTTOAPPLICAZIONECATASTO", strIdSottoAppCatasto); 

//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::valorizzato idsottoapplicazioni");
					objHashTable.Add("CodENTE", txtEnte.Text ); 
					strConnectionStringOPENgovICI = objSessione.oSession.oAppDB.GetConnection().ConnectionString ;//objSessione.oSession.GetPrivateDBManager(ref strIdSottoAppIci).GetConnection().ConnectionString; 
					strConnectionStringAnagrafica = objSessione.oSession.GetPrivateDBManager(ref strIdSottoAppAnag).GetConnection().ConnectionString; 
					strConnectionStringOPENgovProvvedimenti = "";//objSessione.oSession.GetPrivateDBManager(ref strIdSottoAppProvv).GetConnection().ConnectionString; //objSessione.oSession.oAppDB.GetConnection().ConnectionString; 
					strConnectionStringOPENgovTerritorio = "";//objSessione.oSession.GetPrivateDBManager(ref strIdSottoAppTerr).GetConnection().ConnectionString; 
					strConnectionStringOPENgovCatasto = "";//objSessione.oSession.GetPrivateDBManager(ref strIdSottoAppCatasto).GetConnection().ConnectionString; 

				
					objHashTable.Add("CONNECTIONSTRINGOPENGOV", System.Configuration.ConfigurationSettings.AppSettings["connectionStringSQLOPENgov"]); 
					objHashTable.Add("CONNECTIONSTRINGOPENGOVICI", strConnectionStringOPENgovICI); 
					objHashTable.Add("CONNECTIONSTRINGOPENGOVPROVVEDIMENTI", strConnectionStringOPENgovProvvedimenti); 
					objHashTable.Add("CONNECTIONSTRINGANAGRAFICA", strConnectionStringAnagrafica); 
					objHashTable.Add("CONNECTIONSTRINGOPENGOVTERRITORIO", strConnectionStringOPENgovTerritorio); 
					objHashTable.Add("CONNECTIONSTRINGOPENGOVCATASTO", strConnectionStringOPENgovCatasto); 
//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::valorizzato stringhe connessione");

					objHashTable.Add("USER", ConfigurationSettings.AppSettings["username"]); 
					objHashTable.Add("COD_TRIBUTO", "8852"); 					

					objHashTable.Add("ANNODA",txtAnno.Text);
					objHashTable.Add("ANNOA","-1");
					objHashTable.Add("PARAMETROENV",ConfigurationSettings.AppSettings["PARAMETROENV"].ToString());

					objHashTable.Add("CODCONTRIBUENTE",txtCodContribuente.Text);
//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::valorizzato dati contribuente");
					//*** 20120530 - IMU ***
					//prima di richiamare il calcolo devo riaggiornare il valo
					//*** ***
					int TipoCalcolo=CalcoloICI.TIPOCalcolo_STANDARD;
//					if (rdbCalcoloNetto.Checked==true)
//					{
//						TipoCalcolo=CalcoloICI.TIPOCalcolo_NETTOVERSATO;
//					}
				
//					Log.Debug("CalcoloICIPuntuale::btnCalcoloICI::attivo il servizio al percorso::"+ConfigurationSettings.AppSettings["URLServiziFreezer"].ToString());
					bool iRetValCalcoloICI;
					IFreezer remObjectFreezer =(IFreezer)Activator.GetObject(typeof(IFreezer), ConfigurationSettings.AppSettings["URLServiziFreezer"].ToString()); 

					bool ConfigDichiarazione= bool.Parse(ConfigurationSettings.AppSettings["CONFIGURAZIONE_DICHIARAZIONE"].ToString());
					iRetValCalcoloICI=remObjectFreezer.SetCalcoloICISync(objHashTable,ConfigDichiarazione, bVersatoNelDovuto,bCalcolaArrotondamento,TipoCalcolo); // bool ribaltamento versato nel dovuto :: bVersatoNelDovuto

					//*** 20120704 - IMU ***
					MessageBox.Show("Calcolo ICI/IMU puntuale" + (iRetValCalcoloICI == true ? "" : " non") + " effettuato con successo.");
				} 
				catch (Exception ex) 
				{ 
				
					//*** 20120704 - IMU ***
					if (ex.Message=="00000")
					{
						MessageBox.Show("Non sono state individuate dichiarazioni per il contribuente selezionato. Impossibile effettuare il calcolo ICI/IMU.");
					}
					else
					{
						MessageBox.Show("Si sono verificati dei problemi durante il calcolo ICI/IMU puntuale." + ex.Message);
					}
				}

	}
	}
}
