## Local Fuseki
Download the Fuseki server from https://jena.apache.org/download/

Unzip the downloaded file to a folder of your choice.

The following Fuseki configuration files can be found in /configuration folder:
- conf-with-reasoning.ttl
- conf-without-reasoning.ttl

Start the server by running the following command in the Fuseki folder:
```
./fuseki-server --config <config-file.ttl>
```