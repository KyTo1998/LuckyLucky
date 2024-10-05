using OfficeOpenXml;
using System.IO;
using System.Windows;
namespace VongQuayMayMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Danh sách người chơi
        List<string> players = new List<string>();
        private Random random = new Random();
        private string ExcelFilePath;
        public MainWindow()
        {
            InitializeComponent();
            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "DanhSachNguoiChoi.xlsx";
            string filePath = Path.Combine(desktopPath, fileName);
            ExcelFilePath = filePath;
            
        }
       

        private string GeneratePlayerCode(string playerName)
        {
            string randomCode = random.Next(100000, 999999).ToString();
            return $"{playerName}_{randomCode}";
        }

        private void CreateExcelFile(string NamePlay)
        {
            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(ExcelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add(NamePlay);
                worksheet.Cells[1, 1].Value = NamePlay;
                package.Save();
            }
        }

        private void SaveParticipantToExcel(string participant)
        {
            using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(new FileInfo(ExcelFilePath)))
            {
                var worksheet = excelPackage.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                int rowCount = worksheet.Dimension.Rows; // Lấy số lượng dòng

                // Ghi dữ liệu vào file
                for (int i = 0; i < players.Count; i++)
                {
                    worksheet.Cells[rowCount + 1 + i, 1].Value = players; // Chỉ ghi tên vào cột đầu tiên
                }

                excelPackage.Save(); // Lưu thay đổi
            }
        }

        private void AddPlayerButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!File.Exists(ExcelFilePath))
            {
                CreateExcelFile(GeneratePlayerCode(PlayerNameTextBox.Text));
            }
            else
            {
                string playerName = PlayerNameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    string playerCode = GeneratePlayerCode(playerName);
                    PlayerNameTextBox.Clear();
                    SaveParticipantToExcel(playerCode);
                }
                else
                {
                    MessageBox.Show("Please enter a player name.");
                }
            }
        }

        private void SpinButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!File.Exists(ExcelFilePath))
            {
                MessageBox.Show("File không tồn tại. Vui lòng thêm người chơi trước.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Đọc dữ liệu từ file Excel
            using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(new FileInfo(ExcelFilePath)))
            {
                var worksheet = excelPackage.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                int rowCount = worksheet.Dimension.Rows; // Lấy số lượng dòng

                for (int i = 1; i <= rowCount; i++) // Bắt đầu từ dòng 2 để bỏ qua tiêu đề
                {
                    string name = worksheet.Cells[i, 1].Text; // Tên người chơi
                    //string code = worksheet.Cells[i, 2].Text; // Mã người chơi

                    if (!string.IsNullOrEmpty(name)) // Kiểm tra dữ liệu không trống
                    {
                        players.Add(name);
                    }
                }
            }

            // Kiểm tra nếu danh sách người chơi rỗng
            if (players.Count == 0)
            {
                MessageBox.Show("Không có người chơi nào trong danh sách.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Random hóa danh sách và chọn một người chơi ngẫu nhiên
            Random random = new Random();
            int winnerIndex = random.Next(players.Count);
            string winner = players[winnerIndex];
            //winnerLabel.Text = $"Winner: {winner}";
            // Hiển thị người chơi may mắn
            MessageBox.Show($"Người chơi may mắn:{winner}", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
