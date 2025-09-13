// Models/TabelaDePreco.cs
public class TabelaDePreco
{
    public int Id { get; set; } // Chave primária
    public DateTime InicioVigencia { get; set; }
    public DateTime FimVigencia { get; set; }
    public decimal ValorHoraInicial { get; set; }
    public decimal ValorHoraAdicional { get; set; }
}