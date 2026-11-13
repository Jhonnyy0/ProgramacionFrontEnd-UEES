using BibliotecaMVC.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Reportes
{
    public static class GeneradorReportePdf
    {
        public static byte[] Generar(List<Libro> libros, string filtro)
        {
            var fechaGeneracion = DateTime.Now;

            var documento = Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(30);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(10));

                    pagina.Header()
                        .BorderBottom(1)
                        .BorderColor(Colors.Blue.Darken3)
                        .PaddingBottom(5)
                        .Row(fila =>
                        {
                            fila.RelativeItem().Column(titulo =>
                            {
                                titulo.Item().Text("BibliotecaMVC").FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                                titulo.Item().Text("Reporte de Libros").FontSize(14).SemiBold();
                            });

                            fila.ConstantItem(150)
                                .AlignRight()
                                .Text($"Generado el {fechaGeneracion:dd/MM/yyyy HH:mm}")
                                .FontSize(9);
                        });

                    pagina.Content().PaddingVertical(10).Column(columna =>
                    {
                        columna.Spacing(10);

                        columna.Item().Text($"Filtro aplicado: {filtro}").Italic();

                        columna.Item()
                            .Background(Colors.Grey.Lighten4)
                            .Padding(8)
                            .Text(texto =>
                            {
                                texto.Span("Total de libros: ").SemiBold();
                                texto.Span($"{libros.Count}");
                                texto.Span("   |   ");
                                texto.Span("Total de ejemplares: ").SemiBold();
                                texto.Span($"{libros.Sum(l => l.Ejemplares)}");
                                texto.Span("   |   ");
                                texto.Span("Disponibles: ").SemiBold();
                                texto.Span($"{libros.Count(l => l.Disponible)}");
                            });

                        if (libros.Count == 0)
                        {
                            columna.Item().Text("No hay libros que coincidan con el filtro seleccionado.");
                        }
                        else
                        {
                            columna.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columnas =>
                                {
                                    columnas.ConstantColumn(35);
                                    columnas.RelativeColumn(3);
                                    columnas.RelativeColumn(2.5f);
                                    columnas.RelativeColumn(2);
                                    columnas.ConstantColumn(40);
                                    columnas.ConstantColumn(60);
                                    columnas.ConstantColumn(75);
                                });

                                tabla.Header(encabezado =>
                                {
                                    encabezado.Cell().Element(CeldaEncabezado).AlignCenter().Text("ID");
                                    encabezado.Cell().Element(CeldaEncabezado).Text("Título");
                                    encabezado.Cell().Element(CeldaEncabezado).Text("Autor");
                                    encabezado.Cell().Element(CeldaEncabezado).Text("Editorial");
                                    encabezado.Cell().Element(CeldaEncabezado).AlignCenter().Text("Año");
                                    encabezado.Cell().Element(CeldaEncabezado).AlignCenter().Text("Ejemplares");
                                    encabezado.Cell().Element(CeldaEncabezado).AlignCenter().Text("Estado");
                                });

                                for (int i = 0; i < libros.Count; i++)
                                {
                                    var libro = libros[i];
                                    var fondo = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                    Func<IContainer, IContainer> celda = contenedorCelda => contenedorCelda
                                        .Background(fondo)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5);

                                    tabla.Cell().Element(celda).AlignCenter().Text(libro.Id.ToString());
                                    tabla.Cell().Element(celda).Text(libro.Titulo);
                                    tabla.Cell().Element(celda).Text(libro.Autor);
                                    tabla.Cell().Element(celda).Text(string.IsNullOrWhiteSpace(libro.Editorial) ? "-" : libro.Editorial);
                                    tabla.Cell().Element(celda).AlignCenter().Text(libro.AnioPublicacion.ToString());
                                    tabla.Cell().Element(celda).AlignCenter().Text(libro.Ejemplares.ToString());
                                    tabla.Cell().Element(celda).AlignCenter()
                                        .Text(libro.Disponible ? "Disponible" : "No disponible")
                                        .FontColor(libro.Disponible ? Colors.Green.Darken2 : Colors.Red.Darken2);
                                }
                            });
                        }
                    });

                    pagina.Footer()
                        .AlignCenter()
                        .DefaultTextStyle(estilo => estilo.FontSize(9).FontColor(Colors.Grey.Darken1))
                        .Text(texto =>
                        {
                            texto.Span("Página ");
                            texto.CurrentPageNumber();
                            texto.Span(" de ");
                            texto.TotalPages();
                        });
                });
            });

            return documento.GeneratePdf();
        }

        private static IContainer CeldaEncabezado(IContainer contenedor)
        {
            return contenedor
                .Background(Colors.Blue.Darken3)
                .Padding(5)
                .DefaultTextStyle(estilo => estilo.FontColor(Colors.White).Bold());
        }
    }
}
