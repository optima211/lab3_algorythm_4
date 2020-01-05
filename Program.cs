using System;

namespace lab3_algorythm_4
{
    internal static class Program
    {
        /// <summary>
        /// Znachenie lambdi po umolchaniyu.
        /// </summary>
        private const double LambdaCoef = 0.1;
        /// <summary>
        /// Znachenie tochnosti, kotoroe neobhodimo dostich.
        /// </summary>
        private const double AccuracyCoef = 0.0001;
        /// <summary>
        /// Minimalnoe znachenie iz oblasti dopustimih resheniy.
        /// </summary>
        private const int MinRandomValue = 0;
        /// <summary>
        /// Maksimalnoe znachenie iz oblasti dopustimih resheniy.
        /// </summary>
        private const int MaxRandomValue = 10;
        /// <summary>
        /// Ekzemplyar klassa Random, dlya polucheniya sluchainogo chisla.
        /// </summary>
        private static Random _randomizer;

        /// <summary>
        /// Osnovnoy metod programmi.
        /// </summary>
        private static void Main()
        {
            #region Export to file
            ////Esli nujno sohranit rezultati v fail
            //var fileStream = new FileStream("output.txt", FileMode.Create);
            //var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
            //Console.SetOut(streamWriter);
            //Console.SetError(streamWriter);
            #endregion

            _randomizer = new Random();

            Console.WriteLine("Ishodnie dannie:");
            Console.WriteLine("f(x1,x2) = - (x1 - 9)^2 - (x2 - 6)^2 -> max");
            Console.WriteLine("5 * x1 + 6 * x2 <= 60");
            Console.WriteLine("4 * x1 + 9 * x2 <= 72");
            Console.WriteLine("x1;x2 >= 0");
            Console.WriteLine();

            double x1 = 0;
            double x2 = 0;
            var valuesCorrect = false;

            while (!valuesCorrect)
            {
                x1 = _randomizer.Next(MinRandomValue, MaxRandomValue);
                x2 = _randomizer.Next(MinRandomValue, MaxRandomValue);

                valuesCorrect = CheckValues(x1, x2);
            }

            var fResult = FuncF(x1, x2);

            Console.WriteLine("Vozmem lyuboe dopustimoe reshenie sistemi ogranicheniy v kachestve nachalnogo priblizheniya:");
            Console.WriteLine($"x1 = {x1}, x2 = {x2}");
            Console.WriteLine($"f({x1},{x2}) = {fResult}");
            Console.WriteLine();

            double alpha = 0;
            Console.WriteLine($"V kachestve nachalnogo shaga vichisleniy viberem 'lambda' = {LambdaCoef}; 'alpha' = {alpha}");
            Console.WriteLine("Vvedem oboznachenie:");
            Console.WriteLine("g1(x1,x2) = 60 - 5 * x1 - 6 * x2");
            Console.WriteLine("g2(x1,x2) = 72 - 4 * x1 - 9 * x2");
            Console.WriteLine();

            Console.WriteLine("Opredelim chastnie proizvodnie:");
            Console.WriteLine("fx1 = 18 - 2 * x1");
            Console.WriteLine("fx2 = 12 - 2 * x2");
            Console.WriteLine("g1x1 = -5");
            Console.WriteLine("g1x2 = -6");
            Console.WriteLine("g2x1 = -4");
            Console.WriteLine("g2x2 = -9");
            Console.WriteLine();

            var lastFResult = fResult;
            var lastGoodIteration = 0;
            var lastX1 = x1;
            var lastX2 = x2;
            var lastAlpha1 = alpha;
            var lastAlpha2 = alpha;
            var isAccuracyOk = false;
            var index = 0;

            while (!isAccuracyOk)
            {
                index++;

                Console.WriteLine($"Iteration {index}:");
                Console.WriteLine("--------------------");
                var newAlpha1 = GetAlpha1(lastX1, lastX2, lastAlpha1, LambdaCoef);
                Console.WriteLine($"'Alpha1' = {newAlpha1:F3}");

                var newAlpha2 = GetAlpha2(lastX1, lastX2, lastAlpha2, LambdaCoef);
                Console.WriteLine($"'Alpha2' = {newAlpha2:F3}");

                var newX1 = lastX1 + LambdaCoef * (FuncFByX1(lastX1) + newAlpha1 * FuncG1ByX1(lastX1) + newAlpha2 * FuncG2ByX1(lastX1));
                newX1 = newX1 < 0 ? 0 : newX1;

                var newX2 = lastX2 + LambdaCoef * (FuncFByX2(lastX2) + newAlpha1 * FuncG1ByX2(lastX2) + newAlpha2 * FuncG2ByX2(lastX2));
                newX2 = newX2 < 0 ? 0 : newX2;

                Console.WriteLine($"x1 = {newX1:F3}");
                Console.WriteLine($"x2 = {newX2:F3}");

                Console.WriteLine($"Polichili novuyuy tochku x{index} = ({newX1:F3}; {newX2:F3})");

                var newG1Result = FuncG1(newX1, newX2);
                if (newG1Result > 0)
                {
                    Console.WriteLine($"g1(x{index}) = {newG1Result:F3} > 0");
                    Console.WriteLine("Naidennaya tochka prinadlejit oblasti dopustimih resheniy G1.");

                    var newG2Result = FuncG2(newX1, newX2);
                    if (newG2Result > 0)
                    {
                        Console.WriteLine($"g2(x{index}) = {newG2Result:F3} > 0");
                        Console.WriteLine("Naidennaya tochka prinadlejit oblasti dopustimih resheniy G2.");

                        var newFResult = FuncF(newX1, newX2);
                        var calculatedAccuracy = Math.Abs(newFResult - lastFResult);

                        lastFResult = newFResult;

                        Console.WriteLine($"f(x{index}) = {newFResult:F3}");
                        Console.WriteLine($"Proverim dostizhenie trebuemoi tochnosti (sravnivaem so znacheniem funkcii v tochke, kotoraya posldney popadala v oblast):");
                        Console.WriteLine($"|f(x{index}) - f(x{lastGoodIteration})| = {calculatedAccuracy:F5}");

                        isAccuracyOk = calculatedAccuracy < AccuracyCoef;

                        if (!isAccuracyOk)
                        {
                            lastGoodIteration = index;
                            Console.WriteLine($"Tochnost menshe chem {AccuracyCoef} ne dostignuta. Prodoljaem iteracionniy process.");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"g2(x{index}) = {newG2Result:F3} < 0");
                        Console.WriteLine("Naidennaya tochka ne prinadlejit oblasti dopustimih resheniy G2.");
                        Console.WriteLine();
                    }

                }
                else
                {
                    Console.WriteLine($"g1(x{index}) = {newG1Result:F3} < 0");
                    Console.WriteLine("Naidennaya tochka ne prinadlejit oblasti dopustimih resheniy G1.");
                    Console.WriteLine();
                }

                lastX1 = newX1;
                lastX2 = newX2;
                lastAlpha1 = newAlpha1;
                lastAlpha2 = newAlpha2;
            }
            Console.WriteLine($"Tochnost menshe chem {AccuracyCoef} dostignuta!");
            Console.WriteLine();
            Console.WriteLine("Rezultati:");
            Console.WriteLine($"Nomer iteracii = {index}.");
            Console.WriteLine($"Tochka x{index} = ({lastX1:F3}; {lastX2:F3})");

            Console.ReadKey();
        }

