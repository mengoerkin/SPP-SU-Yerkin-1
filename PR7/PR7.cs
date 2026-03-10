using System;
using System.Collections.Generic;
using System.Linq;

#region ПАТТЕРН КОМАНДА

// ── Интерфейс команды
interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}

// ── Устройства
class Light
{
    readonly string _loc;
    int _brightness = 100;
    public bool IsOn { get; private set; }
    public Light(string loc) => _loc = loc;
    public void TurnOn()  { IsOn = true;  Console.WriteLine($"  [Свет] {_loc}: включён ({_brightness}%)"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"  [Свет] {_loc}: выключен"); }
    public void SetBrightness(int v) { _brightness = Math.Clamp(v,0,100); Console.WriteLine($"  [Свет] {_loc}: яркость {_brightness}%"); }
    public int GetBrightness() => _brightness;
}

class AirConditioner
{
    readonly string _loc;
    int _temp = 22;
    public bool IsOn { get; private set; }
    public AirConditioner(string loc) => _loc = loc;
    public void TurnOn()  { IsOn = true;  Console.WriteLine($"  [Кондиционер] {_loc}: включён ({_temp}°C)"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"  [Кондиционер] {_loc}: выключен"); }
    public void SetTemp(int t) { _temp = Math.Clamp(t,16,30); Console.WriteLine($"  [Кондиционер] {_loc}: температура {_temp}°C"); }
    public int GetTemp() => _temp;
}

class Television
{
    readonly string _loc;
    int _ch = 1, _vol = 20;
    public bool IsOn { get; private set; }
    public Television(string loc) => _loc = loc;
    public void TurnOn()  { IsOn = true;  Console.WriteLine($"  [ТВ] {_loc}: включён (канал {_ch}, громкость {_vol})"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"  [ТВ] {_loc}: выключен"); }
    public void SetChannel(int c) { _ch = Math.Max(1,c); Console.WriteLine($"  [ТВ] {_loc}: канал {_ch}"); }
    public void SetVolume(int v)  { _vol = Math.Clamp(v,0,100); Console.WriteLine($"  [ТВ] {_loc}: громкость {_vol}"); }
    public int GetChannel() => _ch;
    public int GetVolume()  => _vol;
}

class SmartCurtains
{
    readonly string _loc;
    int _open = 0;
    public SmartCurtains(string loc) => _loc = loc;
    public void Open(int pct = 100) { _open = Math.Clamp(pct,0,100); Console.WriteLine($"  [Шторы] {_loc}: открыты на {_open}%"); }
    public void Close()             { _open = 0; Console.WriteLine($"  [Шторы] {_loc}: закрыты"); }
    public int GetOpen() => _open;
}

class MusicPlayer
{
    readonly string _loc;
    string _track = "—";
    int _vol = 30;
    public bool IsPlaying { get; private set; }
    public MusicPlayer(string loc) => _loc = loc;
    public void Play(string t = "") { if (!string.IsNullOrWhiteSpace(t)) _track = t; IsPlaying = true; Console.WriteLine($"  [Плеер] {_loc}: «{_track}» (громкость {_vol})"); }
    public void Stop()              { IsPlaying = false; Console.WriteLine($"  [Плеер] {_loc}: остановлен"); }
    public string GetTrack() => _track;
}

// ── Конкретные команды
class LightOnCommand  : ICommand { readonly Light _l; public string Description=>"Включить свет";    public LightOnCommand(Light l)=>_l=l;  public void Execute()=>_l.TurnOn();  public void Undo()=>_l.TurnOff(); }
class LightOffCommand : ICommand { readonly Light _l; public string Description=>"Выключить свет";   public LightOffCommand(Light l)=>_l=l; public void Execute()=>_l.TurnOff(); public void Undo()=>_l.TurnOn();  }

class LightBrightnessCommand : ICommand
{
    readonly Light _l; readonly int _new; int _prev;
    public string Description => $"Яркость {_new}%";
    public LightBrightnessCommand(Light l, int v) { _l=l; _new=v; }
    public void Execute() { _prev=_l.GetBrightness(); _l.SetBrightness(_new); }
    public void Undo()    => _l.SetBrightness(_prev);
}

