using System;
using System.Windows.Forms;

namespace PDI_Tarea_3 {
	public partial class Form : System.Windows.Forms.Form {
		private ImageManipulation image;

		public Form() {
			InitializeComponent();
			image = new ImageManipulation(histogram, fileName, fileMb, fileDimentions, bitsProfundidad, checkBox1, checkBox2);
			comboBox3.SelectedIndex = 1;
			comboBox1.SelectedIndex = 0;
			button2.Enabled = false;
		}

		private void abrirToolStripMenuItem_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Images files (*.bmp, *.gif, *.jpg, *.png, *.tiff)|*.bmp;*.gif;*.jpg;*.png;*.tiff";
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Multiselect = false;
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				image.loadFile(openFileDialog.FileName);
			}
			pictureBox.Image = image.getActualImage();
		}

		private void flipVertical_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.setFlipVertical();
			pictureBox.Image = image.getActualImage();
		}

		private void flipHorizontal_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.setFlipHorizontal();
			pictureBox.Image = image.getActualImage();
		}

		private void negativo_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.setInvertirColores();
			pictureBox.Image = image.getActualImage();
		}

		private void trackBarBrillo_ValueChanged(object sender, EventArgs e) {
			image.setBrillo(trackBarBrillo.Value);
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "BMP file (*.bmp)|*.bmp|PNG file (*.png)|*.png|JPG file (*.jpg)|*.jpg|TIFF file (*.tiff)|*.tiff";
			saveFileDialog.RestoreDirectory = true;
			if(saveFileDialog.ShowDialog() == DialogResult.OK) {
				image.saveImagen(saveFileDialog.FileName);
			}
		}

		private void guardarToolStripMenuItem_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.saveImagen();
		}

		private void verToolStripMenuItem_Click(object sender, EventArgs e) {
			panel1.Visible = !panel1.Visible;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e) {
			image.setEcualizateImage();
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e) {
			image.setUmbralizar();
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void trackBarUmbral_ValueChanged(object sender, EventArgs e) {
			image.setUmbralThreshold(trackBarUmbral.Value);
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e) {
			image.setRotationAngle(trackBarRotacion.Value);
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void trackBarContraste_ValueChanged(object sender, EventArgs e) {
			image.setContraste(trackBarContraste.Value);
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			pictureBox.Image = image.getActualImage();
		}

		private void zoomIn_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.setZoomProportion(false);
			pictureBox.Image = image.getActualImage();
		}

		private void zoomOut_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			image.setZoomProportion(true);
			pictureBox.Image = image.getActualImage();
		}

		private void nearestNeighborToolStripMenuItem_Click(object sender, EventArgs e) {
			nearestNeighborToolStripMenuItem.Enabled = false;
			nearestNeighborToolStripMenuItem.Checked = true;
			bilinealToolStripMenuItem.Checked = false;
			bilinealToolStripMenuItem.Enabled = true;
			image.setInterpolation();
			if(image.imageIsOpen()) {
				pictureBox.Image = image.getActualImage();
			}
		}

		private void bilinealToolStripMenuItem_Click(object sender, EventArgs e) {
			nearestNeighborToolStripMenuItem.Enabled = true;
			nearestNeighborToolStripMenuItem.Checked = false;
			bilinealToolStripMenuItem.Checked = true;
			bilinealToolStripMenuItem.Enabled = false;
			image.setInterpolation();
			if(image.imageIsOpen()) {
				pictureBox.Image = image.getActualImage();
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			if(!image.imageIsOpen()) {
				MessageBox.Show("Para aplicar esta accion, primero debe abrir una imagen.", "Error con la imagen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if(comboBox1.SelectedIndex == 5 && !image.kernelIsLoaded()) {
				MessageBox.Show("Para aplicar esta accion, primero debe cargar un Kernel.", "Error con el Kernel", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			switch(comboBox1.SelectedIndex) {
				case 0:
				image.suavizadoMedia(comboBox3.SelectedIndex);
				break;
				case 1:
				image.suavizadoGaussiano(comboBox3.SelectedIndex);
				break;
				case 2:
				image.SetPrewitt(comboBox3.SelectedIndex);
				break;
				case 3:
				image.SetSobel(comboBox3.SelectedIndex);
				break;
				case 4:
				image.SetPerfilado(comboBox3.SelectedIndex);
				break;
				case 5:
					image.setGeneralKernel();
				break;
			}
			if(image.imageIsOpen()) {
				pictureBox.Image = image.getActualImage();
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if(comboBox1.SelectedIndex == 5) {
				button2.Enabled = true;
				comboBox3.Enabled = false;
				button1.Text = "Aplicar\nDeshacer";
			}else{
				button1.Text = "Aplicar";
				button2.Enabled = false;
				comboBox3.Enabled = true;
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Kernel files (*.txt, *.kernel, *.pdi)|*.txt;*.kernel;*.pdi;";
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Multiselect = false;
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				image.loadKernelFile(openFileDialog.FileName);
			}
		}

		private void Form_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) {
				if(image.imageIsOpen()) {
					image.revertLastAction();
				}
			}
			if(image.imageIsOpen())
				pictureBox.Image = image.getActualImage();
		}

	}
}
