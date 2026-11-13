using BibliotecaMVC.Models;
using ClosedXML.Excel;

namespace BibliotecaMVC.Reportes
{
    public static class GeneradorReporteExcel
    {
        public static byte[] Generar(List<Libro> libros, string filtro)
        {
            const int filaEncabezado = 8;
            string[] titulos = { "ID", "Título", "Autor", "Editorial", "Año", "Ejemplares", "Estado" };

            using var libroExcel = new XLWorkbook();
            var hoja = libroExcel.Worksheets.Add("Reporte de Libros");

            var celdaTitulo = hoja.Cell(1, 1);
            celdaTitulo.Value = "BibliotecaMVC - Reporte de Libros";
            celdaTitulo.Style.Font.Bold = true;
            celdaTitulo.Style.Font.FontSize = 16;
            celdaTitulo.Style.Font.FontColor = XLColor.FromHtml("#0D6EFD");

            hoja.Cell(2, 1).Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Cell(3, 1).Value = $"Filtro aplicado: {filtro}";
            hoja.Cell(4, 1).Value = $"Total de libros: {libros.Count}";
            hoja.Cell(5, 1).Value = $"Total de ejemplares: {libros.Sum(l => l.Ejemplares)}";
            hoja.Cell(6, 1).Value = $"Disponibles: {libros.Count(l => l.Disponible)}";
            hoja.Range(4, 1, 6, 1).Style.Font.Bold = true;

            for (int i = 0; i < titulos.Length; i++)
            {
                hoja.Cell(filaEncabezado, i + 1).Value = titulos[i];
            }

            var encabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, titulos.Length);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int fila = filaEncabezado + 1;

            foreach (var libro in libros)
            {
                hoja.Cell(fila, 1).Value = libro.Id;
                hoja.Cell(fila, 2).Value = libro.Titulo;
                hoja.Cell(fila, 3).Value = libro.Autor;
                hoja.Cell(fila, 4).Value = string.IsNullOrWhiteSpace(libro.Editorial) ? "-" : libro.Editorial;
                hoja.Cell(fila, 5).Value = libro.AnioPublicacion;
                hoja.Cell(fila, 6).Value = libro.Ejemplares;
                hoja.Cell(fila, 7).Value = libro.Disponible ? "Disponible" : "No disponible";

                hoja.Cell(fila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hoja.Cell(fila, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hoja.Cell(fila, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hoja.Cell(fila, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hoja.Cell(fila, 7).Style.Font.FontColor = libro.Disponible
                    ? XLColor.FromHtml("#198754")
                    : XLColor.FromHtml("#6C757D");

                fila++;
            }

            int ultimaFila = fila - 1;

            var tabla = hoja.Range(filaEncabezado, 1, ultimaFila, titulos.Length);
            tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            hoja.Columns().AdjustToContents(filaEncabezado, ultimaFila);

            using var flujo = new MemoryStream();
            libroExcel.SaveAs(flujo);

            return flujo.ToArray();
        }
    }
}