class AcOnCommand  : ICommand { readonly AirConditioner _a; public string Description=>"Включить кондиционер";  public AcOnCommand(AirConditioner a)=>_a=a;  public void Execute()=>_a.TurnOn();  public void Undo()=>_a.TurnOff(); }
class AcOffCommand : ICommand { readonly AirConditioner _a; public string Description=>"Выключить кондиционер"; public AcOffCommand(AirConditioner a)=>_a=a; public void Execute()=>_a.TurnOff(); public void Undo()=>_a.TurnOn();  }

class AcTempCommand : ICommand
{
    readonly AirConditioner _a; readonly int _new; int _prev;
    public string Description => $"Температура {_new}°C";
    public AcTempCommand(AirConditioner a, int t) { _a=a; _new=t; }
    public void Execute() { _prev=_a.GetTemp(); _a.SetTemp(_new); }
    public void Undo()    => _a.SetTemp(_prev);
}

class TvOnCommand  : ICommand { readonly Television _t; public string Description=>"Включить ТВ";  public TvOnCommand(Television t)=>_t=t;  public void Execute()=>_t.TurnOn();  public void Undo()=>_t.TurnOff(); }
class TvOffCommand : ICommand { readonly Television _t; public string Description=>"Выключить ТВ"; public TvOffCommand(Television t)=>_t=t; public void Execute()=>_t.TurnOff(); public void Undo()=>_t.TurnOn();  }

class TvChannelCommand : ICommand
{
    readonly Television _t; readonly int _new; int _prev;
    public string Description => $"Канал {_new}";
    public TvChannelCommand(Television t, int c) { _t=t; _new=c; }
    public void Execute() { _prev=_t.GetChannel(); _t.SetChannel(_new); }
    public void Undo()    => _t.SetChannel(_prev);
}

class CurtainsOpenCommand : ICommand
{
    readonly SmartCurtains _c; readonly int _pct; int _prev;
    public string Description => $"Шторы открыть {_pct}%";
    public CurtainsOpenCommand(SmartCurtains c, int pct=100) { _c=c; _pct=pct; }
    public void Execute() { _prev=_c.GetOpen(); _c.Open(_pct); }
    public void Undo()    { if (_prev==0) _c.Close(); else _c.Open(_prev); }
}

class CurtainsCloseCommand : ICommand
{
    readonly SmartCurtains _c; int _prev;
    public string Description => "Закрыть шторы";
    public CurtainsCloseCommand(SmartCurtains c) => _c=c;
    public void Execute() { _prev=_c.GetOpen(); _c.Close(); }
    public void Undo()    => _c.Open(_prev);
}

class PlayerPlayCommand : ICommand
{
    readonly MusicPlayer _p; readonly string _track;
    public string Description => $"Плеер: «{_track}»";
    public PlayerPlayCommand(MusicPlayer p, string t="") { _p=p; _track=t; }
    public void Execute() => _p.Play(_track);
    public void Undo()    => _p.Stop();
}

// Макрокоманда — выполняет список команд, отмена в обратном порядке
class MacroCommand : ICommand
{
    readonly List<ICommand> _cmds;
    public string Description { get; }
    public MacroCommand(string name, IEnumerable<ICommand> cmds) { Description=name; _cmds=new(cmds); }
    public void AddCommand(ICommand c) => _cmds.Add(c);
    public void Execute() { Console.WriteLine($"\n  ▶ Макрос «{Description}»:"); foreach (var c in _cmds) c.Execute(); }
    public void Undo()    { Console.WriteLine($"\n  ↩ Отмена макроса «{Description}»:"); foreach (var c in _cmds.AsEnumerable().Reverse()) c.Undo(); }
}

// Null Object — для незаполненных слотов пульта
class NoCommand : ICommand
{
    public string Description => "(пусто)";
    public void Execute() => Console.WriteLine("  [Пульт] Слот не назначен.");
    public void Undo()    => Console.WriteLine("  [Пульт] Нечего отменять.");
}

// ── Пульт управления
class RemoteControl
{
    const int Slots = 7;
    readonly ICommand[] _slots;
    readonly Stack<ICommand> _undo = new();
    readonly Stack<ICommand> _redo = new();
    MacroCommand? _recording;

    public RemoteControl()
    {
        _slots = new ICommand[Slots];
        for (int i = 0; i < Slots; i++) _slots[i] = new NoCommand();
    }

