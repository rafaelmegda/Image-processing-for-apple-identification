using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DIPLi;

namespace Limiarizacao
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Abrir o arquivo para selecionar a imagem
        /// </summary>
        /// <returns> Retorna Falso ou Imagem</returns>
        private object Abrir()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecionar Imagem";
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" + "All files (*.*)|*.*";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;
            openFileDialog.FileName = "";

            DialogResult dr = openFileDialog.ShowDialog();
            Imagem image;
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDialog.FileName.ToString();
                image = new Imagem(file);
                return image;
            }
            else
            {
                return false;
            }
        }


        //FILTRAGEM PARA MELHORAR A DEFINIÇÃO DA IMAGEM 
        public Imagem Convolucao(Imagem I, int[,] W)
        {
            Imagem R = new Imagem(I.Largura, I.Altura, TipoImagem.Colorida);

            for (int i = 2; i < I.Altura - 2; i++)
            {
                for (int j = 2; j < I.Largura - 2; j++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        R[i, j, c] = (W[0, 0] * I[i - 2, j - 2, c] + W[0, 1] * I[i - 1, j - 2, c] + W[0, 2] * I[i, j - 2, c] + W[1, 0] * I[i + 1, j - 2, c] + W[1, 1] * I[i + 2, j - 2, c] +
                               W[1, 2] * I[i - 2, j - 1, c] + W[2, 0] * I[i - 1, j - 1, c] + W[2, 1] * I[i, j - 1, c] + W[2, 2] * I[i + 1, j - 1, c]) / 9;
                    }
                }
            }
            return R;
        }

        /// <summary>
        /// BOTÃO NOME
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button2_Click(object sender, EventArgs e)
        {
            Imagem I = (Imagem)Abrir();

            int[,] W = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 1, 1, 1 } };

            int[,] W2 = new int[,] { { -1, -1, -1 },
                                    { -1, 8, -1 },
                                    { -1, -1, -1 } };

            Imagem R = Convolucao(I, W);

            pictureBox1.Image = R.ToBitmap();


            //LIMIARIZAÇÃO DA IMAGEM
            //Método de Limiarização Global
            for (int i = 0; i < R.Altura; i++)
            {
                for (int j = 0; j < R.Largura; j++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        if (R[i, j] <= 245)
                        {
                            R[i, j, c] = 0;
                        }
                        else
                        {
                            R[i, j, c] = 255;
                        }
                    }

                }
            }

            Imagem Y = Convolucao(R, W2);

            for (int i = 0; i < R.Altura; i++)
            {
                for (int j = 0; j < R.Largura; j++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        if (R[i, j, c] == 255)
                        {
                            R[i, j, c] = 0;
                        }
                        else
                        {
                            R[i, j, c] = I[i, j, c];
                        }
                    }
                }
            }
            pictureBox2.Image = R.ToBitmap();
            resultado = R;
            metodo();
        }
        Imagem resultado;

        /// <summary>
        /// Identificar a cor da fruta 0 vermelho e 1 verde
        /// </summary>
        public void metodo()
        {
            int red = 0;
            int green = 0;
            for (int i = 0; i < resultado.Altura; i++)
            {
                for (int j = 0; j < resultado.Largura; j++)
                {
                    if (resultado[i, j, 0] > resultado[i, j, 1])
                    {
                        red++;
                        
                    }
                    else if (resultado[i, j, 1] > resultado[i, j, 0])
                    {
                        green++;
                    }
                }
            }
            if(red > green)
            {
                label1.Text = "Maçã vermelha";
                MessageBox.Show(red.ToString());
                MessageBox.Show(green.ToString());
            }
            else
            {
                label1.Text = "Maçã verde";
                MessageBox.Show(red.ToString());
                MessageBox.Show(green.ToString());
            }
        }
    }
}
