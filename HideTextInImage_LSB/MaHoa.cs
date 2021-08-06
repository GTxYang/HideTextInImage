using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideTextInImage_LSB
{
    class MaHoa
    {


        //Dùng bitmask để mã hóa - mình sẽ chuyển keytext đây sang nhị phân rồi lấy từng bit đó xor với các từg bit trong mess (vì mặc định key đây là 1 byte 8 bit nên mỗi lần = 8 sẽ chuey63n biến đếm về 0)

        public static string XOR(char[] cNhiPhan , char[] cKey)
        {
            string sKey = "";
            int biendem = 0;
            for(int i =0; i< cNhiPhan.Length; i++)
            {
                if(biendem % 8 == 0)
                {
                    biendem = 0;
                }
                int a = cKey[biendem];
                int b = cNhiPhan[i];
                sKey += int.Parse((a^b).ToString());
                biendem++;
            }
            return sKey;
        }



        public static string Giai_Ma(char[] cNhiPhan, char[] cKey)
        {
            return XOR(cNhiPhan, cKey);
        }

        public static string Ma_Hoa(char[] cNhiPhan, char[] cKey)
        {
            return XOR(cNhiPhan, cKey);
        }



    }
}
