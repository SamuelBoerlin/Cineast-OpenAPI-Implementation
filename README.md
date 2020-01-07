# Cineast OpenAPI Implementation
A simple implementation of the Cineast OpenAPI specification in C# capable of running queries.

## Setup

1. Create the Cineast OpenAPI C# source code of the `cineast-openapi-spec.json` api specifications by running `generate.bat` in `swagger-codegen`.
2. Build the Cineast OpenAPI assemblies by running `build.bat` in `swagger-codegen/client`.
3. Open the VS project in `c# implementation/Project` and build the program.

## Generating the Cineast OpenAPI specifications
This repository already has a manually tweaked and functional specification called `cineast-openapi-spec.json`.  
If you want to (re-)generate the specifications:
1. Build the modified spark-swagger fork by running `mvnw compile` and `mvnw package` in `spark-swagger`. The next step will require the built jar file.
2. Proceed in `cineast-openapi` according to Cineast's [README](https://github.com/SamuelBoerlin/cineast/blob/af1933c5d2d99ad557621e254c8313a31484c709/README.md) and
build the OpenAPI specification.
3. You'll find the specification in `cineast-openapi/docs/swagger.json`. This generated specification will not be complete
yet and requires some manual tweaks and fixes.
