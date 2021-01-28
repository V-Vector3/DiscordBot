using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis;
using System.Text.RegularExpressions;


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

            await client.LoginAsync(TokenType.Bot, "ODAzMDU2ODEzNjMyMTI2OTg3.YA4O8A.6e_bs-YTmwNmbAQ-T-CwnAtUDZ4"); //봇의 토큰을 사용해 서버에 로그인
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
    }
}
