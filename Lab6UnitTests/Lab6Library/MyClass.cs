using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6Library
{
    public class MyClass
    {

        /// <summary>
        /// generic database interaction, returns number^2 and logs something in db
        /// </summary>
        /// <param name="number"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int DoDbStuff(int number, DatabaseConnection connection)
        {
            var result = number * number;

            connection.Execute(string.Format("INSERT {0} INTO log", number));

            return result;
        }

        /// <summary>
        /// if sat/sun returns next monday
        /// return date otherwise
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime LeanForward(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
                return date.AddDays(2);
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return date.AddDays(1);
            return date;
        }

        /// <summary>
        /// next day that is not sun/sat
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime NextBusinessDay(DateTime date)
        {
            return LeanForward(date.AddDays(1));
        }
        

        /// <summary>
        /// interaction with external dependency
        /// return enumerable with powers of given number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="powers">max number of returned powers</param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IEnumerable<double> DoArrayStuffWithIface(double number, int powers, IMathService service)
        {
            return Enumerable.Range(1, powers)
                .Select(power => service.Power(number, power))
                .ToArray();
        }

    }
}
