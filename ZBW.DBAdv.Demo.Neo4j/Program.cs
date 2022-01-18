using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace dotnet
{
    class Example
    {
        // Example No-async with IDisposable
        // https://neo4j.com/docs/dotnet-manual/current/get-started/#dotnet-driver-get-started-hello-world-example
        public static async Task Main()
        {
            await Create();
            await Find();
            await FindRelationship();
            await Update();
            //await Remove();
        }

        // Create Node and Relationshipts
        // https://neo4j.com/docs/cypher-manual/current/clauses/create/#create-create-a-node-with-a-label
        // https://neo4j.com/docs/cypher-manual/current/clauses/create/#create-relationships
        public static async Task Create()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));

            var cypherQuery = @"CREATE (ZbwMovie:Movie {title:'ZbW Movie', released:2022, tagline:'Welcome to the ZBW'})
                    CREATE (Kehl:Person {name:'Thomas Kehl', born:1964})
                    CREATE (Angelo:Person {name:'Angelo Lütolf', born:1967})
                    CREATE (Corina:Person {name:'Corina Widmer', born:1961})
                    CREATE (Thomas:Person {name:'Thomas Solenthaler', born:1961})
                    CREATE (WorkgroupZbW:Group {name:'Arbeitsgruppe ZbW', found:2021-01-01})
                    CREATE
                    (Kehl)-[:ACTED_IN {roles:['Leher']}]->(ZbwMovie),
                    (Angelo)-[:ACTED_IN {roles:['Schüler 1']}]->(ZbwMovie),
                    (Corina)-[:ACTED_IN {roles:['Schüler 2']}]->(ZbwMovie),
                    (Thomas)-[:DIRECTED]->(ZbwMovie),
                    (Angelo)-[:IN_GROUP]->(WorkgroupZbW),
                    (Corina)-[:IN_GROUP]->(WorkgroupZbW),
                    (Thomas)-[:IN_GROUP]->(WorkgroupZbW)";

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.WriteTransactionAsync(async tx =>
            {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync();
            });

            Console.WriteLine("CREATE");
        }

        // Find 10 People
        // https://neo4j.com/docs/cypher-manual/current/clauses/match/#get-all-nodes-with-label
        public static async Task Find()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));

            var cypherQuery = @"MATCH (people:Person) RETURN people.name LIMIT 10";

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.ReadTransactionAsync(async tx =>
            {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync();
            });

            await session?.CloseAsync();
            Console.WriteLine("FIND NODE");
            foreach (var row in result)
                Console.WriteLine(row["people.name"]);
            //Console.WriteLine(row["Person"].As<string>());
        }

        // Find Relationship Type
        // https://neo4j.com/docs/cypher-manual/current/clauses/match/#match-on-rel-type
        public static async Task FindRelationship()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));
            //var cypherQuery = @"MATCH (ZbwMovie {title: 'ZbW Movie'}) RETURN ZbwMovie.title";
            var cypherQuery = @"MATCH (WorkgroupZbW {name: 'Arbeitsgruppe ZbW'})<-[:IN_GROUP]-(groupmember) RETURN groupmember.name";

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.ReadTransactionAsync(async tx =>
            {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync();
            });

            await session?.CloseAsync();
            Console.WriteLine("FINDE RELATIONSHIP");
            foreach (var row in result)
                Console.WriteLine(row["groupmember.name"]);
        }

        // Update / SET
        // https://neo4j.com/docs/cypher-manual/current/clauses/set/
        public static async Task Update()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));
            var cypherQuery = @"MATCH (tom {name: 'Thomas Solenthaler'}) SET tom.children = 3 RETURN tom";
            //var cypherQuery = @"MATCH (ZbwMovie {title: 'ZbW Movie'}) RETURN ZbwMovie.title";

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.WriteTransactionAsync(async tx =>
            {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync();
            });

            await session?.CloseAsync();
            Console.WriteLine("UPDATE");
        }

        // Remove
        // https://neo4j.com/docs/cypher-manual/current/clauses/remove/#remove-remove-a-label-from-a-node
        public static async Task Remove()
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("zbw", "zbw"));

            //var cypherQuery = @"MATCH (n:Person {name: 'Thomas Solenthaler'}) DETACH DELETE n"; // Löscht Node mit allen Relationships
            //var cypherQuery = @"MATCH (n:Movie {title: 'ZbW Movie'}) DETACH DELETE n";
            var cypherQuery = @"MATCH (n:Group {name: 'Arbeitsgruppe ZbW'}) DETACH DELETE n";

            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var result = await session.WriteTransactionAsync(async tx =>
            {
                var r = await tx.RunAsync(cypherQuery);
                return await r.ToListAsync();
            });

            await session?.CloseAsync();
            Console.WriteLine("REMOVED");
        }
    }
}