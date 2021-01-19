using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Media;

namespace Team31337_Server
{
    public class SoundClass
    {
        Boolean isTalking = false;

        SoundPlayer sp1 = new SoundPlayer(@"C:\31337\voice\1.wav");
        SoundPlayer sp2 = new SoundPlayer(@"C:\31337\voice\2.wav");
        SoundPlayer sp3 = new SoundPlayer(@"C:\31337\voice\3.wav");
        SoundPlayer sp4 = new SoundPlayer(@"C:\31337\voice\4.wav");
        SoundPlayer sp5 = new SoundPlayer(@"C:\31337\voice\5.wav");
        SoundPlayer sp6 = new SoundPlayer(@"C:\31337\voice\6.wav");
        SoundPlayer sp7 = new SoundPlayer(@"C:\31337\voice\7.wav");
        SoundPlayer sp8 = new SoundPlayer(@"C:\31337\voice\8.wav");
        SoundPlayer sp9 = new SoundPlayer(@"C:\31337\voice\9.wav");
        SoundPlayer sp10 = new SoundPlayer(@"C:\31337\voice\10.wav");
        SoundPlayer sp20 = new SoundPlayer(@"C:\31337\voice\20.wav");
        SoundPlayer sp30 = new SoundPlayer(@"C:\31337\voice\30.wav");
        SoundPlayer spLogin = new SoundPlayer(@"C:\31337\voice\사용을 시작합니다.wav");
        SoundPlayer spLogout = new SoundPlayer(@"C:\31337\voice\로그아웃하였습니다.wav");
        SoundPlayer spBeep = new SoundPlayer(@"C:\31337\voice\Beep.wav");
        SoundPlayer spDing = new SoundPlayer(@"C:\31337\voice\Ding.wav");
        SoundPlayer spOrder = new SoundPlayer(@"C:\31337\voice\주문 도착 효과음.wav");

        public SoundPlayer spMsgRcv = new SoundPlayer(@"C:\31337\voice\메시지 도착 효과음.wav");

        public void Beep()
        {
            spBeep.Play();
        }
        
        public void Ding()
        {
            spDing.Play();
        }

        public void OrderSound()
        {
            spOrder.Play();
        }

        public void PlaySoundClientLogin(String num)
        {
            if (isTalking == false)
            {
                isTalking = true;
                PlaySoundOfClientNum(num);
                spLogin.PlaySync();
                isTalking = false;
            }
        }

        public void PlaySoundClientLogout(String num)
        {
            if (isTalking == false)
            {
                isTalking = true;
                PlaySoundOfClientNum(num);
                spLogout.PlaySync();
                isTalking = false;
            }
        }

        public void PlaySoundOfClientNum(String num)
        {
            try
            {
                if (num.Substring(0, 2) != null) //자릿수가 두 자릿수인 경우
                {
                    Type tp = typeof(SoundPlayer);
                    Type a = typeof(SoundClass);
                    MethodInfo method = tp.GetMethod("PlaySync");

                    FieldInfo fld = a.GetField("sp" + Convert.ToString((Convert.ToInt32(num.Substring(0, 1)) * 10)), BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public |
                                                 BindingFlags.NonPublic);
                    object point = fld.GetValue(this);
                    method.Invoke(point, new object[] { });

                    if (num.Substring(1, 1) != "0")
                    {
                        fld = a.GetField("sp" + num.Substring(1, 1), BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public |
                                                 BindingFlags.NonPublic);
                        point = fld.GetValue(this);
                        method.Invoke(point, new object[] { });
                    }
                }
            }
            catch
            {
                if (num.Substring(0, 1) != null) //자릿수가 한 자릿수인 경우
                {
                    Type tp = typeof(SoundPlayer);
                    Type a = typeof(SoundClass);
                    MethodInfo method = tp.GetMethod("PlaySync");

                    FieldInfo fld = a.GetField("sp" + num, BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public |
                                                 BindingFlags.NonPublic);
                    object point = fld.GetValue(this);

                    method.Invoke(point, new object[] { });
                }
            }
        }
    }
}