    public void SetCommand(int slot, ICommand cmd)
    {
        if (slot < 0 || slot >= Slots) { Console.WriteLine($"  [Пульт] Слот {slot} не существует."); return; }
        _slots[slot] = cmd;
    }

    public void Press(int slot)
    {
        if (slot < 0 || slot >= Slots) { Console.WriteLine($"  [Пульт] Слот {slot} не существует (0–{Slots-1})."); return; }
        var cmd = _slots[slot];
        cmd.Execute();
        if (_recording != null && cmd is not NoCommand) _recording.AddCommand(cmd);
        if (cmd is not NoCommand) { _undo.Push(cmd); _redo.Clear(); }
    }

    public void Undo()
    {
        if (_undo.Count == 0) { Console.WriteLine("  [Пульт] Нечего отменять."); return; }
        var cmd = _undo.Pop();
        Console.Write("  [Пульт] Отмена → "); cmd.Undo();
        _redo.Push(cmd);
    }

    public void Redo()
    {
        if (_redo.Count == 0) { Console.WriteLine("  [Пульт] Нет команд для повтора."); return; }
        var cmd = _redo.Pop();
        Console.Write("  [Пульт] Повтор → "); cmd.Execute();
        _undo.Push(cmd);
    }

    public void StartRecording(string name) { _recording = new MacroCommand(name, []); Console.WriteLine($"  [Пульт] ● Запись макроса «{name}»..."); }

    public MacroCommand? StopRecording(int slot)
    {
        if (_recording == null) { Console.WriteLine("  [Пульт] Запись не ведётся."); return null; }
        var m = _recording; _recording = null;
        SetCommand(slot, m);
        Console.WriteLine($"  [Пульт] ■ Макрос «{m.Description}» записан → слот {slot}.");
        return m;
    }

    public void PrintStatus()
    {
        Console.WriteLine("\n  ┌─────────────────────────────┐");
        Console.WriteLine("  │      Пульт управления       │");
        Console.WriteLine("  ├──────┬──────────────────────┤");
        for (int i = 0; i < Slots; i++)
            Console.WriteLine($"  │ [{i}]  │ {_slots[i].Description,-20} │");
        Console.WriteLine("  └──────┴──────────────────────┘");
        Console.WriteLine($"  Undo: {_undo.Count}  Redo: {_redo.Count}\n");
    }
}

#endregion

#region ПАТТЕРН ШАБЛОННЫЙ МЕТОД

// ── Абстрактный генератор отчётов
abstract class ReportGenerator
{
    readonly List<string> _log = new();

    // Шаблонный метод — sealed, алгоритм не переопределяется
    public sealed void GenerateReport()
    {
        Log("Начало генерации");
        OpenDocument();   Log("Документ открыт");
        CollectData();    Log("Данные собраны");
        FormatData();     Log("Данные отформатированы");
        GenerateHeader(); Log("Заголовок готов");
        GenerateBody();   Log("Тело готово");
        if (ShouldAddCharts()) { AddCharts(); Log("Диаграммы добавлены"); }
        GenerateFooter(); Log("Подвал готов");
        if (CustomerWantsSave()) { SaveToFile(); Log("Сохранено в файл"); }
        else                     { ShowOnScreen(); Log("Отображено на экране"); }
        CloseDocument();  Log("Документ закрыт");
        PrintLog();
    }

    // Абстрактные шаги — обязательны к реализации
    protected abstract string ReportType { get; }
    protected abstract void OpenDocument();
    protected abstract void CollectData();
    protected abstract void FormatData();
    protected abstract void GenerateHeader();
    protected abstract void GenerateBody();
    protected abstract void GenerateFooter();
    protected abstract void SaveToFile();
    protected abstract void CloseDocument();

    // Hooks — переопределяются по желанию
    protected virtual bool ShouldAddCharts()   => false;
    protected virtual void AddCharts()         => Console.WriteLine($"  [{ReportType}] Диаграммы (базовые).");
    protected virtual void ShowOnScreen()      => Console.WriteLine($"  [{ReportType}] Отображение в окне просмотра.");
    protected virtual bool CustomerWantsSave()
    {
        while (true)
        {
            Console.Write($"  [{ReportType}] Сохранить в файл? (да/нет): ");
            return Console.ReadLine()?.Trim().ToLower() switch
            {
                "да" or "д" or "yes" or "y" => true,
                "нет" or "н" or "no"  or "n" => false,
                _ => Invalid()
            };
        }
        bool Invalid() { Console.WriteLine("  [!] Введите «да» или «нет»."); return CustomerWantsSave(); }
    }

