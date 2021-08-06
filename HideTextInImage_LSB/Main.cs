using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideTextInImage_LSB
{
    public partial class Main : Form
    {
            
        private string imageInputPath = ""; // đường link ảnh 
        private string imageOutput;
        public Main()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        //mỗi 8 bit sẽ là 1 byte 
        public static Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        public static string chuyenNhiPhan(int intValue)
        {
            //Chuyển sang hệ số 2 
            return Convert.ToString(intValue, 2).PadLeft(8, '0');
        }
        //Lúc này chưa ngh4i đến l1uc giải mã
        public static char[] charNhiPhanMessage(string message)
        {

            char[] charArray = null;
            string nhiphan = "";
            UnicodeEncoding unicode = new UnicodeEncoding();
            
            byte[] message1 = unicode.GetBytes(message);
            for (int i = 0; i < message.Length; i++)
            {
                if (i % 2 == 0)
                    nhiphan += chuyenNhiPhan(message[i]);
            }
            charArray = nhiphan.ToCharArray();
            return charArray;
        }
        //Này mới thật sự cho mã hóa lẫn gai3i mã
        //Lúc đầu cả text leng và text đều chuey63n dưới dạng nhị phân thoe assi nhưng ...
        //pah26n length của text sẽ chuyển về nhị phân theo chuẩn , còn phần text sẽ chuyển ra nhị phân theo assi , vì ne61u text leng > 9 sẽ là dãy 16bit kh còn 8 bit nữa , còn kia thì có thể lên tới số 256 ch3i với 8 bit 
        public static char[] charNhiPhanMessage_congThemLenght(string message)
        {
            char[] charArray = null;
            int doDaiText = message.Length;
            string nhiphan = "";
            UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] message1 = unicode.GetBytes(message);
            for (int i = 0; i < message1.Length; i++)
            {
                if (i % 2 == 0)
                    nhiphan += chuyenNhiPhan(message1[i]);
            }
            string doDaiTextNhiPhan = chuyenNhiPhan(doDaiText);
            string plus = doDaiTextNhiPhan + nhiphan;

            charArray = plus.ToCharArray();
            return charArray;
        }



        //Hỗ trợ cho tính kích thước bức ảnh 
        Image loadedTrueImage;
        private void button1_Click_1(object sender, EventArgs e)
        {
            

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Chose an Bitmap Image To Hiding";
            //openFileDialog1.Filter = "Bitmap Image(*.bmp)|*.bmp";
            openFileDialog1.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                

                imageInputPath = openFileDialog1.FileName; // lay ra duong dan cua file vua mo

                FileStream inStream = new FileStream(imageInputPath, FileMode.Open, FileAccess.Read);
                //Khi người dùng chọn ảnh ta sẽ kiểm tra liền đây có phải ảnh hợp yêu cầu kh (phải ảnh bmp và 24 kh?)
                //2 ký tự đầu của file bmp là chữ BM 
                char b = (char)inStream.ReadByte();
                char m = (char)inStream.ReadByte();

                inStream.Seek(28, 0);
                int soBit = (int)inStream.ReadByte();
                if (b != 'B' && m !='M')
                {
                    MessageBox.Show("Phải chọn ảnh có đuôi .bmp");
                }
                //ở byte thứ 28 phần header bmp chính là hiển thị số bit của 
                else if(soBit != 24)
                {
                    MessageBox.Show("file .bmp phải có số bit là 24bit");
                }
                else
                {
                    inStream.Seek(0, 0); // trả về vịu trí ban đầu
                    txtPath.Text = openFileDialog1.FileName;

                    loadedTrueImage = Image.FromFile(imageInputPath);
                    picBoxGoc.Image = new Bitmap(imageInputPath);
                    double a = (double)(loadedTrueImage.Height * loadedTrueImage.Width * 24) / (8 * 1024);
                    lbKichThuoc.Text = a.ToString("####0.00") + " KB"; lbChieuCao.Text = loadedTrueImage.Height.ToString() + " Pixel";
                    lbChieuRong.Text = loadedTrueImage.Width.ToString() + " Pixel";

                    int totalPaddingBytes = (4 - ((loadedTrueImage.Width * 3) % 4)) * loadedTrueImage.Height; // tổng tất cả các byte padding trong 1 bức ảnh

                    //-1 là vì mình có chứa thêm 1 byte độ dài text ở đầu 
                    /// 8 vì 8 byte ảnh chỉ chứa đc 1 chữ 
                    lbKyTu.Text =  (((inStream.Length - 54  - totalPaddingBytes) / 8) -1 ) .ToString();
                    txtMessage.MaxLength = int.Parse(lbKyTu.Text);
                }
            }
            else
            {
                imageInputPath = "";
            }
        
        }

        private void btnMaHoa_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(imageInputPath))
                MessageBox.Show("Bạn chưa chọn ảnh để mã hóa");
           else if(string.IsNullOrEmpty(txtMessage.Text))
                MessageBox.Show("Bạn chưa nhập thông điệp để mã hóa");
            else
            {
                SaveFileDialog saveDialog1 = new SaveFileDialog();
                saveDialog1.Title = "Where do you want to save the file?";
                saveDialog1.Filter = "Bitmap (*.bmp)|*.bmp";

                if (saveDialog1.ShowDialog() == DialogResult.OK)
                {
                    Bitmap img = new Bitmap(imageInputPath);
                    using (FileStream inStream = new FileStream(imageInputPath, FileMode.Open, FileAccess.Read))
                    {
                        //FileStream inStream = new FileStream(imageInputPath, FileMode.Open, FileAccess.Read);
                        // char[] charArray = charNhiPhanMessage(txtMessage.Text);


                        //Dùng bitmask để mã hóa - mình sẽ chuyển keytext đây sang nhị phân rồi lấy từng bit đó xor với các từg bit trong mess (vì mặc định key đây là 1 byte 8 bit nên mỗi lần = 8 sẽ chuey63n biến đếm về 0)
                        char[] charArray0 = charNhiPhanMessage_congThemLenght(txtMessage.Text);
                        char[] key = charNhiPhanMessage(txtMatKhau.Text);
                        string sKey = MaHoa.Ma_Hoa(charArray0, key);
                        
                        char[] charArray = sKey.ToCharArray();
                        //MessageBox.Show("text: " + biena +"\n" +"key : "+bienb +"\n" + "kq: "+sKey );


                        //////////
                        int totalbyte_exceptPadding = Convert.ToInt32(((24 * loadedTrueImage.Width) / 32.0) * 4); // tổng các byte đã ngoại trừ bye padding , chỉ còn các bye RGB
                        int totalPaddingBytesPerRow = (4 - ((loadedTrueImage.Width * 3) % 4)); // tổng tất cả các byte padding trong 1 dòng
                        //int totalPaddingBytes = (4 - ((loadedTrueImage.Width * 3) % 4)) * loadedTrueImage.Height; // tổng tất cả các byte padding trong 1 bức ảnh 
                        int totalPaddingBytes = totalPaddingBytesPerRow * loadedTrueImage.Height; // tổng tất cả các byte padding trong 1 bức ảnh 
                        int total = totalbyte_exceptPadding + totalPaddingBytesPerRow;
                        //Tính số lần lặp
                        int solanlap = 1;
                        int solanchay = 0;
                        //-54 là số byte headerfile ,  , -totalPaddingbytes là trừ đi tổng số byte padding kh thuộc RGB
                        if ((charArray.Length) > inStream.Length - 54  - totalPaddingBytes)
                        {
                            
                                    MessageBox.Show("Lưu thất bại - Thông điệp quá dài");
                                
                        }
                        else
                        {

                            imageOutput = saveDialog1.FileName;
                            txtOutput.Text = imageOutput;
                            inStream.Seek(0, 0);
                            //đầu tiên mình sẽ đọc 54 byte đầu của đầu vào gán vô 54byte đầu của đầu ra 
                            int offset = 54;
                            byte[] header = new byte[offset];
                            inStream.Read(header, 0, offset);
                            FileStream outStream = new FileStream(imageOutput, FileMode.Create, FileAccess.Write);
                            outStream.Write(header, 0, offset);

                            // bắt đầu mã  hóa
                            int byteRead;
                            byte byteWrite;
                            inStream.Seek(offset, 0);
                            //54 là bắt đầu đọc các pixel trong ảnh, 54 trở lên là header k đc đụng vào
                            outStream.Seek(offset, 0);


                            if ((img.Width * 3 % 4) != 0)// ảnh có độ dài lẻ nên ta phải dùng thuật toán để bỏ đi những byte padding chỉ lấy màu thôi
                            {
                               // int gh = (int)inStream.Length - 54 - 2 - totalPaddingBytes;
                                for (int i = 0; i < inStream.Length - 54  - totalPaddingBytes; i++)
                                {
                                    byteRead = inStream.ReadByte();
                                    int b = byteRead;
                                    int bitnhonhat = b & 1;
                                    int sodasua = 0;
                                    //nấu mà chạy hết các bit của tin nhắn thì ta in ra nhưng gia trị còn lại thôi
                                    if (solanchay == charArray.Length)
                                    {
                                        outStream.WriteByte((byte)b);

                                    }
                                    else
                                    {
                                        //mình sẽ kiểm tra bit của message đã đổi ra thành nhị phân , so sánh với bit nhỏ nhất của byte hiện tại, nếu giống nhau thì viết lại nếu kh mình sẽ dùng bitswise để đổi bit cuối cùng 
                                        if (int.Parse(charArray[solanchay].ToString()) == bitnhonhat)
                                        {
                                            outStream.WriteByte((byte)b);

                                            solanchay++;
                                        }
                                        else
                                        {
                                            sodasua = b ^ 1;
                                            outStream.WriteByte((byte)sodasua);

                                            solanchay++;
                                        }
                                    }
                                    //Console.WriteLine(chuyenNhiPhan(byteRead));
                                    //Console.WriteLine(byteRead);

                                    if (solanlap == totalbyte_exceptPadding)
                                    {
                                        //ta sẽ đọc những padding bytes khi con trỏ đọc đến tổng các byte rgb 1 hàng
                                        for (int x = 0; x < totalPaddingBytesPerRow; x++)
                                        {
                                            outStream.WriteByte((byte)inStream.ReadByte());
                                        }

                                        //cộng 2 total lại t sẽ đc tổng số byte trên cùng 1 hàng, từ đó ta có thể bỏ nhưng byte padding ở cuối hàng
                                        inStream.Seek(offset += total, 0);

                                        solanlap = 0;
                                    }
                                    solanlap++;
                                } 
                                //outStream.WriteByte(0);
                                //outStream.WriteByte(0);

                                
                                inStream.Close(); // dong file anh dau vao
                                outStream.Close();
                                picBoxMaHoa.Image = new Bitmap(txtOutput.Text);
                            }
                            else
                            {
                                for (int i = 0; i < inStream.Length - 54 ; i++)
                                {
                                    byteRead = inStream.ReadByte();
                                    int b = byteRead;
                                    int bitnhonhat = b & 1;
                                    int sodasua = 0;
                                    string ketqua = "";

                                    //nấu mà chạy hết các bit của tin nhắn thì ta in ra nhưng gia trị còn lại thôi
                                    if (solanchay == charArray.Length)
                                    {
                                        outStream.WriteByte((byte)b);

                                    }
                                    else
                                    {
                                        if (int.Parse(charArray[solanchay].ToString()) == bitnhonhat)
                                        {
                                            outStream.WriteByte((byte)b);
                                            //ketqua += b;
                                            solanchay++;
                                        }
                                        else
                                        {
                                            sodasua = b ^ 1;
                                            outStream.WriteByte((byte)sodasua);
                                            //ketqua += sodasua;
                                            solanchay++;
                                        }
                                    }
                                    // Console.WriteLine(chuyenNhiPhan(byteRead));
                                    //Console.WriteLine(byteRead);

                                }
                                //outStream.WriteByte(0);
                                //outStream.WriteByte(0);


                                //
                                inStream.Close(); // dong file anh dau vao
                                outStream.Close();
                                picBoxMaHoa.Image = new Bitmap(txtOutput.Text);
                            }
                        }
                    }
                    
                }
                else
                    {
                        MessageBox.Show("Chọn nơi để lưu kq ảnh");
                    }

            }
        }

        string imageSauMaHoaPath = "";

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Chose an Bitmap Image To Hiding";
            //openFileDialog1.Filter = "Bitmap Image(*.bmp)|*.bmp";
            openFileDialog1.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imageSauMaHoaPath = openFileDialog1.FileName; // lay ra duong dan cua file vua mo

                FileStream inStream = new FileStream(imageSauMaHoaPath, FileMode.Open, FileAccess.Read);
                //Khi người dùng chọn ảnh ta sẽ kiểm tra liền đây có phải ảnh hợp yêu cầu kh (phải ảnh bmp và 24 kh?)
                //2 ký tự đầu của file bmp là chữ BM 
                char b = (char)inStream.ReadByte();
                char m = (char)inStream.ReadByte();

                inStream.Seek(28, 0);
                int soBit = (int)inStream.ReadByte();
                if (b != 'B' && m != 'M')
                {
                    MessageBox.Show("Phải chọn ảnh có đuôi .bmp");
                }
                //ở byte thứ 28 phần header bmp chính là hiển thị số bit của 
                else if (soBit != 24)
                {
                    MessageBox.Show("file .bmp phải có số bit là 24bit");
                }
                else
                {
                    inStream.Seek(0, 0); // trả về vịu trí ban đầu
                    
                    loadedTrueImage = Image.FromFile(imageSauMaHoaPath);
                    picSauMaHoa.Image = new Bitmap(imageSauMaHoaPath);
                    double a = (double)(loadedTrueImage.Height * loadedTrueImage.Width * 24) / (8 * 1024);
                    labelKichthuoc.Text = a.ToString("####0.00") + " KB"; 
                    labelChieuCao.Text = loadedTrueImage.Height.ToString() + " Pixel";
                    labelChieuRong.Text = loadedTrueImage.Width.ToString() + " Pixel";
                }
            }
            else
            {
                imageSauMaHoaPath = "";
            }
        }

        string numbermahoa = "";
        string ketquacuoicung = "";
        private void btnGiaiMa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imageSauMaHoaPath))
                MessageBox.Show("Bạn chưa chọn ảnh để mã hóa");
            else
            {
                int offset = 54;
                using (FileStream inStream = new FileStream(imageSauMaHoaPath, FileMode.Open, FileAccess.Read))
                {
                    Bitmap img123 = new Bitmap(imageSauMaHoaPath);
                    inStream.Seek(offset, 0);
                    // đọc 8 byte đ6àu và lấy bit cuối cùng của 8 byte đó chuyển sang decimal sẽ ra lengtext, để bik số lần lặp ra kq
                    for (int i = 0; i < 8; i++)
                    {
                        int byteRead1 = inStream.ReadByte();
                        int b = byteRead1;
                        int bitnhonhat = b & 1;
                        numbermahoa += bitnhonhat;
                    }

                    ///Gia3i mã
                    char[] charnhiphan = numbermahoa.ToCharArray();
                    char[] key1 = charNhiPhanMessage(txtKey.Text);
                    string sKey2 = MaHoa.Giai_Ma(charnhiphan, key1);

                    //Chuyển v6e2 dạng decimal bình thg 
                    var data = Convert.ToInt32(sKey2, 2).ToString();
                    int num = int.Parse(data);

                    numbermahoa = ""; //cho thành rỗng vì mình phải set v để chỉ chừa 2 chữ thui 

                    if ((img123.Width * 3 % 4) != 0)// ảnh có độ dài lẻ nên ta phải dùng thuật toán để bỏ đi những byte padding chỉ lấy màu thôi
                    {
                        int totalbyte_exceptPadding1 = Convert.ToInt32(((24 * img123.Width) / 32.0) * 4); // tổng các byte đã ngoại trừ bye padding , chỉ còn các bye RGB
                        int totalPaddingBytesPerRow1 = (4 - ((img123.Width * 3) % 4)); // tổng tất cả các byte padding trong 1 dòng
                        int totalPaddingBytes1 = (4 - ((img123.Width * 3) % 4)) * img123.Height; // tổng tất cả các byte padding trong 1 bức ảnh 
                        int total1 = totalbyte_exceptPadding1 + totalPaddingBytesPerRow1;
                        //Tính số lần lặp
                        int solanlap1 = 9;
                        int solanchay1 = 0;
                        inStream.Seek(offset + 8, 0);
                        for (int i = 0; i < num * 8; i++)
                        {
                            int byteRead1 = inStream.ReadByte();
                            int b = byteRead1;
                            int bitnhonhat = b & 1;
                            numbermahoa += bitnhonhat;

                            if (solanlap1 == totalbyte_exceptPadding1)
                            {
                                //cộng 2 total lại t sẽ đc tổng số byte trên cùng 1 hàng, từ đó ta có thể bỏ nhưng byte padding ở cuối hàng
                                inStream.Seek(offset += total1, 0);
                                solanlap1 = 0;
                            }
                            solanlap1++;
                        }

                        char[] charArray = numbermahoa.ToCharArray();
                        char[] key = charNhiPhanMessage(txtKey.Text);
                        string sKey = MaHoa.Giai_Ma(charArray, key);


                        UnicodeEncoding unicode = new UnicodeEncoding();
                         unicode.GetBytes(sKey);

                        string result = System.Text.Encoding.UTF8.GetString(GetBytesFromBinaryString(sKey));
                        MessageBox.Show(result);

                        var data0 = GetBytesFromBinaryString(sKey);
                        txtKetQua.Text = Encoding.ASCII.GetString(data0);
                    }
                    else
                    {
                        //bỏ 8 byte đầu tiên
                        inStream.Seek(offset + 8, 0);
                        for (int i = 0; i < num * 8; i++)
                        {
                            int byteRead1 = inStream.ReadByte();
                            int b = byteRead1;
                            int bitnhonhat = b & 1;
                            numbermahoa += bitnhonhat;
                        }
                        char[] charArray = numbermahoa.ToCharArray();
                        char[] key = charNhiPhanMessage(txtKey.Text);
                        string sKey = MaHoa.Giai_Ma(charArray, key);

                        var data0 = GetBytesFromBinaryString(sKey);
                        txtKetQua.Text = Encoding.ASCII.GetString(data0);
                    }

                   
                }
             }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
           

        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
           
        }


        public void Reset()
        {
            txtMessage.Text = "";
            txtMatKhau.Text = "";

            lbChieuCao.Text = "x";
            lbChieuRong.Text = "x";
            lbKichThuoc.Text = "x";
            lbKyTu.Text = "x";

            txtPath.Text = "";
            txtOutput.Text = "";

            picBoxGoc.Image = null;
            picBoxMaHoa.Image = null;
            picSauMaHoa.Image = null;

            labelKichthuoc.Text = "x";
            labelChieuCao.Text =  "x";
            labelChieuRong.Text = "x";
            txtKetQua.Text = "";
            txtKey.Text = "";
            imageSauMaHoaPath = "";
            numbermahoa = "";
            imageInputPath = "";
            imageOutput = "";
           
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
                
        }

        private void txtResetGiaiMa_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
