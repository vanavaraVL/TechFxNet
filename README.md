# TechFxNet
Technical Assignment

## Architecture
The architecture of the solution was chosen as known as Clear Architecture which has to do with DDD arhitecture.
The arhitecture could be introduced as a full DDD concept or service layouts by Martin Fauler.

The main concept of Clear Architecture are the following:
- Domain - domain area of entities and DTO
- Application - CQRS handlers for commands and queries
- Infrastructure - repositories and database specific stuff

The design of core architecture was chosen as a CQRS pattern.
As a mapper the core was chosen AutoMapper.

WepAPI uses custom middleware to handle all kind of exception from application's exception hierarchy

## Tech
WebAPI Core 8 project introduces RESTFull application

The Web application is available [here](./src/TechFxNet.Web)

## Tests
For unit test project was chosen NUnit project and it uses AutoFixture framework for Moq and DI containers

The unit tests are available [here](./tests/TechFxNex.UnitTests)

# Overview
Create an ASP.NET Core 8 application with REST API. You do not need to create a UI.
The application must use one of the following databases via code-first approach: PostgreSQL (preferred), MS
SQL, or MySQL.
Database Design
1. Nodes of Independent Trees:
- Each node must belong to a single tree.
- All child nodes must belong to the same tree as their parent.
- Each node must have a mandatory name field.
- You may add any additional fields necessary for ensuring tree independence.

2. Exception Journal:
- Track all exceptions during REST API request processing.
- Each journal record must store:
 - Unique Event ID
 - Timestamp
 - All query/body parameters
 - Stack trace of the exception
API Requirements
Your REST API should replicate (as closely as possible) the structure provided in the example Swagger file
(see attached Swagger JSON). Improvements are welcome, as long as the core concept is maintained.

Exception Handling
Define a custom exception class named SecureException.
If a SecureException (or its subclass) is thrown during request processing:
- Store all details in the journal
- Respond with HTTP 500 and this format:
 {"type": "name of exception", "id": "id of event", "data": {"message": "message of exception"}}