    void Log(string msg) => _log.Add($"  {DateTime.Now:HH:mm:ss.fff} [{ReportType}] {msg}");
    void PrintLog()
    {
        Console.WriteLine($"\n  ── Журнал [{ReportType}] ──");
        _log.ForEach(Console.WriteLine);
    }
}

// ── PDF-отчёт
class PdfReport : ReportGenerator
{
    protected override string ReportType   => "PDF";
    protected override void OpenDocument() => Console.WriteLine("  [PDF] Инициализация PDF-движка...");
    protected override void CollectData()  => Console.WriteLine("  [PDF] Запрос данных из БД...");
    protected override void FormatData()   => Console.WriteLine("  [PDF] Форматирование: шрифты, отступы.");
    protected override void GenerateHeader()=> Console.WriteLine("  [PDF] Заголовок: логотип + название.");
    protected override void GenerateBody() { Console.WriteLine("  [PDF] Формирование таблиц и текста."); Console.WriteLine("  [PDF] Встраивание изображений."); }
    protected override void GenerateFooter()=> Console.WriteLine("  [PDF] Нумерация страниц.");
    protected override void SaveToFile()   => Console.WriteLine("  [PDF] Запись «report.pdf».");
    protected override void CloseDocument()=> Console.WriteLine("  [PDF] Завершение PDF (xref-таблица).");
    protected override bool ShouldAddCharts() => true;
    protected override void AddCharts()       => Console.WriteLine("  [PDF] Векторные диаграммы (SVG → PDF).");
}

// ── Excel-отчёт
class ExcelReport : ReportGenerator
{
    protected override string ReportType    => "Excel";
    protected override void OpenDocument()  => Console.WriteLine("  [Excel] Создание рабочей книги (EPPlus)...");
    protected override void CollectData()   => Console.WriteLine("  [Excel] Выборка данных...");
    protected override void FormatData()    => Console.WriteLine("  [Excel] Форматирование ячеек: числа, даты, цвета.");
    protected override void GenerateHeader()=> Console.WriteLine("  [Excel] Заголовки столбцов + закреплённая строка.");
    protected override void GenerateBody()  { Console.WriteLine("  [Excel] Запись строк данных."); Console.WriteLine("  [Excel] Формулы (SUM, AVERAGE) + условное форматирование."); }
    protected override void GenerateFooter()=> Console.WriteLine("  [Excel] Итоговая строка.");
    protected override void SaveToFile()    => Console.WriteLine("  [Excel] Сохранение «report.xlsx».");
    protected override void CloseDocument() => Console.WriteLine("  [Excel] Закрытие книги.");
    // Excel всегда сохраняет — перекрываем hook
    protected override bool CustomerWantsSave() => true;
    protected override bool ShouldAddCharts()   => true;
    protected override void AddCharts()         => Console.WriteLine("  [Excel] Гистограмма на отдельном листе.");
}

// ── HTML-отчёт
class HtmlReport : ReportGenerator
{
    protected override string ReportType    => "HTML";
    protected override void OpenDocument()  => Console.WriteLine("  [HTML] <!DOCTYPE html> открыт...");
    protected override void CollectData()   => Console.WriteLine("  [HTML] Запрос через REST API...");
    protected override void FormatData()    => Console.WriteLine("  [HTML] CSS-классы + Bootstrap.");
    protected override void GenerateHeader()=> Console.WriteLine("  [HTML] <head>: meta, title, CSS.");
    protected override void GenerateBody()  { Console.WriteLine("  [HTML] Таблица <table> с данными."); Console.WriteLine("  [HTML] Интерактивные фильтры (JS)."); }
    protected override void GenerateFooter()=> Console.WriteLine("  [HTML] <footer> закрыт.");
    protected override void SaveToFile()    => Console.WriteLine("  [HTML] Запись «report.html» в wwwroot.");
    protected override void CloseDocument() => Console.WriteLine("  [HTML] </html>");
}

