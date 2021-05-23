using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Drawing.Imaging;
using System.IO;

using ClosedXML.Excel;
using System.Data.OleDb;

using System.Threading;

using System.Net;

using System.Data;
using System.Data.SQLite;

using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

using InstaSharp;

using System.Security;
using InstaSharp;
using InstaSharp.Models;
using InstaSharper.API;
using InstaSharper.Classes;
using InstaSharper.API.Builder;
using InstaSharper.Logger;
using DocumentFormat.OpenXml.Math;
using System.Drawing.Configuration;
using DocumentFormat.OpenXml.Drawing;
using Keys = OpenQA.Selenium.Keys;

using System.Diagnostics;



//using System.Data.OleDB;


namespace WindowsFormsApp1
{

	public partial class menu : Form
	{
		private OleDbConnection _olecon;
		private OleDbCommand _oleCmd;
		private static String _Arquivo = @"C:\numeros.xls";
		private String _StringConexao = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;ReadOnly=False';", _Arquivo);
		private String _Consulta;

		public static string NomeUsuario = Environment.UserName;
		//private static WebDriver driver;
		//private static WebDriverWait wait;

		//driver = new chromeDriver();

		public menu()
		{
			InitializeComponent();
		}
		public static SQLiteConnection sqlite_conn;

		private void FormataGrid()
		{
			Dgbgrid1.Rows.Clear();
			Dgbgrid1.RowHeadersVisible = false;
			Dgbgrid1.ColumnCount = 3;
			Dgbgrid1.Columns[0].Name = "Conta";
			Dgbgrid1.Columns[1].Name = "Senha";
			Dgbgrid1.Columns[1].Width = 120;
			Dgbgrid1.Columns[2].Name = "Status";
			Dgbgrid1.Columns[2].Width = 120;
			//Dgbgrid1.Columns[3].Name = "Ativo";
			//Dgbgrid1.Columns[3].Width = 60;

			DataGridViewCheckBoxColumn chh = new DataGridViewCheckBoxColumn();
			chh.HeaderText = "Ativo";
			chh.Name = "CheckBox";
			Dgbgrid1.Columns.Add(chh);

		}


