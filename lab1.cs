using System;
using System.Collections.Generic;
using System.Linq;
namespace CourseManagementShort
{
    public abstract class C
    {
        public Guid Id { get; }
        public string T { get; private set; }
        public I I { get; private set; }
        private readonly List<S> _ss = new List<S>();
        public IEnumerable<S> Ss { get { return _ss.AsReadOnly(); } }
        protected C(string t)
        {
            if (t == null) throw new ArgumentNullException("t");
            Id = Guid.NewGuid();
            T = t;
        }
        public void SetI(I i)
        {
            if (i == null) throw new ArgumentNullException("i");
            I = i;
        }
        public void RemI()
        {
            I = null;
        }
        public void AddS(S s)
        {
            if (s == null) throw new ArgumentNullException("s");
            if (_ss.Any(x => x.Id == s.Id)) throw new InvalidOperationException("Студент уже добавлен");
            _ss.Add(s);
        }
        public void RemS(Guid id)
        {
            S found = null;
            foreach (var x in _ss)
            {
                if (x.Id == id) { found = x; break; }
            }
            if (found == null) throw new KeyNotFoundException("Студент не найден");
            _ss.Remove(found);
        }
        public void Ren(string t)
        {
            if (string.IsNullOrWhiteSpace(t)) throw new ArgumentException("Пустое имя");
            T = t;
        }
        public override string ToString()
        {
            var i = I == null ? "(нет)" : I.N;
            return string.Format("[{0}] {1} (Id={2}) - Преподаватель={3}, Студентов={4}", GetType().Name, T, Id, i, _ss.Count);
        }
    }
    public class OC : C
    {
        public string P { get; private set; }
        public Uri U { get; private set; }
        public OC(string t, string p, Uri u) : base(t)
        {
            if (p == null) throw new ArgumentNullException("p");
            P = p;
            U = u;
        }
        public void SetU(string u)
        {
            if (string.IsNullOrWhiteSpace(u)) { U = null; return; }
            U = new Uri(u, UriKind.Absolute);
        }
        public void SetP(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) throw new ArgumentException("Платформа пуста");
            P = p;
        }
    }
    public class FC : C
    {
        public string L { get; private set; }
        public string R { get; private set; }
        public FC(string t, string l, string r) : base(t)
        {
            if (l == null) throw new ArgumentNullException("l");
            if (r == null) throw new ArgumentNullException("r");
            L = l;
            R = r;
        }
        public void SetR(string r)
        {
            if (string.IsNullOrWhiteSpace(r)) throw new ArgumentException("Пустая аудитория");
            R = r;
        }
        public void SetL(string l)
        {
            if (string.IsNullOrWhiteSpace(l)) throw new ArgumentException("Пустая локация");
            L = l;
        }
    }
    public class S
    {
        public Guid Id { get; }
        public string N { get; private set; }
        public S(string n)
        {
            if (n == null) throw new ArgumentNullException("n");
            Id = Guid.NewGuid();
            N = n;
        }
        public override string ToString()
        {
            return string.Format("Студент: {0} ({1})", N, Id);
        }
    }
    public class I
    {
        public Guid Id { get; }
        public string N { get; private set; }

        public I(string n)
        {
            if (n == null) throw new ArgumentNullException("n");
            Id = Guid.NewGuid();
            N = n;
        }
        public override string ToString()
        {
            return string.Format("Преподаватель: {0} ({1})", N, Id);
        }
    }
    public class B
    {
        private string _t;
        private bool _o;
        private string _p;
        private Uri _u;
        private string _l;
        private string _r;
        public static B New()
        {
            return new B();
        }
        public B T(string t) { _t = t; return this; }
        public B O(string p) { _o = true; _p = p; return this; }
        public B U(string u) { _u = string.IsNullOrWhiteSpace(u) ? null : new Uri(u, UriKind.Absolute); return this; }
        public B F(string l, string r) { _o = false; _l = l; _r = r; return this; }
        public C Build()
        {
            if (string.IsNullOrWhiteSpace(_t)) throw new InvalidOperationException("Нужно имя");
            if (_o)
            {
                if (string.IsNullOrWhiteSpace(_p)) throw new InvalidOperationException("Нет платформы");
                return new OC(_t, _p, _u);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_l) || string.IsNullOrWhiteSpace(_r)) throw new InvalidOperationException("Нет данных локации");
                return new FC(_t, _l, _r);
            }
        }
    }
    public interface IR
    {
        void Add(C c);
        void Rem(Guid id);
        C Get(Guid id);
        IEnumerable<C> All();
    }
    public class MemR : IR
    {
        private readonly Dictionary<Guid, C> _d = new Dictionary<Guid, C>();
        public void Add(C c)
        {
            if (c == null) throw new ArgumentNullException("c");
            if (_d.ContainsKey(c.Id)) throw new InvalidOperationException("Уже есть");
            _d[c.Id] = c;
        }
        public void Rem(Guid id)
        {
            if (!_d.Remove(id)) throw new KeyNotFoundException("Курс не найден");
        }
        public C Get(Guid id)
        {
            C c;
            if (_d.TryGetValue(id, out c)) return c;
            return null;
        }
        public IEnumerable<C> All()
        {
            return _d.Values;
        }
    }
    public sealed class M
    {
        private readonly IR _r;
        public M() : this(new MemR()) { }
        public M(IR r)
        {
            if (r == null) throw new ArgumentNullException("r");
            _r = r;
        }
        public void AddC(C c)
        {
            if (c == null) throw new ArgumentNullException("c");
            _r.Add(c);
        }
        public void RemC(Guid id)
        {
            _r.Rem(id);
        }
        public void SetI(Guid cid, I i)
        {
            if (i == null) throw new ArgumentNullException("i");
            var c = _r.Get(cid);
            if (c == null) throw new KeyNotFoundException("Курс не найден");
            c.SetI(i);
        }
        public IEnumerable<C> ByI(Guid iid)
        {
            var res = new List<C>();
            foreach (var c in _r.All())
            {
                if (c.I != null && c.I.Id == iid) res.Add(c);
            }
            return res;
        }
        public void AddS(Guid cid, S s)
        {
            if (s == null) throw new ArgumentNullException("s");
            var c = _r.Get(cid);
            if (c == null) throw new KeyNotFoundException("Курс не найден");
            c.AddS(s);
        }
        public void RemS(Guid cid, Guid sid)
        {
            var c = _r.Get(cid);
            if (c == null) throw new KeyNotFoundException("Курс не найден");
            c.RemS(sid);
        }
        public C GetC(Guid id)
        {
            return _r.Get(id);
        }
        public IEnumerable<C> AllC()
        {
            return _r.All();
        }
    }
    class Program
    {
        static M m = new M();
        static void Main(string[] args)
        {
            Seed();
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Управление курсами ===");
                Console.WriteLine("1. Список курсов");
                Console.WriteLine("2. Добавить курс");
                Console.WriteLine("3. Удалить курс");
                Console.WriteLine("4. Назначить преподавателя");
                Console.WriteLine("5. Записать студента");
                Console.WriteLine("6. Отписать студента");
                Console.WriteLine("7. Показать детали курса");
                Console.WriteLine("8. Курсы преподавателя");
                Console.WriteLine("9. Переименовать курс");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                var o = Console.ReadLine();
                try
                {
                    switch (o)
                    {
                        case "1": ListAll(); break;
                        case "2": AddC(); break;
                        case "3": RemC(); break;
                        case "4": SetI(); break;
                        case "5": AddS(); break;
                        case "6": RemS(); break;
                        case "7": ShowC(); break;
                        case "8": ShowByI(); break;
                        case "9": RenC(); break;
                        case "0": return;
                        default: Console.WriteLine("Неизвестная команда"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.GetType().Name + " - " + ex.Message);
                }
            }
        }
        static void Seed()
        {
            var c1 = B.New().T("Intro to C#").O("ExamplePlatform").U("https://example.org/csharp").Build();
            var c2 = B.New().T("Discrete Math").F("Main Campus", "201").Build();
            m.AddC(c1); m.AddC(c2);
            var ii = new I("Dr. Ivanov");
            m.SetI(c1.Id, ii);
            m.SetI(c2.Id, ii);
            m.AddS(c1.Id, new S("Olga"));
            m.AddS(c2.Id, new S("Petr"));
        }
        static void ListAll()
        {
            Console.WriteLine();
            foreach (var c in m.AllC()) Console.WriteLine(c.ToString());
        }
        static void AddC()
        {
            Console.Write("Название: ");
            var t = Console.ReadLine();
            Console.Write("Тип (1-онлайн, 2-офлайн): ");
            var tp = Console.ReadLine();
            if (tp == "1")
            {
                Console.Write("Платформа: ");
                var p = Console.ReadLine();
                Console.Write("URL (опционально): ");
                var u = Console.ReadLine();
                var cb = B.New().T(t).O(p);
                if (!string.IsNullOrWhiteSpace(u)) cb.U(u);
                var c = cb.Build();
                m.AddC(c);
                Console.WriteLine("Добавлено: " + c.Id);
            }
            else
            {
                Console.Write("Место: ");
                var loc = Console.ReadLine();
                Console.Write("Аудитория: ");
                var rm = Console.ReadLine();
                var c = B.New().T(t).F(loc, rm).Build();
                m.AddC(c);
                Console.WriteLine("Добавлено: " + c.Id);
            }
        }
        static void RemC()
        {
            var id = ReadGuid("Id курса для удаления: ");
            m.RemC(id);
            Console.WriteLine("Удалено.");
        }
        static void SetI()
        {
            var id = ReadGuid("Id курса: ");
            Console.Write("Имя преподавателя: ");
            var n = Console.ReadLine();
            var i = new I(n);
            m.SetI(id, i);
            Console.WriteLine("Преподаватель назначен: " + i.Id);
        }
        static void AddS()
        {
            var id = ReadGuid("Id курса: ");
            Console.Write("Имя студента: ");
            var n = Console.ReadLine();
            var s = new S(n);
            m.AddS(id, s);
            Console.WriteLine("Студент добавлен: " + s.Id);
        }
        static void RemS()
        {
            var id = ReadGuid("Id курса: ");
            var sid = ReadGuid("Id студента: ");
            m.RemS(id, sid);
            Console.WriteLine("Отписан.");
        }
        static void ShowC()
        {
            var id = ReadGuid("Id курса: ");
            var c = m.GetC(id);
            if (c == null) { Console.WriteLine("Курс не найден"); return; }
            Console.WriteLine(c.ToString());
            Console.WriteLine("Студенты:");
            foreach (var s in c.Ss) Console.WriteLine(" - " + s.N + " (" + s.Id + ")");
            var oc = c as OC;
            if (oc != null)
            {
                Console.WriteLine("Платформа: " + oc.P);
                Console.WriteLine("URL: " + (oc.U == null ? "(нет)" : oc.U.ToString()));
            }
            var fc = c as FC;
            if (fc != null)
            {
                Console.WriteLine("Место: " + fc.L);
                Console.WriteLine("Аудитория: " + fc.R);
            }
        }
        static void ShowByI()
        {
            var id = ReadGuid("Id преподавателя: ");
            var list = m.ByI(id).ToList();
            if (list.Count == 0) Console.WriteLine("Курсы для этого преподавателя не найдены.");
            foreach (var c in list) Console.WriteLine(" - " + c.T + " (" + c.Id + ")");
        }
        static void RenC()
        {
            var id = ReadGuid("Id курса: ");
            Console.Write("Новое название: ");
            var t = Console.ReadLine();
            var c = m.GetC(id);
            if (c == null) { Console.WriteLine("Курс не найден"); return; }
            c.Ren(t);
            Console.WriteLine("Переименовано.");
        }
        static Guid ReadGuid(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                Guid g;
                if (Guid.TryParse(s, out g)) return g;
                Console.WriteLine("Неверный GUID, попробуйте снова.");
            }
        }
    }
}
