package buildonsteps

import (
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

type BuildOnStep struct {
	ID               primitive.ObjectID `bson:"_id"`
	BuildOnID        primitive.ObjectID `bson:"buildonID"`
	Name             string             `bson:"name"`
	Description      string             `bson:"description"`
	Index            int                `bson:"index"`
	ProofType        string             `bson:"proofType"`
	ProofDescription string             `bson:"proofDescription"`
}

func (buildonStep *BuildOnStep) ToModel() *model.BuildOnStep {
	return &model.BuildOnStep{
		ID:               buildonStep.ID.Hex(),
		Name:             buildonStep.Name,
		Description:      buildonStep.Description,
		Index:            buildonStep.Index,
		ProofType:        buildonStep.ProofType,
		ProofDescription: buildonStep.ProofDescription,
	}
}

// Creation operation

func Create(buildOnID string, input model.NewBuildOnStep) (*BuildOnStep, error) {
	buildOnObjectID, err := primitive.ObjectIDFromHex(buildOnID)

	if err != nil {
		return nil, err
	}

	databaseBuildOnStep := BuildOnStep{
		ID:               primitive.NewObjectID(),
		BuildOnID:        buildOnObjectID,
		Name:             input.Name,
		Description:      input.Description,
		Index:            input.Index,
		ProofType:        input.ProofType,
		ProofDescription: input.ProofDescription,
	}

	_, err = database.CollectionBuildOnSteps.InsertOne(database.MongoContext, databaseBuildOnStep)

	if err != nil {
		return nil, err
	}

	return &databaseBuildOnStep, nil
}

// Update operation

func Update(changes *BuildOnStep) error {
	filter := bson.D{
		primitive.E{
			Key:   "_id",
			Value: changes.ID,
		},
	}

	_, err := database.CollectionBuildOnSteps.ReplaceOne(database.MongoContext, filter, changes)

	return err
}

// Getters

func GetById(id string) (*BuildOnStep, error) {
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

	buildOnSteps, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(buildOnSteps) == 0 {
		return nil, nil
	}

	return &buildOnSteps[0], nil
}

func GetForBuildOn(buildOnID string) ([]BuildOnStep, error) {
	buildObjectId, err := primitive.ObjectIDFromHex(buildOnID)

	if err != nil {
		return nil, err
	}

	filter := bson.D{
		primitive.E{
			Key:   "buildonID",
			Value: buildObjectId,
		},
	}

	return GetFiltered(filter)
}

func GetFiltered(filter interface{}) ([]BuildOnStep, error) {
	buildOnSteps := []BuildOnStep{}

	cursor, err := database.CollectionBuildOnSteps.Find(database.MongoContext, filter)

	if err != nil {
		return buildOnSteps, err
	}

	for cursor.Next(database.MongoContext) {
		var buildOnStep BuildOnStep

		err := cursor.Decode(&buildOnStep)

		if err != nil {
			return buildOnSteps, err
		}

		buildOnSteps = append(buildOnSteps, buildOnStep)
	}

	if err := cursor.Err(); err != nil {
		return buildOnSteps, err
	}

	return buildOnSteps, nil
}