        /// <summary>
        /// Metod dlya polucheniya Alpha1 koefficienta dlya kajdoy novoi iteraciy.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="lastAlpha"></param>
        /// <param name="lambda"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static double GetAlpha1(double x1, double x2, double lastAlpha, double lambda)
        {
            var gResult = FuncG1(x1, x2);
            return gResult >= 0 ? 0 : lastAlpha - lambda * gResult;
        }

        /// <summary>
        /// Metod dlya polucheniya Alpha2 koefficienta dlya kajdoy novoi iteraciy.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="lastAlpha"></param>
        /// <param name="lambda"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static double GetAlpha2(double x1, double x2, double lastAlpha, double lambda)
        {
            var gResult = FuncG2(x1, x2);
            return gResult >= 0 ? 0 : lastAlpha - lambda * gResult;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii funkcii F(x1,x2).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncF(double x1, double x2)
        {
            return -Math.Pow(x1 - 9, 2) - Math.Pow(x2 - 6, 2);
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii F'(x1).
        /// </summary>
        /// <returns></returns>
        private static double FuncFByX1(double x1)
        {
            return 18 - 2 * x1;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii F'(x2).
        /// </summary>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncFByX2(double x2)
        {
            return 12 - 2 * x2;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii funkcii G1(x1,x2).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncG1(double x1, double x2)
        {
            return 60 - 5 * x1 - 6 * x2;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii G1'(x1).
        /// </summary>
        /// <param name="x1"></param>
        /// <returns></returns>
        private static double FuncG1ByX1(double x1)
        {
            return -5;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii G1'(x2).
        /// </summary>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncG1ByX2(double x2)
        {
            return -6;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii funkcii G2(x1,x2).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncG2(double x1, double x2)
        {
            return 72 - 4 * x1 - 9 * x2;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii G2'(x1).
        /// </summary>
        /// <param name="x1"></param>
        /// <returns></returns>
        private static double FuncG2ByX1(double x1)
        {
            return -4;
        }

        /// <summary>
        /// Metod dlya vichisleniya rezultata pri vipolnenii chastnoy proizvodnoy funkcii G2'(x2).
        /// </summary>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static double FuncG2ByX2(double x2)
        {
            return -9;
        }

        /// <summary>
        /// Metod dlya proverki vibrannih iznachalnih sluchainih znacheniy x1, x2.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        private static bool CheckValues(double x1, double x2)
        {
            if (x1 < 0 || x2 < 0) return false;

            var sum1 = 5 * x1 + 6 * x2;
            if (sum1 > 60) return false;

            var sum2 = 4 * x1 + 9 * x2;
            if (sum2 > 72) return false;

            return true;
        }
    }
}
