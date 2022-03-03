package database

import (
	"context"
	"log"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"new-talents.fr/buildup/internal/config"
)

var MongoContext = context.TODO()

var CollectionUsers *mongo.Collection
var CollectionBuilders *mongo.Collection
var CollectionCoachs *mongo.Collection
var CollectionProjects *mongo.Collection
var CollectionBuildOns *mongo.Collection
var CollectionBuildOnSteps *mongo.Collection

// Initialize the database assuming the informations are in the
// config file
func Init() {
	// Every informations should be retreviewed from the config file to avoid
	// having secrets inside the actual code
	connectionString := config.Cfg.Database.ConnectionString

	clientOptions := options.Client().ApplyURI(connectionString)
	client, err := mongo.Connect(MongoContext, clientOptions)

	if err != nil {
		log.Fatal(err)
	}

	// It's better to ping the database to make sure we can connect
	// to it
	err = client.Ping(MongoContext, nil)

	if err != nil {
		log.Fatal(err)
	}

	// We affect the collections to access them later in the code
	databaseName := config.Cfg.Database.Name
	usersCollectionName := config.Cfg.Database.Collections.Users
	buildersCollectionName := config.Cfg.Database.Collections.Builders
	coachsColletionName := config.Cfg.Database.Collections.Coachs
	projectsColletionName := config.Cfg.Database.Collections.Projects
	buildOnsCollectionName := config.Cfg.Database.Collections.BuildOns
	buildOnStepsCollectionName := config.Cfg.Database.Collections.BuildOnSteps

	CollectionUsers = client.Database(databaseName).Collection(usersCollectionName)
	CollectionBuilders = client.Database(databaseName).Collection(buildersCollectionName)
	CollectionCoachs = client.Database(databaseName).Collection(coachsColletionName)
	CollectionProjects = client.Database(databaseName).Collection(projectsColletionName)
	CollectionBuildOns = client.Database(databaseName).Collection(buildOnsCollectionName)
	CollectionBuildOnSteps = client.Database(databaseName).Collection(buildOnStepsCollectionName)
}