// ── CSV-отчёт (расширение)
class CsvReport : ReportGenerator
{
    readonly char _sep;
    public CsvReport(char sep = ',') => _sep = sep;
    protected override string ReportType    => "CSV";
    protected override void OpenDocument()  => Console.WriteLine($"  [CSV] Инициализация потока (sep='{_sep}').");
    protected override void CollectData()   => Console.WriteLine("  [CSV] Получение плоских данных...");
    protected override void FormatData()    => Console.WriteLine("  [CSV] Экранирование, ISO-даты.");
    protected override void GenerateHeader()=> Console.WriteLine($"  [CSV] Строка заголовков через '{_sep}'.");
    protected override void GenerateBody()  => Console.WriteLine("  [CSV] Запись строк данных.");
    protected override void GenerateFooter()=> Console.WriteLine("  [CSV] Пустая строка-терминатор.");
    protected override void SaveToFile()    => Console.WriteLine("  [CSV] Сохранение «report.csv» (UTF-8 BOM).");
    protected override void CloseDocument() => Console.WriteLine("  [CSV] Поток закрыт.");
}

#endregion

#region ПАТТЕРН ПОСРЕДНИК

// ── Интерфейсы
interface IMediator
{
    void SendMessage(string msg, IChatUser sender, string channel);
    void SendPrivate(string msg, IChatUser sender, IChatUser to);
    void AddUser(IChatUser user, string channel);
    void RemoveUser(IChatUser user, string channel);
}

interface IChatUser
{
    string Name { get; }
    bool IsBlocked { get; }
    void Receive(string msg);
    void SetMediator(IMediator m);
}

// ── Пользователь
class ChatUser : IChatUser
{
    public string Name { get; }
    public bool IsBlocked { get; private set; }
    IMediator? _med;

    public ChatUser(string name) => Name = name;
    public void SetMediator(IMediator m) => _med = m;
    public void Receive(string msg) { if (!IsBlocked) Console.WriteLine($"  [{Name}] ← {msg}"); }

    public void Send(string msg, string channel)
    {
        if (IsBlocked) { Console.WriteLine($"  [{Name}] Заблокирован — сообщение не отправлено."); return; }
        _med?.SendMessage(msg, this, channel);
    }

    public void SendPrivate(string msg, IChatUser to) => _med?.SendPrivate(msg, this, to);

    public void Block()   { IsBlocked = true;  Console.WriteLine($"  [Система] «{Name}» заблокирован."); }
    public void Unblock() { IsBlocked = false; Console.WriteLine($"  [Система] «{Name}» разблокирован."); }
}

// ── Посредник с каналами
class ChannelMediator : IMediator
{
    readonly Dictionary<string, List<IChatUser>> _channels = new();
    readonly Dictionary<string, IChatUser> _registry = new();
    readonly HashSet<string> _admins = new();

    // Управление каналами
    public void CreateChannel(string ch)
    {
        if (_channels.ContainsKey(ch)) { Console.WriteLine($"  [Посредник] Канал «{ch}» уже есть."); return; }
        _channels[ch] = new();
        Console.WriteLine($"  [Посредник] Канал «{ch}» создан.");
    }

    // IMediator
    public void AddUser(IChatUser user, string ch)
    {
        if (!_channels.ContainsKey(ch)) CreateChannel(ch);
        if (_channels[ch].Contains(user)) return;
        _channels[ch].Add(user);
        _registry[user.Name] = user;
        user.SetMediator(this);
        Notify($"*** «{user.Name}» вошёл в «{ch}» ***", ch, except: user);
        Console.WriteLine($"  [Посредник] «{user.Name}» → #{ch}");
    }

    public void RemoveUser(IChatUser user, string ch)
    {
        if (!_channels.TryGetValue(ch, out var list) || !list.Contains(user))
        { Console.WriteLine($"  [Посредник] «{user.Name}» не найден в #{ch}."); return; }
        list.Remove(user);
        Notify($"*** «{user.Name}» покинул «{ch}» ***", ch, except: user);
        Console.WriteLine($"  [Посредник] «{user.Name}» удалён из #{ch}.");
    }

