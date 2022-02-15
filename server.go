package main

import (
	"log"
	"net/http"
	"os"

	"github.com/99designs/gqlgen/graphql/handler"
	"github.com/99designs/gqlgen/graphql/playground"
	"new-talents.fr/buildup/graph"
	"new-talents.fr/buildup/graph/generated"
	"new-talents.fr/buildup/internal/config"
	"new-talents.fr/buildup/internal/database"
)

const defaultPort = "8083"

func main() {
	port := os.Getenv("PORT")
	if port == "" {
		port = defaultPort
	}

	// Config initialization
	configPath := "config.yml"
	if os.Getenv("APP_ENV") == "production" {
		configPath = "config.production.yml"
	}

	config.Init(configPath)

	// Database initialization
	database.Init()

	srv := handler.NewDefaultServer(generated.NewExecutableSchema(generated.Config{Resolvers: &graph.Resolver{}}))

	http.Handle("/", playground.Handler("GraphQL playground", "/query"))
	http.Handle("/query", srv)

	log.Printf("connect to http://localhost:%s/ for GraphQL playground", port)
	log.Fatal(http.ListenAndServe(":"+port, nil))
}