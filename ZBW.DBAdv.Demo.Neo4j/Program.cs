using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace dotnet
{
    class Example
    {
        static async Task Main()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));

           var cypherQuery = @"MATCH (people:Person) RETURN people.name LIMIT 10";

            //Create 
            /*var cypherQuery = @"CREATE (ZbwMovie:Movie {title:'ZbW Movie', released:2022, tagline:'Welcome to the ZBW'})
                    CREATE (Kehl:Person {name:'Thomas Kehl', born:1964})
                    CREATE (Angelo:Person {name:'Angelo Lütolf', born:1967})
                    CREATE (Corina:Person {name:'Corina Widmer', born:1961})
                    CREATE (Thomas:Person {name:'Thomas Solenthaler', born:1961})
                    CREATE
                    (Kehl)-[:ACTED_IN {roles:['Leher']}]->(ZbwMovie),
                    (Angelo)-[:ACTED_IN {roles:['Schüler 1']}]->(ZbwMovie),
                    (Corina)-[:ACTED_IN {roles:['Schüler 2']}]->(ZbwMovie),
                    (Thomas)-[:DIRECTED]->(ZbwMovie)";
            */

            // Finde
            //var findQuery = @"MATCH (cloudAtlas {title: "Cloud Atlas"}) RETURN cloudAtlas";

            // Delete

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.ReadTransactionAsync(async tx => {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync(); 
            });

            

            // Query
            await session?.CloseAsync();
            foreach (var row in result)
                Console.WriteLine(row["people.name"]);
                //Console.WriteLine(row["Person"].As<string>());

        }
    }
}