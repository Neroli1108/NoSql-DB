using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
    public static class QueryEngine
    {
        public static Key[] QueryByKey<Key,Value>(this DBEngine<Key,Value>db ,Func<Key, bool> pred)
        { return db.dbStore.Keys.Where(pred).ToArray(); }// Query by Judgement what the user inputs , if return ture, save keys which meets the requirement into the Collection(array).
        public static Key[] QueryByMetadata<Key, Value>(this DBEngine<Key,Value>db ,Func<Value, bool> pred)
        { return db.dbStore.
                Where(pair => pred(pair.Value)).//input the judgement about Value
                Select(pair => pair.Key).ToArray(); }//output the bool and save all True Keys into Array
                                                    // Query By specify Judgements in the key what the user inputs , if return ture, save keys which meets the requirement into the Collection(array).
    }
}


static class TestSchelduler
{
    static void Main(string[] args)
    {

        Write("\n  All testing of Query class moved to QueryTest package.");
        

        Write("\n\n");
    }
}