    public void SendMessage(string msg, IChatUser sender, string ch)
    {
        if (!_channels.ContainsKey(ch))
        { Console.WriteLine($"  [Посредник] Канал «{ch}» не существует."); return; }
        if (!_channels[ch].Contains(sender))
        { Console.WriteLine($"  [Посредник] «{sender.Name}» не состоит в #{ch}."); return; }
        var full = $"#{ch} | {sender.Name}: {msg}";
        Console.WriteLine($"  → {full}");
        foreach (var u in _channels[ch]) if (!ReferenceEquals(u, sender)) u.Receive(full);
    }

    public void SendPrivate(string msg, IChatUser sender, IChatUser to)
    {
        var full = $"[ЛС от {sender.Name}] {msg}";
        Console.WriteLine($"  → {full}");
        to.Receive(full);
    }

    // Кросс-канальная отправка (расширение)
    public void CrossSend(string msg, IChatUser sender, string targetCh)
    {
        if (!_channels.ContainsKey(targetCh))
        { Console.WriteLine($"  [Посредник] Канал «{targetCh}» не найден."); return; }
        var full = $"[Кросс→#{targetCh}] {sender.Name}: {msg}";
        Console.WriteLine($"  → {full}");
        Notify(full, targetCh, except: sender);
    }

    // Администраторы (расширение)
    public void MakeAdmin(string name) { _admins.Add(name); Console.WriteLine($"  [Посредник] «{name}» — администратор."); }

    public void BlockUser(string admin, string target)
    {
        if (!_admins.Contains(admin)) { Console.WriteLine($"  [Посредник] «{admin}» не является администратором."); return; }
        if (_registry.TryGetValue(target, out var u) && u is ChatUser cu) cu.Block();
        else Console.WriteLine($"  [Посредник] Пользователь «{target}» не найден.");
    }

    public void UnblockUser(string admin, string target)
    {
        if (!_admins.Contains(admin)) { Console.WriteLine($"  [Посредник] «{admin}» не является администратором."); return; }
        if (_registry.TryGetValue(target, out var u) && u is ChatUser cu) cu.Unblock();
    }

    public void PrintChannels()
    {
        Console.WriteLine("\n  ── Каналы ──");
        foreach (var (ch, users) in _channels)
            Console.WriteLine($"  #{ch}: {string.Join(", ", users.Select(u => u.Name))}");
        Console.WriteLine();
    }

    void Notify(string msg, string ch, IChatUser? except = null)
    {
        if (!_channels.ContainsKey(ch)) return;
        foreach (var u in _channels[ch]) if (except == null || !ReferenceEquals(u, except)) u.Receive(msg);
    }
}

#endregion

