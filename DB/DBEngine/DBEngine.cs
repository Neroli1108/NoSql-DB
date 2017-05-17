///////////////////////////////////////////////////////////////
// DBEngine.cs - define noSQL database                       //
// Ver 1.3                                                   //
// Application: Demonstration for CSE687-OOD, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Author:      Yunding Li                                   //
// Reference:   Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 * This class is a starter for the DBEngine package you need to create.
 * It doesn't implement many of the requirements for the db, e.g.,
 * It doesn't remove elements, doesn't persist to XML, doesn't retrieve
 * elements from an XML file, and it doesn't provide hook methods
 * for scheduled persistance.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
  * ver 1.3 : 8 oct 15
 *   added DeleteKey,DeleteValue,DeleteChildren,ReplaceInstance,EditText Function and Extensions methods
 *   and tests in test stub
 * ver 1.2 : 24 Sep 15
 * - removed extensions methods and tests in test stub
 * - testing is now done in DBEngineTest.cs to avoid circular references
 * ver 1.1 : 15 Sep 15
 * - fixed a casting bug in one of the extension methods
 * ver 1.0 : 08 Sep 15
 * - first release
 *
 */
//todo add placeholders for Shard
//todo add reference to class text XML content

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
    public class DBEngine<Key, Value>
    {
        public Dictionary<Key, Value> dbStore;
        public DBEngine()
        {
            dbStore = new Dictionary<Key, Value>();
        }
        public bool insert(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
                return false;
            dbStore[key] = val;
            return true;
        }
        public bool getValue(Key key, out Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                val = dbStore[key];
                return true;
            }
            val = default(Value);
            return false;
        }
        //delete the Key/Value pairs
        public bool DeleteKey(Key key)
        {
            if (dbStore.Keys.Contains<Key>(key))
            {
                dbStore.Remove(key);
                return true;
            }
           
            return false;

        }
        public bool DeleteValue(Key key)
        {
            if (dbStore[key] != null)
            {
                dbStore[key] = default(Value);//instantiate the value to its default value which means null.
                return true;
            }
            return true;
        }
        //delete the children
        public void EditChildren<Data>(DBEngine<Key,DBElement<Key,Data>>DB, Key key, DBElement<Key, Data> Elem)
        {
            DBElement<Key, Data> Telem;
            DB.getValue(key, out Telem);
           Telem.children =Elem.children ;
            DB.insert(key, Telem);
            

          
        }
    
        public bool EditText<Data>(DBElement<Key, Data> Elem)//this function is responsible to edit the Metadata of the value
        {
            Elem.name = "Yunding Li";
            Elem.descr = "Graduate@Syracuse University!";
            return true;
        }

        public bool ReplaceInstance(int key, DBEngine<int, DBElement<int, string>> dbStore)// this function is resonsible to Replace a Value with a new one
        {
            dbStore.DeleteKey(key);

            DBElement<int, string> elem5 = new DBElement<int, string>("Tim Duncan", "Stone");
            elem5.children = new List<int> {21};
            elem5.payload = "He is the greatest Power Forward.";
            elem5.timeStamp = DateTime.Now;
            dbStore.insert(key, elem5);
            return true;
        }

        public IEnumerable<Key> Keys()
        {
          return dbStore.Keys;
        }
  }

#if(TEST_DBENGINE)

  class TestDBEngine
  {
    static void Main(string[] args)
    {
      "Testing DBEngine Package".title('=');
      WriteLine();

      Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
