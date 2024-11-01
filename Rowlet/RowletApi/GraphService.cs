using System.Text.Json;
using System.Text.Json.Serialization;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using VDS.RDF.Shacl;

public static class GraphService
{
    private static UriNode RdfType = new UriNode(new("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
    private static UriNode OwlClass = new UriNode(new("http://www.w3.org/2002/07/owl#Class"));
    private static UriNode OwlNamedIndividual = new UriNode(new("http://www.w3.org/2002/07/owl#NamedIndividual"));
    private static UriNode OwlDatatypeProperty = new UriNode(new("http://www.w3.org/2002/07/owl#DatatypeProperty"));
    private static UriNode OwlObjectProperty = new UriNode(new("http://www.w3.org/2002/07/owl#ObjectProperty"));

    private static List<UriNode> OwlTerms = [OwlClass, OwlNamedIndividual, OwlDatatypeProperty, OwlObjectProperty];


    public static string ValidateStars(string rdfData)
    {
        var dataGraph = new Graph();
        var shapeGraph = new Graph();

        var parser = new TurtleParser();
        parser.Load(dataGraph, new StringReader(rdfData));
        parser.Load(shapeGraph, new StringReader(GetStarShape()));

        var validator = new ShapesGraph(shapeGraph);
        var report = validator.Validate(dataGraph);

        var failedResults = report.Results.Select(r => r.FocusNode.ToString()).ToList();

        return JsonSerializer.Serialize(failedResults);
    }

    public static string ConvertRdfToD3Json(string rdfData)
    {
        var graph = new Graph();
        var parser = new TurtleParser();
        parser.Load(graph, new StringReader(rdfData));

        var nodes = new List<Node>();
        var links = new List<Link>();

        foreach (Triple triple in graph.Triples)
        {
            //Fjerner rusk fra RDF og OWL
            if (triple.Subject.ToString().Contains("rdf") || triple.Subject.ToString().Contains("owl") ||
                triple.Subject.ToString().Contains("XML") || triple.Subject.ToString().Contains("_:b"))

            {
                continue;
            }

            if (triple.Predicate.ToString().Contains("sameAs") || triple.Predicate.ToString().Contains("equivalentClass"))
            {
                continue;
            }

            //Fjerner OWL typer, siden dette legges på som type på noden og bestemmer visning
            if (OwlTerms.Contains((BaseNode)triple.Object))
            {
                Console.WriteLine($"Skipped {triple.Object.ToString()}");
                continue;
            }

            //Fjerner label, siden dette legges på noden og det ikke trengs egne noder for dette.
            if (triple.Predicate.ToString().Contains("label"))
            {
                continue;
            }

            var subjectType = GetType(graph, triple.Subject);
            if (subjectType != null && OwlTerms.Contains(subjectType))
            {
                nodes.Add(new Node(triple.Subject.ToString(), GetLabel(graph, triple.Subject), subjectType.ToString()));
            }


            if (triple.Object.ToString().Contains("http://www.w3.org/2002/07/owl#Thing") ||
                    triple.Object.ToString().Contains("http://www.w3.org/2002/07/owl#Restriction") ||
                    triple.Object.ToString().Contains("http://www.w3.org/2000/01/rdf-schema#Resource") ||
                    triple.Object.ToString().Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#Property") ||
                    triple.Object.ToString().Contains("http://www.w3.org/2000/01/rdf-schema#Class") ||
                    triple.Object.ToString().Contains("http://www.w3.org/2001/XMLSchema#float") ||
                    triple.Object.ToString().Contains("http://www.w3.org/2002/07/owl#Property") ||
                    triple.Object.ToString().Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#List") ||
                    triple.Object.ToString().Contains("_:b")
                    )
            {
                continue;
            }
            var objectType = GetType(graph, triple.Object);
            if (objectType != null && (OwlTerms.Contains(objectType) || objectType.ToString().Contains("Literal")))
            {
                nodes.Add(new Node(triple.Object.ToString(), GetLabel(graph, triple.Object), objectType.ToString()));
            }

            if (triple.Subject.ToString() == triple.Object.ToString())
            {
                continue;
            }

            if (triple.Subject.ToString().Contains("autos") || triple.Object.ToString().Contains("autos") ||
                triple.Subject.ToString().Contains("_:b") || triple.Object.ToString().Contains("_:b"))
            {
                continue;
            }

            // Add link (predicate)
            links.Add(new Link(triple.Subject.ToString(), triple.Object.ToString(), GetPredicateLabel(triple.Predicate)));
        }

        // Remove duplicate nodes based on 'id'
        nodes = nodes.DistinctBy(node => node.Id).ToList();

        var result = new { nodes, links };
        var test = JsonSerializer.Serialize(result);
        return JsonSerializer.Serialize(result);
    }

    private static string GetLabel(Graph graph, INode node)
    {
        var labelTriple = graph.GetTriplesWithSubjectPredicate(node, new UriNode(new("http://www.w3.org/2000/01/rdf-schema#label"))).FirstOrDefault();

        if (labelTriple == null)
        {
            return node is IUriNode uriNode ? uriNode.Uri.Segments.Last() : node.ToString();
        }

        return labelTriple.Object.AsValuedNode().AsString();
    }

    private static INode? GetType(Graph graph, INode node)
    {
        var typeTriples = graph.GetTriplesWithSubjectPredicate(node, RdfType).ToList();

        var owlTypeNode = GetOwlType(typeTriples);

        if (owlTypeNode == null)
        {
            if (node.NodeType == NodeType.Literal)
            {
                return new UriNode(new("http://www.w3.org/2000/01/rdf-schema#Literal"));
            }

            return null;
        }

        return owlTypeNode;
    }

    private static INode? GetOwlType(List<Triple> typeTriples)
    {
        foreach (var typeTriple in typeTriples)
        {
            if (OwlTerms.Contains(typeTriple.Object))
            {
                return typeTriple.Object;
            }
        }

        return null;
    }

    private static string GetPredicateLabel(INode predicate)
    {
        if (predicate.ToString().Contains("label"))
        {
            return "label";
        }

        if (predicate.ToString().Contains("domain"))
        {
            return "domain";
        }

        if (predicate.ToString().Contains("subClassOf"))
        {
            return "subClassOf";
        }

        if (predicate.ToString().Contains("range"))
        {
            return "range";
        }

        if (predicate.ToString().Contains("type"))
        {
            return "type";
        }

        if (predicate.ToString().Contains("onProperty"))
        {
            return "onProperty";
        }

        return predicate is IUriNode uriNode ? uriNode.Uri.Segments.Last() : predicate.ToString();
    }


    private static string GetStarShape()
    {
        return @$"@prefix sh: <http://www.w3.org/ns/shacl#> .
                @prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
                @prefix star: <http://example.org/star-ontology/> .

                # Define the SHACL shape for validating Stars
                star:StarShape a sh:NodeShape ;
                sh:targetClass star:Star ;
                sh:property [
                    sh:path star:luminosity ;
                    sh:datatype xsd:decimal ;
                    sh:minCount 1 ;
                    sh:minInclusive 0.1 ;
                    sh:maxInclusive 30.0 ;
                ] .
        ";  
    }
}



public class Node(
    string id, string label, string type
    )
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
    [JsonPropertyName("label")]
    public string Label { get; set; } = label;
    [JsonPropertyName("type")]
    public string Type { get; set; } = type;
}

public class Link(
    string source, string target, string label = "NA"
)
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = source;
    [JsonPropertyName("target")]
    public string Target { get; set; } = target;
    [JsonPropertyName("label")]
    public string Label { get; set; } = label;
}


