using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PDI_Tarea_3 {

	class ImageManipulation {
		// Data to work.
		private Label fileName, fileSize, fileDimentions, bitsProfundidad;
		private Bitmap originalImageFile, resultImageFile;
		private String filePatch;
		private Chart histogram;
		private CheckBox A, B;

		// Image, modifications data.
		protected int brilloEnLaImagen = 0, umbralThreshold = 127, filterSizeG = 3, filterSizeM = 3, filterSizeP = 3, filterSizeBP = 3, lastOp = -1;
		protected float anguloARotar = 0.0f, escalaAInterpolar = 1.0f, contrasteEnLaImagen = 1.0f;
		protected bool flipVertical = false, flipHorizontal = false, invertirColores = false, ecualizarImagen = false, umbralizar = false, useBilinear = false, gaussianBlur = false, mediaBlur = false, perfilado = false, bordeP = false, bordeS = false, generalFilter = false;
		protected int[] ColorR, ColorG, ColorB, colorDataR, colorDataG, colorDataB;
		double[,] kernelV, kernelH, kernel;

		public ImageManipulation(Chart Histogram, Label fileName, Label fileSize, Label fileDimentions, Label bitsProfundidad, CheckBox A, CheckBox B) {
			this.fileName = fileName; this.fileSize = fileSize; this.fileDimentions = fileDimentions; this.bitsProfundidad = bitsProfundidad;
			histogram = Histogram;
			ColorR = new int[256];
			ColorG = new int[256];
			ColorB = new int[256];
			colorDataR = new int[256];
			colorDataG = new int[256];
			colorDataB = new int[256];
			this.A = A; this.B = B;
		}

		private int clamp(float value) {
			return (int)Math.Max(0.0f, Math.Min(255.0f, value));
		}

		private Bitmap sumBitmap(Bitmap A, Bitmap B) {
			Bitmap OUT = new Bitmap(A.Width, A.Height);
			for(int i = 0; i < A.Height; ++i) {
				for(int j = 0; j < A.Width; ++j) {
					Color Pixel_1 = A.GetPixel(j, i);
					Color Pixel_2 = B.GetPixel(j, i);
					Color result = Color.FromArgb(Pixel_1.A, clamp(Pixel_1.R + Pixel_2.R), clamp(Pixel_1.G + Pixel_2.G), clamp(Pixel_1.B + Pixel_2.B));
					OUT.SetPixel(j, i, result);
				}
			}
			return OUT;
		}

		private Bitmap rotateImageFunction(Bitmap IN, float angle) {
			double radAngle = Math.Max(-angle, angle) * (float)Math.PI / 180.0f;
			double Cos = Math.Abs(Math.Cos(radAngle));
			double Sin = Math.Abs(Math.Sin(radAngle));
			int newWidth = (int)(IN.Width * Cos + IN.Height * Sin);
			int newHeight = (int)(IN.Width * Sin + IN.Height * Cos);
			Bitmap OUT = new Bitmap(newWidth, newHeight);
			Graphics gaphics = Graphics.FromImage(OUT);
			gaphics.TranslateTransform((float)(newWidth - IN.Width) / 2, (float)(newHeight - IN.Height) / 2);
			gaphics.TranslateTransform((float)IN.Width / 2, (float)IN.Height / 2);
			gaphics.RotateTransform(Math.Max(-angle, angle));
			gaphics.TranslateTransform(-(float)IN.Width / 2, -(float)IN.Height / 2);
			gaphics.DrawImage(IN, new Point(0, 0));
			gaphics.Dispose();
			return OUT;
		}

		private Bitmap scalateImageFunction(Bitmap IN, float percent) {
			Bitmap OUT = new Bitmap((int)(IN.Width * percent), (int)(IN.Height * percent));
			double Xratio = (double)IN.Width / (double)OUT.Width;
			double Yratio = (double)IN.Height / (double)OUT.Height;
			for(int Y = 0; Y < OUT.Height; ++Y) {
				for(int X = 0; X < OUT.Width; ++X) {
					double newX = Math.Floor(X * Xratio);
					double newY = Math.Floor(Y * Yratio);
					OUT.SetPixel(X, Y, IN.GetPixel((int)newX, (int)newY));
				}
			}
			return OUT;
		}

		private Bitmap scalateImageFunctionBilinear(Bitmap IN, float percent) {
			Bitmap OUT = new Bitmap((int)(IN.Width * percent), (int)(IN.Height * percent));
			double Xratio = (double)IN.Width / (double)OUT.Width;
			double Yratio = (double)IN.Height / (double)OUT.Height;

			for(int Y = 0; Y < OUT.Height; ++Y) {
				for(int X = 0; X < OUT.Width; ++X) {
					double newX = Math.Floor(X * Xratio);
					double newY = Math.Floor(Y * Yratio);
					double ceil_x = newX + 1;
					if(ceil_x >= IN.Width) ceil_x = (int)newX;
					double ceil_y = newY + 1;
					if(ceil_y >= IN.Height) ceil_y = (int)newY;
					double fraction_x = X * Xratio - newX;
					double fraction_y = Y * Yratio - newY;
					double one_minus_x = 1.0 - fraction_x;
					double one_minus_y = 1.0 - fraction_y;
					Color A = IN.GetPixel((int)newX, (int)newY);
					Color B = IN.GetPixel((int)ceil_x, (int)newY);
					Color C = IN.GetPixel((int)newX, (int)ceil_y);
					Color D = IN.GetPixel((int)ceil_x, (int)ceil_y);
					int AA = (int)(one_minus_y * (one_minus_x * A.A + fraction_x * B.A) + fraction_y * (one_minus_x * C.A + fraction_x * D.A));
					int RR = (int)(one_minus_y * (one_minus_x * A.R + fraction_x * B.R) + fraction_y * (one_minus_x * C.R + fraction_x * D.R));
					int GG = (int)(one_minus_y * (one_minus_x * A.G + fraction_x * B.G) + fraction_y * (one_minus_x * C.G + fraction_x * D.G));
					int BB = (int)(one_minus_y * (one_minus_x * A.B + fraction_x * B.B) + fraction_y * (one_minus_x * C.B + fraction_x * D.B));
					OUT.SetPixel(X, Y, Color.FromArgb(AA, RR, GG, BB));
				}
			}
			return OUT;
		}

		public void loadFile(String Patch) {
			this.filePatch = Patch;
			if(Path.GetExtension(Patch).ToLower() == ".bmp") {
				PDI_Tarea_1.BMP image = new PDI_Tarea_1.BMP();
				image.LoadImageFile(Patch);
				if(image.ImageIsLoaded()) {
					originalImageFile = image.GetImage();
					bitsProfundidad.Text = image.GetFormato() + "bpp";
				}else {
					originalImageFile = (Bitmap)Image.FromFile(filePatch);
					bitsProfundidad.Text = originalImageFile.PixelFormat.ToString();
				}
			} else {
				originalImageFile = (Bitmap)Image.FromFile(filePatch);
				bitsProfundidad.Text = originalImageFile.PixelFormat.ToString();
			}
			fileName.Text = Path.GetFileName(Patch);
			fileSize.Text = ((int)((new FileInfo(Patch)).Length / 1000)).ToString() + " KB";
		}

		public void saveImagen(String patch = null) {
			if(patch == null || patch == filePatch) {
				String extension = Path.GetExtension(filePatch);
				System.Drawing.Imaging.ImageFormat Formato;
				switch(extension.ToLower()) {
					case ".png":
					Formato = System.Drawing.Imaging.ImageFormat.Png;
					break;
					case ".gif":
					Formato = System.Drawing.Imaging.ImageFormat.Gif;
					break;
					case ".tiff":
					Formato = System.Drawing.Imaging.ImageFormat.Tiff;
					break;
					case ".bmp":
					Formato = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
					case ".jpg":
					Formato = System.Drawing.Imaging.ImageFormat.Jpeg;
					break;
					default:
					Formato = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
				}
				originalImageFile.Dispose();
				originalImageFile = (Bitmap) resultImageFile.Clone();
				originalImageFile.Save(filePatch);
			} else {
				String extension = Path.GetExtension(patch);
				System.Drawing.Imaging.ImageFormat Formato;
				switch(extension.ToLower()) {
					case ".png":
					Formato = System.Drawing.Imaging.ImageFormat.Png;
					break;
					case ".gif":
					Formato = System.Drawing.Imaging.ImageFormat.Gif;
					break;
					case ".tiff":
					Formato = System.Drawing.Imaging.ImageFormat.Tiff;
					break;
					case ".bmp":
					Formato = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
					case ".jpg":
					Formato = System.Drawing.Imaging.ImageFormat.Jpeg;
					break;
					default:
					Formato = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
				}
				resultImageFile.Save(patch, Formato);
			}
		}

		public Bitmap getActualImage() {
			if(resultImageFile != null)
				resultImageFile.Dispose();
			Bitmap temp;
			if(useBilinear) {
				temp = scalateImageFunctionBilinear(originalImageFile, escalaAInterpolar);
			} else {
				temp = scalateImageFunction(originalImageFile, escalaAInterpolar);
			}
			resultImageFile = rotateImageFunction(temp, anguloARotar);
			temp.Dispose();
			if(flipHorizontal) {
				for(int y = 0; y < resultImageFile.Height; ++y) {
					for(int x = 0; x < resultImageFile.Width / 2; ++x) {
						Color A = resultImageFile.GetPixel(x, y), B = resultImageFile.GetPixel((resultImageFile.Width - 1) - x, y);
						resultImageFile.SetPixel(x, y, B); resultImageFile.SetPixel((resultImageFile.Width - 1) - x, y, A);
					}
				}
			}
			if(flipVertical) {
				for(int y = 0; y < resultImageFile.Height / 2; ++y) {
					for(int x = 0; x < resultImageFile.Width; ++x) {
						Color A = resultImageFile.GetPixel(x, y), B = resultImageFile.GetPixel(x, (resultImageFile.Height - 1) - y);
						resultImageFile.SetPixel(x, y, B); resultImageFile.SetPixel(x, (resultImageFile.Height - 1) - y, A);
					}
				}
			}
			if(invertirColores) {
				for(int y = 0; y < resultImageFile.Height; ++y) {
					for(int x = 0; x < resultImageFile.Width; ++x) {
						Color A = resultImageFile.GetPixel(x, y);
						resultImageFile.SetPixel(x, y, Color.FromArgb(A.A, 255 - A.R, 255 - A.G, 255 - A.B));
					}
				}
			}
			for(int i = 0; i < resultImageFile.Height; ++i) {
				for(int j = 0; j < resultImageFile.Width; ++j) {
					Color pixel = resultImageFile.GetPixel(j, i);
					int R = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * pixel.R));
					int G = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * pixel.G));
					int B = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * pixel.B));
					pixel = Color.FromArgb(pixel.A, R, G, B);
					resultImageFile.SetPixel(j, i, pixel);
					++ColorR[R]; ++ColorG[G]; ++ColorB[B];
				}
			}
			if(ecualizarImagen) {
				int RAcum = ColorR[0], GAcum = ColorG[0], BAcum = ColorB[0], totalOfPixels = resultImageFile.Width * resultImageFile.Height;
				colorDataR[0] = 0; colorDataG[0] = 0; colorDataB[0] = 0;
				for(int i = 1; i < 255; ++i) {
					colorDataR[i] = RAcum * 255 / totalOfPixels;
					colorDataG[i] = GAcum * 255 / totalOfPixels;
					colorDataB[i] = BAcum * 255 / totalOfPixels;
					RAcum += ColorR[i]; GAcum += ColorG[i]; BAcum += ColorB[i];
					ColorR[i] = ColorG[i] = ColorB[i] = 0;
				}
				colorDataR[255] = 255; colorDataG[255] = 255; colorDataB[255] = 255;
				for(int i = 0; i < resultImageFile.Height; ++i) {
					for(int j = 0; j < resultImageFile.Width; ++j) {
						Color pixel = resultImageFile.GetPixel(j, i);
						int R = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * colorDataR[pixel.R]));
						int G = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * colorDataG[pixel.G]));
						int B = clamp(brilloEnLaImagen + (int)(contrasteEnLaImagen * colorDataB[pixel.B]));
						pixel = Color.FromArgb(pixel.A, R, G, B);
						resultImageFile.SetPixel(j, i, pixel);
						++ColorR[R]; ++ColorG[G]; ++ColorB[B];
					}
				}
			}
			foreach(var series in histogram.Series) {
				series.Points.Clear();
			}
			for(int i = 0; i < 256; ++i) {
				histogram.Series[0].Points.AddXY(i, ColorR[i]);
				histogram.Series[1].Points.AddXY(i, ColorG[i]);
				histogram.Series[2].Points.AddXY(i, ColorB[i]);
				ColorR[i] = ColorG[i] = ColorB[i] = 0;
			}
			if(umbralizar) {
				for(int i = 0; i < resultImageFile.Height; ++i) {
					for(int j = 0; j < resultImageFile.Width; ++j) {
						Color pixel = resultImageFile.GetPixel(j, i);
						int U = ((pixel.R + pixel.G + pixel.B) / 3) > umbralThreshold ? 255 : 0;
						pixel = Color.FromArgb(pixel.A, U, U, U);
						resultImageFile.SetPixel(j, i, pixel);
					}
				}
			}
			if(gaussianBlur) {
				GaussianBlur img = new GaussianBlur(resultImageFile);
				resultImageFile = img.Process(filterSizeG);
			}
			if(mediaBlur) {
				resultImageFile = FilterCollection.MediaFilter(resultImageFile, filterSizeM);
			}
			if(perfilado) {
				double[,] matrix = new double[filterSizeP, filterSizeP];
				for(int i = 0; i < filterSizeP; ++i) {
					for(int j = 0; j < filterSizeP; ++j) {
						if(filterSizeP / 2 == i && filterSizeP / 2 == j)
							matrix[i, j] = filterSizeP * filterSizeP;
						else
							matrix[i, j] = -1;
					}
				}
				resultImageFile = FilterCollection.ConvolutionFilter(resultImageFile, matrix);
			}
			if(bordeP) {
				double[,] matrix_1 = new double[filterSizeBP, filterSizeBP];
				for(int i = 0; i < filterSizeBP; ++i) {
					for(int j = 0; j < filterSizeBP; ++j) {
						if(j == 0)
							matrix_1[i, j] = -1;
						else if(j == filterSizeBP - 1)
							matrix_1[i, j] = 1;
						else
							matrix_1[i, j] = 0;
					}
				}
				double[,] matrix_2 = new double[filterSizeBP, filterSizeBP];
				for(int i = 0; i < filterSizeBP; ++i) {
					for(int j = 0; j < filterSizeBP; ++j) {
						if(i == 0)
							matrix_2[i, j] = -1;
						else if(i == filterSizeBP - 1)
							matrix_2[i, j] = 1;
						else
							matrix_2[i, j] = 0;
					}
				}
				Bitmap temp_1 = FilterCollection.ConvolutionFilter(resultImageFile, matrix_1);
				Bitmap temp_2 = FilterCollection.ConvolutionFilter(resultImageFile, matrix_2);
				resultImageFile = sumBitmap(temp_1, temp_2);
			}
			if(bordeS) {
				Bitmap temp_1 = FilterCollection.ConvolutionFilter(resultImageFile, kernelV);
				Bitmap temp_2 = FilterCollection.ConvolutionFilter(resultImageFile, kernelH);
				resultImageFile = sumBitmap(temp_1, temp_2);
			}
			if(generalFilter) {
				resultImageFile = FilterCollection.ConvolutionFilter(resultImageFile, kernel);
			}
			fileDimentions.Text = resultImageFile.Width.ToString() + " X " + resultImageFile.Height.ToString() + "px";
			return resultImageFile;
		}

		public void setRotationAngle(int angle) {
			anguloARotar = (float)angle;
		}

		public void setZoomProportion(bool Out) {
			escalaAInterpolar += Out ? -0.01f : 0.01f;
			escalaAInterpolar = (escalaAInterpolar < 0) ? 0 : escalaAInterpolar;
		}

		public void setFlipVertical() {
			lastOp = 12;
			flipVertical = !flipVertical;
		}

		public void setFlipHorizontal() {
			lastOp = 11;
			flipHorizontal = !flipHorizontal;
		}

		public void setInvertirColores() {
			lastOp = 10;
			invertirColores = !invertirColores;
		}

		public void setBrillo(int brillo) {
			brilloEnLaImagen = brillo;
		}

		public void setContraste(int contraste) {
			contrasteEnLaImagen = ((float)contraste / 300.0f) + 1.0f;
		}

		public void setEcualizateImage() {
			lastOp = 9;
			ecualizarImagen = !ecualizarImagen;
		}

		public void setUmbralizar() {
			lastOp = 8;
			umbralizar = !umbralizar;
		}

		public void setUmbralThreshold(int threshold) {
			umbralThreshold = threshold;
		}

		public void setInterpolation() {
			lastOp = 7;
			useBilinear = !useBilinear;
		}

		public bool imageIsOpen() {
			return originalImageFile != null;
		}

		public void suavizadoGaussiano(int filterSize) {
			lastOp = 6;
			if(filterSize == 0) {
				gaussianBlur = false;
			}else {
				int[] var = new int[] { 3, 5, 7, 9, 11, 13, 15 };
				this.filterSizeG = var[filterSize - 1];
				gaussianBlur = true;
			}
		}

		public void suavizadoMedia(int filterSize) {
			lastOp = 5;
			if(filterSize == 0) {
				mediaBlur = false;
			} else {
				int[] var = new int[] { 3, 5, 7, 9, 11, 13, 15 };
				this.filterSizeM = var[filterSize - 1];
				mediaBlur = true;
			}
		}

		public void SetPerfilado(int filterSize) {
			lastOp = 4;
			if(filterSize == 0) {
				perfilado = false;
			} else {
				int[] var = new int[] { 3, 5, 7, 9, 11, 13, 15 };
				this.filterSizeP = var[filterSize - 1];
				perfilado = true;
			}
		}

		public void SetPrewitt(int filterSize) {
			lastOp = 3;
			if(filterSize == 0) {
				bordeP = false;
			} else {
				int[] var = new int[] { 3, 5, 7, 9, 11, 13, 15 };
				this.filterSizeBP = var[filterSize - 1];
				bordeP = true;
			}
		}

		public void SetSobel(int filterSize) {
			lastOp = 2;
			if(filterSize == 0) {
				bordeS = false;
			} else if(filterSize == 1) {
				kernelV = new double[,] { { 1,  0, -1 },
										  { 2,  0, -2 },
										  { 1,  0, -1 } };

				kernelH = new double[,] { { 1,  2,  1 },
										  { 0,  0,  0 },
										  {-1, -2, -1 } };
				bordeS = true;
			} else if(filterSize == 2) {
				kernelV = new double[,] { { 1,  2, 0, -2, -1 },
										  { 4,  8, 0, -8, -4 },
										  { 6, 12, 0,-12, -6 },
										  { 4,  8, 0, -8, -4 },
										  { 1,  2, 0, -2, -1 } };

				kernelH = new double[,] { { 1,  4,   6,  4,  1 },
										  { 2,  8,  12,  8,  2 },
										  { 0,  0,   0,  0,  0 },
										  {-2, -8, -12, -8, -4 },
										  {-1, -4,  -6, -4, -1 } };
				bordeS = true;
			} else if(filterSize == 3) {
				kernelV = new double[,] { { 1,  2,  4, 0,  -4, -2, -1 },
										  { 4,  8, 16, 0, -16, -8, -4 },
										  { 6, 12, 24, 0, -24,-12, -6 },
										  { 8, 16, 32, 0, -32,-16, -8 },
										  { 6, 12, 24, 0, -24,-12, -6 },
										  { 4,  8, 16, 0, -16, -8, -4 },
										  { 1,  2,  4, 0,  -4, -2, -1 } };

				kernelH = new double[,] { { 1,  4,  6, 8,  6,  4,  1 },
										  { 2,  8, 12, 16, 12,  8,  2 },
										  { 4, 16, 24, 32, 24, 16,  4 },
										  { 0,  0,  0,  0,  0,  0,  0 },
										  {-4,-16,-24,-32,-24,-16, -4 },
										  {-2, -8,-12,-16,-12, -8, -2 },
										  {-1, -4, -6, -8, -6, -4, -1 } };
				bordeS = true;
			} else if(filterSize == 4) {
				kernelV = new double[,] { {  1,  2,  4,  8,  0, -8, -4, -2, -1 },
										  {  4,  8, 16, 32,  0,-32,-16, -8, -4 },
										  {  6, 12, 24, 48,  0,-48,-24,-12, -6 },
										  {  8, 16, 32, 64,  0,-64,-32,-16, -8 },
										  { 10, 20, 40, 80,  0,-80,-40,-20,-10 },
										  {  8, 16, 32, 64,  0,-64,-32,-16, -8 },
										  {  6, 12, 24, 48,  0,-48,-24,-12, -6 },
										  {  4,  8, 16, 32,  0,-32,-16, -8, -4 },
										  {  1,  2,  4,  8,  0, -8, -4, -2, -1 } };

				kernelH = new double[,] { {  1,  4,  6,  8, 10,  8,  6,  4,  1 },
										  {  2,  8, 12, 16, 20, 16, 12,  8,  2 },
										  {  4, 16, 24, 32, 40, 32, 24, 16,  4 },
										  {  8, 32, 48, 64, 80, 64, 48, 32,  8 },
										  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
										  { -8,-32,-48,-64,-80,-64,-48,-32, -8 },
										  { -4,-16,-24,-32,-40,-32,-24,-16, -4 },
										  { -2, -8,-12,-16,-20,-16,-12, -8, -2 },
										  { -1, -4, -6, -8,-10, -8, -6, -4, -1 } };
				bordeS = true;
			} else if(filterSize == 5) {
				kernelV = new double[,] { {  1,  2,  4,  8, 16,  0, -16, -8, -4, -2, -1 },
										  {  4,  8, 16, 32, 64,  0, -64,-32,-16, -8, -4 },
										  {  6, 12, 24, 48, 96,  0, -96,-48,-24,-12, -6 },
										  {  8, 16, 32, 64,128,  0,-128,-64,-32,-16, -8 },
										  { 10, 20, 40, 80,160,  0,-160,-80,-40,-20,-10 },
										  { 12, 24, 48, 96,192,  0,-192,-96,-48,-24,-12 },
										  { 10, 20, 40, 80,160,  0,-160,-80,-40,-20,-10 },
										  {  8, 16, 32, 64,128,  0,-128,-64,-32,-16, -8 },
										  {  6, 12, 24, 48, 96,  0, -96,-48,-24,-12, -6 },
										  {  4,  8, 16, 32, 64,  0, -64,-32,-16, -8, -4 },
										  {  1,  2,  4,  8, 16,  0, -16, -8, -4, -2, -1 } };

				kernelH = new double[,] { {  1,  4,  6,   8,  10,  12,  10,   8,  6,  4,  1 },
										  {  2,  8, 12,  16,  20,  24,  20,  16, 12,  8,  2 },
										  {  4, 16, 24,  32,  40,  48,  40,  32, 24, 16,  4 },
										  {  8, 32, 48,  64,  80,  96,  80,  64, 48, 32,  8 },
										  { 16, 64, 96, 128, 160, 192, 160, 128, 96,  64,16 },
										  {  0,  0,  0,   0,   0,   0,   0,   0,  0,  0,  0 },
										  {-16,-64,-96,-128,-160,-192,-160,-128,-96,-64,-16 },
										  { -8,-32,-48, -64, -80, -96, -80, -64,-48,-32, -8 },
										  { -4,-16,-24, -32, -40, -48, -40, -32,-24,-16, -4 },
										  { -2, -8,-12, -16, -20, -24, -20, -16,-12, -8, -2 },
										  { -1, -4, -6,  -8,- 10, -12, -10,  -8, -6, -4, -1 } };
				bordeS = true;
			} else if(filterSize == 6) {
				kernelV = new double[,] { {  1,  2,  4,  8, 16, 32, 0, -32, -16,  -8, -4, -2, -1 },
										  {  4,  8, 16, 32, 64,128, 0,-128, -64, -32,-16, -8, -4 },
										  {  6, 12, 24, 48, 96,192, 0,-192, -96, -48,-24,-12, -6 },
										  {  8, 16, 32, 64,128,256, 0,-256,-128, -64,-32,-16, -8 },
										  { 10, 20, 40, 80,160,320, 0,-320,-160, -80,-40,-20,-10 },
										  { 12, 24, 48, 96,192,384, 0,-384,-192, -96,-48,-24,-12 },
										  { 14, 28, 56,112,224,448, 0,-448,-224,-112,-56,-28,-14 },
										  { 12, 24, 48, 96,192,384, 0,-384,-192, -96,-48,-24,-12 },
										  { 10, 20, 40, 80,160,320, 0,-320,-160, -80,-40,-20,-10 },
										  {  8, 16, 32, 64,128,256, 0,-256,-128, -64,-32,-16, -8 },
										  {  6, 12, 24, 48, 96,192, 0,-192, -96, -48,-24,-12, -6 },
										  {  4,  8, 16, 32, 64,128, 0,-128, -64, -32,-16, -8, -4 },
										  {  1,  2,  4,  8, 16, 32, 0, -32, -16,  -8, -4, -2, -1 } };

				kernelH = new double[,] { {  1,   4,   6,   8,  10,  12,  14,  12,  10,   8,   6,  4,   1 },
										  {  2,   8,  12,  16,  20,  24,  28,  24,  20,  16,  12,  8,   2 },
										  {  4,  16,  24,  32,  40,  48,  56,  48,  40,  32,  24, 16,   4 },
										  {  8,  32,  48,  64,  80,  96, 112,  96,  80,  64,  48, 32,   8 },
										  { 16,  64,  96, 128, 160, 192, 224, 192, 160, 128,  96,  64, 16 },
										  { 32, 128, 192, 256, 320, 384, 448, 384, 320, 256, 192, 128, 32 },
										  {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  0 },
										  {-32,-128,-192,-256,-320,-384,-448,-384,-320,-256,-192,-128,-32 },
										  {-16, -64, -96,-128,-160,-192,-224,-192, 160,-128, -96, -64,-16 },
										  { -8, -32, -48, -64, -80, -96,-112, -96, -80, -64, -48, -32, -8 },
										  { -4,- 16, -24, -32, -40, -48, -56, -48, -40, -32, -24, -16, -4 },
										  { -2,  -8, -12, -16, -20, -24, -28, -24, -20, -16,- 12,  -8, -2 },
										  { -1,  -4,  -6,  -8,- 10, -12, -14, -12, -10,  -8,  -6,  -4, -1 } };
				bordeS = true;
			} else {
				kernelV = new double[,] { {  1,  2,  4,  8, 16, 32,  64, 0,  -64, -32, -16,  -8, -4, -2, -1 },
										  {  4,  8, 16, 32, 64,128, 256, 0, -256,-128, -64, -32,-16, -8, -4 },
										  {  6, 12, 24, 48, 96,192, 384, 0, -384,-192, -96, -48,-24,-12, -6 },
										  {  8, 16, 32, 64,128,256, 512, 0, -512,-256,-128, -64,-32,-16, -8 },
										  { 10, 20, 40, 80,160,320, 640, 0, -640,-320,-160, -80,-40,-20,-10 },
										  { 12, 24, 48, 96,192,384, 768, 0, -768,-384,-192, -96,-48,-24,-12 },
										  { 14, 28, 56,112,224,448, 896, 0, -896,-448,-224,-112,-56,-28,-14 },
										  { 16, 32, 64,128,256,512,1024, 0,-1024,-512,-256,-128,-64,-32,-16 },
										  { 14, 28, 56,112,224,448, 896, 0, -896,-448,-224,-112,-56,-28,-14 },
										  { 12, 24, 48, 96,192,384, 768, 0, -768,-384,-192, -96,-48,-24,-12 },
										  { 10, 20, 40, 80,160,320, 640, 0, -640,-320,-160, -80,-40,-20,-10 },
										  {  8, 16, 32, 64,128,256, 512, 0, -512,-256,-128, -64,-32,-16, -8 },
										  {  6, 12, 24, 48, 96,192, 384, 0, -384,-192, -96, -48,-24,-12, -6 },
										  {  4,  8, 16, 32, 64,128, 256, 0, -256,-128, -64, -32,-16, -8, -4 },
										  {  1,  2,  4,  8, 16, 32,  64, 0,   64, -32, -16,  -8, -4, -2, -1 } };

				kernelH = new double[,] { {  1,   4,   6,   8,  10,  12,  14,   16,  14,  12,  10,   8,   6,  4,   1 },
										  {  2,   8,  12,  16,  20,  24,  28,   32,  28,  24,  20,  16,  12,  8,   2 },
										  {  4,  16,  24,  32,  40,  48,  56,   64,  56,  48,  40,  32,  24, 16,   4 },
										  {  8,  32,  48,  64,  80,  96, 112,  128, 112,  96,  80,  64,  48, 32,   8 },
										  { 16,  64,  96, 128, 160, 192, 224,  256, 224, 192, 160, 128,  96,  64, 16 },
										  { 32, 128, 192, 256, 320, 384, 448,  512, 448, 384, 320, 256, 192, 128, 32 },
										  { 64, 256, 384, 512, 640, 768, 896, 1024, 896, 768, 640, 512, 384, 256, 64 },
										  {  0,   0,   0,   0,   0,   0,   0,    0,   0,   0,   0,   0,   0,   0,  0 },
										  {-64,-256,-384,-512,-640,-768,-896,-1024,-896,-768,-640,-512,-384,-256,-64 },
										  {-32,-128,-192,-256,-320,-384,-448, -512,-448,-384,-320,-256,-192,-128,-32 },
										  {-16, -64, -96,-128,-160,-192,-224, -256,-224,-192, 160,-128, -96, -64,-16 },
										  { -8, -32, -48, -64, -80, -96,-112, -128,-112, -96, -80, -64, -48, -32, -8 },
										  { -4,- 16, -24, -32, -40, -48, -56,  -64, -56, -48, -40, -32, -24, -16, -4 },
										  { -2,  -8, -12, -16, -20, -24, -28,  -32, -28, -24, -20, -16,- 12,  -8, -2 },
										  { -1,  -4,  -6,  -8,- 10, -12, -14,  -16, -14, -12, -10,  -8,  -6,  -4, -1 } };
				bordeS = true;
			}
		}

		public void loadKernelFile(String Patch) {
			int kernelSize;
			using(TextReader reader = File.OpenText(Patch)) {
				string line;
				if((line = reader.ReadLine()) != null) {
					if(!int.TryParse(line, out kernelSize)) {
						Console.WriteLine("Bad value");
					}else{
						kernel = new double[kernelSize, kernelSize];
						for(int i = 0; i < kernelSize; ++i) {
							line = reader.ReadLine();
							string[] bits = line.Split(' ');
							for(int j = 0; j < kernelSize; ++j) {
								double.TryParse(bits[j], out kernel[i,j]);
							}
						}
					}
				}
			}
		}

		public void setGeneralKernel() {
			lastOp = 1;
			generalFilter = !generalFilter;
		}

		public bool kernelIsLoaded() {
			return kernel != null;
		}

		public void revertLastAction() {
			switch(lastOp) {
				case 1:
				generalFilter = !generalFilter;
				break;
				case 2:
				bordeS = !bordeS;
				break;
				case 3:
				bordeP = !bordeP;
				break;
				case 4:
				perfilado = !perfilado;
				break;
				case 5:
				mediaBlur = !mediaBlur;
				break;
				case 6:
				gaussianBlur = !gaussianBlur;
				break;
				case 7:
				useBilinear = !useBilinear;
				break;
				case 8:
				B.Checked = !umbralizar;
				break;
				case 9:
				A.Checked = !ecualizarImagen;
				break;
				case 10:
				invertirColores = !invertirColores;
				break;
				case 11:
				flipHorizontal = !flipHorizontal;
				break;
				case 12:
				flipVertical = !flipVertical;
				break;
			}
		}
	}
}