//  КЛИЕНТСКИЙ КОД
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoCommand();
        DemoTemplateMethod();
        DemoMediator();
    }

    // ── 1. Команда
    static void DemoCommand()
    {
        Header("ПАТТЕРН КОМАНДА — Умный дом");

        var light    = new Light("Гостиная");
        var ac       = new AirConditioner("Гостиная");
        var tv       = new Television("Гостиная");
        var curtains = new SmartCurtains("Гостиная");
        var player   = new MusicPlayer("Кухня");

        var remote = new RemoteControl();
        remote.SetCommand(0, new LightOnCommand(light));
        remote.SetCommand(1, new LightOffCommand(light));
        remote.SetCommand(2, new AcOnCommand(ac));
        remote.SetCommand(3, new TvOnCommand(tv));
        remote.SetCommand(4, new CurtainsOpenCommand(curtains, 80));
        remote.SetCommand(5, new PlayerPlayCommand(player, "Beethoven — Лунная соната"));
        // Слот 6 — пустой (NoCommand)
        remote.PrintStatus();

        Section("Тест 1: Выполнение команд");
        remote.Press(0); remote.Press(2); remote.Press(3);
        remote.Press(4); remote.Press(5);

        Section("Тест 2: Undo / Redo");
        remote.Undo(); remote.Undo(); remote.Redo(); remote.Undo();
        remote.Undo(); remote.Undo(); remote.Undo();
        remote.Undo(); // история пуста

        Section("Тест 3: Пустой и несуществующий слот");
        remote.Press(6);  // NoCommand
        remote.Press(10); // несуществующий

        Section("Тест 4: Макрокоманда «Вечер дома»");
        var evening = new MacroCommand("Вечер дома", new ICommand[]
        {
            new LightOnCommand(light),
            new LightBrightnessCommand(light, 30),
            new AcTempCommand(ac, 22),
            new TvOnCommand(tv),
            new TvChannelCommand(tv, 1),
            new CurtainsCloseCommand(curtains),
        });
        remote.SetCommand(5, evening);
        remote.Press(5);
        Console.WriteLine();
        remote.Undo(); // отмена всего макроса

        Section("Тест 5: Запись макроса с пульта");
        remote.SetCommand(0, new LightOnCommand(light));
        remote.SetCommand(1, new AcOnCommand(ac));
        remote.SetCommand(2, new PlayerPlayCommand(player, "Mozart — Eine kleine Nachtmusik"));
        remote.StartRecording("Утро");
        remote.Press(0); remote.Press(1); remote.Press(2);
        remote.StopRecording(6);
        Console.WriteLine("\n  → Запуск макроса «Утро» (слот 6):");
        remote.Press(6);
        Console.WriteLine();
        remote.Undo();
    }

    // ── 2. Шаблонный метод
    static void DemoTemplateMethod()
    {
        Header("ПАТТЕРН ШАБЛОННЫЙ МЕТОД — Генератор отчётов");

        Section("PDF-отчёт");
        WithInput("да",   () => new PdfReport().GenerateReport());

        Section("Excel-отчёт (всегда сохраняет)");
        new ExcelReport().GenerateReport();

        Section("HTML-отчёт");
        WithInput("нет",  () => new HtmlReport().GenerateReport());

        Section("CSV-отчёт (расширение, разделитель «;»)");
        WithInput("да",   () => new CsvReport(';').GenerateReport());
    }

    // ── 3. Посредник
    static void DemoMediator()
    {
        Header("ПАТТЕРН ПОСРЕДНИК — Чат-система");

        var med   = new ChannelMediator();
        var alice = new ChatUser("Alice");
        var bob   = new ChatUser("Bob");
        var carol = new ChatUser("Carol");
        var dave  = new ChatUser("Dave");

        Section("Тест 1: Добавление пользователей в каналы");
        med.AddUser(alice, "general"); med.AddUser(bob,   "general");
        med.AddUser(carol, "general"); med.AddUser(dave,  "dev");
        med.AddUser(alice, "dev");
        med.PrintChannels();

        Section("Тест 2: Отправка сообщений");
        alice.Send("Всем привет!", "general");
        bob.Send("Привет, Alice!", "general");
        dave.Send("Есть разрабы?", "dev");

        Section("Тест 3: Личное сообщение");
        alice.SendPrivate("Bob, созвонимся в 15:00?", bob);

        Section("Тест 4: Кросс-канальная отправка");
        med.CrossSend("Деплой через 5 минут!", alice, "dev");

        Section("Тест 5: Обработка ошибок");
        alice.Send("Пишу в несуществующий канал", "secret");
        bob.Send("Я не в этом канале", "dev");

        Section("Тест 6: Блокировка администратором");
        med.MakeAdmin("Alice");
        med.BlockUser("Alice", "Bob");
        bob.Send("Попробую написать...", "general");
        med.BlockUser("Carol", "Dave"); // Carol не админ
        med.UnblockUser("Alice", "Bob");
        bob.Send("Я снова здесь!", "general");

        Section("Тест 7: Удаление из канала");
        med.RemoveUser(carol, "general");
        alice.Send("Carol ушла — видит ли она это?", "general");
    }

    // ── Вспомогательные
    static void Header(string t)
    {
        Console.WriteLine($"\n{"═",0}{"",0}");
        Console.WriteLine($"{"═"[..0]}{"",0}");
        Console.WriteLine($"\n{"═"[..0].PadRight(60,'═')}");
        Console.WriteLine($"  {t}");
        Console.WriteLine($"{"═"[..0].PadRight(60,'═')}\n");
    }

    static void Section(string t) => Console.WriteLine($"\n  ► {t}\n");

    static void WithInput(string input, Action action)
    {
        var orig = Console.In;
        Console.SetIn(new System.IO.StringReader(input));
        try { action(); } finally { Console.SetIn(orig); }
    }
}
