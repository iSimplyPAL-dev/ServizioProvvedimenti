using System;
using RIBESFrameWork;

namespace TestMotore
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
		public class CreateSessione
		{
		
			string m_Parametro;
			string m_UserName;
			string m_IdentificativoApplicazione;
		
			public RIBESFrameWork.Session oSession;
			public RIBESFrameWork.SessionManager oSM = new RIBESFrameWork.SessionManager();
			public RIBESFrameWork.OperationManager oOM = new RIBESFrameWork.OperationManager();

			/// <summary>
			/// Costruttore della Classe
			/// </summary>
			/// <param name="parametro">Parametro del File Env dalla quale si accede al dataBase di WorkFlow</param>
			/// <param name="username">Utente che ha i diritti di accesso al DataBase Applicativo</param>
			/// <param name="IdentificativoApplicazione">Identigicativo applicazione</param>
			public CreateSessione(string parametro, string username, string IdentificativoApplicazione)
			{
			
				m_Parametro = parametro;
				m_UserName = username;
				m_IdentificativoApplicazione = IdentificativoApplicazione;
			
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="username"></param>
			/// <param name="Errore"></param>
			/// <returns></returns>
			public bool CreaSessione(string username, ref string Errore)
			{
				bool returnValue;
				oSM.sSessionManagerEnvSuffix = m_Parametro;
				oOM.sOperationManagerEnvSuffix = m_Parametro;
			
				if (! oSM.Initialize(ref username, ref m_Parametro))
				{
				
					goto oSMInizialize;
				
				}
			
				oSession = oSM.CreateSession(ref m_IdentificativoApplicazione);
			
				if (oSession == null)
				{
				
					goto ErrorSession;
				
				}
			
				if (! oOM.Initialize(ref oSession))
				{
				
					goto oOMInitialize;
				
				}
			
				returnValue = true;
			
				return returnValue;
			
				oSMInizialize:
			
					returnValue = false;
			
				Errore = oSM.oErr.Description;
			
				return returnValue;
			
				ErrorSession:
			
					returnValue = false;
			
				Errore = oSM.oErr.Description;
			
				return returnValue;
			
				oOMInitialize:
			
					returnValue = false;
			
				Errore = oSession.oErr.Description;
			
				return returnValue;
			
			}

			/// <summary>
			/// Distruttore della classe
			/// </summary>
			public void Kill ()
			{
			
				if (oOM!=null)
				{
					oOM.Terminate();
					oOM=null;
				}
				if (oSession!=null)
				{
					if (oSession.oAppDB!=null)
					{
						oSession.oAppDB.DisposeConnection ();
						oSession.oAppDB.Dispose();
					}
					if (oSession.SecDB!=null)
					{
						oSession.SecDB.DisposeConnection ();
						oSession.SecDB.Dispose();
					}
					oSession.Terminate();
					oSession=null;
				}
				if (oSM!=null)
				{
					oSM.Terminate ();
					oSM=null;
				}
			}
		
		
		
		}

	public class CalcoloICI
	{
		public const int TIPOCalcolo_STANDARD = 0;
		public const int TIPOCalcolo_NETTOVERSATO = 1;
	}
}
