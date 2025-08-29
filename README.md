# Semantic web / Knowledge graph workshop
A work in progress repo for the knowledge graph workshop

## Content of the repo

```
Semantic-Web-Workshop/
    Data/               # Data used in the workshop
    Huldra/
       Original docs/   # The original documentation of the Huldra asset
       RDF data/        # AI generated RDF representations of the various Huldra data
    Presentations/      # Presentation materials for the workshop
    Queries/            # SPARQL queries used in the workshop
    Tools/              # Tools used in the workshop
        JDK/                # Installation instructions for JDK needed for running fuseki and reasoner
        Fuseki/             # Installation instructions and configurations to run Fuseki
        Protégé/            # Instructions for downloading Protégé
```

## Preparations

The following things should be taken care of before the workshop:

- Install or verify installation of JDK
    - Open terminal and run
    ```
    java
    ```

- Clone this repository
- Download and set up Apache Jena Fuseki
- Download Protégé

*Note:* Additional preparations will be added as more of the workshop content becomes available.

## Additional resources

- [RDF Concept Vocabulary](https://www.w3.org/1999/02/22-rdf-syntax-ns#)    
- [RDF 1.1 Primer](https://www.w3.org/TR/rdf11-primer/)
- [Turtle - Terse RDF Triple Language](https://www.w3.org/TR/turtle/)
- [SPARQL 1.1 Query Language](https://www.w3.org/TR/sparql11-query/)
- [RDF Schema 1.1](https://www.w3.org/TR/rdf-schema/)
- [OWL 2 Web Ontology Language Document Overview](https://www.w3.org/TR/owl2-overview/)
- [SHACL - Shapes Constraint Language](https://www.w3.org/TR/shacl/)

## Agenda

**Saturday:**
**09:00 – 09:15 – Welcome**  
We’ll need a cup of coffee to wake up.  

**09:15 – 09:30 – Knowledge Graphs as a Strategic Focus at Webstep**  
What do we actually mean by a knowledge graph? And why is Webstep working on this?  

**09:30 – 10:00 – Resource Description Framework (RDF)**  
An introduction to triples, graphs, and the dataset we’ll be using. The data comes from Equinor’s Huldra field. The field was shut down years ago and turned into scrap metal, but Equinor has released data that can be used for research and development.  

**10:00 – 10:10 – Coffee break**  
Because one cup is never enough.  

**10:10 – 10:30 – Terse RDF Triple Language (Turtle)**  
The format we’ll use to write down triples. The Huldra data is expressed in Turtle.  

**10:30 – 11:00 – Apache Jena Fuseki**  
We’ll set up a Fuseki instance to store our graphs. Fuseki is an open-source database for working with triples – also known as a triplestore.  

**11:00 – 12:00 – SPARQL Protocol and RDF Query Language (SPARQL)**  
Everything else has just been foreplay – this is where the real fun begins. SPARQL is RDF’s query language, and we’ll be using it for most of the weekend.  

**12:00 – 13:00 – Lunch**  
Right when it was getting fun.  

**13:00 – 14:30 – Resource Description Framework Schema (RDFS)**  
RDFS is a small extension to RDF that lets us introduce logical hierarchies between classes and relational constraints. Based on these, a triplestore can reason and infer explicit facts from data that are only implicit.  
We’ll first do the reasoning with SPARQL, and then check if we get the same results when reasoning is turned on in Fuseki.  

**14:30 – 14:40 – Coffee break**  
There’s no such thing as too much coffee.  

**14:40 – 15:10 – Web Ontology Language (OWL) – Part 1**  
With RDFS we can build simple hierarchies. OWL has a richer logical foundation and much greater expressive power. This makes it possible for a triplestore (or another reasoner) to explicitly state information that is only implicit in the data. Once again, we’ll try this out with SPARQL first before checking the built-in reasoning in Fuseki.  

**15:10 – 16:30 – Web Ontology Language (OWL) – Part 2**  
You can’t cover OWL in an hour (not in two either).  

**Dinner**  

---

**Sunday:**
**09:00 – 09:15 – Welcome**  
Coffee. More coffee. Even more coffee.  

**09:15 – 10:00 – Web Ontology Language (OWL) – Part 2**  
We can’t possibly cover OWL in just a couple of hours.  

**10:00 – 10:10 – Coffee break**  
Yes please.  

**10:10 – 12:00 – Logical Reasoning**  
The reasoning we saw with RDFS and the first part of OWL is rule-based reasoning. Now we’ll take it a step further and look at richer logical reasoning.  

**12:00 – 13:00 – Lunch**  
Finally.  

**13:00 – 14:00 – Shapes Constraint Language (SHACL)**  
Over the weekend we’ll (hopefully) have noticed that RDF/RDFS/OWL aren’t particularly good at validating data – SHACL is designed for that.  

**14:00 – 14:30 – Information Modelling Framework (IMF)**  
IMF is a relatively new (and still unfinished) framework for linking together different kinds of data about the same thing. It has gained some traction in oil and gas, and is an important component in DISC (see after the coffee break).  

**14:30 – 14:40 – Coffee break**  
Woohoo!  

**14:40 – 15:30 – DISC and the Road Ahead**  
DISC is a collaboration between Equinor, Aker BP, Aker Solutions and Aibel with the goal of solving interoperability problems in the industry. In December they’re hosting another Show & Tell, and Webstep wants to build a proof-of-concept for a digital plant model based on IMF.  

**15:30 – 16:00 – Wrap-up**