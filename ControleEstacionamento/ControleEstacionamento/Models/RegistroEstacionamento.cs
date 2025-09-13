public class RegistroEstacionamento
{
    public int Id { get; set; } 
    public string Placa { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; } 
    public decimal? ValorTotal { get; set; } 
}