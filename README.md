# Tarea #3 Procesamiento Digital de Imágenes

>Se requiere que Ud. desarrolle una aplicacion en el lenguaje de programacion de su preferencia que permita realizar las siguientes funcionalidaes:

> - Abrir una imagen en formato BMP sin compresion, de cualquier tipo.
> - Calcular y mostrar informacion relevante de la imagen (e.g. dimensiones, profundidad de bits, Mb ocupados.)
> - Calcular y desplegar el histograma de la imagen. Si la imagen es a color, entonces un histograma por cada canal.
> - Realizar la ecualizacion de la imagen
> - Realizar las operaciones de espejo horizontal y vertical.
> - Calcular el negativo de la imagen.
> - Modificar el brillo y contraste basado en modificaciones al histograma.
> - Permitir la umbralizacion de la imagen, dado un valor threshold o un rango (i.e. valor minimo y maximo.)
> - Permitir el escalamiento y rotacion libre de la imagen.
> - Ofrecer la opcion de interpolacion nearest o bilinear para las opciones que lo requieran.
> - Aplicar acercamiento y alejamiento (zoom in/out)
> - Salvar la imagen resultante de las modificaciones realizadas.
> - Ofrecer al menos 2 filtros de convolucion distintos para el suavizado a ser aplicados en una imagen.
> - Aplicar el filtro de convolucion para el perfilado.
> - Permitir aplicar al menos 2 filtros de convolucion distintos para obtener bordes.
> - Ofrecer diversos tamaño para los filtros (3 × 3, 5 × 5, 7 × 7, . . ., 15 × 15)
> - Permitir al usuario agregar/construir su propio kernel de convolucion a ser aplicado en una imagen.
> - Implementar la opcion de deshacer y rehacer de al menos 1 nivel.

- - - - 
### Estructura del Repositorio

Dillinger uses a number of open source projects to work properly:

* [src/] - Contiene el código fuente (Los archivos *.CS)
* [ide/] - Contiene los archivos relacionados al proyecto en Visual Studio 2015.
* [bin/] - Contiene la versión ejecutable para Windows de la tarea.
* [build/] - Contiene los archivos generados por el compilador Visual Studio 2015.
* [doc/] - Contiene la documentación empleada para la elaboracion de la asignació.


- - - - 
### Ejecucion del Programa

Dentro de la carpeta [bin/] se encuentra el archivo ejecutable del proyecto, basta con descargarlo y ejecutar este programa. Esto mostrará una interfaz grafica bastante intuitiva que permitirá cargar la imagen y aplicar las distintas operaciones requeridas. En el menu superior se encuentra el boton que desplegara el histograma. Para aplicar el rehacer y deshacer se usa la convinacion de teclas Ctrl + Z.

### Formato del archivo para el Kernel personalizado

Tamaño del Kernel.
Valores en la matriz del kernel separados por espacio y saltos de linea.

## Ejemplo:
> 3
> 0.0 -1.0 0.0
> -1.0 5.0 -1.0
> 0.0 -1.0 0.0

[Archivo De Ejemplo]

- - - -
### Imagenes de la aplicacion

![Screenshot de la Aplicacion](./doc/screenshot_1.png "Screenshot #1")

![Screenshot de la Aplicacion](./doc/screenshot_2.png "Screenshot #2")

![Screenshot de la Aplicacion](./doc/screenshot_3.png "Screenshot #3")

![Screenshot de la Aplicacion](./doc/screenshot_4.png "Screenshot #3")

- - - - 
### Versión
3.0


- - - - 
### Autor
**Nombre:** Jorge Taoufik Khabazze Maspero  
**C.I.:** 23.692.079  
**E-Mail:** jtkm6jk@gmail.com


   [src/]: <./src/>
   [ide/]: <./ide/>
   [bin/]: <./bin/>
   [build/]: <./build/>
   [doc/]: <./doc/>
   [Archivo De Ejemplo]: <./bin/Example.kernel>
