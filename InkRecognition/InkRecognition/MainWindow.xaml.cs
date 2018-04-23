using Microsoft.Ink;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace InkRecognition
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 文字認識処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            using (var ms = new MemoryStream())
            {
                theInkCanvas.Strokes.Save(ms);
                var myInkCollector = new InkCollector();
                var ink = new Ink();
                ink.Load(ms.ToArray());

                using (var context = new RecognizerContext())
                {
                    if (ink.Strokes.Count > 0)
                    {
                        context.Strokes = ink.Strokes;
                        RecognitionStatus status;

                        var result = context.Recognize(out status);

                        if (status == RecognitionStatus.NoError) textBox1.Text = result.TopString;
                        else MessageBox.Show("認識に失敗しました");
                    }
                    else MessageBox.Show("文字が検出されませんでした");
                }
            }
        }

        /// <summary>
        /// リセット処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            theInkCanvas.Strokes.Clear();
            textBox1.Text = null;
        }

        /// <summary>
        /// 文字認識した結果を左枠欄に追加する処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputMemo(object sender, RoutedEventArgs e)
        {
            memo.Text += textBox1.Text + Environment.NewLine;
            theInkCanvas.Strokes.Clear();
            textBox1.Text = null;
        }

        /// <summary>
        /// txtファイル出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            var sa = new SaveFileDialog();
            sa.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "メモ.txt";

            if (sa.ShowDialog() == true)
            {
                string fileName = sa.FileName;
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                try
                {
                    File.WriteAllText(fileName, memo.Text, encoding);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

    }
}
