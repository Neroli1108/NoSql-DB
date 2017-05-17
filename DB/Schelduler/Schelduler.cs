///////////////////////////////////////////////////////////////
// SChedulert.cs - save the db to xml at same intervel       //
// Ver 1.0                                                   //
// Application: save the db to xml at same intervel          //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Author:      Yunding Li,Syracuse University               //                                            
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *Meet requirment6  
 *
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *  DBElement.cs, DBEngine.cs, Persist.cs 
 * UtilityExtension.cs
 *  
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 3/Oct/ 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static System.Console;


namespace Project2Starter
{
    public static class TimerSave
    {


        public static Timer StartScheduler<Key, Data>(this DBEngine<Key, DBElement<Key, Data>> db, string path, int interval)
        {
            Console.WriteLine("Please wait for the Scheduler");
            var tm = new Timer();//instantiate a new  Timer type
            tm.Interval = interval;
            tm.AutoReset = true; 
            tm.Elapsed += (object source, ElapsedEventArgs e) => 
            {
                XmltoDB.SaveXEngine(db, path);// save the DB at specified Tiimeinterval
                Console.WriteLine("Scheduler triggered.");
            };
            tm.Start();
            return tm;    
        }
    }
}
#if(TestSchelduler)
static class TestSchelduler {
        static void Main(string[] args)
    {
        Write("\n  All testing of DBEngine class moved to TestScheduler package.");

    }
    }

#endif