		public static SQLiteConnection CriarConexao()
		{
			// Create a new database connection:
			if (Directory.Exists(@"C:\Users\" + NomeUsuario+@"\"))
			{
				sqlite_conn = new SQLiteConnection(@"Data Source=C:\Users\" + Environment.UserName + @"\Documents\contas.db; Version = 3; New = True; Compress = True; ");
			}
			else
			{
				sqlite_conn = new SQLiteConnection(@"Data Source=D:\Users\" + Environment.UserName + @"\Documents\contas.db; Version = 3; New = True; Compress = True; ");
			}

			//sqlite_conn.Close();
			return sqlite_conn;
		}
		private void Inicializacao()
		{
			comboBox1.Items.Clear();
			comboBox1.Items.Add("1 - SIM");
			comboBox1.Items.Add("2 - NÃO");

			try
			{
				MultiStatus("Formatando Grid", "i");
				FormataGrid();
				MultiStatus("Grid Formatado","i");
			}
			catch (Exception ex)
			{
				// Get stack trace for the exception with source file information
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				MultiStatus($"ERRO NO MÉTODO CRIAR FORMATAR GRID Linha {line}\n\n{ex}", "d");
			}
			try
			{
				CriarConexao();
				CriarBancoSQLite();
			}
			catch (Exception ex)
			{
				// Get stack trace for the exception with source file information
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				MultiStatus($"ERRO NO MÉTODO CRIAR BANCO DE DADOS Linha {line}\n\n{ex}", "d");
			}


			try
			{
				CriarTabelaInstagram();
				CriarTabela();
				CriarTabelaConfig();
			}
			catch(Exception ex)
			{
				 MultiStatus("Erro ao criar tabela\n\n" + ex, "d");
			}
			try
			{
				MultiStatus("Lendo dados", "i");
				LerDadosInstagram();
				LerDados();
				LerDadosConfig();
			}catch(Exception ex)
			{
				// Get stack trace for the exception with source file information
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				MultiStatus($"Erro ao ler dados do banco Linha {line}\n\n{ex}", "d");
			}
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			// set the current caret position to the end
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			// scroll it automatically
			richTextBox1.ScrollToCaret();
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			Inicializacao();
			//button1_Click(e, e);
		}

		private void Form1_Activated(object sender, EventArgs e)
		{

		}

		public IWebDriver[] drivers = new IWebDriver[20]; // AJUSTAR

		public IWebDriver[] driversF = new IWebDriver[20]; // AJUSTAR

		public static IWebDriver driver1;

		public static IWebDriver driver2;

		public void StartF(int indice, string conta)
		{

			MultiStatus("CRIANDO DRIVER " + conta, "i");
			FirefoxOptions options = new FirefoxOptions();
			FirefoxProfile profile = new FirefoxProfileManager().GetProfile("perfilselenium");
			//FirefoxProfile profile = new FirefoxProfile();

			//options.AddArguments("--headless");
			//options.AddArguments("--window-size=1440, 900");
			//options.EnableMobileEmulation("iPhone 6");
			options.AddArguments("--load-images=no");
			options.AddArguments("--mute-audio");
			options.AddArguments("--disable-infobars");
			options.AddArguments("start-maximized");
			options.AddArgument("--disable-popup-blocking");
			options.AddArgument("--disable-notifications");
			//options.AddArgument("no-sandbox");

			//profile.AddExtension(@"C:\dizu.xpi"); // bloquear perguntas de sair da pagina extensão
			options.Profile = profile;

			var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
			firefoxDriverService.HideCommandPromptWindow = true;

			driversF[indice] = new FirefoxDriver(options);
		}



		public void Start(int indice, string conta)
		{

			MultiStatus("CRIANDO DRIVER " + conta, "i");
			ChromeOptions options = new ChromeOptions();
			//options.AddArguments("--headless");
			//options.AddArguments("--window-size=1440, 900");
			//options.EnableMobileEmulation("iPhone 6");
			options.AddArguments("--load-images=no");
			options.AddArguments("--mute-audio");
			options.AddArguments("--disable-infobars");
			options.AddArguments("start-maximized");
			options.AddArgument("--disable-popup-blocking");
			options.AddArgument("--disable-notifications");
			//options.AddArgument("no-sandbox");
			//options.AddExtensions(@"C:\deixe.crx"); // bloquear perguntas de sair da pagina extensão


			var chromeDriverService = ChromeDriverService.CreateDefaultService();
			chromeDriverService.HideCommandPromptWindow = true;

			drivers[indice] = new ChromeDriver(options);
		}

		public bool SairdoGrupo;
		public bool SalvarContatos;
		public bool EnviarIndividual;
		public bool EnviarGrupo;

		private void IniciarVariaveis()
		{

		}

		private void ErroGoogle(int indice)
		{

			bool v_erro = true;

			while (v_erro == true)
			{
				try
				{
					var Erro = AchaThread(indice).FindElements(By.LinkText("Por que isso aconteceu?"));
					if (Erro.Count == 1)
					{
						Console.Beep(800, 200);
						Console.Beep(700, 200);
						Console.Beep(300, 200);

						MessageBox.Show("VOCÊ PRECISA RESPONDER AO TESTE DO GOOGLE CONTRA ROBÔS, ASSIM QUE RESPONDER E FOR VALIDADO CLIQUE EM OK");
					}
					else
					{
						v_erro = false;
					}
				}
				catch
				{
					v_erro = false;
				}
				Thread.Sleep(10);
			}
		}

		private void LoginGanharInsta(int indice, string  conta)
		{

			MultiStatus("LOGIN NA CONTA GANHENOINSTA " + conta, "i");
			AchaThread(indice).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(20));
			try
			{
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/");
				AchaThread(indice).Navigate().Refresh();

			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/");
			}
			Thread.Sleep(1000);
			AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/");

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("uname")));
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("uname")).SendKeys(textBox1.Text.Trim());
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("pwd")));
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("pwd")).SendKeys(textBox2.Text.Trim());
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("pwd")));
			Thread.Sleep(4000);
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("pwd")).SendKeys("\n\r");
			Thread.Sleep(5000);
		}


		private void LoginDizu(int indice, string conta)
		{

			MultiStatus("LOGIN NA CONTA DIZU " + conta, "i");
			AchaThread(indice).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(20));
			try
			{
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");
				AchaThread(indice).Navigate().Refresh();

			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");
			}
			Thread.Sleep(1000);
			AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("login")));
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("login")).SendKeys(textBox1.Text.Trim());
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("senha")));
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("senha")).SendKeys(textBox2.Text.Trim());
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("senha")));
			Thread.Sleep(4000);
			Thread.Sleep(4000);
			AchaThread(indice).FindElement(By.Id("senha")).SendKeys("\n\r");
			Thread.Sleep(5000);
		}

		private void LoginDizuF(int indice, string conta)
		{

			MultiStatus("LOGIN NA CONTA DIZU " + conta, "i");
			AchaThreadF(indice).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

			WebDriverWait waitForElement = new WebDriverWait(AchaThreadF(indice), TimeSpan.FromSeconds(20));
			try
			{
				AchaThreadF(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");
				AchaThreadF(indice).Navigate().Refresh();

			}
			catch
			{
				AchaThreadF(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");
			}
			Thread.Sleep(1000);
			AchaThreadF(indice).Navigate().GoToUrl("https://dizu.com.br/painel/");
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("login")));
				Thread.Sleep(4000);
				AchaThreadF(indice).FindElement(By.Id("login")).SendKeys(textBox1.Text.Trim());
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("senha")));
				Thread.Sleep(4000);
				AchaThreadF(indice).FindElement(By.Id("senha")).SendKeys(textBox2.Text.Trim());
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("senha")));
				Thread.Sleep(4000);
				Thread.Sleep(4000);
				AchaThreadF(indice).FindElement(By.Id("senha")).SendKeys("\n\r");
			}
			catch
			{

			}
			Thread.Sleep(5000);
			AchaThreadF(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");
			Thread.Sleep(2000);
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("iniciarTarefasExtensao")));
			AchaThreadF(indice).FindElement(By.Id("iniciarTarefasExtensao")).Click();
			MultiStatus($"Iniciando conta {conta} pela extensão", "i");

		}


		private void Seguir(int indice, IJavaScriptExecutor js, WebDriverWait waitForElement, string site)
		{
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("button")));
			AchaThread(indice).FindElement(By.CssSelector("button")).Click();
			var tabs = AchaThread(indice).WindowHandles;
			try
			{
				try
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("_08v79")));//JANELADE BLOQUEIO DO INSTAGRAM
					var erro = AchaThread(indice).FindElement(By.ClassName("_08v79"));
					//#IMPLEMENTAR FECHAMENTO DE DRIVER CASO ENCONTRE ERRO
				}
				catch
				{

				}
				if (checkBox2.Checked == true && checkBox1.Checked == true)
				{
					AchaThread(indice).SwitchTo().Window(tabs[2]);
				}
				else
				{
					AchaThread(indice).SwitchTo().Window(tabs[1]);

				}
				Thread.Sleep(1000);
				AchaThread(indice).Close();
				Thread.Sleep(1000);
				AchaThread(indice).SwitchTo().Window(tabs[0]);
				Thread.Sleep(1000);
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("btn_iniciar")));

				js.ExecuteScript("window.scrollBy(0,-100)");
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("btn-danger")));
				AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
				MultiStatus("PAUSA " + pausa * 1000 + "h - NA CONTA " + indice, "i");
				toolStripStatusLabelLOG.Text = "PAUSA " + pausa * 1000 + "h";
				Thread.Sleep(pausa * 1000);
				pausa += 10;
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));
				AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
			}
			catch
			{

				//Thread.Sleep(5000);
				AchaThread(indice).SwitchTo().Window(tabs[0]);
				//Thread.Sleep(1000);
				if (site == "R")
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn-confirmar")));
					AchaThread(indice).FindElement(By.Id("btn-confirmar")).Click();
				}
				else if (site == "D")
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_5")));
					AchaThread(indice).FindElement(By.Id("conectar_step_5")).Click();
				}
				//Thread.Sleep(2000);
				//AchaThread(indice).FindElement(By.Id("btn_pausar")).Click();
				if (pausa >= 60) { pausa = pausa - 10; }

			}
		}

		private void Curtir(int indice, IJavaScriptExecutor js, WebDriverWait waitForElement, string site)
		{
			var tabs = AchaThread(indice).WindowHandles;
			Thread.Sleep(1000);
			js.ExecuteScript("window.scrollBy(0,100)");
			Thread.Sleep(1000);
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("fr66n")));
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("fr66n")));
			AchaThread(indice).FindElement(By.ClassName("fr66n")).Click();
			Thread.Sleep(1000);
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("_08v79")));
				var erro = AchaThread(indice).FindElement(By.ClassName("_08v79"));

				//Thread.Sleep(1000);
				if (checkBox2.Checked == true && checkBox1.Checked == true)
				{
					AchaThread(indice).SwitchTo().Window(tabs[2]);
				}
				else
				{
					AchaThread(indice).SwitchTo().Window(tabs[1]);

				}
				//Thread.Sleep(1000);
				AchaThread(indice).Close();
				//Thread.Sleep(1000);
				AchaThread(indice).SwitchTo().Window(tabs[0]);
				//Thread.Sleep(1000);
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("btn_iniciar")));

				js.ExecuteScript("window.scrollBy(0,-100)");
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("btn-danger")));
				AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
				toolStripStatusLabelLOG.Text = "PAUSA " + pausa * 1000 + "h";
				//Thread.Sleep(pausa * 1000);
				pausa += 10;
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));
				AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
			}
			catch
			{
				//Thread.Sleep(5000);
				AchaThread(indice).SwitchTo().Window(tabs[0]);
				//Thread.Sleep(1000);
				if (site == "R")
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn-confirmar")));
					AchaThread(indice).FindElement(By.Id("btn-confirmar")).Click();
				}
				else if (site == "D")
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_5")));
					AchaThread(indice).FindElement(By.Id("conectar_step_5")).Click();
				}
				//Thread.Sleep(2000);
				//AchaThread(indice).FindElement(By.Id("btn_pausar")).Click();
				if (pausa >= 60) { pausa = pausa - 10; }
			}
		}
		private void Verifica(int indice)
		{
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
			bool verifica = false;
			while (verifica == false)
			{
				try
				{
					waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("countdownrefresh")));
					//driver.FindElement(By.Id("countdownrefresh")).Click();
					Thread.Sleep(5000);
				}
				catch
				{
					verifica = true;
				}
			}
		}

		string tarefa2 = "";

		private void BuscaTarefaNizu(int indice)
		{
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
			waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_2")));
			var tarefa = AchaThread(indice).FindElement(By.Id("conectar_step_2"));
			tarefa2 = tarefa.Text;
			//Thread.Sleep(1000);
		}
		private void BuscaTarefaGanha(int indice)
		{
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));// adicionado pois o tempo acabava e a tarefa ficava invisivel
				AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
				var tarefa = AchaThread(indice).FindElement(By.Id("tarefa"));
				tarefa2 = tarefa.Text;
			}
			catch
			{
				waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("tarefa")));
				var tarefa = AchaThread(indice).FindElement(By.Id("tarefa"));
				tarefa2 = tarefa.Text;
			}
			//Thread.Sleep(1000);
		}

		int pausa = 60;
		private void RealizarAcoesGanhaNoInsta(int indice,string conta)
		{
			var tabs = AchaThread(indice).WindowHandles;
			AchaThread(indice).SwitchTo().Window(tabs[0]);
			MultiStatus("REALIZANDO AÇOES GANHARNOINSTA DA CONTA " + conta, "i");
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(10));
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("contaig")));
			}
			catch
			{

				//AchaThread(indice).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
				Thread.Sleep(1000);
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/?pagina=sistema");
				Thread.Sleep(1000);
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/?pagina=sistema");
			}
			IJavaScriptExecutor js = (IJavaScriptExecutor)AchaThread(indice);

			//VERIFICA LOGIN
			try
			{
				LoginGanharInsta(indice, conta);
			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/?pagina=sistema");
			}
			Thread.Sleep(1000);
			js.ExecuteScript("window.scrollBy(0,-200)");

			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("contaig"))); //COMBO QUE SELECIONA CONTA INSTAGRAM
			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/?pagina=sistema");
				AchaThread(indice).Navigate().Refresh();
				Thread.Sleep(1000);
				js.ExecuteScript("window.scrollBy(0,-200)");
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("contaig")));
			}
			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Id("contaig")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim());
			//js.ExecuteScript("window.scrollBy(0,200)");
			Thread.Sleep(1000);
			//Thread.Sleep(1000);


			for (int i = 0; i <= numericUpDown1.Value; i++)
			{
				try
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));// BOTÃO INCIAR SISTEMA
					AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
					//Thread.Sleep(1000);
				}
				catch
				{

				}
				tabs = AchaThread(indice).WindowHandles;
				try
				{
					bool verifica = false;
					while (verifica == false)
					{
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(1));
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("countdownrefresh")));
						//driver.FindElement(By.Id("countdownrefresh")).Click();
						Thread.Sleep(5000); //ESPERA NO CASO DE HOUVER PROCURA POR TAREFAS
					}
				}
				catch
				{

				}
				//Estamos carregando o sistema, aguarde alguns segundos...
				try
				{
					BuscaTarefaGanha(indice);
				}
				catch
				{
					try
					{
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("div_load")));
						AchaThread(indice).FindElement(By.Id("div_load"));
						AchaThread(indice).Navigate().Refresh();
					}
					catch
					{
						BuscaTarefaGanha(indice);
					}
				}
				waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));

				if (tarefa2 == "Curtir Publicação")
				{
					try
					{
						 MultiStatus("TAREFA CURTIR DA CONTA " + conta, "i");
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
						Console.Beep(450, 200);
						Console.Beep(450, 200);
						Console.Beep(450, 200);
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn-acessar")));
						AchaThread(indice).FindElement(By.Id("btn-acessar")).Click();
						//Thread.Sleep(1000);
						tabs = AchaThread(indice).WindowHandles;
						AchaThread(indice).SwitchTo().Window(tabs[1]);
						AchaThread(indice).Navigate().Refresh();
						Curtir(indice, js, waitForElement,"R");
					}
					catch(Exception ex)
					{
						status("Erro ao curtir com a conta " + conta, "w");
					}
					tabs = AchaThread(indice).WindowHandles;
					if (tabs.Count >= 2)
					{
						AchaThread(indice).SwitchTo().Window(tabs[1]);
						AchaThread(indice).Close();
						AchaThread(indice).SwitchTo().Window(tabs[0]);
					}
				}
				else
				{
					MultiStatus("TAREFA SEGUIR DA CONTA " + conta, "i");
					waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
					try
					{
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn-acessar")));
						AchaThread(indice).FindElement(By.Id("btn-acessar")).Click();
						tabs = AchaThread(indice).WindowHandles;
						if (checkBox1.Checked == true)
						{
							AchaThread(indice).SwitchTo().Window(tabs[2]);
						}
						else
						{
							AchaThread(indice).SwitchTo().Window(tabs[1]);
						}
						Seguir(indice, js, waitForElement,"R");
					}
					catch
					{
						status("Erro ao seguir com a conta " + conta, "w");
					}

					tabs = AchaThread(indice).WindowHandles;
					if (tabs.Count > 2)
					{
						Thread.Sleep(1000);
						AchaThread(indice).SwitchTo().Window(tabs[2]);
						Thread.Sleep(1000);
						AchaThread(indice).Close();
						Thread.Sleep(1000);
						AchaThread(indice).SwitchTo().Window(tabs[0]);
						Thread.Sleep(1000);
					}
				}
				try
				{
					AchaThread(indice).FindElement(By.Id("btn_pausar")).Click();
				}
				catch(Exception ex)
				{
					MultiStatus($"Conta {conta} não conseguiu pausar","i");
				}
				if (numericUpDown2.Value > 0)
				{
					int tempo = Convert.ToInt32(numericUpDown2.Value * 1000);
					toolStripStatusLabelLOG.Text = "Pausa de " + tempo*indice / 1000 + " Segundos";
					//Refresh();
					Thread.Sleep(tempo*indice);

					toolStripStatusLabelLOG.Text = "Trabalhando...";

				}
				js.ExecuteScript("window.scrollBy(0,-200)");
				waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(30));
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("btn_iniciar")));
				AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
				if (checkBox1.Checked == true)
				{
					RealizarAcoesDizu(indice, conta);
				}
				VerificaBloq(indice, conta);
			}
			AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
			MessageBox.Show("FIM");
		}
		private void RealizarAcoesDizu(int indice, string conta)
		{
			var tabs = AchaThread(indice).WindowHandles;
			if (checkBox2.Checked == true) AchaThread(indice).SwitchTo().Window(tabs[1]);
			MultiStatus("REALIZANDO AÇÕES DIZU DA CONTA " + conta, "i");
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(20));
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("instagram_id")));
			}
			catch
			{
				Thread.Sleep(1000);

				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");
				Thread.Sleep(1000);
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");

			}
			IJavaScriptExecutor js = (IJavaScriptExecutor)AchaThread(indice);

			//AchaThread(indice).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

			try
			{
				LoginDizu(indice, conta);
			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");
			}
			Thread.Sleep(1000);
			js.ExecuteScript("window.scrollBy(0,-200)");

			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("instagram_id")));
			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");
				AchaThread(indice).Navigate().Refresh();
				Thread.Sleep(1000);
				js.ExecuteScript("window.scrollBy(0,-200)");
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("instagram_id")));
			}
			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Id("instagram_id")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim());
			//js.ExecuteScript("window.scrollBy(0,200)");
			Thread.Sleep(1000);
			//Thread.Sleep(1000);


			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("curtida05")));
				AchaThread(indice).FindElement(By.Id("curtida05")).Click();
			}
			catch
			{
				AchaThread(indice).Navigate().GoToUrl("https://dizu.com.br/painel/conectar");
				AchaThread(indice).Navigate().Refresh();
				Thread.Sleep(1000);
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("curtida05")));
				AchaThread(indice).FindElement(By.Id("curtida05")).Click();
			}

			for (int i = 0; i <= numericUpDown1.Value; i++)
			{

				try
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("iniciarTarefas")));
					AchaThread(indice).FindElement(By.Id("iniciarTarefas")).Click();
					//Thread.Sleep(1000);
				}
				catch
				{

				}
				tabs = AchaThread(indice).WindowHandles;
				try
				{
					bool verifica = false;
					while (verifica == false)
					{
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(1));
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("countdownrefresh")));
						//driver.FindElement(By.Id("countdownrefresh")).Click();
						Thread.Sleep(5000);
					}
				}
				catch
				{

				}
				//Estamos carregando o sistema, aguarde alguns segundos...
				try
				{
					BuscaTarefaNizu(indice);
				}
				catch
				{
					try
					{

					}
					catch
					{
						BuscaTarefaNizu(indice);
					}
				}
				waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));

				if (tarefa2 == "2. Clique em 'Curtir' na publicação")
				{


					try
					{
						MultiStatus("AÇÃO CURTIR DIZU DA CONTA " + conta + " ENCONTRADA", "i");
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
						Console.Beep(450, 200);
						Console.Beep(450, 200);
						Console.Beep(450, 200);
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_4")));
						AchaThread(indice).FindElement(By.Id("conectar_step_4")).Click();
						tabs = AchaThread(indice).WindowHandles;
						AchaThread(indice).SwitchTo().Window(tabs[2]);
						AchaThread(indice).Navigate().Refresh();

						Curtir(indice, js, waitForElement,"D");
						Thread.Sleep(1000);
						try
						{
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("_08v79")));
							var erro = AchaThread(indice).FindElement(By.ClassName("_08v79"));
							AchaThread(indice).SwitchTo().Window(tabs[2]);
							AchaThread(indice).Close();
							AchaThread(indice).SwitchTo().Window(tabs[1]);
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("btn-danger")));

							js.ExecuteScript("window.scrollBy(0,-100)");
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("btn-danger")));
							AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
							toolStripStatusLabelLOG.Text = "PAUSA " + pausa * 1000 + "h";
							pausa += 10;
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));
							AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
						}
						catch
						{
							AchaThread(indice).SwitchTo().Window(tabs[0]);
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn-confirmar")));
							AchaThread(indice).FindElement(By.Id("btn-confirmar")).Click();
							if (pausa >= 60) { pausa = pausa - 10; }
						}

					}
					catch
					{

					}

					tabs = AchaThread(indice).WindowHandles;
					if (tabs.Count > 2)
					{
						AchaThread(indice).SwitchTo().Window(tabs[1]);
						AchaThread(indice).Close();
						AchaThread(indice).SwitchTo().Window(tabs[0]);
					}
				}
				else if (tarefa2 == "2. Clique em 'Seguir' no perfil da pessoa")
				{
					 MultiStatus("AÇÃO SEGUIR DIZU DA CONTA " + conta + "ENCONTRADA" , "i");
					waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
					try
					{
						waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
						waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_4")));
						AchaThread(indice).FindElement(By.Id("conectar_step_4")).Click();
						tabs = AchaThread(indice).WindowHandles;
						if (checkBox2.Checked == true)
						{
							AchaThread(indice).SwitchTo().Window(tabs[2]);
						}
						else
						{
							AchaThread(indice).SwitchTo().Window(tabs[1]);
						}

						Seguir(indice, js, waitForElement,"D");
						try
						{
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("_08v79")));
							var erro = AchaThread(indice).FindElement(By.ClassName("_08v79"));
							AchaThread(indice).SwitchTo().Window(tabs[2]);
							AchaThread(indice).Close();
							AchaThread(indice).SwitchTo().Window(tabs[1]);
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("btn-danger")));
							js.ExecuteScript("window.scrollBy(0,-100)");
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("btn-danger")));
							AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
							 MultiStatus("PAUSA " + pausa * 1000 + "h - NA CONTA " + indice, "i");
							toolStripStatusLabelLOG.Text = "PAUSA " + pausa * 1000 + "h";
							Thread.Sleep(pausa * 1000);
							pausa += 10;
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btn_iniciar")));
							AchaThread(indice).FindElement(By.Id("btn_iniciar")).Click();
						}
						catch
						{
							AchaThread(indice).SwitchTo().Window(tabs[1]);
							waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("conectar_step_5")));
							AchaThread(indice).FindElement(By.Id("conectar_step_5")).Click();
							if (pausa >= 60) { pausa = pausa - 10; }
						}
					}
					catch
					{

					}
					tabs = AchaThread(indice).WindowHandles;
					if (tabs.Count > 2)
					{
						Thread.Sleep(1000);
						AchaThread(indice).SwitchTo().Window(tabs[2]);
						Thread.Sleep(1000);
						AchaThread(indice).Close();
						Thread.Sleep(1000);
						AchaThread(indice).SwitchTo().Window(tabs[1]);
						Thread.Sleep(1000);
					}
				}
				else
				{
					MessageBox.Show("TAREFA AINDA NÃO TRATADA/CONHECIDA");
					Console.Beep(350, 200);
					Console.Beep(350, 200);
					Console.Beep(350, 200);
				}
				if (numericUpDown2.Value > 0)
				{
					int tempo = Convert.ToInt32(numericUpDown2.Value * 1000);
					toolStripStatusLabelLOG.Text = " " + tempo / 1000 + " Seg";
					//Refresh();
					Thread.Sleep(tempo);

					toolStripStatusLabelLOG.Text = "Trabalhando...";

				}
				if (checkBox2.Checked == true) RealizarAcoesGanhaNoInsta(indice, conta);
				VerificaBloq(indice, conta);
			}
			AchaThread(indice).FindElement(By.ClassName("btn-danger")).Click();
			MessageBox.Show("FIM");
		}

		private void VerificaLogin(int indice, string conta)
		{
			try
			{
				MultiStatus("Verificando Login Instagram " + conta, "i");
				WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(20)); //_0lGlC
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("_0lGlC")));
				BloqContaComprometida(indice, conta);


			}
			catch
			{
				MultiStatus("Login Instagram ✅" + conta, "i");
			}
		}

		private void LoginInstagramF(int indice, string conta)
		{

			//AchaThreadF(indice).Manage().Window.Size = new Size(1440, 900);
			MultiStatus("LOGIN NA CONTA INSTAGRAM " + conta, "i");
			WebDriverWait waitForElement = new WebDriverWait(AchaThreadF(indice), TimeSpan.FromSeconds(20));
			AchaThreadF(indice).Navigate().GoToUrl("https://www.instagram.com");
			int a = 1;

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("input[name='username']")));
			Thread.Sleep(1000);
			AchaThreadF(indice).FindElement(By.CssSelector("input[name='username']")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim());

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("input[name='password']")));
			Thread.Sleep(1000);
			AchaThreadF(indice).FindElement(By.CssSelector("input[name='password']")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[1].Value.ToString().Trim()); //y3zKF
			Thread.Sleep(2000);
			AchaThreadF(indice).FindElement(By.ClassName("y3zKF")).Click();
			Thread.Sleep(2000);
			//AchaThreadF(indice).FindElement(By.CssSelector("input[name='password']")).SendKeys("\n\r");
			Thread.Sleep(5000);
			//VerificaLogin(indice, conta);

			MultiStatus("Login instagram " + conta, "i");
			Thread.Sleep(3000);

		}

		private void LoginInstagram(int indice, string conta)
		{

			//AchaThread(indice).Manage().Window.Size = new Size(1440, 900);
			MultiStatus("LOGIN NA CONTA INSTAGRAM " + conta, "i");
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(20));
			AchaThread(indice).Navigate().GoToUrl("https://www.instagram.com");
			int a = 1;

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("input[name='username']")));
			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.CssSelector("input[name='username']")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim());

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("input[name='password']")));
			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.CssSelector("input[name='password']")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[1].Value.ToString().Trim());

			Thread.Sleep(2000);
			AchaThread(indice).FindElement(By.CssSelector("input[name='password']")).SendKeys("\n\r");

			VerificaLogin(indice, conta);

			MultiStatus("Login instagram " + conta, "i");
			Thread.Sleep(3000);

		}

		private void BloqContaComprometida(int indice, string conta)
		{
			AchaThread(indice).FindElement(By.ClassName("L3NKy")).Click();

			Random rnd = new Random();
			int SenhaAleatoria = rnd.Next(0, 9999);

			MultiStatus("CONTA " + conta + " PEDINDO ALTERAÇÃO DE SENHA", "w");
			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Name("old_password")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[1].Value.ToString().Trim());

			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Name("new_password1")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim() + SenhaAleatoria);

			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Name("new_password2")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim() + SenhaAleatoria);

			Thread.Sleep(1000);
			AchaThread(indice).FindElement(By.Name("new_password2")).SendKeys("\n\r");


			try
			{
				var Entrou = AchaThread(indice).FindElement(By.LinkText(conta));
				MultiStatus("LOGIN OK, DEPOIS DE ALTERADO A SENHA DA CONTA " + conta + " PARA= " + conta + SenhaAleatoria, "i");
			}
			catch
			{
				FecharDriver(indice, conta);
			}
			//LoginInstagram(indice, conta);
			Dgbgrid1.Rows[indice - 1].Cells[1].Value = Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim() + SenhaAleatoria;
			InserirDadosInstagram(Dgbgrid1.Rows[indice-1].Cells[0].Value.ToString(), Dgbgrid1.Rows[indice-1].Cells[1].Value.ToString());

		}

		private void EscolherConta(int indice)
		{
			AchaThread(indice).Navigate().GoToUrl("https://www.ganharnoinsta.com/painel/?pagina=sistema");

			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(5));
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("contaig")));
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("contaig")));
			Thread.Sleep(2000);
			AchaThread(indice).FindElement(By.Id("contaig")).SendKeys(Dgbgrid1.Rows[indice - 1].Cells[0].Value.ToString().Trim());
			//Thread.Sleep(1000);
		}

		public IWebDriver AchaThread(int indice)
		{
			var dr = driver1;
			dr = drivers[indice];
			if (dr != null)
			{
				var idsessao = ((ChromeDriver)dr).SessionId;
			}
			//		dr.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120000);
			return dr;
		}

		public IWebDriver AchaThreadF(int indice)
		{
			var dr = driver2;
			dr = driversF[indice];
			if (dr != null)
			{
				var idsessao = ((FirefoxDriver)dr).SessionId;
			}
			//		dr.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120000);
			return dr;
		}

		private void FecharDriver(int indice, string conta)
		{
			AchaThread(indice).Quit();
			MultiStatus("FECHANDO OPERAÇÃO DA CONTA " + conta, "d");
			AchaThread(indice).Dispose();
		}

		private void VerificaBloq(int indice, string conta)
		{
			Random rnd = new Random();
			int contaSelecionada = rnd.Next(0, QtContas);
			string ContaSelecionada = Dgbgrid1.Rows[contaSelecionada].Cells[0].Value.ToString().Trim();

			((IJavaScriptExecutor)AchaThread(indice)).ExecuteScript("window.open();");
			var tabs = AchaThread(indice).WindowHandles;
			AchaThread(indice).SwitchTo().Window(tabs[1]);
			MultiStatus("VERIFICANDO BLOQUEIO TEMPORARIO " + conta, "i");
			WebDriverWait waitForElement = new WebDriverWait(AchaThread(indice), TimeSpan.FromSeconds(30));
			AchaThread(indice).Navigate().GoToUrl("https://www.instagram.com/"+ContaSelecionada);

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div/section/main/div/header/section/ul/li/span/span")));
			string posts = AchaThread(indice).FindElement(By.XPath("/html/body/div/section/main/div/header/section/ul/li/span/span")).Text;
			int qtpost = Convert.ToInt32(posts)-1;

			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div/section/main/div/header/section/ul/li/span/span")));
			waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div/section/main/div/header/section/ul/li/span/span")));
			Thread.Sleep(1000);
			try
			{
				var pic = AchaThread(indice).FindElement(By.ClassName("_9AhH0"));
				pic.Click();

				Thread.Sleep(1000);
			}
			catch
			{
				Thread.Sleep(1000);
				var pic = AchaThread(indice).FindElement(By.ClassName("_9AhH0"));
				pic.Click();

				Thread.Sleep(1000);
			}
			//var like = AchaThread(indice).FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/div[2]/section[1]/span[1]/button"));
			//like.Click();

			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div[5]/div[1]/div/article/div/div/div/div/a[1]")));
				var nextpic = AchaThread(indice).FindElement(By.XPath("/html/body/div[5]/div[1]/div/article/div/div/div/div/a[1]"));
				nextpic.Click();
			}
			catch
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div[5]/div[1]/div/div/a[1]")));
				var nextpic = AchaThread(indice).FindElement(By.XPath("/html/body/div[5]/div[1]/div/div/a[1]"));
				nextpic.Click();
			}


			//Thread.Sleep(2000);


			int fotoSelecionada = rnd.Next(1, qtpost-1);

			for (int i =1; i <= (fotoSelecionada); i++)
			{
				//like = AchaThread(indice).FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/div[2]/section[1]/span[1]/button"));
				Thread.Sleep(1000);
				//like.Click();
				//Thread.Sleep(2000);

				try
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div[5]/div[1]/div/article/div/div/div/div/a[2]")));
					var nextpic = AchaThread(indice).FindElement(By.XPath("/html/body/div[5]/div[1]/div/article/div/div/div/div/a[2]"));
					nextpic.Click();
				}
				catch
				{
					waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("/html/body/div[5]/div[1]/div/div/a[2]")));
					var nextpic = AchaThread(indice).FindElement(By.XPath("/html/body/div[5]/div[1]/div/div/a[2]"));
					nextpic.Click();
				}

				Thread.Sleep(1000);
			}
			//nextpic.Click();
			try
			{
				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("Ypffh")));// BOTÃO INCIAR SISTEMA
				AchaThread(indice).FindElement(By.ClassName("Ypffh")).Click();                                                                                                   //AchaThread(indice).FindElement(By.ClassName("mt3GC")).Click();
				AchaThread(indice).FindElement(By.ClassName("Ypffh")).SendKeys("Nice !");
				AchaThread(indice).FindElement(By.ClassName("Ypffh")).SendKeys(Keys.Enter);

				waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("mt3GC")));

				MultiStatus("CONTA " + conta + " TEMPORÁRIAMENTE BLOQUEADA", "d");
				FecharDriver(indice, conta);
			}
			catch
			{
				MultiStatus("CONTA " + conta + " NÃO ESTA TEMPORÁRIAMENTE BLOQUEADA", "i");
				AchaThread(indice).Close();
				AchaThread(indice).SwitchTo().Window(tabs[0]);
			}

		}

		public void Inicia(int indice, string conta)
		{
			Thread.Sleep((indice * 1000) * indice);
			Start(indice, conta);
			LoginInstagram(indice, conta);
			if (((ChromeDriver)AchaThread(indice)).SessionId != null)
				VerificaBloq(indice, conta);
			if (((ChromeDriver)AchaThread(indice)).SessionId != null)
			{
				if (checkBox1.Checked == true)
				{
					LoginDizu(indice, conta);
					if(checkBox2.Checked == true)
						((IJavaScriptExecutor)AchaThread(indice)).ExecuteScript("window.open();");
					RealizarAcoesDizu(indice, conta);
				}

				Thread.Sleep(1000);
				if (checkBox2.Checked == true)
				{
					LoginGanharInsta(indice, conta);
					Thread.Sleep(1000);
					RealizarAcoesGanhaNoInsta(indice, conta);
				}
			}
		}

		int idthread;

		public delegate void Dstatus(string text, string text2);

		public void MultiStatus(string texto, string cor)
		{
			Dstatus dstatus = new Dstatus(status);
			this.richTextBox1.BeginInvoke(dstatus, texto, cor);
			//status(texto, cor);
		}
		public void status(string texto, string cor)
		{
			Color Core = Color.White;
			switch (cor){
				case "i":
					Core = Color.White;
					break;
				case "w":
					Core = Color.Yellow;
					break;
				case "d":
					Core = Color.Red;
					break;
			}


			try
			{
				int ini = richTextBox1.TextLength;
				toolStripStatusLabelLOG.Text = texto;
				richTextBox1.Text += "" + texto + "\n";
				toolStripStatusLabelLOG.Text = "";
				int fin = richTextBox1.TextLength;
				richTextBox1.Select(ini, fin);
				richTextBox1.SelectionColor = Core;
				richTextBox1.Refresh();

				if (!Directory.Exists(@"C:\Log"))
				{
					Directory.CreateDirectory(@"C:\Log");
				}


				string[] start = { DateTime.Now + " " + cor + " " + texto + "\n" };
				File.AppendAllLines(@"C:\Log\Log.txt", start);
			}
			catch
			{

			}
		}
		Thread[] threads = new Thread[50];
		List<int> QtDrivers = new List<int>();

		int QtContas = 0;
		private void button1_Click(object sender, EventArgs e)
		{
			MultiStatus("INICIANDO . . .", "i");
			int x = 0;
			for (int i = 0; i <= Dgbgrid1.RowCount - 2; i++)
			{
				QtContas++;

				if (Dgbgrid1.Rows[i].Cells[0].Value != null && Dgbgrid1.Rows[i].Cells[1].Value != null && Dgbgrid1.Rows[i].Cells[1].Value.ToString() != "" && Dgbgrid1.Rows[i].Cells[3].Value is true)
				{
					x = i+1;
					threads[x] = new Thread(() => Inicia(x, Dgbgrid1.Rows[i].Cells[0].Value.ToString()));
					threads[x].Start();
					QtDrivers.Add(x);
					//idthread = Thread.CurrentThread.ManagedThreadId;
					Thread.Sleep(1000); // VERIFICAR UM SLEEP PARA O THEAD EM USO
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			/*
			Start(0);
			LoginInstagram(0);*/
			/*AchaThread(indice).Navigate().GoToUrl("https://www.instagram.com/p/CBwHDOsn26f/");

			AchaThread(indice).FindElement(By.ClassName("fr66n")).Click();*/
			int a = 1;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				TestaContaInstagram(textBox4.Text, textBox3.Text);
				var result = Task.Run(Login).GetAwaiter().GetResult();
				if (result)
				{
					Dgbgrid1.Rows.Add(textBox4.Text, textBox3.Text,"SENHA VALIDADA");
				}
				else
				{
					//Dgbgrid1.Rows.Add(textBox4.Text, textBox3.Text, "SENHA/USUARIO INCORR.");
					 MultiStatus($"Usuário: {textBox4.Text} Senha: {textBox3.Text} SENHA/USER INCORR.", "d");

				}
			}
			catch (Exception ex)
			{
				// Get stack trace for the exception with source file information
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				MultiStatus($"Erro, linha {line}", "d");
			}
		}

		private static UserSessionData user;
		private static IInstaApi api;
		private InstaLoginResult result;
		private void TestaContaInstagram(string usuario, string senha)
		{
			user = new UserSessionData();
			user.UserName = usuario.ToString();
			user.Password = senha.ToString();
		}
		public static async Task<bool> Login()
		{
			api = InstaApiBuilder.CreateBuilder()
				.SetUser(user)
				.UseLogger(new DebugLogger(InstaSharper.Logger.LogLevel.Exceptions))
				//.SetRequestDelay(TimeSpan.FromSeconds(8))
				.Build();

			var loginRequest = await api.LoginAsync();
			if (loginRequest.Succeeded)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private void groupBox4_Enter(object sender, EventArgs e)
		{

		}


		public void CriarBancoSQLite()
		{

			string userName = Environment.UserName;

			if (File.Exists(@"C:\Users\" + userName + @"\Documents\contas.db"))
			{
				 MultiStatus("BANCO DE DADOS OK", "i");
			}
			else
			{
				try
				{
					SQLiteConnection.CreateFile(@"C:\Users\" + userName + @"\Documents\contas.db");
					MultiStatus("BANCO DE DADOS CRIADO COM SUCESSO, VOCÊ PODERAR GRAVAR SUAS CONTAS", "i");
				}catch(Exception ex)
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(ex, true);
					// Get the top stack frame
					var frame = st.GetFrame(0);
					// Get the line number from the stack frame
					var line = frame.GetFileLineNumber();
					MultiStatus($"ERRO AO CRIAR BANCO DE DADOS\nLinha {line} \n\n{ex.Message}", "d");
				}
			}

		}


		public void CriarTabelaInstagram()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteCommand sqlite_cmd;
				string Createsql = "CREATE TABLE IF NOT EXISTS contasInsta (usuario VARCHAR(20), senha VARCHAR(20), situacao INT )";

				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = Createsql;
				sqlite_cmd.ExecuteNonQuery();
				//conn.Close();
			}

		}


		public void CriarTabela()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteCommand sqlite_cmd;
				string Createsql = "CREATE TABLE IF NOT EXISTS contas (usuario VARCHAR(20), senha VARCHAR(20))";

				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = Createsql;
				sqlite_cmd.ExecuteNonQuery();
				//conn.Close();
			}

		}

		public void CriarTabelaConfig()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteCommand sqlite_cmd;
				string Createsql = "CREATE TABLE IF NOT EXISTS Configuracoes (caminhoExecutavel VARCHAR(100), caminhoFotos VARCHAR(100), UploadAutomatico VARCHAR(1))";

				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = Createsql;
				sqlite_cmd.ExecuteNonQuery();
				//conn.Close();
			}

		}

		public void InserirDadosInstagram(string email, string senha)
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "INSERT INTO contasInsta (usuario, senha) VALUES(@usuario, @senha); ";


				sqlite_cmd.Parameters.AddWithValue("@usuario", email);
				sqlite_cmd.Parameters.AddWithValue("@senha", senha);
				sqlite_cmd.ExecuteNonQuery();

				conn.Close();
			}
		}

		public void InserirDadosConfig(string caminhoExecutavel, string caminhoFotos, string uploadAutomatico)
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				//CREATE TABLE IF NOT EXISTS contas (caminhoExecutavel VARCHAR(100), caminhoFotos VARCHAR(100), UploadAutomatico VARCHAR(1))
				conn.Open();
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "INSERT INTO Configuracoes (caminhoExecutavel, caminhoFotos, UploadAutomatico) VALUES(@caminhoExecutavel, @caminhoFotos, @uploadAutomatico); ";


				sqlite_cmd.Parameters.AddWithValue("@caminhoExecutavel", caminhoExecutavel);
				sqlite_cmd.Parameters.AddWithValue("@caminhoFotos", caminhoFotos);
				sqlite_cmd.Parameters.AddWithValue("@uploadAutomatico", uploadAutomatico);
				sqlite_cmd.ExecuteNonQuery();

				conn.Close();
				MultiStatus("Inserido os dados:\nCaminho Executável: " + caminhoExecutavel + "\nCaminho Fotos: " + caminhoFotos + "\nUpload [Ativo=1|Desativado=2]: " + uploadAutomatico, "i");
			}
		}

		public void InserirDados(string email, string senha)
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "INSERT INTO contas (usuario, senha) VALUES(@usuario, @senha); ";

				sqlite_cmd.Parameters.AddWithValue("@usuario", email);
				sqlite_cmd.Parameters.AddWithValue("@senha", senha);
				sqlite_cmd.ExecuteNonQuery();
				conn.Close();
			}
		}

		public void DeletarDados(string email)
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				try
				{
					conn.Open();
					SQLiteCommand sqlite_cmd;
					sqlite_cmd = conn.CreateCommand();
					sqlite_cmd.CommandText = "DELETE FROM contasInsta WHERE usuario = @usuario";

					sqlite_cmd.Parameters.AddWithValue("@usuario", email);
					sqlite_cmd.ExecuteNonQuery();
					conn.Close();
				}
				catch(Exception ex)
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(ex, true);
					// Get the top stack frame
					var frame = st.GetFrame(0);
					// Get the line number from the stack frame
					var line = frame.GetFileLineNumber();
					MultiStatus($"Erro, linha {line}", "d");
				}
			}
		}


		public void DeletarConfig()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				try
				{
					SQLiteCommand sqlite_cmd;
					sqlite_cmd = conn.CreateCommand();
					sqlite_cmd.CommandText = "Delete * FROM Configuracoes";
					sqlite_cmd.ExecuteNonQuery();
				}
				catch(Exception ex)
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(ex, true);
					// Get the top stack frame
					var frame = st.GetFrame(0);
					// Get the line number from the stack frame
					var line = frame.GetFileLineNumber();
					MultiStatus($"Erro, linha {line}", "d");
				}
			}
		}
		public void UploadFotos(string email, string senha, string caminhoFotos, string caminhoExecutavel)
		{
			string DataHoje = DateTime.Now.ToShortDateString().Replace("/", "");
			string Extensao = "";
			if (File.Exists(caminhoFotos + "\\" + email + "\\" + DataHoje +".jpg"))
			{
				Extensao = ".jpg";
			}
			else if (File.Exists(caminhoFotos + "\\" + email + "\\" + DataHoje +".jpeg"))
			{
				Extensao = ".jpeg";
			}
			else if (File.Exists(caminhoFotos + "\\" + email + "\\" + DataHoje + "png")) {
				Extensao = ".png";
			}

			if (Extensao != "")
			{
				MultiStatus("Imagem encontrada para a conta : " + email, "i");
				string strCmdText = email + " " + senha + " " + caminhoFotos;
				System.Diagnostics.Process.Start(caminhoExecutavel, strCmdText);
			}else if(!Directory.Exists(caminhoFotos + "\\" + email))
			{
				MultiStatus("Criando pasta para a conta : " + email, "i");
				string strCmdText = email + " " + senha + " " + caminhoFotos;
				System.Diagnostics.Process.Start(caminhoExecutavel, strCmdText);
			}
			else
			{
				MultiStatus("Nenhuma imagem encontrada para a conta : " + email, "i");
			}

		}

		public void LerDadosInstagram()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteDataReader sqlite_datareader;
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "SELECT * FROM contasInsta";

				sqlite_datareader = sqlite_cmd.ExecuteReader();
				int l = 0;

				while (sqlite_datareader.Read())
				{
					string email = sqlite_datareader.GetString(0);
					string senha = sqlite_datareader.GetString(1);
					//int situacao = sqlite_datareader.GetInt32(2);

					var result = false;
					//if (checkBox3.Checked == true && situacao != 0)
					if (checkBox3.Checked == true)
						{
						TestaContaInstagram(email, senha);
						result = Task.Run(Login).GetAwaiter().GetResult();
					}
					else
					{
						result = true;
					}
					//var result = true;
					if (result)
					{
						Dgbgrid1.Rows.Add(email, senha, "SENHA VALIDADA", true);
						MultiStatus("Usuário: "+email + " SENHA VALIDADA", "i");

					}
					else
					{
						Dgbgrid1.Rows.Add(email, senha, "SENHA/USER INCORR.",false);
						MultiStatus("Usuário: " + email + " SENHA/USER INCORR.", "i");
					}
					l++;
				}
				sqlite_datareader.Close();
				conn.Close();

			}
		}
		public void Dgbgrid1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex == 1 && e.Value != null)
			{
				e.Value = new String('*', e.Value.ToString().Length);
			}
		}


		public void LerDados()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteDataReader sqlite_datareader;
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "SELECT * FROM contas";

				sqlite_datareader = sqlite_cmd.ExecuteReader();
				if (sqlite_datareader.Read())
				{
					string email = sqlite_datareader.GetString(0);
					string senha = sqlite_datareader.GetString(1);
					textBox1.Text = email.Trim();
					textBox2.Text = senha.Trim();
				}
				sqlite_datareader.Close();
				conn.Close();

			}
		}

		public void LerDadosConfig()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				SQLiteDataReader sqlite_datareader;
				SQLiteCommand sqlite_cmd;
				sqlite_cmd = conn.CreateCommand();
				sqlite_cmd.CommandText = "SELECT * FROM Configuracoes";

				sqlite_datareader = sqlite_cmd.ExecuteReader();
				if (sqlite_datareader.Read())
				{
					string caminhoFotos = sqlite_datareader.GetString(0);
					string caminhoExecutavel = sqlite_datareader.GetString(1);
					string uploadAutomatico = sqlite_datareader.GetString(2);
					textBox5.Text = caminhoFotos.Trim();
					textBox6.Text = caminhoExecutavel.Trim();
					if (uploadAutomatico == "1") {
						comboBox1.Text = "1 - SIM";
					}else if(uploadAutomatico == "2")
					{
						comboBox1.Text = "2 - NÃO";
					}
				}
				sqlite_datareader.Close();
				conn.Close();

			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DeletarTodosDados();
			FormataGrid();
			LerDados();
		}

		public void DeletarTodosDadosInstagram()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				try
				{
					conn.Open();
					SQLiteCommand sqlite_cmd;
					sqlite_cmd = conn.CreateCommand();
					sqlite_cmd.CommandText = "DELETE FROM contasInsta";
					sqlite_cmd.ExecuteNonQuery();
					sqlite_cmd.Dispose();
				}
				catch (Exception ex)
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(ex, true);
					// Get the top stack frame
					var frame = st.GetFrame(0);
					// Get the line number from the stack frame
					var line = frame.GetFileLineNumber();
					MultiStatus($"Erro, linha {line}", "d");
				}
				//conn.Close();
			}
		}

		public void DeletarTodosDados()
		{
			using (SQLiteConnection conn = new SQLiteConnection(sqlite_conn))
			{
				conn.Open();
				try
				{

					SQLiteCommand sqlite_cmd;
					sqlite_cmd = conn.CreateCommand();
					sqlite_cmd.CommandText = "DELETE FROM contas";
					sqlite_cmd.ExecuteNonQuery();
					sqlite_cmd.Dispose();
				}
				catch (Exception ex)
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(ex, true);
					// Get the top stack frame
					var frame = st.GetFrame(0);
					// Get the line number from the stack frame
					var line = frame.GetFileLineNumber();
					MultiStatus($"Erro, linha {line}", "d");
				}
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			DeletarTodosDadosInstagram();
			DeletarTodosDados();
			for (int i = 0; i <= Dgbgrid1.RowCount - 1; i++)
			{
				if (Dgbgrid1.Rows[i].Cells[0].Value != null && Dgbgrid1.Rows[i].Cells[1].Value != null)
				{
					MultiStatus("GRAVANDO " + Dgbgrid1.Rows[i].Cells[0].Value.ToString() + " - " + Dgbgrid1.Rows[i].Cells[1].Value.ToString(), "i");
					InserirDadosInstagram(Dgbgrid1.Rows[i].Cells[0].Value.ToString(), Dgbgrid1.Rows[i].Cells[1].Value.ToString());
				}
			}

			InserirDados(textBox1.Text.ToString(), textBox2.Text.ToString());
		}

		private void button6_Click(object sender, EventArgs e)
		{
			DeletarDados(textBox4.Text);
			FormataGrid();
			LerDados();
			LerDadosInstagram();
		}

		private void Dgbgrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		[DllImport("kernel32.dll")]
		static extern uint SuspendThread(IntPtr hThread);
		Thread threadsF;
		private void button7_Click(object sender, EventArgs e)
		{
			threadsF = new Thread(() => FecharTudo());
			threadsF.Start();
		}

		private void FecharTudo()
		{
			for (int i = 1; i <= QtDrivers.Count-1; i++)
			{
				if (AchaThread(i) != null)
				{
					//FecharDriver(QtDrivers[i]);
					//threads[i].Suspend();
					threads[i].Abort();
					//Thread.Sleep(1000); // VERIFICAR UM SLEEP PARA O THEAD EM USO
					MultiStatus("Fechando processo " + i, "i");
				}
				else
				{
					MultiStatus("Processo " + i + " já fechado", "i");
				}
			}
			for (int i = 0; i <= QtDrivers.Count - 1; i++)
			{
				FecharDriver(QtDrivers[i], "");
				MultiStatus("FECHANDO NAVEFADOR " + i, "i");
			}
			threadsF.Abort();
		}

		private void button8_Click(object sender, EventArgs e)
		{
			DeletarConfig();
			InserirDadosConfig(textBox6.Text.Trim(),textBox5.Text.Trim(), comboBox1.Text.Substring(0,1));
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void button9_Click(object sender, EventArgs e)
		{

			for (int i = 0; i <= Dgbgrid1.RowCount - 2; i++)
			{
				string email = Dgbgrid1.Rows[i].Cells[0].Value.ToString().Trim();
				string senha = Dgbgrid1.Rows[i].Cells[1].Value.ToString().Trim();
				if (Dgbgrid1.Rows[i].Cells[0].Value != null && Dgbgrid1.Rows[i].Cells[1].Value != null && Dgbgrid1.Rows[i].Cells[1].Value.ToString() != "" && Dgbgrid1.Rows[i].Cells[3].Value is true)
				{
					UploadFotos(email, senha, textBox6.Text.Trim(), textBox5.Text.Trim());
				}
			}
		}


		Thread[] threadsFi = new Thread[50];
		List<int> QtDriversF = new List<int>();

		private void button10_Click(object sender, EventArgs e)
		{
			MultiStatus("INICIANDO . . .", "i");
			int x = 0;
			for (int i = 0; i <= Dgbgrid1.RowCount - 2; i++)
			{

				if (Dgbgrid1.Rows[i].Cells[0].Value != null && Dgbgrid1.Rows[i].Cells[1].Value != null && Dgbgrid1.Rows[i].Cells[1].Value.ToString() != "" && Dgbgrid1.Rows[i].Cells[3].Value is true)
				{
					x = i + 1;
					threadsFi[x] = new Thread(() => IniciaF(x, Dgbgrid1.Rows[i].Cells[0].Value.ToString()));
					threadsFi[x].Start();
					QtDrivers.Add(x);
					//idthread = Thread.CurrentThread.ManagedThreadId;
					Thread.Sleep(1000); // VERIFICAR UM SLEEP PARA O THEAD EM USO
				}
			}

		}

		private void IniciaF(int indice, string conta)
		{
			StartF(indice, conta);
			LoginInstagramF(indice, conta);
			LoginDizuF(indice, conta);
		}
	}
}