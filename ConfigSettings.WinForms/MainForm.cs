using ConfigSettings.Shared;
using ConfigSettings.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class MainForm : Form
{
    private readonly ConfigSettingsContext _context;
    private DataGridView _grid;
    private Button _btnAdd, _btnUpdate, _btnDelete, _btnRefresh;
    private Panel panel1;
    private Button btnAdd;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnRefresh;
    private DataGridView dataGridView1;

    public MainForm(ConfigSettingsContext context)
    {
        _context = context;
        InitializeComponent();

        Load += async (s, e) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        panel1 = new Panel();
        btnAdd = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnRefresh = new Button();
        dataGridView1 = new DataGridView();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Controls.Add(dataGridView1);
        panel1.Location = new System.Drawing.Point(7, 6);
        panel1.Name = "panel1";
        panel1.Size = new System.Drawing.Size(865, 498);
        panel1.TabIndex = 0;
        // 
        // btnAdd
        // 
        btnAdd.Location = new System.Drawing.Point(14, 527);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new System.Drawing.Size(98, 23);
        btnAdd.TabIndex = 1;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnUpdate
        // 
        btnUpdate.Location = new System.Drawing.Point(130, 527);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new System.Drawing.Size(98, 23);
        btnUpdate.TabIndex = 2;
        btnUpdate.Text = "Update";
        btnUpdate.UseVisualStyleBackColor = true;
        // 
        // btnDelete
        // 
        btnDelete.Location = new System.Drawing.Point(254, 527);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new System.Drawing.Size(98, 23);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new System.Drawing.Point(384, 527);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new System.Drawing.Size(98, 23);
        btnRefresh.TabIndex = 4;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        // 
        // dataGridView1
        // 
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.Location = new System.Drawing.Point(11, 30);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.Size = new System.Drawing.Size(841, 465);
        dataGridView1.TabIndex = 0;
        // 
        // MainForm
        // 
        ClientSize = new System.Drawing.Size(884, 561);
        Controls.Add(btnRefresh);
        Controls.Add(btnDelete);
        Controls.Add(btnUpdate);
        Controls.Add(btnAdd);
        Controls.Add(panel1);
        Name = "MainForm";
        Text = "Config Settings Demo";
        panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ResumeLayout(false);

        /*
        _grid = new DataGridView { Dock = DockStyle.Top, Height = 420, ReadOnly = true, AutoGenerateColumns = true };
        _btnAdd = new Button { Text = "Add Encrypted", Dock = DockStyle.Left, Width = 120 };
        _btnUpdate = new Button { Text = "Update Selected", Dock = DockStyle.Left, Width = 120 };
        _btnDelete = new Button { Text = "Delete Selected", Dock = DockStyle.Left, Width = 120 };
        _btnRefresh = new Button { Text = "Refresh", Dock = DockStyle.Right, Width = 120 };

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
        panel.Controls.Add(_btnAdd);
        panel.Controls.Add(_btnUpdate);
        panel.Controls.Add(_btnDelete);
        panel.Controls.Add(_btnRefresh);

        Controls.Add(_grid);
        Controls.Add(panel);

        Load += async (s, e) => await LoadDataAsync();
        _btnRefresh.Click += async (s, e) => await LoadDataAsync();
        _btnAdd.Click += async (s, e) => await AddSampleAsync();
        _btnUpdate.Click += async (s, e) => await UpdateSelectedAsync();
        _btnDelete.Click += async (s, e) => await DeleteSelectedAsync();
        */
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var list = (await _context.GetAllAsync()).ToList();
            _grid.DataSource = list;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddSampleAsync()
    {
        var setting = new ConfigSetting
        {
            ApplicationName = "DemoApp",
            Instance = "LOCAL",
            HostName = Environment.MachineName,
            UserName = Environment.UserName,
            Classification = "Secrets",
            FieldName = "ApiKey",
            FieldValue = Guid.NewGuid().ToString("N").Substring(0, 20),
            IsEncrypted = true,
            UpdatedBy = Environment.UserName
        };

        try
        {
            var id = await _context.AddAsync(setting);
            MessageBox.Show($"Inserted with ID {id}", "Inserted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error inserting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task UpdateSelectedAsync()
    {
        if (_grid.CurrentRow == null) { MessageBox.Show("Select a row first"); return; }
        var item = _grid.CurrentRow.DataBoundItem as ConfigSetting;
        if (item == null) return;

        item.FieldValue = item.FieldValue + "-updated";
        try
        {
            var ok = await _context.UpdateAsync(item);
            MessageBox.Show(ok ? "Updated" : "Update failed");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating: {ex.Message}", "Error");
        }
    }

    private async Task DeleteSelectedAsync()
    {
        if (_grid.CurrentRow == null) { MessageBox.Show("Select a row first"); return; }
        var item = _grid.CurrentRow.DataBoundItem as ConfigSetting;
        if (item == null) return;

        var confirm = MessageBox.Show($"Delete ID {item.ConfigSettingsId}?", "Confirm", MessageBoxButtons.YesNo);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var ok = await _context.DeleteAsync(item.ConfigSettingsId);
            MessageBox.Show(ok ? "Deleted" : "Delete failed");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting: {ex.Message}", "Error");
        }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {

    }
}