Example:
 {"type": "Secure", "id": "638136064526554554", "data": {"message": "You have to delete all children nodes
first"}}
For all other exceptions:
- Log full details in the journal
- Respond with HTTP 500 and:
 {"type": "Exception", "id": "id of event", "data": {"message": "Internal server error ID = id of event"}}
Example:
 {"type": "Exception", "id": "638136064187111634", "data": {"message": "Internal server error ID =
638136064187111634"}}

## JSON OpenAPI

```json
{
  "swagger": "2.0",
  "info": {
    "title": "Swagger",
    "version": "0.0.1",
  },
  "tags": [
    {
      "name": "user.journal",
      "description": "Represents journal API"
    },
    {
      "name": "user.partner",
      "description": ""
    },
    {
      "name": "user.tree",
      "description": "Represents entire tree API"
    },
    {
      "name": "user.tree.node",
      "description": "Represents tree node API"
    }
  ],
  "paths": {
    "/api.user.journal.getRange": {
      "post": {
        "summary": "",
        "description": "Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional. ",
        "tags": [
          "user.journal"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "skip",
            "required": true,
            "type": "integer",
            "format": "int32"
          },
          {
            "in": "query",
            "name": "take",
            "required": true,
            "type": "integer",
            "format": "int32"
          },
          {
            "in": "body",
            "name": "filter",
            "required": false,
            "schema": {
              "type": "object"
            }
          }
        ],
        "responses": {
          "200": {
            "schema": {
              "$ref": "#/definitions/FxNet.Web.Model.MRange_MJournalInfo"
            },
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.journal.getSingle": {
      "post": {
        "summary": "",
        "description": "Returns the information about an particular event by ID.",
        "tags": [
          "user.journal"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "id",
            "required": true,
            "type": "integer",
            "format": "int64"
          }
        ],
        "responses": {
          "200": {
            "schema": {
              "$ref": "#/definitions/FxNet.Web.Def.Api.Diagnostic.Model.MJournal"
            },
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.partner.rememberMe": {
      "post": {
        "summary": "",
        "description": "",
        "tags": [
          "user.partner"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "code",
            "required": true,
            "type": "string",
            "format": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.tree.get": {
      "post": {
        "summary": "",
        "description": "Returns your entire tree. If your tree doesn't exist it will be created automatically.",
        "tags": [
          "user.tree"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "treeName",
            "required": true,
            "type": "string",
            "format": "string"
          }
        ],
        "responses": {
          "200": {
            "schema": {
              "$ref": "#/definitions/ReactTest.Tree.Site.Model.MNode"
            },
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.tree.node.create": {
      "post": {
        "summary": "",
        "description": "Create a new node in your tree. You must to specify a parent node ID that belongs to your tree. A new node name must be unique across all siblings.",
        "tags": [
          "user.tree.node"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "treeName",
            "required": true,
            "type": "string",
            "format": "string"
          },
          {
            "in": "query",
            "name": "parentNodeId",
            "required": true,
            "type": "integer",
            "format": "int64"
          },
          {
            "in": "query",
            "name": "nodeName",
            "required": true,
            "type": "string",
            "format": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.tree.node.delete": {
      "post": {
        "summary": "",
        "description": "Delete an existing node in your tree. You must specify a node ID that belongs your tree.",
        "tags": [
          "user.tree.node"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "treeName",
            "required": true,
            "type": "string",
            "format": "string"
          },
          {
            "in": "query",
            "name": "nodeId",
            "required": true,
            "type": "integer",
            "format": "int64"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response"
          }
        }
      }
    },
    "/api.user.tree.node.rename": {
      "post": {
        "summary": "",
        "description": "Rename an existing node in your tree. You must specify a node ID that belongs your tree. A new name of the node must be unique across all siblings.",
        "tags": [
          "user.tree.node"
        ],
        "parameters": [
          {
            "in": "query",
            "name": "treeName",
            "required": true,
            "type": "string",
            "format": "string"
          },
          {
            "in": "query",
            "name": "nodeId",
            "required": true,
            "type": "integer",
            "format": "int64"
          },
          {
            "in": "query",
            "name": "newNodeName",
            "required": true,
            "type": "string",
            "format": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response"
          }
        }
      }
    }
  },
  "definitions": {
    "FxNet.Web.Def.Api.Diagnostic.Model.MJournal": {
      "properties": {
        "text": {
          "type": "string",
          "format": "string"
        },
        "id": {
          "type": "integer",
          "format": "int64"
        },
        "eventId": {
          "type": "integer",
          "format": "int64"
        },
        "createdAt": {
          "type": "string",
          "format": "datetime",
          "example": "2025-05-23T12:18:16.9222634Z"
        }
      },
      "required": [
        "text",
        "id",
        "eventId",
        "createdAt"
      ]
    },
    "FxNet.Web.Def.Api.Diagnostic.Model.MJournalInfo": {
      "properties": {
        "id": {
          "type": "integer",
          "format": "int64"
        },
        "eventId": {
          "type": "integer",
          "format": "int64"
        },
        "createdAt": {
          "type": "string",
          "format": "datetime",
          "example": "2025-05-23T12:18:16.922346Z"
        }
      },
      "required": [
        "id",
        "eventId",
        "createdAt"
      ]
    },
    "FxNet.Web.Def.Api.Diagnostic.View.VJournalFilter": {
      "properties": {
        "from": {
          "type": "string",
          "format": "datetime",
          "example": "2025-05-23T12:18:16.9223615Z"
        },
        "to": {
          "type": "string",
          "format": "datetime",
          "example": "2025-05-23T12:18:16.9223726Z"
        },
        "search": {
          "type": "string",
          "format": "string"
        }
      },
      "required": [
        "search"
      ]
    },
    "ReactTest.Tree.Site.Model.MNode": {
      "properties": {
        "id": {
          "type": "integer",
          "format": "int64"
        },
        "name": {
          "type": "string",
          "format": "string"
        },
        "children": {
          "type": "array",
          "items": {
            "$ref": "#/definitions/ReactTest.Tree.Site.Model.MNode"
          }
        }
      },
      "required": [
        "id",
        "name",
        "children"
      ]
    },
    "FxNet.Web.Model.MRange_MJournalInfo": {
      "properties": {
        "skip": {
          "type": "integer",
          "format": "int32"
        },
        "count": {
          "type": "integer",
          "format": "int32"
        },
        "items": {
          "type": "array",
          "items": {
            "$ref": "#/definitions/FxNet.Web.Def.Api.Diagnostic.Model.MJournalInfo"
          }
        }
      },
      "required": [
        "skip",
        "count",
        "items"
      ]
    }
  }
}
```