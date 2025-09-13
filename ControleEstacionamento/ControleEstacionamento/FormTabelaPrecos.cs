using System;
using System.Linq;
using System.Windows.Forms;

namespace ControleEstacionamento 
{
    public partial class FormTabelaPrecos : Form
    {
        public FormTabelaPrecos()
        {
            InitializeComponent();
        }

        private void FormTabelaPrecos_Load(object sender, EventArgs e)
        {
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            using (var context = new ParkingContext())
            {
                dataGridViewPrecos.DataSource = context.TabelasDePrecos.ToList();
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (dtpInicio.Value.Date >= dtpFim.Value.Date)
            {
                MessageBox.Show("A data de início da vigência deve ser anterior à data de fim.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numValorInicial.Value <= 0 || numValorAdicional.Value <= 0)
            {
                MessageBox.Show("Os valores da hora inicial e adicional devem ser maiores que zero.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var novaTabela = new TabelaDePreco
            {
                InicioVigencia = dtpInicio.Value.Date,
                FimVigencia = dtpFim.Value.Date,
                ValorHoraInicial = numValorInicial.Value,
                ValorHoraAdicional = numValorAdicional.Value
            };

            using (var context = new ParkingContext())
            {
                context.TabelasDePrecos.Add(novaTabela);
                context.SaveChanges();
            }

            MessageBox.Show("Nova tabela de preços salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarGrid();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dataGridViewPrecos.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecione uma tabela de preços para excluir.", "Nenhum item selecionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Tem certeza que deseja excluir a tabela de preços selecionada?\n\nEsta ação não pode ser desfeita.",
                                             "Confirmar Exclusão",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                int idParaExcluir = (int)dataGridViewPrecos.CurrentRow.Cells["Id"].Value;

                using (var context = new ParkingContext())
                {
                    var tabelaParaExcluir = context.TabelasDePrecos.Find(idParaExcluir);

                    if (tabelaParaExcluir != null)
                    {
                        context.TabelasDePrecos.Remove(tabelaParaExcluir);
                        context.SaveChanges(); 

                        AtualizarGrid();
                        MessageBox.Show("Tabela de preços excluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}