using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.FileFormats;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Globalization;
using System.Speech.Synthesis;
using System.Threading;

namespace Catherine
{
	public partial class Form1 : Form
	{
		WaveIn waveIn;
		WaveFileWriter waveFileWriter;

		string output_file = "Sound_FIle.wav";

		public Form1()
		{
			InitializeComponent();
			System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//временная заглушка проверки контрола в каком потоке
		}

		void StopRecord()
		{
			MessageBox.Show("Stop");
			waveIn.StopRecording();
		}

		void waveIn_DataAvailable(object sender, WaveInEventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
			else
				waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
		}

		private void waveIn_RecordingStopped(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
			else
			{
				waveIn.Dispose();
				waveIn = null;
				waveFileWriter.Close();
				waveFileWriter = null;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				//	waveIn = new WaveIn();
				//	waveIn.DeviceNumber = 0;
				//	waveIn.DataAvailable += waveIn_DataAvailable;
				//	waveIn.RecordingStopped += waveIn_RecordingStopped;
				//	waveIn.WaveFormat = new WaveFormat(8000, 1);
				//	waveFileWriter = new WaveFileWriter(output_file, waveIn.WaveFormat);
				//	waveIn.StartRecording();
				//	timer.Interval = 1000; //интервал между срабатываниями 1000 миллисекунд
				//	timer.Tick += new EventHandler(timer_Tick); //подписываемся на события Tick
				//	timer.Start();
				//	textBox1.Text = (++timerCounter).ToString();
				//	if (textBox1.Text.Contains("3"))
				//		Thread.Sleep(8000);
				SaveVoice();
			}
			catch (Exception ex) { MessageBox.Show(ex.Message); }
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (waveIn != null)
				StopRecord();
		}
		private void button3_Click(object sender, EventArgs e)
		{
			SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

			Grammar dictationGrammar = new DictationGrammar();
			recognizer.LoadGrammar(dictationGrammar);
			if (checkBox1.Checked == true)
				recognizer.SetInputToDefaultAudioDevice();
			else
				recognizer.SetInputToWaveFile(@"C:\Users\hardy\source\repos\Catherine\Catherine\bin\Debug\Sound_File.wav");

			RecognitionResult result = recognizer.Recognize();
			recognizer.UnloadAllGrammars();
			string a = String.Format($"({DateTime.Now}) User said: {result.Text}\n", Dialog_box.Text);
			Dialog_box.Text += a;
			///
			/// Будущее сохранение даных в файл
			///
			//FileStream fs = new FileStream(@"C:\Users\hardy\source\repos\Catherine\Catherine\bin\Debug\Test.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
			//byte[] arr = System.Text.Encoding.Default.GetBytes(Dialog_box.Text);
			//fs.Write(arr, 0, arr.Length);
		}

		private void VoiceToText()
		{
			SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

			Grammar dictationGrammar = new DictationGrammar();
			recognizer.LoadGrammar(dictationGrammar);
			if (checkBox1.Checked == true)
				recognizer.SetInputToDefaultAudioDevice();
			else
				recognizer.SetInputToWaveFile(@"C:\Users\hardy\source\repos\Catherine\Catherine\bin\Debug\Sound_File.wav");
			RecognitionResult result = recognizer.Recognize();
			recognizer.UnloadAllGrammars();
			string a = String.Format($"({DateTime.Now}) User said: {result.Text}\n", Dialog_box.Text);
			this.Dialog_box.Text += a;
		}

		private void SaveVoice()
		{
			waveIn = new WaveIn();
			waveIn.DeviceNumber = 0;
			waveIn.DataAvailable += waveIn_DataAvailable;
			waveIn.RecordingStopped += waveIn_RecordingStopped;
			waveIn.WaveFormat = new WaveFormat(8000, 1);
			waveFileWriter = new WaveFileWriter(output_file, waveIn.WaveFormat);
			waveIn.StartRecording();
			//TimerCallback tm = new TimerCallback(Count);
			//System.Threading.Timer timer = new System.Threading.Timer(tm, timerCounter, 0, 2000);
			//timer.Interval = 1000; //интервал между срабатываниями 1000 миллисекунд
			//timer.Tick += new EventHandler(timer_Tick); //подписываемся на события Tick
			//timer.Start();
			if (timer1.Enabled == true)
				timer1.Enabled = false;
			else
				timer1.Enabled = true;
			//if (textBox1.Text.Equals("00:03:000"))
			//{
			//	timer1.Stop();
			//	//Thread.Sleep(6000);
			//	waveIn.StopRecording();
			//	await Task.Run(() => VoiceToText());
			//}
		}

		DateTime date1 = new DateTime(0, 0);
		private async void timer1_Tick_1(object sender, EventArgs e)
		{
			date1 = date1.AddMilliseconds(60);
			textBox1.Text = date1.ToString("mm:ss:fff");
			if (textBox1.Text.Equals("00:03:000"))
			{
				timer1.Stop();
				waveIn.StopRecording();
				await Task.Run(() => VoiceToText());
			}
		}
	}
}
