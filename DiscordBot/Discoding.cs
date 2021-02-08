using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Google.Apis;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Discord.Audio;
using NAudio.Wave;
using VideoLibrary;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

    

namespace DiscordBot
{
    class Program
    {
       

        DiscordSocketClient client; //봇 클라이언트
        CommandService commands;    //명령어 수신 클라이언트
        /// <summary>
        /// 프로그램의 진입점
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {
            new Program().BotMain().GetAwaiter().GetResult();   //봇의 진입점 실행
        }

        /// <summary>
        /// 봇의 진입점, 봇의 거의 모든 작업이 비동기로 작동되기 때문에 비동기 함수로 생성해야 함
        /// </summary>
        /// <returns></returns>
        public async Task BotMain()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {    //디스코드 봇 초기화
                LogLevel = LogSeverity.Verbose                              //봇의 로그 레벨 설정 
            });
            commands = new CommandService(new CommandServiceConfig()        //명령어 수신 클라이언트 초기화
            {
                LogLevel = LogSeverity.Verbose                              //봇의 로그 레벨 설정
            });

            //로그 수신 시 로그 출력 함수에서 출력되도록 설정
            client.Log += OnClientLogReceived;
            commands.Log += OnClientLogReceived;

            await client.LoginAsync(TokenType.Bot, "ODAzMDU2ODEzNjMyMTI2OTg3.YA4O8A.gZU-8nDvkIoXiT-pYHd-l6OpwzE"); //봇의 토큰을 사용해 서버에 로그인
            await client.StartAsync();                         //봇이 이벤트를 수신하기 시작

