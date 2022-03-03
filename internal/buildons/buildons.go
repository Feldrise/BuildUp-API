package buildons

import (
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

type BuildOn struct {
	ID          primitive.ObjectID `bson:"_id"`
	Name        string             `bson:"name"`
	Description string             `bson:"description"`
	Index       int                `bson:"index"`
	AnnexeURL   string             `bson:"annexeUrl"`
	Rewards     string             `bson:"rewards"`
}

func (buildon *BuildOn) ToModel() *model.BuildOn {
	return &model.BuildOn{
		ID:          buildon.ID.Hex(),
		Name:        buildon.Name,
		Description: buildon.Description,
		Index:       buildon.Index,
		AnnexeURL:   buildon.AnnexeURL,
		Rewards:     buildon.Rewards,
	}
}

// Creation operation

func Create(input model.NewBuildOn) (*BuildOn, error) {
	databaseBuildOn := BuildOn{
		ID:          primitive.NewObjectID(),
		Name:        input.Name,
		Description: input.Description,
		Index:       input.Index,
		AnnexeURL:   input.AnnexeURL,
		Rewards:     input.Rewards,
	}

	_, err := database.CollectionBuildOns.InsertOne(database.MongoContext, databaseBuildOn)

	if err != nil {
		return nil, err
	}

	return &databaseBuildOn, nil
}

// Getters
func GetById(id string) (*BuildOn, error) {
	objectId, err := primitive.ObjectIDFromHex(id)

	if err != nil {
		return nil, err
	}

	filter := bson.D{
		primitive.E{
			Key:   "_id",
			Value: objectId,
		},
	}

	buildOns, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(buildOns) == 0 {
		return nil, nil
	}

	return &buildOns[0], nil
}

func GetFiltered(filter interface{}) ([]BuildOn, error) {
	buildOns := []BuildOn{}

	cursor, err := database.CollectionBuildOns.Find(database.MongoContext, filter)

	if err != nil {
		return buildOns, err
	}

	for cursor.Next(database.MongoContext) {
		var buildOn BuildOn

		err := cursor.Decode(&buildOn)

		if err != nil {
			return buildOns, err
		}

		buildOns = append(buildOns, buildOn)
	}

	if err := cursor.Err(); err != nil {
		return buildOns, err
	}

	return buildOns, nil
}
