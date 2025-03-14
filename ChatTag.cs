using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using Microsoft.Extensions.Logging;

namespace ChatTag
{
    public class Config : BasePluginConfig
    {
        public TagConfig TagConfig { get; set; } = new TagConfig();
    }

    public class TagConfig
    {
        public Dictionary<string, Kategori> Kategoriler { get; set; } = new Dictionary<string, Kategori>
        {
            {
                "Administrator", new Kategori
                {
                    Tag = "[Administrator]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Genel Sunucu Sorumlusu", new Kategori
                {
                    Tag = "[GNL Sunucu Sorumlusu]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Sunucu Sorumlusu", new Kategori
                {
                    Tag = "[Sunucu Sorumlusu]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Server Sahibi", new Kategori
                {
                    Tag = "[Server Sahibi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Server Oragi", new Kategori
                {
                    Tag = "[Server Ortagi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Yonetim Lideri", new Kategori
                {
                    Tag = "[Yonetim Lideri]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Yonetim Ekibi", new Kategori
                {
                    Tag = "[Yonetim Ekibi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Yonetim Yardimcisi", new Kategori
                {
                    Tag = "[Yonetim Yardimcisi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Asistan", new Kategori
                {
                    Tag = "[Asistan]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Kaptan Lideri", new Kategori
                {
                    Tag = "[Kaptan Lideri]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Kaptan Yardimcisi", new Kategori
                {
                    Tag = "[Kaptan Yardimcisi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Kaptan", new Kategori
                {
                    Tag = "[Kaptan]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Bas Moderator", new Kategori
                {
                    Tag = "[Bas Moderator]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Moderator Yardimcisi", new Kategori
                {
                    Tag = "[Moderator Yardimcisi]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Moderator", new Kategori
                {
                    Tag = "[Moderator]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Yetkili", new Kategori
                {
                    Tag = "[Yetkili]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Full VIP", new Kategori
                {
                    Tag = "[Full VIP]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "VIP", new Kategori
                {
                    Tag = "[VIP]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Normal VIP", new Kategori
                {
                    Tag = "[Normal VIP]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Full Admin", new Kategori
                {
                    Tag = "[Full Admin]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Admin", new Kategori
                {
                    Tag = "[Admin]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Normal Admin", new Kategori
                {
                    Tag = "[Normal Admin]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Admin III", new Kategori
                {
                    Tag = "[Admin III]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Admin II", new Kategori
                {
                    Tag = "[Admin II]",
                    SteamIDler = new List<string> { }
                }
            },
            {
                "Admin I", new Kategori
                {
                    Tag = "[Admin I]",
                    SteamIDler = new List<string> { }
                }
            }
        };
    }

    public class Kategori
    {
        public string Tag { get; set; }
        public List<string> SteamIDler { get; set; }
    }

    public class Plugin_Init : BasePlugin, IPluginConfig<Config>
    {
        public override string ModuleName => "ChatTag Plugin";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "QuryWesT";

        private readonly string _logFilePath = Path.Combine(Server.GameDirectory, "tag_combinasyon.txt");
        private readonly string _tagConfigPath = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", "tag_config", "tag_config.json");
        private HashSet<uint> _IsPlayerWhoUsedCommand = new HashSet<uint>();
        public Config Config { get; set; } = new Config();

        public void OnConfigParsed(Config config)
        {
            try
            {
                if (File.Exists(_tagConfigPath))
                {
                    string jsonConfig = File.ReadAllText(_tagConfigPath);
                    TagConfig fileConfig = JsonSerializer.Deserialize<TagConfig>(jsonConfig)!;
                    Config.TagConfig = fileConfig;
                    Logger.LogInformation("[Combined Plugin] Config dosyası yüklendi.");
                }
                else
                {
                    Logger.LogError("[Combined Plugin] Config dosyası bulunamadı, varsayılan yapı kullanılıyor.");
                    Config.TagConfig = new TagConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Config okunurken hata oluştu: {ex.Message}");
                Config.TagConfig = new TagConfig();
            }
        }

        public override void Load(bool hotReload)
        {
            EnsureTagConfigExists();
            RegisterEventHandler<EventRoundStart>(pGetPlayerOnRoundStart);
            AddCommandListener("say", pGetPlayerChat);
            AddCommandListener("say_team", pGetPlayerTeamChat);
            AddCommand("css_configyenitageklendi", "Config'i yeniden yükler", pGetPlayerReloadConfig);
        }

        private void LogToFile(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Log dosyasına yazma hatası: {ex.Message}");
            }
        }

        private HookResult pGetPlayerOnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            _IsPlayerWhoUsedCommand.Clear();
            return HookResult.Continue;
        }

        private void EnsureTagConfigExists()
        {
            string configDirectory = Path.GetDirectoryName(_tagConfigPath);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
                Logger.LogInformation($"Dizin oluşturuldu: {configDirectory}");
                LogToFile($"Dizin oluşturuldu: {configDirectory}");
            }

            if (!File.Exists(_tagConfigPath))
            {
                Logger.LogInformation("Tag config dosyası bulunamadı, oluşturuluyor...");
                LogToFile("Tag config dosyası bulunamadı, oluşturuluyor...");

                var defaultConfig = new TagConfig
                {
                    Kategoriler = new Dictionary<string, Kategori>
                    {
                        {
                            "Administrator", new Kategori
                            {
                                Tag = "[Administrator]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Genel Sunucu Sorumlusu", new Kategori
                            {
                                Tag = "[GNL Sunucu Sorumlusu]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Sunucu Sorumlusu", new Kategori
                            {
                                Tag = "[Sunucu Sorumlusu]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Server Sahibi", new Kategori
                            {
                                Tag = "[Server Sahibi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Server Oragi", new Kategori
                            {
                                Tag = "[Server Ortagi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Yonetim Lideri", new Kategori
                            {
                                Tag = "[Yonetim Lideri]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Yonetim Ekibi", new Kategori
                            {
                                Tag = "[Yonetim Ekibi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Yonetim Yardimcisi", new Kategori
                            {
                                Tag = "[Yonetim Yardimcisi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Asistan", new Kategori
                            {
                                Tag = "[Asistan]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Kaptan Lideri", new Kategori
                            {
                                Tag = "[Kaptan Lideri]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Kaptan Yardimcisi", new Kategori
                            {
                                Tag = "[Kaptan Yardimcisi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Kaptan", new Kategori
                            {
                                Tag = "[Kaptan]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Bas Moderator", new Kategori
                            {
                                Tag = "[Bas Moderator]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Moderator Yardimcisi", new Kategori
                            {
                                Tag = "[Moderator Yardimcisi]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Moderator", new Kategori
                            {
                                Tag = "[Moderator]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Yetkili", new Kategori
                            {
                                Tag = "[Yetkili]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Full VIP", new Kategori
                            {
                                Tag = "[Full VIP]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "VIP", new Kategori
                            {
                                Tag = "[VIP]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Normal VIP", new Kategori
                            {
                                Tag = "[Normal VIP]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Full Admin", new Kategori
                            {
                                Tag = "[Full Admin]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Admin", new Kategori
                            {
                                Tag = "[Admin]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Normal Admin", new Kategori
                            {
                                Tag = "[Normal Admin]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Admin III", new Kategori
                            {
                                Tag = "[Admin III]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Admin II", new Kategori
                            {
                                Tag = "[Admin II]",
                                SteamIDler = new List<string> { }
                            }
                        },
                        {
                            "Admin I", new Kategori
                            {
                                Tag = "[Admin I]",
                                SteamIDler = new List<string> { }
                            }
                        }
                    }
                };

                string jsonConfig = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_tagConfigPath, jsonConfig);

                Logger.LogInformation("Varsayılan tag config dosyası oluşturuldu.");
                LogToFile("Varsayılan tag config dosyası oluşturuldu.");
            }
            else
            {
                string jsonConfig = File.ReadAllText(_tagConfigPath);
                Config.TagConfig = JsonSerializer.Deserialize<TagConfig>(jsonConfig)!;
                Logger.LogInformation("Mevcut tag config dosyası yüklendi.");
            }
        }

        private void pGetPlayerReloadConfig(CCSPlayerController? IsPlayer, CommandInfo info)
        {
            if (IsPlayer != null && !AdminManager.PlayerHasPermissions(IsPlayer, "@css/admin"))
            {
                IsPlayer.PrintToChat($"{ChatColors.Red}Bu komutu kullanma yetkiniz yok!");
                return;
            }

            try
            {
                string jsonConfig = File.ReadAllText(_tagConfigPath);
                TagConfig newConfig = JsonSerializer.Deserialize<TagConfig>(jsonConfig)!;

                Logger.LogInformation($"Yeni Config Yüklendi: {jsonConfig}");
                Config.TagConfig = newConfig;
                pGetPlayerRefreshTags();

                Logger.LogInformation("Config başarıyla yeniden yüklendi!");
                if (IsPlayer != null)
                {
                    IsPlayer.PrintToChat($"{ChatColors.Green}Config başarıyla yeniden yüklendi!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Config yeniden yüklenirken hata: {ex.Message}");
                if (IsPlayer != null)
                {
                    IsPlayer.PrintToChat($"{ChatColors.Red}Config yeniden yüklenemedi: {ex.Message}");
                }
            }
        }

        private void pGetPlayerRefreshTags()
        {
            foreach (var IsPlayer in Utilities.GetPlayers())
            {
                if (IsPlayer == null || !IsPlayer.IsValid)
                    continue;

                string IsSteamUID = IsPlayer.SteamID.ToString();
                string IsNewTag = pGetPlayerTag(IsSteamUID);

                if (!string.IsNullOrEmpty(IsNewTag))
                {
                    IsPlayer.PrintToChat($"{ChatColors.Green}Tagınız güncellendi: {IsNewTag}");
                }
            }

            Logger.LogInformation("[Combined Plugin] Tüm oyuncuların tagları güncellendi!");
        }

        private string pGetPlayerTag(string IsSteamUID)
        {
            // Öncelik sırası: Kurucu > Admin > Vip
            string[] oncelikSirasi = {
                "Administrator",
                "Genel Sunucu Sorumlusu",
                "Sunucu Sorumlusu",
                "Server Sahibi",
                "Server Oragi",
                "Yonetim Lideri",
                "Yonetim Ekibi",
                "Yonetim Yardımcısı",
                "Asistan",
                "Kaptan Lideri",
                "Kaptan Yardımcısı",
                "Kaptan",
                "Bas Moderatör",
                "Moderatör Yardımcısı",
                "Moderatör",
                "Yetkili",
                "Full VIP",
                "VIP",
                "Normal VIP",
                "Full Admin",
                "Admin",
                "Normal Admin",
                "Admin III",
                "Admin II",
                "Admin I"
            };

            foreach (var kategoriAdi in oncelikSirasi)
            {
                if (Config.TagConfig.Kategoriler.TryGetValue(kategoriAdi, out var kategori))
                {
                    if (kategori.SteamIDler.Contains(IsSteamUID))
                    {
                        return kategori.Tag;
                    }
                }
            }
            return "";
        }

        private bool pGetPlayerHasUID(CCSPlayerController IsPlayer)
        {
            if (IsPlayer == null || !IsPlayer.IsValid)
                return false;

            string IsSteamUID = IsPlayer.SteamID.ToString();
            foreach (var kategori in Config.TagConfig.Kategoriler)
            {
                if (kategori.Value.SteamIDler.Contains(IsSteamUID))
                {
                    return true;
                }
            }
            return false;
        }

        private HookResult pGetPlayerChat(CCSPlayerController? IsPlayer, CommandInfo info)
        {
            if (IsPlayer == null || !IsPlayer.IsValid || !IsPlayer.PlayerPawn.IsValid || info.GetArg(1).Length == 0)
                return HookResult.Continue;

            if (info.GetArg(1).StartsWith("!") || info.GetArg(1).StartsWith("@") ||
                info.GetArg(1).StartsWith("/") || info.GetArg(1).StartsWith("."))
                return HookResult.Continue;

            string messageText = info.GetArg(1);
            string IsDead = !IsPlayer.PawnIsAlive ? $"{ChatColors.Red}☠ {ChatColors.Default}" : "";
            string IsSteamUID = IsPlayer.SteamID.ToString();
            string IsPlayerName = IsPlayer.PlayerName;
            string IsTag = pGetPlayerTag(IsSteamUID);
            bool IsUID = pGetPlayerHasUID(IsPlayer);

            string coloredPlayerName;
            switch (IsPlayer.TeamNum)
            {
                case 2:
                    coloredPlayerName = $"{ChatColors.Yellow}{IsPlayerName}{ChatColors.Default}";
                    break;
                case 3:
                    coloredPlayerName = $"{ChatColors.Blue}{IsPlayerName}{ChatColors.Default}";
                    break;
                default:
                    coloredPlayerName = $"{ChatColors.Default}{IsPlayerName}{ChatColors.Default}";
                    break;
            }

            foreach (var kategori in Config.TagConfig.Kategoriler)
            {
                if (kategori.Value.SteamIDler.Contains(IsSteamUID))
                {
                    IsTag = kategori.Value.Tag;
                    break;
                }
            }

            if (IsUID)
            {
                Server.PrintToChatAll($" {IsDead}{ChatColors.Purple}{IsTag} {ChatColors.Red}{IsPlayerName}: {ChatColors.Green}{messageText}");
            }
            else
            {
                Server.PrintToChatAll($" {IsDead}{ChatColors.Lime} {coloredPlayerName}: {ChatColors.Default}{messageText}");
            }

            Logger.LogInformation("Finished processing player chat.");
            return HookResult.Stop;
        }

        private HookResult pGetPlayerTeamChat(CCSPlayerController? IsPlayer, CommandInfo info)
        {
            if (IsPlayer == null || !IsPlayer.IsValid || !IsPlayer.PlayerPawn.IsValid || info.GetArg(1).Length == 0)
                return HookResult.Continue;

            if (info.GetArg(1).StartsWith("!") || info.GetArg(1).StartsWith("@") ||
                info.GetArg(1).StartsWith("/") || info.GetArg(1).StartsWith("."))
                return HookResult.Continue;

            string IsDead = !IsPlayer.PawnIsAlive ? $"{ChatColors.Red}☠ {ChatColors.Default}" : "";
            string IsSteamUID = IsPlayer.SteamID.ToString();
            string IsPlayerName = IsPlayer.PlayerName;
            string IsTag = pGetPlayerTag(IsSteamUID);
            bool IsUID = pGetPlayerHasUID(IsPlayer);

            string coloredPlayerName;
            switch (IsPlayer.TeamNum)
            {
                case 2:
                    coloredPlayerName = $"{ChatColors.Yellow}{IsPlayerName}{ChatColors.Default}";
                    break;
                case 3:
                    coloredPlayerName = $"{ChatColors.Blue}{IsPlayerName}{ChatColors.Default}";
                    break;
                default:
                    coloredPlayerName = $"{ChatColors.Default}{IsPlayerName}{ChatColors.Default}";
                    break;
            }

            foreach (var kategori in Config.TagConfig.Kategoriler)
            {
                if (kategori.Value.SteamIDler.Contains(IsSteamUID))
                {
                    IsTag = kategori.Value.Tag;
                    break;
                }
            }
            foreach (var p in Utilities.GetPlayers().Where(p => p.TeamNum == IsPlayer.TeamNum && p.IsValid && !p.IsBot))
            {
                if (IsUID)
                {
                    p.PrintToChat($" {IsDead}{pGetPlayerTeamName(IsPlayer.TeamNum)}{ChatColors.Purple}{IsTag} {coloredPlayerName}: {ChatColors.Green}{info.GetArg(1)}");
                }
                else
                {
                    p.PrintToChat($" {IsDead}{pGetPlayerTeamName(IsPlayer.TeamNum)}{ChatColors.Lime} {coloredPlayerName}: {ChatColors.Default}{info.GetArg(1)}");
                }
            }
            return HookResult.Stop;
        }

        private string pGetPlayerTeamName(int IsTeamNum)
        {
            string IsTeamName = "";

            switch (IsTeamNum)
            {
                case 0:
                    IsTeamName = "(NONE)";
                    break;
                case 1:
                    IsTeamName = "(SPEC)";
                    break;
                case 2:
                    IsTeamName = $"{ChatColors.Yellow}(T)";
                    break;
                case 3:
                    IsTeamName = $"{ChatColors.Blue}(CT)";
                    break;
            }

            return IsTeamName;
        }
    }
}