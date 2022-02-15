package main

import (
	"context"
	"log"
	"net/http"
	"os"

	"github.com/99designs/gqlgen/graphql"
	"github.com/99designs/gqlgen/graphql/handler"
	"github.com/99designs/gqlgen/graphql/playground"
	"github.com/go-chi/chi"
	"new-talents.fr/buildup/graph"
	"new-talents.fr/buildup/graph/generated"
	"new-talents.fr/buildup/internal/auth"
	"new-talents.fr/buildup/internal/config"
	"new-talents.fr/buildup/internal/database"
	"new-talents.fr/buildup/internal/users"
)

const defaultPort = "8083"

func main() {
	// Rooter initialization
	port := os.Getenv("PORT")
	if port == "" {
		port = defaultPort
	}

	router := chi.NewRouter()
	router.Use(auth.Middleware())

	// Config initialization
	configPath := "config.yml"
	if os.Getenv("APP_ENV") == "production" {
		configPath = "config.production.yml"
	}

	config.Init(configPath)

	// Database initialization
	database.Init()

	c := generated.Config{Resolvers: &graph.Resolver{}}
	c.Directives.NeedAuthentication = func(ctx context.Context, obj interface{}, next graphql.Resolver) (interface{}, error) {
		if auth.ForContext(ctx) == nil {
			return nil, &users.UserAccessDeniedError{}
		}

		return next(ctx)
	}

	srv := handler.NewDefaultServer(generated.NewExecutableSchema(c))

	router.Handle("/", playground.Handler("GraphQL playground", "/query"))
	router.Handle("/query", srv)

	log.Printf("connect to http://localhost:%s/ for GraphQL playground", port)
	log.Fatal(http.ListenAndServe(":"+port, router))
}