            client.MessageReceived += OnClientMessage;         //봇이 메시지를 수신할 때 처리하도록 설정
            // 봇에 명령어 모듈 등
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);  //봇에 명령어 모듈 등록

            await Task.Delay(-1);   //봇이 종료되지 않도록 블로킹
        }

        private async Task OnClientMessage(SocketMessage arg)
        {
            //수신한 메시지가 사용자가 보낸 게 아닐 때 취소
            var message = arg as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int pos = 0;

            //메시지 앞에 !이 달려있지 않고, 자신이 호출된게 아니거나 다른 봇이 호출했다면 취소
            if (!(message.HasCharPrefix('!', ref pos) || message.HasMentionPrefix(client.CurrentUser, ref pos)) || message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(client, message);                    //수신된 메시지에 대한 컨텍스트 생성   

            var result = await commands.ExecuteAsync(context: context, argPos: pos, services: null);
        }
        
        
        /// <summary>
        /// 봇의 로그를 출력하는 함수
        /// </summary>
        /// <param name="msg">봇의 클라이언트에서 수신된 로그</param>
        /// <returns></returns>
        private Task OnClientLogReceived(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());  //로그 출력
            return Task.CompletedTask;
        }
        public class Command_One : ModuleBase<SocketCommandContext> //!Hi 커맨ㄷ
        {
            [Command("hi")]
            [Alias("안녕", "ㅎㅇ", "Hello", "안뇽")] 
            public async Task HelloCommand()
            {
                await Context.Channel.SendMessageAsync("안녕하세요!");
            }

        }
        public class Command_Two : ModuleBase<SocketCommandContext>
        {
            [Command("language")]
            [Alias("코딩 언어", "프로그래밍언어", "프로그래밍 언어")]
            public async Task languageCommand()
            {
                
                await Context.Channel.SendMessageAsync("언어들");
                EmbedBuilder eb = new EmbedBuilder();
                eb.Color = Color.Red;
                eb.Title = "프로그래밍 언어(이 서버에서 얘기할수 있음)";
                eb.Description = "이 서버에서 토크&프로젝트를 올릴수 있는 언어들 입니다.(신청하면 목록에 추가됨)";
                eb.AddField("1.C#", "Unity,.NET CORE, WPF, Blazor, DiscordBot");
                eb.AddField("2.Python", "모든것을 토킹할수 있음");
                eb.AddField("3.Java,Kotlin", "JavaFX,Android,plugin");
                eb.AddField("4.함수형 프로그래밍 언어", "(go,scala)모든것");
                eb.AddField("5.Rust,C,C++", "임베디드,최적화,언리얼 엔진,시스템 프로그래밍, 보안");
                eb.AddField("6.Dart", "Flutter");
                eb.AddField("7.웹 표준", "JS,TS,HTML,CSS(JS,TS는 프레임 워크 얘기도 가능)");
                await Context.Channel.SendMessageAsync("", false, eb.Build());
            }
        }
        public class Command_Three : ModuleBase<SocketCommandContext>
        {
            [Command("Who are you?")]
            [Alias("넌 누구니?", "BotINFO", "botInfo", "BotInfo", "안녕 로봇")]
            public async Task WhoAreYou()
            {
                await Context.Channel.SendMessageAsync("삐빕!저는 메인 도우미 봇 입니다!");
            }
        }
        public class Command_Four : ModuleBase<SocketCommandContext>
        {
            [Command("Bot List")]
            [Alias("봇 리스트", "bot list", "botList", "BotList", "봇리스트")]
            public async Task BotList()
            {
                await Context.Channel.SendMessageAsync("MainManager,SubManager");
                EmbedBuilder eb = new EmbedBuilder();
                eb.Color = Color.Red;
                eb.Title = "봇 리스트";
                eb.Description = "유저를 도와주는 봇들 리스트";
                eb.AddField("1.MainManager", "현업", true);
                eb.AddField("2.SubManager", "곧 추가됨", true);
                await Context.Channel.SendMessageAsync("", false, eb.Build());
            }
        }
        public class Command_Five : ModuleBase<SocketCommandContext>
        {
            [Command("나라")]
            [Alias("언어", "국가", "Country", "country")]
            public async Task 나라()
            {
                await Context.Channel.SendMessageAsync("나라");
                EmbedBuilder eb = new EmbedBuilder();
                eb.Color = Color.Blue;
                eb.Title = "나라And언어";
                eb.Description = "나라와 언어";
                eb.AddField("나라:Korea", "아시아에 위치한 나라", true);
                eb.AddField("언어:한국어", "한국의 언어", true);
                await Context.Channel.SendMessageAsync("", false, eb.Build());
            }
        }
        public class Command_Six : ModuleBase<SocketCommandContext>
        {
            
            Random rdm1 = new Random();
            Random rdm2 = new Random();
            long[] rdm_final = new long[2];
            [Command("주사위")]
            [Alias("랜덤", "Dice", "dice")]
            public async Task RandomDice()
            {
                {
                    rdm_final[0] = rdm1.Next(6) + 1; // 사용자의 값
                    rdm_final[1] = rdm2.Next(6) + 1; // 컴퓨터의 값
                    if (rdm_final[0] > rdm_final[1])
                    {
                        await Context.Channel.SendMessageAsync("플레이어 Win!");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Orange;
                        eb.Title = "Player Win!";
                        eb.Description = "플레이어가 컴퓨터를 이겼습니다!";
                        eb.AddField($"Player", $"{rdm_final[0]}", true);
                        eb.AddField("Computer", $"{rdm_final[1]}", true);
                        this.rdm_final[0] = rdm1.Next(6) + 1;
                        this.rdm_final[1] = rdm2.Next(6) + 1;
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                        
                    }
                    if (rdm_final[0] < rdm_final[1])
                    {
                        await Context.Channel.SendMessageAsync("플레이어 Lose...");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Red;
                        eb.Title = "Player Lose";
                        eb.Description = "플레이어가 컴퓨터에게 졌습니다.";
                        eb.AddField("Player", $"{rdm_final[0]}", true);
                        eb.AddField("Computer", $"{rdm_final[1]}", true);
                        this.rdm_final[0] = rdm1.Next(6) + 1;
                        this.rdm_final[1] = rdm2.Next(6) + 1;
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }
                    if (rdm_final[0] == rdm_final[1])
                    {
                        await Context.Channel.SendMessageAsync("Vi겼습니다");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Blue;
                        eb.Title = "Tie";
                        eb.Description = "Player와 Computer는 비겼습니다.";
                        eb.AddField("Player", $"{rdm_final[0]}", true);
                        eb.AddField("Computer", $"{rdm_final[1]}", true);
                        this.rdm_final[0] = rdm1.Next(6) + 1;
                        this.rdm_final[1] = rdm2.Next(6) + 1;
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }

                }

            }
        }
        public class CommandSeven : ModuleBase<SocketCommandContext>
        {
            List<string> sungu = new List<string>();
            [Command("투표")]
            [Alias("poll", "Poll", "투표한다", " 투표", "toopyo")]
            public async Task Minjoo(string title, string b, string c)
            {
                try
                {
                    int soo = 1;
                    if (title != null)
                    {
                        Console.WriteLine("통과");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("타이틀을 입력해 주십시오.");
                        soo = 0;
                    }
                    if (b != null)
                    {
                        sungu.Add(b);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("투표값을 입력해 주십시오.");
                        soo = 0;
                    }
                    if (c != null)
                    {
                        sungu.Add(c);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("투표 값을 입력해 주십시오");
                        soo = 0;
                    }

                    if (soo == 1)
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Title = $"{title}";
                        eb.Color = Color.Blue;
                        eb.Description = $"{b} or {c}";
                        eb.AddField($"1.{b}", $"투표하기 {b}", true);
                        eb.AddField($"2.{c}", $"투표하기 {c}", true);
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                        
                    }
                    if (soo == 0)
                    {
                        await Context.Channel.SendMessageAsync("입력값을 제데로 입력해 주세요");
                    }
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("삐빅 오류가 났습니다!");
                }
            }
        }
        public class Command_Eight : ModuleBase<SocketCommandContext>
        {
            [Command("유튜브 검색")]
            [Alias("YT검색", "Youtube검색", "유튭검색", "Youtube Search", "youtube검색", "youtube search")]
            public async Task YTSearch(string sech)
            {
                if (sech == null)
                {
                    await Context.Channel.SendMessageAsync("검색할 값을 입력해 주십시오");
                }
                string Search = Regex.Replace(sech, @"\s", "");
                await Context.Channel.SendMessageAsync($"https://www.youtube.com/results?search_query=" + $"{Search}");
            }
        }
        public class Command_Nine : ModuleBase<SocketCommandContext>
        {
            [Command("구글 검색&네이버 검색")]
            [Alias("Search by Google", "search by google", "search by Google", "Search Google", "Search google", "Search by naver", "Search by Naver", "search by naver", "Search by Naver&Google", "네이버검색", "네이버 검색", "Naver 검색", "naver 검색", "구글 검색", "google 검색", "Google 검색", "검색")]
            public async Task SearchAll(string sechAll)
            {
                if (sechAll == null)
                {
                    await Context.Channel.SendMessageAsync("검색할 값을 입력해 주십시오");
                }
                string Search = Regex.Replace(sechAll, @"\s", "");
                await Context.Channel.SendMessageAsync("https://www.google.com/search?q=" + $"{Search}");
                await Context.Channel.SendMessageAsync("https://m.search.naver.com/search.naver?sm=mtb_hty.top&where=m&oquery=%EB%84%A4%EC%9D%B4%EB%B2%84&tqi=ht6IFdp0Jxossl%2BkPhlsssssty8-234953&query=" + $"{Search}");
            }
        }
        public class Command_Ten : ModuleBase<SocketCommandContext>
        {
            [Command("평균")]
            [Alias("센터", "중간")]
            public async Task Centor(params double[] centor)
            {
                int a = 0;
                string b = "d";
                if (centor.GetType() == a.GetType() || centor.GetType() == b.GetType())
                {
                    await Context.Channel.SendMessageAsync("정수를 입력하세요.");
                }
                double sum = 0;
                List<double> list = new List<double>();
                double plus = 0;
                for (int i = 0; i < centor.Length; ++i)
                {
                    if (centor[i] == 0) await Context.Channel.SendMessageAsync("0은 취급하지 않습니다.");
                    list.Add(centor[i]);
                    plus += centor[i];
                    ++sum;
                }
                double awnser = plus / sum;
                await Context.Channel.SendMessageAsync($"평균 값: {awnser}");
            }
        }
        public class Command_Eleven : ModuleBase<SocketCommandContext>
        {
            [Command("번역")]
            [Alias("파파고", "네이버 번역")]
            public async Task 번역(string changeFirst)
            {
                try
                {
                    string sUrl = "https://openapi.naver.com/v1/papago/n2mt";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                    // 헤더 추가하기 (파파고 NMT API 가이드에서 -h 부분이 헤더이다)
                    request.Headers.Add("X-Naver-Client-Id", "1YD9BdOF4mYi8fWH3Nhv");
                    request.Headers.Add("X-Naver-Client-Secret", "7a42oYn5uT");
                    request.Method = "POST";

                    // 파라미터에 값 넣기 (파파고 NMT API가이드에서 -d부분이 파라미터)
                    //string sParam = string.Format("source=auto&target=en&text="+txtSendText.Text);

                    string Original_string = $"{changeFirst}"; // 번역하고싶은 데이터

                    // 파라미터를 char Set에 맞게 변경
                    byte[] bytearry = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + Original_string);

                    request.ContentType = "application/x-www-form-urlencoded";

                    // 요청 데이터 길이
                    request.ContentLength = bytearry.Length;

                    Stream st = request.GetRequestStream();
                    st.Write(bytearry, 0, bytearry.Length);
                    st.Close();

                    // 응답 데이터 가져오기 (출력포맷)
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string text = reader.ReadToEnd();

                    stream.Close();
                    response.Close();
                    reader.Close();
                    JObject jObject = JObject.Parse(text);
                    await Context.Channel.SendMessageAsync($"번역 된 값 : " + $"{jObject["message"]["result"]["translatedText"].ToString()}"); // 결과 출력
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync("에러!!");
                }
            }
        }
        public class Command_Twelve : ModuleBase<SocketCommandContext>
        {
            [Command("역번역")]
            [Alias("거꾸로 번역", "거꾸로번역", "역 파파고", "역파파고", "거꾸로 파파고", "거꾸로 파파고")]
            public async Task 역번역(string downPapago)
            {
                try
                {
                    string sUrl = "https://openapi.naver.com/v1/papago/n2mt";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                    // 헤더 추가하기 (파파고 NMT API 가이드에서 -h 부분이 헤더이다)
                    request.Headers.Add("X-Naver-Client-Id", "1YD9BdOF4mYi8fWH3Nhv");
                    request.Headers.Add("X-Naver-Client-Secret", "7a42oYn5uT");
                    request.Method = "POST";

                    // 파라미터에 값 넣기 (파파고 NMT API가이드에서 -d부분이 파라미터)
                    //string sParam = string.Format("source=auto&target=en&text="+txtSendText.Text);

                    string Original_string = $"{downPapago}"; // 번역하고싶은 데이터

                    // 파라미터를 char Set에 맞게 변경
                    byte[] bytearry = Encoding.UTF8.GetBytes("source=ko&target=en&text=" + Original_string);

                    request.ContentType = "application/x-www-form-urlencoded";

                    // 요청 데이터 길이
                    request.ContentLength = bytearry.Length;

                    Stream st = request.GetRequestStream();
                    st.Write(bytearry, 0, bytearry.Length);
                    st.Close();

                    // 응답 데이터 가져오기 (출력포맷)
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string text = reader.ReadToEnd();

                    stream.Close();
                    response.Close();
                    reader.Close();
                    JObject jObject = JObject.Parse(text);
                    await Context.Channel.SendMessageAsync($"번역 된 값 : " + $"{jObject["message"]["result"]["translatedText"].ToString()}");
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync("에러!!");
                }
            }
        }
        public class Command_Thirteen : ModuleBase<SocketCommandContext>
        {
            [Command("릴리즈")]
            [Alias("프로젝트 릴리즈", "Realess")]
            public async Task Realess(string Name, string WriteThisProject, double version, string Jongryu, string lang, string link)
            {
                try
                {
                    await Context.Channel.SendMessageAsync($"{Name}");
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.Title = $"{Name}";
                    eb.Color = Color.Red;
                    eb.Description = $"{WriteThisProject}";
                    eb.AddField("버전", $"v{version}", true);
                    eb.AddField("종류", $"{Jongryu}", true);
                    eb.AddField("개발 언어", $"{lang}");
                    eb.AddField("링크", $"{link}");
                    await Context.Channel.SendMessageAsync("", false, eb.Build());
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("Error!");
                }
            }
        }
        public class Command_Forteen : ModuleBase<SocketCommandContext>
        {
            [Command("사용법")]
            [Alias("봇 사용법", "봇사용법", "What are bots can do?", "기능")]
            public async Task VangVub()
            {
                await Context.Channel.SendMessageAsync("봇 사용법");
                EmbedBuilder eb = new EmbedBuilder();
                eb.Title = "봇 사용법";
                eb.Color = Color.Blue;
                eb.Description = "봇 명령어의 종류와 사용법";
                eb.AddField("1.!hi", "답변:Hello", true);
                eb.AddField("2.!코딩 언어", "이 서버에서 얘기할수 있는 프로그래밍 언어들을 보여줌", true);
                eb.AddField("3.!넌 누구니&!봇 리스트", "두 명령어 다 봇에 정보를 보여줌", true);
                eb.AddField("4.!나라", "나라와 언어 정보를 보여줌", true);
                eb.AddField("5.!주사위", "말 그대로 주사위 게임(랜덤)", true);
                eb.AddField("6.!투표", "투표를 생성할수 있음.사용법 : !투표 (타이틀) (선택지1) (선택지2)", true);
                eb.AddField("7.!유튜브 검색", "유튜브를 검색해 줍니다.사용법 : !유튜브 검색 (검색하고싶은거)", true);
                eb.AddField("8.!검색", "구글&네이버를 검색해 줌.사용법 : !검색 (검색하고 싶은거)", true);
                eb.AddField("9.!평균", "숫자들의 평균을 구해줌.사용법 : !평균 1 2 4 2 5 3 6", true);
                eb.AddField("10.!번역", "영어 => 한국어 로 번역해줌", true);
                eb.AddField("11.!역번역", "한국어 => 영어 로 번역해 줌", true);
                eb.AddField("12.!릴리즈", "프로젝트를 올리게 도와주는 명령어.사용법 : !릴리즈 (이름) (설명) (버전(꼭 실수여야 함)) (종류(오프소스 등등)) (개발언어(Python,C#,C++등등)) (github 링크");
                await Context.Channel.SendMessageAsync("", false, eb.Build());
            }
        }
        public class MusicFunc : ModuleBase<SocketCommandContext>
        {
            [Command("play", RunMode = RunMode.Async)]
            public async Task DownAndPlay(string url = null)
            {
                if (url == null)    //url이 매개변수로 주어지지 않았을 경우
                {
                    await Context.Channel.SendMessageAsync("영상의 링크를 제공해주세요.");
                    return;
                }

                if (Context.Guild.CurrentUser.VoiceChannel == null) //사용자가 음성 채널에 들어가지 않은 경우
                {
                    await Context.Channel.SendMessageAsync("음악을 재생하려면 음성 채널에 있어야 합니다.");
                    return;
                }

                await Context.Channel.SendMessageAsync("영상 다운로드 시작");

                YouTube yt = YouTube.Default;   //VideoLibrary의 유튜브 인스턴스 초기화
                YouTubeVideo video = await yt.GetVideoAsync(url);   //링크의 영상을 변수에 저장

                //영상을 저장할 폴더가 없다면 생성
                if (!Directory.Exists(Environment.CurrentDirectory + "\\audio\\"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\audio\\");

                string videoPath = Environment.CurrentDirectory + "\\audio\\" + video.FullName;
                //폴더 속에 영상을 다운로드
                await File.WriteAllBytesAsync(videoPath, await video.GetBytesAsync());
                await Context.Channel.SendMessageAsync("영상 다운로드 완료");

                //FFmpeg 경로 설정
                FFmpeg.SetExecutablesPath(Environment.CurrentDirectory);
                await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);    //FFmpeg 다운로드

                //영상파일에서 음성파일로 변환
                await Conversion.ExtractAudio(
                videoPath,
                Path.ChangeExtension(videoPath, ".mp3")).Start();

                //영상파일 삭제
                File.Delete(videoPath);

                await Context.Channel.SendMessageAsync("음성 추출 완료");
                //유저에게 보내기
                var audioClient = await ((IGuildUser)Context.User).VoiceChannel.ConnectAsync();

                //음성 파일을 스트림 형태로 읽기
                var reader = new Mp3FileReader(Path.ChangeExtension(videoPath, ".mp3"));
                var naudio = WaveFormatConversionStream.CreatePcmStream(reader);

                //음성 채널과 연결된 음성 스트림 생성
                var audioStream = audioClient.CreatePCMStream(AudioApplication.Music);

                byte[] buffer = new byte[naudio.Length];    //음성 데이터 버퍼

                int count = (int)(naudio.Length - naudio.Position);  //읽어들일 데이터의 크기
                await naudio.ReadAsync(buffer, 0, count);    //음성 파일의 데이터를 버퍼에 저장
                await audioStream.WriteAsync(buffer, 0, count);  //버퍼의 데이터를 음성 채널 스트림에 저장

                //스트림 정리
                await audioStream.FlushAsync();
                await ((IGuildUser)Context.User).VoiceChannel.DisconnectAsync();
                await audioStream.DisposeAsync();
                audioClient.Dispose();
                await naudio.DisposeAsync();
                await reader.DisposeAsync();

                //음성 파일 삭제
                File.Delete(Path.ChangeExtension(videoPath, ".mp3"));
            }
        }
    }
}