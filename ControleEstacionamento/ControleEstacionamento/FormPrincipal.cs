using System;
using System.Windows.Forms;

namespace ControleEstacionamento
{
    public partial class FormPrincipal : Form
    {
        
        private readonly EstacionamentoService _estacionamentoService;

        public FormPrincipal()
        {
            InitializeComponent();
           
            _estacionamentoService = new EstacionamentoService();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
           
            _estacionamentoService.InicializarBancoDeDados();
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            try
            {
                


                dataGridViewVeiculos.DataSource = _estacionamentoService.ObterVeiculosEstacionados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao atualizar a lista de veículos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrarEntrada_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim().ToUpper();

            try
            {
           
                _estacionamentoService.RegistrarEntrada(placa);

                MessageBox.Show("Entrada registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPlaca.Clear();
                AtualizarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                SaidaInfo info = _estacionamentoService.RegistrarSaida(placa);

                
                MessageBox.Show($"Saída registrada!\n\nPermanência: {info.Permanencia.Hours}h {info.Permanencia.Minutes}m\nValor a pagar: {info.ValorTotal:C}", "Saída de Veículo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtPlaca.Clear();
                AtualizarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabelaDePreçosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            using (FormTabelaPrecos formPrecos = new FormTabelaPrecos())
            {
                formPrecos.ShowDialog();
            }
        }
    }
}