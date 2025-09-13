using System;
using System.Linq;
using System.Windows.Forms;

namespace ControleEstacionamento 
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            using (var context = new ParkingContext())
            {
                context.Database.EnsureCreated();
            }
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            using (var context = new ParkingContext())
            {
                var veiculosEstacionados = context.RegistrosEstacionamento
                                                  .Where(r => r.DataSaida == null)
                                                  .ToList();
                dataGridViewVeiculos.DataSource = veiculosEstacionados;
            }
        }

        private void btnRegistrarEntrada_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(placa))
            {
                MessageBox.Show("Por favor, digite a placa do veículo.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new ParkingContext())
            {
                bool jaEstacionado = context.RegistrosEstacionamento.Any(r => r.Placa == placa && r.DataSaida == null);
                if (jaEstacionado)
                {
                    MessageBox.Show("Este veículo já se encontra no estacionamento.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var novoRegistro = new RegistroEstacionamento
                {
                    Placa = placa,
                    DataEntrada = DateTime.Now
                };

                context.RegistrosEstacionamento.Add(novoRegistro);
                context.SaveChanges();

                MessageBox.Show("Entrada registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPlaca.Clear();
                AtualizarGrid();
            }
        }

        private void btnRegistrarSaida_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(placa))
            {
                if (dataGridViewVeiculos.CurrentRow != null)
                {
                    placa = dataGridViewVeiculos.CurrentRow.Cells["Placa"].Value.ToString();
                }
                else
                {
                    MessageBox.Show("Digite ou selecione a placa do veículo para registrar a saída.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var context = new ParkingContext())
            {
                var registro = context.RegistrosEstacionamento.FirstOrDefault(r => r.Placa == placa && r.DataSaida == null);
                if (registro == null)
                {
                    MessageBox.Show("Veículo não encontrado no pátio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var tabelaPreco = context.TabelasDePrecos.FirstOrDefault(t => registro.DataEntrada >= t.InicioVigencia && registro.DataEntrada <= t.FimVigencia);
                if (tabelaPreco == null)
                {
                    MessageBox.Show("Não há uma tabela de preços válida para a data de entrada do veículo. Cadastre uma tabela de preços.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime dataSaida = DateTime.Now;
                decimal valorTotal = CalcularValor(registro.DataEntrada, dataSaida, tabelaPreco);

                registro.DataSaida = dataSaida;
                registro.ValorTotal = valorTotal;
                context.SaveChanges();

                TimeSpan permanencia = dataSaida - registro.DataEntrada;
                MessageBox.Show($"Saída registrada!\n\nPermanência: {permanencia.Hours}h {permanencia.Minutes}m\nValor a pagar: {valorTotal:C}", "Saída de Veículo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtPlaca.Clear();
                AtualizarGrid();
            }
        }

        private decimal CalcularValor(DateTime entrada, DateTime saida, TabelaDePreco tabela)
        {
            TimeSpan permanencia = saida - entrada;
            double totalMinutos = permanencia.TotalMinutes;

            if (totalMinutos <= 30)
            {
                return tabela.ValorHoraInicial / 2;
            }

            decimal valorTotal = tabela.ValorHoraInicial;
            double minutosRestantes = totalMinutos - 60;

            if (minutosRestantes > 0)
            {
                int horasAdicionaisCobradas = 0;
                while (minutosRestantes > 10)
                {
                    horasAdicionaisCobradas++;
                    minutosRestantes -= 60;
                }
                valorTotal += horasAdicionaisCobradas * tabela.ValorHoraAdicional;
            }
            return valorTotal;
        }

        private void tabelaDePreçosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTabelaPrecos formPrecos = new FormTabelaPrecos();
            formPrecos.ShowDialog();
        }
    }
}