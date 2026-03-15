static class Nastroje {
    public static string? CeskeDny(string denDnes) {
        switch (denDnes) {
        case "Monday":
            return "pondělí";
        case "Tuesday":
            return "úterý";
        case "Wednesday":
            return "středa";
        case "Thursday":
            return "čtvrtek";
        case "Friday":
            return "pátek";
        case "Saturday":
            return "sobota";
        case "Sunday":
            return "neděle";
        default:
            return null;
        }
    }
    private static int vypocetMesice(int mesic, int rok) {
        switch (mesic) {
        case 1:
        case 3:
        case 5:
        case 7:
        case 8:
        case 10:
        case 12:
            return 31;
        case 4:
        case 6:
        case 9:
        case 11:
            return 30;
        case 2:
            if ((rok % 4 == 0 && rok % 100 != 0) || rok % 400 == 0) return 29;
            else return 28;
        default:
            // Unreachable
            return 0;
        }
    }
    public static string kalendar(string datum, DateTime dnes) {
        int rok = dnes.Year;
        int mesic = dnes.Month;
        int den = 0;
        if (datum.Length == 8) {
            try {
               string rokStr = datum.Substring(0,4);
               rok = Convert.ToInt32(rokStr);
               if (rok == 0) throw new Exception();
               datum = datum.Substring(4);
            } catch (Exception) {
                Console.WriteLine("ERROR: Nesprávné zadání!");
                return String.Empty;
            }
        }
        if (datum.Length == 6) {
           try {
               string rokDvaStr = datum.Substring(0,2);
               int rokDva = Convert.ToInt32(rokDvaStr);
               rok = 2000 + rokDva;
               datum = datum.Substring(2);
            } catch (Exception) {
                Console.WriteLine("ERROR: Nesprávné zadání!");
                return String.Empty;
            }
        }
        if (datum.Length == 4) {
           try {
               string mesStr = datum.Substring(0,2);
               int m = Convert.ToInt32(mesStr);
               if (m == 0) throw new Exception();
               if (m > 12) throw new Exception();
               mesic = m;
               datum = datum.Substring(2);
            } catch (Exception) {
                Console.WriteLine("ERROR: Nesprávné zadání!");
                return String.Empty;
            }
        }
        int kolikDniMesic = vypocetMesice(mesic, rok);
        if (datum.Length == 2) {
            try {
                int d = Convert.ToInt32(datum);
                if (d == 0) throw new Exception();
                if (d > kolikDniMesic) throw new Exception();
                den = d;
            } catch (Exception) {
                Console.WriteLine("ERROR: Nesprávné zadání!");
                return String.Empty;
            }
        }
        return String.Concat(rok.ToString("0000"), '-', mesic.ToString("00"), '-', den.ToString("00"));
    }
}

class Program {
    static void Main(String[] args) {
        DateTime dnes = DateTime.Today;
        string dnesStr = dnes.ToString("yyyy-MM-dd").Split(" ")[0];
        int den = dnes.Day;
        int mesic = dnes.Month;
        string denDnes = Nastroje.CeskeDny(dnes.DayOfWeek.ToString()) ?? String.Empty;
        string svatek = String.Empty;
        bool nalezenMesic = false;
        int pocetDni = 1;
        foreach (string radka in File.ReadAllLines(@"jmena.txt")) {
            if (nalezenMesic) {
                if (pocetDni++ == den) {
                    svatek = radka;
                    break;
                }
                continue;
            }
            if (radka == mesic.ToString()) nalezenMesic = true;
        }

        Console.WriteLine($"Dnes je {denDnes} {dnesStr}, svátek slaví {svatek}");
        string kalendar = String.Empty;
        while (kalendar != "q" || kalendar.Length != 1) {
            Console.Write("Kalendář: ");
            kalendar = Console.ReadLine() ?? String.Empty;
            if (String.IsNullOrEmpty(kalendar)) continue;
            kalendar = kalendar.Trim(' ');
            if (kalendar[0] == 'l' && kalendar.Length == 1) {
                try {
                    foreach (string radka in File.ReadAllLines(@"list.txt")) {
                        Console.WriteLine(radka);
                    }
                } catch (Exception) {
                    Console.WriteLine("Nemáte v kalendáři nic napsáno!");
                }
            }
            if (kalendar[0] == 'w') {
                string command = kalendar.Substring(1);
                string[] datumObsah = command.Split(" ");
                if (datumObsah.Length < 2) {
                    Console.WriteLine("ERROR: Nesprávné zadání!");
                    continue;
                }
                string datum = datumObsah[0];
                string obsah = String.Empty;
                for (int i = 1; i < datumObsah.Length; i++)
                    obsah += datumObsah[i] + ' ';
                if (datum == String.Empty || datum.Length > 1 && datum.Length % 2 == 0 && datum.Length < 9) {
                    if (datum != String.Empty) {
                        string writeDatum = Nastroje.kalendar(datum, dnes);
                        if (!String.IsNullOrEmpty(writeDatum)) {
                            using (StreamWriter outputFile = new StreamWriter("list.txt", true)) {
                                outputFile.WriteLine(String.Concat(writeDatum, ": ", obsah));
                            }
                            Console.WriteLine(String.Concat(writeDatum, ": ", obsah));
                        }
                    } else {
                        using (StreamWriter outputFile = new StreamWriter("list.txt", true)) {
                            outputFile.WriteLine(String.Concat(dnesStr, ": ", obsah));
                        }
                        Console.WriteLine(String.Concat(dnesStr, ": ", obsah));
                    }
                } else {
                    Console.WriteLine("ERROR: Nesprávné zadání!");
                }
            }
        }
    }
}
