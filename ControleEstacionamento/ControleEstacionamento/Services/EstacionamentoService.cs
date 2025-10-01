using System;
using System.Collections.Generic;
using System.Linq;

namespace ControleEstacionamento
{
    
    public class SaidaInfo
    {
        public TimeSpan Permanencia { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class EstacionamentoService
    {
     
        public void InicializarBancoDeDados()
        {
            using (var context = new ParkingContext())
            {
                context.Database.EnsureCreated();
            }
        }

   
        public List<RegistroEstacionamento> ObterVeiculosEstacionados()
        {
            using (var context = new ParkingContext())
            {
                return context.RegistrosEstacionamento
                              .Where(r => r.DataSaida == null)
                              .ToList();
            }
        }

        
        public void RegistrarEntrada(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa não pode ser vazia.");
            }

            using (var context = new ParkingContext())
            {
                bool jaEstacionado = context.RegistrosEstacionamento.Any(r => r.Placa == placa && r.DataSaida == null);
                if (jaEstacionado)
                {
                    throw new InvalidOperationException("Este veículo já se encontra no estacionamento.");
                }

                var novoRegistro = new RegistroEstacionamento
                {
                    Placa = placa,
                    DataEntrada = DateTime.Now
                };

                context.RegistrosEstacionamento.Add(novoRegistro);
                context.SaveChanges();
            }
        }

       
        public SaidaInfo RegistrarSaida(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa não pode ser vazia.");
            }

            using (var context = new ParkingContext())
            {
                var registro = context.RegistrosEstacionamento.FirstOrDefault(r => r.Placa == placa && r.DataSaida == null);
                if (registro == null)
                {
                    throw new InvalidOperationException("Veículo não encontrado no pátio.");
                }

                var tabelaPreco = context.TabelasDePrecos.FirstOrDefault(t => registro.DataEntrada >= t.InicioVigencia && registro.DataEntrada <= t.FimVigencia);
                if (tabelaPreco == null)
                {
                    throw new InvalidOperationException("Não há uma tabela de preços válida para a data de entrada do veículo.");
                }

                DateTime dataSaida = DateTime.Now;
                decimal valorTotal = CalcularValor(registro.DataEntrada, dataSaida, tabelaPreco);

                registro.DataSaida = dataSaida;
                registro.ValorTotal = valorTotal;
                context.SaveChanges();

                return new SaidaInfo
                {
                    Permanencia = dataSaida - registro.DataEntrada,
                    ValorTotal = valorTotal
                };
            }
        }

        
        private decimal CalcularValor(DateTime entrada, DateTime saida, TabelaDePreco tabela)
        {
            TimeSpan permanencia = saida - entrada;
            double totalMinutos = permanencia.TotalMinutes;

            if (totalMinutos <= 0) return 0;

            if (totalMinutos <= 30)
            {
                return tabela.ValorHoraInicial / 2;
            }

            decimal valorTotal = tabela.ValorHoraInicial;
            double minutosRestantes = totalMinutos - 60;

            if (minutosRestantes > 10) 
            {
                
                int horasAdicionais = (int)Math.Ceiling(minutosRestantes / 60.0);
                valorTotal += horasAdicionais * tabela.ValorHoraAdicional;
            }

            return valorTotal;
        }
    }
}